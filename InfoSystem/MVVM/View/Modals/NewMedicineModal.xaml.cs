using System.Windows;

namespace InfoSystem
{
    public partial class NewMedicineModal : Window
    {
        public NewMedicineModal(Window parent)
        {
            Owner = parent;
            InitializeComponent();

            DataContextChanged += (_, e) =>
            {
                if (e.NewValue is NewMedicineViewModel viewModel)
                {
                    viewModel.CloseRequested += result => DialogResult = result;
                }
            };

            PreviewKeyDown += (_, e) =>
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
