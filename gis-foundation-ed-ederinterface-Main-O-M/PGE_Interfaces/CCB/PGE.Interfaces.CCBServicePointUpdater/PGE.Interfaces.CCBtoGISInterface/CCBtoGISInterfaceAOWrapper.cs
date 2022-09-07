using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualBasic.Compatibility;

using System.Runtime.InteropServices;

using System.Configuration;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.Interop.Process;
using PGE.Common.ChangesManagerShared;




namespace PGE.Interfaces.CCBtoGISInterface
{
    class CcBtoGISInterfaceAo 
    {

        #region DATA FIELDS

            private static IAoInitialize pESRILicense;
            private static IMMAppInitialize pArcFMLicense;
            private static int _passFail = 0;
            private static IWorkspace _wSpace;

            private static string _versionName;
            private static IMMSession _mmSession;

            private static IMMPxApplication _pxApplication;

            private static PGE.Common.ChangesManagerShared.MinerSession GISUtilities;
       
        #endregion

        
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                _pxApplication = new PxApplicationClass();

                string sdeConnFile = ConfigurationManager.AppSettings["SDEConnFile"];
                PGE.Common.ChangesManagerShared.SDEWorkspaceConnection sdeConn =
                    new PGE.Common.ChangesManagerShared.SDEWorkspaceConnection(sdeConnFile);
                Console.WriteLine("Initialized SDE Connection file of " + sdeConnFile);
                string adoConnFile = ConfigurationManager.AppSettings["ADOConnection"];
                PGE.Common.ChangesManagerShared.AdoOleDbConnection adoConn =
                    new PGE.Common.ChangesManagerShared.AdoOleDbConnection(adoConnFile);
                Console.WriteLine("Initialized ADO Connection file of " + adoConnFile);
                GISUtilities = new MinerSession(sdeConn, adoConn);

                Console.WriteLine("Getting Licenses");
                GetLicenses(ref pESRILicense, ref pArcFMLicense);
                Console.WriteLine("Successfully checked out licenses");
                Console.WriteLine("About to create Session");
                if (_passFail == 0)
                {
                    if (!CreateSession())
                    {
                        _passFail = 1;
                    }
                }
                Console.WriteLine("About to create Version" + _versionName);
                if (_passFail == 0)
                {
                    if (!CreateVersion())
                    {
                        _passFail = 1;
                    }
                }
                if (_passFail == 0)
                {
                    if (!RunSQL("SPActionSQL", false))
                    {
                        _passFail = 1;
                    }
                }
                if (_passFail == 0)
                {
                    if (!RunSQL("GISSQL", true))
                    {
                        _passFail = 1;
                    }
                }
                if (_passFail == 0)
                {
                    if (!RunSQL("POSTGISSQL", false))
                    {
                        _passFail = 1;
                    }
                }
                if (_passFail == 0)
                {
                    if (!submitSessionForPost(_pxApplication))
                    {
                        _passFail = 1;
                    }

                    //if (!submitSessionForQC(_pxApplication))
                    //{
                    //    _passFail = 1;
                    //}
                }
                Console.WriteLine("Program Complete , hit the X key to close");

            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message);
                _passFail = 1;
            }
            finally
            {
                ReleaseLicenses(pESRILicense, pArcFMLicense);
                if (_wSpace != null) { Marshal.FinalReleaseComObject(_wSpace); }
                if (_pxApplication != null) { Marshal.FinalReleaseComObject(_pxApplication); }
                if (_mmSession != null) { Marshal.FinalReleaseComObject(_mmSession); }
                ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads; 
                foreach (ProcessThread childThread in currentThreads)
                {
                    childThread.Dispose(); // If thread is waiting, stop waiting

                }
                Environment.Exit(_passFail);
               
            }

            return _passFail;
        }


        internal static bool CreateSession()
        {
            
            IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();

            _pxApplication.StandAlone = true;
            _pxApplication.Visible = false;

            string smUser = ConfigurationManager.AppSettings["SMUser"];
            string smPwd = ConfigurationManager.AppSettings["SMUPWD"];
            string smDB = ConfigurationManager.AppSettings["SMDB"];
            string versionNamePrefix = ConfigurationManager.AppSettings["VersionNamePrefix"];

            string sessionName = ConfigurationManager.AppSettings["SessionName"];

            pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;

            
            _pxApplication.Startup(pxLogin);

            IMMSessionManager sessionMgr = (IMMSessionManager)_pxApplication.FindPxExtensionByName("MMSessionManager");
            _mmSession = sessionMgr.CreateSession();

            int sID = _mmSession.get_ID();

            sessionName = sessionName + "_" + DateTime.Now.ToShortDateString();
            sessionName = Regex.Replace(sessionName, "/", "");
            _versionName = versionNamePrefix + sID;

            _mmSession.set_Name(sessionName);

            return true;
        }
       
        internal static bool CreateVersion()
        {
            string sdeConnFile = ConfigurationManager.AppSettings["SDEConnFile"];
            SdeWorkspaceFactory sdeWSpaceFactory = new SdeWorkspaceFactoryClass();
            _wSpace = sdeWSpaceFactory.OpenFromFile(sdeConnFile, 0);
             
             IFeatureWorkspace fWSpace = (IFeatureWorkspace) _wSpace;
             IVersionedWorkspace vWSpace = (IVersionedWorkspace) fWSpace;

            IVersion pVersion = null;

            try
            {
                pVersion = vWSpace.FindVersion(_versionName);
                pVersion.Delete();
            }
            catch (Exception EX)
            {
                Console.WriteLine("Version did not exist to delete");
            }
            try
            {
                pVersion = ((IVersionedWorkspace) fWSpace).DefaultVersion.CreateVersion(_versionName);
                pVersion.Access = esriVersionAccess.esriVersionAccessPublic;
            }
            catch (Exception EX)
            {
                Console.WriteLine("Unable to create version specified: "+_versionName);
                return false;
            }
            return true;
        }


      


        internal static bool RunSQL(string appSetting, bool useVersion)
        {
            try
            {
                String versionOwner = ConfigurationManager.AppSettings["SMUser"];

                string sqlFile = ConfigurationManager.AppSettings[appSetting];
                List<string> cmdList = sqlFile.Split(',').ToList();
                foreach (string SQLcmd in cmdList)
                {
                    string tempString=SQLcmd;
                    if (useVersion)
                    {
                        tempString = tempString + "('" + versionOwner +"." + _versionName + "')";
                    }
                    string plSQLBuilder = "DECLARE BEGIN " + tempString + "; END;";
                    Console.WriteLine("Executing the plsql of "+plSQLBuilder);
                    _wSpace.ExecuteSQL(plSQLBuilder);
                    Console.WriteLine("Successfully Executed SQL");
                }
               
                return true;
            }
            catch (Exception EX)
            {
                return false;
            }
           

        }


        internal static bool submitSessionForQC(IMMPxApplication pxApplication)
        {
            const int QA_QC_STATE = 2;

            IMMPxNodeEdit nodeedit = new MMPxNodeListClass();
            int nodetype = pxApplication.Helper.GetNodeTypeID("Session");

            nodeedit.Initialize(nodetype, "Session", _mmSession.get_ID());
            IMMPxNode node = (IMMPxNode)nodeedit;

            ((IMMPxApplicationEx)pxApplication).HydrateNodeFromDB(node);

            GISUtilities.SetNodeState(_pxApplication, node, QA_QC_STATE);

            //GISUtilities.UpdateMMPxVersion(_pxApplication, node.Id);

            string user = ((IDatabaseConnectionInfo)_wSpace).ConnectedUser;

            string sql = "UPDATE process.mm_px_versions SET has_edits=-1, status=1, user_name='" + user + "' WHERE node_id=" +
                         node.Id;
            object recordsaffected = null;
           pxApplication.Connection.Execute(sql, out recordsaffected, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);

            ((IWorkspaceEdit)_wSpace).StopEditing(true);

            
            return true;
        }

        internal static bool submitSessionForPost(IMMPxApplication pxApplication)
        {
           

            IMMPxNodeEdit nodeedit = new MMPxNodeListClass();
            int nodetype = pxApplication.Helper.GetNodeTypeID("Session");

            nodeedit.Initialize(nodetype, "Session", _mmSession.get_ID());
            IMMPxNode node = (IMMPxNode)nodeedit;
            
            ((IMMPxApplicationEx)pxApplication).HydrateNodeFromDB(node);


            string user = ((IDatabaseConnectionInfo)_wSpace).ConnectedUser;
            ((IWorkspaceEdit)_wSpace).StopEditing(true);

            string sql = "UPDATE process.mm_px_versions SET has_edits=-1, status=1, user_name='" + user + "' WHERE node_id=" +
                         node.Id;
            object recordsaffected = null;
            pxApplication.Connection.Execute(sql, out recordsaffected, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);

            sql = "UPDATE process.mm_px_current_state set state_id=(select state_id from process.mm_px_state where name = 'Post Queue') where soid='" + node.Id + "'";
            pxApplication.Connection.Execute(sql, out recordsaffected, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);

            sql = "UPDATE process.mm_px_current_state set state_id=(select state_id from process.mm_px_state where name = 'Post Queue') where soid='" + node.Id + "'";
            pxApplication.Connection.Execute(sql, out recordsaffected, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);

            string currentUser  = _mmSession.get_Owner();
            string versionOwner = user;
            string versionName  = _versionName;
            string description  = _mmSession.get_Description();
            int priority        = 5;
            int nodeTypeID      = node.NodeType;
            string nodeTypeName = "Session";

            ((IWorkspaceEdit2)_wSpace).StartEditing(false);
            ((IWorkspaceEdit2)_wSpace).StartEditOperation();

            IFeatureWorkspace fPostQueue = (IFeatureWorkspace)_wSpace;
            ITable tPostQueue = fPostQueue.OpenTable("SDE.GDBM_POST_QUEUE");
            ICursor insertRows = tPostQueue.Insert(false);
            IRow row = (IRow)tPostQueue;
            int iCURRENT_USER = insertRows.FindField("CURRENTUSER");
            int iVERSION_OWNER = insertRows.FindField("VERSION_OWNER");
            int iVERSION_NAME = insertRows.FindField("VERSION_NAME");
            int iDESCRIPTION = insertRows.FindField("DESCRIPTION");
            int iPRIORITY = insertRows.FindField("PRIORITY");
            int iPX_NODE_ID = insertRows.FindField("PX_NODE_ID");
            int iNODE_TYPE_NAME = insertRows.FindField("NODE_TYPE_NAME");
            int iNODE_TYPE_ID = insertRows.FindField("NODE_TYPE_ID");
            int iSUBMIT_TIME = insertRows.FindField("SUBMIT_TIME");

            row.set_Value(iCURRENT_USER, currentUser);
            row.set_Value(iVERSION_OWNER, versionOwner);
            row.set_Value(iVERSION_NAME, versionName);
            row.set_Value(iDESCRIPTION, description);
            row.set_Value(iPRIORITY, priority);
            row.set_Value(iPX_NODE_ID, node.Id);
            row.set_Value(iNODE_TYPE_NAME, nodeTypeName);
            row.set_Value(iNODE_TYPE_ID, nodeTypeID);
            row.set_Value(iSUBMIT_TIME, DateTime.Now);
            
            insertRows.InsertRow(row);
            ((IWorkspaceEdit2)_wSpace).StartEditing(false);
            ((IWorkspaceEdit2)_wSpace).StartEditOperation();
            Marshal.ReleaseComObject(row);
            Marshal.ReleaseComObject(insertRows);
            Marshal.ReleaseComObject(tPostQueue);
            return true;
        }


        internal static bool GetLicenses(ref IAoInitialize pESRILicense, ref IMMAppInitialize pArcFMLicense)
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);

            pESRILicense = new AoInitialize();
            esriLicenseStatus pESRILicenseStatus = pESRILicense.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeAdvanced);

            if (pESRILicenseStatus == esriLicenseStatus.esriLicenseAvailable) { pESRILicenseStatus = pESRILicense.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced); }
            else { MessageBox.Show("Unable to Obtain ArcGIS License...", " PG&E", MessageBoxButtons.OK); return false; }

            pArcFMLicense = new MMAppInitialize();
            mmLicenseStatus pArcFMLicenseStatus = pArcFMLicense.IsProductCodeAvailable(mmLicensedProductCode.mmLPArcFM);

            if (pArcFMLicenseStatus == mmLicenseStatus.mmLicenseAlreadyInitialized) { return true; }

            if (pArcFMLicenseStatus == mmLicenseStatus.mmLicenseAvailable) { pArcFMLicenseStatus = pArcFMLicense.Initialize(mmLicensedProductCode.mmLPArcFM); }
            else { MessageBox.Show("Unable to Obtain ArcFM License...", " PG&Ek", MessageBoxButtons.OK); return false; }

            return true;
        }

        internal static void ReleaseLicenses(IAoInitialize pESRILicense, IMMAppInitialize pArcFMLicense)
        {
            if (pArcFMLicense != null)
            {
                pArcFMLicense.Shutdown();
                Marshal.ReleaseComObject(pArcFMLicense);
            }

            if (pESRILicense != null)
            {
                pESRILicense.Shutdown();
                Marshal.ReleaseComObject(pESRILicense);
            }
        }
    }
}
