using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InfoSystem
{
    public partial class FormSelect : UserControl, INotifyPropertyChanged
    {
        public FormSelect()
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
            
        }

        private void inpValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
