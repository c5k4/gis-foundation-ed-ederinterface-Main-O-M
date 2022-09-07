using System;
using System.Collections.Generic;
using System.Reflection;
using Miner.Interop.Process;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
//using mmSystem;
using System.Configuration;
using PGE_DBPasswordManagement;
using System.Text.RegularExpressions;
using Miner.Process.GeodatabaseManager.Subtasks;


namespace ETGIS_AssetSync_Process
{
    public class Createsession
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ETGISAssetSync.log4net.config");
        private static AoInitializeClass m_AoInit = null;
        public static IMMAppInitialize _mmAppInit;

        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        private static IMMAppInitialize arcFMAppInitialize = new MMAppInitialize();
        public static string _versionName = string.Empty;
        public static string _oldversionName = string.Empty;
        private static IWorkspace _workspaceCon;
        public static IVersion editVersion;
        private static IMMSession _mmSession;
        private static IMMPxApplication _pxApplication;
        private static string _sessionName = ConfigurationManager.AppSettings["VERSION_NAME"];
        private static IMMPxLogin pxLogin;
        private static IWorkspace wSpace_ed = null;
        public static bool SessionCreate()
        {
            _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            bool success = true;
            try
            {
                _logger.Info("Getting ESRI/ARCFM Licenses");
                InitializeESRILicense();
                InitializeArcFMLicense();
                wSpace_ed = GetSDEWorkSpace(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["ED_SDEConnection"]).ToUpper());
                if (wSpace_ed != null)
                {
                    _logger.Info("Sde Connection Opened successfully ");
                    _pxApplication = new PxApplicationClass();
                    CreateSession();
                    CreateVersion();
                    //SubmitSessionToGDBM();
                }
                else
                {
                    throw new Exception("Sde Connection failed");
                    //_logger.Info("Sde Connection failed ");
                    //success = false;
                    //return success;
                }

            }            
            catch (Exception ex)
            {
                _logger.Error("Exception encountered while running main Createsession Function." + ex.ToString());
                throw ex;
                //success = false;
            }
            finally
            {
                //if (_pMMxApplication != null) Marshal.ReleaseComObject(_pMMxApplication);
                //if (_pxLogin != null) Marshal.ReleaseComObject(_pxLogin);
                //if (_Workspace != null) Marshal.ReleaseComObject(_Workspace);
            }
            return success;
        }
        public static IWorkspace GetSDEWorkSpace(string sdeConnection)
        {
            _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            IWorkspace featureWorkspace = null;
            try
            {
                if (string.IsNullOrEmpty(sdeConnection))
                {
                    throw new Exception("Sdeconnection file is null.");
                }
                //_logger.Info("Try to connect to SDE: " + sdeConnection);
                Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                IWorkspaceFactory wsf = Activator.CreateInstance(t) as IWorkspaceFactory;
                featureWorkspace = wsf.OpenFromFile(sdeConnection, 0);
                _logger.Info("Successfully connected to database");                
            }            
            catch (Exception Ex)
            {
                _logger.Error("Unable to checkout ARCFM license" + Ex.ToString());
                throw Ex;
                //return null;
            }
            return featureWorkspace;
        }
        public static bool CreateVersion()
        {
            _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                if (_versionName == string.Empty)
                {
                    throw new Exception("Version name not found.");
                }
                if (wSpace_ed == null)
                {
                    throw new Exception("Workspace not found.");
                }
                IFeatureWorkspace fWSpace = (IFeatureWorkspace)wSpace_ed;
                IVersionedWorkspace vWSpace = (IVersionedWorkspace)fWSpace;

                try
                {
                    editVersion = vWSpace.FindVersion(_oldversionName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not find the version " + _versionName);
                }
                if (editVersion != null)
                {
                    editVersion.Delete();
                    editVersion = ((IVersionedWorkspace)fWSpace).DefaultVersion.CreateVersion(_versionName);
                    editVersion.Access = esriVersionAccess.esriVersionAccessPublic;
                }
                else
                {
                    editVersion = ((IVersionedWorkspace)fWSpace).DefaultVersion.CreateVersion(_versionName);
                    editVersion.Access = esriVersionAccess.esriVersionAccessPublic;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to create version specified: " + _versionName + ex.ToString());
                throw ex;
                //return false;
            }
            return true;
        }
        public static bool CreateSession()
        {
            _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                bool connection = CreatePxConnection();
                _oldversionName = ConfigurationManager.AppSettings["VersionNamePrefix"] + GetExistingSessionId().ToString();
                if (connection)
                {
                    Console.WriteLine("Px Application connected sucessfully");
                    IMMSessionManager sessionMgr = (IMMSessionManager)_pxApplication.FindPxExtensionByName("MMSessionManager");

                    //Create session                
                    _mmSession = sessionMgr.CreateSession();
                    int sID = _mmSession.get_ID();
                    _sessionName = _sessionName + "_" + DateTime.Now.ToShortDateString();
                    _sessionName = Regex.Replace(_sessionName, "/", "");
                    _versionName = ConfigurationManager.AppSettings["VersionNamePrefix"] + sID;
                    _mmSession.set_Name(_sessionName);

                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to create session specified:" + ex.ToString());
                throw ex;
                //return false;
            }
            return true;
        }        
        private static bool CreatePxConnection()
        {
            _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            bool createConnection = true;
            try
            {
                pxLogin = (IMMPxLogin)new PxLoginClass();

                _pxApplication.StandAlone = true;
                _pxApplication.Visible = false;
                string[] strconnection = ConfigurationManager.AppSettings["ED_SDEConnection"].Split('@');
                string smUser = strconnection[0];
                string smDB = strconnection[1];

                string smPwd = ReadEncryption.GetPassword(smUser + "@" + smDB);
                pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;
                _pxApplication.Startup(pxLogin);
            }
            catch (Exception ex)
            {
                _logger.Error("Unhandled exception encountered while making PX application connection" + ex.ToString());
                throw ex;
                //createConnection = false;
                //return createConnection;
            }
            return createConnection;
        }
        public static bool SubmitSessionToGDBM()
        {
            bool InsertpostQueue = false;
            _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                //Reconcile the Version If conflict occurrs then resolve the conflict in favour of Target Version                
                IVersionEdit4 versionEdit = (IVersionEdit4)editVersion;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)versionEdit;
                if (workspaceEdit.IsBeingEdited() == false)
                {
                    workspaceEdit.StartEditing(true);
                    workspaceEdit.StartEditOperation();
                }
                // Reconcile
                Boolean conflictsDetected = versionEdit.Reconcile4(editVersion.VersionInfo.Parent.VersionName, false, false, false, false);
                workspaceEdit.StopEditing(true);
                workspaceEdit.StopEditOperation();

                //no conflicts detected so post can be performed-        
                if (conflictsDetected != true)
                {
                    IMMPxSubtask subtask = new SetPostingPriority();
                    Miner.Interop.Process.IDictionary parameters = new MMPxNodeListClass();

                    object k = "Priority";
                    object d = 20;
                    parameters.Add(ref k, ref d);

                    object k1 = "ShowMessages";
                    object d1 = false;
                    parameters.Add(ref k1, ref d1);
                    ((SetPostingPriority)subtask).Parameters = parameters;

                    bool initialized = subtask.Initialize(_pxApplication);
                    Console.WriteLine("Initialized [ " + initialized + " ]");
                    IMMPxNodeEdit nodeedit = new MMPxNodeListClass();
                    //Console.WriteLine("Initialized Nodeedit.");
                    int nodetype = _pxApplication.Helper.GetNodeTypeID("Session");
                    //Console.WriteLine(nodetype);
                    nodeedit.Initialize(nodetype, "Session", _mmSession.get_ID());
                    //Console.WriteLine("Initialized Nodeedit");
                    IMMPxNode node = (IMMPxNode)nodeedit;
                    //Console.WriteLine("Node edit start");
                    ((IMMPxApplicationEx)_pxApplication).HydrateNodeFromDB(node);
                    //Console.WriteLine("HydrateNodeFromDB");
                    SetNodeState(_pxApplication, node, 9);
                    //Console.WriteLine("SetNodeState finished");
                    if (InsertSessionIntoPostingQueue() == true)
                    {
                        InsertpostQueue = true;
                        Console.WriteLine("InsertSessionIntoPostingQueue finished");
                    }
                    else
                    {
                        Console.WriteLine("false return from InsertSessionIntoPostingQueue");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error in SubmitSessionToGDBM." + ex.ToString());
                throw ex;
                //InsertpostQueue = false;
            }
            return InsertpostQueue;
        }
        public static bool SetNodeState(IMMPxApplication pxApp, IMMPxNode pxNode, int state)
        {
            _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
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
                _logger.Error("Unable to update note state." + ex.ToString());
                throw ex;
            }
            return nodeState;
        }
        public static bool InsertSessionIntoPostingQueue()
        {
            _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
            bool InsertSuccess = false;
            try
            {
                ITable table = null;
                IRow newRow = null;
                IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)editVersion;
                table = ((IFeatureWorkspace)versionedWorkspace).OpenTable("SDE.GDBM_POST_QUEUE");

                newRow = table.CreateRow();
                newRow.set_Value(newRow.Fields.FindField("SUBMIT_TIME"), DateTime.Now);
                newRow.set_Value(newRow.Fields.FindField("CURRENTUSER"), _mmSession.get_Owner().ToUpper());
                newRow.set_Value(newRow.Fields.FindField("VERSION_OWNER"), _mmSession.get_Owner().ToUpper());
                newRow.set_Value(newRow.Fields.FindField("VERSION_NAME"), "SN_" + _mmSession.get_ID());
                newRow.set_Value(newRow.Fields.FindField("DESCRIPTION"), _mmSession.get_Description());
                newRow.set_Value(newRow.Fields.FindField("PRIORITY"), 20);
                newRow.set_Value(newRow.Fields.FindField("PX_NODE_ID"), _mmSession.get_ID());
                newRow.set_Value(newRow.Fields.FindField("NODE_TYPE_NAME"), "Session");
                newRow.set_Value(newRow.Fields.FindField("NODE_TYPE_ID"), 3);

                newRow.Store();
                InsertSuccess = true;

                //Release Com object
                Marshal.FinalReleaseComObject(newRow);
                Marshal.FinalReleaseComObject(table);
                Marshal.FinalReleaseComObject(versionedWorkspace);
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to submit session in post queue." + ex.ToString());
                throw ex;
                //InsertSuccess = false;
            }
            return InsertSuccess;
        }
        public static void InitializeESRILicense()
        {
            try
            {
                _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
                //Cache product codes by enum int so can be sorted without custom sorter
                List<int> m_requestedProducts = new List<int>();
                foreach (esriLicenseProductCode code in new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced })
                {
                    int requestCodeNum = Convert.ToInt32(code);
                    if (!m_requestedProducts.Contains(requestCodeNum))
                    {
                        m_requestedProducts.Add(requestCodeNum);
                    }
                }
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);
                m_AoInit = new AoInitializeClass();
                esriLicenseProductCode currentProduct = new esriLicenseProductCode();
                foreach (int prodNumber in m_requestedProducts)
                {
                    esriLicenseProductCode prod = (esriLicenseProductCode)Enum.ToObject(typeof(esriLicenseProductCode), prodNumber);
                    esriLicenseStatus status = m_AoInit.IsProductCodeAvailable(prod);
                    if (status == esriLicenseStatus.esriLicenseAvailable)
                    {
                        status = m_AoInit.Initialize(prod);
                        if (status == esriLicenseStatus.esriLicenseAlreadyInitialized ||
                            status == esriLicenseStatus.esriLicenseCheckedOut)
                        {
                            currentProduct = m_AoInit.InitializedProduct();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.Error("Unable to checkout ESRI license" + Ex.ToString());
                throw Ex;
            }
        }
        public static void InitializeArcFMLicense()
        {
            try
            {
                _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
                //Comm.LogManager.WriteLine("Checking out ArcFM license...");
                _mmAppInit = new MMAppInitialize();
                // check and see if the type of license is available to check out
                mmLicenseStatus mmLS = _mmAppInit.IsProductCodeAvailable(mmLicensedProductCode.mmLPArcFM);
                if (mmLS == mmLicenseStatus.mmLicenseAvailable)
                {
                    // if the license is available, try to check it out.
                    mmLicenseStatus arcFMLicenseStatus = _mmAppInit.Initialize(mmLicensedProductCode.mmLPArcFM);
                    if (arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                    {
                        // if the license cannot be checked out, an exception is raised
                        throw new Exception("The ArcFM license requested could not be checked out");
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.Error("Unable to checkout ARCFM license" + Ex.ToString());
                throw Ex;

            }
        }
        public static int GetExistingSessionId()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            int sessionId = -1;
            try
            {
                string sapdailyinterfaceversionname = ConfigurationManager.AppSettings["VERSION_NAME"];
                string sql = "SELECT max(SESSION_ID) from PROCESS.MM_SESSION where hidden=0 and " +
                            " upper(Session_Name) like upper('%" + sapdailyinterfaceversionname + "%')";
                object recordsaffected = null;
                ADODB.Recordset recordset = _pxApplication.Connection.Execute(sql, out recordsaffected);
                string deletesql = "DELETE FROM process.MM_SESSION WHERE " +
                            " upper(Session_Name) like upper('%" + sapdailyinterfaceversionname + "%')";
                ADODB.Recordset recordset1 = _pxApplication.Connection.Execute(deletesql, out recordsaffected);
                if (!recordset.EOF)
                {
                    object sessionObj = recordset.GetRows();
                    if (sessionObj != DBNull.Value)
                    {
                        System.Array array = sessionObj as System.Array;
                        if (array.GetValue(0, 0) != DBNull.Value)
                        {
                            sessionId = Convert.ToInt32(array.GetValue(0, 0));
                            _logger.Info("Session found with SESSION_ID " + sessionId);
                        }
                    }
                }
            }
            catch (Exception EX)
            {
                _logger.Error("Exception encountered while getting the sessionid" + EX.ToString());
                throw EX;
            }
            return sessionId;
        }    

    }
}
