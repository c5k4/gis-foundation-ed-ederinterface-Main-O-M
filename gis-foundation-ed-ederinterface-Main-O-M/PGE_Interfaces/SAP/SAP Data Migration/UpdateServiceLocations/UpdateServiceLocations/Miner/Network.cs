using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Miner.NetworkModel
{
    internal class Network<TJunction, TEdge> : INetwork<TJunction, TEdge>
        where TJunction : class, IJunction
        where TEdge : class, IEdge
    {
        internal class AdjacencyList : List<IAdjacency<TJunction, TEdge>>
        {
            public bool Complete
            {
                get;
                set;
            }
        }

        private IDictionary<int, TJunction> _junctions = new Dictionary<int, TJunction>();

        private IDictionary<int, TEdge> _edges = new Dictionary<int, TEdge>();

        private IDictionary<int, Network<TJunction, TEdge>.AdjacencyList> _adjacencies = new Dictionary<int, Network<TJunction, TEdge>.AdjacencyList>();

        public static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<TJunction> Junctions
        {
            get
            {
                return this._junctions.Values;
            }
        }

        public IEnumerable<TEdge> Edges
        {
            get
            {
                return this._edges.Values;
            }
        }

        public int JunctionCount
        {
            get
            {
                return this._junctions.Count;
            }
        }

        public int EdgeCount
        {
            get
            {
                return this._edges.Count;
            }
        }

        protected IDictionary<int, Network<TJunction, TEdge>.AdjacencyList> Adjacencies
        {
            get
            {
                return this._adjacencies;
            }
        }

        public void Allocate(int initialCapacity)
        {
            this._junctions = new Dictionary<int, TJunction>(initialCapacity);
            this._edges = new Dictionary<int, TEdge>(initialCapacity);
            this._adjacencies = new Dictionary<int, Network<TJunction, TEdge>.AdjacencyList>(initialCapacity);
        }

        public bool Add(TJunction junction)
        {
            if (junction == null)
            {
                throw new ArgumentNullException("junction");
            }
            if (this._junctions.ContainsKey(junction.ID))
            {
                return false;
            }
            this._adjacencies[junction.ID] = new Network<TJunction, TEdge>.AdjacencyList();
            this._junctions[junction.ID] = junction;
            return true;
        }

        public bool Add(TEdge edge)
        {
            if (edge == null)
            {
                throw new ArgumentNullException("edge");
            }
            if (this._edges.ContainsKey(edge.ID))
            {
                return false;
            }
            this.Add((TJunction)((object)edge.JunctionOne));
            this.Add((TJunction)((object)edge.JunctionTwo));
            this._edges[edge.ID] = edge;
            this._adjacencies[edge.JunctionOne.ID].Add(new Adjacency<TJunction, TEdge>(edge, this.GetJunction(edge.JunctionTwo.ID)));
            this._adjacencies[edge.JunctionTwo.ID].Add(new Adjacency<TJunction, TEdge>(edge, this.GetJunction(edge.JunctionOne.ID)));
            return true;
        }

        public void AddRange(IEnumerable<TEdge> edges)
        {
            foreach (TEdge current in edges)
            {
                this.Add(current);
            }
        }

        public void AddRange(IEnumerable<TJunction> junctions)
        {
            foreach (TJunction current in junctions)
            {
                this.Add(current);
            }
        }

        public virtual bool RemoveJunction(int id)
        {
            if (!this._junctions.Remove(id))
            {
                return false;
            }
            Network<TJunction, TEdge>.AdjacencyList adjacencyList = this._adjacencies[id];
            this._adjacencies.Remove(id);
            foreach (IAdjacency<TJunction, TEdge> current in adjacencyList)
            {
                this.RemoveReverseAdjacency(current, id);
                TEdge edge = current.Edge;
                this.RemoveEdge(edge.ID);
            }
            return true;
        }

        public virtual bool RemoveEdge(int edgeID)
        {
            TEdge edge = this.GetEdge(edgeID);
            bool flag = this._edges.Remove(edgeID);
            if (flag)
            {
                this.RemoveAdjacency(edge.JunctionOne.ID, edgeID);
                this.RemoveAdjacency(edge.JunctionTwo.ID, edgeID);
            }
            return flag;
        }

        public virtual TJunction GetJunction(int id)
        {
            TJunction result;
            this._junctions.TryGetValue(id, out result);
            return result;
        }

        public virtual TEdge GetEdge(int edgeID)
        {
            TEdge result;
            this._edges.TryGetValue(edgeID, out result);
            return result;
        }

        public virtual IEnumerable<IAdjacency<TJunction, TEdge>> GetAdjacencies(int junctionID)
        {
            Network<TJunction, TEdge>.AdjacencyList result;
            this._adjacencies.TryGetValue(junctionID, out result);
            return result;
        }

        public virtual void Clear()
        {
            this._edges.Clear();
            this._junctions.Clear();
            this._adjacencies.Clear();
        }

        private bool RemoveAdjacency(int junctionID, int edgeID)
        {
            bool result = false;
            Network<TJunction, TEdge>.AdjacencyList adjacencyList;
            this._adjacencies.TryGetValue(junctionID, out adjacencyList);
            if (adjacencyList != null)
            {
                IAdjacency<TJunction, TEdge> item = adjacencyList.FirstOrDefault(delegate(IAdjacency<TJunction, TEdge> adj)
                {
                    TEdge edge = adj.Edge;
                    return edge.ID == edgeID;
                });
                result = adjacencyList.Remove(item);
                adjacencyList.Complete = false;
            }
            return result;
        }

        private void RemoveReverseAdjacency(IAdjacency<TJunction, TEdge> adjacency, int junctionID)
        {
            IDictionary<int, Network<TJunction, TEdge>.AdjacencyList> arg_27_0 = this._adjacencies;
            TJunction junction = adjacency.Junction;
            Network<TJunction, TEdge>.AdjacencyList adjacencyList = arg_27_0[junction.ID];
            IAdjacency<TJunction, TEdge> item = adjacencyList.FirstOrDefault(delegate(IAdjacency<TJunction, TEdge> adj)
            {
                TJunction junction2 = adj.Junction;
                return junction2.ID == junctionID;
            });
            adjacencyList.Remove(item);
            adjacencyList.Complete = false;
        }
    }
}
