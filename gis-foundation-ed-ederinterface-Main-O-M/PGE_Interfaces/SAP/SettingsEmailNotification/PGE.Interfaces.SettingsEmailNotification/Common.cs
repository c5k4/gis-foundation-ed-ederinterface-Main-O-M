using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;

using PGE.Common.Delivery.Framework;
using Diagnostics = PGE.Common.Delivery.Diagnostics;
using PGE_DBPasswordManagement;


namespace PGE.Interfaces.SettingsEmailNotification
{
    public class Common
    {

        private static readonly Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "DeleteAnnos.log4net.config");
        public static int OracleTimeout = 360;


        private static IWorkspace ws = null;



        /// <summary>
        /// Opens a workspace given a direct connect string (i.e. sde:oracle11g:/;local=EDGISA1T)
        /// </summary>
        /// <param name="directConnectString"></param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace() //string directConnectString, string user, string pass
        {
            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();
            //return wsFactory.OpenFromFile(ConfigurationManager.AppSettings[configConnectionFileName], 0);

            // m4jf edgisrearch 919
             //return wsFactory.OpenFromFile(ConfigurationManager.AppSettings["SDEConnectionFile"], 0);

            return wsFactory.OpenFromFile(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper()), 0);
            //return wsFactory.Open(propertySet, 0);
        }

        public static IWorkspace OpenSubWorkspace() //string directConnectString, string user, string pass
        {
            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();
            //return wsFactory.OpenFromFile(ConfigurationManager.AppSettings[configConnectionFileName], 0);
            // m4jf edgisrearch 919
            //return wsFactory.OpenFromFile(ConfigurationManager.AppSettings["SDESubConnectionFile"], 0);
            return wsFactory.OpenFromFile(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDERSUB_SDEConnection"].ToUpper()), 0);
            //return wsFactory.Open(propertySet, 0);
        }

        /// <summary>
        /// Given a ModelName and Workspace gets the First featureclass with the given modelname from the Workspace
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        //public static IEnumFeatureClass FeatureClassesByModelName(IWorkspace ws, string modelName)
        //{
        //    return ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(ws, modelName);
        //}

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
    }

    public enum LoggingLevel
    {
        Error,
        Info,
        Debug,
        Warning
    }
}
