using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.Interop.Process;
using Miner.Process.GeodatabaseManager.Subtasks;
using PGE.Common.ChangesManagerShared.Utilities;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.ChangesManagerShared
{
    /// <summary>
    /// Class to wrap all sorts of MM nastiness including database calls
    /// </summary>
    public class MinerSession
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private IMMPxApplication _pxApplication = null;
        private IMMSession _mmSession = null;
        private AdoOleDbConnection _sessionManagerOleDbConnection;
        private SDEWorkspaceConnection _sdeWorkspaceConnection;
        private IVersion _sessionVersion;

        static private IMMAppInitialize _mmAppInit = null;

        private IWorkspace _childVersionWorkspace = null;

        private string _sessionVersionName;

        public string SessionName { get; set; }
        public bool ReuseSession { get; set; }
        public bool UnlockSession { get; set; }
        public bool ReuseOnly { get; set; } //Added by Tanveer-Oct-2017

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

        public MinerSession(SDEWorkspaceConnection sdeWorkspaceConnection, AdoOleDbConnection oleDbConnection)
        {
            _sdeWorkspaceConnection = sdeWorkspaceConnection;
            _sessionManagerOleDbConnection = oleDbConnection;
            ReuseOnly = false;
        }

        private void ConnectToSessionManager()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Probably remove this elsewhere into calling code
            GetArcFMLicense(mmLicensedProductCode.mmLPArcFM);

            _pxApplication = new PxApplicationClass();
            IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();

            _pxApplication.StandAlone = true;
            _pxApplication.Visible = false;
            pxLogin.ConnectionString = _sessionManagerOleDbConnection.ConnectionStringDecrypted;
            //                "Provider=OraOLEDB.Oracle.1;User ID=" + user + ";Data Source=" + dataSource + ";Password=" + password;

            _pxApplication.StandAlone = true;
            _pxApplication.Visible = false;
            _pxApplication.Startup(pxLogin);

        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_childVersionWorkspace != null) Marshal.FinalReleaseComObject(_childVersionWorkspace);
            _childVersionWorkspace = null;

            _mmAppInit.Shutdown();
        }

        static public void GetArcFMLicense()
        {
            GetArcFMLicense(mmLicensedProductCode.mmLPArcFM);
        }

        /// <summary>
        /// Check out an ArcFM License by passing in an enumerated value of 
        /// license options to check out.  Gets an ArcFM license if one exists,
        /// attempts to check it out, and throws an exception if one cannot be checked out.
        /// </summary>
        /// <param name="LicenseToCheckOut">Enumerated value of the license to check out</param>
        static public void GetArcFMLicense(mmLicensedProductCode LicenseToCheckOut)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _mmAppInit = new MMAppInitializeClass();
            // check and see if the type of license is available to check out
            mmLicenseStatus mmLS = _mmAppInit.IsProductCodeAvailable(LicenseToCheckOut);
            if (mmLS == mmLicenseStatus.mmLicenseAvailable)
            {
                // if the license is available, try to check it out.
                mmLicenseStatus arcFMLicenseStatus = _mmAppInit.Initialize(LicenseToCheckOut);

                if (arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut && arcFMLicenseStatus != mmLicenseStatus.mmLicenseAlreadyInitialized)
                {
                    // if the license cannot be checked out, an exception is raised
                    throw new Exception("The ArcFM license requested could not be checked out");
                }
            }
        }

        /// <summary>
        /// Shutdown the ArcFM Application Initializion object
        /// Should be the last call to make on the object.
        /// </summary>
        static public void ReleaseArcFMLicense()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _mmAppInit.Shutdown();
        }

        private void CreateMMSessionVersionInternal()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + SessionName + " ]");

            // Create Session
            IMMSessionManager sessionMgr = (IMMSessionManager)PxApplication.FindPxExtensionByName("MMSessionManager");
            _mmSession = sessionMgr.CreateSession();
            _mmSession.set_Name(SessionName + DateTime.Now);

            _sessionVersionName = "SN_" + _mmSession.get_ID();
            _logger.Debug("Creating Version " + ((IDatabaseConnectionInfo)_sdeWorkspaceConnection.Workspace).ConnectedUser + "." +
                _sessionVersionName);
            _sessionVersion = ((IVersion)_sdeWorkspaceConnection.Workspace).CreateVersion(_sessionVersionName);
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
                if (!ReuseOnly)
                    CreateMMSessionVersionInternal();
            }

            if (_mmSession == null) return;
            _childVersionWorkspace = AOHelper.OpenWorkspaceInDifferentVersion(_sdeWorkspaceConnection.Workspace,
                                    _sessionVersionName);
        }

        private int GetExistingSessionId()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            int sessionId = -1;
            // ADO to retrieve the Session ID
            string user = ((IDatabaseConnectionInfo)_sdeWorkspaceConnection.Workspace).ConnectedUser;

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
                            _logger.Info("Session is Locked or ReadOnly.");
                        }
                        else
                        {
                            _sessionVersionName = "SN_" + _mmSession.get_ID();
                            _logger.Debug("FindVersion [ " + _sessionVersionName + " ]");
                            _sessionVersion = ((IVersionedWorkspace)_sdeWorkspaceConnection.Workspace).FindVersion(_sessionVersionName);
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

        public void TransitionToQaQc()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            const int QA_QC_STATE = 2;

            IMMPxNodeEdit nodeedit = new MMPxNodeListClass();
            int nodetype = _pxApplication.Helper.GetNodeTypeID("Session");
            nodeedit.Initialize(nodetype, "Session", _mmSession.get_ID());
            IMMPxNode node = (IMMPxNode)nodeedit;
            ((IMMPxApplicationEx)_pxApplication).HydrateNodeFromDB(node);

            SetNodeState(PxApplication, node, QA_QC_STATE);

            UpdateMMPxVersion(PxApplication, node.Id);

            ((IWorkspaceEdit)_childVersionWorkspace).StopEditing(true);
        }

        public void SubmitSessionToGDBM()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            const int DATA_PROCESSING_STATE = 9;//10;

            try
            {
                IMMPxSubtask subtask = new SetPostingPriority();
                Miner.Interop.Process.IDictionary parameters = new MMPxNodeListClass();

                object k = "Priority";
                object d = 5;
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
                _logger.Debug("Setting Node State to DATA_PROCESSING");
                SetNodeState(_pxApplication, node, DATA_PROCESSING_STATE);
                InsertSessionIntoPostingQueue(_mmSession);

            }
            catch (Exception)
            {
                // Try to Set the node state to an error state?

                throw;
            }

        }

        public void SubmitDirectly_To_Post_InGDBM(int postingState)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            //const int DATA_PROCESSING_STATE = 9;//10;
            //const int DIRECT_POSTING_STATE = 5;

            //By-passes the QA/QC process in GDBM and goes diretly to posting.
            //Function inserted by: TMIR - Oct-06-2017
            try
            {
                IMMPxSubtask subtask = new SetPostingPriority();
                Miner.Interop.Process.IDictionary parameters = new MMPxNodeListClass();

                object k = "Priority";
                object d = 5;
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
                _logger.Debug("Setting Node State to Direct Posting");
                //SetNodeState(_pxApplication, node, DATA_PROCESSING_STATE);
                SetNodeState(_pxApplication, node, postingState);
                InsertSessionIntoPostingQueue(_mmSession);
                _logger.Info("Successfully set Session State to Direct Posting. State: " + postingState.ToString());

            }
            catch (Exception)
            {
                // Try to Set the node state to an error state?
                throw;
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

        public void UpdateMMPxVersion(IMMPxApplication pxApplication, int nodeId)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            string user = ((IDatabaseConnectionInfo)_sdeWorkspaceConnection.Workspace).ConnectedUser;

            string sql = "UPDATE process.mm_px_versions SET has_edits=-1, status=1, user_name='" + user + "' WHERE node_id=" +
                         nodeId;
            object recordsaffected = null;
            _pxApplication.Connection.Execute(sql, out recordsaffected, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);

            if (UnlockSession)
            {
                sql = "update PROCESS.MM_PX_CURRENT_STATE SET LOCKED = 0 WHERE soid=" + _mmSession.get_ID();
                _pxApplication.Connection.Execute(sql, out recordsaffected, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);
            }
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
