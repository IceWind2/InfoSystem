using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class DiagnosisViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<Diagnosis> _diagnoses;
        private ICollectionView _diagnosesView;
        public ObservableCollection<Diagnosis> Diagnoses
        {
            get
            {
                _diagnosesView = CollectionViewSource.GetDefaultView(_diagnoses);
                _diagnosesView.Filter = (x) => ((Diagnosis)x).Name.Contains(_filter);
                return _diagnoses;
            }
        }

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public DiagnosisViewModel(Window mainWindow)
        {
            var context = new InfoContext();
            _diagnoses = new ObservableCollection<Diagnosis>(context.Diagnoses.AsNoTracking());

            SearchCommand = new RelayCommand(o =>
            {
                _filter = (string)Application.Current.Properties["SearchBoxFilter"]!;
                _diagnosesView!.Refresh();
            });

            RefreshCommand = new RelayCommand(o =>
            {
                ((MainViewModel)mainWindow.DataContext).UpdateView();
            });

            AddCommand = new AsyncRelayCommand(async o =>
            {
                mainWindow.Opacity = 0.4;
                var newDiagnosisModal = new NewDiagnosisModal(mainWindow);
                newDiagnosisModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newDiagnosisModal.Success)
                {
                    await DatabaseManager.AddDiagnosis(newDiagnosisModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Diagnoses.Add(newDiagnosisModal.Result!);
                    });
                }
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

                if (newDiagnosisModal.Success)
                {
                    newDiagnosisModal.Result!.Id = currentSelectedDiagnosis.Id;
                    await DatabaseManager.UpdateDiagnosis(newDiagnosisModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Diagnoses[Diagnoses.IndexOf(currentSelectedDiagnosis)] = newDiagnosisModal.Result!;
                    });
                }
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedDiagnosis == null)
                {
                    return;
                }

                if (!await DatabaseManager.RemoveDiagnosis(SelectedDiagnosis))
                {
                    MessageBox.Show("Невозможно удалить диагноз, так как он еще используется", "Ошибка удаления", MessageBoxButton.OK);
                    return;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Diagnoses.Remove(SelectedDiagnosis);
                });
            });
        }

        public void UpdateData()
        {
            var context = new InfoContext();
            _diagnoses = new ObservableCollection<Diagnosis>(context.Diagnoses.AsNoTracking());
        }

        private Diagnosis _selectedDiagnosis;
        public Diagnosis SelectedDiagnosis
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
