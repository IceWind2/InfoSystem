using System;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace InfoSystem
{
    public partial class NewLocationModal : Window
    {
        public bool Success { get; set; }
        internal Location? Result { get; set; } = null;

        public NewLocationModal(Window parent)
        {
            Owner = parent;
            InitializeComponent();

            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    Success = false;
                    Close();
                }
            };
        }

        internal void SetData(Location location)
        {
            NameFieldBox.inpValue.Text = location.Name;
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

                Result = new Location()
                {
                    Name = NameFieldBox.inpValue.Text,
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
                errorMessage.AppendLine("Название должно быть заполнено.");
                NameFieldBox.BorderColour = Application.Current.Resources.MergedDictionaries[0]["errorColourBrush"] as Brush;
            }

            return errorMessage.ToString();
        }
    }
}
