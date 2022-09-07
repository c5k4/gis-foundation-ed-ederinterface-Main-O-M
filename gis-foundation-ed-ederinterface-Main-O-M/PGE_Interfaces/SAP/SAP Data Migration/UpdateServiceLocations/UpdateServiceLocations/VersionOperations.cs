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
using Oracle.DataAccess.Client;
using System.Data.OleDb;
using PGE_DBPasswordManagement;

namespace UpdateServiceLocations
{
    public class VersionOperations
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static List<int> listOfOidsProcessed = new List<int>();
        private IFeatureClass _primaryGen = null;
        private IRelationshipClass _primaryGenSPRelClass = null;
        private IRelationshipClass _spSlRelClass = null;
        private static IMMSession _mmSession;
        private static IMMPxApplication _pxApplication;
        private static string _versionName;
        private IWorkspace _workspace = null;
        private static int count = 0;
        int _MapNoFldIedx;
        int _MapNo2000FldIedx;
        int _MapNo500FldIedx;
        int _AlphaFldIedx;
        int _NumFldIedx;
        int _GlobalIdFldIedx;
        private static List<string> listOfSLsProcessed = new List<string>();
        private static List<string> listOfSessionsCreated = new List<string>();
        private static List<string> listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes = new List<string>();
        private static List<string> listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes = new List<string>();
 
        private static int iCountPrimGensRelatedToServicePoint = 0;
        private static int iCountPrimGensNOTRelatedToServicePoint = 0;
        private static int iCountPrimGensRelatedToServiceLocation = 0;
        private static int iCountPrimGensNOTRelatedToServiceLocation = 0;
        private static int iCountPrimGensReplacedByServiceLocations = 0;

        private static DataTable sessionVsPrimGenerationAndServiceLocUpdated = new DataTable();
        private static DataTable dtRecordsAlreadyReplaced = null;

        private static IMMAppInitialize _mmAppInit = null;
        private static mmLicenseStatus mmLS;
        private static mmLicenseStatus arcFMLicenseStatus;
                


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

        public bool StartProcess(string argStrOIDsPrimaryGenToProcess)
        {
            iCountPrimGensRelatedToServicePoint = 0;
            iCountPrimGensNOTRelatedToServicePoint = 0;
            iCountPrimGensRelatedToServiceLocation = 0;
            iCountPrimGensNOTRelatedToServiceLocation = 0;
            iCountPrimGensReplacedByServiceLocations = 0;

            bool bOpeartionSuccess = true;
            listOfOidsProcessed = new List<int>();
            count = 0;
            _pxApplication = new PxApplicationClass();
            //Code block to disable Auto Updaters
            Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
            IMMAutoUpdater auController = (IMMAutoUpdater)Activator.CreateInstance(type);
            mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;
            int TotalPrimaryGenFound = 0;
            IQueryFilter qF = null;
            IFeatureCursor featCur = null;
            IFeature feat = null;
            List<int> listOids = null;
            try
            {
                DateTime start = DateTime.Now;
                _log.Info("");               
                _log.Info("Starting process at : " + start);

                _log.Info("Initializing Arc GIS / Arc FM licenses.");
                bOpeartionSuccess = InitilizeLicenses();
                _log.Info("Reading config file.");
                ReadConfigurations.ReadFromConfiguration();

                if (!bOpeartionSuccess)
                    return bOpeartionSuccess;

                VersionOperations._mmAppInit = new MMAppInitialize();
                // check and see if the type of license is available to check out
                VersionOperations.mmLS = VersionOperations._mmAppInit.IsProductCodeAvailable(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                if (VersionOperations.mmLS == mmLicenseStatus.mmLicenseAvailable)
                {
                    // if the license is available, try to check it out.
                    VersionOperations.arcFMLicenseStatus = _mmAppInit.Initialize(Miner.Interop.mmLicensedProductCode.mmLPArcFM);

                    if (VersionOperations.arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                    {
                        // if the license cannot be checked out, an exception is raised
                        throw new Exception("The ArcFM license requested could not be checked out");
                    }
                    else
                    {
                        listOids = new List<int>();

                        //auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                        //_log.Info("Disabled Auto Updaters..");
                        _workspace = GetWorkspace(ReadConfigurations.EDWorkSpaceConnString);
                        IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)_workspace;

                        if (string.IsNullOrEmpty(argStrOIDsPrimaryGenToProcess))
                        {
                            IFeatureClass primaryGen = featversionWorkspace.OpenFeatureClass(ReadConfigurations.PrimaryGenerationClassName);
                            //int cnt = primaryGen.FeatureCount(null);

                            qF = new QueryFilterClass();
                            qF.SubFields = "OBJECTID";
                            ((IQueryFilterDefinition)qF).PostfixClause = "ORDER BY CIRCUITID";
                           
                            try
                            {
                                featCur = primaryGen.Search(qF, true);
                                if (featCur != null)
                                {
                                    feat = featCur.NextFeature();
                                    while (feat != null)
                                    {
                                        listOids.Add(feat.OID);
                                        feat = featCur.NextFeature();
                                    }
                                }
                            }
                            catch (Exception exp)
                            {
                                _log.Info(exp.Message + " at " + exp.StackTrace);
                            }
                            finally
                            {
                                if (featCur != null && Marshal.IsComObject(featCur)) { while (Marshal.ReleaseComObject(featCur) > 0) { } }
                                if (qF != null && Marshal.IsComObject(qF)) { while (Marshal.ReleaseComObject(qF) > 0) { } }
                            }
                        }
                        else
                        {
                            listOids = new List<int>(System.Array.ConvertAll(argStrOIDsPrimaryGenToProcess.Split(','), int.Parse));
                        }

                        TotalPrimaryGenFound = listOids.Count;

                        // Refine the list and remove already processed primary generations from the list.
                        //RemoveAlreadyProcessedPrimaryGeneration(featversionWorkspace, listOids);

                        RemoveAlreadyProcessedPrimaryGeneration_New(featversionWorkspace, listOids);

                        _log.Info("Total primary generations found : " + listOids.Count);

                        if (!sessionVsPrimGenerationAndServiceLocUpdated.Columns.Contains("PRIMGENOID"))
                            sessionVsPrimGenerationAndServiceLocUpdated.Columns.Add("PRIMGENOID", typeof(string));

                        if (!sessionVsPrimGenerationAndServiceLocUpdated.Columns.Contains("PRIMGENGUID"))
                            sessionVsPrimGenerationAndServiceLocUpdated.Columns.Add("PRIMGENGUID", typeof(string));

                        if (!sessionVsPrimGenerationAndServiceLocUpdated.Columns.Contains("SLOID"))
                            sessionVsPrimGenerationAndServiceLocUpdated.Columns.Add("SLOID", typeof(string));

                        if (!sessionVsPrimGenerationAndServiceLocUpdated.Columns.Contains("SLGUID"))
                            sessionVsPrimGenerationAndServiceLocUpdated.Columns.Add("SLGUID", typeof(string));

                        if (!sessionVsPrimGenerationAndServiceLocUpdated.Columns.Contains("SESSIONNAME"))
                            sessionVsPrimGenerationAndServiceLocUpdated.Columns.Add("SESSIONNAME", typeof(string));

                        if (!sessionVsPrimGenerationAndServiceLocUpdated.Columns.Contains("CONNECTEDTOPRIMCOND"))
                            sessionVsPrimGenerationAndServiceLocUpdated.Columns.Add("CONNECTEDTOPRIMCOND", typeof(bool));

                        bOpeartionSuccess = MakeEditsInVersion1(_workspace, count, listOids);

                        _log.Info("");
                        _log.Info("");
                        _log.Info("Total sessions created : " + listOfSessionsCreated.Count);
                        _log.Info("");

                        foreach (string sessionName in listOfSessionsCreated)
                        {
                            _log.Info(sessionName);
                        }

                        _log.Info("");
                        _log.Info("");

                        _log.Info("Updates done per session.");

                        _log.Info("");
                        _log.Info("");
                        string SLGUID = null;
                        string SLOID = null;
                        string PrimGenOID = null;
                        string Combination = null;
                        StringBuilder sb = new StringBuilder();

                        foreach (string sessionName in listOfSessionsCreated)
                        {
                            _log.Info("Service locations OIDs updated in session :" + sessionName);
                            _log.Info("");
                            DataRow[] rows = sessionVsPrimGenerationAndServiceLocUpdated.Select("SESSIONNAME='" + sessionName + "'");

                            foreach (DataRow pRow in rows)
                            {
                                SLOID = pRow.Field<string>("SLOID");

                                if (!string.IsNullOrEmpty(SLOID))
                                {
                                    sb.Append(SLOID + ",");
                                }
                            }
                            _log.Info(sb.ToString());

                            sb.Remove(0, sb.Length);

                            _log.Info("");
                            _log.Info("");


                            _log.Info("Service locations GUIDs updated in session :" + sessionName);
                            _log.Info("");
                            rows = sessionVsPrimGenerationAndServiceLocUpdated.Select("SESSIONNAME='" + sessionName + "'");

                            foreach (DataRow pRow in rows)
                            {
                                SLGUID = pRow.Field<string>("SLGUID");

                                if (!string.IsNullOrEmpty(SLGUID))
                                {
                                    sb.Append("'" + SLGUID + "',");
                                }
                            }
                            _log.Info(sb.ToString());

                            sb.Remove(0, sb.Length);

                            _log.Info("");
                            _log.Info("");

                            _log.Info("Primary generations OIDs updated in session :" + sessionName);
                            _log.Info("");
                            rows = sessionVsPrimGenerationAndServiceLocUpdated.Select("SESSIONNAME='" + sessionName + "'");

                            foreach (DataRow pRow in rows)
                            {
                                PrimGenOID = pRow.Field<string>("PRIMGENOID");

                                if (!string.IsNullOrEmpty(PrimGenOID))
                                {
                                    sb.Append(PrimGenOID + ",");
                                }
                            }
                            _log.Info(sb.ToString());

                            sb.Remove(0, sb.Length);

                            _log.Info("");
                            _log.Info("");

                            _log.Info("Service locations OID,primgen OID , SL GUID combination updated in session :" + sessionName);
                            _log.Info("");
                            rows = sessionVsPrimGenerationAndServiceLocUpdated.Select("SESSIONNAME='" + sessionName + "'");

                            foreach (DataRow pRow in rows)
                            {
                                Combination = pRow.Field<string>("SLOID") + "#" + pRow.Field<string>("PRIMGENOID") + "#" + pRow.Field<string>("SLGUID");

                                if (!string.IsNullOrEmpty(Combination))
                                {
                                    sb.Append("'" + Combination + "',");
                                }
                            }
                            _log.Info(sb.ToString());

                            sb.Remove(0, sb.Length);

                            _log.Info("");
                            _log.Info("");
                        }

                        _log.Info("");
                        _log.Info("");
                        _log.Info("");

                        sb.Remove(0, sb.Length);
                        _log.Info("Total Primary generations updated in the process in all the sessions : " + sessionVsPrimGenerationAndServiceLocUpdated.Rows.Count);
                        _log.Info("Primary generations OIDs updated in the process in all the sessions.");
                        _log.Info("");
                        _log.Info("");
                        DataRow[] rows2 = sessionVsPrimGenerationAndServiceLocUpdated.Select();

                        foreach (DataRow pRow in rows2)
                        {
                            PrimGenOID = pRow.Field<string>("PRIMGENOID");

                            if (!string.IsNullOrEmpty(PrimGenOID))
                            {
                                sb.Append(PrimGenOID + ",");
                            }
                        }
                        _log.Info(sb.ToString());

                        sb.Remove(0, sb.Length);

                        _log.Info("");
                        _log.Info("");

                        _log.Info("Total service locations updated : " + listOfSLsProcessed.Count);
                        _log.Info("");
                        _log.Info("");
                        _log.Info("These all service locations shape has been updated in the process in all the sessions.");

                        // Log the service locations updated 
                        foreach (string guid in listOfSLsProcessed)
                        {
                            sb.Append("'" + guid + "',");
                        }

                        _log.Info("");
                        _log.Info("");
                        _log.Info(sb.ToString());

                        _log.Info("");
                        _log.Info("");
                        _log.Info("Total primary generations not connected to specified conductor : " + listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes.Count);
                        _log.Info("These primary generations are not connected to specified conductor type.");

                        sb.Remove(0, sb.Length);

                        foreach (string oid in listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes)
                        {
                            sb.Append(oid + ",");
                        }

                        _log.Info("");
                        _log.Info(sb.ToString());
                        _log.Info("");
                        _log.Info("");

                        _log.Info("");
                        _log.Info("");
                        _log.Info("Total primary generations connected to specified conductor : " + listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes.Count);
                        _log.Info("These primary generations are connected to specified conductor type.");

                        sb.Remove(0, sb.Length);

                        foreach (string oid in listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes)
                        {
                            sb.Append(oid + ",");
                        }

                        _log.Info("");
                        _log.Info(sb.ToString());
                        _log.Info("");
                        _log.Info("");

                        sb.Remove(0, sb.Length);
                                                
                        foreach (string sessionName in listOfSessionsCreated)
                        {
                            _log.Info("Service locations OIDs updated in session :" + sessionName);
                            _log.Info("");
                            DataRow[] rows = sessionVsPrimGenerationAndServiceLocUpdated.Select("SESSIONNAME='" + sessionName + "'");

                            foreach (DataRow pRow in rows)
                            {
                                SLOID = pRow.Field<string>("SLOID");

                                if (!string.IsNullOrEmpty(SLOID))
                                {
                                    sb.Append(SLOID + ",");
                                }
                            }
                            _log.Info(sb.ToString());

                            sb.Remove(0, sb.Length);

                            _log.Info("");
                            _log.Info("");
                        }


                        _log.Info("Count Summary - ");
                        _log.Info("");
                        _log.Info("Total primary generations found : " + TotalPrimaryGenFound);
                        _log.Info("Total primary generations already processed : " + (TotalPrimaryGenFound - listOids.Count));
                        _log.Info("Total primary generations taken under consideration : " + listOids.Count);
                        _log.Info("Total primary generations NOT related to service point  : " + iCountPrimGensNOTRelatedToServicePoint);
                        _log.Info("Total primary generations related to service points  : " + iCountPrimGensRelatedToServicePoint);
                        _log.Info("Total primary generations related to SP and SP is NOT related to service locations  : " + iCountPrimGensNOTRelatedToServiceLocation);
                        _log.Info("Total primary generations related to SP and SP is related to service locations  : " + iCountPrimGensRelatedToServiceLocation);
                        _log.Info("Total primary generations not connected to specified conductor : " + listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes.Count);
                        _log.Info("Total primary generations connected to specified conductor : " + listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes.Count);
                        _log.Info("Total primary generations replaced by service location : " + iCountPrimGensReplacedByServiceLocations);

                        _log.Info("Process completed.");
                        _log.Info("Total time taken : " + (DateTime.Now - start));
                    }
                }
            }
            catch (Exception exp)
            {
                _log.Info(exp.Message + " at " + exp.StackTrace);
            }
            finally
            {  
                if (VersionOperations._mmAppInit != null)
                    VersionOperations._mmAppInit.Shutdown();

                Common.CloseLicenseObject();
               
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (listOids != null)
                {
                    listOids.Clear();
                    listOids = null;
                }
            }
            return bOpeartionSuccess;
        }

        private bool MakeEditsInVersion1(IWorkspace workspace, int count, List<int> listOids)
        {
            bool bPrimGenDisconnected = false;
            //string currentCircuitID = null;
            //string prevCircuitID = null;
            string StrVersionName = null;
            string StrUserName = null;
            IFeatureWorkspace featversionWorkspace = null;
            IFeature feat_primgen = null;
            ESRI.ArcGIS.esriSystem.ISet relSet = null;
            object obj = null;
            ESRI.ArcGIS.esriSystem.ISet slLocations = null;
            object slocatons = null;
            IFeature feat_serviceLoc = null;
            ISimpleJunctionFeature simplejunctionFeature = null;
            IFeature edgeFeat = null;
            INetworkFeature netWorkFeat = null;
            INetworkFeature slNet = null;
            List<IFeature> lstOfedgeFeaturetoDel = null;
            IPoint pnt = null;
            string sl_globalid = null;
            string primgen_globalid = null;
            List<IFeature> lstOfEdges = null;

            IQueryFilter pQF = null;
            IFeatureCursor pFCur = null;
            string circuitIDPrevious_PG = null;
            string circuitIDCurrent_PG = null;
            string circuitIDCurrent_SL = null;
            bool bCreateAnotherSesssionBasedOnCktIDAndNoOfUpdates = false;
            bool bExceptionOccured = false;
            int EditsPerBatchRun = 0;
            int TotalEdits = 0;
            bool primaryGenConnectedtoPrimOHCond;
            //VersionVsSession
            int versionCount = 1;
            try
            {
               
               // Try initilizing ArcFM license in using statement 

                StrUserName = ConfigurationManager.AppSettings["SMUser"];

                int localCount = 0;
                int editCount = 0;
                int editCountInCurrentSession = 0;
                int editsPerSessios = Convert.ToInt32(ConfigurationManager.AppSettings["EditsPerSession"]);

                EditsPerBatchRun = Convert.ToInt32(ConfigurationManager.AppSettings["EditsPerBatchRun"]);

                string slversionname = string.Empty;
                IWorkspaceEdit editSession = null;
                IVersion2 dataversion = null;
                int processedPG = 0;
                //IFeatureClass schematicsGrid = ((IFeatureWorkspace)workspace).OpenFeatureClass("edgis.schematics_unified_grid");
                //if (schematicsGrid == null)
                //{
                //    _log.Error("unable to open 'edgis.schematics_unified_grid' feature class");
                //}
                //else
                //{
                //    _MapNoFldIedx = schematicsGrid.Fields.FindField("MAPNO");
                //    _MapNo2000FldIedx = schematicsGrid.Fields.FindField("MAPNO_2000");
                //    _MapNo500FldIedx = schematicsGrid.Fields.FindField("MAPNO_500");
                //    _AlphaFldIedx = schematicsGrid.Fields.FindField("ALPHA");
                //    _NumFldIedx = schematicsGrid.Fields.FindField("NUM");
                //    _GlobalIdFldIedx = schematicsGrid.Fields.FindField("GLOBALID");
                //}
                Connect connect = new Connect();
                Disconnect disConnect = new Disconnect();

                pQF = new QueryFilterClass();

                foreach (int oid in listOids)
                {
                    try
                    { 
                        // Updates in a session on  circuitID basis - for a circuitID all updates will be done in single session.
                        if ((processedPG > 0 && editCountInCurrentSession >= editsPerSessios) || TotalEdits >= EditsPerBatchRun)
                        {
                            IFeature primGen = _primaryGen.GetFeature(oid);
                            circuitIDCurrent_PG = Convert.ToString(primGen.get_Value(primGen.Fields.FindField("CIRCUITID")));

                            if (string.IsNullOrEmpty(circuitIDCurrent_PG) && !string.IsNullOrEmpty(circuitIDPrevious_PG))
                            {
                                // Create another session
                                bCreateAnotherSesssionBasedOnCktIDAndNoOfUpdates = true;
                            }
                            else if (string.IsNullOrEmpty(circuitIDCurrent_PG) && string.IsNullOrEmpty(circuitIDPrevious_PG))
                            {
                                // Can't take decision
                            }
                            else if (!string.IsNullOrEmpty(circuitIDCurrent_PG) && !string.IsNullOrEmpty(circuitIDPrevious_PG))
                            {
                                // Check if circuitID differ then create another session
                                if (circuitIDCurrent_PG != circuitIDPrevious_PG)
                                {
                                    // Create another session                                  
                                        bCreateAnotherSesssionBasedOnCktIDAndNoOfUpdates = true;

                                        if (TotalEdits >= EditsPerBatchRun)
                                            break;
                                }
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        _log.Error(exp.Message+" at "+exp.StackTrace);
                    }



                    if (VersionOperations.arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                    {
                        _log.Info("Arc FM license is not checked out. So checking out Arc FM license.");
                        // if the license is not checked out then , try to check it out.
                        VersionOperations.arcFMLicenseStatus = _mmAppInit.Initialize(Miner.Interop.mmLicensedProductCode.mmLPArcFM);

                        _log.Info("Arc FM license checked out successfully.");
                    }
                   
                    try
                    {
                        bPrimGenDisconnected = false;

                        try
                        {
                            if (processedPG == 0)
                            {
                                //VersionVsSession
                                //CreateSession(localCount.ToString());

                                //VersionVsSession
                                _versionName = ReadConfigurations.VersionNamePrefix+ versionCount;

                                dataversion = CreateVersion(workspace, _versionName);
                                _log.Info("Creating new version " + _versionName);
                                featversionWorkspace = (IFeatureWorkspace)dataversion;
                                _primaryGen = featversionWorkspace.OpenFeatureClass(ReadConfigurations.PrimaryGenerationClassName);

                                _primaryGenSPRelClass = featversionWorkspace.OpenRelationshipClass("EDGIS.PrimaryGeneration_ServicePoint");
                                _spSlRelClass = featversionWorkspace.OpenRelationshipClass("EDGIS.ServiceLocation_ServicePoint");

                                //schematicsGrid = featversionWorkspace.OpenFeatureClass("edgis.schematics_unified_grid");
                            }
                            else if (bCreateAnotherSesssionBasedOnCktIDAndNoOfUpdates)
                            {
                                editCount++;
                                try
                                {
                                    editCountInCurrentSession = 0;

                                    count = count + localCount;
                                    if (Editor.EditState == EditState.StateEditing)
                                    {
                                        // Add the primary generations successfully processed in current session in the list of replcaed primary generations
                                        AddProcessedPrimaryGenerationInProcessedList(featversionWorkspace, _versionName);

                                        Editor.StopOperation("");
                                        Editor.StopEditing(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
                                }

                                //VersionVsSession
                                //CreateSession(localCount.ToString());

                                //VersionVsSession
                                versionCount++;
                                _versionName = ReadConfigurations.VersionNamePrefix + versionCount;

                                _log.Info("Creating new version " + _versionName);
                                dataversion = CreateVersion(workspace, _versionName);

                                featversionWorkspace = (IFeatureWorkspace)dataversion;
                                _primaryGen = featversionWorkspace.OpenFeatureClass(ReadConfigurations.PrimaryGenerationClassName);

                                _primaryGenSPRelClass = featversionWorkspace.OpenRelationshipClass("EDGIS.PrimaryGeneration_ServicePoint");
                                _spSlRelClass = featversionWorkspace.OpenRelationshipClass("EDGIS.ServiceLocation_ServicePoint");

                                //schematicsGrid = featversionWorkspace.OpenFeatureClass("edgis.schematics_unified_grid");

                                localCount = 0;
                                bCreateAnotherSesssionBasedOnCktIDAndNoOfUpdates = false;
                            }

                            if (dataversion == null)
                            {
                                _log.Info("Data version found null");
                            }

                            if (dataversion != null)
                            {
                                editSession = (IWorkspaceEdit)dataversion;
                                if (Editor.EditState == EditState.StateNotEditing)
                                {
                                    //editSession.StartEditing(false);
                                    //editSession.StartEditOperation();
                                    Editor.StartEditing((IWorkspace)editSession);
                                    Editor.StartOperation();
                                }

                                // This line giving error at server so need to change this approach to fetch primary generation
                                // feat_primgen = _primaryGen.GetFeature(oid);

                                pQF.WhereClause = "OBJECTID=" + oid;
                                pFCur = _primaryGen.Search(pQF, false);
                                if (pFCur != null)
                                {
                                    feat_primgen = pFCur.NextFeature();
                                }

                                if (feat_primgen != null && _primaryGenSPRelClass != null && _spSlRelClass != null)
                                {
                                    // _log.Info("Started editing data for primary generation OID: " + oid.ToString());
                                    try
                                    {
                                        relSet = _primaryGenSPRelClass.GetObjectsRelatedToObject((IObject)feat_primgen);
                                        relSet.Reset();
                                        //cr.ManageLifetime(relSet);
                                        obj = relSet.Next();                                       
                                        //_log.Info("Checking Primary Generation OID: " + feat_primgen.OID.ToString());

                                        if (obj == null)
                                        {
                                            _log.Info("Primary Generation OID: " + feat_primgen.OID.ToString() + " does not has related Service Point.");
                                            iCountPrimGensNOTRelatedToServicePoint++;
                                        }
                                        else
                                        {
                                            // _log.Info("Primary Generation OID: " + feat_primgen.OID.ToString() + " has related Service Points");
                                            iCountPrimGensRelatedToServicePoint++;
                                        }

                                        while (obj != null)
                                        {
                                            _log.Info("Primary Generation OID: " + feat_primgen.OID.ToString() + " has related Service Points");
                                            slLocations = _spSlRelClass.GetObjectsRelatedToObject((IObject)obj);
                                            slLocations.Reset();
                                            slocatons = slLocations.Next();

                                            if (slocatons == null)
                                            {
                                                _log.Info("Primary Generation OID: " + feat_primgen.OID.ToString() + " has related Service Points but does not has related service location.");
                                                iCountPrimGensNOTRelatedToServiceLocation++;
                                            }
                                            else
                                            {
                                                _log.Info("Primary Generation OID: " + feat_primgen.OID.ToString() + " has related Service Points AND has related service location also.");
                                                iCountPrimGensRelatedToServiceLocation++;
                                            }

                                            while (slocatons != null)
                                            {
                                                _log.Info("Started editing data for primary generation OID: " + oid.ToString());

                                                _log.Info("Primary Generation OID: " + feat_primgen.OID + " will be peplaced by Service Location OID: " + ((IFeature)slocatons).OID + "");
                                                feat_serviceLoc = slocatons as IFeature;
                                                if (feat_serviceLoc == null)
                                                {
                                                    _log.Info("Service location feature found NULL for primary generation OID: " + oid.ToString());
                                                }

                                                if (feat_serviceLoc != null)
                                                {
                                                    // Check here , if this service location is already been replaced then dont use it further
                                                    if (CheckServiceLocationIsAlreadyReplaced(feat_serviceLoc.OID))
                                                    {
                                                        _log.Info("Service location with OID : " + feat_serviceLoc.OID + " is already been replaced so not replacing it now.");
                                                        slocatons = slLocations.Next();
                                                        continue;
                                                    }

                                                    _log.Info("Finding Connected Edges for Primary generation OID: " + feat_primgen.OID);
                                                    simplejunctionFeature = (ISimpleJunctionFeature)feat_primgen;
                                                    lstOfEdges = new List<IFeature>();
                                                    for (int i = 0; i < simplejunctionFeature.EdgeFeatureCount; i++)
                                                    {
                                                        edgeFeat = simplejunctionFeature.get_EdgeFeature(i) as IFeature;
                                                        if (!edgeFeat.Shape.IsEmpty && ((IPolyline)edgeFeat.Shape).Length > 0)
                                                        {
                                                            lstOfEdges.Add(simplejunctionFeature.get_EdgeFeature(i) as IFeature);
                                                        }
                                                    }

                                                    if (IsPrimGenConnectedToPriOHConductor(lstOfEdges))
                                                    {
                                                        //listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes.Add(feat_primgen.OID.ToString());
                                                        primaryGenConnectedtoPrimOHCond = true;
                                                    }
                                                    else
                                                    {
                                                        primaryGenConnectedtoPrimOHCond = false;
                                                    }

                                                    if (ProcessFurther(lstOfEdges))
                                                    {
                                                        _log.Info("Current primary generaiton OID : " + feat_primgen.OID.ToString() + " was connected to one of Conductor types : Primary Overhead Conductor,Primary Underground Conductor,Secondary Overhead Conductor,Secondary Underground Conductor");
                                                    }
                                                    else
                                                    {
                                                        //Log these all primary generation together
                                                        _log.Info("Primary generation OID : " + feat_primgen.OID + " is not connected to any of the conductior type - Primary Overhead Conductor ,Primary Underground Conductor , Secondary Overhead Conductor,Secondary Underground Conductor");
                                                        listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes.Add(feat_primgen.OID.ToString());
                                                    }

                                                    try
                                                    {

                                                        //Disconnect Primary generation first
                                                        netWorkFeat = feat_primgen as INetworkFeature;
                                                        if (netWorkFeat != null)
                                                        {
                                                           
                                                            //Capturing circuitID first -
                                                            circuitIDPrevious_PG = Convert.ToString(feat_primgen.get_Value(feat_primgen.Fields.FindField("CIRCUITID")));

                                                            disConnect.DisconnectFeature(feat_primgen);
                                                            _log.Info("Disconnected primary generation  OID : " + feat_primgen.OID + " from network.");
                                                        }
                                                        slNet = feat_serviceLoc as INetworkFeature;
                                                        if (slNet != null)
                                                        {
                                                            lstOfEdges.Add(feat_serviceLoc);
                                                            lstOfedgeFeaturetoDel = GetConnectedEdgeToDelete(feat_serviceLoc);

                                                            try
                                                            {
                                                                circuitIDCurrent_SL = Convert.ToString(feat_serviceLoc.get_Value(feat_serviceLoc.Fields.FindField("CIRCUITID")));
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
                                                            }
                                                            
                                                            
                                                            //Then disconnect Service location                                                            
                                                            disConnect.DisconnectFeature(feat_serviceLoc);


                                                            _log.Info("Disconnected Service Location OID: " + ((IFeature)slocatons).OID + " from network.");
                                                            //Update shape of Service location with primary generation
                                                            feat_serviceLoc.Shape = feat_primgen.Shape;
                                                            //if (schematicsGrid != null)
                                                                //GetIntersectingSchematicsGrid(schematicsGrid, feat_serviceLoc.Shape);
                                                            //serviceLoc.Value[serviceLoc.Fields.FindField("GENCATEGORY")] = 5;
                                                            pnt = new PointClass();
                                                            pnt.X = ((IPoint)feat_primgen.Shape).X + 30;
                                                            pnt.Y = ((IPoint)feat_primgen.Shape).Y;
                                                            pnt.SpatialReference = ((IPoint)feat_primgen.Shape).SpatialReference;

                                                            feat_primgen.Shape = pnt;
                                                            _log.Info("Updated Service location OID: " + ((IFeature)slocatons).OID.ToString() + " Shape with Primary generation Shape OID:" + feat_primgen.OID.ToString());
                                                            //Connect Service location back in network
                                                            //slNet.Connect();

                                                            //Version name will be updated through auto updaters if not updated then uncomment below line
                                                            StrVersionName = StrUserName + "." + _versionName;
                                                            StrVersionName = StrVersionName.ToUpper();
                                                            feat_serviceLoc.set_Value(feat_serviceLoc.Fields.FindField("VERSIONNAME"), StrVersionName);

                                                           
                                                            sl_globalid = Convert.ToString(feat_serviceLoc.get_Value(feat_serviceLoc.Fields.FindField("GLOBALID")));
                                                            primgen_globalid = Convert.ToString(feat_primgen.get_Value(feat_primgen.Fields.FindField("GLOBALID")));

                                                            listOfSLsProcessed.Add(sl_globalid);
                                                            iCountPrimGensReplacedByServiceLocations++;

                                                            bPrimGenDisconnected = true;
                                                            sessionVsPrimGenerationAndServiceLocUpdated.Rows.Add(oid, primgen_globalid, feat_serviceLoc.OID, sl_globalid, _versionName, primaryGenConnectedtoPrimOHCond);

                                                            try
                                                            {
                                                                connect.ConnectFeatures(lstOfEdges);

                                                                 //Updating circuitID for servicelocation if not matching --
                                                                try
                                                                {
                                                                    if (circuitIDCurrent_SL != circuitIDPrevious_PG)
                                                                    {                                                                       
                                                                        feat_serviceLoc.set_Value(feat_serviceLoc.Fields.FindField("CIRCUITID"), circuitIDPrevious_PG);
                                                                        _log.Info("Updating ckt ID value : '" + circuitIDPrevious_PG + "' for service location OID : " + feat_serviceLoc.OID);
                                                                    }
                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                     _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
                                                                }


                                                                //Once after disconnect service location, delete the orphan edges

                                                                if (lstOfedgeFeaturetoDel != null && lstOfedgeFeaturetoDel.Count > 0)
                                                                {
                                                                    _log.Info("Deleting orphan Edges for Service Location OID: " + ((IFeature)slocatons).OID.ToString() + "");
                                                                    foreach (IFeature featToDel in lstOfedgeFeaturetoDel)
                                                                    {
                                                                        //Pre-prod - issue - only deleting pseudo conductor
                                                                        string subtypevalue = Convert.ToString(featToDel.get_Value(featToDel.Fields.FindField("SUBTYPECD")));

                                                                        if (featToDel.Class.AliasName.ToUpper().Contains("SECONDARY OVERHEAD CONDUCTOR"))
                                                                        {
                                                                            if (subtypevalue == "5")
                                                                            {
                                                                                _log.Info("Deleting SECONDARY OVERHEAD CONDUCTOR");
                                                                                featToDel.Delete();
                                                                            }
                                                                            else
                                                                            {
                                                                                _log.Info("Not Deleting SECONDARY OVERHEAD CONDUCTOR : OID : " + featToDel.OID);
                                                                            }
                                                                        }

                                                                        if (featToDel.Class.AliasName.ToUpper().Contains("SECONDARY UNDERGROUND CONDUCTOR"))
                                                                        {
                                                                            if (subtypevalue == "6")
                                                                            {
                                                                                _log.Info("Deleting UNDERGROUND OVERHEAD CONDUCTOR");
                                                                                featToDel.Delete();
                                                                            }
                                                                            else
                                                                            {
                                                                                _log.Info("Not Deleting SECONDARY UNDERGROUND CONDUCTOR : OID : " + featToDel.OID);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
                                                            }

                                                            _log.Info("Service location OID: " + ((IFeature)slocatons).OID.ToString() + " Connected back in network in place of Primary Generation OID:" + feat_primgen.OID);
                                                            localCount++;
                                                            editCount++;

                                                            editCountInCurrentSession++;
                                                            TotalEdits++;

                                                            _log.Info("Till now total service locations processed : " + sessionVsPrimGenerationAndServiceLocUpdated.Rows.Count);
                                                            _log.Info("Till now service locations processed in current session: " + localCount);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
                                                    }

                                                    try
                                                    {
                                                        feat_serviceLoc.Store();
                                                        feat_primgen.Store();
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
                                                    }
                                                }
                                                slocatons = slLocations.Next();
                                            }
                                            obj = relSet.Next();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
                                    }


                                    //if (!bPrimGenDisconnected)
                                    //{
                                    //    sessionVsPrimGenerationAndServiceLocUpdated.Rows.Add(_versionName, oid, null);
                                    //}

                                    //Checking again if Prim gen is connected with any feature and if found then disconnect it 

                                    //ISimpleJunctionFeature simplejunctionFeaturePrimGen = (ISimpleJunctionFeature)feat_primgen;
                                    //List<IFeature> lstOfEdgesPrim = new List<IFeature>();
                                    //for (int i = 0; i < simplejunctionFeaturePrimGen.EdgeFeatureCount; i++)
                                    //{
                                    //    IFeature edgeFeat = simplejunctionFeaturePrimGen.get_EdgeFeature(i) as IFeature;
                                    //    if (!edgeFeat.Shape.IsEmpty && ((IPolyline)edgeFeat.Shape).Length > 0)
                                    //    {
                                    //        lstOfEdgesPrim.Add(simplejunctionFeaturePrimGen.get_EdgeFeature(i) as IFeature);
                                    //    }
                                    //}

                                    //if (lstOfEdgesPrim.Count > 0)
                                    //{
                                    //    INetworkFeature netWorkFeat = feat_primgen as INetworkFeature;
                                    //    if (netWorkFeat != null)
                                    //    {
                                    //        //netWorkFeat.Disconnect();
                                    //        disConnect.DisconnectFeature(feat_primgen);
                                    //        editCount++;
                                    //        _log.Info("Disconnected primary generation from network");
                                    //    }
                                    //}
                                }
                                else
                                {
                                    _log.Info("For Primary Generation OID: " + feat_primgen.OID.ToString() + " either primary gen or gen_SP relationship or SP_SL relationship is null.");
                                }   
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);

                            bExceptionOccured = true;
                        }
                        processedPG++;
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
                    }
                    finally
                    {
                        if (pFCur != null && Marshal.IsComObject(pFCur)) { while (Marshal.ReleaseComObject(pFCur) > 0) { } }
                        pFCur = null;

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }                    
                }
                if (Editor.EditState == EditState.StateEditing)
                {
                    // Add the primary generations successfully processed in current session in the list of replcaed primary generations
                    AddProcessedPrimaryGenerationInProcessedList(featversionWorkspace, _versionName);

                    //editSession.StopEditOperation();
                    //editSession.StopEditing(true);
                    Editor.StopOperation("");
                    Editor.StopEditing(true);
                    //Common.InitializeESRILicense();
                    //Common.InitializeArcFMLicense();
                }
                _log.Info("Stopped editing data in " + _versionName);
                _log.Info("Total " + (count + localCount).ToString() + " Service Locations are updated");
                return true;
            }
            catch (Exception ex)
            {
                _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }

        private bool ProcessFurther(List<IFeature> lstOfEdges)
        {
            bool bProcessFurther = false;
            try
            {
                bProcessFurther = lstOfEdges.Count > 0 && lstOfEdges.Any(x => ((IFeature)x).Class.AliasName == "Primary Overhead Conductor" || ((IFeature)x).Class.AliasName == "Primary Underground Conductor" || ((IFeature)x).Class.AliasName == "Secondary Overhead Conductor" || ((IFeature)x).Class.AliasName == "Secondary Underground Conductor");
            }
            catch (Exception ex)
            {
                _log.Error("Error : " + ex.Message + " at " + ex.StackTrace);
            }
            return bProcessFurther;
        }

        private bool IsPrimGenConnectedToPriOHConductor(List<IFeature> lstOfEdges)
        {
            bool bProcessFurther = false;
            try
            {
                bProcessFurther = lstOfEdges.Count > 0 && lstOfEdges.Any(x => ((IFeature)x).Class.AliasName == "Primary Overhead Conductor" || ((IFeature)x).Class.AliasName == "Primary Underground Conductor");
            }
            catch (Exception exp)
            {
                // throw;
            }
            return bProcessFurther;
        }

        /// <summary>
        /// Deletes the connected edge for service location
        /// </summary>
        /// <param name="serviceLoc"></param>
        private List<IFeature> GetConnectedEdgeToDelete(IFeature serviceLoc)
        {
            List<IFeature> lstOfEdges = new List<IFeature>();
            try
            {
                ISimpleJunctionFeature simplejunctionFeature = (ISimpleJunctionFeature)serviceLoc;
                for (int i = 0; i < simplejunctionFeature.EdgeFeatureCount; i++)
                {
                    IFeature edgeFeat = simplejunctionFeature.get_EdgeFeature(i) as IFeature;
                    if (!edgeFeat.Shape.IsEmpty && ((IPolyline)edgeFeat.Shape).Length > 0)
                    {
                        lstOfEdges.Add(edgeFeat);
                    }
                }
                return lstOfEdges;
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);
                return null;
            }
        }

        private void GetIntersectingSchematicsGrid(IFeatureClass schematicsGrid, IGeometry iGeometry)
        {
            ISpatialFilter sF = null;
            IFeatureCursor featCur = null;
            try
            {
                sF = new SpatialFilterClass();
                sF.Geometry = iGeometry;
                sF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                featCur = schematicsGrid.Search(sF, false);
                IFeature schematicGridFeat = null;
                while ((schematicGridFeat = featCur.NextFeature()) != null)
                {
                    _log.Info("----------Intersecting Schematic GriD Found-----------");
                    _log.Info("MAPNO: " + schematicGridFeat.get_Value(_MapNoFldIedx).ToString() + ", MAPNO_2000: " + schematicGridFeat.get_Value(_MapNo2000FldIedx).ToString() + ", MAPNO_500: " + schematicGridFeat.get_Value(_MapNo500FldIedx).ToString() + ", ALPHA: " + schematicGridFeat.get_Value(_AlphaFldIedx).ToString() + " NUM: " + schematicGridFeat.get_Value(_NumFldIedx).ToString() + ", GLOBALID: " + schematicGridFeat.get_Value(_GlobalIdFldIedx).ToString());
                    _log.Info("------------------------------------------------------");
                }
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);
            }
            finally 
            {
                if (featCur != null && Marshal.IsComObject(featCur)) { while (Marshal.ReleaseComObject(featCur) > 0) { } }
                if (sF != null && Marshal.IsComObject(sF)) { while (Marshal.ReleaseComObject(sF) > 0) { } }
            }
        }

        //private static bool MakeEditsInVersion(IWorkspace workspace,int count)
        //{
        //    try
        //    {
        //        using (ComReleaser cr = new ComReleaser())
        //        {
        //            IVersion2 dataversion = null;
        //            string slversionname = string.Empty;
        //            slversionname = ReadConfigurations.TargetVersionName + "_" + count.ToString();
        //            _log.Info("Creating new version " + slversionname);
        //            dataversion = CreateVersion(workspace, slversionname);

        //            if(dataversion != null)
        //            {
        //                IFeatureWorkspace featversionWorkspace = (IFeatureWorkspace)dataversion;
        //                IFeatureClass primaryGen = featversionWorkspace.OpenFeatureClass(ReadConfigurations.PrimaryGenerationClassName);

        //                IRelationshipClass primaryGenSPRelClass = featversionWorkspace.OpenRelationshipClass("EDGIS.PrimaryGeneration_ServicePoint");
        //                IRelationshipClass spSlRelClass = featversionWorkspace.OpenRelationshipClass("EDGIS.ServiceLocation_ServicePoint");

        //                IWorkspaceEdit editSession = (IWorkspaceEdit)dataversion;
        //                IFeatureCursor featCur = primaryGen.Search(null, false);
        //                if (primaryGenSPRelClass != null && spSlRelClass != null)
        //                {
        //                    editSession.StartEditing(false);
        //                    editSession.StartEditOperation();
        //                    _log.Info("Started editing data..");
        //                    try
        //                    {
        //                        int localCount = 0;
        //                        cr.ManageLifetime(featCur);
        //                        IFeature feat = null;
        //                        while ((feat = featCur.NextFeature()) != null)
        //                        {
        //                            if (localCount > 0 && localCount % 100 == 0)
        //                            {
        //                                try
        //                                {
        //                                    if (editSession.IsBeingEdited())
        //                                    {
        //                                        editSession.StopEditOperation();
        //                                        editSession.StopEditing(true);
        //                                    }
        //                                }
        //                                catch (Exception ex1)
        //                                {
        //                                }
        //                                count = count + localCount;
        //                                MakeEditsInVersion(workspace,count);
        //                                break;
        //                            }
        //                            if (!listOfOidsProcessed.Contains(feat.OID))
        //                            {
        //                                listOfOidsProcessed.Add(feat.OID);
        //                                ESRI.ArcGIS.esriSystem.ISet relSet = primaryGenSPRelClass.GetObjectsRelatedToObject((IObject)feat);
        //                                relSet.Reset();
        //                                cr.ManageLifetime(relSet);
        //                                object obj = null;
        //                                while ((obj = relSet.Next()) != null)
        //                                {
        //                                    _log.Info("Primary Generation OID: " + feat.OID.ToString() + " has related Service Points");
        //                                    ESRI.ArcGIS.esriSystem.ISet slLocations = spSlRelClass.GetObjectsRelatedToObject((IObject)obj);
        //                                    slLocations.Reset();
        //                                    object slocatons = null;
        //                                    while ((slocatons = slLocations.Next()) != null)
        //                                    {
        //                                        _log.Info("Updating Service Location OID: " + ((IFeature)slocatons).OID.ToString() + " ....");
        //                                        IFeature serviceLoc = slocatons as IFeature;
        //                                        if (serviceLoc != null)
        //                                        {
        //                                            _log.Info("Finding Connected Edges for Primary generation...");
        //                                            ISimpleJunctionFeature simplejunctionFeature = (ISimpleJunctionFeature)feat;
        //                                            List<IEdgeFeature> lstOfEdges = new List<IEdgeFeature>();
        //                                            for (int i = 0; i < simplejunctionFeature.EdgeFeatureCount; i++)
        //                                            {
        //                                                IFeature edgeFeat = simplejunctionFeature.get_EdgeFeature(i) as IFeature;
        //                                                if (!edgeFeat.Shape.IsEmpty && ((IPolyline)edgeFeat.Shape).Length >0)
        //                                                {
        //                                                    lstOfEdges.Add(simplejunctionFeature.get_EdgeFeature(i));
        //                                                }
        //                                            }
        //                                            if(lstOfEdges.Count > 0 && lstOfEdges.Any(x => ((IFeature)x).Class.AliasName == "Primary Overhead Conductor" || ((IFeature)x).Class.AliasName == "Primary Underground Conductor" || ((IFeature)x).Class.AliasName == "Secondary Overhead Conductor" || ((IFeature)x).Class.AliasName == "Secondary Underground Conductor")) 
        //                                            {
        //                                                _log.Info("Current primary generaiton OID : " + feat.OID.ToString() + " was connected to one of Conductor types");
        //                                                //Disconnect Primary generation first
        //                                                INetworkFeature netWorkFeat = feat as INetworkFeature;
        //                                                if (netWorkFeat != null)
        //                                                {
        //                                                    netWorkFeat.Disconnect();
        //                                                    _log.Info("Disconnected primary generation from network");
        //                                                }
        //                                                INetworkFeature slNet = serviceLoc as INetworkFeature;
        //                                                if (slNet != null)
        //                                                {
        //                                                    //Then disconnect Service location
        //                                                    slNet.Disconnect();
        //                                                    _log.Info("Disconnected current Service Location from network");
        //                                                    //Update shape of Service location with primary generation
        //                                                    serviceLoc.Shape = feat.Shape;
        //                                                    _log.Info("Updated Service location Shape OID: " + ((IFeature)slocatons).OID.ToString() + " with Primary generation Shape :" + feat.OID.ToString());
        //                                                    //Connect Service location back in network
        //                                                    slNet.Connect();
        //                                                    _log.Info("Service location Connected back in network");
        //                                                    localCount++;
        //                                                }
        //                                                try
        //                                                {
        //                                                    serviceLoc.Store();
        //                                                    feat.Store();
        //                                                }
        //                                                catch (Exception ex)
        //                                                {
        //                                                    _log.Error("Error : " + ex.Message);
        //                                                }
        //                                            }
        //                                            //foreach (IEdgeFeature edgeFeat in lstOfEdges)
        //                                            //{
        //                                            //    try
        //                                            //    {
        //                                            //        IFeature line = edgeFeat as IFeature;
        //                                            //        INetworkFeature simpleEdge = line as INetworkFeature;
        //                                            //        if (simpleEdge != null)
        //                                            //        {
        //                                            //            simpleEdge.Disconnect();
        //                                            //        }
        //                                            //        line.Store();
        //                                            //    }
        //                                            //    catch (Exception ex)
        //                                            //    {

        //                                            //    }
        //                                            //}   
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        _log.Error("Error : " + ex.Message);
        //                    }
        //                    if (editSession.IsBeingEdited())
        //                    {
        //                        editSession.StopEditOperation();
        //                        editSession.StopEditing(true);
        //                    }
        //                    _log.Info("Stopped editing data in " + slversionname);
        //                }
        //            }
        //        }
        //        _log.Info("Total " + count.ToString() + " Service Locations are updated");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

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
                _log.Info(exp.Message + " at " + exp.StackTrace);
            }
            return workspace;
        }

        //internal static bool submitSessionForQC(IMMPxApplication pxApplication)
        //{
        //    const int QA_QC_STATE = 2;

        //    IMMPxNodeEdit nodeedit = new MMPxNodeListClass();
        //    int nodetype = pxApplication.Helper.GetNodeTypeID("Session");

        //    nodeedit.Initialize(nodetype, "Session", _mmSession.get_ID());
        //    IMMPxNode node = (IMMPxNode)nodeedit;

        //    ((IMMPxApplicationEx)pxApplication).HydrateNodeFromDB(node);

        //    GISUtilities.SetNodeState(_pxApplication, node, QA_QC_STATE);

        //    //GISUtilities.UpdateMMPxVersion(_pxApplication, node.Id);

        //    string user = ((IDatabaseConnectionInfo)_wSpace).ConnectedUser;

        //    string sql = "UPDATE process.mm_px_versions SET has_edits=-1, status=1, user_name='" + user + "' WHERE node_id=" +
        //                 node.Id;
        //    object recordsaffected = null;
        //    pxApplication.Connection.Execute(sql, out recordsaffected, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);

        //    //    if (UnlockSession)
        //    //    {
        //    //        sql = "update PROCESS.MM_PX_CURRENT_STATE SET LOCKED = 0 WHERE soid=" + _mmSession.get_ID();
        //    //        _pxApplication.Connection.Execute(sql, out recordsaffected, (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);
        //    //    }
        //    //}

        //    ((IWorkspaceEdit)_wSpace).StopEditing(true);


        //    return true;
        //}

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
            string smUser =userInst[0].ToUpper();
            string smDB = userInst[1].ToUpper();
            string smPwd = ReadEncryption.GetPassword(smUser+"@"+smDB);
            


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
                            //_log.Info(slversionname + " already exists , finding another version number .");

                            //slversion.Delete();

                            slversionname = GetVersionName(argWorkspace, slversionname);
                            //slversionname = "SLUP_" + versionNumber;
                            _versionName = slversionname;
                        }
                        catch (Exception exp)
                        {
                            _log.Info(exp.Message + " at " + exp.StackTrace);
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
                _log.Info(exp.Message + " at " + exp.StackTrace);
            }
            return slversion;
        }

        public static string GetVersionName(IWorkspace argWorkspace, string slversionname)
        {
            IVersion slversion = null;
            int versionNumber = 0;
            bool bVersionNotFound = true;
            try
            {
                versionNumber = Convert.ToInt32(slversionname.Replace("SLUP_", ""));

                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)argWorkspace;
                {
                    IVersion sourceVersion = (IVersion2)vWorkspace.FindVersion(ReadConfigurations.DefaultVersionName);
                    try
                    {
                        //_log.Info("Checking whether " + slversionname + " already exist or not");
                        while (bVersionNotFound)
                        {
                            try
                            {
                                slversion = (IVersion2)vWorkspace.FindVersion(slversionname);
                            }
                            catch (Exception exp)
                            {                                
                              //  throw;
                                slversion = null;
                            }
                           

                            if (slversion == null)
                            {
                                break;
                                bVersionNotFound = false;
                            }
                            else
                            { 
                                versionNumber++;
                                slversionname = "SLUP_" + versionNumber;                            
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        _log.Info(exp.Message + " at " + exp.StackTrace);
                    }
                }               
            }
            catch (Exception exp)
            {
                _log.Info(exp.Message + " at " + exp.StackTrace);
            }
            return slversionname;

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
                _log.Info(exp.Message + " at " + exp.StackTrace);
            }
            return defaultVersion;
        }

        public static bool RemoveAlreadyProcessedPrimaryGeneration(IFeatureWorkspace featversionWorkspace, List<int> argListOids)
        {
            bool processSuccess = true;           
            int PrimGenOID = 0;
            string strConnectionString = null;
            string strSqlQuery = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                // m4jf edgisrearch 919 - get connection string using PGE_DBPasswordmanagement
                //string[] con = ReadConfigurations.GetCommaSeparatedList(ReadConfigurations.EDConnection, new string[4]);
                //strConnectionString = Common.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);
                strConnectionString = ReadConfigurations.EDConnection;
                //strConnectionString = ReadConfigurations.connString;

                Common._log.Info("Connection string to fetch SL to update data : " + strConnectionString);

                strSqlQuery = "SELECT * FROM " + ReadConfigurations.ChangesLoggedTableName;

                using (OracleConnection oracleconn = new OracleConnection(strConnectionString))
                {
                    oracleconn.Open();
                    using (OracleCommand cmd = new OracleCommand(strSqlQuery, oracleconn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 600;

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            dtRecordsAlreadyReplaced = ds.Tables[0];                           
                        }
                    }
                }

                Common._log.Info("Total record found in table : " + ReadConfigurations.ChangesLoggedTableName + " : " + dtRecordsAlreadyReplaced.Rows.Count);

                foreach (DataRow pRow in dtRecordsAlreadyReplaced.Rows)
                {
                    try
                    {
                        PrimGenOID = Convert.ToInt32(pRow["OBJECTID_PRIMGEN"]);

                        if (argListOids.Contains(PrimGenOID))
                        {
                            argListOids.Remove(PrimGenOID);
                            sb.Append(PrimGenOID+",");
                        }
                    }
                    catch (Exception exp)
                    {
                        _log.Error(exp.Message + " at " + exp.StackTrace);
                    }                
                }

                _log.Info("Below Primary generaions removed from the list of all primary generations as these are alreay replaced.");
                _log.Info(sb.ToString());

                _log.Info("");  
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);
                processSuccess = false;
            }
            finally
            {
                sb = null;        
            }
            return processSuccess;
        }

        public static bool RemoveAlreadyProcessedPrimaryGeneration_New(IFeatureWorkspace featversionWorkspace, List<int> argListOids)
        {
            bool processSuccess = true;
            int PrimGenOID = 0;
            string strConnectionString = null;
            string strSqlQuery = null;
            StringBuilder sb = new StringBuilder();
           
            try
            {
                //string[] con = ReadConfigurations.GetCommaSeparatedList(ReadConfigurations.EDConnection, new string[4]);
                //strConnectionString = Common.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);

                strConnectionString = ReadConfigurations.connString;

                Common._log.Info("Connection string to fetch SL to update data : " + strConnectionString);

                strSqlQuery = "SELECT * FROM " + ReadConfigurations.ChangesLoggedTableName;
                dtRecordsAlreadyReplaced = new DataTable();

                using (OleDbConnection con = new OleDbConnection(strConnectionString))
                {
                    con.Open();
                    _log.Info("Connected To DataBase...");
                    //string sql = "Select * from PGEDATA.GEN_SUMMARY_STAGE ";
                    OleDbDataAdapter adp = new OleDbDataAdapter(strSqlQuery, con);
                    adp.Fill(dtRecordsAlreadyReplaced);
                    //return dt;
                }


                //using (OracleConnection oracleconn = new OracleConnection(strConnectionString))
                //{
                //    oracleconn.Open();
                //    using (OracleCommand cmd = new OracleCommand(strSqlQuery, oracleconn))
                //    {
                //        cmd.CommandType = CommandType.Text;
                //        cmd.CommandTimeout = 600;

                //        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                //        {
                //            DataSet ds = new DataSet();
                //            da.Fill(ds);
                //            dtRecordsAlreadyReplaced = ds.Tables[0];
                //        }
                //    }
                //}

                Common._log.Info("Total record found in table : " + ReadConfigurations.ChangesLoggedTableName + " : " + dtRecordsAlreadyReplaced.Rows.Count);

                foreach (DataRow pRow in dtRecordsAlreadyReplaced.Rows)
                {
                    try
                    {
                        PrimGenOID = Convert.ToInt32(pRow["OBJECTID_PRIMGEN"]);

                        if (argListOids.Contains(PrimGenOID))
                        {
                            argListOids.Remove(PrimGenOID);
                            sb.Append(PrimGenOID + ",");
                        }
                    }
                    catch (Exception exp)
                    {
                        _log.Error(exp.Message + " at " + exp.StackTrace);
                    }
                }

                _log.Info("Below Primary generaions removed from the list of all primary generations as these are alreay replaced.");
                _log.Info(sb.ToString());

                _log.Info("");
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);
                processSuccess = false;
            }
            finally
            {
                sb = null;
            }
            return processSuccess;
        }

    

        public static bool AddProcessedPrimaryGenerationInProcessedList(IFeatureWorkspace featversionWorkspace, string argSessionName)
        {
            bool processSuccess = true;
            IQueryFilter pQF = null;
            ICursor pCursor = null;
            string PRIMGENOID = null;
            string CONNECTEDTOPRIMCOND = null;
            try
            {
                ITable pTable = featversionWorkspace.OpenTable(ReadConfigurations.ChangesLoggedTableName);
                pQF = new QueryFilterClass();
                DataRow[] rows = sessionVsPrimGenerationAndServiceLocUpdated.Select("SESSIONNAME='" + argSessionName + "'");

                foreach (DataRow pRow in rows)
                {
                    PRIMGENOID = pRow.Field<string>("PRIMGENOID");

                    if (pRow.Field<bool>("CONNECTEDTOPRIMCOND"))
                        CONNECTEDTOPRIMCOND = "YES";
                    else
                        CONNECTEDTOPRIMCOND = "NO";

                    if (!string.IsNullOrEmpty(PRIMGENOID))
                    {
                        pQF.WhereClause = "OBJECTID_PRIMGEN=" + PRIMGENOID;

                        if (pTable.RowCount(pQF) > 0)
                        {
                            pCursor = pTable.Update(pQF, false);

                            IRow pOldRow = pCursor.NextRow();
                            pOldRow.set_Value(pOldRow.Fields.FindField("SESSIONNAME"), pRow.Field<string>("SESSIONNAME"));
                            pOldRow.set_Value(pOldRow.Fields.FindField("CONNECTEDTOPRIMCOND"), CONNECTEDTOPRIMCOND);
                            pOldRow.Store();

                            _log.Error("Program trying to update already replcaed primary generation with OID : " + PRIMGENOID + ".");
                        }
                        else
                        {
                            IRow pNewRow = pTable.CreateRow();

                            pNewRow.set_Value(pNewRow.Fields.FindField("OBJECTID_PRIMGEN"), pRow.Field<string>("PRIMGENOID"));
                            pNewRow.set_Value(pNewRow.Fields.FindField("GLOBALID_PRIMGEN"), pRow.Field<string>("PRIMGENGUID"));
                            pNewRow.set_Value(pNewRow.Fields.FindField("OBJECTID_SERVLOC"), pRow.Field<string>("SLOID"));
                            pNewRow.set_Value(pNewRow.Fields.FindField("GLOBALID_SERVLOC"), pRow.Field<string>("SLGUID"));
                            pNewRow.set_Value(pNewRow.Fields.FindField("SESSIONNAME"), pRow.Field<string>("SESSIONNAME"));
                            pNewRow.set_Value(pNewRow.Fields.FindField("CONNECTEDTOPRIMCOND"), CONNECTEDTOPRIMCOND);

                            pNewRow.Store();

                            _log.Info("Adding new row in table " + ReadConfigurations.ChangesLoggedTableName + " with primary generation OID : " + pRow.Field<string>("PRIMGENOID"));
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                processSuccess = false;
                _log.Error(exp.Message + " at " + exp.StackTrace);
            }
            finally             
            {
                if (pCursor != null && Marshal.IsComObject(pCursor)) { while (Marshal.ReleaseComObject(pCursor) > 0) { } }
                if (pQF != null && Marshal.IsComObject(pQF)) { while (Marshal.ReleaseComObject(pQF) > 0) { } }            
            }
            return processSuccess;
        }

        public bool ReleaseStaticVariables()
        {
            bool bSuccess = true;
            try
            {
                if (listOfOidsProcessed != null)
                {
                    listOfOidsProcessed.Clear();
                    listOfOidsProcessed = null;

                }

                if (listOfSLsProcessed != null)
                {
                    listOfSLsProcessed.Clear();
                    listOfSLsProcessed = null;

                }
                if (listOfSessionsCreated != null)
                {
                    listOfSessionsCreated.Clear();
                    listOfSessionsCreated = null;

                }

                if (listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes != null)
                {
                    listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes.Clear();
                    listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes = null;

                }

                if (listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes != null)
                {
                    listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes.Clear();
                    listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes = null;

                }

                if (sessionVsPrimGenerationAndServiceLocUpdated != null)
                {
                    sessionVsPrimGenerationAndServiceLocUpdated.Clear();
                    sessionVsPrimGenerationAndServiceLocUpdated = null;

                }              
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);
                bSuccess = false;
            }
            return bSuccess;
        }

        public bool InitializeStaticVariables()
        {
            bool bSuccess = true;
            try
            {
                if (listOfOidsProcessed == null)
                {
                    
                    listOfOidsProcessed = new List<int>();

                }

                if (listOfSLsProcessed == null)
                {
                    
                    listOfSLsProcessed = new List<string>();

                }
                if (listOfSessionsCreated == null)
                {
                   
                    listOfSessionsCreated = new List<string>();

                }

                if (listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes == null)
                {
                   
                    listOfPrimaryGenerationsNotConnectedtoSpecifiedConductorTypes = new List<string>();

                }

                if (listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes == null)
                {
                  
                    listOfPrimaryGenerationsConnectedtoSpecifiedConductorTypes = new List<string>();

                }

                if (sessionVsPrimGenerationAndServiceLocUpdated == null)
                {
                   
                    sessionVsPrimGenerationAndServiceLocUpdated = new DataTable();

                }
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);
                bSuccess = false;
            }
            return bSuccess;
        }

        public bool CheckServiceLocationIsAlreadyReplaced(int argintServiceLocOID)
        {
            bool ServiceLocationAlreadyReplaced = false;
            try
            {
                if (dtRecordsAlreadyReplaced.Select("OBJECTID_SERVLOC=" + argintServiceLocOID).Length > 0 || sessionVsPrimGenerationAndServiceLocUpdated.Select("SLOID=" + argintServiceLocOID).Length > 0)
                {
                    ServiceLocationAlreadyReplaced = true;
                }
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);              
            }
            return ServiceLocationAlreadyReplaced;
        }
    }
}
