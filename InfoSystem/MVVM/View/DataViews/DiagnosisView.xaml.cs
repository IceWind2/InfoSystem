using System.Windows.Controls;

namespace InfoSystem
{
    public partial class DiagnosisView : UserControl
    {
        public DiagnosisView()
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
