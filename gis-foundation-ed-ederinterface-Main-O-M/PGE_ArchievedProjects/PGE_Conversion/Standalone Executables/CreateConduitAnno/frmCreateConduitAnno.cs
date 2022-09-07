using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CreateConduitAnno
{
    public partial class frmCreateConduitAnno : Form
    {
        public frmCreateConduitAnno()
        {
            InitializeComponent();
        }


        private void btnRun_Click(object sender, EventArgs e)
        {
            Form_Status.Cancel_Pressed = false;
            Form_Status.OK_Pressed = true;
            this.Hide();
        }


        private void btnSecUGConduit_Click(object sender, EventArgs e)
        {
            txtAnno_class_id_to_copy.Text = "0";
            txtAnno_class_id.Text = "2";
            txtAnno_symbol_id.Text = "12";
            txtAnno_cursor_table_name.Text = "EDGIS.SecUGConductorAnno.";
            txtConductor_feature_cursor_table_name.Text = "EDGIS.SecUGConductor.";
            txtAnno_feature_class_alias.Text = "EDGIS.SecUGConductorAnno";
            txtConductor_feature_class_alias.Text = "Secondary Underground Conductor";
            txtConductor_labeltext_attrib.Text = "EDGIS.SecUGConductor.LABELTEXT3";
        }

        private void btnPriUGConduit_Click(object sender, EventArgs e)
        {
            txtAnno_class_id_to_copy.Text = "0";
            txtAnno_class_id.Text = "2";
            txtAnno_symbol_id.Text = "12";
            txtAnno_cursor_table_name.Text = "EDGIS.PriUGConductorAnno.";
            txtConductor_feature_cursor_table_name.Text = "EDGIS.PriUGConductor.";
            txtAnno_feature_class_alias.Text = "EDGIS.PriUGConductorAnno";
            txtConductor_feature_class_alias.Text = "Primary Underground Conductor";
            txtConductor_labeltext_attrib.Text = "EDGIS.PriUGConductor.LABELTEXT4";
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Needs to be coded.");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Form_Status.Cancel_Pressed = true;
            Form_Status.OK_Pressed = false;
            this.Close();
        }

    }
}
