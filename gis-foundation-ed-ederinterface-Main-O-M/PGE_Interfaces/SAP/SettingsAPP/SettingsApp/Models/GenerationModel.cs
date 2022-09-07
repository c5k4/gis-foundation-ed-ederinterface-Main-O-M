﻿using System;
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
    public class GenerationModel
    {
        [SettingsValidatorAttribute("GENERATION", "ID")]
        [Display(Name = "ID:")]
        public long ID { get; set; }

        [SettingsValidatorAttribute("GENERATION", "GLOBAL_ID")]
        [Display(Name = "Global ID:")]
        public System.String GlobalID { get; set; }

        [SettingsValidatorAttribute("GENERATION", "DATECREATED")]
        [Display(Name = "Date Created:")]
        public System.String DateCreated { get; set; }

        [SettingsValidatorAttribute("GENERATION", "CREATEDBY")]
        [Display(Name = "Created By:")]
        public System.String CreatedBy { get; set; }

        [SettingsValidatorAttribute("GENERATION", "DATEMODIFIED")]
        [Display(Name = "Date modified:")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("GENERATION", "MODIFIEDBY")]
        [Display(Name = "Modified By:")]
        public System.String ModifiedBy { get; set; }

        [SettingsValidatorAttribute("GENERATION", "MODIFIEDBY")]
        [Display(Name = "SAP EGI Notification:")]
        [MaxLength(12)]
        public System.String SAPEGINotification { get; set; }

        [Display(Name = "Project Name:")]
        [MaxLength(40)]
        public System.String ProjectName { get; set; }

        [SettingsValidatorAttribute("GENERATION", "GEN_TYPE")]
        [Display(Name = "Gen Type:")]
        [MaxLength(20)]
        public System.String GenType { get; set; }

        [SettingsValidatorAttribute("GENERATION", "PROGRAM_TYPE")]
        [Display(Name = "Program Type:")]
        public System.String ProgramType { get; set; }

        [SettingsValidatorAttribute("GENERATION", "POWER_SOURCE")]
        [Display(Name = "Power Source:")]
        [MaxLength(40)]
        public System.String PowerSource { get; set; }

        [Display(Name = "Effective Rating Machine kW:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example 1423345.750, 0.93, 345")]
        public System.Decimal? EffRatingMachkW { get; set; }

        /******************************ENOS2SAP PhaseIII Start**************************************/
        [NotMapped]
        [Display(Name = "Total System Nameplate kW:")]
        public System.String NameplateRating { get; set; }
        /******************************ENOS2SAP PhaseIII End**************************************/

        [Display(Name = "Effective Rating Inverter kW:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example 14233459.559, 0878.93, 7878")]
        public System.Decimal? EffRatingInvkW { get; set; }

        [Display(Name = "Effective Rating Machine kVA:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example 1423345.759, 0.93, 345")]
        public System.Decimal? EffRatingMachkVA { get; set; }

        [Display(Name = "Effective Rating Inverter kVA:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example 1423345.759, 0.93, 345")]
        public System.Decimal? EffRatingInvkVA { get; set; }

        [SettingsValidatorAttribute("GENERATION", "BACKUP_GENERATION")]
        [Display(Name = "Backup Generation:")]
        public System.Boolean BackupGeneration { get; set; }

        [Display(Name = "Export kW:")]
        //[RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example 1423345.759, 0.93, 345")]
        public System.Decimal? ExportkW { get; set; }

        [Display(Name = "Max Storage Capacity (kWh):")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example 1423345.756, 0.933, 345.00")]
        public System.Decimal? MaxStorageCapacity  { get; set; }

        [Display(Name = "Charge Demand kW:")]
        //[RegularExpression(@"^\d+\.\d{0,3}$")]
        [Range(0, 9999999999.999, ErrorMessage = "Value should be in between 0 - 9999999999.999 : Maximum Three decimal points. For Example 1423345.756, 0.933, 345.00")]
        public System.Decimal? ChargeDemandkW { get; set; }

        [SettingsValidatorAttribute("GENERATION", "DIRECT_TRANSFER_TRIP")]
        [Display(Name = "Direct Transfer Trip:")]
        public Boolean DirectTransferTrip { get; set; }

        [SettingsValidatorAttribute("GENERATION", "GRD_FAULT_DETECTION_CD")]
        [Display(Name = "Ground Fault Detection:")]
        public Boolean GrdFaultDetectionCd { get; set; }

        [SettingsValidatorAttribute("GENERATION", "NOTES")]
        [Display(Name = "Notes:")]
        [MaxLength(2000, ErrorMessage = "Should not exceed more than 2000 charectors")]
        public System.String Notes { get; set; }

        [MaxLength(1)]
        [SettingsValidatorAttribute("GENERATION", "CURRENT_FUTURE")]
        [Display(Name = "Current or Future:")]

       

        public String CurrentOrFuture { get; set; }

        public List<ProtectionViewModel> ListOfProtection { get; set; }

        public SelectList ProgramTypeList { get; set; }
        public SelectList TechnologyTypeList { get; set; }

        private string CurrentOrFuture_C = ConfigurationManager.AppSettings["CurrentOrFuture_C"];
        private string CurrentOrFuture_F = ConfigurationManager.AppSettings["CurrentOrFuture_F"];

        public void PopulateEntityFromModel(SM_GENERATION e)
        {
            e.ID = this.ID;
            e.GLOBAL_ID = this.GlobalID;
            e.DATECREATED = Utility.ParseDateTime(this.DateCreated);
            e.CREATEDBY = this.CreatedBy;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.MODIFIEDBY = this.ModifiedBy;
            e.SAP_EGI_NOTIFICATION = this.SAPEGINotification;
            e.PROJECT_NAME = this.ProjectName;
            e.GEN_TYPE = this.GenType;
            e.PROGRAM_TYPE = this.ProgramType;
            e.POWER_SOURCE = this.PowerSource;
            e.EFF_RATING_MACH_KW = this.EffRatingMachkW;
            e.EFF_RATING_INV_KW = this.EffRatingInvkW;
            e.EFF_RATING_MACH_KVA = this.EffRatingMachkVA;
            e.EFF_RATING_INV_KVA = this.EffRatingInvkVA;
            e.BACKUP_GENERATION = this.BackupGeneration ? "Y":"N";
            e.EXPORT_KW = this.ExportkW;
            e.MAX_STORAGE_CAPACITY = this.MaxStorageCapacity;
            e.CHARGE_DEMAND_KW = this.ChargeDemandkW;
            e.DIRECT_TRANSFER_TRIP = this.DirectTransferTrip ? "Y":"N";
            e.GRD_FAULT_DETECTION_CD = this.GrdFaultDetectionCd ? "Y" : "N";
            e.NOTES = this.Notes;
            e.CURRENT_FUTURE = !string.IsNullOrEmpty(this.CurrentOrFuture) ? this.CurrentOrFuture : CurrentOrFuture_C;
        }

        public void PopulateModelFromEntity(SM_GENERATION e)
        {
            this.ID = e.ID;
            this.GlobalID = e.GLOBAL_ID;
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy = e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy = e.MODIFIEDBY;
            this.SAPEGINotification = e.SAP_EGI_NOTIFICATION;
            this.ProjectName = e.PROJECT_NAME;
            this.GenType = e.GEN_TYPE;
            this.ProgramType = e.PROGRAM_TYPE;
            this.PowerSource = e.POWER_SOURCE;
            this.EffRatingMachkW = e.EFF_RATING_MACH_KW;
            this.EffRatingInvkW = e.EFF_RATING_INV_KW;
            this.EffRatingMachkVA = e.EFF_RATING_MACH_KVA;
            this.EffRatingInvkVA = e.EFF_RATING_INV_KVA;
            this.BackupGeneration = !string.IsNullOrEmpty(e.BACKUP_GENERATION) && e.BACKUP_GENERATION == "Y" ? true : false;
            this.ExportkW = e.EXPORT_KW;
            this.MaxStorageCapacity = e.MAX_STORAGE_CAPACITY;
            this.ChargeDemandkW = e.CHARGE_DEMAND_KW;
            this.DirectTransferTrip = !string.IsNullOrEmpty(e.DIRECT_TRANSFER_TRIP) && e.DIRECT_TRANSFER_TRIP == "Y" ? true : false ;
            this.GrdFaultDetectionCd = !string.IsNullOrEmpty(e.GRD_FAULT_DETECTION_CD) && e.GRD_FAULT_DETECTION_CD == "Y" ? true : false;
            this.Notes = e.NOTES;
            this.CurrentOrFuture = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
        }

        public void PopulateHistoryFromEntity(SM_GENERATION_HIST entityHistory, SM_GENERATION e)
        {
            entityHistory.GLOBAL_ID = e.GLOBAL_ID;
            entityHistory.DATECREATED = e.DATECREATED;
            entityHistory.CREATEDBY = e.CREATEDBY;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.MODIFIEDBY = e.MODIFIEDBY;
            entityHistory.SAP_EGI_NOTIFICATION = e.SAP_EGI_NOTIFICATION;
            entityHistory.PROJECT_NAME = e.PROJECT_NAME;
            entityHistory.GEN_TYPE = e.GEN_TYPE;
            entityHistory.PROGRAM_TYPE = e.PROGRAM_TYPE;
            entityHistory.POWER_SOURCE = e.POWER_SOURCE;
            entityHistory.EFF_RATING_MACH_KW = e.EFF_RATING_MACH_KW;
            entityHistory.EFF_RATING_INV_KW = e.EFF_RATING_INV_KW;
            entityHistory.EFF_RATING_MACH_KVA = e.EFF_RATING_MACH_KVA;
            entityHistory.EFF_RATING_INV_KVA = e.EFF_RATING_INV_KVA;
            entityHistory.BACKUP_GENERATION = e.BACKUP_GENERATION;
            entityHistory.EXPORT_KW = e.EXPORT_KW;
            entityHistory.MAX_STORAGE_CAPACITY = e.MAX_STORAGE_CAPACITY;
            entityHistory.CHARGE_DEMAND_KW = e.CHARGE_DEMAND_KW;
            entityHistory.DIRECT_TRANSFER_TRIP = e.DIRECT_TRANSFER_TRIP;
            entityHistory.GRD_FAULT_DETECTION_CD = e.GRD_FAULT_DETECTION_CD;
            entityHistory.NOTES = e.NOTES;
            entityHistory.CURRENT_FUTURE = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
            entityHistory.SM_GENERATION_ID = e.ID;
        }

        public void PopulateModelFromHistoryEntity(SM_GENERATION_HIST e)
        {
            this.ID = e.ID;
            this.DateCreated = e.DATECREATED.HasValue ? e.DATECREATED.Value.ToShortDateString() : String.Empty;
            this.CreatedBy = e.CREATEDBY;
            this.DateModified = e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : String.Empty;
            this.ModifiedBy = e.MODIFIEDBY;
            this.SAPEGINotification = e.SAP_EGI_NOTIFICATION;
            this.ProjectName = e.PROJECT_NAME;
            this.GenType = e.GEN_TYPE;
            this.ProgramType = e.PROGRAM_TYPE;
            this.PowerSource = e.POWER_SOURCE;
            this.EffRatingMachkW = e.EFF_RATING_MACH_KW;
            this.EffRatingInvkW = e.EFF_RATING_INV_KW;
            this.EffRatingMachkVA = e.EFF_RATING_MACH_KVA;
            this.EffRatingInvkVA = e.EFF_RATING_INV_KVA;
            this.BackupGeneration = !string.IsNullOrEmpty(e.BACKUP_GENERATION) && e.BACKUP_GENERATION == "Y" ? true : false;
            this.ExportkW = e.EXPORT_KW;
            this.MaxStorageCapacity = e.MAX_STORAGE_CAPACITY;
            this.ChargeDemandkW = e.CHARGE_DEMAND_KW;
            this.DirectTransferTrip = !string.IsNullOrEmpty(e.DIRECT_TRANSFER_TRIP) && e.DIRECT_TRANSFER_TRIP == "Y" ? true : false ;;
            this.GrdFaultDetectionCd = !string.IsNullOrEmpty(e.GRD_FAULT_DETECTION_CD) && e.GRD_FAULT_DETECTION_CD == "Y" ? true : false;
            this.Notes = e.NOTES;
            this.CurrentOrFuture = !string.IsNullOrEmpty(e.CURRENT_FUTURE) ? e.CURRENT_FUTURE : CurrentOrFuture_C;
        }
    }
}