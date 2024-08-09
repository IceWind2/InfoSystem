using System.Windows;

namespace InfoSystem
{
    public partial class NewPatientModal : Window
    {
        public bool Success {  get; set; }
        internal Patient Result { get; set; }

        public NewPatientModal(Window parent)
        {
            Owner = parent;
            Result = new Patient()
            {
                Name = "new new new",
                Age = 111,
            };
            InitializeComponent();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            Success = true;
            Close();
        }
    }
}
