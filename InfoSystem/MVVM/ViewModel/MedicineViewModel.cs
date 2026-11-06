using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class MedicineViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<Medicine> _medicine;
        private ICollectionView? _medicineView;
        public ObservableCollection<Medicine> Medicine
        {
            get
            {
                _medicineView = CollectionViewSource.GetDefaultView(_medicine);
                _medicineView.Filter = (x) => (x.ContainsFilter(_filter));
                return _medicine;
            }
        }

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public MedicineViewModel(Window mainWindow)
        {
            using var context = InfoContext.CreateDbContext();
            _medicine = new ObservableCollection<Medicine>(context.GetAllEntities<Medicine>());

            SearchCommand = new RelayCommand(o =>
            {
                _filter = (string)Application.Current.Properties["SearchBoxFilter"]!;
                _medicineView!.Refresh();
            });

            RefreshCommand = new RelayCommand(o =>
            {
                ((MainViewModel)mainWindow.DataContext).UpdateView();
            });

            AddCommand = new AsyncRelayCommand(async o =>
            {
                mainWindow.Opacity = 0.4;
                var newMedicineModal = new NewMedicineModal(mainWindow);
                newMedicineModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (!newMedicineModal.Success)
                {
                    MessageBox.Show($"При добавлении лекарства произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                context.AddEntity(newMedicineModal.Result!);
                await context.CommitAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Medicine.Add(newMedicineModal.Result!);
                });
            });

            EditCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedMedicine == null)
                {
                    return;
                }

                var currentSelectedMedicine = SelectedMedicine;
                mainWindow.Opacity = 0.4;
                var medicineModal = new NewMedicineModal(mainWindow);
                medicineModal.SetData(currentSelectedMedicine);
                medicineModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (medicineModal.Cancelled)
                {
                    return;
                }

                if (!medicineModal.Success)
                {
                    MessageBox.Show($"При обновлении лекарства произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                using var context = await InfoContext.CreateDbContextWithTransactionAsync();
                medicineModal.Result!.Id = currentSelectedMedicine.Id;
                context.UpdateEntity(medicineModal.Result!);
                await context.CommitAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Medicine[Medicine.IndexOf(currentSelectedMedicine)] = medicineModal.Result!;
                });
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedMedicine == null)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                var patientsWithMedicine = await context.Patients
                    .Include(p => p.PatientMedicine)
                    .Where(p => p.PatientMedicine.Select(pm => pm.MedicineId).Contains(SelectedMedicine.Id))
                    .ToListAsync();

                if (patientsWithMedicine.Count > 0)
                {
                    var response = MessageBox.Show("Данное лекарство привязано к одному или нескольким пациентам, вы уверены, что хотите его удалить?", "Подтвердите действие", MessageBoxButton.YesNo);
                    if (response == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                await context.BeginTransactionAsync();
                context.RemoveEntity(SelectedMedicine);
                await context.SaveChangesAsync();

                foreach (var patient in patientsWithMedicine)
                {
                    context.PatientsQueries.WritePatientHistory(patient.Id, string.Empty, $"Лекарство {SelectedMedicine.Name} удалено из системы");
                }

                await context.CommitAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Medicine.Remove(SelectedMedicine);
                });
            });
        }

        public void UpdateData()
        {
            using var context = InfoContext.CreateDbContext();
            _medicine = new ObservableCollection<Medicine>(context.GetAllEntities<Medicine>());
        }

        private Medicine? selectedMedicine;
        public Medicine? SelectedMedicine
        {
            get { return selectedMedicine; }
            set
            {
                selectedMedicine = value;
                OnPropertyChanged();
            }
        }
    }
}
