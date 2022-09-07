using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class SectionalizerModel
    {
        [SettingsValidatorAttribute("Sectionalizer", "ID")]
        [Display(Name = "ID :")]
        public System.Int32 ID { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceId { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String ControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "NOTES")]
        [Display(Name = "Notes :")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "CONTROL_TYPE")]
        [Display(Name = "Control Type :")]
        public System.String ControlType { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "SECT_TYPE")]
        [Display(Name = "Sectionalizer Type :")]
        public System.String SectionalizerType { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "MIN_PC_TO_CT")]
        [Display(Name = "PHA Min Acutating Current :")]
        public System.Int16? PhaMinAcutatingCurrent { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "PHA_INRUSH_DURATION")]
        [Display(Name = "PHA Inrush Sensing Duration :")]
        public System.Decimal? PhaInrushSensingDuration { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "PHA_INRUSH_MULTIPLIER")]
        [Display(Name = "PHA Inrush Mult. :")]
        public System.String PhaInrushMult { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "PHA_INRUSH_TIME")]
        [Display(Name = "PHA Current Inrush Time :")]
        public System.Int32? PhaCurrentInrushTime { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "FIRST_RECLOSE_RESET_TIME")]
        [Display(Name = "Time from First Reclose Time to Reset :")]
        public System.Int16? TimeFromFirstRecloseTimeToReset { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "REQUIRED_FAULT_CURRENT")]
        [Display(Name = "Fault Required Befor Voltage Loss :")]
        public System.String FaultRequiredBeforVoltageLoss { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "MIN_GRD_TO_CT")]
        [Display(Name = "GND Min Actuating Current :")]
        public System.Int16? GndMinActuatingCurrent { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "GRD_INRUSH_DURATION")]
        [Display(Name = "GND Inrush Sensing Duration :")]
        public System.Decimal? GndInrushSensingDuration { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "GRD_INRUSH_MULTIPLIER")]
        [Display(Name = "GND Inrush Mult :")]
        public System.String GndInrushMult { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "GRD_INRUSH_TIME")]
        [Display(Name = "GND Current Inrush Time :")]
        public System.Int32? GndCurrentInrushTime { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "RESET")]
        [Display(Name = "Successful Reclose Reset Time :")]
        public System.Decimal? SuccessfulRecloseResetTime { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "VOLT_THRESHOLD")]
        [Display(Name = "Voltage Threshold :")]
        public System.Decimal? VoltageThresshold { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "LOCKOUT_NUM")]
        [Display(Name = "Count Before Lockout :")]
        public System.Int16? CountBeforeLockout { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "ONE_SHOT_LOCKOUT_NUM")]
        [Display(Name = "One-Shot-to-Lockout Count :")]
        public System.Int16? OneShotToLockoutCount { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "ONE_SHOT_LOCKOUT_TIME")]
        [Display(Name = "One-Shot-to-Lockout Time :")]
        public System.Int16? OneShotToLockoutTime { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String FirmwareVersion { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String SoftwareVersion { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "ENGINEERING_DOCUMENT")]
        [Display(Name = "Engineering Document :")]
        public System.String EngineeringDocument { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "SCADA")]
        [Display(Name = "SCADA :")]
        public System.String Scada { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "SCADA_TYPE")]
        [Display(Name = "SCADA Type :")]
        public System.String ScadaType { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "MASTER_STATION")]
        [Display(Name = "Master Station :")]
        public System.String MasterStation { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "BAUD_RATE")]
        [Display(Name = "Baud Rate :")]
        public System.Int32? BaudRate { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "TRANSMIT_ENABLE_DELAY")]
        [Display(Name = "Transmit Enable Delay :")]
        public System.Int32? TransmitEnableDelay { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "TRANSMIT_DISABLE_DELAY")]
        [Display(Name = "Transmit Disable Delay :")]
        public System.Int32? TransmitDisableDelay { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "RTU_ADDRESS")]
        [Display(Name = "RTU Address :")]
        public System.String RtuAddress { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "REPEATER")]
        [Display(Name = "(if applicable) Repeater :")]
        public System.String Repeater { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special conditions :")]
        public System.String SpecialConditions { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "RADIO_MANF_CD")]
        [Display(Name = "SCADA radio manufacturer :")]
        public System.String ScadaRadioManufacturer { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "RADIO_MODEL_NUM")]
        [Display(Name = "SCADA radio model # :")]
        public System.String ScadaRadioModelNum { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "RADIO_SERIAL_NUM")]
        [Display(Name = "SCADA radio serial # :")]
        public System.String ScadaRadioSerialNum { get; set; }


        /*New fields Settingsgaps*/
        [SettingsValidatorAttribute("Sectionalizer", "ENGINEERING_COMMENTS")]
        [Display(Name = "Engineering Comments :")]
        public System.String EngineeringComments { get; set; }


        [SettingsValidatorAttribute("Sectionalizer", "RTU_EXISTS")]
        [Display(Name = "External RTU :")]
        public System.String RTUExists { get; set; }


        [SettingsValidatorAttribute("Sectionalizer", "RTU_MANF_CD")]
        [Display(Name = "RTU Manufacturer :")]
        public System.String RTUManufacture { get; set; }


        [SettingsValidatorAttribute("Sectionalizer", "RTU_MODEL_NUM")]
        [Display(Name = "RTU Model # :")]
        public System.String RTUModelNumber { get; set; }


        [SettingsValidatorAttribute("Sectionalizer", "RTU_SERIAL_NUM")]
        [Display(Name = "RTU Serial # :")]
        public System.String RTUSerialNumber { get; set; }


        [SettingsValidatorAttribute("Sectionalizer", "RTU_SOFTWARE_VERSION")]
        [Display(Name = "RTU Software version :")]
        public System.String RTUSoftwareVersion { get; set; }


        [SettingsValidatorAttribute("Sectionalizer", "RTU_FIRMWARE_VERSION")]
        [Display(Name = "RTU Firmware version :")]
        public System.String RTUFirmwareVersion { get; set; }


        [SettingsValidatorAttribute("Sectionalizer", "OPERATING_MODE")]
        [Display(Name = "Operating As  :")]
        public System.String OperatingMode { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "FLISR_ENGINEERING_COMMENT")]
        [Display(Name = "FLISR Comments  :")]
        public System.String FLISREngineeringComments { get; set; }

        [SettingsValidatorAttribute("Sectionalizer", "FLISR")]
        [Display(Name = "FLISR Automation Device :")]
        public System.String FlisrAutomationDevice { get; set; }

        public SelectList RTUManufactureList { get; set; }
        public SelectList OperatingModeList { get; set; }
        public string DropDownPostbackScriptScada { get; set; }
        /*end of changes*/


        public bool Release { get; set; }
        public List<GISAttributes> GISAttributes { get; set; }

        public SelectList ControlTypeList { get; set; }
        public SelectList EngineeringDocumentList { get; set; }
        public SelectList GndInrushMultList { get; set; }
        public SelectList PhaInrushMultList { get; set; }
        public SelectList ScadaRadioManufacturerList { get; set; }
        public SelectList FaultRequiredBeforVoltageLossList { get; set; }
        public SelectList ScadaTypeList { get; set; }
        public SelectList ScadaList { get; set; }
        public SelectList SectionalizerTypeList { get; set; }
        public SelectList OkToBypassList { get; set; }

        public void PopulateEntityFromModel(SM_SECTIONALIZER e)
        {
            e.DEVICE_ID = this.DeviceId;
            e.PREPARED_BY = this.PreparedBy;
            e.CONTROL_SERIAL_NUM = this.ControllerSerialNum;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            //e.NOTES = this.Notes;
            e.OK_TO_BYPASS = this.OkToBypass;
            e.CONTROL_TYPE = this.ControlType;
            e.SECT_TYPE = this.SectionalizerType;
            e.MIN_PC_TO_CT = this.PhaMinAcutatingCurrent;
            e.PHA_INRUSH_DURATION = this.PhaInrushSensingDuration;
            e.PHA_INRUSH_MULTIPLIER = this.PhaInrushMult;
            e.PHA_INRUSH_TIME = this.PhaCurrentInrushTime;
            e.FIRST_RECLOSE_RESET_TIME = this.TimeFromFirstRecloseTimeToReset;
            e.REQUIRED_FAULT_CURRENT = this.FaultRequiredBeforVoltageLoss;
            e.MIN_GRD_TO_CT = this.GndMinActuatingCurrent;
            e.GRD_INRUSH_DURATION = this.GndInrushSensingDuration;
            e.GRD_INRUSH_MULTIPLIER = this.GndInrushMult;
            e.GRD_INRUSH_TIME = this.GndCurrentInrushTime;
            e.RESET = this.SuccessfulRecloseResetTime;
            e.VOLT_THRESHOLD = this.VoltageThresshold;
            e.LOCKOUT_NUM = this.CountBeforeLockout;
            e.ONE_SHOT_LOCKOUT_NUM = this.OneShotToLockoutCount;
            e.ONE_SHOT_LOCKOUT_TIME = this.OneShotToLockoutTime;
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
            e.RTU_EXIST = this.RTUExists;
            e.RTU_MANF_CD = this.RTUManufacture;
            e.RTU_FIRMWARE_VERSION = this.RTUFirmwareVersion;
            e.RTU_MODEL_NUM = this.RTUModelNumber;
            e.RTU_SERIAL_NUM = this.RTUSerialNumber;
            e.RTU_SOFTWARE_VERSION = this.RTUSoftwareVersion;
            e.ENGINEERING_COMMENTS = this.EngineeringComments;
            e.FLISR_ENGINEERING_COMMENTS = this.FLISREngineeringComments;
            e.OPERATING_MODE = this.OperatingMode;
            e.FLISR = this.FlisrAutomationDevice;
        }

        public void PopulateModelFromHistoryEntity(SM_SECTIONALIZER_HIST e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            this.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            //this.Notes = e.NOTES;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.ControlType = e.CONTROL_TYPE;
            this.SectionalizerType = e.SECT_TYPE;
            this.PhaMinAcutatingCurrent = e.MIN_PC_TO_CT;
            this.PhaInrushSensingDuration = e.PHA_INRUSH_DURATION;
            this.PhaInrushMult = e.PHA_INRUSH_MULTIPLIER;
            this.PhaCurrentInrushTime = e.PHA_INRUSH_TIME;
            this.TimeFromFirstRecloseTimeToReset = e.FIRST_RECLOSE_RESET_TIME;
            this.FaultRequiredBeforVoltageLoss = e.REQUIRED_FAULT_CURRENT;
            this.GndMinActuatingCurrent = e.MIN_GRD_TO_CT;
            this.GndInrushSensingDuration = e.GRD_INRUSH_DURATION;
            this.GndInrushMult = e.GRD_INRUSH_MULTIPLIER;
            this.GndCurrentInrushTime = e.GRD_INRUSH_TIME;
            this.SuccessfulRecloseResetTime = e.RESET;
            this.VoltageThresshold = e.VOLT_THRESHOLD;
            this.CountBeforeLockout = e.LOCKOUT_NUM;
            this.OneShotToLockoutCount = e.ONE_SHOT_LOCKOUT_NUM;
            this.OneShotToLockoutTime = e.ONE_SHOT_LOCKOUT_TIME;
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
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
            this.FLISREngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            this.OperatingMode = e.OPERATING_MODE;
            this.FlisrAutomationDevice = e.FLISR;
        }

        public void PopulateModelFromEntity(SM_SECTIONALIZER e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            this.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            //this.Notes = e.NOTES;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.ControlType = e.CONTROL_TYPE;
            this.SectionalizerType = e.SECT_TYPE;
            this.PhaMinAcutatingCurrent = e.MIN_PC_TO_CT;
            this.PhaInrushSensingDuration = e.PHA_INRUSH_DURATION;
            this.PhaInrushMult = e.PHA_INRUSH_MULTIPLIER;
            this.PhaCurrentInrushTime = e.PHA_INRUSH_TIME;
            this.TimeFromFirstRecloseTimeToReset = e.FIRST_RECLOSE_RESET_TIME;
            this.FaultRequiredBeforVoltageLoss = e.REQUIRED_FAULT_CURRENT;
            this.GndMinActuatingCurrent = e.MIN_GRD_TO_CT;
            this.GndInrushSensingDuration = e.GRD_INRUSH_DURATION;
            this.GndInrushMult = e.GRD_INRUSH_MULTIPLIER;
            this.GndCurrentInrushTime = e.GRD_INRUSH_TIME;
            this.SuccessfulRecloseResetTime = e.RESET;
            this.VoltageThresshold = e.VOLT_THRESHOLD;
            this.CountBeforeLockout = e.LOCKOUT_NUM;
            this.OneShotToLockoutCount = e.ONE_SHOT_LOCKOUT_NUM;
            this.OneShotToLockoutTime = e.ONE_SHOT_LOCKOUT_TIME;
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
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
            this.FLISREngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            this.OperatingMode = e.OPERATING_MODE;
            this.FlisrAutomationDevice = e.FLISR;
        }

        public void PopulateHistoryFromEntity(SM_SECTIONALIZER_HIST entityHistory, SM_SECTIONALIZER e)
        {
            entityHistory.GLOBAL_ID = e.GLOBAL_ID;
            entityHistory.FEATURE_CLASS_NAME = e.FEATURE_CLASS_NAME;
            entityHistory.OPERATING_NUM = e.OPERATING_NUM;
            entityHistory.DEVICE_ID = e.DEVICE_ID;
            entityHistory.PREPARED_BY = e.PREPARED_BY;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.TIMESTAMP = e.TIMESTAMP;
            entityHistory.EFFECTIVE_DT = e.EFFECTIVE_DT;
            entityHistory.PEER_REVIEW_DT = e.PEER_REVIEW_DT;
            entityHistory.PEER_REVIEW_BY = e.PEER_REVIEW_BY;
            entityHistory.DIVISION = e.DIVISION;
            entityHistory.DISTRICT = e.DISTRICT;
            //entityHistory.NOTES = e.NOTES;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
            entityHistory.CONTROL_SERIAL_NUM = e.CONTROL_SERIAL_NUM;
            entityHistory.PROCESSED_FLAG = e.PROCESSED_FLAG;
            entityHistory.RELEASED_BY = e.RELEASED_BY;
            entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
            entityHistory.CONTROL_TYPE = e.CONTROL_TYPE;
            entityHistory.SECT_TYPE = e.SECT_TYPE;
            entityHistory.MIN_PC_TO_CT = e.MIN_PC_TO_CT;
            entityHistory.PHA_INRUSH_DURATION = e.PHA_INRUSH_DURATION;
            entityHistory.PHA_INRUSH_MULTIPLIER = e.PHA_INRUSH_MULTIPLIER;
            entityHistory.PHA_INRUSH_TIME = e.PHA_INRUSH_TIME;
            entityHistory.FIRST_RECLOSE_RESET_TIME = e.FIRST_RECLOSE_RESET_TIME;
            entityHistory.REQUIRED_FAULT_CURRENT = e.REQUIRED_FAULT_CURRENT;
            entityHistory.MIN_GRD_TO_CT = e.MIN_GRD_TO_CT;
            entityHistory.GRD_INRUSH_DURATION = e.GRD_INRUSH_DURATION;
            entityHistory.GRD_INRUSH_MULTIPLIER = e.GRD_INRUSH_MULTIPLIER;
            entityHistory.GRD_INRUSH_TIME = e.GRD_INRUSH_TIME;
            entityHistory.RESET = e.RESET;
            entityHistory.VOLT_THRESHOLD = e.VOLT_THRESHOLD;
            entityHistory.LOCKOUT_NUM = e.LOCKOUT_NUM;
            entityHistory.ONE_SHOT_LOCKOUT_NUM = e.ONE_SHOT_LOCKOUT_NUM;
            entityHistory.ONE_SHOT_LOCKOUT_TIME = e.ONE_SHOT_LOCKOUT_TIME;
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
            entityHistory.RTU_EXIST = e.RTU_EXIST;
            entityHistory.RTU_MANF_CD = e.RTU_MANF_CD;
            entityHistory.RTU_FIRMWARE_VERSION = e.RTU_FIRMWARE_VERSION;
            entityHistory.RTU_MODEL_NUM = e.RTU_MODEL_NUM;
            entityHistory.RTU_SERIAL_NUM = e.RTU_SERIAL_NUM;
            entityHistory.RTU_SOFTWARE_VERSION = e.RTU_SOFTWARE_VERSION;
            entityHistory.ENGINEERING_COMMENTS = e.ENGINEERING_COMMENTS;
            entityHistory.FLISR_ENGINEERING_COMMENTS = e.FLISR_ENGINEERING_COMMENTS;
            entityHistory.OPERATING_MODE = e.OPERATING_MODE;
            entityHistory.FLISR = e.FLISR;
        }
    }
}