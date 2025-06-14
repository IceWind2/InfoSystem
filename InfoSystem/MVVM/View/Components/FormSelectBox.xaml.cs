using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace InfoSystem
{
    public partial class FormSelectBox : UserControl, INotifyPropertyChanged
    {
        public FormSelectBox()
        {
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _fieldName;
        public string FieldName
        {
            get
            {
                return _fieldName;
            }

            set
            {
                _fieldName = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
