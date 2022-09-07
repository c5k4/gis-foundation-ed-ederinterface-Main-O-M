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

        public AveryLabel(string[] line)
        {
            InitializeComponent();
            this.textBlock1.Text = line[0];
            this.textBlock2.Text = line[1];
            this.textBlock3.Text = line[2];
            this.textBlock4.Text = line[3];
            this.textBlock5.Text = line[4];
        }
    }
}
