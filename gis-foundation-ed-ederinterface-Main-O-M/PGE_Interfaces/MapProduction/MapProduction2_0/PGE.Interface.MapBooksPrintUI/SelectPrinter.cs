using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PGE.Interfaces.MapBooksPrintUI
{
    public partial class SelectPrinter : Form
    {
        public bool Cancelled = false;
        public string SelectedPrinter
        {
            get
            {
                return (cboPrinters.SelectedItem ?? "").ToString();
            }
        }

        public SelectPrinter(List<string> Printers)
        {
            InitializeComponent();

            foreach (string printer in Printers)
            {
                if (!cboPrinters.Items.Contains(Printers))
                {
                    cboPrinters.Items.Add(printer);
                }
            }
        }

        private void SelectPrinter_Load(object sender, EventArgs e)
        {

        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedPrinter))
            {
                MessageBox.Show("Please select a printer.", "Select Printer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            this.Close();
        }
    }
}
