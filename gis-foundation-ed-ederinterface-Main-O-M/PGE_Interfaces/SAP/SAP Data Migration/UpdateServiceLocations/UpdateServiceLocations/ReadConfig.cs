using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace UpdateServiceLocations
{
    class ReadConfigurations
    {
        #region Global Variables

        // m4jf edgisrearch 919
        //public const string EDConnection = "EDConnection";
        public static string EDConnection ;
        public static string EDWorkSpaceConnString;

        public static string ServiceLocationClassName;
        public static string PrimaryGenerationClassName;

        public static string ServiceLocationUpdatesVersionName;
        public static string TargetVersionName;
        public static string DefaultVersionName;

        public static string ChangesLoggedTableName;

        public static string InputFilesDir;

        public static string VersionNamePrefix;

        public static string connString;
        
        #endregion 

        public static void ReadFromConfiguration()
        {
            try
            {
                // M4JF EDGISREARCH 919 - get sde connection using PGE_DBPasswordManagement
                //EDWorkSpaceConnString = ConfigurationManager.AppSettings["EDWorkSpaceConnString"];
                EDWorkSpaceConnString = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());
                EDConnection = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());

                ServiceLocationClassName = ConfigurationManager.AppSettings["ServiceLocationClassName"];
                PrimaryGenerationClassName = ConfigurationManager.AppSettings["PrimaryGenerationClassName"];
                ServiceLocationUpdatesVersionName = ConfigurationManager.AppSettings["ServiceLocationUpdatesVersionName"];
                TargetVersionName = ConfigurationManager.AppSettings["TargetVersionName"];
                DefaultVersionName = ConfigurationManager.AppSettings["DefaultVersionName"];
                InputFilesDir = ConfigurationManager.AppSettings["InputFilesDir"];
                ChangesLoggedTableName = ConfigurationManager.AppSettings["ChangesLoggedTableName"];
                VersionNamePrefix = ConfigurationManager.AppSettings["VersionNamePrefix"];

                // m4jf edgisrearch 919 - removed connection strings from the config
                // connString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
                string [] connStringParametrs = ConfigurationManager.AppSettings["EDER_ConnectionStr"].Split('@');
                string password = ReadEncryption.GetPassword(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
                connString = string.Format("Provider=MSDAORA;Persist Secutiry Info=True;Data Source={0};PLSQLRSet = 1;User Id={1};Password={2}", connStringParametrs[1], connStringParametrs[0], password);

                VersionOperations._log.Info("Config values .. Start..");
                VersionOperations._log.Info("EDWorkSpaceConnString : "+EDWorkSpaceConnString);
                VersionOperations._log.Info("ServiceLocationClassName : " + ServiceLocationClassName);
                VersionOperations._log.Info("PrimaryGenerationClassName : " + PrimaryGenerationClassName);
                VersionOperations._log.Info("ServiceLocationUpdatesVersionName : " + ServiceLocationUpdatesVersionName);
                VersionOperations._log.Info("TargetVersionName : " + TargetVersionName);
                VersionOperations._log.Info("DefaultVersionName : " + DefaultVersionName);
                // m4jf edgisrearc 919 - sm db name is removed from config
              //  VersionOperations._log.Info("SM DB Name : " + ConfigurationManager.AppSettings["SMDB"]);
                VersionOperations._log.Info("EditsPerSession : " + ConfigurationManager.AppSettings["EditsPerSession"]);
                VersionOperations._log.Info("Config values .. End..");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.Message + " Occured in Reading Config File");
            }
        }
        public static void EraseAllStatic()
        {

        }

        public static string[] GetCommaSeparatedList(string Key, string[] Default)
        {
            string[] output = Default;
            string value = GetValue(Key);
            if (value != null)
            {
                try
                {
                    string[] temp = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    output = new string[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        output[i] = temp[i].Trim();
                    }

                }
                catch { }
            }
            return output;
        }

        public static string GetValue(string key)
        {
            string setting = null;
            try
            {
                setting = System.Configuration.ConfigurationManager.AppSettings[key];
            }
            catch { }
            return setting;
        }
    }
}
