using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TLM.Models
{
    public class TransformerPeakByCustType
    {
        public int DisplayOrder { get; set; }
        [DisplayName("Customer Type")]
        public string CustomerType { get; set; }
        [DisplayName("Qty")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? WinterQty { get; set; }
        [DisplayName("kVA")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? WinterKVA { get; set; }
        [DisplayName("Qty")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SummerQty { get; set; }
        [DisplayName("kVA")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public decimal? SummerKVA { get; set; }
        


    }
}