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

            AgeFilter.ItemsList = Enumerable.Range(0, 18).Select(x => x.ToString());
            MedicineFilter.ItemsList = DatabaseManager.GetAllMedicine();
            LocationFilter.ItemsList = DatabaseManager.GetAllLocations();
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
