using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InfoSystem
{
    internal class PatientsViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<Patient> _patients;
        private ICollectionView _patientsView;
        public ObservableCollection<Patient> Patients
        {
            get
            {
                _patientsView = CollectionViewSource.GetDefaultView(_patients);
                _patientsView.Filter = (x) => ((Patient)x).Name.Contains(_filter);
                return _patients;
            }
        }

        public RelayCommand SearchCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public PatientsViewModel(Window mainWindow)
        {
            var context = new InfoContext();
            _patients = new ObservableCollection<Patient>(context.Patients.AsNoTracking()
                                                                          .Include(p => p.Medicine)
                                                                          .Include(p => p.Location)
                                                                          .OrderBy(p => p.Id));

            SearchCommand = new RelayCommand(o =>
            {
                _filter = (string)Application.Current.Properties["SearchBoxFilter"]!;
                _patientsView!.Refresh();
            });

            RefreshCommand = new RelayCommand(o =>
            {
                ((MainViewModel)mainWindow.DataContext).UpdateView();
            });

            AddCommand = new AsyncRelayCommand(async o =>
            {
                mainWindow.Opacity = 0.4;
                var newPatientModal = new NewPatientModal(mainWindow);
                newPatientModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newPatientModal.Success)
                {
                    var newPatient = await DatabaseManager.AddPatient(newPatientModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Patients.Add(newPatient);
                    });
                }
            });

            EditCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                var currentSelectedPatient = SelectedPatient;
                mainWindow.Opacity = 0.4;
                var newPatientModal = new NewPatientModal(mainWindow);
                newPatientModal.SetData(currentSelectedPatient);
                newPatientModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newPatientModal.Success)
                {
                    newPatientModal.Result!.Id = currentSelectedPatient.Id;
                    var newPatient = await DatabaseManager.UpdatePatient(newPatientModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Patients[Patients.IndexOf(currentSelectedPatient)] = newPatient;
                    });
                }
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                await DatabaseManager.RemovePatient(SelectedPatient);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Patients.Remove(SelectedPatient);
                });
            });
        }

        public void UpdateData()
        {
            var context = new InfoContext();
            _patients = new ObservableCollection<Patient>(context.Patients.AsNoTracking()
                                                                          .Include(p => p.Medicine)
                                                                          .Include(p => p.Location)
                                                                          .OrderBy(p => p.Id));
        }

        private Patient selectedPatient;
        public Patient SelectedPatient
        {
            get { return selectedPatient; }
            set
            {
                selectedPatient = value;
                OnPropertyChanged();
            }
        }
    }
}
