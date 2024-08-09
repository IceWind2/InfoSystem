﻿using System.Windows;

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

        public RelayCommand PatientsViewCommand {  get; set; }
        public RelayCommand MedicineViewCommand {  get; set; }

        public PatientsViewModel PatientsVM { get; set; }
        public MedicineViewModel MedicineVM { get; set; }

        public MainViewModel(Window mainWindow)
        {
            PatientsVM = new PatientsViewModel(mainWindow);
            MedicineVM = new MedicineViewModel(mainWindow);

            PatientsViewCommand = new RelayCommand(o =>
            {
                CurrentView = PatientsVM;
            });
            MedicineViewCommand = new RelayCommand(o =>
            {
                CurrentView = MedicineVM;
            });

            CurrentView = PatientsVM;
        }
    }
}
