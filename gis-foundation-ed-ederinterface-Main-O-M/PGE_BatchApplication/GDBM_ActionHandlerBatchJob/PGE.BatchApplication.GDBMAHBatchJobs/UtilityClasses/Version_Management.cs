// ========================================================================
// Copyright © 2021 PGE 
// <history>
// Reconcile and Post the Session
// YXA6 4/14/2021	Created
// JeeraID-> EDGISRearch-376
// </history>
// All rights reserved.
// ========================================================================
using System;
using ESRI.ArcGIS.Geodatabase;
using System.Configuration;

namespace PGE.BatchApplication.GDBMAHBatchJobs
{
    public class Version_Management
    {       
        Common CommonFuntions = new Common();
        DBHelper _dbhelper = new DBHelper();
     
        /// <summary>
        /// This function will return default ESRI Version
        /// </summary>
        /// <returns></returns>
        public IWorkspace FindVersionFromVersionName(string sVersion_Name,IWorkspace pParentDefaultworkspace)
        {
            IWorkspace pWorkspace = null;         
            if (!string.IsNullOrEmpty(sVersion_Name))
            {
                //GetVersion

                //App crash issue resolution
               
                IFeatureWorkspace fWSpace = (IFeatureWorkspace)pParentDefaultworkspace;
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
                    defaultVersion = (IVersion2)vWorkspace.FindVersion("SDE.DEFAULT");
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
        public bool ReconcileAndPostVersion(IVersion2 pChildVersion,IVersion2 Parentversion, ref string POSTERROR,string sCircuitID)
        {
            bool bOpeartionSuccess = false;
            IVersion defaultVersion = null;
            IWorkspaceEdit workspaceEdit = null;
            try
            {
                //Creating workspace using SDE user to POST the version 
                CommonFuntions.WriteLine_Info("Creating workspace using SDE user to POST the version ");

                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)Parentversion;
                defaultVersion = Parentversion as IVersion;
                CommonFuntions.WriteLine_Info("Version to Post: " + pChildVersion.VersionName );
                IMultiuserWorkspaceEdit muWorkspaceEdit = (IMultiuserWorkspaceEdit)pChildVersion;
                workspaceEdit = (IWorkspaceEdit2)pChildVersion;
                // IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;
                IVersionEdit versionEdit = (IVersionEdit)workspaceEdit;


                if (muWorkspaceEdit.SupportsMultiuserEditSessionMode(esriMultiuserEditSessionMode.esriMESMVersioned))
                {
                    muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                    //Reconcile with the default version.
                    // Keeping conlict in favour of edit version
                    CommonFuntions.WriteLine_Info("Start Reconciling the version..." );

                    //bool conflicts = versionEdit.Reconcile4(defaultVersion.VersionName, false, false, true, false);

                    bool conflicts = versionEdit.Reconcile(defaultVersion.VersionName);
                    if (conflicts == true)
                    {
                        CommonFuntions.WriteLine_Info("Conflict in Version Posting!!!Exiting Interface" );
                        POSTERROR = ReadConfigurations.STATUS.CONFLICT;
                    }
                    else
                    {
                        CommonFuntions.WriteLine_Info("No Conflict found, version Reconcile successfull." );

                    }
                    if (POSTERROR != ReadConfigurations.STATUS.CONFLICT)
                    {
                        CommonFuntions.WriteLine_Info("Start Editing in Workspace" );

                        workspaceEdit.StartEditing(true);
                        workspaceEdit.StartEditOperation();
                        if (versionEdit.CanPost())
                        {
                            CommonFuntions.WriteLine_Info("Version Post start " );

                            try
                            {
                                versionEdit.Post(defaultVersion.VersionName);
                                CommonFuntions.WriteLine_Info("Version Post end" );
                                CommonFuntions.WriteLine_Info("Stop editing in Workspace" );
                                workspaceEdit.StopEditOperation();
                                workspaceEdit.StopEditing(true);
                                CommonFuntions.WriteLine_Info("Editing stopped in Workspace" );
                                DeleteVersion(pChildVersion.VersionName);
                                bOpeartionSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                //_log.Info("Couldn't Stop editing in SDEworkspace.: " + ex.Message + " at " + ex.StackTrace);
                                CommonFuntions.WriteLine_Error("Couldn't Stop editing in SDEworkspace.: " + ex.Message + " at " + ex.StackTrace);
                                if (workspaceEdit.IsBeingEdited())
                                {
                                    workspaceEdit.StopEditOperation();
                                    workspaceEdit.StopEditing(false);
                                    workspaceEdit = null;
                                }
                                
                                bOpeartionSuccess = false;
                            }
                        }
                        else
                        {
                            CommonFuntions.WriteLine_Info("Cannot Post version " + defaultVersion.VersionName + ". Make SDE connection with user SDE only." );
                            POSTERROR = ReadConfigurations.STATUS.POSTERROR;
                            bOpeartionSuccess = false;
                        }
                    }

                }
            }
            catch (Exception exp)
            {
                
                CommonFuntions.WriteLine_Error("Error in version Post, function ReconcileAndPostVersion: " + exp.Message + " at " + exp.StackTrace);
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
                    IWorkspace workspace_sde = CommonFuntions.GetWorkspace(ConfigurationManager.AppSettings["SDEDefaultWorkSpaceConnString"]);
                    IVersion2 versionToDelete = ((IVersionedWorkspace)workspace_sde).FindVersion( sVersionName) as IVersion2;
                    try
                    {
                        versionToDelete.Delete();
                        CommonFuntions.WriteLine_Info("Version " + sVersionName + " deleted successfully.");

                    }
                    catch
                    {
                        
                    }
                    
                    blRetFlag = true;
                }
                catch (Exception exp)
                {
                    
                    CommonFuntions.WriteLine_Error("Error in deleting Version , function DeleteVersion " + exp.Message + " at " + exp.StackTrace); 
                    blRetFlag = false;
                }
            }
            return blRetFlag;
        }

      
    }
}
