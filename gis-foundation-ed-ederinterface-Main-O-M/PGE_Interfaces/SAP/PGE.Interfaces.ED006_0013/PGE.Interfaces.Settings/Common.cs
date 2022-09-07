using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.IO;

using Oracle.DataAccess.Client;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using PGE_DBPasswordManagement;



namespace PGE.Interfaces.Settings
{
    public class Common
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.Settings.log4net.config");
        public static int OracleTimeout = 360;

        public static Common Lock = new Common();

        public struct DataMigrationStatus
        {
            public const string Success = "SUCCESS";
            public const string Failure = "FAILED";
        }

        private static IWorkspace ws = null;
        public static IWorkspace SetWorkspace()   
        {
            try
            {
                if (ws == null)
                {
                    ws = Common.OpenWorkspace();
                }
            }
            catch (Exception ex)
            {
                Common.WriteToLog("Error: Invalid SDE database connection. Workspace could not set. Please check the config entry." + ex.Message, LoggingLevel.Error);
                Console.WriteLine("Error: Invalid SDE database connection.  Workspace could not set. Please check the config entry.");
                throw;
            }

            return ws;
        }

        /// <summary>
        /// Opens a workspace given a direct connect string (i.e. sde:oracle11g:/;local=EDGISA1T)
        /// </summary>
        /// <param name="directConnectString"></param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace() //string directConnectString, string user, string pass
        {
            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();

            // m4jf edgisrearch 919 - get sde connection using PGE_DBPasswordManagement
            //return wsFactory.OpenFromFile(ConfigurationManager.AppSettings["SDEConnectionFile"], 0);
            return wsFactory.OpenFromFile(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper()), 0);
        }

        public static void WriteIDsToFile(string content, string path)
        {
            StreamWriter sw = new StreamWriter(path, false);
            sw.Write(content);
            sw.Close();
        }

        public static string ReadIDsFromFile(string path)
        {
            string line;
            StreamReader sr = new StreamReader(path);
            line = sr.ReadLine();
            sr.Close();

            return line;
        }
        
        public static void WriteToLog(string content, LoggingLevel level)
        {
            if (level == LoggingLevel.Debug)
            {
                _logger.Debug(content);
            }
            else if (level == LoggingLevel.Error)
            {
                _logger.Error(content);
            }
            else if (level == LoggingLevel.Info)
            {
                _logger.Info(content);
            }
            else if (level == LoggingLevel.Warning)
            {
                _logger.Warn(content);
            }
        }

        public static NameValueCollection GetSubtypes(IFeatureClass featureClass)
        {
            string subTypeName;
            int subTypeCode;

            NameValueCollection subTypes = new NameValueCollection();
            ISubtypes structureSubTypes = featureClass as ISubtypes;
            IEnumSubtype enumSubtypes = structureSubTypes.Subtypes;
            subTypeName = enumSubtypes.Next(out subTypeCode);
            while (!string.IsNullOrEmpty(subTypeName))
            {
                subTypes.Add(subTypeName, subTypeCode.ToString());
                subTypeName = enumSubtypes.Next(out subTypeCode);
            }

            return subTypes;
        }

        public static void GenerateCSVFromDataTable(DataTable dt, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).
                                                ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        public static List<string> GetCommaSeperatedValues(string[] stringValues)
        {
            List<string> commaSeperatedValues = new List<string>();
            StringBuilder sb = new StringBuilder();
            int endIndex = 1000;
            for (int i = 0; i <= stringValues.Length - 1; i++)
            {
                if ((i == stringValues.Length - 1) || (i == endIndex - 1))
                {
                    sb.Append(string.Format("'{0}'", stringValues[i]));
                    endIndex += 1000;
                    commaSeperatedValues.Add(sb.ToString());
                    sb = new StringBuilder();
                }
                else
                {
                    sb.Append(string.Format("'{0}',", stringValues[i]));
                }

            }

            return commaSeperatedValues;
        }
    }

    public enum LoggingLevel
    {
        Error,
        Info,
        Debug,
        Warning
    }
}
