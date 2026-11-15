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
        private readonly IDialogService _dialogService = new DialogService();
        private Location? _selectedLocation;

        public ObservableCollection<Location> Locations { get; private set; }

        public Location? SelectedLocation
        {
            get => _selectedLocation;
            set => SetProperty(ref _selectedLocation, value);
        }

        public override void ApplySort()
        {
            _collectionView.SortDescriptions.Add(new SortDescription(nameof(Location.Name), ListSortDirection.Ascending));
        }

        public LocationsViewModel(Window mainWindow)
        {
            using (var context = InfoContext.CreateDbContext())
            {
                Locations = new ObservableCollection<Location>(context.GetAllEntities<Location>());
            }

            _collectionView = CollectionViewSource.GetDefaultView(Locations);
            _collectionView.Filter = item => item is Location location && location.ContainsFilter(SearchText);
            RefreshCollectionView();

            SearchCommand = new RelayCommand(_ => RefreshCollectionView());

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
                    Locations.Add(viewModel.Result!);
                    RefreshCollectionView();
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
                    RefreshCollectionView();
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
                    Locations.Remove(locationToRemove);
                    RefreshCollectionView();
                });
            });
        }
    }
}
