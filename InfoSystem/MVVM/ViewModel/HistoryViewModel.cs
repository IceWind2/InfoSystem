using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace InfoSystem
{
    internal class HistoryViewModel : ObservableObject
    {
        private string _filter = "";
        private ObservableCollection<History> _history;
        private ICollectionView _historyView;
        public ObservableCollection<History> History
        {
            get
            {
                _historyView = CollectionViewSource.GetDefaultView(_history);
                _historyView.Filter = (x) => (x.ContainsFilter(_filter)); ;
                return _history;
            }
        }

        public object HistoryView
        {
            get { return this; }
        }

        public HistoryViewModel(int patientId)
        {
            _history = new ObservableCollection<History>(DatabaseManager.GetPatientHistory(patientId).GetAwaiter().GetResult());
        }
    }
}
