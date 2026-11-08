using System.Windows;

namespace InfoSystem
{
    public partial class NewDiagnosisModal : Window
    {
        public NewDiagnosisModal(Window parent)
        {
            Owner = parent;
            InitializeComponent();

            DataContextChanged += (_, e) =>
            {
                if (e.NewValue is NewDiagnosisViewModel viewModel)
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
