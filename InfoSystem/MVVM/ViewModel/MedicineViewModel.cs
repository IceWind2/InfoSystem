using System.Collections.ObjectModel;
using System.Windows;

namespace InfoSystem
{
    internal class MedicineViewModel : ObservableObject
    {
        public ObservableCollection<Medicine> Medicine { get; set; }

        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public MedicineViewModel(Window mainWindow)
        {
            var context = new InfoContext();
            Medicine = new ObservableCollection<Medicine>(context.Medicine);

            AddCommand = new AsyncRelayCommand(async o =>
            {
                mainWindow.Opacity = 0.4;
                var newMedicineModal = new NewMedicineModal(mainWindow);
                newMedicineModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newMedicineModal.Success)
                {
                    await DatabaseManager.AddMedicine(newMedicineModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Medicine.Add(newMedicineModal.Result!);
                    });
                }
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedMedicine == null)
                {
                    return;
                }

                await DatabaseManager.RemoveMedicine(SelectedMedicine);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Medicine.Remove(SelectedMedicine);
                });
            });
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
