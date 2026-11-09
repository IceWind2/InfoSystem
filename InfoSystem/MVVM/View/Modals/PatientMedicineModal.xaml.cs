using System.Windows;

namespace InfoSystem
{
    public partial class PatientMedicineModal : Window
    {
        public PatientMedicineModal(Window parent)
        {
            Owner = parent;
            InitializeComponent();

            DataContextChanged += (_, e) =>
            {
                if (e.NewValue is PatientMedicineViewModel viewModel)
                {
                    viewModel.CloseRequested += result => DialogResult = result;
                }
            };

            PreviewKeyDown += (_, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    DialogResult = false;
                }
            };
        }
    }
}
