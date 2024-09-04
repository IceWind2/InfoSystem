using System.Windows;
using System.Windows.Controls;

namespace InfoSystem
{
    public partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            InitializeComponent();
        }

        private void SearchBoxText_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SetFilter();
                SearchBoxButton.Command.Execute(null);
            }
        }

        private void SearchBoxButton_Click(object sender, RoutedEventArgs e)
        {
            SetFilter();
        }

        private void SetFilter()
        {
            Application.Current.Properties["SearchBoxFilter"] = SearchBoxText.Text ?? "";
        }
    }
}
