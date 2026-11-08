using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class DiagnosisViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<Diagnosis> _diagnoses;
        private ICollectionView? _diagnosesView;
        public ObservableCollection<Diagnosis> Diagnoses
        {
            get
            {
                _diagnosesView = CollectionViewSource.GetDefaultView(_diagnoses);
                _diagnosesView.Filter = (x) => x.ContainsFilter(_filter);
                return _diagnoses;
            }
        }

        public RelayCommand SearchCommand { get; set; }
        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public DiagnosisViewModel(Window mainWindow)
        {
            using var context = InfoContext.CreateDbContext();
            _diagnoses = new ObservableCollection<Diagnosis>(context.GetAllEntities<Diagnosis>());

            SearchCommand = new RelayCommand(o =>
            {
                _filter = (string)Application.Current.Properties["SearchBoxFilter"]!;
                _diagnosesView!.Refresh();
            });

            AddCommand = new AsyncRelayCommand(async o =>
            {
                mainWindow.Opacity = 0.4;
                var newDiagnosisModal = new NewDiagnosisModal(mainWindow);
                newDiagnosisModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (!newDiagnosisModal.Success)
                {
                    MessageBox.Show($"При добавлении диагноза произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                context.AddEntity(newDiagnosisModal.Result!);
                await context.CommitAsync();
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Diagnoses.Add(newDiagnosisModal.Result!);
                });
            });

            EditCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedDiagnosis == null)
                {
                    return;
                }

                var currentSelectedDiagnosis = SelectedDiagnosis;
                mainWindow.Opacity = 0.4;
                var newDiagnosisModal = new NewDiagnosisModal(mainWindow);
                newDiagnosisModal.SetData(currentSelectedDiagnosis);
                newDiagnosisModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newDiagnosisModal.Cancelled)
                {
                    return;
                }

                if (!newDiagnosisModal.Success)
                {
                    MessageBox.Show($"При обновлении диагноза произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                newDiagnosisModal.Result!.Id = currentSelectedDiagnosis.Id;
                context.UpdateEntity(newDiagnosisModal.Result!);
                await context.CommitAsync();
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Diagnoses[Diagnoses.IndexOf(currentSelectedDiagnosis)] = newDiagnosisModal.Result!;
                });
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedDiagnosis == null)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                var patientsWithDiagnosis = await context.Patients
                    .Where(p => p.DiagnosisId == SelectedDiagnosis.Id)
                    .ToListAsync();

                if (patientsWithDiagnosis.Count > 0)
                {
                    var response = MessageBox.Show("Данный диагноз привязан к одному или нескольким пациентам, вы уверены, что хотите его удалить?", "Подтвердите действие", MessageBoxButton.YesNo);
                    if (response == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                await context.BeginTransactionAsync();
                context.RemoveEntity(SelectedDiagnosis);
                await context.SaveChangesAsync();

                foreach (var patient in patientsWithDiagnosis)
                {
                    context.PatientsQueries.WritePatientHistory(patient.Id, string.Empty, "Диагноз удален из системы");
                }

                await context.CommitAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Diagnoses.Remove(SelectedDiagnosis);
                });
            });
        }

        public void UpdateData()
        {
            using var context = InfoContext.CreateDbContext();
            _diagnoses = new ObservableCollection<Diagnosis>(context.GetAllEntities<Diagnosis>());
        }

        private Diagnosis? _selectedDiagnosis;
        public Diagnosis? SelectedDiagnosis
        {
            get { return _selectedDiagnosis; }
            set
            {
                _selectedDiagnosis = value;
                OnPropertyChanged();
            }
        }
    }
}
