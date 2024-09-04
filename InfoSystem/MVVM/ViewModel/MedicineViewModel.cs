using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class MedicineViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<Medicine> _medicine;
        private ICollectionView _medicineView;
        public ObservableCollection<Medicine> Medicine
        {
            get
            {
                _medicineView = CollectionViewSource.GetDefaultView(_medicine);
                _medicineView.Filter = (x) => ((Medicine)x).Name.Contains(_filter);
                return _medicine;
            }
        }

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public MedicineViewModel(Window mainWindow)
        {
            var context = new InfoContext();
            _medicine = new ObservableCollection<Medicine>(context.Medicine.AsNoTracking());

            SearchCommand = new RelayCommand(o =>
            {
                _filter = (string)Application.Current.Properties["SearchBoxFilter"]!;
                _medicineView!.Refresh();
            });

            RefreshCommand = new RelayCommand(o =>
            {
                ((MainViewModel)mainWindow.DataContext).UpdateView();
            });

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

            EditCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedMedicine == null)
                {
                    return;
                }

                mainWindow.Opacity = 0.4;
                var newMedicineModal = new NewMedicineModal(mainWindow);
                newMedicineModal.SetData(SelectedMedicine);
                newMedicineModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newMedicineModal.Success)
                {
                    newMedicineModal.Result!.Id = SelectedMedicine.Id;
                    await DatabaseManager.UpdateMedicine(newMedicineModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Medicine[Medicine.IndexOf(SelectedMedicine)] = newMedicineModal.Result!;
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

        public void UpdateData()
        {
            var context = new InfoContext();
            _medicine = new ObservableCollection<Medicine>(context.Medicine.AsNoTracking());
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
