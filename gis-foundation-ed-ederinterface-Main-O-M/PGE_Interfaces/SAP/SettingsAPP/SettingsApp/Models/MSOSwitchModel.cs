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
    public partial class MSOSwitchModel
    {

        [SettingsValidatorAttribute("MSOSWITCH", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceID { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String ControllerSerialNo { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String Datemodified { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        //[SettingsValidatorAttribute("SWITCH", "NOTES")]
        //[Display(Name = "Notes :")]
        //public System.String Notes { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "CONTROL_UNIT_TYPE")]
        [Display(Name = "Control Unit Type :")]
        public System.String ControlUnitType { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "SWITCH_TYPE")]
        [Display(Name = "Switch Type :")]
        public System.String SwitchType { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "SECTIONALIZING_FEATURE")]
        [Display(Name = "Sectionalizing Feature :")]
        public System.String SectionalizingFeature { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "ATS_CAPABLE")]
        [Display(Name = "ATS Capable :")]
        public System.String ATSCapable { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "ATS_FEATURE")]
        [Display(Name = "ATS Feature :")]
        public System.String ATSFeature { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "CURRENT_RATIO_CTR")]
        [Display(Name = "Current Ratio_CTR :")]
        public System.Int64? CurrentRatioCTR { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "VOLTAGE_RATIO_VTR")]
        [Display(Name = "Voltage Ratio_VTR :")]
        public System.Int64? VoltageRatioVTR { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "PHASE_INST_50P")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Phase Instantaneous_(50P) can't have more than 1 decimal places")]
        [Display(Name = "Phase Instantaneous_(50P) :")]
        public System.Decimal? PhaseInstantaneous50P { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "PHASE_TIME_DELAY")]
        [RegularExpression(@"^-?\d+(.\d{0,2})?$", ErrorMessage = "Time Delay(Sec.) can't have more than 2 decimal places")]
        [Display(Name = "Time Delay(Sec.) :")]
        public System.Decimal? PhaseTimeDelay { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "RESIDUAL_GR_INST_50G")]
        [RegularExpression(@"^-?\d+(.\d{0,1})?$", ErrorMessage = "Residual (Ground) Instantaneous_(50G) can't have more than 1 decimal places")]
        [Display(Name = "Residual (Ground) Instantaneous_(50G) :")]
        public System.Decimal? ResidualGroundInstantaneous50G { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "GROUND_TIME_DELAY")]
        [RegularExpression(@"^-?\d+(.\d{0,2})?$", ErrorMessage = "Time Delay(Sec.)  can't have more than 2 decimal places")]
        [Display(Name = "Time Delay(Sec.) :")]
        public System.Decimal? GroundTimeDelay { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "SHOT_TO_LOCKOUT_SECT")]
        [Display(Name = "Shot to Lockout (Sectionalizer) :")]
        public System.Int64? ShotToLockoutSectionalizer { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "RESET_TIME")]
        [Display(Name = "Reset Time(Sec.) :")]
        public System.Int64? ResetTime { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "PHASE_A_SCADA_IDENTI_A")]
        [Display(Name = "Phase A_SCADA Identification (A) :")]
        public System.Int64? PhaseASCADAIdentificationA { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "PHASE_B_SCADA_IDENTI_A")]
        [Display(Name = "Phase B_SCADA Identification (A) :")]
        public System.Int64? PhaseBSCADAIdentificationA { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "PHASE_C_SCADA_IDENTI_A")]
        [Display(Name = "Phase C_SCADA Identification (A) :")]
        public System.Int64? PhaseCSCADAIdentificationA { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "GROUND_SCADA_IDENTI_A")]
        [Display(Name = "Ground_SCADA Identification (A) :")]
        public System.Int64? GroundSCADAIdentificationA { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "FLISR")]
        [Display(Name = "FLISR Automation Device :")]
        public System.String FLISRAutomationDevice { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "SUMMER_LOAD_LIMIT")]
        [Display(Name = "Summer Load Limit (amps) :")]
        public System.Int64? SummerLoadLimit { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "WINTER_LOAD_LIMIT")]
        [Display(Name = "Winter Load Limit (amps) :")]
        public System.Int64? WinterLoadLimit { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "LIMITING_FACTOR")]
        [Display(Name = "Limiting Factor :")]
        public System.String LimitingFactor { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "FLISR_ENGINEERING_COMMENTS")]
        [Display(Name = "FLISR Comments :")]
        public System.String FLISREngineeringComments { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "OPERATING_MODE")]
        [Display(Name = "Operating As  :")]
        public System.String OperatingMode { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String FirmwareVersion { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String SoftwareVersion { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "ENGINEERING_DOCUMENT")]
        [Display(Name = "Engineering Document :")]
        public System.String EngineeringDocument { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "SCADA")]
        [Display(Name = "SCADA :")]
        public System.String SCADA { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "SCADA_TYPE")]
        [Display(Name = "SCADA Type :")]
        public System.String SCADAType { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "MASTER_STATION")]
        [Display(Name = "Master Station :")]
        public System.String MasterStation { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "BAUD_RATE")]
        [Display(Name = "Baud Rate :")]
        public System.Int32? BaudRate { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "TRANSMIT_ENABLE_DELAY")]
        [Display(Name = "Transmit Enable Delay :")]
        public System.Int32? TransmitEnableDelay { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "TRANSMIT_DISABLE_DELAY")]
        [Display(Name = "Transmit Disable Delay :")]
        public System.Int32? TransmitDisableDelay { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "RTU_ADDRESS")]
        [Display(Name = "RTU Address :")]
        public System.String RTUAddress { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "REPEATER")]
        [Display(Name = "(if applicable) Repeater :")]
        public System.String Repeater { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special conditions :")]
        public System.String Specialconditions { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "RADIO_MANF_CD")]
        [Display(Name = "SCADA radio manufacturer :")]
        public System.String SCADAradiomanufacturer { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "RADIO_MODEL_NUM")]
        [Display(Name = "SCADA radio model # :")]
        public System.String SCADAradiomodelNo { get; set; }

        [SettingsValidatorAttribute("MSOSWITCH", "RADIO_SERIAL_NUM")]
        [Display(Name = "SCADA radio serial # :")]
        public System.String SCADAradioserialNo { get; set; }

        [SettingsValidatorAttribute("SWITCH", "RTU_EXISTS")]
        [Display(Name = "External RTU :")]
        public System.String RTUExists { get; set; }


        [SettingsValidatorAttribute("MSOSWITCH", "RTU_MANF_CD")]
        [Display(Name = "RTU Manufacturer :")]
        public System.String RTUManufacture { get; set; }


        [SettingsValidatorAttribute("MSOSWITCH", "RTU_MODEL_NUM")]
        [Display(Name = "RTU Model # :")]
        public System.String RTUModelNumber { get; set; }


        [SettingsValidatorAttribute("MSOSWITCH", "RTU_SERIAL_NUM")]
        [Display(Name = "RTU Serial # :")]
        public System.String RTUSerialNumber { get; set; }


        [SettingsValidatorAttribute("MSOSWITCH", "RTU_SOFTWARE_VERSION")]
        [Display(Name = "RTU Software version :")]
        public System.String RTUSoftwareVersion { get; set; }


        [SettingsValidatorAttribute("MSOSWITCH", "RTU_FIRMWARE_VERSION")]
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

        public void PopulateEntityFromModel(SM_SWITCH_MSO e)
        {
            //added oct 2019 - start
            e.CURRENT_RATIO_CTR = this.CurrentRatioCTR;
            e.VOLTAGE_RATIO_VTR = this.VoltageRatioVTR;
            e.PHASE_INST_50P = this.PhaseInstantaneous50P;
            e.PHASE_TIME_DELAY = this.PhaseTimeDelay;
            e.RESIDUAL_GR_INST_50G = this.ResidualGroundInstantaneous50G;
            e.GROUND_TIME_DELAY = this.GroundTimeDelay;
            e.SHOT_TO_LOCKOUT_SECT = this.ShotToLockoutSectionalizer;
            e.RESET_TIME = this.ResetTime;
            e.PHASE_A_SCADA_IDENTI_A = this.PhaseASCADAIdentificationA;
            e.PHASE_B_SCADA_IDENTI_A = this.PhaseBSCADAIdentificationA;
            e.PHASE_C_SCADA_IDENTI_A = this.PhaseCSCADAIdentificationA;
            e.GROUND_SCADA_IDENTI_A = this.GroundSCADAIdentificationA;
            //added oct 2019 - end
            e.PREPARED_BY = this.PreparedBy;
            e.CONTROL_SERIAL_NUM = this.ControllerSerialNo;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.Datemodified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            e.OK_TO_BYPASS = this.OkToBypass;
            e.CONTROL_UNIT_TYPE = this.ControlUnitType;
            e.SWITCH_TYPE = this.SwitchType;
            e.SECTIONALIZING_FEATURE = this.SectionalizingFeature;
            e.ATS_CAPABLE = this.ATSCapable;
            e.ATS_FEATURE = this.ATSFeature;
            e.FLISR = this.FLISRAutomationDevice;
            e.SUMMER_LOAD_LIMIT = this.SummerLoadLimit;
            e.WINTER_LOAD_LIMIT = this.WinterLoadLimit;
            e.LIMITING_FACTOR = this.LimitingFactor;
            e.FLISR_ENGINEERING_COMMENTS = this.FLISREngineeringComments;
            e.OPERATING_MODE = this.OperatingMode;
            e.FIRMWARE_VERSION = this.FirmwareVersion;
            e.SOFTWARE_VERSION = this.SoftwareVersion;
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
        }

        public void PopulateModelFromEntity(SM_SWITCH_MSO e)
        {
            //added oct 2019 - start
            this.CurrentRatioCTR = e.CURRENT_RATIO_CTR;
            this.VoltageRatioVTR = e.VOLTAGE_RATIO_VTR;
            this.PhaseInstantaneous50P = e.PHASE_INST_50P;
            this.PhaseTimeDelay = e.PHASE_TIME_DELAY;
            this.ResidualGroundInstantaneous50G = e.RESIDUAL_GR_INST_50G;
            this.GroundTimeDelay = e.GROUND_TIME_DELAY;
            this.ShotToLockoutSectionalizer = e.SHOT_TO_LOCKOUT_SECT;
            this.ResetTime = e.RESET_TIME;
            this.PhaseASCADAIdentificationA = e.PHASE_A_SCADA_IDENTI_A;
            this.PhaseBSCADAIdentificationA = e.PHASE_B_SCADA_IDENTI_A;
            this.PhaseCSCADAIdentificationA = e.PHASE_C_SCADA_IDENTI_A;
            this.GroundSCADAIdentificationA = e.GROUND_SCADA_IDENTI_A;
            //added oct 2019 - end
            this.DeviceID = e.DEVICE_ID;
            this.OperatingNumber = e.OPERATING_NUM;
            this.PreparedBy = e.PREPARED_BY;
            this.ControllerSerialNo = e.CONTROL_SERIAL_NUM;
            this.Datemodified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.ControlUnitType = e.CONTROL_UNIT_TYPE;
            this.SwitchType = e.SWITCH_TYPE;
            this.SectionalizingFeature = e.SECTIONALIZING_FEATURE;
            this.ATSCapable = e.ATS_CAPABLE;
            this.ATSFeature = e.ATS_FEATURE;
            this.FLISRAutomationDevice = e.FLISR;
            this.SummerLoadLimit = e.SUMMER_LOAD_LIMIT;
            this.WinterLoadLimit = e.WINTER_LOAD_LIMIT;
            this.LimitingFactor = e.LIMITING_FACTOR;
            this.FLISREngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            this.OperatingMode = e.OPERATING_MODE;
            this.FirmwareVersion = e.FIRMWARE_VERSION;
            this.SoftwareVersion = e.SOFTWARE_VERSION;
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
        }

        public void PopulateModelFromHistoryEntity(SM_SWITCH_MSO_HIST e)
        {
            //added oct 2019 - start
            this.CurrentRatioCTR = e.CURRENT_RATIO_CTR;
            this.VoltageRatioVTR = e.VOLTAGE_RATIO_VTR;
            this.PhaseInstantaneous50P = e.PHASE_INST_50P;
            this.PhaseTimeDelay = e.PHASE_TIME_DELAY;
            this.ResidualGroundInstantaneous50G = e.RESIDUAL_GR_INST_50G;
            this.GroundTimeDelay = e.GROUND_TIME_DELAY;
            this.ShotToLockoutSectionalizer = e.SHOT_TO_LOCKOUT_SECT;
            this.ResetTime = e.RESET_TIME;
            this.PhaseASCADAIdentificationA = e.PHASE_A_SCADA_IDENTI_A;
            this.PhaseBSCADAIdentificationA = e.PHASE_B_SCADA_IDENTI_A;
            this.PhaseCSCADAIdentificationA = e.PHASE_C_SCADA_IDENTI_A;
            this.GroundSCADAIdentificationA = e.GROUND_SCADA_IDENTI_A;
            //added oct 2019 - end
            this.OperatingNumber = e.OPERATING_NUM;
            this.ATSCapable = e.ATS_CAPABLE;
            this.ATSFeature = e.ATS_FEATURE;
            this.BaudRate = e.BAUD_RATE;
            this.ControllerSerialNo = e.CONTROL_SERIAL_NUM;
            this.ControlUnitType = e.CONTROL_UNIT_TYPE;
            this.Datemodified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.DeviceID = e.DEVICE_ID;
            this.FLISREngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            this.FirmwareVersion = e.FIRMWARE_VERSION;
            this.FLISRAutomationDevice = e.FLISR;
            this.LimitingFactor = e.LIMITING_FACTOR;
            this.MasterStation = e.MASTER_STATION;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.OperatingMode = e.OPERATING_MODE;
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.PreparedBy = e.PREPARED_BY;
            this.Repeater = e.REPEATER;
            this.RTUAddress = e.RTU_ADDRESS;
            this.SCADA = e.SCADA;
            this.SCADAradiomanufacturer = e.RADIO_MANF_CD;
            this.SCADAradiomodelNo = e.RADIO_MODEL_NUM;
            this.SCADAradioserialNo = e.RADIO_SERIAL_NUM;
            this.SCADAType = e.SCADA_TYPE;
            this.SectionalizingFeature = e.SECTIONALIZING_FEATURE;
            this.SoftwareVersion = e.SOFTWARE_VERSION;
            this.Specialconditions = e.SPECIAL_CONDITIONS;
            this.SummerLoadLimit = e.SUMMER_LOAD_LIMIT;
            this.SwitchType = e.SWITCH_TYPE;
            this.TransmitDisableDelay = e.TRANSMIT_DISABLE_DELAY;
            this.TransmitEnableDelay = e.TRANSMIT_ENABLE_DELAY;
            this.WinterLoadLimit = e.WINTER_LOAD_LIMIT;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
        }

        public void PopulateHistoryFromEntity(SM_SWITCH_MSO_HIST entityHistory, SM_SWITCH_MSO e)
        {
            //added oct 2019 - start
            entityHistory.CURRENT_RATIO_CTR = e.CURRENT_RATIO_CTR;
            entityHistory.VOLTAGE_RATIO_VTR = e.VOLTAGE_RATIO_VTR;
            entityHistory.PHASE_INST_50P = e.PHASE_INST_50P;
            entityHistory.PHASE_TIME_DELAY = e.PHASE_TIME_DELAY;
            entityHistory.RESIDUAL_GR_INST_50G = e.RESIDUAL_GR_INST_50G;
            entityHistory.GROUND_TIME_DELAY = e.GROUND_TIME_DELAY;
            entityHistory.SHOT_TO_LOCKOUT_SECT = e.SHOT_TO_LOCKOUT_SECT;
            entityHistory.RESET_TIME = e.RESET_TIME;
            entityHistory.PHASE_A_SCADA_IDENTI_A = e.PHASE_A_SCADA_IDENTI_A;
            entityHistory.PHASE_B_SCADA_IDENTI_A = e.PHASE_B_SCADA_IDENTI_A;
            entityHistory.PHASE_C_SCADA_IDENTI_A = e.PHASE_C_SCADA_IDENTI_A;
            entityHistory.GROUND_SCADA_IDENTI_A = e.GROUND_SCADA_IDENTI_A;
            //added oct 2019 - end
            entityHistory.DIVISION = e.DIVISION;
            entityHistory.DISTRICT = e.DISTRICT;
            entityHistory.OPERATING_NUM = e.OPERATING_NUM;
            entityHistory.ATS_CAPABLE = e.ATS_CAPABLE;
            entityHistory.ATS_FEATURE = e.ATS_FEATURE;
            entityHistory.BAUD_RATE = e.BAUD_RATE;
            entityHistory.CONTROL_SERIAL_NUM = e.CONTROL_SERIAL_NUM;
            entityHistory.CONTROL_UNIT_TYPE = e.CONTROL_UNIT_TYPE;
            entityHistory.CURRENT_FUTURE = e.CURRENT_FUTURE;
            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.DEVICE_ID = e.DEVICE_ID;
            entityHistory.EFFECTIVE_DT = e.EFFECTIVE_DT;
            entityHistory.FLISR_ENGINEERING_COMMENTS = e.FLISR_ENGINEERING_COMMENTS;
            entityHistory.FIRMWARE_VERSION = e.FIRMWARE_VERSION;
            entityHistory.FLISR = e.FLISR;
            entityHistory.LIMITING_FACTOR = e.LIMITING_FACTOR;
            entityHistory.MASTER_STATION = e.MASTER_STATION;
            entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
            entityHistory.OPERATING_MODE = e.OPERATING_MODE;
            entityHistory.PEER_REVIEW_BY = e.PEER_REVIEW_BY;
            entityHistory.PEER_REVIEW_DT = e.PEER_REVIEW_DT;
            entityHistory.PREPARED_BY = e.PREPARED_BY;
            entityHistory.REPEATER = e.REPEATER;
            entityHistory.RTU_ADDRESS = e.RTU_ADDRESS;
            entityHistory.SCADA = e.SCADA;
            entityHistory.RADIO_MANF_CD = e.RADIO_MANF_CD;
            entityHistory.RADIO_MODEL_NUM = e.RADIO_MODEL_NUM;
            entityHistory.RADIO_SERIAL_NUM = e.RADIO_SERIAL_NUM;
            entityHistory.SCADA_TYPE = e.SCADA_TYPE;
            entityHistory.SECTIONALIZING_FEATURE = e.SECTIONALIZING_FEATURE;
            entityHistory.SOFTWARE_VERSION = e.SOFTWARE_VERSION;
            entityHistory.SPECIAL_CONDITIONS = e.SPECIAL_CONDITIONS;
            entityHistory.SUMMER_LOAD_LIMIT = e.SUMMER_LOAD_LIMIT;
            entityHistory.SWITCH_TYPE = e.SWITCH_TYPE;
            entityHistory.TRANSMIT_DISABLE_DELAY = e.TRANSMIT_DISABLE_DELAY;
            entityHistory.TRANSMIT_ENABLE_DELAY = e.TRANSMIT_ENABLE_DELAY;
            entityHistory.WINTER_LOAD_LIMIT = e.WINTER_LOAD_LIMIT;
            entityHistory.RTU_EXIST = e.RTU_EXIST;
            entityHistory.RTU_MANF_CD = e.RTU_MANF_CD;
            entityHistory.RTU_FIRMWARE_VERSION = e.RTU_FIRMWARE_VERSION;
            entityHistory.RTU_MODEL_NUM = e.RTU_MODEL_NUM;
            entityHistory.RTU_SERIAL_NUM = e.RTU_SERIAL_NUM;
            entityHistory.RTU_SOFTWARE_VERSION = e.RTU_SOFTWARE_VERSION;

        }
    }
}
