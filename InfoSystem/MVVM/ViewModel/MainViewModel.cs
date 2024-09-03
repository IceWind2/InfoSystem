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
        }

        public RelayCommand PatientsViewCommand {  get; set; }
        public RelayCommand MedicineViewCommand {  get; set; }

        public PatientsViewModel PatientsVM { get; set; }
        public MedicineViewModel MedicineVM { get; set; }

        private Window _mainWindow;

        public MainViewModel(Window mainWindow)
        {
            _mainWindow = mainWindow;
            PatientsVM = new PatientsViewModel(mainWindow);
            MedicineVM = new MedicineViewModel(mainWindow);

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

            CurrentView = PatientsVM;
        }
    }
}
