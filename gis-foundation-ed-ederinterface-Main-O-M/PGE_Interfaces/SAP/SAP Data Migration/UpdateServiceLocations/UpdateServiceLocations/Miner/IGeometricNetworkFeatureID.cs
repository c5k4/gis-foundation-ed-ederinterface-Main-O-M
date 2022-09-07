using System;

namespace Miner.Geodatabase.Network
{
    internal interface IGeometricNetworkFeatureID
    {
        int ObjectClassID
        {
            get;
            set;
        }

        int ObjectID
        {
            get;
            set;
        }

        FeatureKey Key
        {
            get;
        }

        int WeightValue
        {
            get;
            set;
        }

        int SubId
        {
            get;
            set;
        }

        bool Disabled
        {
            get;
            set;
        }
    }
}
