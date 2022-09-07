using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PGE.Desktop.EDER.ArcCatalogCommands.SDPropSync
{
    public partial class PropChooser : Form
    {
        public bool DoUpdate { get; private set; }
        public bool Visibility { get; private set; }
        public bool Aliases { get; private set; }
        public bool Order { get; private set; }

        public string VisibilityFields { get; private set; }

        public PropChooser()
        {
            DoUpdate = false;
            InitializeComponent();
        }

        private void PropChooser_Load(object sender, EventArgs e)
        {

        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            DoUpdate = true;
            Visibility = visibility.Checked;
            if (Visibility)
            {
                VisibilityFields = fields.Text;
            }

            Aliases = aliases.Checked;
            Order = order.Checked;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void order_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
