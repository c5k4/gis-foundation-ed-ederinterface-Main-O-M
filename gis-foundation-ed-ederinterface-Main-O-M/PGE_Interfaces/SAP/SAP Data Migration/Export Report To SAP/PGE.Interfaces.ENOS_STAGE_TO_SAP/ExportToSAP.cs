using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Data.OleDb;
using PGE.Interfaces.ENOS_STAGE_TO_SAP;
using System.IO;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.ENOS_STAGE_TO_SAP
{
    class ExportToSAP
    {
        private static string OUTPUT_FILE = string.Empty;
        public static string CONNECTION_STRING = string.Empty;
        private static string QUERY_SELECTALL = string.Empty;
        public static string CONNECTION_STRING_EDGISA2Q = string.Empty;
        public static string Qry_Slct_Stage = string.Empty;
        public static string Qry_Slct_Changed_CID = string.Empty;
        public static string Qry_Slct_Changed_SPID = string.Empty;
        public static string Proc_Unique_Records=string.Empty;
        public static string Output_File_Archive = string.Empty;
        public static string Qry_Clear_Curr_Data = string.Empty;

        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,"pge.log4net.config");
        public static void exportStart()
        {
            readFromConfiguration();
          
            
            //Call of Procedure that Return table without duplicate records
            DataTable Dt_Unique_Record = call_enos_procedure();
            //Fetching data From DataBase...
            //fetchSPIDFromEDGISIA2Q();
           // DataTable DT_Status_Data = fetchDataFromDB(QUERY_SELECTALL);
           // DataTable DT_With_Unique_Record = removeDuplicateRows(DT_Status_Data, "PROGRAM_TYPE");
           // bool status = exportReportToText(DT_Status_Data);
            bool status = ExportAndUpdateFinalStatus(Dt_Unique_Record);
            if (status)
            {
                Console.WriteLine("EXPORT DONE AT "+DateTime.Now +"...Press any key to end.");
                Console.ReadLine();
            }
        }
       
        private static DataTable removeDuplicateRows(DataTable DT_Status_Data, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateRecordList = new ArrayList();
            foreach (DataRow dRow in DT_Status_Data.Rows)
            {
                if (hTable.Contains(dRow[colName]))
                    duplicateRecordList.Add(dRow);
                else
                {
                    hTable.Add(dRow[colName], string.Empty);
                }

            }
            foreach (DataRow dRow in duplicateRecordList)
            {
                DT_Status_Data.Rows.Remove(dRow);
            }
            return DT_Status_Data;
        }
        public static void readFromConfiguration()
        {
            //m4jf edgisrearch 919
            //CONNECTION_STRING = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
            CONNECTION_STRING= "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
            // m4jf edgisrearch 919 
            CONNECTION_STRING_EDGISA2Q = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDAUX_ConnectionStr"].ToUpper());
            //CONNECTION_STRING_EDGISA2Q = ConfigurationManager.ConnectionStrings["connString_EDGISA2Q"].ConnectionString;
            
            OUTPUT_FILE = ConfigurationManager.AppSettings["Output_File"];
            Output_File_Archive = ConfigurationManager.AppSettings["Output_File_Archive"];
            QUERY_SELECTALL = ConfigurationManager.AppSettings["Query_SelectAll"];
            Qry_Slct_Stage = ConfigurationManager.AppSettings["Qry_Slct_Stage"];
            Qry_Slct_Changed_CID = ConfigurationManager.AppSettings["Qry_Slct_Changed_CID"];
            Qry_Slct_Changed_SPID = ConfigurationManager.AppSettings["Qry_Slct_Changed_SPID"];
            Proc_Unique_Records = ConfigurationManager.AppSettings["Proc_Unique_Records"];
            Qry_Clear_Curr_Data = ConfigurationManager.AppSettings["Qry_Clear_Curr_Data"];

        }
        private static DataTable fetchDataFromDB(string sql)
        {
            try
            {
                DataTable dt = new DataTable();
                using (OleDbConnection con = new OleDbConnection(CONNECTION_STRING))
                {
                    con.Open();
                    _log.Info("Connected To DataBase...");
                    //string sql = "Select * from PGEDATA.GEN_SUMMARY_STAGE ";
                    OleDbDataAdapter adp = new OleDbDataAdapter(sql, con);
                    adp.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error Occurred in Fetching the Data...");
                Console.ReadLine();
                Environment.Exit(0);

                return null;
            }
        }
        private static bool exportReportToText(DataTable DT_To_Export)
        {
            try
            {
                _log.Info("Export Data Initilaizing...");
                string toDay = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();

                /*To Clear Previous Content of Text File
                File.WriteAllText(OUTPUT_FILE + "_" + toDay + ".txt", String.Empty); */

                StreamWriter sw = new StreamWriter(OUTPUT_FILE + "ENOS_STATUS_REPORT_" + toDay + ".txt", true);
                int i;
                sw.WriteLine(Environment.NewLine);
                sw.WriteLine("********START********" + DateTime.Now.ToString());
                sw.WriteLine(Environment.NewLine);
                foreach (DataRow row in DT_To_Export.Rows)
                {
                    object[] arr = row.ItemArray;
                    for (i = 0; i < arr.Length - 1; i++)
                    {
                        sw.Write(arr[i].ToString().Trim() + "|");
                    }
                    sw.WriteLine(arr[i].ToString());
                }
                sw.WriteLine(Environment.NewLine);
                if (System.IO.Directory.Exists(OUTPUT_FILE))
                {
                    string[] files = System.IO.Directory.GetFiles(OUTPUT_FILE);

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        string fileName = System.IO.Path.GetFileName(s);
                        string destFile = System.IO.Path.Combine(Output_File_Archive, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                }
                else
                {
                    _log.Info("Source path does not exist!");
                }
                sw.WriteLine("********END********" + DateTime.Now.ToString());
                sw.Flush();
                sw.Close();
                _log.Info("Data Exported... Record Count: " + DT_To_Export.Rows.Count);
                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Error Exporting:" + ex.Message);
                Console.ReadLine();
                return false;
            }
        }
        private static DataTable runSelectSQL(string query,string conn_string)
        {
            try
            {
                DataTable dt = new DataTable();
                using (OleDbConnection con = new OleDbConnection(conn_string))
                {
                    con.Open();
                    OleDbDataAdapter adp = new OleDbDataAdapter(query, con);
                    adp.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                _log.Error("Error in Fetching Data. " + ex.Message);
                Console.ReadLine();
                Environment.Exit(0);
                return null;
            }
        }

        private static DataTable call_enos_procedure()
        {
            try
            {
                // m4jf edgisrearch 919 
                string ConnectionString = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());

               // using (OleDbConnection connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["connString"].ConnectionString))
                using (OleDbConnection connection = new OleDbConnection(ConnectionString))
                {
                    DataTable dt = new DataTable();
                    connection.Open();
                    OleDbDataAdapter adp = new OleDbDataAdapter();

                    OleDbCommand cmd = new OleDbCommand(Proc_Unique_Records, connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //OleDbParameter EQUIPMENTID = new OleDbParameter("@EQUIPMENTID", OleDbType.r);
                   // EQUIPMENTID.Direction = ParameterDirection.Output;

                   // EQUIPMENTID.Size = 200;
                   // cmd.Parameters.Add(EQUIPMENTID);
                    //OleDbParameter GUID = new OleDbParameter("@OBJECTID", OleDbType.Integer);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //cmd.Parameters.Add(GUID);
                    //OleDbParameter SPID = new OleDbParameter("@SPID", OleDbType.Numeric);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //cmd.Parameters.Add(SPID);
                    //OleDbParameter CURRENT_PROJECT = new OleDbParameter("@CURRENT_PROJECT", OleDbType.VarChar);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //CURRENT_PROJECT.Size = 200;
                    //cmd.Parameters.Add(CURRENT_PROJECT);
                    //OleDbParameter STATUS = new OleDbParameter("@STATUS", OleDbType.VarChar);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //STATUS.Size = 200;
                    //cmd.Parameters.Add(STATUS);
                    //OleDbParameter STATUS_MESSAGE = new OleDbParameter("@STATUS_MESSAGE", OleDbType.VarChar);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //STATUS_MESSAGE.Size = 200;
                    //cmd.Parameters.Add(STATUS_MESSAGE);
                    //OleDbParameter CGC = new OleDbParameter("@CGC", OleDbType.VarChar);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //CGC.Size = 200;
                    //cmd.Parameters.Add(CGC);
                    //OleDbParameter CIRCUITID = new OleDbParameter("@CIRCUITID", OleDbType.VarChar);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //CIRCUITID.Size = 200;
                    //cmd.Parameters.Add(CIRCUITID);
                    //OleDbParameter DATE_INSERTED = new OleDbParameter("@DATE_INSERTED", OleDbType.Date);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //cmd.Parameters.Add(DATE_INSERTED);
                    //OleDbParameter INSERTED_BY = new OleDbParameter("@INSERTED_BY", OleDbType.VarChar);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //INSERTED_BY.Size = 200;
                    //cmd.Parameters.Add(INSERTED_BY);
                    //OleDbParameter FINAL_STATUS = new OleDbParameter("@FINAL_STATUS", OleDbType.VarChar);
                    //EQUIPMENTID.Direction = ParameterDirection.Output;
                    //FINAL_STATUS.Size = 200;
                    //cmd.Parameters.Add(FINAL_STATUS);
                  //  cmd.ExecuteNonQuery();
                   // var myDataReader = cmd.ExecuteReader();
                  //  while (myDataReader.Read())
                  //      for (int i = 0; i < myDataReader.FieldCount; i++)
                  //          Console.WriteLine(myDataReader.GetName(i));
                    OleDbParameter input_dm_di_flag = new OleDbParameter("input_dm_di_flag", OleDbType.VarChar);
                    input_dm_di_flag.Size = 200;
                    input_dm_di_flag.Direction = ParameterDirection.Input;
                    input_dm_di_flag.Value="DM";
                    cmd.Parameters.Add(input_dm_di_flag);
                    adp.SelectCommand = cmd;
                    adp.Fill(dt);
                    connection.Close();
                    return dt;
                   // int value = cmd.ExecuteNonQuery();
                   
                    
                }
            }
            catch(Exception ex)
            {
                 _log.Error(ex.Message);
                 Environment.Exit(0);
                 return null;
            }
            

            }
        private static bool ExportAndUpdateFinalStatus(DataTable Dt_To_Update)
        {
            try
            {
                //using (OleDbConnection con = new OleDbConnection(CONNECTION_STRING))
                //{
                //    OleDbCommand cmd = new OleDbCommand();
                //    con.Open();
                //    cmd.Connection = con;
                   
                //    foreach (DataRow row in Dt_To_Update.Rows)
                //    {
                //        try
                //        {
                //            string sql = "UPDATE PGEDATA.ENOS_TO_SAP_STATUS SET FINAL_STATUS = 'FINAL' where EQUIPMENTID='" + row["EQUIPMENTID"] + "' and GUID='" + row["GUID"] + "' and SPID= " + Convert.ToDouble(row["SPID"])+" and DATE_INSERTED = TRUNC(sysdate)";
                //            cmd.CommandText = sql;
                //            cmd.ExecuteNonQuery();
                            
                //        }
                //        catch (Exception e)
                //        {
                //            Console.WriteLine(e.Message);
                //            Console.WriteLine("Error In Update Record with GUID: " + row["GUID"].ToString());
                //            Console.ReadLine();
                //        }
                //    }
                //    con.Close();
                    bool status  = exportReportToText(Dt_To_Update);
                    if (status)
                        return true;
                    else
                        return false;
                    
                //}
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }
        }
    
       
    }
}
