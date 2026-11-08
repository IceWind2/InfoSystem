using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class MedicineViewModel : TablePageViewModel
    {
        private readonly ObservableCollection<Medicine> _medicines;
        private readonly ICollectionView _medicinesView;
        private Medicine? _selectedMedicine;
        private readonly IDialogService _dialogService = new DialogService();

        public ObservableCollection<Medicine> Medicines => _medicines;

        public Medicine? SelectedMedicine
        {
            get => _selectedMedicine;
            set => SetProperty(ref _selectedMedicine, value);
        }

        public MedicineViewModel(Window mainWindow)
        {
            using (var context = InfoContext.CreateDbContext())
            {
                _medicines = new ObservableCollection<Medicine>(context.GetAllEntities<Medicine>());
            }

            _medicinesView = CollectionViewSource.GetDefaultView(_medicines);
            _medicinesView.Filter = item => item is Medicine medicine && medicine.ContainsFilter(SearchText);
            UpdateTotalCount();

            FilterCommand = new RelayCommand(_ =>
            {
                _medicinesView.Refresh();
                UpdateTotalCount();
            });

            AddCommand = new AsyncRelayCommand(async _ =>
            {
                var viewModel = new NewMedicineViewModel();
                bool? modalResult = _dialogService.ShowNewMedicine(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show("При добавлении лекарства произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
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
                    _medicines.Add(viewModel.Result!);
                    _medicinesView.Refresh();
                    UpdateTotalCount();
                });
            });

            EditCommand = new AsyncRelayCommand(async _ =>
            {
                if (SelectedMedicine == null)
                {
                    return;
                }

                var currentSelectedMedicine = SelectedMedicine;
                var viewModel = new NewMedicineViewModel(currentSelectedMedicine);
                bool? modalResult = _dialogService.ShowNewMedicine(mainWindow, viewModel);

                if (modalResult == null)
                {
                    MessageBox.Show("При обновлении лекарства произошла ошибка", "Ошибка выполнения команды", MessageBoxButton.OK);
                    return;
                }

                if (!modalResult.Value)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                context.UpdateEntity(currentSelectedMedicine);
                await context.SaveChangesAsync(); 

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var index = _medicines.IndexOf(currentSelectedMedicine);
                    if (index >= 0)
                    {
                        _medicines[index] = currentSelectedMedicine;
                        _medicinesView.Refresh();
                        UpdateTotalCount();
                    }
                });
            });

            RemoveCommand = new AsyncRelayCommand(async _ =>
            {
                if (SelectedMedicine == null)
                {
                    return;
                }

                var medicineToRemove = SelectedMedicine;
                using var context = InfoContext.CreateDbContext();
                var patientsWithMedicine = await context.Patients
                    .Include(patient => patient.PatientMedicine)
                    .Where(patient => patient.PatientMedicine!.Any(patientMedicine => patientMedicine.MedicineId == medicineToRemove.Id))
                    .ToListAsync();

                if (patientsWithMedicine.Count > 0)
                {
                    var response = MessageBox.Show("Данное лекарство привязано к одному или нескольким пациентам, вы уверены, что хотите его удалить?", "Подтвердите действие", MessageBoxButton.YesNo);
                    if (response == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                foreach (var patient in patientsWithMedicine)
                {
                    context.PatientsQueries.WritePatientHistory(patient.Id, string.Empty, $"Лекарство {medicineToRemove.Name} удалено из системы");
                }

                context.RemoveEntity(medicineToRemove);
                await context.SaveChangesAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SelectedMedicine = null;
                    _medicines.Remove(medicineToRemove);
                    _medicinesView.Refresh();
                    UpdateTotalCount();
                });
            });
        }

        public void UpdateData()
        {
            using var context = InfoContext.CreateDbContext();
            var medicines = context.GetAllEntities<Medicine>();

            _medicines.Clear();
            foreach (var medicine in medicines)
            {
                _medicines.Add(medicine);
            }

            _medicinesView.Refresh();
            UpdateTotalCount();
        }

        private void UpdateTotalCount()
        {
            TotalCount = _medicinesView.Cast<object>().Count();
            OnPropertyChanged(nameof(TotalCount));
        }
    }
}
