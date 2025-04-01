using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

            LocationSelect.ItemsList = DatabaseManager.Locations.OrderBy(m => m.Name);
            FillSexFormRadio();

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
            LocationSelect.SelectedItems = LocationSelect.ItemsList.Where(l => ((Location)l).Id == patient.LocationId);
            SexRadio.SelectValue(patient.Sex == Sex.Male ? "М" : "Ж");
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
                    Sex = SexRadio.GetSelectedValue() == "М" ? Sex.Male : Sex.Female,
                    LocationId = ((Location)LocationSelect.SelectedItems.First()).Id,
                    Note = NoteTextField.inpValue.Text,
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
                errorMessage.AppendLine("Необходимо заполнить ФИО.");
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

            if (SexRadio.GetSelectedValue() != null)
            {
                SexRadio.BorderColour = Application.Current.Resources.MergedDictionaries[0]["secondaryColourBrush"] as Brush;
            }
            else
            {
                errorMessage.AppendLine("Необходимо выбрать пол.");
                SexRadio.BorderColour = Application.Current.Resources.MergedDictionaries[0]["errorColourBrush"] as Brush;
            }

            if (LocationSelect.SelectedItems.Any())
            {
                LocationSelect.BorderColour = Application.Current.Resources.MergedDictionaries[0]["secondaryColourBrush"] as Brush;
            }
            else
            {
                errorMessage.AppendLine("Необходимо выбрать район.");
                LocationSelect.BorderColour = Application.Current.Resources.MergedDictionaries[0]["errorColourBrush"] as Brush;
            }

            return errorMessage.ToString();
        }

        private void FillSexFormRadio()
        {
            SexRadio.spRadio.Children.Add(new RadioButton
            {
                Content = "М",
                IsChecked = false,
                GroupName = "Sex",
            });
            SexRadio.spRadio.Children.Add(new RadioButton
            {
                Content = "Ж",
                IsChecked = false,
                GroupName = "Sex"
            });
        }
    }
}
