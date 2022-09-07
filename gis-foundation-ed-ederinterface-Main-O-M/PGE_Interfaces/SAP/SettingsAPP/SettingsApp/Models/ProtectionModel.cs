using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SettingsApp.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Configuration;
using System.ComponentModel.DataAnnotations.Schema;

//ENOS2EDGIS
namespace SettingsApp.Models
{
    public class ProtectionModel
    {
        
        [SettingsValidatorAttribute("PROTECTION", "ID")]
        [Display(Name = "ID:")]
        public long ID { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "PARENT_TYPE")]
        [Display(Name = "Parent Type:")]
        public System.String ParentType { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "PARENT_ID")]
        [Display(Name = "Parent Id:")]
        public System.Int32 ParentID { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "DATECREATED")]
        [Display(Name = "Date Created:")]
        public System.String DateCreated { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "CREATEDBY")]
        [Display(Name = "Created By:")]
        public System.String CreatedBy { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "DATEMODIFIED")]
        [Display(Name = "Date modified:")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "MODIFIEDBY")]
        [Display(Name = "Modified By:")]
        public System.String ModifiedBy { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "PROTECTION_TYPE")]
        [Display(Name = "Protection Type:")]
        public System.String ProtectionType { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "EXPORT_KW")]
        [Display(Name = "Export kW:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999.999,ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example : 1234567.569, 4456.76")]
        public System.Decimal? ExportkW { get; set; }

        /******************************ENOS2SAP PhaseIII Start**************************************/
        [NotMapped]    
        [Display(Name = "Total System Nameplate kW:")]
        public System.String NameplateRating { get; set; }
        /******************************ENOS2SAP PhaseIII End**************************************/

        [SettingsValidatorAttribute("PROTECTION", "FUSE_SIZE")]
        [Display(Name = "Fuse Size:")]
        [MaxLength(10)]
        public System.String FuseSize { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "FUSE_TYPE")]
        [Display(Name = "Fuse Type:")]
        public System.String FuseType { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "MANUFACTURER")]
        [Display(Name = "Manufacturer:")]
        [MaxLength(100)]
        public System.String Manufacturer { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "CERTIFIED_INVERTER")]
        [Display(Name = "Certified Inverter:")]
        public System.String CertifiedInverter { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "PHA_MIN_AMPS_TRIP")]
        [Display(Name = "Phase Min Amps Trip:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 999.99, ErrorMessage = "Value should be in between 0 - 999.99 : Maximum Two decimal points.For Example : 123.56, 476")]
        public System.Decimal? PhaseMinAmpsTrip { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "PHA_SLOW_CURVE")]
        [Display(Name = "Phase Slow Curve:")]
        public System.String PhaseSlowCurve { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "PHA_FAST_CURVE")]
        [Display(Name = "Phase Fast Curve:")]
        public System.String PhaseFastCurve { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "PHA_TIME_DIAL")]
        [Display(Name = "Phase Time Dial:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9.99, ErrorMessage = "Value should be in between 0 - 9.99 : Maximum Two decimal points.For Example : 0.56, 4.76")]
        public System.Decimal? PhaseTimeDial { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "PHA_INST")]
        [Display(Name = "Phase Instantaneous:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 999.99, ErrorMessage = "Value should be in between 0 - 999.99 : Maximum Two decimal points.For Example : 123.56, 47")]
        public System.Decimal? PhaseInst { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "GRD_MIN_AMPS_TRIP")]
        [Display(Name = "Ground Min Ams Trip:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 999.99, ErrorMessage = "Value should be in between 0 - 999.99 : Maximum Two decimal points.For Example : 123.56, 47")]
        public System.Decimal? GroundMinAmpsTrip { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "GRD_SLOW_CURVE")]
        [Display(Name = "Ground Slow Curve:")]
        public System.String GroundSlowCurve { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "GRD_FAST_CURVE")]
        [Display(Name = "Ground Fast Curve:")]
        public System.String GroundFastCurve { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "GRD_TIME_DIAL")]
        [Display(Name = "Ground Time Dial:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9.99, ErrorMessage = "Value should be in between 0 - 9.99 : Maximum Two decimal points.For Example : 0.56, 4.76")]
        public System.Decimal? GroundTimeDial { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "GRD_INST")]
        [Display(Name = "Ground Instantaneous:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 999.99, ErrorMessage = "Value should be in between 0 - 999.99 : Maximum Two decimal points.For Example : 123.56, 478.00")]
        public System.Decimal? GroundInst { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "NOTES")]
        [Display(Name = "Notes:")]
        [MaxLength(500, ErrorMessage = "Value should be in exceed 500 charectors")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("PROTECTION", "CURRENT_FUTURE")]
        [Display(Name = "Current or Future:")]
        public String CurrentOrFuture { get; set; }

        [HiddenInput]
        public List<RelayModel> Relays { get; set; }
        [HiddenInput]
        public List<GeneratorModel> Generators { get; set; }

        public SelectList ProtectionTypeList { get; set; }
        public SelectList FuseTypeList { get; set; }
        public SelectList RecloseCurveList { get; set; }

        public string ProtectionTypeDropDownPostbackScript { get; set; }
        private string CurrentOrFuture_C = ConfigurationManager.AppSettings["CurrentOrFuture_C"];

        public void PopulateEntityFromModel(SM_PROTECTION e)
        {
            e.ID = this.ID;
            e.PARENT_TYPE = this.ParentType;
            e.PARENT_ID = this.ParentID;
            e.DATECREATED = Utility.ParseDateTime(this.DateCreated);
            e.CREATEDBY = this.CreatedBy;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.MODIFIEDBY = this.ModifiedBy;
            e.PROTECTION_TYPE = this.ProtectionType;
            e.EXPORT_KW = this.ExportkW;
            e.FUSE_SIZE = this.FuseSize;
            e.FUSE_TYPE = this.FuseType;
            e.MANUFACTURER = this.Manufacturer;
            e.CERTIFIED_INVERTER = this.CertifiedInverter;
            e.PHA_MIN_AMPS_TRIP = this.PhaseMinAmpsTrip;
            e.PHA_SLOW_CURVE = this.PhaseSlowCurve;
            e.PHA_FAST_CURVE = this.PhaseFastCurve;
            e.PHA_TIME_DIAL = this.PhaseTimeDial;
            e.PHA_INST = this.PhaseInst;
            e.GRD_MIN_AMPS_TRIP = this.GroundMinAmpsTrip;
            e.GRD_SLOW_CURVE = this.GroundSlowCurve;
            e.GRD_FAST_CURVE = this.GroundFastCurve;
            e.GRD_TIME_DIAL = this.GroundTimeDial;
            e.GRD_INST = this.GroundInst;
            e.NOTES = this.Notes;
            e.CURRENT_FUTURE = !string.IsNullOrEmpty(this.CurrentOrFuture) ? this.CurrentOrFuture : CurrentOrFuture_C;
        }

        public void PopulateModelFromEntity(SM_PROTECTION e)
        {
            this.ID = e.ID;
            this.ParentType = e.PARENT_TYPE;
            this.ParentID = Convert.ToInt32(e.PARENT_ID);
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy = e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy = e.MODIFIEDBY;
            this.ProtectionType = e.PROTECTION_TYPE ;
            this.ExportkW = e.EXPORT_KW;
            this.FuseSize = e.FUSE_SIZE;
            this.FuseType = e.FUSE_TYPE;
            this.Manufacturer = e.MANUFACTURER;
            this.CertifiedInverter = e.CERTIFIED_INVERTER;
            this.PhaseMinAmpsTrip = e.PHA_MIN_AMPS_TRIP;
            this.PhaseSlowCurve = e.PHA_SLOW_CURVE;
            this.PhaseFastCurve = e.PHA_FAST_CURVE;
            this.PhaseTimeDial = e.PHA_TIME_DIAL;
            this.PhaseInst = e.PHA_INST;
            this.GroundMinAmpsTrip = e.GRD_MIN_AMPS_TRIP;
            this.GroundSlowCurve = e.GRD_SLOW_CURVE;
            this.GroundFastCurve = e.GRD_FAST_CURVE;
            this.GroundTimeDial = e.GRD_TIME_DIAL;
            this.GroundInst = e.GRD_INST;
            this.Notes = e.NOTES;
            this.CurrentOrFuture = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
        }
        public void PopulateHistoryFromEntity(SM_PROTECTION_HIST entityHistory, SM_PROTECTION e)
        {
            entityHistory.ID = e.ID;
            entityHistory.PARENT_TYPE = e.PARENT_TYPE;
            entityHistory.PARENT_ID = e.PARENT_ID;
            entityHistory.DATECREATED = e.DATECREATED;
            entityHistory.CREATEDBY = e.CREATEDBY;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.MODIFIEDBY = e.MODIFIEDBY;
            entityHistory.PROTECTION_TYPE = e.PROTECTION_TYPE;
            entityHistory.EXPORT_KW = e.EXPORT_KW;
            entityHistory.FUSE_SIZE = e.FUSE_SIZE;
            entityHistory.FUSE_TYPE = e.FUSE_TYPE;
            entityHistory.MANUFACTURER = e.MANUFACTURER;
            entityHistory.CERTIFIED_INVERTER = e.CERTIFIED_INVERTER;
            entityHistory.PHA_MIN_AMPS_TRIP = e.PHA_MIN_AMPS_TRIP;
            entityHistory.PHA_SLOW_CURVE = e.PHA_SLOW_CURVE;
            entityHistory.PHA_FAST_CURVE = e.PHA_FAST_CURVE;
            entityHistory.PHA_TIME_DIAL = e.PHA_TIME_DIAL;
            entityHistory.PHA_INST = e.PHA_INST;
            entityHistory.GRD_MIN_AMPS_TRIP = e.GRD_MIN_AMPS_TRIP;
            entityHistory.GRD_SLOW_CURVE = e.GRD_SLOW_CURVE;
            entityHistory.GRD_FAST_CURVE = e.GRD_FAST_CURVE;
            entityHistory.GRD_TIME_DIAL = e.GRD_TIME_DIAL;
            entityHistory.GRD_INST = e.GRD_INST;
            entityHistory.NOTES = e.NOTES;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
        }

        public void PopulateModelFromHistoryEntity(SM_PROTECTION_HIST e)
        {
            this.ID = e.ID;
            this.ParentType = e.PARENT_TYPE;
            this.ParentID = Convert.ToInt32(e.PARENT_ID);
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy = e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy = e.MODIFIEDBY;
            this.ProtectionType = e.PROTECTION_TYPE;
            this.ExportkW = e.EXPORT_KW;
            this.FuseSize = e.FUSE_SIZE;
            this.FuseType = e.FUSE_TYPE;
            this.Manufacturer = e.MANUFACTURER;
            this.CertifiedInverter = e.CERTIFIED_INVERTER;
            this.PhaseMinAmpsTrip = e.PHA_MIN_AMPS_TRIP;
            this.PhaseSlowCurve = e.PHA_SLOW_CURVE;
            this.PhaseFastCurve = e.PHA_FAST_CURVE;
            this.PhaseTimeDial = e.PHA_TIME_DIAL;
            this.PhaseInst = e.PHA_INST;
            this.GroundMinAmpsTrip = e.GRD_MIN_AMPS_TRIP;
            this.GroundSlowCurve = e.GRD_SLOW_CURVE;
            this.GroundFastCurve = e.GRD_FAST_CURVE;
            this.GroundTimeDial = e.GRD_TIME_DIAL;
            this.GroundInst = e.GRD_INST;
            this.Notes = e.NOTES;
            this.CurrentOrFuture = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
        }
    }
}