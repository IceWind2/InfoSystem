using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
                _historyView.Filter = (x) => ((History)x).PatientData.Contains(_filter);
                return _history;
            }
        }

        public object HistoryView
        {
            get { return this; }
        }

        public HistoryViewModel(int patientId)
        {
            var context = new InfoContext();
            _history = new ObservableCollection<History>(context.History.Where(h => h.PatientId == patientId)
                                                                        .OrderByDescending(h => h.Timestamp)
                                                                        .AsNoTracking());
        }
    }
}
