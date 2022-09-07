using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Data;
using log4net;
using System.Reflection;


namespace ChangeDetection_NP
{
    public class ProcessData
    {
        public static string ConnectionStr6Q = ConfigurationManager.ConnectionStrings["ConnectionStr6Q"].ConnectionString;
        public static string ConnectionStr1D = ConfigurationManager.ConnectionStrings["ConnectionStr1D"].ConnectionString;
        public static string queryGetData = ConfigurationManager.AppSettings["QueryGetData"];
        public Log4NetLogger logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetection_NP.log4net.config");

        public bool GetDataFrmDB()
        {
            Console.WriteLine("Test");
            Console.WriteLine(MethodBase.GetCurrentMethod().DeclaringType);
            Console.WriteLine("GetDatafromDB with Logger");

            using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(ConnectionStr1D))
            {
                try
                {
                    connection.Open();
                    logger.Info("Connection Opened");
                    OracleCommand cmdGetData = new OracleCommand(queryGetData, connection);
                    OracleDataAdapter da = new OracleDataAdapter(cmdGetData);
                    DataTable dtNetworkProtector = new DataTable();
                    da.Fill(dtNetworkProtector);
                    logger.Info("Row Count of Network Protector table fetched from T1D database: " + dtNetworkProtector.Rows.Count);
                    Console.WriteLine("Row Count of Network Protector table fetched from T1D database: " + dtNetworkProtector.Rows.Count);
                    InsertInDatabase(dtNetworkProtector);
                    Console.WriteLine("End of funciton");
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    return false;
                }
            }
            return true;
        }

        public void InsertInDatabase(DataTable dtNetworkProtector)
        {
            using (Oracle.DataAccess.Client.OracleConnection objConn = new Oracle.DataAccess.Client.OracleConnection(ConnectionStr6Q))
            {
                objConn.Open();
                int insertedRecordCount = 0;
                for (int i = 0; i < dtNetworkProtector.Rows.Count; i++)
                {
                    try
                    {
                        OracleCommand objCmd = new OracleCommand();
                        objCmd.Connection = objConn;
                        objCmd.CommandText = "SM_CHANGE_DETECTION_PKG.SP_NETWORK_PROTECTOR_DETECTION";
                        objCmd.CommandType = CommandType.StoredProcedure;
                        objCmd.Parameters.Add("I_Global_id_Current", OracleDbType.Varchar2).Value = dtNetworkProtector.Rows[i]["GLOBALID"].ToString();
                        objCmd.Parameters.Add("I_reason_type", OracleDbType.Varchar2).Value = "I";
                        objCmd.Parameters.Add("I_feature_class_name", OracleDbType.Varchar2).Value = "EDGIS.NetworkProtector";
                        objCmd.Parameters.Add("I_feature_class_subtype", OracleDbType.Int32).Value = null;
                        objCmd.Parameters.Add("I_operating_num", OracleDbType.Varchar2).Value = dtNetworkProtector.Rows[i]["OPERATINGNUMBER"].ToString();
                        objCmd.Parameters.Add("I_Global_id_Previous", OracleDbType.Varchar2).Value = null;
                        objCmd.Parameters.Add("I_Division", OracleDbType.Varchar2).Value = dtNetworkProtector.Rows[i]["DIVISION"].ToString();
                        objCmd.Parameters.Add("I_District", OracleDbType.Varchar2).Value = dtNetworkProtector.Rows[i]["DISTRICT"].ToString();
                        objCmd.Parameters.Add("I_Control_type_code", OracleDbType.Varchar2).Value = null;
                        objCmd.Parameters.Add("I_Switch_type_code", OracleDbType.Varchar2).Value = null;
                        objCmd.Parameters.Add("I_Bank_code", OracleDbType.Int32).Value = null;
                        objCmd.ExecuteNonQuery();
                        insertedRecordCount++;
                        // Console.WriteLine("Record inserted in Settings database for GlobalID: " + dtNetworkProtector.Rows[i]["GLOBALID"].ToString());
                        logger.Info("Record inserted in Settings database for GlobalID: " + dtNetworkProtector.Rows[i]["GLOBALID"].ToString());
                    }
                    catch (Exception ex)
                    {
                        insertedRecordCount--;
                        logger.Error("Exception for GlobalID: " + dtNetworkProtector.Rows[i]["GLOBALID"].ToString() + "Exception is: " + ex.Message);
                    }
                }
                logger.Info("Total records inserted in Setting database: " + insertedRecordCount);
                objConn.Close();
            }
        }
        // }
    }
}
