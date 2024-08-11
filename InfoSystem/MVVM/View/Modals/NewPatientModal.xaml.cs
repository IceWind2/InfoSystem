using System.Windows;

namespace InfoSystem
{
    public partial class NewPatientModal : Window
    {
        public bool Success { get; set; }
        internal Patient Result { get; set; }

        public NewPatientModal(Window parent)
        {
            Owner = parent;
            var vm = new NewPatientViewModel();
            DataContext = vm;
            InitializeComponent();

            test.ItemsList = vm.Medicine;

            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    Success = false;
                    Close();
                }
            };
        }

        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            Success = false;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Success = true;
            Result = new Patient()
            {
                Name = "new new new",
                Age = 111,
            };
            Close();
        }
    }
}
