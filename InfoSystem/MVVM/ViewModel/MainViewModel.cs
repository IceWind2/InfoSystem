using System.Windows;

namespace InfoSystem
{
    internal class MainViewModel : ObservableObject
    {
        private object _currentView;

        public object CurrentView
        {
            get
            {
                return _currentView;
            }

            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public void UpdateView()
        {
            if (CurrentView is PatientsViewModel)
            {
                PatientsVM = new PatientsViewModel(_mainWindow);
                CurrentView = PatientsVM;
            }
            if (CurrentView is MedicineViewModel)
            {
                MedicineVM = new MedicineViewModel(_mainWindow);
                CurrentView = MedicineVM;
            }
            if (CurrentView is DiagnosisViewModel)
            {
                DiagnosisVM = new DiagnosisViewModel(_mainWindow);
                CurrentView = DiagnosisVM;
            }
            if (CurrentView is LocationsViewModel)
            {
                LocationsVM = new LocationsViewModel(_mainWindow);
                CurrentView = LocationsVM;
            }
        }

        public RelayCommand PatientsViewCommand {  get; set; }
        public RelayCommand MedicineViewCommand {  get; set; }
        public RelayCommand DiagnosisViewCommand {  get; set; }
        public RelayCommand LocationsViewCommand {  get; set; }

        public PatientsViewModel PatientsVM { get; set; }
        public MedicineViewModel MedicineVM { get; set; }
        public DiagnosisViewModel DiagnosisVM { get; set; }
        public LocationsViewModel LocationsVM { get; set; }

        private Window _mainWindow;

        public MainViewModel(Window mainWindow)
        {
            _mainWindow = mainWindow;
            PatientsVM = new PatientsViewModel(mainWindow);
            MedicineVM = new MedicineViewModel(mainWindow);
            DiagnosisVM = new DiagnosisViewModel(mainWindow);
            LocationsVM = new LocationsViewModel(mainWindow);

            PatientsViewCommand = new RelayCommand(o =>
            {
                PatientsVM.UpdateData();
                CurrentView = PatientsVM;
            });
            MedicineViewCommand = new RelayCommand(o =>
            {
                MedicineVM.UpdateData();
                CurrentView = MedicineVM;
            });
            DiagnosisViewCommand = new RelayCommand(o =>
            {
                MedicineVM.UpdateData();
                CurrentView = DiagnosisVM;
            });
            LocationsViewCommand = new RelayCommand(o =>
            {
                MedicineVM.UpdateData();
                CurrentView = LocationsVM;
            });

            CurrentView = PatientsVM;
        }
    }
}
