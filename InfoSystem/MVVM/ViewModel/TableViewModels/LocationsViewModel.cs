using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class LocationsViewModel : TablePageViewModel
    {
        private readonly ObservableCollection<Location> _locations;
        private readonly ICollectionView _locationsView;
        private readonly IDialogService _dialogService = new DialogService();
        private Location? _selectedLocation;

        public ObservableCollection<Location> Locations => _locations;

        public Location? SelectedLocation
        {
            get => _selectedLocation;
            set => SetProperty(ref _selectedLocation, value);
        }

        public LocationsViewModel(Window mainWindow)
        {
            using (var context = InfoContext.CreateDbContext())
            {
                _locations = new ObservableCollection<Location>(context.GetAllEntities<Location>());
            }

            _locationsView = CollectionViewSource.GetDefaultView(_locations);
            _locationsView.Filter = item => item is Location location && location.ContainsFilter(SearchText);
            UpdateTotalCount();

            FilterCommand = new RelayCommand(_ =>
            {
                _locationsView.Refresh();
                UpdateTotalCount();
            });

            AddCommand = new AsyncRelayCommand(async _ =>
            {
                var viewModel = new NewLocationViewModel();
                var modalResult = _dialogService.ShowNewLocation(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show("При добавлении адреса произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                if (!modalResult.Value)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                context.AddEntity(viewModel.Result!);
                await context.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _locations.Add(viewModel.Result!);
                    _locationsView.Refresh();
                    UpdateTotalCount();
                });
            });

            EditCommand = new AsyncRelayCommand(async _ =>
            {
                if (SelectedLocation == null)
                {
                    return;
                }

                var locationToEdit = SelectedLocation;
                var viewModel = new NewLocationViewModel(locationToEdit);
                var modalResult = _dialogService.ShowNewLocation(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show("При обновлении адреса произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                if (!modalResult.Value)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                context.UpdateEntity(locationToEdit);
                await context.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _locationsView.Refresh();
                    UpdateTotalCount();
                });
            });

            RemoveCommand = new AsyncRelayCommand(async _ =>
            {
                if (SelectedLocation == null)
                {
                    return;
                }

                var locationToRemove = SelectedLocation;
                using var context = InfoContext.CreateDbContext();
                var patientsWithLocation = await context.Patients
                    .Where(patient => patient.LocationId == locationToRemove.Id)
                    .ToListAsync();

                if (patientsWithLocation.Count > 0)
                {
                    var response = MessageBox.Show("Данный адрес привязан к одному или нескольким пациентам, вы уверены, что хотите его удалить?", "Подтвердите действие", MessageBoxButton.YesNo);
                    if (response == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                foreach (var patient in patientsWithLocation)
                {
                    context.PatientsQueries.WritePatientHistory(patient.Id, string.Empty, "Адрес удален из системы");
                }

                context.RemoveEntity(locationToRemove);
                await context.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SelectedLocation = null;
                    _locations.Remove(locationToRemove);
                    _locationsView.Refresh();
                    UpdateTotalCount();
                });
            });
        }

        public void UpdateData()
        {
            using var context = InfoContext.CreateDbContext();
            var locations = context.GetAllEntities<Location>();

            _locations.Clear();
            foreach (var location in locations)
            {
                _locations.Add(location);
            }

            _locationsView.Refresh();
            UpdateTotalCount();
        }

        private void UpdateTotalCount()
        {
            TotalCount = _locationsView.Cast<object>().Count();
            OnPropertyChanged(nameof(TotalCount));
        }
    }
}
