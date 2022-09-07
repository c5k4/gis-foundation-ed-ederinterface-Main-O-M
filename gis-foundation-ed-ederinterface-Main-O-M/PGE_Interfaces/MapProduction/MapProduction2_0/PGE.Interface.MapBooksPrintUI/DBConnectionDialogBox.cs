using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using PGE.Common.Delivery.Systems.Configuration;
using PGE.Common.Delivery.Framework;

namespace PGE.Interfaces.MapBooksPrintUI
{
    public partial class DBConnectionDialogBox : Form
    {
        private string _service;
        private string _user;
        private string _password;

        private UserRegistry _userReg;

        private bool _isCancelled;
        public bool IsCancelled
        {
            get { return _isCancelled; }
        }

        public DBConnectionDialogBox()
        {
            InitializeComponent();
            _userReg = new UserRegistry(Common.MapBookDBConnectionRegKey);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _service = txtService.Text.Trim();
            _user = txtUser.Text.Trim();
            _password = EncryptionFacade.Encrypt(txtPassword.Text.Trim());

            _userReg.SetSetting<string>(Common.MapBookDBConnectionKeyName, _service);
            _userReg.SetSetting<string>(Common.MapBookDBUserKeyName, _user);
            _userReg.SetSetting<string>(Common.MapBookDBPasswordKeyName, _password);

            _isCancelled = false;
        }

        private void DBConnectionDialogBox_Load(object sender, EventArgs e)
        {
            try
            {
                txtService.Text = _userReg.GetSetting<string>(Common.MapBookDBConnectionKeyName, "");
                txtUser.Text = _userReg.GetSetting<string>(Common.MapBookDBUserKeyName, "");
                txtPassword.Text = EncryptionFacade.Decrypt(_userReg.GetSetting<string>(Common.MapBookDBPasswordKeyName, ""));
            }
            catch { }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _isCancelled = true;
        }
    }
}
