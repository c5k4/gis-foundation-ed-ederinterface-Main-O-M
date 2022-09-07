using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.Geodatabase.AutoUpdaters;
using Miner.Geodatabase.CathodicProtection;
using Miner.Geodatabase.FeederManager;
using Miner.Geodatabase.Network;
using Miner.Interop;
using Miner.NetworkModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Miner.Geodatabase.Edit
{
    public class Connect
    {
        public static bool isConnecting;

        private BulkEditEventHandler _bulkEdits;

        private Dictionary<int, JunctionCollection> _junctionSetByEID;

        private Dictionary<int, Junction> _junctionsByEID;

        private int _fakeJunctionEID = -1;

        private double NetworkTolerance
        {
            get;
            set;
        }

        private IGeometricNetwork Network
        {
            get
            {
                return this._bulkEdits.GeometricNetwork;
            }
            set
            {
                this._bulkEdits.GeometricNetwork = value;
            }
        }

        public Connect()
        {
            this._bulkEdits = new BulkEditEventHandler();
        }

        public void ConnectFeatures(List<IFeature> electricFeatures)
        {
            if (electricFeatures == null)
            {
                throw new ArgumentNullException("electricFeatures");
            }
            if (!Editor.IsOperationInProgress())
            {
                throw new InvalidOperationException("An edit operation must be started to connect features.");
            }
            JunctionsByLocation junctionsByLocation = this.GetJunctionsByLocation(electricFeatures);
            try
            {
                Connect.isConnecting = true;
                if (this._bulkEdits.FeederManagerAUsEnabled())
                {
                    this.FindNewNonIslandFeatures(junctionsByLocation, this.Network);
                    this._bulkEdits.BeforeEdits(electricFeatures);
                }
                this.ConnectJunctions(junctionsByLocation);
                //if (junctionsByLocation.Count > 0)
                //{
                //    IFeature feature = electricFeatures.First<IFeature>();
                //    FeatureKey key = new FeatureKey(feature.Class.ObjectClassID, feature.OID);
                //    IWorkspace workspace = feature.Class.GetWorkspace();
                //    //FeederWorkspaceExtension instance = FeederWorkspaceExtension.GetInstance(new ConnectionProperties(workspace));
                //    //instance.ClearCacheForFeature(key);
                //}
            }
            finally
            {
                Connect.isConnecting = false;
            }
        }

        public void AfterConnect(List<IFeature> electricFeatures)
        {
            if (!Editor.IsOperationInProgress())
            {
                throw new InvalidOperationException("An edit operation must be started to connect features.");
            }
            try
            {
                Connect.isConnecting = true;
                if (this._bulkEdits.FeederManagerAUsEnabled())
                {
                    this._bulkEdits.UpdateFeederLevels(electricFeatures, mmEditEvent.mmEventFeatureCreate);
                    this._bulkEdits.AfterEdits(electricFeatures);
                }
            }
            finally
            {
                Connect.isConnecting = false;
            }
        }


        private void ConnectJunctions(JunctionsByLocation junctions)
        {
            foreach (KeyValuePair<Location, JunctionCollection> current in junctions)
            {
                JunctionCollection value = current.Value;
                Connect.ConnectJunctions(value);
            }
        }

        private static void ConnectJunctions(JunctionCollection junctionSet)
        {

                int i = 0;
                HashSet<Junction>.Enumerator enumerator = junctionSet.GetEnumerator();
                enumerator.MoveNext();
                Junction current = enumerator.Current;
                while (i < junctionSet.Count)
                {
                    INetworkFeature networkFeature = current.Feature as INetworkFeature;
                    if (networkFeature != null)
                    {
                        INetElements netElements = networkFeature.GeometricNetwork.Network as INetElements;
                        if (netElements.IsValidElement(current.EID, esriElementType.esriETJunction))
                        {
                            try
                            {
                                networkFeature.Connect();
                            }
                            catch (Exception ex)
                            {

                            }
                            i++;
                            //Connect.CheckForSwitchableDeviceConnectedToTooManyEdges(networkFeature);
                        }
                        else
                        {
                            enumerator.MoveNext();
                            current = enumerator.Current;
                        }
                    }
                }
        }

        private static void CheckForSwitchableDeviceConnectedToTooManyEdges(INetworkFeature junctionFeature)
        {
            GeometricNetworkWrapper<GeometricNetworkJunction, GeometricNetworkEdge> geometricNetworkWrapper = new GeometricNetworkWrapper<GeometricNetworkJunction, GeometricNetworkEdge>(junctionFeature.GeometricNetwork, ElectricTraceWeight.WeightName);
            int eID = ((ISimpleJunctionFeature)junctionFeature).EID;
            GeometricNetworkJunction junction = geometricNetworkWrapper.GetJunction(eID);
            if (junction != null && new JunctionElectricTraceWeight(junction.WeightValue).IsSwitch)
            {
                IEnumerable<IAdjacency<GeometricNetworkJunction, GeometricNetworkEdge>> adjacencies = geometricNetworkWrapper.GetAdjacencies(eID);
                if (adjacencies.Count<IAdjacency<GeometricNetworkJunction, GeometricNetworkEdge>>() > 2 && MinerRuntimeEnvironment.IsUserInterfaceSupported)
                {
                    string arg = Connect.ReportJunctionAndAdjacentEdgesAsString(junctionFeature, adjacencies);
                    Resourcer resourcer = new Resourcer();
                    string brandedString = resourcer.GetBrandedString("ConnectCommand.Name");
                    string @string = resourcer.GetString("ConnectCommand.SwitchableDeviceHasTooManyEdges");
                    string message = string.Format("{0}{1}", @string, arg);
                    
                }
            }
        }

        private static string ReportJunctionAndAdjacentEdgesAsString(INetworkFeature junctionFeature, IEnumerable<IAdjacency<GeometricNetworkJunction, GeometricNetworkEdge>> adjacencies)
        {
            IFeatureClassContainer featureClassContainer = junctionFeature.GeometricNetwork as IFeatureClassContainer;
            string text = string.Empty;
            IObject @object = (IObject)junctionFeature;
            text += string.Format("\t{0} ObjectID = {1}", @object.Class.AliasName, @object.OID);
            foreach (IAdjacency<GeometricNetworkJunction, GeometricNetworkEdge> current in adjacencies)
            {
                GeometricNetworkEdge edge = current.Edge;
                IFeatureClass featureClass = featureClassContainer.get_ClassByID(edge.ObjectClassID);
                if (featureClass != null)
                {
                    text += string.Format("\n\t{0} ObjectID = {1}", featureClass.AliasName, edge.ObjectID);
                }
            }
            return text;
        }

        private void FindNewNonIslandFeatures(JunctionsByLocation junctions, IGeometricNetwork network)
        {
            if (junctions == null)
            {
                return;
            }
            if (network == null)
            {
                return;
            }
            int[] junctionsForVisit = this.GetJunctionsForVisit(junctions);
            IMMFeederBulkEditing2 iMMFeederBulkEditing = (IMMFeederBulkEditing2)this._bulkEdits.FeederTracer;
            iMMFeederBulkEditing.FindIslandFeatures(network, this._bulkEdits.TraceSettings, null, junctionsForVisit);
        }

        private JunctionsByLocation GetJunctionsByLocation(List<IFeature> electricFeatures)
        {
            this.Network = null;
            this._junctionSetByEID = new Dictionary<int, JunctionCollection>();
            this._junctionsByEID = new Dictionary<int, Junction>();
            JunctionsByLocation junctionsByLocation = new JunctionsByLocation();
            foreach (IFeature current in electricFeatures)
            {
                if (current.FeatureType == esriFeatureType.esriFTSimpleJunction)
                {
                    junctionsByLocation = this.AddJunction(current, junctionsByLocation);
                }
                else if (current.FeatureType == esriFeatureType.esriFTComplexEdge || current.FeatureType == esriFeatureType.esriFTSimpleEdge)
                {
                    junctionsByLocation = this.AddJunctionsForEdge(current, junctionsByLocation);
                }
            }
            return junctionsByLocation;
        }

        private JunctionProxy GetProxyForEdgeJunction(JunctionCollection junctions, IFeature edge)
        {
            if (junctions == null)
            {
                return null;
            }
            if (junctions.Count < 1)
            {
                return null;
            }
            HashSet<Junction>.Enumerator enumerator = junctions.GetEnumerator();
            enumerator.MoveNext();
            Junction current = enumerator.Current;
            if (current == null)
            {
                return null;
            }
            if (this.GetElectricNetworkFeature(edge) == null)
            {
                return null;
            }
            IFeature feature = current.Feature as IFeature;
            if (feature == null)
            {
                return null;
            }
            List<string> connectedEdgeIDs = Connect.GetConnectedEdgeIDs(current);
            JunctionProxy junctionProxy = null;
            IEnumFeature enumFeature = this.Network.SearchForNetworkFeature(feature.Shape as IPoint, esriFeatureType.esriFTComplexEdge);
            enumFeature.Reset();
            for (IFeature feature2 = enumFeature.Next(); feature2 != null; feature2 = enumFeature.Next())
            {
                int objectClassID = feature2.Class.ObjectClassID;
                int oID = feature2.OID;
                if (!connectedEdgeIDs.Contains(objectClassID + "," + oID))
                {
                    IEdgeFeature edgeFeature = (IEdgeFeature)feature2;
                    junctionProxy = new JunctionProxy(edgeFeature.FromJunctionFeature as IFeature, this._fakeJunctionEID--);
                    junctions.Add(junctionProxy);
                    this._junctionSetByEID[junctionProxy.EID] = junctions;
                    this._junctionsByEID[junctionProxy.EID] = junctionProxy;
                    break;
                }
            }
            return junctionProxy;
        }

        private JunctionsByLocation AddJunction(IFeature junctionFeature, JunctionsByLocation junctions)
        {
            if (junctionFeature.FeatureType == esriFeatureType.esriFTSimpleJunction)
            {
                INetworkFeature electricNetworkFeature = this.GetElectricNetworkFeature(junctionFeature);
                if (electricNetworkFeature != null)
                {
                    JunctionCollection junctions2 = junctions.GetJunctionsForLocation(this.GetLocation(junctionFeature));
                    junctions2 = this.AddJunctionToCollection(junctionFeature, junctions2);
                    IEnumFeature enumFeature = electricNetworkFeature.GeometricNetwork.SearchForNetworkFeature(junctionFeature.Shape as IPoint, esriFeatureType.esriFTSimpleJunction);
                    enumFeature.Reset();
                    for (IFeature feature = enumFeature.Next(); feature != null; feature = enumFeature.Next())
                    {
                        junctions2 = this.AddJunctionToCollection(feature, junctions2);
                    }
                }
            }
            return junctions;
        }

        private JunctionsByLocation AddJunctionsForEdge(IFeature feature, JunctionsByLocation junctions)
        {
            INetworkFeature electricNetworkFeature = this.GetElectricNetworkFeature(feature);
            if (electricNetworkFeature != null)
            {
                HashSet<JunctionProxy> hashSet = new HashSet<JunctionProxy>();
                IPointCollection pointCollection = feature.Shape as IPointCollection;
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    IPoint point = pointCollection.get_Point(i);
                    JunctionCollection junctions2 = junctions.GetJunctionsForLocation(this.GetLocation(point));
                    IEnumFeature enumFeature = this.Network.SearchForNetworkFeature(point, esriFeatureType.esriFTSimpleJunction);
                    enumFeature.Reset();
                    for (IFeature feature2 = enumFeature.Next(); feature2 != null; feature2 = enumFeature.Next())
                    {
                        junctions2 = this.AddJunctionToCollection(feature2, junctions2);
                    }
                    JunctionProxy proxyForEdgeJunction = this.GetProxyForEdgeJunction(junctions2, feature);
                    if (proxyForEdgeJunction != null)
                    {
                        hashSet.Add(proxyForEdgeJunction);
                    }
                }
                foreach (JunctionProxy current in hashSet)
                {
                    foreach (JunctionProxy current2 in hashSet)
                    {
                        current.AddJunction(current2);
                    }
                }
            }
            return junctions;
        }

        private JunctionCollection AddJunctionToCollection(IFeature feature, JunctionCollection junctions)
        {
            Junction junction = new Junction(feature);
            junctions.Add(junction);
            this._junctionSetByEID[junction.EID] = junctions;
            this._junctionsByEID[junction.EID] = junction;
            return junctions;
        }

        private INetworkFeature GetElectricNetworkFeature(IFeature feature)
        {
            INetworkFeature networkFeature = feature as INetworkFeature;
            if (networkFeature != null)
            {
                IGeometricNetwork geometricNetwork = networkFeature.GeometricNetwork;
                if (this.Network == null)
                {
                    if (!NetworkIdentification.IsElectric(geometricNetwork))
                    {
                        networkFeature = null;
                    }
                    else
                    {
                        this.Network = networkFeature.GeometricNetwork;
                        this.NetworkTolerance = Utility.GetXYTolerance(this.Network);
                    }
                }
                else if (NetworkIdentification.QualifiedName(this.Network) != NetworkIdentification.QualifiedName(geometricNetwork))
                {
                    networkFeature = null;
                }
            }
            return networkFeature;
        }

        private int[] GetJunctionsForVisit(JunctionsByLocation junctionsByLocation)
        {
            HashSet<int> hashSet = new HashSet<int>();
            foreach (KeyValuePair<Location, JunctionCollection> current in junctionsByLocation)
            {
                JunctionCollection value = current.Value;
                this.ProcessJunctionsForIslandsAndNonIslands(value, ref hashSet, true);
            }
            int[] array = new int[hashSet.Count];
            hashSet.CopyTo(array);
            return array;
        }

        private void GetJunctionsForVisit(ref HashSet<int> junctionsForVisit, Junction currentJunction, JunctionCollection junctions, bool addToSet)
        {
            foreach (Junction junction in junctions)
            {
                if (currentJunction.EID != junction.EID && !junctionsForVisit.Contains(junction.EID))
                {
                    JunctionCollection junctions2 = null;
                    if (this._junctionSetByEID.TryGetValue(junction.EID, out junctions2))
                    {
                        var junction1 = this._junctionsByEID[junction.EID];
                        if (junction1.IsIsland)
                        {
                            junction1.IsIsland = false;
                            if (addToSet)
                            {
                                junctionsForVisit = Connect.AddJunctionToSet(junction1, junctionsForVisit);
                            }
                            this.GetJunctionsForVisit(ref junctionsForVisit, junction1, junctions2, true);
                            this.ProcessJunctionsForIslandsAndNonIslands(junctions2, ref junctionsForVisit, false);
                            junctions2 = junction1.GetConnectedEdgeJunctions(this._junctionsByEID);
                            this.GetJunctionsForVisit(ref junctionsForVisit, junction1, junctions2, false);
                        }
                    }
                }
            }
        }

        private void ProcessJunctionsForIslandsAndNonIslands(JunctionCollection junctions, ref HashSet<int> junctionsForVisit, bool addToSet)
        {
            if (junctions.Processed)
            {
                return;
            }
            List<Junction> list = new List<Junction>();
            List<Junction> list2 = new List<Junction>();
            using (HashSet<Junction>.Enumerator enumerator = junctions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsIsland)
                    {
                        list.Add(enumerator.Current);
                    }
                    else
                    {
                        list2.Add(enumerator.Current);
                    }
                }
            }
            if (list.Count > 0 && list2.Count > 0)
            {
                junctions.Processed = true;
                for (int i = 0; i < list.Count; i++)
                {
                    Junction junction = list[i];
                    if (addToSet)
                    {
                        junctionsForVisit = Connect.AddJunctionToSet(junction, junctionsForVisit);
                    }
                    JunctionCollection connectedEdgeJunctions = junction.GetConnectedEdgeJunctions(this._junctionsByEID);
                    this.GetJunctionsForVisit(ref junctionsForVisit, junction, connectedEdgeJunctions, false);
                }
            }
        }

        private Location GetLocation(IFeature junctionFeature)
        {
            return this.GetLocation(junctionFeature.Shape as IPoint);
        }

        private Location GetLocation(IPoint point)
        {
            return new Location(point)
            {
                Tolerance = this.NetworkTolerance
            };
        }

        private static List<string> GetConnectedEdgeIDs(Junction junction)
        {
            List<string> list = new List<string>();
            ISimpleJunctionFeature feature = junction.Feature;
            if (feature != null)
            {
                for (int i = 0; i < feature.EdgeFeatureCount; i++)
                {
                    IFeature feature2 = feature.get_EdgeFeature(i) as IFeature;
                    int objectClassID = feature2.Class.ObjectClassID;
                    int oID = feature2.OID;
                    list.Add(objectClassID + "," + oID);
                }
            }
            return list;
        }

        private static HashSet<int> AddJunctionToSet(Junction junction, HashSet<int> junctions)
        {
            if (junction.IsValid())
            {
                junctions.Add(junction.EID);
            }
            else
            {
                JunctionProxy junctionProxy = junction as JunctionProxy;
                if (junctionProxy != null)
                {
                    junctions.Add(junctionProxy.ActualEID);
                }
            }
            return junctions;
        }
    }
}
