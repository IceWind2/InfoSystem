using System.Collections.ObjectModel;

namespace InfoSystem
{
    internal class PatientsViewModel : ObservableObject
    {
        public ObservableCollection<Patient> Patients { get; set; }

        public AsyncRelayCommand AddCommand { get; set; }
        public AsyncRelayCommand EditCommand { get; set; }
        public AsyncRelayCommand RemoveCommand { get; set; }

        public PatientsViewModel()
        {
            var context = new InfoContext();
            Patients = new ObservableCollection<Patient>(context.Patients);

            AddCommand = new AsyncRelayCommand(async o =>
            {
                var newPatient = new Patient()
                {
                    Name = "new new new",
                    Age = 11
                };

                await DatabaseManager.AddPatient(newPatient);
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Patients.Add(newPatient);
                });
            });

            RemoveCommand = new AsyncRelayCommand(async o =>
            {
                if (SelectedPatient == null)
                {
                    return;
                }

                await DatabaseManager.RemovePatient(SelectedPatient);
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
