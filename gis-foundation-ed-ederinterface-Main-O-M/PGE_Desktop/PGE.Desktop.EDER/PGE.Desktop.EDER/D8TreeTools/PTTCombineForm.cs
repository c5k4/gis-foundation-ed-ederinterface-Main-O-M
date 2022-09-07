using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER.D8TreeTools
{
    public partial class PTTCombineForm : Form
    {
        public PTTCombineForm(string formMessage, List<int> featuresForSelection)
        {
            InitializeComponent();

            if (featuresForSelection.Count > 0)
            {
                this.Text = formMessage;

                foreach (int oid in featuresForSelection)
                {
                    cboPolesToSelect.Items.Add(oid);
                }
            }
        }

        public int ObjectIDSelected { get; set; }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ObjectIDSelected = (int)cboPolesToSelect.SelectedItem;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
