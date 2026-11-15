using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class DiagnosisViewModel : TablePageViewModel
    {
        private readonly IDialogService _dialogService = new DialogService();
        private Diagnosis? _selectedDiagnosis;

        public ObservableCollection<Diagnosis> Diagnoses { get; private set; }

        public Diagnosis? SelectedDiagnosis
        {
            get => _selectedDiagnosis;
            set => SetProperty(ref _selectedDiagnosis, value);
        }

        public override void ApplySort()
        {
            _collectionView.SortDescriptions.Add(new SortDescription(nameof(Diagnosis.Name), ListSortDirection.Ascending));
        }

        public DiagnosisViewModel(Window mainWindow)
        {
            using (var context = InfoContext.CreateDbContext())
            {
                Diagnoses = new ObservableCollection<Diagnosis>(context.GetAllEntities<Diagnosis>());
            }

            _collectionView = CollectionViewSource.GetDefaultView(Diagnoses);
            _collectionView.Filter = item => item is Diagnosis diagnosis && diagnosis.ContainsFilter(SearchText);
            RefreshCollectionView();

            SearchCommand = new RelayCommand(_ => RefreshCollectionView());

            AddCommand = new AsyncRelayCommand(async _ =>
            {
                var viewModel = new NewDiagnosisViewModel();
                var modalResult = _dialogService.ShowNewDiagnosis(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show("При добавлении диагноза произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                if (!modalResult.Value)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                context.AddEntity(viewModel.Result!);
                await context.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Diagnoses.Add(viewModel.Result!);
                    RefreshCollectionView();
                });
            });

            EditCommand = new AsyncRelayCommand(async _ =>
            {
                if (SelectedDiagnosis == null)
                {
                    return;
                }

                var diagnosisToEdit = SelectedDiagnosis;
                var viewModel = new NewDiagnosisViewModel(diagnosisToEdit);
                var modalResult = _dialogService.ShowNewDiagnosis(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show("При обновлении диагноза произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                if (!modalResult.Value)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                context.UpdateEntity(diagnosisToEdit);
                await context.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    RefreshCollectionView();
                });
            });

            RemoveCommand = new AsyncRelayCommand(async _ =>
            {
                if (SelectedDiagnosis == null)
                {
                    return;
                }

                var diagnosisToRemove = SelectedDiagnosis;
                using var context = InfoContext.CreateDbContext();
                var patientsWithDiagnosis = await context.Patients
                    .Where(patient => patient.DiagnosisId == diagnosisToRemove.Id)
                    .ToListAsync();

                if (patientsWithDiagnosis.Count > 0)
                {
                    var response = MessageBox.Show("Данный диагноз привязан к одному или нескольким пациентам, вы уверены, что хотите его удалить?", "Подтвердите действие", MessageBoxButton.YesNo);
                    if (response == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                foreach (var patient in patientsWithDiagnosis)
                {
                    context.PatientsQueries.WritePatientHistory(patient.Id, string.Empty, "Диагноз удален из системы");
                }

                context.RemoveEntity(diagnosisToRemove);
                await context.CommitAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SelectedDiagnosis = null;
                    Diagnoses.Remove(diagnosisToRemove);
                    RefreshCollectionView();
                });
            });
        }
    }
}
