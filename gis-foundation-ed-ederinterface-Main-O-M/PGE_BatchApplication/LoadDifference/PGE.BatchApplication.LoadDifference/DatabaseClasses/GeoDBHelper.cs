// ========================================================================
// Copyright © 2021 PGE 
// <history>
// Common GeoDatabase Handler Methods 
// YXA6 4/14/2021	Created
// JeeraID-> EDGISRearch-376
// </history>
// All rights reserved.
// ========================================================================
using System;
using ESRI.ArcGIS.Geodatabase;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Data;
using Miner.ComCategories;
using Miner.Interop.Process;
using Miner.Process.GeodatabaseManager.Subtasks;

namespace PGE.BatchApplication.LoadDifference
{
    public  class GeoDBHelper
    {
       
       // public  Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        Common CommonFuntions= new Common();
        DBHelper DBHelper = new DBHelper();
        #region Getting Workspace
        public  IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {
                CommonFuntions.WriteLine_Info("Connection string used in process .");
                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
                CommonFuntions.WriteLine_Info("Workspace checked out successfully.");

                if (workspace == null)
                {
                    CommonFuntions.WriteLine_Error("Workspace not found for conn string : " + argStrworkSpaceConnectionstring);
                    throw new Exception("Exiting the process.");

                }

            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error("Error in getting SDE Workspace, function GetWorkspace: " + exp.Message);
                CommonFuntions.WriteLine_Info(exp.Message + "   " + exp.StackTrace);
                throw exp;
            }
            return workspace;
        }

        internal IVersion RollVersion(IWorkspace pdefaultworkspace ,string sversionname)
        {
           

            IVersionedWorkspace vWSpace = (IVersionedWorkspace)pdefaultworkspace;
            IVersion pbkpVersion, pTargetVersion = null,oldVersion;
            try
            {
                pbkpVersion = FindVersion(vWSpace, sversionname + "_bkp");
                oldVersion = FindVersion(vWSpace, sversionname);
                if ((pbkpVersion == null) && (oldVersion == null))
                {
                    pbkpVersion = CreateVersion(pdefaultworkspace as IFeatureWorkspace, ReadConfigurations.VersionName + "_bkp") as IVersion2;
                    CommonFuntions.WriteLine_Warn("Neither backup nor target version found , create the backup version for further processing");
                    pTargetVersion = FindTargetVersion(vWSpace, pbkpVersion, oldVersion, ReadConfigurations.VersionName);
                }
                else if ((pbkpVersion != null) || (oldVersion != null))
                {
                    pTargetVersion=FindTargetVersion(vWSpace, pbkpVersion, oldVersion, ReadConfigurations.VersionName);
                }
               
                if (pTargetVersion != null)
                {
                    CommonFuntions.WriteLine_Info("Target Version Created Successfully");
                }
                IVersion2 DailyEDERTempVersion = CreateVersion(pdefaultworkspace as IFeatureWorkspace, ReadConfigurations.VersionName + "_bkp") as IVersion2;
                CommonFuntions.WriteLine_Info("Backup Version Created Successfully");
            }
            catch (Exception EX)
            {
                throw EX;
            }
            return pbkpVersion;
        }

        private IVersion FindTargetVersion(IVersionedWorkspace vWSpace, IVersion pbkpVersion, IVersion OldTargetVersion,string sversionname)
        {
            IVersion pTargetVersion = null;
            try
            {
                if (pbkpVersion != null)
                {
                    //Start rollout of Version
                    if (OldTargetVersion != null)
                    {
                        //Delete Target and then rename backup version as target version.
                        if (VersionDeleted(OldTargetVersion))
                        {
                            pbkpVersion.VersionName = sversionname;
                        }
                        pTargetVersion = FindVersion(vWSpace, sversionname);
                        if (pTargetVersion == null)
                        {
                            pTargetVersion = pbkpVersion;
                        }
                    }
                    else
                    {
                        // Rename the backupversion
                        pbkpVersion.VersionName = sversionname;
                        pTargetVersion = FindVersion(vWSpace, sversionname);
                        if (pTargetVersion == null)
                        {
                            pTargetVersion = pbkpVersion;
                        }
                    }
                }
                else
                {
                    if (OldTargetVersion != null)
                    {
                        
                        pTargetVersion = OldTargetVersion;
                        
                    }
                }
                if (pTargetVersion == null)
                {
                    pTargetVersion = FindVersion(vWSpace, sversionname);
                }
            }
            catch { }
            return pTargetVersion;
        }

        private bool VersionDeleted(IVersion pTargetVersion)
        {
            bool deletesuccess = false;
            try
            {
                pTargetVersion.Delete();
                deletesuccess = true;
            }
            catch { }
            return deletesuccess;
        }

        public IVersion FindVersion(IVersionedWorkspace vWSpace, string sversionname)
        {
            IVersion _pVersion=null;
            try
            {
                _pVersion = vWSpace.FindVersion(sversionname);
               
            }
            catch 
            {
                
            }
            return _pVersion;

        }

        internal void DeleteVersion(IVersion pversion)
        {
            try
            {
                pversion.Delete();
            }
            catch(Exception ex)
            { throw ex; }
        }
        #endregion

        #region ArcFM Function

        internal  IWorkspace CreateVersion(IFeatureWorkspace fWSpace,string sversionname)
        {
            
                IWorkspace pVersionWorkspace=null;
            
                IVersionedWorkspace vWSpace = (IVersionedWorkspace)fWSpace;
                IVersion pVersion;
                

                try
                {
                    pVersion = vWSpace.FindVersion(sversionname);
                    pVersion.Delete();

                }
                catch (Exception EX)
                {
                    //throw EX; 
                }

                try
                {
                    pVersion = ((IVersionedWorkspace)fWSpace).DefaultVersion.CreateVersion(sversionname);
                    pVersion.Access = esriVersionAccess.esriVersionAccessPublic;
                    pVersionWorkspace = (IWorkspace)pVersion;
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Unable to create version specified: " + ReadConfigurations.VersionName + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                    throw ex;
                }
            
            return pVersionWorkspace;
        }
       

        private int GetSessionState()
        {
            //set state =9 so that if code fail then session goes to default data processing state
            int _state = 9;
            try
            {
                _state = Convert.ToInt32(ConfigurationManager.AppSettings["SESSION_STATE"].ToString());
            }
            catch (Exception ex)
            {                
                CommonFuntions.WriteLine_Error("Unhandled exception encountered while getting the Session_State from config file"  + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _state;
        }

        public  bool SetNodeState(IMMPxApplication pxApp, IMMPxNode pxNode, int state)
        {
            bool nodeState = false;
            try
            {
                IMMEnumPxState enumPxStates = pxApp.States;
                enumPxStates.Reset();

                IMMPxState pxState = null;

                while ((pxState = enumPxStates.Next()) != null)
                {
                    if (pxState.State == state & pxState.NodeType == pxNode.NodeType)
                    {
                        break;
                    }
                }

                if (pxState != null)
                {
                    ((IMMPxNodeEdit)pxNode).State = pxState;
                    ((IMMPxApplicationEx)pxApp).UpdateNodeToDB(pxNode);
                    nodeState = true;
                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Unhandled exception encountered while setting the node state " + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return nodeState;
        }

       
        //internal  bool ChangeSessionOwner(int sSessionID, string sUserName)
        //{
        //    bool _retval = false;
        //    try
        //    {
        //        if (CheckUserExist(sUserName) == true)
        //        {
        //            //Update User or Change Current User
        //            string sSQL = "Update "  + ReadConfigurations.Process_TableName.MM_SessionTableName + " SET CURRENT_OWNER = '" + sUserName + "' WHERE SESSION_ID = " + sSessionID;
        //            CommonFuntions._log.Debug("Assign Session : " + sSessionID + " to User  : " + sUserName   );

        //            DBHelper.ExecuteScalerQuery(sSQL);

        //            // "Update State"
        //            sSQL = "select state_ID from " + ReadConfigurations.Process_TableName.MM_PxStateTableName + " where Upper(Name)='IN PROGRESS'";
        //            DataTable dt = DBHelper.GetDataTableByQuery(sSQL);
        //            if (dt.Rows.Count > 0)
        //            {
        //                string StateID = dt.Rows[0].ItemArray[0].ToString();
        //                sSQL = "Update " + ReadConfigurations.Process_TableName.MM_CurrentPxStateTableName + " set STATE_ID= " + StateID + " where SOID=" + sSessionID;
        //                DBHelper.ExecuteScalerQuery(sSQL);
        //                CommonFuntions.WriteLine_Info("Change Session State QueryString= " + sSQL);
        //                _retval = true;
        //            }
        //        }
        //        else
        //        {
        //            CommonFuntions.WriteLine_Error("User does not exist in database = " + sUserName);
        //           throw new Exception("User does not exist in database = " + sUserName );

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonFuntions.WriteLine_Error("Unhandled exception encountered while changing the session owner: " + ex.Message.ToString() + " at " + ex.StackTrace);
        //        _retval = false;
        //        throw ex;
        //    }
        //    return _retval;
        //}

        //private bool CheckUserExist(string sUserName)
        //{
        //        bool _retval = false;
        //    try
        //    {
        //        string  strquery= " select * from "  + ReadConfigurations.Process_TableName.MM_PxUserTableName + " where USER_NAME='" + sUserName + "'";
        //        DataTable dt = DBHelper.GetDataTableByQuery(strquery);
        //         if ((dt !=null) && (dt.Rows.Count > 0))
        //         {
        //             _retval = true;
        //         }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonFuntions.WriteLine_Error("Unhandled exception encountered while checking that user is exist in the database or not : " + ex.Message.ToString() + " at " + ex.StackTrace);
        //        _retval = false;
        //    }
        //    return _retval;
        //}

        #endregion

        internal  IFeatureClass OpenFeatureclass(IFeatureWorkspace pFeatureWorkspace, string FeatureClassName)
        {
            IFeatureClass _ReturnFeatureclass = null;
            try
            {
               _ReturnFeatureclass= pFeatureWorkspace.OpenFeatureClass(FeatureClassName);
               if (_ReturnFeatureclass == null)
                {
                    CommonFuntions.WriteLine_Error("Queried Featureclass is not getting from the database." + FeatureClassName);
                }
            }
            catch(Exception ex)
            {
                CommonFuntions.WriteLine_Error("Queried Featureclass is not getting from the database." + FeatureClassName + ex.Message.ToString() + " at " + ex.StackTrace);
                _ReturnFeatureclass = null;
            }

            return _ReturnFeatureclass;
        }
        
    }

}
