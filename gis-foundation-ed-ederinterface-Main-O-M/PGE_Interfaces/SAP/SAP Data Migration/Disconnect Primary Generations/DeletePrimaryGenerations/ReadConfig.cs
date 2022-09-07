using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace DeletePrimaryGenerations
{
    class ReadConfigurations
    {
        #region Global Variables

        public static string EDWorkSpaceConnString;

        public static string ServiceLocationClassName;
        public static string PrimaryGenerationClassName;

        public static string ServiceLocationUpdatesVersionName;
        public static string TargetVersionName;
        public static string DefaultVersionName;

        public static string ChangesLoggedTableName;

        public static string InputFilesDir;
        public static string VersionNamePrefix;

        #endregion

        public static void ReadFromConfiguration()
        {
            try
            {
                // m4jf edgisrerch 919
                //EDWorkSpaceConnString = ConfigurationManager.AppSettings["EDWorkSpaceConnString"];
                EDWorkSpaceConnString = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());

                ServiceLocationClassName = ConfigurationManager.AppSettings["ServiceLocationClassName"];
                PrimaryGenerationClassName = ConfigurationManager.AppSettings["PrimaryGenerationClassName"];
                ServiceLocationUpdatesVersionName = ConfigurationManager.AppSettings["ServiceLocationUpdatesVersionName"];
                TargetVersionName = ConfigurationManager.AppSettings["TargetVersionName"];
                DefaultVersionName = ConfigurationManager.AppSettings["DefaultVersionName"];
                ChangesLoggedTableName = ConfigurationManager.AppSettings["ChangesLoggedTableName"];

                VersionNamePrefix = ConfigurationManager.AppSettings["VersionNamePrefix"];
                // m4jf edgisrearch 919
                string[] parameters = ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper().Split('@');

                VersionOperations._log.Info("Config values .. Start..");
                VersionOperations._log.Info("EDWorkSpaceConnString : " + EDWorkSpaceConnString);
                VersionOperations._log.Info("ServiceLocationClassName : " + ServiceLocationClassName);
                VersionOperations._log.Info("PrimaryGenerationClassName : " + PrimaryGenerationClassName);
                VersionOperations._log.Info("ServiceLocationUpdatesVersionName : " + ServiceLocationUpdatesVersionName);
                VersionOperations._log.Info("TargetVersionName : " + TargetVersionName);
                VersionOperations._log.Info("DefaultVersionName : " + DefaultVersionName);
                // m4jf edgisrearch 919
                //VersionOperations._log.Info("SM DB Name : " + ConfigurationManager.AppSettings["SMDB"]);
                VersionOperations._log.Info("SM DB Name : " + parameters[1]);
                VersionOperations._log.Info("EditsPerSession : " + ConfigurationManager.AppSettings["EditsPerSession"]);
                VersionOperations._log.Info("Config values .. End..");

                VersionOperations._log.Info("Config file read successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.Message + " Occured in Reading Config File");
                VersionOperations._log.Error("Exception " + ex.Message + " Occured in Reading Config File");
            }
        }
        public static void EraseAllStatic()
        {

        }
    }
}
