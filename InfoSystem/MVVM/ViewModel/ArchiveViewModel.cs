using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class ArchiveViewModel : TablePageViewModel
    {
        private readonly ObservableCollection<ArchivedPatient> _patients;
        private readonly ICollectionView _patientsView;
        private readonly IDialogService _dialogService = new DialogService();
        private ArchivedPatient? _selectedPatient;

        public ObservableCollection<ArchivedPatient> Patients => _patients;

        public ArchivedPatient? SelectedPatient
        {
            get => _selectedPatient;
            set => SetProperty(ref _selectedPatient, value);
        }

        public RelayCommand HistoryCommand { get; }

        public ArchiveViewModel(Window mainWindow)
        {
            using (var context = InfoContext.CreateDbContext())
            {
                _patients = new ObservableCollection<ArchivedPatient>(
                    context.PatientsQueries.GetArchivedPatientsListAsync().GetAwaiter().GetResult());
            }

            _patientsView = CollectionViewSource.GetDefaultView(_patients);
            _patientsView.Filter = item => item is ArchivedPatient patient && patient.ContainsFilter(SearchText);
            UpdateTotalCount();

            CanAdd = false;
            CanEdit = false;
            CanDelete = false;

            FilterCommand = new RelayCommand(_ =>
            {
                _patientsView.Refresh();
                UpdateTotalCount();
            });

            HistoryCommand = new RelayCommand(item =>
            {
                if (item is ArchivedPatient archivedPatient)
                {
                    var viewModel = new HistoryViewModel(archivedPatient);
                    _dialogService.ShowHistory(mainWindow, viewModel);
                }
            });
        }

        private void UpdateTotalCount()
        {
            TotalCount = _patientsView.Cast<object>().Count();
            OnPropertyChanged(nameof(TotalCount));
        }
    }
}
