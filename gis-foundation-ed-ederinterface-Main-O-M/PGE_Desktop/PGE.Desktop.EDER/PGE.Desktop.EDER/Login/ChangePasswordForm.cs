using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using Oracle.DataAccess.Client;
using PGE_DBPasswordManagement;

namespace PGE.Desktop.EDER.Login
{
    /// <summary>
    /// Form for Changing User Password
    /// </summary>
    public partial class ChangePasswordForm : Form
    {
        #region Private Members
        /// <summary>
        /// The Return Message
        /// </summary>
        private String PasswordMessage;
        #endregion

        #region Properties
        /// <summary>
        /// Private Password Field
        /// </summary>
        private string _password = string.Empty;
        /// <summary>
        /// Public Accessor for Password Field
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }
        /// <summary>
        /// Private Field for the Password Success or Failure Change Message
        /// </summary>
        private bool _changePasswordResult = false;
        /// <summary>
        /// public accessor for the Password change Message
        /// </summary>
        public bool ChangePasswordResult
        {
            get
            {
                return _changePasswordResult;
            }
            set
            {
                _changePasswordResult = value;
            }
        }
        /// <summary>
        /// Private field for the new Password
        /// </summary>
        private string _changedPassword = string.Empty;
        /// <summary>
        /// Public accessor for the changed password
        /// </summary>
        public string ChangedPassword
        {
            get
            {
                return _changedPassword;
            }
            set
            {
                _changedPassword = value;
            }
        }
        /// <summary>
        /// Private field for the Server
        /// </summary>
        private string _server = string.Empty;
        /// <summary>
        /// Public accessor for the Server property
        /// </summary>
        public string Server
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
            }
        }
        /// <summary>
        /// Private field of the Datasource
        /// </summary>
        private string _datasource = string.Empty;
        /// <summary>
        /// Public accessor for the Datasource field
        /// </summary>
        public string DataSource
        {
            get
            {
                return _datasource;
            }
            set
            {
                _datasource = value;
            }
        }

        /// <summary>
        /// Private field for the 
        /// </summary>
        private string _userName = string.Empty;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }


        #endregion

        #region CTOR
        public ChangePasswordForm(string password, string server, string datasource, string userName)
        {
            _password = password;
            _server = server;
            _datasource = datasource;
            _userName = userName;            
            InitializeComponent();
        }
        #endregion

        #region Form Control Events
        [Obsolete]
        private void btnOk_Click(object sender, EventArgs e)
        {
            PasswordMessage = "The password supplied does not meet the minimum complexity requirements. Please select another password that meets all of the following criteria:\n \n";
            PasswordMessage += "1) Password must be at least 8 characters \n";
            PasswordMessage += "2) Password cannot be reused \n";
            PasswordMessage += "3) Password should not be simple word. Such as (database,password,computer) \n";
            PasswordMessage += "4) Password should contain at least one digit, one character and one special character (!#%()*+,;<=>?_)\n";
            PasswordMessage += "5) Password should contain at least one upper case character \n";
            PasswordMessage += "6) Password should contain at least one lower case character \n";
            PasswordMessage += "7) Password should contain a number or special character at the beginning or end \n";
            PasswordMessage += "8) Password should differ by at least 3 characters from the current password \n";
           
            if (String.IsNullOrEmpty(txtBoxCurrentPassword.Text) || String.IsNullOrEmpty(txtBoxNewPassword.Text))
            {
                MessageBox.Show("Please enter valid passwords.", "Change Password");
                DialogResult = DialogResult.None;
                return;
            }
            if ((txtBoxNewPassword.Text.Contains("@")) || (txtBoxNewPassword.Text.Contains("?"))|| (txtBoxNewPassword.Text.Contains("-"))|| (txtBoxNewPassword.Text.Contains("/")) 
                || (txtBoxNewPassword.Text.Contains(":")))
            {
                MessageBox.Show("New Password should not contain special character<@?-/:>", "Change Password");
                DialogResult = DialogResult.None;
                return;
            }
            if (txtBoxNewPassword.Text != txtBoxRetypePassword.Text)
            {
                MessageBox.Show("New Passwords do not match.", "Change Password");
                DialogResult = DialogResult.None;
                return;
            }
            Password = txtBoxCurrentPassword.Text;
            //Establish a Connection and Change Password.
            //Automatic change of password for edersub or vice versa by using superuser credentials.
            try
            {

                Boolean result = ChangeDBPassword();
                if (result == true)
                {
                    string origDS = DataSource;
                    string message = string.Empty;
                    bool check1 = false;
                    bool check2 = false;
                    // m4jf edgisrearch 390 
                    bool check3 = false;
                    string strCondCheckPrimary = null;
                    string strCondCheckSecondary = null;
                    // m4jf edgisrearch 390 - datasource handling when it include environment type also
                    string Env = MMRegistrySettings.Environment;
                    string substrdatasrc = DataSource.Substring(DataSource.Length - 2);
                    if (substrdatasrc == "_" + Env)
                    {
                        DataSource = DataSource.Substring(0, DataSource.Length - 3);
                    }

                    switch (DataSource.ToUpper())
                    {
                        case "EDER":
                            #region If Login DB is EDER- Change EDERSUB,LBMAINT and Schematic
                            DataSource = "EDERSUB";
                            bool user_access = user_exists_check(UserName, DataSource);
                            message = "Password has been changed successfully for EDER";
                            if (user_access == true)
                            {
                                check1 = ChangeDBPassword();
                                //check1 = ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckPrimary);
                                if (check1 == true)
                                {
                                    message = "Password has been changed successfully for EDER & EDERSUB";
                                }
                            }
                            DataSource = "LBMAINT";
                            user_access = user_exists_check(UserName, DataSource);
                            if (user_access == true)
                            {
                                check2 = ChangeDBPassword();
                                // check2 = ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckSecondary);
                                if (check2 == true && check1 == true)
                                {
                                    message = "Password has been changed successfully for EDER, EDERSUB & LANDBASE";
                                }
                                else if (check2 == true && check1 != true)
                                {
                                    message = "Password has been changed successfully for EDER & LANDBASE";
                                }
                            }
                            // M4JF EDGISREARCH 390 - Password will also be changed for ESCHM dtabase
                            DataSource = "EDSCHM";
                            user_access = user_exists_check(UserName, DataSource);
                            if (user_access == true)
                            {
                                check3 = ChangeDBPassword();
                                // check3 = ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckSecondary);
                                if (check2 == true && check1 == true && check3 == true)
                                {
                                    message = "Password has been changed successfully for EDER, EDERSUB ,EDSCHM & LANDBASE";
                                }
                                else if (check3 == true && check1 != true && check2 != true)
                                {
                                    message = "Password has been changed successfully for EDER & EDSCHM";
                                }
                            }


                            if (strCondCheckPrimary != null & strCondCheckSecondary != null)
                            { message = "Password cannot be reused, Please choose a new password for: EDERSUB and LBMAINT, " + message; }
                            else { message = strCondCheckPrimary + strCondCheckSecondary + ", " + message; }
                            //MessageBox.Show(message);
                            break;
                        #endregion

                        case "EDERSUB":
                            #region If Login DB is EDERSUB- Change EDER,LBMAINT and Schematic
                            message = "Password has been changed successfully for EDERSUB";

                            DataSource = "EDER";
                            check1 = ChangeDBPassword();//ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckPrimary);
                            if (check1 == true)
                            {
                                message = "Password has been changed successfully for EDER & EDERSUB";
                            }

                            DataSource = "LBMAINT";
                            user_access = user_exists_check(UserName, DataSource);
                            if (user_access == true)
                            {
                                check2 = ChangeDBPassword();// ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckPrimary);
                                if (check2 == true && check1 == true)
                                {
                                    message = "Password has been changed successfully for EDER, EDERSUB & LANDBASE";
                                }
                                else if (check2 == true && check1 != true)
                                {
                                    message = "Password has been changed successfully for EDERSUB & LANDBASE";
                                }
                            }

                            // M4JF EDGISREARCH 390 - Password will also be changed for ESCHM dtabase
                            DataSource = "EDSCHM";
                            user_access = user_exists_check(UserName, DataSource);
                            if (user_access == true)
                            {

                                check3 = ChangeDBPassword();//ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckSecondary);
                                if (check2 == true && check1 == true && check3 == true)
                                {
                                    message = "Password has been changed successfully for EDER, EDERSUB ,EDSCHM & LANDBASE";
                                }
                                else if (check3 == true && check1 != true && check2 != true)
                                {
                                    message = "Password has been changed successfully for EDERSUB & EDSCHM";
                                }
                            }

                           // MessageBox.Show(message);
                            break;
                        #endregion
                        case "LBMAINT":
                            #region If Login DB is LBMAINT- Change EDER,EDERSUB and Schematic

                            message = "Password has been changed successfully for LANDBASE";
                            DataSource = "EDER";
                            check1 = ChangeDBPassword();// ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckPrimary);
                            if (check1 == true)
                            {
                                message = "Password has been changed successfully for EDER & LANDBASE";
                            }
                            DataSource = "EDERSUB";
                            user_access = user_exists_check(UserName, DataSource);
                            if (user_access == true)
                            {
                                check2 = ChangeDBPassword();// ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckSecondary);
                                if (check2 == true && check1 == true)
                                {
                                    message = "Password has been changed successfully for EDER, EDERSUB & LANDBASE";
                                }
                                else if (check2 == true && check1 != true)
                                {
                                    message = "Password has been changed successfully for EDERSUB & LANDBASE";
                                }
                            }


                            // M4JF EDGISREARCH 390 - Password will also be changed for ESCHM dtabase
                            DataSource = "EDSCHM";
                            user_access = user_exists_check(UserName, DataSource);
                            if (user_access == true)
                            {

                                check3 = ChangeDBPassword();//ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckSecondary);
                                if (check2 == true && check1 == true && check3 == true)
                                {
                                    message = "Password has been changed successfully for EDER, EDERSUB ,EDSCHM & LANDBASE";
                                }
                                else if (check3 == true && check1 != true && check2 != true)
                                {
                                    message = "Password has been changed successfully for LANDBASE & EDSCHM";
                                }
                            }
                            //MessageBox.Show(message);
                            break;
                        #endregion
                        case "EDSCHM":
                            #region If Login DB is  EDSCHM - Change EDER,EDERSUB and LBMAINT

                            message = "Password has been changed successfully for LANDBASE";
                            DataSource = "EDER";
                            check1 = ChangeDBPassword();// ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckPrimary);
                            if (check1 == true)
                            {
                                message = "Password has been changed successfully for EDER & LANDBASE";
                            }
                            DataSource = "EDERSUB";
                            user_access = user_exists_check(UserName, DataSource);
                            if (user_access == true)
                            {
                                check2 = ChangeDBPassword();//ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckPrimary);
                                if (check2 == true && check1 == true)
                                {
                                    message = "Password has been changed successfully for EDER, EDERSUB & LANDBASE";
                                }
                                else if (check2 == true && check1 != true)
                                {
                                    message = "Password has been changed successfully for EDERSUB & LANDBASE";
                                }
                            }
                            DataSource = "LBMAINT";
                            user_access = user_exists_check(UserName, DataSource);
                            if (user_access == true)
                            {

                                check3 = ChangeDBPassword();//ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckSecondary);
                                if (check2 == true && check1 == true && check3 == true)
                                {
                                    message = "Password has been changed successfully for EDER, EDERSUB ,EDSCHM & LANDBASE";
                                }
                                else if (check3 == true && check1 != true && check2 != true)
                                {
                                    message = "Password has been changed successfully for LANDBASE & EDSCHM";
                                }
                            }
                           
                            break;
                        #endregion

                        default:
                            break;
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        MessageBox.Show(message, "Change Password", MessageBoxButtons.OK,MessageBoxIcon.Information);
                        DataSource = origDS;
                        ChangePasswordResult = true;
                        ChangedPassword = txtBoxNewPassword.Text.ToString();
                        DialogResult = DialogResult.Cancel;
                    }
                   
                }
                else
                {
                    MessageBox.Show("Password not changed,Please try again.");
                }
            }
            catch (Exception exce)
            {
                MessageBox.Show(exce.Message.ToString());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
           
            DialogResult = DialogResult.Cancel;
        }

        private void txtBoxCurrentPassword_Enter(object sender, EventArgs e)
        {
        }

        private void txtBoxCurrentPassword_Leave(object sender, EventArgs e)
        {

        }

        private void txtBoxCurrentPassword_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtBoxNewPassword_Enter(object sender, EventArgs e)
        {
            
        }

        private void txtBoxNewPassword_Leave(object sender, EventArgs e)
        {
        }

        private void txtBoxNewPassword_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void txtBoxRetypePassword_Enter(object sender, EventArgs e)
        {
        }

        private void txtBoxRetypePassword_Leave(object sender, EventArgs e)
        {
        }

        private void txtBoxRetypePassword_KeyUp(object sender, KeyEventArgs e)
        {
        }
        #endregion

        #region Database Methods
        /// <summary>
        /// Changes DB Password by calling in the Alter User Command.
        /// </summary>
        /// <returns>Return the result of change password operation. True if the change password operation succeeded else false.</returns>
        public Boolean ChangeDBPassword()
        {
            try
            {
                string connectionString = string.Empty;
                //string[] dataSourceArray = DataSource.Split('=');
                //string datasource = dataSourceArray[1].ToString();               
                if (!string.IsNullOrEmpty(DataSource))
                {
                    connectionString = "provider=OraOLEDB.Oracle;Data Source=" + DataSource + ";" +
                                "User ID=" + UserName + ";" +
                                "Password=" + Password + ";";

                    OleDbConnection connection = new OleDbConnection(connectionString);
                    connection.Open();
                    string sql = "ALTER USER " + UserName + " IDENTIFIED BY \"" + txtBoxNewPassword.Text + "\"" + " REPLACE \"" + Password + "\"";

                    OleDbCommand cmd = new OleDbCommand(sql, connection);
                    cmd.ExecuteScalar();
                    cmd.Dispose();
                    connection.Close();
                    return true;
                }

                return false;
            }
            catch (OleDbException Oraex)
            {

                if (Oraex.Message.Contains("ORA-28003"))
                {

                    MessageBox.Show(PasswordMessage, "Change password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   
                }
                else if (Oraex.Message.Contains("ORA-28007"))
                {
                    MessageBox.Show(PasswordMessage, "Change password");
                   
                }
                else if (Oraex.Message.Contains("ORA-01017"))
                {
                    MessageBox.Show("Invalid username/password", "Change password");
                   
                }

                else
                {
                    string message = Oraex.Message;
                    if (message.Contains("locked"))
                    {
                        string strCondCheckPrimary= null;
                        ChangePassword_superuser(UserName, txtBoxNewPassword.Text, out strCondCheckPrimary);
                       // return true;
                    }
                    else
                    {
                        MessageBox.Show(Oraex.Message, "Change password");
                        //return false;
                    }
                   
                }
                throw Oraex;
            }
            catch (Exception exce)
            {
                MessageBox.Show(exce.Message.ToString());
                throw exce;
            }
        }

        //Automatic change of password for edersub or vice versa by using superuser credentials.
        #region new changes

        [Obsolete]
        public Boolean ChangePassword_superuser(string username, string new_password, out string strCondCheck)
        {
            int count = 0;
            strCondCheck = null;
            try
            {
                string connectionString = string.Empty;
                //string[] dataSourceArray = DataSource.Split('=');
                //string datasource = dataSourceArray[1].ToString();
                string UserName_super = MMRegistrySettings.DBAdminUser;
              
                // M4JF EDGISREARCH 390 - GETPASSWORD FOR SUPERUSER FROM CONNECTIONS.XML OF >PGE_DBPasswordManagement
                // string Password_super = MMRegistrySettings.DBAdminPassword;
                string Password_super = ReadEncryption.GetPassword(UserName_super.ToUpper() + "@" + DataSource.ToUpper());
                new_password = "\"" + new_password + "\"";
                if (!string.IsNullOrEmpty(DataSource))
                {
                    connectionString = "Data Source=" + DataSource + ";" +
                            "User ID=" + UserName_super + ";" +
                            "Password=" + Password_super + ";";

                    OracleConnection connection = new OracleConnection(connectionString);
                    connection.Open();
                    //string sql = "ALTER USER " + UserName + " IDENTIFIED BY \"" + txtBoxNewPassword.Text + "\"";
                    OracleCommand objSelectCmd = new OracleCommand();
                    objSelectCmd.Connection = connection;
                    objSelectCmd.CommandText = "SYS.USER_CHANGE_PASSWD_PKG.SP_CHANGEPASSWD";
                    objSelectCmd.CommandType = CommandType.StoredProcedure;
                    OracleParameter prmInputUserName = new OracleParameter("i_username", OracleDbType.Varchar2);
                    prmInputUserName.Direction = ParameterDirection.Input;
                    prmInputUserName.Value = username;
                    objSelectCmd.Parameters.Add(prmInputUserName);
                    OracleParameter prmInputPassword = new OracleParameter("i_passwd", OracleDbType.Varchar2);
                    prmInputPassword.Direction = ParameterDirection.Input;
                    prmInputPassword.Value = new_password;
                    objSelectCmd.Parameters.Add(prmInputPassword);

                    // M4JF EDGISREARCH 390 - COMMENTED BELOW LINE AS STOREPROCEDURE DOES NOT RETURN ANY VALUE 
                    //count = objSelectCmd.ExecuteNonQuery();
                    objSelectCmd.ExecuteNonQuery();
                    count = 1;



                    //objSelectCmd.ExecuteScalar();
                    //objSelectCmd.Dispose();
                    connection.Close();
                    if (count == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }            
            catch (Exception exce)
            {
                if (exce.Message.ToString().Contains("cannot be reused"))
                {
                    //MessageBox.Show("Password cannot be reused. Please choose a new password." + "\n" + "Password is not changed for: " + DataSource);
                    strCondCheck = "Password cannot be reused. Please choose a new password." + "\n" + "Password is not changed for: " + DataSource; 
                }
                else
                {
                    MessageBox.Show("Currently, there is a connectivity issue. Please contact System Administrator."+ "\n"+ "Password is not changed for: " + DataSource);
                }
                return false;
            }
        }

        public static Boolean user_exists_check(string UserName, string DataSource)
        {
            try
            {
                string connectionString = string.Empty;
                string UserName_super = PGE.Desktop.EDER.Login.MMRegistrySettings.DBAdminUser;

                // m4jf edgisrearch 9191
                // string Password_super = PGE.Desktop.EDER.Login.MMRegistrySettings.DBAdminPassword;
                string Password_super = ReadEncryption.GetPassword(UserName_super + "@" + DataSource);

                if (!string.IsNullOrEmpty(DataSource))
                {
                    connectionString = "provider=OraOLEDB.Oracle;Data Source=" + DataSource + ";" +
                                "User ID=" + UserName_super + ";" +
                                "Password=" + Password_super + ";";

                    OleDbConnection connection = new OleDbConnection(connectionString);
                    connection.Open();

                    string sql = "SELECT count(*) FROM ALL_USERS where UPPER(username)=UPPER(" + "'" + UserName + "'" + ")";
                    OleDbCommand cmd = new OleDbCommand(sql, connection);
                    object value_user = cmd.ExecuteScalar();
                    cmd.Dispose();
                    connection.Close();
                    if (Convert.ToInt32(value_user) >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception exce)
            {
                MessageBox.Show("Unable to connect " + DataSource + " database, " + "\n" + "Hence password cannot be changed/reset for " + DataSource);
                return false;
            }

        }
     

#endregion


        /// <summary>
        /// Creates a message to be shown to the user if the Password does not meet the PG&E standards/requirments.
        /// </summary>
        /// <returns></returns>
        public String GetOraclePasswordMessage()
        {
            String PasswordMessage = "The password supplied does not meet the minimum complexity requirements.Please select another password that meets all of the following criteria:\n \n";
            PasswordMessage += "1) Password must be at least 8 characters \n";
            PasswordMessage += "2) Password cannot be reused \n";
            PasswordMessage += "3) Password should not be simple word. such as (database,password,computer) \n";
            PasswordMessage += "4) Password should contain at least one digit, one character and one special character (!#$%&()*+,-/:;<=>?_)\n";
            PasswordMessage += "5) Password should contain at least one upper case character \n";
            PasswordMessage += "6) Password should contain at least one lower case character \n";
            PasswordMessage += "7) Password should contain a number or special character at the begining or end \n";
            PasswordMessage += "8) Password should differ by at least 3 characters form the current password \n";

            return PasswordMessage;

        }
        #endregion

    }
}
