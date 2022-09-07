using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SettingsApp.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class PrimaryMeterModel
    {
        [SettingsValidatorAttribute("PRIMARY_METER", "ID")]
        [Display(Name = "ID :")]
        public Decimal ID { get; set; }

        [SettingsValidatorAttribute("PRIMARY_METER", "GLOBAL_ID")]
        [Display(Name = "GlobalId :")]
        public System.String GlobalId { get; set; }

        [SettingsValidatorAttribute("PRIMARY_METER", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("PRIMARY_METER", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("PRIMARY_METER", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }


        [SettingsValidatorAttribute("PRIMARY_METER", "PEER_REVIEW_DT")]
        [Display(Name = "Review Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("PRIMARY_METER", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("PRIMARY_METER", "NOTES")]
        [Display(Name = "Notes :")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("PRIMARY_METER", "DIRECT_TRANSFER_TRIP")]
        [Display(Name = "Direct Transfer Trip :")]
        public bool DirectTransferTrip { get; set; }


        [SettingsValidatorAttribute("GENERATOR", "CURRENT_FUTURE")]
        [Display(Name = "Current or Future:")]
        public String CurrentOrFuture { get; set; }


        public bool Release { get; set; }
        public List<GISAttributes> GISAttributes { get; set; }


        [HiddenInput]
        public RelayModel ActivePrimaryPhaseRelay { get; set; }
        [HiddenInput]
        public RelayModel ActivePrimaryGroundRelay { get; set; }
        [HiddenInput]
        public RelayModel ActiveBackupPhaseRelay { get; set; }
        [HiddenInput]
        public RelayModel ActiveBackupGroundRelay { get; set; }

        [HiddenInput]
        public ProtectionModel ActiveProtection { get; set; }

        public PrimaryMeterModel ActivePrimaryMeter { get; set; }

        [HiddenInput]
        public List<RelayModel> Relays { get; set; }

        public void PopulateEntityFromModel(SM_PRIMARY_METER e)
        {
            //e.ID = this.ID;
            e.GLOBAL_ID = this.GlobalId;
            e.OPERATING_NUM = this.OperatingNumber;
            e.PREPARED_BY = this.PreparedBy;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            e.NOTES = this.Notes;
            e.DIRECT_TRANSFER_TRIP = this.DirectTransferTrip ? "Y" : "N";
            e.CURRENT_FUTURE = this.CurrentOrFuture;
        }

        public void PopulateModelFromEntity(SM_PRIMARY_METER e)
        {
            this.ID = e.ID;
            this.GlobalId = e.GLOBAL_ID;
            this.OperatingNumber = e.OPERATING_NUM;
            this.PreparedBy = e.PREPARED_BY;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.Notes = e.NOTES;
            this.DirectTransferTrip = !string.IsNullOrEmpty(e.DIRECT_TRANSFER_TRIP) && e.DIRECT_TRANSFER_TRIP == "Y" ? true : false;
            this.CurrentOrFuture = e.CURRENT_FUTURE;
        }

        public void PopulateHistoryFromEntity(SM_PRIMARY_METER_HIST entityHistory, SM_PRIMARY_METER e)
        {
            entityHistory.GLOBAL_ID = e.GLOBAL_ID;
            entityHistory.FEATURE_CLASS_NAME = e.FEATURE_CLASS_NAME;
            entityHistory.OPERATING_NUM = e.OPERATING_NUM;
            entityHistory.PREPARED_BY = e.PREPARED_BY;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.PEER_REVIEW_DT = e.PEER_REVIEW_DT;
            entityHistory.PEER_REVIEW_BY = e.PEER_REVIEW_BY;
            entityHistory.NOTES = e.NOTES;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
            entityHistory.DIRECT_TRANSFER_TRIP = e.DIRECT_TRANSFER_TRIP;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
        }

        public void PopulateModelFromHistoryEntity(SM_PRIMARY_METER_HIST e)
        {
            this.GlobalId = e.GLOBAL_ID;
            this.OperatingNumber = e.OPERATING_NUM;
            this.PreparedBy = e.PREPARED_BY;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.Notes = e.NOTES;
            this.DirectTransferTrip = !string.IsNullOrEmpty(e.DIRECT_TRANSFER_TRIP) && e.DIRECT_TRANSFER_TRIP == "Y" ? true : false;
            this.CurrentOrFuture = e.CURRENT_FUTURE;
        }
        /*
        public void PopulateHistoryFromEntity(SM_PRIMARY_METER_HIST entityHistory, SM_PRIMARY_METER e)
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
            entityHistory.NOTES = e.NOTES;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
            entityHistory.PROCESSED_FLAG = e.PROCESSED_FLAG;
            entityHistory.RELEASED_BY = e.RELEASED_BY;
            entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
            entityHistory.GRD_PR_RELAY_TYPE = e.GRD_PR_RELAY_TYPE;
            entityHistory.GRD_PR_MIN_TRIP = e.GRD_PR_MIN_TRIP;
            entityHistory.GRD_PR_INS_TRIP = e.GRD_PR_INS_TRIP;
            entityHistory.GRD_PR_LEVER_SET = e.GRD_PR_LEVER_SET;
            entityHistory.GRD_BK_RELAY_TYPE = e.GRD_BK_RELAY_TYPE;
            entityHistory.GRD_BK_MIN_TRIP = e.GRD_BK_MIN_TRIP;
            entityHistory.GRD_BK_INS_TRIP = e.GRD_BK_INS_TRIP;
            entityHistory.GRD_BK_LEVER_SET = e.GRD_BK_LEVER_SET;
            entityHistory.PHA_PR_RELAY_TYPE = e.PHA_PR_RELAY_TYPE;
            entityHistory.PHA_PR_MIN_TRIP = e.PHA_PR_MIN_TRIP;
            entityHistory.PHA_PR_INS_TRIP = e.PHA_PR_INS_TRIP;
            entityHistory.PHA_PR_LEVER_SET = e.PHA_PR_LEVER_SET;
            entityHistory.PHA_BK_RELAY_TYPE = e.PHA_BK_RELAY_TYPE;
            entityHistory.PHA_BK_MIN_TRIP = e.PHA_BK_MIN_TRIP;
            entityHistory.PHA_BK_INS_TRIP = e.PHA_BK_INS_TRIP;
            entityHistory.PHA_BK_LEVER_SET = e.PHA_BK_LEVER_SET;
            entityHistory.MIN_NOR_VOLT = e.MIN_NOR_VOLT;
            entityHistory.ANNUAL_LF = e.ANNUAL_LF;
            entityHistory.NETWORK = e.NETWORK;
            entityHistory.DPA_CD = e.DPA_CD;
            entityHistory.CC_RATING = e.CC_RATING;
            entityHistory.FLISR = e.FLISR;
            entityHistory.SUMMER_LOAD_LIMIT = e.SUMMER_LOAD_LIMIT;
            entityHistory.WINTER_LOAD_LIMIT = e.WINTER_LOAD_LIMIT;
            entityHistory.LIMITING_FACTOR = e.LIMITING_FACTOR;
            entityHistory.OPERATING_MODE = e.OPERATING_MODE;
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
            entityHistory.OPS_TO_LOCKOUT = e.OPS_TO_LOCKOUT;
            entityHistory.FLISR_ENGINEERING_COMMENTS = e.FLISR_ENGINEERING_COMMENTS;
            entityHistory.FLISR_OPERATING_MODE = e.FLISR_OPERATING_MODE;
            entityHistory.ENGINEERING_COMMENTS = e.ENGINEERING_COMMENTS;
            entityHistory.OPS_TO_LOCKOUT = e.OPS_TO_LOCKOUT;
            entityHistory.DIRECT_TRANSFER_TRIP = e.DIRECT_TRANSFER_TRIP;
            entityHistory.RECLOSE_BLOCKING = e.RECLOSE_BLOCKING;
            entityHistory.PHA_PR_CONTROL_SERIAL_NUM = e.PHA_PR_CONTROL_SERIAL_NUM;
            entityHistory.PHA_PR_FIRMWARE_VERSION = e.PHA_PR_FIRMWARE_VERSION;
            entityHistory.PHA_PR_SOFTWARE_VERSION = e.PHA_PR_SOFTWARE_VERSION;
            entityHistory.PHA_BK_CONTROL_SERIAL_NUM = e.PHA_BK_CONTROL_SERIAL_NUM;
            entityHistory.PHA_BK_FIRMWARE_VERSION = e.PHA_BK_FIRMWARE_VERSION;
            entityHistory.PHA_BK_SOFTWARE_VERSION = e.PHA_BK_SOFTWARE_VERSION;
            entityHistory.GRD_PR_CONTROL_SERIAL_NUM = e.GRD_PR_CONTROL_SERIAL_NUM;
            entityHistory.GRD_PR_FIRMWARE_VERSION = e.GRD_PR_FIRMWARE_VERSION;
            entityHistory.GRD_PR_SOFTWARE_VERSION = e.GRD_PR_SOFTWARE_VERSION;
            entityHistory.GRD_BK_CONTROL_SERIAL_NUM = e.GRD_BK_CONTROL_SERIAL_NUM;
            entityHistory.GRD_BK_FIRMWARE_VERSION = e.GRD_BK_FIRMWARE_VERSION;
            entityHistory.GRD_BK_SOFTWARE_VERSION = e.GRD_BK_SOFTWARE_VERSION;
            entityHistory.RTU_EXIST = e.RTU_EXIST;
            entityHistory.RTU_MANF_CD = e.RTU_MANF_CD;
            entityHistory.RTU_FIRMWARE_VERSION = e.RTU_FIRMWARE_VERSION;
            entityHistory.RTU_MODEL_NUM = e.RTU_MODEL_NUM;
            entityHistory.RTU_SERIAL_NUM = e.RTU_SERIAL_NUM;
            entityHistory.RTU_SOFTWARE_VERSION = e.RTU_SOFTWARE_VERSION;
            entityHistory.ENGINEERING_COMMENTS = e.ENGINEERING_COMMENTS;

        }

        public void PopulateModelFromHistoryEntity(SM_PRIMARY_METER e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.Notes = e.NOTES;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.GrdPrRelayType = e.GRD_PR_RELAY_TYPE;
            this.GrdPrMinimumAmpsToTrip = e.GRD_PR_MIN_TRIP;
            this.GrdPrInstantaneousMinimumTrip = e.GRD_PR_INS_TRIP;
            this.GrdPrRelayLeverSetting = e.GRD_PR_LEVER_SET;
            this.GrdBkRelayType = e.GRD_BK_RELAY_TYPE;
            this.GrdBkMinimumAmpsToTrip = e.GRD_BK_MIN_TRIP;
            this.GrdBkInstantaneousMinimumTrip = e.GRD_BK_INS_TRIP;
            this.GrdBkRelayLeverSetting = e.GRD_BK_LEVER_SET;
            this.PhaRrRelayType = e.PHA_PR_RELAY_TYPE;
            this.PhaRrMinimumAmpsToTrip = e.PHA_PR_MIN_TRIP;
            this.PhaRrInstantaneousMinimumTrip = e.PHA_PR_INS_TRIP;
            this.PhaRrRelayLeverSetting = e.PHA_PR_LEVER_SET;
            this.PhaBkRelayType = e.PHA_BK_RELAY_TYPE;
            this.PhaBkMinimumAmpsToTrip = e.PHA_BK_MIN_TRIP;
            this.PhaBkInstantaneousMinimumTrip = e.PHA_BK_INS_TRIP;
            this.PhaBkRelayLeverSetting = e.PHA_BK_LEVER_SET;
            this.MinimumAllowableVoltage = e.MIN_NOR_VOLT;
            this.AnnualLoadFactor = e.ANNUAL_LF;
            this.Network = e.NETWORK;
            this.DistributionPlanningArea = e.DPA_CD;
            this.CcRating = e.CC_RATING;
            this.FlisrAutomationDevice = e.FLISR;
            this.SummerLoadLimitAmps = e.SUMMER_LOAD_LIMIT;
            this.WinterLoadLimitAmps = e.WINTER_LOAD_LIMIT;
            this.LimitingFactor = e.LIMITING_FACTOR;
            this.FLISREngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            this.OperatingMode = e.OPERATING_MODE;
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
            this.OpsToLockout = e.OPS_TO_LOCKOUT;
            this.DirectTransferTrip = e.DIRECT_TRANSFER_TRIP;
            this.RecloseBlocking = e.RECLOSE_BLOCKING;
            this.PhaPrControllerSerialNum = e.PHA_PR_CONTROL_SERIAL_NUM;
            this.PhaPrFirmwareVersion = e.PHA_PR_FIRMWARE_VERSION;
            this.PhaPrSoftwareVersion = e.PHA_PR_SOFTWARE_VERSION;
            this.PhaBkControllerSerialNum = e.PHA_BK_CONTROL_SERIAL_NUM;
            this.PhaBkFirmwareVersion = e.PHA_BK_FIRMWARE_VERSION;
            this.PhaBkSoftwareVersion = e.PHA_BK_SOFTWARE_VERSION;
            this.GrdPrControllerSerialNum = e.GRD_PR_CONTROL_SERIAL_NUM;
            this.GrdPrFirmwareVersion = e.GRD_PR_FIRMWARE_VERSION;
            this.GrdPrSoftwareVersion = e.GRD_PR_SOFTWARE_VERSION;
            this.GrdBkControllerSerialNum = e.GRD_BK_CONTROL_SERIAL_NUM;
            this.GrdBkFirmwareVersion = e.GRD_BK_FIRMWARE_VERSION;
            this.GrdBkSoftwareVersion = e.GRD_BK_SOFTWARE_VERSION;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
        }
         */
    }
}