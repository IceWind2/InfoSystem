using System.Collections.ObjectModel;

namespace InfoSystem
{
    internal class HistoryViewModel : ObservableObject
    {
        private ObservableCollection<History> _history;
        public ObservableCollection<History> History
        {
            get
            {
                return _history;
            }
        }

        public object HistoryView
        {
            get { return this; }
        }

        public string Title { get; }

        public HistoryViewModel(Patient patient)
            : this(patient.Id, patient.Name)
        {
        }

        public HistoryViewModel(ArchivedPatient patient)
            : this(patient.PatientId, patient.Name)
        {
        }

        private HistoryViewModel(int patientId, string patientName)
        {
            Title = $"История пациента {patientName}";

            using var context = InfoContext.CreateDbContext();
            _history = new ObservableCollection<History>(context.PatientsQueries.GetPatientHistoryAsync(patientId).GetAwaiter().GetResult());
        }
    }
}
