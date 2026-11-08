using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class PatientsViewModel : TablePageViewModel
    {
        private readonly ObservableCollection<Patient> _patients;
        private readonly ICollectionView _patientsView;
        private Patient? _selectedPatient;
        private readonly IDialogService _dialogService = new DialogService();

        public ObservableCollection<Patient> Patients => _patients;

        public Patient? SelectedPatient
        {
            get => _selectedPatient;
            set => SetProperty(ref _selectedPatient, value);
        }

        public RelayCommand HistoryCommand { get; }

        public PatientsViewModel(Window mainWindow)
        {
            using (var context = InfoContext.CreateDbContext())
            {
                _patients = new ObservableCollection<Patient>(context.PatientsQueries.GetPatientsListAsync().GetAwaiter().GetResult());
            }

            _patientsView = CollectionViewSource.GetDefaultView(_patients);
            _patientsView.Filter = item => item is Patient patient && patient.ContainsFilter(SearchText);
            UpdateTotalCount();

            FilterCommand = new RelayCommand(_ =>
            {
                _patientsView.Refresh();
                UpdateTotalCount();
            });

            HistoryCommand = new RelayCommand(item =>
            {
                if (item is Patient patient)
                {
                    var viewModel = new HistoryViewModel(patient);
                    _dialogService.ShowHistory(mainWindow, viewModel);
                }
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
                    _patients.Add(newPatient);
                    _patientsView.Refresh();
                    UpdateTotalCount();
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
                    var index = _patients.IndexOf(currentSelectedPatient);
                    if (index >= 0)
                    {
                        _patients[index] = currentSelectedPatient;
                        _patientsView.Refresh();
                        UpdateTotalCount();
                    }
                });
            });

            RemoveCommand = new AsyncRelayCommand(async _ =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                var response = MessageBox.Show("Вы уверены, что хотите удалить пациента?", "Подтвердите действие", MessageBoxButton.YesNo);
                if (response == MessageBoxResult.No)
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
                    _patients.Remove(patientToRemove);
                    _patientsView.Refresh();
                    UpdateTotalCount();
                });
            });
        }

        private void UpdateTotalCount()
        {
            TotalCount = _patientsView.Cast<object>().Count();
            OnPropertyChanged(nameof(TotalCount));
        }
    }
}
