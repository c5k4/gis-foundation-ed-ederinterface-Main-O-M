using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.CircuitProtectionZone.Data
{
    class NetworkEntityFactory
    {
        internal NetworkEntity Create(int eid, int objectClassId, int objectId)
        {
            NetworkEntity networkEntity = null;

            if (eid > 0)
            {
                networkEntity = new NetworkEntity
                {
                    Eid = eid,
                    ObjectClassId = objectClassId,
                    ObjectId = objectId,
                    SubId = 0
                };
            }

            return networkEntity;
        }

        internal List<NetworkEntity> Create(ref IEnumNetEID junctionElements, INetwork network)
        {
            List<NetworkEntity> networkEntities = new List<NetworkEntity>();

            int userClassId;
            int userId;
            int userSubId;

            var netElements = (INetElements)network;

            junctionElements.Reset();
            int eid = junctionElements.Next();

            while (eid > 0)
            {
                NetworkEntity networkEntity = null;

                netElements.QueryIDs(eid, esriElementType.esriETJunction, out userClassId, out userId, out userSubId);

                if (eid > 0)
                {
                    networkEntity = new NetworkEntity
                    {
                        Eid = eid,
                        ObjectClassId = userClassId,
                        ObjectId = userId,
                        SubId = userSubId
                    };
                }

                if (networkEntity != null)
                {
                    networkEntities.Add(networkEntity);
                }

                eid = junctionElements.Next();
            }

            return networkEntities;
        }

        internal NetworkEntity Create(INetwork network, IFeature feature, esriElementType elementType)
        {
            var netElements = (INetElements)network;

            NetworkEntity networkEntity = null;

            int userSubId;
            int userClassId;
            int userId;

            int elementId = netElements.GetEID(feature.Class.ObjectClassID, feature.OID, 0, elementType);

            if (elementId > 0)
            {
                netElements.QueryIDs(elementId, elementType, out userClassId, out userId,
                                        out userSubId);

                networkEntity = new NetworkEntity
                {
                    Eid = elementId,
                    ObjectClassId = userClassId,
                    ObjectId = userId,
                    SubId = userSubId
                };
            }
            else
            {
                networkEntity = new NetworkEntity
                {
                    Eid = 0,
                    ObjectClassId = feature.Class.ObjectClassID,
                    ObjectId = feature.OID,
                    SubId = 0
                };
            }

            return networkEntity;
        }

        internal NetworkEntity Create(BarrierFeature barrierFeature, INetwork network)
        {
            var netElements = (INetElements)network;

            NetworkEntity networkEntity = null;

            int eid = netElements.GetEID(barrierFeature.FeatureClassId, barrierFeature.ObjectId, 0,
                                         esriElementType.esriETJunction);

            if (eid > 0)
            {
                networkEntity = new NetworkEntity
                {
                    Eid = eid,
                    ObjectClassId = barrierFeature.FeatureClassId,
                    ObjectId = barrierFeature.ObjectId,
                    GlobalId = barrierFeature.GlobalId,
                    OperatingNumber = barrierFeature.OperatingNumber,
                    SubId = 0,
                    Status = barrierFeature.Status,
                    NormalPosition = barrierFeature.NormalPosition
                };
            }

            return networkEntity;
        }
    }
}
