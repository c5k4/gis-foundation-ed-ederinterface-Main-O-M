using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.ROBCService
{
    public class CircuitSourceModel
    {
        public string CircuitId { get; set; }
        public string CircuitSourceGuid { get; set; }
        public string Division { get; set; }
        public string FeederId { get; set; }
        public string SubstationName { get; set; }
        public string FeederName { get; set; } //concacte feeder id & substation name
        public string IsScada { get; set; }

        public string SummerKVA { get; set; }
        public string WinterKVA { get; set; }
        public string TotalCustomer { get; set; }
        public int DomesticCustomers { get; set; }
        public int CommercialCustomers { get; set; }
        public int IndustrialCustomers { get; set; }
        public int AgriculturalCustomers { get; set; }
        public int OtherCustomers { get; set; }
        
    }
}
