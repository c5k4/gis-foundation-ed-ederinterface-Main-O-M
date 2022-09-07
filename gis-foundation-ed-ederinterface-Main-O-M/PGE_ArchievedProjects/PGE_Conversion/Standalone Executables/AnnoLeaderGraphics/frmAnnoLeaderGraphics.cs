using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AnnoLeaderGraphics
{
    public partial class frmAnnoLeaderGraphics : Form
    {
        public frmAnnoLeaderGraphics()
        {
            InitializeComponent();
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            AnnoLeaderGraphics this_tool = new AnnoLeaderGraphics(this, 1);
            this_tool.Run();

            //Form_Status.Delete_Pressed = false;
            //Form_Status.Proceed_Pressed = true;
            //this.Hide();
        }

        private void btnDeleteGraphics_Click(object sender, EventArgs e)
        {
            AnnoLeaderGraphics this_tool = new AnnoLeaderGraphics(this, 2);
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
