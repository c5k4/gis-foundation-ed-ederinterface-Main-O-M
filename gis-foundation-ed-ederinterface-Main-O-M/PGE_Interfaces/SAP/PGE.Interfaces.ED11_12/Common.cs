using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using System.Configuration;
using PGE.Common.Delivery.Framework;
using Miner;
using System.Text.RegularExpressions;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.ED11_12
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

        public static string ResultsOutputLocation = "";
        public static string PTTInputFileName = "";
        public static string PTTXmlInputDirectory = "";
        public static string PTTED11ArchiveDirectory = "";
        public static string PTTED12ArchiveDirectory = "";
        public static string TriggerFileName = "";
        public static string PTTTransactionStatusFileName = "";
        public static string PTTTransactionStatusFile = "";
        public static string PTTStagingTable = "EDGIS.SupportStructurePTTT";
        public static double GhostPoleSearchBuffer = 10.0;
        public static string Session_state = default;



        #region EDGIS Rearch-v1t8
        //Below Code added for EDGIS Rearch Project by V1T8
        public static string ED12StagingTable = "";
        public static string ED12StagingTableName = "";
        public static string RecordSeq = "";

        public static string RecordID = "";
        public static string BatchID = "";
        public static string CreationDate = "";
        public static string ProcessedFlag = "";
        public static string SAPEquipID = "";
        public static string ProcessedTime = "";
        public static string ErrorDescription = "";
        public static string RelatedRecordID = "";
        public static string GUID = "";
        public static string TRANSACTIONTYPE = "";
        public static string TransactionStatus = "";
        public static string ProjectID = "";
        public static string MessageCode = "";
        public static string MesgDescription = "";
        public static string messageDetails = "";
        public static string dateTimeProcessed = "";
        public static string deletedGhostPole = "";
        public static string pttOrginalTransction = "";     



        #endregion


        // m4jf- edgisrearch 415
        public static string intExecutionSummary = default;
        public static string ED11StagingTbl = default;
        public static string ED11UserName = default;

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

            try
            {
                EsriLicense = (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), config.AppSettings.Settings["EsriLicense"].Value);
            }
            catch (Exception e)
            {
                EsriLicense = (esriLicenseProductCode)Enum.Parse(typeof(esriLicenseProductCode), "esriLicenseProductCodeArcInfo");
            }

            try
            {
                ArcFMLicense = (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), config.AppSettings.Settings["ArcFMLicense"].Value);
            }
            catch (Exception e)
            {
                ArcFMLicense = (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), "mmLPArcFM");
            }

            // m4jf edgisrearch 919
            //string passwordTemp = config.AppSettings.Settings["Password"].Value;
            //Password = EncryptionFacade.Decrypt(passwordTemp);

            Password = ReadEncryption.GetPassword(config.AppSettings.Settings["EDER_SDEConnection"].Value);

             //string tempString = config.AppSettings.Settings["ConnectionString"].Value;
            string tempString = ReadEncryption.GetConnectionStr(config.AppSettings.Settings["EDER_ConnectionStr"].Value.ToUpper());

            SessionManagerConnectionStringNoPassword = tempString;

            //Append the decripted password in to form the full connectionstring
            SessionManagerConnectionString = "";

            // m4jf edgisrearch 919 - craeting connection string using PGE_DBPasswordManagement

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
            // m4jf edgisrearch 919
            string[] UserInst = ConfigurationManager.AppSettings["EDER_ConnectionStr"].Split('@');
            string User = UserInst[0].ToUpper();

            string DB = UserInst[1].ToUpper();
            string Pwd = ReadEncryption.GetPassword(User + "@" + DB);

            SessionManagerConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + User + ";Data Source= " + DB + ";Password= " + Pwd;




            try { OracleConnectionString = SessionManagerConnectionString.Replace("Provider=OraOLEDB.Oracle.1;", ""); }
            catch { }

            try
            {
                // m4jf edgisrearch 919
                // SDEConnectionFile = config.AppSettings.Settings["SDEConnectionFile"].Value;
                SDEConnectionFile = ReadEncryption.GetSDEPath(config.AppSettings.Settings["EDER_SDEConnection"].Value.ToUpper());
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to determine SDEConnectionFile from configuration file: " + ex.Message);
            }

            try
            {
                // m4jf edgisrearch 919
                //LandbaseConnectionFile = config.AppSettings.Settings["LandbaseConnectionFile"].Value;
                LandbaseConnectionFile = ReadEncryption.GetSDEPath(config.AppSettings.Settings["LBMAINT_SDEConnection"].Value.ToUpper());
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to determine LandbaseConnectionFile from configuration file: " + ex.Message);
            }

            //Below code commented for EDGIS Rearch Project-v1t8
            //try
            //{
            //    ResultsOutputLocation = config.AppSettings.Settings["ED12OutputLocation"].Value;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Failed to determine the ED12 results output location from the configuration file: " + ex.Message);
            //}

            // M4JF EDGISREARCH 415 - COMMENTED CODE AS FILE SYSTEM IS REMOVED FROM INTERFACE
            //try
            //{
            //    PTTXmlInputDirectory = config.AppSettings.Settings["PTTED11InputDirectory"].Value;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Failed to determine the PTT ED11 xml input directory: " + ex.Message);
            //}

            //try
            //{
            //    PTTED11ArchiveDirectory = config.AppSettings.Settings["PTTED11ArchiveDirectory"].Value;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Failed to determine the PTT ED11 xml archive directory: " + ex.Message);
            //}

            //try
            //{
            //    PTTED12ArchiveDirectory = config.AppSettings.Settings["PTTED12ArchiveDirectory"].Value;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Failed to determine the PTT ED12 archive directory: " + ex.Message);
            //}


            //try
            //{
            //    PTTInputFileName = config.AppSettings.Settings["PTTED11FileName"].Value;
            //}
            //catch (Exception ex)
            //{
            //    PTTInputFileName = "sap2gis_ptt_pole";
            //}

            try
            {
                TriggerFileName = config.AppSettings.Settings["TriggerFileName"].Value;
                TriggerFileName = Path.Combine(ResultsOutputLocation, TriggerFileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to determine the PT&T trigger file name from the configuration file: " + ex.Message);
            }

            try
            {
                DateTime time = DateTime.Now;
                PTTTransactionStatusFile = config.AppSettings.Settings["PTTTransactionStatusFileName"].Value;

                string month = time.Month.ToString();
                if (month.Length < 2) { month = "0" + month; }
                string day = time.Day.ToString();
                if (day.Length < 2) { day = "0" + day; }
                string hour = time.Hour.ToString();
                if (hour.Length < 2) { hour = "0" + hour; }
                string minute = time.Minute.ToString();
                if (minute.Length < 2) { minute = "0" + minute; }
                string second = time.Second.ToString();
                if (second.Length < 2) { second = "0" + second; }

#if DEBUG
                if (Directory.Exists(ResultsOutputLocation))
                {
                    string[] files = Directory.GetFiles(ResultsOutputLocation);
                    foreach (string file in files)
                    {
                        if (file.Contains(PTTTransactionStatusFile)) { File.Delete(file); }
                    }
                }
#endif

                PTTTransactionStatusFile = PTTTransactionStatusFile + "_" + time.Year + month + day + "_" + hour + minute + second + ".xml";
                PTTTransactionStatusFileName = PTTTransactionStatusFile;
                PTTTransactionStatusFile = Path.Combine(ResultsOutputLocation, PTTTransactionStatusFile);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to determine the PT&T transaction status file name from the configuration file: " + ex.Message);
            }

            try
            {
                PTTStagingTable = config.AppSettings.Settings["StagingTable"].Value;
            }
            catch (Exception ex)
            {
                PTTStagingTable = "EDGIS.SupportStructurePTT";
            }

            #region EDGIS Rearch Project ED12 
            //Below code added for EDGIS Rearch Project for ED12 integration improvement by v1t8  to fill data in staging table instead of files
            try
            {
                PTTProcessor._log.Info("reading ED12 Staging table  name from the configuration file");
                //ED12 Staging table Name from config
                ED12StagingTable = config.AppSettings.Settings["ED12StagingTable"].Value;
            }
            catch (Exception ex)
            {
                PTTProcessor._log.Error("Failed to determine the ED12 Staging table name from the configuration file: " + ex.Message);
               //ED12 Staging table Name from app.config
                ED12StagingTable = "EDGIS.PGE_ED12_OUTBOUND_STAGING";
            }
            try
            {
                PTTProcessor._log.Info("reading ED12 Record sequence  name from the configuration file");
                //ED12 Record sequence Name from config
                RecordSeq  = config.AppSettings.Settings["ED12_RECORDID_SEQ"].Value;
            }
            catch (Exception ex)
            {
                PTTProcessor._log.Error("Failed to determine the ED12 Record sequence name from the configuration file: " + ex.Message);
                //ED12 Record sequence Name from config
                RecordSeq  = "EDGIS.ED12_RECORDID_SEQ";
            }
            try
            {
                PTTProcessor._log.Info("Reading ED12 Staging table  name from the configuration file");
                //ED12 Staging table Name from config
                ED12StagingTableName = config.AppSettings.Settings["ED12StagingTableName"].Value;
            }
            catch (Exception ex)
            {
                PTTProcessor._log.Error("Failed to determine the ED12 Staging table name from the configuration file: " + ex.Message);
                //ED12 Staging table Name from app.config
                ED12StagingTableName = "PGE_ED12_OUTBOUND_STAGING";
            }
            try
            {
                PTTProcessor._log.Info("Reading ED12 Staging table fields name from the configuration file");
                //ED12 staging table fields Name from config file
                RecordID = config.AppSettings.Settings["RecordID"].Value;
                BatchID = config.AppSettings.Settings["BatchID"].Value;
                CreationDate  = config.AppSettings.Settings["CreationDate"].Value;
                ProcessedFlag = config.AppSettings.Settings["ProcessedFlag"].Value;

                SAPEquipID = config.AppSettings.Settings["SAPEquipID"].Value;
                ProcessedTime = config.AppSettings.Settings["ProcessedTime"].Value;
                RelatedRecordID = config.AppSettings.Settings["RelatedRecordID"].Value;
                GUID = config.AppSettings.Settings["GUID"].Value;
                TRANSACTIONTYPE = config.AppSettings.Settings["TRANSACTIONTYPE"].Value;
                TransactionStatus = config.AppSettings.Settings["TransactionStatus"].Value;
                ProjectID = config.AppSettings.Settings["ProjectID"].Value;
                MessageCode = config.AppSettings.Settings["MessageCode"].Value;
                messageDetails = config.AppSettings.Settings["MESSAGEDETAIL"].Value;
                MesgDescription= config.AppSettings.Settings["MESSAGEDESCRIPTION"].Value;
                dateTimeProcessed= config.AppSettings.Settings["DATETIMEPROCESSED"].Value;
                pttOrginalTransction= config.AppSettings.Settings["PTTORIGINALTRANSACTION"].Value;
                deletedGhostPole= config.AppSettings.Settings["DELETEDGHOSTPOLE"].Value;
                ErrorDescription= config.AppSettings.Settings["ErrorDescription"].Value; ;



                //EquipmentNumber_I = config.AppSettings.Settings["EquipmentNumber_I"].Value;
                //GUID_I = config.AppSettings.Settings["GUID_I"].Value;
                //PoleClass_I  = config.AppSettings.Settings["PoleClass_I"].Value;
                //Material_I = config.AppSettings.Settings["Material_I"].Value;
                //Height_I  = config.AppSettings.Settings["Height_I"].Value;
                //PlantSection_I  = config.AppSettings.Settings["PlantSection_I"].Value;
                //StartupDate_I  = config.AppSettings.Settings["StartupDate_I"].Value;
                //Latitude_I  = config.AppSettings.Settings["Latitude_I"].Value;
                //Longitude_I  = config.AppSettings.Settings["Longitude_I"].Value;
                //ProjectID_I  = config.AppSettings.Settings["ProjectID_I"].Value;
                //Division_I = config.AppSettings.Settings["Division_I"].Value;
                //District_I  = config.AppSettings.Settings["District_I"].Value;
                //City_I = config.AppSettings.Settings["City_I"].Value;
                //LocDesc2_I  = config.AppSettings.Settings["LocDesc2_I"].Value;
                //Zip_I  = config.AppSettings.Settings["Zip_I"].Value;
                //Barcode_I  = config.AppSettings.Settings["Barcode_I"].Value;
                //GemsMapOffice_I  = config.AppSettings.Settings["GemsMapOffice_I"].Value;
                //JPNumber_I  = config.AppSettings.Settings["JPNumber_I"].Value;
                //DistMap_I   = config.AppSettings.Settings["DistMap_I"].Value;

                //EquipmentNumber_D  = config.AppSettings.Settings["EquipmentNumber_D"].Value;
                //GUID_D  = config.AppSettings.Settings["GUID_D"].Value;
                //ProjectID_D  = config.AppSettings.Settings["ProjectID_D"].Value;

                //EquipmentNumber_U = config.AppSettings.Settings["EquipmentNumber_U"].Value;
                //AttributeName_U  = config.AppSettings.Settings["AttributeName_U"].Value;
                //OldValue_U = config.AppSettings.Settings["OldValue_U"].Value;                
                //NewValue_U = config.AppSettings.Settings["NewValue_U"].Value;
                //ProjectID_U = config.AppSettings.Settings["ProjectID_U"].Value;
            
            }
            catch (Exception ex)
            {
                PTTProcessor._log.Error("Failed to determine the ED12 Staging table fields name from the configuration file: " + ex.Message);
                throw new Exception("Failed to determine the ED12 Staging table fields name from the configuration file: " + ex.Message);
            }
            #endregion 

            try
            {
                GhostPoleSearchBuffer = Double.Parse(config.AppSettings.Settings["GhostPoleSearchBuffer"].Value);
            }
            catch (Exception ex)
            {
                GhostPoleSearchBuffer = 10.0;
            }
            // m4jf edgisrearch 415 - read inSummaryExecution path
            try
            {
                intExecutionSummary = config.AppSettings.Settings["IntExecutionSummaryExePath"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //m4jf edgisrearch 415 - read  ed11 user name
           

            try
            {
                ED11UserName = config.AppSettings.Settings["ED11UserName"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            try
            {
                Session_state = config.AppSettings.Settings["sessionState"].Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

    }
}
