using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.NetworkAnalysis;
using System.Reflection;
using System.Data;
using System.Diagnostics;
using System.Threading;
using Oracle.DataAccess.Client;
using ESRI.ArcGIS.Geometry;
using System.IO;
using Miner.Geodatabase;
using System.Xml;
using PGE.Common.Delivery.Systems.Configuration;

namespace PGE.BatchApplication.PGE_SSD_Initialization
{
    public class PGE_SSD_Initialization
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "CachedTracing.log4net.config");
        private List<IFeatureClass> SSDClasses = new List<IFeatureClass>();

        public const string CircuitID = "PGE_CIRCUITID";
        public const string SourceSideDeviceId = "PGE_SOURCESIDEDEVICEID";
        public const string ProtectiveSSD = "PGE_PROTECTIVESSD";
        public const string AutoProtectiveSSD = "PGE_AUTOPROTECTIVESSD";
        public const string OperatingNumber = "PGE_OPERATINGNUMBER";
        public const string SourceSideDevice = "PGE_SOURCESIDEDEVICE";

        public const string SourceSideDeviceGUId = "PGE_SSDGUID";                                                           // SSD GUID MODEL NAME

        IWorkspace EDWorkspace = null;
        private OracleConnectionControl EDoracleConnectionControl = new OracleConnectionControl(Common.EDOracleConnection);
        private static Dictionary<string, IGeometricNetwork> geomNetworks = new Dictionary<string, IGeometricNetwork>();
        IGeometricNetwork subGeomNetwork = null;
        IGeometricNetwork elecGeomNetwork = null;

        public PGE_SSD_Initialization()
        {
            LoadSSDConfiguration();
        }

        public void ExecuteSSDInitilization(string EDConnectionFile, bool isParent)
        {
            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();

            if (!File.Exists(EDConnectionFile))
            {
                throw new Exception("Could not find ED SDE connection file: " + EDConnectionFile
                    + ": Please correct the configuration file");
            }

            _logger.Debug("Connecting to geodatabase using sde file: " + EDConnectionFile);
            if (isParent) { Console.WriteLine("Connecting to geodatabase using sde file: " + EDConnectionFile); }
            EDWorkspace = wsFactory.OpenFromFile(EDConnectionFile, 0);

            _logger.Debug("Connected to geodatabase.");
            if (isParent) { Console.WriteLine("Connected to geodatabase."); }           

            InitializeFeederSpaces(isParent);

            if (geomNetworks.ContainsKey(Common.DistributionGeomNetwork))
            {
                elecGeomNetwork = geomNetworks[Common.DistributionGeomNetwork];
            }

            if (EDWorkspace != null && isParent)
            {
                EDoracleConnectionControl.ExecuteProcedure("EDGIS.TruncateSSDInitTable", new List<OracleParameter>());
                GetSSDClasses(elecGeomNetwork);

                _logger.Debug("Determining Circuits to process");
                Console.WriteLine("Determining Circuits to process");
                
                PrepareTracingProcessTable(elecGeomNetwork);

                ExecuteTracingInChildren();
            }
            else
            {
                try
                {
                    GetSSDClasses(elecGeomNetwork);

                    List<string> FeederIDsToProcess = new List<string>();

                    FeederIDsToProcess = EDoracleConnectionControl.GetNextCircuitToProcess(CircuitType.Distribution, Common.CircuitsPerRequest);

                    while (FeederIDsToProcess.Count > 0)
                    {
                        foreach (string FeederIDToProcess in FeederIDsToProcess)
                        {
                            if (!String.IsNullOrEmpty(FeederIDToProcess))
                            {
                                TraceForSSDs(elecGeomNetwork, FeederIDToProcess);                                
                            }
                        }

                        FeederIDsToProcess = EDoracleConnectionControl.GetNextCircuitToProcess(CircuitType.Distribution, Common.CircuitsPerRequest);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error during child processing. Message: " + ex.Message + " Stacktrace: " + ex.StackTrace);
                    throw ex;
                }
            }   
        }

        IMMFeederSpace distributionFeederSpace = null;
        double totalFeeders = 0;
        double totalFeedersProcessed = 0;

        /// <summary>
        /// Initialize the feeder spaces for both the substation and distribution networks
        /// </summary>
        private void InitializeFeederSpaces(bool isParent)
        {
            string message = string.Empty;
            IMMFeederExt feederExt = null;
            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);
            feederExt = obj as IMMFeederExt;

            _logger.Debug("Obtaining geometric networks from " + Common.EDSDEConnectionFile);
            if (isParent) { Console.WriteLine("Obtaining geometric networks from " + Common.EDSDEConnectionFile); }
            GetNetworks(EDWorkspace);
            foreach (KeyValuePair<string, IGeometricNetwork> kvp in geomNetworks)
            {
                if (kvp.Key != Common.DistributionGeomNetwork) { continue; }

                if (kvp.Key == Common.DistributionGeomNetwork)
                {
                    if (isParent) { Console.WriteLine("Initializing distribution feeder space"); }
                    _logger.Debug("Initializing distribution feeder space");

                    distributionFeederSpace = feederExt.get_FeederSpace(kvp.Value, false);
                    if (distributionFeederSpace == null)
                    {
                        throw new Exception("Unable to get distribution feeder space information from geometric network");
                    }
                }
            }
        }

        private string PrepareTracingProcessTable(IGeometricNetwork elecGeomNetwork)
        {
            string message = string.Empty;
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;
            IMMElectricNetworkTracing downstreamTracer = null;
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = null;
            IMMElectricTraceSettingsEx electricTraceSettings = null;
            IMMEnumFeederSource feederSources = null;
            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);

            try
            {
                //Console.WriteLine("Initializing feeder space");
                _logger.Debug("Initializing feeder space");

                feederExt = obj as IMMFeederExt;

                //Console.WriteLine("Finsihed initializing feeder space");
                _logger.Debug("Finished initializing feeder space");

                List<string> distributionFeedersToProcess = new List<string>();

                if (elecGeomNetwork != null)
                {
                    feederSpace = feederExt.get_FeederSpace(elecGeomNetwork, false);
                    feederSources = feederSpace.FeederSources;
                    feederSources.Reset();
                    while ((feederSource = feederSources.Next()) != null)
                    {
                        string feederID;
                        try
                        {
                            feederID = feederSource.FeederID.ToString();
                            distributionFeedersToProcess.Add(feederID);
                        }
                        catch (Exception ex) { }
                    }
                }

                totalFeeders += distributionFeedersToProcess.Count;

                EDoracleConnectionControl.InsertCircuits(distributionFeedersToProcess, CircuitType.Distribution);

                GC.Collect();

            }
            catch (Exception ex)
            {
                _logger.Error("Error gathering circuit ID list. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (feederExt != null) { while (Marshal.ReleaseComObject(feederExt) > 0);}
                if (feederSpace != null) { while (Marshal.ReleaseComObject(feederSpace) > 0);}
                if (feederSource != null) { while (Marshal.ReleaseComObject(feederSource) > 0);}
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0);}
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0);}
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0);}
            }

            return message;
        }

        private List<Process> childProcessesRunning = new List<Process>();
        private void ExecuteTracingInChildren()
        {
            for (int i = 0; i < Common.numProcessors; i++)
            {
                string AssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                Process myProcess = null;
                string arguments = "-c";

                AssemblyLocation = AssemblyLocation.Substring(0, AssemblyLocation.LastIndexOf("\\") + 1) + "PGE.BatchApplication.PGE_SSD_Initialization.exe";
                ProcessStartInfo startInfo = new ProcessStartInfo(AssemblyLocation, arguments)
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                };

                try
                {
                    //_logger.Debug("Spawning new child process.  Arguments: " + startInfo.Arguments);
                    myProcess = new Process();
                    myProcess.StartInfo = startInfo;
                    myProcess.EnableRaisingEvents = true;
                    myProcess.Exited += new EventHandler(Process_Exited);
                    myProcess.Start();                    

                    //Place our processes in our list so we can monitor the progress of each
                    childProcessesRunning.Add(myProcess);
                }
                catch (Exception ex)//if exception occurred, does the control table need to be cleaned?
                {
                    _logger.Error("Error creating Process. ", ex);
                }
            }

            bool showPercentage = true;
            int cursorPos = 0;
            int windowPos = 0;
            try
            {
                cursorPos = Console.CursorLeft;
                windowPos = Console.CursorTop;
            }
            catch { showPercentage = false; }

            while (ChildrenStillProcessing())
            {
                if (showPercentage)
                {
                    try
                    {
                        Console.CursorLeft = cursorPos;
                        Console.CursorTop = windowPos;
                        double currentlyProcessed = EDoracleConnectionControl.GetCircuitsTraced().Count;
                        Console.WriteLine("Percent Complete: " + (Math.Round(currentlyProcessed / totalFeeders, 2) * 100.0) + "%");
                    }
                    catch { showPercentage = false; }
                }
                Thread.Sleep(3000);
            }

        }

        System.Object childProcsRunningLock = new System.Object();
        void Process_Exited(object sender, EventArgs e)
        {
            Process p = sender as Process;

            if (p.ExitCode != 0)
            {
                _logger.Error("Error in child processing. Killing all child processes");
                lock (childProcsRunningLock)
                {
                    foreach (Process process in childProcessesRunning)
                    {
                        if (process != null && !process.HasExited)
                        {
                            process.Kill();
                        }
                    }
                }
                _logger.Error("Error in child process. Refer to log files for information");
                Environment.Exit(1);
            }

            lock (childProcsRunningLock) { childProcessesRunning.Remove(p); }
        }

        private bool ChildrenStillProcessing()
        {
            bool processesStillRunning = false;

            try
            {
                if (childProcessesRunning.Count > 0)
                {
                    processesStillRunning = true;
                }

                bool circuitsStillToProcess = true;
                if (EDoracleConnectionControl.GetCircuitsTraced().Count == totalFeeders)
                {
                    circuitsStillToProcess = false;
                }

                if (!processesStillRunning && !circuitsStillToProcess) { return false; }
                else if (!processesStillRunning && circuitsStillToProcess)
                {
                    throw new Exception("Child processes are no longer running, but there are still circuits to process");
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                try
                {
                    foreach (Process p in childProcessesRunning)
                    {
                        if (!p.HasExited) { p.Kill(); }
                    }
                }
                catch { }

                throw e;
            }
        }

        /// <summary>
        /// This method will return the collection of netwotrks from the specified workspace
        /// </summary>
        /// <param name="networkName"></param>
        /// <returns></returns>
        private static void GetNetworks(IWorkspace ws)
        {
            if (geomNetworks.ContainsKey(Common.DistributionGeomNetwork)) { return; }
            if (geomNetworks == null) { geomNetworks = new Dictionary<string, IGeometricNetwork>(); }
            IGeometricNetwork geomNetwork = null;
            IEnumDataset enumDataset = ws.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            enumDataset.Reset();

            IDataset dsName = enumDataset.Next();
            while (dsName != null)
            {
                IFeatureDataset featureDataset = dsName as IFeatureDataset;
                if (featureDataset != null)
                {
                    IEnumDataset geomDatasets = featureDataset.Subsets;
                    geomDatasets.Reset();
                    IDataset ds = null;
                    while ((ds = geomDatasets.Next()) != null)
                    {
                        if (ds is IGeometricNetwork)
                        {
                            geomNetworks.Add(ds.BrowseName.ToUpper(), ds as IGeometricNetwork);
                        }
                    }
                }
                dsName = enumDataset.Next();
            }

            if (geomNetworks.Count > 0)
            {
                return;
            }
            else
            {
                throw new Exception("Could not find the geometric networks in the specified database");
            }
        }

        Dictionary<int, string> ClassIDToClassNameLookup = new Dictionary<int, string>();
        private void TraceForSSDs(IGeometricNetwork geomNetwork, string feederID)
        {
            string message = string.Empty;
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;
            IMMElectricNetworkTracing downstreamTracer = null;
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = null;
            IMMElectricTraceSettingsEx electricTraceSettings = null;

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);

            try
            {
                //Console.WriteLine("Initializing feeder space");
                //this.Log.Info("Initializing feeder space");

                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                INetwork network = geomNetwork.Network;
                INetSchema networkSchema = network as INetSchema;
                INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
                downstreamTracer = new MMFeederTracerClass();

                electricTraceSettings = new MMElectricTraceSettingsClass();
                electricTraceSettings.RespectESRIBarriers = true;
                electricTraceSettings.UseFeederManagerCircuitSources = true;
                electricTraceSettings.UseFeederManagerProtectiveDevices = true;

                networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
                networkAnalysisExtForFramework.DrawComplex = true;
                networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;

                //Phases to trace.
                mmPhasesToTrace phasesToTrace = mmPhasesToTrace.mmPTT_Any;
                IMMEnumFeederSource feederSources = feederSpace.FeederSources;
                feederSources.Reset();

                feederSource = feederSources.get_FeederSourceByFeederID(feederID);
                if ((feederSource) != null)
                {
                    feederID = feederSource.FeederID.ToString();

                    _logger.Info("Processing Circuit ID: " + feederID);

                    Dictionary<int, object> SSDs = new Dictionary<int, object>();
                    Dictionary<int, object> ProtectiveSSDs = new Dictionary<int, object>();
                    Dictionary<int, object> AutoProtectiveSSDs = new Dictionary<int, object>();
                    _logger.Info("Determining SSDs on this circuit");
                    GetSSDLists(ref SSDs, ref ProtectiveSSDs, ref AutoProtectiveSSDs, feederID);

                    List<int> SSDEids = new List<int>();
                    SSDEids.AddRange(SSDs.Keys);

                    List<int> ProtectiveSSDEids = new List<int>();
                    ProtectiveSSDEids.AddRange(ProtectiveSSDs.Keys);

                    List<int> AutoProtectiveSSDEids = new List<int>();
                    AutoProtectiveSSDEids.AddRange(AutoProtectiveSSDs.Keys);

                    DateTime startTime = DateTime.Now;
                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    _logger.Info("Tracing circuit: " + feederSource.FeederID);

                    //Trace
                    downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, feederSource.JunctionEID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    //this.Log.Info("Finished Tracing circuit: " + feederSource.FeederID);

                    List<int> edgesList = new List<int>();
                    List<int> junctionsList = new List<int>();
                    junctions.Reset();
                    edges.Reset();
                    int junction = -1;
                    while ((junction = junctions.Next()) > 0)
                    {
                        junctionsList.Add(junction);
                    }

                    int edge = -1;
                    while ((edge = edges.Next()) > 0)
                    {
                        edgesList.Add(edge);
                    }

                    List<int> visitedJunctions = new List<int>();
                    List<int> visitedEdges = new List<int>();
                    Dictionary<int, List<int>> SSDToJunctions = new Dictionary<int, List<int>>();

                    //Dictionary<int, List<int>> SSDToEdges = new Dictionary<int, List<int>>();
                    Dictionary<int, List<int>> ProtectiveSSDToJunctions = new Dictionary<int, List<int>>();
                    //Dictionary<int, List<int>> ProtectiveSSDToEdges = new Dictionary<int, List<int>>();
                    Dictionary<int, List<int>> AutoProtectiveSSDToJunctions = new Dictionary<int, List<int>>();
                    //Dictionary<int, List<int>> AutoProtectiveSSDToEdges = new Dictionary<int, List<int>>();

                    _logger.Info("Determining associated SSD for all junctions");
                    DetermineSourceSideDevices(ref visitedJunctions, ref visitedEdges, network, feederSource.JunctionEID,
                        ref edgesList, ref junctionsList, ref electricTraceWeight, -1, -1, -1, ref SSDToJunctions, ref ProtectiveSSDToJunctions,
                        ref AutoProtectiveSSDToJunctions, ref SSDEids, ref ProtectiveSSDEids, ref AutoProtectiveSSDEids);

                    //Now we have our SSD to junction mappings for all types.  Let's execute the sql queries for this one.
                    _logger.Info("Obtaining junction feature information");
                    IEIDHelper eidHelper = new EIDHelperClass();
                    eidHelper.GeometricNetwork = geomNetwork;
                    eidHelper.ReturnFeatures = true;
                    eidHelper.ReturnGeometries = false;
                    eidHelper.AddField("OBJECTID");
                    IEnumEIDInfo eidInfo = eidHelper.CreateEnumEIDInfo(junctions);
                    eidInfo.Reset();
                    IEIDInfo info = null;

                    //Modify to get list of Eid->ObjectIds and then build the where clauses with an in statement. Should be less loop iterations that way

                    Dictionary<IObjectClass, Dictionary<int, int>> ObjClassEidToOID = new Dictionary<IObjectClass, Dictionary<int, int>>();

                    List<string> updateStatements = new List<string>();
                    while ((info = eidInfo.Next()) != null)
                    {
                        try
                        {
                            ObjClassEidToOID[info.Feature.Class].Add(info.EID, info.Feature.OID);
                        }
                        catch
                        {
                            ObjClassEidToOID.Add(info.Feature.Class, new Dictionary<int, int>());
                            ObjClassEidToOID[info.Feature.Class].Add(info.EID, info.Feature.OID);
                        }
                    }
                    
                    SSDs.Add(-1, "");
                    ProtectiveSSDs.Add(-1, "");
                    AutoProtectiveSSDs.Add(-1, "");
                    _logger.Info("Determining sql update statements");
                    GetUpdateStatement(ref ObjClassEidToOID, ref SSDToJunctions, ref ProtectiveSSDToJunctions, ref AutoProtectiveSSDToJunctions,
                        ref SSDs, ref ProtectiveSSDs, ref AutoProtectiveSSDs, ref updateStatements);

                    _logger.Info("Executing update statements");
                    foreach (string updateStatement in updateStatements)
                    {
                        _logger.Info(updateStatement);
                        ((IDataset)geomNetwork).Workspace.ExecuteSQL(updateStatement);
                    }

                    EDoracleConnectionControl.UpdateCircuitStatus(feederID, "", CircuitStatus.Finished);
                    _logger.Info("Finished Processing Circuit ID: " + feederSource.FeederID);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
               // throw ex;
            }
            finally
            {
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0);}
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0);}
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0);}
                GC.Collect();
            }
        }


        private void GetUpdateStatement(ref Dictionary<IObjectClass, Dictionary<int, int>> ObjClassEidToOIDMap, ref Dictionary<int, List<int>> SSDsToJunctions,
            ref Dictionary<int, List<int>> ProtectiveSSDsToJunctions, ref Dictionary<int, List<int>> AutoProtectiveSSDsToJunctions,
            ref Dictionary<int, object> SSDsList, ref Dictionary<int, object> ProtectiveSSDsList, ref Dictionary<int, object> AutoProtectiveSSDsList,
            ref List<string> updateStatements)
        {
            string updateStmt = string.Empty;

            try
            {
                foreach (KeyValuePair<IObjectClass, Dictionary<int, int>> kvp in ObjClassEidToOIDMap)
                {
                    updateStmt = string.Empty;

                    string updateStmt1 = "UPDATE " + ((IDataset)kvp.Key).BrowseName;
                    string oidfieldName = kvp.Key.OIDFieldName;
                    string SSDFieldName = GetFieldName(kvp.Key, SourceSideDeviceId);

                    if (string.IsNullOrEmpty(SSDFieldName)) { continue; }

                    string SSDGUIDFieldName = GetFieldName(kvp.Key, SourceSideDeviceGUId);                                                      // GET THE SSD GUID FIELD

                    foreach (KeyValuePair<int, List<int>> kvp2 in SSDsToJunctions)
                    {
                        List<int> EIDsToProcess = kvp2.Value.Intersect(kvp.Value.Keys).ToList();
                        if (EIDsToProcess.Count < 1) { continue; }

                        string procValue = (string)SSDsList[kvp2.Key];
                        if (string.IsNullOrEmpty(procValue)) { continue; }

                        object operatingNumber = "";
                        object GUIDNumber = "";

                        string[] arrVal;
                        char splitChar = Convert.ToChar("$");
                        arrVal = procValue.Split(new[] { "$" }, StringSplitOptions.RemoveEmptyEntries);

                        if (!DBNull.Value.Equals(arrVal[0])) { operatingNumber = Convert.ToString(arrVal[0]); }
                        if (!DBNull.Value.Equals(arrVal[1])) { GUIDNumber = Convert.ToString(arrVal[1]); }

                        updateStmt = updateStmt1 + " SET " + SSDFieldName + " = '" + operatingNumber + "' WHERE " + oidfieldName + " in (";

                        string updateGUIDStmt = updateStmt1 + " SET " + SSDGUIDFieldName + " = '" + GUIDNumber + "' WHERE " + oidfieldName + " in (";

                        string origUpdateStmt = updateStmt;
                        string origGUIDUpdateStmt = updateGUIDStmt;

                        int counter = 0;
                        for (int i = 0; i < EIDsToProcess.Count; i++)
                        {
                            int oid = kvp.Value[EIDsToProcess[i]];
                            if ((i == EIDsToProcess.Count - 1) || counter == 999)
                            {
                                updateStmt += oid + ")";
                                updateGUIDStmt += oid + ")";

                                counter = 0;
                                updateStatements.Add(updateStmt);
                                updateStatements.Add(updateGUIDStmt);

                                updateStmt = origUpdateStmt;
                                updateGUIDStmt = origGUIDUpdateStmt;
                            }
                            else { updateStmt += oid + ","; updateGUIDStmt += oid + ","; }
                            counter++;
                        }

                    }

                    SSDFieldName = GetFieldName(kvp.Key, ProtectiveSSD);
                    foreach (KeyValuePair<int, List<int>> kvp2 in ProtectiveSSDsToJunctions)
                    {
                        updateStmt = string.Empty;

                        List<int> EIDsToProcess = kvp2.Value.Intersect(kvp.Value.Keys).ToList();
                        if (EIDsToProcess.Count < 1) { continue; }

                        string procValue = (string)ProtectiveSSDsList[kvp2.Key];
                        if (string.IsNullOrEmpty(procValue)) { continue; }


                        //object operatingNumber = ProtectiveSSDsList[kvp2.Key];

                        object operatingNumber = "";

                        string[] arrVal;
                        char splitChar = Convert.ToChar("$");
                        arrVal = procValue.Split(new[] { "$" }, StringSplitOptions.RemoveEmptyEntries);

                        if (!DBNull.Value.Equals(arrVal[0])) { operatingNumber = Convert.ToString(arrVal[0]); }

                        updateStmt = updateStmt1 + " SET " + SSDFieldName + " = '" + operatingNumber + "' WHERE " + oidfieldName + " in (";
                        string origUpdateStmt = updateStmt;
                        int counter = 0;
                        for (int i = 0; i < EIDsToProcess.Count; i++)
                        {
                            int oid = kvp.Value[EIDsToProcess[i]];
                            if ((i == EIDsToProcess.Count - 1) || counter == 999)
                            {
                                updateStmt += oid + ")";
                                counter = 0;
                                updateStatements.Add(updateStmt);
                                updateStmt = origUpdateStmt;
                            }
                            else { updateStmt += oid + ","; }
                            counter++;
                        }
                    }

                    SSDFieldName = GetFieldName(kvp.Key, AutoProtectiveSSD);
                    foreach (KeyValuePair<int, List<int>> kvp2 in AutoProtectiveSSDsToJunctions)
                    {
                        updateStmt = string.Empty;

                        List<int> EIDsToProcess = kvp2.Value.Intersect(kvp.Value.Keys).ToList();
                        if (EIDsToProcess.Count < 1) { continue; }

                        string procValue = (string)AutoProtectiveSSDsList[kvp2.Key];
                        if (string.IsNullOrEmpty(procValue)) { continue; }

                        //object operatingNumber = AutoProtectiveSSDsList[kvp2.Key];

                        object operatingNumber = "";

                        string[] arrVal;
                        char splitChar = Convert.ToChar("$");
                        arrVal = procValue.Split(new[] { "$" }, StringSplitOptions.RemoveEmptyEntries);

                        if (!DBNull.Value.Equals(arrVal[0])) { operatingNumber = Convert.ToString(arrVal[0]); }

                        updateStmt = updateStmt1 + " SET " + SSDFieldName + " = '" + operatingNumber + "' WHERE " + oidfieldName + " in (";

                        string origUpdateStmt = updateStmt;
                        int counter = 0;
                        for (int i = 0; i < EIDsToProcess.Count; i++)
                        {
                            int oid = kvp.Value[EIDsToProcess[i]];
                            if ((i == EIDsToProcess.Count - 1) || counter == 999)
                            {
                                updateStmt += oid + ")";
                                counter = 0;
                                updateStatements.Add(updateStmt);
                                updateStmt = origUpdateStmt;
                            }
                            else { updateStmt += oid + ","; }
                            counter++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Generating Update Statement: (" + updateStmt +  ")" +ex.Message + " StackTrace: " + ex.StackTrace);
                //throw ex;
            }
        }

        Dictionary<int, Dictionary<string, string>> ClassIDToModelNameToFieldName = new Dictionary<int, Dictionary<string, string>>();
        private string GetFieldName(IObjectClass objclass, string modelName)
        {
            try
            {
                return ClassIDToModelNameToFieldName[objclass.ObjectClassID][modelName];
            }
            catch
            {
                string fieldName = "";
                IField SSDField = ModelNameManager.Instance.FieldFromModelName(objclass, modelName);
                if (SSDField != null)
                {
                    fieldName = SSDField.Name;
                    if (!ClassIDToModelNameToFieldName.ContainsKey(objclass.ObjectClassID))
                    {
                        ClassIDToModelNameToFieldName.Add(objclass.ObjectClassID, new Dictionary<string, string>());
                    }
                    ClassIDToModelNameToFieldName[objclass.ObjectClassID].Add(modelName, fieldName);
                }
                return fieldName;
            }
        }

        private void DetermineSourceSideDevices(ref List<int> visitedJunctions, ref List<int> visitedEdges, INetwork networkDataset,
            int startingEID, ref List<int> edgeEIDs, ref List<int> junctionEIDs, ref INetWeight electricTraceWeight, int sourceSideDeviceEID,
            int protectiveSSDEid, int AutoProtectiveSSDEid, ref Dictionary<int, List<int>> SSDToJunctions,
            ref Dictionary<int, List<int>> protectiveSSDToJunctions,
            ref Dictionary<int, List<int>> autoProtectiveSSDToJunctions,
            ref List<int> sourceSideDeviceEIDs, ref List<int> protectiveSSDEids, ref List<int> autoProtectiveSSDEids)
        {
            if (visitedJunctions.Contains(startingEID)) { return; }
            visitedJunctions.Add(startingEID);

            AddSSDToList(ref SSDToJunctions, sourceSideDeviceEID, startingEID);
            AddSSDToList(ref protectiveSSDToJunctions, protectiveSSDEid, startingEID);
            AddSSDToList(ref autoProtectiveSSDToJunctions, AutoProtectiveSSDEid, startingEID);

            //Is this starting eid a source side device?
            if (sourceSideDeviceEIDs.Contains(startingEID)) { sourceSideDeviceEID = startingEID; }
            if (protectiveSSDEids.Contains(startingEID)) { protectiveSSDEid = startingEID; }
            if (autoProtectiveSSDEids.Contains(startingEID)) { AutoProtectiveSSDEid = startingEID; }

            IForwardStar forwardStar = networkDataset.CreateForwardStar(false, electricTraceWeight, null, null, null);

            try
            {
                int edgeCount = 0;
                forwardStar.FindAdjacent(0, startingEID, out edgeCount);

                for (int i = 0; i < edgeCount; i++)
                {
                    int adjacentEdgeEID = -1;
                    bool reverseOrientation = false;
                    object edgeWeightValue = null;
                    forwardStar.QueryAdjacentEdge(i, out adjacentEdgeEID, out reverseOrientation, out edgeWeightValue);

                    if (edgeEIDs.Contains(adjacentEdgeEID) && !visitedEdges.Contains(adjacentEdgeEID))
                    {
                        visitedEdges.Add(adjacentEdgeEID);

                        //Find the adjacent junction for this edge and add a new row
                        int adjacentJunctionEID = -1;
                        object junctionWeightValue = null;
                        forwardStar.QueryAdjacentJunction(i, out adjacentJunctionEID, out junctionWeightValue);

                        if (junctionEIDs.Contains(adjacentJunctionEID))
                        {
                            if (JunctionIsClosed(junctionWeightValue))
                            {
                                DetermineSourceSideDevices(ref visitedJunctions, ref visitedEdges, networkDataset, adjacentJunctionEID,
                                    ref edgeEIDs, ref junctionEIDs, ref electricTraceWeight, sourceSideDeviceEID, protectiveSSDEid, AutoProtectiveSSDEid,
                                    ref SSDToJunctions, ref protectiveSSDToJunctions, ref autoProtectiveSSDToJunctions,
                                    ref sourceSideDeviceEIDs, ref protectiveSSDEids, ref autoProtectiveSSDEids);
                            }
                            else
                            {
                                AddSSDToList(ref SSDToJunctions, sourceSideDeviceEID, adjacentJunctionEID);
                                AddSSDToList(ref protectiveSSDToJunctions, protectiveSSDEid, adjacentJunctionEID);
                                AddSSDToList(ref autoProtectiveSSDToJunctions, AutoProtectiveSSDEid, adjacentJunctionEID);
                            }
                            //AddSSDToList(ref SSDToEdges, sourceSideDeviceEID, adjacentEdgeEID);
                            //AddSSDToList(ref protectiveSSDToEdges, protectiveSSDEid, adjacentEdgeEID);
                            //AddSSDToList(ref autoProtectiveSSDToEdges, AutoProtectiveSSDEid, adjacentEdgeEID);
                        }
                        else
                        {
                            //AddSSDToList(ref SSDToEdges, sourceSideDeviceEID, adjacentEdgeEID);
                            //AddSSDToList(ref protectiveSSDToEdges, protectiveSSDEid, adjacentEdgeEID);
                            //AddSSDToList(ref autoProtectiveSSDToEdges, AutoProtectiveSSDEid, adjacentEdgeEID);
                        }
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (forwardStar != null) { while (Marshal.ReleaseComObject(forwardStar) > 0);}
            }
        }

        private bool JunctionIsClosed(object electricTraceWeightObject)
        {
            int electricTraceWeight = -1;
            Int32.TryParse(electricTraceWeightObject.ToString(), out electricTraceWeight);

            //If the trace weight is null or less than 0 assume we can trace through
            if (electricTraceWeight < 0) { return true; }

            //Determine the operational phases.  Values of 0 indicate the device is operational on a phase
            bool cPhase = (electricTraceWeight & 32) != 32;
            bool bPhase = (electricTraceWeight & 16) != 16;
            bool aPhase = (electricTraceWeight & 8) != 8;

            //If all phases are showing as not operational (i.e. Null phase designation), we will go ahead and trace through the device
            //as this appears to be how ArcFM handles this scenario as well.
            if ((!aPhase) && (!bPhase) && (!cPhase)) { return true; }

            //Determine open states.  Values of 0 indicate the device is closed on a phase
            bool cPhaseOpen = (electricTraceWeight & 1073741824) != 1073741824;
            bool bPhaseOpen = (electricTraceWeight & 536870912) != 536870912;
            bool aPhaseOpen = (electricTraceWeight & 268435456) != 268435456;

            bool traceThrough = false;
            if (aPhase && aPhaseOpen) { traceThrough = true; }
            else if (bPhase && bPhaseOpen) { traceThrough = true; }
            else if (cPhase && cPhaseOpen) { traceThrough = true; }

            return traceThrough;
        }

        private void AddSSDToList(ref Dictionary<int, List<int>> SSDList, int junctionSSDEID, int otherEID)
        {
            try
            {
                SSDList[junctionSSDEID].Add(otherEID);
            }
            catch
            {
                SSDList.Add(junctionSSDEID, new List<int>());
                SSDList[junctionSSDEID].Add(otherEID);
            }
        }

        /// <summary>
        /// This method will determine all of the features that represent SSDs, Protective SSDs, and Auto Protective SSDS. It provides this
        /// information as ObjectClassId->EID->OID
        /// </summary>
        /// <param name="SSDs">Dictionary of SSDs by object class ID -> EID -> OperatingNumber</param>
        /// <param name="ProtectiveSSDs">Dictionary of Protective SSDs by object class ID -> EID -> OperatingNumber</param>
        /// <param name="AutoProtectiveSSDs">Dictionary of Auto Protective SSDs by object class ID -> EID -> OperatingNumber</param>
        private void GetSSDLists(ref Dictionary<int, object> SSDs, ref Dictionary<int, object> ProtectiveSSDs,
            ref Dictionary<int, object> AutoProtectiveSSDs, string circuitID)
        {
            foreach (IFeatureClass featClass in SSDClasses)
            {
                if (!ClassIDToClassNameLookup.ContainsKey(featClass.ObjectClassID))
                {
                    ClassIDToClassNameLookup.Add(featClass.ObjectClassID, ((IDataset)featClass).BrowseName);
                }
                string circuitIDFieldName = GetFieldName(featClass, CircuitID);
                //Get SSDs
                if (SSDClassIDToWhereClause.ContainsKey(featClass.ObjectClassID))
                {
                    string whereClause = circuitIDFieldName + " = '" + circuitID + "'";
                    if (!string.IsNullOrEmpty(SSDClassIDToWhereClause[featClass.ObjectClassID])) { whereClause += " AND (" + SSDClassIDToWhereClause[featClass.ObjectClassID] + ")"; }
                    QueryTableForSSDs(featClass, whereClause, ref SSDs);
                }
                //Get Protective SSDs
                if (ProtectiveSSDClassIDToWhereClause.ContainsKey(featClass.ObjectClassID))
                {
                    string whereClause = circuitIDFieldName + " = '" + circuitID + "'";
                    if (!string.IsNullOrEmpty(ProtectiveSSDClassIDToWhereClause[featClass.ObjectClassID])) { whereClause += " AND (" + ProtectiveSSDClassIDToWhereClause[featClass.ObjectClassID] + ")"; }
                    QueryTableForSSDs(featClass, whereClause, ref ProtectiveSSDs);
                }
                //Get Auto Protective SSDs
                if (AutoProtectiveSSDClassIDToWhereClause.ContainsKey(featClass.ObjectClassID))
                {
                    string whereClause = circuitIDFieldName + " = '" + circuitID + "'";
                    if (!string.IsNullOrEmpty(AutoProtectiveSSDClassIDToWhereClause[featClass.ObjectClassID])) { whereClause += " AND (" + AutoProtectiveSSDClassIDToWhereClause[featClass.ObjectClassID] + ")"; }
                    QueryTableForSSDs(featClass, whereClause, ref AutoProtectiveSSDs);
                }
            }
        }

        /// <summary>
        /// This method will determine the feature classes that have the SSD model name assigned and the feature classes
        /// that have the SSD field model names assigned. It will then prompt the user to continue based on this information.
        /// </summary>
        /// <param name="geomNetwork">IGeometricNetwork that will have the SSD information initialized</param>
        /// <returns></returns>
        private void GetSSDClasses(IGeometricNetwork geomNetwork)
        {
            SSDClasses.Clear();

            List<string> SSDFeatureClasses = new List<string>();
            List<string> SSDToPopulateFeatureClasses = new List<string>();
           
            IEnumFeatureClass featureClasses = geomNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleJunction);
            featureClasses.Reset();
            IFeatureClass featClass = null;
            while ((featClass = featureClasses.Next()) != null)
            {
                //Check if this feature class is considered a source side device.
                bool isSSD = ModelNameManager.Instance.ContainsClassModelName(featClass, SourceSideDevice);

                if (isSSD)
                {
                    SSDClasses.Add(featClass);
                    SSDFeatureClasses.Add((((IDataset)featClass).BrowseName));
                    //if (string.IsNullOrEmpty(SSDFeatureClasses)) { SSDFeatureClasses += (((IDataset)featClass).BrowseName); }
                    //else { SSDFeatureClasses += "\r\n" + (((IDataset)featClass).BrowseName); }
                }

                //Check if this feature class has the SSD fields to be populated
                bool hasSSDId = ModelNameManager.Instance.FieldFromModelName(featClass, SourceSideDeviceId) != null;
                bool hasProtectiveSSDId = ModelNameManager.Instance.FieldFromModelName(featClass, ProtectiveSSD) != null;
                bool hasAutoProtectiveSSDId = ModelNameManager.Instance.FieldFromModelName(featClass, AutoProtectiveSSD) != null;

                bool hasSSDGUId = ModelNameManager.Instance.FieldFromModelName(featClass, SourceSideDeviceGUId) != null;  // CHECK FOR SSD GUID MODEL NAME

                if (hasSSDId && hasProtectiveSSDId && hasAutoProtectiveSSDId && hasSSDGUId)
                {
                    SSDToPopulateFeatureClasses.Add((((IDataset)featClass).BrowseName));
                    //if (string.IsNullOrEmpty(SSDToPopulateFeatureClasses)) { SSDToPopulateFeatureClasses += (((IDataset)featClass).BrowseName); }
                    //else { SSDToPopulateFeatureClasses += "\r\n" + (((IDataset)featClass).BrowseName); }
                }
            }

            SSDFeatureClasses.Sort();
            SSDToPopulateFeatureClasses.Sort();

            string SSDFeatClassPrompt = "";
            foreach (string featureClass in SSDFeatureClasses)
            {
                if (string.IsNullOrEmpty(SSDFeatClassPrompt)) { SSDFeatClassPrompt += featureClass; }
                else { SSDFeatClassPrompt += "\r\n" + featureClass; }
            }
            string SSDToPopulateFeatClassPrompt = "";
            foreach (string featureClass in SSDToPopulateFeatureClasses)
            {
                if (string.IsNullOrEmpty(SSDToPopulateFeatClassPrompt)) { SSDToPopulateFeatClassPrompt += featureClass; }
                else { SSDToPopulateFeatClassPrompt += "\r\n" + featureClass; }
            }

            _logger.Info("The following feature classes are configured as source side devices\r\n" + SSDFeatClassPrompt +
                "\r\n\r\n The following feature classes will have their source side device fields populated\r\n" + SSDToPopulateFeatClassPrompt);
        }

        private void QueryTableForSSDs(IFeatureClass featClass, string whereClause, ref Dictionary<int, object> ToUpdate)
        {
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = whereClause;
            IFeatureCursor featCursor = featClass.Search(qf, false);
            IFeature feat = null;

            IField opNumField = ModelNameManager.Instance.FieldFromModelName(featClass, OperatingNumber);
            int opNumFieldIdx = featClass.FindField(opNumField.Name);

            int GUIDFieldIdx = featClass.FindField("GLOBALID");

            qf.AddField(opNumField.Name);
            if (featClass.FeatureType == esriFeatureType.esriFTSimpleJunction)
            {
                while ((feat = featCursor.NextFeature()) != null)
                {
                   // ToUpdate.Add(((ISimpleJunctionFeature)feat).EID, feat.get_Value(opNumFieldIdx));
                    
                    string dataToUpdate = (object) feat.get_Value(opNumFieldIdx) + "$" + feat.get_Value(GUIDFieldIdx);
                    ToUpdate.Add(((ISimpleJunctionFeature)feat).EID, dataToUpdate);
                }
            }
            else if (featClass.FeatureType == esriFeatureType.esriFTSimpleEdge)
            {
                while ((feat = featCursor.NextFeature()) != null)
                {
                    object updateValue = (object) feat.get_Value(opNumFieldIdx) + (string) feat.get_Value(feat.Fields.FindField("GLOBALID"));
                    ToUpdate.Add(((ISimpleJunctionFeature)feat).EID, updateValue);
                    
                    //ToUpdate.Add(((ISimpleJunctionFeature)feat).EID, (object) feat.get_Value(opNumFieldIdx));
                }
            }
        }

        bool initializedSSDConfiguration = false;
        Dictionary<int, string> SSDClassIDToWhereClause = new Dictionary<int, string>();
        Dictionary<int, string> ProtectiveSSDClassIDToWhereClause = new Dictionary<int, string>();
        Dictionary<int, string> AutoProtectiveSSDClassIDToWhereClause = new Dictionary<int, string>();
        private void LoadSSDConfiguration()
        {
            if (!initializedSSDConfiguration)
            {
                initializedSSDConfiguration = true;
                //Read the config location from the Registry Entry
                SystemRegistry sysRegistry = new SystemRegistry("PGE");
                string SSDConfigFilePath = sysRegistry.ConfigPath;
                //prepare the xmlfile if config path exist
                string SSDConfigXmlFilePath = System.IO.Path.Combine(SSDConfigFilePath, "PGESSDConfig.xml");

                if (!System.IO.File.Exists(SSDConfigXmlFilePath))
                {
                    string assemblyLoc = Assembly.GetExecutingAssembly().Location;
                    SSDConfigXmlFilePath = System.IO.Path.Combine(assemblyLoc, "PGESSDConfig.xml");
                }

                _logger.Debug("Loading SSD configuration from " + SSDConfigXmlFilePath);

                try
                {
                    bool processingSSDs = false;
                    bool processingProtectiveSSDs = false;
                    bool processingAutoProtectiveSSDs = false;
                    int currentClassID = -1;
                    using (XmlReader reader = XmlReader.Create(new StreamReader(SSDConfigXmlFilePath)))
                    {
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    if (reader.Name.ToUpper() == "PROTECTIVESSDS")
                                    {
                                        processingAutoProtectiveSSDs = false;
                                        processingProtectiveSSDs = true;
                                        processingSSDs = false;
                                    }
                                    else if (reader.Name.ToUpper() == "AUTOPROTECTIVESSDS")
                                    {
                                        processingAutoProtectiveSSDs = true;
                                        processingProtectiveSSDs = false;
                                        processingSSDs = false;
                                    }
                                    else if (reader.Name.ToUpper() == "SSDS")
                                    {
                                        processingAutoProtectiveSSDs = false;
                                        processingProtectiveSSDs = false;
                                        processingSSDs = true;
                                    }
                                    else if (reader.Name.ToUpper() == "CLASS")
                                    {
                                        currentClassID = Int32.Parse(reader.GetAttribute("ClassID"));
                                        string whereClause = reader.GetAttribute("WhereClause");
                                        if (processingProtectiveSSDs)
                                        {
                                            ProtectiveSSDClassIDToWhereClause.Add(currentClassID, whereClause);
                                        }
                                        else if (processingAutoProtectiveSSDs)
                                        {
                                            AutoProtectiveSSDClassIDToWhereClause.Add(currentClassID, whereClause);
                                        }
                                        else if (processingSSDs)
                                        {
                                            SSDClassIDToWhereClause.Add(currentClassID, whereClause);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error loading SSD configuration: " + ex.Message);
                }
            }
        }

    }
}
