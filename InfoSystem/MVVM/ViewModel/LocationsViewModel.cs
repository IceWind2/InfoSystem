using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class LocationsViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<Location> _locations;
        private ICollectionView? _locationsView;
        public ObservableCollection<Location> Locations
        {
            get
            {
                _locationsView = CollectionViewSource.GetDefaultView(_locations);
                _locationsView.Filter = (x) => (x.ContainsFilter(_filter));
                return _locations;
            }
        }

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public LocationsViewModel(Window mainWindow)
        {
            using var context = InfoContext.CreateDbContext();
            _locations = new ObservableCollection<Location>(context.GetAllEntities<Location>());

            SearchCommand = new RelayCommand(o =>
            {
                _filter = (string)Application.Current.Properties["SearchBoxFilter"]!;
                _locationsView!.Refresh();
            });

            RefreshCommand = new RelayCommand(o =>
            {
                ((MainViewModel)mainWindow.DataContext).UpdateView();
            });

            AddCommand = new AsyncRelayCommand(async o =>
            {
                mainWindow.Opacity = 0.4;
                var newLocationModal = new NewLocationModal(mainWindow);
                newLocationModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (!newLocationModal.Success)
                {
                    MessageBox.Show($"При добавлении адреса произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                context.AddEntity(newLocationModal.Result!);
                await context.CommitAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Locations.Add(newLocationModal.Result!);
                });
            });

            EditCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedLocation == null)
                {
                    return;
                }

                var currentSelectedLocation = SelectedLocation;
                mainWindow.Opacity = 0.4;
                var locationModal = new NewLocationModal(mainWindow);
                locationModal.SetData(currentSelectedLocation);
                locationModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (locationModal.Cancelled)
                {
                    return;
                }

                if (!locationModal.Success)
                {
                    MessageBox.Show($"При обновлении адреса произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                locationModal.Result!.Id = currentSelectedLocation.Id;
                context.UpdateEntity(locationModal.Result!);
                await context.CommitAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Locations[Locations.IndexOf(currentSelectedLocation)] = locationModal.Result!;
                });
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedLocation == null)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                var patientsWithLocation = await context.Patients
                    .Where(p => p.LocationId == SelectedLocation.Id)
                    .ToListAsync();

                if (patientsWithLocation.Count > 0)
                {
                    var response = MessageBox.Show("Данный адрес привязан к одному или нескольким пациентам, вы уверены, что хотите его удалить?", "Подтвердите действие", MessageBoxButton.YesNo);
                    if (response == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                await context.BeginTransactionAsync();
                context.RemoveEntity(SelectedLocation);
                await context.SaveChangesAsync();

                foreach (var patient in patientsWithLocation)
                {
                    context.PatientsQueries.WritePatientHistory(patient.Id, string.Empty, "Адрес удален из системы");
                }

                await context.CommitAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Locations.Remove(SelectedLocation);
                });
            });
        }

        public void UpdateData()
        {
            using var context = InfoContext.CreateDbContext();
            _locations = new ObservableCollection<Location>(context.GetAllEntities<Location>());
        }

        private Location? _selectedLocation;
        public Location? SelectedLocation
        {
            get { return _selectedLocation; }
            set
            {
                _selectedLocation = value;
                OnPropertyChanged();
            }
        }
    }
}
