using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;

namespace InfoSystem
{
    public partial class FormRadio : UserControl, INotifyPropertyChanged
    {
        public FormRadio()
        {
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private Brush _borderColour;
        public Brush BorderColour
        {
            get
            {
                return _borderColour;
            }

            set
            {
                _borderColour = value;
                OnPropertyChanged();
            }
        }

        private string fieldName;
        public string FieldName
        {
            get
            {
                return fieldName;
            }

            set
            {
                fieldName = value;
                OnPropertyChanged();
            }
        }

        public string? GetSelectedValue()
        {
            foreach (var child in spRadio.Children)
            {
                if (child is not RadioButton button)
                {
                    continue;
                }

                if (button.IsChecked.HasValue && button.IsChecked.Value)
                {
                    return button.Content.ToString();
                }
            }

            return null;
        }

        public void SelectValue(string value)
        {
            foreach (var child in spRadio.Children)
            {
                if (child is not RadioButton button)
                {
                    continue;
                }

                if (value.Equals(button.Content as string, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    button.IsChecked = true;
                }
                else
                {
                    button.IsChecked = false;
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
