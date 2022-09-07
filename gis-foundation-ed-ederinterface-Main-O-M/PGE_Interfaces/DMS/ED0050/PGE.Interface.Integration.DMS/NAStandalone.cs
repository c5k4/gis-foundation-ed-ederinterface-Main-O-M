using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interface.Integration.DMS.Common;
using Miner.Geodatabase;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Configuration;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using System.Xml;
using System.Configuration;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;
using PGE.Interface.Integration.DMS.Manager;
using System.Data;
using PGE.Interface.Integration.DMS.Tracers;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using PGE_DBPasswordManagement;

namespace PGE.Interface.Integration.DMS
{
    public class NAStandalone
    {
        private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        private LicenseInitializer _AOLicenseInitializer;
        private IMMAppInitialize _MMAppInitialize;
        private Exporter _exporter;
        private ScopeFactory _scopeFactory;
        private GeodatabaseAccess _geodatabaseAccess;
        private ElectricExportScope _electricExportScope;
        private bool _cancelled = false;
        private static bool _substation;
        public static string SDEelementConnection = default;

        public static bool Substation
        {
            get { return NAStandalone._substation; }
        }
        public ElectricExportScope ElectricExportScope
        {
            get { return _electricExportScope; }
            set { _electricExportScope = value; }
        }
        private NetworkAdapterSettingsElement _NAsettings;
        public NetworkAdapterSettingsElement NAsettings
        {
            get { return _NAsettings; }
            set { _NAsettings = value; }
        }

        public NAStandalone()
        {

        }
        public void Execute(ExtractorMessage msg, bool isSubstation, bool processRetryCircuits)
        {
            // #16
            CADOPS.IsSecondAttempt = processRetryCircuits;
            ControlTable control = new ControlTable(PGE.Interface.Integration.DMS.Common.Configuration.CadopsConnection);
            _substation = isSubstation;

            //control.ClearStagingTable();
            //Debugger.Break();
            //ControlRecord record = control.GetProcessRecord(msg.ProcessID);
            int processed = 0;
            msg.ProcessID = Process.GetCurrentProcess().Id.ToString();
            try
            {

                bool enableSchemaCache = PGE.Interface.Integration.DMS.Common.Configuration.getBoolSetting("EnableSchemaCache", true);

                if (enableSchemaCache)
                {
                    _log4.Debug("Schema caching is currently enabled.");
                    //Enable schema caching for this workspace
                    IWorkspaceFactorySchemaCache workspaceFactorySchemaCache = new SdeWorkspaceFactory() as IWorkspaceFactorySchemaCache;
                    workspaceFactorySchemaCache.EnableSchemaCache(_electricExportScope.GdbAccess.Workspace);
                }
                else
                {
                    _log4.Debug("Schema caching is currently disabled.");
                }
                
                string location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string statusLogDirectory = location.Substring(0, location.LastIndexOf("\\") + 1) + "StatusLogs";

                _log4.Debug("Working with geometric network: " + ((IDataset)_electricExportScope.Sources.FeederSpace.GeometricNetwork).BrowseName);

                bool electricSuccess = true;
                int circuitsProcessed = 0;
                CADOPS.isSubstation = Substation;
                if (!Substation)
                {
                    string circuit = control.GetNextCircuitToProcess(System.Environment.MachineName, msg.ProcessID, false, processRetryCircuits);
                    while (!string.IsNullOrEmpty(circuit))
                    {
                        DateTime startTime = DateTime.Now;
                        try
                        {
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();
                            CADOPS.ErrorMessage = "";
                            _log4.Debug("Processing Circuit: " + circuit);
                            electricSuccess = ExportCircuit(circuit);

                            //The exception from our custom class doesn't fall through, so we need to check for any exceptions here
                            if (CADOPS.CadopsException != null) { throw CADOPS.CadopsException; }

                            if (electricSuccess)
                            {
                                processed++;
                                stopwatch.Stop();
                                string time = stopwatch.Elapsed.ToString();
                                _log4.Debug("Circuit " + circuit + " Processed. Total Time: " + time);
                                circuitsProcessed++;
                                string errorMessage = CADOPS.ErrorMessage;
                                if (errorMessage.Length > 4000) { errorMessage = "Errors were encountered during export. Please refer to log files to view errors."; }
                                control.UpdateCircuitsAsFinished(circuit, Math.Round((DateTime.Now - startTime).TotalMinutes, 1), CircuitStatus.Finished, errorMessage);
                            }
                            else
                            {
                                _log4.Debug("Circuit: " + circuit + " failed to export: " + CADOPS.ErrorMessage);
                                string errorMessage = CADOPS.ErrorMessage;
                                throw new Exception(errorMessage);
                                //if (errorMessage.Length > 4000) { errorMessage = "Errors were encountered during export. Please refer to log files to view errors."; }
                                //if (!processRetryCircuits) { control.UpdateCircuitsAsFinished(circuit, Convert.ToInt32((DateTime.Now - startTime).TotalMinutes), CircuitStatus.Retry, errorMessage); }
                                //else { control.UpdateCircuitsAsFinished(circuit, Math.Round((DateTime.Now - startTime).TotalMinutes, 1), CircuitStatus.Error, "Failed processing circuit. Refer to logs on server"); }
                            }
                        }
                        catch (Exception e)
                        {
                            string errorMessage = "Failed processing circuit. Refer to logs on server. " + e.Message;
                            if (errorMessage.Length > 4000) { errorMessage = "Errors were encountered during export. Please refer to log files to view errors."; }
                            if (!processRetryCircuits) { control.UpdateCircuitsAsFinished(circuit, Convert.ToInt32((DateTime.Now - startTime).TotalMinutes), CircuitStatus.Retry, errorMessage); }
                            else { control.UpdateCircuitsAsFinished(circuit, Math.Round((DateTime.Now - startTime).TotalMinutes, 1), CircuitStatus.Error, errorMessage); }
                            throw e;
                        }
                        circuit = control.GetNextCircuitToProcess(System.Environment.MachineName, msg.ProcessID, false, processRetryCircuits);
                    }
                }

                if (Substation)
                {
                    List<String> substations = new List<string>();
                    if (substations != null)
                    {
                        string circuitToProcess = control.GetNextCircuitToProcess(System.Environment.MachineName, msg.ProcessID, true, processRetryCircuits);

                        while (!string.IsNullOrEmpty(circuitToProcess))
                        {
                            substations.Clear();
                            substations.Add(circuitToProcess);
                            Dictionary<string, List<string>> substationCircuitIDs = GetSubFeeders(substations);
                            foreach (KeyValuePair<string, List<string>> kvp in substationCircuitIDs)
                            {
                                DateTime startTime = DateTime.Now;
                                bool allSuccessful = true;
                                try
                                {
                                    int substationsProcessed = 0;
                                    foreach (String substationCircuit in kvp.Value)
                                    {
                                        Stopwatch stopwatch = new Stopwatch();
                                        stopwatch.Start();
                                        _log4.Debug("Processing Circuit: " + substationCircuit);
                                        bool success = ExportCircuit(substationCircuit);

                                        //The exception from our custom class doesn't fall through, so we need to check for any exceptions here
                                        if (CADOPS.CadopsException != null) { throw CADOPS.CadopsException; }

                                        if (!success)
                                        {
                                            allSuccessful = false;
                                            _log4.Debug("Substation: " + substationCircuit + " failed to export");
                                        }
                                        processed++;
                                        stopwatch.Stop();
                                        string time = stopwatch.Elapsed.ToString();
                                        _log4.Debug("Substation " + substationCircuit + " Processed. Total Time: " + time);
                                        substationsProcessed++;
                                    }
                                }
                                catch (Exception e)
                                {
                                    if (!processRetryCircuits) { control.UpdateCircuitsAsFinished(kvp.Key, Math.Round((DateTime.Now - startTime).TotalMinutes, 1), CircuitStatus.Retry, ""); }
                                    else { control.UpdateCircuitsAsFinished(kvp.Key, Math.Round((DateTime.Now - startTime).TotalMinutes, 1), CircuitStatus.Error, "Failed processing substation. Refer to logs on server. " + e.Message); }
                                    throw e;
                                }
                                if (allSuccessful) { control.UpdateCircuitsAsFinished(kvp.Key, Math.Round((DateTime.Now - startTime).TotalMinutes, 1), CircuitStatus.Finished, ""); }
                                else
                                {
                                    if (!processRetryCircuits) { control.UpdateCircuitsAsFinished(kvp.Key, Convert.ToInt32((DateTime.Now - startTime).TotalMinutes), CircuitStatus.Retry, ""); }
                                    else { control.UpdateCircuitsAsFinished(kvp.Key, Math.Round((DateTime.Now - startTime).TotalMinutes, 1), CircuitStatus.Error, "Failed processing substation. Refer to logs on server. "); }
                                }
                            }
                            if (substationCircuitIDs.Count < 1)
                            {
                                //If count of substation circuit IDs was 0 then there was one missing and we need to update the circuit in our processing table.
                                if (!processRetryCircuits) { control.UpdateCircuitsAsFinished(circuitToProcess, 0, CircuitStatus.Retry, ""); }
                                else { control.UpdateCircuitsAsFinished(circuitToProcess, 0, CircuitStatus.Error, "Failed processing substation. Substation Circuit is missing. "); }
                            }
                            circuitToProcess = control.GetNextCircuitToProcess(System.Environment.MachineName, msg.ProcessID, true, processRetryCircuits);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log4.Debug("Error during processing.  Error: " + e.Message + " StackTrace: " + e.StackTrace);
            }
            finally
            {
            }
        }

        public string Concatenate(List<string> items, string seperator = "")
        {
            if (seperator == null) seperator = "";
            var builder = new StringBuilder();
            builder.Append(items.FirstOrDefault() ?? "");
            foreach (var item in items.Skip(1))
            {
                builder.Append(seperator);
                builder.Append(item);
            }
            return builder.ToString();
        }

        public bool GetLicenses()
        {
            _log4.Debug("Checking out ArcInfo License");
            //Get esri license 
            _AOLicenseInitializer = new LicenseInitializer();
            if (_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[] { }) == false)
            {
                return false;
            }
            // get arcfm license
            _MMAppInitialize = new MMAppInitializeClass();
            if (_MMAppInitialize == null)
            {
                return false;
            }

            _log4.Debug("Checking out ArcFM License");
            mmLicenseStatus arcFMLicenseStatus = mmLicenseStatus.mmLicenseUnavailable;
            //Determine if the product is available
            arcFMLicenseStatus = _MMAppInitialize.IsProductCodeAvailable(mmLicensedProductCode.mmLPArcFM);
            if (arcFMLicenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                //Initialize the license
                arcFMLicenseStatus = _MMAppInitialize.Initialize(mmLicensedProductCode.mmLPArcFM);
            }
            if (arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
            {
                return false;
            }

            _log4.Debug("Licenses successfully checked out");
            return true;
        }

        public void ReleaseLicenses()
        {
            if (_AOLicenseInitializer != null)
            {
                _AOLicenseInitializer.ShutdownApplication();
            }
            if (_MMAppInitialize != null)
            {
                _MMAppInitialize.Shutdown();
            }
        }
        public static NetworkAdapterSettingsElement GetNetworkAdapterSettings(string exeConfigFilename, string settingsKey)
        {
            ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap();
            ecfm.ExeConfigFilename = exeConfigFilename;

            System.Configuration.Configuration config = null;
            try
            {
                config = ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
            }
            catch (Exception e)
            {
                throw e;
            }

            ConfigurationSection section = null;
            NetworkAdapterConfigurationSection NAConfigSection = null;
            string failMsg = "failed";
            try
            {
                section = config.GetSection("NetworkAdapterSection");
                NAConfigSection = (NetworkAdapterConfigurationSection)section;

                if (NAConfigSection != null)
                {
                    if (NAConfigSection.NetworkAdapterSettings.Count > 0)
                    {
                        if (settingsKey == null)
                        {
                            settingsKey = NAConfigSection.DefaultSettingsElement;
                        }

                        if (!string.IsNullOrEmpty(settingsKey))
                        {
                            if (!NAConfigSection.NetworkAdapterSettings.Contains(settingsKey))
                            {
                                failMsg = string.Format(ecfm.ExeConfigFilename, Environment.NewLine, settingsKey);
                                throw new ConfigurationErrorsException(failMsg);
                            }

                            return NAConfigSection.NetworkAdapterSettings.Get(settingsKey);
                        }

                        //no default, so return the first
                        return NAConfigSection.NetworkAdapterSettings.Get(0);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }
        public bool InitializeNetworkAdapter(NetworkAdapterSettingsElement networkAdapterSettings)
        {
            string[] UserInst = SDEelementConnection.Split('@');
            _log4.Debug("Validating settings");
            networkAdapterSettings.ValidateSettings();
            _log4.Debug("Creating Exporter");
            _exporter = new Exporter(networkAdapterSettings);
            _log4.Debug("Getting scope factory");
            _scopeFactory = ScopeFactory.GetFactory();
            _log4.Debug("Creating electric export scope");
            _electricExportScope = _scopeFactory.CreateElectricExportScope(networkAdapterSettings, null);
            //_electricExportScope = new GenericExportScope(networkAdapterSettings);

            _log4.Debug("Connecting to geodatabase");

            // m4jf edgisrearch 919
            _exporter.Settings.SdeElement.Instance = "sde:oracle11g:/;local=" + UserInst[1].ToString();
            _exporter.Settings.SdeElement.Version = "SDE.DEFAULT";
            _exporter.Settings.SdeElement.User = UserInst[0].ToString();
            _exporter.Settings.SdeElement.Password = ReadEncryption.GetPassword(SDEelementConnection);
            _geodatabaseAccess = _electricExportScope.ConnectToGeodatabase();
            if (!_electricExportScope.IsGdbConnectionLive)
            {
                return false;
            }
            //ESRITrace trace = new ESRITrace(_geodatabaseAccess);
            //trace.OldTrace = _electricExportScope.Tracer;
            //_electricExportScope.Tracer = trace;
            //populate the scope groups on the same thread as the workspace
            _log4.Debug("Getting scope groups");
            ScopeGroups nestedSources = _electricExportScope.SelectionTree;

            return true;
        }
        private Dictionary<string, List<string>> GetSubFeeders(List<string> subIDs)
        {
#if DEBUG
            Debugger.Launch();
#endif
            Dictionary<string, List<string>> output = new Dictionary<string, List<string>>();
            foreach (string subID in subIDs)
            {
                List<string> substationCircuitIDs = new List<string>();
                IList<ScopeGroup> subfeeders = _electricExportScope.SelectionTree.FindByOwner(subID);
                if (subfeeders != null && subfeeders.Count > 0)
                {
                    foreach (ScopeGroup sg in subfeeders)
                    {
                        if (sg.Key == "0") 
                        { 
                            _log4.Warn("A substation is being returned with no EID and will be skipped: " + sg.Caption);
                            continue;
                        }
                        substationCircuitIDs.Add(sg.Caption);
                    }
                    output.Add(subID, substationCircuitIDs);
                }
                else
                {
                    sendMissingCircuit(subID);
                }
            }
            return output;
        }

        public bool ExportCircuit(string feederToExport)
        {
            // #17
            try
            {
                //string outputFile = Path.Combine(_feederFolder.FullName, feederToExport + ".xml");
                //if (File.Exists(outputFile))
                //{
                //    File.Delete(outputFile);
                //}

                // refresh version before each export of a feeder
                if (_electricExportScope.GdbAccess.Workspace is IVersion)
                {
                    IVersion vWorkspace = (IVersion)_electricExportScope.GdbAccess.Workspace;
                    vWorkspace.RefreshVersion();
                }
                var feederToSelect = _electricExportScope.SelectionTree.FindByCaption(feederToExport);
                if (feederToSelect == null)
                {
                    sendMissingCircuit(feederToExport);
                    return false;
                }

                _electricExportScope.ClearSelection();
                _electricExportScope.Select(feederToSelect.Key);
                //_electricExportScope[feederToSelect.Key].Selected = true;
                //_electricExportScope.RefreshSelection();

                _cancelled = false;
                XmlDocument output = _exporter.Export(_electricExportScope, out _cancelled);
                if (_cancelled == true)
                {
                    _log4.Error("Circuit: " + feederToExport + " export canceled.");
                    return false;
                }
                //output.Save(outputFile);
                _electricExportScope.ClearSelection();
                return true;
            }
            catch (Exception ex)
            {
                _log4.Error("Failed to export: " + feederToExport, ex);
                return false;
            }
        }


        private void BuildSpatialCache(ElectricExportScope electricScope, string sCircuitID)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                IWorkspace workspace = electricScope.GdbAccess.Workspace;
                IGeometricNetwork geomNetwork = electricScope.Sources.FeederSpace.GeometricNetwork;
                _log4.Debug(DateTime.Now + " Obtaining envelope for spatial cache for circuit: " + sCircuitID);
                IEnvelope m_pCacheEnv = GetCircuitEnv(sCircuitID, workspace, geomNetwork);
                _log4.Debug(DateTime.Now + " Finished obtaining envelope for spatial cache for circuit: " + sCircuitID);
                _log4.Debug(DateTime.Now + " Building spatial cache for circuit: " + sCircuitID);
                BuildCache(workspace, m_pCacheEnv);
                _log4.Debug(DateTime.Now + " Finishied building spatial cache for circuit: " + sCircuitID);
                DateTime stopTime = DateTime.Now;
                TimeSpan totalTime = stopTime.Subtract(startTime);
                _log4.Debug("Total time to build spatial cache: " + totalTime.Duration());
            }
            catch (Exception e)
            {
                _log4.Error("Error in building Cache: " + e.Message + " StackTrace: " + e.StackTrace);
            }
        }

        private IEnvelope GetCircuitEnv(string sFeederID1, IWorkspace pWKS, IGeometricNetwork geomNetwork)
        {
            IEnumFeatureClass pEFC;
            IFeatureClass pFC;
            IDataset pDS;
            IQueryFilter pQF;
            IFeatureCursor pFCur;
            IFeature pFeat;
            IEnvelope pRetEnv = null;
            int iCnt;

            try
            {
                for (int x = 0; x < 3; x++)
                {
                    if (x == 0)
                    {
                        pEFC = geomNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleJunction);
                    }
                    else if (x == 1)
                    {
                        pEFC = geomNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleEdge);
                    }
                    else if (x == 2)
                    {
                        pEFC = geomNetwork.get_ClassesByType(esriFeatureType.esriFTComplexEdge);
                    }
                    else
                    {
                        pEFC = null;
                    }

                    pFC = pEFC.Next();

                    iCnt = 0;

                    while (pFC != null)
                    {
                        pDS = pFC as IDataset;
                        if (pFC.Fields.FindField("CIRCUITID") > 0)
                        {
                            pQF = new QueryFilter();
                            pQF.WhereClause = "CIRCUITID='" + sFeederID1 + "'";
                            pQF.WhereClause = pQF.WhereClause + " or CIRCUITID2='" + sFeederID1 + "'";
                            pQF.SubFields = pFC.ShapeFieldName;
                            pFCur = pFC.Search(pQF, true);

                            pFeat = pFCur.NextFeature();
                            while (pFeat != null)
                            {
                                iCnt = iCnt + 1;
                                if (pRetEnv == null)
                                {
                                    pRetEnv = pFeat.Shape.Envelope;
                                }
                                else
                                {
                                    pRetEnv.Union(pFeat.Shape.Envelope);
                                }
                                pFeat = pFCur.NextFeature();
                            }
                        }
                        pFC = pEFC.Next();
                    }
                }

                if (pRetEnv == null) { }
                else
                {
                    pRetEnv.Expand(10.0, 10.0, false);
                }
            }
            catch (Exception ex)
            {
                _log4.Error("GetCircuitEnv:" + ex.Message + " StackTrace:" + ex.StackTrace);
            }
            return pRetEnv;
        }

        private void BuildCache(IWorkspace pWKS, IEnvelope pEnv)
        {
            ISpatialCacheManager3 pSC;
            //IEnumFeatureClass pEFC;

            //pEFC = geomNetwork.get_ClassesByType(ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTSimpleEdge);
            //pEFC = geomNetwork.get_ClassesByType(ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTSimpleJunction);
            //pEFC = geomNetwork.get_ClassesByType(ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTComplexEdge);

            try
            {
                if (pEnv == null) { }
                else
                {
                    pSC = pWKS as ISpatialCacheManager3;
                    pSC.EmptyCache();
                    pSC.FillCache(pEnv);
                }
            }
            catch (Exception e)
            {
                _log4.Error("BuildCache:" + e.Message + " :" + e.StackTrace);
            }
        }

        /// <summary>
        /// Adds a row to the EXPORTED staging table to flag the circuit as missing (most likely deleted)
        /// </summary>
        /// <param name="circuitID">The ID of the circuit</param>
        private void sendMissingCircuit(string circuitID)
        {
            _log4.Debug("Sending circuit: " + circuitID + " as missing");
            DataSet schema = Utilities.BuildDataSet();
            DataTable exported = schema.Tables["DMSSTAGING.EXPORTED"];
            DataRow row = exported.NewRow();
            row["EXPORT_ID"] = circuitID;
            if (NAStandalone.Substation)
            {
                row["EXPORT_TYPE"] = "S";
            }
            else
            {
                row["EXPORT_TYPE"] = "C";
            }
            row["DATE_PROCESSED"] = DateTime.Now;
            row["STATUS"] = 2; //set this missing
            row["MESSAGE"] = "Missing";
            exported.Rows.Add(row);

            Common.Oracle.BulkLoadData(schema, Common.Configuration.CadopsConnection);
        }
    }
}
