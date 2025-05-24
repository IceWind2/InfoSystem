using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class ArchiveViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<ArchivedPatient> _patients;
        private ICollectionView _patientsView;

        private ArchivedPatient selectedPatient;

        public ObservableCollection<ArchivedPatient> Patients
        {
            get
            {
                _patientsView = CollectionViewSource.GetDefaultView(_patients);
                _patientsView.Filter = (x) => (x.ContainsFilter(_filter));
                return _patients;
            }
        }

        public ArchivedPatient SelectedPatient
        {
            get { return selectedPatient; }
            set
            {
                selectedPatient = value;
                OnPropertyChanged();
            }
        }

        // Context menu commands
        public RelayCommand HistoryCommand { get; set; }

        // Generic commands
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public ArchiveViewModel(Window mainWindow)
        {
            _patients = new ObservableCollection<ArchivedPatient>(DatabaseManager.GetAllArchivedPatients());

            HistoryCommand = new RelayCommand(o =>
            {
                if (o is ArchivedPatient archivedPatient)
                {
                    mainWindow.Opacity = 0.4;
                    var newPatientModal = new HistoryModal(mainWindow, archivedPatient.PatientId);
                    newPatientModal.ShowDialog();
                    mainWindow.Opacity = 1;
                }
            });


            SearchCommand = new RelayCommand(o =>
            {
                _filter = (string)Application.Current.Properties["SearchBoxFilter"]!;
                _patientsView!.Refresh();
            });

            RefreshCommand = new RelayCommand(o =>
            {
                ((MainViewModel)mainWindow.DataContext).UpdateView();
            });
        }

        public void UpdateData()
        {
            using var context = new InfoContext();
            _patients = new ObservableCollection<ArchivedPatient>(DatabaseManager.GetAllArchivedPatients());
        }
    }
}
