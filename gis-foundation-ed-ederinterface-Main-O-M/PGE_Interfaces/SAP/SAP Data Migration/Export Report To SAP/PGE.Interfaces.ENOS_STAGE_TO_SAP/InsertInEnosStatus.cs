using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.ENOS_STAGE_TO_SAP
{
    class InsertInEnosStatus
    {
        public static string Table_Servicepoint = string.Empty;
        public static string Table_PrimaryMeter = string.Empty;
        public static string Table_Transformer = string.Empty;
        public static string Table_Gen_Eqp_Stg = string.Empty;

        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,"pge.log4net.config");
        public static void InsertData()
        {
            //Call Procedure to Insert From StageTables
            ExportToSAP.readFromConfiguration();
            _log.Info("STARTING PROCESS AT " + DateTime.Now);
            string proc_stage_insrt = ConfigurationManager.AppSettings["Proc_Stage"];
            Take_Backup_Eder_Status_insert();
            // m4jf edgisrerch 414
            string ConnectionString = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
            // using (OleDbConnection connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["connString_pgedata"].ConnectionString))
            using(OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = ExportToSAP.Qry_Clear_Curr_Data;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _log.Info("Error Occurred " + ex.Message);
                }
            }
            bool status = call_procedure(proc_stage_insrt, 1);
            if (status)
            {
                _log.Info("Successfully Inserted Data in Status Table");
                //fetchSPIDFromEDGISIA2Q();
            }
            else
                _log.Info("Error In Procedure: " + proc_stage_insrt);

          //  string proc_changed_cid_insrt = ConfigurationManager.AppSettings["Proc_Changed_Cid"];

          //  bool status_cid = call_procedure(proc_changed_cid_insrt, 0);
            //if (status_cid)
            //    Console.WriteLine("Successfully Inserted Data From Changed CID");
            //else
            //    Console.WriteLine("Error In Procedure: " + proc_changed_cid_insrt);
            ////Fetching changed SPID and Insert them to ENOS_TO_SAP_STATUS Table
            //fetchSPIDFromEDGISIA2Q();

        }
        
        private static bool call_procedure(string procedure_name, int obid)
        {
            try
            {
                //Table_Servicepoint = ConfigurationManager.AppSettings["Table_Servicepoint"];
                //Table_PrimaryMeter = ConfigurationManager.AppSettings["Table_PrimaryMeter"];
                //Table_Transformer = ConfigurationManager.AppSettings["Table_Transformer"];
                //Table_Gen_Eqp_Stg = ConfigurationManager.AppSettings["Table_Gen_Eqp_Stg"];

                // m4jf edgisrearch 919 
                string ConnectionString = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
                //using (OleDbConnection connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["connString_pgedata"].ConnectionString))
                using (OleDbConnection connection = new OleDbConnection(ConnectionString))
                {
                    connection.Open();
                    OleDbCommand cmd = new OleDbCommand(procedure_name, connection);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    //  cmd.Parameters.Add("@SERVICEPOINT", OleDbType.VarChar).Value = Table_Servicepoint;
                    //  cmd.Parameters.Add("@PRIMARYMETER", OleDbType.VarChar).Value = Table_PrimaryMeter;
                    //  cmd.Parameters.Add("@TRANSFORMER", OleDbType.VarChar).Value = Table_Transformer;
                    //  cmd.Parameters.Add("@GEN_EQUIPMENT_STAGE", OleDbType.VarChar).Value = Table_Gen_Eqp_Stg;
                    OleDbParameter STATUS = new OleDbParameter("@STATUS", OleDbType.VarChar);
                    STATUS.Direction = ParameterDirection.Output;
                    STATUS.Size = 20;
                    cmd.Parameters.Add(STATUS);
                    if (obid != 0)
                    {
                        cmd.Parameters.Add("input_dm_di_flag", OleDbType.VarChar).Value = "DM";
                    }

                    int value = cmd.ExecuteNonQuery();
                    string status = cmd.Parameters["@STATUS"].Value.ToString();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return false;
            }

        }

        
        public static void fetchSPIDFromEDGISIA2Q()
        {
            try
            {
                // m4jf edgisrearch 919 
                string ConnectionString = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
                //OleDbConnection con1 = new OleDbConnection(ConfigurationManager.ConnectionStrings["connstring_pgedata"].ConnectionString);
                OleDbConnection con1 = new OleDbConnection(ConnectionString);
                DataTable DT_DataToExport = new DataTable();
                string sql1 = "Select * from PGEDATA.ENOS_TO_SAP_STATUS where date_inserted=trunc(sysdate)";

                con1.Open();
                OleDbDataAdapter adp1 = new OleDbDataAdapter(sql1, con1);
                adp1.Fill(DT_DataToExport);

                foreach (DataRow row in DT_DataToExport.Rows)
                {
                    try
                    {

                        // m4jf edgisrearch 919 
                        string ConnectionStr = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDAUX_ConnectionStr"].ToUpper());
                        string sql2 = "select sm_genrtr.SAP_EQUIPMENT_ID from (select sm_pt.ID ID from (select sm_gen.ID ID from edsett.sm_generation sm_gen where global_id='" + row["GUID"] + "') gen_id, edsett.sm_protection sm_pt" +
                                    "where sm_pt.parent_id = gen_id.ID) pt_id, edsett.sm_generator sm_genrtr where sm_genrtr.protection_id = pt_id.ID";
                        DataTable Dt_EQPID = new DataTable();

                        // m4jf edgisrearch 919
                        //OleDbConnection con = new OleDbConnection(ConfigurationManager.ConnectionStrings["connString_EDGISA2Q"].ConnectionString);
                        OleDbConnection con = new OleDbConnection(ConnectionStr);
                        con.Open();
                        OleDbDataAdapter adp = new OleDbDataAdapter(sql2, con);
                        adp.Fill(Dt_EQPID);
                        foreach (DataRow row1 in Dt_EQPID.Rows)
                        {
                            string sql = "update pgedata.enos_to_sap_status  set equipmentid="+row1["SAP_EQUIPMENT_ID"]+" WHERE ROWID IN (SELECT   MIN (ROWID) "+
                              "FROM pgedata.enos_to_sap_status where guid= '"+row["GUID"]+"' and equipmentid is null GROUP BY guid) ";
                            OleDbCommand cmd = new OleDbCommand(sql, con1);
                            int status = cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message);
                    }
                    con1.Close();
                }
                
                
              
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
            }
        }

        public static void Truncate_Eder_Status_insert()
        {

            string truncate_stage_insrt = "Delete from eder_to_sap_status";

            // m4jf edgisrearch 919 
            string ConnectionString = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
            //using (OleDbConnection connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["connString_pgedata"].ConnectionString))
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = truncate_stage_insrt;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _log.Info("Error Occurred " + ex.Message);
                }
            }
        }
        public static void Take_Backup_Eder_Status_insert()
        {

           
            string backup_stage_insrt = "create table eder_status_"  + DateTime.Now.ToString("yyyyMMdd")+ DateTime.Now.Hour.ToString()+DateTime.Now.Minute.ToString()+" as select * from eder_to_sap_status";
            // m4jf edgisrearch 919 
            string ConnectionString = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());

            //using (OleDbConnection connection = new OleDbConnection(ConfigurationManager.ConnectionStrings["connString_pgedata"].ConnectionString))
            using (OleDbConnection connection = new OleDbConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = backup_stage_insrt;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    Truncate_Eder_Status_insert();
                }
                catch (Exception ex)
                {
                    _log.Info("Error Occurred " + ex.Message);
                }
            }
        }
    }
}
