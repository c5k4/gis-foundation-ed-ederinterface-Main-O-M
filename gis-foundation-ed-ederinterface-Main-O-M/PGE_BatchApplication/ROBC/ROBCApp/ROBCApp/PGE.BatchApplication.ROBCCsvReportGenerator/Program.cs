using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using Oracle.DataAccess.Client;
using PGE_DBPasswordManagement;



namespace PGE.BatchApplication.ROBCCsvReportGenerator
{
    class Program
    {

        #region Private Variable

        private static StreamWriter _fileWriter;

        #endregion

        static int Main(string[] args)
        {
            try
            {

                if (args.Length == 0)
                    return 0;

                SetupLogFile();
                
                
                if (args[0].Equals("ROBC", StringComparison.CurrentCultureIgnoreCase))
                {
                    int retVal = 0;
                    if (args.Length == 2)
                    {
                        switch (args[1].ToUpper())
                        {
                            case "REPORTS":
                                retVal = ProcessRobcData();
                                break;
                            default:
                                retVal = ProcessRobcData();
                                break;
                        }

                    }
                    return retVal;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    _fileWriter.WriteLine(string.Format("Following error occurred at {0}:{1}\nStackTrace:{2}", DateTime.Now, ex.Message, ex.StackTrace));
                }
                catch (Exception e)
                {

                }
                return 1;
            }
            finally
            {
                if (_fileWriter != null)
                    _fileWriter.Close();
            }
            
        }

        private static void CreateDirectory(string logFileName)
        {
            string logFolderPath = logFileName.Substring(0, logFileName.LastIndexOf("\\"));
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
        }
        static void SetupLogFile()
        {
            string logFileName = ConfigurationManager.AppSettings["LogFile"];
            logFileName = string.Format(logFileName, DateTime.Now.ToString("MM-dd-yyyy"));
            CreateDirectory(logFileName);

            try
            {
                _fileWriter = File.AppendText(logFileName);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static int ProcessRobcData()
        {
            try
            {
                _fileWriter.WriteLine(string.Format("*************************ROBC Csv generation process started****************************************{0}", DateTime.Now));
                
                string csvFileDir = ConfigurationManager.AppSettings["CSVFileDir"].ToString();

                using (OracleConnection connection = GetDBConnection())
                {
                    string ECTPSSD_REPORTQuery = ConfigurationManager.AppSettings["ECTPSSD_REPORT"].ToString();

                    string cmdText = ECTPSSD_REPORTQuery;
                    OracleCommand cmdSQL = new OracleCommand();

                    cmdSQL.Connection = connection;
                    cmdSQL.CommandText = cmdText;
                    cmdSQL.CommandType = CommandType.Text;

                    _fileWriter.WriteLine("Executing Oracle CMD to create result.");
                    DataTable dt_ECTPSSD_REPORT = new DataTable();
                    dt_ECTPSSD_REPORT.Load(cmdSQL.ExecuteReader());
                    GenerateCSVFromDataTable(dt_ECTPSSD_REPORT, csvFileDir + "\\" + dt_ECTPSSD_REPORT.TableName + ".csv");


                    string EEP_REPORTQuery = ConfigurationManager.AppSettings["EEP_REPORT"].ToString();

                    cmdText = EEP_REPORTQuery;
                    cmdSQL = new OracleCommand();

                    cmdSQL.Connection = connection;
                    cmdSQL.CommandText = cmdText;
                    cmdSQL.CommandType = CommandType.Text;

                    _fileWriter.WriteLine("Executing Oracle CMD to create result.");
                    DataTable dt_EEP_REPORT = new DataTable();
                    dt_EEP_REPORT.Load(cmdSQL.ExecuteReader());
                    GenerateCSVFromDataTable(dt_EEP_REPORT, csvFileDir + "\\" + dt_EEP_REPORT.TableName + ".csv");
                }

                Console.WriteLine("csv files generated successfully...");
                _fileWriter.WriteLine("csv files generated successfully...");
                return 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error!!!");
                _fileWriter.WriteLine(string.Format("Error!!!\n{0}\n{1} ", ex.Message, ex.StackTrace));
                return 1;
            }
            finally
            {
                _fileWriter.WriteLine(string.Format("*************************ROBC Csv generation process ended****************************************{0}", DateTime.Now));
            }
        }



        private static void GenerateCSVFromDataTable(DataTable dt, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            string[] columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        public static OracleConnection GetDBConnection()
        {
            //string[] oracleConnectionInfo = ConfigurationManager.AppSettings["OracleConnectionString"].Split(',');
            OracleConnection oraConn = new OracleConnection();
            //string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
            string oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["ROBC"].ConnectionString);
            if ((oraConn != null) && (oraConn.State != ConnectionState.Open))
            {
                try
                {
                    oraConn = new OracleConnection(oracleConnectionString);
                    _fileWriter.WriteLine("Connecting to Database...");
                    oraConn.Open();
                    _fileWriter.WriteLine("Database connection successful...");
                }
                catch (Exception ex)
                {
                    _fileWriter.WriteLine("Error: [" + DateTime.Now.ToLocalTime() + "] Connecting Database -- " + ex.Message);
                    throw;
                }
            }

            return oraConn;
        }
    }
}
