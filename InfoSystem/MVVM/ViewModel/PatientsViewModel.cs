using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace InfoSystem
{
    internal class PatientsViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<Patient> _patients;
        private ICollectionView _patientsView;
 
        private Patient selectedPatient;

        public ObservableCollection<Patient> Patients
        {
            get
            {
                _patientsView = CollectionViewSource.GetDefaultView(_patients);
                _patientsView.Filter = (x) => (x.ContainsFilter(_filter));
                return _patients;
            }
        }

        public Patient SelectedPatient
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
        public AsyncRelayCommand AddMedicineCommand {  get; set; }
        public AsyncRelayCommand RemoveMedicineCommand {  get; set; }

        // Generic commands
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public PatientsViewModel(Window mainWindow)
        {
            var context = new InfoContext();
            _patients = new ObservableCollection<Patient>(DatabaseManager.GetAllPatients());

            HistoryCommand = new RelayCommand(o =>
            {
                if (o is Patient patient)
                {
                    mainWindow.Opacity = 0.4;
                    var newPatientModal = new HistoryModal(mainWindow, patient.Id);
                    newPatientModal.ShowDialog();
                    mainWindow.Opacity = 1;
                }
            });

            AddMedicineCommand = new AsyncRelayCommand(async o =>
            {
                if (o is Patient patient)
                {
                    mainWindow.Opacity = 0.4;
                    var addMedicineModal = new PatientMedicineModal(mainWindow, patient, true);
                    addMedicineModal.ShowDialog();
                    mainWindow.Opacity = 1;

                    if (addMedicineModal.Success)
                    {
                        await DatabaseManager.AddPatientMedicine(addMedicineModal.Result!);
                        ((MainViewModel)mainWindow.DataContext).UpdateView();
                    }
                }
            });

            RemoveMedicineCommand = new AsyncRelayCommand(async o =>
            {
                if (o is Patient patient)
                {
                    mainWindow.Opacity = 0.4;
                    var addMedicineModal = new PatientMedicineModal(mainWindow, patient, false);
                    addMedicineModal.ShowDialog();
                    mainWindow.Opacity = 1;

                    if (addMedicineModal.Success)
                    {
                        await DatabaseManager.RemovePatientMedicine(addMedicineModal.Result!);
                        ((MainViewModel)mainWindow.DataContext).UpdateView();
                    }
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

            AddCommand = new AsyncRelayCommand(async o =>
            {
                mainWindow.Opacity = 0.4;
                var newPatientModal = new NewPatientModal(mainWindow);
                newPatientModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newPatientModal.Success)
                {
                    var newPatient = await DatabaseManager.AddPatient(newPatientModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Patients.Add(newPatient);
                    });
                }
            });

            EditCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                var currentSelectedPatient = SelectedPatient;
                mainWindow.Opacity = 0.4;
                var newPatientModal = new NewPatientModal(mainWindow);
                newPatientModal.SetData(currentSelectedPatient);
                newPatientModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newPatientModal.Success)
                {
                    newPatientModal.Result!.Id = currentSelectedPatient.Id;
                    var newPatient = await DatabaseManager.UpdatePatient(newPatientModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Patients[Patients.IndexOf(currentSelectedPatient)] = newPatient;
                    });
                }
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                await DatabaseManager.RemovePatient(SelectedPatient);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Patients.Remove(SelectedPatient);
                });
            });
        }

        public void UpdateData()
        {
            var context = new InfoContext();
            _patients = new ObservableCollection<Patient>(DatabaseManager.GetAllPatients());
        }
    }
}
