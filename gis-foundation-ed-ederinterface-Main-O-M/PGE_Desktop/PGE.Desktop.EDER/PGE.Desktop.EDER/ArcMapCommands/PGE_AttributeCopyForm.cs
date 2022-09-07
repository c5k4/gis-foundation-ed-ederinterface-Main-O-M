// ========================================================================
// Copyright © 2021 PGE.
// <history>
// PGE Attribute Copy Tool Form
// TCS V3SF (EDGISREARC-1363)              09/11/2021               Created
// </history>
// All rights reserved.
// ========================================================================

using System;
using System.Windows.Forms;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    public partial class PGE_AttributeCopyForm : Form
    {
        public PGE_AttributeCopyForm()
        {
            InitializeComponent();
        }

        public PGE_AttributeCopyForm(string resultMessage)
        {
            InitializeComponent();
            txtResultWindow.Text = resultMessage;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
