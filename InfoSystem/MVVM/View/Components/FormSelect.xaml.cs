using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace InfoSystem
{
    public partial class FormSelect : UserControl, INotifyPropertyChanged
    {
        public FormSelect()
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

        private IEnumerable<object> _itemsList;
        public IEnumerable<object> ItemsList
        {
            get
            {
                return _itemsList;
            }

            set
            {
                _itemsList = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<object> SelectedItems
        {
            get
            {
                return (IEnumerable<object>)lvSelect.SelectedItems;
            }

            set
            {
                foreach (object item in value)
                {
                    lvSelect.SelectedItems.Add(item);
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
