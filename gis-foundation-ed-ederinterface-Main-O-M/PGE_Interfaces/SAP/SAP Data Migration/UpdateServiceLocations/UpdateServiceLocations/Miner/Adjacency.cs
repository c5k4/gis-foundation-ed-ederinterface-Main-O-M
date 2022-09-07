using System;
using System.Diagnostics;

namespace Miner.NetworkModel
{
    [DebuggerDisplay("Edge:{Edge==null?-1:Edge.ID} Junction:{Junction==null?-1:Junction.ID}")]
    [Serializable]
    internal class Adjacency<TJunction, TEdge> : IAdjacency<TJunction, TEdge>, IEquatable<Adjacency<TJunction, TEdge>>
        where TJunction : class, IJunction
        where TEdge : class, IEdge
    {
        public TEdge Edge
        {
            get;
            private set;
        }

        public TJunction Junction
        {
            get;
            private set;
        }

        public Adjacency(TEdge edge, TJunction junction)
        {
            this.Edge = edge;
            this.Junction = junction;
        }

        public bool Equals(Adjacency<TJunction, TEdge> other)
        {
            if (object.ReferenceEquals(null, other))
            {
                return false;
            }
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (!object.ReferenceEquals(this.Edge, null) || !object.ReferenceEquals(other.Edge, null))
            {
                if (!object.ReferenceEquals(this.Edge, null) && !object.ReferenceEquals(other.Edge, null))
                {
                    TEdge edge = this.Edge;
                    int arg_90_0 = edge.ID;
                    TEdge edge2 = other.Edge;
                    if (arg_90_0 == edge2.ID)
                    {
                        goto IL_92;
                    }
                }
                return false;
            }
        IL_92:
            if (object.ReferenceEquals(this.Junction, null) && object.ReferenceEquals(other.Junction, null))
            {
                return true;
            }
            if (!object.ReferenceEquals(this.Junction, null) && !object.ReferenceEquals(other.Junction, null))
            {
                TJunction junction = this.Junction;
                int arg_106_0 = junction.ID;
                TJunction junction2 = other.Junction;
                return arg_106_0 == junction2.ID;
            }
            return false;
        }
    }
}
