using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using System.Configuration;
using Miner;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.FAISessionProcessor
{
    public static class Common
    {
        #region Public variables

        public static esriLicenseProductCode EsriLicense;
        public static mmLicensedProductCode ArcFMLicense;
        public static List<esriLicenseExtensionCode> EsriExtensions;

        public static string SDEConnectionFile = "";
        public static string LandbaseConnectionFile = "";
        public static string OracleConnectionString = "";
        public static string SessionManagerConnectionString = "";
        public static string SessionManagerConnectionStringNoPassword = "";
        public static string Password = "";

        //FAI updates information
        public static string InputFGDBLocation = "";
        public static string ArchiveFGDBLocation = "";
        public static string OriginaGUIDFieldName = "ORIGINAL_GLOBALID";
        public static string AddedFeaturesFCWhereClause = "";
        public static string DeletedFeaturesFCWhereClause = "";
        public static string UpdatedFeaturesFCWhereClause = "";
        public static string AddedFeaturesTablesWhereClause = "";
        public static string DeletedFeaturesTablesWhereClause = "";
        public static string UpdatedFeaturesTablesWhereClause = "";

        #endregion

        #region Private static variables

        private static IMMRegistry _reg = new MMRegistry();

        #endregion

        #region Public Static methods

        /// <summary>
        /// Reads the application settings into static vars
        /// </summary>
        public static void ReadAppSettings(string configurationFile)
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configurationFile;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            try { EsriLicense = (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), config.AppSettings.Settings["EsriLicense"].Value); }
            catch (Exception e) { EsriLicense = (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), "esriLicenseProductCodeArcInfo"); }

            try { ArcFMLicense = (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), config.AppSettings.Settings["ArcFMLicense"].Value); }
            catch (Exception e) { ArcFMLicense = (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), "mmLPArcFM"); }

            // m4jf edgisrearch 919
            //string passwordTemp = config.AppSettings.Settings["Password"].Value;
            //Password = EncryptionFacade.Decrypt(passwordTemp);
            //string tempString = config.AppSettings.Settings["ConnectionString"].Value;
            //SessionManagerConnectionStringNoPassword = tempString;

            ////Append the decripted password in to form the full connectionstring
            //SessionManagerConnectionString = ""; 
            SessionManagerConnectionString = "Provider=OraOLEDB.Oracle.1;"+ReadEncryption.GetConnectionStr(config.AppSettings.Settings["EDER_ConnectionStr"].Value.ToUpper());
            //string[] connectParams = tempString.Split(';');
            //for (int i = 0; i < connectParams.Length; i++)
            //{
            //    if (connectParams[i] != string.Empty)
            //    {
            //        if (connectParams[i].ToLower().StartsWith("password"))
            //            SessionManagerConnectionString += "password=" + Password + ";";
            //        else
            //            SessionManagerConnectionString += connectParams[i] + ";";
            //    }
            //}

            try { OracleConnectionString = SessionManagerConnectionString.Replace("Provider=OraOLEDB.Oracle.1;", ""); }
            catch { }

            try 
            { 
                // m4jf edgisrearch 919
               // SDEConnectionFile = config.AppSettings.Settings["SDEConnectionFile"].Value;
                SDEConnectionFile = ReadEncryption.GetSDEPath(config.AppSettings.Settings["EDER_SDEConnection"].Value.ToUpper());
            }
            catch (Exception ex) { throw new Exception("Failed to determine SDEConnectionFile from configuration file: " + ex.Message); }

            try 
            {
                // m4jf edgisrearch 919
               // LandbaseConnectionFile = config.AppSettings.Settings["LandbaseConnectionFile"].Value;
                LandbaseConnectionFile = ReadEncryption.GetSDEPath(config.AppSettings.Settings["LANDBASE_SDEConnection"].Value.ToUpper());
            }
            catch (Exception ex) { throw new Exception("Failed to determine LandbaseConnectionFile from configuration file: " + ex.Message); }

            try { InputFGDBLocation = config.AppSettings.Settings["FAIInputFGDBLocation"].Value; }
            catch (Exception ex) { throw new Exception("Failed to determine FAI input FGDB Location from configuration file: " + ex.Message); }

            try { ArchiveFGDBLocation = config.AppSettings.Settings["FAIArchiveFGDBLocation"].Value; }
            catch (Exception ex) { throw new Exception("Failed to determine FAI Archive FGDB Location from configuration file: " + ex.Message); }

            try { AddedFeaturesFCWhereClause = config.AppSettings.Settings["AddedFeaturesFC"].Value; }
            catch (Exception ex) { throw new Exception("Failed to determine Added features FC sql from configuration file: " + ex.Message); }

            try { DeletedFeaturesFCWhereClause = config.AppSettings.Settings["DeletedFeaturesFC"].Value; }
            catch (Exception ex) { throw new Exception("Failed to determine deleted features FC sql from configuration file: " + ex.Message); }

            try { UpdatedFeaturesFCWhereClause = config.AppSettings.Settings["UpdatedFeaturesFC"].Value; }
            catch (Exception ex) { throw new Exception("Failed to determine updated features FC sql from configuration file: " + ex.Message); }

            try { AddedFeaturesTablesWhereClause = config.AppSettings.Settings["AddedFeaturesTables"].Value; }
            catch (Exception ex) { throw new Exception("Failed to determine Added features Tables sql from configuration file: " + ex.Message); }

            try { DeletedFeaturesTablesWhereClause = config.AppSettings.Settings["DeletedFeaturesTables"].Value; }
            catch (Exception ex) { throw new Exception("Failed to determine deleted features Tables sql from configuration file: " + ex.Message); }

            try { UpdatedFeaturesTablesWhereClause = config.AppSettings.Settings["UpdatedFeaturesTables"].Value; }
            catch (Exception ex) { throw new Exception("Failed to determine updated features Tables sql from configuration file: " + ex.Message); }

            try { OriginaGUIDFieldName = config.AppSettings.Settings["OriginalGUIDField"].Value; }
            catch (Exception ex) { throw new Exception("Failed to determine Original GUID field name from configuration file: " + ex.Message); }

        }

        /// <summary>
        /// Return the feature class specified from the workspace.  Returns null if feature class does not exist
        /// </summary>
        /// <param name="workspace">IWorkspace to search</param>
        /// <param name="FeatureClassName">Name of feature class to find</param>
        /// <returns></returns>
        public static void ResetPassword(string password, string configurationFile) 
        {
            try
            {
                //Open our configuration file
                bool hasUpdate = false; 
                //string assemLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configurationFile);
                string encPwd = EncryptionFacade.Encrypt(password);
                XmlNodeList settingsList = xmlDoc.SelectNodes("//configuration/appSettings/add");
                for (int i = 0; i < settingsList.Count; i++)
                {
                    XmlNode node = settingsList[i];
                    string settingName = node.Attributes["key"].Value.ToLower();

                    if (settingName == "password")
                    {
                        node.Attributes["value"].InnerText = encPwd;
                        hasUpdate = true;
                        break; 
                    }
                }

                if (hasUpdate)
                    xmlDoc.Save(configurationFile); 

            }
            catch (Exception ex) 
            {
                throw new Exception("Error resetting password: " + ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public static List<string> GetWhereInClauses(List<string> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                int counter = 0;
                foreach (string guid in guids)
                {
                    if (counter == 999)
                    {
                        builder.Append("'" + guid + "'");
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                        counter = 0;
                    }
                    else
                    {
                        builder.Append("'" + guid + "',");
                        counter++;
                    }
                }

                if (builder.Length > 0)
                {
                    string whereInClause = builder.ToString();
                    whereInClause = whereInClause.Substring(0, whereInClause.LastIndexOf(","));
                    whereInClauses.Add(whereInClause);
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of integers
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public static List<string> GetWhereInClauses(List<int> oids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                int counter = 0;
                foreach (int oid in oids)
                {
                    if (counter == 999)
                    {
                        builder.Append(oid);
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                        counter = 0;
                    }
                    else
                    {
                        builder.Append(oid + ",");
                        counter++;
                    }
                }

                if (builder.Length > 0)
                {
                    string whereInClause = builder.ToString();
                    whereInClause = whereInClause.Substring(0, whereInClause.LastIndexOf(","));
                    whereInClauses.Add(whereInClause);
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create OID where in clauses. Error: " + ex.Message);
            }
        }

    }

    public static class Logger
    {
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "PGE.BatchApplication.FAISessionProcessor.log4net.config");

        public static void Error(string message)
        {
            _log.Error(message);
            Console.WriteLine("ERROR: " + message);
        }

        public static void Warning(string message)
        {
            _log.Warn(message);
            //Console.WriteLine("WARNING: " + message);
        }

        public static void Info(string message)
        {
            _log.Info(message);
            Console.WriteLine(message);
        }

        public static void Debug(string message)
        {
            _log.Info(message);
            //Console.WriteLine("DEBUG: " + message);
        }
    }
}
