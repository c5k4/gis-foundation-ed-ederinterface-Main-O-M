using Miner.NetworkModel;
using System;

namespace Miner.Geodatabase.Network
{
    [Serializable]
    internal class GeometricNetworkEdge : IGeometricNetworkFeatureID, IEdge, IElement, IEquatable<GeometricNetworkEdge>, IComparable<GeometricNetworkEdge>
    {
        public int ObjectClassID
        {
            get;
            set;
        }

        public int ObjectID
        {
            get;
            set;
        }

        public int SubId
        {
            get;
            set;
        }

        public FeatureKey Key
        {
            get
            {
                return new FeatureKey(this.ObjectClassID, this.ObjectID);
            }
        }

        public int WeightValue
        {
            get;
            set;
        }

        public bool Disabled
        {
            get;
            set;
        }

        public int ID
        {
            get;
            set;
        }

        public IJunction JunctionOne
        {
            get;
            set;
        }

        public IJunction JunctionTwo
        {
            get;
            set;
        }

        public GeometricNetworkEdge()
        {
        }

        public GeometricNetworkEdge(int id, int weight, int classid, int objectid, IJunction j1, IJunction j2)
        {
            this.ID = id;
            this.WeightValue = weight;
            this.ObjectClassID = classid;
            this.ObjectID = objectid;
            this.JunctionOne = j1;
            this.JunctionTwo = j2;
            this.SubId = -1;
        }

        public GeometricNetworkJunction GetOther(GeometricNetworkJunction junction)
        {
            if (this.JunctionOne.Equals(junction))
            {
                return (GeometricNetworkJunction)this.JunctionTwo;
            }
            if (this.JunctionTwo.Equals(junction))
            {
                return (GeometricNetworkJunction)this.JunctionOne;
            }
            return null;
        }

        public bool Equals(GeometricNetworkEdge other)
        {
            return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || other.GetHashCode() == this.GetHashCode());
        }

        public int CompareTo(GeometricNetworkEdge other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj is GeometricNetworkEdge && obj.GetHashCode() == this.GetHashCode()));
        }

        public override int GetHashCode()
        {
            return -this.ID;
        }

        public static bool operator ==(GeometricNetworkEdge left, GeometricNetworkEdge right)
        {
            return object.ReferenceEquals(left, right) || (left != null && right != null && left.Equals(right));
        }

        public static bool operator !=(GeometricNetworkEdge left, GeometricNetworkEdge right)
        {
            return !(left == right);
        }
    }
}
