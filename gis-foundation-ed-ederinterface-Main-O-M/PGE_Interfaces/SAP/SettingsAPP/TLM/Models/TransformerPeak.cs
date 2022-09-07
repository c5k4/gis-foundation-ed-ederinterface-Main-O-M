using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TLM.Models
{
    public class TransformerPeak
    {
        public decimal CGCId { get; set; }
        public decimal TransformerId { get; set; }
        public decimal TransformerPeakId { get; set; }
        [DisplayName("Winter Load Status")]
        public string WinterLoadStatus { get; set; }
        [DisplayName("Summer Load Status")]
        public string SummerLoadStatus { get; set; }
        [DisplayName("Data Start Date")]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime? DataStartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM-yyyy}")]
        [DisplayName("Peak Month")]
        public DateTime? WinterPeakDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM-yyyy}")]
        public DateTime? SummerPeakDate { get; set; }
        [DisplayName("Calc. Capability (kVA)")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? WinterCalcCap { get; set; }
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? SummerCalcCap { get; set; }
        [DisplayName("Calc. Peak Load (kVA)")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? WinterKVAPeak { get; set; }
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? SummerKVAPeak { get; set; }
        [DisplayName("Percent Loading")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? WinterPercLoading { get; set; }
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? SummerPercLoading { get; set; }
        public decimal? WinterSMCustCount { get; set; }
        public decimal? SummerSMCustCount { get; set; }
        public decimal? WinterTotalCustCount { get; set; }
        public decimal? SummerTotalCustCount { get; set; }
        [DisplayName("SM Customers / Total Customers")]
        public string WinterSMCustToTotalCust { get; set; }
        public string SummerSMCustToTotalCust { get; set; }
        public decimal? WinterCustRatio { get; set; }
        public decimal? SummerCustRatio { get; set; }
        [DisplayName("SM Ratio")]
        public string WinterCustomersRatio { get; set; }
        public string SummerCustomersRatio { get; set; }
        [DisplayName("Climate Zone")]
        public string ClimateZoneCD { get; set; }
        [DisplayName("Secondary Voltage")]
        public short? SecondaryVoltage { get; set; }
        [DisplayName("Secondary Voltage")]
        public string SecondaryVoltageDesc { get; set; }
        [DisplayName("Current Nameplate kVA")]
        public string NameplateKVA { get; set; }
        [DisplayName("Nameplate kVA at Peak Month")]   
        public decimal? WinterNameplateKVA { get; set; }
        public decimal? SummerNameplateKVA { get; set; }
        [DisplayName("Load Factor")]
        public string LoadFactor { get; set; }
        [DisplayName("Transformer Unit")]
        public string TransformerBankType { get; set; }
        public List<TransformerPeakByCustType> TrfInfoByCustType { get; set; }


        public int TotalUnits { get; set; }
        public string SummerLoadFactor { get; set; }
        public string WinterLoadFactor { get; set; }
        public string SummerLoadUndetermined { get; set; }
        public string WinterLoadUndetermined { get; set; }
        public string[] WinterBankTypes { get; set; }
        public string[] SummerBankTypes { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM-yyyy}")]
        public DateTime?[] WinterPeakMonths { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM-yyyy}")]
        public DateTime?[] SummerPeakMonths { get; set; }
        public decimal?[] WinterNamePlateKVAs { get; set; }
        public decimal?[] SummerNamePlateKVAs { get; set; }
        public decimal?[] WinterCalcCaps { get; set; }
        public decimal?[] SummerCalcCaps { get; set; }
        public decimal?[] WinterKVAPeaks { get; set; }
        public decimal?[] SummerKVAPeaks { get; set; }
        public string[] WinterPercLoadings { get; set; }
        public string[] SummerPercLoadings { get; set; }


        //Extra Properties
        public decimal[] BankIds { get; set; }
        public decimal[] TrfPeakIds { get; set; }
        public decimal[] TrfPeakGenIds { get; set; }

        public string WinterCodeDescription { get; set; }
        public string SummerCodeDescription { get; set; }

        public int CustomerCount { get; set; }
        
    }
}
