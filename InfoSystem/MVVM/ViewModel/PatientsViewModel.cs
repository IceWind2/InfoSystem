using System;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace InfoSystem
{
    internal class PatientsViewModel : TablePageViewModel
    {
        private ObservableCollection<Patient> _patients;
        private ICollectionView _patientsView;
        private Patient selectedPatient;

        public ObservableCollection<Patient> Patients
        {
            get
            {
                _patientsView = CollectionViewSource.GetDefaultView(_patients);
                _patientsView.Filter = (x) => (x.ContainsFilter(SearchText));
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

        private readonly IDialogService _dialogService;

        public PatientsViewModel(Window mainWindow)
        {
            _dialogService = new DialogService();

            using var context = InfoContext.CreateDbContext();
            _patients = new ObservableCollection<Patient>(context.PatientsQueries.GetPatientsListAsync().GetAwaiter().GetResult());

            //HistoryCommand = new RelayCommand(o =>
            //{
            //    if (o is Patient patient)
            //    {
            //        mainWindow.Opacity = 0.4;
            //        var newPatientModal = new HistoryModal(mainWindow, patient.Id);
            //        newPatientModal.ShowDialog();
            //        mainWindow.Opacity = 1;
            //    }
            //});

            //AddMedicineCommand = new AsyncRelayCommand(async o =>
            //{
            //    if (o is Patient patient)
            //    {
            //        mainWindow.Opacity = 0.4;
            //        var addMedicineModal = new PatientMedicineModal(mainWindow, patient, true);
            //        addMedicineModal.ShowDialog();
            //        mainWindow.Opacity = 1;

            //        if (!addMedicineModal.Success)
            //        {
            //            MessageBox.Show($"При добавлении лекарства к пациенту произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
            //            return;
            //        }

            //        using var context = await InfoContext.CreateDbContextWithTransactionAsync();
            //        context.PatientsQueries.AddPatientMedicine(addMedicineModal.Result!);
            //        await context.CommitAsync();

            //        ((MainViewModel)mainWindow.DataContext).UpdateView();
            //    }
            //});

            //RemoveMedicineCommand = new AsyncRelayCommand(async o =>
            //{
            //    if (o is Patient patient)
            //    {
            //        mainWindow.Opacity = 0.4;
            //        var removeMedicineModal = new PatientMedicineModal(mainWindow, patient, false);
            //        removeMedicineModal.ShowDialog();
            //        mainWindow.Opacity = 1;

            //        if (!removeMedicineModal.Success)
            //        {
            //            MessageBox.Show($"При удалении лекарства у пациента произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
            //            return;
            //        }

            //        using var context = await InfoContext.CreateDbContextWithTransactionAsync();
            //        context.PatientsQueries.RemovePatientMedicine(removeMedicineModal.Result!);
            //        await context.CommitAsync();

            //        ((MainViewModel)mainWindow.DataContext).UpdateView();
            //    }
            //});

            FilterCommand = new RelayCommand(o =>
            {
                _patientsView!.Refresh();
                TotalCount = ((CollectionView)_patientsView).Count;
            });

            RefreshCommand = new RelayCommand(o =>
            {
                ((MainViewModel)mainWindow.DataContext).UpdateView();
            });

            AddCommand = new AsyncRelayCommand(async o =>
            {
                using var context = await InfoContext.CreateDbContextWithTransactionAsync();

                var viewModel = new NewPatientViewModel(context.GetAllEntities<Location>());
                bool? modalResult = _dialogService.ShowNewPatient(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show($"При добавлении пациента произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                if (!modalResult.Value)
                {
                    return;
                }

                var newPatient = context.PatientsQueries.AddPatient(viewModel.Result!);
                await context.SaveChangesAsync();
                context.PatientsQueries.WritePatientHistory(newPatient.Id, string.Empty, HistoryChange.NewPatient);
                await context.CommitAsync();

                await context.Entry(newPatient).Collection(p => p.PatientMedicine!).LoadAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Patients.Add(newPatient);
                });
            });

            //EditCommand = new AsyncRelayCommand(async o =>
            //{
            //    if (SelectedPatient == null)
            //    {
            //        return;
            //    }

            //    var currentSelectedPatient = SelectedPatient;
            //    mainWindow.Opacity = 0.4;
            //    var patientModal = new NewPatientModal(mainWindow);
            //    patientModal.SetData(currentSelectedPatient);
            //    patientModal.ShowDialog();
            //    mainWindow.Opacity = 1;

            //    if (patientModal.Cancelled)
            //    {
            //        return;
            //    }

            //    if (!patientModal.Success)
            //    {
            //        MessageBox.Show($"При обновлении пациента произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
            //        return;
            //    }

            //    using var context = await InfoContext.CreateDbContextWithTransactionAsync();
            //    patientModal.Result!.Id = currentSelectedPatient.Id;
            //    var updatedPatient = await context.PatientsQueries.UpdatePatientAsync(patientModal.Result!);
            //    context.PatientsQueries.WritePatientHistory(updatedPatient.Id, "", HistoryChange.NewPatient); // note???
            //    await context.CommitAsync();

            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        Patients[Patients.IndexOf(currentSelectedPatient)] = updatedPatient;
            //    });
            //});

            //RemoveCommand = new AsyncRelayCommand(async o =>
            //{
            //    if (SelectedPatient == null)
            //    {
            //        return;
            //    }

            //    using var context = await InfoContext.CreateDbContextWithTransactionAsync();
            //    context.RemoveEntity(SelectedPatient);
            //    await context.CommitAsync();

            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        Patients.Remove(SelectedPatient);
            //    });
            //});
        }

        public void UpdateData()
        {
            using var context = InfoContext.CreateDbContext();
            _patients = new ObservableCollection<Patient>(context.PatientsQueries.GetPatientsListAsync().GetAwaiter().GetResult());
        }
    }
}