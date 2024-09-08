using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class LocationsViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<Location> _locations;
        private ICollectionView _locationsView;
        public ObservableCollection<Location> Locations
        {
            get
            {
                _locationsView = CollectionViewSource.GetDefaultView(_locations);
                _locationsView.Filter = (x) => ((Location)x).Name.Contains(_filter);
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
            var context = new InfoContext();
            _locations = new ObservableCollection<Location>(context.Locations.AsNoTracking());

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

                if (newLocationModal.Success)
                {
                    await DatabaseManager.AddLocation(newLocationModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Locations.Add(newLocationModal.Result!);
                    });
                }
            });

            EditCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedLocation == null)
                {
                    return;
                }

                var currentSelectedLocation = SelectedLocation;
                mainWindow.Opacity = 0.4;
                var newLocationModal = new NewLocationModal(mainWindow);
                newLocationModal.SetData(currentSelectedLocation);
                newLocationModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newLocationModal.Success)
                {
                    newLocationModal.Result!.Id = currentSelectedLocation.Id;
                    await DatabaseManager.UpdateLocation(newLocationModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Locations[Locations.IndexOf(currentSelectedLocation)] = newLocationModal.Result!;
                    });
                }
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedLocation == null)
                {
                    return;
                }

                if (!await DatabaseManager.TryRemoveLocation(SelectedLocation))
                {
                    MessageBox.Show("Невозможно удалить адрес, так как он еще используется", "Ошибка удаления", MessageBoxButton.OK);
                    return;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Locations.Remove(SelectedLocation);
                });
            });
        }

        public void UpdateData()
        {
            var context = new InfoContext();
            _locations = new ObservableCollection<Location>(context.Locations.AsNoTracking());
        }

        private Location _selectedLocation;
        public Location SelectedLocation
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
