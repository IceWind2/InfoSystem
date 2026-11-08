using System.Globalization;
using System.Threading;
using System.Windows;

namespace InfoSystem
{
    public partial class NewPatientModal : Window
    {
        public NewPatientModal(Window parent)
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            Owner = parent;
            InitializeComponent();

            DataContextChanged += (_, e) =>
            {
                if (e.NewValue is NewPatientViewModel vm)
                    vm.CloseRequested += ok => DialogResult = ok;
            };

            PreviewKeyDown += (s, e) =>
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
