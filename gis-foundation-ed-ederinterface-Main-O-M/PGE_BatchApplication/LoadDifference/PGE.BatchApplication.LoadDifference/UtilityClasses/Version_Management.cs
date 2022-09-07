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

namespace PGE.BatchApplication.LoadDifference
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
