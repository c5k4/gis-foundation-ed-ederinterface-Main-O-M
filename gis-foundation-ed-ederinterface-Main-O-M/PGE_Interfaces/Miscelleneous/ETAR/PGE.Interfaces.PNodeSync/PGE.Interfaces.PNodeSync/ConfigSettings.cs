using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using PGE_DBPasswordManagement;


namespace PGE.Interface.PNodeSync
{
    
    public static class ConfigSettings
    {
        /// <summary>
        /// Feb-2019, ETAR Project - Moving FNM data from MDR to GIS
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

                // m4jf edgisrearch 919

                string[] Userinst = GetConfigSettings(appSettings, "GISServiceName").Split('@');
                //GISService = GetConfigSettings(appSettings, "GISServiceName");
                //GISUserName = GetConfigSettings(appSettings, "GISUserName");
                //GISPass = GetConfigSettings(appSettings, "GISPass");
                GISService = Userinst[1].ToString();
                GISUserName = Userinst[0].ToString();
                GISPass = ReadEncryption.GetPassword(GetConfigSettings(appSettings, "GISServiceName"));

                //MDROraclehost = GetConfigSettings(appSettings, "MDROraclehost");
                //MDRPort = Convert.ToInt32(GetConfigSettings(appSettings, "MDRPort"));
                //MDRSid = GetConfigSettings(appSettings, "MDRSid");

                // m4jf edgisrearch 919
                //MDRService = GetConfigSettings(appSettings, "MDRServiceName");
                //MDRUserName = GetConfigSettings(appSettings, "MDRUserName");
                //MDRPass = GetConfigSettings(appSettings, "MDRPass");
                string[] serviceInst = GetConfigSettings(appSettings, "MDRService").Split('@');
                MDRService = serviceInst[1].ToString();
                MDRUserName = serviceInst[0].ToString();
                MDRPass = ReadEncryption.GetPassword(GetConfigSettings(appSettings, "MDRService"));

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

                //if (string.IsNullOrEmpty(MDROraclehost))
                //    throw new Exception("Failed to get value for MDROraclehost.");
                //if (MDRPort < 1)
                //    throw new Exception("Failed to get value for MDRPort.");
                //if (string.IsNullOrEmpty(MDRSid))
                //    throw new Exception("Failed to get value for MDRSid.");

                if (string.IsNullOrEmpty(MDRService))
                    throw new Exception("Failed to get value for MDRServiceName.");
                if (string.IsNullOrEmpty(MDRUserName))
                    throw new Exception("Failed to get value for MDRUserName.");
                if (string.IsNullOrEmpty(MDRPass))
                    throw new Exception("Failed to get value for MDRPass.");
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
        // M4JF EDGISREARCH 919
        //public static string GISService { get; set; }
        //public static string GISUserName { get; set; }
        //public static string GISPass { get; set; }
        public static string GISService;
        public static string GISUserName;
        public static string GISPass;
        //public static string MDROraclehost { get; set; }
        //public static int MDRPort { get; set; }
        //public static string MDRSid { get; set; }
        public static string MDRService { get; set; }
        public static string MDRUserName { get; set; }
        public static string MDRPass { get; set; }
        #endregion
    }

}
