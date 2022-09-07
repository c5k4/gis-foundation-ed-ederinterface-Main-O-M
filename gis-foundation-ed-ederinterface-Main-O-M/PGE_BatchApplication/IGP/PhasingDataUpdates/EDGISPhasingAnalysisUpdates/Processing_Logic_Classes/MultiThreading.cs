using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Threading;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes
{
    class MultiThreading
    {
        private List<Process> m_pList_ChildProcessesRunning = new List<Process>();
        Common CommonFuntions = new Common();
        DBHelper DBHelperClass = new DBHelper();
        public int m_iTotalFeeders = 0;

        public bool ExecuteMultiThreading()
        {
            int iExistingCount = 0;
            int iMaxNumOfProcess = 0;
            int iThresholdCount = 0;
            int iAllowedCount = 0;
            int iProcessCountCurrent = 0;
            string sWhereClause = null;
            try
            {
                iMaxNumOfProcess = ReadConfigurations.MaxNumProcessors;
                iThresholdCount = ReadConfigurations.ThresholdProcessCount;

                iExistingCount = CheckPendingSessionCount();
                if (iThresholdCount > iExistingCount)
                {
                    iAllowedCount = iThresholdCount - iExistingCount;

                    if (iAllowedCount >= iMaxNumOfProcess)
                    {
                        iProcessCountCurrent = iMaxNumOfProcess;
                    }
                    else
                    {
                        iProcessCountCurrent = iAllowedCount;
                    }
                }
                else
                {
                    //if exiting versions are greater than r equal to threshold, no need to process more
                    iProcessCountCurrent = 0;
                }

                if (iProcessCountCurrent > 0)
                {
                    m_iTotalFeeders = AssignCircuitIds(ref sWhereClause, iProcessCountCurrent);

                    //Write Skipped CircuitIds in log
                    WriteSkippedCircuitIds(m_iTotalFeeders, sWhereClause, iProcessCountCurrent);

                    ////Copy data from another db to pgedata
                    if (ConfigurationManager.AppSettings["DATAMART_LOADING_REQUIRED"].ToString().ToUpper() == ("true").ToUpper())
                    {
                        UpdateFeederNetworkTraceTable();
                    }

                    ExecuteTracingInChildren();
                }
                else
                {
                    CommonFuntions.WriteLine_Info("No process could be started," + DateTime.Now);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private int CheckPendingSessionCount()
        {
            String sQuery = null;
            DataTable pDataTable = new DataTable();
            string sStatusToBeSkipped = null;
            int iResult = 0;
            try
            {
                sStatusToBeSkipped = ReadConfigurations.Status_To_Be_Skipped;

                sQuery = "SELECT COUNT(*) FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.StatusField + 
                    " IN (" + sStatusToBeSkipped + ")";

                pDataTable = DBHelperClass.GetDataTable(sQuery);
                if ((pDataTable != null) && (pDataTable.Rows.Count > 0))
                {
                    iResult = Convert.ToInt32(pDataTable.Rows[0].ItemArray[0].ToString());
                }
                return iResult;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                return iResult;
            }
        }


        private bool UpdateFeederNetworkTraceTable()
        {
            DBHelper DBHelperClass = new DBHelper();
            DataTable pDataTable = new DataTable();
            string sQuery = null;
            string sConnectionString = null;
            string sCircuitId = null;
            string sAllCircuitIds = null;
            try
            {
                sQuery = "SELECT DISTINCT " + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " +
                        ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_Assigned + "' AND " +
                        ReadConfigurations.DAPHIETablesFields.StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Loaded + "'";
                pDataTable = DBHelperClass.GetDataTable(sQuery);

                if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                {
                    return false;
                }

                for (int i = 0; i < pDataTable.Rows.Count; i++)
                {
                    sCircuitId = null;
                    sCircuitId = pDataTable.Rows[i][0].ToString();

                    sAllCircuitIds = sAllCircuitIds + "'" + sCircuitId + "',";
                }
                sAllCircuitIds = sAllCircuitIds.Substring(0, sAllCircuitIds.Length - 1);

                CommonFuntions.WriteLine_Info("Feeder Network Table update started for CircuitIDs," + sAllCircuitIds + "," + DateTime.Now);
               
                // M4JF EDGISREARCH 919
                //sConnectionString = ConfigurationManager.AppSettings["OracleConnectionString_Datamart"];
                sConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr_SDE"].ToUpper());

                //To fetch data from database to DataTable
                sQuery = "SELECT * FROM " + ReadConfigurations.FeederNetworkTraceTableName_DataMart + " WHERE " + "FEEDERID" + " IN (" + sAllCircuitIds + ") OR FEEDERFEDBY IN (" + sAllCircuitIds + ")";
                pDataTable = DBHelperClass.GetDataTableFromConnectionstring(sConnectionString, sQuery);

                if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                {
                    return false;
                }
                //To delete data from table 
                sQuery = "DELETE FROM " + ReadConfigurations.FeederNetworkTraceTableName + " WHERE " + "FEEDERID" + " IN (" + sAllCircuitIds + ") OR FEEDERFEDBY IN (" + sAllCircuitIds + ")";
                DBHelperClass.UpdateQuery(sQuery);

                //To copy data from DataTable to Database 

                if (DBHelperClass.BulkCopyDataFromDataTable(pDataTable, ReadConfigurations.FeederNetworkTraceTableName) == false)
                {
                    throw new Exception("Feeder Network Trace Table not updated for CircuitIDs," + sAllCircuitIds + "," + DateTime.Now);
                }

                CommonFuntions.WriteLine_Info("Feeder Network Trace Table has been updated for CircuitIDs," + sAllCircuitIds + "," + DateTime.Now);

                return true;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                return false;
            }
        }

        private void ExecuteTracingInChildren()
        {
            string sQuery = null;
            DBHelper DBHelperClass = new DBHelper();
            DataTable pDataTable = new DataTable();
            string sCircuitId = null;
            int waitTime = 0;
            int recreateConncetionTime = 0;
            OracleConnection argConOraEDER = null;
            try
            {
                waitTime = Convert.ToInt32(ConfigurationManager.AppSettings["WAIT_TIME"]);
                recreateConncetionTime = Convert.ToInt32(ConfigurationManager.AppSettings["RECREATE_CONNECTION_TIME"]);

                sQuery = "SELECT DISTINCT " + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " +
                        ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_Assigned + "'";
                pDataTable = DBHelperClass.GetDataTable(sQuery);

                if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                {
                    return;
                }

                for (int i = 0; i < m_iTotalFeeders; i++)
                {
                    sCircuitId = null;
                    sCircuitId = pDataTable.Rows[i][0].ToString();

                    string AssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    Process myProcess = null;
                    string arguments = "-c," + sCircuitId;


                    AssemblyLocation = AssemblyLocation.Substring(0, AssemblyLocation.LastIndexOf("\\") + 1) + "PGE.BatchApplication.IGPPhaseUpdate.exe";
                    ProcessStartInfo startInfo = new ProcessStartInfo(AssemblyLocation, arguments)
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                    };

                    try
                    {
                        //_logger.Debug("Spawning new child process.  Arguments: " + startInfo.Arguments);
                        myProcess = new Process();
                        myProcess.StartInfo = startInfo;
                        myProcess.EnableRaisingEvents = true;
                        myProcess.Exited += new EventHandler(Process_Exited);
                        myProcess.Start();
                        // Thread.Sleep(5000);
                        //Place our processes in our list so we can monitor the progress of each
                        m_pList_ChildProcessesRunning.Add(myProcess);
                        CommonFuntions.WriteLine_Info("Process has been started for CircuitID," + sCircuitId + "," + DateTime.Now);

                    }
                    catch (Exception ex)//if exception occurred, does the control table need to be cleaned?
                    {
                        CommonFuntions.WriteLine_Error("Error creating Process," + ex.Message + "   " + ex.StackTrace);
                    }
                }
                
                bool showPercentage = true;
                int cursorPos = 0;
                int windowPos = 0;
                try
                {
                    cursorPos = Console.CursorLeft;
                    windowPos = Console.CursorTop;
                }
                catch { showPercentage = false; }

                argConOraEDER = DBHelper.GetOracleConnection();

                while (ChildrenStillProcessing(argConOraEDER))
                {
                    //kill processes if processing time is over
                    TimeSpan pTimeDiff = DateTime.Now - MainClass.m_dStartTime;

                    // Changes for IGP Issue : Database Error 
                    // Recreating conncetion after given time 
                    if (pTimeDiff.Minutes >= recreateConncetionTime)
                    {
                        //CommonFuntions.WriteLine_Info("Recreating the connection again.");
                        DBHelper.CloseConnection(argConOraEDER);
                        argConOraEDER = DBHelper.GetOracleConnection();
                    }

                    if (pTimeDiff.Hours >= ReadConfigurations.ProcessingHours)
                    {
                        foreach (Process process in m_pList_ChildProcessesRunning)
                        {
                            if (process != null && !process.HasExited)
                            {
                                process.Kill();
                            }
                        }
                        CommonFuntions.WriteLine_Info("Processing Time is now more than " + ReadConfigurations.ProcessingHours.ToString() + ". Killing all the processes.");
                        break;
                    }
                    #region To Close the console.--Added by Yasha- 23-jul -2019
                    bool allprocesscompleted = true;
                    foreach (Process process in m_pList_ChildProcessesRunning)
                    {
                        if (process != null)
                        {
                            allprocesscompleted = false;
                        }
                    }
                    if (allprocesscompleted == true)
                    {
                        CommonFuntions.WriteLine_Info("All child process has been completed so close the main window.");
                        break;
                    }
                    #endregion
                    if (showPercentage)
                    {
                        try
                        {
                            Console.CursorLeft = cursorPos;
                            Console.CursorTop = windowPos;

                            // Changes for IGP Issue : Database Error 
                            // Parmetizing the oracle conncetion
                            //double currentlyProcessed = GetProcessedCircuitsCount();
                            double currentlyProcessed = GetProcessedCircuitsCount(ref argConOraEDER);

                            Console.WriteLine("Percent Complete: " + (Math.Round(currentlyProcessed / m_iTotalFeeders, 2) * 100.0) + "%");
                        }
                        catch { showPercentage = false; }
                    }

                    // Changes for IGP Issue : Database Error 
                    // Parmetizing the wait time
                    //CommonFuntions.WriteLine_Info("Process will wait for : " + waitTime);
                    Thread.Sleep(waitTime);
                }
            }
            catch (Exception ex1)
            {
                CommonFuntions.WriteLine_Error(ex1.Message + "   " + ex1.StackTrace);
            }
            finally
            {
                // Changes for IGP Issue : Database Error 
                DBHelper.CloseConnection(argConOraEDER);
            }
        }

        System.Object childProcsRunningLock = new System.Object();
        void Process_Exited(object sender, EventArgs e)
        {
            Process p = sender as Process;
            try
            {
                if (p.ExitCode != 0)
                {
                    //CommonFuntions.WriteLine_Error("Error in child processing. Killing all child processes");
                    lock (childProcsRunningLock)
                    {
                        foreach (Process process in m_pList_ChildProcessesRunning)
                        {
                            if (process != null && !process.HasExited)
                            {
                                //temp
                                //process.Kill();
                            }
                        }
                    }
                    //CommonFuntions.WriteLine_Error("Error in child process. Refer to log files for information");
                    //Environment.Exit(1);
                }

                lock (childProcsRunningLock) { m_pList_ChildProcessesRunning.Remove(p); }
            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error(exp.Message + "   " + exp.StackTrace);
                //throw exp;
            }

        }

        private bool ChildrenStillProcessing(OracleConnection argConOraEDER)
        {
            bool processesStillRunning = false;
            double dProcessedCount = -1;
            try
            {
                if (m_pList_ChildProcessesRunning.Count > 0)
                {
                    processesStillRunning = true;
                }

                bool circuitsStillToProcess = true;

                // Changes for IGP Issue : Database Error 
                // Passing conncetion as parameter
                dProcessedCount = GetProcessedCircuitsCount(ref argConOraEDER);

                if (dProcessedCount == m_iTotalFeeders)
                {
                    circuitsStillToProcess = false;
                }
               
                else if (dProcessedCount < m_iTotalFeeders)
                {
                    //Start Process for another circuit
                    StartMoreProcesses_CalculateError();
                }

                if ((processesStillRunning == false) && (circuitsStillToProcess == false))
                {
                    return false;
                }
                else if (!processesStillRunning && circuitsStillToProcess)
                {
                    CommonFuntions._log.Info("Child processes are no longer running, but there are still circuits to process");
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                try
                {
                    foreach (Process p in m_pList_ChildProcessesRunning)
                    {
                        if (!p.HasExited) { p.Kill(); }
                    }
                }
                catch { }

                throw e;
            }
        }

        private bool StartMoreProcesses_CalculateError()
        {
            string sQuery = null;
            DBHelper DBHelperClass = new DBHelper();
            DataTable pDataTable = new DataTable();
            string sCircuitId = null;
            int iCount = 0;
            try
            {
                iCount = AssignCircuitIds_Additional_CalculateError();
                if (iCount < 1)
                {
                    return true;
                }
                sQuery = "SELECT DISTINCT " + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " +
                        ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_Assigned + "'";
                pDataTable = DBHelperClass.GetDataTable(sQuery);

                if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                {
                    return false;
                }

                Console.WriteLine("More Circuits added to the Process: " + pDataTable.Rows.Count.ToString());

                for (int i = 0; i < pDataTable.Rows.Count; i++)
                {
                    sCircuitId = null;
                    sCircuitId = pDataTable.Rows[i][0].ToString();

                    string AssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    Process myProcess = null;
                    string arguments = "-c," + sCircuitId;


                    AssemblyLocation = AssemblyLocation.Substring(0, AssemblyLocation.LastIndexOf("\\") + 1) + "PGE.BatchApplication.IGPPhaseUpdate.exe";
                    ProcessStartInfo startInfo = new ProcessStartInfo(AssemblyLocation, arguments)
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                    };

                    try
                    {
                        //_logger.Debug("Spawning new child process.  Arguments: " + startInfo.Arguments);
                        myProcess = new Process();
                        myProcess.StartInfo = startInfo;
                        myProcess.EnableRaisingEvents = true;
                        myProcess.Exited += new EventHandler(Process_Exited);
                        myProcess.Start();
                        //Place our processes in our list so we can monitor the progress of each
                        m_pList_ChildProcessesRunning.Add(myProcess);
                    }
                    catch (Exception ex)//if exception occurred, does the control table need to be cleaned?
                    {
                        CommonFuntions._log.Error("Error creating Process," + ex.Message + "   " + ex.StackTrace);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                return false;
            }
        }

        private double GetProcessedCircuitsCount()
        {
            String sQuery = null;
            DataTable pDT_Ret = new DataTable();
            double dRetOut = -1;
            try
            {
                sQuery = "SELECT COUNT(*) FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" +
                    ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_CycleCompleted + "'";

                pDT_Ret = DBHelperClass.GetDataTableByQuery(sQuery);

                if ((pDT_Ret != null) && (pDT_Ret.Rows.Count > 0))
                {
                    dRetOut = Convert.ToDouble(pDT_Ret.Rows[0][0].ToString());
                }

                return dRetOut;
            }
            catch (Exception ex)
            {
                CommonFuntions._log.Error(ex.Message + "   " + ex.StackTrace);
                return dRetOut;
            }
        }

        // Changes for IGP Issue : Database Error 
        // Passing the conenction as parameter 
        private double GetProcessedCircuitsCount(ref OracleConnection conOraEDER)
        {
            String sQuery = null;
            DataTable pDT_Ret = new DataTable();
            double dRetOut = -1;
            try
            {
                sQuery = "SELECT COUNT(*) FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" +
                    ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_CycleCompleted + "'";

                pDT_Ret = DBHelperClass.GetDataTableByQuery(ref conOraEDER, sQuery);

                if ((pDT_Ret != null) && (pDT_Ret.Rows.Count > 0))
                {
                    dRetOut = Convert.ToDouble(pDT_Ret.Rows[0][0].ToString());
                }

                return dRetOut;
            }
            catch (Exception ex)
            {
                CommonFuntions._log.Error(ex.Message + "   " + ex.StackTrace);
                return dRetOut;
            }
        }

        private int AssignCircuitIds(ref string sWhereClause, int iProcessCountCurrent)
        {
            String sQuery = null;
            DataTable pDT = new DataTable();
            int iRetOut = -1;
            string sStageToBeDone = null;
            string[] sArrayOfStages = null;
            string sStatusToBeSkipped = null;
            try
            {
                //update current status as null for whole table
                sQuery = "UPDATE " + ReadConfigurations.DAPHIEFileDetTableName + " SET " + ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = NULL";
                DBHelperClass.UpdateQuery(sQuery);

                sStageToBeDone = ReadConfigurations.Stage_To_Be_Completed;
                sStatusToBeSkipped = ReadConfigurations.Status_To_Be_Skipped;
                sArrayOfStages = sStageToBeDone.Split(',');

                if (sStageToBeDone == "ALL")
                {
                    sWhereClause = "'" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Loaded + "','" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Calculated + "','" +
                        ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Updated + "','" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CalculationError + "','" +
                        ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_UpdateError + "','" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_ValidateError + "'";

                    sQuery = "UPDATE " + ReadConfigurations.DAPHIEFileDetTableName +
                    " a SET " +
                    ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_Assigned +
                    "' WHERE " +
                    ReadConfigurations.DAPHIETablesFields.CircuitIdField + " IN (SELECT * FROM (SELECT " + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " +
                    ReadConfigurations.DAPHIEFileDetTableName + " b WHERE " +
                    ReadConfigurations.DAPHIETablesFields.CircuitIdField + " NOT IN (SELECT " + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " +
                    ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sStatusToBeSkipped + ")) AND " +
                    ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sWhereClause + ") AND " + ReadConfigurations.DAPHIETablesFields.Serial_NoField +
                    " = (SELECT MIN(" + ReadConfigurations.DAPHIETablesFields.Serial_NoField + ") FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " +
                    ReadConfigurations.DAPHIETablesFields.CircuitIdField + " = b." + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " AND " +
                    ReadConfigurations.DAPHIETablesFields.StatusField + " NOT IN ('POSTED','DELETED') AND " +
                    ReadConfigurations.DAPHIETablesFields.StatusField + " NOT LIKE '%" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_LimitExceeded + "%')" +
                    " ORDER BY " + ReadConfigurations.DAPHIETablesFields.Serial_NoField +
                    //") WHERE ROWNUM <= " + ReadConfigurations.MaxNumProcessors + ") AND " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sWhereClause +
                    ") WHERE ROWNUM <= " + iProcessCountCurrent + ") AND " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sWhereClause +
                    ") AND " + ReadConfigurations.DAPHIETablesFields.Serial_NoField + " = (SELECT MIN(" + ReadConfigurations.DAPHIETablesFields.Serial_NoField + ") FROM " +
                    ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " = a." +
                    ReadConfigurations.DAPHIETablesFields.CircuitIdField + " AND " +
                    ReadConfigurations.DAPHIETablesFields.StatusField + " NOT IN ('POSTED','DELETED') AND " + 
                    ReadConfigurations.DAPHIETablesFields.StatusField + " NOT LIKE '%" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_LimitExceeded + "%')";

                }
                else
                {
                    for (int i = 0; i < sArrayOfStages.Length; ++i)
                    {
                        if (sArrayOfStages[i].ToString() == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Calculated)
                        {
                            sWhereClause = sWhereClause + "'" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Loaded + "','" +
                                ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CalculationError + "',";
                        }
                        else if (sArrayOfStages[i].ToString() == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Updated)
                        {
                            sWhereClause = sWhereClause + "'" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Calculated + "','" +
                                ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_UpdateError + "',";
                        }
                        else if (sArrayOfStages[i].ToString() == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_QAQCPassed)
                        {
                            sWhereClause = sWhereClause + "'" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_Updated + "','" +
                                ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_ValidateError + "',";
                        }
                        else if (sArrayOfStages[i].ToString() == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_POSTED)
                        {
                            sWhereClause = sWhereClause + "'" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_QAQCPassed + "',";
                        }
                    }
                    sWhereClause = sWhereClause.Substring(0, sWhereClause.Length - 1);

                    sQuery = "UPDATE " + ReadConfigurations.DAPHIEFileDetTableName +
                    " a SET " +
                    ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_Assigned +
                    "' WHERE " +
                    ReadConfigurations.DAPHIETablesFields.CircuitIdField + " IN (SELECT * FROM (SELECT " + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " +
                    ReadConfigurations.DAPHIEFileDetTableName + " b WHERE " +
                    ReadConfigurations.DAPHIETablesFields.CircuitIdField + " NOT IN (SELECT " + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " +
                    ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sStatusToBeSkipped + ")) AND " +
                    ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sWhereClause + ") AND " + ReadConfigurations.DAPHIETablesFields.Serial_NoField +
                    " = (SELECT MIN(" + ReadConfigurations.DAPHIETablesFields.Serial_NoField + ") FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " +
                    ReadConfigurations.DAPHIETablesFields.CircuitIdField + " = b." + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " AND " +
                    ReadConfigurations.DAPHIETablesFields.StatusField + " NOT IN ('POSTED','DELETED') AND " +
                    ReadConfigurations.DAPHIETablesFields.StatusField + " NOT LIKE '%" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_LimitExceeded + "%')" +
                    " ORDER BY " + ReadConfigurations.DAPHIETablesFields.Serial_NoField +
                    //") WHERE ROWNUM <= " + ReadConfigurations.MaxNumProcessors + ") AND " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sWhereClause +
                    ") WHERE ROWNUM <= " + iProcessCountCurrent + ") AND " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sWhereClause +
                    ") AND " + ReadConfigurations.DAPHIETablesFields.Serial_NoField + " = (SELECT MIN(" + ReadConfigurations.DAPHIETablesFields.Serial_NoField + ") FROM " +
                    ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " = a." +
                    ReadConfigurations.DAPHIETablesFields.CircuitIdField + " AND " +
                    ReadConfigurations.DAPHIETablesFields.StatusField + " NOT IN ('POSTED','DELETED') AND " +
                    ReadConfigurations.DAPHIETablesFields.StatusField + " NOT LIKE '%" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_LimitExceeded + "%')";

                }

                if (string.IsNullOrEmpty(sQuery) == false)
                {
                    iRetOut = DBHelperClass.UpdateQuery(sQuery);
                }

                return iRetOut;
            }
            catch (Exception ex)
            {
                CommonFuntions._log.Error(ex.Message + "   " + ex.StackTrace);
                return iRetOut;
            }
        }

        private int AssignCircuitIds_Additional_CalculateError()
        {
            String sQuery = null;
            int iRetOut = -1;
            try
            {
                sQuery = "UPDATE " + ReadConfigurations.DAPHIEFileDetTableName + " SET " +
                     ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_Assigned +
                     "' WHERE " + ReadConfigurations.DAPHIETablesFields.StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CalculationError +
                     "' AND " + ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_CycleCompleted + "'";

                iRetOut = DBHelperClass.UpdateQuery(sQuery);

                return iRetOut;
            }
            catch (Exception ex)
            {
                CommonFuntions._log.Error(ex.Message + "   " + ex.StackTrace);
                return iRetOut;
            }
        }

        public string GetBatchNumber(ref string sSerial_No, ref string Batch_Number, string sCircuitID)
        {
            DataTable pDT = new DataTable();
            String sQuery = null;

            try
            {
                pDT = new DataTable();
                sQuery = "SELECT " + ReadConfigurations.DAPHIETablesFields.Serial_NoField + " , " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + " FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " +
                        ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" + ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_Assigned + "' AND CIRCUITID = '" + sCircuitID + "'";
                pDT = DBHelperClass.GetDataTable(sQuery);

                if ((pDT != null) && (pDT.Rows.Count > 0))
                {

                    sSerial_No = pDT.Rows[0][0].ToString();
                    Batch_Number = pDT.Rows[0][1].ToString();
                }
                return sQuery;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                return sQuery;
            }
        }

        public int UpdateCurrentStatusFlag(string sCircuitId, string sFlagValue, string Batch_Number)
        {
            String sQuery = null;
            int iRetOut = -1;
            try
            {
                //records from DAPHIE
                sQuery = "UPDATE " + ReadConfigurations.DAPHIEFileDetTableName + " SET " +
                    ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" + sFlagValue + "' WHERE " +
                    ReadConfigurations.DAPHIETablesFields.CircuitIdField + " = '" + sCircuitId + "' and BATCHID='" + Batch_Number + "'";
                iRetOut = DBHelperClass.UpdateQuery(sQuery);

                return iRetOut;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);

                return iRetOut;
            }
        }

        private bool WriteSkippedCircuitIds(int iFeeders, string sWhereClause,int iProcessCountCurrent)
        {
            String sQuery = null;
            DataTable pDataTable = new DataTable();
            string sBatchId = null;
            string sCircuitId = null;
            try
            {
                if (iFeeders == 0)
                {
                    sQuery = "SELECT " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + "," + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " +
                        ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.Current_StatusField +
                        " IS NULL AND " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sWhereClause + ") ORDER BY " + ReadConfigurations.DAPHIETablesFields.Serial_NoField;
                }
                //else if (iFeeders < ReadConfigurations.MaxNumProcessors)
                else if (iFeeders < iProcessCountCurrent)
                {
                    sQuery = "SELECT " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + "," + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " +
                       ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.Current_StatusField +
                       " IS NULL AND " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sWhereClause + ") ORDER BY " + ReadConfigurations.DAPHIETablesFields.Serial_NoField;

                }
                //else if (iFeeders == ReadConfigurations.MaxNumProcessors)
                else if (iFeeders == iProcessCountCurrent)
                {
                    sQuery = "SELECT " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + "," + ReadConfigurations.DAPHIETablesFields.CircuitIdField + " FROM " +
                        ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.Current_StatusField +
                        " IS NULL AND " + ReadConfigurations.DAPHIETablesFields.StatusField + " IN (" + sWhereClause + ") AND " +
                        ReadConfigurations.DAPHIETablesFields.Serial_NoField + " < (SELECT MAX(" + ReadConfigurations.DAPHIETablesFields.Serial_NoField +
                        ") FROM " + ReadConfigurations.DAPHIEFileDetTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.Current_StatusField + " = '" +
                        ReadConfigurations.STATUS_DAPHIEFILE_DET.CurrentStatus_Assigned + "') ORDER BY " + ReadConfigurations.DAPHIETablesFields.Serial_NoField;

                }

                if (string.IsNullOrEmpty(sQuery) == true)
                {
                    return true;
                }
                pDataTable = DBHelperClass.GetDataTable(sQuery);
                if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                {
                    return true;
                }

                for (int i = 0; i < pDataTable.Rows.Count; i++)
                {
                    sCircuitId = null;
                    sBatchId = null;
                    sCircuitId = pDataTable.Rows[i][ReadConfigurations.DAPHIETablesFields.CircuitIdField].ToString();
                    sBatchId = pDataTable.Rows[i][ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER].ToString();

                    CommonFuntions.WriteLine_Info("CircuitId could not be processed - " + sCircuitId + ", Batch Id - " + sBatchId);
                }

                return true;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                return false;
            }
        }


    }
}
