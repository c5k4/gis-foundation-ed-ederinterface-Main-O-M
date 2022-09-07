using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using PGE_DBPasswordManagement;


namespace PGE.Interface.PNodeMDSSSync
{
    
    public static class ConfigSettings
    {
        /// <summary>
        /// Feb-2019, ETAR Project - Moving MDSS data from MDSS to GIS
        /// Initializes all of the settings as specified in the application configuration file.
        /// </summary>
        
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
       
        /// <summary>
        /// Initializes all of the settings as specified in the application configuration file.
        /// </summary>
        public static void InitializeSettings()
        {
            try
            {
                //configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                //ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                //configFileMap.ExeConfigFilename = configFile;
                //Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                //AppSettingsSection appSettings = (AppSettingsSection)config.GetSection("appSettings");

                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    Console.WriteLine("AppSettings is empty.");
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        Console.WriteLine("Key: {0} Value: {1}", key, appSettings[key]);
                    }
                }
                //GISOraclehost = GetConfigSettings(appSettings, "GISOraclehost");
                //GISPort = Convert.ToInt32(GetConfigSettings(appSettings, "GISPort"));
                //GISSid = GetConfigSettings(appSettings, "GISSid");

                string[] UserInst = GetConfigSettings(appSettings, "GISServiceName").Split('@');
                //GISService = GetConfigSettings(appSettings, "GISServiceName");
                //GISUserName = GetConfigSettings(appSettings, "GISUserName");
                //GISPass = GetConfigSettings(appSettings, "GISPass");

                GISService = UserInst[1].ToString();
                GISUserName = UserInst[0].ToString();
                GISPass = ReadEncryption.GetPassword(GetConfigSettings(appSettings, "GISServiceName"));

                //MDSSOraclehost = GetConfigSettings(appSettings, "MDSSOraclehost");
                //MDSSPort = Convert.ToInt32(GetConfigSettings(appSettings, "MDSSPort"));
                //MDSSSid = GetConfigSettings(appSettings, "MDSSSid");

                // m4jf edgisrearch 919

                //Changes done for Removing MDSS conenction and objects : Start
                //string[] MDSSSer = GetConfigSettings(appSettings, "MDSSService").Split('@');
                ////MDSSService = GetConfigSettings(appSettings, "MDSSServiceName");
                ////MDSSUserName = GetConfigSettings(appSettings, "MDSSUserName");
                ////MDSSPass = GetConfigSettings(appSettings, "MDSSPass");

                //MDSSService = MDSSSer[1].ToString();
                //MDSSUserName = MDSSSer[0].ToString();
                //MDSSPass = ReadEncryption.GetPassword(GetConfigSettings(appSettings, "MDSSService"));
                //Changes done for Removing MDSS conenction and objects : Start

                //Get location of SDE connectin file.
                // m4jf edgisrearch 919
                ModFunctions.SdeConnString = ReadEncryption.GetSDEPath(GetConfigSettings(appSettings, "EDERSUB_SDEConnection"));

                //if(string.IsNullOrEmpty(GISOraclehost))
                //    throw new Exception("Failed to get value for GISOraclehost.");
                //if (GISPort<1)
                //    throw new Exception("Failed to get value for GISPort.");
                //if (string.IsNullOrEmpty(GISSid))
                //    throw new Exception("Failed to get value for GISSid.");

                if (string.IsNullOrEmpty(GISService))
                    throw new Exception("Failed to get value for GISServiceName.");
                if (string.IsNullOrEmpty(GISUserName))
                    throw new Exception("Failed to get value for GISUserName.");
                if (string.IsNullOrEmpty(GISPass))
                    throw new Exception("Failed to get value for GISPass.");

                //if (string.IsNullOrEmpty(MDSSOraclehost))
                //    throw new Exception("Failed to get value for MDSSOraclehost.");
                //if (MDSSPort < 1)
                //    throw new Exception("Failed to get value for MDSSPort.");
                //if (string.IsNullOrEmpty(MDSSSid))
                //    throw new Exception("Failed to get value for MDSSSid.");

                //Changes done for Removing MDSS conenction and objects : Start
                //if (string.IsNullOrEmpty(MDSSService))
                //    throw new Exception("Failed to get value for MDSSServiceName.");
                //if (string.IsNullOrEmpty(MDSSUserName))
                //    throw new Exception("Failed to get value for MDSSUserName.");
                //if (string.IsNullOrEmpty(MDSSPass))
                //    throw new Exception("Failed to get value for MDSSPass.");
                //Changes done for Removing MDSS conenction and objects : End

                if (string.IsNullOrEmpty(ModFunctions.SdeConnString))
                    throw new Exception("Failed to get value for SDE-Connection file.");
            }
            catch (Exception ex)
            { PNodeSyncProcess.InitializeProcess.ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
            
        }
        
        /// <summary>
        /// Read the value from the application-setting for the passed key, return back the value.
        /// </summary>
        /// <param name="appSettings">The application setting.</param>
        /// <param name="key">The Name of the key.</param>
        /// <returns>Return value of the passed key if found, else return null</returns>
        private static string GetConfigSettings(AppSettingsSection appSettings, string key)
        {
            string returnValue = null;
            try
            {
                returnValue = appSettings.Settings[key].Value.ToString();
                
            }
            catch { }
            return returnValue;
        }

        /// <summary>
        /// Read the value from the application-setting for the passed key, return back the value.
        /// </summary>
        /// <param name="appSettings">The application settings as NameValueCollection.</param>
        /// <param name="key">The Name of the key.</param>
        /// <returns>Return value of the passed key if found, else return null</returns>
        static string GetConfigSettings(System.Collections.Specialized.NameValueCollection appSettings, string key)
        {
            string result = null;
            try
            {
                //var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key];// ?? "Not Found";
                Console.WriteLine(result);
            }
            catch (ConfigurationErrorsException)
            {}
            return result;
        }

        #region Public Properties        

        //public static string GISOraclehost { get; set; }
        //public static int GISPort { get; set; }
        //public static string GISSid { get; set; }
        // m4jf edgisrearch 919
        public static string GISService;
        public static string GISUserName;
        public static string GISPass;

        //public static string MDSSOraclehost { get; set; }
        //public static int MDSSPort { get; set; }
        //public static string MDSSSid { get; set; }

        //Changes done for Removing MDSS conenction and objects : Start
        //public static string MDSSService { get; set; }
        //public static string MDSSUserName { get; set; }
        //public static string MDSSPass { get; set; }
        //Changes done for Removing MDSS conenction and objects : End

        #endregion
    }

}
