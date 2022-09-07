using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using PGE.Common.Delivery.Framework;
using Miner;

namespace PGE.Desktop.EDER.Login
{
    /// <summary>
    /// Class used to read Registry information for the ArcFM custom Login and Session Manager Logins
    /// Reads Registry information stored under User Registry Setting and Local Machine Registry.
    /// HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection is hte path for User Settings
    /// HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin for System settings
    /// HKCU\Software\Miner and Miner\Process Framework\Database Properties
    /// </summary>
    public class MMRegistrySettings
    {
        private static IMMRegistry _reg = new MMRegistry();

        #region User Level Connection Registry setting
        /// <summary>
        /// ConnectionType string value stored under HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection
        /// </summary>
        public static ConnectionType UserConnectionType
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                return (ConnectionType)Enum.ToObject(typeof(ConnectionType), ReadRegistry<int>("ConnectionType", 0)); 
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                _reg.Write("ConnectionType", (int)value);
               // _reg.Write("Server", value);
            }
        }
        /// <summary>
        /// Server string value stored under HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection
        /// </summary>
        public static string UserServer
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");

                // M4JF
                string server= ReadRegistry<string>("Server", string.Empty);
                if (server.IsNullOrEmpty())
                {
                    return (System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Contains("SUB") ? "EDERSUB" : "EDER");
                }
                // m4jf
                //return (System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Contains("SUB") ? "EDERSUB" : "EDER");

                return server;
               //return ReadRegistry<string>("Server", string.Empty);
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                _reg.Write("Server", value);
            }
        }
        

        /// <summary>
        /// Instance string value stored under HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection
        /// </summary>
        public static string UserInstance
        {
            get
             {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                //return (System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName.Contains("SUB") ? "sde:oracle11g:EDERSUB" : "sde:oracle11g:EDER"); 

                // m4jf 
                // string instance = ReadRegistry<string>("Instance", string.Empty);
                string server = MMRegistrySettings.UserServer;
                if (!server.Contains("sde: oracle11g:"))
                {
                    server = "sde:oracle11g:" + MMRegistrySettings.UserServer;
                }
                Microsoft.Win32.RegistryKey key;
                // key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Computer\\HKEY_CURRENT_USER\\Software\\Miner and Miner\\ArcFM8\\DefaultConnection");
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\Miner and Miner\\ArcFM8\\DefaultConnection");


                key.SetValue("Instance", server);
                key.Close();
                string instance = ReadRegistry<string>("Instance", string.Empty);
                
               
                // return ReadRegistry<string>("Instance", string.Empty);
                if (instance.IsNullOrEmpty())
                {
                  
                    _reg.Write("Instance", server);
                    instance = ReadRegistry<string>("Instance", string.Empty);
                }
                return instance;
            }
            set
            { 
                #region yxa6-Jeera Story- EDGISREARCH-1047-get the instance from registry
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                if (MMRegistrySettings.StoredDisplayName.ToUpper().Contains("SUBSTATION"))
                {
                    _reg.Write("Instance", "sde:oracle11g:EDERSUB");
                }
                else
                {
                   
                    _reg.Write("Instance", value);
                }


                #endregion yxa6-Jeera Story- EDGISREARCH-1047-get the instance from registry

            }
        }
        //YXA6 -JeeraID EDGISREARC-1218
        //Open WEBR from EDER where URL is configured in PGEDATA.PGE_EDERCONFIG Config table
        /// <summary>
        /// Get WEBR URL from Registry.
        /// </summary>
        public static string WEBR_URL
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ARCFMLOGIN");
                return ReadRegistry<string>("WEBR_URL", string.Empty);
                //return ReadRegistry<string>("Instance", string.Empty);
            }

        }
    /// <summary>
    /// DatabasePath string value stored under HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection
    /// </summary>
    public static string UserDBPath
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                return ReadRegistry<string>("DatabasePath", string.Empty);
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                _reg.Write("DatabasePath", value);
            }
        }
        /// <summary>
        /// User string value stored under HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection
        /// </summary>
        public static string UserUserName
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                return ReadRegistry<string>("User", string.Empty);
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                _reg.Write("User", value);
            }
        }
        /// <summary>
        /// Database string value stored under HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection
        /// </summary>
        public static string UserDatabase
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                return ReadRegistry<string>("Database", string.Empty);
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                _reg.Write("Database", value);
            }
        }

        /// <summary>
        /// Stored Display Name value stored under HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection
        /// </summary>
        public static string StoredDisplayName
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection\\PGEEDER");
                return ReadRegistry<string>("LaunchSD", string.Empty);
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "DefaultConnection");
                _reg.Write("Database", value);
            }


        }
        ///// <summary>
        ///// Stored Display Name value stored under HKCU\Software\Miner and Miner\ArcFM8\DefaultConnection
        ///// </summary>
        //public static void DeleteStoredDisplayName(string skeyvalue)
        //{

        //    using (RegistryKey key = Registry.CurrentUser.OpenSubKey("DefaultConnection\\PGEEDER", true))
        //    {
        //        if (key == null)
        //        {
        //            // Key doesn't exist. Do whatever you want to handle
        //            // this case
        //        }
        //        else
        //        {
        //            key.DeleteValue(skeyvalue);
        //        }

        //    }
        //}
        #endregion

        #region System Level 
        #region Connection Registry Setting
        /// <summary>
        /// ArcFMLoginSuperUserName string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// </summary>
        public static string DBAdminUser
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                string retVal = ReadRegistry<string>("ArcFMLoginSuperUserName", string.Empty);
                if (!string.IsNullOrEmpty(retVal))
                    retVal = PGE.Common.Delivery.Framework.EncryptionFacade.Decrypt(retVal);
                return retVal;
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("ArcFMLoginSuperUserName", EncryptionFacade.Encrypt(value));
            }
        }
        /// <summary>
        /// ArcFMLoginSuperUserPassword string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// </summary>
        public static string DBAdminPassword
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                string retVal = ReadRegistry<string>("ArcFMLoginSuperUserPassword", string.Empty);
                if (!string.IsNullOrEmpty(retVal))
                    retVal = PGE.Common.Delivery.Framework.EncryptionFacade.Decrypt(retVal);
                return retVal;
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("ArcFMLoginSuperUserPassword", EncryptionFacade.Encrypt(value));
            }
        }
        /// <summary>
        /// ArcFMShowSMLogin string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// Converts the string to a boolean and returns the result
        /// </summary>
        public static bool ShowSessionManager
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                return Convert.ToBoolean(ReadRegistry<string>("ArcFMShowSMLogin", "false"));
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("ArcFMShowSMLogin", value.ToString());
            }
        }
        #endregion
        #region Mail Server
        /// <summary>
        /// SMTPServer string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// </summary>
        public static string SMTPServer
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                string retVal = ReadRegistry<string>("SMTPServer", string.Empty);
                if (!string.IsNullOrEmpty(retVal))
                    retVal = PGE.Common.Delivery.Framework.EncryptionFacade.Decrypt(retVal);
                return retVal;
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("SMTPServer", EncryptionFacade.Encrypt(value));
            }
        }
        /// <summary>
        /// MailFromUser string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// </summary>
        public static string MailFromUser
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                string retVal = ReadRegistry<string>("MailFromUser", string.Empty);
                if (!string.IsNullOrEmpty(retVal))
                    retVal = PGE.Common.Delivery.Framework.EncryptionFacade.Decrypt(retVal);
                return retVal;
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("MailFromUser", EncryptionFacade.Encrypt(value));
            }
        }
        /// <summary>
        /// EmailDomain string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// </summary>
        public static string EmailDomain
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                string retVal = ReadRegistry<string>("EmailDomain", string.Empty);
                if (!string.IsNullOrEmpty(retVal))
                    retVal = PGE.Common.Delivery.Framework.EncryptionFacade.Decrypt(retVal);
                return retVal;
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("EmailDomain", EncryptionFacade.Encrypt(value));
            }
        }
        /// <summary>
        /// m4jf edgisrearch 919
        /// Environment string value stored under HKLM\Software\Miner and Miner\ArcFM8\ArcFMLogin
        /// </summary>
        /// 

        public static string Environment
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                string retVal = ReadRegistry<string>("Environment", string.Empty);
                if (!string.IsNullOrEmpty(retVal))
                    retVal = PGE.Common.Delivery.Framework.EncryptionFacade.Decrypt(retVal);
                return retVal;
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "ArcFMLogin");
                _reg.Write("Environment", EncryptionFacade.Encrypt(value));
            }
        }

        #endregion
        #endregion

        #region SessionManager Registry Setting
        /// <summary>
        /// EnableAutoLogin string value stored under HKCU\Software\Miner and Miner\Process Framework\Database Properties
        /// </summary>
        public static bool EnableAutoLogin
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmProcessMgr, "Database Properties");
                return ReadRegistry<bool>("EnableAutoLogin", false);
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmProcessMgr, "Database Properties");
                _reg.Write("EnableAutoLogin", value);
            }
        }
        /// <summary>
        /// Provider string value stored under HKCU\Software\Miner and Miner\Process Framework\Database Properties
        /// </summary>
        public static string Provider
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmProcessMgr, "Database Properties");
                return ReadRegistry<string>("Provider", string.Empty);
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmProcessMgr, "Database Properties");
                _reg.Write("Provider", value);
            }
        
        }
       
        /// <summary>
        /// Datasource string value stored under HKCU\Software\Miner and Miner\Process Framework\Database Properties\ + Value from Provider member
        /// </summary>
        public static string DataSource
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmProcessMgr, "Database Properties\\" + Provider);
                return ReadRegistry<string>("DataSource", string.Empty);
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmProcessMgr, "Database Properties\\" + Provider);
                _reg.Write("DataSource", value);
            }
        }
        /// <summary>
        /// Server string value stored under HKCU\Software\Miner and Miner\Process Framework\Database Properties\ + Value from Provider member
        /// </summary>
        public static string SMServer
        {
            get
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmProcessMgr, "Database Properties\\" + Provider);
                return ReadRegistry<string>("Server", string.Empty);
            }
            set
            {
                _reg.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmProcessMgr, "Database Properties\\" + Provider);
                _reg.Write("Server", value);
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Given a Name of the Registry key and type to return will read he registry and return the value stored in teh registry.
        /// If the registry key is not found will return the Default value for the Type value sent
        /// </summary>
        /// <typeparam name="T">Type to return the value in</typeparam>
        /// <param name="name">Name of the Registry</param>
        /// <param name="defaultValue">Default value to pass if the value is not found in the registry</param>
        /// <returns></returns>
        private static T ReadRegistry<T>(string name,T defaultValue)
        {
            T retVal = default(T);
            retVal = _reg.Read(name, defaultValue) != null ? (T)_reg.Read(name,defaultValue) : defaultValue; 
            return retVal;
        }
        #endregion
    }
}
