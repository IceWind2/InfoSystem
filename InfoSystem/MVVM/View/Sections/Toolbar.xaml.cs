using System.Windows;
using System.Windows.Controls;

namespace InfoSystem
{
    public partial class Toolbar : UserControl
    {
        public Toolbar()
        {
            InitializeComponent();
            UpdateCount(-1);
        }

        public void UpdateCount(int count)
        {
            if (count < 0)
            {
                CounterBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                CounterBlock.Visibility = Visibility.Visible;
                CounterBlock.Text = $"Всего: {count}";
            }
        }
    }
}
