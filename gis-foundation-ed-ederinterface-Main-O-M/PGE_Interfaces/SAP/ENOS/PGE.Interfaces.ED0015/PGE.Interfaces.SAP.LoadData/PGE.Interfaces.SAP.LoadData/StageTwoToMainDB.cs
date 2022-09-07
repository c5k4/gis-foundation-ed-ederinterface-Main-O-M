using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ESRI.ArcGIS.esriSystem;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using PGE.Interfaces.SAP.LoadData.Classes;

namespace PGE.Interfaces.SAP.LoadData
{
    public class StageTwoToMainDB
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        public static string successComment = default;

        public static bool StartMainProcess()
        {
            bool processSuccessful = false;
            DataTable dtRecords = null;
            string strConnectionString = null;
            string strSqlQuery = null;
            DataTable dtRecordsToUpdateInStageTable = null;
            bool bVersionPosted = false;
           
            try
            {
                // Create SQL connection  to EDGIS to read Stage 2 Gen and Eqp tables 
               // string[] con = ReadConfigurations.GetCommaSeparatedList(ReadConfigurations.EDConnection, new string[4]);
                //strConnectionString = DBHelper_Settings.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);
                ReadConfigurations.ReadFromConfiguration();
                
                //// Create SDE conn  to EDGIS to insert data from Stage 2 Gen to Generationinfo tables 
                strSqlQuery = ReadConfigurations.GetValue(ReadConfigurations.QueryToGetRecordsFromGenInfoStage);
                // dtRecords = DBHelper_Settings.GetDataTableFromDataBase(strSqlQuery, strConnectionString);
                dtRecords = cls_DBHelper_For_EDER.GetDataTableByQuery(strSqlQuery);
                string[] args = Environment.GetCommandLineArgs();
                if (args[1].ToUpper() == "PREPOSTPROCESS")
                {

                    _log.Info("******Starting Process to Insert in Main from Stage2******");
                    _log.Info("Getting Records from Stage2 Generation Info");
                    if (dtRecords.Rows.Count > 0)
                    {
                        dtRecordsToUpdateInStageTable = Common.GetDataTableWithSchema();

                        processSuccessful = VersionOperations.StartProcess(dtRecords, ref dtRecordsToUpdateInStageTable, ref bVersionPosted);

                        // - Dont need this function now , GLOBALID already captured.
                        //processSuccessful = FinalizeDataTableToUpdateStageTablesBack(ref dtRecordsToUpdateInStageTable);
                        if (!processSuccessful)
                        {
                            _log.Info("Error in Editing Version.. Exiting Process");

                            // m4jf edgisrearch 416 - update in interface execution summary table
                            Program.comment = "Error in Editing Version.. Exiting Process";                           
                            processSuccessful = false;
                        }

                      

                    }
                    else
                    {
                        _log.Info("No Record Found in Generation Info Stage Table..Logging Final Statistics");
                        
                        LogFinalStatistics();
                        _log.Info("No Record Found in Generation Info Stage Table");
                        // m4jf edgisrearch 416 ed15 improvements 
                        // Commented code as now GIS will pull data from SAP through EI .
                       // LoadDataFromPrimaryStagetoSettings.moveTextFileToArchive();
                        
                    }
                }
                else if (args[1].ToUpper() == "POSTPROCESS")
                {
                   bool isSuccess =  VersionOperations.PostProcess();
                   if (isSuccess)
                   {
                      
                           //Change inside the fn to update Y , N 
                           // Use dtRecordsToUpdateInStageTable and update stage 2 generation and settings tables 
                           
                           //UpdateStageTablesAfterInsertingDataInMainTables(dtRecordsToUpdateInStageTable, 'Y');
                           processSuccessful = true;
                           SAPDailyInterfaceProcess.ProcessDataAfterPost();
                           LogFinalStatistics();
                           Program.ExecutionSummary(Program.startTime, "ED15 interface POSTPROCESS completed successfully ." + StageTwoToMainDB.successComment, Argument.Done);
                    }
                   else
                   {
                       _log.Info("Version Posting Failed...Updating Status in Stage Tables");
                       // If version posting is failed 
                       //UpdateStageTablesAfterInsertingDataInMainTables(dtRecordsToUpdateInStageTable, 'N');
                       processSuccessful = false;

                        // m4jf edgisrearch - 416 ed15 improvements
                        // Send Email notification for ed 15 post job fail .
                        Common.SendMail(ReadConfigurations.MAIL_BODY_POSTFAIL, ReadConfigurations.MAIL_SUBJECT_POSTFAIL);
                        Program.comment = "ED15 interface POSTPROCESS  Version Posting Failed .";
                    }
                 
                }
            }
            catch (Exception exp)
            {
                _log.Error("ErroR in function StartMainProcess: " + exp.Message);
                processSuccessful = false;
                
            }
            
            return processSuccessful;
        }
        public static bool UpdateStageTablesAfterInsertingDataInMainTables(DataTable argdtRecordsToUpdateInStageTable, char argSessionPosted)
        {
            bool bOperation = true;
            string sqlQueryToUpdateGenInfoStage = null;
            string sqlQueryToUpdateGenerationStage = null;
            //DataRow[] pSelectedRows = null;
            string strAction = null;
           
            try
            {
                //using (OracleConnection oracleconn = new OracleConnection(argConnectionString))
                //{
                //    oracleconn.Open();

                //Updating generationinfo stage table after updating data in main tables ------
                //pSelectedRows = argdtRecordsToUpdateInStageTable.Select("ACTION='I'");

                foreach (DataRow pRow in argdtRecordsToUpdateInStageTable.Rows)
                {
                    try
                    {


                        strAction = pRow.Field<string>(ReadConfigurations.GetValue(ReadConfigurations.DTCol_Action));

                        if (strAction == "I" && argSessionPosted == 'Y')
                        {
                            sqlQueryToUpdateGenInfoStage = "UPDATE " + ReadConfigurations.GetValue(ReadConfigurations.GenerationInfoStageTableName) + " SET " + ReadConfigurations.GetValue(ReadConfigurations.Col_GlobalID) + "='" + pRow[ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDMain)] + "'," + ReadConfigurations.GetValue(ReadConfigurations.Col_Updated_in_main) + "='" + argSessionPosted + "' where " + ReadConfigurations.GetValue(ReadConfigurations.Col_ServicePointGuid) + "='" + pRow[ReadConfigurations.GetValue(ReadConfigurations.DTCol_SPGuidStage)] + "'";
                            sqlQueryToUpdateGenerationStage = "UPDATE " + ReadConfigurations.GetValue(ReadConfigurations.SMGenerationStageTableName) + " SET " + ReadConfigurations.GetValue(ReadConfigurations.Col_Updated_in_ED_main) + "='" + argSessionPosted + "'," + ReadConfigurations.GetValue(ReadConfigurations.Col_GlobalID_Settings) + "='" + pRow[ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDMain)] + "' where " + ReadConfigurations.GetValue(ReadConfigurations.Col_GlobalID_Settings) + "='" + pRow[ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDStage)] + "'";
                          
                        }
                        else if (strAction == "I" && argSessionPosted == 'N')
                        {
                            sqlQueryToUpdateGenInfoStage = "UPDATE " + ReadConfigurations.GetValue(ReadConfigurations.GenerationInfoStageTableName) + " SET " + ReadConfigurations.GetValue(ReadConfigurations.Col_Updated_in_main) + "='" + argSessionPosted + "' where " + ReadConfigurations.GetValue(ReadConfigurations.Col_ServicePointGuid) + "='" + pRow[ReadConfigurations.GetValue(ReadConfigurations.DTCol_SPGuidStage)] + "'";
                            sqlQueryToUpdateGenerationStage = "UPDATE " + ReadConfigurations.GetValue(ReadConfigurations.SMGenerationStageTableName) + " SET " + ReadConfigurations.GetValue(ReadConfigurations.Col_Updated_in_ED_main) + "='" + argSessionPosted + "'," + ReadConfigurations.GetValue(ReadConfigurations.Col_GlobalID_Settings) + "='" + pRow[ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDMain)] + "' where " + ReadConfigurations.GetValue(ReadConfigurations.Col_GlobalID_Settings) + "='" + pRow[ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDStage)] + "'";
                           
                        }
                        else if (strAction == "U" || strAction == "D")
                        {
                            sqlQueryToUpdateGenInfoStage = "UPDATE " + ReadConfigurations.GetValue(ReadConfigurations.GenerationInfoStageTableName) + " SET " + ReadConfigurations.GetValue(ReadConfigurations.Col_Updated_in_main) + "='" + argSessionPosted + "' where " + ReadConfigurations.GetValue(ReadConfigurations.Col_GlobalID) + "='" + pRow[ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDStage)] + "'";
                            sqlQueryToUpdateGenerationStage = "UPDATE " + ReadConfigurations.GetValue(ReadConfigurations.SMGenerationStageTableName) + " SET " + ReadConfigurations.GetValue(ReadConfigurations.Col_Updated_in_ED_main) + "='" + argSessionPosted + "' where " + ReadConfigurations.GetValue(ReadConfigurations.Col_GlobalID_Settings) + "='" + pRow[ReadConfigurations.GetValue(ReadConfigurations.DTCol_GlobalIDMain)] + "'";
                           
                        }

                        int iresult = cls_DBHelper_For_EDER.UpdateQuery(sqlQueryToUpdateGenInfoStage);

                        if (iresult !=-1)
                        {
                            cls_DBHelper_For_EDER.UpdateQuery(sqlQueryToUpdateGenerationStage);
                            
                        }

                        

                    }
                    catch (Exception exp)
                    {
                        // Fix by: m4jf -EDGISREARCH 416 - Interface Improvement ED15
                        _log.Error(exp.Message);

                        // throw;
                    }
                }

            }
            catch (Exception exp)
            {
                // throw;
                // Fix by: m4jf -EDGISREARCH 416 - Interface Improvement ED15
                _log.Error(exp.Message);
                bOperation = false;
            }
            finally
            {

            }
            return bOperation;
        }

        public static void LogFinalStatistics()
        {
            try
            {
                string qry1 = "Select * from pgedata.gen_summary_stage";
                DataTable Dt_Summary_Records = cls_DBHelper_For_EDER.GetDataTableByQuery(qry1);
                int total_summary = Dt_Summary_Records.Rows.Count;
                int total_pass = Dt_Summary_Records.Select("STATUS='S'").Count();
                int total_fail = Dt_Summary_Records.Select("STATUS='F'").Count();
                //ENOS2SAP_PI Logs improvements
                int totat_si = Dt_Summary_Records.Select("ACTION='I'").Count();
                int totat_su = Dt_Summary_Records.Select("ACTION='U'").Count();
                int totat_sd = Dt_Summary_Records.Select("ACTION='D'").Count();
                int total_unrz = total_summary - (total_pass + total_fail);
                _log.Info("Total Summary Records: " + total_summary + ", Insert:"+totat_si+", Update:"+totat_su+", Delete:"+totat_sd+", Pass: " + total_pass + ", Fail:" + total_fail + ", Status Unknown:" + total_unrz);

                // m4jf edgisrearch 416 

                successComment = "Total Summary Records: " + total_summary + ", Insert:" + totat_si + ", Update:" + totat_su + ", Delete:" + totat_sd + ", Pass: " + total_pass + ", Fail:" + total_fail + ", Status Unknown:" + total_unrz;
                foreach (DataRow row in Dt_Summary_Records.Rows)
                {
                    _log.Info("Action: "+row["ACTION"].ToString()+", GenerationGUID: " + row["GUID"].ToString() + ", ServicePointId:" + row["SERVICE_POINT_ID"].ToString() + ", STATUS: " + row["STATUS"].ToString() + ", Status Message: " + row["STATUS_MESSAGE"].ToString());
                }

                string qry2 = "Select * from pgedata.gen_equipment_stage";
                DataTable Dt_Eqp_Records = cls_DBHelper_For_EDER.GetDataTableByQuery(qry2);
                int total_eqp = Dt_Eqp_Records.Rows.Count;
                int total_eqp_pass = Dt_Eqp_Records.Select("STATUS='S'").Count();
                int total_eqp_fail = Dt_Eqp_Records.Select("STATUS='F'").Count();
                int total_eqp_warn = Dt_Eqp_Records.Select("STATUS='W'").Count();
                //ENOS2SAP_PI Logs improvements
                int totat_ai = Dt_Eqp_Records.Select("ACTION='I'").Count();
                int totat_au = Dt_Eqp_Records.Select("ACTION='U'").Count();
                int totat_ad = Dt_Eqp_Records.Select("ACTION='D'").Count();
                int total_eqp_unrz = total_eqp - (total_eqp_pass + total_eqp_fail+total_eqp_warn);
                _log.Info("Total Equipment Records: " + total_eqp + ", Insert:" + totat_ai + ", Update:" + totat_au + ", Delete:" + totat_ad + ",  Pass: " + total_eqp_pass + ", Fail:" + total_eqp_fail + ", Warnings: "+total_eqp_warn+" Status Unknown:" + total_eqp_unrz);

                // m4jf edgisrearch 416 - get remarks for interface execution summary table for records processed .
                successComment+= " Total Equipment Records: " + total_eqp + ", Insert:" + totat_ai + ", Update:" + totat_au + ", Delete:" + totat_ad + ",  Pass: " + total_eqp_pass + ", Fail:" + total_eqp_fail + ", Warnings: " + total_eqp_warn + " Status Unknown:" + total_eqp_unrz;


                foreach (DataRow row in Dt_Eqp_Records.Rows)
                {
                    _log.Info("Action: " + row["ACTION"].ToString() + ", ServicePointId: " + row["SERVICE_POINT_ID"].ToString() + ", STATUS: " + row["STATUS"].ToString() + ", Status Message: " + row["STATUS_MESSAGE"].ToString());
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error in Function LogFinalStatisitcs: " + ex.Message);
                Program.comment = "Error in Function LogFinalStatisitcs: " + ex.Message;
            }

        }

      
    }
}
