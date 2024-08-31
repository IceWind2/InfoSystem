using System.Windows.Controls;

namespace InfoSystem
{
    /// <summary>
    /// Interaction logic for MedicineView.xaml
    /// </summary>
    public partial class MedicineView : UserControl
    {
        public MedicineView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            if (sender is DataGrid grid && grid.SelectedItems != null && grid.SelectedItems.Count == 1)
            {
                var dgr = grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem) as DataGridRow;
                if (!dgr!.IsMouseOver)
                {
                    dgr.IsSelected = false;
                }
            }
        }
    }
}
