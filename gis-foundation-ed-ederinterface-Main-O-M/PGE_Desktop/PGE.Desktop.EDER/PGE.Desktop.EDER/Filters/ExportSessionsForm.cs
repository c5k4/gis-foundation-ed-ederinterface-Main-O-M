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
    public partial class ExportSessionsForm : Form
    {
        private IMMPxApplication _PxApp;
        private DataTable _dtUser;
        private DataTable _dtStatus;
        private string _selectedUser;
        private string _selectedStatus;
        private string _searchText;
        private string _selectedRadio;

        public string selectedUser
        {
            get {return _selectedUser; }
        }
        public string selectedStatus
        {
            get {return _selectedStatus;}
        }
        public string searchText
        {
            get {return _searchText;}
        }
        public string selectedRadio
        {
            get { return _selectedRadio;}
        }
        

        public ExportSessionsForm(IMMPxApplication pxApp, DataTable dtUser,DataTable dtStatus)
        {
            InitializeComponent();
            _PxApp = pxApp;
            _dtUser = dtUser;
            _dtStatus = dtStatus;
            panel1.Enabled = false;
            textBox1.Enabled = false;
            
        }
        //User/Status Session
        private void RadioUserStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioUserStatus.Checked)
            {
                List<string> userList = new List<string>();
                List<string> status = new List<string>(); ;
                panel1.Enabled = true;
                //comboBox1.Enabled = false;
                userList.Add("ALL USERS");
                foreach (DataRow r in _dtUser.Rows)
                {
                    string row = r["USER_NAME"].ToString();
                    userList.Add(row);
                }
                
                comboBox1.DataSource = userList;
                comboBox1.SelectedIndex = -1;
                status.Add("ALL STATUS");
                foreach (DataRow r in _dtStatus.Rows)
                {
                    status.Add(r["NAME"].ToString());
                }
                                
                comboBox2.DataSource = status;
                comboBox2.SelectedIndex = -1;
            }
            else
                panel1.Enabled = false;
        }
        
        //PM Search Session
        private void RadioPMSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioPMSearch.Checked)
                textBox1.Enabled = true;
            else
                textBox1.Enabled = false;
        }

        private void ExportClick_Click(object sender, EventArgs e)
        {
            
            if (RadioMySssion.Checked)
            {
                _selectedRadio = RadioMySssion.Text;
                this.DialogResult = DialogResult.OK;
                
            }
            else if (RadioAllSession.Checked)
            {
                _selectedRadio = RadioAllSession.Text;
                this.DialogResult = DialogResult.OK;
            }
            else if (RadioUserStatus.Checked)
            {
                try
                {
                    _selectedRadio = RadioUserStatus.Text;
                    _selectedUser = comboBox1.SelectedValue.ToString();
                    _selectedStatus = comboBox2.SelectedValue.ToString();

                    if (!(_selectedUser.Equals("") || _selectedUser.Equals(null) || _selectedStatus.Equals("") || _selectedStatus.Equals(null)))
                    {

                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Please select user/status to get session export");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please select user/status to get session export");
                    
                }
            }
            else if (RadioPMSearch.Checked)
            {
               // textBox1.Text = string.Empty;
                try
                {
                    _selectedRadio = RadioPMSearch.Text;
                    _searchText = textBox1.Text;
                    if (!(_searchText.Equals("") || _searchText.Equals(null)))
                    {

                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Please enter keyword to get session export");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please enter keyword to get session export");
                }
            }
            else
            {
                MessageBox.Show("Please Select option");
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult dr = DialogResult.Cancel;
            this.Close();
        }

             

       

        
    }
}
