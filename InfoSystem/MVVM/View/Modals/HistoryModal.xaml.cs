using System.Windows;

namespace InfoSystem
{
    public partial class HistoryModal : Window
    {
        public HistoryModal(Window parent)
        {
            Owner = parent;
            InitializeComponent();

            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    DialogResult = false;
                    Close();
                }
            };
        }
    }
}
