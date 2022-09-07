using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections;
using Miner.Interop;
using Miner.Geodatabase.Edit;
using Miner.Interop.Process;
using System.Data.OleDb;
using Miner.Geodatabase;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using PGE_DBPasswordManagement;

namespace PGE.Interface.Powerbase_To_GIS
{
    public class MainClass
    {
        public static DataTable _dtPBGISMapping = null;
        public static DataTable _dtRejectedRecords = null;
        public static int _ibatchID = 0;

        /// <summary>
        /// This function is the main entry point for this class and called from Program.cs to start the process
        /// </summary>
        /// <param name="argInputOIDs"></param>
        /// <returns></returns>
        public static bool StartProcess()
        {
            bool bOperaionSuccess = true;
            IWorkspace workspace_edgis = null;
            DataTable dtDataToUpdate = null;
            string strSessionName = null;
            bool bUpdateStatus = false;
            string connstringpgedata = null;
            string connstringpbuser = null;
            string strQuery = null;
            try
            {
                //connstringpgedata = ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pgedata);
                //EmailMainClass.ComplieAndSendMail(connstringpgedata);

                Common._log.Info("Process Started.");
                bOperaionSuccess = PreRequiste();

                if (!bOperaionSuccess)
                {
                    Common._log.Info("Exiting the process.");
                    return false;
                }

                bool bReadConfigValuesAndInitializesStaticVariables = ReadConfigValuesAndInitializeStaticVariables();

                if (!bReadConfigValuesAndInitializesStaticVariables)
                {
                    Common._log.Info("Error in retreiving configuration values.");
                }

                // Check the staus of the previous session here 

                // m4jf edgisrearch 919 -Commented below lines - Get connection string using PGE_DBPasswordmanagement 
                //connstringpgedata = ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pgedata);
                //connstringpbuser = ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pbuser);
                connstringpgedata = ReadEncryption.GetConnectionStr(ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pgedata));
                connstringpbuser = ReadEncryption.GetConnectionStr(ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pgedata));
                string strSessionStatus = VersionOperations.CheckPreviousRunSessionStatus(connstringpgedata, connstringpbuser);

                Common._log.Info("The previous Session status is : " + strSessionStatus);

                if (strSessionStatus == ReadConfigurations.SESSION_STATUS_INPROGRESS || strSessionStatus == ReadConfigurations.SESSION_STATUS_PENDING_QAQC || strSessionStatus == ReadConfigurations.SESSION_STATUS_HOLD || strSessionStatus == ReadConfigurations.SESSION_STATUS_POST_QUEUE || strSessionStatus == ReadConfigurations.SESSION_STATUS_POST_ERROR || strSessionStatus == ReadConfigurations.SESSION_STATUS_RECONCILE_ERROR || strSessionStatus == ReadConfigurations.SESSION_STATUS_CONFLICT || strSessionStatus == ReadConfigurations.SESSION_STATUS_DATA_PROCESSING || strSessionStatus == ReadConfigurations.SESSION_STATUS_QA_QC_ERROR)
                {
                    // Don't process anything further and exit the program 
                    Common._log.Info("The previous Session status is : " + strSessionStatus + ". Exiting the process.");
                    // Do we want to send the mail to mapper here 
                    return false;
                }
                else if (strSessionStatus == ReadConfigurations.SESSION_STATUS_POSTED || strSessionStatus == ReadConfigurations.SESSION_STATUS_DELETED)
                {
                    // Update status in table , move record to archive and Process further 
                    Common._log.Info("The previous Session status is : " + strSessionStatus);
                    Common.MakeChangesForPBSessionTable(strSessionStatus, strSessionName, "UPDATE");
                    Common.MoveDataToArchiveTable(connstringpgedata, ReadConfigurations.GetValue(ReadConfigurations.TB_PB_Session), ReadConfigurations.GetValue(ReadConfigurations.TB_PB_Session_Archive));
                }
                else if (strSessionStatus == ReadConfigurations.SESSION_STATUS_UNKNOWN)
                {
                    Common._log.Info("The previous Session status is : " + strSessionStatus + ". Exiting the process.");
                    // Session status unknown , Send mail to operation team and don't proceed further.
                    return false;
                }

                Common._log.Info("Connection string used in process ." + ReadConfigurations.GetValue(ReadConfigurations.ConnString_EDWorkSpace));

                // M4JF EDGISREARCH 919 - get sde connection using PGE_DBPasswordmanagement
                // string connection_string = ReadConfigurations.GetValue(ReadConfigurations.ConnString_EDWorkSpace);

                string connection_string = ReadEncryption.GetSDEPath(ReadConfigurations.GetValue(ReadConfigurations.ConnString_EDWorkSpace));
               workspace_edgis = GetWorkspace(connection_string);
                Common._log.Info("Workspace checked out successfully.");

                if (workspace_edgis == null)
                {
                    Common._log.Error("Workspace could not be retreived for Connection : " + ReadConfigurations.GetValue(ReadConfigurations.ConnString_EDWorkSpace));
                    Common._log.Info("Exiting the Process.");
                    return false;
                }

                string queryToGetDataToUpdate = "Select * from " + ReadConfigurations.GetValue(ReadConfigurations.TB_PowerbaseStage);
                //queryToGetDataToUpdate+=" where globalid= '{003A6303-79D7-4DB7-9BD5-BE1F1BC2CCBC}'";
                //queryToGetDataToUpdate += " where globalid= '{13628735-FA7E-4F8C-BD7E-A67F2896E7EE}'";
                //queryToGetDataToUpdate += " where globalid in ('{D572802B-AE9C-4FAE-B3DC-F30C6A9FEF06}','{00196C43-CEB5-4CCA-B469-0689EC176B82}')";

                Common._log.Info("Query to get data to update : " + queryToGetDataToUpdate);

                dtDataToUpdate = DBHelper.GetDataTable(connstringpgedata, queryToGetDataToUpdate);

                if (dtDataToUpdate !=null && dtDataToUpdate.Rows.Count > 0)
                {
                    bUpdateStatus = DataUpdateAndVersionOperations(workspace_edgis,ref dtDataToUpdate, ref strSessionName);

                    if (bUpdateStatus)
                    {
                        Common._log.Info("Updates done successfully in version : " + strSessionName);
                        Common._log.Info("Submitting sersion : " + strSessionName + " to GDBM posting queue.");
                        bool bSessionSubmitToGDBM = VersionOperations.SubmitSessionToGDBM(VersionOperations._mmSession, workspace_edgis);

                        if (bSessionSubmitToGDBM)
                        {
                            EmailMainClass.ComplieAndSendMail(connstringpgedata);
                                                         
                            Common._log.Info("Session " + strSessionName + " submitted successfully to GDBM.");
                            UpdateStagingtableStatusandMovingDataToArchiveTable(dtDataToUpdate,connstringpgedata);
                            // Create entry in table PB_SESSION 
                            Common.MakeChangesForPBSessionTable("INPROGRESS", strSessionName, "INSERT");
                        }
                        else
                        {                           
                            Common._log.Info("There is some error while submitting the session to GDBM : " + strSessionName);
                            Common._log.Info("Updating Status in stage table to FAILED for all records.");
                            Common._log.Info("Query used to update : " + strQuery);

                            strQuery = "UPDATE " + ReadConfigurations.GetValue(ReadConfigurations.TB_PowerbaseStage) + " SET " + ReadConfigurations.col_RECORD_STATUS + "='"+ReadConfigurations.value_FAILED+"'";
                            DBHelper.ExecuteQueryOnDatabase(connstringpgedata, strQuery);
                        }
                    }
                    else
                    {
                        Common._log.Info("There is some error while saving the features in session : " + strSessionName);
                        Common._log.Info("Updating Status in stage table to FAILED for all records.");
                        Common._log.Info("Query used to update : " + strQuery);

                        strQuery = "UPDATE " + ReadConfigurations.GetValue(ReadConfigurations.TB_PowerbaseStage) + " SET "+ ReadConfigurations.col_RECORD_STATUS +"='" + ReadConfigurations.value_FAILED + "'";
                        DBHelper.ExecuteQueryOnDatabase(connstringpgedata, strQuery);
                    }
                }
                else
                {                   
                    Common._log.Info("There is no data present in input table " + ReadConfigurations.GetValue(ReadConfigurations.TB_PowerbaseStage) + " to update in EDGIS.");                    
                }

                Common._log.Info("Process Completed.");
            }
            catch (Exception exp)
            {
                bOperaionSuccess = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                ReleaseStaticVariables();
                Common.CloseLicenseObject();
            }
            return bOperaionSuccess;
        }

        /// <summary>
        /// This function read all the configuration values from config file and initializes required static variables
        /// </summary>
        /// <returns></returns>
        public static bool ReadConfigValuesAndInitializeStaticVariables()
        {
            bool bAllConfigReadSuccessfully = true;
            try
            {
                _dtPBGISMapping = ReadPBGISMappings();

                _dtRejectedRecords = new System.Data.DataTable();
                _dtRejectedRecords.Columns.Add(ReadConfigurations.col_FEATURECLASS, typeof(string));
                _dtRejectedRecords.Columns.Add(ReadConfigurations.col_GLOBALID, typeof(string));
                _dtRejectedRecords.Columns.Add(ReadConfigurations.col_OPERATINGNUMBER, typeof(string));
                _dtRejectedRecords.Columns.Add(ReadConfigurations.col_PB_RLID, typeof(Int64));	 
                _dtRejectedRecords.Columns.Add(ReadConfigurations.col_ATTRIBUTENAME, typeof(string));
                _dtRejectedRecords.Columns.Add(ReadConfigurations.col_ATTRIBUTEVALUE, typeof(string));
                _dtRejectedRecords.Columns.Add(ReadConfigurations.col_COMMENTS, typeof(string));
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                bAllConfigReadSuccessfully = false;
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bAllConfigReadSuccessfully;
        }

        /// <summary>
        /// This function releases all static variables
        /// </summary>
        public static void ReleaseStaticVariables()
        {            
            try
            {
                if (_dtPBGISMapping != null)
                {
                    _dtPBGISMapping.Clear();
                    _dtPBGISMapping = null;
                }

                if (_dtRejectedRecords != null)
                {
                    _dtRejectedRecords.Clear();
                    _dtRejectedRecords = null;
                }

                _ibatchID = 0;
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());                
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }           
        }

        /// <summary>
        /// This function return the workspace for given connection string
        /// </summary>
        /// <param name="argStrworkSpaceConnectionstring"></param>
        /// <returns></returns>
        public static IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {
                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in getting SDE Workspace, function GetWorkspace: " + "Exception : " + exp.Message);
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return workspace;
        }

        /// <summary>
        /// This function checks for all the pre requisites for running the process
        /// </summary>
        /// <returns></returns>
        public static bool PreRequiste()
        {
            bool ballPreRequisiteSuccessful = true;
            try
            {
                Common.InitializeESRILicense();
                Common.InitializeArcFMLicense();
            }
            catch (Exception exp)
            {
                Common._log.Error("Error in Initializing Arc GIS/ArcFM License, function PreRequiste " + "Exception : " + exp.Message);
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return ballPreRequisiteSuccessful;
        }

        /// <summary>
        /// This function updates the status in staging table and them move data to archive table.
        /// </summary>
        /// <param name="argdtRecords"></param>
        /// <param name="argStrConnstring"></param>
        /// <returns></returns>
        private static bool UpdateStagingtableStatusandMovingDataToArchiveTable(DataTable argdtRecords,string argStrConnstring)
        {
            bool bProcessSuccess = false;
            try
            {               
                DBHelper.UpdateStatusForRecords(argdtRecords,argStrConnstring);

                Common.MoveDataToArchiveTable(argStrConnstring, ReadConfigurations.GetValue(ReadConfigurations.TB_PowerbaseStage), ReadConfigurations.GetValue(ReadConfigurations.TB_PowerbaseStage_Archive));

                bProcessSuccess = true;
            }
            catch (Exception exp)
            {
                bProcessSuccess = false;
                Common._log.Error("Function Name UpdateStagingtableStatusandMovingDataToArchiveTable. " + "Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bProcessSuccess;
        }

        /// <summary>
        /// This function create session and update all data in the session
        /// </summary>
        /// <param name="argWorkspaceedgis"></param>
        /// <param name="argDataToUpdate"></param>
        /// <param name="argStrVersionName"></param>
        /// <returns></returns>
        private static bool DataUpdateAndVersionOperations(IWorkspace argWorkspaceedgis,ref DataTable argDataToUpdate, ref string argStrVersionName)
        {
            bool bOperationSuccess = false;
            IVersion2 powerBaseVersion;                   
            //Do not disable Auto Updaters // If we will then Symbol attributes will not be updated            
            try
            {
                powerBaseVersion = VersionOperations.CreateVersion(argWorkspaceedgis);

                if (powerBaseVersion == null)
                {
                    Common._log.Error("Version Creation Failed.");
                    return false;
                }

                argStrVersionName = powerBaseVersion.VersionName;
                Common._log.Info("Version Created Successfully. Version Name : " + powerBaseVersion.VersionName.ToUpper());
                //5.Create version - End

                //6.Make edits in version - Start
                IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)powerBaseVersion;
                IWorkspaceEdit editSession = (IWorkspaceEdit)powerBaseVersion;
                if (Editor.EditState == EditState.StateNotEditing)
                {
                    Editor.StartEditing((IWorkspace)editSession);
                    Editor.StartOperation();
                }

                bool updateIsSuccessFull = Common.UpdatePowerbaseDataInVersion(featversionWorkspace, ref argDataToUpdate);

                if (Editor.EditState == EditState.StateEditing)
                {
                    Editor.StopOperation("");
                    Editor.StopEditing(true);
                }

                Common._log.Info("Editing in version " + argStrVersionName + " Completed.");
                //Make edits in version - End

                bOperationSuccess = true;
            }
            catch (Exception exp)
            {
                bOperationSuccess = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {

            }
            return bOperationSuccess;
        }

        ///// <summary>
        ///// This function read the mapping and store mapping in a datatable
        ///// </summary>
        ///// <returns></returns>
        //private static bool ReadPBGISMappingsTemp()
        //{
        //    bool bSuccess = false;
        //    try
        //    {
        //        _dtPBGISMapping = new DataTable();

        //        //// Read the mxl file data and insert into datatable 
        //        //string configFileName = "PBGIS_Synch_Config1.xml";
        //        //string configPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        //        _dtPBGISMapping.Columns.Add("FeatureClass", typeof(string));
        //        _dtPBGISMapping.Columns.Add("Subtype", typeof(string));
        //        _dtPBGISMapping.Columns.Add("PBFieldName", typeof(string));
        //        _dtPBGISMapping.Columns.Add("GISFieldName", typeof(string));

        //        _dtPBGISMapping.Rows.Add("EDGIS.CapacitorBank", "", "SUPERVISORYCONTROL", "SUPERVISORYCONTROL");
        //        _dtPBGISMapping.Rows.Add("EDGIS.CapacitorBank", "", "SWITCHMODEIDC", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.CapacitorBank", "", "OPERATINGAS", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.CapacitorBank", "", "MULTIFUNCTIONALIDC", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.CapacitorBank", "", "DEVICETYPE", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.CapacitorBank", "", "FLISRAUTOMATIONDEVICEIDC", "EDGIS.SCADA.FLISRAUTOMATIONDEVICEIDC");
        //        _dtPBGISMapping.Rows.Add("EDGIS.CapacitorBank", "", "CONTROLLERSERIALNUMBER", "EDGIS.CONTROLLER.SERIALNUMBER");
        //        _dtPBGISMapping.Rows.Add("EDGIS.CapacitorBank", "", "CONTROLLERTYPE", "EDGIS.CONTROLLER.CONTROLLERTYPE");

        //        _dtPBGISMapping.Rows.Add("EDGIS.Switch", "", "SUPERVISORYCONTROL", "SUPERVISORYCONTROL");
        //        _dtPBGISMapping.Rows.Add("EDGIS.Switch", "", "SWITCHMODEIDC", "SWITCHMODEIDC");

        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "1", "SUPERVISORYCONTROL", "SUPERVISORYCONTROL");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "1", "SWITCHMODEIDC", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "1", "OPERATINGAS", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "1", "MULTIFUNCTIONALIDC", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "1", "DEVICETYPE", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "1", "FLISRAUTOMATIONDEVICEIDC", "EDGIS.SCADA.FLISRAUTOMATIONDEVICEIDC");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "1", "CONTROLLERSERIALNUMBER", "EDGIS.CONTROLLER.SERIALNUMBER");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "1", "CONTROLLERTYPE", "EDGIS.CONTROLLER.CONTROLLERTYPE");


        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "2", "SUPERVISORYCONTROL", "SUPERVISORYCONTROL");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "2", "SWITCHMODEIDC", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "2", "OPERATINGAS", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "2", "MULTIFUNCTIONALIDC", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "2", "DEVICETYPE", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "2", "FLISRAUTOMATIONDEVICEIDC", "EDGIS.SCADA.FLISRAUTOMATIONDEVICEIDC");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "2", "CONTROLLERSERIALNUMBER", "EDGIS.CONTROLLER.SERIALNUMBER");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "2", "CONTROLLERTYPE", "EDGIS.CONTROLLER.CONTROLLERTYPE");

        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "3", "SUPERVISORYCONTROL", "SUPERVISORYCONTROL");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "3", "SWITCHMODEIDC", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "3", "OPERATINGAS", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "3", "MULTIFUNCTIONALIDC", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "3", "DEVICETYPE", "");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "3", "FLISRAUTOMATIONDEVICEIDC", "EDGIS.SCADA.FLISRAUTOMATIONDEVICEIDC");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "3", "CONTROLLERSERIALNUMBER", "EDGIS.CONTROLLER.SERIALNUMBER");
        //        _dtPBGISMapping.Rows.Add("EDGIS.DynamicProtectiveDevice", "3", "CONTROLLERTYPE", "EDGIS.CONTROLLER.CONTROLLERTYPE");

        //        _dtPBGISMapping.AcceptChanges();

        //        bSuccess = true;
        //    }
        //    catch (Exception exp)
        //    {
        //        bSuccess = false;
        //        //   throw;
        //    }
        //    return bSuccess;
        //}

        /// <summary>
        /// This function reads all mapping
        /// </summary>
        /// <returns></returns>
        private static DataTable ReadPBGISMappings()
        {
            string configFileName = null;
            string filepath = null;
            string configpath = null;
            Boolean checkDT = false;
            DataTable dtMapping = new DataTable();
            DataColumn dtColumn;
            DataRow myDataRow;
            try
            {
                //// Read the mxl file data and insert mappings into datatable 

                filepath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                configFileName = ReadConfigurations.GetValue(ReadConfigurations.PB_GIS_Mapping_ConfigFileName);

                configpath = System.IO.Path.Combine(filepath, configFileName);

                Common._log.Info("PB-GIS config file path : " + configpath);

                XElement elements = XElement.Load(configpath);
                foreach (XElement element in elements.Elements())
                {
                    //Console.WriteLine(element.Name.ToString());                    
                    if (element.HasAttributes)
                    {
                        IEnumerable<XAttribute> parentattribute = element.Attributes();
                        IEnumerable<XAttribute> attList = from at in element.Attributes() select at;
                        foreach (XAttribute att in attList)
                        {
                            if (checkDT == false)
                            {
                                dtColumn = new DataColumn();
                                dtColumn.DataType = typeof(String);
                                dtColumn.ColumnName = att.Name.ToString();
                                dtColumn.Caption = att.Name.ToString();
                                dtColumn.ReadOnly = true;
                                dtColumn.Unique = false;
                                dtMapping.Columns.Add(dtColumn);
                            }
                            //Console.WriteLine(att.Value);
                        }
                    }
                    if (element.HasElements)
                    {
                        foreach (XElement elementchild in element.Elements())
                        {
                            if (elementchild.HasElements)
                            {
                                foreach (XElement elementchild1 in elementchild.Elements())
                                {
                                    if (elementchild1.HasAttributes)
                                    {
                                        IEnumerable<XAttribute> childattribute = elementchild1.Attributes();
                                        IEnumerable<XAttribute> attList1 = from at in elementchild1.Attributes() select at;
                                        foreach (XAttribute att1 in attList1)
                                        {
                                            if (checkDT == false)
                                            {
                                                dtColumn = new DataColumn();
                                                dtColumn.DataType = typeof(String);
                                                dtColumn.ColumnName = att1.Name.ToString();
                                                dtColumn.Caption = att1.Name.ToString();
                                                dtColumn.ReadOnly = true;
                                                dtColumn.Unique = false;
                                                dtMapping.Columns.Add(dtColumn);
                                            }
                                        }
                                        checkDT = true;
                                    }
                                }
                            }
                        }
                    }
                    checkDT = true;
                }

                XElement childelements = XElement.Load(configpath);
                var FCname = new List<string>();
                foreach (XElement element in childelements.Elements())
                {

                    if (element.HasAttributes)
                    {
                        IEnumerable<XAttribute> parentattribute = element.Attributes();
                        IEnumerable<XAttribute> attList = from at in element.Attributes() select at;
                        FCname.Clear();
                        foreach (XAttribute att in attList)
                        {

                            FCname.Add(att.Value.ToString());
                        }
                    }
                    if (element.HasElements)
                    {
                        foreach (XElement elementchild in element.Elements())
                        {
                            if (elementchild.HasElements)
                            {
                                foreach (XElement elementchild1 in elementchild.Elements())
                                {
                                    if (elementchild1.HasAttributes)
                                    {
                                        myDataRow = dtMapping.NewRow();
                                        for (int i = 0; i < FCname.Count; i++)
                                        {
                                            myDataRow[i] = FCname[i].ToString();
                                        }
                                        IEnumerable<XAttribute> childattribute = elementchild1.Attributes();
                                        IEnumerable<XAttribute> attList1 = from at in elementchild1.Attributes() select at;
                                        foreach (XAttribute att1 in attList1)
                                        {
                                            myDataRow[att1.Name.ToString()] = att1.Value.ToString();
                                        }
                                        dtMapping.Rows.Add(myDataRow);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return dtMapping;
        }
    }
}
