using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SettingsApp.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class RegulatorUnit
    {
        
        [Display(Name = "Controller Serial # :")]
        public System.String ControllerSerialNum { get; set; }

       
        [Display(Name = "Firmware Version :")]
        public System.String FirmwareVersion { get; set; }

        
        [Display(Name = "Software Version :")]
        public System.String SoftwareVersion { get; set; }

        [SettingsValidatorAttribute("Regulator", "CONTROL_TYPE")]
        [Display(Name = "Control Type :")]
        public System.String ControlType { get; set; }

        [Display(Name = "GlobalID :")]
        public System.String GlobalID { get; set; }


        [Display(Name = "Bank Code :")]
        public System.Int16? BankCode { get; set; }



    }
}