using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using System.Data;
using System.Text;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Interfaces.ED08TaxReport
{
    public class LoadData
    {
        #region Private Variable

        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED08.log4net.config");
        private static StreamWriter fileWriter = System.IO.File.AppendText(ConfigurationManager.AppSettings["Result_and_Exception_File"]);
        private static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private static int ReturnPass = 0;
        private static int ReturnFail = 1;
        private static bool exitFlag = false;
        private static int dt = 0;
        private static bool ExecuteSuccess = false;

        #endregion

        private static void CreateCSVExportFile()
        {
            fileWriter.WriteLine("Application is starting up, loading the config.");
            _log.Debug("Application is starting up, loading the config.");
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OracleConnectionString"].ToString()))
            {
                fileWriter.WriteLine("Application is starting up, loaded the configuration.");
                _log.Debug("Application is starting up, loaded the configuration.");
                fileWriter.WriteLine("Creating oracle connection.");
                _log.Debug("Creating oracle connection.");
                using (OracleConnection connection = GetDBConnection())
                {
                    string Parameterlist = ConfigurationManager.AppSettings["Parameterlist"].ToString();
                    string cmdText = string.Format("{0}", Parameterlist);

                    OracleCommand cmdSQL = new OracleCommand();
                    cmdSQL.Connection = connection;
                    cmdSQL.CommandText = cmdText;
                    cmdSQL.CommandType = CommandType.Text;

                    fileWriter.WriteLine("Executing Oracle CMD to create result.");
                    _log.Debug("Executing Oracle CMD to create result.");
                    OracleDataReader dataReader = cmdSQL.ExecuteReader();

                    string Path = ConfigurationManager.AppSettings["Path"].ToString();
                    string str = "sap2gis_stcirc_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    StreamWriter sw = new StreamWriter(Path + str + ".csv");
                    fileWriter.WriteLine("Writing output.");
                    _log.Debug("Writing output.");
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            sw.WriteLine(Convert.ToString(dataReader[0]));
                        }
                        sw.Close();
                        fileWriter.WriteLine("Operation successfull and Csv file generated successfully. ");
                        _log.Debug("Operation successfull and Csv file generated successfully. ");
                        connection.Close();
                    }
                    else
                    {
                        fileWriter.WriteLine("No records found");
                        _log.Debug("No records found");
                    }
                }
            }
            else
            {
                throw new Exception("Invalid configuration. Please check the oracle connection string in the configuration file");
            }
        }

        private static void CreateCircuitToMapNumCSVExportFile()
        {
            fileWriter.WriteLine("CreateCircuitToMapNumCSVExportFile is starting up, loading the config.");
            _log.Debug("CreateCircuitToMapNumCSVExportFile is starting up, loading the config.");
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OracleConnectionString"].ToString()))
            {
                fileWriter.WriteLine("CreateCircuitToMapNumCSVExportFile is starting up, loaded the configuration.");
                _log.Debug("CreateCircuitToMapNumCSVExportFile is starting up, loaded the configuration.");
                fileWriter.WriteLine("Creating oracle connection.");
                _log.Debug("Creating oracle connection.");
                using (OracleConnection connection = GetDBConnection())
                {
                    string tableName = ConfigurationManager.AppSettings["CircuitToMapTable"].ToString();
                    string cmdText = string.Format("SELECT * FROM {0}", tableName);

                    OracleCommand cmdSQL = new OracleCommand();
                    cmdSQL.Connection = connection;
                    cmdSQL.CommandText = cmdText;
                    cmdSQL.CommandType = CommandType.Text;

                    fileWriter.WriteLine("Executing Oracle CMD to create result.");
                    _log.Debug("Executing Oracle CMD to create result.");

                    OracleDataReader dataReader = cmdSQL.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        DataTable circuitToMapNumTable = new DataTable();
                        circuitToMapNumTable.Load(dataReader);

                        string filePath = ConfigurationManager.AppSettings["CircuitMapNumCSVFilePath"].ToString();

                        GenerateCSVFromDataTable(circuitToMapNumTable, filePath);
                    }
                }
            }
            else
            {
                throw new Exception("Invalid configuration. Please check the oracle connection string in the configuration file");
            }
        }

        private static void GenerateCSVFromDataTable(DataTable dt, string filePath)
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

        public static OracleConnection GetDBConnection()
        {
            string[] oracleConnectionInfo = ConfigurationManager.AppSettings["OracleConnectionString"].Split(',');
            OracleConnection oraConn = new OracleConnection();
            string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
            if ((oraConn != null) && (oraConn.State != ConnectionState.Open))
            {
                try
                {
                    //string oraConnectionString = string.Format("Data Source={0};User Id={1};Password={2};", oracleConnectionInfo[1], oracleConnectionInfo[2], oracleConnectionInfo[3]);
                    oraConn = new OracleConnection(oracleConnectionString);
                    fileWriter.WriteLine("Connecting Database [" + oracleConnectionInfo[1] + "]...");
                    _log.Info("Connecting Database [" + oracleConnectionInfo[1] + "]...");
                    oraConn.Open();
                    fileWriter.WriteLine("Database [" + oracleConnectionInfo[1] + "] connection successful...");
                    _log.Info("Database [" + oracleConnectionInfo[1] + "] connection successful...");
                }
                catch (Exception ex)
                {
                    fileWriter.WriteLine("Error: [" + DateTime.Now.ToLocalTime() + "] Connecting Database [" + oracleConnectionInfo[1] + "] -- " + ex.Message);
                    _log.Error("Error: [" + DateTime.Now.ToLocalTime() + "] Connecting Database [" + oracleConnectionInfo[1] + "] -- " + ex.Message);
                    throw;
                }
            }

            return oraConn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            if (args.Count() > 0 && args[0] == "/?")
            {
                Console.WriteLine("-s: Create CSV for support structures");
                Console.WriteLine("-g: Create CSV for map grids");
                Environment.Exit(0);
            }
            if (args.Count() == 0)
            {
                Console.WriteLine("Not enough arguments specified");
                Console.WriteLine("-s: Create CSV for support structures");
                Console.WriteLine("-g: Create CSV for map grids");
                Environment.Exit(1);
            }

            int returnCode = 0;
            try
            {
                string arg1 = "";
                string arg2 = "";

                if (args.Count() > 0)
                {
                    arg1 = args[0];
                }
                if (args.Count() > 1)
                {
                    arg2 = args[1];
                }


                bool executeED08Structure = false; //Create CSV for support structures
                bool executeED08MapGrids = false; //Create CSV for map grids

                if (arg1 == "-s" || arg2 == "-s") { executeED08Structure = true; }
                if (arg1 == "-g" || arg2 == "-g") { executeED08MapGrids = true; }

                if (!executeED08MapGrids && !executeED08Structure)
                {
                    Console.WriteLine("Invalid arguments specified");
                    Console.WriteLine("-s: Create CSV for support structures");
                    Console.WriteLine("-g: Create CSV for map grids");
                    Environment.Exit(1);
                }

                try
                {
                    if (executeED08Structure)
                    {
                        CreateCSVExportFile();
                    }

                    if (executeED08MapGrids)
                    {
                        CreateCircuitToMapNumCSVExportFile();
                    }
                }
                catch (Exception e)
                {
                    returnCode = -1;
                    fileWriter.WriteLine("Error creating the csv output file");
                    _log.Error("Error creating the csv output file");
                    throw e;
                }

                try
                {
                    //Check trigger file (empty .txt file)
                    fileWriter.WriteLine("About to create trigger file");
                    _log.Debug("About to create trigger file");
                    string triggerFileName = ConfigurationManager.AppSettings["TriggerFileName"];
                    if (File.Exists(triggerFileName))
                    {
                        _log.Error("Previous trigger file detected will not create another.");
                        fileWriter.WriteLine("Previous trigger file detected will not create another.");
                    }

                    //create trigger file (empty .txt file)
                    File.CreateText(triggerFileName);
                    fileWriter.WriteLine("Created trigger file.");
                    _log.Info("Created trigger file.");
                }
                catch (Exception e)
                {
                    returnCode = -1;
                    _log.Error("Error creating trigger file", e);
                    fileWriter.WriteLine("Error creating trigger file", e);
                    throw e;
                }
            }
            catch (Exception ex)
            {
                returnCode = -1;
                _log.Debug("Error occurred" + ex);
                fileWriter.WriteLine("Error occurred" + ex);
            }
            finally
            {
                fileWriter.Close();
            }

            return returnCode;
        }
    }
}