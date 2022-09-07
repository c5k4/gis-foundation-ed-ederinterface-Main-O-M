using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace PGEElecCleanup
{
    public partial class frmMain : Form
    {
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmLogIn login = new frmLogIn();
            //login.ShowDialog();
        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //grbConnectionProperties.Visible = true;
            frmSpatialDatabaseConnection frmConnectionProperties = new frmSpatialDatabaseConnection();
            frmConnectionProperties.ShowDialog();
            if (clsTestWorkSpace.State) connectionToolStripMenuItem.Enabled = false;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        

        private void ConductorInfoDelete_Click(object sender, EventArgs e)
        {
            if (clsTestWorkSpace.Workspace == null)
            {
                MessageBox.Show("Please make connection to the Database then try.", "Database Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ConductorInfoDeleteForm cuuForm = new ConductorInfoDeleteForm();
            cuuForm.Show();
        }
    }
}