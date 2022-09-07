using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SettingsApp.Common;
using System.ComponentModel.DataAnnotations;

namespace SettingsApp.Models
{
    public class CircuitPeakModel
    {
        public decimal Id { get; set; }
        [SettingsValidatorAttribute("CIRCUIT_LOAD_HIST", "PEAKDATE")]
        [Display(Name = "Date of peak")]
        public DateTime? PeakDate { get; set; }
        [SettingsValidatorAttribute("CIRCUIT_LOAD_HIST", "PEAKTIME")]
        [Display(Name = "Time of peak")]
        public string PeakTime { get; set; }
        [SettingsValidatorAttribute("CIRCUIT_LOAD_HIST", "TOTAL_KW_LOAD")]
        [Display(Name = "Total KW load")]
        public int? TotalKWLoad { get; set; }
    }
}