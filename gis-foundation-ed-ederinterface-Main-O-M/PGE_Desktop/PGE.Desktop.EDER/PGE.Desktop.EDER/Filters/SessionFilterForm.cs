using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Miner.Interop.Process;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.Filters
{
    public partial class SessionFilterForm : Form
    {
        private IMMPxApplication _PxApp;
        private ADODB.Connection _connection = null;
        private DBFacade _DBFacade = null;

        public SessionFilterForm(IMMPxApplication _pxApp)
        {
            InitializeComponent();

            _PxApp = _pxApp;

            _connection = _PxApp.Connection;
            _DBFacade = new DBFacade(_connection);

            //SetupAutoText(txtSessionName, "{Enter Session Name}");

        }

        //private void SetupAutoText(TextBox textBox, string text)
        //{
        //    textBox.Tag = text;
        //    textBox.Text = text;

        //    textBox.GotFocus += new EventHandler(RemovePlaceholder);
        //    textBox.LostFocus += new EventHandler(AddPlaceholder);

        //}

        //private void AddPlaceholder(object sender, EventArgs e)
        //{
        //    TextBox tb = sender as TextBox;
        //    if (tb == null) return;

        //    if (tb.Text.Trim() == "")
        //    {
        //        tb.Text = (string)tb.Tag;
        //    }
        //}

        //private void RemovePlaceholder(object sender, EventArgs e)
        //{
        //    TextBox tb = sender as TextBox;
        //    if (tb == null) return;

        //    //let's retain the value if existed
        //    if (tb.Text == "{Enter Session Name}")
        //        tb.Text = "";
            
        //}

    
        public bool FilterJobName
        {
            get
            {
                return txtSessionName.Text != (string)txtSessionName.Tag;
            }
        }

     public string SessionName
        {
            get
            {
                if (txtSessionName.Text != "{Enter Session Name}")
                {
                    return txtSessionName.Text;
                }
                else
                    return null;
            }
        }

     
        private void cmdOK_Click(object sender, EventArgs e)
        {
            //This is to return the OK event when the OK button is clicked.
            if (txtSessionName.Text.Equals(null) || txtSessionName.Text.Equals(""))
            {
                MessageBox.Show("Please enter some keyword to search sesssion");
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }


}
