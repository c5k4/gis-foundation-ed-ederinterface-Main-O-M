using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;

namespace PGE.BatchApplication.DLMTools.Commands.AnnoStackingValidation
{
    public partial class frmAnnoLeaderGraphics : Form
    {
        private IApplication _app;

        public frmAnnoLeaderGraphics(IApplication app)
        {
            InitializeComponent();
            _app = app;
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            AnnoLeaderGraphics this_tool = new AnnoLeaderGraphics(this, 1, _app);
            this_tool.Run();

            //Form_Status.Delete_Pressed = false;
            //Form_Status.Proceed_Pressed = true;
            //this.Hide();
        }

        private void btnDeleteGraphics_Click(object sender, EventArgs e)
        {
            AnnoLeaderGraphics this_tool = new AnnoLeaderGraphics(this, 2, _app);
            this_tool.Run();

            //Form_Status.Proceed_Pressed = false;
            //Form_Status.Delete_Pressed = true;
            //this.Hide();
        }

        private void frmAnnoLeaderGraphics_Load(object sender, EventArgs e)
        {

        }

        private void frmAnnoLeaderGraphics_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form_Status.Proceed_Pressed = false;
            Form_Status.Delete_Pressed = false;
            Form_Status.Cancel_Pressed = true;
            this.Hide();
        }
    }
}
