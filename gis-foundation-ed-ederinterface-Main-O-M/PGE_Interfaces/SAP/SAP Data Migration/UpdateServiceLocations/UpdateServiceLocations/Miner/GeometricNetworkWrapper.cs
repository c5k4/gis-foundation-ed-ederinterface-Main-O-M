using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.Adf;
using Miner.Interop;
using Miner.NetworkModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF;

namespace Miner.Geodatabase.Network
{
    internal class GeometricNetworkWrapper<TJunction, TEdge> : Network<TJunction, TEdge>, IFeatureMapper<FeatureKey>, INetworkFeatureProvider<FeatureKey>, IDisposable
        where TJunction : class, IJunction
        where TEdge : class, IEdge
    {
        public delegate TResult CreateEdgeFunc<TJunc, TResult>(int id, int weight, int classid, int objectid, int subid, TJunc j1, TJunc j2) where TJunc : IJunction;

        public delegate TResult CreateJunctionFunc<TResult>(int id, int weight, int classid, int objectid);

        private IGeometricNetwork _geometricNetwork;

        private INetWeight _netWeight;

        private Dictionary<int, esriElementType> _elementTypeByObjectClassID = new Dictionary<int, esriElementType>();

        private IForwardStar _forwardStar;

        private INetAttributes _netAttributes;

        private INetTopology _netTopology;

        private INetElements _netElements;

        protected internal bool AlreadyDisposed;

        public GeometricNetworkWrapper<TJunction, TEdge>.CreateJunctionFunc<TJunction> CreateJunction
        {
            get;
            set;
        }

        public GeometricNetworkWrapper<TJunction, TEdge>.CreateEdgeFunc<TJunction, TEdge> CreateEdge
        {
            get;
            set;
        }

        internal int? Capacity
        {
            get;
            set;
        }

        public IGeometricNetwork InnerGeometricNetwork
        {
            get
            {
                return this._geometricNetwork;
            }
        }

        public bool RespectEnabled
        {
            get;
            set;
        }

        protected INetElements NetElements
        {
            get
            {
                if (this._netElements == null)
                {
                    this._netElements = (this._geometricNetwork.Network as INetElements);
                    if (this._netElements == null)
                    {
                        throw new SystemException("Network does not support INetElements");
                    }
                }
                return this._netElements;
            }
        }

        private IForwardStar ForwardStar
        {
            get
            {
                if (this._forwardStar == null && this._geometricNetwork != null)
                {
                    this._forwardStar = this._geometricNetwork.Network.CreateForwardStar(false, this._netWeight, this._netWeight, this._netWeight, null);
                }
                return this._forwardStar;
            }
        }

        private INetAttributes NetAttributes
        {
            get
            {
                if (this._netAttributes == null)
                {
                    this._netAttributes = (this._geometricNetwork.Network as INetAttributes);
                    if (this._netAttributes == null)
                    {
                        throw new SystemException("Network does not support INetAttributes");
                    }
                }
                return this._netAttributes;
            }
        }

        private INetTopology NetTopology
        {
            get
            {
                if (this._netTopology == null)
                {
                    this._netTopology = (this._geometricNetwork.Network as INetTopology);
                    if (this._netTopology == null)
                    {
                        throw new SystemException("Network does not support INetTopology");
                    }
                }
                return this._netTopology;
            }
        }

        protected internal GeometricNetworkWrapper(IGeometricNetwork network)
            : this(network, null)
        {
        }

        public GeometricNetworkWrapper(IGeometricNetwork network, string weightName)
        {
            if (network == null)
            {
                throw new ArgumentNullException("network");
            }
            this._geometricNetwork = network;
            if (!string.IsNullOrEmpty(weightName))
            {
                INetSchema netSchema = (INetSchema)network.Network;
                this._netWeight = netSchema.get_WeightByName(weightName);
            }
            this.PopulateElementTypeByObjectClassID();
            this.CreateEdge = ((int id, int weight, int classid, int objectid, int subid, TJunction j1, TJunction j2) => new GeometricNetworkEdge(id, weight, classid, objectid, j1, j2)
            {
                SubId = subid
            } as TEdge);
            this.CreateJunction = ((int id, int weight, int classid, int objectid) => new GeometricNetworkJunction(id, weight, classid, objectid) as TJunction);
            int num = 100000;
            MMRegistry mMRegistry = new MMRegistry();
            if (mMRegistry.Open(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "Feeder Manager"))
            {
                num = Convert.ToInt32(mMRegistry.Read("NetworkCacheSize", 100000));
            }
            if (num == 100000 && mMRegistry.Open(mmHKEY.mmHKEY_LOCAL_MACHINE, mmBaseKey.mmArcFM, "Feeder Manager"))
            {
                num = Convert.ToInt32(mMRegistry.Read("NetworkCacheSize", 100000));
            }
            if (num > -1)
            {
                this.Capacity = new int?(num);
            }
        }

        private void CheckCapacity()
        {
            if (this.Capacity.HasValue)
            {
                int junctionCount = base.JunctionCount;
                if (!(junctionCount >= this.Capacity - 2))
                {
                    int edgeCount = base.EdgeCount;
                    if (!(edgeCount >= this.Capacity - 1))
                    {
                        return;
                    }
                }
                this.Clear();
            }
        }

        public IList<IElement> GetElements(FeatureKey featureID)
        {
            List<IElement> list = new List<IElement>();
            esriElementType esriElementType;
            if (this._elementTypeByObjectClassID.TryGetValue(featureID.ObjectClassID, out esriElementType))
            {
                INetElements netElements = this.NetElements;
                if (netElements != null)
                {
                    IEnumNetEID eIDs = netElements.GetEIDs(featureID.ObjectClassID, featureID.ObjectID, esriElementType);
                    eIDs.Reset();
                    for (int i = 0; i < eIDs.Count; i++)
                    {
                        int num = eIDs.Next();
                        int weight = this.GetWeight(num, esriElementType);
                        IElement element;
                        if (esriElementType == esriElementType.esriETJunction)
                        {
                            element = this.GetJunction(num, new int?(weight));
                        }
                        else
                        {
                            int nearJunctionID;
                            int adjacentJunctionID;
                            this.NetTopology.GetFromToJunctionEIDs(num, out nearJunctionID, out adjacentJunctionID);
                            element = this.GetEdge(num, nearJunctionID, adjacentJunctionID, new int?(weight), null);
                        }
                        if (element != null)
                        {
                            list.Add(element);
                        }
                    }
                }
            }
            return list;
        }

        public FeatureKey GetFeatureKey(IElement element)
        {
            IGeometricNetworkFeatureID geometricNetworkFeatureID = element as IGeometricNetworkFeatureID;
            if (geometricNetworkFeatureID != null)
            {
                return geometricNetworkFeatureID.Key;
            }
            FeatureKey result = null;
            INetElements netElements = this.NetElements;
            if (netElements != null)
            {
                esriElementType elementType = esriElementType.esriETJunction;
                if (!(element is IJunction))
                {
                    if (element is IEdge)
                    {
                        elementType = esriElementType.esriETEdge;
                    }
                    else if (element is ElementID)
                    {
                        elementType = ((ElementID)element).ElementType;
                    }
                }
                try
                {
                    int objectClassID;
                    int objectID;
                    int num;
                    netElements.QueryIDs(element.ID, elementType, out objectClassID, out objectID, out num);
                    result = new FeatureKey(objectClassID, objectID);
                }
                catch (COMException ex)
                {
                    if (ex.ErrorCode != -2147205088)
                    {
                        throw new COMException("INetElements.QueryIDs", ex);
                    }
                }
            }
            return result;
        }

        private TJunction GetJunction(int id, int? weight)
        {
            TJunction tJunction = base.GetJunction(id);
            if (tJunction == null)
            {
                this.CheckCapacity();
                if (this.NetElements.IsValidElement(id, esriElementType.esriETJunction))
                {
                    if (!weight.HasValue)
                    {
                        weight = new int?(this.GetWeight(id, esriElementType.esriETJunction));
                    }
                    int classid;
                    int objectid;
                    int num;
                    this.NetElements.QueryIDs(id, esriElementType.esriETJunction, out classid, out objectid, out num);
                    if (this.CreateJunction != null)
                    {
                        tJunction = this.CreateJunction(id, weight.Value, classid, objectid);
                    }
                    IGeometricNetworkFeatureID geometricNetworkFeatureID = tJunction as IGeometricNetworkFeatureID;
                    if (geometricNetworkFeatureID != null)
                    {
                        geometricNetworkFeatureID.Disabled = this.GetDisabledState(id, esriElementType.esriETJunction);
                    }
                    try
                    {
                        base.Add(tJunction);
                    }
                    catch (OutOfMemoryException)
                    {
                        this.Clear();
                        base.Add(tJunction);
                    }
                }
            }
            return tJunction;
        }

        public override TJunction GetJunction(int id)
        {
            return this.GetJunction(id, null);
        }

        private bool GetDisabledState(int id, esriElementType elementType)
        {
            return this.RespectEnabled && this.NetAttributes.GetDisabledState(id, elementType);
        }

        public override TEdge GetEdge(int edgeID)
        {
            TEdge edge = base.GetEdge(edgeID);
            if (edge == null && this.NetElements.IsValidElement(edgeID, esriElementType.esriETEdge))
            {
                int weight = this.GetWeight(edgeID, esriElementType.esriETEdge);
                int nearJunctionID;
                int adjacentJunctionID;
                this.NetTopology.GetFromToJunctionEIDs(edgeID, out nearJunctionID, out adjacentJunctionID);
                edge = this.GetEdge(edgeID, nearJunctionID, adjacentJunctionID, new int?(weight), null);
            }
            return edge;
        }

        public override IEnumerable<IAdjacency<TJunction, TEdge>> GetAdjacencies(int junctionID)
        {
            bool flag = true;
            Network<TJunction, TEdge>.AdjacencyList adjacencyList;
            if (!base.Adjacencies.TryGetValue(junctionID, out adjacencyList) || !adjacencyList.Complete)
            {
                flag = this.FetchAdjacencies(junctionID);
                if (base.Adjacencies.TryGetValue(junctionID, out adjacencyList))
                {
                    adjacencyList.Complete = true;
                }
            }
            if (!flag)
            {
                return null;
            }
            return base.GetAdjacencies(junctionID) ?? new List<IAdjacency<TJunction, TEdge>>();
        }

        private bool FetchAdjacencies(int junctionID)
        {
            if (!this.NetElements.IsValidElement(junctionID, esriElementType.esriETJunction))
            {
                return false;
            }
            int num;
            this.ForwardStar.FindAdjacent(0, junctionID, out num);
            for (int i = 0; i < num; i++)
            {
                int eid = -1;
                int adjacentJunctionID = -1;
                bool flag = false;
                object o = null;
                object o2 = null;
                this.ForwardStar.QueryAdjacentEdge(i, out eid, out flag, out o);
                this.ForwardStar.QueryAdjacentJunction(i, out adjacentJunctionID, out o2);
                this.GetEdge(eid, junctionID, adjacentJunctionID, o.ToNullableInt(), o2.ToNullableInt());
            }
            return true;
        }

        private void PopulateElementTypeByObjectClassID()
        {
            IFeatureClassContainer featureClassContainer = this._geometricNetwork as IFeatureClassContainer;
            if (featureClassContainer != null)
            {
                for (int i = 0; i < featureClassContainer.ClassCount; i++)
                {
                    INetworkClass networkClass = featureClassContainer.get_Class(i) as INetworkClass;
                    if (networkClass != null)
                    {
                        this._elementTypeByObjectClassID[networkClass.ObjectClassID] = ((networkClass.FeatureType == esriFeatureType.esriFTSimpleJunction) ? esriElementType.esriETJunction : esriElementType.esriETEdge);
                    }
                }
            }
        }

        private TEdge GetEdge(int eid, int nearJunctionID, int adjacentJunctionID, int? weight, int? adjacentJunctionWeight = null)
        {
            TEdge tEdge = base.GetEdge(eid);
            if (tEdge != null)
            {
                return tEdge;
            }
            this.CheckCapacity();
            TJunction junction = this.GetJunction(nearJunctionID);
            TJunction junction2 = this.GetJunction(adjacentJunctionID, adjacentJunctionWeight);
            int classid;
            int objectid;
            int num;
            this.NetElements.QueryIDs(eid, esriElementType.esriETEdge, out classid, out objectid, out num);
            if (!weight.HasValue)
            {
                weight = new int?(this.GetWeight(eid, esriElementType.esriETEdge));
            }
            TJunction j = junction;
            TJunction j2 = junction2;
            int num2;
            int num3;
            this.NetTopology.GetFromToJunctionEIDs(eid, out num2, out num3);
            if (num2 != nearJunctionID)
            {
                j = junction2;
                j2 = junction;
            }
            if (this.CreateEdge != null)
            {
                tEdge = this.CreateEdge(eid, weight.Value, classid, objectid, num, j, j2);
            }
            if (tEdge == null)
            {
                return default(TEdge);
            }
            IGeometricNetworkFeatureID geometricNetworkFeatureID = tEdge as IGeometricNetworkFeatureID;
            if (geometricNetworkFeatureID != null)
            {
                geometricNetworkFeatureID.Disabled = this.GetDisabledState(eid, esriElementType.esriETEdge);
                geometricNetworkFeatureID.SubId = num;
            }
            try
            {
                base.Add(tEdge);
            }
            catch (OutOfMemoryException)
            {
                this.Clear();
                base.Add(tEdge);
            }
            return tEdge;
        }

        protected internal virtual int GetWeight(int id, esriElementType et)
        {
            int result = 0;
            if (this._netWeight != null)
            {
                result = Convert.ToInt32(this.NetAttributes.GetWeightValue(id, et, this._netWeight.WeightID));
            }
            return result;
        }

        public IFeature GetFeature(FeatureKey key)
        {
            IFeatureClassContainer featureClassContainer = (IFeatureClassContainer)this._geometricNetwork;
            IFeatureClass featureClass = featureClassContainer.get_ClassByID(key.ObjectClassID);
            return featureClass.GetFeature(key.ObjectID);
        }

        public IEnumerable<IFeature> GetFeatures(IEnumerable<FeatureKey> keys)
        {
            Dictionary<int, IList<int>> dictionary = new Dictionary<int, IList<int>>();
            foreach (FeatureKey current in keys)
            {
                if (dictionary.ContainsKey(current.ObjectClassID))
                {
                    if (!dictionary[current.ObjectClassID].Contains(current.ObjectID))
                    {
                        dictionary[current.ObjectClassID].Add(current.ObjectID);
                    }
                }
                else
                {
                    dictionary[current.ObjectClassID] = new List<int>();
                    dictionary[current.ObjectClassID].Add(current.ObjectID);
                }
            }
            List<IFeature> list = new List<IFeature>();
            foreach (int current2 in dictionary.Keys)
            {
                IFeatureClassContainer featureClassContainer = (IFeatureClassContainer)this._geometricNetwork;
                IFeatureClass featureClass = featureClassContainer.get_ClassByID(current2);
                using (ComReleaser comReleaser = new ComReleaser())
                {
                    IFeatureCursor features = featureClass.GetFeatures(dictionary[current2].ToArray<int>(), false);
                    comReleaser.ManageLifetime(features);
                    for (IFeature item = features.NextFeature(); item != null; item = features.NextFeature())
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public virtual void Dispose(bool isDisposing)
        {
            if (this.AlreadyDisposed)
            {
                return;
            }
            if (this._netElements != null && Marshal.IsComObject(this._netElements))
            {
                Marshal.FinalReleaseComObject(this._netElements);
                this._netElements = null;
            }
            if (this._forwardStar != null && Marshal.IsComObject(this._forwardStar))
            {
                Marshal.FinalReleaseComObject(this._forwardStar);
                this._forwardStar = null;
            }
            if (this._netAttributes != null && Marshal.IsComObject(this._netAttributes))
            {
                Marshal.FinalReleaseComObject(this._netAttributes);
                this._netAttributes = null;
            }
            if (this._netTopology != null && Marshal.IsComObject(this._netTopology))
            {
                Marshal.FinalReleaseComObject(this._netTopology);
                this._netTopology = null;
            }
            if (this._geometricNetwork != null && Marshal.IsComObject(this._geometricNetwork))
            {
                //Marshal.FinalReleaseComObject(this._geometricNetwork);
                //this._geometricNetwork = null;
            }
            if (this._netWeight != null && Marshal.IsComObject(this._netWeight))
            {
                Marshal.FinalReleaseComObject(this._netWeight);
                this._netWeight = null;
            }
            if (this._elementTypeByObjectClassID != null)
            {
                this._elementTypeByObjectClassID.Clear();
                this._elementTypeByObjectClassID = null;
            }
            this.AlreadyDisposed = true;
        }
    }
}
