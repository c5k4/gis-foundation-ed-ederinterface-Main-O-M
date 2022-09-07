using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Electric;

namespace PGE.Interface.Integration.DMS.Tracers
{
    /// <summary>
    /// Class to find all the features downstream of a starting junction
    /// </summary>
    public class NADownstreamTrace
    {
        private List<EdgeInfo> _tracedEdges;
        private List<JunctionInfo> _tracedJunctions;
        private Queue<EdgeInfo> _edgesToBeTraced;
        private Dictionary<ObjectKey, bool> _junctionsTraced;
        private List<JunctionInfo> _openPoints;
        private bool _loops;
        private Dictionary<int, bool> _ignoredEdgeClass;
        private Dictionary<int, bool> _ignoredJunctionClass;

        /// <summary>
        /// Add the FCID of junction classes not to trace past
        /// </summary>
        public Dictionary<int, bool> IgnoredJunctionClass
        {
            get { return _ignoredJunctionClass; }
        }

        /// <summary>
        /// Add the FCID of edge classes not to trace past
        /// </summary>
        public Dictionary<int, bool> IgnoredEdgeClass
        {
            get { return _ignoredEdgeClass; }
        }

        /// <summary>
        /// True if any loops have been found while tracing
        /// </summary>
        public bool Loops
        {
            get { return _loops; }
            set { _loops = value; }
        }
        /// <summary>
        /// A list of all of the edges traced so far. Only calling Clear will reset this.
        /// </summary>
        public List<EdgeInfo> TracedEdges
        {
            get { return _tracedEdges; }
        }
        /// <summary>
        /// A list of all of the junctions traced so far. Only calling Clear will reset this.
        /// </summary>
        public List<JunctionInfo> TracedJunctions
        {
            get { return _tracedJunctions; }
        }
        /// <summary>
        /// Initialize internal data structures
        /// </summary>
        public NADownstreamTrace()
        {
            Clear();
            _ignoredEdgeClass = new Dictionary<int, bool>();
            _ignoredJunctionClass = new Dictionary<int, bool>();
        }
        /// <summary>
        /// Clear the list of traced edges and junctions
        /// </summary>
        public void Clear()
        {
            
            _tracedEdges = new List<EdgeInfo>();
            _tracedJunctions = new List<JunctionInfo>();
        }
        /// <summary>
        /// Trace downstream from a junction. Appends traced edges to TracedEdges and traced junctions to TracedJunctions.
        /// Call Clear to reset the lists of traced edges and junctions
        /// </summary>
        /// <param name="start">The junction to start tracing from. It is not included in the list of traced junctions</param>
        public void Trace(JunctionFeatureInfo start)
        {

            _edgesToBeTraced = new Queue<EdgeInfo>();
            _junctionsTraced = new Dictionary<ObjectKey, bool>();
            _openPoints = new List<JunctionInfo>();

            getEdges(start.Junction);

            while (_edgesToBeTraced.Count > 0)
            {
                EdgeInfo edge = _edgesToBeTraced.Dequeue();
                JunctionInfo toJunction = edge.ToJunction;
                if (_junctionsTraced.ContainsKey(toJunction.ObjectKey))
                {
                    _loops = true;
                    continue;
                }
                else
                {
                    //check if we are ignoring this junction
                    if (!_ignoredJunctionClass.ContainsKey(toJunction.ObjectClassID))
                    {
                        //need to check if this junction is open
                        if (IsOpen(toJunction))
                        {
                            _openPoints.Add(toJunction);
                            //if open don't trace down any further
                        }
                        else
                        {
                            _junctionsTraced.Add(toJunction.ObjectKey, true);
                            _tracedJunctions.Add(toJunction);
                            getEdges(toJunction);
                        }
                    }
                }
            }
            //now that we are finished tracing check the open points
            foreach (JunctionInfo open in _openPoints)
            {
                //if all of the connected edges have been traced, remove the open point
                bool remove = true;
                foreach (EdgeInfo edge in open.AdjacentEdges.Values)
                {
                    if (!_tracedEdges.Contains(edge))
                    {
                        remove = false;
                    }
                }
                if (remove)
                {
                    _tracedJunctions.Add(open);
                }
            }

        }

        /// <summary>
        /// Find all the downstream edges connected to the junction that are not ignored. Adds them
        /// to TracedEdges and the trace queue
        /// </summary>
        /// <param name="toJunction">The junction to check</param>
        private void getEdges(JunctionInfo toJunction)
        {
            List<EdgeInfo> edges = findDownstreamEdges(toJunction);

            foreach (EdgeInfo e in edges)
            {
                //check if we are not tracing down this edge type
                if (!_ignoredEdgeClass.ContainsKey(e.ObjectClassID))
                {
                    _tracedEdges.Add(e);
                    _edgesToBeTraced.Enqueue(e);
                }
            }
        }
        /// <summary>
        /// Check if all of the phases of a junction are normally open or NA
        /// </summary>
        /// <param name="junction">The junction to check</param>
        /// <returns>True if all of the phases of the junction are open or NA</returns>
        private bool IsOpen(JunctionInfo junction)
        {
            bool output = true;
            if (junction is ElectricJunction)
            {
                ElectricJunction ejunct = (ElectricJunction)junction;
                if (ejunct.NormalStatusA == EnumNormalStatus.NC || ejunct.NormalStatusB == EnumNormalStatus.NC || ejunct.NormalStatusC == EnumNormalStatus.NC)
                {
                    output = false;
                }
            }
            else
            {
                return false;
            }
            return output;
        }
        /// <summary>
        /// Find all the edges downstream of the junction
        /// </summary>
        /// <param name="junction">The junction to check</param>
        /// <returns>A list of the downstream edges</returns>
        private List<EdgeInfo> findDownstreamEdges(JunctionInfo junction)
        {
            List<EdgeInfo> output = new List<EdgeInfo>();
            foreach (EdgeInfo e in junction.AdjacentEdges.Values)
            {
                if (e.FromJunction.Equals(junction))
                {
                    output.Add(e);
                }
            }

            return output;
        }
    }
}
