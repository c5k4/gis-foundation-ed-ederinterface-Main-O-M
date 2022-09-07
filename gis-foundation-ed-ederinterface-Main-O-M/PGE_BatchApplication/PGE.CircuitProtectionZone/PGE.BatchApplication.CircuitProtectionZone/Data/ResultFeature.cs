using System;
using System.Collections.Generic;
using System.Linq;

namespace PGE.BatchApplication.CircuitProtectionZone.Data
{
    public class ResultFeature
    {
        public string DeviceOperatingNumber;
        public string DeviceType;
        public Guid GlobalId { get; set; }
        public Guid SubInterruptingDeviceGuid { get; set; }
        public int Zone { get; set; }
        public string CircuitId { get; set; }
        public string CircuitName { get; set; }
        public double PrimaryOhMiles { get; set; }
        public double T1PrimaryOhMiles { get; set; }
        public double T2PrimaryOhMiles { get; set; }
        public double T3PrimaryOhMiles { get; set; }
        public double PrimaryUgMiles { get; set; }
        public double T1PrimaryUgMiles { get; set; }
        public double T2PrimaryUgMiles { get; set; }
        public double T3PrimaryUgMiles { get; set; }
        public Guid? DeviceGlobalId { get; set; }
        public int CustomersInZone { get; set; }
        public int CustomersLifeSupportInZone { get; set; }
        public int CustomersSensitiveInZone { get; set; }
        public int CustomersEssentialInZone { get; set; }
        public int CustomersDOMDInZone { get; set; }
        public int CustomersAGRInZone { get; set; }
        public int CustomersINDInZone { get; set; }
        public int CustomersCOMInZone { get; set; }
        public int CustomersOTHInZone { get; set; }

        public string ZoneType { get; set; }
        public string ListOfDpds { get; set; }

        public ESRI.ArcGIS.Geometry.IPolygon Polygon { get; set; }
        public string FireTier { get; set; }

        public int FireIndex { get; set; }
        public int FireIndex1 { get; set; }
        public double FireIndex1OHMiles { get; set; }
        public double FireIndex1UGMiles { get; set; }

        public int FireIndex2 { get; set; }
        public double FireIndex2OHMiles { get; set; }
        public double FireIndex2UGMiles { get; set; }

        public int FireIndex3 { get; set; }
        public double FireIndex3OHMiles { get; set; }
        public double FireIndex3UGMiles { get; set; }
        public string Status { get; set; }
        public string Position { get; set; }

        public void SetIndexInformation(FireTierAndIndexInformation fireInfo)
        {
            List<int> fireIndices = new List<int>();
            fireIndices.AddRange(fireInfo.OHMilesPerFireIndex.Keys);
            fireIndices.AddRange(fireInfo.UGMilesPerFireIndex.Keys);
            fireIndices.Sort();
            fireIndices = fireIndices.Distinct().ToList();

            FireIndex1 = -1;
            FireIndex1OHMiles = 0.0;
            FireIndex1UGMiles = 0.0;
            FireIndex2 = -1;
            FireIndex2OHMiles = 0.0;
            FireIndex2UGMiles = 0.0;
            FireIndex3 = -1;
            FireIndex3OHMiles = 0.0;
            FireIndex3UGMiles = 0.0;

            for (int i = 0; i < fireIndices.Count; i++)
            {
                int fireIndex = fireIndices[i];
                if (i == 0)
                {
                    FireIndex1 = fireIndex;
                    if (fireInfo.OHMilesPerFireIndex.ContainsKey(fireIndex)) { FireIndex1OHMiles = fireInfo.OHMilesPerFireIndex[fireIndex]; }
                    if (fireInfo.UGMilesPerFireIndex.ContainsKey(fireIndex)) { FireIndex1UGMiles = fireInfo.UGMilesPerFireIndex[fireIndex]; }
                }
                else if (i == 1)
                {
                    FireIndex2 = fireIndex;
                    if (fireInfo.OHMilesPerFireIndex.ContainsKey(fireIndex)) { FireIndex2OHMiles = fireInfo.OHMilesPerFireIndex[fireIndex]; }
                    if (fireInfo.UGMilesPerFireIndex.ContainsKey(fireIndex)) { FireIndex2UGMiles = fireInfo.UGMilesPerFireIndex[fireIndex]; }
                }
                else if (i == 2)
                {
                    FireIndex3 = fireIndex;
                    if (fireInfo.OHMilesPerFireIndex.ContainsKey(fireIndex)) { FireIndex3OHMiles = fireInfo.OHMilesPerFireIndex[fireIndex]; }
                    if (fireInfo.UGMilesPerFireIndex.ContainsKey(fireIndex)) { FireIndex3UGMiles = fireInfo.UGMilesPerFireIndex[fireIndex]; }
                }
            }
        }

        
    }
}

