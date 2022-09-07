using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TLM.Models
{
    public class TransformerInfo
    {
        [DisplayName("Switch Position")]
        public string SwitchPosition { get; set; }

        public string Vendor { get; set; }
        public int Maxcycles { get; set; }

        [DisplayName("Voltage change time")]
        [DisplayFormat(DataFormatString = "{0:n1}")]
        public double VoltageChangeTime { get; set; }

        [DisplayName("Pulse time")]
        public int PulseTime { get; set; }

        [DisplayName("Min. switch volt")]
        public int MinSwitchVolt { get; set; }

        [DisplayName("Est. bank voltage rise")]
        public double EstBankVoltRise { get; set; }

        [DisplayName("Auto BVR calc")]
        public string AutoBVRCalc { get; set; }

        [DisplayName("Low voltage override setpoint")]
        public int LowVoltSetPoint { get; set; }

        [DisplayName("High voltage override setpoint")]
        public int HighVoltSetPoint { get; set; }

        [DisplayName("Voltage override time")]
        public int VoltOverrideTime { get; set; }

        [DisplayName("Voltage change time")]
        public int VoltChangeTime { get; set; }

        [DisplayName("Climate Zone CD")]
        public string ClimateZoneCD { get; set; }
    }
}
