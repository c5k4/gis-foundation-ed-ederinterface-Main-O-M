using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;

namespace Miner.Geodatabase.Network
{
    internal interface INetworkFeatureProvider<TFeatureKey>
    {
        IFeature GetFeature(TFeatureKey key);

        IEnumerable<IFeature> GetFeatures(IEnumerable<TFeatureKey> keys);
    }
}