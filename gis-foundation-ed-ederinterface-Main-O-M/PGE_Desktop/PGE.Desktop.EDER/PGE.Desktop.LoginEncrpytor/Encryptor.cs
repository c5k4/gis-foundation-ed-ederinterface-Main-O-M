using PGE.Common.Delivery.Framework;
using System;
using System.Windows.Forms;

namespace PGE.Desktop.LoginEncrpytor
{
    public partial class Encryptor : Form
    {
        public Encryptor()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    MMRegistrySettings.DBAdminUser = txtUserName.Text;
                    MMRegistrySettings.DBAdminPassword = txtPassword.Text;
                    MMRegistrySettings.MailFromUser = txtFromuser.Text;
                    MMRegistrySettings.SMTPServer = txtSMTP.Text;
                    MMRegistrySettings.EmailDomain = txtEmail.Text;
                    MMRegistrySettings.ShowSessionManager = chkShowSM.Checked;
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtPlainText.Text))
                    {
                        txtEncrypted.Text = EncryptionFacade.Encrypt(txtPlainText.Text);
                    }
                    else if (!string.IsNullOrEmpty(txtEncrypted.Text))
                    {
                        txtPlainText.Text = EncryptionFacade.Decrypt(txtEncrypted.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                MessageBox.Show("Failed persisting data to registry");
            }
        }

        private void Encryptor_Load(object sender, EventArgs e)
        {
            tabControl1.TabIndex = 1;
            txtUserName.Text = MMRegistrySettings.DBAdminUser;
            txtPassword.Text = MMRegistrySettings.DBAdminPassword;
            txtFromuser.Text = MMRegistrySettings.MailFromUser;
            txtSMTP.Text = MMRegistrySettings.SMTPServer;
            txtEmail.Text = MMRegistrySettings.EmailDomain;
            chkShowSM.Checked = MMRegistrySettings.ShowSessionManager;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
