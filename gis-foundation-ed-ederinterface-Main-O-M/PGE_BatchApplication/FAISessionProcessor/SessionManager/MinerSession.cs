using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.Interop.Process;
using Miner.Process.GeodatabaseManager.Subtasks;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.FAISessionProcessor.SessionManager
{
    /// <summary>
    /// Class to wrap all sorts of MM nastiness including database calls
    /// </summary>
    public class MinerSession
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private IMMPxApplication _pxApplication = null;
        private IMMSession _mmSession = null;
        private string SessionManagerOleDbConnection;
        private IVersion _sessionVersion;

        static private IMMAppInitialize _mmAppInit = null;

        private IWorkspace DefaultVersionWorkspace = null;
        private IWorkspace _childVersionWorkspace = null;

        private string _sessionVersionName;

        public string SessionName { get; set; }
        public bool ReuseSession { get; set; }
        public bool UnlockSession { get; set; }

        public IVersion Version
        {
            get
            {
                return _sessionVersion;
            }
        }
        public IWorkspace Workspace
        {
            get
            {
                return _childVersionWorkspace;
            }
        }

        public IMMSession MMSession
        {
            get
            {
                return _mmSession;
            }
        }

        private IMMPxApplication PxApplication
        {
            get
            {
                if (_pxApplication == null)
                {
                    ConnectToSessionManager();
                }
                return _pxApplication;
            }
        }

        public MinerSession(IWorkspace DefaultWorkspace, string _sessionManagerOleDbConnection)
        {
            SessionManagerOleDbConnection = _sessionManagerOleDbConnection;
            DefaultVersionWorkspace = DefaultWorkspace;
        }

        private void ConnectToSessionManager()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _pxApplication = new PxApplicationClass();
            IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();

            _pxApplication.StandAlone = true;
            _pxApplication.Visible = false;
            pxLogin.ConnectionString = SessionManagerOleDbConnection;
            //                "Provider=OraOLEDB.Oracle.1;User ID=" + user + ";Data Source=" + dataSource + ";Password=" + password;

            _pxApplication.StandAlone = true;
            _pxApplication.Visible = false;
            _pxApplication.Startup(pxLogin);
        }

        public void SubmitSessionToGDBM()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            //const int DATA_PROCESSING_STATE = 9;//10;
            const int POST_QUEUE_STATE = 5;
            try
            {
                IMMPxSubtask subtask = new SetPostingPriority();
                Miner.Interop.Process.IDictionary parameters = new MMPxNodeListClass();

                object k = "Priority";
                object d = 10;
                parameters.Add(ref k, ref d);

                object k1 = "ShowMessages";
                object d1 = false;
                parameters.Add(ref k1, ref d1);
                ((SetPostingPriority)subtask).Parameters = parameters;

                bool initialized = subtask.Initialize(PxApplication);
                Console.WriteLine("Initialized [ " + initialized + " ]");

                IMMPxNodeEdit nodeedit = new MMPxNodeListClass();
                int nodetype = _pxApplication.Helper.GetNodeTypeID("Session");
                nodeedit.Initialize(nodetype, "Session", _mmSession.get_ID());
                IMMPxNode node = (IMMPxNode)nodeedit;
                ((IMMPxApplicationEx)_pxApplication).HydrateNodeFromDB(node);

                SetNodeState(PxApplication, node, 2); // 2 == Pending QA / QC
                bool executedSuccessfully = subtask.Execute(node);
                _logger.Debug("executedSuccessfully [ " + executedSuccessfully + " ]");
                _logger.Debug("Setting Node State to POST_QUEUE");
                SetNodeState(_pxApplication, node, POST_QUEUE_STATE);
                InsertSessionIntoPostingQueue(_mmSession);

            }
            catch (Exception)
            {
                // Try to Set the node state to an error state?

                throw;
            }

        }

        private void InsertSessionIntoPostingQueue(IMMSession session)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            ITable table = null;
            IRow newRow = null;
            IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)this.Workspace;
            ((IWorkspaceEdit)versionedWorkspace).StartEditing(false);
            table = ((IFeatureWorkspace)versionedWorkspace).OpenTable("SDE.GDBM_POST_QUEUE");

            newRow = table.CreateRow();
            newRow.set_Value(newRow.Fields.FindField("SUBMIT_TIME"), DateTime.Now);
            newRow.set_Value(newRow.Fields.FindField("CURRENTUSER"), session.get_Owner().ToUpper());
            newRow.set_Value(newRow.Fields.FindField("VERSION_OWNER"), session.get_Owner().ToUpper());
            newRow.set_Value(newRow.Fields.FindField("VERSION_NAME"), "SN_" + session.get_ID());
            newRow.set_Value(newRow.Fields.FindField("DESCRIPTION"), session.get_Description());
            newRow.set_Value(newRow.Fields.FindField("PRIORITY"), 5);
            newRow.set_Value(newRow.Fields.FindField("PX_NODE_ID"), session.get_ID());
            newRow.set_Value(newRow.Fields.FindField("NODE_TYPE_NAME"), "Session");
            newRow.set_Value(newRow.Fields.FindField("NODE_TYPE_ID"), 3);
            _logger.Debug("Creating Row [ SDE.GDBM_POST_QUEUE ]");

            newRow.Store();

            ((IWorkspaceEdit)versionedWorkspace).StopEditing(true);
            Marshal.FinalReleaseComObject(newRow);
            Marshal.FinalReleaseComObject(table);
            Marshal.FinalReleaseComObject(versionedWorkspace);
        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_childVersionWorkspace != null) Marshal.FinalReleaseComObject(_childVersionWorkspace);
            _childVersionWorkspace = null;

            _mmAppInit.Shutdown();
        }

        private void CreateMMSessionVersionInternal()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + SessionName + " ]");

            // Create Session
            IMMSessionManager sessionMgr = (IMMSessionManager)PxApplication.FindPxExtensionByName("MMSessionManager");
            _mmSession = sessionMgr.CreateSession();
            _mmSession.set_Name(SessionName +"_" + DateTime.Now);

            _sessionVersionName = "SN_" + _mmSession.get_ID();
            _logger.Debug("Creating Version " + ((IDatabaseConnectionInfo)DefaultVersionWorkspace).ConnectedUser + "." +
                _sessionVersionName);
            _sessionVersion = ((IVersion)DefaultVersionWorkspace).CreateVersion(_sessionVersionName);
            _sessionVersion.Access = esriVersionAccess.esriVersionAccessPublic;
        }

        public void CreateMMSessionVersion(string name)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + name + " ]");

            SessionName = name;
            if (ReuseSession)
            {
                TryGetExistingSession();
            }
            if (_mmSession == null)
            {
                CreateMMSessionVersionInternal();
            }

            //IVersion childVersion = ((IVersionedWorkspace)DefaultVersionWorkspace).FindVersion(_sessionVersionName);
            _childVersionWorkspace = _sessionVersion as IWorkspace;
            //_childVersionWorkspace = OpenWorkspaceInDifferentVersion(DefaultVersionWorkspace, _sessionVersionName);
        }

        private int GetExistingSessionId()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            int sessionId = -1;
            // ADO to retrieve the Session ID
            string user = ((IDatabaseConnectionInfo)DefaultVersionWorkspace).ConnectedUser;

            string sql = "SELECT max(SESSION_ID) from PROCESS.MM_SESSION where hidden=0 and create_user ='" + user + "' and substr(session_name,0," +
                         SessionName.Length + ") = '" + SessionName + "'";
            object recordsaffected = null;
            ADODB.Recordset recordset = PxApplication.Connection.Execute(sql, out recordsaffected);

            if (!recordset.EOF)
            {
                object sessionObj = recordset.GetRows();
                if (sessionObj != DBNull.Value)
                {
                    System.Array array = sessionObj as System.Array;
                    if (array.GetValue(0, 0) != DBNull.Value)
                    {
                        sessionId = Convert.ToInt32(array.GetValue(0, 0));
                    }
                }
            }
            return sessionId;
        }

        private void TryGetExistingSession()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                int sessionId = GetExistingSessionId();
                _logger.Debug("Found existing SessionId [ " + sessionId + " ]");
                // Did we find a Session? then let's interrogate it for status info
                if (sessionId > -1)
                {
                    IMMPxNodeEdit nodeedit = new MMPxNodeListClass();
                    int nodetype = _pxApplication.Helper.GetNodeTypeID("Session");
                    nodeedit.Initialize(nodetype, "Session", sessionId);
                    IMMPxNode node = (IMMPxNode)nodeedit;
                    ((IMMPxApplicationEx)_pxApplication).HydrateNodeFromDB(node);

                    // If it's in QA/QC let's transition it back
                    if (node.State.StateID == 4)
                    {
                        _logger.Debug("Transitioning back");
                        SetNodeState(PxApplication, node, 1);
                    }

                    if (node.State.StateID == 1) // In Progress
                    {
                        bool readOnly = false;
                        IMMSessionManager sessionMgr = (IMMSessionManager)PxApplication.FindPxExtensionByName("MMSessionManager");
                        _mmSession = sessionMgr.GetSession(ref sessionId, ref readOnly);
                        if (readOnly || SessionIsLocked())
                        {
                            _mmSession = null;
                        }
                        else
                        {
                            _sessionVersionName = "SN_" + _mmSession.get_ID();
                            _logger.Debug("FindVersion [ " + _sessionVersionName + " ]");
                            _sessionVersion = ((IVersionedWorkspace)DefaultVersionWorkspace).FindVersion(_sessionVersionName);
                        }
                    }

                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());

                _mmSession = null;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pxApp"></param>
        /// <param name="pxNode"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetNodeState(IMMPxApplication pxApp, IMMPxNode pxNode, int state)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            bool nodeState = false;

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
            return nodeState;
        }

        public bool SessionIsLocked()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            object recordsaffected = null;
            bool isLocked = false;

            string sql = "select locked from PROCESS.MM_PX_CURRENT_STATE WHERE soid=" + _mmSession.get_ID();
            ADODB.Recordset recordset = PxApplication.Connection.Execute(sql, out recordsaffected);

            if (!recordset.EOF)
            {
                object sessionObj = recordset.GetRows();
                if (sessionObj != DBNull.Value)
                {
                    System.Array array = sessionObj as System.Array;
                    if (array.GetValue(0, 0) != DBNull.Value)
                    {
                        int locked = Convert.ToInt32(array.GetValue(0, 0));
                        isLocked = locked != 0;
                    }
                }
            }

            return isLocked;
        }

    }
}
