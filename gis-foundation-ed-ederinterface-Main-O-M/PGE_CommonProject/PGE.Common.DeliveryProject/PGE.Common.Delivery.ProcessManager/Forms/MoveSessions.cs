using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Miner.Interop.Process;

namespace PGE.Common.Delivery.Process.Forms
{
    public partial class MoveSessions : Form
    {
        private Dictionary<string, IMMPxUser> _userList = new Dictionary<string, IMMPxUser>();
        private DataGridView dataGrd = new DataGridView();
        DataTable dtSession = new DataTable();
        private IMMPxUser _selectedUser = null;
        private List<string> _selectedRows = new List<string>();
        //private string _selected = null;
       
        public IMMPxUser SelectedUser
        {
            get { return _selectedUser; }
        }

        public List<string> selectedRows
        {
            get { return _selectedRows; }
        }

        public MoveSessions()
        {
            InitializeComponent();
        }

        public MoveSessions(DataTable dt, Dictionary<string, IMMPxUser> userNames)
        {

            InitializeComponent();
            dtSession = dt.Copy();
           //dt.Columns.Add("Select Session", typeof(bool));
            dataGridView1.DataSource = dt;
            //Add checkbox to DataGridView
            DataGridViewCheckBoxColumn chkBox = new DataGridViewCheckBoxColumn();
            chkBox.HeaderText = "";
            chkBox.Width = 30;
            chkBox.Name = "SelectSession";
            dataGridView1.Columns.Insert(0, chkBox);
       

            foreach (DataGridViewColumn dc in dataGridView1.Columns)
            {
                if (dc.Index.Equals(0))
                {
                    dc.ReadOnly = false;
                }
                else
                {
                    dc.ReadOnly = true;
                }
            }

            dataGrd = dataGridView1;
            //Populate List of Px Users
            _userList = userNames;
            cboUsers.DataSource = new BindingSource(_userList, null);
            cboUsers.DisplayMember = "Key";
            cboUsers.ValueMember = "Value";
            cboUsers.SelectedIndex = -1;
           // InitializeComponent();
        }

        //private void MoveSessions_Load(object sender, EventArgs e)
        //{
        //    InitializeComponent();
            
        //    dataGrd.DataSource = dtSession;
        //    DataGridViewCheckBoxColumn chkBox = new DataGridViewCheckBoxColumn();
        //    chkBox.HeaderText = "";
        //    chkBox.Width = 30;
        //    chkBox.Name = "SelectSession";
        //    dataGridView1.Columns.Insert(0, chkBox);
            
        //    //Populate List of Px Users
            
        //    cboUsers.DataSource = new BindingSource(_userList, null);
            
        //    cboUsers.DisplayMember = "Key";
        //    cboUsers.ValueMember = "Value";
        //    cboUsers.SelectedIndex = -1;
        //    InitializeComponent();
        //}

        private void ClickAssign_Click(object sender, EventArgs e)
        {
            //Get Selected sessions from gridview
            string sessionName = string.Empty;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool isSelected = false;
                isSelected = Convert.ToBoolean(row.Cells["SelectSession"].Value);
                if (isSelected)
                {
                    sessionName = row.Cells["Session ID"].Value.ToString();
                    _selectedRows.Add(sessionName);

                }
                else { }
                   
            }
            //Get Selcted Px User from combobox
            _selectedUser = (Miner.Interop.Process.IMMPxUser)cboUsers.SelectedValue;
           

            // Session Assignment Form Validation
            Boolean chkboxFlag = false;
            foreach (DataGridViewRow dataGridRow in dataGridView1.Rows)
            {
                if (dataGridRow.Cells["SelectSession"].Value != null && (bool)dataGridRow.Cells["SelectSession"].Value)
                {
                    chkboxFlag = true;
                }
               
            }


            if ((_selectedUser == null) || _selectedRows.Count <= 0 || chkboxFlag == false)
            {
                MessageBox.Show("Please select session(s) and user for session assignment");

            }
            else
            {
                
                this.DialogResult = DialogResult.OK;
            }



        }

        private void CancelClk_Click(object sender, EventArgs e)
        {
            
                this.Close();
           
        }
      
    }
}
