using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Net;
using System.Configuration;
using System.Data.OleDb;
using System.Data;
using log4net;
using PGE.BatchApplication.GISPLDBAssetSynchProcess.Common;
using PGE.BatchApplication.GISPLDBAssetSynchProcess.DAL;

namespace PGE.BatchApplication.GISPLDBAssetSynchProcess
{
    class Program
    {
        //private static Miner.Interop.IMMAppInitialize _appInitialize;
        //private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        private static StreamWriter pSWriter = default(StreamWriter);
        private static string sPath =  System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)  + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private log4net.ILog Logger = null;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "GISPLDB.log4net.config");
        static void Main(string[] args)
        {
            try
            {
                //CommonUtil cmnUtil = new CommonUtil();
                //string removed =cmnUtil.RemoveCurlybracesInGUID("{7F1876B0-8DF7-4DFC-8F79-FD71C137D9F4}");
              
               GISPLDBAssetSyncTemplateService GISTempService= new GISPLDBAssetSyncTemplateService();

               //Dictionary<string, string> dictFinalList = new Dictionary<string, string>();

               //dictFinalList.Add("PLDBID", "7701407939");
               //dictFinalList.Add("PGE_SAPEQUIPID", "103860773");
               //dictFinalList.Add("PGE_GLOBALID", "7F1876B0-8DF7-4DFC-8F79-FD71C137D9F4");
               //dictFinalList.Add("Latitude", "35.3176037");
               //dictFinalList.Add("Longitude", "-119.0691485");

               //GISTempService.postData(dictFinalList, "Insert");
               //string s1 = "LATITUDE";
               //string s = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s1.ToLower());

               DataTable DTUpdateEd06 = GISTempService.ReadandUpdateEd06fromCSV(System.Configuration.ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED06"], System.Configuration.ConfigurationManager.AppSettings["NAS_FILE_LOCATION_ED07"], AppConstants.CNFG_ED07_FIELDMAPPINGSECTION);
               GISTempService.BulkInsertAfterSApEQUIPIDUpdate(DTUpdateEd06);
               GISTempService.RetreiveDataFromTableAndProcess();
                
            }
            catch(Exception ex)
            {
                WriteLine("Exception occurred, Process Terminated : - " + ex.Message.ToString());
                _logger.Info("Exception occurred, Process Terminated : - " + ex.Message.ToString());
            }
            
        }
        
    
        //Function to get PLDBID
        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }
        //private static void InitializeLicense()
        //{
        //    try
        //    {
        //        if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeEngine, esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
        //         new esriLicenseExtensionCode[] { }))
        //        {
        //            System.Console.WriteLine(m_AOLicenseInitializer.LicenseMessage());
        //            System.Console.WriteLine("This application could not initialize with the correct ArcGIS license and will shutdown.");
        //            m_AOLicenseInitializer.ShutdownApplication();
        //            return;
        //        }

        //        //Check out Telvent license
        //        if (CheckOutArcFMLicense(mmLicensedProductCode.mmLPArcFM) != mmLicenseStatus.mmLicenseCheckedOut)
        //        {
        //            WriteLine("This application could not initialize with the correct ArcFM license and will shutdown.");
        //            return;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //       WriteLine("Exception occurred while intializing the ArcFM license." + ex.Message.ToString());
        //        throw ex;
        //    }
        //}

        public DataTable GetDataTable(string filePath)
        {
            OleDbConnection conn = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0; Data Source = " + Path.GetDirectoryName(filePath) + "; Extended Properties = \"Text;HDR=YES;FMT=Delimited\"");
            conn.Open();
            string strQuery = "SELECT * FROM [" + Path.GetFileName(filePath) + "]";
            OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, conn);
            DataSet ds = new System.Data.DataSet("CSV File");
            adapter.Fill(ds);
            return ds.Tables[0];
        }

        //private static mmLicenseStatus CheckOutArcFMLicense(mmLicensedProductCode productCode)
        //{
        //    if (_appInitialize == null) _appInitialize = new MMAppInitializeClass();

        //    var licenseStatus = _appInitialize.IsProductCodeAvailable(productCode);
        //    if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
        //    {
        //        licenseStatus = _appInitialize.Initialize(productCode);
        //    }

        //    return licenseStatus;
        //}
        /// <summary>
        /// InitializeLogger
        /// </summary>
        public void InitializeLogger()
        {
            string executingAssemblyPath = string.Empty;
            try
            {
                //Get log file with complete path
                executingAssemblyPath = GetLogfilepath();

                if (executingAssemblyPath.Trim().Length == 0)
                {
                    executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                string logPath = System.IO.Path.Combine(executingAssemblyPath, @"GISPLDBAssetSyncProcess_" + DateTime.Now.ToString("MM_dd_yyyy") + ".log");

                //log4net 
                log4net.GlobalContext.Properties["LogName"] = logPath;
                //string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                executingAssemblyPath = (System.IO.Path.Combine(executingAssemblyPath, "logging.xml"));

                StreamWriter wr = new StreamWriter(executingAssemblyPath);
                wr.Write(Getlog4netConfig());
                wr.Close();
                FileInfo fi = new FileInfo(executingAssemblyPath);
                log4net.Config.XmlConfigurator.Configure(fi);

                //Initialize the logging 
                Logger = log4net.LogManager.GetLogger(typeof(Program));
                Logger.Info("Logger Initialized");
            }
            catch (Exception ex)
            {
                Logger.Info(ex.Message);
            }
        }
        /// <summary>
        /// GetLogfilepath gets local folder for log
        /// </summary>
        /// <returns></returns>
        private string GetLogfilepath()
        {
            try
            {
                string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string logPath = (System.IO.Path.Combine(executingAssemblyPath, "C:\\Document\\Logs\\GIStoSAPLog.txt"));
                string path = System.Configuration.ConfigurationManager.AppSettings["LogFolder"];
                //string logPath = string.Format( path + "\\GIStoSAPLog_{0}.txt", DateTime.Now.ToShortDateString().Replace("/", "_"));
                return path;
            }
            catch (Exception ex)
            {
                Logger.Info("Error in Reading Log" + ex.Message);
            }
            return "";
        }
        /// <summary>
        /// Getlog4netConfig
        /// </summary>
        /// <returns></returns>
        private string Getlog4netConfig()
        {

            string Config = @"<?xml version=""1.0"" encoding=""utf-8"" ?> 
                                    <configuration> 
                                      <log4net debug=""False""> 
                                        <!-- Define some output appenders --> 
                                        <appender name=""RollingLogFileAppender"" type=""log4net.Appender.RollingFileAppender""> 
                                          <file type=""log4net.Util.PatternString"" value=""%property{LogName}"" /> 
                                          <appendToFile value=""true"" /> 
                                          <rollingStyle value=""Composite"" />                               
                                         <datePattern value=""ddMMyyyy"" />
                                          <maxSizeRollBackups value=""30"" /> 
                                          <maximumFileSize value=""1MB"" />                                                                                    
                                        <layout type=""log4net.Layout.PatternLayout"">
                                           <conversionPattern value=""%date [%thread] %level %logger - %message%newline""/>
                                        </layout>
                                        </appender> 
                 
                                   <!-- Setup the root category, add the appenders and set the default level --> 
                                   <root> 
                                   <level value=""ALL""/> 
                                   <appender-ref ref=""RollingLogFileAppender"" /> 
                                   </root> 
                                   </log4net> 
                                    </configuration>";
            return Config;
        }           

    }
}
