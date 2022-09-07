using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Miner.Interop.Process;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    public partial class PGEUpdateSessionDesForm : Form
    {
        private string newSDescription = null;

        public string newDescription
        {
            get { return newSDescription; }
        }

        public PGEUpdateSessionDesForm()
        {
            InitializeComponent();
        }

        public PGEUpdateSessionDesForm(IMMPxApplication pxApp)
        {
            InitializeComponent();
            
        }
       
        public PGEUpdateSessionDesForm(string sessionName,string Description)
        {
            InitializeComponent();
            textBox2.Text = Description;
            textBox1.Text = "SN_"+sessionName;
            textBox1.Enabled = false;
        }

        private void Submit_Click(object sender, EventArgs e)
        {

            newSDescription = textBox2.Text;
            if (newSDescription.Equals(null) || newSDescription.Equals(""))
            {
                MessageBox.Show("Please enter session description");
            }
            else
            {
                if (newSDescription.Contains("'"))
                {                    
                    string[] splitDes = newSDescription.Split('\'') ;
                    newSDescription = string.Empty;
                    for(int i = 0; i< splitDes.Count() ; i++)
                    {                        
                        newSDescription = newSDescription + splitDes[i];                        
                    }

                }
                this.DialogResult = DialogResult.OK;
            }

        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.SelectionStart = 0;
        }
    }
}
