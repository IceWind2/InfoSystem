using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace InfoSystem
{
    public partial class Toolbar : UserControl
    {
        public Toolbar()
        {
            InitializeComponent();
        }

        public ToggleButton FilterMenuButton => FilterMenuToggle;
    }
}
