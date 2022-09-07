using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class NetworkProtectorModel
    {

        [SettingsValidatorAttribute("Network_Protector", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "SERIAL_NUM")]
        [Display(Name = "Serial Number :")]
        public System.String SerialNumber { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special Conditions / Comments :")]
        public System.String SpecialConditions { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "TRIP_MODE")]
        [Display(Name = "Trip Mode :")]
       
        public System.Int16? TripMode { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "CTRATIO")]
        [Display(Name = "CT Ratio :")]
        public System.Int64? CtRatio { get; set; }


        [SettingsValidatorAttribute("Network_Protector", "TIME_DELAY")]
        [Display(Name = "Time Delay :")]
        [Range(0, 300, ErrorMessage = "Enter number between 0 to 300")]
        public System.Int16? TimeDelay { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "REVERSETRIP")]
        [Display(Name = "Reverse Trip :")]
        public System.Decimal? ReverseTripSetting { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "OVERCURRENT")]
        [Display(Name = "Over current :")]
        public System.Decimal? OvercurTrip { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "CLOSEMODE")]
        [Display(Name = "Close Mode :")]
        public System.String ClosingMode { get; set; }

      

        [SettingsValidatorAttribute("Network_Protector", "MASTERLINEVOLTAGEOFFSET")]
        [Display(Name = "Master Line :")]
        public System.Decimal MasterLineML { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "PHASINGLINEVOLTAGEOFFSET")]
        [Display(Name = "Phasing Line :")]
        public System.Decimal PhasingLineOffset { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "PHASINGLINEANGLE")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Phasing Line Angle :")]
        public System.Decimal PhasingLineAngle { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "MASTERLINEANGLE")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Master Line :")]
        public System.Decimal MasterLineAngle { get; set; }

       // [SettingsValidatorAttribute("Network_Protector", "WATTVARTRIP")]
        [Display(Name = "Watt-Var Trip")]
        public System.Int16? WattVarTrip { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "WATTTRIPANGLE")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Watt-Trip Angle")]
        public System.Decimal? WattTripAngle { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "GULLWINGANGLE")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Gull-Wing Angle :")]
        public System.Decimal? GullWingAngle { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "VARTRIPANGLE")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Voltage can't have more than 1 decimal places")]
        [Display(Name = "Var-Trip Angle :")]
        public System.Decimal? VarTripAngle { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceId { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("Network_Protector", "NOTES")]
        [Display(Name = "Notes :")]
        public System.String Notes { get; set; }


       // [SettingsValidatorAttribute("Network_Protector", "CTPRIMARYRATING")]
        // [Range(50, 1000, ErrorMessage = "Value should be in between 50 - 1000")]
        [Display(Name = "Ct Primary Rating :")]
        public System.Decimal CtPrimaryRating { get; set; }

        
        //ssbl

        public bool IniLowCutOffChk { get; set; }
        public bool Release { get; set; }
        public List<GISAttributes> GISAttributes { get; set; }

        public SelectList TripModeList { get; set; }   //new
        public SelectList ClosingModeList { get; set; }//new
        public SelectList WattVarTripCharList { get; set; }
        public string DropDownPostbackScript { get; set; }
        public string IniLowChkPostbackScript { get; set; }
        public bool IniDefiniteTime1Chk { get; set; }
        public void PopulateEntityFromModel(SM_NETWORK_PROTECTOR e)
        {
            e.DEVICE_ID = this.DeviceId;
            e.PREPARED_BY = this.PreparedBy;
            e.SERIAL_NUM = this.SerialNumber;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            e.NOTES = this.Notes;
            e.CTRATIO = this.CtRatio;
            e.REVERSETRIP = this.ReverseTripSetting;
            e.TIMEDELAY = Convert.ToInt16(this.TimeDelay);
            e.TRIPMODE = this.TripMode;
            e.CLOSEMODE = this.ClosingMode;
            e.SPECIAL_CONDITIONS = this.SpecialConditions;
            e.VARTRIPANGLE = this.VarTripAngle;
            e.WATTTRIPANGLE = this.WattTripAngle;
            e.WATTVARTRIP = this.WattVarTrip;
            
            e.CTPRIMARYRATING = this.CtPrimaryRating;
            e.GULLWINGANGLE = this.GullWingAngle;
            e.MASTERLINEANGLE = this.MasterLineAngle;
            e.MASTERLINEVOLTAGEOFFSET = this.MasterLineML;
            e.OVERCURRENT = this.OvercurTrip;
            e.PHASINGLINEANGLE = this.PhasingLineAngle;
            e.PHASINGLINEVOLTAGEOFFSET = this.PhasingLineOffset;
           
        }

        public void PopulateModelFromEntity(SM_NETWORK_PROTECTOR e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            this.SerialNumber = e.SERIAL_NUM;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.Notes = e.NOTES;

            if (e.TRIPMODE == null)
                this.TripMode = e.TRIPMODE = 3;
            else
                this.TripMode = e.TRIPMODE;
            this.ClosingMode = e.CLOSEMODE;
            this.SpecialConditions = e.SPECIAL_CONDITIONS;
            if (e.CTRATIO == null)
                this.CtRatio = Convert.ToInt64(e.CTRATIO);
            else
                this.CtRatio = Convert.ToInt64(e.CTRATIO);
            if (e.REVERSETRIP == null)
                this.ReverseTripSetting = Convert.ToDecimal(e.REVERSETRIP);

            else
                this.ReverseTripSetting = Convert.ToDecimal(e.REVERSETRIP);
            if (e.TIMEDELAY == null)
                this.TimeDelay = 0;
            else

                this.TimeDelay = e.TIMEDELAY;
            if(e.OVERCURRENT==null)
                this.OvercurTrip = 0;
            else
                this.OvercurTrip = Convert.ToDecimal(e.OVERCURRENT);
            this.PhasingLineAngle = Convert.ToDecimal(e.PHASINGLINEANGLE);
            this.PhasingLineOffset = Convert.ToDecimal(e.PHASINGLINEVOLTAGEOFFSET);
            this.MasterLineML = Convert.ToDecimal(e.MASTERLINEVOLTAGEOFFSET);
            this.MasterLineAngle = Convert.ToDecimal(e.MASTERLINEANGLE);
            this.GullWingAngle = Convert.ToDecimal(e.GULLWINGANGLE);
            this.VarTripAngle = Convert.ToDecimal(e.VARTRIPANGLE);
            this.WattTripAngle = Convert.ToDecimal(e.WATTTRIPANGLE);
            if (e.WATTVARTRIP == null)
                this.WattVarTrip = e.WATTVARTRIP = 0;

            else
                this.WattVarTrip = e.WATTVARTRIP;
               //this.ScadaRadioSerialNum = e.RADIO_SERIAL_NUM;
            //if (this.WattVarTrip == 0)
            //    this.IniLowCutOffChk = true;
            //else
            //{
            //    this.IniLowCutOffChk = false;
            //    this.WattVarTrip = 1;
            //}
            
        }

        public void PopulateHistoryFromEntity(SM_NETWORK_PROTECTOR_HIST entityHistory, SM_NETWORK_PROTECTOR e)
        {

            entityHistory.GLOBAL_ID = e.GLOBAL_ID;
            entityHistory.FEATURE_CLASS_NAME = e.FEATURE_CLASS_NAME;
            entityHistory.OPERATING_NUM = e.OPERATING_NUM;
            entityHistory.DEVICE_ID = e.DEVICE_ID;
            entityHistory.PREPARED_BY = e.PREPARED_BY;
           // entityHistory.RELAY_TYPE = e.RELAY_TYPE;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
          //  entityHistory.TIMESTAMP = e.TIMESTAMP;
            entityHistory.EFFECTIVE_DT = e.EFFECTIVE_DT;
            entityHistory.PEER_REVIEW_DT = e.PEER_REVIEW_DT;
            entityHistory.PEER_REVIEW_BY = e.PEER_REVIEW_BY;
            entityHistory.DIVISION = e.DIVISION;
            entityHistory.DISTRICT = e.DISTRICT;
            entityHistory.NOTES = e.NOTES;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
            entityHistory.CTRATIO = e.CTRATIO;
            entityHistory.SPECIAL_CONDITIONS = e.SPECIAL_CONDITIONS;
            entityHistory.VARTRIPANGLE = e.VARTRIPANGLE;
            entityHistory.TIMEDELAY = e.TIMEDELAY;
            entityHistory.TRIPMODE = e.TRIPMODE;
            entityHistory.CLOSEMODE = e.CLOSEMODE;
            entityHistory.WATTTRIPANGLE = e.WATTTRIPANGLE;
            entityHistory.WATTVARTRIP = e.WATTVARTRIP;
            entityHistory.MASTERLINEANGLE = e.MASTERLINEANGLE;
            entityHistory.MASTERLINEVOLTAGEOFFSET = e.MASTERLINEVOLTAGEOFFSET;
            entityHistory.NOTES = e.NOTES;
            entityHistory.OVERCURRENT = e.OVERCURRENT;
            entityHistory.PHASINGLINEANGLE = e.PHASINGLINEANGLE;
            entityHistory.PHASINGLINEVOLTAGEOFFSET = e.PHASINGLINEVOLTAGEOFFSET;
            entityHistory.REVERSETRIP = e.REVERSETRIP;
            entityHistory.SERIAL_NUM = e.SERIAL_NUM;
            entityHistory.SPECIAL_CONDITIONS = e.SPECIAL_CONDITIONS;
            entityHistory.GULLWINGANGLE = e.GULLWINGANGLE;
        }

        public void PopulateModelFromHistoryEntity(SM_NETWORK_PROTECTOR_HIST e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
          //  this.SerialNumber = e.CONTROL_SERIAL_NUM;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.Notes = e.NOTES;
            this.SpecialConditions = e.SPECIAL_CONDITIONS;
            this.SerialNumber = e.SERIAL_NUM;
            this.ReverseTripSetting = Convert.ToDecimal(e.REVERSETRIP);
            this.TimeDelay = e.TIMEDELAY;
            if (e.TRIPMODE == null)
                this.TripMode = e.TRIPMODE = 3;
            else
                this.TripMode = e.TRIPMODE;
            this.VarTripAngle = Convert.ToDecimal(e.VARTRIPANGLE);
            this.WattTripAngle = Convert.ToDecimal(e.WATTTRIPANGLE);
            //this.WattVarTrip = e.WATTVARTRIP;
            this.CtRatio = Convert.ToInt32(e.CTRATIO);
            this.CtPrimaryRating = Convert.ToInt32(e.CTPRIMARYRATING);
            this.GullWingAngle = Convert.ToDecimal(e.GULLWINGANGLE);
            this.MasterLineAngle = Convert.ToDecimal(e.MASTERLINEANGLE);
            this.MasterLineML = Convert.ToInt32(e.MASTERLINEVOLTAGEOFFSET);
            this.OvercurTrip = Convert.ToDecimal(e.OVERCURRENT);

            this.ClosingMode = e.CLOSEMODE;
            this.PhasingLineAngle = Convert.ToDecimal(e.PHASINGLINEANGLE);
            this.PhasingLineOffset = Convert.ToInt32(e.PHASINGLINEVOLTAGEOFFSET);

            if (e.WATTVARTRIP == null)
                this.WattVarTrip = e.WATTVARTRIP = 0;

            else
                this.WattVarTrip = e.WATTVARTRIP;
        }
    }
} 