using System;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.CircuitProtectionZone.Data
{
    public class BarrierFeature
    {
        public int FeatureClassId;
        public int ObjectId { get; set; }
        public string FeatureClassName { get; set; }
        public bool IsRecloser { get; set; }
        public Guid GlobalId { get; set; }
        public string OperatingNumber { get; set; }
        public int Status { get; set; }
        public int NormalPosition { get; set; }
        public bool CustomerOwned { get; set; }
    }
}
