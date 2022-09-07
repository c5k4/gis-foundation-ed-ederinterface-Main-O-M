using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop.Process;
using System.Configuration;
using System.Text.RegularExpressions;
using Miner.Process.GeodatabaseManager.Subtasks;
using System.Runtime.InteropServices;
using System.Data;

namespace PGE.BatchApplication.IGPPhaseUpdate
{
    public  class GeoDBHelper
    {
       
        public  IMMSession m_mmSession;
        private  string m_sVersionName;
        public  int sID;
        public  Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
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
                CommonFuntions.WriteLine_Info("Connection string used in process ." + ReadConfigurations.GetValue(ReadConfigurations.SDEWorkSpaceConnString));
                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
                CommonFuntions.WriteLine_Info("Workspace checked out successfully.");

                if (workspace == null)
                {
                    CommonFuntions.WriteLine_Error("Workspace not found for conn string : " + ReadConfigurations.GetValue(ReadConfigurations.SDEWorkSpaceConnString));
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
        #endregion

        #region ArcFM Function
        public  bool CreateArcFMConnection( )
        {
            bool _retval = false;
            IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();
            try
            {
                MainClass.m_pxApplication = new PxApplicationClass();
                MainClass.m_pxApplication.StandAlone = true;
                MainClass.m_pxApplication.Visible = false;
                // M4JF EDGISREARCH 919
                //string[] ConnectionInfo = ReadConfigurations.OracleConnString.Split(',');
                //string smDB = ConnectionInfo[1];
                //string smUser = ConnectionInfo[2];
                //string smPwd = ConnectionInfo[3];



                // pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;
                pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;"+ReadConfigurations.OracleConnString;
                 MainClass.m_pxApplication.Startup(pxLogin);
                  _retval = true;

            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Error while Creating ArcFM Connection");
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                _retval = false;
            }
            return _retval;
        }

        internal  bool CreateSession( )
        {
              bool _retval = false;
            try
            {
                if (MainClass.m_pxApplication!=null)
                {
                    string versionNamePrefix = ConfigurationManager.AppSettings["VersionNamePrefix"];

                    string sessionName = ConfigurationManager.AppSettings["SessionName"];

                    IMMSessionManager sessionMgr = (IMMSessionManager)MainClass.m_pxApplication.FindPxExtensionByName("MMSessionManager");
                    m_mmSession = sessionMgr.CreateSession();

                     sID = m_mmSession.get_ID();
                    CommonFuntions.WriteLine_Info("Session ID = " + sID.ToString());
                    sessionName = sessionName + "_" + sID.ToString();
                    sessionName = Regex.Replace(sessionName, "/", "");
                    m_sVersionName = versionNamePrefix + sID;
                    //m_sVersionName = sessionName;
                    CommonFuntions.WriteLine_Info("Version Name = " + m_sVersionName);
                    m_mmSession.set_Name(sessionName);

                    _retval = true;
                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception occurred while creating ArcFM Session");
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                _retval = false;
            }
            return _retval;
        }

        internal  IWorkspace CreateVersion()
        {
            IWorkspace pVersionWorkspace=null;
            if (CreateSession() == true)
            {
                IFeatureWorkspace fWSpace = (IFeatureWorkspace)MainClass.m_SDEDefaultworkspace;
                IVersionedWorkspace vWSpace = (IVersionedWorkspace)fWSpace;
                IVersion pVersion;
                

                try
                {
                    pVersion = vWSpace.FindVersion(m_sVersionName);
                    pVersion.Delete();

                }
                catch (Exception EX)
                {
                     
                }

                try
                {
                    pVersion = ((IVersionedWorkspace)fWSpace).DefaultVersion.CreateVersion(m_sVersionName);
                    pVersion.Access = esriVersionAccess.esriVersionAccessPublic;

                    pVersionWorkspace = (IWorkspace)pVersion;
                }
                catch (Exception ex)
                {
                    CommonFuntions.WriteLine_Error("Unable to create version specified: " + m_sVersionName + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                    return null;
                }
            }
            return pVersionWorkspace;
        }

        private  int GetSessionState()
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

       
        internal  bool ChangeSessionOwner(int sSessionID, string sUserName)
        {
            bool _retval = false;
            try
            {
                if (CheckUserExist(sUserName) == true)
                {
                    //Update User or Change Current User
                    string sSQL = "Update "  + ReadConfigurations.Process_TableName.MM_SessionTableName + " SET CURRENT_OWNER = '" + sUserName + "' WHERE SESSION_ID = " + sSessionID;
                    CommonFuntions._log.Debug("Assign Session : " + sSessionID + " to User  : " + sUserName   );

                    DBHelper.ExecuteScalerQuery(sSQL);

                    // "Update State"
                    sSQL = "select state_ID from " + ReadConfigurations.Process_TableName.MM_PxStateTableName + " where Upper(Name)='IN PROGRESS'";
                    DataTable dt = DBHelper.GetDataTableByQuery(sSQL);
                    if (dt.Rows.Count > 0)
                    {
                        string StateID = dt.Rows[0].ItemArray[0].ToString();
                        sSQL = "Update " + ReadConfigurations.Process_TableName.MM_CurrentPxStateTableName + " set STATE_ID= " + StateID + " where SOID=" + sSessionID;
                        DBHelper.ExecuteScalerQuery(sSQL);
                        CommonFuntions.WriteLine_Info("Change Session State QueryString= " + sSQL);
                        _retval = true;
                    }
                }
                else
                {
                    CommonFuntions.WriteLine_Error("User does not exist in database = " + sUserName);
                   throw new Exception("User does not exist in database = " + sUserName );

                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Unhandled exception encountered while changing the session owner: " + ex.Message.ToString() + " at " + ex.StackTrace);
                _retval = false;
                throw ex;
            }
            return _retval;
        }

        private bool CheckUserExist(string sUserName)
        {
                bool _retval = false;
            try
            {
                string  strquery= " select * from "  + ReadConfigurations.Process_TableName.MM_PxUserTableName + " where USER_NAME='" + sUserName + "'";
                DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                 if ((dt !=null) && (dt.Rows.Count > 0))
                 {
                     _retval = true;
                 }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Unhandled exception encountered while checking that user is exist in the database or not : " + ex.Message.ToString() + " at " + ex.StackTrace);
                _retval = false;
            }
            return _retval;
        }

        public void SubmitSessionToGDBM(IWorkspace pworkspace,int SessionID)
        {
         
            try
            {
                bool RowExist = false;
                string strquery = "select count(*) from SDE.GDBM_POST_QUEUE where VERSION_NAME='SN_" + SessionID + "'";
                DataTable dt = (new DBHelper()).GetDataTable(strquery);
                if (dt != null)
                {
                    if (Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString()) > 0)
                    {
                        RowExist = true;
                    }
                }
                if (RowExist == false)
                {
                    CommonFuntions.WriteLine_Info("Send Session to GDBM Queue started with SessionName = SN_" + SessionID.ToString() + " ,at," + DateTime.Now);
                    //const int DATA_PROCESSING_STATE = 9;//10;
                    int POST_QUEUE_STATE = Convert.ToInt32(ReadConfigurations.POST_QUEUE_STATE);
                    //get Session
                    bool readOnly = false;
                    IMMSessionManager sessionMgr = (IMMSessionManager)MainClass.m_pxApplication.FindPxExtensionByName("MMSessionManager");
                    IMMSession _mmSession = sessionMgr.GetSession(ref SessionID, ref readOnly);
                    if (_mmSession == null)
                    {
                        throw new Exception("Session does not exist in database.Session Name:- " + SessionID);
                    }
                    if (readOnly)
                    {
                        _mmSession = null;
                        CommonFuntions.WriteLine_Info("Session is Locked or ReadOnly.");
                    }

                    IMMPxSubtask subtask = new SetPostingPriority();
                    Miner.Interop.Process.IDictionary parameters = new MMPxNodeListClass();

                    object k = "Priority";
                    object d = ReadConfigurations.GDBMPriority;
                    parameters.Add(ref k, ref d);

                    object k1 = "ShowMessages";
                    object d1 = false;
                    parameters.Add(ref k1, ref d1);
                    ((SetPostingPriority)subtask).Parameters = parameters;

                    bool initialized = subtask.Initialize(MainClass.m_pxApplication);
                    Console.WriteLine("Initialized [ " + initialized + " ]");

                    IMMPxNodeEdit nodeedit = new MMPxNodeListClass();
                    int nodetype = MainClass.m_pxApplication.Helper.GetNodeTypeID("Session");
                    nodeedit.Initialize(nodetype, "Session", SessionID);
                    IMMPxNode node = (IMMPxNode)nodeedit;
                    ((IMMPxApplicationEx)MainClass.m_pxApplication).HydrateNodeFromDB(node);

                    //SetNodeState(MainClass.m_pxApplication, node, 2); // 2 == Pending QA / QC
                    //bool executedSuccessfully = subtask.Execute(node);

                    //CommonFuntions._log.Debug("Setting Node State to POST_QUEUE");
                    SetNodeState(MainClass.m_pxApplication, node, POST_QUEUE_STATE);
                    InsertSessionIntoPostingQueue(_mmSession, pworkspace);
                    CommonFuntions.WriteLine_Info("Send Session to GDBM Queue Completed  " + " at," + DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                // Try to Set the node state to an error state?

                throw ex;
            }

        }
        private bool InsertSessionIntoPostingQueue(IMMSession session, IWorkspace workspace)
        {
            bool InsertSuccess = false;
            try
            {
                ITable table = null;
                IRow newRow = null;
                IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)workspace;
                ((IWorkspaceEdit)versionedWorkspace).StartEditing(false);
                table = ((IFeatureWorkspace)versionedWorkspace).OpenTable("SDE.GDBM_POST_QUEUE");                           
                newRow = table.CreateRow();

                newRow.set_Value(newRow.Fields.FindField("SUBMIT_TIME"), DateTime.Now);
                newRow.set_Value(newRow.Fields.FindField("CURRENTUSER"), session.get_Owner().ToUpper());
                newRow.set_Value(newRow.Fields.FindField("VERSION_OWNER"), session.get_Owner().ToUpper());
                newRow.set_Value(newRow.Fields.FindField("VERSION_NAME"), "SN_" + session.get_ID());
                newRow.set_Value(newRow.Fields.FindField("DESCRIPTION"), session.get_Description());
                newRow.set_Value(newRow.Fields.FindField("PRIORITY"), ReadConfigurations.GDBMPriority);
                newRow.set_Value(newRow.Fields.FindField("PX_NODE_ID"), session.get_ID());
                newRow.set_Value(newRow.Fields.FindField("NODE_TYPE_NAME"), "Session");
                newRow.set_Value(newRow.Fields.FindField("NODE_TYPE_ID"), 3);

                newRow.Store();
                
                ((IWorkspaceEdit)versionedWorkspace).StopEditing(true);
                CommonFuntions.WriteLine_Info("Creating Row [ SDE.GDBM_POST_QUEUE ]");

                InsertSuccess = true;
                Marshal.FinalReleaseComObject(newRow);
                Marshal.FinalReleaseComObject(table);
                Marshal.FinalReleaseComObject(versionedWorkspace);
            }

            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Error occurred while session sending to posting queue: " + ex.Message.ToString() + " at " + ex.StackTrace);
                InsertSuccess = false;
                throw ex;
            }
            return InsertSuccess;
        }

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
