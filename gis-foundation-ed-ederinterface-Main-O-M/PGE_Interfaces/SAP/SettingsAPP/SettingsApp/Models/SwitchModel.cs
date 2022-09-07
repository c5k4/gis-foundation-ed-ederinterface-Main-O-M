using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    [Serializable()]
    public partial class SwitchModel
    {

        [SettingsValidatorAttribute("SWITCH", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceID { get; set; }

        [SettingsValidatorAttribute("SWITCH", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("SWITCH", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("SWITCH", "CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String ControllerSerialNo { get; set; }

        [SettingsValidatorAttribute("SWITCH", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String Datemodified { get; set; }

        [SettingsValidatorAttribute("SWITCH", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("SWITCH", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("SWITCH", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("SWITCH", "NOTES")]
        [Display(Name = "Notes :")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("SWITCH", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        [SettingsValidatorAttribute("SWITCH", "CONTROL_UNIT_TYPE")]
        [Display(Name = "Control Unit Type :")]
        public System.String ControlUnitType { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SWITCH_TYPE")]
        [Display(Name = "Switch Type :")]
        public System.String SwitchType { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SECTIONALIZING_FEATURE")]
        [Display(Name = "Sectionalizing Feature :")]
        public System.String SectionalizingFeature { get; set; }

        [SettingsValidatorAttribute("SWITCH", "ATS_CAPABLE")]
        [Display(Name = "ATS Capable :")]
        public System.String ATSCapable { get; set; }

        [SettingsValidatorAttribute("SWITCH", "ATS_FEATURE")]
        [Display(Name = "ATS Feature :")]
        public System.String ATSFeature { get; set; }

        [SettingsValidatorAttribute("SWITCH", "PHA_FAULT_CUR_LEVEL")]
        [Display(Name = "Phase Fault DPhase Fault Detection Current Level (RMS Amp) :")]
        public System.Int64? PhaseFaultDPhaseFaultDetectionCurrentLevelRMSAmp { get; set; }

        [SettingsValidatorAttribute("SWITCH", "GRD_FAULT_CUR_LEVEL")]
        [Display(Name = "Ground Fault Detection Current Level (RMS Amps) :")]
        public System.Int64? GroundFaultDetectionCurrentLevelRMSAmps { get; set; }

        [SettingsValidatorAttribute("SWITCH", "PHA_FAULT_DURATION")]
        [Display(Name = "Phase Fault Duration Time Threshold (Msecs) :")]
        public System.Int64? PhaseFaultDurationTimeThresholdMsecs { get; set; }

        [SettingsValidatorAttribute("SWITCH", "GRD_FAULT_DURATION")]
        [Display(Name = "Ground Fault Duration Time Threshold (Msecs) :")]
        public System.Int64? GroundFaultDurationTimeThresholdMsecs { get; set; }

        [SettingsValidatorAttribute("SWITCH", "PHA_INRUSH_TIME")]
        [Display(Name = "Phase Current Inrush Restraint Time (Msecs) :")]
        public System.Int64? PhaseCurrentInrushRestraintTimeMsecs { get; set; }

        [SettingsValidatorAttribute("SWITCH", "GRD_INRUSH_TIME")]
        [Display(Name = "Ground Current Inrush Restraint Time (Msecs) :")]
        public System.Int64? GroundCurrentInrushRestraintTimeMsecs { get; set; }

        [SettingsValidatorAttribute("SWITCH", "PHA_INRUSH_MULT")]
        [Display(Name = "Phase Current Inrush Restraint Multiplier :")]
        public System.Decimal? PhaseCurrentInrushRestraintMultiplier { get; set; }

        [SettingsValidatorAttribute("SWITCH", "GRD_INRUSH_MULT")]
        [Display(Name = "Ground Current Inrush Restraint Multiplier :")]
        public System.Decimal? GroundCurrentInrushRestraintMultiplier { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SECT_RESET_TIME")]
        [Display(Name = "Sectionalizer Reset Time (seconds) :")]
        public System.Int64? SectionalizerResetTime { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RECLOSE_RESET_TIME")]
        [Display(Name = "Successful Reclose Reset Time (seconds) :")]
        public System.Int64? SuccessfulRecloseResetTime { get; set; }

        [SettingsValidatorAttribute("SWITCH", "OC_TO_VOLT_TIME")]
        [Display(Name = "OC to Volt Loss Association Time (tenths, both feeds) :")]
        public System.Decimal? OCtoVoltLossAssociationTime { get; set; }

        [SettingsValidatorAttribute("SWITCH", "FAULT_CUR_LOSS")]
        [Display(Name = "Fault Current before First/All Vltage Loss(es) :")]
        public System.Int64? FaultCurrentbeforeFirst_AllVltageLoss { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RECL_COUNT_TO_TRIP")]
        [Display(Name = "Recloser Counts to Sectionalizer Trip :")]
        public System.Int64? RecloserCountstoSectionalizerTrip { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SHOTS_REQ_LOCKOUT")]
        [Display(Name = "Number of Shots Required for Lockout :")]
        public System.Int64? NumberofShotsRequiredforLockout { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SHOTS_TO_LOCKOUT_TIME")]
        [Display(Name = "Shots-To-Lockout Time Threshold (secs) :")]
        public System.Int64? Shots_To_LockoutTimeThreshold { get; set; }

        [SettingsValidatorAttribute("SWITCH", "OVERC_SHOTS_TO_LO_OPER")]
        [Display(Name = "Overcurrent Required before Shots-To-LO Oper. :")]
        public System.Int64? OvercurrentRequiredbeforeShots_To_LOOper { get; set; }

        [SettingsValidatorAttribute("SWITCH", "VOLT_LOSS_THRESH")]
        [Display(Name = "Voltage Loss Threshoold (RMS Volts) :")]
        public System.Int64? VoltageLossThreshooldRMSVolts { get; set; }

        [SettingsValidatorAttribute("SWITCH", "TIME_THRESH")]
        [Display(Name = "Time Threshold (seconds) :")]
        public System.Int64? TimeThreshold { get; set; }

        [SettingsValidatorAttribute("SWITCH", "CURR_THRESH")]
        [Display(Name = "Current Threshold (amps, both feeds) :")]
        public System.Int64? CurrentThreshold { get; set; }

        [SettingsValidatorAttribute("SWITCH", "AUTO_RECLOSE_TIME")]
        [Display(Name = "Automatic Reclose Time Threshold :")]
        public System.Int64? AutomaticRecloseTimeThreshold { get; set; }

        [SettingsValidatorAttribute("SWITCH", "ATS_PREFERRED_FEED")]
        [Display(Name = "ATS preferred feed :")]
        public System.String ATSpreferredfeed { get; set; }

        [SettingsValidatorAttribute("SWITCH", "ATS_ALTERNATE_FEED")]
        [Display(Name = "ATS alternate feed :")]
        public System.String ATSalternatefeed { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SELECT_PREFERRED")]
        [Display(Name = "Select preferred :")]
        public System.String Selectpreferred { get; set; }

        [SettingsValidatorAttribute("SWITCH", "UNBALANCE_DETECT")]
        [Display(Name = "Unbalance detect :")]
        public System.String Unbalancedetect { get; set; }

        [SettingsValidatorAttribute("SWITCH", "UNBALANCE_DETECT_VOLT")]
        [Display(Name = "Unbalance detect Volt:")]
        public System.Int64? UnbalanceDetectVolt { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SELECT_RETURN")]
        [Display(Name = "Select return :")]
        public System.String Selectreturn { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SELECT_TRANSACTION")]
        [Display(Name = "Select Transition :")]
        public System.String SelectTransition { get; set; }

        [SettingsValidatorAttribute("SWITCH", "DWELL_TIMER")]
        [Display(Name = "Dwell Timer :")]
        public System.String DwellTimer { get; set; }

        [SettingsValidatorAttribute("SWITCH", "NORMALIZE_LEFT")]
        [Display(Name = "Normalize Left :")]
        public System.Int64? NormalizeLeft { get; set; }

        [SettingsValidatorAttribute("SWITCH", "NORMALIZE_RIGHT")]
        [Display(Name = "Normalize Right :")]
        public System.Int64? NormalizeRight { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SET_BASE_LEFT")]
        [Display(Name = "Set Base Left :")]
        public System.Int64? SetBaseLeft { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SET_BASE_RIGHT")]
        [Display(Name = "Set Base Right :")]
        public System.Int64? SetBaseRight { get; set; }

        [SettingsValidatorAttribute("SWITCH", "ACCESS_CODE")]
        [Display(Name = "Access Code :")]
        public System.String AccessCode { get; set; }

        [SettingsValidatorAttribute("SWITCH", "COMM_O_BIT_RATE")]
        [Display(Name = "Comm O bit rate :")]
        public System.Int64? CommObitrate { get; set; }

        [SettingsValidatorAttribute("SWITCH", "LOCKOUT_LEVEL")]
        [Display(Name = "Lockout Level :")]
        public System.Int64? LockoutLevel { get; set; }

        [SettingsValidatorAttribute("SWITCH", "LOSS_OF_SOURCE")]
        [Display(Name = "Loss of Source :")]
        public System.Int64? LossofSource { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RETURN_TO_SOURCE_TIME")]
        [Display(Name = "Return to Source Time :")]
        public System.Int64? ReturntoSourceTime { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RETURN_TO_SOURCE_VOLT")]
        [Display(Name = "Return to Source Volt :")]
        public System.Int64? ReturntoSourceVolt { get; set; }

        [SettingsValidatorAttribute("SWITCH", "OVERVOLT_DETECT")]
        [Display(Name = "Overvolt detect :")]
        public System.Int64? Overvoltdetect { get; set; }

        [SettingsValidatorAttribute("SWITCH", "LOSS_OF_LEFT_SOURCE")]
        [Display(Name = "Loss of Left source :")]
        public System.Decimal? LossofLeftsource { get; set; }

        [SettingsValidatorAttribute("SWITCH", "LOSS_OF_RIGHT_SOURCE")]
        [Display(Name = "Loss of Right source :")]
        public System.Decimal? LossofRightsource { get; set; }

        [SettingsValidatorAttribute("SWITCH", "LOCKOUT_RESET")]
        [Display(Name = "Lockout reset :")]
        public System.Decimal? Lockoutreset { get; set; }

        [SettingsValidatorAttribute("SWITCH", "OC_LOCKOUT_PICKUP")]
        [Display(Name = "OC lockout pickup :")]
        public System.Int64? OClockoutpickup { get; set; }

        [SettingsValidatorAttribute("SWITCH", "TRANSITION_DWELL")]
        [Display(Name = "Transition Dwell :")]
        public System.Decimal? TransitionDwell { get; set; }

        [SettingsValidatorAttribute("SWITCH", "WINDOW_BEGIN")]
        [Display(Name = "Window Begin :")]
        public System.String WindowBegin { get; set; }

        [SettingsValidatorAttribute("SWITCH", "WINDOW_LENGTH")]
        [Display(Name = "Window length (24 Hr) :")]
        public System.String Windowlength { get; set; }

        [SettingsValidatorAttribute("SWITCH", "FLISR")]
        [Display(Name = "FLISR Automation Device :")]
        public System.String FLISRAutomationDevice { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SUMMER_LOAD_LIMIT")]
        [Display(Name = "Summer Load Limit (amps) :")]
        public System.Int64? SummerLoadLimit { get; set; }

        [SettingsValidatorAttribute("SWITCH", "WINTER_LOAD_LIMIT")]
        [Display(Name = "Winter Load Limit (amps) :")]
        public System.Int64? WinterLoadLimit { get; set; }

        [SettingsValidatorAttribute("SWITCH", "LIMITING_FACTOR")]
        [Display(Name = "Limiting Factor :")]
        public System.String LimitingFactor { get; set; }

        [SettingsValidatorAttribute("SWITCH", "FLISR_ENGINEERING_COMMENTS")]
        [Display(Name = "FLISR Comments :")]
        public System.String FLISREngineeringComments { get; set; }

        [SettingsValidatorAttribute("SWITCH", "OPERATING_MODE")]
        [Display(Name = "Operating As  :")]
        public System.String OperatingMode { get; set; }

        [SettingsValidatorAttribute("SWITCH", "FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String FirmwareVersion { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String SoftwareVersion { get; set; }

        [SettingsValidatorAttribute("SWITCH", "ENGINEERING_DOCUMENT")]
        [Display(Name = "Engineering Document :")]
        public System.String EngineeringDocument { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SCADA")]
        [Display(Name = "SCADA :")]
        public System.String SCADA { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SCADA_TYPE")]
        [Display(Name = "SCADA Type :")]
        public System.String SCADAType { get; set; }

        [SettingsValidatorAttribute("SWITCH", "MASTER_STATION")]
        [Display(Name = "Master Station :")]
        public System.String MasterStation { get; set; }

        [SettingsValidatorAttribute("SWITCH", "BAUD_RATE")]
        [Display(Name = "Baud Rate :")]
        public System.Int32? BaudRate { get; set; }

        [SettingsValidatorAttribute("SWITCH", "TRANSMIT_ENABLE_DELAY")]
        [Display(Name = "Transmit Enable Delay :")]
        public System.Int32? TransmitEnableDelay { get; set; }

        [SettingsValidatorAttribute("SWITCH", "TRANSMIT_DISABLE_DELAY")]
        [Display(Name = "Transmit Disable Delay :")]
        public System.Int32? TransmitDisableDelay { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RTU_ADDRESS")]
        [Display(Name = "RTU Address :")]
        public System.String RTUAddress { get; set; }

        [SettingsValidatorAttribute("SWITCH", "REPEATER")]
        [Display(Name = "(if applicable) Repeater :")]
        public System.String Repeater { get; set; }

        [SettingsValidatorAttribute("SWITCH", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special conditions :")]
        public System.String Specialconditions { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RADIO_MANF_CD")]
        [Display(Name = "SCADA radio manufacturer :")]
        public System.String SCADAradiomanufacturer { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RADIO_MODEL_NUM")]
        [Display(Name = "SCADA radio model # :")]
        public System.String SCADAradiomodelNo { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RADIO_SERIAL_NUM")]
        [Display(Name = "SCADA radio serial # :")]
        public System.String SCADAradioserialNo { get; set; }

        /*New fields Settingsgaps*/
        [SettingsValidatorAttribute("SWITCH", "ENGINEERING_COMMENTS")]
        [Display(Name = "Engineering Comments :")]
        public System.String EngineeringComments { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RTU_EXISTS")]
        [Display(Name = "External RTU :")]
        public System.String RTUExists { get; set; }


        [SettingsValidatorAttribute("SWITCH", "RTU_MANF_CD")]
        [Display(Name = "RTU Manufacturer :")]
        public System.String RTUManufacture { get; set; }


        [SettingsValidatorAttribute("SWITCH", "RTU_MODEL_NUM")]
        [Display(Name = "RTU Model # :")]
        public System.String RTUModelNumber { get; set; }


        [SettingsValidatorAttribute("SWITCH", "RTU_SERIAL_NUM")]
        [Display(Name = "RTU Serial # :")]
        public System.String RTUSerialNumber { get; set; }


        [SettingsValidatorAttribute("SWITCH", "RTU_SOFTWARE_VERSION")]
        [Display(Name = "RTU Software version :")]
        public System.String RTUSoftwareVersion { get; set; }


        [SettingsValidatorAttribute("SWITCH", "RTU_FIRMWARE_VERSION")]
        [Display(Name = "RTU Firmware version :")]
        public System.String RTUFirmwareVersion { get; set; }

        public SelectList RTUManufactureList { get; set; }

        /* End of changes*/
        public bool Release { get; set; }

        public SelectList OkToBypassList { get; set; }
        public SelectList FLISRAutomationDeviceList { get; set; }

        public SelectList AtsFeatureList { get; set; }

        public SelectList ATSCapableList { get; set; }

        public SelectList ScadaTypeList { get; set; }

        public SelectList UnbalanceDetectList { get; set; }

        public SelectList DwellTimerList { get; set; }

        public SelectList SelectReturnList { get; set; }

        public SelectList EngineeringDocumentList { get; set; }

        public SelectList SelectPreferredList { get; set; }

        public SelectList SelectTransitionList { get; set; }

        public SelectList OperatingModeList { get; set; }

        public SelectList SCADARadioManufacturerList { get; set; }

        public SelectList SwitchTypeList { get; set; }

        public SelectList ControlUnitTypeList { get; set; }

        public SelectList SectionalizingFeatureList { get; set; }

        public List<GISAttributes> GISAttributes { get; set; }

        public string DropDownPostbackScript { get; set; }
        public string DropDownPostbackScriptScada { get; set; }

        public HashSet<string> FieldsToDisplay { get; set; }

        public void PopulateEntityFromModel(SM_SWITCH e)
        {
            e.PREPARED_BY = this.PreparedBy;
            e.CONTROL_SERIAL_NUM = this.ControllerSerialNo;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.Datemodified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            //e.NOTES = this.Notes;
            e.OK_TO_BYPASS = this.OkToBypass;
            e.CONTROL_UNIT_TYPE = this.ControlUnitType;
            e.SWITCH_TYPE = this.SwitchType;
            e.SECTIONALIZING_FEATURE = this.SectionalizingFeature;
            e.ATS_CAPABLE = this.ATSCapable;
            e.ATS_FEATURE = this.ATSFeature;
            e.PHA_FAULT_CUR_LEVEL = this.PhaseFaultDPhaseFaultDetectionCurrentLevelRMSAmp;
            e.GRD_FAULT_CUR_LEVEL = this.GroundFaultDetectionCurrentLevelRMSAmps;
            e.PHA_FAULT_DURATION = this.PhaseFaultDurationTimeThresholdMsecs;
            e.GRD_FAULT_DURATION = this.GroundFaultDurationTimeThresholdMsecs;
            e.PHA_INRUSH_TIME = this.PhaseCurrentInrushRestraintTimeMsecs;
            e.GRD_INRUSH_TIME = this.GroundCurrentInrushRestraintTimeMsecs;
            e.PHA_INRUSH_MULT = this.PhaseCurrentInrushRestraintMultiplier;
            e.GRD_INRUSH_MULT = this.GroundCurrentInrushRestraintMultiplier;
            e.SECT_RESET_TIME = this.SectionalizerResetTime;
            e.RECLOSE_RESET_TIME = this.SuccessfulRecloseResetTime;
            e.OC_TO_VOLT_TIME = this.OCtoVoltLossAssociationTime;
            e.FAULT_CUR_LOSS = this.FaultCurrentbeforeFirst_AllVltageLoss;
            e.RECL_COUNT_TO_TRIP = this.RecloserCountstoSectionalizerTrip;
            e.SHOTS_REQ_LOCKOUT = this.NumberofShotsRequiredforLockout;
            e.SHOTS_TO_LOCKOUT_TIME = this.Shots_To_LockoutTimeThreshold;
            e.OVERC_SHOTS_TO_LO_OPER = this.OvercurrentRequiredbeforeShots_To_LOOper;
            e.VOLT_LOSS_THRESH = this.VoltageLossThreshooldRMSVolts;
            e.TIME_THRESH = this.TimeThreshold;
            e.CURR_THRESH = this.CurrentThreshold;
            e.AUTO_RECLOSE_TIME = this.AutomaticRecloseTimeThreshold;
            e.ATS_PREFERRED_FEED = this.ATSpreferredfeed;
            e.ATS_ALTERNATE_FEED = this.ATSalternatefeed;
            e.SELECT_PREFERRED = this.Selectpreferred;
            e.UNBALANCE_DETECT = this.Unbalancedetect;
            e.UNBALANCE_DETECT_VOLT = this.UnbalanceDetectVolt;
            e.SELECT_RETURN = this.Selectreturn;
            e.SELECT_TRANSACTION = this.SelectTransition;
            e.DWELL_TIMER = this.DwellTimer;
            e.NORMALIZE_LEFT = this.NormalizeLeft;
            e.NORMALIZE_RIGHT = this.NormalizeRight;
            e.SET_BASE_LEFT = this.SetBaseLeft;
            e.SET_BASE_RIGHT = this.SetBaseRight;
            e.ACCESS_CODE = this.AccessCode;
            e.COMM_O_BIT_RATE = this.CommObitrate;
            e.LOCKOUT_LEVEL = this.LockoutLevel;
            e.LOSS_OF_SOURCE = this.LossofSource;
            e.RETURN_TO_SOURCE_TIME = this.ReturntoSourceTime;
            e.RETURN_TO_SOURCE_VOLT = this.ReturntoSourceVolt;
            e.OVERVOLT_DETECT = this.Overvoltdetect;
            e.LOSS_OF_LEFT_SOURCE = this.LossofLeftsource;
            e.LOSS_OF_RIGHT_SOURCE = this.LossofRightsource;
            e.LOCKOUT_RESET = this.Lockoutreset;
            e.OC_LOCKOUT_PICKUP = this.OClockoutpickup;
            e.TRANSITION_DWELL = this.TransitionDwell;
            e.WINDOW_BEGIN = this.WindowBegin;
            e.WINDOW_LENGTH = this.Windowlength;
            e.FLISR = this.FLISRAutomationDevice;
            e.SUMMER_LOAD_LIMIT = this.SummerLoadLimit;
            e.WINTER_LOAD_LIMIT = this.WinterLoadLimit;
            e.LIMITING_FACTOR = this.LimitingFactor;
            e.FLISR_ENGINEERING_COMMENTS = this.FLISREngineeringComments;
            e.OPERATING_MODE = this.OperatingMode;
            e.FIRMWARE_VERSION = this.FirmwareVersion;
            e.SOFTWARE_VERSION = this.SoftwareVersion;
            //e.ENGINEERING_DOCUMENT = this.EngineeringDocument;
            e.SCADA = this.SCADA;
            e.SCADA_TYPE = this.SCADAType;
            e.MASTER_STATION = this.MasterStation;
            e.BAUD_RATE = this.BaudRate;
            e.TRANSMIT_ENABLE_DELAY = this.TransmitEnableDelay;
            e.TRANSMIT_DISABLE_DELAY = this.TransmitDisableDelay;
            e.RTU_ADDRESS = this.RTUAddress;
            e.REPEATER = this.Repeater;
            e.SPECIAL_CONDITIONS = this.Specialconditions;
            e.RADIO_MANF_CD = this.SCADAradiomanufacturer;
            e.RADIO_MODEL_NUM = this.SCADAradiomodelNo;
            e.RADIO_SERIAL_NUM = this.SCADAradioserialNo;
            e.RTU_EXIST = this.RTUExists;
            e.RTU_MANF_CD = this.RTUManufacture;
            e.RTU_FIRMWARE_VERSION = this.RTUFirmwareVersion;
            e.RTU_MODEL_NUM = this.RTUModelNumber;
            e.RTU_SERIAL_NUM = this.RTUSerialNumber;
            e.RTU_SOFTWARE_VERSION = this.RTUSoftwareVersion;
            e.ENGINEERING_COMMENTS = this.EngineeringComments;

        }

        public void PopulateModelFromEntity(SM_SWITCH e)
        {
            this.DeviceID = e.DEVICE_ID;
            this.OperatingNumber = e.OPERATING_NUM;
            this.PreparedBy = e.PREPARED_BY;
            this.ControllerSerialNo = e.CONTROL_SERIAL_NUM;
            this.Datemodified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            //this.Notes = e.NOTES;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.ControlUnitType = e.CONTROL_UNIT_TYPE;
            this.SwitchType = e.SWITCH_TYPE;
            this.SectionalizingFeature = e.SECTIONALIZING_FEATURE;
            this.ATSCapable = e.ATS_CAPABLE;
            this.ATSFeature = e.ATS_FEATURE;
            this.PhaseFaultDPhaseFaultDetectionCurrentLevelRMSAmp = e.PHA_FAULT_CUR_LEVEL;
            this.GroundFaultDetectionCurrentLevelRMSAmps = e.GRD_FAULT_CUR_LEVEL;
            this.PhaseFaultDurationTimeThresholdMsecs = e.PHA_FAULT_DURATION;
            this.GroundFaultDurationTimeThresholdMsecs = e.GRD_FAULT_DURATION;
            this.PhaseCurrentInrushRestraintTimeMsecs = e.PHA_INRUSH_TIME;
            this.GroundCurrentInrushRestraintTimeMsecs = e.GRD_INRUSH_TIME;
            this.PhaseCurrentInrushRestraintMultiplier = e.PHA_INRUSH_MULT;
            this.GroundCurrentInrushRestraintMultiplier = e.GRD_INRUSH_MULT;
            this.SectionalizerResetTime = e.SECT_RESET_TIME;
            this.SuccessfulRecloseResetTime = e.RECLOSE_RESET_TIME;
            this.OCtoVoltLossAssociationTime = e.OC_TO_VOLT_TIME;
            this.FaultCurrentbeforeFirst_AllVltageLoss = e.FAULT_CUR_LOSS;
            this.RecloserCountstoSectionalizerTrip = e.RECL_COUNT_TO_TRIP;
            this.NumberofShotsRequiredforLockout = e.SHOTS_REQ_LOCKOUT;
            this.Shots_To_LockoutTimeThreshold = e.SHOTS_TO_LOCKOUT_TIME;
            this.OvercurrentRequiredbeforeShots_To_LOOper = e.OVERC_SHOTS_TO_LO_OPER;
            this.VoltageLossThreshooldRMSVolts = e.VOLT_LOSS_THRESH;
            this.TimeThreshold = e.TIME_THRESH;
            this.CurrentThreshold = e.CURR_THRESH;
            this.AutomaticRecloseTimeThreshold = e.AUTO_RECLOSE_TIME;
            this.ATSpreferredfeed = e.ATS_PREFERRED_FEED;
            this.ATSalternatefeed = e.ATS_ALTERNATE_FEED;
            this.Selectpreferred = e.SELECT_PREFERRED;
            this.Unbalancedetect = e.UNBALANCE_DETECT;
            this.UnbalanceDetectVolt = e.UNBALANCE_DETECT_VOLT;
            this.Selectreturn = e.SELECT_RETURN;
            this.SelectTransition = e.SELECT_TRANSACTION;
            this.DwellTimer = e.DWELL_TIMER;
            this.NormalizeLeft = e.NORMALIZE_LEFT;
            this.NormalizeRight = e.NORMALIZE_RIGHT;
            this.SetBaseLeft = e.SET_BASE_LEFT;
            this.SetBaseRight = e.SET_BASE_RIGHT;
            this.AccessCode = e.ACCESS_CODE;
            this.CommObitrate = e.COMM_O_BIT_RATE;
            this.LockoutLevel = e.LOCKOUT_LEVEL;
            this.LossofSource = e.LOSS_OF_SOURCE;
            this.ReturntoSourceTime = e.RETURN_TO_SOURCE_TIME;
            this.ReturntoSourceVolt = e.RETURN_TO_SOURCE_VOLT;
            this.Overvoltdetect = e.OVERVOLT_DETECT;
            this.LossofLeftsource = e.LOSS_OF_LEFT_SOURCE;
            this.LossofRightsource = e.LOSS_OF_RIGHT_SOURCE;
            this.Lockoutreset = e.LOCKOUT_RESET;
            this.OClockoutpickup = e.OC_LOCKOUT_PICKUP;
            this.TransitionDwell = e.TRANSITION_DWELL;
            this.WindowBegin = e.WINDOW_BEGIN;
            this.Windowlength = e.WINDOW_LENGTH;
            this.FLISRAutomationDevice = e.FLISR;
            this.SummerLoadLimit = e.SUMMER_LOAD_LIMIT;
            this.WinterLoadLimit = e.WINTER_LOAD_LIMIT;
            this.LimitingFactor = e.LIMITING_FACTOR;
            this.FLISREngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            this.OperatingMode = e.OPERATING_MODE;
            this.FirmwareVersion = e.FIRMWARE_VERSION;
            this.SoftwareVersion = e.SOFTWARE_VERSION;
            //this.EngineeringDocument = e.ENGINEERING_DOCUMENT;
            this.SCADA = e.SCADA;
            this.SCADAType = e.SCADA_TYPE;
            this.MasterStation = e.MASTER_STATION;
            this.BaudRate = e.BAUD_RATE;
            this.TransmitEnableDelay = e.TRANSMIT_ENABLE_DELAY;
            this.TransmitDisableDelay = e.TRANSMIT_DISABLE_DELAY;
            this.RTUAddress = e.RTU_ADDRESS;
            this.Repeater = e.REPEATER;
            this.Specialconditions = e.SPECIAL_CONDITIONS;
            this.SCADAradiomanufacturer = e.RADIO_MANF_CD;
            this.SCADAradiomodelNo = e.RADIO_MODEL_NUM;
            this.SCADAradioserialNo = e.RADIO_SERIAL_NUM;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
        }

        public void PopulateModelFromHistoryEntity(SM_SWITCH_HIST e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.AccessCode = e.ACCESS_CODE;
            this.ATSalternatefeed = e.ATS_ALTERNATE_FEED;
            this.ATSCapable = e.ATS_CAPABLE;
            this.ATSFeature = e.ATS_FEATURE;
            this.ATSpreferredfeed = e.ATS_PREFERRED_FEED;
            this.AutomaticRecloseTimeThreshold = e.AUTO_RECLOSE_TIME;
            this.BaudRate = e.BAUD_RATE;
            this.CommObitrate = e.COMM_O_BIT_RATE;
            this.ControllerSerialNo = e.CONTROL_SERIAL_NUM;
            this.ControlUnitType = e.CONTROL_UNIT_TYPE;
            this.CurrentThreshold = e.CURR_THRESH;
            this.Datemodified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.DeviceID = e.DEVICE_ID;
            this.DwellTimer = e.DWELL_TIMER;
            this.FLISREngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            //this.EngineeringDocument = e.ENGINEERING_DOCUMENT;
            this.FaultCurrentbeforeFirst_AllVltageLoss = e.FAULT_CUR_LOSS;
            this.FirmwareVersion = e.FIRMWARE_VERSION;
            this.FLISRAutomationDevice = e.FLISR;
            this.GroundCurrentInrushRestraintMultiplier = e.GRD_INRUSH_MULT;
            this.GroundCurrentInrushRestraintTimeMsecs = e.GRD_INRUSH_TIME;
            this.GroundFaultDetectionCurrentLevelRMSAmps = e.GRD_FAULT_CUR_LEVEL;
            this.GroundFaultDurationTimeThresholdMsecs = e.GRD_FAULT_DURATION;
            this.LimitingFactor = e.LIMITING_FACTOR;
            this.LockoutLevel = e.LOCKOUT_LEVEL;
            this.Lockoutreset = e.LOCKOUT_RESET;
            this.LossofLeftsource = e.LOSS_OF_LEFT_SOURCE;
            this.LossofRightsource = e.LOSS_OF_RIGHT_SOURCE;
            this.LossofSource = e.LOSS_OF_SOURCE;
            this.MasterStation = e.MASTER_STATION;
            this.NormalizeLeft = e.NORMALIZE_LEFT;
            this.NormalizeRight = e.NORMALIZE_RIGHT;
            //this.Notes = e.NOTES;
            this.NumberofShotsRequiredforLockout = e.SHOTS_REQ_LOCKOUT;
            this.OClockoutpickup = e.OC_LOCKOUT_PICKUP;
            this.OCtoVoltLossAssociationTime = e.OC_TO_VOLT_TIME;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.OperatingMode = e.OPERATING_MODE;
            this.OvercurrentRequiredbeforeShots_To_LOOper = e.OVERC_SHOTS_TO_LO_OPER;
            this.Overvoltdetect = e.OVERVOLT_DETECT;
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.PhaseCurrentInrushRestraintMultiplier = e.PHA_INRUSH_MULT;
            this.PhaseCurrentInrushRestraintTimeMsecs = e.PHA_INRUSH_TIME;
            this.PhaseFaultDPhaseFaultDetectionCurrentLevelRMSAmp = e.PHA_FAULT_CUR_LEVEL;
            this.PhaseFaultDurationTimeThresholdMsecs = e.PHA_FAULT_DURATION;
            this.PreparedBy = e.PREPARED_BY;
            this.RecloserCountstoSectionalizerTrip = e.RECL_COUNT_TO_TRIP;
            this.Repeater = e.REPEATER;
            this.ReturntoSourceTime = e.RETURN_TO_SOURCE_TIME;
            this.ReturntoSourceVolt = e.RETURN_TO_SOURCE_VOLT;
            this.RTUAddress = e.RTU_ADDRESS;
            this.SCADA = e.SCADA;
            this.SCADAradiomanufacturer = e.RADIO_MANF_CD;
            this.SCADAradiomodelNo = e.RADIO_MODEL_NUM;
            this.SCADAradioserialNo = e.RADIO_SERIAL_NUM;
            this.SCADAType = e.SCADA_TYPE;
            this.SectionalizerResetTime = e.SECT_RESET_TIME;
            this.SectionalizingFeature = e.SECTIONALIZING_FEATURE;
            this.Selectpreferred = e.SELECT_PREFERRED;
            this.Selectreturn = e.SELECT_RETURN;
            this.SelectTransition = e.SELECT_TRANSACTION;
            this.SetBaseLeft = e.SET_BASE_LEFT;
            this.SetBaseRight = e.SET_BASE_RIGHT;
            this.Shots_To_LockoutTimeThreshold = e.SHOTS_TO_LOCKOUT_TIME;
            this.SoftwareVersion = e.SOFTWARE_VERSION;
            this.Specialconditions = e.SPECIAL_CONDITIONS;
            this.SuccessfulRecloseResetTime = e.RECLOSE_RESET_TIME;
            this.SummerLoadLimit = e.SUMMER_LOAD_LIMIT;
            this.SwitchType = e.SWITCH_TYPE;
            this.TimeThreshold = e.TIME_THRESH;
            this.TransitionDwell = e.TRANSITION_DWELL;
            this.TransmitDisableDelay = e.TRANSMIT_DISABLE_DELAY;
            this.TransmitEnableDelay = e.TRANSMIT_ENABLE_DELAY;
            this.Unbalancedetect = e.UNBALANCE_DETECT;
            this.UnbalanceDetectVolt = e.UNBALANCE_DETECT_VOLT;
            this.VoltageLossThreshooldRMSVolts = e.VOLT_LOSS_THRESH;
            this.WindowBegin = e.WINDOW_BEGIN;
            this.Windowlength = e.WINDOW_LENGTH;
            this.WinterLoadLimit = e.WINTER_LOAD_LIMIT;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
        }

        public void PopulateHistoryFromEntity(SM_SWITCH_HIST entityHistory, SM_SWITCH e)
        {
            entityHistory.OPERATING_NUM = e.OPERATING_NUM;
            entityHistory.ACCESS_CODE = e.ACCESS_CODE;
            entityHistory.ATS_ALTERNATE_FEED = e.ATS_ALTERNATE_FEED;
            entityHistory.ATS_CAPABLE = e.ATS_CAPABLE;
            entityHistory.ATS_FEATURE = e.ATS_FEATURE;
            entityHistory.ATS_PREFERRED_FEED = e.ATS_PREFERRED_FEED;
            entityHistory.AUTO_RECLOSE_TIME = e.AUTO_RECLOSE_TIME;
            entityHistory.BAUD_RATE = e.BAUD_RATE;
            entityHistory.COMM_O_BIT_RATE = e.COMM_O_BIT_RATE;
            entityHistory.CONTROL_SERIAL_NUM = e.CONTROL_SERIAL_NUM;
            entityHistory.CONTROL_UNIT_TYPE = e.CONTROL_UNIT_TYPE;
            entityHistory.CURR_THRESH = e.CURR_THRESH;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.DEVICE_ID = e.DEVICE_ID;
            entityHistory.DWELL_TIMER = e.DWELL_TIMER;
            entityHistory.EFFECTIVE_DT = e.EFFECTIVE_DT;
            entityHistory.FLISR_ENGINEERING_COMMENTS = e.FLISR_ENGINEERING_COMMENTS;
            //entityHistory.ENGINEERING_DOCUMENT = e.ENGINEERING_DOCUMENT;
            entityHistory.FAULT_CUR_LOSS = e.FAULT_CUR_LOSS;
            entityHistory.FIRMWARE_VERSION = e.FIRMWARE_VERSION;
            entityHistory.FLISR = e.FLISR;
            entityHistory.GRD_INRUSH_MULT = e.GRD_INRUSH_MULT;
            entityHistory.GRD_INRUSH_TIME = e.GRD_INRUSH_TIME;
            entityHistory.GRD_FAULT_CUR_LEVEL = e.GRD_FAULT_CUR_LEVEL;
            entityHistory.GRD_FAULT_DURATION = e.GRD_FAULT_DURATION;
            entityHistory.LIMITING_FACTOR = e.LIMITING_FACTOR;
            entityHistory.LOCKOUT_LEVEL = e.LOCKOUT_LEVEL;
            entityHistory.LOCKOUT_RESET = e.LOCKOUT_RESET;
            entityHistory.LOSS_OF_LEFT_SOURCE = e.LOSS_OF_LEFT_SOURCE;
            entityHistory.LOSS_OF_RIGHT_SOURCE = e.LOSS_OF_RIGHT_SOURCE;
            entityHistory.LOSS_OF_SOURCE = e.LOSS_OF_SOURCE;
            entityHistory.MASTER_STATION = e.MASTER_STATION;
            entityHistory.NORMALIZE_LEFT = e.NORMALIZE_LEFT;
            entityHistory.NORMALIZE_RIGHT = e.NORMALIZE_RIGHT;
            //entityHistory.NOTES = e.NOTES;
            entityHistory.SHOTS_REQ_LOCKOUT = e.SHOTS_REQ_LOCKOUT;
            entityHistory.OC_LOCKOUT_PICKUP = e.OC_LOCKOUT_PICKUP;
            entityHistory.OC_TO_VOLT_TIME = e.OC_TO_VOLT_TIME;
            entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
            entityHistory.OPERATING_MODE = e.OPERATING_MODE;
            entityHistory.OVERC_SHOTS_TO_LO_OPER = e.OVERC_SHOTS_TO_LO_OPER;
            entityHistory.OVERVOLT_DETECT = e.OVERVOLT_DETECT;
            entityHistory.PEER_REVIEW_BY = e.PEER_REVIEW_BY;
            entityHistory.PEER_REVIEW_DT = e.PEER_REVIEW_DT;
            entityHistory.PHA_INRUSH_MULT = e.PHA_INRUSH_MULT;
            entityHistory.PHA_INRUSH_TIME = e.PHA_INRUSH_TIME;
            entityHistory.PHA_FAULT_CUR_LEVEL = e.PHA_FAULT_CUR_LEVEL;
            entityHistory.PHA_FAULT_DURATION = e.PHA_FAULT_DURATION;
            entityHistory.PREPARED_BY = e.PREPARED_BY;
            entityHistory.RECL_COUNT_TO_TRIP = e.RECL_COUNT_TO_TRIP;
            entityHistory.REPEATER = e.REPEATER;
            entityHistory.RETURN_TO_SOURCE_TIME = e.RETURN_TO_SOURCE_TIME;
            entityHistory.RETURN_TO_SOURCE_VOLT = e.RETURN_TO_SOURCE_VOLT;
            entityHistory.RTU_ADDRESS = e.RTU_ADDRESS;
            entityHistory.SCADA = e.SCADA;
            entityHistory.RADIO_MANF_CD = e.RADIO_MANF_CD;
            entityHistory.RADIO_MODEL_NUM = e.RADIO_MODEL_NUM;
            entityHistory.RADIO_SERIAL_NUM = e.RADIO_SERIAL_NUM;
            entityHistory.SCADA_TYPE = e.SCADA_TYPE;
            entityHistory.SECT_RESET_TIME = e.SECT_RESET_TIME;
            entityHistory.SECTIONALIZING_FEATURE = e.SECTIONALIZING_FEATURE;
            entityHistory.SELECT_PREFERRED = e.SELECT_PREFERRED;
            entityHistory.SELECT_RETURN = e.SELECT_RETURN;
            entityHistory.SELECT_TRANSACTION = e.SELECT_TRANSACTION;
            entityHistory.SET_BASE_LEFT = e.SET_BASE_LEFT;
            entityHistory.SET_BASE_RIGHT = e.SET_BASE_RIGHT;
            entityHistory.SHOTS_TO_LOCKOUT_TIME = e.SHOTS_TO_LOCKOUT_TIME;
            entityHistory.SOFTWARE_VERSION = e.SOFTWARE_VERSION;
            entityHistory.SPECIAL_CONDITIONS = e.SPECIAL_CONDITIONS;
            entityHistory.RECLOSE_RESET_TIME = e.RECLOSE_RESET_TIME;
            entityHistory.SUMMER_LOAD_LIMIT = e.SUMMER_LOAD_LIMIT;
            entityHistory.SWITCH_TYPE = e.SWITCH_TYPE;
            entityHistory.TIME_THRESH = e.TIME_THRESH;
            entityHistory.TRANSITION_DWELL = e.TRANSITION_DWELL;
            entityHistory.TRANSMIT_DISABLE_DELAY = e.TRANSMIT_DISABLE_DELAY;
            entityHistory.TRANSMIT_ENABLE_DELAY = e.TRANSMIT_ENABLE_DELAY;
            entityHistory.UNBALANCE_DETECT = e.UNBALANCE_DETECT;
            entityHistory.UNBALANCE_DETECT_VOLT = e.UNBALANCE_DETECT_VOLT;
            entityHistory.VOLT_LOSS_THRESH = e.VOLT_LOSS_THRESH;
            entityHistory.WINDOW_BEGIN = e.WINDOW_BEGIN;
            entityHistory.WINDOW_LENGTH = e.WINDOW_LENGTH;
            entityHistory.WINTER_LOAD_LIMIT = e.WINTER_LOAD_LIMIT;
            entityHistory.RTU_EXIST = e.RTU_EXIST;
            entityHistory.RTU_MANF_CD = e.RTU_MANF_CD;
            entityHistory.RTU_FIRMWARE_VERSION = e.RTU_FIRMWARE_VERSION;
            entityHistory.RTU_MODEL_NUM = e.RTU_MODEL_NUM;
            entityHistory.RTU_SERIAL_NUM = e.RTU_SERIAL_NUM;
            entityHistory.RTU_SOFTWARE_VERSION = e.RTU_SOFTWARE_VERSION;
            entityHistory.ENGINEERING_COMMENTS = e.ENGINEERING_COMMENTS;

        }
    }
}
