using System.Collections.ObjectModel;
using System.Windows;

namespace InfoSystem
{
    internal class PatientsViewModel : ObservableObject
    {
        public ObservableCollection<Patient> Patients { get; set; }

        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public PatientsViewModel(Window mainWindow)
        {
            var context = new InfoContext();
            Patients = new ObservableCollection<Patient>(context.Patients);

            AddCommand = new AsyncRelayCommand(async o =>
            {
                mainWindow.Opacity = 0.4;
                var newPatientModal = new NewPatientModal(mainWindow);
                newPatientModal.ShowDialog();
                mainWindow.Opacity = 1;

                if (newPatientModal.Success)
                {
                    await DatabaseManager.AddPatient(newPatientModal.Result!);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Patients.Add(newPatientModal.Result!);
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
