using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalysis;
using Miner.Interop.Process;
using ESRI.ArcGIS.Framework;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes;
using Oracle.DataAccess.Client;

namespace PGE.BatchApplication.IGPPhaseUpdate
{
    class MainClass
    {
        Version_Management VersionManagement = new Version_Management();
        Common CommonFuntions = new Common();
        DBHelper DBHelperClass = new DBHelper();
        MultiThreading MultiThreadingClass = new MultiThreading();
        MultiThreading_UpdateSession MultiThreadingClass_US = new MultiThreading_UpdateSession(); 
        public static bool ValidatedError=false;
        public static DataTable g_dtAllDetails = null;
        public static DataTable g_dtAllDetailsForDevice = null;
        public static DataTable g_DT_QAQC = null;
        public static DataTable g_DT_DMSError = null;
        public static IWorkspace m_SDEDefaultworkspace = null;
        public static Hashtable m_pHtable_ValuesToBeUpdated = null;
        public static Hashtable m_pHT_All_FeatCls_DownStream = null;
        public static String m_sFeatClass_ToBeUpdated;
        public static string m_sLogFileName = string.Empty;
        public static string Batch_Number = string.Empty;
        public static string sSerial_No = string.Empty;
        public static bool m_blIfChild = false;
        public static IMMPxApplication m_pxApplication;
        public static DateTime m_dStartTime;

        public bool StartProcess(bool blIfChild,string sCircuitID)
        {
           
            string json = string.Empty;
            string sLogFilePath = string.Empty;
            bool bOperaionSuccess = true;
            string sQueryRet = string.Empty;
            try
            {
                m_dStartTime = DateTime.Now;
              
                m_blIfChild = blIfChild;
              
                //Open Sde Workspace
                m_SDEDefaultworkspace = CommonFuntions.GetWorkspace(ReadConfigurations.SDEWorkSpaceConnString);
                if ((new GeoDBHelper()).CreateArcFMConnection() == true)
                {
                    if (m_pxApplication == null)
                    {
                        CommonFuntions._log.Error("Error in getting ARCFM application handler,Please refer the log file");
                        return false;
                    }
                }
                else
                {
                    CommonFuntions._log.Error("Error in getting ARCFM connection,Please refer the log file");
                    return false;
                }
              
                //multi threading
                if (blIfChild == false)
                {
                    
                    if (ConfigurationManager.AppSettings["DATALOADING_REQUIRED"].ToString().ToUpper() == ("true").ToUpper())
                    {
                        //JSON to DB                      
                        
                        (new ReadJSON()).LoadJSONData(ReadConfigurations.DAPHIEFilePath);
                    }

                    MainClass.Batch_Number = null;
                    //EGIS-923 : ADDING CONFIGURABLE KEY TO ACTIVATE OR INACTIVATE SEND REMINDER EMAILS TO MAPPER
                    if (ConfigurationManager.AppSettings["REMINDER_MAIL_TO_MAPPER"].ToString().ToUpper() == ("TRUE").ToUpper())
                    {
                        SendReminderMails();
                    }


                    MultiThreadingClass.ExecuteMultiThreading();
                  
                }
                else
                {
                    
                     if (string.IsNullOrEmpty(sCircuitID))
                    {
                        CommonFuntions._log.Error("CircuitId could not be passed as an argument," + sCircuitID);
                        return false;
                    }

                    sQueryRet = MultiThreadingClass.GetBatchNumber(ref sSerial_No, ref Batch_Number, sCircuitID);
                    //sSerial_No = "502"; Batch_Number = "502";
                     //log file circuit wise
                     sLogFilePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\LOG";
                    if (System.IO.Directory.Exists(sLogFilePath) == false)
                    { 
                        System.IO.Directory.CreateDirectory(sLogFilePath);
                    }
                    m_sLogFileName = sLogFilePath + "\\Logfile_B1_" + sCircuitID + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".csv";
                  
                    if ((!String.IsNullOrEmpty(sSerial_No)) && (!String.IsNullOrEmpty(Batch_Number)))
                    {
                        MultiThreadingClass.UpdateCurrentStatusFlag(sCircuitID, ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_InProgress, Batch_Number);

                        (new Process_IGPPhaseUpdate()).ProcessPhaseUpdation(sCircuitID, Batch_Number);
                        if (ConfigurationManager.AppSettings["STATISTICAL_REPORT"].ToString().Trim().ToUpper() == ("true").ToUpper())
                        {
                            //Generate Statistical Report
                            ExtracttheReportinVersion(sCircuitID, Batch_Number);
                        }
                    }
                    else
                    {
                        CommonFuntions.WriteLine_Error("Process is stopped as BATCHID or SERIAL_NO is null for CircuitID = " + sCircuitID + ",Query = " + sQueryRet);
                    }
                    MultiThreadingClass.UpdateCurrentStatusFlag(sCircuitID, ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_CycleCompleted, Batch_Number);
                }

            }
            catch (Exception exp)
            {
                bOperaionSuccess = false;
                CommonFuntions.WriteLine_Error(exp.Message + "   " + exp.StackTrace);
            }

            return bOperaionSuccess;

        }

        private void ExtracttheReportinVersion(string sCircuitID, string Batch_Number)
        {
           
            ICursor cursor_insert = null;
            IRowBuffer row_buffer = null;
            ITable Table_Statistical = null;
            string s_tablename_statistical = string.Empty;
            IWorkspaceEdit workspaceEdit = null;
            IFeatureWorkspace pFeatureWorkspace = null;
            IWorkspace pVersionWorkspace = null;
            DataRow rowInsert = null;
            int Flush_Cursor_Count = Convert.ToInt32(ConfigurationManager.AppSettings["THRESHOLD_FLUSHCURSOR_COUNT"].ToString());
            try
            {
                DataTable DT_StatisticalReport = new DataTable();

                string strquery = ConfigurationManager.AppSettings["STATISTICAL_REPORT_QUERY"].ToString().Trim() + "  and  a.batchid='" + Batch_Number + "'" +
                    " and status in('" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_USERQUEUE + "','" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_QAQCPassed + "')";
                DT_StatisticalReport = DBHelperClass.GetDataTable(strquery);
                if (DT_StatisticalReport.Rows.Count > 0)
                {
                    CommonFuntions.WriteLine_Info("Generate Statistical Report for CircuitID," + sCircuitID + "," + DateTime.Now);
                    int sSessionID = -1;

                    s_tablename_statistical = ConfigurationManager.AppSettings["STATISTICALREPORT_TABLENAME"].ToString();
                    pVersionWorkspace = (new Process_IGPPhaseUpdate()).GetVersionFromDaphieDetTable(sCircuitID, out sSessionID, Batch_Number);
                    if (pVersionWorkspace == null)
                    {
                        throw new Exception("Error in getting version for BatchID-" + Batch_Number);
                    }

                    workspaceEdit = (IWorkspaceEdit)pVersionWorkspace;
                    pFeatureWorkspace = (IFeatureWorkspace)pVersionWorkspace;
                    Table_Statistical = pFeatureWorkspace.OpenTable(s_tablename_statistical);
                    if (Table_Statistical != null)
                    {

                        #region TruncateTable
                        workspaceEdit.StartEditing(true);
                        workspaceEdit.StartEditOperation();


                        IQueryFilter pqueryfilter = new QueryFilterClass();
                        pqueryfilter.WhereClause = "BATCHID='" + Batch_Number + "'";
                        Table_Statistical.DeleteSearchedRows(pqueryfilter);

                      
                        //Archiving/Deleting old records
                        if (ConfigurationManager.AppSettings["ARCHIVING_STAT_REQUIRED"].ToString().ToUpper() == ("true").ToUpper())
                        {
                            DeleteFromStatsTable(sCircuitID, Table_Statistical);
                            CommonFuntions.WriteLine_Info("Archiving Completed for CircuitID," + sCircuitID + "," + DateTime.Now);
                        }

                        //App crash issue resolution 
                        workspaceEdit.StopEditOperation();
                        workspaceEdit.StopEditing(true);
                        System.Threading.Thread.Sleep(5000);

                        #endregion

                        #region Statistical Report Creation
                        workspaceEdit.StartEditing(true);
                        workspaceEdit.StartEditOperation();
                        cursor_insert = Table_Statistical.Insert(true);

                        //App crash issue resolution - Taking below line inside for loop 
                    
                        int irowcount = 0;
                        CommonFuntions.WriteLine_Info("Total Feature need to insert," + DT_StatisticalReport.Rows.Count.ToString() + "," + DateTime.Now);
                        for (int i = 0; i < DT_StatisticalReport.Rows.Count; i++)
                        {
                            irowcount++;

                            //App crash issue resolution 
                            row_buffer = Table_Statistical.CreateRowBuffer();
                            
                            rowInsert = DT_StatisticalReport.Rows[i];
                            for (int columncount = 0; columncount < rowInsert.ItemArray.Length; columncount++)
                            {
                                int indx = row_buffer.Fields.FindField(rowInsert.Table.Columns[columncount].ColumnName.ToString());
                                row_buffer.set_Value(indx, rowInsert.ItemArray[columncount].ToString());
                            }

                            cursor_insert.InsertRow(row_buffer);

                            if (irowcount % Flush_Cursor_Count == 0)
                            {
                                cursor_insert.Flush();
                                //App crash issue resolution 
                                //irowcount = 0;
                                CommonFuntions.WriteLine_Info("Cursor Flushed after feature count," + i + "," + DateTime.Now);
                            }
                        }

                        cursor_insert.Flush();
                        CommonFuntions.WriteLine_Info("Feature Inserted for CircuitID," + sCircuitID + "," + DateTime.Now);
                                               
                        workspaceEdit.StopEditOperation();
                        workspaceEdit.StopEditing(true);
                        #endregion Statistical Report Creation

                        CommonFuntions.WriteLine_Info("Statistical Report Generated Successfully for CircuitID," + sCircuitID + "," + DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception occurred while generating the statistical report for CircuitID," + sCircuitID + "," + DateTime.Now);
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
            }
            finally
            {
                if (cursor_insert != null) 
                {
                    Marshal.FinalReleaseComObject(cursor_insert);
                    cursor_insert = null;
                } 
            }
        }

        private bool DeleteFromStatsTable(string sCircuitID, ITable pTable_Statistical)
        {
            int iConfigCounter = 0;
            int iUniqueBatchIdCount = 0;
            int iCounter = 0;
            int iBatchId = 0;
            string sBatchId = null;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            ICursor pCursor = default(ICursor);
            IDataStatistics pDataStatistics = default(IDataStatistics);
            try
            {
                iConfigCounter = Convert.ToInt32(ConfigurationManager.AppSettings["ARCHIVE_STAT_REPORT_COUNTER"].ToString());

                //Minus by 1, as one batchid records are going to be addded after this function
                iConfigCounter--;

                pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = "CIRCUITID = '" + sCircuitID + "'";
                
                pCursor = pTable_Statistical.Search(pQueryFilter, false);
                pDataStatistics = new DataStatisticsClass();
                pDataStatistics.Field = "BATCHID";
                pDataStatistics.Cursor = pCursor;
                
                System.Collections.IEnumerator enumerator = pDataStatistics.UniqueValues;
                iUniqueBatchIdCount = pDataStatistics.UniqueValueCount;

                if (iConfigCounter >= iUniqueBatchIdCount)
                {
                    return true;
                }
                enumerator.Reset();

                int[] iArrayBatchId = new int[iUniqueBatchIdCount];
                
                while (enumerator.MoveNext())
                {
                    iBatchId = 0;
                    iBatchId = Convert.ToInt32(enumerator.Current.ToString());
                    iArrayBatchId[iCounter] = iBatchId;
                    iCounter++;
                }
                System.Array.Sort(iArrayBatchId);

                for (int i = 0; i < (iUniqueBatchIdCount - iConfigCounter); ++i)
                {
                    sBatchId = null;

                    sBatchId = iArrayBatchId[i].ToString();

                    pQueryFilter = new QueryFilterClass();
                    pQueryFilter.WhereClause = "BATCHID = '" + sBatchId + "'";
                    pTable_Statistical.DeleteSearchedRows(pQueryFilter);
                }

                CommonFuntions.WriteLine_Info("Records successfully deleted from Statistical Report table for CircuitID," + sCircuitID + "," + DateTime.Now);

                return true;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception occurred while deleting the records from statistical report table for CircuitID," + sCircuitID + "," + DateTime.Now);
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                return false;
            }
            finally
            {
                //App crash issue resolution
                if (pDataStatistics != null) 
                {                    
                    Marshal.FinalReleaseComObject(pDataStatistics);
                    pDataStatistics = null;
                }

                if (pCursor != null) 
                {                    
                    Marshal.FinalReleaseComObject(pCursor);
                    //App crash issue resolution
                    pCursor = null;
                }

                //App crash issue resolution
                if (pQueryFilter != null)
                {
                    Marshal.FinalReleaseComObject(pQueryFilter);
                    pQueryFilter = null;
                }
            }
        }

/// <summary>
/// Batch 2 Process starting Point
/// </summary>
/// <param name="blIfChild"></param>
/// <param name="sBatchId"></param>
/// <returns></returns>
        public bool StartProcess_UpdateSession(bool blIfChild, string sBatchId)
        {
            string sLogFilePath = string.Empty;
            bool bOperaionSuccess = true;
            string sCircuitID = string.Empty;
            string sSessionName = string.Empty;
            string sStatus = string.Empty;
            string sQuery = string.Empty;
            try
            {
                MainClass.Batch_Number = sBatchId;
                m_dStartTime = DateTime.Now;

                m_blIfChild = blIfChild;
                //Open Sde Workspace
                m_SDEDefaultworkspace = CommonFuntions.GetWorkspace(ReadConfigurations.SDEWorkSpaceConnString);
                if ((new GeoDBHelper()).CreateArcFMConnection() == true)
                {
                    if (m_pxApplication == null)
                    {
                        CommonFuntions._log.Error("Error in getting ARCFM application handler,Please refer the log file");
                        return false;
                    }
                }
                else
                {
                    CommonFuntions._log.Error("Error in getting ARCFM connection,Please refer the log file");
                    return false;
                }
               
                //multi threading
                if (blIfChild == false)
                {
                    MultiThreadingClass_US.ExecuteMultiThreading_UpdateSession();
                }
                else
                {
                    if (string.IsNullOrEmpty(sBatchId))
                    {
                        CommonFuntions._log.Error("BatchId could not be passed as an argument," + sBatchId);
                        return false;
                    }

                    Batch_Number = sBatchId;

                    MultiThreadingClass_US.GetDetailsFromDaphieDet(Batch_Number, ref sCircuitID,ref sSessionName,ref sStatus);
                    //log file circuit wise
                    sLogFilePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\LOG";
                    if (System.IO.Directory.Exists(sLogFilePath) == false)
                    {
                        System.IO.Directory.CreateDirectory(sLogFilePath);
                    }
                    m_sLogFileName = sLogFilePath + "\\Logfile_B2_" + sCircuitID + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".csv";

                    CommonFuntions.WriteLine_Info("USER_QUEUE re-validation process started for CircuitId," + sCircuitID + ",SessionName," + sSessionName + ",BatchNumber," + Batch_Number + "," + DateTime.Now);

                    sQuery = "UPDATE " + ReadConfigurations.DAPHIEFileDetTableName + " SET " + ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" +
                        ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_InProgress_Session + "' WHERE " + ReadConfigurations.DAPHIETablesFields.SessionNameField + " = '" + sSessionName + "'";
                    DBHelperClass.UpdateQuery(sQuery);


                    CommonFuntions.WriteLine_Info("Current Status," + sStatus + "," + DateTime.Now);
                    if (string.IsNullOrEmpty(sSessionName))
                    {
                        CommonFuntions.WriteLine_Error("Circuit could not processed as Session_Name is null in " + ReadConfigurations.DAPHIEFileDetTableName);
                        CommonFuntions.WriteLine_Info("USER_QUEUE re-validation process not completed successfully," + DateTime.Now);
                        return false;
                    }
                    if( (sStatus == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_QAQCPassed) || (sStatus == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CONFLICT)
                         || (sStatus == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_RECONCILEERROR)|| (sStatus == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_POSTERROR))
                    {
                        CommonFuntions.WriteLine_Info("Check Session state for those session which are in GDBM QUEUE(QAQC_PASSED,CONFLICT,RECONCILE_ERROR,POST_ERROR) is started= " + sSessionName );
                      
                        (new Process_IGPPhaseUpdate()).Check_Daphie_Det_ForQAQCPASSED_MT(sCircuitID, sSessionName);
                        CommonFuntions.WriteLine_Info("Check Session state for those session which are in GDBM QUEUE(QAQC_PASSED,CONFLICT,RECONCILE_ERROR,POST_ERROR) is completed = " + sSessionName);
                      
                    }
                    else if (sStatus == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_USERQUEUE)
                    {
                        CommonFuntions.WriteLine_Info("Check Session state for USER_QUEUE SESSION = " + sSessionName);
                        //Check DAPHIE DET file and find the status for those which are either in USER_QUEUE or in CONFLICT.
                        (new Process_IGPPhaseUpdate()).Check_Daphie_Det_ForDeletedOrUserQueue_MT(sCircuitID, sSessionName);
                        CommonFuntions.WriteLine_Info("Check Session state for USER_QUEUE SESSION = " + sSessionName);
                    }
                    else
                    {
                        CommonFuntions.WriteLine_Info("Session not re-validated as session state is other than QAQC_PASSED,CONFLICT,RECONCILE_ERROR,POST_ERROR and USER_QUEUE  = " + sSessionName);
                    }

                    sQuery = "UPDATE " + ReadConfigurations.DAPHIEFileDetTableName + " SET " + ReadConfigurations.DAPHIETablesFields.Current_StatusField +
                            " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_ProcessCompleted + "' WHERE " + 
                            ReadConfigurations.DAPHIETablesFields.SessionNameField + " = '" + sSessionName + "'";
                    DBHelperClass.UpdateQuery(sQuery);

                    CommonFuntions.WriteLine_Info("USER_QUEUE re-validation process completed," + DateTime.Now);

                }

            }
            catch (Exception exp)
            {
                bOperaionSuccess = false;
                CommonFuntions.WriteLine_Error(exp.Message + "   " + exp.StackTrace);
            }

            return bOperaionSuccess;

        }

        public bool SendReminderMails()
        {
            DataTable pDT = new DataTable();
            String sQuery = null;
            int iSessionID = 0;
            string CurrentOwner = null;
            string sSessionName = null;
            string sStatus = null;
            string sCircuitID = null;
            try
            {
                sQuery = "SELECT * FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN ('" +
                    ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_USERQUEUE + "','" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CONFLICT + "')";
                pDT = DBHelperClass.GetDataTable(sQuery);

                if ((pDT == null) || (pDT.Rows.Count == 0))
                {
                    return true;
                }

                for (int i = 0; i < pDT.Rows.Count; ++i)
                {
                    if (string.IsNullOrEmpty(pDT.Rows[i][ReadConfigurations.DAPHIETablesFields.SessionNameField].ToString()))
                    {
                        continue;
                    }
                    sSessionName = pDT.Rows[i][ReadConfigurations.DAPHIETablesFields.SessionNameField].ToString();
                    sStatus = pDT.Rows[i][ReadConfigurations.DAPHIETablesFields.StatusField].ToString();
                    sCircuitID = pDT.Rows[i][ReadConfigurations.DAPHIETablesFields.CircuitIdField].ToString();
                    
                    MainClass.Batch_Number = pDT.Rows[i][ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER].ToString();

                    iSessionID = Convert.ToInt32(sSessionName.Substring(3));//remove SN_
                    sQuery = "SELECT CURRENT_OWNER FROM " + ReadConfigurations.Process_TableName.MM_SessionTableName + " WHERE SESSION_ID = '" + iSessionID + "' AND HIDDEN = 0";
                    DataTable pDTPOSTED = DBHelperClass.GetDataTable(sQuery);
                    if ((pDTPOSTED != null) && (pDTPOSTED.Rows.Count > 0))
                    {
                        CurrentOwner = pDTPOSTED.Rows[0].ItemArray[0].ToString();

                        if (CurrentOwner == ReadConfigurations.ChangeUserName)
                        {
                            //send mail
                            CommonFuntions.WriteLine_Info("Send Reminder Mail Process is Started for CircuitID " + sCircuitID + " at," + DateTime.Now);
                            CommonFuntions.SendMailToMapper_Reminder(sStatus, string.Empty, iSessionID, sCircuitID);
                            CommonFuntions.WriteLine_Info("Send Reminder Mail Process is Completed for CircuitID " + sCircuitID + " at," + DateTime.Now);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                CommonFuntions._log.Error(ex.Message + "   " + ex.StackTrace);

                return false;
            }
        }

        
    }
}
