using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using Miner.Geodatabase.Integration.Electric;
using Miner.Interop;

namespace PGE.Interface.Integration.DMS.Tracers
{
    public class ESRITrace : ICircuitTracer, IDisposable
    {
        private GeodatabaseAccess _geoDatabase;
        private bool _cancel;
        private IFeederTracer _trace;

        public IFeederTracer OldTrace
        {
            get { return _trace; }
            set { _trace = value; }
        }

        public ESRITrace(GeodatabaseAccess gdbAccess)
        {
            _geoDatabase = gdbAccess;
        }
        #region ICircuitTracer Members

        public void QueryFeatures(FeatureQueryType queryType, IDictionary<string, Miner.Geodatabase.Integration.Electric.ICircuit> features)
        {
            ((ICircuitTracer)_trace).QueryFeatures(queryType, features);
        }

        public Miner.Geodatabase.Integration.Electric.ICircuit Trace(string circuitName, int sourceEID, bool includeDeEnergized)
        {
            return Trace(circuitName, sourceEID, null, includeDeEnergized); ;
        }

        public Miner.Geodatabase.Integration.Electric.ICircuit Trace(string circuitName, int sourceEID, IList<int> barrierJunctions, bool includeDeEnergized)
        {
            string networkName = _geoDatabase.GetTableName(((IDataset)_geoDatabase.Network).Name);
            //create a FeederInfo
            FeederInfo feederInfo = new FeederInfo(circuitName, sourceEID, networkName);

            int outAdjacentEdgeCount = 0;
            Queue junctionEIDsToVisit = new Queue();
            junctionEIDsToVisit.Enqueue(sourceEID);
            Dictionary<int, bool> visitedJunctions = new Dictionary<int, bool>();
            Dictionary<int, bool> usedEdges = new Dictionary<int, bool>();//only use each edge once
            //do an ESRI trace
            IGeometricNetwork geomnetwork = _geoDatabase.Network;
            INetwork network = geomnetwork.Network;
            INetSchema netSchema = (INetSchema)network;
            IForwardStarGEN fStar = (IForwardStarGEN)network.CreateForwardStar(false, null, null, null, null);
            while (junctionEIDsToVisit.Count > 0)
            {
                int currentJunction = System.Convert.ToInt32(junctionEIDsToVisit.Dequeue());

                if (visitedJunctions.ContainsKey(currentJunction))
                {
                    continue;
                }
                else
                {
                    visitedJunctions.Add(currentJunction, true);
                }

                fStar.FindAdjacent(0, currentJunction, out outAdjacentEdgeCount);
                if (outAdjacentEdgeCount > 0)
                {
                    int[] outAdjanceEdgeEIDS = new int[outAdjacentEdgeCount];
                    object[] outEdgeWeightValues = new object[outAdjacentEdgeCount];
                    bool[] revOrientations = new bool[outAdjacentEdgeCount];
                    int[] outAdjacentJuncEIDS = new int[outAdjacentEdgeCount];
                    object[] outJuncWeightValues = new object[outAdjacentEdgeCount];
                    fStar.QueryAdjacentEdges(ref outAdjanceEdgeEIDS, ref revOrientations, ref outEdgeWeightValues);
                    fStar.QueryAdjacentJunctions(ref outAdjacentJuncEIDS, ref outJuncWeightValues);

                    //for each edge connected to this junction
                    for (int i = 0; i < outAdjacentEdgeCount; i++)
                    {
                        int connectedEdgeEID = outAdjanceEdgeEIDS[i];
                        if (!usedEdges.ContainsKey(connectedEdgeEID))//if we haven't been down this edge before
                        {
                            usedEdges.Add(connectedEdgeEID, true);
                            int oppositeJuncEID = outAdjacentJuncEIDS[i];
                            if (visitedJunctions.ContainsKey(oppositeJuncEID))
                            {
                                continue;
                            }
                            else if (oppositeJuncEID > 0)
                            {
                                junctionEIDsToVisit.Enqueue(oppositeJuncEID);
                            }
                            addEdge(connectedEdgeEID, currentJunction, oppositeJuncEID, feederInfo, network);
                        }
                    }
                }
            }
            BuildAdjacentEdgeLists(feederInfo);
            return feederInfo;
        }

        #endregion
        private void addJunction(int EID, FeederInfo feeder, INetwork network)
        {
            INetElements netelem = (INetElements)network;
            int FCID = 0;
            int OID = 0;
            int SUBID = 0;
            netelem.QueryIDs(EID, esriElementType.esriETJunction, out FCID, out OID, out SUBID);
            JunctionInfo junctionInfo = new ElectricJunction(
                            FCID,
                            OID,
                            EID,
                            0);

            junctionInfo.Origin = ElementOrigin.Trace;
            ((ElectricJunction)junctionInfo).EnergizedPhases = SetOfPhases.abc;
            feeder.AddJunction(junctionInfo);
            if (_geoDatabase.GdbConfig.Classes.ContainsKey(junctionInfo.ObjectClassID)
                    && !feeder.JunctionFeatures.ContainsKey(junctionInfo.ObjectKey))
            {
                feeder.AddJunctionFeature(junctionInfo);
            }
        }
        private void addEdge(int EID, int FromEID, int ToEID, FeederInfo feeder, INetwork network)
        {
            INetElements netelem = (INetElements)network;
            int FCID = 0;
            int OID = 0;
            int SUBID = 0;
            netelem.QueryIDs(EID, esriElementType.esriETEdge, out FCID, out OID, out SUBID);
            ElectricEdge edgeInfo = new ElectricEdge(
                            FCID,
                            OID,
                            EID,
                            SUBID,
                            0);
            edgeInfo.Origin = ElementOrigin.Trace;
            edgeInfo.EnergizedPhases = SetOfPhases.abc;
            if (!feeder.JunctionElements.ContainsKey(FromEID))
            {
                addJunction(FromEID, feeder, network);
            }
            edgeInfo.FromJunction = feeder.JunctionElements[FromEID];
            if (!feeder.JunctionElements.ContainsKey(ToEID))
            {
                addJunction(ToEID, feeder, network);
            }
            edgeInfo.ToJunction = feeder.JunctionElements[ToEID];
            feeder.AddEdge(edgeInfo);
            if (_geoDatabase.GdbConfig.Classes.ContainsKey(edgeInfo.ObjectClassID))
            {
                feeder.AddEdgeFeature(edgeInfo);
            }
        }
        private static void BuildAdjacentEdgeLists(FeederInfo feeder)
        {
            //throw an error if there aren't any edges or junctions
            //but not for an empty feeder (there aren't any elements at all)
            if (feeder.JunctionElements == null ||
                feeder.EdgeElements == null)
            {
                throw new InvalidOperationException(string.Format("Feeder {0} is missing it's Edge and Junction collections.", feeder.FeederID));
            }

            foreach (EdgeInfo edge in feeder.EdgeElements.Values)
            {
                if (edge.FromJunction != null)
                {
                    if (!edge.FromJunction.AdjacentEdges.ContainsKey(edge.EID))
                    {
                        edge.FromJunction.AdjacentEdges.Add(edge.EID, edge);
                    }
                }

                if (edge.ToJunction != null)
                {
                    if (!edge.ToJunction.AdjacentEdges.ContainsKey(edge.EID))
                    {
                        edge.ToJunction.AdjacentEdges.Add(edge.EID, edge);
                    }
                }
            }
        }
        #region IFeederTracer Members

        public void QueryFeatures(FeatureQueryType queryType, IDictionary<string, Miner.Geodatabase.Integration.Electric.FeederInfo> feeders)
        {
            throw new NotImplementedException();
        }

        public Miner.Geodatabase.Integration.Electric.FeederInfo Trace(string feederID, bool includeDeEnergized)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICancel Members

        public bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                _cancel = value;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
