using System.Collections.ObjectModel;
using System.Windows;

namespace InfoSystem
{
    internal class MedicineViewModel : ObservableObject
    {
        public ObservableCollection<Medicine> Medicine { get; set; }

        public MedicineViewModel(Window mainWindow)
        {
            var context = new InfoContext();
            Medicine = new ObservableCollection<Medicine>(context.Medicine);
        }

        private Medicine selectedMedicine;
        public Medicine SelectedMedicine
        {
            get { return selectedMedicine; }
            set
            {
                selectedMedicine = value;
                OnPropertyChanged();
            }
        }
    }
}
