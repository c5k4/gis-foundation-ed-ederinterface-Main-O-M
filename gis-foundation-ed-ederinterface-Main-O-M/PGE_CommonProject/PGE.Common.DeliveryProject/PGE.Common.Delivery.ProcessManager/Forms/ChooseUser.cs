using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using Miner.Interop.Process;

namespace PGE.Common.Delivery.Process.BaseClasses
{
    /// <summary>
    /// Form to display Px Users
    /// </summary>
    public partial class ChooseUser : Form
    {
        #region Private Members

        private Dictionary<string, IMMPxUser> _userList = new Dictionary<string, IMMPxUser>();

        #endregion

        #region Public Properties

        private IMMPxUser _selectedUser = null;
        /// <summary>
        /// 
        /// </summary>
        public IMMPxUser SelectedUser
        {
            get { return _selectedUser; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public ChooseUser()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userNames"></param>
        public ChooseUser(Dictionary<string, IMMPxUser> userNames)
        {
            _userList = userNames;
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void ChooseUser_Load(object sender, EventArgs e)
        {
            cboUsers.DataSource = new BindingSource(_userList, null);
            cboUsers.DisplayMember = "Key";
            cboUsers.ValueMember = "Value";
            cboUsers.SelectedIndex = -1;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            _selectedUser = (Miner.Interop.Process.IMMPxUser)cboUsers.SelectedValue;
        }

        #endregion

    }
}
