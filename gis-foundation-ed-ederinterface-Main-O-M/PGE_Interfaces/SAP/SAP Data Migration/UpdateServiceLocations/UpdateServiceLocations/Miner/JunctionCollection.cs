using System;
using System.Collections.Generic;

namespace Miner.Geodatabase.FeederManager
{
    internal class JunctionCollection : HashSet<Junction>
    {
        public bool Processed
        {
            get;
            set;
        }
    }
}