using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace InfoSystem
{
    public partial class PatientMedicineModal : Window
    {
        public bool Success { get; set; }
        internal ChangePatientMedicineDTO? Result { get; set; } = null;

        private Patient _patient;

        public PatientMedicineModal(Window parent, Patient patient, bool addMode)
        {
            Owner = parent;
            _patient = patient;
            InitializeComponent();

            var existingMedicine = patient.PatientMedicine!.Select(pm => pm.MedicineId);
            MedicineSelect.ItemsList = DatabaseManager.GetAllMedicine().OrderBy(m => m.Name).Where(m => existingMedicine.Contains(m.Id) != addMode);

            if (!addMode)
            {
                PrescriptionField.Visibility = Visibility.Collapsed;
            }

            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    Success = false;
                    Close();
                }
            };
        }

        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            Success = false;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var errorMessage = ValidateFields();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Success = false;
                    MessageBox.Show(errorMessage, "Ошибка создания объекта", MessageBoxButton.OK);
                    return;
                }

                Result = new ChangePatientMedicineDTO()
                {
                    PatientId = _patient.Id,
                    MedicineId = ((Medicine)MedicineSelect.SelectedItems.First()).Id,
                    Prescription = PrescriptionField.inpValue.Text,
                    Note = NoteField.inpValue.Text,
                };

                Success = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка создания объекта", MessageBoxButton.OK);
                Success = false;
            }
        }
        private string ValidateFields()
        {
            var errorMessage = new StringBuilder();

            if (MedicineSelect.SelectedItems.Any())
            {
                MedicineSelect.BorderColour = Application.Current.Resources.MergedDictionaries[0]["secondaryColourBrush"] as Brush;
            }
            else
            {
                errorMessage.AppendLine("Необходимо выбрать лекарство.");
                MedicineSelect.BorderColour = Application.Current.Resources.MergedDictionaries[0]["errorColourBrush"] as Brush;
            }

            return errorMessage.ToString();
        }

    }
}
