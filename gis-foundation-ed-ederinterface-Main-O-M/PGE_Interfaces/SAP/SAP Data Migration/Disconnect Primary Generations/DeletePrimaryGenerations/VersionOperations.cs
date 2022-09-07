using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using Miner.Geodatabase.Edit;
using Miner.Interop.Process;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using PGE_DBPasswordManagement;

namespace DeletePrimaryGenerations
{
    public class VersionOperations
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IFeatureClass _primaryGen = null;       
        private static IMMSession _mmSession;
        private static IMMPxApplication _pxApplication;
        private static string _versionName;
        private IWorkspace _workspace = null;
        private static int count = 0;            
        private static List<string> listOfSessionsCreated = new List<string>();
        private static DataTable sessionVsPrimGenerationsDeleted = new DataTable();
       
        public static bool InitilizeLicenses()
        {
            bool ballPreRequisiteSuccessful = true;
            try
            {
                Common.InitializeESRILicense();
                //Common.InitializeArcFMLicense();
            }
            catch (Exception exp)
            {
                //throw;
                return false;
            }
            return ballPreRequisiteSuccessful;
        }

        public bool StartProcess()
        {
            IMMAppInitialize _mmAppInit = null;
            bool bOpeartionSuccess = true;          
            count = 0;
            _pxApplication = new PxApplicationClass();
            try
            {
                DateTime start = DateTime.Now;

                _log.Info("Initializing Arc GIS / Arc FM licenses.");
                bOpeartionSuccess = InitilizeLicenses();
                _log.Info("Reading config file.");
                ReadConfigurations.ReadFromConfiguration();

                if (!bOpeartionSuccess)
                    return bOpeartionSuccess;               

                _mmAppInit = new MMAppInitialize();
                // check and see if the type of license is available to check out
                mmLicenseStatus mmLS = _mmAppInit.IsProductCodeAvailable(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                if (mmLS == mmLicenseStatus.mmLicenseAvailable)
                {
                    // if the license is available, try to check it out.
                    mmLicenseStatus arcFMLicenseStatus = _mmAppInit.Initialize(Miner.Interop.mmLicensedProductCode.mmLPArcFM);

                    if (arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                    {
                        // if the license cannot be checked out, an exception is raised
                        throw new Exception("The ArcFM license requested could not be checked out");
                    }
                    else
                    {
                        //auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                        _log.Info("Started reading workspace.");

                        _workspace = GetWorkspace(ReadConfigurations.EDWorkSpaceConnString);
                        IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)_workspace;
                        _log.Info("Workspace read successfully.");

                        IFeatureClass primaryGen = featversionWorkspace.OpenFeatureClass(ReadConfigurations.PrimaryGenerationClassName);
                         
                        IQueryFilter qF = new QueryFilterClass();
                        qF.WhereClause = ConfigurationManager.AppSettings["QueryToGetPrimaryGeneration"];
                        qF.SubFields = "OBJECTID";
                        List<int> listOids = new List<int>();
                        IFeatureCursor featCur = primaryGen.Search(qF, false);
                        using (ComReleaser cr = new ComReleaser())
                        {
                            cr.ManageLifetime(featCur);
                            IFeature feat = null;
                            while ((feat = featCur.NextFeature()) != null)
                            {
                                listOids.Add(feat.OID);
                            }
                        }

                        _log.Info("Total primary generations found : " + listOids.Count);

                        // Refine the list and remove already processed primary generations from the list.
                        RemoveAlreadyProcessedPrimaryGeneration(featversionWorkspace, listOids);

                        _log.Info("Total primary generations considered for disconnection : " + listOids.Count);

                        if (!sessionVsPrimGenerationsDeleted.Columns.Contains("PRIMGENOID"))
                            sessionVsPrimGenerationsDeleted.Columns.Add("PRIMGENOID", typeof(string));

                        if (!sessionVsPrimGenerationsDeleted.Columns.Contains("SESSIONNAME"))
                            sessionVsPrimGenerationsDeleted.Columns.Add("SESSIONNAME", typeof(string));

                        //Delete all primary generations
                        bOpeartionSuccess = MakeEditsInVersion(_workspace, count, listOids);
                        _log.Info("");
                        _log.Info("");
                        _log.Info("Total sessions created : " + listOfSessionsCreated.Count);
                        _log.Info("");

                        foreach (string sessionName in listOfSessionsCreated)
                        {
                            _log.Info(sessionName);
                        }

                        _log.Info("");
                        _log.Info("Updates done per session.");
                        _log.Info("");
                        string PrimGenOID = null;
                        StringBuilder sb = new StringBuilder();

                        foreach (string sessionName in listOfSessionsCreated)
                        {
                            _log.Info("Primary generations disconnected in session :" + sessionName);
                            _log.Info("");
                            DataRow[] rows = sessionVsPrimGenerationsDeleted.Select("SESSIONNAME='" + sessionName + "'");

                            foreach (DataRow pRow in rows)
                            {
                                PrimGenOID = Convert.ToString(pRow.Field<string>("PRIMGENOID"));

                                if (!string.IsNullOrEmpty(PrimGenOID))
                                {
                                    sb.Append(PrimGenOID + ",");
                                }
                            }
                            _log.Info(sb.ToString());
                            sb.Remove(0, sb.Length);
                            _log.Info("");

                        }
                        _log.Info("");
                        _log.Info("Process completed.");
                        _log.Info("Total time taken : " + (DateTime.Now - start));
                        _log.Info("");
                        _log.Info("");
                    }
                }
                else
                {
                    _log.Info("Arc FM license not available.");
                }
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message+" at "+exp.StackTrace);
            }
            finally
            {
                if (_mmAppInit != null)
                    _mmAppInit.Shutdown();

                Common.CloseLicenseObject();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return bOpeartionSuccess;
        }

        private bool MakeEditsInVersion(IWorkspace workspace, int count, List<int> listOids)
        {
            ISimpleJunctionFeature simplejunctionFeature = null;
            List<IFeature> lstOfEdges = new List<IFeature>();
            IFeature edgeFeat = null;
            int OIDPrimGen = 0;
            //VersionVsSession
            int versionCount = 1;
            try
            {
                int localCount = 0;
                int editCount = 0;
                int editsPerSessios = Convert.ToInt32(ConfigurationManager.AppSettings["EditsPerSession"]);

                string slversionname = string.Empty;
                IWorkspaceEdit editSession = null;
                IVersion2 dataversion = null;
                int processedPG = 0;
                Disconnect disConnect = new Disconnect();
                foreach (int oid in listOids)
                {
                    using (ComReleaser cr = new ComReleaser())
                    {
                        if (processedPG == 0)
                        {
                            //VersionVsSession
                            //CreateSession(localCount.ToString());

                            //VersionVsSession
                            _versionName = ReadConfigurations.VersionNamePrefix + versionCount;

                            dataversion = CreateVersion(workspace, _versionName);
                            _log.Info("Creating new version " + _versionName);
                            IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)dataversion;
                            _primaryGen = featversionWorkspace.OpenFeatureClass(ReadConfigurations.PrimaryGenerationClassName);
                        }
                        else if (editCount > 0 && editCount % editsPerSessios == 0)
                        {
                            try
                            {
                                count = count + localCount;
                                if (Editor.EditState == EditState.StateEditing)
                                {
                                    Editor.StopOperation("");
                                    Editor.StopEditing(true);
                                }
                            }
                            catch (Exception ex1)
                            {
                            }
                            //VersionVsSession
                            //CreateSession(localCount.ToString());

                            //VersionVsSession
                            _versionName = ReadConfigurations.VersionNamePrefix + versionCount;

                            _log.Info("Creating new version " + _versionName);
                            dataversion = CreateVersion(workspace, _versionName);

                            IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)dataversion;
                            _primaryGen = featversionWorkspace.OpenFeatureClass(ReadConfigurations.PrimaryGenerationClassName);

                            localCount = 0;
                        }

                        if (dataversion != null)
                        {
                            editSession = (IWorkspaceEdit)dataversion;
                            if (Editor.EditState == EditState.StateNotEditing)
                            {
                                Editor.StartEditing((IWorkspace)editSession);
                                Editor.StartOperation();
                            }
                            IFeature feat_primgen = _primaryGen.GetFeature(oid);
                            if (feat_primgen != null)
                            {
                                try
                                {
                                    //_log.Info("Finding Connected Edges for Primary generation OID: " + feat_primgen.OID);
                                    simplejunctionFeature = (ISimpleJunctionFeature)feat_primgen;
                                    lstOfEdges.Clear();

                                    for (int i = 0; i < simplejunctionFeature.EdgeFeatureCount; i++)
                                    {
                                        edgeFeat = simplejunctionFeature.get_EdgeFeature(i) as IFeature;
                                        if (!edgeFeat.Shape.IsEmpty && ((IPolyline)edgeFeat.Shape).Length > 0)
                                        {
                                            lstOfEdges.Add(simplejunctionFeature.get_EdgeFeature(i) as IFeature);
                                        }
                                    }

                                    //Disconnect Primary generation first
                                    INetworkFeature netWorkFeat = feat_primgen as INetworkFeature;
                                    if (netWorkFeat != null && lstOfEdges.Count > 0)
                                    {
                                        disConnect.DisconnectFeature(feat_primgen);
                                        _log.Info("Disconnected primary generation : " + feat_primgen.OID + " from geometric network");

                                        sessionVsPrimGenerationsDeleted.Rows.Add(feat_primgen.OID, _versionName);
                                        editCount++;
                                    }
                                    else
                                    {
                                        _log.Info("Primary generation : " + feat_primgen.OID + " is already disconencted from geometric network");
                                    }

                                    // Not deleting primary generation as per discussion with Buisness
                                    // Deleting primary generation feature
                                    //_log.Info("Deleting primary generation : " + feat_primgen.OID + "");                                   
                                    //feat_primgen.Delete();
                                    //_log.Info("Primary generation successfully deleted.");                                   
                                }
                                catch (Exception ex)
                                {
                                    _log.Error("Error : " + ex.Message);
                                }
                            }
                        }
                    }
                    processedPG++;
                }
                if (Editor.EditState == EditState.StateEditing)
                {
                    Editor.StopOperation("");
                    Editor.StopEditing(true);
                }
                _log.Info("Stopped editing data in " + slversionname);
                //_log.Info("Total " + (count + localCount).ToString() + " primary generations are deleted");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Error : " + ex.Message);
                return false;
            }
            finally
            {                
            }
        }
                  
        public static IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {

                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
            }
            catch (Exception exp)
            {
                // throw;
            }
            return workspace;
        }

        internal bool CreateSession(string localCount)
        {

            IMMPxLogin pxLogin = (IMMPxLogin)new PxLoginClass();

            _pxApplication.StandAlone = true;
            _pxApplication.Visible = false;

            // m4jf edgisrearch 919 
            //string smUser = ConfigurationManager.AppSettings["SMUser"];
            //string smPwd = ConfigurationManager.AppSettings["SMUPWD"];
            //string smDB = ConfigurationManager.AppSettings["SMDB"];
            string[] userInst = ConfigurationManager.AppSettings["EDER_SDEConnection"].Split('@');
            string smUser = userInst[0].ToUpper() ;
           
            string smDB = userInst[1].ToUpper();
            string smPwd = ReadEncryption.GetPassword(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper()) ;
            string versionNamePrefix = ConfigurationManager.AppSettings["VersionNamePrefix"];

            string sessionName = ConfigurationManager.AppSettings["SessionName"];

            pxLogin.ConnectionString = "Provider=OraOLEDB.Oracle.1;User ID= " + smUser + ";Data Source= " + smDB + ";Password= " + smPwd;
            //pxLogin.PromptConnection(

            _pxApplication.Startup(pxLogin);

            IMMSessionManager sessionMgr = (IMMSessionManager)_pxApplication.FindPxExtensionByName("MMSessionManager");
            _mmSession = sessionMgr.CreateSession();

            int sID = _mmSession.get_ID();

            sessionName = sessionName + +sID;  //session name will be SN_<NodeId>
            sessionName = Regex.Replace(sessionName, "/", "");
            _versionName = versionNamePrefix + sID; //session name will be SN_<NodeId>

            _mmSession.set_Name(sessionName);
            _log.Info("----------------------------------------------------------------------");
            _log.Info("Created Session: " + sessionName + ", Session ID: " + sID.ToString());
            listOfSessionsCreated.Add(sessionName);
            _log.Info("----------------------------------------------------------------------");

            return true;
        }

        public static IVersion2 CreateVersion(IWorkspace argWorkspace, string slversionname)
        {
            IVersion2 slversion = null;
            try
            {
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)argWorkspace;
                {
                    IVersion sourceVersion = (IVersion2)vWorkspace.FindVersion(ReadConfigurations.DefaultVersionName);
                    try
                    {
                        _log.Info("Checking whether " + slversionname + " already exist or not");

                        slversion = (IVersion2)vWorkspace.FindVersion(slversionname);
                    }
                    catch (Exception exp)
                    {
                        // throw;
                    }

                    if (slversion != null)
                    {
                        try
                        {
                            _log.Info(slversionname + " already, deleting it.");

                            slversion.Delete();
                        }
                        catch (Exception exp)
                        {
                            //   throw;
                        }
                    }

                    _log.Info("Creating  " + slversionname + " version");
                    sourceVersion.CreateVersion(slversionname);

                    slversion = (IVersion2)vWorkspace.FindVersion(slversionname);
                    slversion.Access = esriVersionAccess.esriVersionAccessPublic;
                }
            }
            catch (Exception exp)
            {
                // throw;
            }
            return slversion;
        }

        public static IVersion2 GetDefaultVersion(IWorkspace argWorkspace)
        {
            IVersion2 defaultVersion = null;
            try
            {
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)argWorkspace;
                {
                    defaultVersion = (IVersion2)vWorkspace.FindVersion(ReadConfigurations.TargetVersionName);
                    //defaultVersion.Access = esriVersionAccess.esriVersionAccessPublic;
                }
            }
            catch (Exception exp)
            {
                // throw;
            }
            return defaultVersion;
        }

        public static bool RemoveAlreadyProcessedPrimaryGeneration(IFeatureWorkspace featversionWorkspace, List<int> argListOids)
        {
            bool processSuccess = true;
            IQueryFilter pQF = null;
            ICursor pCursor = null;
            int PrimGenOID = 0;
            try
            {
                _log.Info("");
                _log.Info("");
                _log.Info("Finding Primary generations that have been replaced earlier.");

                ITable pTable = featversionWorkspace.OpenTable(ReadConfigurations.ChangesLoggedTableName);
                pQF = new QueryFilterClass();
                pCursor = pTable.Search(pQF, false);

                IRow pRow = pCursor.NextRow();

                int primarygenAlreadyReplaced = pTable.RowCount(pQF);

                _log.Info("Count Primary generations that have been replaced earlier : " + primarygenAlreadyReplaced);

                while (pRow != null)
                {
                    try
                    {
                        if (int.TryParse(Convert.ToString(pRow.get_Value(pRow.Fields.FindField("OBJECTID_PRIMGEN"))), out PrimGenOID))
                        {
                            //_log.Info("Primary generaion with OID " + PrimGenOID + " has already been replaced.");

                            if (argListOids.Contains(PrimGenOID))
                            {
                                argListOids.Remove(PrimGenOID);
                                _log.Info("Primary generaion with OID " + PrimGenOID + " removed from the list of all primary generations.");
                            }
                            else
                            {
                                //_log.Info("Primary generaion with OID " + PrimGenOID + " not in list of all primary generations.");
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        _log.Error(exp.Message + " at " + exp.StackTrace);
                    }
                    finally
                    {
                        pRow = pCursor.NextRow();
                    }
                }

                _log.Info("");
                _log.Info("");
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);
                processSuccess = false;
            }
            finally
            {
                if (pCursor != null && Marshal.IsComObject(pCursor)) { while (Marshal.ReleaseComObject(pCursor) > 0) { } }
                if (pQF != null && Marshal.IsComObject(pQF)) { while (Marshal.ReleaseComObject(pQF) > 0) { } }
            }
            return processSuccess;
        }


    }
}
