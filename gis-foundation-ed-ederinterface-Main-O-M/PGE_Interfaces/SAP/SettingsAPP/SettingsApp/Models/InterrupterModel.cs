using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class InterrupterModel
    {

        [SettingsValidatorAttribute("Interrupter", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("Interrupter", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceId { get; set; }

        [SettingsValidatorAttribute("Interrupter", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("Interrupter", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("Interrupter", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("Interrupter", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("Interrupter", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("Interrupter", "NOTES")]
        [Display(Name = "Notes :")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("Interrupter", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        [SettingsValidatorAttribute("Interrupter", "CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String ControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("Interrupter", "GRD_CUR_TRIP")]
        [Display(Name = "Overcurrent min. trip :")]
        public System.Int32? GRDOvercurrentMinTrip { get; set; }

        [SettingsValidatorAttribute("Interrupter", "GRD_TRIP_CD")]
        [Display(Name = "Ground Instantaneous :")]
        public System.Decimal? GRDPhaseInstantaneous { get; set; }

        [SettingsValidatorAttribute("Interrupter", "TYP_CRV_GRD")]
        [Display(Name = "Curve :")]
        public System.String GRDCurve { get; set; }

        [SettingsValidatorAttribute("Interrupter", "PHA_CUR_TRIP")]
        [Display(Name = "Overcurrent min. trip :")]
        public System.Int32? PHAOvercurrentMinTrip { get; set; }

        [SettingsValidatorAttribute("Interrupter", "PHA_TRIP_CD")]
        [Display(Name = "Phase instantaneous :")]
        public System.Decimal? PHAPhaseInstantaneous { get; set; }

        [SettingsValidatorAttribute("Interrupter", "TYP_CRV_PHA")]
        [Display(Name = "Curve :")]
        public System.String PHACurve { get; set; }

        [SettingsValidatorAttribute("Interrupter", "MANF_CD")]
        [Display(Name = "Manufacturer :")]
        public System.String Manufacturer { get; set; }

        [SettingsValidatorAttribute("Interrupter", "CT_RATIO")]
        [Display(Name = "CT Ratio :")]
        public System.String CtRatio { get; set; }

        [SettingsValidatorAttribute("Interrupter", "PHA_PICKUP_SETTING")]
        [Display(Name = "Phase Pickup Setting :")]
        public System.String PHAPhasePickupSetting { get; set; }

        [SettingsValidatorAttribute("Interrupter", "GRD_PICKUP_SETTING")]
        [Display(Name = "Ground Pickup Setting :")]
        public System.String GRDPhasePickupSetting { get; set; }

        [SettingsValidatorAttribute("Interrupter", "CONTROL_TYPE")]
        [Display(Name = "Control Type :")]
        public System.String RelayType { get; set; }

        [SettingsValidatorAttribute("Interrupter", "PHA_TD_LEVER_SETTING")]
       // [Display(Name = "TD or Lever Setting :")]
        [DisplayFormat(DataFormatString = "{0:n1}"), Display(Name = "TD or Lever Setting  :")]
        public System.Decimal? PHATdOrLeverSetting { get; set; }

        [SettingsValidatorAttribute("Interrupter", "GRD_TD_LEVER_SETTING")]
       // [Display(Name = "TD or Lever Setting :")]
        [DisplayFormat(DataFormatString = "{0:n1}"), Display(Name = "TD or Lever Setting  :")]
        public System.Decimal? GRDTdOrLeverSetting { get; set; }

        [SettingsValidatorAttribute("Interrupter", "PHA_INST_PICKUP_SETTING")]
        [Display(Name = "Inst Pickup Setting :")]
        public System.Int32? PHAInstPickupSetting { get; set; }

        [SettingsValidatorAttribute("Interrupter", "GRD_INST_PICKUP_SETTING")]
        [Display(Name = "Inst Pickup Setting :")]
        public System.Int32? GRDInstPickupSetting { get; set; }

        [SettingsValidatorAttribute("Interrupter", "OPERATIONAL_MODE_SWITCH")]
        [Display(Name = "Operational Mode Switch :")]
        public System.String OperationalModeSwitch { get; set; }

        [SettingsValidatorAttribute("Interrupter", "ENGINEERING_COMMENTS")]
        [Display(Name = "Engineering Comments :")]
        public System.String EngineeringComment { get; set; }

        [SettingsValidatorAttribute("Interrupter", "PRIMARY_VOLTAGE")]
        [Display(Name = "Primary Voltage :")]
        public System.Int64? PrimaryVoltage { get; set; }

        [SettingsValidatorAttribute("Interrupter", "FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String FirmwareVersion { get; set; }

        [SettingsValidatorAttribute("Interrupter", "SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String SoftwareVersion { get; set; }

        [SettingsValidatorAttribute("Interrupter", "ENGINEERING_DOCUMENT")]
        [Display(Name = "Engineering Document :")]
        public System.String EngineeringDocument { get; set; }

        [SettingsValidatorAttribute("Interrupter", "SCADA")]
        [Display(Name = "SCADA :")]
        public System.String Scada { get; set; }

        [SettingsValidatorAttribute("Interrupter", "SCADA_TYPE")]
        [Display(Name = "SCADA Type :")]
        public System.String ScadaType { get; set; }

        [SettingsValidatorAttribute("Interrupter", "MASTER_STATION")]
        [Display(Name = "Master Station :")]
        public System.String MasterStation { get; set; }

        [SettingsValidatorAttribute("Interrupter", "BAUD_RATE")]
        [Display(Name = "Baud Rate :")]
        public System.Int32? BaudRate { get; set; }

        [SettingsValidatorAttribute("Interrupter", "TRANSMIT_ENABLE_DELAY")]
       // [DisplayFormat(DataFormatString = "{0:n1}"), Display(Name = "Transmit Enable Delay :")]
       [Display(Name = "Transmit Enable Delay :")]
        public System.Int32? TransmitEnableDelay { get; set; }

        [SettingsValidatorAttribute("Interrupter", "TRANSMIT_DISABLE_DELAY")]
       // [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
       
        [Display(Name = "Transmit Disable Delay :")]
        public System.Int32? TransmitDisableDelay { get; set; }
       
        [SettingsValidatorAttribute("Interrupter", "RTU_ADDRESS")]
        [Display(Name = "RTU Address :")]
        public System.String RtuAddress { get; set; }

        [SettingsValidatorAttribute("Interrupter", "REPEATER")]
        [Display(Name = "(if applicable) Repeater :")]
        public System.String Repeater { get; set; }

        [SettingsValidatorAttribute("Interrupter", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special conditions :")]
        public System.String SpecialConditions { get; set; }

        [SettingsValidatorAttribute("Interrupter", "RADIO_MANF_CD")]
        [Display(Name = "SCADA radio manufacturer :")]
        public System.String ScadaRadioManufacturer { get; set; }

        [SettingsValidatorAttribute("Interrupter", "RADIO_MODEL_NUM")]
        [Display(Name = "SCADA radio model # :")]
        public System.String ScadaRadioModelNum { get; set; }

        [SettingsValidatorAttribute("Interrupter", "RADIO_SERIAL_NUM")]
        [Display(Name = "SCADA radio serial # :")]
        public System.String ScadaRadioSerialNum { get; set; }

        [SettingsValidatorAttribute("Interrupter", "FLISR")]
        [Display(Name = "FLISR Automation Device :")]
        public System.String Flisr { get; set; }

        [SettingsValidatorAttribute("Interrupter", "SUMMER_LOAD_LIMIT")]
        [Display(Name = "Summer Load Limit (amps) :")]
        public System.Int64? SummerLoadLimit { get; set; }

        [SettingsValidatorAttribute("Interrupter", "WINTER_LOAD_LIMIT")]
        [Display(Name = "Winter Load Limit (amps) :")]
        public System.Int64? WinterLoadLimit { get; set; }

        [SettingsValidatorAttribute("Interrupter", "LIMITING_FACTOR")]
        [Display(Name = "Limiting Factor :")]
        public System.String LimitingFactor { get; set; }

        [SettingsValidatorAttribute("Interrupter", "FLISR_ENGINEERING_COMMENTS")]
        [Display(Name = "FLISR Comments :")]
        public System.String FlisrEngineeringComments { get; set; }

        [SettingsValidatorAttribute("Interrupter", "OPERATING_MODE")]
        [Display(Name = "Operating As :")]
        public System.String OperatingMode{ get; set; }


        /*New fields Settingsgaps*/
        [SettingsValidatorAttribute("Interrupter", "RTU_EXISTS")]
        [Display(Name = "External RTU :")]
        public System.String RTUExists { get; set; }


        [SettingsValidatorAttribute("Interrupter", "RTU_MANF_CD")]
        [Display(Name = "RTU Manufacturer :")]
        public System.String RTUManufacture { get; set; }


        [SettingsValidatorAttribute("Interrupter", "RTU_MODEL_NUM")]
        [Display(Name = "RTU Model # :")]
        public System.String RTUModelNumber { get; set; }


        [SettingsValidatorAttribute("Interrupter", "RTU_SERIAL_NUM")]
        [Display(Name = "RTU Serial # :")]
        public System.String RTUSerialNumber { get; set; }


        [SettingsValidatorAttribute("Interrupter", "RTU_SOFTWARE_VERSION")]
        [Display(Name = "RTU Software version :")]
        public System.String RTUSoftwareVersion { get; set; }


        [SettingsValidatorAttribute("Interrupter", "RTU_FIRMWARE_VERSION")]
        [Display(Name = "RTU Firmware version :")]
        public System.String RTUFirmwareVersion { get; set; }

        public SelectList RTUManufactureList { get; set; }

        /*End of changes Settingsgaps*/

        public bool Release { get; set; }
        public List<GISAttributes> GISAttributes { get; set; }

        public string DropDownPostbackScript { get; set; }
        public string DropDownPostbackScriptScada { get; set; }

        public SelectList RelayTypeList { get; set; }
        public SelectList EngineeringDocumentList { get; set; }
        public SelectList GRDCurveList { get; set; }
        public SelectList GRDPhaseInstantaneousList { get; set; }
        public SelectList ScadaTypeList { get; set; }
        public SelectList ScadaRadioManufacturerList { get; set; }
        public SelectList ManufacturerList { get; set; }
        public SelectList PHACurveList { get; set; }
        public SelectList PHAPhaseInstantaneousList { get; set; }
        public SelectList OperatingModeList { get; set; }


        public void PopulateEntityFromModel(SM_INTERRUPTER e)
        {
            e.DEVICE_ID = this.DeviceId;
            e.PREPARED_BY = this.PreparedBy;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            //e.NOTES = this.Notes;
            e.OK_TO_BYPASS = this.OkToBypass;
            e.CONTROL_SERIAL_NUM = this.ControllerSerialNum;
            e.GRD_CUR_TRIP = this.GRDOvercurrentMinTrip;
            e.GRD_TRIP_CD = this.GRDPhaseInstantaneous;
            e.TYP_CRV_GRD = this.GRDCurve;
            e.PHA_CUR_TRIP = this.PHAOvercurrentMinTrip;
            e.PHA_TRIP_CD = this.PHAPhaseInstantaneous;
            e.TYP_CRV_PHA = this.PHACurve;
            e.MANF_CD = this.Manufacturer;
            e.CT_RATIO = this.CtRatio;
            e.PHA_PICKUP_SETTING = this.PHAPhasePickupSetting;
            e.GRD_PICKUP_SETTING = this.GRDPhasePickupSetting;
            e.CONTROL_TYPE = this.RelayType;
            e.PHA_TD_LEVER_SETTING = this.PHATdOrLeverSetting;
            e.GRD_TD_LEVER_SETTING = this.GRDTdOrLeverSetting;
            e.PHA_INST_PICKUP_SETTING = this.PHAInstPickupSetting;
            e.GRD_INST_PICKUP_SETTING = this.GRDInstPickupSetting;
            e.OPERATIONAL_MODE_SWITCH = this.OperationalModeSwitch;
            e.ENGINEERING_COMMENTS = this.EngineeringComment;
            e.PRIMARY_VOLTAGE = this.PrimaryVoltage;
            e.FIRMWARE_VERSION = this.FirmwareVersion;
            e.SOFTWARE_VERSION = this.SoftwareVersion;
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
            e.FLISR = this.Flisr;
            e.SUMMER_LOAD_LIMIT = this.SummerLoadLimit;
            e.WINTER_LOAD_LIMIT = this.WinterLoadLimit;
            e.LIMITING_FACTOR = this.LimitingFactor;
            e.FLISR_ENGINEERING_COMMENTS = this.FlisrEngineeringComments;
            e.OPERATING_MODE = this.OperatingMode;
            e.RTU_EXIST = this.RTUExists;
            e.RTU_MANF_CD = this.RTUManufacture;
            e.RTU_FIRMWARE_VERSION = this.RTUFirmwareVersion;
            e.RTU_MODEL_NUM = this.RTUModelNumber;
            e.RTU_SERIAL_NUM = this.RTUSerialNumber;
            e.RTU_SOFTWARE_VERSION = this.RTUSoftwareVersion;
            
        }

        public void PopulateModelFromEntity(SM_INTERRUPTER e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            //this.Notes = e.NOTES;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
            this.GRDOvercurrentMinTrip = e.GRD_CUR_TRIP;
            this.GRDPhaseInstantaneous = e.GRD_TRIP_CD;
            this.GRDCurve = e.TYP_CRV_GRD;
            this.PHAOvercurrentMinTrip = e.PHA_CUR_TRIP;
            this.PHAPhaseInstantaneous = e.PHA_TRIP_CD;
            this.PHACurve = e.TYP_CRV_PHA;
            this.Manufacturer = e.MANF_CD;
            this.CtRatio = e.CT_RATIO;
            this.PHAPhasePickupSetting = e.PHA_PICKUP_SETTING;
            this.GRDPhasePickupSetting = e.GRD_PICKUP_SETTING;
            this.RelayType = e.CONTROL_TYPE;
            this.PHATdOrLeverSetting = e.PHA_TD_LEVER_SETTING;
            this.GRDTdOrLeverSetting = e.GRD_TD_LEVER_SETTING;
            this.PHAInstPickupSetting = e.PHA_INST_PICKUP_SETTING;
            this.GRDInstPickupSetting = e.GRD_INST_PICKUP_SETTING;
            this.OperationalModeSwitch = e.OPERATIONAL_MODE_SWITCH;
            this.EngineeringComment = e.ENGINEERING_COMMENTS;
            this.PrimaryVoltage = e.PRIMARY_VOLTAGE;
            this.FirmwareVersion = e.FIRMWARE_VERSION;
            this.SoftwareVersion = e.SOFTWARE_VERSION;
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
            this.Flisr = e.FLISR;
            this.SummerLoadLimit = e.SUMMER_LOAD_LIMIT;
            this.WinterLoadLimit = e.WINTER_LOAD_LIMIT;
            this.LimitingFactor = e.LIMITING_FACTOR;
            this.FlisrEngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            this.OperatingMode = e.OPERATING_MODE;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
        }

        public void PopulateHistoryFromEntity(SM_INTERRUPTER_HIST entityHistory, SM_INTERRUPTER e)
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
            //entityHistory.NOTES = e.NOTES;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
            entityHistory.FIRMWARE_VERSION = e.FIRMWARE_VERSION;
            entityHistory.SOFTWARE_VERSION = e.SOFTWARE_VERSION;
            entityHistory.CONTROL_SERIAL_NUM = e.CONTROL_SERIAL_NUM;
            entityHistory.PROCESSED_FLAG = e.PROCESSED_FLAG;
            entityHistory.RELEASED_BY = e.RELEASED_BY;
            entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
            entityHistory.GRD_CUR_TRIP = e.GRD_CUR_TRIP;
            entityHistory.GRD_TRIP_CD = e.GRD_TRIP_CD;
            entityHistory.TYP_CRV_GRD = e.TYP_CRV_GRD;
            entityHistory.PHA_CUR_TRIP = e.PHA_CUR_TRIP;
            entityHistory.PHA_TRIP_CD = e.PHA_TRIP_CD;
            entityHistory.TYP_CRV_PHA = e.TYP_CRV_PHA;
            entityHistory.MANF_CD = e.MANF_CD;
            entityHistory.CT_RATIO = e.CT_RATIO;
            entityHistory.PHA_PICKUP_SETTING = e.PHA_PICKUP_SETTING;
            entityHistory.GRD_PICKUP_SETTING = e.GRD_PICKUP_SETTING;
            entityHistory.CONTROL_TYPE = e.CONTROL_TYPE;
            entityHistory.PHA_TD_LEVER_SETTING = e.PHA_TD_LEVER_SETTING;
            entityHistory.GRD_TD_LEVER_SETTING = e.GRD_TD_LEVER_SETTING;
            entityHistory.PHA_INST_PICKUP_SETTING = e.PHA_INST_PICKUP_SETTING;
            entityHistory.GRD_INST_PICKUP_SETTING = e.GRD_INST_PICKUP_SETTING;
            entityHistory.OPERATIONAL_MODE_SWITCH = e.OPERATIONAL_MODE_SWITCH;
            entityHistory.ENGINEERING_COMMENTS = e.ENGINEERING_COMMENTS;
            entityHistory.PRIMARY_VOLTAGE = e.PRIMARY_VOLTAGE;
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
            entityHistory.FLISR = e.FLISR;
            entityHistory.SUMMER_LOAD_LIMIT = e.SUMMER_LOAD_LIMIT;
            entityHistory.WINTER_LOAD_LIMIT = e.WINTER_LOAD_LIMIT;
            entityHistory.LIMITING_FACTOR = e.LIMITING_FACTOR;
            entityHistory.FLISR_ENGINEERING_COMMENTS = e.FLISR_ENGINEERING_COMMENTS;
            entityHistory.OPERATING_MODE = e.OPERATING_MODE;
            entityHistory.RTU_EXIST = e.RTU_EXIST;
            entityHistory.RTU_MANF_CD = e.RTU_MANF_CD;
            entityHistory.RTU_FIRMWARE_VERSION = e.RTU_FIRMWARE_VERSION;
            entityHistory.RTU_MODEL_NUM = e.RTU_MODEL_NUM;
            entityHistory.RTU_SERIAL_NUM = e.RTU_SERIAL_NUM;
            entityHistory.RTU_SOFTWARE_VERSION = e.RTU_SOFTWARE_VERSION;
           
        }

        public void PopulateModelFromHistoryEntity(SM_INTERRUPTER_HIST e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            //this.Notes = e.NOTES;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
            this.GRDOvercurrentMinTrip = e.GRD_CUR_TRIP;
            this.GRDPhaseInstantaneous = e.GRD_TRIP_CD;
            this.GRDCurve = e.TYP_CRV_GRD;
            this.PHAOvercurrentMinTrip = e.PHA_CUR_TRIP;
            this.PHAPhaseInstantaneous = e.PHA_TRIP_CD;
            this.PHACurve = e.TYP_CRV_PHA;
            this.Manufacturer = e.MANF_CD;
            this.CtRatio = e.CT_RATIO;
            this.PHAPhasePickupSetting = e.PHA_PICKUP_SETTING;
            this.GRDPhasePickupSetting = e.GRD_PICKUP_SETTING;
            this.RelayType = e.CONTROL_TYPE;
            this.PHATdOrLeverSetting = e.PHA_TD_LEVER_SETTING;
            this.GRDTdOrLeverSetting = e.GRD_TD_LEVER_SETTING;
            this.PHAInstPickupSetting = e.PHA_INST_PICKUP_SETTING;
            this.GRDInstPickupSetting = e.GRD_INST_PICKUP_SETTING;
            this.OperationalModeSwitch = e.OPERATIONAL_MODE_SWITCH;
            this.EngineeringComment = e.ENGINEERING_COMMENTS;
            this.PrimaryVoltage = e.PRIMARY_VOLTAGE;
            this.FirmwareVersion = e.FIRMWARE_VERSION;
            this.SoftwareVersion = e.SOFTWARE_VERSION;
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
            this.Flisr = e.FLISR;
            this.SummerLoadLimit = e.SUMMER_LOAD_LIMIT;
            this.WinterLoadLimit = e.WINTER_LOAD_LIMIT;
            this.LimitingFactor = e.LIMITING_FACTOR;
            this.FlisrEngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            this.OperatingMode = e.OPERATING_MODE;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
        }
    }
}