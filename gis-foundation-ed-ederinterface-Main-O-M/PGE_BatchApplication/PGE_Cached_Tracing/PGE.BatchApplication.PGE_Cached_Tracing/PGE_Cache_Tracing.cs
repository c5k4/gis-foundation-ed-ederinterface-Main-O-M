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

namespace PGE.BatchApplication.PGE_Cached_Tracing
{
    public class PGE_Cache_Tracing
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "CachedTracing.log4net.config");
        IWorkspace EDWorkspace = null;
        IWorkspace SUBWorkspace = null;
        Dictionary<string, List<string>> CircuitIDMap = new Dictionary<string, List<string>>();
        //Dictionary<int, string> EIDtoCircuitIDMap = new Dictionary<int, string>();
        Dictionary<int, string> SubSourceEIDtoCircuitIDMap = new Dictionary<int, string>();
        Dictionary<int, string> SubLoadEIDtoCircuitIDMap = new Dictionary<int, string>();
        Dictionary<int, string> EDSourceEIDtoCircuitIDMap = new Dictionary<int, string>();
        Dictionary<int, string> EDLoadEIDtoCircuitIDMap = new Dictionary<int, string>();
        //Dictionary<string, int> CircuitIDtoEIDMap = new Dictionary<string, int>();
        Dictionary<int, int> SubstationToDistributionEIDMap = new Dictionary<int, int>();
        Dictionary<int, int> DistributionToSubstationEIDMap = new Dictionary<int, int>();
        List<int> SubstationStitchPointsWithNoElecRel = new List<int>();
        private OracleConnectionControl EDoracleConnectionControl = new OracleConnectionControl(Common.EDOracleConnection);
        private OracleConnectionControl SUBoracleConnectionControl = new OracleConnectionControl(Common.SUBOracleConnection);
        private OracleConnectionControl SCHEMoracleConnectionControl = new OracleConnectionControl(Common.SCHEMOracleConnection);
        private static Dictionary<string, IGeometricNetwork> geomNetworks = new Dictionary<string, IGeometricNetwork>();
        private Dictionary<int, int> SUBJunctionClassIDs = new Dictionary<int, int>();
        private Dictionary<int, int> SUBEdgeClassIDs = new Dictionary<int, int>();
        private Dictionary<int, int> SUBJunctionOIDs = new Dictionary<int, int>();
        private Dictionary<int, int> SUBEdgeOIDs = new Dictionary<int, int>();
        private Dictionary<int, int> EDJunctionClassIDs = new Dictionary<int, int>();
        private Dictionary<int, int> EDEdgeClassIDs = new Dictionary<int, int>();
        private Dictionary<int, int> EDJunctionOIDs = new Dictionary<int, int>();
        private Dictionary<int, int> EDEdgeOIDs = new Dictionary<int, int>();
        IGeometricNetwork subGeomNetwork = null;
        IGeometricNetwork elecGeomNetwork = null;

        public PGE_Cache_Tracing()
        {

        }

        public void ExecuteTraceCaching(string EDConnectionFile, string SUBConnectionFile, bool isParent)
        {
            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();

            if (!File.Exists(EDConnectionFile))
            {
                throw new Exception("Could not find ED SDE connection file: " + EDConnectionFile
                    + ": Please correct the configuration file");
            }
            if (!File.Exists(SUBConnectionFile))
            {
                throw new Exception("Could not find SUB SDE connection file: " + SUBConnectionFile
                    + ": Please correct the configuration file");
            }

            _logger.Debug("Connecting to geodatabase using sde file: " + EDConnectionFile);
            if (isParent) { Console.WriteLine("Connecting to geodatabase using sde file: " + EDConnectionFile); }
            EDWorkspace = wsFactory.OpenFromFile(EDConnectionFile, 0);

            _logger.Debug("Connecting to geodatabase using sde file: " + SUBConnectionFile);
            if (isParent) { Console.WriteLine("Connecting to geodatabase using sde file: " + SUBConnectionFile); }
            SUBWorkspace = wsFactory.OpenFromFile(SUBConnectionFile, 0);

            _logger.Debug("Connected to geodatabases.");
            if (isParent) { Console.WriteLine("Connected to geodatabases."); }           

            InitializeFeederSpaces(isParent);

            if (geomNetworks.ContainsKey(Common.SubstationGeomNetwork))
            {
                subGeomNetwork = geomNetworks[Common.SubstationGeomNetwork];
            }
            if (geomNetworks.ContainsKey(Common.DistributionGeomNetwork))
            {
                elecGeomNetwork = geomNetworks[Common.DistributionGeomNetwork];
            }

            //Only build substation to distribution map if both substation and distribution are specified
            if (!string.IsNullOrEmpty(Common.SubstationGeomNetwork) && !string.IsNullOrEmpty(Common.DistributionGeomNetwork))
            {
                _logger.Info("Determine relationships between substation and distribution");
                if (isParent) { Console.WriteLine("Determine relationships between substation and distribution"); }
                BuildSubtationToDistributionMap();
            }

            if (EDWorkspace != null && SUBWorkspace != null && isParent)
            {
                //Execute any stored procedures defined to be run prior to tracing
                foreach (string procedure in Common.EDBeforeTracingStoredProcedures)
                {
                    try
                    {
                        EDoracleConnectionControl.ExecuteProcedure(procedure, new List<OracleParameter>());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error executing stored procedure " + procedure + ". Message: " + ex.Message);
                        Console.WriteLine("Error executing stored procedure " + procedure + ". Message: " + ex.Message);
                        throw ex;
                    }
                }

                //Execute any stored procedures defined to be run prior to tracing
                foreach (string procedure in Common.SUBBeforeTracingStoredProcedures)
                {
                    try
                    {
                        SUBoracleConnectionControl.ExecuteProcedure(procedure, new List<OracleParameter>());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error executing stored procedure " + procedure + ". Message: " + ex.Message);
                        Console.WriteLine("Error executing stored procedure " + procedure + ". Message: " + ex.Message);
                        throw ex;
                    }
                }
                
                //Clear our staging tables first
                EDoracleConnectionControl.ExecuteProcedure("EDGIS.TruncateTracingTables", new List<OracleParameter>());
                SUBoracleConnectionControl.ExecuteProcedure("EDGIS.TruncateTracingTables", new List<OracleParameter>());

                _logger.Debug("Determining Circuits to process");
                Console.WriteLine("Determining Circuits to process");
                
                PrepareTracingProcessTable(elecGeomNetwork, subGeomNetwork);

                ExecuteTracingInChildren();
                
                //Now to execute the specified stored procedures
                foreach (string procedure in Common.EDAfterTracingStoredProcedures)
                {
                    try
                    {
                        EDoracleConnectionControl.ExecuteProcedure(procedure, new List<OracleParameter>());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error executing electric distribution stored procedure " + procedure + ". Message: " + ex.Message);
                        Console.WriteLine("Error executing electric distribution stored procedure " + procedure + ". Message: " + ex.Message);
                        throw ex;
                    }
                }
                foreach (string procedure in Common.SUBAfterTracingStoredProcedures)
                {
                    try
                    {
                        SUBoracleConnectionControl.ExecuteProcedure(procedure, new List<OracleParameter>());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error executing substation stored procedure " + procedure + ". Message: " + ex.Message);
                        Console.WriteLine("Error executing substation stored procedure " + procedure + ". Message: " + ex.Message);
                        throw ex;
                    }
                }

                //Because our table reside in separate databases now, special logic is needed to merge them into one table.
                PopulateFeederFedFeederTable();
            }
            else
            {
                try
                {
                    List<string> FeederIDsToProcess = new List<string>();
                    FeederIDsToProcess = EDoracleConnectionControl.GetNextCircuitToProcess(CircuitType.UltimateSource, Common.CircuitsPerRequest);

                    while (FeederIDsToProcess.Count > 0)
                    {
                        foreach (string FeederIDToProcess in FeederIDsToProcess)
                        {
                            if (!String.IsNullOrEmpty(FeederIDToProcess))
                            {
                                TraceDownstream(subGeomNetwork, FeederIDToProcess, true);                                
                            }
                        }
                        FeederIDsToProcess = EDoracleConnectionControl.GetNextCircuitToProcess(CircuitType.UltimateSource, Common.CircuitsPerRequest);
                    }

                    while (EDoracleConnectionControl.GetUltimateSourceCircuitsLeft().Count > 0)
                    {
                        Thread.Sleep(3000);
                    }

                    FeederIDsToProcess = EDoracleConnectionControl.GetNextCircuitToProcess(CircuitType.Substation, Common.CircuitsPerRequest);

                    while (FeederIDsToProcess.Count > 0)
                    {
                        foreach (string FeederIDToProcess in FeederIDsToProcess)
                        {
                            if (!String.IsNullOrEmpty(FeederIDToProcess))
                            {
                                TraceDownstream(subGeomNetwork, FeederIDToProcess, true);                                
                            }
                        }
                        FeederIDsToProcess = EDoracleConnectionControl.GetNextCircuitToProcess(CircuitType.Substation, Common.CircuitsPerRequest);
                    }

                    while (EDoracleConnectionControl.GetSubstationCircuitsLeft().Count > 0)
                    {
                        Thread.Sleep(3000);
                    }

                    FeederIDsToProcess = EDoracleConnectionControl.GetNextCircuitToProcess(CircuitType.Distribution, Common.CircuitsPerRequest);

                    while (FeederIDsToProcess.Count > 0)
                    {
                        foreach (string FeederIDToProcess in FeederIDsToProcess)
                        {
                            if (!String.IsNullOrEmpty(FeederIDToProcess))
                            {
                                TraceDownstream(elecGeomNetwork, FeederIDToProcess, false);                                
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

        private Dictionary<int, int> _EDFCIDToSchemFCIDMap;
        /// <summary>
        /// ED FCID to schem FCID map loaded from the config file: EDFcid,SchemFcid
        /// </summary>
        private Dictionary<int, int> EDFcidToSchemFcidMap
        {
            get
            {

                if (_EDFCIDToSchemFCIDMap == null)
                {
                    try
                    {
                        _EDFCIDToSchemFCIDMap = new Dictionary<int, int>();

                        Dictionary<string, string> featClassNameMapping = SCHEMoracleConnectionControl.GetSchematicsFeatureClassNameMap();

                        foreach (KeyValuePair<string, string> KeyPhysicalNameKVP in featClassNameMapping)
                        {
                            int classID = EDoracleConnectionControl.GetObjectClassID(KeyPhysicalNameKVP.Key);
                            int schemClassID = SCHEMoracleConnectionControl.GetObjectClassID(KeyPhysicalNameKVP.Value);
                            if (classID > 0 && schemClassID > 0)
                            {
                                _EDFCIDToSchemFCIDMap.Add(classID, schemClassID);
                            }
                            else
                            {
                                _logger.Info("Schematics mapping in ELTCLASS table is incorrect and the GIS feature class could not be found: "
                                        + "GIS Feature Class: " + KeyPhysicalNameKVP.Key + " Schematics Feature Class: " + KeyPhysicalNameKVP.Value);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Unable to determine class ID mapping between schematics and Electric distribution databases: " + ex.Message);
                    }
                }
                return _EDFCIDToSchemFCIDMap;
            }
        }

        /// <summary>
        /// This method is will populate the feeder fed feeder table with results from the electric database and substation database tracing tables that
        /// were just completed
        /// </summary>
        private void PopulateFeederFedFeederTable()
        {
            try
            {
                if (feederfedfeederDataset == null)
                {
                    string tableName = Common.FeederFedFeederTraceResultsTable.Substring(Common.FeederFedFeederTraceResultsTable.IndexOf(".") + 1);
                    feederfedfeederDataset = EDoracleConnectionControl.GetDataset(tableName);
                }

                using (OracleDataReader reader = SUBoracleConnectionControl.SelectRows("select FEEDERFEDBY,FEEDERID,FROM_FEATURE_EID,TO_FEATURE_EID,TO_FEATURE_OID,TO_FEATURE_GLOBALID," +
                    "TO_FEATURE_FCID,TO_FEATURE_FEEDERINFO,TO_FEATURE_TYPE,ORDER_NUM,MIN_BRANCH,MAX_BRANCH,TREELEVEL from EDGIS.PGE_SUBGEOMNETWORK_TRACE"))
                {
                    while (reader.Read())
                    {
                        DataRow export = feederfedfeederDataset.Tables[Common.FeederFedFeederTraceResultsTable].NewRow();
                        export[Common.FeederFedByFieldName] = reader[0];
                        export[Common.CircuitIDFieldName] = reader[1];
                        export[Common.FromFeatureEIDFieldName] = reader[2];
                        export[Common.ToFeatureEIDFieldName] = reader[3];
                        export[Common.ToFeatureOIDFieldName] = reader[4];
                        export[Common.ToFeatureGlobalID] = reader[5];
                        export[Common.ToFeatureFCIDFieldName] = reader[6];
                        export[Common.ToFeatureFeederInfo] = reader[7];
                        export[Common.ToFeatureTypeFieldName] = reader[8];
                        export[Common.OrderFieldName] = reader[9];
                        export[Common.MinBranchFieldName] = reader[10];
                        export[Common.MaxBranchFieldName] = reader[11];
                        export[Common.LevelFieldName] = reader[12];
                        export[Common.ToFeatureSCHEMFCIDFieldName] = -1;
                        feederfedfeederDataset.Tables[Common.FeederFedFeederTraceResultsTable].Rows.Add(export);
                    }
                }

                EDoracleConnectionControl.BulkLoadData(feederfedfeederDataset);

                try
                {
                    EDoracleConnectionControl.ExecuteSql("insert into " + Common.FeederFedFeederTraceResultsTable + " select * from EDGIS.PGE_ELECDISTNETWORK_TRACE");
                    EDoracleConnectionControl.ExecuteProcedure("EDGIS.Create_FeederFed_Trace_Indices", new List<OracleParameter>());
                }
                catch (Exception ex)
                {
                    _logger.Error("Error executing electric distribution stored procedure " + "EDGIS.Create_FeederFed_Trace_Indices" + ". Message: " + ex.Message);
                    Console.WriteLine("Error executing electric distribution stored procedure " + "EDGIS.Create_FeederFed_Trace_Indices" + ". Message: " + ex.Message);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error obtaining electric distribution feature information. Message: " + ex.Message + " Stacktrace: " + ex.StackTrace);
                Console.WriteLine("Error obtaining electric distribution feature information. Message: " + ex.Message + " Stacktrace: " + ex.StackTrace);
                throw ex;
            }
        }

        IMMFeederSpace substationFeederSpace = null;
        IMMFeederSpace distributionFeederSpace = null;
        double totalFeeders = 0;
        double totalFeedersProcessed = 0;

        private void UpdateCircuitMap(string strFeederID)
        {
            try
            {
                if (!CircuitIDMap.Keys.Contains(strFeederID))
                {
                    List<string> tempList = new List<string>();
                    CircuitIDMap.Add(strFeederID, tempList);
                }
            }
            catch (Exception EX)
            {
                return;
            }
        }

        private void UpdateCircuitMap(string strSourceFeederID,string strChildFeederID)
        {
            try
            {
                if (!CircuitIDMap.Keys.Contains(strSourceFeederID))
                {
                    List<string> tempList = new List<string>();
                    tempList.Add(strChildFeederID);
                    CircuitIDMap.Add(strSourceFeederID, tempList);
                }
                else
                {
                    if (!CircuitIDMap[strSourceFeederID].Contains(strChildFeederID))
                    {
                        CircuitIDMap[strSourceFeederID].Add(strChildFeederID);
                    }
                }
            }
            catch (Exception EX)
            {
                return;
            }
        }

        private void PopulateChildCircuits()
        {
            bool foundChild = false;
            foreach (int intSUBeid in SubstationStitchPointsWithNoElecRel)
            {
                foundChild = false;
                        //Dictionary<int, string> SubSourceEIDtoCircuitIDMap = new Dictionary<int, string>();
                        //Dictionary<int, string> SubLoadEIDtoCircuitIDMap = new Dictionary<int, string>();
                        //Dictionary<int, string> EDSourceEIDtoCircuitIDMap = new Dictionary<int, string>();
                        //Dictionary<int, string> EDLoadEIDtoCircuitIDMap = new Dictionary<int, string>();
                if (SubSourceEIDtoCircuitIDMap.Keys.Contains(intSUBeid))
                {
                    UpdateCircuitMap(SubSourceEIDtoCircuitIDMap[intSUBeid]);
                    _logger.Info("Circuit ID :" + SubSourceEIDtoCircuitIDMap[intSUBeid] + " does not have any children circuits.");
                    foundChild = true;
                }
                if (SubLoadEIDtoCircuitIDMap.Keys.Contains(intSUBeid))
                {
                    UpdateCircuitMap(SubLoadEIDtoCircuitIDMap[intSUBeid]);
                    _logger.Info("Circuit ID :" + SubLoadEIDtoCircuitIDMap[intSUBeid] + " does not have any children circuits.");
                    foundChild = true;
                }
                if (!foundChild)
                {
                    _logger.Info("SubStationEID :" + intSUBeid.ToString() + " does not have any Circuit ID.");
                }
            }
            foreach (int intEDeid in DistributionToSubstationEIDMap.Keys)
            {   //Dictionary<int, string> EDSourceEIDtoCircuitIDMap = new Dictionary<int, string>();
                //Dictionary<int, string> EDLoadEIDtoCircuitIDMap = new Dictionary<int, string>();
                if (EDSourceEIDtoCircuitIDMap.Keys.Contains(intEDeid))
                {
                    foundChild = true;
                    string destCircuitID  = EDSourceEIDtoCircuitIDMap[intEDeid];
                    if (DistributionToSubstationEIDMap[intEDeid] != null)
                    {
                        int subEID = DistributionToSubstationEIDMap[intEDeid];
                        if (SubLoadEIDtoCircuitIDMap.Keys.Contains(subEID))
                        {
                            string srcCircuitID = SubLoadEIDtoCircuitIDMap[subEID];
                            UpdateCircuitMap(srcCircuitID, destCircuitID);
                            _logger.Info("Circuit ID :" + srcCircuitID + " has a child of CircuitID:" + destCircuitID + " ");
                        }
                        else
                        {
                            UpdateCircuitMap(destCircuitID);
                        }
                    }
                    else
                    {
                        UpdateCircuitMap(destCircuitID);
                    }
                }
                if (EDLoadEIDtoCircuitIDMap.Keys.Contains(intEDeid))
                {
                    foundChild = true;
                    string srcCircuitID = EDLoadEIDtoCircuitIDMap[intEDeid];
                    if (DistributionToSubstationEIDMap[intEDeid] != null)
                    {
                        int intSUBeid = DistributionToSubstationEIDMap[intEDeid];
                        if (SubSourceEIDtoCircuitIDMap.Keys.Contains(intSUBeid))
                        {
                            string destCircuitID = SubSourceEIDtoCircuitIDMap[intSUBeid];
                            UpdateCircuitMap(srcCircuitID, destCircuitID);
                            _logger.Info("Circuit ID :" + srcCircuitID + " has a child of CircuitID:" + destCircuitID + " ");
                        }
                        else
                        {
                            UpdateCircuitMap(srcCircuitID);
                        }
                    }
                    else
                    {
                        UpdateCircuitMap(srcCircuitID);
                    }
                }
                if (!foundChild)
                {
                    _logger.Info("DistributionEID :" + intEDeid.ToString() + " does not have any Circuit ID.");
                }
            }
            foreach (int intEDeid in EDSourceEIDtoCircuitIDMap.Keys)
            {
                
                if (EDSourceEIDtoCircuitIDMap[intEDeid] != null)
                {
                    if (!CircuitIDMap.Keys.Contains(EDSourceEIDtoCircuitIDMap[intEDeid]))
                    {
                        UpdateCircuitMap(EDSourceEIDtoCircuitIDMap[intEDeid]);
                        _logger.Info("DistributionEID: " + intEDeid.ToString() + " has CircuitID:" + EDSourceEIDtoCircuitIDMap[intEDeid] + " does not have any child Circuit ID.");
                    }
                }
                else
                {
                    _logger.Info("DistributionEID:" + intEDeid.ToString() + " does not have any Circuit ID.");
                }
            }
            foreach (int intSUBeid in SubSourceEIDtoCircuitIDMap.Keys)
            {

                if (SubSourceEIDtoCircuitIDMap[intSUBeid] != null)
                {
                    if (!CircuitIDMap.Keys.Contains(SubSourceEIDtoCircuitIDMap[intSUBeid]))
                    {
                        UpdateCircuitMap(SubSourceEIDtoCircuitIDMap[intSUBeid]);
                        _logger.Info("SubStationEID: " + intSUBeid.ToString() + " has CircuitID:" + SubSourceEIDtoCircuitIDMap[intSUBeid] + " and does not have any child Circuit ID.");
                    }
                }
                else
                {
                    _logger.Info("SubstationEID:" + intSUBeid.ToString() + " does not have any Circuit ID.");
                }
            }
        }

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
            _logger.Debug("Obtaining geometric networks from " + Common.SUBSDEConnectionFile);
            if (isParent) { Console.WriteLine("Obtaining geometric networks from " + Common.SUBSDEConnectionFile); }
            GetNetworks(SUBWorkspace);
            foreach (KeyValuePair<string, IGeometricNetwork> kvp in geomNetworks)
            {
                if (kvp.Key != Common.DistributionGeomNetwork && kvp.Key != Common.SubstationGeomNetwork) { continue; }

                if (kvp.Key == Common.SubstationGeomNetwork)
                {
                    if (isParent) { Console.WriteLine("Initializing substation feeder space"); }
                    _logger.Debug("Initializing substation feeder space");

                    substationFeederSpace = feederExt.get_FeederSpace(kvp.Value, false);
                    if (substationFeederSpace == null)
                    {
                        throw new Exception("Unable to get substation feeder space information from geometric network");
                    }
                }
                else if (kvp.Key == Common.DistributionGeomNetwork)
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

        /// <summary>
        /// This method will determine the current state of the relationships between the Substation Stitch Points
        /// and the Electric Stitch Points
        /// </summary>
        private void BuildSubtationToDistributionMap()
        {
            if (elecGeomNetwork == null || subGeomNetwork == null) { return; }
            if (((IDataset)subGeomNetwork).Workspace is IFeatureWorkspace && ((IDataset)elecGeomNetwork).Workspace is IFeatureWorkspace)
            {
                IFeatureWorkspace SUBFeatWS = ((IDataset)subGeomNetwork).Workspace as IFeatureWorkspace;
                IFeatureClass subElectricStitchPoint = SUBFeatWS.OpenFeatureClass(Common.SUBElectricStitchPoint);
                IFeatureWorkspace EDFeatWS = ((IDataset)elecGeomNetwork).Workspace as IFeatureWorkspace;
                IFeatureClass electricStitchPoint = EDFeatWS.OpenFeatureClass(Common.ElectricStitchPoint);
                int searchDistance = Common.SubStitchToElecStitchSearchDistance;

                //Now query for all features such that the foreign key is not null
                IQueryFilter qf = new QueryFilter();
                //SubtypeCD = 1 for Load
                //SubtypeCD = 2 for Source
                int intSubField_SUBTYPECDidx = subElectricStitchPoint.FindField("SUBTYPECD");
                int intSubField_CIRCUITIDidx = subElectricStitchPoint.FindField("CIRCUITID");
                qf.SubFields = subElectricStitchPoint.OIDFieldName + "," + "SUBTYPECD" + "," + "CIRCUITID" + "," + subElectricStitchPoint.ShapeFieldName;
                IFeatureCursor SUBFeatCursor = subElectricStitchPoint.Search(qf, true);
                List<string> elecStitchPointGUIDs = new List<string>();
                IFeature substationRow = null;
                while ((substationRow = SUBFeatCursor.NextFeature()) != null)
                {
                    string substationRowOID = "";
                    string substationRowCircuitID = "";
                    string substationRowSubTypeCD = "";
                    try
                    {
                        substationRowOID = substationRow.OID.ToString();
                        substationRowCircuitID = substationRow.get_Value(intSubField_CIRCUITIDidx).ToString();
                        substationRowSubTypeCD = substationRow.get_Value(intSubField_SUBTYPECDidx).ToString();
                        ISimpleJunctionFeature subJunctFeature = substationRow as ISimpleJunctionFeature;
                        //SubtypeCD = 1 for Load
                        //SubtypeCD = 2 for Source
                        if (substationRowSubTypeCD == "2")
                        {
                            if (!SubSourceEIDtoCircuitIDMap.Keys.Contains(subJunctFeature.EID))
                            {
                                SubSourceEIDtoCircuitIDMap.Add(subJunctFeature.EID, substationRowCircuitID);
                            }
                        }
                        else
                        {
                            if (!SubLoadEIDtoCircuitIDMap.Keys.Contains(subJunctFeature.EID))
                            {
                                SubLoadEIDtoCircuitIDMap.Add(subJunctFeature.EID, substationRowCircuitID);
                            }
                        }
                        string distributionOID = "";
                        string distributionCircuitID = "";
                        string distributionSubtypeCD = "";
                        ISpatialFilter spatialFilter = new SpatialFilterClass();
                        ITopologicalOperator topologicalOperator = substationRow.ShapeCopy as ITopologicalOperator;
                        spatialFilter.Geometry = topologicalOperator.Buffer(searchDistance);
                        spatialFilter.GeometryField = electricStitchPoint.ShapeFieldName;
                        spatialFilter.SubFields = electricStitchPoint.OIDFieldName + "," + "SUBTYPECD" + "," + "CIRCUITID" ;
                        spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                        IFeatureCursor relatedElecFeatures = electricStitchPoint.Search(spatialFilter, false);
                        IFeature elecFeature = null;
                        int intEDField_SUBTYPECDidx = electricStitchPoint.FindField("SUBTYPECD");
                        int intEDField_CIRCUITIDidx = electricStitchPoint.FindField("CIRCUITID");
                        int numRelated = 0;
                        while ((elecFeature = relatedElecFeatures.NextFeature()) != null)
                        {
                            try
                            {
                                numRelated++;
                                ISimpleJunctionFeature elecJunctFeature = elecFeature as ISimpleJunctionFeature;
                                SubstationToDistributionEIDMap.Add(subJunctFeature.EID, elecJunctFeature.EID);
                                DistributionToSubstationEIDMap.Add(elecJunctFeature.EID, subJunctFeature.EID);
                                distributionOID = elecFeature.OID.ToString();
                                distributionCircuitID = elecFeature.get_Value(intEDField_CIRCUITIDidx).ToString();
                                distributionSubtypeCD = elecFeature.get_Value(intEDField_SUBTYPECDidx).ToString();
                                //SubtypeCD = 1 for Load
                                //SubtypeCD = 2 for Source
                                if (distributionSubtypeCD == "2")
                                {
                                    if (!EDSourceEIDtoCircuitIDMap.Keys.Contains(elecJunctFeature.EID))
                                    {
                                        EDSourceEIDtoCircuitIDMap.Add(elecJunctFeature.EID, distributionCircuitID);
                                    }
                                }
                                else
                                {
                                   if (!EDLoadEIDtoCircuitIDMap.Keys.Contains(elecJunctFeature.EID))
                                   {
                                       EDLoadEIDtoCircuitIDMap.Add(elecJunctFeature.EID, distributionCircuitID);
                                   }
                                }
                            }
                            catch (Exception e)
                            {
                                //If something fails here we can continue as the FeederSource will pick up anything
                                //that we may miss here, even if relationships are invalid.
                                _logger.Warn("Error building relationship for SubStitchPoint: " + substationRowOID +
                                    " ElecStitchPoint: " + distributionOID + " Message: " + e.Message);
                            }

                            if (elecFeature != null) { while (Marshal.ReleaseComObject(elecFeature) > 0) { } }
                        }

                        if (relatedElecFeatures != null) { while (Marshal.ReleaseComObject(relatedElecFeatures) > 0) { } }

                        if (numRelated == 0)
                        {                           
                            SubstationStitchPointsWithNoElecRel.Add(subJunctFeature.EID);
                        }
                        subJunctFeature = null;
                        if (substationRow != null) { while (Marshal.ReleaseComObject(substationRow) > 0) { } }
                    }
                    catch (Exception e)
                    {
                        _logger.Warn("Error building relationship for SubStitchPoint to ElecStitchPoint , Message: \r\n" + e.Message);
                    }
                }

                if (SUBFeatCursor != null) { while (Marshal.ReleaseComObject(SUBFeatCursor) > 0) { } }
            }
        }

        private string PrepareTracingProcessTable(IGeometricNetwork elecGeomNetwork, IGeometricNetwork subGeomNetwork)
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

                List<string> ultimateSourceFeedersToProcess = new List<string>();
                List<string> substationFeedersToProcess = new List<string>();
                List<string> distributionFeedersToProcess = new List<string>();

                if (subGeomNetwork != null)
                {
                    feederSpace = feederExt.get_FeederSpace(subGeomNetwork, false);
                    feederSources = feederSpace.FeederSources;
                    feederSources.Reset();
                    while ((feederSource = feederSources.Next()) != null)
                    {
                        //start EID
                        int startEID = feederSource.JunctionEID;
                        string feederID = feederSource.FeederID.ToString();
                        if (!SubstationStitchPointsWithNoElecRel.Contains(startEID))
                        {
                            substationFeedersToProcess.Add(feederID);                            
                        }
                        else
                        {
                            ultimateSourceFeedersToProcess.Add(feederID);
                        }
                        if (!SubSourceEIDtoCircuitIDMap.Keys.Contains(startEID))
                        {
                            SubSourceEIDtoCircuitIDMap.Add(startEID, feederID);
                        }
                    }
                }

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
                            int startEID = feederSource.JunctionEID;
                            distributionFeedersToProcess.Add(feederID);
                            if (!EDSourceEIDtoCircuitIDMap.Keys.Contains(startEID))
                            {
                                EDSourceEIDtoCircuitIDMap.Add(startEID, feederID);
                            }
                        }
                        catch (Exception ex) 
                        { 
                        }
                    }
                }

                totalFeeders += distributionFeedersToProcess.Count;
                totalFeeders += substationFeedersToProcess.Count;
                totalFeeders += ultimateSourceFeedersToProcess.Count;

                PopulateChildCircuits();
                
                EDoracleConnectionControl.InsertCircuits(ultimateSourceFeedersToProcess, CircuitType.UltimateSource);
                EDoracleConnectionControl.InsertCircuits(substationFeedersToProcess, CircuitType.Substation);
                EDoracleConnectionControl.InsertCircuits(distributionFeedersToProcess, CircuitType.Distribution);
                EDoracleConnectionControl.InsertCircuitMap(CircuitIDMap);
                GC.Collect();

            }
            catch (Exception ex)
            {
                _logger.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
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

                AssemblyLocation = AssemblyLocation.Substring(0, AssemblyLocation.LastIndexOf("\\") + 1) + "PGE.BatchApplication.PGE_Cached_Tracing.exe";
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

        private string TraceDownstream(IGeometricNetwork geomNetwork, string feederID, bool isSubstation)
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
                _logger.Debug("Initializing feeder space");

                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                _logger.Debug("Finished initializing feeder space");

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
                feederSource = feederSources.get_FeederSourceByFeederID(feederID);
                feederSources.Reset();
                if (feederSource != null)
                {
                    //start EID
                    int startEID = feederSource.JunctionEID;

                    DateTime startTime = DateTime.Now;
                    _logger.Debug("Processing Circuit ID: " + feederSource.FeederID);

                    //#if DEBUG
                    //&& feederID != "254072112" 254072115 254072114
                    //                    if (feederID != "254072114") { continue; }
                    //#endif

                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    _logger.Debug("Tracing circuit: " + feederSource.FeederID);

                    //Trace
                    downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, startEID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    _logger.Debug("Finished Tracing circuit: " + feederSource.FeederID);

                    List<int> junctionEIDs = new List<int>();
                    List<int> edgeEIDs = new List<int>();
                    junctions.Reset();
                    edges.Reset();

                    int edgeEID = -1;
                    while ((edgeEID = edges.Next()) > 0)
                    {
                        edgeEIDs.Add(edgeEID);
                    }

                    int junctionEID = -1;
                    while ((junctionEID = junctions.Next()) > 0)
                    {
                        junctionEIDs.Add(junctionEID);
                    }

                    List<int> visitedJunctions = new List<int>();
                    List<int> visitedEdges = new List<int>();

                    int order = 0;
                    int branch = 0;
                    string feedID = feederID;
                    int maxBranch = GetResults(ref visitedJunctions, ref visitedEdges, network, startEID,
                        ref edgeEIDs, ref junctionEIDs, ref feedID, ref branch, 1, ref order, ref electricTraceWeight, isSubstation, feedID, "");

                    InsertRow(-1, startEID, feedID, 0, maxBranch, 0, ++order, 1, isSubstation, false, "");

                    //Now we have to go back through and process the EIDs encountered to add the FCIDs
                    AppendFCIDsToDataset(isSubstation, false, geomNetwork.OrphanJunctionFeatureClass.ObjectClassID);

                    //Process our conduit systems for the underground network
                    PopulateConduitSystemTraceTable(isSubstation);
                    if (!isSubstation) { AppendFCIDsToDataset(isSubstation, true, geomNetworks[Common.UndergroundGeomNetwork].OrphanJunctionFeatureClass.ObjectClassID); }
                    
                    InsertToDatabase(isSubstation);

                    _logger.Debug("Finished Processing Circuit ID: " + feederSource.FeederID +
                        " Total Processing time: " + Math.Round((DateTime.Now - startTime).TotalMinutes, 2) + " minutes");

                    EDoracleConnectionControl.UpdateCircuitStatus(feederID, "", CircuitStatus.Finished);

                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                EDoracleConnectionControl.UpdateCircuitStatus(feederID, "Error executing tracing. Message: " + ex.Message, CircuitStatus.Error);
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

        private int TraceDownstream(IGeometricNetwork geomNetwork, int startEID, ref int branch, ref int order, int level, bool isSubstation, string feederFedBy)
        {
            string feederID = "";
            int minBranch = branch;
            int maxBranch = branch;
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
                _logger.Debug("Initializing feeder space");

                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                //Console.WriteLine("Finsihed initializing feeder space");
                _logger.Debug("Finished initializing feeder space");

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
                feederSource = feederSpace.FeederSources.get_FeederSourceByJunctionEID(startEID);
                if (feederSource != null)
                {
                    feederID = feederSource.FeederID.ToString();

                    if (!EDoracleConnectionControl.CanProcessCircuit(feederID))
                    {
                        return maxBranch;
                    }

                    DateTime startTime = DateTime.Now;
                    _logger.Debug("Processing Circuit ID: " + feederSource.FeederID);
                    //Console.WriteLine("Processing Circuit ID: " + feederSource.FeederID);

                    //#if DEBUG
                    //&& feederID != "254072112" 254072115 254072114
                    //                    if (feederID != "254072114") { continue; }
                    //#endif

                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    _logger.Debug("Tracing circuit: " + feederSource.FeederID);

                    //Trace
                    downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, startEID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    _logger.Debug("Finished Tracing circuit: " + feederSource.FeederID);

                    List<int> junctionEIDs = new List<int>();
                    List<int> edgeEIDs = new List<int>();
                    junctions.Reset();
                    edges.Reset();

                    int edgeEID = -1;
                    while ((edgeEID = edges.Next()) > 0)
                    {
                        edgeEIDs.Add(edgeEID);
                    }

                    int junctionEID = -1;
                    while ((junctionEID = junctions.Next()) > 0)
                    {
                        junctionEIDs.Add(junctionEID);
                    }

                    List<int> visitedJunctions = new List<int>();
                    List<int> visitedEdges = new List<int>();

                    maxBranch = GetResults(ref visitedJunctions, ref visitedEdges, network, startEID,
                        ref edgeEIDs, ref junctionEIDs, ref feederID, ref branch, level, ref order, ref electricTraceWeight, isSubstation, feederID, feederFedBy);

                    InsertRow(-1, startEID, feederID, minBranch, maxBranch, level, ++order, 1, isSubstation, false, feederFedBy);

                    //Now we have to go back through and process the EIDs encountered to add the FCIDs
                    AppendFCIDsToDataset(isSubstation, false, geomNetwork.OrphanJunctionFeatureClass.ObjectClassID);

                    //Process conduit systems for underground network
                    PopulateConduitSystemTraceTable(isSubstation);
                    if (!isSubstation) { AppendFCIDsToDataset(isSubstation, true, geomNetworks[Common.UndergroundGeomNetwork].OrphanJunctionFeatureClass.ObjectClassID); }

                    InsertToDatabase(isSubstation);

                    _logger.Debug("Finished Processing Circuit ID: " + feederSource.FeederID +
                        " Total Processing time: " + Math.Round((DateTime.Now - startTime).TotalMinutes, 2) + " minutes");

                    EDoracleConnectionControl.UpdateCircuitStatus(feederID, "", CircuitStatus.Finished);

                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing tracing. Message: " + ex.Message + " StackTrace: " + ex.StackTrace);
                EDoracleConnectionControl.UpdateCircuitStatus(feederID, "Error executing tracing. Message: " + ex.Message, CircuitStatus.Error);
                throw ex;
            }
            finally
            {
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0);}
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0);}
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0);}
            }

            return maxBranch;
        }

        private void AppendFCIDsToDataset(bool isSubstation, bool isUnderground, int defaultJunctionFCIDToUpdate)
        {
            OracleConnectionControl connectionControlToUse = null;
            DataSet dsToUpdate = null;
            string TableName = "";
            if (isSubstation)
            {
                connectionControlToUse = SUBoracleConnectionControl;
                dsToUpdate = substationDataset;
                TableName = Common.SubstationTraceResultsTable;
            }
            else if (isUnderground)
            {
                connectionControlToUse = EDoracleConnectionControl;
                dsToUpdate = conduitTraceDataset;
                TableName = Common.UndergroundTraceResultsTable;
            }
            else
            {
                connectionControlToUse = EDoracleConnectionControl;
                dsToUpdate = distributionDataset;
                TableName = Common.DistributionTraceResultsTable;
            }

            StringBuilder whereInJunctionClauseBuilder = new StringBuilder();
            int junctionCounter = 0;
            StringBuilder whereInEdgeClauseBuilder = new StringBuilder();
            int edgeCounter = 0;
            Dictionary<string, List<string>> whereInClauses = new Dictionary<string, List<string>>();
            whereInClauses.Add("1", new List<string>());
            whereInClauses.Add("2", new List<string>());
            int rowCount = dsToUpdate.Tables[TableName].Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                string eid = dsToUpdate.Tables[TableName].Rows[i][Common.ToFeatureEIDFieldName].ToString();
                string featureType = dsToUpdate.Tables[TableName].Rows[i][Common.ToFeatureTypeFieldName].ToString();

                if (featureType == "1")
                {
                    //Junctions
                    if ((junctionCounter != 0 && (junctionCounter % 999) == 0))
                    {
                        whereInJunctionClauseBuilder.Append(eid);
                        whereInClauses["1"].Add(whereInJunctionClauseBuilder.ToString());
                        whereInJunctionClauseBuilder = new StringBuilder();
                        junctionCounter = 0;
                    }
                    else
                    {
                        whereInJunctionClauseBuilder.Append(eid + ",");
                        junctionCounter++;
                    }
                }
                else
                {
                    //Edges
                    if ((edgeCounter != 0 && (edgeCounter % 999) == 0))
                    {
                        whereInEdgeClauseBuilder.Append(eid);
                        whereInClauses["2"].Add(whereInEdgeClauseBuilder.ToString());
                        whereInEdgeClauseBuilder = new StringBuilder();
                        edgeCounter = 0;
                    }
                    else
                    {
                        whereInEdgeClauseBuilder.Append(eid + ",");
                        edgeCounter++;
                    }
                }

                if (i == rowCount - 1)
                {
                    if (featureType == "1") { whereInJunctionClauseBuilder.Append(eid); }
                    else { whereInEdgeClauseBuilder.Append(eid); }

                    if (whereInEdgeClauseBuilder.Length > 0 && whereInEdgeClauseBuilder[whereInEdgeClauseBuilder.Length - 1].ToString() == ",")
                    { whereInEdgeClauseBuilder.Remove(whereInEdgeClauseBuilder.Length - 1, 1); }
                    if (whereInJunctionClauseBuilder.Length > 0 && whereInJunctionClauseBuilder[whereInJunctionClauseBuilder.Length - 1].ToString() == ",")
                    { whereInJunctionClauseBuilder.Remove(whereInJunctionClauseBuilder.Length - 1, 1); }

                    if (whereInJunctionClauseBuilder.Length > 0) { whereInClauses["1"].Add(whereInJunctionClauseBuilder.ToString()); }
                    if (whereInEdgeClauseBuilder.Length > 0) { whereInClauses["2"].Add(whereInEdgeClauseBuilder.ToString()); }
                }
            }

            Dictionary<int, int> JunctionEIDsToFCIDs = new Dictionary<int, int>();
            Dictionary<int, int> JunctionEIDsToOIDs = new Dictionary<int, int>();
            Dictionary<int, int> EdgeEIDsToFCIDs = new Dictionary<int, int>();
            Dictionary<int, int> EdgeEIDsToOIDs = new Dictionary<int, int>();
            connectionControlToUse.GetFCIDListFromNetwork("EDGIS", defaultJunctionFCIDToUpdate, ref JunctionEIDsToFCIDs, ref EdgeEIDsToFCIDs, ref JunctionEIDsToOIDs, ref EdgeEIDsToOIDs, whereInClauses);

            for (int i = 0; i < dsToUpdate.Tables[TableName].Rows.Count; i++)
            {
                int eid = Int32.Parse(dsToUpdate.Tables[TableName].Rows[i][Common.ToFeatureEIDFieldName].ToString());
                string featureType = dsToUpdate.Tables[TableName].Rows[i][Common.ToFeatureTypeFieldName].ToString();
                int OID = -1;
                int FCID = -1;
                int schemFCID = -1;
                if (featureType == "1")
                {
                    try
                    {
                        FCID = JunctionEIDsToFCIDs[eid];
                        OID = JunctionEIDsToOIDs[eid];
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn("Unable to populate junction FCID for Junction EID: " + eid);
                        continue;
                    }
                }
                else
                {
                    try
                    {
                        FCID = EdgeEIDsToFCIDs[eid];
                        OID = EdgeEIDsToOIDs[eid];
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn("Unable to populate edge FCID for edge EID: " + eid);
                        continue;
                    }
                }

                if (!isSubstation && !isUnderground)
                {
                    if (EDFcidToSchemFcidMap.ContainsKey(FCID)) { schemFCID = EDFcidToSchemFcidMap[FCID]; }
                    dsToUpdate.Tables[TableName].Rows[i][Common.ToFeatureSCHEMFCIDFieldName] = schemFCID;
                }

                dsToUpdate.Tables[TableName].Rows[i][Common.ToFeatureFCIDFieldName] = FCID;
                dsToUpdate.Tables[TableName].Rows[i][Common.ToFeatureOIDFieldName] = OID;
            }
        }

        private void PopulateConduitSystemTraceTable(bool isSubstation)
        {
            //No need to do anything with a substation trace
            if (isSubstation) { return; }

            _logger.Debug("Gathering related conduit information");
            if (conduitTraceDataset == null)
            {
                conduitTraceDataset = EDoracleConnectionControl.GetDataset(Common.UndergroundTraceResultsTable);
            }

            StringBuilder whereInPriUGClauseBuilder = new StringBuilder();
            int PriUGCounter = 0;
            StringBuilder whereInSecUGClauseBuilder = new StringBuilder();
            int SecUGCounter = 0;
            StringBuilder whereInDCCondClauseBuilder = new StringBuilder();
            int DCCondCounter = 0;
            Dictionary<int, List<string>> whereInClauses = new Dictionary<int, List<string>>();
            whereInClauses.Add(Common.PriUGFCID, new List<string>());
            whereInClauses.Add(Common.SecUGFCID, new List<string>());
            whereInClauses.Add(Common.DCCondFCID, new List<string>());
            int rowCount = distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                string oid = distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.ToFeatureOIDFieldName].ToString();
                int FCID = Int32.Parse(distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.ToFeatureFCIDFieldName].ToString());
                if (FCID == Common.PriUGFCID)
                {
                    //Junctions
                    if ((PriUGCounter != 0 && (PriUGCounter % 999) == 0))
                    {
                        whereInPriUGClauseBuilder.Append(oid);
                        whereInClauses[FCID].Add(whereInPriUGClauseBuilder.ToString());
                        whereInPriUGClauseBuilder = new StringBuilder();
                        PriUGCounter = 0;
                    }
                    else
                    {
                        whereInPriUGClauseBuilder.Append(oid + ",");
                        PriUGCounter++;
                    }
                }
                else if (FCID == Common.SecUGFCID)
                {
                    //Junctions
                    if ((SecUGCounter != 0 && (SecUGCounter % 999) == 0))
                    {
                        whereInSecUGClauseBuilder.Append(oid);
                        whereInClauses[FCID].Add(whereInSecUGClauseBuilder.ToString());
                        whereInSecUGClauseBuilder = new StringBuilder();
                        SecUGCounter = 0;
                    }
                    else
                    {
                        whereInSecUGClauseBuilder.Append(oid + ",");
                        SecUGCounter++;
                    }
                }
                else if (FCID == Common.DCCondFCID)
                {
                    //Junctions
                    if ((DCCondCounter != 0 && (DCCondCounter % 999) == 0))
                    {
                        whereInDCCondClauseBuilder.Append(oid);
                        whereInClauses[FCID].Add(whereInDCCondClauseBuilder.ToString());
                        whereInDCCondClauseBuilder = new StringBuilder();
                        DCCondCounter = 0;
                    }
                    else
                    {
                        whereInDCCondClauseBuilder.Append(oid + ",");
                        DCCondCounter++;
                    }
                }

                if (i == rowCount - 1)
                {
                    if (FCID == Common.PriUGFCID) { whereInPriUGClauseBuilder.Append(oid); }
                    else if (FCID == Common.SecUGFCID) { whereInSecUGClauseBuilder.Append(oid); }
                    else if (FCID == Common.DCCondFCID) { whereInDCCondClauseBuilder.Append(oid); }

                    if (whereInPriUGClauseBuilder.Length > 0 && whereInPriUGClauseBuilder[whereInPriUGClauseBuilder.Length - 1].ToString() == ",")
                    { whereInPriUGClauseBuilder.Remove(whereInPriUGClauseBuilder.Length - 1, 1); }
                    if (whereInSecUGClauseBuilder.Length > 0 && whereInSecUGClauseBuilder[whereInSecUGClauseBuilder.Length - 1].ToString() == ",")
                    { whereInSecUGClauseBuilder.Remove(whereInSecUGClauseBuilder.Length - 1, 1); }
                    if (whereInDCCondClauseBuilder.Length > 0 && whereInDCCondClauseBuilder[whereInDCCondClauseBuilder.Length - 1].ToString() == ",")
                    { whereInDCCondClauseBuilder.Remove(whereInDCCondClauseBuilder.Length - 1, 1); }

                    if (whereInPriUGClauseBuilder.Length > 0) { whereInClauses[Common.PriUGFCID].Add(whereInPriUGClauseBuilder.ToString()); }
                    if (whereInSecUGClauseBuilder.Length > 0) { whereInClauses[Common.SecUGFCID].Add(whereInSecUGClauseBuilder.ToString()); }
                    if (whereInDCCondClauseBuilder.Length > 0) { whereInClauses[Common.DCCondFCID].Add(whereInDCCondClauseBuilder.ToString()); }
                }
            }

            //Now we have the list of object IDs for those feature class which are related to the conduit system feature classes.
            //We need to query the relationship table to get a list of objectids that we need to query the conduit system feature class for now.
            List<string> ConduitOIDwhereInClauses = new List<string>();
            Dictionary<int, List<int>> PriUGOIDToConduitOID = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> SecUGOIDToConduitOID = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> DCCondOIDToConduitOID = new Dictionary<int, List<int>>();

            //Process PriUG related conduit features first.
            GetConduitOIDsWhereClauses(ref PriUGOIDToConduitOID, ref ConduitOIDwhereInClauses, Common.ConduitPriUGRelTable, 
                Common.ConduitPriUGRelConduitFieldName, Common.ConduitPriUGRelUGFieldName, whereInClauses[Common.PriUGFCID]); 

            //Process SecUG related conduit features next
            GetConduitOIDsWhereClauses(ref SecUGOIDToConduitOID, ref ConduitOIDwhereInClauses, Common.ConduitSecUGRelTable, 
                Common.ConduitSecUGRelConduitFieldName, Common.ConduitSecUGRelUGFieldName, whereInClauses[Common.SecUGFCID]);

            //Process DC conductor related conduit features last
            GetConduitOIDsWhereClauses(ref DCCondOIDToConduitOID, ref ConduitOIDwhereInClauses, Common.ConduitDCCondRelTable, 
                Common.ConduitDCCondRelConduitFieldName, Common.ConduitDCCondRelUGFieldName, whereInClauses[Common.DCCondFCID]);
           
            //Now that we have the list of object IDs we can query for the associated network EIDs from the associated network table.
            Dictionary<int, int> ConduitOIDToEIDMap = new Dictionary<int, int>();
            EDoracleConnectionControl.GetConduitEIDsFromNetwork(ref ConduitOIDToEIDMap, "EDGIS", geomNetworks[Common.UndergroundGeomNetwork].OrphanJunctionFeatureClass.ObjectClassID,
                Common.ConduitFCID, ConduitOIDwhereInClauses);

            //Now that we have the list of OIDs to EIDs we can finally start some "traces" of the network to determine the connected junctions
            //for everything
            Dictionary<int, List<int>> junctionEIDToConduitEID = new Dictionary<int, List<int>>();
            GetConnectedJunctionsForConduit(ref junctionEIDToConduitEID, geomNetworks[Common.UndergroundGeomNetwork].Network, ConduitOIDToEIDMap.Values.ToList());

            InsertUndergroundRows(PriUGOIDToConduitOID, SecUGOIDToConduitOID, DCCondOIDToConduitOID, junctionEIDToConduitEID, ConduitOIDToEIDMap);

            _logger.Debug("Finished gathering related conduit information");
        }

        private struct UndergroundJunctionInfo
        {
            public int FromFeatureEID;
            public int ToFeatureEID;
            public int ToFeatureOID;
            public int OrderNumber;
            public int Level;
            public int MaxBranch;
            public int MinBranch;
            public string CircuitID;
            public string FeederFedBy;
            public int FeatureType;
        }

        private struct UndergroundEdgeInfo
        {
            public int FromFeatureEID;
            public int ToFeatureEID;
            public int ToFeatureOID;
            public int OrderNumber;
            public int Level;
            public int MaxBranch;
            public int MinBranch;
            public string CircuitID;
            public string FeederFedBy;
            public int FeatureType;
        }

        private void InsertUndergroundRows(Dictionary<int, List<int>> priUGOIDToConduitOIDMap, Dictionary<int, List<int>> secUGOIDToConduitOIDMap, 
            Dictionary<int, List<int>> DCCondOIDToConduitOIDMap, Dictionary<int, List<int>> junctionEIDToConduitEIDMap, Dictionary<int, int> conduitOIDToEIDMapping)
        {
            List<UndergroundEdgeInfo> edgeInfoObjects = new List<UndergroundEdgeInfo>();

            int rowCount = distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                int FCID = Int32.Parse(distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.ToFeatureFCIDFieldName].ToString());

                if (FCID == Common.PriUGFCID || FCID == Common.SecUGFCID || FCID == Common.DCCondFCID)
                {
                    List<int> conduitOIDsToAdd = new List<int>();
                    int OID = Int32.Parse(distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.ToFeatureOIDFieldName].ToString());

                    if (FCID == Common.PriUGFCID && priUGOIDToConduitOIDMap.ContainsKey(OID)) { conduitOIDsToAdd = priUGOIDToConduitOIDMap[OID]; }
                    else if (FCID == Common.SecUGFCID && secUGOIDToConduitOIDMap.ContainsKey(OID)) { conduitOIDsToAdd = secUGOIDToConduitOIDMap[OID]; }
                    else if (FCID == Common.DCCondFCID && DCCondOIDToConduitOIDMap.ContainsKey(OID)) { conduitOIDsToAdd = DCCondOIDToConduitOIDMap[OID]; }

                    foreach (int conduitOID in conduitOIDsToAdd)
                    {
                        int fromFeatureEID = -1;
                        if (!conduitOIDToEIDMapping.ContainsKey(conduitOID)) { continue; }
                        int toFeatureEID = conduitOIDToEIDMapping[conduitOID];
                        string circuitID = distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.CircuitIDFieldName].ToString();
                        int min_branch = Int32.Parse(distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.MinBranchFieldName].ToString());
                        int max_branch = Int32.Parse(distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.MaxBranchFieldName].ToString());
                        int level = Int32.Parse(distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.LevelFieldName].ToString());
                        int order = Int32.Parse(distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.OrderFieldName].ToString());
                        int featureType = Int32.Parse(distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.ToFeatureTypeFieldName].ToString());
                        string feederFedBy = distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows[i][Common.FeederFedByFieldName].ToString();

                        UndergroundEdgeInfo edgeInfo = new UndergroundEdgeInfo();
                        edgeInfo.CircuitID = circuitID;
                        edgeInfo.FeatureType = featureType;
                        edgeInfo.FeederFedBy = feederFedBy;
                        edgeInfo.FromFeatureEID = fromFeatureEID;
                        edgeInfo.Level = level;
                        edgeInfo.MaxBranch = max_branch;
                        edgeInfo.MinBranch = min_branch;
                        edgeInfo.OrderNumber = order;
                        edgeInfo.ToFeatureEID = toFeatureEID;
                        edgeInfo.ToFeatureOID = conduitOID;
                        edgeInfoObjects.Add(edgeInfo);
                    }
                }
            }

            List<UndergroundJunctionInfo> undergroundJunctionInfo = new List<UndergroundJunctionInfo>();

            //Find the connected junctions.
            foreach (KeyValuePair<int, List<int>> kvp in junctionEIDToConduitEIDMap)
            {
                List<UndergroundEdgeInfo> possibleEdgeInfo = new List<UndergroundEdgeInfo>();
                foreach (UndergroundEdgeInfo edge in edgeInfoObjects)
                {
                    if (kvp.Value.Contains(edge.ToFeatureEID))
                    {
                        possibleEdgeInfo.Add(edge);
                    }
                }

                int maxOrderNum = -1;
                foreach(UndergroundEdgeInfo edge in possibleEdgeInfo)
                {
                    if (edge.OrderNumber > maxOrderNum) { maxOrderNum = edge.OrderNumber; }
                }

                foreach (UndergroundEdgeInfo edge in possibleEdgeInfo)
                {
                    if (edge.OrderNumber == maxOrderNum)
                    {
                        int newOrderNum = maxOrderNum + 1;
                        if (kvp.Value.Count > 1)
                        {
                            newOrderNum = maxOrderNum - 1;
                        }
                        UndergroundJunctionInfo newjunction = GetNewJunctionInfo(edge.CircuitID, edge.FeederFedBy, edge.ToFeatureEID, kvp.Key,
                                        edge.Level, edge.MaxBranch, edge.MinBranch, newOrderNum);
                        undergroundJunctionInfo.Add(newjunction);
                    }
                }
            }

            foreach (UndergroundJunctionInfo junction in undergroundJunctionInfo)
            {
                InsertRow(junction.FromFeatureEID, junction.ToFeatureEID, junction.CircuitID, junction.MinBranch, junction.MaxBranch, junction.Level,
                    junction.OrderNumber, junction.FeatureType, false, true, junction.FeederFedBy);
            }

            foreach (UndergroundEdgeInfo edge in edgeInfoObjects)
            {
                InsertRow(edge.FromFeatureEID, edge.ToFeatureEID, edge.CircuitID, edge.MinBranch, edge.MaxBranch, edge.Level,
                    edge.OrderNumber, edge.FeatureType, false, true, edge.FeederFedBy);
            }
        }

        private UndergroundJunctionInfo GetNewJunctionInfo(string circuitID, string feederFedBy, int fromFeatureEID, int ToFeatureEID,
            int Level, int MaxBranch, int MinBranch, int OrderNumber)
        {
            UndergroundJunctionInfo newjunction = new UndergroundJunctionInfo();
            newjunction.CircuitID = circuitID;
            newjunction.FeatureType = 1;
            newjunction.FeederFedBy = feederFedBy;
            newjunction.FromFeatureEID = fromFeatureEID;
            newjunction.ToFeatureEID = ToFeatureEID;
            newjunction.Level = Level;
            newjunction.MaxBranch = MaxBranch;
            newjunction.MinBranch = MinBranch;
            newjunction.OrderNumber = OrderNumber;
            return newjunction;
        }

        private void GetConnectedJunctionsForConduit(ref Dictionary<int, List<int>> junctionEIDToConduitEID, INetwork conduitNetwork, List<int> conduitEIDs)
        {
            INetTopology netTopology = conduitNetwork as INetTopology;
            foreach (int conduitEID in conduitEIDs)
            {
                int toEID = -1;
                int fromEID = -1;

                try
                {
                    netTopology.GetFromToJunctionEIDs(conduitEID, out fromEID, out toEID);
                    if (!junctionEIDToConduitEID.ContainsKey(fromEID)) { junctionEIDToConduitEID.Add(fromEID, new List<int>()); }
                    if (!junctionEIDToConduitEID.ContainsKey(toEID)) { junctionEIDToConduitEID.Add(toEID, new List<int>()); }
                    junctionEIDToConduitEID[fromEID].Add(conduitEID);
                    junctionEIDToConduitEID[toEID].Add(conduitEID);
                }
                catch (Exception ex)
                {

                }
            }
        }

        IFeatureClass conduitFeatureClass = null;
        /// <summary>
        /// Determines the junctions that are connected to our list of conduit EIDs
        /// </summary>
        private void GetConnectedJunctionsForConduit(List<string> conduitOIDWhereClauses, ref Dictionary<int, List<int>> conduitOID)
        {
            if (conduitFeatureClass == null)
            {
                IFeatureWorkspace EDFeatWS = ((IDataset)elecGeomNetwork).Workspace as IFeatureWorkspace;
                conduitFeatureClass = EDFeatWS.OpenFeatureClass(Common.ConduitFeatureClass);
            }

            Dictionary<int, List<int>> subsurfaceStructureOIDsToConduitOIDs = new Dictionary<int, List<int>>();
            IQueryFilter qf = new QueryFilterClass();
            IFeatureCursor featCursor = null;
            qf.AddField(conduitFeatureClass.OIDFieldName);
            foreach (string whereClause in conduitOIDWhereClauses)
            {
                qf.WhereClause = conduitFeatureClass.OIDFieldName + " in (" + whereClause + ")";
                featCursor = conduitFeatureClass.Search(qf, false);
                IFeature feature = null;
                while ((feature = featCursor.NextFeature()) != null)
                {
                    IEdgeFeature edgeFeature = feature as IEdgeFeature;
                    //Would it be faster to get a list of the EIDs and then get the features associated with that later???
                    if (((IFeature)edgeFeature.FromJunctionFeature).Class.ObjectClassID == Common.SubsurfaceStructureFCID)
                    {
                        if (!subsurfaceStructureOIDsToConduitOIDs.ContainsKey(feature.OID))
                        {
                            subsurfaceStructureOIDsToConduitOIDs.Add(feature.OID, new List<int>());
                        }
                        subsurfaceStructureOIDsToConduitOIDs[feature.OID].Add(((IFeature)edgeFeature.FromJunctionFeature).OID);
                    }
                    if (((IFeature)edgeFeature.ToJunctionFeature).Class.ObjectClassID == Common.SubsurfaceStructureFCID)
                    {
                        if (!subsurfaceStructureOIDsToConduitOIDs.ContainsKey(feature.OID))
                        {
                            subsurfaceStructureOIDsToConduitOIDs.Add(feature.OID, new List<int>());
                        }
                        subsurfaceStructureOIDsToConduitOIDs[feature.OID].Add(((IFeature)edgeFeature.FromJunctionFeature).OID);
                    }
                    
                    if (feature != null) { while (Marshal.ReleaseComObject(feature) > 0) { } }
                }
                if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
            }
        }

        /// <summary>
        /// Obtains a list of object IDs for the conduit feature class using the related table. Populates a list of where In Clauses
        /// that can be uses to get the conduits.  It also populates a dictionary that defines the UG OIDs to the conduit OIDs for mapping later
        /// </summary>
        /// <param name="relationshipTableName">Name of relationship table</param>
        /// <param name="conduitOIDFieldName">Name of conduit field name in rel table</param>
        /// <param name="UGOIDFieldName">Name of UG field name in rel table</param>
        /// <param name="whereClauses">List of where clauses contains OIDs of UG objects</param>
        /// <returns></returns>
        private void GetConduitOIDsWhereClauses(ref Dictionary<int, List<int>> UGOIDToConduitOID, ref List<string> conduitOIDWhereInClauses, string relationshipTableName, string conduitOIDFieldName, string UGOIDFieldName, List<string> whereClauses)
        {
            foreach (string whereClause in whereClauses)
            {
                Dictionary<int, List<int>> UGOIDToConduitOIDTemp = EDoracleConnectionControl.GetConduitOIDs(ref UGOIDToConduitOID, relationshipTableName, 
                    conduitOIDFieldName, UGOIDFieldName, whereClause);
            }

            List<int> conduitOIDs = new List<int>();
            foreach (KeyValuePair<int, List<int>> kvp in UGOIDToConduitOID)
            {
                conduitOIDs.AddRange(kvp.Value);
            }
            conduitOIDs = conduitOIDs.Distinct().ToList();

            StringBuilder whereInClauseBuilder = new StringBuilder();
            for (int i = 0; i < conduitOIDs.Count; i++)
            {
                if ((i != 0 && (i % 999) == 0) || (i == conduitOIDs.Count - 1))
                {
                    whereInClauseBuilder.Append(conduitOIDs[i]);
                    conduitOIDWhereInClauses.Add(whereInClauseBuilder.ToString());
                    whereInClauseBuilder = new StringBuilder();
                }
                else
                {
                    whereInClauseBuilder.Append(conduitOIDs[i] + ",");
                }
            }
        }

        private int GetResults(ref List<int> visitedJunctions, ref List<int> visitedEdges, INetwork networkDataset, 
            int startingEID, ref List<int> edgeEIDs, ref List<int> junctionEIDs, ref string feederID, ref int branch, int level, 
            ref int order, ref INetWeight electricTraceWeight, bool isSubstation, string currentFeeder, string FeederFedBy)
        {
            if (visitedJunctions.Contains(startingEID)) { return -1; }

            int maxBranch = branch;
            visitedJunctions.Add(startingEID);

            if (isSubstation)
            {
                if (SubstationToDistributionEIDMap.ContainsKey(startingEID))
                {
                    string feederFedBy = FeederFedBy;
                    if (string.IsNullOrEmpty(feederFedBy)) { feederFedBy = currentFeeder; }
                    int startEID = SubstationToDistributionEIDMap[startingEID];
                    DistributionToSubstationEIDMap.Remove(SubstationToDistributionEIDMap[startingEID]);
                    SubstationToDistributionEIDMap.Remove(startingEID);
                    //This EID feeds the distribution network
                    int returnBranchValue = TraceDownstream(geomNetworks[Common.DistributionGeomNetwork], startEID, ref branch, ref order, level, false, feederFedBy);
                    if (returnBranchValue > maxBranch) { maxBranch = returnBranchValue; }
                }
            }
            else
            {
                if (DistributionToSubstationEIDMap.ContainsKey(startingEID))
                {
                    string feederFedBy = FeederFedBy;
                    if (string.IsNullOrEmpty(feederFedBy)) { feederFedBy = currentFeeder; }
                    int startEID = DistributionToSubstationEIDMap[startingEID];
                    SubstationToDistributionEIDMap.Remove(DistributionToSubstationEIDMap[startingEID]);
                    DistributionToSubstationEIDMap.Remove(startingEID);
                    //This EID feeds a substation network
                    int returnBranchValue = TraceDownstream(geomNetworks[Common.SubstationGeomNetwork], startEID, ref branch, ref order, level, true, feederFedBy);
                    if (returnBranchValue > maxBranch) { maxBranch = returnBranchValue; }
                }
            }

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
                            int currentMinBranch = branch;
                            if (JunctionIsClosed(junctionWeightValue))
                            {
                                int returnBranchValue = GetResults(ref visitedJunctions, ref visitedEdges, networkDataset, adjacentJunctionEID,
                                    ref edgeEIDs, ref junctionEIDs, ref feederID, ref branch, level + 2, ref order, ref electricTraceWeight, isSubstation, currentFeeder, FeederFedBy);
                                if (returnBranchValue > maxBranch) { maxBranch = returnBranchValue; }
                            }

                            order++;
                            InsertRow(adjacentEdgeEID, adjacentJunctionEID, feederID, currentMinBranch, maxBranch, level + 1, order, 1, isSubstation, false, FeederFedBy);
                            order++;
                            InsertRow(startingEID, adjacentEdgeEID, feederID, currentMinBranch, maxBranch, level, order, 2, isSubstation, false, FeederFedBy);
                        }
                        else
                        {
                            order++;
                            InsertRow(startingEID, adjacentEdgeEID, feederID, branch, maxBranch, level, order, 2, isSubstation, false, FeederFedBy);
                        }
                        branch++;
                    }
                }
                return maxBranch;
            }
            catch (Exception ex)
            {
                _logger.Error("Error processing circuit: " + feederID + " Message: " + ex.Message);
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

        private DataSet substationDataset = null;
        private DataSet distributionDataset = null;
        private DataSet conduitTraceDataset = null;
        private DataSet feederfedfeederDataset = null;

        private void InsertToDatabase(bool isSubstation)
        {
            if (substationDataset == null) 
            {
                string tableName = Common.SubstationTraceResultsTable.Substring(Common.SubstationTraceResultsTable.IndexOf(".") + 1);
                substationDataset = EDoracleConnectionControl.GetDataset(tableName); 
            }
            if (distributionDataset == null)
            {
                string tableName = Common.DistributionTraceResultsTable.Substring(Common.DistributionTraceResultsTable.IndexOf(".") + 1);
                distributionDataset = EDoracleConnectionControl.GetDataset(tableName); 
            }
            if (conduitTraceDataset == null)
            {
                string tableName = Common.UndergroundTraceResultsTable.Substring(Common.UndergroundTraceResultsTable.IndexOf(".") + 1);
                conduitTraceDataset = EDoracleConnectionControl.GetDataset(tableName);
            }
            if (isSubstation)
            {
                SUBoracleConnectionControl.BulkLoadData(substationDataset);
                substationDataset.Tables[Common.SubstationTraceResultsTable].Rows.Clear();
            }
            else
            {
                EDoracleConnectionControl.BulkLoadData(distributionDataset);
                EDoracleConnectionControl.BulkLoadData(conduitTraceDataset);
                distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows.Clear();
                conduitTraceDataset.Tables[Common.UndergroundTraceResultsTable].Rows.Clear();
            }
        }

        private void InsertRow(int fromFeatureEID, int toFeatureEID, string circuitID, int min_branch, 
            int max_branch, int level, int order, int featureType, bool isSubstation, bool isUnderground, string feederFedBy)
        {
            if (substationDataset == null)
            {
                string tableName = Common.SubstationTraceResultsTable.Substring(Common.SubstationTraceResultsTable.IndexOf(".") + 1);
                substationDataset = SUBoracleConnectionControl.GetDataset(tableName);
            }
            if (distributionDataset == null)
            {
                string tableName = Common.DistributionTraceResultsTable.Substring(Common.DistributionTraceResultsTable.IndexOf(".") + 1);
                distributionDataset = EDoracleConnectionControl.GetDataset(tableName);
            }
            if (conduitTraceDataset == null)
            {
                string tableName = Common.UndergroundTraceResultsTable.Substring(Common.UndergroundTraceResultsTable.IndexOf(".") + 1);
                conduitTraceDataset = EDoracleConnectionControl.GetDataset(tableName);
            }
            

            if (isSubstation)
            {
                DataRow export = substationDataset.Tables[Common.SubstationTraceResultsTable].NewRow();
                export[Common.FeederFedByFieldName] = feederFedBy;
                export[Common.CircuitIDFieldName] = circuitID;
                export[Common.FromFeatureEIDFieldName] = fromFeatureEID;
                export[Common.ToFeatureEIDFieldName] = toFeatureEID;
                export[Common.MinBranchFieldName] = min_branch;
                export[Common.MaxBranchFieldName] = max_branch;
                export[Common.LevelFieldName] = level;
                export[Common.OrderFieldName] = order;
                export[Common.ToFeatureTypeFieldName] = featureType;
                substationDataset.Tables[Common.SubstationTraceResultsTable].Rows.Add(export);
            }
            else if (isUnderground)
            {
                DataRow export = conduitTraceDataset.Tables[Common.UndergroundTraceResultsTable].NewRow();
                export[Common.FeederFedByFieldName] = feederFedBy;
                export[Common.CircuitIDFieldName] = circuitID;
                export[Common.FromFeatureEIDFieldName] = fromFeatureEID;
                export[Common.ToFeatureEIDFieldName] = toFeatureEID;
                export[Common.MinBranchFieldName] = min_branch;
                export[Common.MaxBranchFieldName] = max_branch;
                export[Common.LevelFieldName] = level;
                export[Common.OrderFieldName] = order;
                export[Common.ToFeatureTypeFieldName] = featureType;
                conduitTraceDataset.Tables[Common.UndergroundTraceResultsTable].Rows.Add(export);
            }
            else
            {
                DataRow export = distributionDataset.Tables[Common.DistributionTraceResultsTable].NewRow();
                export[Common.FeederFedByFieldName] = feederFedBy;
                export[Common.CircuitIDFieldName] = circuitID;
                export[Common.FromFeatureEIDFieldName] = fromFeatureEID;
                export[Common.ToFeatureEIDFieldName] = toFeatureEID;
                export[Common.MinBranchFieldName] = min_branch;
                export[Common.MaxBranchFieldName] = max_branch;
                export[Common.LevelFieldName] = level;
                export[Common.OrderFieldName] = order;
                export[Common.ToFeatureTypeFieldName] = featureType;
                distributionDataset.Tables[Common.DistributionTraceResultsTable].Rows.Add(export);
            }
        }

        /// <summary>
        /// This method will return the collection of netwotrks from the specified workspace
        /// </summary>
        /// <param name="networkName"></param>
        /// <returns></returns>
        private static void GetNetworks(IWorkspace ws)
        {
            if (geomNetworks.ContainsKey(Common.SubstationGeomNetwork) && geomNetworks.ContainsKey(Common.DistributionGeomNetwork)) { return; }
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
    
    }

    public struct EdgeInformation
    {
        public int edgeEID;
        public int fromJunctionEID;
        public int toJunctionEID;
    }
}
