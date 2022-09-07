using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TLM.Models
{
    public class SpecialLoadModel
    {
        public string CGCId { get; set; }
        [Display(Name = "Description :")]
        public string Description { get; set; }
        [Display(Name = "Summer KW :")]
        public decimal? SummerKW { get; set; }
        [Display(Name = "Summer KVAR :")]
        public decimal? SummerKVAR { get; set; }
        [Display(Name = "Winter KW :")]
        public decimal? WinterKW { get; set; }
        [Display(Name = "Winter KVAR :")]
        public decimal? WinterKVAR { get; set; }
        [Display(Name = "Created :")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Last Modified :")]
        public DateTime ModifiedDate { get; set; }
    }
}