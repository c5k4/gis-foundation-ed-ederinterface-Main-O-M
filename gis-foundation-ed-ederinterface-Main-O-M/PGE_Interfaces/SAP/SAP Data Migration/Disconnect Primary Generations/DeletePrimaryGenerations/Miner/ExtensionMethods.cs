using Miner.NetworkModel.Electric;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Miner.Geodatabase.Network
{
    public static class ExtensionMethods
    {

        public static int? ToNullableInt(this object o)
        {
            int i;
            if (int.TryParse(o.ToString(), out i))
                return i;
            return null;
        }

    }
}