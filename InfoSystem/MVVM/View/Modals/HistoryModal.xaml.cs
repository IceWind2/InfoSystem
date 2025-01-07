using System.Windows;


namespace InfoSystem
{
    public partial class HistoryModal : Window
    {
        public HistoryModal(Window parent, int patientId)
        {
            var historyViewModel = new HistoryViewModel(patientId);
            Owner = parent;
            DataContext = historyViewModel;
            InitializeComponent();

            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    Close();
                }
            };
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
