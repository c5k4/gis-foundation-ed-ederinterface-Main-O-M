using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TLM.Models
{
    public class TransformerServicePointInfo:ICloneable
    {
        [DisplayName("#")]
        public int DisplayOrder { get; set; }
        public decimal ServicePointRowNumber { get; set; }
        public decimal TransformerHistoryId { get; set; }
        [DisplayName("Meter #")]
        public string MeterNumber { get; set; }
        [DisplayName("Conn")]
        public string IsConnected { get; set; }
        [DisplayName("SM")]
        public string IsSmartMeter { get; set; }
        [DisplayName("Service Point ID")]
        public string ServicePointId { get; set; }
        [DisplayName("Peak Timestamp")]
        public DateTime PeakTimestamp { get; set; }
        [DisplayName("Peak kVA")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? PeakKVA { get; set; }
        [DisplayName("kVA at Trf Peak")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? KVATrfPeak { get; set; }
        [DisplayName("Interval")]
        public decimal? Interval { get; set; }
        [DisplayName("Estimated?")]
        public string IsEstimated { get; set; }
        [DisplayName("Customer Type")]
        public string CustomerType { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("kVA at Trf Peak")]
        public string KVATrfPeakSt { get; set; }
        [DisplayName("Peak KVA")]
        public string PeakKVASt { get; set; }
        [DisplayName("Address StNo")]
        public string StNo { get; set; }
        [DisplayName("Address StName")]
        public string StName { get; set; }
        public object Clone()
        {
            TransformerServicePointInfo ClonedServicePointInfo = (TransformerServicePointInfo)this.MemberwiseClone();
            return ClonedServicePointInfo;
        }
    }
}
