using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TLM.Models
{
    public class TransformerLoadHistory
    {
        public int Id { get; set; }
        public decimal TransformerId { get; set; }
        public long TransformerCGC { get; set; }
        public List<int> Months = new List<int>() {1,2,3,4,5,6,7,8,9,10,11,12};
        public List<string> GenerationFilters = new List<string> {"Delivered","Generation"};
        [DisplayName("Data Availability ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/yyyy}")]
        public string DataStartDate { get; set; }
        [DisplayName("Start Month")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/yyyy}")]
        public string StartMonth { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/yyyy}")]
        public string DataLastDate { get; set; }
        [DisplayName("Num of Months")]
        public int SelectedMonth { get; set; }
        [DisplayName("Filters")]
        public string ShowRecOrGen { get; set; }
        [DisplayName("Peak kVA Demand")]
        public string PeakKVADemand { get; set; }
        [DisplayName("Date Peaked")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}")]
        public DateTime? DatePeaked { get; set; }
        [DisplayName("Transformer Meter Ratios")]
        public List<Tuple<string, string, string>> MeterRatios { get; set; }
        [DisplayName("Service Point Info")]
        public List<TransformerServicePointInfo> ServicePointInfo { get; set; }

        public string DataDisplayMessage { get; set; }
        public string InfoMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string WarningDisplayMessage { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime? EarliestLoadDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime? LatestLoadDate { get; set; }
        public DateTime SearchFromDate { get; set; }
        public DateTime SearchToDate { get; set; }
        public bool IsAllMetersLegacy { get; set; }
        //public DateTime DataStartDateDT { get; set; }
        //public DateTime StartMonthDT { get; set; }
    }

    
}