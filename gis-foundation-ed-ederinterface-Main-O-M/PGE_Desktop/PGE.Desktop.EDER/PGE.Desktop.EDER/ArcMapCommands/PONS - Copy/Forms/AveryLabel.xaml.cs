using System.Windows.Controls;

namespace Telvent.PGE.ED.Desktop.ArcMapCommands.PONS
{
    /// <summary>
    /// Interaction logic for AveryBarcodeLabel.xaml
    /// </summary>
    public partial class AveryLabel : UserControl
    {
        public AveryLabel()
        {
            InitializeComponent();
        }

        public AveryLabel(string line1, string line2, string line3)
        {
            InitializeComponent();
            this.textBlock1.Text = line1;
            this.textBlock2.Text = line2;
            this.textBlock3.Text = line3;
            this.textBlock4.Text = string.Empty;
        }

        public AveryLabel(string line1, string line2, string line3, string line4)
        {
            InitializeComponent();
            this.textBlock1.Text = line1;
            this.textBlock2.Text = line2;
            this.textBlock3.Text = line3;
            this.textBlock4.Text = line4;
        }
    }
}
