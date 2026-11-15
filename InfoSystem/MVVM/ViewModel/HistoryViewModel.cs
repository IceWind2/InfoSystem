using System.Collections.ObjectModel;
using System.Windows;

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
        public AsyncRelayCommand RemoveHistoryCommand { get; }

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

            RemoveHistoryCommand = new AsyncRelayCommand(async item =>
            {
                if (item is not History historyToRemove || !_history.Contains(historyToRemove))
                {
                    return;
                }

                var confirmation = MessageBox.Show("Вы уверены, что хотите удалить запись из истории?", "Подтвердите действие", MessageBoxButton.YesNo);
                if (confirmation == MessageBoxResult.No)
                {
                    return;
                }

                using var context = InfoContext.CreateDbContext();
                context.RemoveEntity(historyToRemove);
                await context.SaveChangesAsync();

                _history.Remove(historyToRemove);
            });
        }
    }
}
