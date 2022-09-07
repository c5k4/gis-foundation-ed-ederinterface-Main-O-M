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



namespace PGE.BatchApplication.ConductorInTrench
{
    public class VersionOperations
    {
        public static IVersion2 _PriUGDailyVersion;
        public static string _servicelocationGUID = string.Empty;
        public static int _genCategory = 0;
        public static IMMPxLogin _pxLogin;
        public static IWorkspace _workspace;        
        private static IMMPxApplication _pxApplication;
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        public static OracleConnection _conOraEDER = null;        
        public static IMMAppInitialize _mmAppInit;
        public static IMMSession _mmSession;
        private static string _versionName;
        public static int _sID = 0;       

        /// <summary>
        /// This function will create Arc FM Session
        /// </summary>
        /// <returns></returns>
        public static bool CreateArcFMConnection( )
        {
            bool _retval = false;
            IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();
            try
            {
                _pxApplication = new PxApplicationClass();
                _pxApplication.StandAlone = true;
                _pxApplication.Visible = false;

                // M4JF EDGISREARCH 919 - Get connection string using Password Manageent tool
                
                //string smUser = ConfigurationManager.AppSettings["SMUser"];
                //string smPwd = ConfigurationManager.AppSettings["SMUPWD"];
                //string smDB = ConfigurationManager.AppSettings["SMDB"];
                string versionNamePrefix = ConfigurationManager.AppSettings["VersionNamePrefix"];

                // pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;
                pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;" + ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
               _pxApplication.Startup(pxLogin);
               _retval = true;
            }
            catch (Exception exp)
            {
                Common._log.Error("Error while Creating ArcFM Connection");
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
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
                    //string versionNamePrefix = ConfigurationManager.AppSettings["VersionNamePrefix"];
                    string sessionName = ConfigurationManager.AppSettings["SessionNamePrefix"];
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
                Common._log.Error("Exception occurred while creating ArcFM Session");
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
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
                    _log.Error("Issue occured in creating session.");
                }
            }
            catch (Exception exp)
            {
                _log.Error(" Exception in Function CreateVersion. Exception : "+exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return slversion;
        }

        /// <summary>
        /// This function will return default ESRI Version
        /// </summary>
        /// <returns></returns>
        public static IVersion GetDefaultVersion(IWorkspace argWorkspace)
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
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return defaultVersion;
        }

        /// <summary>
        /// This function will return default ESRI Version
        /// </summary>
        /// <returns></returns>
        public static IVersion2 GetRequiredVersion(IWorkspace argWorkspace,string argStrVersionName)
        {
            IVersion2 pVersion = null;
            try
            {
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)argWorkspace;
                {
                    pVersion = (IVersion2)vWorkspace.FindVersion(argStrVersionName);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return pVersion;
        }     

        /// <summary>
        /// This function will delete ESRI version
        /// </summary>
        /// <returns></returns>
        public static Boolean DeleteVersion(string sVersionName, string strConnString)
        {
            bool blRetFlag = false;
            if (!(string.IsNullOrEmpty(sVersionName)))
            {
                try
                {
                    // M4JF EDGISREARCH 919
                    IWorkspace workspace_sde = MainClass.GetWorkspace(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection_SDE"].ToUpper()));
                    IVersion2 versionToDelete = ((IVersionedWorkspace)workspace_sde).FindVersion(sVersionName) as IVersion2;
                    versionToDelete.Delete();
                    _log.Info("Version " + sVersionName + " deleted successfully.");

                    DeleteSession(_versionName, strConnString);                   
                    blRetFlag = true;
                }
                catch (Exception exp)
                {
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                    blRetFlag = false;                    
                }
            }
            return blRetFlag;
        }

        /// <summary>
        /// This function will delete Arc FM Session
        /// </summary>
        /// <returns></returns>
        private static void DeleteSession(string p, string strConnString)
        {
            try
            {
                string strQuery = "select * from process.MM_PX_CURRENT_STATE where SOID=" + _sID;
                DataRow dr= DBHelper.GetSingleDataRowByQuery(strConnString, strQuery);
                if (dr != null)
                {
                    strQuery = "Delete from process.MM_PX_CURRENT_STATE where SOID=" + _sID;
                    DBHelper.UpdateQuery(strConnString, strQuery);
                }
                 strQuery = "Delete from process.mm_session where session_id=" + _sID;
                DBHelper.UpdateQuery(strConnString, strQuery);
                strQuery = "Delete from process.MM_PX_VERSIONS  where NAME='" + p + "'";
                DBHelper.UpdateQuery(strConnString, strQuery); 
            }
            catch(Exception exp) 
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
        }

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
                InsertpostQueue = false;
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
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
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }           
            return nodeState;
        }

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
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            } 
            return _state;
        }

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
                table = ((IFeatureWorkspace)versionedWorkspace).OpenTable("SDE.GDBM_POST_QUEUE");

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
                InsertSuccess = false;
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }            
            return InsertSuccess;
        }

        public static bool CheckPreviousRunSessionPosted(string argStrConnString,ref string argStrVersionName)
        {
            string strVersionName = null;
            bool bPreviousSessionPosted = false;
            try
            {
                strVersionName = GetSessionNameToCheck(argStrConnString);

                if (!string.IsNullOrEmpty(strVersionName))
                {
                    string strQuery = "select POST_RESULT from sde.gdbm_post_history where version_name = '" + strVersionName + "' and post_result='POSTED'";
                    DataRow pRow = DBHelper.GetSingleDataRowByQuery(argStrConnString, strQuery);
                    if (pRow != null)
                    {
                        _log.Info("Previous session " + strVersionName + " has been successfully posted through gdbm.");
                        bPreviousSessionPosted = true;
                        argStrVersionName = strVersionName;
                    }
                    else
                    {
                        _log.Info("There is some issue in gdbm posting of the previous session :" + strVersionName);
                        argStrVersionName = strVersionName;
                    }
                }
                else
                {
                    _log.Info("The previous session name could not be find from table, session name :" + strVersionName);
                    bPreviousSessionPosted = true;
                }
            }
            catch (Exception exp)
            {
                bPreviousSessionPosted = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
            }
            return bPreviousSessionPosted;
        }

        private static string GetSessionNameToCheck(string argStrConnString)
        {
            string versionName = null;
            string query = null;
            try
            {
                query = ReadConfigurations.GetValue(ReadConfigurations.queryToGetVersionName);
                DataRow pRow = DBHelper.GetSingleDataRowByQuery(argStrConnString, query);
                versionName = Convert.ToString(pRow[ReadConfigurations.col_VERSION_NAME]);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
            }
            return versionName;
        }
    }
}
