using System;
using System.Collections.Generic;

namespace Miner.Geodatabase.FeederManager
{
    internal class JunctionsByLocation : Dictionary<Location, JunctionCollection>
    {
        public JunctionCollection GetJunctionsForLocation(Location point)
        {
            JunctionCollection junctionCollection = null;
            base.TryGetValue(point, out junctionCollection);
            if (junctionCollection == null)
            {
                junctionCollection = new JunctionCollection();
                base[point] = junctionCollection;
            }
            return junctionCollection;
        }
    }
}