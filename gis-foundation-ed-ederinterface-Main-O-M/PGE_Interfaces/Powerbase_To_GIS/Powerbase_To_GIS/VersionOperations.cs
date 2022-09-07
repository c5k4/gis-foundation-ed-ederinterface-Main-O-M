using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Data;
using Oracle.DataAccess.Client;
using System.Collections;
using System.Configuration;
using Miner.Interop.Process;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using Miner.Geodatabase;
using Miner.Process.GeodatabaseManager.Subtasks;
using PGE_DBPasswordManagement;

namespace Powerbase_To_GIS
{
    public class VersionOperations
    {       
        private static IMMPxApplication _pxApplication;
        public static IMMAppInitialize _mmAppInit;
        public static IMMSession _mmSession;
        private static string _versionName;
        public static int _sID = 0;

        /// <summary>
        /// This function will create Arc FM Session
        /// </summary>
        /// <returns></returns>
        public static bool CreateArcFMConnection()
        {
            bool _retval = false;
            IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();
            try
            {
                _pxApplication = new PxApplicationClass();
                _pxApplication.StandAlone = true;
                _pxApplication.Visible = false;

                //string connectionString = ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pbuser);
                string[] strconnection = ConfigurationManager.AppSettings["ConnectionString_pbuser"].Split('@');
                string smUser = strconnection[0];
                string smDB = strconnection[1];
                string smPwd = ReadEncryption.GetPassword(smUser + "@" + smDB);
                pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;

                _pxApplication.Startup(pxLogin);
                _retval = true;
            }
            catch (Exception exp)
            {
                Program.comment = "Error while Creating ArcFM Connection " + exp.Message;
                Common._log.Error("Error while Creating ArcFM Connection");
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                _retval = false;
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return _retval;
        }

        /// <summary>
        /// This function will create Arc FM Session
        /// </summary>
        /// <returns></returns>
        internal static bool CreateSession()
        {
            bool _retval = false;
            try
            {
                if (CreateArcFMConnection() == true)
                {
                    string sessionName = ReadConfigurations.GetValue(ReadConfigurations.SessionNamePrefix);
                    IMMSessionManager sessionMgr = (IMMSessionManager)_pxApplication.FindPxExtensionByName("MMSessionManager");
                    _mmSession = sessionMgr.CreateSession();
                    _sID = _mmSession.get_ID();
                    Common._log.Info("Session ID = " + _sID.ToString());
                    //Creating the session with name
                    sessionName = sessionName + "_" + _sID;
                    sessionName = Regex.Replace(sessionName, "/", "");
                    //_versionName = versionNamePrefix + _sID;
                    _versionName = sessionName;
                    Common._log.Info("Session Name = " + _versionName);
                    _mmSession.set_Name(sessionName);
                    _retval = true;
                }
            }
            catch (Exception exp)
            {
                Program.comment = "Exception occurred while creating ArcFM Session" + exp.Message;
                Common._log.Error("Exception occurred while creating ArcFM Session");
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                _retval = false;
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return _retval;
        }

        /// <summary>
        /// This function will create ESRI Version
        /// </summary>
        /// <returns></returns>
        public static IVersion2 CreateVersion(IWorkspace argWorkspace)
        {
            IVersion2 slversion = null;
            string slversionname = null;
            try
            {
                if (CreateSession() == true)
                {
                    slversionname = _versionName;

                    IVersionedWorkspace vWorkspace = (IVersionedWorkspace)argWorkspace;
                    {
                        IVersion sourceVersion = (IVersion2)vWorkspace.FindVersion(ReadConfigurations.defaultVersionName);
                        sourceVersion.CreateVersion(slversionname);
                        slversion = (IVersion2)vWorkspace.FindVersion(slversionname);
                        slversion.Access = esriVersionAccess.esriVersionAccessPublic;
                    }
                }
                else
                {
                    Common._log.Error("Issue occured in creating session.");
                }
            }
            catch (Exception exp)
            {
                Program.comment = "Exception in Function CreateVersion" + exp.Message;
                Common._log.Error(" Exception in Function CreateVersion. Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return slversion;
        }

        /// <summary>
        /// This function submits given session to GDBM
        /// </summary>
        /// <param name="session"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        public static bool SubmitSessionToGDBM(IMMSession session, IWorkspace workspace)
        {
            int _SessionState = GetSessionState();
            bool InsertpostQueue = false;
            try
            {
                IMMPxSubtask subtask = new SetPostingPriority();
                Miner.Interop.Process.IDictionary parameters = new MMPxNodeListClass();

                object k = "Priority";
                object d = Convert.ToInt32(ConfigurationManager.AppSettings["SESSION_PRIORITY"]);

                parameters.Add(ref k, ref d);

                object k1 = "ShowMessages";
                object d1 = false;
                parameters.Add(ref k1, ref d1);
                ((SetPostingPriority)subtask).Parameters = parameters;

                bool initialized = subtask.Initialize(_pxApplication);
                Common._log.Info("Initialized [ " + initialized + " ]");

                IMMPxNodeEdit nodeedit = new MMPxNodeListClass();
                int nodetype = _pxApplication.Helper.GetNodeTypeID("Session");
                nodeedit.Initialize(nodetype, "Session", _mmSession.get_ID());
                IMMPxNode node = (IMMPxNode)nodeedit;
                ((IMMPxApplicationEx)_pxApplication).HydrateNodeFromDB(node);
                //_logger.Debug("Setting Node State to Posting Queue");
                SetNodeState(_pxApplication, node, _SessionState);
                if (InsertSessionIntoPostingQueue(session, workspace) == true)
                {
                    InsertpostQueue = true;
                }
            }
            catch (Exception exp)
            {
                Program.comment = "Exception" + exp.Message;
                InsertpostQueue = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return InsertpostQueue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pxApp"></param>
        /// <param name="pxNode"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static bool SetNodeState(IMMPxApplication pxApp, IMMPxNode pxNode, int state)
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
            catch (Exception exp)
            {
                Program.comment = "Exception" + exp.Message;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return nodeState;
        }

        /// <summary>
        /// This function return the session state
        /// </summary>
        /// <returns></returns>
        private static int GetSessionState()
        {
            //set state =9 so that if code fail then session goes to default data processing state
            int _state = 9;
            try
            {
                _state = Convert.ToInt32(ConfigurationManager.AppSettings["SESSION_STATE"].ToString());
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return _state;
        }

        /// <summary>
        /// This function submit the session to posting queue
        /// </summary>
        /// <param name="session"></param>
        /// <param name="workspace"></param>
        /// <returns></returns>
        private static bool InsertSessionIntoPostingQueue(IMMSession session, IWorkspace workspace)
        {

            //_logger.Debug(MethodBase.GetCurrentMethod().Name);
            bool InsertSuccess = false;
            try
            {
                ITable table = null;
                IRow newRow = null;
                IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)workspace;
                //((IWorkspaceEdit)versionedWorkspace).StartEditing(false);
                table = ((IFeatureWorkspace)versionedWorkspace).OpenTable(ReadConfigurations.TB_GDBM_POST_QUEUE);

                newRow = table.CreateRow();
                newRow.set_Value(newRow.Fields.FindField("SUBMIT_TIME"), DateTime.Now);
                newRow.set_Value(newRow.Fields.FindField("CURRENTUSER"), session.get_Owner().ToUpper());
                newRow.set_Value(newRow.Fields.FindField("VERSION_OWNER"), session.get_Owner().ToUpper());
                newRow.set_Value(newRow.Fields.FindField("VERSION_NAME"), "SN_" + session.get_ID());
                newRow.set_Value(newRow.Fields.FindField("DESCRIPTION"), session.get_Description());
                newRow.set_Value(newRow.Fields.FindField("PRIORITY"), 20);
                newRow.set_Value(newRow.Fields.FindField("PX_NODE_ID"), session.get_ID());
                newRow.set_Value(newRow.Fields.FindField("NODE_TYPE_NAME"), "Session");
                newRow.set_Value(newRow.Fields.FindField("NODE_TYPE_ID"), 3);
                //_logger.Debug("Creating Row [ SDE.GDBM_POST_QUEUE ]");

                newRow.Store();

                // ((IWorkspaceEdit)versionedWorkspace).StopEditing(true);
                InsertSuccess = true;
                Marshal.FinalReleaseComObject(newRow);
                Marshal.FinalReleaseComObject(table);
                Marshal.FinalReleaseComObject(versionedWorkspace);
            }
            catch (Exception exp)
            {
                Program.comment = "Exception" + exp.Message;
                InsertSuccess = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return InsertSuccess;
        }

        /// <summary>
        /// This function will check the status of the session for previous run, The return values may be POSTED,DELETED,ERROR
        /// </summary>
        /// <param name="argStrConnStringpgedata"></param>
        /// <returns></returns>
        public static string CheckPreviousRunSessionStatus(string argStrConnStringpgedata, string argStrConnStringpbuser)
        {
            string strSessionName = null;
            string strSessionStatus = string.Empty;
            try
            {
                strSessionName = GetSessionNameToCheck(argStrConnStringpgedata);
                Common._log.Info("Previous session : " + strSessionName);

                if (!string.IsNullOrEmpty(strSessionName))
                {
                    string strQuery = "select POST_RESULT from " + ReadConfigurations.TB_GDBM_POST_HISTORY + " where version_name = '" + strSessionName + "' and post_result='POSTED'";
                    DataRow pRow = DBHelper.GetSingleDataRowByQuery(argStrConnStringpbuser, strQuery);
                    if (pRow != null)
                    {
                        Common._log.Info("Previous session " + strSessionName + " has been successfully posted through gdbm.");
                        strSessionStatus = ReadConfigurations.SESSION_STATUS_POSTED;
                    }
                    else
                    {
                        // Check if session got deleted or in other state 
                        int sSessionID = Convert.ToInt32(strSessionName.Split('.')[1].Substring(3));//remove username and SN_      

                        string sQuery = "SELECT Current_OWNER FROM " + ReadConfigurations.TB_MM_SessionTableName + " WHERE SESSION_ID = '" + sSessionID + "' and hidden=0";
                        DataTable pDTPOSTED = DBHelper.GetDataTable(argStrConnStringpbuser, sQuery);
                        if ((pDTPOSTED != null) && (pDTPOSTED.Rows.Count > 0))
                        {
                            string CurrentOwner = pDTPOSTED.Rows[0].ItemArray[0].ToString();
                            Common._log.Info(" Session CurrentOwner : " + CurrentOwner + "," + DateTime.Now);

                            sQuery = "select State from " + ReadConfigurations.TB_MM_PxStateTableName + " where STATE_ID = (select STATE_ID from " + ReadConfigurations.TB_MM_CurrentPxStateTableName + " where SOID=" + sSessionID + ")";
                            Common._log.Info("Query to check STATE  in GDBM-----  " + sQuery);

                            DataTable dtResults = DBHelper.GetDataTable(argStrConnStringpbuser, sQuery);
                            if ((dtResults != null) && (dtResults.Rows.Count > 0))
                            {
                                int stateID = Convert.ToInt32(dtResults.Rows[0].ItemArray[0].ToString());
                                strSessionStatus = GetSessionStatusFromStateID(stateID);
                            }
                            else
                            {
                                strSessionStatus = ReadConfigurations.SESSION_STATUS_UNKNOWN;
                            }
                        }
                        else
                        {
                            Common._log.Info("Session is deleted from the database.");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_DELETED;
                        }                        
                    }
                }
                else
                {
                    Common._log.Info("The previous session name could not be find from table, session name :" + strSessionName);
                }
            }
            catch (Exception exp)
            {
                Program.comment = "Exception" + exp.Message;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return strSessionStatus;
        }

        /// <summary>
        /// This function returns the session name to check
        /// </summary>
        /// <param name="argStrConnString"></param>
        /// <returns></returns>
        public static string GetSessionNameToCheck(string argStrConnString)
        {
            string SessionName = null;
            string query = null;
            DataRow pRow = null;
            try
            {
                query = "select " + ReadConfigurations.col_SESSION_NAME + " from " + ReadConfigurations.GetValue(ReadConfigurations.TB_PB_Session);
                pRow = DBHelper.GetSingleDataRowByQuery(argStrConnString, query);

                if (pRow != null)
                {
                    SessionName = Convert.ToString(pRow[ReadConfigurations.col_SESSION_NAME]);
                }
            }
            catch (Exception exp)
            {
                Program.comment = "Exception" + exp.Message;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return SessionName;
        }

        /// <summary>
        /// THis function returns the session status from given state ID
        /// </summary>
        /// <param name="argStateID"></param>
        /// <returns></returns>
        private static string GetSessionStatusFromStateID(int argStateID)
        {
            string strSessionStatus = null;
            try
            {
                switch (argStateID)
                {
                    case 1:
                        {
                            Common._log.Info("Session is in INPROGRESS state");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_INPROGRESS;
                            break;
                        }
                    case 2:
                        {
                            Common._log.Info("Session is in PENDING_QAQC state");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_PENDING_QAQC;
                            break;
                        }
                    case 4:
                        {
                            Common._log.Info("Session is in HOLD state");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_HOLD;
                            break;
                        }
                    case 5:
                        {
                            Common._log.Info("Session is in POST_QUEUE state");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_POST_QUEUE;
                            break;
                        }
                    case 6:
                        {
                            Common._log.Info("Session is in POST_ERROR state");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_POST_ERROR;
                            break;
                        }
                    case 7:
                        {
                            Common._log.Info("Session is in RECONCILE_ERROR state");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_RECONCILE_ERROR;
                            break;
                        }
                    case 8:
                        {
                            Common._log.Info("Session is in CONFLICT state");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_CONFLICT;
                            break;
                        }
                    case 9:
                        {
                            Common._log.Info("Session is in DATA_PROCESSING state");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_DATA_PROCESSING;
                            break;
                        }
                    case 10:
                        {
                            Common._log.Info("Session is in QA_QC_ERROR state");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_QA_QC_ERROR;
                            break;
                        }
                    default:
                        {
                            Common._log.Info("Session is not present in the GDBM QUEUE , Please check with administrator.");
                            strSessionStatus = ReadConfigurations.SESSION_STATUS_UNKNOWN;
                            break;
                        }
                }

            }
            catch (Exception exp)
            {
                Program.comment = "Exception" + exp.Message;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return strSessionStatus;
        }
    }
}
