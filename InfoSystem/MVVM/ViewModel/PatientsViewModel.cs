using System.Collections.ObjectModel;

namespace InfoSystem
{
    internal class PatientsViewModel : ObservableObject
    {
        public ObservableCollection<Patient> Patients { get; set; }

        public PatientsViewModel()
        {
            Patients = new ObservableCollection<Patient>();
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
