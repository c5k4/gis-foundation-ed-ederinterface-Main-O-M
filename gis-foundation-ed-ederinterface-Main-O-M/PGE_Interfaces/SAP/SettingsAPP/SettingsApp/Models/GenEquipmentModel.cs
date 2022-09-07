using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SettingsApp.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


//ENOS2EDGIS
namespace SettingsApp.Models
{
    public class GenEquipmentModel
    {
        public SelectList GenTechCodeList { get; set; }
        

        [SettingsValidatorAttribute("GENEQUIPMENT", "ID")]
        [Display(Name = "ID :")]
        public long ID { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "GENERATOR_ID")]
        [Display(Name = "Generator ID :")]
        public long GeneratorID { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "SAP_EQUIPMENT_ID")]
        [Display(Name = "SAP Equipment ID :")]
        [Required(ErrorMessage = "SAP_EQUIPMENT_ID should not be empty")]
        //[RegularExpression(@"^[a-zA-Z0-9'' ']+$", ErrorMessage = "Special character or -1 should not be entered")]

        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Value should be number ")]
        [MaxLength(18)]
        //[Remote("IsEquipmentIDAvailble", "Protection", ErrorMessage = "Entered SAP Equipment ID already Exist.")]
        public System.String SAPEquipmentID { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "DATECREATED")]
        [Display(Name = "Date Created :")]
        public System.String DateCreated { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "CREATEDBY")]
        [Display(Name = "Created By :")]
        public System.String CreatedBy { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "DATEMODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "MODIFIEDBY")]
        [Display(Name = "Modified By :")]
        public System.String ModifiedBy { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "GEN_TECH_CD")]
        [Display(Name = "Equipment Type:")]
        public System.String GenTechCD { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "MANUFACTURER")]
        [Display(Name = "Equipment Manufacturer:")]
        [MaxLength(100, ErrorMessage="Should not exceed more than 100 charectors")]
        public System.String Manufacturer { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "MODEL")]
        [Display(Name = "Equipment Model:")]
        [MaxLength(100, ErrorMessage = "Should not exceed more than 100 charectors")]
        public System.String Model { get; set; }

        //[SettingsValidatorAttribute("GENEQUIPMENT", "PTC_RATED_KW")]
        [Display(Name = "PTC Rated kW:")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example : 3453455.456, 5.789")]
        public System.Decimal? PTCRatedkW { get; set; }

        //[SettingsValidatorAttribute("GENEQUIPMENT", "NAMEPLATE_RATING")]
        [Display(Name = "Nameplate Rating per Unit (kW-AC):")] // ENOS2EDGIS
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example : 3453455.456, 5.789")]
        public System.Decimal? NameplateRating { get; set; }

        /*********************ENOS2EDGIS Start**********************/
        [Display(Name = "Total Nameplate Rating (kW-AC):")]
        // [RegularExpression(@"^\d+\.\d{0,3}$")]       
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points.For Example : 234324.555, 0.45")]
        public Decimal? NameplateCapacity { get; set; }
        /*********************ENOS2EDGIS End**********************/

        [SettingsValidatorAttribute("GENEQUIPMENT", "QUANTITY")]
        [Display(Name = "Quantity:")]
        [Range(0, 99999, ErrorMessage = "Please enter valid integer number between 0 to 99999")]
        public System.Int32? Quantity { get; set; }

        //[SettingsValidatorAttribute("GENEQUIPMENT", "MAX_STORAGE_CAPACITY")]
        [Display(Name = "Max Storage Capacity:")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example : 3453455.456, 5.789")]
        public System.Decimal? MaxStorageCapacity { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "RATED_DISCHARGE")]
        [Display(Name = "Rated Discharge:")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 9999999.999, ErrorMessage = "Value should be in between 0 - 9999999.999 : Maximum Three decimal points. For Example : 3453455.456, 5.789")]
        public System.Decimal? RatedDischarge { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "CHARGE_DEMAND_KW")]
        [Display(Name = "Charge Demand kW:")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example : 3453455.456, 5.789")]
        public System.Decimal? ChargeDemandkW { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "GRID_CHARGED")]
        [Display(Name = "Grid Charged:")]
        public System.String GridCharged { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "NOTES")]
        [Display(Name = "Notes:")]
        [MaxLength(500, ErrorMessage = "Should not exceed more than 500 charectors")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("GENEQUIPMENT", "CURRENT_FUTURE")]
        [Display(Name = "Current or Future:")]
        public String CurrentOrFuture { get; set; }

        //ENOS2SAP phase 2 Changes Start
        [SettingsValidatorAttribute("GENEQUIPMENT", "PROGRAM_TYPE")]
        [Display(Name = "Program Type:")]
        public System.String ProgramType { get; set; }
        public SelectList ProgramTypeList { get; set; }
        //ENOS2SAP phase 2 Changes End

        public string GenEquipTypeDropDownPostbackScript { get; set; }


        public void PopulateEntityFromModel(SM_GEN_EQUIPMENT e)
        {
            e.ID = this.ID;
            e.GENERATOR_ID = this.GeneratorID;
            e.SAP_EQUIPMENT_ID = this.SAPEquipmentID;
            e.DATECREATED = Utility.ParseDateTime(this.DateCreated);
            e.CREATEDBY = this.CreatedBy;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.MODIFIEDBY = this.ModifiedBy;
            e.GEN_TECH_CD = this.GenTechCD;
            e.MANUFACTURER = this.Manufacturer;
            e.MODEL = this.Model;
            e.PTC_RATED_KW = this.PTCRatedkW;
            e.NAMEPLATE_RATING = this.NameplateRating;
            e.QUANTITY = this.Quantity;
            e.MAX_STORAGE_CAPACITY = this.MaxStorageCapacity;
            e.RATED_DISCHARGE = this.RatedDischarge;
            e.CHARGE_DEMAND_KW = this.ChargeDemandkW;
            e.GRID_CHARGED = this.GridCharged;
            e.NOTES = this.Notes;
            e.CURRENT_FUTURE = this.CurrentOrFuture;
            e.PROGRAM_TYPE = this.ProgramType;
        }

        public void PopulateModelFromEntity(SM_GEN_EQUIPMENT e)
        {
            this.ID = e.ID;
            this.GeneratorID = e.GENERATOR_ID;
            this.SAPEquipmentID = e.SAP_EQUIPMENT_ID;
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy = e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy = e.MODIFIEDBY;
            this.GenTechCD = e.GEN_TECH_CD;
            this.Manufacturer = e.MANUFACTURER;
            this.Model = e.MODEL;
            this.PTCRatedkW = e.PTC_RATED_KW;
            this.NameplateRating = e.NAMEPLATE_RATING;
            /*********************ENOS2EDGIS Start**********************/
            this.NameplateCapacity = (e.NAMEPLATE_RATING.HasValue ? e.NAMEPLATE_RATING : 0) * (e.QUANTITY.HasValue ? Convert.ToInt32(e.QUANTITY) : 0);
            /*********************ENOS2EDGIS End**********************/
            this.Quantity = e.QUANTITY;
            this.MaxStorageCapacity = e.MAX_STORAGE_CAPACITY;
            this.RatedDischarge = e.RATED_DISCHARGE;
            this.ChargeDemandkW = e.CHARGE_DEMAND_KW;
            this.GridCharged = e.GRID_CHARGED;
            this.Notes = e.NOTES;
            this.CurrentOrFuture = e.CURRENT_FUTURE;
            this.ProgramType = e.PROGRAM_TYPE;
        }

        public void PopulateHistoryFromEntity(SM_GEN_EQUIPMENT_HIST entityHistory, SM_GEN_EQUIPMENT e)
        {
            entityHistory.SM_GEN_EQUIPMENT_ID = e.ID;
            entityHistory.GENERATOR_ID = e.GENERATOR_ID;
            entityHistory.SAP_EQUIPMENT_ID = e.SAP_EQUIPMENT_ID;
            entityHistory.DATECREATED = e.DATECREATED;
            entityHistory.CREATEDBY = e.CREATEDBY;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.MODIFIEDBY = e.MODIFIEDBY;
            entityHistory.GEN_TECH_CD = e.GEN_TECH_CD;
            entityHistory.MANUFACTURER = e.MANUFACTURER;
            entityHistory.MODEL = e.MODEL;
            entityHistory.PTC_RATED_KW = e.PTC_RATED_KW;
            entityHistory.NAMEPLATE_RATING = e.NAMEPLATE_RATING;
            entityHistory.QUANTITY = e.QUANTITY;
            entityHistory.MAX_STORAGE_CAPACITY = e.MAX_STORAGE_CAPACITY;
            entityHistory.RATED_DISCHARGE = e.RATED_DISCHARGE;
            entityHistory.CHARGE_DEMAND_KW = e.CHARGE_DEMAND_KW;
            entityHistory.GRID_CHARGED = e.GRID_CHARGED;
            entityHistory.NOTES = e.NOTES;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
            entityHistory.PROGRAM_TYPE = e.PROGRAM_TYPE;
        }

        public void PopulateModelFromHistoryEntity(SM_GEN_EQUIPMENT_HIST e)
        {
            this.ID = (long)e.SM_GEN_EQUIPMENT_ID;
            this.GeneratorID = e.GENERATOR_ID;
            this.SAPEquipmentID = e.SAP_EQUIPMENT_ID;
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy = e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy = e.MODIFIEDBY;
            this.GenTechCD = e.GEN_TECH_CD;
            this.Manufacturer = e.MANUFACTURER;
            this.Model = e.MODEL;
            this.PTCRatedkW = e.PTC_RATED_KW;
            this.NameplateRating = e.NAMEPLATE_RATING;
            this.Quantity = e.QUANTITY;
            this.MaxStorageCapacity = e.MAX_STORAGE_CAPACITY;
            this.RatedDischarge = e.RATED_DISCHARGE;
            this.ChargeDemandkW = e.CHARGE_DEMAND_KW;
            this.GridCharged = e.GRID_CHARGED;
            this.Notes = e.NOTES;
            this.CurrentOrFuture = e.CURRENT_FUTURE;
            this.ProgramType = e.PROGRAM_TYPE;
        }
    }
}