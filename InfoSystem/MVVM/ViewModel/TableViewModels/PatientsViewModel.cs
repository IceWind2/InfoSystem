using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class PatientsViewModel : TablePageViewModel
    {
        private Patient? _selectedPatient;
        private Medicine? _filterMedicine;
        private Diagnosis? _filterDiagnosis;
        private Location? _filterLocation;
        private readonly IDialogService _dialogService = new DialogService();

        public ObservableCollection<Patient> Patients { get; private set; }
        public IReadOnlyList<Medicine> Medicines { get; }
        public IReadOnlyList<Diagnosis> Diagnoses { get; }
        public IReadOnlyList<Location> Locations { get; }
        public bool HasSelectedFilters =>
            FilterMedicine != null || FilterDiagnosis != null || FilterLocation != null;

        public Patient? SelectedPatient
        {
            get => _selectedPatient;
            set => SetProperty(ref _selectedPatient, value);
        }

        public Medicine? FilterMedicine
        {
            get => _filterMedicine;
            set
            {
                if (SetProperty(ref _filterMedicine, value))
                {
                    OnPropertyChanged(nameof(HasSelectedFilters));
                    RefreshCollectionView();
                }
            }
        }

        public Diagnosis? FilterDiagnosis
        {
            get => _filterDiagnosis;
            set
            {
                if (SetProperty(ref _filterDiagnosis, value))
                {
                    OnPropertyChanged(nameof(HasSelectedFilters));
                    RefreshCollectionView();
                }
            }
        }

        public Location? FilterLocation
        {
            get => _filterLocation;
            set
            {
                if (SetProperty(ref _filterLocation, value))
                {
                    OnPropertyChanged(nameof(HasSelectedFilters));
                    RefreshCollectionView();
                }
            }
        }

        public RelayCommand HistoryCommand { get; }
        public RelayCommand ClearFiltersCommand { get; }
        public AsyncRelayCommand AddMedicineCommand { get; }
        public AsyncRelayCommand RemoveMedicineCommand { get; }

        public override void ApplySort()
        {
            _collectionView.SortDescriptions.Add(new SortDescription(nameof(Patient.Name), ListSortDirection.Ascending));
        }

        public PatientsViewModel(Window mainWindow)
        {
            HasAdvancedFilters = true;

            using (var context = InfoContext.CreateDbContext())
            {
                Patients = new ObservableCollection<Patient>(context.PatientsQueries.GetPatientsListAsync().GetAwaiter().GetResult());
                Medicines = context.GetAllEntities<Medicine>().OrderBy(medicine => medicine.Name).ToList();
                Diagnoses = context.GetAllEntities<Diagnosis>().OrderBy(diagnosis => diagnosis.Name).ToList();
                Locations = context.GetAllEntities<Location>().OrderBy(location => location.Name).ToList();
            }

            _collectionView = CollectionViewSource.GetDefaultView(Patients);
            _collectionView.Filter = item => item is Patient patient
                && patient.ContainsFilter(SearchText)
                && (_filterMedicine == null || patient.PatientMedicine?.Any(
                    patientMedicine => patientMedicine.MedicineId == _filterMedicine.Id) == true)
                && (_filterDiagnosis == null || patient.DiagnosisId == _filterDiagnosis.Id)
                && (_filterLocation == null || patient.LocationId == _filterLocation.Id);

            RefreshCollectionView();

            SearchCommand = new RelayCommand(_ => RefreshCollectionView());
            ClearFiltersCommand = new RelayCommand(_ =>
            {
                FilterMedicine = null;
                FilterDiagnosis = null;
                FilterLocation = null;
            });

            HistoryCommand = new RelayCommand(item =>
            {
                if (item is Patient patient)
                {
                    var viewModel = new HistoryViewModel(patient);
                    _dialogService.ShowHistory(mainWindow, viewModel);
                }
            });

            AddMedicineCommand = new AsyncRelayCommand(async item =>
            {
                if (item is not Patient patient)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                var viewModel = new PatientMedicineViewModel(
                    patient,
                    context.GetAllEntities<Medicine>());
                var modalResult = _dialogService.ShowPatientMedicine(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show("При добавлении лекарства пациенту произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                if (!modalResult.Value)
                {
                    return;
                }

                context.PatientsQueries.AddPatientMedicine(viewModel.Result!);
                context.PatientsQueries.WritePatientHistory(
                    patient.Id,
                    viewModel.Result!.Note ?? string.Empty,
                    $"{HistoryChange.MedicineAdded}: {viewModel.Result.Medicine!.Name}");

                await context.SaveChangesAsync();

                await RefreshPatientAsync(patient.Id, context);
            });

            RemoveMedicineCommand = new AsyncRelayCommand(async item =>
            {
                if (item is not PatientMedicine patientMedicine)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();

                context.PatientsQueries.RemovePatientMedicine(new AddPatientMedicineDTO
                {
                    PatientId = patientMedicine.PatientId,
                    MedicineId = patientMedicine.MedicineId,
                    Prescription = patientMedicine.Prescription,
                    Note = null
                });

                context.PatientsQueries.WritePatientHistory(
                    patientMedicine.PatientId,
                    string.Empty,
                    $"{HistoryChange.MedicineRemoved}: {patientMedicine.Medicine?.Name}");

                await context.SaveChangesAsync();

                await RefreshPatientAsync(patientMedicine.PatientId, context);
            });

            AddCommand = new AsyncRelayCommand(async _ =>
            {
                using var context = await InfoContext.CreateDbContextWithTransactionAsync();

                var viewModel = new NewPatientViewModel(
                    context.GetAllEntities<Location>(),
                    context.GetAllEntities<Diagnosis>());
                bool? modalResult = _dialogService.ShowNewPatient(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show("При добавлении пациента произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
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

                newPatient.Location = viewModel.Result!.Location;
                newPatient.Diagnosis = viewModel.Result!.Diagnosis;
                await context.Entry(newPatient).Collection(patient => patient.PatientMedicine!).LoadAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Patients.Add(newPatient);
                    RefreshCollectionView();
                });
            });

            EditCommand = new AsyncRelayCommand(async _ =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();

                var currentSelectedPatient = SelectedPatient;
                var viewModel = new NewPatientViewModel(
                    context.GetAllEntities<Location>(),
                    context.GetAllEntities<Diagnosis>(),
                    currentSelectedPatient);
                bool? modalResult = _dialogService.ShowNewPatient(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show("При обновлении пациента произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                if (!modalResult.Value)
                {
                    return;
                }

                context.UpdateEntity(currentSelectedPatient);
                await context.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var index = Patients.IndexOf(currentSelectedPatient);
                    if (index >= 0)
                    {
                        Patients[index] = currentSelectedPatient;
                        RefreshCollectionView();
                    }
                });
            });

            RemoveCommand = new AsyncRelayCommand(async _ =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                var confirmation = MessageBox.Show("Вы уверены, что хотите удалить пациента?", "Подтвердите действие", MessageBoxButton.YesNo);
                if (confirmation == MessageBoxResult.No)
                {
                    return;
                }

                var patientToRemove = SelectedPatient;
                using var context = InfoContext.CreateDbContext();
                context.RemoveEntity(patientToRemove);
                await context.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SelectedPatient = null;
                    Patients.Remove(patientToRemove);
                    RefreshCollectionView();
                });
            });
        }

        private async Task RefreshPatientAsync(int patientId, InfoContext infoContext)
        {
            var updatedPatient = await infoContext.Patients
                .AsNoTracking()
                .Include(p => p.PatientMedicine!).ThenInclude(pm => pm.Medicine)
                .FirstAsync(p => p.Id == patientId);

            Application.Current.Dispatcher.Invoke(() =>
            {
                var currentPatient = Patients.FirstOrDefault(patient => patient.Id == patientId);
                if (currentPatient != null)
                {
                    currentPatient.PatientMedicine.Clear();
                    foreach (var patientMedicine in updatedPatient.PatientMedicine)
                    {
                        currentPatient.PatientMedicine.Add(patientMedicine);
                    }

                    _collectionView.Refresh();
                }
            });
        }
    }
}
