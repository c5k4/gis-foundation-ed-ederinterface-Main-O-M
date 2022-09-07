using Miner.NetworkModel;
using System;
using System.Collections.Generic;

namespace Miner.Geodatabase.Network
{
    internal interface IFeatureMapper<TKey>
    {
        IList<IElement> GetElements(TKey featureID);

        TKey GetFeatureKey(IElement element);
    }
}