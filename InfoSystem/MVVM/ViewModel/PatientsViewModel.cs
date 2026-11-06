using System;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace InfoSystem
{
    internal class PatientsViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<Patient> _patients;
        private ICollectionView _patientsView;
        private Patient selectedPatient;

        public event Action<int> CountUpdated;
        
        public ObservableCollection<Patient> Patients
        {
            get
            {
                _patientsView = CollectionViewSource.GetDefaultView(_patients);
                _patientsView.Filter = (x) => (x.ContainsFilter(_filter));
                CountUpdated?.Invoke(((CollectionView)_patientsView).Count);
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
            using var context = InfoContext.CreateDbContext();
            _patients = new ObservableCollection<Patient>(context.PatientsQueries.GetPatientsListAsync().GetAwaiter().GetResult());
            
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

                    if (!addMedicineModal.Success)
                    {
                        MessageBox.Show($"При добавлении лекарства к пациенту произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                        return;
                    }

                    using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                    context.PatientsQueries.AddPatientMedicine(addMedicineModal.Result!);
                    await context.CommitAsync();

                    ((MainViewModel)mainWindow.DataContext).UpdateView();
                }
            });

            RemoveMedicineCommand = new AsyncRelayCommand(async o =>
            {
                if (o is Patient patient)
                {
                    mainWindow.Opacity = 0.4;
                    var removeMedicineModal = new PatientMedicineModal(mainWindow, patient, false);
                    removeMedicineModal.ShowDialog();
                    mainWindow.Opacity = 1;

                    if (!removeMedicineModal.Success)
                    {
                        MessageBox.Show($"При удалении лекарства у пациента произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                        return;
                    }

                    using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                    context.PatientsQueries.RemovePatientMedicine(removeMedicineModal.Result!);
                    await context.CommitAsync();

                    ((MainViewModel)mainWindow.DataContext).UpdateView();
                }
            });

            SearchCommand = new RelayCommand(o =>
            {
                _filter = (string)Application.Current.Properties["SearchBoxFilter"]!;
                _patientsView!.Refresh();
                CountUpdated?.Invoke(((CollectionView)_patientsView).Count);
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
                
                if (newPatientModal.Cancelled)
                {
                    return;
                }

                if (!newPatientModal.Success)
                {
                    MessageBox.Show($"При добавлении пациента произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                var newPatient = context.PatientsQueries.AddPatient(newPatientModal.Result!);
                await context.SaveChangesAsync();
                context.PatientsQueries.WritePatientHistory(newPatient.Id, string.Empty, HistoryChange.NewPatient);
                await context.CommitAsync();

                await context.Entry(newPatient).Collection(p => p.PatientMedicine).LoadAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Patients.Add(newPatient);
                });
            });

            EditCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                var currentSelectedPatient = SelectedPatient;
                mainWindow.Opacity = 0.4;
                var patientModal = new NewPatientModal(mainWindow);
                patientModal.SetData(currentSelectedPatient);
                patientModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (patientModal.Cancelled)
                {
                    return;
                }

                if (!patientModal.Success)
                {
                    MessageBox.Show($"При обновлении пациента произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                patientModal.Result!.Id = currentSelectedPatient.Id;
                var updatedPatient = await context.PatientsQueries.UpdatePatientAsync(patientModal.Result!);
                context.PatientsQueries.WritePatientHistory(updatedPatient.Id, "", HistoryChange.NewPatient); // note???
                await context.CommitAsync();
                    
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Patients[Patients.IndexOf(currentSelectedPatient)] = updatedPatient;
                });
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                context.RemoveEntity(SelectedPatient);
                await context.CommitAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Patients.Remove(SelectedPatient);
                });
            });
        }

        public void UpdateData()
        {
            using var context = InfoContext.CreateDbContext();
            _patients = new ObservableCollection<Patient>(context.PatientsQueries.GetPatientsListAsync().GetAwaiter().GetResult());
        }
    }
}