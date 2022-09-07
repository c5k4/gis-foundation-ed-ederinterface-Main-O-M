using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PGE.Interfaces.ENOS_SAP_TO_PRIMARY_CONSOLE;
using System.IO;
using System.Data.OleDb;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Globalization;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.ENOS_SAP_TO_PRIMARY_CONSOLE
{
    class LoadData
    {

        public static string INPUT_FOLDER_PATH = string.Empty;
        public static string CONNECTION_STRING = string.Empty;
        public static string ARCHIVE_FOLDER_PATH = string.Empty;
        public static string LOG_FILE_PATH = string.Empty;
        public static string SUMMARY_FILE_NAME = string.Empty;
        public static string EQP_FILE_NAME = string.Empty;
        public static string DELIMITER = string.Empty;
        public static string SUMMARY_TABLE_NAME = string.Empty;
        public static string EQP_TABLE_NAME = string.Empty;
        public static string SUMMARY_COLUMN_LIST = string.Empty;
        public static string EQP_COLUMN_LIST = string.Empty;
        public static string SAP_FILE_PATH = string.Empty;
        public static string PTO_DATE_INDEX = string.Empty;
        public static string _summaryFileName = string.Empty;
        public static string _equipFileName = string.Empty;
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "pge.log4net.config");
        public static void readTextFiles()
        {
            readFromConfiguration();
            DirectoryInfo sourceinfo = new DirectoryInfo(INPUT_FOLDER_PATH);
            FileInfo[] info = sourceinfo.GetFiles();
            //List<KeyValuePair<DateTime, string>> File_Date = new List<KeyValuePair<DateTime, string>>();
            _log.Info("Reading Files Received From SAP...");
            FileInfo[] files = sourceinfo.GetFiles().OrderBy(p => p.CreationTime).ToArray();

            if (info.Length != 0)
            {
                for (int index = 0; index < info.Length; index++)
                {
                    try
                    {
                        string file_Split = info[index].Name.Substring(0, info[index].Name.LastIndexOf('_'));
                        if (file_Split.ToUpper() == SUMMARY_FILE_NAME.ToUpper())
                        {
                            _summaryFileName = info[index].Name.ToString();
                            LoadSummaryFile(info[index].FullName);

                        }
                        else
                            if (file_Split.ToUpper() == EQP_FILE_NAME.ToUpper())
                            {

                                _equipFileName = info[index].Name.ToString();
                                LoadEquipFile(info[index].FullName);
                            }
                            else
                            {
                                _log.Info(file_Split + " File Couldn't be read.. Please check wheather File Name is in Correct Format");
                            }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error in function StartLoadProcess: " + ex.Message);
                    }
                }

            }
            else
            {
                _log.Info("No Files Found to Load...");
            }

        }


        public static void readFromConfiguration()
        {
            INPUT_FOLDER_PATH = ConfigurationManager.AppSettings["Path_To_SAP_Files"];
            ARCHIVE_FOLDER_PATH = ConfigurationManager.AppSettings["Path_To_Archive"];
            // M4JF EDGISREARCH 919 - Get connection string using PGE_DBPasswordManagement
           // CONNECTION_STRING = ConfigurationManager.AppSettings["connString"];
            CONNECTION_STRING = "Persist Security Info=True;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
            LOG_FILE_PATH = ConfigurationManager.AppSettings["Log_File_Path"];
            SUMMARY_FILE_NAME = ConfigurationManager.AppSettings["Summary_File_Name"];
            EQP_FILE_NAME = ConfigurationManager.AppSettings["Equipment_File_Name"];
            DELIMITER = ConfigurationManager.AppSettings["Delimiter"];
            SUMMARY_TABLE_NAME = ConfigurationManager.AppSettings["Summary_Table_Name"];
            EQP_TABLE_NAME = ConfigurationManager.AppSettings["Equipment_Table_Name"];
            SUMMARY_COLUMN_LIST = ConfigurationManager.AppSettings["Summary_Column_List"];
            EQP_COLUMN_LIST = ConfigurationManager.AppSettings["Equip_Insert_Columns"];
            SAP_FILE_PATH = ConfigurationManager.AppSettings["Path_To_SAP_Files"];
            PTO_DATE_INDEX = ConfigurationManager.AppSettings["PTO_DATE_INDEX"];

        }
        /// <summary>
        /// Load Summary File
        /// </summary>
        /// <param name="summary_path"></param>
        public static void LoadSummaryFile(string summary_path)
        {
            try
            {
                _log.Info("Processing Summary File.. " + DateTime.Now);

                DataTable DT_Summary_Content = new DataTable();
                DT_Summary_Content = SetColumns(DT_Summary_Content, SUMMARY_COLUMN_LIST);
                Double oId = 0;
                using (var fileStream = File.OpenRead(summary_path))
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string line;
                    // Read lines from the file until the end of 
                    // the file is reached.

                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                var cols = line.Split('|');
                                if (cols[0] != "ACTION")
                                {
                                    //string[] _content = line.Split(new Char[] { '\t' });
                                    //contents.Add(_content);
                                    oId++;
                                    DataRow dr = DT_Summary_Content.NewRow();
                                    int cIndex = 0;
                                    for (; cIndex < DT_Summary_Content.Columns.Count - 1; cIndex++)
                                    {
                                        try
                                        {
                                            if (cols[cIndex] != "")
                                            {
                                                if (cIndex == 10)
                                                {
                                                    if (cols[cIndex].Length == 1)
                                                    {
                                                        dr[cIndex] = cols[cIndex].Replace(cols[cIndex], "0" + cols[cIndex]);
                                                    }
                                                    else
                                                    {
                                                        dr[cIndex] = cols[cIndex];
                                                    }
                                                }
                                                else
                                                {
                                                    dr[cIndex] = cols[cIndex];
                                                }
                                            }
                                            else
                                            {
                                                dr[cIndex] = DBNull.Value;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _log.Error("Error in Reading Record" + cols[1] + " ex.Message");
                                            throw;
                                        }
                                    }
                                    dr[cIndex] = Convert.ToDouble(oId);
                                    DT_Summary_Content.Rows.Add(dr);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Error While reading Summary File, Function name:LoadSummaryFile : " + ex.Message);
                            continue;
                        }
                    }
                    if (DT_Summary_Content.Rows.Count > 0)
                    {

                        //Adding Extra Columns to Datatable that to be used in OracleBulkCopy
                        //DT_Summary_Content.Columns.Add(ReadConfigurations.COL_POWER_SOURCE);
                        DT_Summary_Content.Columns.Add("STATUS");
                        // table.Columns.Add("OBJECTID");
                        DT_Summary_Content.Columns.Add("STATUS_MESSAGE");
                        DT_Summary_Content.Columns.Add("UPDATED_IN_MAIN");
                        DT_Summary_Content.Columns.Add("UPDATED_IN_ED_MAIN");
                        DT_Summary_Content.Columns.Add("UPDATED_IN_SETTINGS");
                        DT_Summary_Content.Columns.Add("CEDSA_MATCH_FOUND");

                        int writeStatus = writeToDatabase(DT_Summary_Content, SUMMARY_TABLE_NAME);

                        if (writeStatus == 1)
                        {
                            _log.Info("\nInsertion in " + SUMMARY_TABLE_NAME + " Completed Successfuly... Total rows Inserted : " + DT_Summary_Content.Rows.Count);
                            fileStream.Close();
                            try
                            {
                                File.Move(summary_path, Path.Combine(ARCHIVE_FOLDER_PATH, _summaryFileName));
                            }
                            catch (Exception e)
                            {
                                _log.Info("Error in Moving File " + _summaryFileName + " Error: " + e.Message);
                            }
                        }
                        else
                        {
                            _log.Info("\nError Occured in Insertion.");
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        _log.Info("\nNo Data Found to Insert in SUMMARY STAGE!!!");

                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception e)
            {

                _log.Error("Error in function LoadSummaryFile: " + e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Load Equipment File
        /// </summary>
        /// <param name="equip_path"></param>
        public static void LoadEquipFile(string equip_path)
        {
            try
            {
                _log.Info("Processing Equipment File.. " + DateTime.Now);

                _log.Info("Reading Equipment Text File...");
                DataTable DT_Equipment_Content = new DataTable();
                DT_Equipment_Content = SetColumns(DT_Equipment_Content, EQP_COLUMN_LIST);


                double oId_Equip = 0;
                using (var fileStream = File.OpenRead(equip_path))
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    string line;
                    // Read lines from the file until the end of 
                    // the file is reached.

                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            line = sr.ReadLine();
                            if (!string.IsNullOrEmpty(line))
                            {
                                var cols = line.Split('|');
                                if (cols[0] != "ACTION")
                                {
                                    //string[] _content = line.Split(new Char[] { '\t' });
                                    //contents.Add(_content);

                                    //Incrementing Object Id Variable for each row
                                    oId_Equip++;
                                    
                                    DataRow dr = DT_Equipment_Content.NewRow();
                                    int cIndex = 0;
                                    for (; cIndex < DT_Equipment_Content.Columns.Count - 1; cIndex++)
                                    {
                                        try
                                        {
                                            if (cols[cIndex] != "")
                                            {

                                                if (cIndex == Convert.ToInt32(PTO_DATE_INDEX))
                                                {
                                                    // Convert.ToDateTime(String.Format("")
                                                    //dr[cIndex] = Convert.ToDateTime(cols[cIndex].ToString());
                                                    DateTime creationDate;
                                                    try
                                                    {
                                                        //  CultureInfo culture = new CultureInfo("en-US");
                                                        creationDate = //Convert.ToDateTime(cols[cIndex].ToString());

                                                        DateTime.ParseExact(cols[cIndex].ToString(), "yyyymmdd", CultureInfo.InvariantCulture);
                                                        string dt = creationDate.ToString("dd-MMM-yy");
                                                        if (creationDate != null)
                                                        {
                                                            dr[cIndex] = dt;
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        dr[cIndex] = null;
                                                    }
                                                }
                                                else
                                                {
                                                    dr[cIndex] = cols[cIndex];
                                                }
                                            }
                                            else
                                            {
                                                dr[cIndex] = DBNull.Value;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _log.Error("Error in Reading Record"+cols[1]+" ex.Message");
                                            throw ;
                                        }

                                    }
                                   
                                    dr[cIndex] = oId_Equip;
                                    DT_Equipment_Content.Rows.Add(dr);
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            _log.Error("Error While reading Equipment File, function name LoadEquipFile: " + e.Message);
                            continue;
                            //  Environment.Exit(0);
                        }
                    }
                    if (DT_Equipment_Content.Rows.Count > 0)
                    {

                        int writeStatus = writeToDatabase(DT_Equipment_Content, EQP_TABLE_NAME);
                        if (writeStatus == 1)
                        {
                            _log.Info("\nInsertion in " + EQP_TABLE_NAME + " Completed Successfuly... Total rows Inserted : " + DT_Equipment_Content.Rows.Count);
                            fileStream.Close();
                            try
                            {
                                File.Move(equip_path, Path.Combine(ARCHIVE_FOLDER_PATH, _equipFileName));
                            }
                            catch (Exception e)
                            {
                                _log.Info("Error in Moving File " + _equipFileName + " Error: " + e.Message);
                            }
                        }
                        else
                        {
                            _log.Info("\nError Occured in Insertion into Equipment Stage.");
                        }
                    }
                    else
                    {

                        _log.Info("No Data Found To Insert in Equipment Stage");
                        //Console.ReadLine();
                        // Environment.Exit(0);
                    }
                }
            }
            catch (Exception e)
            {

                _log.Error("Error in function LoadEquipFile: " + e.Message);
                Environment.Exit(0);
            }
        }

        public static System.Data.DataTable SetColumns(System.Data.DataTable table, String column_list)
        {
            int columnIndex = 0;
            string[] columnNames = column_list.Split(',');
            foreach (var columnName in columnNames)
            {
                if (columnName == "PTO_DATE")
                {
                    table.Columns.Add(columnName, System.Type.GetType("System.DateTime"));
                }
                else
                    if (columnName == "OBJECTID")
                    {
                        table.Columns.Add(columnName, System.Type.GetType("System.Double"));
                    }
                    else
                    {
                        table.Columns.Add(columnName);
                    }

                columnIndex++;
            }
            return table;
        }


        private static bool checkDouble(string val, double result)
        {
            if (Double.TryParse(val, out result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static void PopulateNullParameters(OleDbCommand cmd)
        {
            foreach (OleDbParameter p in cmd.Parameters)
            {
                if (p.Value == null)
                {
                    p.Value = DBNull.Value;
                }
            }
        }
        public static System.Data.DataTable SetColumnsOrder(System.Data.DataTable table, String column_list)
        {
            try
            {

                int columnIndex = 0;
                string[] columnNames = column_list.Split(',');
                foreach (var columnName in columnNames)
                {
                    table.Columns[columnName].SetOrdinal(columnIndex);
                    columnIndex++;
                }
                return table;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Method to Insert Bulk Data to DataBase
        public static int writeToDatabase(DataTable dt, string tableName)
        {
            try
            {

                int flag = 0;
                #region Inserting Data Using INSERT command
                //TNS File Path C:\ProgramData\TNS_ADMIN
                //using (OleDbConnection connection = new OleDbConnection(CONNECTION_STRING))
                //{
                //    connection.Open();
                //    Console.Write("Truncating Table " + tableName);
                //    string sql = "DELETE FROM "+ tableName;
                //    OleDbCommand cmd = new OleDbCommand(sql, connection);
                //    cmd.ExecuteNonQuery();
                //    Console.Write("\nTable "+tableName+" Truncated");
                //    Console.Write("\nData Insertion in table "+tableName+" Started...");
                //    foreach (DataRow row in dt.Rows)
                //    {

                //        string action = ConfigurationManager.AppSettings["Summary_Action"];
                //        string sap_notification = ConfigurationManager.AppSettings["Summary_SAP_Noti"];
                //        string project_name = ConfigurationManager.AppSettings["Summary_Project_Name"];
                //        string eff_rating_mach_kw = ConfigurationManager.AppSettings["Summary_Mach_KW"];
                //        string eff_rating_inv_kw = ConfigurationManager.AppSettings["Summary_Inv_KW"];
                //        string eff_rating_mach_kva = ConfigurationManager.AppSettings["Summary_Mach_KVA"];
                //        string eff_rating_inv_kva = ConfigurationManager.AppSettings["Summary_Inv_KVA"];
                //        string max_cap = ConfigurationManager.AppSettings["Summary_Max_Cap"];
                //        string charge_demand_kw = ConfigurationManager.AppSettings["Summary_Charfe_Dmd"];
                //        string program_type = ConfigurationManager.AppSettings["Summary_Program_Type"];
                //        string service_pt = ConfigurationManager.AppSettings["Summary_Spid"];
                //        string guid = ConfigurationManager.AppSettings["Summary_Guid"];
                //        string objid = ConfigurationManager.AppSettings["Summary_Objid"];
                //        string summary_col = ConfigurationManager.AppSettings["Summary_Insert_Columns"];
                //        Double eff_mach_kw,eff_mach_kva,eff_inv_kw,eff_inv_kva,maxCap,spid,charge_dmd;

                //        Double.TryParse(row[eff_rating_mach_kw].ToString(), out eff_mach_kw);
                //        Double.TryParse(row[eff_rating_inv_kw].ToString(), out eff_inv_kw);
                //        Double.TryParse(row[eff_rating_mach_kva].ToString(), out eff_mach_kva);
                //        Double.TryParse(row[eff_rating_inv_kva].ToString(),out eff_inv_kva);
                //        Double.TryParse(row[charge_demand_kw].ToString(),out charge_dmd );
                //        Double.TryParse(row[service_pt].ToString(), out spid);
                //        Double.TryParse(row[max_cap].ToString(), out maxCap);

                //        string sql_insert = "INSERT INTO " + tableName + " ("+summary_col+") values(" +
                //            "'" + row[action] + "','" + row[sap_notification] + "','" + row[project_name] + "'," + eff_mach_kw + "," + eff_inv_kw + "," + eff_mach_kva +
                //            "," +eff_inv_kva + "," + maxCap + "," +charge_dmd+ ",'" + row[program_type] + "'," +spid + ",'" + row[guid] + "')";

                //        OleDbCommand cmd_insert = new OleDbCommand(sql_insert, connection);


                //        flag = cmd_insert.ExecuteNonQuery();
                //    }

                //    connection.Close();
                #endregion

                #region Inserting Data Using Oracle Bulk Copy
                using (OracleConnection conn = new OracleConnection(LoadData.CONNECTION_STRING))
                {
                    try
                    {
                        conn.Open();
                        string sqlTrunc = "DELETE FROM " + tableName;
                        OracleCommand cmd = new OracleCommand(sqlTrunc, conn);
                        cmd.ExecuteNonQuery();

                        // DataTable DT_Reordered = LoadData.SetColumnsOrder(dt, column_list);

                        using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conn))
                        {
                            bulkCopy.BulkCopyTimeout = 1000000;
                            bulkCopy.DestinationTableName = tableName;
                            foreach (DataColumn column in dt.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
                            }
                            _log.Info("Inserting Data in " + tableName + ": " + DateTime.Now);
                            bulkCopy.WriteToServer(dt);
                            _log.Info("Inserted Data in Table " + tableName + " at " + DateTime.Now);
                            flag = 1;
                        }

                        conn.Close();
                    }
                    catch (Exception e)
                    {
                        _log.Error(e.Message);
                        Console.WriteLine("Error Occurred while inserting DATA!!! Error: " + e.Message);
                        Console.WriteLine("Press Any to Exit... ");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                #endregion

                }
                return flag;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                LoadData._log.Error(e.Message);
                Console.WriteLine("Press Any Key to Exit...");
                Console.ReadLine();
                Environment.Exit(0);
                return 0;
            }


        }
        public static bool call_procedure(string procedure_name, List<OracleParameter> parameters)
        {
            try
            {
                _log.Info("Executing Procedure " + procedure_name);
                using (OracleConnection connection = new OracleConnection(LoadData.CONNECTION_STRING))
                {
                    connection.Open();
                    DataTable dt = new DataTable();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = procedure_name;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (OracleParameter param in parameters)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    int value = cmd.ExecuteNonQuery();
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dt);
                    _log.Info("Executed Procedure " + procedure_name);

                    return true;

                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);

                return false;
            }

        }
        //public void WriteToServer(string qualifiedDBName, DataTable dataTable)
        //{

        //    OracleConnection objConnection = new OracleConnection(connection);
        //    OracleTransaction OracleTransaction;
        //    objConnection.Open();
        //    try
        //    {

        //        using (OracleTransaction = objConnection.BeginTransaction())
        //        {
        //            using (Oracle.DataAccess.Client.OracleBulkCopy bulkCopy = new Oracle.DataAccess.Client.OracleBulkCopy(connection))
        //            {
        //                try
        //                {


        //                    bulkCopy.BulkCopyTimeout = 1000000;
        //                    bulkCopy.DestinationTableName = qualifiedDBName;
        //                    bulkCopy.WriteToServer(dataTable);


        //                    OracleTransaction.Commit();
        //                }
        //                catch (Exception exc)
        //                {

        //                    OracleTransaction.Rollback();
        //                    objConnection.Close();
        //                    Logger.Error(exc.Message);
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception exc)
        //    {

        //        if (objConnection.State == ConnectionState.Open)
        //        {
        //            objConnection.Close();
        //        }
        //        Logger.Error(exc.Message);
        //        // throw;
        //    }
        //    finally
        //    {
        //        if (objConnection.State == ConnectionState.Open)
        //        {
        //            objConnection.Close();
        //        }

        //        objConnection.Dispose();

        //    }
        //}

    }
}
