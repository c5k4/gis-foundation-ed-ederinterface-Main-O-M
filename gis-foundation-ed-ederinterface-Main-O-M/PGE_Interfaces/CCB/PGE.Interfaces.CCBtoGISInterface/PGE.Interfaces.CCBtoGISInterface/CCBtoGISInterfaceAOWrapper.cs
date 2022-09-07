using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;

using System.Runtime.InteropServices;

using System.Configuration;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.Interop.Process;
using PGE.Common.ChangesManagerShared;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using Miner.Process.GeodatabaseManager.Subtasks;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Threading;
using ClosedXML.Excel;
using PGE_DBPasswordManagement;



namespace PGE.Interfaces.CCBtoGISInterface
{
    class CcBtoGISInterfaceAo 
    {

        #region DATA FIELDS
            private static IMMPxLogin pxLogin;
            private static IAoInitialize pESRILicense;
            private static IMMAppInitialize pArcFMLicense;
            private static IVersion CCBVersion;
            private static int _passFail = 0;
            private static IWorkspace _wSpace;
            static readonly Log4NetLogger _logger = new Log4NetLogger("PGE.Interfaces.CCBtoGISInterface", "PGE.Interfaces.CCBtoGISInterface.log4net.config");
            private static string _versionName;
            private static IMMSession _mmSession;
            private static string strSessionTable = "PROCESS.MM_SESSION";
            public static IMMPxApplication _pxApplication;
        // M4JF EGISREARCH 391 - Initialized parameters for Inbound /Outbound interface and Interface execution summary
            private static bool inBound = false;
            private static bool outBound = false;
            public static string intExecutionSummary = ConfigurationManager.AppSettings["IntExecutionSummaryExePath"];
           
            public static string comment = default;
            private static string startTime;
            private static StringBuilder remark = default;
            private static StringBuilder Argumnet = default;
            private static string interfaceType = default;
            public static int Recordcount = 0 ;
            public static int Successrecordcount = 0;
            private static string sdeConnFile = default;


        private static PGE.Common.ChangesManagerShared.MinerSession GISUtilities;
       
        #endregion

        
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                startTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                //(m4jf) EDGISREARCH-391 - GIS-CCB Integration Improvements
                //parameters for inbound and outbound interfaces
                if (args.Length > 0)
                {

                    string[] splitarg = args[0].Split(',');
                    if (splitarg.Length > 0)
                    {
                        //inbound parameter "INBOUNDPROCESS"
                        if (splitarg[0].ToString() == "INPROCESS")
                        {
                            interfaceType = Argument.Type1;
                            inBound = true;

                        }

                        //outbound parameter "OUTBOUNDPROCESS"
                        else if (splitarg[0].ToString() == "OUTPROCESS")
                        {
                            interfaceType = Argument.Type2;
                            outBound = true;

                        }
                        else
                        {
                           // MailNotificationforDataErrors();
                            _passFail = 1;
                            _logger.Info("No input parameter(INPROCESS/OUTPROCESS) defined .CCBTOGIS stopped");
                            Console.WriteLine("No input parameter(INPROCESS/OUTPROCESS) defined .CCBTOGIS stopped");
                        }
                    }
                }
                _pxApplication = new PxApplicationClass();

                // m4jf edgisrearch 919 
                //string sdeConnFile = ConfigurationManager.AppSettings["SDEConnFile"];
              
                 sdeConnFile = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"]);

                PGE.Common.ChangesManagerShared.SDEWorkspaceConnection sdeConn =
                    new PGE.Common.ChangesManagerShared.SDEWorkspaceConnection(sdeConnFile);
                _logger.Info("Initialized SDE Connection file of " + sdeConnFile);
                string adoConnFile = ConfigurationManager.AppSettings["ADOConnection"];
                PGE.Common.ChangesManagerShared.AdoOleDbConnection adoConn =
                    new PGE.Common.ChangesManagerShared.AdoOleDbConnection(adoConnFile);
                _logger.Info("Initialized ADO Connection file of " + adoConnFile);
                GISUtilities = new MinerSession(sdeConn, adoConn);

                _logger.Info("Getting Licenses");
                GetLicenses(ref pESRILicense, ref pArcFMLicense);
                _logger.Info("Successfully checked out licenses");

                // (m4jf) EDGISREARCH-391 - GIS-CCB Integration Improvements
                if (inBound == true)
                {
                    _logger.Info("About to create Session");                    
                }

                CreateArcFMConnection();

                if (_passFail == 0)
                {
                    if (!OpenWorkspace())
                    {
                        _passFail = 1;
                        
                    }
                }

                //(m4jf) EDGISREARCH-391 - GIS-CCB Integration Improvements
                // For Inboud interface
                if (_passFail == 0 && inBound == true)
                {
                    string sSessionName = string.Empty;
                    if (CheckVersionExistFromSessionName(_wSpace, out sSessionName) == true)
                    {

                        SendEmail(sSessionName);
                        _passFail = 1;
                        _logger.Warn("CCBToGIS Session Already Exist... Stopping PGE.Interfaces.CCBtoGISInterface. ");
                        comment = "CCBToGIS Session Already Exist... Stopping PGE.Interfaces.CCBtoGISInterface. ";
                    }
                }

                //(m4jf) EDGISREARCH-391 - GIS-CCB Integration Improvements
                if (_passFail == 0 && outBound == true)
                {
                    if (!RunSQL("CheckStatusSQL", false))
                                      
                    {
                        _passFail = 1;
                        _logger.Warn("Inbound or Outbound table is under process... Stopping PGE.Interfaces.CCBtoGISInterface. ");
                        comment = "Inbound or Outbound table is under process... Stopping PGE.Interfaces.CCBtoGISInterface. ";

                    }
                }

                //(m4jf) EDGISREARCH-391 - GIS-CCB Integration Improvements
                // For Inboud interface
                if (_passFail == 0 && inBound == true)
                {
                    if (!CreateSession())
                    {
                        _passFail = 1;
                       
                    }
                }

                if (inBound == true)
                {
                    _logger.Info("About to create Version" + _versionName);
                }

                //(m4jf) EDGISREARCH-391 - GIS-CCB Integration Improvements
                // For Inboud interface
                if (_passFail == 0 && inBound == true)
                {
                    if (!CreateVersion())
                    {
                        _passFail = 1;
                       
                    }
                }

              

                //(m4jf) EDGISREARCH-391 - GIS-CCB Integration Improvements
                // inbound job procedures
                if (_passFail == 0 && inBound == true)
                {

                    if (!RunSQL("SPActionSQL", false))
                    {
                        _passFail = 1;
                       

                    }
                    else
                    {
                        // (m4jf) EDGISREARCH-391 - GIS-CCB Integration Improvements
                        _logger.Info("Sending email notification for SPID errors");
                        //send email notification for SPID errors .
                        MailNotificationforDataErrors();

                    }
                }
                if (_passFail == 0 && inBound == true)
                {
                    if (!RunSQL("GISSQL", true))
                    {
                        _passFail = 1;
                       
                    }
                }

                //(m4jf) EDGISREARCH-391 - GIS-CCB Integration Improvements
                //outbound procedures
                if (_passFail == 0 && outBound == true)
                {
                    if (RunSQL("POSTGISSQL", false))
                    {

                        // update comment for Interface Execution Summary table
                        // m4jf edgisrearch 391 CCBtoGIS Interface improvements
                        // Get no. of records proccessed .
                        string sql = "Select * from " + ConfigurationManager.AppSettings["OutboundStagingTable"];
                        int outBoundRecordcount = 0;
                        object ErrorRecords = null;
                        DataTable dt = new DataTable();
                        ADODB.Recordset recordset = _pxApplication.Connection.Execute(sql, out ErrorRecords);
                        OleDbDataAdapter adapter = new OleDbDataAdapter();
                        // fill datatable
                        adapter.Fill(dt, recordset);
                        outBoundRecordcount = dt.Rows.Count;
                        comment = "GIS to CC&B outbound executed successfull . Records proccessed : " + outBoundRecordcount;
                        ExecutionSummary(startTime, comment, Argument.Done);
                    }
                    else
                    {
                        _passFail = 1;
                       
                    }
                }

                if (_passFail == 0 && inBound == true)
                {
                    //Reconcile the Version If conflict occurrs then resolve the conflict in favour of Target Version                
                    IVersionEdit4 versionEdit = (IVersionEdit4)CCBVersion ;
                    IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)versionEdit;
                    if (workspaceEdit.IsBeingEdited() == false)
                    {
                        workspaceEdit.StartEditing(true);
                        workspaceEdit.StartEditOperation();
                    }
                    // Reconcile
                    Boolean conflictsDetected = versionEdit.Reconcile4(CCBVersion.VersionInfo.Parent.VersionName, false, false, false, false);
                    workspaceEdit.StopEditing(true);
                    workspaceEdit.StopEditOperation();
                    //no conflicts detected so post can be performed-        
                    if (conflictsDetected != true)
                    {
                        if (SubmitSessionToGDBM(_mmSession, _wSpace) == false)
                        {
                            _passFail = 1;
                           
                        }
                    }
                    else
                    {
                        SendMailForConflictError();
                    }
                    //if (!submitSessionForQC(_pxApplication))
                    //{
                    //    _passFail = 1;
                    //}

                    // update comment for Interface Execution Summary table
                    // m4jf edgisrearch 391 CCBtoGIS Interface improvements
                   
                    // Update Interface Execution Summary table
                    comment = "CCB to GIS executed successfully .Records proccessed successfully : " + Successrecordcount + "Error Records :" + Recordcount;
                    ExecutionSummary(startTime, comment, Argument.Done);


                }
                if (_passFail == 1)
                {
                    ExecutionSummary(startTime, comment, Argument.Error);
                }
                _logger.Info("Program Complete , hit the X key to close");

            }
            catch (Exception EX)
            {
                _logger.Error("Unhandled exception encountered while running",EX);
                //m4jf edgisrearc 391 - updated comment for interface execution summary
                comment = "Unhandled exception encountered while running" + EX;
               
                _passFail = 1;
                ExecutionSummary(startTime, comment, Argument.Error);
            }
            
            finally
            {
                ReleaseLicenses(pESRILicense, pArcFMLicense);
                Marshal.FinalReleaseComObject(_wSpace);
                Marshal.FinalReleaseComObject(_pxApplication);
                if (_mmSession != null)
                    Marshal.FinalReleaseComObject(_mmSession);
                ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;
                foreach (ProcessThread childThread in currentThreads)
                {
                    childThread.Dispose(); // If thread is waiting, stop waiting

                }
                
                Environment.Exit(_passFail);
               
            }

            return _passFail;
        }

        private static void CreateArcFMConnection()
        {
            try
            {
                 pxLogin = (IMMPxLogin)new PxLoginClass();

                _pxApplication.StandAlone = true;
                _pxApplication.Visible = false;
                // m4jf edgisrearc 919 - Create Connection string using PGE_DBPasswordManagement

                //string smUser = ConfigurationManager.AppSettings["SMUser"];
                //string smPwd = ConfigurationManager.AppSettings["SMUPWD"];
                //string smDB = ConfigurationManager.AppSettings["SMDB"];
                string[] UserInst = ConfigurationManager.AppSettings["EDER_ConnectionStr"].Split('@');
                string smUser = UserInst[0].ToUpper();
               
                string smDB = UserInst[1].ToUpper();
                string smPwd = ReadEncryption.GetPassword(smUser + "@" + smDB);

               string versionNamePrefix = ConfigurationManager.AppSettings["VersionNamePrefix"];



                 pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;
                

                 _pxApplication.Startup(pxLogin);
            }
            catch(Exception ex)
            {
                // m4jf - EDGISREARCH - 391
                _logger.Info(ex.Message);
            }
        }
     

        internal static bool CreateSession()
        {
            bool returnValue = false;
            try
            {
                //IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();

                //_pxApplication.StandAlone = true;
                //_pxApplication.Visible = false;

                //string smUser = ConfigurationManager.AppSettings["SMUser"];
                //string smPwd = ConfigurationManager.AppSettings["SMUPWD"];
                //string smDB = ConfigurationManager.AppSettings["SMDB"];

                //pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;


                //_pxApplication.Startup(pxLogin);
                string versionNamePrefix = ConfigurationManager.AppSettings["VersionNamePrefix"];

                string sessionName = ConfigurationManager.AppSettings["SessionName"];

                IMMSessionManager sessionMgr = (IMMSessionManager)_pxApplication.FindPxExtensionByName("MMSessionManager");
                _mmSession = sessionMgr.CreateSession();

                int sID = _mmSession.get_ID();

                sessionName = sessionName + "_" + DateTime.Now.ToShortDateString();
                sessionName = Regex.Replace(sessionName, "/", "");
                _versionName = versionNamePrefix + sID;

                _mmSession.set_Name(sessionName);
                returnValue = true;
                
            }
            catch(Exception ex)
            {
                // m4jf edgisrearch 391
                _logger.Error(ex.Message);
                comment = ex.Message ;

            }
            return returnValue;
        }
       
        internal static bool CreateVersion()
        {
            //string sdeConnFile = ConfigurationManager.AppSettings["SDEConnFile"];
            //SdeWorkspaceFactory sdeWSpaceFactory = new SdeWorkspaceFactoryClass();
            //_wSpace = sdeWSpaceFactory.OpenFromFile(sdeConnFile, 0);
             
             IFeatureWorkspace fWSpace = (IFeatureWorkspace) _wSpace;
             IVersionedWorkspace vWSpace = (IVersionedWorkspace) fWSpace;

          

            try
            {
                CCBVersion = vWSpace.FindVersion(_versionName);
                CCBVersion.Delete();
            }
            catch (Exception EX)
            {
                _logger.Info("Version did not exist to delete");
            }
            try
            {
                CCBVersion = ((IVersionedWorkspace) fWSpace).DefaultVersion.CreateVersion(_versionName);
                CCBVersion.Access = esriVersionAccess.esriVersionAccessPublic;
            }
            catch (Exception EX)
            {
                _logger.Info("Unable to create version specified: "+_versionName);
                comment = "Unable to create version specified: " + _versionName;
                return false;
            }
            return true;
        }


        internal static bool OpenWorkspace()
        {
            // m4jf edgisrearch 919 

            //string sdeConnFile = ConfigurationManager.AppSettings["SDEConnFile"];
           // string sdeConnFile = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"]);
            SdeWorkspaceFactory sdeWSpaceFactory = new SdeWorkspaceFactoryClass();
            _wSpace = sdeWSpaceFactory.OpenFromFile(sdeConnFile, 0);
            return true;
        }


        internal static bool RunSQL(string appSetting, bool useVersion)
        {
            try
            {

                // m4jf edgisrearch 919
                //String versionOwner = ConfigurationManager.AppSettings["SMUser"];
                String versionOwner = ConfigurationManager.AppSettings["SessionOwner"];

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
                    _logger.Info("Executing the plsql of " + plSQLBuilder + " started at : " + DateTime.Now.ToString());
                    _wSpace.ExecuteSQL(plSQLBuilder);
                    _logger.Info("Successfully Executed SQL at : " + DateTime.Now.ToString());

                }
               
                return true;
            }
            catch (Exception EX)
            {
                _logger.Error("Unhandled Exception in RunSQL function.", EX);
                comment = "Unhandled Exception in RunSQL function."+ EX.Message;
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

        //    if (UnlockSession)
        //    {
        //        sql = "update PROCESS.MM_PX_CURRENT_STATE SET LOCKED = 0 WHERE soid=" + _mmSession.get_ID();
        //        _pxApplication.Connection.Execute(sql, out recordsaffected, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);
        //    }
        //}

            ((IWorkspaceEdit)_wSpace).StopEditing(true);

            
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
        #region New Methods  : To resolve the Incident	INC000004340064

        private static bool InsertSessionIntoPostingQueue(IMMSession session, IWorkspace workspace)
        {

            _logger.Debug(MethodBase.GetCurrentMethod().Name);
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
                _logger.Debug("Creating Row [ SDE.GDBM_POST_QUEUE ]");

                newRow.Store();

                // ((IWorkspaceEdit)versionedWorkspace).StopEditing(true);
                InsertSuccess = true;
                Marshal.FinalReleaseComObject(newRow);
                Marshal.FinalReleaseComObject(table);
                Marshal.FinalReleaseComObject(versionedWorkspace);
            }

            catch (Exception ex)
            {
                _logger.Error("Error occurred while session sending to posting queue: ", ex);
                InsertSuccess = false;
            }
            return InsertSuccess;
        }

        private static void SendEmail(string sSessionName)
        {
            try
            {


                string lanID = string.Empty;
                string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
                string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];
                string subject = ConfigurationManager.AppSettings["MAIL_SUBJECT"] + " Session: " + sSessionName;
                string bodyText = ConfigurationManager.AppSettings["MAIL_BODY"];
                string lanIDs = ConfigurationManager.AppSettings["LANIDS"];
                string[] LanIDsList = lanIDs.Split(';');

                for (int i = 0; i < LanIDsList.Length; i++)
                {
                    lanID = LanIDsList[i];
                    EmailService.Send(mail =>
                    {
                        mail.From = fromAddress;
                        mail.FromDisplayName = fromDisplayName;
                        mail.To = lanID + ";";
                        mail.Subject = subject;
                        mail.BodyText = bodyText;

                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error: ", ex);
            }
        }

        private static bool CheckVersionExistFromSessionName(IWorkspace _wSpace, out string sSessionName)
        {
            sSessionName = string.Empty;
            bool SessionExist = false;

            IFeatureWorkspace fWSpace = (IFeatureWorkspace)_wSpace;
            IVersionedWorkspace vWSpace = (IVersionedWorkspace)fWSpace;
            try
            {
                int iSessionID = GetExistingSessionId(out sSessionName);
                if (iSessionID != -1)
                {
                    string sessionName = ConfigurationManager.AppSettings["VersionNamePrefix"] + iSessionID.ToString();
                    IEnumVersionInfo vinfo = (IEnumVersionInfo)vWSpace.Versions;
                    IVersionInfo pversioninfo = vinfo.Next();
                    while (pversioninfo != null)
                    {
                        string vname = pversioninfo.VersionName.ToString();

                        if (vname.Contains(sessionName))
                        {
                            SessionExist = true;
                            _logger.Info("CCBToGIS Version Exist");
                            break;
                        }
                        pversioninfo = vinfo.Next();
                    }
                }

            }
            catch (Exception EX)
            {

                _logger.Error("Unhandled exception encountered while checking the existing version", EX);

            }
            return SessionExist;

        }

        private static int GetExistingSessionId(out string sSessionName)
        {
            sSessionName=string.Empty;
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            int sessionId = -1;
            try
            {
                string sessionName = ConfigurationManager.AppSettings["SessionName"];
                string sql = "select  session_id, session_name from process.mm_session where session_id = (SELECT max(SESSION_ID) from " + strSessionTable + " where hidden=0 and " +
                            " upper(Session_Name) like upper('%" + sessionName + "%'))";
                object recordsaffected = null;
                ADODB.Recordset recordset = _pxApplication.Connection.Execute(sql, out recordsaffected);

                if (!recordset.EOF)
                {
                    object sessionObj = recordset.GetRows();
                    if (sessionObj != DBNull.Value)
                    {
                        System.Array array = sessionObj as System.Array;
                        if (array.GetValue(0, 0) != DBNull.Value)
                        {
                            sessionId = Convert.ToInt32(array.GetValue(0, 0));
                            sSessionName = Convert.ToString(array.GetValue(1, 0));
                            _logger.Info("SESSION_ID is " + sessionId );
                        }
                    }
                }
            }
            
            catch (Exception EX)
            {
                 
                _logger.Error("Unhandled exception encountered while getting the sessionid", EX);
               
            }
            
            return sessionId;
        }

        public static bool SubmitSessionToGDBM(IMMSession session, IWorkspace workspace)
        {
            int _SessionState = GetSessionState();
            bool InsertpostQueue = false;
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            try
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
                int nodetype = _pxApplication.Helper.GetNodeTypeID("Session");
                nodeedit.Initialize(nodetype, "Session", _mmSession.get_ID());
                IMMPxNode node = (IMMPxNode)nodeedit;
                ((IMMPxApplicationEx)_pxApplication).HydrateNodeFromDB(node);              
                _logger.Debug("Setting Node State to Posting Queue");
                SetNodeState(_pxApplication, node, _SessionState);
                if (InsertSessionIntoPostingQueue(session, workspace) == true)
                {
                    InsertpostQueue = true;
                }
                
              
            }
            catch (Exception EX)
            {
                InsertpostQueue = false;
                _logger.Error("Unhandled exception encountered while Session send to GDBM Post Queue", EX);
                // m4jf edgisrearch 391 
                comment = "Unhandled exception encountered while Session send to GDBM Post Queue"+ EX.Message;
            }

            return InsertpostQueue;
        }

        private static int GetSessionState()
        {
            //set state =9 so that if code fail then session goes to default data processing state
            int _state = 9;
            try
            {
               _state= Convert.ToInt32(ConfigurationManager.AppSettings["SESSION_STATE"].ToString());
            }
            catch
            { }
            return _state;
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

        private static void SendMailForConflictError()
        {
            ICursor pSearchCursor = null;
            try
            {

                string strPostErrorLookupTableName = ConfigurationManager.AppSettings["LOOKUPTABLENAME"];
               IFeatureWorkspace featureWorkspace = (IFeatureWorkspace) _wSpace ;

                ITable AHLookupTable = null;
                QueryFilter queryFilter = null;
                IRow codeRow = null;
                String strToList = String.Empty;
                String strFromList = String.Empty;
                String strSubject = String.Empty;
                String strMessage = String.Empty;

                AHLookupTable = featureWorkspace.OpenTable(strPostErrorLookupTableName);

                if (AHLookupTable == null) { return; }

                string SessionName = FindSessionNameFromVersionName(featureWorkspace, _versionName);
                if (string.IsNullOrEmpty(SessionName))
                { 
                    return;
                }
                string[] splitSessionName = SessionName.Split('_');
                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("UPPER(VERSIONNAME) like '%" + splitSessionName[0].ToUpper() + "%'");

                pSearchCursor = AHLookupTable.Search(queryFilter, false);
                codeRow = pSearchCursor.NextRow();

                if (codeRow == null)
                {

                    return;
                }
                strFromList = (codeRow.get_Value(codeRow.Fields.FindField("FROMLIST"))).ToString();

                strToList = (codeRow.get_Value(codeRow.Fields.FindField("TOLIST"))).ToString();

                strMessage = ConfigurationManager.AppSettings["MAIL_BODY_FOR_CONFLICT"];

                strSubject = ConfigurationManager.AppSettings["MAIL_SUBJECT_FOR_CONFLICT"];

               // strHost = (codeRow.get_Value(codeRow.Fields.FindField("HOST"))).ToString();

                string[] LanIDsList = strToList.Split(';');
                //send email
                
                for (int i = 0; i < LanIDsList.Length; i++)
                {
                    string lanID = LanIDsList[i];
                    EmailService.Send(mail =>
                    {
                        mail.From = strFromList;
                        mail.FromDisplayName = strFromList;
                        mail.To = lanID + "@pge.com;";
                        mail.Subject = strSubject + " Session Name: " + _mmSession.get_Name();
                        mail.BodyText = strMessage;

                    });
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (pSearchCursor != null)
                {
                    Marshal.ReleaseComObject(pSearchCursor);
                }
            }
        }

        private static string FindSessionNameFromVersionName(IFeatureWorkspace featureWorkspace, string strVersionName)
        {
            string sessionname = string.Empty;
            try
            {

                string SessionID = GetSessionID(strVersionName);

                ITable PSessionTable = featureWorkspace.OpenTable(strSessionTable);

                IQueryFilter pqf = new QueryFilterClass();
                pqf.WhereClause = "SESSION_ID = " + SessionID;
                ICursor pcursor = PSessionTable.Search(pqf, false);
                IRow prow = pcursor.NextRow();
                if (prow != null)
                {
                    sessionname = prow.get_Value(prow.Fields.FindField("SESSION_NAME")).ToString();
                }
                Marshal.ReleaseComObject(pcursor);

            }
            catch { }


            return sessionname;
        }
        public static string GetSessionID(string strVersionName)
        {
            string strSessionID = String.Empty;
            try
            {
                string[] words = strVersionName.Split('_');
                if (words.Length > 1)
                {
                    strSessionID = words[1];
                }
                else
                {
                    strSessionID = words[0];
                }

            }
            catch
            { }

            return strSessionID;

        }




        /// <summary>
        /// //(M4JF) EDGISREARC 391 - GIS-CCB Integration Improvements
        /// This function Sends mail notification for SPID errors .
        /// </summary>
        public static void MailNotificationforDataErrors()

        {
            // Initialize variables .
                                
            string sql = default;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataTable Successdt = new DataTable();

            //Initialize email parameters
            string strMessage = default;
            string strSubject = default;            
            String fromLanID = ConfigurationManager.AppSettings["FROM_LANID"];
            String toLanID = ConfigurationManager.AppSettings["TO_LANID"];
            String fromDisplayName = ConfigurationManager.AppSettings["FROM_DISPLAYNAME"];
           // int Recordcount = 0; 


            try
            {
                DateTime currentDateTime = DateTime.Now;
                string dateStr = currentDateTime.ToString("MM-dd-yyyy");
                // Get servicepoint id's with errors . 
                sql = "SELECT  OBJECTID , error_Description FROM PGEDATA.PGE_CCBTOEDGIS_STG WHERE error_description IS NOT NULL";             
                object ErrorRecords = null;
                ADODB.Recordset recordset = _pxApplication.Connection.Execute(sql, out ErrorRecords);
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                // fill datatable
                adapter.Fill(dt, recordset);
                ds.Tables.Add(dt);
                Recordcount = dt.Rows.Count;

                sql = "SELECT  OBJECTID  FROM PGEDATA.PGE_CCBTOEDGIS_STG WHERE error_description IS  NULL";
                object Records = null;
                ADODB.Recordset recordset1 = _pxApplication.Connection.Execute(sql, out Records);
                OleDbDataAdapter adapter1 = new OleDbDataAdapter();
                // fill datatable
                adapter1.Fill(Successdt, recordset1);
              
                Successrecordcount = Successdt.Rows.Count;

                // if error records exists
                if (Recordcount > 0)
                {
                    try
                    {
                        // export error records to an .xlsv file
                        string ErrorReportpath = GetPath("xlsx");
                        ExportExcel(dt, ErrorReportpath);
                        _logger.Info("Exported excel file at " + ErrorReportpath);

                        strMessage = ConfigurationManager.AppSettings["MAIL_BODY_FOR_SPIDERROR"];
                        strSubject = ConfigurationManager.AppSettings["MAIL_SUBJECT_FOR_SPIDERROR"];

                        //send mail notification along with error report exported.
                        EmailService.Send(mail =>
                        {
                            mail.From = fromLanID ;
                            mail.FromDisplayName = fromDisplayName;
                            mail.To = toLanID ;
                            mail.Subject = strSubject + "(" + dateStr + ")";
                            mail.BodyText = strMessage;
                            mail.Attachments.Add(ErrorReportpath);

                        });
                    }
                    catch (Exception ex)
                    {

                     _logger.Error(ex.Message);
                     throw ex;

                    }


                }
                else
                {
                    //send email notification for no SPID errors
                    try
                    {
                        strMessage = ConfigurationManager.AppSettings["MAIL_BODY_FOR_NOSPIDERROR"];
                        strSubject = ConfigurationManager.AppSettings["MAIL_SUBJECT_FOR_NOSPIDERROR"];

                        EmailService.Send(mail =>
                        {
                            mail.From = fromLanID ;
                            mail.FromDisplayName = fromDisplayName;
                            mail.To = toLanID ;
                            mail.Subject = strSubject + "(" + dateStr + ")";
                            mail.BodyText = strMessage;


                        });
                    }

                    catch (Exception ex)
                    {

                        _logger.Error(ex.Message);
                    }


                }

            }

            catch (Exception ex)
            {

                _logger.Error(ex.Message);
            }

        }


        /// <summary>
        /// (M4JF) EDGISREARC 391 - GIS-CCB Integration Improvements
        /// Exports datatable to an excel file .
        /// </summary>
        /// <param name="dt">dataset to export datatable to excel</param>
        /// <param name="strReportpath">path to save excel to local system</param>

        public static void ExportExcel(DataTable dt, string sPath)
        {

            try
            {
                _logger.Info("Exporting excel file");
                // string sPath = GetPath("xlsx");
                XLWorkbook xlw = new XLWorkbook();
                IXLWorksheet xls = xlw.AddWorksheet("CCBtoGISErrorReport");
                IXLCell xlc = xls.Cell(1, 1);
                xls.Range("A1:B1").Merge();
                xlc.Value = "Interruption date:";
                xlc.RichText.Underline = XLFontUnderlineValues.Single;
                xlc.RichText.Bold = true;


                //Create a Table;;
              
                DataTable strTable = new DataTable("ErrorTable");
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    string dCol = null;
                    dCol = dt.Columns[j].ToString();
                    strTable.Columns.Add(dCol);

                }

                string strCoumnvalue = "";
               // strCoumnvalue += "";
                //making insert query
                int i = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow pRow = strTable.NewRow();

                    for (int k = 0; k <= dt.Columns.Count - 1; k++)
                    {

                        strCoumnvalue = dr[k].ToString();
                        pRow[k] = strCoumnvalue;
                       

                    }
                strTable.Rows.Add(pRow);

                }
                xlc.InsertTable(strTable);

                xlw.SaveAs(sPath);
                strTable.Clear();

            }
            catch(Exception ex)
            {
                _logger.Info(ex.Message);
            }




        }



       
        /// <summary>
        /// (M4JF) EDGISREARC 391 - GIS-CCB Integration Improvements
        /// Provide temp path of system.
        /// </summary>
        /// <param name="sModule"></param>
        /// <returns></returns>
        public static string GetPath(string sModule)
        {
            string sPath = default;
            DateTime currentDateTime = DateTime.Now;
            string dateStr = currentDateTime.ToString("MMddyyyy");
            string directoryPath = default;
            try
            {
                directoryPath = ConfigurationManager.AppSettings["ERROR_REPORT_EXPORTPATH_DIRECTORY"];
                // get temp path to export excel .
                //sPath = (Directory.Exists(@"P:")) ? @"P:\" : Path.GetTempPath();
                sPath = (Directory.Exists(directoryPath)) ? directoryPath : Path.GetTempPath();
                if (Directory.Exists(sPath))
                {
                    
                    sPath = sPath + "CCBtoGISErrorReport";
                    // Try to create the directory.
                    if (!Directory.Exists(sPath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(sPath);
                    }
                }

                else
                {
                    MessageBox.Show(directoryPath + " Drive does not exist");
                }
                string[] sFiles = Directory.GetFiles(sPath, "CCBToGIS_SPIDErrorReport_*" + sModule);
               
                foreach (string f in sFiles)
                {
                    File.Delete(f);
                }


                sPath += "\\CCBToGIS_SPIDErrorReport_" + dateStr + "." + sModule;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return sPath;
        }

        /// <summary>
        /// This methos is to use for log the interface success or fail in interface summary table for EDGIS Rearch Project-m4jf
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="comment"></param>
        /// <param name="status"></param>
        private static void ExecutionSummary(string startTime, string comment, string status)
        {
            try
            {

                Argumnet = new StringBuilder();
                remark = new StringBuilder();
                //Setting arguments
                Argumnet.Append(Argument.Interface);
                Argumnet.Append(interfaceType);
                Argumnet.Append(Argument.Integration);
                Argumnet.Append(startTime + ";");
                remark.Append(comment);
                Argumnet.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                Argumnet.Append(status);
                Argumnet.Append(remark);

                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(CcBtoGISInterfaceAo.intExecutionSummary, "\"" + Convert.ToString(Argumnet) + "\"");
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;

                Process proc = new Process();
                proc.StartInfo = processStartInfo;
                proc.EnableRaisingEvents = true;
                proc.Start();
                proc.BeginOutputReadLine();
                while (!proc.HasExited)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occures while executing the Interface Summary exe" + ex.Message);
                _logger.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while executing the Interface Summary exe", ex.StackTrace);
                //  fileWriter.WriteLine(" Error Occurred while executing the Interface Summary exe at " + DateTime.Now + " " + ex);
            }
        }


        #endregion New Methods  : To resolve the Incident	INC000004340064
    }


    /// <summary>
    /// This class is for interface Summary Execution
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// This property to get Sucess status of process
        /// </summary>
        public static string Done { get; } = "D;";
        /// <summary>
        /// This property to get failure status of process
        /// </summary>
        public static string Error { get; } = "F;";
        /// <summary>
        /// This property to get interface name 
        /// </summary>
        public static string Interface { get; } = "CC&BTOGIS;";
        /// <summary>
        /// This property to get name of the system to integration
        /// </summary>
        public static string Integration { get; } = "CC&B;";
        /// <summary>
        /// This property to get type of interface
        /// </summary>
        public static string Type1 { get; } = "Inbound;";
        public static string Type2 { get; } = "Outbound;";

    }
}
