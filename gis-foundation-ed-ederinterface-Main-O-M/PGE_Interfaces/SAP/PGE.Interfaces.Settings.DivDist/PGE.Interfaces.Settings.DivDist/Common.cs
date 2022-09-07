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
using Diagnostics = PGE.Common.Delivery.Diagnostics;
//using PGE.Common.Delivery.Systems.Configuration;
//using PGE.Common.Delivery.Framework;



namespace PGE.Interfaces.Settings.DivDist
{
    public class Common
    {
        private static readonly Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.Settings.DivDist.log4net.config");
        public static int OracleTimeout = 360;

        public static Common Lock = new Common();

        private static ConsoleCursorSpiner _Spiner = new ConsoleCursorSpiner();

        public struct DataMigrationStatus
        {
            public const string Success = "SUCCESS";
            public const string Failure = "FAILED";
        }

        public struct ClassModelNames
        {
            public const string conductor = "CONDUCTOR";
        }

        public struct FieldModelNames
        {
            public const string SubstationID = "SUBSTATIONID";
            public const string FeederID = "FEEDERID";
            public const string FeederName = "FEEDERNAME";
            public const string CircuitID = "PGE_CIRCUITID";
            public const string Region = "PGE_INHERITREGION";
            public const string JobNumber = "PGE_JOBNUMBER";
        }

        public struct FieldName
        {
            public const string SubtypeCD = "SUBTYPECD";
            public const string GlobalID = "GLOBALID";
            public const string Shape = "SHAPE";
            public const string Region = "REGION";
            public const string CircuitID = "CIRCUITID";
            public const string JobNumber = "INSTALLJOBNUMBER";
            public const string Status = "STATUS";
            public const string SAPEquipID = "SAPEQUIPID";
        }
        private static IWorkspace ws = null;
        public static IWorkspace SetWorkspace(string configConnectionFileName)   
        {
            try
            {
                return Common.OpenWorkspace(configConnectionFileName);
                //if (ws == null)
                //{
                //    ws = Common.OpenWorkspace(configConnectionFileName);
                //}
            }
            catch (Exception ex)
            {
                Common.WriteToLog("Error: Invalid SDE database connection. Workspace could not set. Please check the config entry for: " + configConnectionFileName + "  " + ex.Message, LoggingLevel.Error);
                Console.WriteLine("Error: Invalid SDE database connection. Workspace could not set. Please check the config entry for: " + configConnectionFileName + "  " + ex.Message);
                throw;
            }

        }

        public static string GetConnectionString()
        {
            string[] oracleConnectionInfo = ConfigurationManager.AppSettings["OracleConnection"].Split(',');
            OracleConnection oraConn = new OracleConnection();
            string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
            return oracleConnectionString;
        }

        public static OracleConnection GetDBConnection()
        {
            string[] oracleConnectionInfo = ConfigurationManager.AppSettings["OracleConnection"].Split(',');
            OracleConnection oraConn = new OracleConnection();
            string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
            if ((oraConn != null) && (oraConn.State != ConnectionState.Open))
            {
                try
                {
                    //string oraConnectionString = string.Format("Data Source={0};User Id={1};Password={2};", oracleConnectionInfo[1], oracleConnectionInfo[2], oracleConnectionInfo[3]);
                    oraConn = new OracleConnection(oracleConnectionString);
                    Common.WriteToLog("Connecting Database [" + oracleConnectionInfo[1] + "]...", LoggingLevel.Info);
                    oraConn.Open();
                    Common.WriteToLog("Database [" + oracleConnectionInfo[1] + "] connection successful...", LoggingLevel.Info);
                    using (var cmd = new OracleCommand("alter session set ddl_lock_timeout = " + Common.OracleTimeout, oraConn))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: [" + DateTime.Now.ToLocalTime() + "] Connecting Database [" + oracleConnectionInfo[1] + "] -- " + ex.Message);
                    Common.WriteToLog("Error: [" + DateTime.Now.ToLocalTime() + "] Connecting Database ["+ oracleConnectionInfo[1] + "] -- " + ex.Message, LoggingLevel.Error);
                    throw;
                }
            }

            return oraConn;
        }

        /// <summary>
        /// Opens a workspace given a direct connect string (i.e. sde:oracle11g:/;local=EDGISA1T)
        /// </summary>
        /// <param name="directConnectString"></param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace(string configConnectionFileName) //string directConnectString, string user, string pass
        {
            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();
            return wsFactory.OpenFromFile(ConfigurationManager.AppSettings[configConnectionFileName], 0);
            //return wsFactory.OpenFromFile(ConfigurationManager.AppSettings["SDEConnectionFile"], 0);
            //return wsFactory.Open(propertySet, 0);
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
        
        public static void SpinConsoleCursor()
        {
            Console.Write("Working....");
            while (true)
            {
                _Spiner.Spin();
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

    public class ConsoleCursorSpiner
    {
        int counter;
        public ConsoleCursorSpiner()
        {
            counter = 0;
        }
        public void Spin()
        {
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }

            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
    }
}
