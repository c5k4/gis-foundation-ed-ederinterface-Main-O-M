using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.Interop.Process;
using System.Configuration;
using Oracle.DataAccess.Client;
using System.Text.RegularExpressions;
using System.Data;
using PGE.BatchApplication.IGPPhaseUpdate.Utility_Classes;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes
{
    public class Version_Management
    {       
        Common CommonFuntions = new Common();
        DBHelper _dbhelper = new DBHelper();
     
        /// <summary>
        /// This function will return default ESRI Version
        /// </summary>
        /// <returns></returns>
        public IWorkspace FindVersionFromVersionName(string sVersion_Name)
        {
            IWorkspace pWorkspace = null;         
            if (!string.IsNullOrEmpty(sVersion_Name))
            {
                //GetVersion

                //App crash issue resolution
                if (MainClass.m_SDEDefaultworkspace ==null)
                {
                    MainClass.m_SDEDefaultworkspace = CommonFuntions.GetWorkspace(ReadConfigurations.SDEWorkSpaceConnString);
                }

                IFeatureWorkspace fWSpace = (IFeatureWorkspace)MainClass.m_SDEDefaultworkspace;
                IVersionedWorkspace vWSpace = (IVersionedWorkspace)fWSpace;
                try
                {
                    IVersion pVersion = vWSpace.FindVersion(sVersion_Name);
                    if (pVersion != null)
                    {
                        pWorkspace = (IWorkspace)pVersion;
                    }
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Version not found in database : " + sVersion_Name);
                    CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                }
            }
            return pWorkspace;
        }
      
        /// <summary>
        /// This function will return default ESRI Version
        /// </summary>
        /// <returns></returns>
        public IVersion GetDefaultVersion(IWorkspace argWorkspace)
        {
            IVersion2 defaultVersion = null;
            try
            {
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)argWorkspace;
                {
                    defaultVersion = (IVersion2)vWorkspace.FindVersion(ReadConfigurations.defaultVersionName);
                }
            }
            catch (Exception exp)
            {
                //_log.Error("Error in getting Default Version, function GetDefaultVersion: " + exp.Message);
                CommonFuntions.WriteLine_Error("Error in getting Default Version, function GetDefaultVersion: " + exp.Message); 
            }
            return defaultVersion;
        }

        /// <summary>
        /// This function will reconcile and post ESRI version
        /// </summary>
        /// <returns></returns>
        public bool ReconcileAndPostVersion(string IGPVersionName, ref string POSTERROR,string sCircuitID)
        {
            bool bOpeartionSuccess = false;
            IVersion defaultVersion = null;
            try
            {
                //Creating workspace using SDE user to POST the version 
                CommonFuntions.WriteLine_Info("Creating workspace using SDE user to POST the version ");
                //Creating the workspace for version post from user sde                
                IWorkspace _Workspace = CommonFuntions.GetWorkspace(ConfigurationManager.AppSettings["SDEWorkSpaceConnString"]);
                CommonFuntions.WriteLine_Info("Workspace Created.");

                defaultVersion = GetDefaultVersion(_Workspace);
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)_Workspace;
                IVersion2 _IGPDailyVersion=(IVersion2)vWorkspace.FindVersion(IGPVersionName);
                CommonFuntions.WriteLine_Info("Version to Post: " + IGPVersionName + " at," + DateTime.Now);
                IMultiuserWorkspaceEdit muWorkspaceEdit = (IMultiuserWorkspaceEdit)_IGPDailyVersion;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)_IGPDailyVersion;
               // IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;
                IVersionEdit versionEdit = (IVersionEdit)workspaceEdit;
                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();
                CommonFuntions.TraceFeeder(_IGPDailyVersion as IVersion, sCircuitID);
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);

                if (muWorkspaceEdit.SupportsMultiuserEditSessionMode(esriMultiuserEditSessionMode.esriMESMVersioned))
                {
                    muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                    //Reconcile with the default version.
                    // Keeping conlict in favour of edit version
                    CommonFuntions.WriteLine_Info("Start Reconciling the version..." + " at," + DateTime.Now);
                    
                    //bool conflicts = versionEdit.Reconcile4(defaultVersion.VersionName, false, false, true, false);
                  
                    bool conflicts = versionEdit.Reconcile(defaultVersion.VersionName);
                    if (conflicts==true)
                    {
                        CommonFuntions.WriteLine_Info("Conflict in Version Posting!!!Exiting Interface" + " at," + DateTime.Now);
                        POSTERROR = ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CONFLICT;
                    }
                    else
                    {
                        CommonFuntions.WriteLine_Info("No Conflict found, version Reconcile successfull." + " at," + DateTime.Now);
                        
                    }
                    if (POSTERROR != ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CONFLICT)
                    {
                        CommonFuntions.WriteLine_Info("Start Editing in Workspace" + " at," + DateTime.Now);

                        workspaceEdit.StartEditing(true);
                        workspaceEdit.StartEditOperation();
                        if (versionEdit.CanPost())
                        {
                            CommonFuntions.WriteLine_Info("Version Post start " + " at," + DateTime.Now);

                            try
                            {
                                versionEdit.Post(defaultVersion.VersionName);
                                CommonFuntions.WriteLine_Info("Version Post end" + " at," + DateTime.Now);
                                CommonFuntions.WriteLine_Info("Stop editing in Workspace" + " at," + DateTime.Now);
                                workspaceEdit.StopEditOperation();
                                workspaceEdit.StopEditing(true);
                                CommonFuntions.WriteLine_Info("Editing stopped in Workspace" + " at," + DateTime.Now);
                                DeleteVersion(IGPVersionName);
                                bOpeartionSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                //_log.Info("Couldn't Stop editing in SDEworkspace.: " + ex.Message + " at " + ex.StackTrace);
                                CommonFuntions.WriteLine_Error("Couldn't Stop editing in SDEworkspace.: " + ex.Message + " at " + ex.StackTrace);
                                workspaceEdit = null;
                                bOpeartionSuccess = false;
                            }
                        }
                        else
                        {
                            CommonFuntions.WriteLine_Info("Cannot Post version " + defaultVersion.VersionName + ". Make SDE connection with user SDE only." + " at," + DateTime.Now);
                            POSTERROR = ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_POSTERROR;
                            bOpeartionSuccess = false;
                        }
                    }
                   
                }
            }
            catch (Exception exp)
            {
                //_log.Error("Error in version Post, function ReconcileAndPostVersion: " + exp.Message + " at " + exp.StackTrace);
                //_log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace);

                CommonFuntions.WriteLine_Error("Error in version Post, function ReconcileAndPostVersion: " + exp.Message + " at " + exp.StackTrace); 
                bOpeartionSuccess = false;
            }
            return bOpeartionSuccess;
        }
        /// <summary>
        /// This function will reconcile and post ESRI version
        /// </summary>
        /// <returns></returns>
        public bool ReconcileTheBatchForPosting(ref string POSTERROR, IVersion2 _IGPDailyBatchVersion)
        {
            bool bOpeartionSuccess = false;
            IVersion defaultVersion = null;
            try
            {
                IMultiuserWorkspaceEdit muWorkspaceEdit = (IMultiuserWorkspaceEdit)_IGPDailyBatchVersion;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)muWorkspaceEdit;
                 //IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;
                IVersionEdit versionEdit = (IVersionEdit)workspaceEdit;
                defaultVersion = GetDefaultVersion(_IGPDailyBatchVersion as IWorkspace );
                if (muWorkspaceEdit.SupportsMultiuserEditSessionMode(esriMultiuserEditSessionMode.esriMESMVersioned))
                {
                    muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                    //Reconcile with the default version.
                    // Keeping conlict in favour of edit version
                    CommonFuntions.WriteLine_Info("Start Reconciling the version..." + " at," + DateTime.Now);
                   //bool conflicts = versionEdit.Reconcile4(defaultVersion.VersionName, true,true, true, true);
                    bool conflicts = versionEdit.Reconcile(defaultVersion.VersionName);
                    if (conflicts)
                    {
                        CommonFuntions.WriteLine_Info("Conflict in Version Posting!!!Exiting Interface" + " at," + DateTime.Now);
                        POSTERROR = ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CONFLICT;
                        bOpeartionSuccess = false;
                    }
                    else
                    {
                        CommonFuntions.WriteLine_Info("No Conflict found, version Reconcile successfull." + " at," + DateTime.Now);
                        bOpeartionSuccess = true;
                    }                  
                }
            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error("Exception occurred while reconciling the Version : " + exp.Message + " at " + exp.StackTrace);
                POSTERROR = ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_RECONCILEERROR;
                bOpeartionSuccess = false;
            }
            return bOpeartionSuccess;
        }

        /// <summary>
        /// This function will delete ESRI version
        /// </summary>
        /// <returns></returns>
        public Boolean DeleteVersion(string sVersionName)
        {
            bool blRetFlag = false;
            if (!(string.IsNullOrEmpty(sVersionName)))
            {
                try
                {
                    // M4JF EDGISREARCH 919
                    IWorkspace workspace_sde = CommonFuntions.GetWorkspace(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_ConnectionStr_SDE"].ToUpper()));
                    IVersion2 versionToDelete = ((IVersionedWorkspace)workspace_sde).FindVersion(ReadConfigurations.ApplicationUser + "." + sVersionName) as IVersion2;
                    try
                    {
                        versionToDelete.Delete();
                        CommonFuntions.WriteLine_Info("Version " + sVersionName + " deleted successfully.");

                    }
                    catch (Exception exp)
                    {
                        //_log.Info("Error in deleting daily interface Version , function DeleteVersion " + exp.Message + " at " + exp.StackTrace);
                        //CommonFuntions.WriteLine_Error("Error in deleting Version , function DeleteVersion " + exp.Message + " at " + exp.StackTrace);
                    }
                    DeleteSession(sVersionName);
                    blRetFlag = true;
                }
                catch (Exception exp)
                {
                    //_log.Info("Error in deleting daily interface Version , function DeleteVersion " + exp.Message + " at " + exp.StackTrace);
                    CommonFuntions.WriteLine_Error("Error in deleting Version , function DeleteVersion " + exp.Message + " at " + exp.StackTrace); 
                    blRetFlag = false;
                }
            }
            return blRetFlag;
        }

        /// <summary>
        /// This function will delete Arc FM Session
        /// </summary>
        /// <returns></returns>
        private void DeleteSession(string sversionname )
        {
            try
            {
                int _sID = Convert.ToInt32(sversionname.Substring(3));
                string strQuery = "select * from "  + ReadConfigurations.Process_TableName.MM_CurrentPxStateTableName+ " where SOID=" + _sID;
                DataRow dr = _dbhelper.GetSingleDataRowByQuery(strQuery);
                if (dr != null)
                {
                    strQuery = "Delete from " + ReadConfigurations.Process_TableName.MM_CurrentPxStateTableName + " where SOID=" + _sID;
                    _dbhelper.UpdateQuery( strQuery);
                }
                strQuery = "Delete from " + ReadConfigurations.Process_TableName.MM_SessionTableName + " where session_id=" + _sID;
                _dbhelper.UpdateQuery(strQuery);
                strQuery = "Delete from " + ReadConfigurations.Process_TableName.MM_PxVersionTableName + " where NAME='" + sversionname + "'";
                _dbhelper.UpdateQuery(strQuery);
            }
            catch (Exception exp)
            {
                //_log.Error(exp.Message + " |  Stack Trace | " + exp.StackTrace);
                CommonFuntions.WriteLine_Error(exp.Message + " |  Stack Trace | " + exp.StackTrace); 
            }
        }

        internal bool ReconcileAndPostAllPendingVersions()
        {
            bool retval = false;
            string POSTERROR =string.Empty;
            try
            {

                string strQuery = "select SESSION_NAME,CIRCUITID,BATCHID from "  + ReadConfigurations.DAPHIEFileDetTableName + " where STATUS in ('"  + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_QAQCPassed + "')";
                DataTable PendingVersionList = _dbhelper.GetDataTableByQuery(strQuery);
                if (PendingVersionList != null && PendingVersionList.Rows.Count > 0)
                {
                    foreach (DataRow dr in PendingVersionList.Rows)
                    {
                        if (ReconcileAndPostVersion(dr["SESSION_NAME"].ToString(), ref POSTERROR, dr["CIRCUITID"].ToString()) == true)
                        {
                            strQuery = "update " + ReadConfigurations.DAPHIEFileDetTableName + " set STATUS='"  + ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_POSTED + "' where SESSION_NAME='" + dr["SESSION_NAME"].ToString() + "'";
                            _dbhelper.UpdateQuery(strQuery);
                            retval = true;
                        }
                        else
                        {
                            strQuery = "update " + ReadConfigurations.DAPHIEFileDetTableName + " set STATUS='" + POSTERROR + "' where SESSION_NAME='" + dr["SESSION_NAME"].ToString() + "'";
                            _dbhelper.UpdateQuery(strQuery);
                            if (POSTERROR == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CONFLICT)
                            {
                                CommonFuntions.SendMailToMapper(ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_CONFLICT, string.Empty, Convert.ToInt32(dr["SESSION_NAME"].ToString().Substring(3)), dr["CIRCUITID"].ToString());
                                string sChangeUserName = ReadConfigurations.ChangeUserName;
                                (new GeoDBHelper()).ChangeSessionOwner(Convert.ToInt32(dr["SESSION_NAME"].ToString().Substring(3)), sChangeUserName);
                            }
                            else if (POSTERROR == ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_POSTERROR)
                            {
                                CommonFuntions.SendMailToSupportTeam(ReadConfigurations.STATUS_DAPHIEFILE_DET.Status_POSTERROR, dr["CIRCUITID"].ToString(),dr["BATCHID"].ToString(),string.Empty);
                            }
                        }
                    }
                }

            }
           catch (Exception exp)
            {
                //_log.Error(exp.Message + " |  Stack Trace | " + exp.StackTrace);
                CommonFuntions.WriteLine_Error(exp.Message + " |  Stack Trace | " + exp.StackTrace); 
            }
            
            return retval;
        }
    }
}
