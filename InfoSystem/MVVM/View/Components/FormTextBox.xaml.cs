using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InfoSystem
{
    public partial class FormTextBox : UserControl, INotifyPropertyChanged
    {
        public FormTextBox()
        {
            DataContext = this;
            InitializeComponent();
        }

        private string fieldName;

        public event PropertyChangedEventHandler? PropertyChanged;

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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            inpValue.Clear();
            inpValue.Focus();
        }

        private void inpValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbPlaceHolder.Visibility = string.IsNullOrEmpty(inpValue.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
