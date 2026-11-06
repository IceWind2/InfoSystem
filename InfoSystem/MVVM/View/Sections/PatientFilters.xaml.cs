using System.Linq;
using System.Windows.Controls;

namespace InfoSystem
{
    public partial class PatientFilters : UserControl
    {
        public PatientFilters()
        {
            InitializeComponent();
            UpdateCount(0);

            using var context = InfoContext.CreateDbContext();
            AgeFilter.ItemsList = Enumerable.Range(0, 18).Select(x => x.ToString());
            MedicineFilter.ItemsList = context.GetAllEntities<Medicine>();
            LocationFilter.ItemsList = context.GetAllEntities<Location>();
        }

        public void UpdateCount(int count)
        {
            CounterBlock.Text = $"Всего: {count}";
        }

        private void FilterClick(object sender, System.Windows.RoutedEventArgs e)
        {
            FilterPopup.IsOpen = !FilterPopup.IsOpen;
        }
    }
}
