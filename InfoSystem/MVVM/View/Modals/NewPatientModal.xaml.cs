using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace InfoSystem
{
    public partial class NewPatientModal : Window
    {
        public bool Success { get; set; }
        internal Patient? Result { get; set; } = null;

        public NewPatientModal(Window parent)
        {
            Owner = parent;
            InitializeComponent();

            MedicineSelect.ItemsList = DatabaseManager.Medicine.OrderBy(m => m.Name);

            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    Success = false;
                    Close();
                }
            };
        }

        internal void SetData(Patient patient)
        {
            NameFieldBox.inpValue.Text = patient.Name;
            AgeFieldBox.inpValue.Text = patient.Age.ToString();
            var medicineIds = patient.PatientMedicine!.Select(pm => pm.MedicineId).ToHashSet();
            MedicineSelect.SelectedItems = MedicineSelect.ItemsList.Where(m => medicineIds.Contains(((Medicine)m).Id));
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

                Result = new Patient()
                {
                    Name = NameFieldBox.inpValue.Text,
                    Age = int.Parse(AgeFieldBox.inpValue.Text),
                    PatientMedicine = MedicineSelect.SelectedItems.Select(ms => new PatientMedicine { MedicineId = ((Medicine)ms).Id }).ToList()
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

            if (!string.IsNullOrEmpty(NameFieldBox.inpValue.Text))
            {
                NameFieldBox.BorderColour = Application.Current.Resources.MergedDictionaries[0]["secondaryColourBrush"] as Brush;
            }
            else
            {
                errorMessage.AppendLine("ФИО должно быть заполнено.");
                NameFieldBox.BorderColour = Application.Current.Resources.MergedDictionaries[0]["errorColourBrush"] as Brush;
            }

            if (int.TryParse(AgeFieldBox.inpValue.Text, out var _))
            {
                AgeFieldBox.BorderColour = Application.Current.Resources.MergedDictionaries[0]["secondaryColourBrush"] as Brush;
            }
            else
            {
                errorMessage.AppendLine("Возраст должен быть числом.");
                AgeFieldBox.BorderColour = Application.Current.Resources.MergedDictionaries[0]["errorColourBrush"] as Brush;
            }

            return errorMessage.ToString();
        }
    }
}
