using System;

namespace PGE.BatchApplication.CircuitProtectionZone.Data
{
    public class NetworkEntity
    {
        public string OperatingNumber;
        public int ObjectClassId { get; set; }
        public int ObjectId { get; set; }
        public Guid GlobalId { get; set; }
        public int Eid { get; set; }
        public int SubId { get; set; }
        public int NormalPosition { get; set; }
        public int Status { get; set; }

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            NetworkEntity p = obj as NetworkEntity;
            if ((Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (ObjectId == p.ObjectId) && (ObjectClassId == p.ObjectClassId);
        }

        public bool Equals(NetworkEntity p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (ObjectId == p.ObjectId) && (ObjectClassId == p.ObjectClassId);
        }

        public override int GetHashCode()
        {
            return ObjectId ^ ObjectClassId;
        }

    }
}
