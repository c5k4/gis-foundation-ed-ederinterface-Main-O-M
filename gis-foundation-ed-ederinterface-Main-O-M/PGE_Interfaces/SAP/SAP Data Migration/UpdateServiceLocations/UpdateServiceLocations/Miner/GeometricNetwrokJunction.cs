using Miner.NetworkModel;
using System;

namespace Miner.Geodatabase.Network
{
    [Serializable]
    internal class GeometricNetworkJunction : IGeometricNetworkFeatureID, IJunction, IElement, IEquatable<GeometricNetworkJunction>, IComparable<GeometricNetworkJunction>
    {
        public bool Disabled
        {
            get;
            set;
        }

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
            get
            {
                return 0;
            }
            set
            {
            }
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

        public int ID
        {
            get;
            set;
        }

        public GeometricNetworkJunction()
        {
        }

        public GeometricNetworkJunction(int id, int weight, int classid, int objectid)
        {
            this.ID = id;
            this.WeightValue = weight;
            this.ObjectClassID = classid;
            this.ObjectID = objectid;
        }

        public bool Equals(GeometricNetworkJunction other)
        {
            return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || other.GetHashCode() == this.GetHashCode());
        }

        public int CompareTo(GeometricNetworkJunction other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj is GeometricNetworkJunction && obj.GetHashCode() == this.GetHashCode()));
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public static bool operator ==(GeometricNetworkJunction left, GeometricNetworkJunction right)
        {
            return object.ReferenceEquals(left, right) || (left != null && right != null && left.Equals(right));
        }

        public static bool operator !=(GeometricNetworkJunction left, GeometricNetworkJunction right)
        {
            return !(left == right);
        }
    }
}
