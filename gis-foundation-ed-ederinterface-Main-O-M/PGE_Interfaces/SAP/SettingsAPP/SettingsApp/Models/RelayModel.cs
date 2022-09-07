using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SettingsApp.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Configuration;
//ENOS2EDGIS
namespace SettingsApp.Models
{
    public class RelayModel
    {

        [SettingsValidatorAttribute("RELAY", "ID")]
        [Display(Name = "ID:")]
        public long ID { get; set; }

        [SettingsValidatorAttribute("RELAY", "PROTECTION_ID")]
        [Display(Name = "Protection ID:")]
        public long ProtectionId { get; set; }

        [SettingsValidatorAttribute("RELAY", "DATECREATED")]
        [Display(Name = "Date Created:")]
        public String DateCreated { get; set; }

        [SettingsValidatorAttribute("RELAY", "CREATEDBY")]
        [Display(Name = "Created By:")]
        public String CreatedBy { get; set; }

        [SettingsValidatorAttribute("RELAY", "DATEMODIFIED")]
        [Display(Name = "Date Modified:")]
        public String DateModified { get; set; }

        [SettingsValidatorAttribute("RELAY", "MODIFIEDBY")]
        [Display(Name = "Modified By:")]
        public String ModifiedBy { get; set; }

        [SettingsValidatorAttribute("RELAY", "RELAY_CD")]
        [Display(Name = "Relay:")]
        public String RelayCd { get; set; }

        [SettingsValidatorAttribute("RELAY", "RELAY_TYPE")]
        [Display(Name = "Relay Type:")]
        public String RelayType { get; set; }

        [SettingsValidatorAttribute("RELAY", "CURVE_TYPE")]
        [Display(Name = "Relay Curve:")]
        public String CurveType { get; set; }


        [Display(Name = "Minimum Amps to Trip:")]
        [Range(0, 9999, ErrorMessage = "Please enter valid integer number, maximum length should be 4. For Example 1423, 345 ")]
        public Int32 MinAmpsTrip { get; set; }

        [Display(Name = "Relay Lever Settings:")]
        //[RegularExpression(@"^\d+\.\d{0,1}$", ErrorMessage = "Value should be in between 0 - 9999.9 : Maximum One decimal point")]
        [Range(0, 999.9, ErrorMessage = "Value should be in between 0 - 999.9 , Maximum One decimal point. For Example 142.7, 45.9")]
        public Decimal LeverSettings { get; set; }

        [Display(Name = "Instantaneous Minimum Trip:")]
        [Range(0, 99999, ErrorMessage = "Please enter valid integer number between 0 to 99999")]
        public Decimal InstTrip { get; set; }

        [SettingsValidatorAttribute("RELAY", "RESTRAINT_TYPE")]
        [Display(Name = "Restraint (type):")]
        public String RestraintType { get; set; }

        [Display(Name = "Restraint Pickup (amps):")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 999.999, ErrorMessage = "Value should be in between 0 to 999.999 : Maximum Three decimal points. For Example 142.756, 45.934")]
        public Decimal RestraintPickup { get; set; }

        [Display(Name = "Minimum Voltage Pickup (P.U):")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 99.999, ErrorMessage = "Value should be in between 0 to 99.999 : Maximum Three decimal points.For Example 45.756, 9.345")]
        public Decimal MinVoltPickup { get; set; }
        
        [Display(Name = "Volatge Setpoint (Volts):")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 999.999, ErrorMessage = "Value should be in between 0 to 999.999 : Maximum Three decimal points. For Example 142.756, 45.934")]
        public Decimal VoltageSetPoint { get; set; }

        [SettingsValidatorAttribute("RELAY", "CURRENT_FUTURE")]
        [Display(Name = "Current or Future:")]
        public String CurrentOrFuture { get; set; }

        public SelectList RelayCodeList { get; set; }
        public SelectList RelayTypeList { get; set; }
        public SelectList CurveTypeList { get; set; }
        public SelectList RestraintList { get; set; }
        private string CurrentOrFuture_C = ConfigurationManager.AppSettings["CurrentOrFuture_C"];

        public void PopulateEntityFromModel(SM_RELAY e)
        {
            e.ID = this.ID;
            e.PROTECTION_ID = this.ProtectionId;
            e.DATECREATED = Utility.ParseDateTime(this.DateCreated);
            e.CREATEDBY = this.CreatedBy;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.MODIFIEDBY = this.ModifiedBy;
            e.RELAY_CD = this.RelayCd;
            e.RELAY_TYPE = this.RelayType;
            e.CURVE_TYPE = this.CurveType;
            e.MIN_AMPS_TRIP = Convert.ToInt16(this.MinAmpsTrip);
            e.LEVER_SETTING = this.LeverSettings;
            e.INST_TRIP = Convert.ToInt32(this.InstTrip);
            e.RESTRAINT_TYPE = this.RestraintType;
            e.RESTRAINT_PICKUP = this.RestraintPickup;
            e.MIN_VOLT_PICKUP = this.MinVoltPickup;
            e.VOLTAGE_SETPOINT = this.VoltageSetPoint;
            e.CURRENT_FUTURE = !string.IsNullOrEmpty(this.CurrentOrFuture) ? this.CurrentOrFuture : CurrentOrFuture_C;
        }

        public void PopulateModelFromEntity(SM_RELAY e)
        {
            this.ID = e.ID;
            this.ProtectionId = e.PROTECTION_ID;
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy = e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy = e.MODIFIEDBY;
            this.RelayCd = e.RELAY_CD;
            this.RelayType = e.RELAY_TYPE;
            this.CurveType = e.CURVE_TYPE;
            this.MinAmpsTrip = e.MIN_AMPS_TRIP.HasValue ? Convert.ToInt16(e.MIN_AMPS_TRIP) : 0;
            this.LeverSettings = e.LEVER_SETTING.HasValue ? Convert.ToDecimal(e.LEVER_SETTING): 0;
            this.InstTrip = e.INST_TRIP.HasValue ? Convert.ToInt32(e.INST_TRIP): 0;
            this.RestraintType = e.RESTRAINT_TYPE;
            this.RestraintPickup = e.RESTRAINT_PICKUP.HasValue ? Convert.ToDecimal(e.RESTRAINT_PICKUP):0;
            this.MinVoltPickup = e.MIN_VOLT_PICKUP.HasValue ? Convert.ToDecimal(e.MIN_VOLT_PICKUP):0;
            this.VoltageSetPoint = e.VOLTAGE_SETPOINT.HasValue ? Convert.ToDecimal(e.VOLTAGE_SETPOINT):0;
            this.CurrentOrFuture = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
        }

        public void PopulateHistoryFromEntity(SM_RELAY_HIST entityHistory, SM_RELAY e)
        {
            entityHistory.SM_RELAY_ID = e.ID;
            entityHistory.PROTECTION_ID = e.PROTECTION_ID;
            entityHistory.DATECREATED = e.DATECREATED;
            entityHistory.CREATEDBY = e.CREATEDBY;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.MODIFIEDBY = e.MODIFIEDBY;
            entityHistory.RELAY_CD = e.RELAY_CD;
            entityHistory.RELAY_TYPE = e.RELAY_TYPE;
            entityHistory.CURVE_TYPE = e.CURVE_TYPE;
            entityHistory.MIN_AMPS_TRIP = e.MIN_AMPS_TRIP;
            entityHistory.LEVER_SETTING = e.LEVER_SETTING;
            entityHistory.INST_TRIP = e.INST_TRIP;
            entityHistory.RESTRAINT_TYPE = e.RESTRAINT_TYPE;
            entityHistory.RESTRAINT_PICKUP = e.RESTRAINT_PICKUP;
            entityHistory.MIN_VOLT_PICKUP = e.MIN_VOLT_PICKUP;
            entityHistory.VOLTAGE_SETPOINT = e.VOLTAGE_SETPOINT;
            entityHistory.CURRENT_FUTURE = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
        }

        public void PopulateModelFromHistoryEntity(SM_RELAY_HIST e)
        {
            this.ID = (long)e.SM_RELAY_ID;
            this.ProtectionId = e.PROTECTION_ID;
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy = e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy = e.MODIFIEDBY;
            this.RelayCd = e.RELAY_CD;
            this.RelayType = e.RELAY_TYPE;
            this.CurveType = e.CURVE_TYPE;
            this.MinAmpsTrip = e.MIN_AMPS_TRIP.HasValue ? Convert.ToInt16(e.MIN_AMPS_TRIP) : 0; ;
            this.LeverSettings = e.LEVER_SETTING.HasValue ? Convert.ToDecimal(e.LEVER_SETTING) : 0;
            this.InstTrip = e.INST_TRIP.HasValue ? Convert.ToInt32(e.INST_TRIP) : 0;
            this.RestraintType = e.RESTRAINT_TYPE;
            this.RestraintPickup = e.RESTRAINT_PICKUP.HasValue ? Convert.ToDecimal(e.RESTRAINT_PICKUP) : 0;
            this.MinVoltPickup = e.MIN_VOLT_PICKUP.HasValue ? Convert.ToDecimal(e.MIN_VOLT_PICKUP) : 0;
            this.VoltageSetPoint = e.VOLTAGE_SETPOINT.HasValue ? Convert.ToDecimal(e.VOLTAGE_SETPOINT) : 0;
            this.CurrentOrFuture = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
        }
    }
}