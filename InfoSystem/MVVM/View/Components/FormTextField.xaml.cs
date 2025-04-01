using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;

namespace InfoSystem
{
    public partial class FormTextField : UserControl, INotifyPropertyChanged
    {
        public FormTextField()
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
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
