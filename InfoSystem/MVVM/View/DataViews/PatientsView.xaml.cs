using System.Windows;
using System.Windows.Controls;

namespace InfoSystem
{
    public partial class PatientsView : UserControl
    {
        public PatientsView()
        {
            InitializeComponent();
            DataContextChanged += OnContextChange;
        }

        private void OnContextChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is PatientsViewModel newPvm)
            {
                newPvm.CountUpdated += Filters.UpdateCount;
            }

            if (e.OldValue is PatientsViewModel oldPvm)
            {
                oldPvm.CountUpdated -= Filters.UpdateCount;
            }
        }

        private void DataGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not DataGrid grid)
            {
                return;
            }

            if (grid.SelectedItems != null && grid.SelectedItems.Count == 1)
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
