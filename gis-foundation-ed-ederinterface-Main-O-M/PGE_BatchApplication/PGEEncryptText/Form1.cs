using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telvent.Delivery.Framework;

namespace PGE.BatchApplication.PGEEncryptText
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdCopyToClipboard_Click(object sender, EventArgs e)
        {
            if (txtConfirm.Text != txtPassword.Text)
            {
                MessageBox.Show("Text and Confirm Text must match");
            }
            else if (txtPassword.Text.Length == 0)
            {
                MessageBox.Show("No Text entered");
            }
            else
            {
                Clipboard.SetText(EncryptionFacade.Encrypt(txtPassword.Text));
                MessageBox.Show("Text copied to clipboard");
            }
        }
    }
}
