using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Oracle.DataAccess.Client;
using PGE.Interfaces.SAP.LoadData.Classes;
using System.IO;
using System.Configuration;

namespace PGE.Interfaces.SAP.LoadData
{
    class SendFilesToSAP
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        public static bool exportStart()
        {
            bool status = false;
            try
            {
                ReadConfigurations.ReadFromConfiguration();
                _log.Info("STARTING EXPORT...");
                InsertInENOSStatusForSAPReport.InsertData();
                //Call of Procedure that Return table without duplicate records
                DataTable Dt_Unique_Record = call_enos_procedure();
                //Fetching data From DataBase...
                //fetchSPIDFromEDGISIA2Q();
                // DataTable DT_Status_Data = fetchDataFromDB(ReadConfigurations.QUERY_SELECTALL);
                // DataTable DT_With_Unique_Record = removeDuplicateRows(DT_Status_Data, "PROGRAM_TYPE");
                // bool status = exportReportToText(DT_Status_Data);
                  status = ExportAndUpdateFinalStatus(Dt_Unique_Record);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return status ;
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
       
        private static DataTable fetchDataFromDB(string sql)
        {
            try
            {
                DataTable dt = new DataTable();
                //using (OracleConnection con = Common.GetDBConnection(ReadConfigurations.EDER_CONNECTIONSTRING))
                //{
                    //con.Open();
                    //Console.WriteLine("Connected To DataBase...");
                    //string sql = "Select * from PGEDATA.GEN_SUMMARY_STAGE ";
                    //OracleDataAdapter adp = new OracleDataAdapter(sql, con);
                    //adp.Fill(dt);
                    dt = cls_DBHelper_For_EDER.GetDataTableByQuery(sql);
                    return dt;
                //}
            }
            catch (Exception ex)
            {
                _log.Error("Error Occurred in Fetching the Data, function fetchDataFromDB" + ex.Message);
                Console.ReadLine();
                //Environment.Exit(0);
                //Fix by: m4jf -EDGISREARCH 416 - Interface Improvement ED15
                Environment.Exit(1);

                return null;
            }
        }
        private static bool exportReportToText(DataTable DT_To_Export)
        {
            try
            {
                _log.Info("Export Data Initilaizing...");
                string toDay = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();

                /*To Clear Previous Content of Text File
                File.WriteAllText(OUTPUT_FILE + "_" + toDay + ".txt", String.Empty); */

                StreamWriter sw = new StreamWriter(ReadConfigurations.OUTPUT_FILE + "ENOS_STATUS_REPORT_" + toDay + ".txt", true);
                int i;
                
                _log.Info("********START********" + DateTime.Now.ToString());
                
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
                if (System.IO.Directory.Exists(ReadConfigurations.OUTPUT_FILE))
                {
                    string[] files = System.IO.Directory.GetFiles(ReadConfigurations.OUTPUT_FILE);

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        string fileName = System.IO.Path.GetFileName(s);
                        string destFile = System.IO.Path.Combine(ReadConfigurations.Output_File_Archive, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                }
                else
                {
                    _log.Info("Source path does not exist!");
                }
                _log.Info("********END********" + DateTime.Now.ToString());
                sw.Flush();
                sw.Close();
                _log.Info("Data Exported... Record Count: " + DT_To_Export.Rows.Count);
                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Error Exporting, fnction exportReportToText:" + ex.Message);
                Console.ReadLine();
                return false;
            }
        }
        private static DataTable runSelectSQL(string query, string conn_string)
        {
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection con = new OracleConnection(conn_string))
                {
                    con.Open();
                    OracleDataAdapter adp = new OracleDataAdapter(query, con);
                    adp.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                _log.Error("Error in Fetching Data, function runSelectSQL: " + ex.Message);

                //Environment.Exit(0);
                // Fix by: m4jf -EDGISREARCH 416 - Interface Improvement ED15
                Environment.Exit(1);
                return null;
            }
        }

        private static DataTable call_enos_procedure()
        {
            try
            {
                //using (OracleConnection connection = Common.GetDBConnection(ReadConfigurations.EDER_CONNECTIONSTRING))
                //{
                    DataTable dt = new DataTable();
                  //  connection.Open();
                    //OracleDataAdapter adp = new OracleDataAdapter();

                    //OracleCommand cmd = new OracleCommand(ReadConfigurations.Proc_Unique_Records, connection);
                    //cmd.CommandType = CommandType.StoredProcedure;
                //DBCHNG 18: need to check
                    OracleParameter[]  Param=new OracleParameter[1];
                      Param[0]=new OracleParameter("Unique_recordset", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                    //cmd.Parameters.Add(param);
                    //adp.SelectCommand = cmd;
                    //adp.Fill(dt);
                    //connection.Close();
                    dt = cls_DBHelper_For_EDER.GetDataTableByStoredProcedure(ReadConfigurations.Proc_Unique_Records, Param);
                    return dt;
                    
               // }
            }
            catch (Exception ex)
            {
                _log.Error("Error Executing Procedure to calculate unique Records,function call_enos_procedure:" + ex.Message);
                //Environment.Exit(0);
                // Fix by: m4jf -EDGISREARCH 416 - Interface Improvement ED15
                Environment.Exit(1);
                return null;
            }


        }
        private static bool ExportAndUpdateFinalStatus(DataTable Dt_To_Update)
        {
            try
            {
                //using (OracleConnection con = Common.GetDBConnection(ReadConfigurations.EDER_CONNECTIONSTRING))
                //{
                    OracleCommand cmd = new OracleCommand();
                  //  con.Open();
                    //cmd.Connection = con;

                    foreach (DataRow row in Dt_To_Update.Rows)
                    {
                        try
                        {
                            string sql = "UPDATE PGEDATA.EDER_TO_SAP_STATUS SET FINAL_STATUS = 'FINAL' where EQUIPMENTID='" + row["EQUIPMENTID"] + "' and GUID='" + row["GUID"] + "' and SPID= " + Convert.ToDouble(row["SPID"]) + " and DATE_INSERTED = TRUNC(sysdate)";
                            //cmd.CommandText = sql;
                            //cmd.ExecuteNonQuery();
                            cls_DBHelper_For_EDER.UpdateQuery(sql);

                        }
                        catch (Exception e)
                        {
                            
                            _log.Error("Error In Update Record with GUID: " + row["GUID"].ToString() + "Error:" + e.Message);
                            
                        }
                    }
                   // con.Close();
                    bool status = exportReportToText(Dt_To_Update);
                    if (status)
                        return true;
                    else
                        return false;

              //  }
            }
            catch (Exception ex)
            {
                _log.Error("Error In Update Record,function ExportAndUpdateFinalStatus,Error:" + ex.Message);
                return false;
            }
        }
    }
}
