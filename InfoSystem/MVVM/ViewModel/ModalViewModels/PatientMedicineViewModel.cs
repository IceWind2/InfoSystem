using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace InfoSystem
{
    internal class PatientMedicineViewModel : ObservableObject
    {
        private int _patientId;
        private Medicine? _selectedMedicine;
        private string _prescription = "";
        private string _note = "";

        private string _medicineError = "";
        private bool _submitted;

        public PatientMedicineViewModel(Patient patient, IEnumerable<Medicine> medicines)
        {
            _patientId = patient.Id;

            var existingMedicineIds = patient.PatientMedicine?.Select(item => item.MedicineId).ToHashSet()
                ?? new HashSet<int>();

            Medicines = medicines
                .Where(medicine => !existingMedicineIds.Contains(medicine.Id))
                .OrderBy(medicine => medicine.Name)
                .ToList();

            Title = "Добавить лекарство пациенту";

            SaveCommand = new RelayCommand(_ =>
            {
                _submitted = true;
                if (!Validate())
                {
                    return;
                }

                Result = new AddPatientMedicineDTO
                {
                    PatientId = _patientId,
                    MedicineId = SelectedMedicine!.Id,
                    Medicine = SelectedMedicine,
                    Prescription = Prescription,
                    Note = string.IsNullOrWhiteSpace(Note) ? null : Note.Trim()
                };

                CloseRequested?.Invoke(true);
            });
        }

        public IReadOnlyList<Medicine> Medicines { get; }
        public string Title { get; }
        public ICommand SaveCommand { get; }
        public AddPatientMedicineDTO? Result { get; private set; }
        public event Action<bool>? CloseRequested;

        public Medicine? SelectedMedicine
        {
            get => _selectedMedicine;
            set { if (SetProperty(ref _selectedMedicine, value)) Revalidate(); }
        }

        public string Prescription
        {
            get => _prescription;
            set => SetProperty(ref _prescription, value);
        }

        public string Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }

        public string MedicineError
        {
            get => _medicineError;
            private set => SetProperty(ref _medicineError, value);
        }

        private void Revalidate()
        {
            if (_submitted)
            {
                Validate();
            }
        }

        private bool Validate()
        {
            MedicineError = SelectedMedicine == null ? "Выберите лекарство" : "";
            return MedicineError.Length == 0;
        }
    }
}
