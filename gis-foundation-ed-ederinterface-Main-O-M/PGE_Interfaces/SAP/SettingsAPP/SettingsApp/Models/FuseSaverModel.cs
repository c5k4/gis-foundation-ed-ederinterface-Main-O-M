using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class FuseSaverModel
    {
        [SettingsValidatorAttribute("FuseSaver", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceId { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String ControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "POLICY_TYPE")]
        [Display(Name = "Policy Type:")]
        public System.String PolicyType { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "BYPASS_PLANS")]
        [Display(Name = "Bypass plan :")]
        public System.String BypassPlan { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String FirmwareVersion { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String SoftwareVersion { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "ENGINEERING_COMMENTS")]
        [Display(Name = "Engineering Comments :")]
        public System.String EngineeringComments { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "SCADA")]
        [Display(Name = "SCADA :")]
        public System.String Scada { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "SCADA_TYPE")]
        [Display(Name = "SCADA Type :")]
        public System.String ScadaType { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "MASTER_STATION")]
        [Display(Name = "Master Station :")]
        public System.String MasterStation { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "BAUD_RATE")]
        [Display(Name = "Baud Rate :")]
        public System.Int32? BaudRate { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "TRANSMIT_ENABLE_DELAY")]
        [Display(Name = "Transmit Enable Delay :")]
        public System.Int32? TransmitEnableDelay { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "TRANSMIT_DISABLE_DELAY")]
        [Display(Name = "Transmit Disable Delay :")]
        public System.Int32? TransmitDisableDelay { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "RTU_ADDRESS")]
        [Display(Name = "RTU Address :")]
        public System.String RtuAddress { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "REPEATER")]
        [Display(Name = "(if applicable) Repeater :")]
        public System.String Repeater { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special conditions :")]
        public System.String SpecialConditions { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "RADIO_MANF_CD")]
        [Display(Name = "SCADA radio manufacturer :")]
        public System.String ScadaRadioManufacturer { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "RADIO_MODEL_NUM")]
        [Display(Name = "SCADA radio model # :")]
        public System.String ScadaRadioModelNum { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "PERMIT_RB_CUTIN")]
        [Display(Name = "Permit RB Cut-In :")]
        public System.String PermitRbCutIn { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "RADIO_SERIAL_NUM")]
        [Display(Name = "SCADA radio serial # :")]
        public System.String ScadaRadioSerialNum { get; set; }

        //[SettingsValidatorAttribute("FuseSaver", "BOC_VOLTAGE")]
        //[Display(Name = "BOC Voltage :")]
        //public System.Int16? BocVoltage { get; set; }

        //[SettingsValidatorAttribute("FuseSaver", "RB_CUTOUT_TIME")]
        //[Display(Name = "RB C/OUT Time :")]
        //public System.Int16? RbCutoutTime { get; set; }
        
        [SettingsValidatorAttribute("FuseSaver", "RTU_EXISTS")]
        [Display(Name = "External RTU :")]
        public System.String RTUExists { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "RTU_MANF_CD")]
        [Display(Name = "RTU Manufacturer :")]
        public System.String RTUManufacture { get; set; }


        [SettingsValidatorAttribute("FuseSaver", "RTU_MODEL_NUM")]
        [Display(Name = "RTU Model # :")]
        public System.String RTUModelNumber { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "RTU_SERIAL_NUM")]
        [Display(Name = "RTU Serial # :")]
        public System.String RTUSerialNumber { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "RTU_SOFTWARE_VERSION")]
        [Display(Name = "RTU Software version :")]
        public System.String RTUSoftwareVersion { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "RTU_FIRMWARE_VERSION")]
        [Display(Name = "RTU Firmware version :")]
        public System.String RTUFirmwareVersion { get; set; }

        //Fuse Saver new fields
        [SettingsValidatorAttribute("FuseSaver", "NORMAL_MIN_TRIP_CM")]
        [Display(Name = "Normal Min Trip Current Multiplier")]
        public System.String NORMALMINTRIPCM { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "NORMAL_MAX_FT")]
        [Display(Name = "Normal Max Fault Time")]
        public System.String NORMALMAXFT { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "NORMAL_INST_TS")]
        [Display(Name = "Normal Inst Trip Setting")]
        public System.String NORMALINSTTS { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "NORMAL_MIN_TT")]
        [Display(Name = "Normal Min Trip Time")]
        public System.String NORMALMINTT { get; set; }

        //Fast Mode
        [SettingsValidatorAttribute("FuseSaver", "FASTMODE_MIN_TCM")]
        [Display(Name = "Fast Mode Min Trip Current Multiplier")]
        public System.String FASTMODEMINTCM { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "FASTMODE_MAX_FT")]
        [Display(Name = "Fast Mode Max Fault Time")]
        public System.String FASTMODEMAXFT { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "FASTMODE_INST_TS")]
        [Display(Name = "Fast Mode Inst Trip Setting")]
        public System.String FASTMODEINSTTS { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "FASTMODE_MIN_TT")]
        [Display(Name = "Fast Mode Min Trip Time")]
        public System.String FASTMODEMINTT { get; set; }

        //OPeration
        [SettingsValidatorAttribute("FuseSaver", "OS_DEAD_TS")]
        [Display(Name = "Dead Time Setting")]
        public System.Int16? OSDEADTS { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OS_ENABLE_EXT_LEV")]
        [Display(Name = " Enable External Level")]
        public System.String OSENABLEEXTLEV { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OS_MANUAL_GAN_OP")]
        [Display(Name = " Manual Ganged Operation")]
        public System.String OSMANUALGANOP { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OS_DE_ENER_LT")]
        [Display(Name = "De-Energised Line Time")]
        public System.String OSDEENERLT { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OS_RECLAIM_TIME")]
        [Display(Name = "Reclaim Time")]
        public System.Int32? OSRECLAIMTIME { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OS_INRUSH_RM")]
        [Display(Name = "Inrush Restraint Multiplier")]
        public System.String OSINRUSHRM { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OS_INRUSH_RT")]
        [Display(Name = "Inrush Restraint Time (mS)")]
        public System.Int16? OSINRUSHRT { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OS_CAP_CHARGE")]
        [Display(Name = "Capacitor Charge from Communications Module")]
        public System.String OSCAPCHARGE { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "LEVER_UP_PM")]
        [Display(Name = "Lever UP Protection Mode")]
        public System.String LEVERUPPM { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "LEVER_UP_DE_EL_PM")]
        [Display(Name = "Lever UP De-Energised Line Protection Mode")]
        public System.String LEVERUPDEELPM { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "LEVER_UP_PSEDO_3PH")]
        [Display(Name = "Lever UP Enable Pseudo 3-Phase Trip")]
        public System.String LEVERUPPSEDO3PH { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "LEVER_UP_3PH_LOCK")]
        [Display(Name = "Lever UpThree phase lockout")]
        public System.String LEVERUP3PHLOCK { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "LEVER_DOWN_PM")]
        [Display(Name = "Lever Down Protection Mod")]
        public System.String LEVERDOWNPM { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "LEVER_DOWN_DE_EL_PM")]
        [Display(Name = "Lever Down De-Energised Line Protection Mode")]
        public System.String LEVERDOWNDEELPM { get; set; }


        [SettingsValidatorAttribute("FuseSaver", "LEVER_DOWN_PSEDO_3PH")]
        [Display(Name = "Lever Down Enable Pseudo 3-Phase Trip")]
        public System.String LEVERDOWNPSEDO3PH { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "LEVER_DOWN_3PH_LOCK")]
        [Display(Name = "Lever Down 3 Phase lockout")]
        public System.String LEVERDOWN3PHLOCK { get; set; }


        [SettingsValidatorAttribute("FuseSaver", "LEVER_DOWN_MI")]
        [Display(Name = "Lever Down Manual Inhibit")]
        public System.String LEVERDOWNMI { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OTH_LOAD_PP")]
        [Display(Name = "Load Profile Period")]
        public System.String OTHLOADPP { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OTH_ENDLIFE_POLICY")]
        [Display(Name = "End Of Life Policy Seting")]

        public System.String OTHENDLIFEPOLICY { get; set; }
        [SettingsValidatorAttribute("FuseSaver", "OTH_LINE_FREQ")]
        [Display(Name = "Line Frequency Setting(Hz)")]
        public System.String OTHLINEFREQ { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OTH_TIMEZONE_DISPLAY")]
        [Display(Name = "Time Zone")]
        public System.String OTHTIMEZONEDISPLAY { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OTH_FAULT_PI")]
        [Display(Name = "Fault Passage Indicator(hrs)")]
        public System.String OTHFAULTPI { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "FUSE_TYPE")]
        [Display(Name = "Fuse Type")]
        public System.String FUSETYPE { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "FUSE_RATING")]
        [Display(Name = "Fuse Rating")]
        public System.String FUSERATING { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OTH_PH_A_LABEL")]
        [Display(Name = "Phase A :")]
        public System.String otherPhaseA { get; set; }

        [SettingsValidatorAttribute("FuseSaver", "OTH_PH_B_LABEL")]
        [Display(Name = "Phase B :")]
        public System.String otherPhaseB { get; set; }
        [SettingsValidatorAttribute("FuseSaver", "OTH_PH_C_LABEL")]
        [Display(Name = "Phase C :")]
        public System.String otherPhaseC { get; set; }


        public bool Release { get; set; }
        public List<GISAttributes> GISAttributes { get; set; }

        public string DropDownPostbackScript { get; set; }
        // public string DropDownPostbackScriptScada { get; set; }

        public HashSet<string> FieldsToDisplay { get; set; }

        public SelectList PolicyTypeList { get; set; }
        public SelectList RTUManufactureList { get; set; }
        public SelectList ScadaRadioManufacturerList { get; set; } 
        
        public SelectList NORMAL_MIN_TRIP_CM_List { get; set; }
        public SelectList NORMAL_MAX_FT_List { get; set; }
        public SelectList NORMAL_INST_TS_list { get; set; }
        public SelectList NORMAL_MIN_TT_list { get; set; }
        public SelectList FASTMODE_MIN_TCM_List { get; set; }
        public SelectList FASTMODE_MAX_FT_list { get; set; }
        public SelectList FASTMODE_INST_TS_list { get; set; }
        public SelectList FASTMODE_MIN_TT_list { get; set; }
        public SelectList OS_DEAD_TS_list { get; set; }
        public SelectList OS_ENABLE_EXT_LEV_list { get; set; }
        public SelectList OS_MANUAL_GAN_OP_list { get; set; }
        public SelectList OS_DE_ENER_LT_list { get; set; }
        public SelectList OS_RECLAIM_TIME_list { get; set; }
        public SelectList OS_INRUSH_RM_list { get; set; }
        public SelectList OS_INRUSH_RT_list { get; set; }
        public SelectList OS_CAP_CHARGE_list { get; set; }
        public SelectList ScadaTypeList { get; set; }
        public SelectList LEVER_UP_PM_list { get; set; }
        public SelectList LEVER_UP_DE_EL_PM_list { get; set; }
        public SelectList LEVER_UP_PSEDO_3PH_list { get; set; }
        public SelectList LEVER_UP_3PH_LOCK_list { get; set; }
        public SelectList LEVER_DOWN_PM_list { get; set; }
        public SelectList LEVER_DOWN_DE_EL_PM_list { get; set; }
        public SelectList LEVER_DOWN_PSEDO_3PH_list { get; set; }
        public SelectList LEVER_DOWN_3PH_LOCK_list { get; set; }
        public SelectList LEVER_DOWN_MI_list { get; set; }
        public SelectList OTH_LOAD_PP_list { get; set; }
        public SelectList OTH_ENDLIFE_POLICY_list { get; set; }
        public SelectList OTH_LINE_FREQ_list { get; set; }
        public SelectList OTH_TIMEZONE_DISPLAY_list { get; set; }
        public SelectList OTH_FAULT_PI_list { get; set; }
        public SelectList FUSE_TYPE_List { get; set; }
        public SelectList FUSE_RATING_list { get; set; }

      

        public void PopulateEntityFromModel(SM_RECLOSER_FS e)
        {
            e.DEVICE_ID = this.DeviceId;
            e.PREPARED_BY = this.PreparedBy;
            e.POLICY_TYPE = this.PolicyType;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            e.CONTROL_SERIAL_NUM = this.ControllerSerialNum;
            e.OK_TO_BYPASS = this.OkToBypass;

            e.BYPASS_PLANS = this.BypassPlan;

            e.FIRMWARE_VERSION = this.FirmwareVersion;
            e.SOFTWARE_VERSION = this.SoftwareVersion;
            e.ENGINEERING_COMMENTS = this.EngineeringComments;
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
            e.PERMIT_RB_CUTIN = this.PermitRbCutIn;

            e.RTU_EXIST = this.RTUExists;
            e.RTU_MANF_CD = this.RTUManufacture;
            e.RTU_FIRMWARE_VERSION = this.RTUFirmwareVersion;
            e.RTU_MODEL_NUM = this.RTUModelNumber;
            e.RTU_SERIAL_NUM = this.RTUSerialNumber;
            e.RTU_SOFTWARE_VERSION = this.RTUSoftwareVersion;

            e.NORMAL_MIN_TRIP_CM = this.NORMALMINTRIPCM;
            e.NORMAL_MAX_FT = this.NORMALMAXFT;
            e.NORMAL_INST_TS = Convert.ToString(this.NORMALINSTTS);
            e.NORMAL_MIN_TT = Convert.ToString(this.NORMALMINTT);
            e.FASTMODE_MIN_TCM = this.FASTMODEMINTCM;
            e.FASTMODE_MAX_FT = this.FASTMODEMAXFT;
            e.FASTMODE_INST_TS = Convert.ToString(this.FASTMODEINSTTS);
            e.FASTMODE_MIN_TT = this.FASTMODEMINTT;
            e.OS_DEAD_TS = Convert.ToInt16(this.OSDEADTS);
            e.OS_ENABLE_EXT_LEV = this.OSENABLEEXTLEV;
            e.OS_MANUAL_GAN_OP = this.OSMANUALGANOP;
            e.OS_RECLAIM_TIME = Convert.ToInt16(this.OSRECLAIMTIME);
            e.OS_INRUSH_RM = this.OSINRUSHRM;
            e.OS_INRUSH_RT = Convert.ToInt16(this.OSINRUSHRT);
            e.OS_CAP_CHARGE = this.OSCAPCHARGE;
            e.LEVER_UP_PM = this.LEVERUPPM;
            e.LEVER_UP_DE_EL_PM = this.LEVERUPDEELPM;
            e.LEVER_UP_PSEDO_3PH = this.LEVERUPPSEDO3PH;
            e.LEVER_UP_3PH_LOCK = this.LEVERUP3PHLOCK;
            e.LEVER_DOWN_PM = this.LEVERDOWNPM;
            e.LEVER_DOWN_MI = this.LEVERDOWNMI;
            e.LEVER_DOWN_PSEDO_3PH = this.LEVERDOWNPSEDO3PH;
            e.LEVER_DOWN_3PH_LOCK = this.LEVERDOWN3PHLOCK;
            e.LEVER_DOWN_DE_EL_PM = this.LEVERDOWNDEELPM;

            //oth
            e.OTH_LOAD_PP = this.OTHLOADPP;
            e.OTH_ENDLIFE_POLICY = this.OTHENDLIFEPOLICY;
            e.OTH_LINE_FREQ = this.OTHLINEFREQ;
            e.OTH_PH_A_LABEL = this.otherPhaseA;
            e.OTH_PH_B_LABEL = this.otherPhaseB;
            e.OTH_PH_C_LABEL = this.otherPhaseC;
            e.OTH_TIMEZONE_DISPLAY = this.OTHTIMEZONEDISPLAY;
            e.OTH_FAULT_PI = this.OTHFAULTPI;
            e.FUSE_TYPE = this.FUSETYPE;
            e.FUSE_RATING = this.FUSERATING;
        }

        public void PopulateModelFromEntity(SM_RECLOSER_FS e)
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
            this.BypassPlan = e.BYPASS_PLANS;
            this.PolicyType = e.POLICY_TYPE;

            this.FirmwareVersion = e.FIRMWARE_VERSION;
            this.SoftwareVersion = e.SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
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
            this.PermitRbCutIn = e.PERMIT_RB_CUTIN;

            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            //this.EngineeringComments = e.ENGINEERING_COMMENTS;

            this.NORMALMINTRIPCM = e.NORMAL_MIN_TRIP_CM;
            this.NORMALMAXFT = e.NORMAL_MAX_FT;
            this.NORMALINSTTS = e.NORMAL_INST_TS;
            this.NORMALMINTT = e.NORMAL_MIN_TT;
            this.FASTMODEMINTCM = e.FASTMODE_MIN_TCM;
            this.FASTMODEMAXFT = e.FASTMODE_MAX_FT;
            this.FASTMODEINSTTS = e.FASTMODE_INST_TS;
            this.FASTMODEMINTT = e.FASTMODE_MIN_TT;
            this.OSDEADTS = e.OS_DEAD_TS;
            this.OSENABLEEXTLEV = e.OS_ENABLE_EXT_LEV;
            this.OSMANUALGANOP = e.OS_MANUAL_GAN_OP;
            this.OSRECLAIMTIME = e.OS_RECLAIM_TIME;
            this.OSINRUSHRM = e.OS_INRUSH_RM;
            this.OSINRUSHRT = Convert.ToInt16(e.OS_INRUSH_RT);
            this.OSCAPCHARGE = e.OS_CAP_CHARGE;
            this.LEVERUPPM = e.LEVER_UP_PM;
            this.LEVERUPDEELPM = e.LEVER_UP_DE_EL_PM;
            this.LEVERUPPSEDO3PH = e.LEVER_UP_PSEDO_3PH;
            this.LEVERUP3PHLOCK = e.LEVER_UP_3PH_LOCK;
            this.LEVERDOWNPM = e.LEVER_DOWN_PM;
            this.LEVERDOWNMI = e.LEVER_DOWN_MI;
            this.LEVERDOWNPSEDO3PH = e.LEVER_DOWN_PSEDO_3PH;
            this.LEVERDOWN3PHLOCK = e.LEVER_DOWN_3PH_LOCK;
            this.LEVERDOWNDEELPM = e.LEVER_DOWN_DE_EL_PM;

            //oth
            this.OTHLOADPP = e.OTH_LOAD_PP;
            this.OTHENDLIFEPOLICY = e.OTH_ENDLIFE_POLICY;
            this.OTHLINEFREQ = e.OTH_LINE_FREQ;
            this.otherPhaseA = e.OTH_PH_A_LABEL;
            this.otherPhaseB = e.OTH_PH_B_LABEL;
            this.otherPhaseC = e.OTH_PH_C_LABEL;
            this.OTHTIMEZONEDISPLAY = e.OTH_TIMEZONE_DISPLAY;
            this.OTHFAULTPI = e.OTH_FAULT_PI;
            this.FUSETYPE = e.FUSE_TYPE;
            this.FUSERATING = e.FUSE_RATING;
        }

        public void PopulateHistoryFromEntity(SM_RECLOSER_FS_HIST entityHistory, SM_RECLOSER_FS e)
        {
            entityHistory.GLOBAL_ID = e.GLOBAL_ID;
            entityHistory.FEATURE_CLASS_NAME = e.FEATURE_CLASS_NAME;
            entityHistory.OPERATING_NUM = e.OPERATING_NUM;
            entityHistory.DEVICE_ID = Convert.ToInt32(e.DEVICE_ID);
            entityHistory.PREPARED_BY = e.PREPARED_BY;
            entityHistory.POLICY_TYPE = e.POLICY_TYPE;
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
            entityHistory.RTU_EXIST = e.RTU_EXIST;
            entityHistory.RTU_MANF_CD = e.RTU_MANF_CD;
            entityHistory.RTU_FIRMWARE_VERSION = e.RTU_FIRMWARE_VERSION;
            entityHistory.RTU_MODEL_NUM = e.RTU_MODEL_NUM;
            entityHistory.RTU_SERIAL_NUM = e.RTU_SERIAL_NUM;
            entityHistory.RTU_SOFTWARE_VERSION = e.RTU_SOFTWARE_VERSION;
            entityHistory.ENGINEERING_COMMENTS = e.ENGINEERING_COMMENTS;

            //
            entityHistory.NORMAL_MIN_TRIP_CM = e.NORMAL_MIN_TRIP_CM;
            entityHistory.NORMAL_MAX_FT = e.NORMAL_MAX_FT;
            entityHistory.NORMAL_INST_TS = e.NORMAL_INST_TS;
            entityHistory.NORMAL_MIN_TT = e.NORMAL_MIN_TT;
            entityHistory.FASTMODE_MIN_TCM = e.FASTMODE_MIN_TCM;
            entityHistory.FASTMODE_MAX_FT = e.FASTMODE_MAX_FT;
            entityHistory.FASTMODE_INST_TS = e.FASTMODE_INST_TS;
            entityHistory.FASTMODE_MIN_TT = e.FASTMODE_MIN_TT;
            entityHistory.OS_DEAD_TS = e.OS_DEAD_TS;
            entityHistory.OS_ENABLE_EXT_LEV = e.OS_ENABLE_EXT_LEV;
            entityHistory.OS_MANUAL_GAN_OP = e.OS_MANUAL_GAN_OP;
            entityHistory.OS_RECLAIM_TIME = e.OS_RECLAIM_TIME;
            entityHistory.OS_INRUSH_RM = e.OS_INRUSH_RM;
            entityHistory.OS_INRUSH_RT = e.OS_INRUSH_RT;
            entityHistory.OS_CAP_CHARGE = e.OS_CAP_CHARGE;
            entityHistory.LEVER_UP_PM = e.LEVER_UP_PM;
            entityHistory.LEVER_UP_DE_EL_PM = e.LEVER_UP_DE_EL_PM;
            entityHistory.LEVER_UP_PSEDO_3PH = e.LEVER_UP_PSEDO_3PH;
            entityHistory.LEVER_UP_3PH_LOCK = e.LEVER_UP_3PH_LOCK;
            entityHistory.LEVER_DOWN_PM = e.LEVER_DOWN_PM;
            entityHistory.LEVER_DOWN_MI = e.LEVER_DOWN_MI;
            entityHistory.LEVER_DOWN_PSEDO_3PH = e.LEVER_DOWN_PSEDO_3PH;
            entityHistory.LEVER_DOWN_3PH_LOCK = e.LEVER_DOWN_3PH_LOCK;
            entityHistory.LEVER_DOWN_DE_EL_PM = e.LEVER_DOWN_DE_EL_PM;

            //oth
            entityHistory.OTH_LOAD_PP = e.OTH_LOAD_PP;
            entityHistory.OTH_ENDLIFE_POLICY = e.OTH_ENDLIFE_POLICY;
            entityHistory.OTH_LINE_FREQ = e.OTH_LINE_FREQ;
            entityHistory.OTH_PH_A_LABEL = e.OTH_PH_A_LABEL;
            entityHistory.OTH_PH_B_LABEL = e.OTH_PH_B_LABEL;
            entityHistory.OTH_PH_C_LABEL = e.OTH_PH_C_LABEL;
            entityHistory.OTH_TIMEZONE_DISPLAY = e.OTH_TIMEZONE_DISPLAY;
            entityHistory.OTH_FAULT_PI = e.OTH_FAULT_PI;
            entityHistory.FUSE_TYPE = e.FUSE_TYPE;
            entityHistory.FUSE_RATING = e.FUSE_RATING;
            entityHistory.BYPASS_PLANS = e.BYPASS_PLANS;
            entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
           //
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
          //
           

        }

        public void PopulateModelFromHistoryEntity(SM_RECLOSER_FS_HIST e)
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

            this.BypassPlan = e.BYPASS_PLANS;
            this.PolicyType = e.POLICY_TYPE;

            this.FirmwareVersion = e.FIRMWARE_VERSION;
            this.SoftwareVersion = e.SOFTWARE_VERSION;

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
            this.PermitRbCutIn = e.PERMIT_RB_CUTIN;

            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;

            this.NORMALMINTRIPCM = e.NORMAL_MIN_TRIP_CM;
            this.NORMALMAXFT = e.NORMAL_MAX_FT;
            this.NORMALINSTTS = e.NORMAL_INST_TS;
            this.NORMALMINTT = e.NORMAL_MIN_TT;
            this.FASTMODEMINTCM = e.FASTMODE_MIN_TCM;
            this.FASTMODEMAXFT = e.FASTMODE_MAX_FT;
            this.FASTMODEINSTTS = e.FASTMODE_INST_TS;
            this.FASTMODEMINTT = e.FASTMODE_MIN_TT;
            this.OSDEADTS = e.OS_DEAD_TS;
            this.OSENABLEEXTLEV = e.OS_ENABLE_EXT_LEV;
            this.OSMANUALGANOP = e.OS_MANUAL_GAN_OP;
            this.OSRECLAIMTIME = e.OS_RECLAIM_TIME;
            this.OSINRUSHRM = e.OS_INRUSH_RM;
            this.OSINRUSHRT = Convert.ToInt16(e.OS_INRUSH_RT);
            this.OSCAPCHARGE = e.OS_CAP_CHARGE;
            this.LEVERUPPM = e.LEVER_UP_PM;
            this.LEVERUPDEELPM = e.LEVER_UP_DE_EL_PM;
            this.LEVERUPPSEDO3PH = e.LEVER_UP_PSEDO_3PH;
            this.LEVERUP3PHLOCK = e.LEVER_UP_3PH_LOCK;
            this.LEVERDOWNPM = e.LEVER_DOWN_PM;
            this.LEVERDOWNMI = e.LEVER_DOWN_MI;
            this.LEVERDOWNPSEDO3PH = e.LEVER_DOWN_PSEDO_3PH;
            this.LEVERDOWN3PHLOCK = e.LEVER_DOWN_3PH_LOCK;
            this.LEVERDOWNDEELPM = e.LEVER_DOWN_DE_EL_PM;

            //oth
            this.OTHLOADPP = e.OTH_LOAD_PP;
            this.OTHENDLIFEPOLICY = e.OTH_ENDLIFE_POLICY;
            this.OTHLINEFREQ = e.OTH_LINE_FREQ;
            this.otherPhaseA = e.OTH_PH_A_LABEL;
            this.otherPhaseB = e.OTH_PH_B_LABEL;
            this.otherPhaseC = e.OTH_PH_C_LABEL;
            this.OTHTIMEZONEDISPLAY = e.OTH_TIMEZONE_DISPLAY;
            this.OTHFAULTPI = e.OTH_FAULT_PI;
            this.FUSETYPE = e.FUSE_TYPE;
            this.FUSERATING = e.FUSE_RATING;

        }
    }
}