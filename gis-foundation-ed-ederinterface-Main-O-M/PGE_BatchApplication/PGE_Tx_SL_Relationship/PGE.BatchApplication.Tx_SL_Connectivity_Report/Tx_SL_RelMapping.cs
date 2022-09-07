using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using Miner.Interop;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.NetworkAnalysis;
using System.Reflection;
using System.Data;

namespace PGE.BatchApplication.PGE_Tx_SL_Connectivity_Report
{
    public class Tx_SL_RelMapping
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IWorkspace workspace = null;
        Dictionary<int, int> transformerEIDs = new Dictionary<int, int>();
        Dictionary<int, int> serviceLocationEIDs = new Dictionary<int, int>();
        List<int> secondaryEIDs = new List<int>();
        List<int> junctionEIDs = new List<int>();
        List<int> edgeEIDs = new List<int>();

        public Tx_SL_RelMapping()
        {

        }

        /// <summary>
        /// This method will perform the necessary tracing functionality to generate the geometric connectivity
        /// mapping between the transformers and their service points
        /// </summary>
        /// <param name="sdeConnectionFile">Path to SDE Connection file</param>
        public void Execute_Tx_SL_Mapping(string sdeConnectionFile)
        {
            _logger.Debug("Connecting to geodatabase using sde file: " + sdeConnectionFile);
            Console.WriteLine("Connecting to geodatabase using sde file: " + sdeConnectionFile);
            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();
            workspace = wsFactory.OpenFromFile(sdeConnectionFile, 0);
            _logger.Debug("Connected to geodatabase.");
            Console.WriteLine("Connected to geodatabase.");

            //Clear our staging table first
            _logger.Debug("Clearing EDGIS.PGE_TX_SL_CONNECTIVITY table");
            Console.WriteLine("Clearing EDGIS.PGE_TX_SL_CONNECTIVITY table");
            workspace.ExecuteSQL("DELETE FROM EDGIS.PGE_TX_SL_CONNECTIVITY");

            if (workspace != null)
            {
                _logger.Debug("Finding geometric network: " + Common.GeometricNetworkName);
                Console.WriteLine("Finding geometric network: " + Common.GeometricNetworkName);
                Dictionary<string, IGeometricNetwork> geomNetworks = GetNetworks(workspace);
                if (geomNetworks.Count > 0 && geomNetworks.ContainsKey(Common.GeometricNetworkName))
                {
                    _logger.Debug("Found geometric network");
                    Console.WriteLine("Found geometric network");
                    IGeometricNetwork geomNetwork = geomNetworks[Common.GeometricNetworkName];
                    TraceDownstream(geomNetwork);
                }
                else
                {
                    _logger.Error("Unable to find the geometric network: " + Common.GeometricNetworkName);
                    Console.WriteLine("Unable to find the geometric network: " + Common.GeometricNetworkName);
                }
            }
        }

        private string TraceDownstream(IGeometricNetwork geomNetwork)
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
                Console.WriteLine("Initializing feeder space");
                _logger.Debug("Initializing feeder space");

                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                Console.WriteLine("Finsihed initializing feeder space");
                _logger.Debug("Finished initializing feeder space");

                INetwork network = geomNetwork.Network;

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
                double totalFeeders = feederSources.Count;
                double totalFeedersProcessed = 0;
                while ((feederSource = feederSources.Next()) != null)
                {
                    
                    Dictionary<int, List<int>> Tx_SL_Map = new Dictionary<int, List<int>>();

                    DateTime startTime = DateTime.Now;
                    _logger.Debug("Processing Circuit ID: " + feederSource.FeederID);
                    Console.WriteLine("Processing Circuit ID: " + feederSource.FeederID);
                    
                    //start EID
                    int startEID = feederSource.JunctionEID;

                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    _logger.Debug("Tracing circuit: " + feederSource.FeederID);

                    //Trace
                    downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, startEID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);

                    _logger.Debug("Finished Tracing circuit: " + feederSource.FeederID);

                    transformerEIDs = new Dictionary<int, int>();
                    serviceLocationEIDs = new Dictionary<int, int>();
                    junctionEIDs = new List<int>();
                    edgeEIDs = new List<int>();
                    

                    IEIDHelper eidHelper = new EIDHelperClass();
                    eidHelper.ReturnFeatures = true;
                    eidHelper.ReturnGeometries = false;
                    eidHelper.GeometricNetwork = geomNetwork;

                    _logger.Debug("Determining Transformer to Service Location Relationships for circuit: " + feederSource.FeederID);

                    IEnumEIDInfo junctionEIDInfoEnum = eidHelper.CreateEnumEIDInfo(junctions);
                    IEIDInfo junctionEIDInfo = null;
                    junctionEIDInfoEnum.Reset();
                    //IEnumEIDInfo junctionEIDInfo = eidHelper.CreateEnumEIDInfo(junctions);
                    while ((junctionEIDInfo = junctionEIDInfoEnum.Next()) != null)
                    {
                        junctionEIDs.Add(junctionEIDInfo.EID);
                        if (junctionEIDInfo.Feature.Class.ObjectClassID == Common.TransformerClassID)
                        {
                            transformerEIDs.Add(junctionEIDInfo.EID, junctionEIDInfo.Feature.OID);
                        }
                        else if (junctionEIDInfo.Feature.Class.ObjectClassID == Common.ServiceLocationClassID)
                        {
                            serviceLocationEIDs.Add(junctionEIDInfo.EID, junctionEIDInfo.Feature.OID);
                        }
                        else if (Common.SecondaryClassIDs.Contains(junctionEIDInfo.Feature.Class.ObjectClassID))
                        {
                            secondaryEIDs.Add(junctionEIDInfo.EID);
                        }
                    }

                    IEnumEIDInfo edgeEIDInfoEnum = eidHelper.CreateEnumEIDInfo(edges);
                    IEIDInfo edgeEIDInfo = null;
                    edgeEIDInfoEnum.Reset();
                    //IEnumEIDInfo junctionEIDInfo = eidHelper.CreateEnumEIDInfo(junctions);
                    while ((edgeEIDInfo = edgeEIDInfoEnum.Next()) != null)
                    {
                        edgeEIDs.Add(edgeEIDInfo.EID);
                        if (Common.SecondaryClassIDs.Contains(edgeEIDInfo.Feature.Class.ObjectClassID))
                        {
                            secondaryEIDs.Add(edgeEIDInfo.EID);
                        }
                    }

                    foreach (KeyValuePair<int,int> eid in transformerEIDs)
                    {
                        Get_Tx_SL_RelMapping_For_Feeder(network, eid.Key, ref Tx_SL_Map);
                    }

                    _logger.Debug("Transformers processed: " + transformerEIDs.Count);
                    _logger.Debug("Finished determining Transformer to Service Location Relationships for circuit: " + feederSource.FeederID);

                    _logger.Debug("Inserting entries to the EDGIS.PGE_TX_SL_CONNECTIVITY table");
                    InsertToDatabase(ref Tx_SL_Map);
                    Tx_SL_Map.Clear();
                    _logger.Debug("Finished inserting entries to the EDGIS.PGE_TX_SL_CONNECTIVITY table");

                    //Clear some memory
                    if (junctionEIDInfoEnum != null) { while (Marshal.ReleaseComObject(junctionEIDInfoEnum) > 0) { } }
                    if (edgeEIDInfoEnum != null) { while (Marshal.ReleaseComObject(edgeEIDInfoEnum) > 0) { } }
                    transformerEIDs.Clear();
                    serviceLocationEIDs.Clear();
                    junctionEIDs.Clear();
                    edgeEIDs.Clear();

                    _logger.Debug("Finished Processing Circuit ID: " + feederSource.FeederID + 
                        " Total Processing time: " + Math.Round((DateTime.Now - startTime).TotalMinutes, 2) + " minutes");
                    Console.WriteLine(" Total Processing time: " + Math.Round((DateTime.Now - startTime).TotalMinutes, 2) + " minutes");

                    totalFeedersProcessed++;
                    Console.WriteLine("Percent Complete: " + (Math.Round((totalFeedersProcessed / totalFeeders), 2) * 100.0));

                    GC.Collect();
                }
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

        private DataSet dataset = null;

        private void InsertToDatabase(ref Dictionary<int, List<int>> Tx_SL_Map)
        {
            if (dataset == null) { dataset = OracleConnectionControl.GetDataset(); }
            else { dataset.Tables["EDGIS.PGE_TX_SL_CONNECTIVITY"].Rows.Clear(); }

            foreach (KeyValuePair<int, List<int>> kvp in Tx_SL_Map)
            {
                int transformerOID = transformerEIDs[kvp.Key];
                foreach (int ServiceLocationEID in kvp.Value)
                {
                    int serviceLocationOID = serviceLocationEIDs[ServiceLocationEID];
                    DataRow export = dataset.Tables["EDGIS.PGE_TX_SL_CONNECTIVITY"].NewRow();
                    export["TRANSFORMEROID"] = transformerOID;
                    export["SERVICELOCATIONOID"] = serviceLocationOID;
                    dataset.Tables["EDGIS.PGE_TX_SL_CONNECTIVITY"].Rows.Add(export);
                }
            }

            OracleConnectionControl.BulkLoadData(dataset);

            //string sql = "insert into EDGIS.PGE_TX_SL_CONNECTIVITY (TRANSFORMEROID,SERVICELOCATIONOID) values ";
            
        }

        private void Get_Tx_SL_RelMapping_For_Feeder(INetwork networkDataset, int startingTransformerEID, ref Dictionary<int, List<int>> Tx_SL_Map)
        {
            if (Tx_SL_Map.ContainsKey(startingTransformerEID)) { return; }
            else { Tx_SL_Map.Add(startingTransformerEID, new List<int>()); }

            List<int> visitedJunctions = new List<int>();
            visitedJunctions.Add(startingTransformerEID);

            IForwardStar forwardStar = networkDataset.CreateForwardStar(false, null, null, null, null);

            try
            {
                //Find our adjacent edges to this junction EID
                int edgeCount = 0;
                forwardStar.FindAdjacent(0, startingTransformerEID, out edgeCount);

                for (int i = 0; i < edgeCount; i++)
                {
                    int adjacentEdgeEID = -1;
                    bool reverseOrientation = false;
                    object edgeWeightValue = null;
                    forwardStar.QueryAdjacentEdge(i, out adjacentEdgeEID, out reverseOrientation, out edgeWeightValue);

                    if (secondaryEIDs.Contains(adjacentEdgeEID))
                    {
                        //Find the adjacent junction for this edge and add it to our queue to process
                        int adjacentJunctionEID = -1;
                        object junctionWeightValue = null;
                        forwardStar.QueryAdjacentJunction(i, out adjacentJunctionEID, out junctionWeightValue);
                        Get_Tx_SL_RelMapping_For_Feeder(networkDataset, adjacentJunctionEID, ref Tx_SL_Map, ref visitedJunctions, startingTransformerEID);
                    }
                }
            }
            finally
            {
                if (forwardStar != null) { while (Marshal.ReleaseComObject(forwardStar) > 0) { } }
            }
        }

        private void Get_Tx_SL_RelMapping_For_Feeder(INetwork networkDataset, int startingEID, ref Dictionary<int, List<int>> Tx_SL_Map,
            ref List<int> visitedJunctions, int currentTransformerEID)
        {
            if (visitedJunctions.Contains(startingEID)) { return; }
            
            visitedJunctions.Add(startingEID);

            if (transformerEIDs.ContainsKey(startingEID)) { currentTransformerEID = startingEID; }

            IForwardStar forwardStar = networkDataset.CreateForwardStar(false, null, null, null, null);

            try
            {
                if (serviceLocationEIDs.ContainsKey(startingEID))
                {
                    if (!Tx_SL_Map.ContainsKey(currentTransformerEID)) { Tx_SL_Map.Add(currentTransformerEID, new List<int>()); }
                    if (!Tx_SL_Map[currentTransformerEID].Contains(startingEID)) { Tx_SL_Map[currentTransformerEID].Add(startingEID); }
                }

                //Find our adjacent edges to this junction EID
                int edgeCount = 0;
                forwardStar.FindAdjacent(0, startingEID, out edgeCount);

                for (int i = 0; i < edgeCount; i++)
                {
                    int adjacentEdgeEID = -1;
                    bool reverseOrientation = false;
                    object edgeWeightValue = null;
                    forwardStar.QueryAdjacentEdge(i, out adjacentEdgeEID, out reverseOrientation, out edgeWeightValue);

                    if (edgeEIDs.Contains(adjacentEdgeEID))
                    {
                        //Find the adjacent junction for this edge and add it to our queue to process
                        int adjacentJunctionEID = -1;
                        object junctionWeightValue = null;
                        forwardStar.QueryAdjacentJunction(i, out adjacentJunctionEID, out junctionWeightValue);
                        Get_Tx_SL_RelMapping_For_Feeder(networkDataset, adjacentJunctionEID, ref Tx_SL_Map, ref visitedJunctions, currentTransformerEID);
                    }
                }
            }
            finally
            {
                if (forwardStar != null) { while (Marshal.ReleaseComObject(forwardStar) > 0) { } }
            }
        }


        /// <summary>
        /// This method will return the collection of netwotrks from the specified workspace
        /// </summary>
        /// <param name="networkName"></param>
        /// <returns></returns>
        private static Dictionary<string, IGeometricNetwork> GetNetworks(IWorkspace ws)
        {
            Dictionary<string, IGeometricNetwork> geomNetworks = new Dictionary<string, IGeometricNetwork>();
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
                    INetworkCollection networkCollection = featureDataset as INetworkCollection;
                    for (int index = 0; index <= networkCollection.GeometricNetworkCount - 1; index++)
                    {
                        geomNetwork = networkCollection.get_GeometricNetwork(index);
                        geomNetworks.Add((geomNetwork as IDataset).BrowseName.ToUpper(), geomNetwork);
                    }
                }
                dsName = enumDataset.Next();
            }

            if (geomNetworks.Count > 0)
            {
                return geomNetworks;
            }
            else
            {
                throw new Exception("Could not find the geometric networks in the specified database");
            }
        }
    }
}
