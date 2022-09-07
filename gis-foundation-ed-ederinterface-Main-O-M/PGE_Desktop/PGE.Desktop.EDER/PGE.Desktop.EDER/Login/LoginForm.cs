using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using Miner.Framework;
using ADODB;
using System.Data;
using Oracle.DataAccess.Client;
using System.Data.OleDb;
using Microsoft.Win32;
using System.Net.Mail;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using Miner;
using Miner.Interop.Process;
using PGE_DBPasswordManagement;


namespace PGE.Desktop.EDER.Login
{
    public enum ConnectionType { NotSet = 0, SDE, PersonalGDB, SQlServer }

    /// <summary>
    /// Class that is used to show hte custom login form to hte user
    /// Implements System.Windows.Form, IMMLoginObject, IMMChangeDefaultVersion and IMMAdoConnection
    /// </summary>
    [ComVisible(true)]
    [Guid("2E33B960-E9F2-46f2-A50F-D830289DA0C6")]
    [ProgId("PGE.CustomLogin")]
    public class LoginForm : System.Windows.Forms.Form, IMMLoginObject, IMMChangeDefaultVersion,IMMAdoConnection
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #region Designer Generated Private Fields
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label lblDB;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private PictureBox pictureBox;
        private System.Windows.Forms.Label lblPassword;
        private System.ComponentModel.Container components = null;

        #endregion

        #region Private Fields
        private MMDefaultLoginObjectClass _defaultLogin;
        //private Logger logger = Logger.Instance;
        private IPropertySet _propSet;
        private bool _initialLogin;
        private bool _loginChanged;
        private ConnectionType connectionType;
        private string server = null;
        private string instance = null;
        private string personalGeoDbPath = null;
        private string datasource = null;
        private System.Windows.Forms.Button btnChangePassword;
        private System.Windows.Forms.Button btnResetPassword;
        private GroupBox groupBox1;
        private System.Windows.Forms.Button btnSessionManager;
        IMMRegistry reg = new MMRegistry();


        #region ADO Connection Params for Session Manager
        private bool _enableAutoLogin = false;
        private string _provider = string.Empty;
        private string _smdatasource = string.Empty;
        private Label lblDataSourceWarn;
        private string _server = string.Empty;
        #endregion
        #endregion

        #region CTOR
        public LoginForm()
        {
            // windows forms generated code
            InitializeComponent();
            btnChangePassword.Enabled = false;
            btnResetPassword.Enabled = false;
            btnChangePassword.Visible = false;
            btnResetPassword.Visible = false;
            _defaultLogin = new MMDefaultLoginObjectClass();
            _propSet = new PropertySetClass();
            ReadCurrentConnectionInformation();
            GetDSAndSetControlOptions();
            //btnSessionManager.Visible = ShowSessionManager();
            btnSessionManager.Visible = MMRegistrySettings.ShowSessionManager;
            btnOK.Focus();
        }
        public LoginForm(bool initialLogin):this()
        {
            _initialLogin = initialLogin;
        }
        #endregion

        #region Dispose of Components
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Windows Form Designer generated code

        [Obsolete]
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.lblDataSourceWarn = new System.Windows.Forms.Label();
            this.btnSessionManager = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblDB = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.btnResetPassword = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.lblDataSourceWarn);
            this.groupBox.Controls.Add(this.btnSessionManager);
            this.groupBox.Controls.Add(this.btnBrowse);
            this.groupBox.Controls.Add(this.lblDB);
            this.groupBox.Location = new System.Drawing.Point(12, 214);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(274, 162);
            this.groupBox.TabIndex = 10;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Database";
            // 
            // lblDataSourceWarn
            // 
            this.lblDataSourceWarn.ForeColor = System.Drawing.Color.Red;
            this.lblDataSourceWarn.Location = new System.Drawing.Point(9, 91);
            this.lblDataSourceWarn.Name = "lblDataSourceWarn";
            this.lblDataSourceWarn.Size = new System.Drawing.Size(253, 33);
            this.lblDataSourceWarn.TabIndex = 13;
            this.lblDataSourceWarn.Text = "Compatible Service property format: sde:oracle11g:<SID>";
            this.lblDataSourceWarn.Visible = false;
            // 
            // btnSessionManager
            // 
            this.btnSessionManager.BackColor = System.Drawing.SystemColors.Control;
            this.btnSessionManager.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSessionManager.BackgroundImage")));
            this.btnSessionManager.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSessionManager.Location = new System.Drawing.Point(235, 131);
            this.btnSessionManager.Name = "btnSessionManager";
            this.btnSessionManager.Size = new System.Drawing.Size(29, 24);
            this.btnSessionManager.TabIndex = 12;
            this.btnSessionManager.UseVisualStyleBackColor = false;
            this.btnSessionManager.Click += new System.EventHandler(this.btnSessionManager_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(8, 131);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(118, 24);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "&Browse";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblDB
            // 
            this.lblDB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDB.Location = new System.Drawing.Point(9, 21);
            this.lblDB.Name = "lblDB";
            this.lblDB.Size = new System.Drawing.Size(259, 62);
            this.lblDB.TabIndex = 11;
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(112, 23);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(153, 20);
            this.txtUser.TabIndex = 3;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(111, 50);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(153, 20);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.Enter += new System.EventHandler(this.txtPassword_Enter);
            this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
            this.txtPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(43, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "User Name: ";
            // 
            // lblPassword
            // 
            this.lblPassword.Location = new System.Drawing.Point(50, 50);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(64, 20);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password: ";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(58, 467);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(105, 28);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(184, 467);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.Location = new System.Drawing.Point(41, 12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(224, 196);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Location = new System.Drawing.Point(58, 506);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(105, 28);
            this.btnChangePassword.TabIndex = 11;
            this.btnChangePassword.Text = "Change &Password";
            this.btnChangePassword.UseVisualStyleBackColor = true;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.Location = new System.Drawing.Point(184, 506);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(105, 28);
            this.btnResetPassword.TabIndex = 12;
            this.btnResetPassword.Text = "&Reset Password";
            this.btnResetPassword.UseVisualStyleBackColor = true;
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtUser);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lblPassword);
            this.groupBox1.Location = new System.Drawing.Point(12, 382);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 79);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database authentication";
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(301, 507);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnResetPassword);
            this.Controls.Add(this.btnChangePassword);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login to ArcFM";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.groupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        #region public Properties
        public string PersonalGeoDbPath
        {
            get { return personalGeoDbPath; }
        }
        public string UserName
        {
            get { return txtUser.Text.ToUpper(); }
        }
        public string Password
        {
            get { return txtPassword.Text; }
        }
        public ConnectionType ConnectionType
        {
            get { return connectionType; }
        }
        public string Server
        {
            get { return server; }
        }
        public string Instance
        {
            get { return instance; }
        }
        public string DataSource
        {
            get { return datasource; }
        }
        #endregion

        #region Database Methods
        /// <summary>
        /// Generates a new password and tries to reset password to the new password for hte user sent.
        /// SMTP Server name and the From User name is obtained from Registry value.
        /// "SMTPServer" and "MailFromUser". 
        /// These should be under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin key
        /// </summary>
        /// <param name="UserName">Username for which the password has to be reset</param>
        /// <param name="serverInstance">Server Name</param>
        private void CreateNewPasswordAndMail(string UserName, string serverInstance)
        {
            int new_value_edersub = 0;
            int new_value_landbase = 0;
            int new_value_eder = 0;
            string origDS = string.Empty;

            // m4jf edgisrearch - 390 reset and change password will now change /reset password for four databases (EDER, EDERSUB , EDSCHM & LBMAINT )
            int new_value_edschm = 0;
            // m4jf edgisrearch 919 get env type from registry
            string Env = default;
            
            try
            {
                // hourglass cursor
                Cursor = Cursors.WaitCursor;
                Env = MMRegistrySettings.Environment;
                string substrdatasrc = datasource.Substring(datasource.Length - 2);
                if(!(substrdatasrc.ToUpper() == Env.ToUpper()))
                {
                    if (!datasource.ToString().ToUpper().Contains("_"))
                    {
                        datasource = datasource + "_" + Env;
                    }
                    
                }
                // string substrServerInst = serverInstance.Substring(datasource.Length - 3);
                if (!serverInstance.ToString().ToUpper().Contains("_"))
                {
                    serverInstance = serverInstance + "_" + Env;
                }
               
                // m4jf edgisrearch - 390  Get new password using PGE_DBPasswordManagement
                // string newPassword = UserName + "p" + DateTime.Now.ToString().GetHashCode().ToString() + "#";
                string newPassword = ReadEncryption.GenPassword(UserName.ToUpper() + "@" + datasource.ToUpper());

                //newPassword = newPassword.Replace('-', 'd');
                string reciepientEmailAddress = UserName + "@" + MMRegistrySettings.EmailDomain;
                int result = ResetPassword(UserName, newPassword);
                //reset password for EDER, EDERSUB & LANDBASE
                if (result == 1)
                {
                    int length = datasource.Length - 3;
                    origDS = datasource.Substring(0, length);
                    switch (origDS.ToUpper())
                    {
                        case "EDER":
                            serverInstance = "EDERSUB";
                            bool user_access = ChangePasswordForm.user_exists_check(UserName, serverInstance);
                            if (user_access == true)
                            {
                                datasource = "EDERSUB";
                                new_value_edersub = ResetPassword(UserName, newPassword);
                                if (new_value_edersub == 0)
                                {
                                    MessageBox.Show("Password cannot be reset for EDERSUB", "Reset Password");
                                }
                            }
                            serverInstance = "LBMAINT";
                            user_access = ChangePasswordForm.user_exists_check(UserName, serverInstance);
                            if (user_access == true)
                            {
                                datasource = "LBMAINT";
                                new_value_landbase = ResetPassword(UserName, newPassword);
                                if (new_value_landbase == 0)
                                {
                                    MessageBox.Show("Password cannot be reset for LANDBASE", "Reset Password");
                                }
                            }
                            //M4JF EDGISREARCH 390 
                            serverInstance = "EDSCHM";
                            user_access = ChangePasswordForm.user_exists_check(UserName, serverInstance);
                            if (user_access == true)
                            {
                                datasource = "EDSCHM";
                                new_value_edschm = ResetPassword(UserName, newPassword);
                                if (new_value_edschm == 0)
                                {
                                    MessageBox.Show("Password cannot be reset for EDSCHM", "Reset Password");
                                }
                            }
                            datasource = "EDER";
                            break;

                        case "EDERSUB":
                            //serverInstance = "EDER";
                            datasource = "EDER";
                            new_value_eder = ResetPassword(UserName, newPassword);
                            if (new_value_eder == 0)
                            {
                                MessageBox.Show("Password cannot be reset for EDER_"+Env, "Reset Password");
                            }
                            datasource = "LBMAINT";
                            serverInstance = "LBMAINT";
                            user_access = ChangePasswordForm.user_exists_check(UserName, serverInstance);
                            if (user_access == true)
                            {
                                new_value_landbase = ResetPassword(UserName, newPassword);
                                if (new_value_landbase == 0)
                                {
                                    MessageBox.Show("Password cannot be reset for LANDBASE_" + Env, "Reset Password");
                                }
                            }
                            //M4JF EDGISREARCH 390 
                            serverInstance = "EDSCHM";
                            user_access = ChangePasswordForm.user_exists_check(UserName, serverInstance);
                            if (user_access == true)
                            {
                                datasource = "EDSCHM";
                                new_value_edschm = ResetPassword(UserName, newPassword);
                                if (new_value_edschm == 0)
                                {
                                    MessageBox.Show("Password cannot be reset for EDSCHM_" + Env, "Reset Password");
                                }
                            }
                            datasource = "EDERSUB";
                            break;
                        case "LBMAINT":
                            //serverInstance = "EDER";
                            datasource = "EDER";
                            new_value_eder = ResetPassword(UserName, newPassword);                           
                            if (new_value_eder == 0)
                            {
                                MessageBox.Show("Password cannot be reset for EDER_" + Env, "Reset Password");
                            }
                            datasource = "EDERSUB";
                            serverInstance = "EDERSUB";
                            user_access = ChangePasswordForm.user_exists_check(UserName, serverInstance);
                            if (user_access == true)
                            {
                                new_value_edersub = ResetPassword(UserName, newPassword);
                                if (new_value_edersub == 0)
                                {
                                    MessageBox.Show("Password cannot be reset for EDERSUB_" + Env, "Reset Password");
                                }
                            }

                            //M4JF EDGISREARCH 390 
                            serverInstance = "EDSCHM";
                            datasource = "EDSCHM";
                            user_access = ChangePasswordForm.user_exists_check(UserName, serverInstance);
                            if (user_access == true)
                            {
                               
                                new_value_edschm = ResetPassword(UserName, newPassword);
                                if (new_value_edschm == 0)
                                {
                                    MessageBox.Show("Password cannot be reset for EDSCHM_" + Env, "Reset Password");
                                }
                            }

                            datasource = "LBMAINT";
                            break;

                            //M4JF EDGISREARCH 390 EDSCHM database password will also be reset while reset password . 
                        case "EDSCHM":
                            datasource = "EDER";
                            new_value_eder = ResetPassword(UserName, newPassword);
                            if (new_value_eder == 0)
                            {
                                MessageBox.Show("Password cannot be reset for EDER_" + Env, "Reset Password");
                            }
                            datasource = "LBMAINT";
                            serverInstance = "LBMAINT";
                            user_access = ChangePasswordForm.user_exists_check(UserName, serverInstance);
                            if (user_access == true)
                            {
                                new_value_landbase = ResetPassword(UserName, newPassword);
                                if (new_value_landbase == 0)
                                {
                                    MessageBox.Show("Password cannot be reset for LANDBASE_" + Env, "Reset Password");
                                }
                            }
                            datasource = "EDERSUB";
                            serverInstance = "EDERSUB";
                            user_access = ChangePasswordForm.user_exists_check(UserName, serverInstance);
                            if (user_access == true)
                            {
                                datasource = "EDERSUB";
                                new_value_edersub = ResetPassword(UserName, newPassword);
                                if (new_value_edersub == 0)
                                {
                                    MessageBox.Show("Password cannot be reset for EDERSUB_" + Env, "Reset Password");
                                }
                            }
                            break;

                        default:
                            break;
                    }
                    serverInstance = origDS;
                }
                string smtpServer = MMRegistrySettings.SMTPServer;
                string fromUser = MMRegistrySettings.MailFromUser;
                if (result != 0 && !string.IsNullOrEmpty(smtpServer) && !string.IsNullOrEmpty(fromUser))
                {
                    // Send newPassword to reciepientEmailAddress
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(fromUser);
                    message.To.Add(new MailAddress(reciepientEmailAddress));
                    //message.From = "gtgis@gtgisdev01.comp.pge.com";
                    message.Subject = "Your ArcFM Login Password has been reset.";
                    
                    switch (origDS.ToUpper())
                    {
                        case "LBMAINT":
                            if (new_value_eder == 0 && new_value_edersub == 0)
                            {
                                message.Body = "Your ArcFM Login Password for LANDBASE has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            else if (new_value_eder != 0 && new_value_edersub == 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDER & LANDBASE has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            else if (new_value_eder == 0 && new_value_edersub != 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDERSUB & LANDBASE has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            else
                            {
                                message.Body = "Your ArcFM Login Password for EDER, EDERSUB & LANDBASE has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            break;
                        case "EDER":
                            if (new_value_edersub == 0 && new_value_landbase == 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDER has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            else if (new_value_edersub != 0 && new_value_landbase == 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDER & EDERSUB has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            else if (new_value_edersub == 0 && new_value_landbase != 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDER & LANDBASE has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            // M4JF EDGISREARCH 390
                            else if (new_value_edersub == 0 && new_value_edschm != 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDER & EDSCHM has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            // M4JF EDGISREARCH 390 - Updated email body to include EDSCHM database detail also 
                            else
                            {
                                message.Body = "Your ArcFM Login Password for EDER, EDERSUB, EDSCHM & LANDBASE has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            break;
                        case "EDERSUB":
                            if (new_value_eder == 0 && new_value_landbase == 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDERSUB has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            else if (new_value_eder != 0 && new_value_landbase == 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDER & EDERSUB has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            else if (new_value_eder == 0 && new_value_landbase != 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDERSUB & LANDBASE has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            // M4JF EDGISREARCH 390
                            else if (new_value_eder == 0 && new_value_edschm != 0)
                            {
                                message.Body = "Your ArcFM Login Password for EDERSUB & EDSCHM has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            // M4JF EDGISREARCH 390 - Updated email body to include EDSCHM database detail also
                            else
                            {
                                message.Body = "Your ArcFM Login Password for EDER, EDERSUB , EDSCHM & LANDBASE has been reset to :- " + newPassword + " . Please Click on the change password button to change your password on next login.";
                            }
                            break;
                        default:
                            break;
                    } 
                    SmtpClient client = new SmtpClient(smtpServer);
                    client.Send(message);
                    //SmtpMail.SmtpServer = "mailhost.utility.pge.com";
                    MessageBox.Show("Please check your email at : " + reciepientEmailAddress + " for the new password.", "Reset Password");
                }
                else
                {
                    MessageBox.Show("Unable to reset your password.");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Sets the User Password to the password sent by calling hte Oracle Procedure SYS.USER_CHANGE_PASSWD_PKG.SP_CHANGEPASSWD
        /// The Package should be created in the Database and the Admin user ID and Password who has privilege on the Package should be stored in the Registry key
        /// "ArcFMLoginSuperUserName" "ArcFMLoginSuperUserPassword"
        /// These should be under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin key
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public Int32 ResetPassword(string userId, string newPassword)
        {
            string connectionString = string.Empty;
            Int32 Count = 0;
            if (!string.IsNullOrEmpty(datasource))
            {
                
                string superUserName = MMRegistrySettings.DBAdminUser;
               

                 // m4jf edgisrearch - 390 Get superuser password from Connections.xml of PGE_DBPasswordManagement
                 //string SuperUserPassword = MMRegistrySettings.DBAdminPassword;
                 string SuperUserPassword = ReadEncryption.GetPassword(superUserName.ToUpper()+"@"+datasource.ToUpper());

                
                newPassword = "\"" + newPassword + "\"";

                if ((!string.IsNullOrEmpty(superUserName)) && (!string.IsNullOrEmpty(SuperUserPassword)))
                {                    
                    connectionString = "Data Source=" + datasource + ";" +
                            "User ID=" + superUserName + ";" +
                            "Password=" + SuperUserPassword + ";";
                    OracleConnection connection = new OracleConnection(connectionString);
                    try
                    {
                        connection.Open();
                        OracleCommand objSelectCmd = new OracleCommand();
                        objSelectCmd.Connection = connection;
                        objSelectCmd.CommandText = "SYS.USER_CHANGE_PASSWD_PKG.SP_CHANGEPASSWD";
                        objSelectCmd.CommandType = CommandType.StoredProcedure;
                        OracleParameter prmInputUserName = new OracleParameter("i_username", OracleDbType.Varchar2);
                        prmInputUserName.Direction = ParameterDirection.Input;
                        prmInputUserName.Value = userId;
                        objSelectCmd.Parameters.Add(prmInputUserName);
                        OracleParameter prmInputPassword = new OracleParameter("i_passwd", OracleDbType.Varchar2);
                        prmInputPassword.Direction = ParameterDirection.Input;
                        prmInputPassword.Value = newPassword;
                        objSelectCmd.Parameters.Add(prmInputPassword);
                        // M4JF EDGISREARCH 390 - below line is commented as SP does not return any value 
                       // Count = objSelectCmd.ExecuteNonQuery();
                        objSelectCmd.ExecuteNonQuery();
                        Count = 1;

                    }                    
                    catch (Exception ex)
                    {
                        _logger.Debug(ex.Message);
                    }
                    connection.Close();
                }
               
            }

            return Count;
        }

        /// <summary>
        /// Checks for PAssword expiry for the User name s populated in the User Name text box by calling the Oracle Procedure "SYS.USER_CHANGE_PASSWD_PKG.SP_USER_STATUS_SELECT"
        /// If the Password has expired 
        /// The Package should be created in the Database and the Admin user ID and Password who has privilege on the Package should be stored in the Registry key
        /// "ArcFMLoginSuperUserName" "ArcFMLoginSuperUserPassword"
        /// These should be under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin key
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public string CheckPasswordExpiry()
        {
            string connectionString = string.Empty;
            string result = "OPEN";

            string superUserName = MMRegistrySettings.DBAdminUser;
            // m4jf edgisrearch 919 - get password using PGE_DBPasswordmanagement
            //string SuperUserPassword = MMRegistrySettings.DBAdminPassword;

            string SuperUserPassword = ReadEncryption.GetPassword(superUserName.ToUpper() + "@" + datasource.ToUpper());


            if ((!string.IsNullOrEmpty(superUserName)) && (!string.IsNullOrEmpty(SuperUserPassword)))
            {
                if (!string.IsNullOrEmpty(datasource))
                {

                    connectionString = "Data Source=" + datasource + ";" +
                               "User ID=" + superUserName + ";" +
                               "Password=" + SuperUserPassword + ";";
                    OracleConnection oracleConnection = new OracleConnection(connectionString);
                    try
                    {
                        oracleConnection.Open();
                        string strquery = "SELECT ACCOUNT_STATUS, LOCK_DATE, EXPIRY_DATE,CREATED FROM DBA_USERS WHERE USERNAME = '" + UserName + "'";
                        DataTable dt = GetDataTable_Login(strquery, oracleConnection);
                        if (dt != null && dt.Rows.Count == 1)
                        {
                            DataRow dr = dt.Rows[0];
                            if (!string.IsNullOrEmpty(dr["ACCOUNT_STATUS"].ToString()))
                            {
                                if (dr["ACCOUNT_STATUS"].ToString() == "OPEN")
                                {
                                    result = dr["ACCOUNT_STATUS"].ToString();
                                }
                                else
                                {
                                    result = dr["ACCOUNT_STATUS"].ToString() + ";" + dr["EXPIRY_DATE"].ToString();
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Debug(ex.Message);
                    }
                    finally
                    {
                        if(oracleConnection !=null && oracleConnection.State ==ConnectionState.Open)   oracleConnection.Close();
                    }
                }
            }
            return result;
        }
        public DataTable GetDataTable_Login(string strQuery, OracleConnection Connec)
        {
            strQuery = strQuery.Trim();
            //Open Connection

            DataSet dsData = new DataSet();
            DataTable DT = new DataTable();
            OracleCommand cmdExecuteSQL = null;
            OracleDataAdapter daOracle = null;
            try
            {
                cmdExecuteSQL = new OracleCommand();
                cmdExecuteSQL.CommandType = CommandType.Text;
                cmdExecuteSQL.CommandTimeout = 0;
                cmdExecuteSQL.CommandText = strQuery;
                cmdExecuteSQL.Connection = Connec;

                daOracle = new OracleDataAdapter();
                daOracle.SelectCommand = cmdExecuteSQL;
                daOracle.Fill(dsData);
                DT = dsData.Tables[0];

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmdExecuteSQL.Dispose();
                daOracle.Dispose();
                // CloseConnection(conOraEDER, out conOraEDER);

            }
            return DT;
        }

        /// <summary>
        /// Wrapper to read a Property from IPropertySet.
        /// IPropertySet.GetProperty will error out if hte property name provided is not in the list.
        /// This just wraps and catches exception and returns empty string if this happens.
        /// </summary>
        /// <param name="propSet">IPropertySet from which the Property should be read from</param>
        /// <param name="propertyName">Name of the property to read.</param>
        /// <returns>Returns the value as stored in the IPropertySet if the value is found else returns empty string</returns>
        private string ReadProperty(IPropertySet propSet, string propertyName)
        {
            string retVal = string.Empty;
            try
            {
                retVal = (propSet.GetProperty(propertyName) != DBNull.Value && propSet.GetProperty(propertyName) != null) ? propSet.GetProperty(propertyName).ToString() : string.Empty;
            }
            catch
            {
                retVal = string.Empty;
            }
            return retVal;
        }

        /// <summary>
        /// Setsup the GXDialog to show and returns an instance of it.
        /// </summary>
        /// <returns>An Instance of GxDialogClass with filter set to MMGxFilterDatabasesClass</returns>
        private IGxDialog GetGxDialog()
        {
            IGxDialog dialog = new GxDialogClass();
            dialog.ButtonCaption = "OK";
            dialog.AllowMultiSelect = false;
            dialog.ObjectFilter = new MMGxFilterDatabasesClass();
            return dialog;
        }

        /// <summary>
        /// Reads the Connection Property and sets the Server,Instance if SDE else File path if PGDB
        /// </summary>
        /// <param name="connectionProperties">IPropertySet from which the parameters should be read</param>
        private void ReadNewConnectionProperties(IPropertySet connectionProperties)
        {
            switch (ConnectionType)
            {
                case ConnectionType.SDE:
                    server = ReadProperty(connectionProperties, "SERVER");
                    instance = ReadProperty(connectionProperties, "INSTANCE");
                    personalGeoDbPath = "";
                    TogglePassword(true);
                    break;
                case ConnectionType.PersonalGDB:
                    personalGeoDbPath = ReadProperty(connectionProperties, "DATABASE");
                    server = "";
                    instance = "";
                    TogglePassword(false);
                    break;
                case ConnectionType.SQlServer:
                    break;
                default:
                    return;
            }
        }
        #endregion

        #region Registry Reading/Writing Methods
        /// <summary>
        /// Read connection information from Registry and set up the Login Form with the value
        /// </summary>
        private void ReadCurrentConnectionInformation()
        {
            try
            {
                connectionType = MMRegistrySettings.UserConnectionType;
                if (ConnectionType == ConnectionType.NotSet)
                    return;

                SetConnectionParameterFields();
                PopulateConnectionInfoLabel();
                //PopulateUserNameTextBox();
                txtUser.Text = MMRegistrySettings.UserUserName;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed reading connection information from registry", ex);
            }
        }
        /// <summary>
        /// Set the connection parameters by reading registry and setup the fields in the class.
        /// </summary>
        private void SetConnectionParameterFields()
        {
            switch (ConnectionType)
            {
                case ConnectionType.SDE:
                    server = MMRegistrySettings.UserServer;
                    instance = MMRegistrySettings.UserInstance;
                    personalGeoDbPath = string.Empty;
                    //ReadConnectionParameters_SDE();
                    TogglePassword(true);
                    break;
                case ConnectionType.PersonalGDB:
                    server = string.Empty;
                    instance = string.Empty;
                    personalGeoDbPath = MMRegistrySettings.UserDBPath;
                    //ReadConnectionParameters_PGDB();
                    TogglePassword(false);
                    break;
                default:
                    return;
            }
        }

        #endregion

        #region Update User Interface Methods

        /// <summary>
        /// Toggles the UI Control's visiblity based on passed in value
        /// </summary>
        /// <param name="enabled">Visibility to set</param>
        private void TogglePassword(bool enabled)
        {
            txtPassword.Enabled = enabled;
            lblPassword.Enabled = enabled;
        }
        /// <summary>
        /// Populates the Label with the data from the fields that defines the server and instance for SDE conneciton and database path for PGDB connections
        /// </summary>
        private void PopulateConnectionInfoLabel()
        {
            switch (ConnectionType)
            {
                case ConnectionType.SDE:
                    lblDB.Text = "Server: " + server;
                    lblDB.Text += "\nInstance: " + instance;
                    break;
                case ConnectionType.PersonalGDB:
                    lblDB.Text = "Path:\n";
                    lblDB.Text += personalGeoDbPath;
                    break;
                case ConnectionType.SQlServer:
                    break;
                default:
                    return;
            }
        }
        /// <summary>
        /// For SDE connection checks if the Instance property is set as prescribed and enables or disable the Change Passwod and Reset Password methods
        /// </summary>
        /// <returns></returns>
        private string GetDSAndSetControlOptions()
        {

            datasource = string.Empty;
            if (ConnectionType == ConnectionType.SDE)
            {
                if (Instance.Contains(":"))
                {
                    string[] dataSourceArray = Instance.Split(':');
                    datasource = dataSourceArray[dataSourceArray.Length-1].ToString();
                }
                if (string.IsNullOrEmpty(DataSource))
                {
                    lblDataSourceWarn.Visible = true;
                    btnChangePassword.Enabled = false;
                   btnResetPassword.Enabled = false;

                }
                else
                {
                    lblDataSourceWarn.Visible = false;
                    //btnChangePassword.Enabled = true;
                    //btnResetPassword.Enabled = true;
                }
            }
            else
            {
                //btnChangePassword.Enabled = false;
                //btnResetPassword.Enabled = false;
            }
            return DataSource;
        }
        #endregion

        #region IMMChangeDefaultVersion Members

        void IMMChangeDefaultVersion.ChangeDefaultVersion(IVersion pVersion)
        {
            _defaultLogin.ChangeDefaultVersion(pVersion);
        }

        #endregion

        #region IMMLoginObject Members

        string IMMLoginObject.GetFullTableName(string bstrBaseTableName)
        {
            return _defaultLogin.GetFullTableName(bstrBaseTableName);
        }

        bool IMMLoginObject.IsValidLogin
        {
            get
            { return _defaultLogin.IsValidLogin; }
        }

        bool IMMLoginObject.Login(bool vbInitialLogin)
        {
            _initialLogin = vbInitialLogin;

            ArcGISRuntimeEnvironment rte = new ArcGISRuntimeEnvironment();
            IntPtr hwnd = new IntPtr(rte.hWnd);
            _defaultLogin.ShowDialog = false;
            this.ShowDialog(new WindowWrapper(hwnd));
            return _loginChanged;
        }

        IWorkspace IMMLoginObject.LoginWorkspace
        {
            get { return _defaultLogin.LoginWorkspace; }
        }

        string IMMLoginObject.UserName
        {
            get { return _defaultLogin.UserName; }
        }

        #endregion

        #region User Notification Methods
        /// <summary>
        /// Informs user about ArcFM functionality not available when Login is cancelled.
        /// </summary>
        /// <param name="vbInitialLogin"></param>
        private void InformUserNoArcFM(bool vbInitialLogin)
        {
            if (!vbInitialLogin)
                return;

            string cancelMessage = "WARNING! Canceling from the login dialog will disable the ArcFM \n";
            cancelMessage += "Favorites, Page Templates, Stored Displays, Documents, and Map Production tools. \n \n";
            cancelMessage += "Cancel Login?";
            
            DialogResult dlgResult=MessageBox.Show(cancelMessage, "Login to ArcFM", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            if (dlgResult == DialogResult.No)
            {
                DialogResult = DialogResult.None;
            }
        }

        /// <summary>
        /// Shows a messagebox to user and gets the information if the user wants to change password
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private DialogResult InformUserChangePassword(string result)
        {
            string changePasswordMessage = "";
            DateTime Expirydate ;
            int DaysLeftToExpire = 0;
          
            string[] results = result.Split(';');
            if (! string.IsNullOrEmpty(results[1]))
            {
                Expirydate = Convert.ToDateTime(results[1]);
                DaysLeftToExpire = (Expirydate - DateTime.Now).Days;

            }
          
            result = results[0];


            switch (result)
            {
                case "EXPIRED":
                    changePasswordMessage = "your password has expired. \n" + "Please contact your Database Administrator to reset your password.";
                    break;

                case "EXPIRED(GRACE)":
                    if (DaysLeftToExpire == 0)
                    {
                        changePasswordMessage = "Your Password is going to expire today.\n" + "Please contact your Database Administrator to reset your password..";
                    }
                    else
                    {
                        changePasswordMessage = "Your Password is going to expire in " + DaysLeftToExpire.ToString() + " days.\n" + "Please contact your Database Administrator to reset your password.";
                    }
                    break;

                case "EXPIRED & LOCKED":
                    changePasswordMessage = "Your Password is Expired and Locked.\n" + " Please contact your Database Administrator to reset your password.\n";
                    break;

                case "LOCKED":
                    changePasswordMessage = "Your Password is Locked.\n" + "Please contact your Database Administrator to reset your password.\n";
                    break;

                default:
                    changePasswordMessage = "Unknown Status for this Account.\n" + "Please contact your Database Administrator to reset your password.\n";
                    break;
            }

            return MessageBox.Show(changePasswordMessage, "Reset Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Form Events

        private void LoginForm_Load(object sender, System.EventArgs e)
        {
            btnChangePassword.Enabled = false;
            btnResetPassword.Enabled = false;
            btnChangePassword.Visible = false;
            btnResetPassword.Visible = false;
            this.Focus();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            
            try
            {
                if (string.IsNullOrEmpty(lblDB.Text))
                {
                    MessageBox.Show("Please enter a valid Conection.");
                    return;
                }
                if (string.IsNullOrEmpty(txtUser.Text))
                {
                    MessageBox.Show("Please enter a valid User Name.");
                    return;
                }

                if (!string.IsNullOrEmpty(PersonalGeoDbPath))
                {
                    MessageBox.Show("Please select an SDE connection.");
                    return;
                }

                ChangePasswordForm chgPassword = new ChangePasswordForm(Password, Server, DataSource, UserName);
                chgPassword.ShowDialog();
                if (chgPassword.ChangePasswordResult)
                {
                    //Password = chgPassword.ChangedPassword;
                    IPropertySet propertySet = new PropertySetClass();
                    switch (connectionType)
                    {
                        case ConnectionType.PersonalGDB:
                            propertySet.SetProperty("DATABASE", personalGeoDbPath);
                            break;

                        case ConnectionType.SDE:
                            //Set server, instance, database
                            propertySet.SetProperty("SERVER", Server);
                            propertySet.SetProperty("INSTANCE", Instance);
                            //Set user and password
                            propertySet.SetProperty("USER", UserName);
                            propertySet.SetProperty("PASSWORD", chgPassword.ChangedPassword);
                            break;

                        default:
                            return;
                    }

                    if (!_initialLogin)
                    {
                        MessageBox.Show("Please close and reopen the Arcmap Session for your changes to be effective.");
                    }
                    else
                    {
                        _defaultLogin.SetConnectionProperties(propertySet);
                        _defaultLogin.ShowDialog = false;
                        _loginChanged = _defaultLogin.Login(_initialLogin);
                        if (_loginChanged)
                        {
                            DialogResult = DialogResult.OK;
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.ToString());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            InformUserNoArcFM(_initialLogin);
            
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                string usrName = txtUser.Text;
                if (!string.IsNullOrEmpty(usrName))
                {
                    CreateNewPasswordAndMail(usrName.ToUpper(), server);
                    if (!_initialLogin)
                    {
                        MessageBox.Show("Please close and reopen the ArcMap Session for your changes to be effective.", "Change Password");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid UserName", "Reset Password");
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.Message);
            }
        }

        private void txtPassword_Enter(object sender, EventArgs e)
        {
            
        }

        private void txtPassword_Leave(object sender, EventArgs e)
        {

        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            try
            {
                btnChangePassword.Enabled = false;
                btnResetPassword.Enabled = false;
                btnChangePassword.Visible = false;
                btnResetPassword.Visible = false;
                IGxDialog dialog = GetGxDialog();
                dialog.Title = "Select Default Workspace";
                IEnumGxObject gxObjects;
                dialog.DoModalOpen(this.Handle.ToInt32(), out gxObjects);

                IGxDatabase gxDatabase = gxObjects.Next() as IGxDatabase;
                if (gxDatabase == null)
                    return;

                connectionType = gxDatabase.IsRemoteDatabase ? ConnectionType.SDE : ConnectionType.PersonalGDB;
                _propSet = gxDatabase.WorkspaceName.ConnectionProperties;
                ReadNewConnectionProperties(_propSet);
                PopulateConnectionInfoLabel();
                GetDSAndSetControlOptions();
            }
            catch (Exception ex)
            {
                _logger.Error("failed to load information from Connection file", ex);
            }
        }

        [Obsolete]
        private void btnOK_Click(object sender, System.EventArgs e)
        {
            try
            {
                // hourglass cursor
                Cursor = Cursors.WaitCursor;


                if (String.IsNullOrEmpty(txtUser.Text.ToString()) || (String.IsNullOrEmpty(txtPassword.Text.ToString()) && connectionType==ConnectionType.SDE))
                {
                    MessageBox.Show("Unable to login. Please enter a user name and password", "Login to ArcFM", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.None;
                    return;
                }


                //OpenKey();
                MMRegistrySettings.UserConnectionType = ConnectionType;
                MMRegistrySettings.UserUserName = UserName;
                MMRegistrySettings.UserServer = Server;
                # region yxa6-Jeera Story- EDGISREARCH-1047-get the instance from registry

                if (MMRegistrySettings.StoredDisplayName.ToUpper().Contains("SUBSTATION"))
                {
                    instance = "sde:oracle11g:EDERSUB";
                }

                if (!instance.ToString().ToUpper().Contains("_"))
                {
                    instance = instance + "_" + MMRegistrySettings.Environment;  
                }
                #endregion yxa6-Jeera Story- EDGISREARCH-1047-get the instance from registry
                MMRegistrySettings.UserInstance = Instance;
                MMRegistrySettings.UserDBPath = PersonalGeoDbPath;

                //Setup PropertySet for connecitng to workspace.
                IPropertySet propertySet = new PropertySetClass();
                string userName = txtUser.Text.Trim();
                string password = txtPassword.Text.Trim();

                switch (connectionType)
                {
                    case ConnectionType.PersonalGDB:
                        propertySet.SetProperty("DATABASE", personalGeoDbPath);
                        break;

                    case ConnectionType.SDE:
                        //Set server, instance, database
                        propertySet.SetProperty("SERVER", server);
                        propertySet.SetProperty("INSTANCE", instance);
                        //Set user and password
                        propertySet.SetProperty("USER", userName);
                        propertySet.SetProperty("PASSWORD", password);
                        break;
                    default:
                        return;
                }

                if (connectionType == ConnectionType.SDE && !string.IsNullOrEmpty(datasource))
                {
                    string result = CheckPasswordExpiry();
                    if (result != "OPEN")
                    {
                        if (InformUserChangePassword(result) == DialogResult.Yes)
                        {
                            DialogResult = DialogResult.None;
                            //Logic to Create a New Password and Mail it to the USER.
                            if (result.Contains("EXPIRED(GRACE)"))
                            {
                               // ChangePasswordForm chgPassword = new ChangePasswordForm(Password, Server, DataSource, UserName);
                               // chgPassword.ShowDialog();
                                //if (chgPassword.ChangePasswordResult)
                                //{
                                //    propertySet.SetProperty("PASSWORD", chgPassword.ChangedPassword);
                                //}
                            }
                            else
                            {
                               // CreateNewPasswordAndMail(userName.ToUpper(), server);
                            }
                            return;
                        }
                        else
                        {
                            if (result.Contains("EXPIRED(GRACE)"))
                            {
                                DialogResult = DialogResult.OK;
                            }
                            else
                            {
                                DialogResult = DialogResult.None;
                                return;
                            }
                        }
                    }
                }

                _defaultLogin.SetConnectionProperties(propertySet);
                _defaultLogin.ShowDialog = false;
                _loginChanged = _defaultLogin.Login(_initialLogin);

                if (_loginChanged)
                {
                    this.Close();
                }
                else
                {
                    DialogResult = DialogResult.None;
                }
            }
            catch (OracleException exc)
            {
                DialogResult = DialogResult.None;
                MessageBox.Show(exc.Message.ToString());
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region IMMADOConnection
        public ADODB.Connection Connection
        {
            get
            {
                return GetConnection();
            }
        }
        #endregion

        #region Session Manager Login Methods
        private void btnSessionManager_Click(object sender, EventArgs e)
        {
            SessionManager smForm = new SessionManager();
            smForm.ShowDialog();
            _enableAutoLogin = smForm.AutoLogin;
            _provider = smForm.Provider;
            _smdatasource = smForm.DataSource;
            _server = smForm.Server;
            smForm.Close();
        }

        /// <summary>
        /// Gets the Process Framework Connection 
        /// </summary>
        /// <returns>ADODB.Connection null if not logged into Process framework</returns>
        public ADODB.Connection GetConnection()
        {
            try
            {
                if (MMRegistrySettings.ShowSessionManager)
                {
                    if (_enableAutoLogin)
                    {
                        ADODB.Connection conn = new ConnectionClass();
                        string connstring = string.Empty;
                        if (_provider == "OraOLEDB.Oracle")
                        {
                            connstring = "Provider=OraOLEDB.Oracle.1;Password=" + Password + ";Persist Security Info=False;User ID=" + UserName + " ;Data Source=" + _smdatasource;
                        }
                        else if (_provider == "Microsoft.Jet.OLEDB.4.0")
                        {
                            connstring = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + _smdatasource + "; User Id=admin; Password=;";
                        }
                        else
                        {
                            return null;
                        }
                        object optionalParam = Type.Missing;
                        conn.Open(connstring, UserName, Password, 0);
                        return conn;
                    }
                    return null;
                }
                else
                {
                    //ConnectionType SDE is considered an Oracle SDE Connection.
                    if (connectionType == ConnectionType.SDE && !string.IsNullOrEmpty(datasource))
                    {
                        //Getting DataSource similar to how it is got in CheckPassword to keep it consistent
                        string connString = "Provider=OraOLEDB.Oracle.1;Password=" + Password + ";Persist Security Info=False;User ID=" + UserName + " ;Data Source=" + MMRegistrySettings.DataSource;
                        ADODB.Connection pConn = new ADODB.ConnectionClass();
                        object optionalParam = Type.Missing;
                        pConn.Open(connString, UserName, Password, 0);
                        return pConn;
                    }
                    //Assuming the Process Schema is in the Same Access GDB
                    else if (connectionType == ConnectionType.PersonalGDB)
                    {
                        string connString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + personalGeoDbPath + "; User Id=admin; Password=;";
                        ADODB.Connection pConn = new ADODB.ConnectionClass();
                        object optionalParam = Type.Missing;
                        pConn.Open(connString, UserName, Password, 0);
                        return pConn;
                    }
                    //Write more code to handle SQL Server or Not set when needed.
                    return null;
                }
            }
            catch
            {
            }
            return null;
        }


        #endregion

       
    }
}
