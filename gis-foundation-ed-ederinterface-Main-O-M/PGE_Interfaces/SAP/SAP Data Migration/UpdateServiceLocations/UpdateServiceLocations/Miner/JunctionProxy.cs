using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;

namespace Miner.Geodatabase.FeederManager
{
    internal class JunctionProxy : Junction
    {
        private HashSet<Junction> _junctions = new HashSet<Junction>();

        public int ActualEID
        {
            get;
            set;
        }

        public JunctionProxy(IFeature feature, int eid)
            : base(feature)
        {
            this.ActualEID = base.EID;
            base.EID = eid;
        }

        public void AddJunction(Junction junction)
        {
            this._junctions.Add(junction);
        }

        public override JunctionCollection GetConnectedEdgeJunctions(Dictionary<int, Junction> allJunctions)
        {
            JunctionCollection connectedEdgeJunctions = base.GetConnectedEdgeJunctions(allJunctions);
            foreach (Junction current in this._junctions)
            {
                connectedEdgeJunctions.Add(current);
            }
            return connectedEdgeJunctions;
        }
    }
}