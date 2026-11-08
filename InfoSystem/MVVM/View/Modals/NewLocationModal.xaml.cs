using System.Windows;

namespace InfoSystem
{
    public partial class NewLocationModal : Window
    {
        public NewLocationModal(Window parent)
        {
            Owner = parent;
            InitializeComponent();

            DataContextChanged += (_, e) =>
            {
                if (e.NewValue is NewLocationViewModel viewModel)
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
