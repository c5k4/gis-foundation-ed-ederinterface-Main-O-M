using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PGE.BatchApplication.AllignAnnotation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            chkAlignment.Checked = false;
            chkAngle.Checked = false;
        }

        private void btnAllign_Click(object sender, EventArgs e)
        {
            this.Close();
            AnnoAllignment.gBlnStart = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chkAlignment.Checked = false;
            chkAngle.Checked = false;

            if (true == AnnoAllignment.gBlnJobAnno)
            {
                chkAlignment.Visible = false;
                chkAngle.Visible = false;
            }
        }

        private void chkAlignment_CheckedChanged(object sender, EventArgs e)
        {
            if (true == chkAlignment.Checked)
            {
                AnnoAllignment.gBlnAllign = true;
            }
        }

        private void chkAngle_CheckedChanged(object sender, EventArgs e)
        {
            if (true == chkAngle.Checked)
            {
                AnnoAllignment.gBlnPerp = true;
            }
        }

    }
}
