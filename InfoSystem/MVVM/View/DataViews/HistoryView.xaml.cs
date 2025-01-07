using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InfoSystem
{
    public partial class HistoryView : UserControl
    {
        public HistoryView()
        {
            InitializeComponent();
            
            HistoryTable.SelectionChanged += (obj, e) =>
                Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
                HistoryTable.UnselectAll()));
        }
    }
}
