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
    public class GeneratorModel
    {
        public SelectList ConnectionTypeList { get; set; }
        public SelectList StatusList { get; set; }
        public SelectList GenTechCodeList { get; set; }
        public SelectList ModeOfInverterList { get; set; }
        public SelectList ControlList { get; set; }
        public SelectList ProgramTypeList { get; set; }
        public SelectList TechnologyTypeList { get; set; }
        public SelectList PowerSourceList { get; set; }
        public SelectList CertificationList {get;set;}

        [SettingsValidatorAttribute("GENERATOR", "ID")]
        [Display(Name = "ID:")]
        public long ID { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "PROTECTION_ID")]
        [Display(Name = "Protection ID:")]
        public long ProtectionId { get; set; }

        //[SettingsValidatorAttribute("GENERATOR", "SAP_EGI_NOTIFICATION")]
        [Display(Name = "SAP EGI Notification:")]
        [MaxLength(12)]
        public String SapEgiNotification { get; set; }


        //[SettingsValidatorAttribute("GENERATOR", "SAP_QUEUE_NUMBER")]
        [Display(Name = "SAP Queue Number:")]
        [MaxLength(8)]
        public String SapQueueNumber { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "SAP_EQUIPMENT_ID")]
        [Display(Name = "SAP Equipment ID:")]
        [Required(ErrorMessage = "SAP_EQUIPMENT_ID should not be empty")]
       // [RegularExpression(@"^[a-zA-Z0-9'' ']+$", ErrorMessage = "Special character or -1 should not be entered")]
       [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Value should be number ")]

        [MaxLength(18)]
        public String SapEquipmentID { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "DATECREATED")]
        [Display(Name = "Date Created:")]
        public String DateCreated { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "CREATEDBY")]
        [Display(Name = "Created By:")]
        public String CreatedBy { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "DATEMODIFIED")]
        [Display(Name = "Date Modified:")]
        public String DateModified { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "MODIFIEDBY")]
        [Display(Name = "Modified By:")]
        public String ModifiedBy { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "POWER_SOURCE")]
        [Display(Name = "Power Source:")]
        //[MaxLength(40, ErrorMessage = "Should not exceed more than 40 charectors.")]
        public String PowerSource { get; set; }

        [Display(Name = "Project Name:")]
        [MaxLength(40, ErrorMessage = "Should not exceed more than 40 charectors.")]
        public String ProjectName { get; set; }

        [Display(Name = "Manufacturer")]
        [MaxLength(100, ErrorMessage = "Should not exceed more than 100 charectors.")]
        public String Manufacturer { get; set; }

        [Display(Name = "Model")]
        [MaxLength(100, ErrorMessage = "Should not exceed more than 100 charectors.")]
        public String Model { get; set; }


        [Display(Name = "Inverter Efficiency(%):")]
        //[RegularExpression(@"^\d+\.\d{0,4}$")]
        [Range(0, 9.9999, ErrorMessage = "Value should be in between 0 - 9.9999 : Maximum Four decimal points. For Example : 0.5555, 2.45")]
        public Decimal? InverterEfficiency { get; set; }


        [Display(Name = "Nameplate Rating kW per Unit:")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]       
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points.For Example : 234324.555, 0.45")]
        public Decimal? NameplateRating { get; set; }

        [Display(Name = "Quantity")]
        [Range(0, 99999, ErrorMessage = "Please enter valid integer number between 0 to 99999")]
        public Int32? Quantity { get; set; }


        [Display(Name = "Total Nameplate Capacity kW:")] //ENOS2EDGIS
       // [RegularExpression(@"^\d+\.\d{0,3}$")]       
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points.For Example : 234324.555, 0.45")]
        public Decimal? NameplateCapacity { get; set; }

        /*********************ENOS2EDGIS Start**********************/
        [Display(Name = "Rated Power Factor(%):")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]            
        [Range(-9.99, 9.99, ErrorMessage = "Value should be in between -9.99 - 9.99 : Maximum Two decimal points.For Example : 0.56, 2.45")]
        public Decimal? PowerFactor { get; set; }
        /*********************ENOS2EDGIS End***********************/

        [Display(Name = "Effective Rating kW:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 999999999.999, ErrorMessage = "Value should be in between 0 - 999999999.999 : Maximum Three decimal points. For Example : 345345.569, 67.4")]
        public Decimal? EffectiveRatingkW { get; set; }

        [Display(Name = "Effective Rating kVA:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 999999999.999, ErrorMessage = "Value should be in between 0 - 999999999.999 : Maximum Three decimal points. For Example : 345345.569, 67.4")]
        public Decimal? EffectiveRatingkVa { get; set; }


        [Display(Name = "Rated Voltage (KV L-L):")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 99999999.999, ErrorMessage = "Value should be in between 0 - 99999999.999 : Maximum Three decimal points. For Example : 34535.569, 0.445")]
        public Decimal? RatedVolatge { get; set; }

        [Display(Name = "Number Of Phases")]
        [Range(0, 99999, ErrorMessage = "Please enter valid integer number between 0 to 99999")]
        public Int32? NumberOfPhases { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "MODE_OF_INVERTER")]
        [Display(Name = "Mode of Inverter Usage:")]
        public String ModeOfInverter { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "GEN_TECH_CD")]
        [Display(Name = "Gen Tech:")]
        public String GenTechCd { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "PTO_DATE")]
        [Display(Name = "PTO:")]
        public String PTODate { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "PROGRAM_TYPE")]
        [Display(Name = "Program Type:")]
        public String ProgramType { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "CONTROL_CD")]
        [Display(Name = "Control:")]
        public String ControlCD { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "CONNECTION_CD")]
        [Display(Name = "Connection:")]
        public String ConnectionCD { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "STATUS_CD")]
        [Display(Name = "Status:")]
        public String StatusCD { get; set; }


        [Display(Name = "Steady State Reactance (Xd)(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? SSReactance { get; set; }


        [Display(Name = "Steady State Resistance (Rd)(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? SSResistance { get; set; }


        [Display(Name = "Transient Reactance (Xd')(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? TransReactance { get; set; }

        [Display(Name = "Transient Resistance (Rd')(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? TransResistance { get; set; }

        [Display(Name = @"Subtransient Reactance (Xd"")(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? SubTransReactance { get; set; }

        [Display(Name = @"Subtransient Resistance (Rd"")(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? SubTransResistance { get; set; }

        [Display(Name = "Negative Sequence Reactance (X2)(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? NegativeReactance { get; set; }

        [Display(Name = "Negative Sequence Resistance (R2)(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? NegativeResistance { get; set; }

        [Display(Name = "Zero Sequence Reactance (X0)(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? ZeroReactance { get; set; }

        [Display(Name = "Zero Sequence Resistance (X0)(P.U.):")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? ZeroResistance { get; set; }

        [Display(Name = "Ground (Ohms) Reactance:")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? GroundReactance { get; set; }


        [Display(Name = "Ground (Ohms) Resistance:")]
        //[RegularExpression(@"^\d+\.\d{0,5}$")]
        [Range(0, 99999999.99999, ErrorMessage = "Value should be in between 0 - 99999999.99999 : Maximum Five decimal points. For Example : 34534.56789, 7657.23")]
        public Decimal? GroundResistance { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "NOTES")]
        [Display(Name = "Notes:")]
        [MaxLength(500, ErrorMessage="Should not exceed more than 500 charectors")]
        public String Notes { get; set; }

        //[SettingsValidatorAttribute("GENERATOR", "CERTIFICATION")]
        [Display(Name = "Certification:")]
        public String Certification { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "GEN_TECH_EQUIPMENT")]
        [Display(Name = "Gen Tech Equipment:")]
        public String GenTechEquipment { get; set; }


        [SettingsValidatorAttribute("GENERATOR", "CURRENT_FUTURE")]
        [Display(Name = "Current or Future:")]
        public String CurrentOrFuture { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "TECH_TYPE_CD")]
        [Display(Name = "Technology Type:")]
        public String TechType { get; set; }

        [SettingsValidatorAttribute("GENERATOR", "BACKUP_GEN")]
        [Display(Name = "Backup Generator Present at Site:")]
        public System.Boolean BackupGeneration { get; set; }

        [HiddenInput]
        public List<GenEquipmentModel> Equipments { get; set; }

        public string GenTypeDropDownPostbackScript { get; set; }

        private string CurrentOrFuture_C = ConfigurationManager.AppSettings["CurrentOrFuture_C"];
        public void PopulateEntityFromModel(SM_GENERATOR e)
        {
            e.ID = this.ID;
            e.PROTECTION_ID = this.ProtectionId;
            e.SAP_EGI_NOTIFICATION = this.SapEgiNotification;
            e.SAP_EQUIPMENT_ID = this.SapEquipmentID;
            e.SAP_QUEUE_NUMBER = this.SapQueueNumber;
            e.DATECREATED = Utility.ParseDateTime(this.DateCreated);
            e.CREATEDBY = this.CreatedBy;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.MODIFIEDBY = this.ModifiedBy;
            e.POWER_SOURCE = this.PowerSource;
            e.PROJECT_NAME = this.ProjectName;
            e.MANUFACTURER = this.Manufacturer;
            e.MODEL = this.Model;
            e.INVERTER_EFFICIENCY = this.InverterEfficiency;
            e.NAMEPLATE_RATING = this.NameplateRating;
            e.QUANTITY = this.Quantity;
            e.POWER_FACTOR = this.PowerFactor;
            e.EFF_RATING_KW = this.EffectiveRatingkW;
            e.EFF_RATING_KVA = this.EffectiveRatingkVa;
            e.RATED_VOLTAGE = this.RatedVolatge;
            e.NUMBER_OF_PHASES = this.NumberOfPhases;
            e.MODE_OF_INVERTER = this.ModeOfInverter;
            e.GEN_TECH_CD = this.GenTechCd;
            e.PTO_DATE = Utility.ParseDateTime(this.PTODate);
            e.PROGRAM_TYPE = this.ProgramType;
            e.CONTROL_CD = this.ControlCD;
            e.CONNECTION_CD = this.ConnectionCD;
            e.STATUS_CD = this.StatusCD;
            e.SS_REACTANCE = this.SSReactance;
            e.SS_RESISTANCE = this.SSResistance;
            e.TRANS_REACTANCE = this.TransReactance;
            e.TRANS_RESISTNACE = this.TransResistance;
            e.SUBTRANS_REACTANCE = this.SubTransReactance;
            e.SUBTRANS_RESISTANCE = this.SubTransResistance;
            e.NEG_REACTANCE = this.NegativeReactance;
            e.NEG_RESISTANCE = this.NegativeResistance;
            e.ZERO_REACTANCE = this.ZeroReactance;
            e.ZERO_RESISTANCE = this.ZeroResistance;
            e.GRD_REACTANCE = this.GroundReactance;
            e.GRD_RESISTANCE = this.GroundResistance;
            e.NOTES = this.Notes;
            e.CERTIFICATION = this.Certification;
            e.GEN_TECH_EQUIPMENT = this.GenTechEquipment;
            e.TECH_TYPE_CD = this.TechType;
            e.CURRENT_FUTURE = !string.IsNullOrEmpty(this.CurrentOrFuture) ? this.CurrentOrFuture : CurrentOrFuture_C ;
            e.BACKUP_GEN = this.BackupGeneration ? "Y" : "N";

        }

        public void PopulateModelFromEntity(SM_GENERATOR e)
        {
            this.ID = e.ID;
            this.ProtectionId = e.PROTECTION_ID;
            this.SapEgiNotification= e.SAP_EGI_NOTIFICATION ;
            this.SapEquipmentID = e.SAP_EQUIPMENT_ID;
            this.SapQueueNumber= e.SAP_QUEUE_NUMBER ;
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy= e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy= e.MODIFIEDBY;
            this.PowerSource = e.POWER_SOURCE;
            this.ProjectName = e.PROJECT_NAME;
            this.Manufacturer= e.MANUFACTURER;
            this.Model= e.MODEL;
            this.InverterEfficiency = e.INVERTER_EFFICIENCY;
            this.NameplateRating = e.NAMEPLATE_RATING;
            this.Quantity = e.QUANTITY.HasValue ? Convert.ToInt32(e.QUANTITY) : e.QUANTITY;
            this.NameplateCapacity = (e.NAMEPLATE_RATING.HasValue ? e.NAMEPLATE_RATING : 0) * (e.QUANTITY.HasValue ? Convert.ToInt32(e.QUANTITY) : 0);
            this.PowerFactor = e.POWER_FACTOR;
            this.EffectiveRatingkW = e.EFF_RATING_KW;
            this.EffectiveRatingkVa = e.EFF_RATING_KVA;
            this.RatedVolatge = e.RATED_VOLTAGE;
            this.NumberOfPhases = e.NUMBER_OF_PHASES.HasValue ? Convert.ToInt32(e.NUMBER_OF_PHASES):e.NUMBER_OF_PHASES;
            this.ModeOfInverter = e.MODE_OF_INVERTER;
            this.GenTechCd = e.GEN_TECH_CD;
            this.PTODate = e.PTO_DATE.HasValue ? e.PTO_DATE.Value.ToShortDateString():string.Empty;
            this.ProgramType = e.PROGRAM_TYPE;
            this.ControlCD = e.CONTROL_CD;
            this.ConnectionCD = e.CONNECTION_CD;
            this.StatusCD = e.STATUS_CD;
            this.SSReactance= e.SS_REACTANCE;
            this.SSResistance= e.SS_RESISTANCE;
            this.TransReactance = e.TRANS_REACTANCE;
            this.TransResistance = e.TRANS_RESISTNACE;
            this.SubTransReactance = e.SUBTRANS_REACTANCE;
            this.SubTransResistance = e.SUBTRANS_RESISTANCE;
            this.NegativeReactance = e.NEG_REACTANCE;
            this.NegativeResistance = e.NEG_RESISTANCE;
            this.ZeroReactance = e.ZERO_REACTANCE;
            this.ZeroResistance = e.ZERO_RESISTANCE;
            this.GroundReactance = e.GRD_REACTANCE;
            this.GroundResistance = e.GRD_RESISTANCE;
            this.Notes = e.NOTES;
            if (e.CERTIFICATION == null) {
                e.CERTIFICATION = "";
            }
            this.Certification = e.CERTIFICATION;
            this.GenTechEquipment = e.GEN_TECH_EQUIPMENT;
            this.TechType = e.TECH_TYPE_CD;
            this.CurrentOrFuture = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
            this.BackupGeneration = !string.IsNullOrEmpty(e.BACKUP_GEN) && e.BACKUP_GEN == "Y" ? true : false;

        }

        public void PopulateHistoryFromEntity(SM_GENERATOR_HIST entityHistory, SM_GENERATOR e)
        {
            entityHistory.SM_GENERATOR_ID = e.ID;
            entityHistory.PROTECTION_ID = e.PROTECTION_ID;
            entityHistory.SAP_EGI_NOTIFICATION = e.SAP_EGI_NOTIFICATION;
            entityHistory.SAP_EQUIPMENT_ID = e.SAP_EQUIPMENT_ID;
            entityHistory.SAP_QUEUE_NUMBER = e.SAP_QUEUE_NUMBER;
            entityHistory.DATECREATED = e.DATECREATED;
            entityHistory.CREATEDBY = e.CREATEDBY;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.MODIFIEDBY = e.MODIFIEDBY;
            entityHistory.POWER_SOURCE = e.POWER_SOURCE;
            entityHistory.PROJECT_NAME = e.PROJECT_NAME;
            entityHistory.MANUFACTURER = e.MANUFACTURER;
            entityHistory.MODEL = e.MODEL;
            entityHistory.INVERTER_EFFICIENCY = e.INVERTER_EFFICIENCY;
            entityHistory.NAMEPLATE_RATING = e.NAMEPLATE_RATING;
            entityHistory.QUANTITY = e.QUANTITY;
            entityHistory.POWER_FACTOR = e.POWER_FACTOR;
            entityHistory.EFF_RATING_KW = e.EFF_RATING_KW;
            entityHistory.EFF_RATING_KVA = e.EFF_RATING_KVA;
            entityHistory.RATED_VOLTAGE = e.RATED_VOLTAGE;
            entityHistory.NUMBER_OF_PHASES = e.NUMBER_OF_PHASES;
            entityHistory.MODE_OF_INVERTER = e.MODE_OF_INVERTER;
            entityHistory.GEN_TECH_CD = e.GEN_TECH_CD;
            entityHistory.PTO_DATE = e.PTO_DATE;
            entityHistory.PROGRAM_TYPE = e.PROGRAM_TYPE;
            entityHistory.CONTROL_CD = e.CONTROL_CD;
            entityHistory.CONNECTION_CD = e.CONNECTION_CD;
            entityHistory.STATUS_CD = e.STATUS_CD;
            entityHistory.SS_REACTANCE = e.SS_REACTANCE;
            entityHistory.SS_RESISTANCE = e.SS_RESISTANCE;
            entityHistory.TRANS_REACTANCE = e.TRANS_REACTANCE;
            entityHistory.TRANS_RESISTNACE = e.TRANS_RESISTNACE;
            entityHistory.SUBTRANS_REACTANCE = e.SUBTRANS_REACTANCE;
            entityHistory.SUBTRANS_RESISTANCE = e.SUBTRANS_RESISTANCE;
            entityHistory.NEG_REACTANCE = e.NEG_REACTANCE;
            entityHistory.NEG_RESISTANCE = e.NEG_RESISTANCE;
            entityHistory.ZERO_REACTANCE = e.ZERO_REACTANCE;
            entityHistory.ZERO_RESISTANCE = e.ZERO_RESISTANCE;
            entityHistory.GRD_REACTANCE = e.GRD_REACTANCE;
            entityHistory.GRD_RESISTANCE = e.GRD_RESISTANCE;
            entityHistory.NOTES = e.NOTES;
            if (e.CERTIFICATION == null)
            {
                e.CERTIFICATION = "";
            }

            entityHistory.CERTIFICATION = e.CERTIFICATION;
            entityHistory.GEN_TECH_EQUIPMENT = e.GEN_TECH_EQUIPMENT;
            entityHistory.TECH_TYPE_CD = e.TECH_TYPE_CD;
            entityHistory.CURRENT_FUTURE = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
            entityHistory.BACKUP_GEN = e.BACKUP_GEN;

        }

        public void PopulateModelFromHistoryEntity(SM_GENERATOR_HIST e)
        {
            this.ID = (long)e.SM_GENERATOR_ID;
            this.ProtectionId = e.PROTECTION_ID;
            this.SapEgiNotification = e.SAP_EGI_NOTIFICATION;
            this.SapEquipmentID = e.SAP_EQUIPMENT_ID;
            this.SapQueueNumber = e.SAP_QUEUE_NUMBER;
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy = e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy = e.MODIFIEDBY;
            this.PowerSource = e.POWER_SOURCE;
            this.ProjectName = e.PROJECT_NAME;
            this.Manufacturer = e.MANUFACTURER;
            this.Model = e.MODEL;
            this.InverterEfficiency = e.INVERTER_EFFICIENCY;
            this.NameplateRating = e.NAMEPLATE_RATING;
            this.Quantity = e.QUANTITY.HasValue ? Convert.ToInt32(e.QUANTITY) : 0;
            this.NameplateCapacity = (e.NAMEPLATE_RATING.HasValue ? e.NAMEPLATE_RATING : 0) * (e.QUANTITY.HasValue ? Convert.ToInt32(e.QUANTITY) : 0);
            this.PowerFactor = e.POWER_FACTOR;
            this.EffectiveRatingkW = e.EFF_RATING_KW;
            this.EffectiveRatingkVa = e.EFF_RATING_KVA;
            this.RatedVolatge = e.RATED_VOLTAGE;
            this.NumberOfPhases = e.NUMBER_OF_PHASES.HasValue ? Convert.ToInt32(e.NUMBER_OF_PHASES) : 0;
            this.ModeOfInverter = e.MODE_OF_INVERTER;
            this.GenTechCd = e.GEN_TECH_CD;
            this.PTODate = e.PTO_DATE.HasValue ? e.PTO_DATE.Value.ToShortDateString() : string.Empty;
            this.ProgramType = e.PROGRAM_TYPE;
            this.ControlCD = e.CONTROL_CD;
            this.ConnectionCD = e.CONNECTION_CD;
            this.StatusCD = e.STATUS_CD;
            this.SSReactance = e.SS_REACTANCE;
            this.SSResistance = e.SS_RESISTANCE;
            this.TransReactance = e.TRANS_REACTANCE;
            this.TransResistance = e.TRANS_RESISTNACE;
            this.SubTransReactance = e.SUBTRANS_REACTANCE;
            this.SubTransResistance = e.SUBTRANS_RESISTANCE;
            this.NegativeReactance = e.NEG_REACTANCE;
            this.NegativeResistance = e.NEG_RESISTANCE;
            this.ZeroReactance = e.ZERO_REACTANCE;
            this.ZeroResistance = e.ZERO_RESISTANCE;
            this.GroundReactance = e.GRD_REACTANCE;
            this.GroundResistance = e.GRD_RESISTANCE;
            this.Notes = e.NOTES;
            if (e.CERTIFICATION == null)
            {
                e.CERTIFICATION = "";
            }

            this.Certification = e.CERTIFICATION;
            this.GenTechEquipment = e.GEN_TECH_EQUIPMENT;
            this.TechType = e.TECH_TYPE_CD;
            this.CurrentOrFuture = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
            this.BackupGeneration = !string.IsNullOrEmpty(e.BACKUP_GEN) && e.BACKUP_GEN== "Y" ? true : false;
        }

    }

    public static class IntExtensions
    {
        public static string EmptyIfZero(this int value)
        {
            if (value == 0)
                return string.Empty;

            return value.ToString();
        }
    }
}