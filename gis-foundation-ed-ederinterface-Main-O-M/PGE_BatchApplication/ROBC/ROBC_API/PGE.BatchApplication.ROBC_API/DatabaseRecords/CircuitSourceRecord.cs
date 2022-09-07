using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.ROBC_API.DatabaseRecords
{
    public class CircuitSourceRecord : BaseRecord
    {
        public CircuitSourceRecord()
        {
            GlobalID = "";
            IsScada = ScadaIndicator.Unknown;
        }
        public ScadaIndicator IsScada { get; set; }
        public double SummerKVA { get; set; }
        public double WinterKVA { get; set; }
        public int TotalCustomers { get; set; }
        //public int TotalCustomers
        //{
        //    get
        //    {
        //        return DomesticCustomers + CommercialCustomers + IndustrialCustomers + AgriculturalCustomers + OtherCustomers;
        //    }
        //}
        //public int DomesticCustomers { get; set; }
        //public int CommercialCustomers { get; set; }
        //public int IndustrialCustomers { get; set; }
        //public int AgriculturalCustomers { get; set; }
        //public int OtherCustomers { get; set; }
    }

    public enum ScadaIndicator
    {
        Yes, No, Unknown
    };
}
