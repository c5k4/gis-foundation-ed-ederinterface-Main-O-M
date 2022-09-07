using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class RegulatorModel
    {
        
        
        [SettingsValidatorAttribute("Regulator", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("Regulator", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceId { get; set; }

        [SettingsValidatorAttribute("Regulator", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        //[SettingsValidatorAttribute("Regulator", "CONTROL_SERIAL_NUM")]
        //[Display(Name = "Controller Serial # :")]
        //public System.String ControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("Regulator", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("Regulator", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("Regulator", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("Regulator", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("Regulator", "NOTES")]
        [Display(Name = "Notes :")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("Regulator", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        

        [SettingsValidatorAttribute("Regulator", "MODE")]
        [Display(Name = "Mode :")]
        public System.String Mode { get; set; }

        [SettingsValidatorAttribute("Regulator", "PRIMARY_CT_RATING")]
        [Display(Name = "Primary CT Rating :")]
        public System.Int16? PrimaryCtRating { get; set; }

        [SettingsValidatorAttribute("Regulator", "BAND_WIDTH")]
        [Display(Name = "Band width :")]
        public System.Decimal? BandWidth { get; set; }

        [SettingsValidatorAttribute("Regulator", "PT_RATIO")]
        [Display(Name = "PT Ratio :")]
        public System.String PtRatio { get; set; }

        [SettingsValidatorAttribute("Regulator", "RANGE_UNBLOCKED")]
        [Display(Name = "Range unblocked :")]
        public System.Decimal? RangeUnblocked { get; set; }

        [SettingsValidatorAttribute("Regulator", "BLOCKED_PCT")]
        [Display(Name = "Blocked pct :")]
        public System.String BlockedPct { get; set; }

        [SettingsValidatorAttribute("Regulator", "STEPS")]
        [Display(Name = "Steps unblocked :")]
        public System.Int16? StepsUnblocked { get; set; }

        [SettingsValidatorAttribute("Regulator", "PEAK_LOAD")]
        [Display(Name = "Peak load :")]
        public System.Int16? PeakLoad { get; set; }

        [SettingsValidatorAttribute("Regulator", "MIN_LOAD")]
        [Display(Name = "Min. load :")]
        public System.Int16? MinLoad { get; set; }

        [SettingsValidatorAttribute("Regulator", "PVD_MAX")]
        [Display(Name = "PVD Max :")]
        public System.Decimal? PvdMax { get; set; }

        [SettingsValidatorAttribute("Regulator", "PVD_MIN")]
        [Display(Name = "PVD Min :")]
        public System.Decimal? PvdMin { get; set; }

        [SettingsValidatorAttribute("Regulator", "SVD_MIN")]
        [Display(Name = "SVD Min :")]
        public System.Decimal? SvdMin { get; set; }

        [SettingsValidatorAttribute("Regulator", "POWER_FACTOR")]
        [Display(Name = "Power factor :")]
        public System.Decimal? PowerFactor { get; set; }

        [SettingsValidatorAttribute("Regulator", "LOAD_CYCLE")]
        [Display(Name = "Load cycle :")]
        public System.String LoadCycle { get; set; }

        [SettingsValidatorAttribute("Regulator", "RISE_RATING")]
        [Display(Name = "Rise rating :")]
        public System.String RiseRating { get; set; }

        [SettingsValidatorAttribute("Regulator", "TIMER")]
        [Display(Name = "Timer :")]
        public System.Int16? Timer { get; set; }

        [SettingsValidatorAttribute("Regulator", "VOLT_VAR_TEAM_MEMBER")]
        [Display(Name = "Volt Var team member :")]
        public System.String VoltVarTeamMember { get; set; }

        [SettingsValidatorAttribute("Regulator", "REV_THRESHOLD")]
        [Display(Name = "Reversible threshold percentage :")]
        public System.String ReversibleThresholdPercentage { get; set; }

        [SettingsValidatorAttribute("Regulator", "HIGH_VOLTAGE_LIMIT")]
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true), Display(Name = "High voltage limit :")]
        [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
       
       // [Display(Name = "High voltage limit :")]
        public System.Decimal? HighVoltageLimit { get; set; }

        [SettingsValidatorAttribute("Regulator", "LOW_VOLTAGE_LIMIT")]
        [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Low Voltage limit :")]
        public System.Decimal? LowVoltageLimit { get; set; }


        //1
        [SettingsValidatorAttribute("Regulator", "FWD_A_STATUS")]
        [Display(Name = "Forward A Status :")]
        public System.String ForwardAStatus { get; set; }
        //ForwardAVoltage
        [SettingsValidatorAttribute("Regulator", "FWD_A_VOLT")]
        [DisplayFormat(DataFormatString = "{0:n1}", ApplyFormatInEditMode = true), Display(Name = "Forward A voltage (120V) :")]
        [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
       
        public System.Decimal? ForwardAVoltage { get; set; }

        //ForwardARset
        [SettingsValidatorAttribute("Regulator", "FWD_A_RESET")]
        [DisplayFormat(DataFormatString = "{0:n1}", ApplyFormatInEditMode = true), Display(Name = "Forward A Rset (volt) :")]
        //[RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$",ErrorMessage = "Voltage can't have more than 1 decimal places")]
      
        public System.Decimal? ForwardARset { get; set; }

        [SettingsValidatorAttribute("Regulator", "FWD_A_XSET")]
        [DisplayFormat(DataFormatString = "{0:n1}", ApplyFormatInEditMode = true), Display(Name = "Forward A Xset (volt) :")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
       // [Display(Name = "Forward A Xset (volt) :")]
        public System.Decimal? ForwardAXset { get; set; }
        //2
        [SettingsValidatorAttribute("Regulator", "FWD_B_STATUS")]
        [Display(Name = "Forward B Status :")]
        public System.String ForwardBStatus { get; set; }

        [SettingsValidatorAttribute("Regulator", "FWD_B_VOLT")]
        [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Forward B voltage (120V) :")]
        public System.Decimal? ForwardBVoltage { get; set; }

        [SettingsValidatorAttribute("Regulator", "FWD_B_RESET")]
        //[RegularExpression(@"^\d+.\d{0,1}$", ErrorMessage = "voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Forward B Rset (volt) :")]
        public decimal? ForwardBRset { get; set; }

        [SettingsValidatorAttribute("Regulator", "FWD_B_XSET")]
      //  [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Forward B Xset (volt) :")]
        public decimal? ForwardBXset { get; set; }
        //3
        [SettingsValidatorAttribute("Regulator", "FWD_C_STATUS")]
        [Display(Name = "Forward C Status :")]
        public System.String ForwardCStatus { get; set; }

        [SettingsValidatorAttribute("Regulator", "FWD_C_VOLT")]
        [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Forward C voltage (120V) :")]
        public System.Decimal? ForwardCVoltage { get; set; }

        [SettingsValidatorAttribute("Regulator", "FWD_C_RESET")]
       // [RegularExpression(@"^\d+.\d{0,1}$", ErrorMessage = "voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Forward C Rset (volt) :")]
        public decimal? ForwardCRset { get; set; }

        [SettingsValidatorAttribute("Regulator", "FWD_C_XSET")]
       // [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Forward C Xset (volt) :")]
        public decimal? ForwardCXset { get; set; }
        //4
        [SettingsValidatorAttribute("Regulator", "REV_A_STATUS")]
        [Display(Name = "Reverse A Status :")]
        public System.String ReverseAStatus { get; set; }

        [SettingsValidatorAttribute("Regulator", "REV_A_VOLT")]

        [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Reverse A voltage (120V) :")]
        public System.Decimal? ReverseAVoltage { get; set; }

        [SettingsValidatorAttribute("Regulator", "REV_A_RESET")]
       // [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Reverse A Rset (volt) :")]
        public decimal? ReverseARset { get; set; }

        [SettingsValidatorAttribute("Regulator", "REV_A_XSET")]
       // [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Reverse A Xset (volt) :")]
        public decimal? ReverseAXset { get; set; }
        //5
        [SettingsValidatorAttribute("Regulator", "REV_B_STATUS")]
        [Display(Name = "Reverse B Status :")]
        public System.String ReverseBStatus { get; set; }
       
        [SettingsValidatorAttribute("Regulator", "REV_B_VOLT")]
        [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Reverse B voltage (120V) :")]
        public System.Decimal? ReverseBVoltage { get; set; }

        [SettingsValidatorAttribute("Regulator", "REV_B_RESET")]
       // [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Reverse B Rset (volt) :")]
        public decimal? ReverseBRset { get; set; }

        [SettingsValidatorAttribute("Regulator", "REV_B_XSET")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Reverse B Xset (volt) :")]
        public decimal? ReverseBXset { get; set; }
        //6
        [SettingsValidatorAttribute("Regulator", "REV_C_STATUS")]
        [Display(Name = "Reverse C Status :")]
        public System.String ReverseCStatus { get; set; }

        [SettingsValidatorAttribute("Regulator", "REV_C_VOLT")]
        [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Reverse C voltage (120V) :")]
        public System.Decimal? ReverseCVoltage { get; set; }

        [SettingsValidatorAttribute("Regulator", "REV_C_RESET")]
       // [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Reverse C Rset (volt) :")]
        public decimal? ReverseCRset { get; set; }

        [SettingsValidatorAttribute("Regulator", "REV_C_XSET")]
       // [RegularExpression(@"^-?[0-9]+.\d{0,1}$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Reverse C Xset (volt) :")]
        public decimal? ReverseCXset { get; set; }


        [SettingsValidatorAttribute("Regulator", "ENGINEERING_DOCUMENT")]
        [Display(Name = "Engineering Document :")]
        public System.String EngineeringDocument { get; set; }

        [SettingsValidatorAttribute("Regulator", "SCADA")]
        [Display(Name = "SCADA :")]
        public System.String Scada { get; set; }

        [SettingsValidatorAttribute("Regulator", "SCADA_TYPE")]
        [Display(Name = "SCADA Type :")]
        public System.String ScadaType { get; set; }

        [SettingsValidatorAttribute("Regulator", "MASTER_STATION")]
        [Display(Name = "Master Station :")]
        public System.String MasterStation { get; set; }

        [SettingsValidatorAttribute("Regulator", "BAUD_RATE")]
        [Display(Name = "Baud Rate :")]
        public System.Int32? BaudRate { get; set; }

        [SettingsValidatorAttribute("Regulator", "TRANSMIT_ENABLE_DELAY")]
        [Display(Name = "Transmit Enable Delay :")]
        public System.Int32? TransmitEnableDelay { get; set; }

        [SettingsValidatorAttribute("Regulator", "TRANSMIT_DISABLE_DELAY")]
        [Display(Name = "Transmit Disable Delay :")]
        public System.Int32? TransmitDisableDelay { get; set; }

        [SettingsValidatorAttribute("Regulator", "RTU_ADDRESS")]
        [Display(Name = "RTU Address :")]
        public System.String RtuAddress { get; set; }

        [SettingsValidatorAttribute("Regulator", "REPEATER")]
        [Display(Name = "(if applicable) Repeater :")]
        public System.String Repeater { get; set; }

        [SettingsValidatorAttribute("Regulator", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special conditions :")]
        public System.String SpecialConditions { get; set; }

        [SettingsValidatorAttribute("Regulator", "RADIO_MANF_CD")]
        [Display(Name = "SCADA radio manufacturer :")]
        public System.String ScadaRadioManufacturer { get; set; }

        [SettingsValidatorAttribute("Regulator", "RADIO_MODEL_NUM")]
        [Display(Name = "SCADA radio model # :")]
        public System.String ScadaRadioModelNum { get; set; }

        [SettingsValidatorAttribute("Regulator", "RADIO_SERIAL_NUM")]
        [Display(Name = "SCADA radio serial # :")]
        public System.String ScadaRadioSerialNum { get; set; }

        [SettingsValidatorAttribute("Regulator", "USE_RX")]
        [Display(Name = "Use R&X Format :")]
        public System.String UseRx { get; set; }

        [SettingsValidatorAttribute("Regulator", "REVERSIBLE")]
        [Display(Name = "Reversible :")]
        public System.String Reversible { get; set; }






        /*New fields Settingsgaps*/
        [SettingsValidatorAttribute("Regulator", "ENGINEERING_COMMENTS")]
        [Display(Name = "Engineering Comments :")]
        public System.String EngineeringComments { get; set; }


        [SettingsValidatorAttribute("Regulator", "RTU_EXISTS")]
        [Display(Name = "External RTU :")]
        public System.String RTUExists { get; set; }


        [SettingsValidatorAttribute("Regulator", "RTU_MANF_CD")]
        [Display(Name = "RTU Manufacturer :")]
        public System.String RTUManufacture { get; set; }


        [SettingsValidatorAttribute("Regulator", "RTU_MODEL_NUM")]
        [Display(Name = "RTU Model # :")]
        public System.String RTUModelNumber { get; set; }


        [SettingsValidatorAttribute("Regulator", "RTU_SERIAL_NUM")]
        [Display(Name = "RTU Serial # :")]
        public System.String RTUSerialNumber { get; set; }


        [SettingsValidatorAttribute("Regulator", "RTU_SOFTWARE_VERSION")]
        [Display(Name = "RTU Software version :")]
        public System.String RTUSoftwareVersion { get; set; }


        [SettingsValidatorAttribute("Regulator", "RTU_FIRMWARE_VERSION")]
        [Display(Name = "RTU Firmware version :")]
        public System.String RTUFirmwareVersion { get; set; }

        public SelectList RTUManufactureList { get; set; }

        /* end of changes */
        public List<RegulatorUnit> RegulatorUnits { get; set; }
        public Dictionary<string, string> ControlTypeDictionary { get; set; } 

        //[SettingsValidatorAttribute("Regulator", "CONTROL_TYPE")]
        //[Display(Name = "Control Type :")]
        //public System.String ControlType { get; set; }


        public System.String GlobalIDCopy { get; set; }
        public bool Release { get; set; }
        public List<GISAttributes> GISAttributes { get; set; }

        public bool SamePhaseSettings { get; set; }
        public string SamePhaseSettingsPostbackScript { get; set; }
        public SelectList BlockedPctList { get; set; }
        public SelectList ControlTypeList { get; set; }
        public SelectList EngineeringDocumentList { get; set; }
        public SelectList LoadCycleList { get; set; }
        public SelectList ModeList { get; set; }
        public SelectList PtRatioList { get; set; }
        public SelectList ScadaRadioManufacturerList { get; set; }
        public SelectList RiseRatingList { get; set; }
        public SelectList ScadaTypeList { get; set; }
        public SelectList ControllerUnitTypeList { get; set; }
        public SelectList OperatingModeList { get; set; }
        public SelectList AltPhaInstTripList { get; set; }
        public SelectList Alt3PhaInstTripList { get; set; }
        public SelectList PhaInstTripList { get; set; }
        public SelectList AltPhaFastCurveList { get; set; }
        public SelectList Alt3PhaFastCurveList { get; set; }
        public SelectList PhaFastCurveList { get; set; }
        public SelectList Alt2PhaInstTripList { get; set; }
        public SelectList Alt2PhaCurveList { get; set; }
        public SelectList AltPhaSlowCurveList { get; set; }
        public SelectList Alt3PhaSlowCurveList { get; set; }
        public SelectList PhaSlowCurveList { get; set; }
        public SelectList AltPhaResponseTimeList { get; set; }
        public SelectList PhaResponseTimeList { get; set; }
        public SelectList AltGrdInstTripList { get; set; }
        public SelectList Alt3GrdInstTripList { get; set; }
        public SelectList GrdInstTripList { get; set; }
        public SelectList AltGrdFastCurveList { get; set; }
        public SelectList Alt3GrdFastCurveList { get; set; }
        public SelectList GrdFastCurveList { get; set; }
        public SelectList Alt2GrdInstTripList { get; set; }
        public SelectList Alt2GrdCurveList { get; set; }
        public SelectList AltGrdSlowCurveList { get; set; }
        public SelectList Alt3GrdSlowCurveList { get; set; }
        public SelectList GrdSlowCurveList { get; set; }
        public SelectList AltGrdResponseTimeList { get; set; }
        public SelectList GrdResponseTimeList { get; set; }
        public SelectList AtlSfgList { get; set; }
        public SelectList Alt3SensitiveGroundFaultList { get; set; }
        public SelectList SgfCdList { get; set; }
        public SelectList AtlHighCurrentLockoutPhaseList { get; set; }
        public SelectList Alt3HighCurrentLockoutPhaseList { get; set; }
        public SelectList HighCurrentLockoutPhaseList { get; set; }
        public SelectList AtlHighCurrentLockoutGroundList { get; set; }
        public SelectList Alt3HighCurrentLockoutGroundList { get; set; }
        public SelectList HighCurrentLockoutGroundList { get; set; }

        public void PopulateEntityFromModel(SM_REGULATOR e)
        {
            e.DEVICE_ID = this.DeviceId;
            e.PREPARED_BY = this.PreparedBy;
            //e.CONTROL_SERIAL_NUM = this.ControllerSerialNum;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            //e.NOTES = this.Notes;
            e.OK_TO_BYPASS = this.OkToBypass;
            //e.CONTROL_TYPE = this.ControlType;
            e.MODE = this.Mode;
            e.PRIMARY_CT_RATING = this.PrimaryCtRating;
            e.BAND_WIDTH = this.BandWidth;
            e.PT_RATIO = this.PtRatio;
            e.RANGE_UNBLOCKED = this.RangeUnblocked;
            e.BLOCKED_PCT = this.BlockedPct;
            e.STEPS = this.StepsUnblocked;
            e.PEAK_LOAD = this.PeakLoad;
            e.MIN_LOAD = this.MinLoad;
            e.PVD_MAX = this.PvdMax;
            e.PVD_MIN = this.PvdMin;
            e.SVD_MIN = this.SvdMin;
            e.POWER_FACTOR = this.PowerFactor;
            e.LOAD_CYCLE = this.LoadCycle;
            e.RISE_RATING = this.RiseRating;
            e.TIMER = this.Timer;
            e.VOLT_VAR_TEAM_MEMBER = this.VoltVarTeamMember;
            e.REV_THRESHOLD = this.ReversibleThresholdPercentage;
            e.HIGH_VOLTAGE_LIMIT = this.HighVoltageLimit;
            e.LOW_VOLTAGE_LIMIT = this.LowVoltageLimit;
            e.FWD_A_STATUS = this.ForwardAStatus;
            e.FWD_A_VOLT = this.ForwardAVoltage;
            e.FWD_A_RESET = this.ForwardARset;
            e.FWD_A_XSET = this.ForwardAXset;
            e.FWD_B_STATUS = this.ForwardBStatus;
            e.FWD_B_VOLT = this.ForwardBVoltage;
            e.FWD_B_RESET = this.ForwardBRset;
            e.FWD_B_XSET = this.ForwardBXset;
            e.FWD_C_STATUS = this.ForwardCStatus;
            e.FWD_C_VOLT = this.ForwardCVoltage;
            e.FWD_C_RESET = this.ForwardCRset;
            e.FWD_C_XSET = this.ForwardCXset;
           
            //e.FIRMWARE_VERSION = this.FirmwareVersion;
            //e.SOFTWARE_VERSION = this.SoftwareVersion;
            //e.ENGINEERING_DOCUMENT = this.EngineeringDocument;
            e.REV_A_VOLT = this.ReverseAVoltage;
            e.REV_A_RESET = this.ReverseARset;
            e.REV_A_XSET = this.ReverseAXset;
            e.REV_B_VOLT = this.ReverseBVoltage;
            e.REV_B_RESET = this.ReverseBRset;
            e.REV_B_XSET = this.ReverseBXset;
            e.REV_C_VOLT = this.ReverseCVoltage;
            e.REV_C_RESET = this.ReverseCRset;
            e.REV_C_XSET = this.ReverseCXset;
            e.SCADA = this.Scada;
            e.SCADA_TYPE = this.ScadaType;
            e.MASTER_STATION = this.MasterStation;
            e.BAUD_RATE = this.BaudRate;
            e.TRANSMIT_ENABLE_DELAY = this.TransmitEnableDelay;
            e.TRANSMIT_DISABLE_DELAY = this.TransmitDisableDelay;
            e.RTU_ADDRESS = this.RtuAddress;
            e.REPEATER = this.Repeater;
            e.SPECIAL_CONDITIONS = this.SpecialConditions;
            e.RADIO_MANF_CD = this.ScadaRadioManufacturer;
            e.RADIO_MODEL_NUM = this.ScadaRadioModelNum;
            e.RADIO_SERIAL_NUM = this.ScadaRadioSerialNum;
            e.USE_RX = this.UseRx;
            e.REVERSIBLE = this.Reversible;
            e.RTU_EXIST = this.RTUExists;
            e.RTU_MANF_CD = this.RTUManufacture;
            e.RTU_FIRMWARE_VERSION = this.RTUFirmwareVersion;
            e.RTU_MODEL_NUM = this.RTUModelNumber;
            e.RTU_SERIAL_NUM = this.RTUSerialNumber;
            e.RTU_SOFTWARE_VERSION = this.RTUSoftwareVersion;
            e.ENGINEERING_COMMENTS = this.EngineeringComments;
           
        }

        public void PopulateModelFromEntity(SM_REGULATOR e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            //this.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            //this.Notes = e.NOTES;
            this.OkToBypass = e.OK_TO_BYPASS;
            //this.ControlType = e.CONTROL_TYPE;
            this.Mode = e.MODE;
            this.PrimaryCtRating = e.PRIMARY_CT_RATING;
            this.BandWidth = e.BAND_WIDTH;
            this.PtRatio = e.PT_RATIO;
            this.RangeUnblocked = e.RANGE_UNBLOCKED;
            this.BlockedPct = e.BLOCKED_PCT;
            this.StepsUnblocked = e.STEPS;
            this.PeakLoad = e.PEAK_LOAD;
            this.MinLoad = e.MIN_LOAD;
            this.PvdMax = e.PVD_MAX;
            this.PvdMin = e.PVD_MIN;
            this.SvdMin = e.SVD_MIN;
            this.PowerFactor = e.POWER_FACTOR;
            this.LoadCycle = e.LOAD_CYCLE;
            this.RiseRating = e.RISE_RATING;
            this.Timer = e.TIMER;
            this.VoltVarTeamMember = e.VOLT_VAR_TEAM_MEMBER;
            this.ReversibleThresholdPercentage = e.REV_THRESHOLD;
            this.HighVoltageLimit = e.HIGH_VOLTAGE_LIMIT;
            this.LowVoltageLimit = e.LOW_VOLTAGE_LIMIT;
            this.ForwardAStatus = e.FWD_A_STATUS;
            this.ForwardAVoltage = e.FWD_A_VOLT;
            this.ForwardARset = e.FWD_A_RESET;
            this.ForwardAXset = e.FWD_A_XSET;
            this.ForwardBStatus = e.FWD_B_STATUS;
            this.ForwardBVoltage = e.FWD_B_VOLT;
            this.ForwardBRset = e.FWD_B_RESET;
            this.ForwardBXset = e.FWD_B_XSET;
            this.ForwardCStatus = e.FWD_C_STATUS;
            this.ForwardCVoltage = e.FWD_C_VOLT;
            this.ForwardCRset = e.FWD_C_RESET;
            this.ForwardCXset = e.FWD_C_XSET;
            this.ReverseAVoltage = e.REV_A_VOLT;
            this.ReverseARset = e.REV_A_RESET;
            this.ReverseAXset = e.REV_A_XSET;
            this.ReverseBVoltage = e.REV_B_VOLT;
            this.ReverseBRset = e.REV_B_RESET;
            this.ReverseBXset = e.REV_B_XSET;
            this.ReverseCVoltage = e.REV_C_VOLT;
            this.ReverseCRset = e.REV_C_RESET;
            this.ReverseCXset = e.REV_C_XSET;
            //this.FirmwareVersion = e.FIRMWARE_VERSION;
            //this.SoftwareVersion = e.SOFTWARE_VERSION;
            //this.EngineeringDocument = e.ENGINEERING_DOCUMENT;
            this.Scada = e.SCADA;
            this.ScadaType = e.SCADA_TYPE;
            this.MasterStation = e.MASTER_STATION;
            this.BaudRate = e.BAUD_RATE;
            this.TransmitEnableDelay = e.TRANSMIT_ENABLE_DELAY;
            this.TransmitDisableDelay = e.TRANSMIT_DISABLE_DELAY;
            this.RtuAddress = e.RTU_ADDRESS;
            this.Repeater = e.REPEATER;
            this.SpecialConditions = e.SPECIAL_CONDITIONS;
            this.ScadaRadioManufacturer = e.RADIO_MANF_CD;
            this.ScadaRadioModelNum = e.RADIO_MODEL_NUM;
            this.ScadaRadioSerialNum = e.RADIO_SERIAL_NUM;
            this.UseRx = e.USE_RX;
            this.Reversible = e.REVERSIBLE;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
        }

        public SM_REGULATOR PopulateEntityFromEnityCopy(SM_REGULATOR entityNew, SM_REGULATOR entityOld)
        {
            //entityNew.OPERATING_NUM = entityyOld.OPERATING_NUM;
            //entityNew.DEVICE_ID = entityyOld.DEVICE_ID;
            entityNew.PREPARED_BY = entityOld.PREPARED_BY;
            entityNew.CONTROL_SERIAL_NUM = entityOld.CONTROL_SERIAL_NUM;
            //entityNew.DATE_MODIFIED = entityyOld.DATE_MODIFIED;
            //entityNew.EFFECTIVE_DT = entityyOld.EFFECTIVE_DT;
            //entityNew.PEER_REVIEW_DT = entityyOld.PEER_REVIEW_DT;
            //entityNew.PEER_REVIEW_BY = entityyOld.PEER_REVIEW_BY;
            //entityNew.NOTES = entityOld.NOTES;
            entityNew.OK_TO_BYPASS = entityOld.OK_TO_BYPASS;
            entityNew.CONTROL_TYPE = entityOld.CONTROL_TYPE;
            entityNew.MODE = entityOld.MODE;
            entityNew.PRIMARY_CT_RATING = entityOld.PRIMARY_CT_RATING;
            entityNew.BAND_WIDTH = entityOld.BAND_WIDTH;
            entityNew.PT_RATIO = entityOld.PT_RATIO;
            entityNew.RANGE_UNBLOCKED = entityOld.RANGE_UNBLOCKED;
            entityNew.BLOCKED_PCT = entityOld.BLOCKED_PCT;
            entityNew.STEPS = entityOld.STEPS;
            entityNew.PEAK_LOAD = entityOld.PEAK_LOAD;
            entityNew.MIN_LOAD = entityOld.MIN_LOAD;
            entityNew.PVD_MAX = entityOld.PVD_MAX;
            entityNew.PVD_MIN = entityOld.PVD_MIN;
            entityNew.SVD_MIN = entityOld.SVD_MIN;
            entityNew.POWER_FACTOR = entityOld.POWER_FACTOR;
            entityNew.LOAD_CYCLE = entityOld.LOAD_CYCLE;
            entityNew.RISE_RATING = entityOld.RISE_RATING;
            entityNew.TIMER = entityOld.TIMER;
            entityNew.VOLT_VAR_TEAM_MEMBER = entityOld.VOLT_VAR_TEAM_MEMBER;
            entityNew.REV_THRESHOLD = entityOld.REV_THRESHOLD;
            entityNew.HIGH_VOLTAGE_LIMIT = entityOld.HIGH_VOLTAGE_LIMIT;
            entityNew.LOW_VOLTAGE_LIMIT = entityOld.LOW_VOLTAGE_LIMIT;
            entityNew.FWD_A_STATUS = entityOld.FWD_A_STATUS;
            entityNew.FWD_A_VOLT = entityOld.FWD_A_VOLT;
            entityNew.FWD_A_RESET = entityOld.FWD_A_RESET;
            entityNew.FWD_A_XSET = entityOld.FWD_A_XSET;
            entityNew.FWD_B_STATUS = entityOld.FWD_B_STATUS;
            entityNew.FWD_B_VOLT = entityOld.FWD_B_VOLT;
            entityNew.FWD_B_RESET = entityOld.FWD_B_RESET;
            entityNew.FWD_B_XSET = entityOld.FWD_B_XSET;
            entityNew.FWD_C_STATUS = entityOld.FWD_C_STATUS;
            entityNew.FWD_C_VOLT = entityOld.FWD_C_VOLT;
            entityNew.FWD_C_RESET = entityOld.FWD_C_RESET;
            entityNew.FWD_C_XSET = entityOld.FWD_C_XSET;
            entityNew.REV_A_VOLT = entityOld.REV_A_VOLT;
            entityNew.REV_A_RESET = entityOld.REV_A_RESET;
            entityNew.REV_A_XSET = entityOld.REV_A_XSET;
            entityNew.REV_B_VOLT = entityOld.REV_B_VOLT;
            entityNew.REV_B_RESET = entityOld.REV_B_RESET;
            entityNew.REV_B_XSET = entityOld.REV_B_XSET;
            entityNew.REV_C_VOLT = entityOld.REV_C_VOLT;
            entityNew.REV_C_RESET = entityOld.REV_C_RESET;
            entityNew.REV_C_XSET = entityOld.REV_C_XSET;

            entityNew.FIRMWARE_VERSION = entityOld.FIRMWARE_VERSION;
            entityNew.SOFTWARE_VERSION = entityOld.SOFTWARE_VERSION;
            //entityNew.ENGINEERING_DOCUMENT = entityOld.ENGINEERING_DOCUMENT;
            entityNew.SCADA = entityOld.SCADA;
            entityNew.SCADA_TYPE = entityOld.SCADA_TYPE;
            entityNew.MASTER_STATION = entityOld.MASTER_STATION;
            entityNew.BAUD_RATE = entityOld.BAUD_RATE;
            entityNew.TRANSMIT_ENABLE_DELAY = entityOld.TRANSMIT_ENABLE_DELAY;
            entityNew.TRANSMIT_DISABLE_DELAY = entityOld.TRANSMIT_DISABLE_DELAY;
            entityNew.RTU_ADDRESS = entityOld.RTU_ADDRESS;
            entityNew.REPEATER = entityOld.REPEATER;
            entityNew.SPECIAL_CONDITIONS = entityOld.SPECIAL_CONDITIONS;
            entityNew.RADIO_MANF_CD = entityOld.RADIO_MANF_CD;
            entityNew.RADIO_MODEL_NUM = entityOld.RADIO_MODEL_NUM;
            entityNew.RADIO_SERIAL_NUM = entityOld.RADIO_SERIAL_NUM;
            entityNew.USE_RX = entityOld.USE_RX;
            entityNew.REVERSIBLE = entityOld.REVERSIBLE;
            entityNew.RTU_EXIST = entityOld.RTU_EXIST;
            entityNew.RTU_MANF_CD = entityOld.RTU_MANF_CD;
            entityNew.RTU_FIRMWARE_VERSION = entityOld.RTU_FIRMWARE_VERSION;
            entityNew.RTU_MODEL_NUM = entityOld.RTU_MODEL_NUM;
            entityNew.RTU_SERIAL_NUM = entityOld.RTU_SERIAL_NUM;
            entityNew.RTU_SOFTWARE_VERSION = entityOld.RTU_SOFTWARE_VERSION;
            entityNew.ENGINEERING_COMMENTS = entityOld.ENGINEERING_COMMENTS;

            return entityNew;
        }

        public void PopulateHistoryFromEntity(SM_REGULATOR_HIST entityHistory, SM_REGULATOR e)
        {
            entityHistory.GLOBAL_ID = e.GLOBAL_ID;
            entityHistory.FEATURE_CLASS_NAME = e.FEATURE_CLASS_NAME;
            entityHistory.OPERATING_NUM = e.OPERATING_NUM;
            entityHistory.DEVICE_ID = e.DEVICE_ID;
            entityHistory.PREPARED_BY = e.PREPARED_BY;
            entityHistory.RELAY_TYPE = e.RELAY_TYPE;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.TIMESTAMP = e.TIMESTAMP;
            entityHistory.EFFECTIVE_DT = e.EFFECTIVE_DT;
            entityHistory.PEER_REVIEW_DT = e.PEER_REVIEW_DT;
            entityHistory.PEER_REVIEW_BY = e.PEER_REVIEW_BY;
            entityHistory.DIVISION = e.DIVISION;
            entityHistory.DISTRICT = e.DISTRICT;
          
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
            ////
            entityHistory.FIRMWARE_VERSION = e.FIRMWARE_VERSION;
            entityHistory.SOFTWARE_VERSION = e.SOFTWARE_VERSION;
            entityHistory.CONTROL_SERIAL_NUM = e.CONTROL_SERIAL_NUM;
            entityHistory.PROCESSED_FLAG = e.PROCESSED_FLAG;
            entityHistory.RELEASED_BY = e.RELEASED_BY;
            entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
            entityHistory.CONTROL_TYPE = e.CONTROL_TYPE;
            entityHistory.MODE = e.MODE;
            entityHistory.PRIMARY_CT_RATING = e.PRIMARY_CT_RATING;
            entityHistory.BAND_WIDTH = e.BAND_WIDTH;
            entityHistory.PT_RATIO = e.PT_RATIO;
            ////
            entityHistory.RANGE_UNBLOCKED = e.RANGE_UNBLOCKED;
            entityHistory.BLOCKED_PCT = e.BLOCKED_PCT;
            entityHistory.STEPS = e.STEPS;
            entityHistory.PEAK_LOAD = e.PEAK_LOAD;
            entityHistory.MIN_LOAD = e.MIN_LOAD;
            entityHistory.PVD_MAX = e.PVD_MAX;
            entityHistory.PVD_MIN = e.PVD_MIN;
            entityHistory.SVD_MIN = e.SVD_MIN;
            entityHistory.POWER_FACTOR = e.POWER_FACTOR;
            entityHistory.LOAD_CYCLE = e.LOAD_CYCLE;
            entityHistory.RISE_RATING = e.RISE_RATING;
            entityHistory.TIMER = e.TIMER;
            entityHistory.VOLT_VAR_TEAM_MEMBER = e.VOLT_VAR_TEAM_MEMBER;
            entityHistory.REV_THRESHOLD = e.REV_THRESHOLD;
            entityHistory.HIGH_VOLTAGE_LIMIT = e.HIGH_VOLTAGE_LIMIT;
            entityHistory.LOW_VOLTAGE_LIMIT = e.LOW_VOLTAGE_LIMIT;
            ////
            entityHistory.FWD_A_STATUS = e.FWD_A_STATUS;
            entityHistory.FWD_A_VOLT = e.FWD_A_VOLT;
            entityHistory.FWD_A_RESET = e.FWD_A_RESET;
            entityHistory.FWD_A_XSET = e.FWD_A_XSET;
            entityHistory.FWD_B_STATUS = e.FWD_B_STATUS;
            entityHistory.FWD_B_VOLT = e.FWD_B_VOLT;
            entityHistory.FWD_B_RESET = e.FWD_B_RESET;
            entityHistory.FWD_B_XSET = e.FWD_B_XSET;
            entityHistory.FWD_C_STATUS = e.FWD_C_STATUS;
            entityHistory.FWD_C_VOLT = e.FWD_C_VOLT;
            entityHistory.FWD_C_RESET = e.FWD_C_RESET;
            entityHistory.FWD_C_XSET = e.FWD_C_XSET;
            entityHistory.REV_A_VOLT = e.REV_A_VOLT;
            entityHistory.REV_A_RESET = e.REV_A_RESET;
            entityHistory.REV_A_XSET = e.REV_A_XSET;
            entityHistory.REV_B_VOLT = e.REV_B_VOLT;
            entityHistory.REV_B_RESET = e.REV_B_RESET;
            entityHistory.REV_B_XSET = e.REV_B_XSET;
            entityHistory.REV_C_VOLT = e.REV_C_VOLT;
            entityHistory.REV_C_RESET = e.REV_C_RESET;
            entityHistory.REV_C_XSET = e.REV_C_XSET;
            ////
            entityHistory.FIRMWARE_VERSION = e.FIRMWARE_VERSION;
            entityHistory.SOFTWARE_VERSION = e.SOFTWARE_VERSION;
            //entityHistory.ENGINEERING_DOCUMENT = e.ENGINEERING_DOCUMENT;
            entityHistory.SCADA = e.SCADA;
            entityHistory.SCADA_TYPE = e.SCADA_TYPE;
            entityHistory.MASTER_STATION = e.MASTER_STATION;
            entityHistory.BAUD_RATE = e.BAUD_RATE;
            entityHistory.TRANSMIT_ENABLE_DELAY = e.TRANSMIT_ENABLE_DELAY;
            entityHistory.TRANSMIT_DISABLE_DELAY = e.TRANSMIT_DISABLE_DELAY;
            entityHistory.RTU_ADDRESS = e.RTU_ADDRESS;
            entityHistory.REPEATER = e.REPEATER;
            entityHistory.SPECIAL_CONDITIONS = e.SPECIAL_CONDITIONS;
            entityHistory.RADIO_MANF_CD = e.RADIO_MANF_CD;
            entityHistory.RADIO_MODEL_NUM = e.RADIO_MODEL_NUM;
            entityHistory.RADIO_SERIAL_NUM = e.RADIO_SERIAL_NUM;
            entityHistory.USE_RX = e.USE_RX;
            entityHistory.REVERSIBLE = e.REVERSIBLE;
            entityHistory.RTU_EXIST = e.RTU_EXIST;
            entityHistory.RTU_MANF_CD = e.RTU_MANF_CD;
            entityHistory.RTU_FIRMWARE_VERSION = e.RTU_FIRMWARE_VERSION;
            entityHistory.RTU_MODEL_NUM = e.RTU_MODEL_NUM;
            entityHistory.RTU_SERIAL_NUM = e.RTU_SERIAL_NUM;
            entityHistory.RTU_SOFTWARE_VERSION = e.RTU_SOFTWARE_VERSION;
            entityHistory.ENGINEERING_COMMENTS = e.ENGINEERING_COMMENTS;
        }

        public void PopulateModelFromHistoryEntity(SM_REGULATOR_HIST e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            //this.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            //this.Notes = e.NOTES;
            this.OkToBypass = e.OK_TO_BYPASS;
            //this.ControlType = e.CONTROL_TYPE;
            this.Mode = e.MODE;
            this.PrimaryCtRating = e.PRIMARY_CT_RATING;
            this.BandWidth = e.BAND_WIDTH;
            this.PtRatio = e.PT_RATIO;
            this.RangeUnblocked = e.RANGE_UNBLOCKED;
            this.BlockedPct = e.BLOCKED_PCT;
            this.StepsUnblocked = e.STEPS;
            this.PeakLoad = e.PEAK_LOAD;
            this.MinLoad = e.MIN_LOAD;
            this.PvdMax = e.PVD_MAX;
            this.PvdMin = e.PVD_MIN;
            this.SvdMin = e.SVD_MIN;
            this.PowerFactor = e.POWER_FACTOR;
            this.LoadCycle = e.LOAD_CYCLE;
            this.RiseRating = e.RISE_RATING;
            this.Timer = e.TIMER;
            this.VoltVarTeamMember = e.VOLT_VAR_TEAM_MEMBER;
            this.ReversibleThresholdPercentage = e.REV_THRESHOLD;
            this.HighVoltageLimit = e.HIGH_VOLTAGE_LIMIT;
            this.LowVoltageLimit = e.LOW_VOLTAGE_LIMIT;
            this.ForwardAStatus = e.FWD_A_STATUS;
            this.ForwardAVoltage = e.FWD_A_VOLT;
            this.ForwardARset = e.FWD_A_RESET;
            this.ForwardAXset = e.FWD_A_XSET;
            this.ForwardBStatus = e.FWD_B_STATUS;
            this.ForwardBVoltage = e.FWD_B_VOLT;
            this.ForwardBRset = e.FWD_B_RESET;
            this.ForwardBXset = e.FWD_B_XSET;
            this.ForwardCStatus = e.FWD_C_STATUS;
            this.ForwardCVoltage = e.FWD_C_VOLT;
            this.ForwardCRset = e.FWD_C_RESET;
            this.ForwardCXset = e.FWD_C_XSET;
            this.ReverseAVoltage = e.REV_A_VOLT;
            this.ReverseARset = e.REV_A_RESET;
            this.ReverseAXset = e.REV_A_XSET;
            this.ReverseBVoltage = e.REV_B_VOLT;
            this.ReverseBRset = e.REV_B_RESET;
            this.ReverseBXset = e.REV_B_XSET;
            this.ReverseCVoltage = e.REV_C_VOLT;
            this.ReverseCRset = e.REV_C_RESET;
            this.ReverseCXset = e.REV_C_XSET;
            //this.FirmwareVersion = e.FIRMWARE_VERSION;
            //this.SoftwareVersion = e.SOFTWARE_VERSION;
            //this.EngineeringDocument = e.ENGINEERING_DOCUMENT;
            this.Scada = e.SCADA;
            this.ScadaType = e.SCADA_TYPE;
            this.MasterStation = e.MASTER_STATION;
            this.BaudRate = e.BAUD_RATE;
            this.TransmitEnableDelay = e.TRANSMIT_ENABLE_DELAY;
            this.TransmitDisableDelay = e.TRANSMIT_DISABLE_DELAY;
            this.RtuAddress = e.RTU_ADDRESS;
            this.Repeater = e.REPEATER;
            this.SpecialConditions = e.SPECIAL_CONDITIONS;
            this.ScadaRadioManufacturer = e.RADIO_MANF_CD;
            this.ScadaRadioModelNum = e.RADIO_MODEL_NUM;
            this.ScadaRadioSerialNum = e.RADIO_SERIAL_NUM;
            this.UseRx = e.USE_RX;
            this.Reversible = e.REVERSIBLE;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
        }

        public void PopulateEntityFromModelCopy(SM_REGULATOR e)
        {
            //e.OPERATING_NUM = this.OperatingNumber;
            //e.DEVICE_ID = this.DeviceId;
            e.PREPARED_BY = this.PreparedBy;
            //e.CONTROL_SERIAL_NUM = this.ControllerSerialNum;
            //e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            //e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            //e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            //e.PEER_REVIEW_BY = this.PeerReviewer;
            //e.NOTES = this.Notes;
            e.OK_TO_BYPASS = this.OkToBypass;
            //e.CONTROL_TYPE = this.ControlType;
            e.MODE = this.Mode;
            e.PRIMARY_CT_RATING = this.PrimaryCtRating;
            e.BAND_WIDTH = this.BandWidth;
            e.PT_RATIO = this.PtRatio;
            e.RANGE_UNBLOCKED = this.RangeUnblocked;
            e.BLOCKED_PCT = this.BlockedPct;
            e.STEPS = this.StepsUnblocked;
            e.PEAK_LOAD = this.PeakLoad;
            e.MIN_LOAD = this.MinLoad;
            e.PVD_MAX = this.PvdMax;
            e.PVD_MIN = this.PvdMin;
            e.SVD_MIN = this.SvdMin;
            e.POWER_FACTOR = this.PowerFactor;
            e.LOAD_CYCLE = this.LoadCycle;
            e.RISE_RATING = this.RiseRating;
            e.TIMER = this.Timer;
            e.VOLT_VAR_TEAM_MEMBER = this.VoltVarTeamMember;
            e.REV_THRESHOLD = this.ReversibleThresholdPercentage;
            e.HIGH_VOLTAGE_LIMIT = this.HighVoltageLimit;
            e.LOW_VOLTAGE_LIMIT = this.LowVoltageLimit;
            e.FWD_A_STATUS = this.ForwardAStatus;
            e.FWD_A_VOLT = this.ForwardAVoltage;
            e.FWD_A_RESET = this.ForwardARset;
            e.FWD_A_XSET = this.ForwardAXset;
            e.FWD_B_STATUS = this.ForwardBStatus;
            e.FWD_B_VOLT = this.ForwardBVoltage;
            e.FWD_B_RESET = this.ForwardBRset;
            e.FWD_B_XSET = this.ForwardBXset;
            e.FWD_C_STATUS = this.ForwardCStatus;
            e.FWD_C_VOLT = this.ForwardCVoltage;
            e.FWD_C_RESET = this.ForwardCRset;
            e.FWD_C_XSET = this.ForwardCXset;
            e.REV_B_VOLT = this.ReverseBVoltage;
            e.REV_B_XSET = this.ReverseBXset;
            e.REV_C_XSET = this.ReverseCXset;
            //e.FIRMWARE_VERSION = this.FirmwareVersion;
            //e.SOFTWARE_VERSION = this.SoftwareVersion;
            //e.ENGINEERING_DOCUMENT = this.EngineeringDocument;
            e.SCADA = this.Scada;
            e.SCADA_TYPE = this.ScadaType;
            e.MASTER_STATION = this.MasterStation;
            e.BAUD_RATE = this.BaudRate;
            e.TRANSMIT_ENABLE_DELAY = this.TransmitEnableDelay;
            e.TRANSMIT_DISABLE_DELAY = this.TransmitDisableDelay;
            e.RTU_ADDRESS = this.RtuAddress;
            e.REPEATER = this.Repeater;
            e.SPECIAL_CONDITIONS = this.SpecialConditions;
            e.RADIO_MANF_CD = this.ScadaRadioManufacturer;
            e.RADIO_MODEL_NUM = this.ScadaRadioModelNum;
            e.RADIO_SERIAL_NUM = this.ScadaRadioSerialNum;
            e.USE_RX = this.UseRx;
            e.REVERSIBLE = this.Reversible;
            e.RTU_EXIST = this.RTUExists;
            e.RTU_MANF_CD = this.RTUManufacture;
            e.RTU_FIRMWARE_VERSION = this.RTUFirmwareVersion;
            e.RTU_MODEL_NUM = this.RTUModelNumber;
            e.RTU_SERIAL_NUM = this.RTUSerialNumber;
            e.RTU_SOFTWARE_VERSION = this.RTUSoftwareVersion;
            e.ENGINEERING_COMMENTS = this.EngineeringComments;

        }
    }
}