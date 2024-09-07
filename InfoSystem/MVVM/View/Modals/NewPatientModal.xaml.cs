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
        internal CreatePatientDTO? Result { get; set; } = null;

        public NewPatientModal(Window parent)
        {
            Owner = parent;
            InitializeComponent();

            MedicineSelect.ItemsList = DatabaseManager.Medicine.OrderBy(m => m.Name);
            LocationSelect.ItemsList = DatabaseManager.Locations.OrderBy(m => m.Name);

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
            BirthDateFieldBox.inpValue.Text = patient.BirthDate.Date.ToShortDateString();
            GenderFieldBox.inpValue.Text = patient.Gender;
            var medicineIds = patient.Medicine!.Select(m => m.Id).ToHashSet();
            MedicineSelect.SelectedItems = MedicineSelect.ItemsList.Where(m => medicineIds.Contains(((Medicine)m).Id));
            LocationSelect.SelectedItems = LocationSelect.ItemsList.Where(l => ((Location)l).Id == patient.LocationId);
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

                Result = new CreatePatientDTO()
                {
                    Name = NameFieldBox.inpValue.Text,
                    BirthDate = DateTime.Parse(BirthDateFieldBox.inpValue.Text),
                    Gender = GenderFieldBox.inpValue.Text.ToUpper(),
                    MedicineIds = MedicineSelect.SelectedItems.Select(ms => ((Medicine)ms).Id).ToHashSet(),
                    LocationId = ((Location)LocationSelect.SelectedItems.First()).Id
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

            if (DateTime.TryParse(BirthDateFieldBox.inpValue.Text, out var _))
            {
                BirthDateFieldBox.BorderColour = Application.Current.Resources.MergedDictionaries[0]["secondaryColourBrush"] as Brush;
            }
            else
            {
                errorMessage.AppendLine("Неверный формат даты.");
                BirthDateFieldBox.BorderColour = Application.Current.Resources.MergedDictionaries[0]["errorColourBrush"] as Brush;
            }

            if (GenderFieldBox.inpValue.Text.Equals("М", StringComparison.CurrentCultureIgnoreCase) ||
                GenderFieldBox.inpValue.Text.Equals("Ж", StringComparison.CurrentCultureIgnoreCase))
            {
                GenderFieldBox.BorderColour = Application.Current.Resources.MergedDictionaries[0]["secondaryColourBrush"] as Brush;
            }
            else
            {
                errorMessage.AppendLine("Пол должен быть указан как М или Ж.");
                GenderFieldBox.BorderColour = Application.Current.Resources.MergedDictionaries[0]["errorColourBrush"] as Brush;
            }

            if (LocationSelect.SelectedItems.Any())
            {
                LocationSelect.BorderColour = Application.Current.Resources.MergedDictionaries[0]["secondaryColourBrush"] as Brush;
            }
            else
            {
                errorMessage.AppendLine("Необходимо выбрать адрес.");
                LocationSelect.BorderColour = Application.Current.Resources.MergedDictionaries[0]["errorColourBrush"] as Brush;
            }

            return errorMessage.ToString();
        }
    }
}
