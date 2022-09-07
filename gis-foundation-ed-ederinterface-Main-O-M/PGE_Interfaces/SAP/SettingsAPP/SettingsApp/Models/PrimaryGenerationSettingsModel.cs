using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SettingsApp.Common;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class PrimaryGenerationSettingsModel
    {
        [SettingsValidatorAttribute("PRIMARY_GEN", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceId { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "NOTES")]
        [Display(Name = "Notes :")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_PR_RELAY_TYPE")]
        [Display(Name = "Relay Type :")]
        public System.String GrdPrRelayType { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_PR_MIN_TRIP")]
        [Display(Name = "Minimum AMPs to trip :")]
        public System.Int16? GrdPrMinimumAmpsToTrip { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_PR_INS_TRIP")]
        [Display(Name = "Instantaneous minimum trip :")]
        public System.Int32? GrdPrInstantaneousMinimumTrip { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_PR_LEVER_SET")]
        [Display(Name = "Relay Lever Setting :")]
        public System.Decimal? GrdPrRelayLeverSetting { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_BK_RELAY_TYPE")]
        [Display(Name = "Relay Type :")]
        public System.String GrdBkRelayType { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_BK_MIN_TRIP")]
        [Display(Name = "Minimum AMPs to trip :")]
        public System.Int16? GrdBkMinimumAmpsToTrip { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_BK_INS_TRIP")]
        [Display(Name = "Instantaneous minimum trip :")]
        public System.Int32? GrdBkInstantaneousMinimumTrip { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_BK_LEVER_SET")]
        [Display(Name = "Relay Lever Setting :")]
        public System.Decimal? GrdBkRelayLeverSetting { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_PR_RELAY_TYPE")]
        [Display(Name = "Relay Type :")]
        public System.String PhaRrRelayType { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_PR_MIN_TRIP")]
        [Display(Name = "Minimum AMPs to trip :")]
        public System.Int16? PhaRrMinimumAmpsToTrip { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_PR_INS_TRIP")]
        [Display(Name = "Instantaneous minimum trip :")]
        public System.Int32? PhaRrInstantaneousMinimumTrip { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_PR_LEVER_SET")]
        [Display(Name = "Relay Lever Setting :")]
        public System.Decimal? PhaRrRelayLeverSetting { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_BK_RELAY_TYPE")]
        [Display(Name = "Relay Type :")]
        public System.String PhaBkRelayType { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_BK_MIN_TRIP")]
        [Display(Name = "Minimum AMPs to trip :")]
        public System.Int16? PhaBkMinimumAmpsToTrip { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_BK_INS_TRIP")]
        [Display(Name = "Instantaneous minimum trip :")]
        public System.Int32? PhaBkInstantaneousMinimumTrip { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_BK_LEVER_SET")]
        [Display(Name = "Relay Lever Setting :")]
        public System.Decimal? PhaBkRelayLeverSetting { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "MIN_NOR_VOLT")]
        [Display(Name = "Minimum allowable voltage :")]
        public System.Decimal? MinimumAllowableVoltage { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "ANNUAL_LF")]
        [Display(Name = "Annual load factor :")]
        public System.Int16? AnnualLoadFactor { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "NETWORK")]
        [Display(Name = "Network :")]
        public System.String Network { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "DPA_CD")]
        [Display(Name = "Distribution Planning Area :")]
        public System.String DistributionPlanningArea { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "CC_RATING")]
        [Display(Name = "CC Rating :")]
        public System.Int32? CcRating { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "FLISR")]
        [Display(Name = "FLISR Automation Device :")]
        public System.String FlisrAutomationDevice { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "SUMMER_LOAD_LIMIT")]
        [Display(Name = "Summer Load Limit (amps) :")]
        public System.Int64? SummerLoadLimitAmps { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "WINTER_LOAD_LIMIT")]
        [Display(Name = "Winter Load Limit (amps) :")]
        public System.Int64? WinterLoadLimitAmps { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "LIMITING_FACTOR")]
        [Display(Name = "Limiting Factor :")]
        public System.String LimitingFactor { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "FLISR_ENGINEERING_COMMENTS")]
        [Display(Name = "FLISR Comments :")]
        public System.String FLISREngineeringComments { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "OPERATING_MODE")]
        [Display(Name = "Operating Mode  :")]
        public System.String OperatingMode { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "ENGINEERING_DOCUMENT")]
        [Display(Name = "Engineering Document :")]
        public System.String EngineeringDocument { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "SCADA")]
        [Display(Name = "SCADA :")]
        public System.String Scada { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "SCADA_TYPE")]
        [Display(Name = "SCADA Type :")]
        public System.String ScadaType { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "MASTER_STATION")]
        [Display(Name = "Master Station :")]
        public System.String MasterStation { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "BAUD_RATE")]
        [Display(Name = "Baud Rate :")]
        public System.Int32? BaudRate { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "TRANSMIT_ENABLE_DELAY")]
        [Display(Name = "Transmit Enable Delay :")]
        public System.Int32? TransmitEnableDelay { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "TRANSMIT_DISABLE_DELAY")]
        [Display(Name = "Transmit Disable Delay :")]
        public System.Int32? TransmitDisableDelay { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "RTU_ADDRESS")]
        [Display(Name = "RTU Address :")]
        public System.String RtuAddress { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "REPEATER")]
        [Display(Name = "(if applicable) Repeater :")]
        public System.String Repeater { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special conditions :")]
        public System.String SpecialConditions { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "RADIO_MANF_CD")]
        [Display(Name = "SCADA radio manufacturer :")]
        public System.String ScadaRadioManufacturer { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "RADIO_MODEL_NUM")]
        [Display(Name = "SCADA radio model # :")]
        public System.String ScadaRadioModelNum { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "RADIO_SERIAL_NUM")]
        [Display(Name = "SCADA radio serial # :")]
        public System.String ScadaRadioSerialNum { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "OPS_TO_LOCKOUT")]
        [Display(Name = "Ops to Lockout:")]
        public short? OpsToLockout { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "DIRECT_TRANSFER_TRIP")]
        [Display(Name = "Direct Transfer Trip :")]
        public System.String DirectTransferTrip { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "RECLOSE_BLOCKING")]
        [Display(Name = "Reclose Blocking :")]
        public System.String RecloseBlocking { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_PR_CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String PhaPrControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_PR_FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String PhaPrFirmwareVersion { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_PR_SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String PhaPrSoftwareVersion { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_BK_CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String PhaBkControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_BK_FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String PhaBkFirmwareVersion { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "PHA_BK_SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String PhaBkSoftwareVersion { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_PR_CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String GrdPrControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_PR_FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String GrdPrFirmwareVersion { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_PR_SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String GrdPrSoftwareVersion { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_BK_CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String GrdBkControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_BK_FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String GrdBkFirmwareVersion { get; set; }

        [SettingsValidatorAttribute("PRIMARY_GEN", "GRD_BK_SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String GrdBkSoftwareVersion { get; set; }


        /*New fields Settingsgaps*/
        [SettingsValidatorAttribute("PRIMARY_GEN", "ENGINEERING_COMMENTS")]
        [Display(Name = "Engineering Comments :")]
        public System.String EngineeringComments { get; set; }


        [SettingsValidatorAttribute("PRIMARY_GEN", "RTU_EXISTS")]
        [Display(Name = "External RTU :")]
        public System.String RTUExists { get; set; }


        [SettingsValidatorAttribute("PRIMARY_GEN", "RTU_MANF_CD")]
        [Display(Name = "RTU Manufacturer :")]
        public System.String RTUManufacture { get; set; }


        [SettingsValidatorAttribute("PRIMARY_GEN", "RTU_MODEL_NUM")]
        [Display(Name = "RTU Model # :")]
        public System.String RTUModelNumber { get; set; }


        [SettingsValidatorAttribute("PRIMARY_GEN", "RTU_SERIAL_NUM")]
        [Display(Name = "RTU Serial # :")]
        public System.String RTUSerialNumber { get; set; }


        [SettingsValidatorAttribute("PRIMARY_GEN", "RTU_SOFTWARE_VERSION")]
        [Display(Name = "RTU Software version :")]
        public System.String RTUSoftwareVersion { get; set; }


        [SettingsValidatorAttribute("PRIMARY_GEN", "RTU_FIRMWARE_VERSION")]
        [Display(Name = "RTU Firmware version :")]
        public System.String RTUFirmwareVersion { get; set; }

        public SelectList RTUManufactureList { get; set; }


        public SelectList OperatingModeList { get; set; }

        /*end of changes*/

        public bool Release { get; set; }
        public List<GISAttributes> GISAttributes { get; set; }

        public SelectList DistributionPlanningAreaList { get; set; }
        public SelectList EngineeringDocumentList { get; set; }
        public SelectList GrdBkRelayCodeList { get; set; }
        public SelectList GrdBkRelayTypeList { get; set; }
        public SelectList GrdPrRelayCodeList { get; set; }
        public SelectList GrdPrRelayTypeList { get; set; }
        public SelectList PhaBkRelayCodeList { get; set; }
        public SelectList PhaBkRelayTypeList { get; set; }
        public SelectList PhaRrRelayCodeList { get; set; }
        public SelectList PhaRrRelayTypeList { get; set; }
        public SelectList ScadaRadioManufacturerList { get; set; }
        public SelectList ScadaTypeList { get; set; }

        public string DropDownPostbackScriptScada { get; set; }

        public void PopulateEntityFromModel(SM_PRIMARY_GEN e)
        {
            e.OPERATING_NUM = this.OperatingNumber;
            e.DEVICE_ID = this.DeviceId;
            e.PREPARED_BY = this.PreparedBy;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            e.NOTES = this.Notes;
            e.OK_TO_BYPASS = this.OkToBypass;
            e.GRD_PR_RELAY_TYPE = this.GrdPrRelayType;
            e.GRD_PR_MIN_TRIP = this.GrdPrMinimumAmpsToTrip;
            e.GRD_PR_INS_TRIP = this.GrdPrInstantaneousMinimumTrip;
            e.GRD_PR_LEVER_SET = this.GrdPrRelayLeverSetting;
            e.GRD_BK_RELAY_TYPE = this.GrdBkRelayType;
            e.GRD_BK_MIN_TRIP = this.GrdBkMinimumAmpsToTrip;
            e.GRD_BK_INS_TRIP = this.GrdBkInstantaneousMinimumTrip;
            e.GRD_BK_LEVER_SET = this.GrdBkRelayLeverSetting;
            e.PHA_PR_RELAY_TYPE = this.PhaRrRelayType;
            e.PHA_PR_MIN_TRIP = this.PhaRrMinimumAmpsToTrip;
            e.PHA_PR_INS_TRIP = this.PhaRrInstantaneousMinimumTrip;
            e.PHA_PR_LEVER_SET = this.PhaRrRelayLeverSetting;
            e.PHA_BK_RELAY_TYPE = this.PhaBkRelayType;
            e.PHA_BK_MIN_TRIP = this.PhaBkMinimumAmpsToTrip;
            e.PHA_BK_INS_TRIP = this.PhaBkInstantaneousMinimumTrip;
            e.PHA_BK_LEVER_SET = this.PhaBkRelayLeverSetting;
            e.MIN_NOR_VOLT = this.MinimumAllowableVoltage;
            e.ANNUAL_LF = this.AnnualLoadFactor;
            e.NETWORK = this.Network;
            e.DPA_CD = this.DistributionPlanningArea;
            e.CC_RATING = this.CcRating;
            e.FLISR = this.FlisrAutomationDevice;
            e.SUMMER_LOAD_LIMIT = this.SummerLoadLimitAmps;
            e.WINTER_LOAD_LIMIT = this.WinterLoadLimitAmps;
            e.LIMITING_FACTOR = this.LimitingFactor;
            e.FLISR_ENGINEERING_COMMENTS = this.FLISREngineeringComments;
            e.OPERATING_MODE = this.OperatingMode;
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
            e.OPS_TO_LOCKOUT = this.OpsToLockout;
            e.DIRECT_TRANSFER_TRIP = this.DirectTransferTrip;
            e.RECLOSE_BLOCKING = this.RecloseBlocking;
            e.PHA_PR_CONTROL_SERIAL_NUM = this.PhaPrControllerSerialNum;
            e.PHA_PR_FIRMWARE_VERSION = this.PhaPrFirmwareVersion;
            e.PHA_PR_SOFTWARE_VERSION = this.PhaPrSoftwareVersion;
            e.PHA_BK_CONTROL_SERIAL_NUM = this.PhaBkControllerSerialNum;
            e.PHA_BK_FIRMWARE_VERSION = this.PhaBkFirmwareVersion;
            e.PHA_BK_SOFTWARE_VERSION = this.PhaBkSoftwareVersion;
            e.GRD_PR_CONTROL_SERIAL_NUM = this.GrdPrControllerSerialNum;
            e.GRD_PR_FIRMWARE_VERSION = this.GrdPrFirmwareVersion;
            e.GRD_PR_SOFTWARE_VERSION = this.GrdPrSoftwareVersion;
            e.GRD_BK_CONTROL_SERIAL_NUM = this.GrdBkControllerSerialNum;
            e.GRD_BK_FIRMWARE_VERSION = this.GrdBkFirmwareVersion;
            e.GRD_BK_SOFTWARE_VERSION = this.GrdBkSoftwareVersion;
            e.RTU_EXIST = this.RTUExists;
            e.RTU_MANF_CD = this.RTUManufacture;
            e.RTU_FIRMWARE_VERSION = this.RTUFirmwareVersion;
            e.RTU_MODEL_NUM = this.RTUModelNumber;
            e.RTU_SERIAL_NUM = this.RTUSerialNumber;
            e.RTU_SOFTWARE_VERSION = this.RTUSoftwareVersion;
            e.ENGINEERING_COMMENTS = this.EngineeringComments;
        }

        public void PopulateModelFromEntity(SM_PRIMARY_GEN e)
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

        public void PopulateHistoryFromEntity(SM_PRIMARY_GEN_HIST entityHistory, SM_PRIMARY_GEN e)
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

        public void PopulateModelFromHistoryEntity(SM_PRIMARY_GEN_HIST e)
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
        
    }
}