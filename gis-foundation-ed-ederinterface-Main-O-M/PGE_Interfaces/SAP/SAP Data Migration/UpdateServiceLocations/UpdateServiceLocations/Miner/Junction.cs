using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Miner.Geodatabase.FeederManager
{
    internal class Junction
    {
        private bool? _isIsland;

        public int EID
        {
            get;
            set;
        }

        public ISimpleJunctionFeature Feature
        {
            get;
            private set;
        }

        public bool IsIsland
        {
            get
            {
                if (!this._isIsland.HasValue)
                {
                    this._isIsland = new bool?(this.GetIslandStatus());
                }
                return this._isIsland.Value;
            }
            set
            {
                this._isIsland = new bool?(value);
            }
        }

        public Junction(IFeature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }
            this.Feature = (feature as ISimpleJunctionFeature);
            if (this.Feature == null)
            {
                throw new ArgumentException("Feature must be of type ISimpleJunctionFeature");
            }
            this.EID = this.Feature.EID;
        }

        public bool IsValid()
        {
            return this.EID >= 0 && this.Feature != null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Junction junction = obj as Junction;
            return junction != null && junction.EID == this.EID;
        }

        public override int GetHashCode()
        {
            return this.EID.GetHashCode();
        }

        public virtual JunctionCollection GetConnectedEdgeJunctions(Dictionary<int, Junction> allJunctions)
        {
            JunctionCollection junctionCollection = new JunctionCollection();
            for (int i = 0; i < this.Feature.EdgeFeatureCount; i++)
            {
                IEdgeFeature edgeFeature = this.Feature.get_EdgeFeature(i);
                IComplexEdgeFeature complexEdgeFeature = edgeFeature as IComplexEdgeFeature;
                if (complexEdgeFeature != null)
                {
                    for (int j = 0; j < complexEdgeFeature.JunctionFeatureCount; j++)
                    {
                        ISimpleJunctionFeature simpleJunctionFeature = complexEdgeFeature.get_JunctionFeature(j) as ISimpleJunctionFeature;
                        if (simpleJunctionFeature != null)
                        {
                            junctionCollection.Add(new Junction(simpleJunctionFeature as IFeature));
                        }
                    }
                }
                else
                {
                    junctionCollection.Add(new Junction(edgeFeature.FromJunctionFeature as IFeature));
                    junctionCollection.Add(new Junction(edgeFeature.ToJunctionFeature as IFeature));
                }
            }
            IEnumerable<Junction> enumerable = from kvp in allJunctions.AsEnumerable<KeyValuePair<int, Junction>>()
                                               where kvp.Value is JunctionProxy && ((JunctionProxy)kvp.Value).ActualEID == this.EID
                                               select kvp.Value;
            foreach (Junction current in enumerable)
            {
                junctionCollection.Add(current);
            }
            return junctionCollection;
        }

        private bool GetIslandStatus()
        {
            bool result = true;
            IFeature feature = this.Feature as IFeature;
            FeederManagerFieldModelNames feederManagerFieldModelNames = new FeederManagerFieldModelNames(feature.Class as IFeatureClass);
            int feederInfoFieldIndex = feederManagerFieldModelNames.FeederInfoFieldIndex;
            if (feederInfoFieldIndex >= 0)
            {
                object obj = feature.get_Value(feederInfoFieldIndex);
                if (obj != DBNull.Value)
                {
                    int num = Convert.ToInt32(obj);
                    result = ((num & 8) != 0);
                }
            }
            else
            {
                result = this.GetIslandStatusFromAdjacentEdges();
            }
            return result;
        }

        private bool GetIslandStatusFromAdjacentEdges()
        {
            bool result = true;
            if (this.Feature.EdgeFeatureCount > 0)
            {
                IFeature feature = this.Feature.get_EdgeFeature(0) as IFeature;
                FeederManagerFieldModelNames feederManagerFieldModelNames = new FeederManagerFieldModelNames(feature.Class as IFeatureClass);
                int feederInfoFieldIndex = feederManagerFieldModelNames.FeederInfoFieldIndex;
                if (feederInfoFieldIndex >= 0)
                {
                    object obj = feature.get_Value(feederInfoFieldIndex);
                    if (obj != DBNull.Value)
                    {
                        int num = Convert.ToInt32(obj);
                        result = ((num & 8) != 0);
                    }
                }
            }
            return result;
        }
    }
}
