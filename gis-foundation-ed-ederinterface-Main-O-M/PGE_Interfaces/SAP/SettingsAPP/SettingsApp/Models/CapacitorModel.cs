using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class CapacitorModel
    {
        [SettingsValidatorAttribute("Capacitor", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("Capacitor", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceID { get; set; }

        [SettingsValidatorAttribute("Capacitor", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("Capacitor", "CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String ControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("Capacitor", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("Capacitor", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("Capacitor", "NOTES")]
        [Display(Name = "Notes :")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("Capacitor", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        [SettingsValidatorAttribute("Capacitor", "CONTROL_TYPE")]
        [Display(Name = "Controller Unit Type :")]
        public System.String ControlType { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_SCHEDULE")]
        [Display(Name = "Schedule Unit type :")]
        public System.Int16? Sch1_ScheduleUnitType { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_CONTROL_STRATEGY")]
        [Display(Name = "Control Strategy :")]
        public System.String Sch1_ControlStrategy { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_TIME_ON")]
        [Display(Name = "Time on :")]
        public System.Int16? Sch1_TimeOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_TIME_OFF")]
        [Display(Name = "Time off :")]
        public System.Int16? Sch1_TimeOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_LOW_VOLTAGE_SETPOINT")]
        [Display(Name = "Low voltage setpoint :")]
        public System.Decimal? Sch1_LowVoltageSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_HIGH_VOLTAGE_SETPOINT")]
        [Display(Name = "High voltage setpoint :")]
        public System.Decimal? Sch1_HighVoltageSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_TEMP_SETPOINT_ON")]
        [Display(Name = "Temp. setpoint on :")]
        public System.Int32? Sch1_TempSetPointOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_TEMP_SETPOINT_OFF")]
        [Display(Name = "Temp. setpoint off :")]
        public System.Int32? Sch1_TempSetPointOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_START_DATE")]
        [Display(Name = "Start date :")]
        public System.String Sch1_StartDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_END_DATE")]
        [Display(Name = "End date :")]
        public System.String Sch1_EndDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_WEEKDAYS")]
        [Display(Name = "Inactive Weekdays :")]
        public System.String Sch1_Weekdays { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_SATURDAY")]
        [Display(Name = "Inactive Saturday :")]
        public System.String Sch1_Saturday { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_SUNDAY")]
        [Display(Name = "Inactive Sunday :")]
        public System.String Sch1_Sunday { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_HOLIDAYS")]
        [Display(Name = "Inactive Holidays :")]
        public System.String Sch1_Holidays { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_KVAR_SETPOINT_ON")]
        [Display(Name = "kVAR setpoint on :")]
        public System.Int32? Sch1_KvarSetPointOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_KVAR_SETPOINT_OFF")]
        [Display(Name = "kVAR setpoint off :")]
        public System.Int32? Sch1_KvarSetPointOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_SCHEDULE")]
        [Display(Name = "Schedule Unit type :")]
        public System.Int16? Sch2_ScheduleUnitType { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_CONTROL_STRATEGY")]
        [Display(Name = "Control Strategy :")]
        public System.String Sch2_ControlStrategy { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_TIME_ON")]
        [Display(Name = "Time on :")]
        public System.Int16? Sch2_TimeOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_TIME_OFF")]
        [Display(Name = "Time off :")]
        public System.Int16? Sch2_TimeOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_LOW_VOLTAGE_SETPOINT")]
        [Display(Name = "Low voltage setpoint :")]
        public System.Decimal? Sch2_LowVoltageSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_HIGH_VOLTAGE_SETPOINT")]
        [Display(Name = "High voltage setpoint :")]
        public System.Decimal? Sch2_HighVoltageSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_TEMP_SETPOINT_ON")]
        [Display(Name = "Temp. setpoint on :")]
        public System.Int32? Sch2_TempSetPointOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_TEMP_SETPOINT_OFF")]
        [Display(Name = "Temp. setpoint off :")]
        public System.Int32? Sch2_TempSetPointOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_START_DATE")]
        [Display(Name = "Start date :")]
        public System.String Sch2_StartDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_END_DATE")]
        [Display(Name = "End date :")]
        public System.String Sch2_EndDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_WEEKDAYS")]
        [Display(Name = "Inactive Weekdays :")]
        public System.String Sch2_Weekdays { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_SATURDAY")]
        [Display(Name = "Inactive Saturday :")]
        public System.String Sch2_Saturday { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_SUNDAY")]
        [Display(Name = "Inactive Sunday :")]
        public System.String Sch2_Sunday { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_HOLIDAYS")]
        [Display(Name = "Inactive Holidays :")]
        public System.String Sch2_Holidays { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_KVAR_SETPOINT_ON")]
        [Display(Name = "kVAR setpoint on :")]
        public System.Int32? Sch2_KvarSetPointOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_KVAR_SETPOINT_OFF")]
        [Display(Name = "kVAR setpoint off :")]
        public System.Int32? Sch2_KvarSetPointOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_SCHEDULE")]
        [Display(Name = "Schedule Unit type :")]
        public System.Int16? Sch3_ScheduleUnitType { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_CONTROL_STRATEGY")]
        [Display(Name = "Control Strategy :")]
        public System.String Sch3_ControlStrategy { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_TIME_ON")]
        [Display(Name = "Time on :")]
        public System.Int16? Sch3_TimeOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_TIME_OFF")]
        [Display(Name = "Time off :")]
        public System.Int16? Sch3_TimeOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_LOW_VOLTAGE_SETPOINT")]
        [Display(Name = "Low voltage setpoint :")]
        public System.Decimal? Sch3_LowVoltageSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_HIGH_VOLTAGE_SETPOINT")]
        [Display(Name = "High voltage setpoint :")]
        public System.Decimal? Sch3_HighVoltageSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_TEMP_SETPOINT_ON")]
        [Display(Name = "Temp. setpoint on :")]
        public System.Int32? Sch3_TempSetPointOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_TEMP_SETPOINT_OFF")]
        [Display(Name = "Temp. setpoint off :")]
        public System.Int32? Sch3_TempSetPointOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_START_DATE")]
        [Display(Name = "Start date :")]
        public System.String Sch3_StartDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_END_DATE")]
        [Display(Name = "End date :")]
        public System.String Sch3_EndDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_WEEKDAYS")]
        [Display(Name = "Inactive Weekdays :")]
        public System.String Sch3_Weekdays { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_SATURDAY")]
        [Display(Name = "Inactive Saturday :")]
        public System.String Sch3_Saturday { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_SUNDAY")]
        [Display(Name = "Inactive Sunday :")]
        public System.String Sch3_Sunday { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_HOLIDAYS")]
        [Display(Name = "Inactive Holidays :")]
        public System.String Sch3_Holidays { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_KVAR_SETPOINT_ON")]
        [Display(Name = "kVAR setpoint on :")]
        public System.Int32? Sch3_KvarSetPointOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_KVAR_SETPOINT_OFF")]
        [Display(Name = "kVAR setpoint off :")]
        public System.Int32? Sch3_KvarSetPointOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_SCHEDULE")]
        [Display(Name = "Schedule Unit type :")]
        public System.Int16? Sch4_ScheduleUnitType { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_CONTROL_STRATEGY")]
        [Display(Name = "Control Strategy :")]
        public System.String Sch4_ControlStrategy { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_TIME_ON")]
        [Display(Name = "Time on :")]
        public System.Int16? Sch4_TimeOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_TIME_OFF")]
        [Display(Name = "Time off :")]
        public System.Int16? Sch4_TimeOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_LOW_VOLTAGE_SETPOINT")]
        [Display(Name = "Low voltage setpoint :")]
        public System.Decimal? Sch4_LowVoltageSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_HIGH_VOLTAGE_SETPOINT")]
        [Display(Name = "High voltage setpoint :")]
        public System.Decimal? Sch4_HighVoltageSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_TEMP_SETPOINT_ON")]
        [Display(Name = "Temp. setpoint on :")]
        public System.Int32? Sch4_TempSetPointOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_TEMP_SETPOINT_OFF")]
        [Display(Name = "Temp. setpoint off :")]
        public System.Int32? Sch4_TempSetPointOff { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_START_DATE")]
        [Display(Name = "Start date :")]
        public System.String Sch4_StartDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_END_DATE")]
        [Display(Name = "End date :")]
        public System.String Sch4_EndDate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_WEEKDAYS")]
        [Display(Name = "Inactive Weekdays :")]
        public System.String Sch4_Weekdays { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_SATURDAY")]
        [Display(Name = "Inactive Saturday :")]
        public System.String Sch4_Saturday { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_SUNDAY")]
        [Display(Name = "Inactive Sunday :")]
        public System.String Sch4_Sunday { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_HOLIDAYS")]
        [Display(Name = "Inactive Holidays :")]
        public System.String Sch4_Holidays { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_KVAR_SETPOINT_ON")]
        [Display(Name = "kVAR setpoint on :")]
        public System.Int32? Sch4_KvarSetPointOn { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_KVAR_SETPOINT_OFF")]
        [Display(Name = "kVAR setpoint off :")]
        public System.Int32? Sch4_KvarSetPointOff { get; set; }

        //[SettingsValidatorAttribute("Capacitor", "CONTROLLER_UNIT_TYPE")]
        //[Display(Name = "Controller Unit Type :")]
        //public System.String ControllerUnitType { get; set; }

        [SettingsValidatorAttribute("Capacitor", "CONTROLLER_UNIT_MODEL")]
        [Display(Name = "Controller Unit Model :")]
        public System.String ControllerUnitModel { get; set; }

        [SettingsValidatorAttribute("Capacitor", "VOLT_VAR_TEAM_MEMBER")]
        [Display(Name = "Volt Var team member :")]
        public System.String VoltVarTeamMember { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SWITCH_POSITION")]
        [Display(Name = "Switch position :")]
        public System.String SwitchPosition { get; set; }

        //INC000004165708
        //[SettingsValidatorAttribute("Capacitor", "PREFERED_BANK_POSITION")]
        //[Display(Name = "Bank Position :")]
        //public System.String BankPosition { get; set; }

        [SettingsValidatorAttribute("Capacitor", "MAXCYCLES")]
        [Display(Name = "Maxcycles :")]
        public System.Int16? MaxCycles { get; set; }

        [SettingsValidatorAttribute("Capacitor", "DAYLIGHT_SAVINGS_TIME")]
        [Display(Name = "Daylight Time :")]
        public System.String DaylightTime { get; set; }

        [SettingsValidatorAttribute("Capacitor", "EST_VOLTAGE_CHANGE")]
        [Display(Name = "Est. Voltage Change (V) :")]
        public System.Decimal? ESTVoltageChangeTime { get; set; }

        [SettingsValidatorAttribute("Capacitor", "VOLTAGE_OVERRIDE_TIME")]
        [Display(Name = "Voltage override time :")]
        public System.Int16? VoltageOverrideTime { get; set; }

        [SettingsValidatorAttribute("Capacitor", "HIGH_VOLTAGE_OVERRIDE_SETPOINT")]
        [Display(Name = "Voltage Override High (V) :")]
        public System.Decimal? HighVoltageOverrideSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "LOW_VOLTAGE_OVERRIDE_SETPOINT")]
        [Display(Name = "Voltage Override Low (V) :")]
        public System.Decimal? LowVoltageOverrideSetPoint { get; set; }

        [SettingsValidatorAttribute("Capacitor", "VOLTAGE_CHANGE_TIME")]
        [Display(Name = "Voltage change time :")]
        public System.Int16? VoltageChangeTime { get; set; }

        [SettingsValidatorAttribute("Capacitor", "TEMPERATURE_OVERRIDE")]
        [Display(Name = "Temp. override :")]
        public System.String TempOverride { get; set; }

        [SettingsValidatorAttribute("Capacitor", "TEMPERATURE_CHANGE_TIME")]
        [Display(Name = "Temp. change time :")]
        public System.Int16? TempChangeTime { get; set; }

        [SettingsValidatorAttribute("Capacitor", "EST_BANK_VOLTAGE_RISE")]
        [Display(Name = "Est. Bank Voltage Rise (V) :")]
        public System.Decimal? EstBankVoltageRise { get; set; }

        [SettingsValidatorAttribute("Capacitor", "AUTO_BVR_CALC")]
        [Display(Name = "Auto BVR Calc :")]
        public System.String AutoBvrCalc { get; set; }

        [SettingsValidatorAttribute("Capacitor", "DATA_LOGGING_INTERVAL")]
        [Display(Name = "Log Interval :")]
        public System.String LogInterval { get; set; }

        [SettingsValidatorAttribute("Capacitor", "PULSE_TIME")]
        [Display(Name = "Pulse time :")]
        public System.Int16? PulseTime { get; set; }

        [SettingsValidatorAttribute("Capacitor", "MIN_SW_VOLTAGE")]
        [Display(Name = "Min. switch volt. :")]
        public System.Decimal? MinSwitchVolt { get; set; }

        [SettingsValidatorAttribute("Capacitor", "TIME_DELAY")]
        [Display(Name = "Delay :")]
        public System.Int16? Delay { get; set; }

        [SettingsValidatorAttribute("Capacitor", "FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String FirmwareVersion { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String SoftwareVersion { get; set; }

        [SettingsValidatorAttribute("Capacitor", "ENGINEERING_DOCUMENT")]
        [Display(Name = "Engineering Document :")]
        public System.String EngineeringDocument { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCADA")]
        [Display(Name = "SCADA :")]
        public System.String Scada { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCADA_TYPE")]
        [Display(Name = "SCADA Type :")]
        public System.String ScadaType { get; set; }

        [SettingsValidatorAttribute("Capacitor", "MASTER_STATION")]
        [Display(Name = "Master Station :")]
        public System.String MasterStation { get; set; }

        [SettingsValidatorAttribute("Capacitor", "BAUD_RATE")]
        [Display(Name = "Baud Rate :")]
        public System.Int32? BaudRate { get; set; }

        [SettingsValidatorAttribute("Capacitor", "TRANSMIT_ENABLE_DELAY")]
        [Display(Name = "Transmit Enable Delay :")]
        public System.Int32? TransmitEnableDelay { get; set; }

        [SettingsValidatorAttribute("Capacitor", "TRANSMIT_DISABLE_DELAY")]
        [Display(Name = "Transmit Disable Delay :")]
        public System.Int32? TransmitDisableDelay { get; set; }

        [SettingsValidatorAttribute("Capacitor", "RTU_ADDRESS")]
        [Display(Name = "RTU Address :")]
        public System.String RtuAddress { get; set; }

        [SettingsValidatorAttribute("Capacitor", "REPEATER")]
        [Display(Name = "(if applicable) Repeater :")]
        public System.String Repeater { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special conditions :")]
        public System.String SpecialConditions { get; set; }

        [SettingsValidatorAttribute("Capacitor", "RADIO_MANF_CD")]
        [Display(Name = "SCADA radio manufacturer :")]
        public System.String ScadaRadioManufacturer { get; set; }

        [SettingsValidatorAttribute("Capacitor", "RADIO_MODEL_NUM")]
        [Display(Name = "SCADA radio model # :")]
        public System.String ScadaRadioModelNum { get; set; }

        [SettingsValidatorAttribute("Capacitor", "RADIO_SERIAL_NUM")]
        [Display(Name = "SCADA radio serial # :")]
        public System.String ScadaRadioSerialNum { get; set; }



        /*New fields Settingsgaps*/
        [SettingsValidatorAttribute("Capacitor", "FLISR_ENGINEERING_COMMENTS")]
        [Display(Name = "FLISR Comments :")]
        public System.String FLISREngineeringComments { get; set; }

        [SettingsValidatorAttribute("Capacitor", "ENGINEERING_COMMENTS")]
        [Display(Name = "Engineering Comments :")]
        public System.String EngineeringComments { get; set; }


        [SettingsValidatorAttribute("Capacitor", "RTU_EXISTS")]
        [Display(Name = "External RTU :")]
        public System.String RTUExists { get; set; }


        [SettingsValidatorAttribute("Capacitor", "RTU_MANF_CD")]
        [Display(Name = "RTU Manufacturer :")]
        public System.String RTUManufacture { get; set; }


        [SettingsValidatorAttribute("Capacitor", "RTU_MODEL_NUM")]
        [Display(Name = "RTU Model # :")]
        public System.String RTUModelNumber { get; set; }


        [SettingsValidatorAttribute("Capacitor", "RTU_SERIAL_NUM")]
        [Display(Name = "RTU Serial # :")]
        public System.String RTUSerialNumber { get; set; }


        [SettingsValidatorAttribute("Capacitor", "RTU_SOFTWARE_VERSION")]
        [Display(Name = "RTU Software version :")]
        public System.String RTUSoftwareVersion { get; set; }


        [SettingsValidatorAttribute("Capacitor", "RTU_FIRMWARE_VERSION")]
        [Display(Name = "RTU Firmware version :")]
        public System.String RTUFirmwareVersion { get; set; }

        public SelectList RTUManufactureList { get; set; }

        /*end of changes settingsgaps*/

        //INC000004112585 Start
        [SettingsValidatorAttribute("Capacitor", "SEASON_OFF")]
        [Display(Name = "Season Off :")]
        public System.String SeasonOff { get; set; }
        //INC000004112585 End

        //INC000004165708 start
        [SettingsValidatorAttribute("Capacitor", "SCH1_BANK_POSITION")]
        [Display(Name = "Bank Position :")]
        public System.String Sch1BankPosition { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_BANK_POSITION")]
        [Display(Name = "Bank Position :")]
        public System.String Sch2BankPosition { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_BANK_POSITION")]
        [Display(Name = "Bank Position :")]
        public System.String Sch3BankPosition { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_BANK_POSITION")]
        [Display(Name = "Bank Position :")]
        public System.String Sch4BankPosition { get; set; }
        //INC000004165708 end

        //INC000004165778 start
        [SettingsValidatorAttribute("Capacitor", "SCH1_TIME_ON2")]
        [Display(Name = "Time on :")]
        public System.Int16? Sch1_TimeOn2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_TIME_OFF2")]
        [Display(Name = "Time off :")]
        public System.Int16? Sch1_TimeOff2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_WEEKDAYS2")]
        [Display(Name = "Inactive Weekdays :")]
        public System.String Sch1_Weekdays2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_SATURDAY2")]
        [Display(Name = "Inactive Saturday :")]
        public System.String Sch1_Saturday2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_SUNDAY2")]
        [Display(Name = "Inactive Sunday :")]
        public System.String Sch1_Sunday2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_HOLIDAYS2")]
        [Display(Name = "Inactive Holidays :")]
        public System.String Sch1_Holidays2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_TIME_ON2")]
        [Display(Name = "Time on :")]
        public System.Int16? Sch2_TimeOn2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_TIME_OFF2")]
        [Display(Name = "Time off :")]
        public System.Int16? Sch2_TimeOff2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_WEEKDAYS2")]
        [Display(Name = "Inactive Weekdays :")]
        public System.String Sch2_Weekdays2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_SATURDAY2")]
        [Display(Name = "Inactive Saturday2 :")]
        public System.String Sch2_Saturday2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_SUNDAY2")]
        [Display(Name = "Inactive Sunday :")]
        public System.String Sch2_Sunday2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_HOLIDAYS2")]
        [Display(Name = "Inactive Holidays :")]
        public System.String Sch2_Holidays2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_TIME_ON2")]
        [Display(Name = "Time on :")]
        public System.Int16? Sch3_TimeOn2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_TIME_OFF2")]
        [Display(Name = "Time off :")]
        public System.Int16? Sch3_TimeOff2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_WEEKDAYS2")]
        [Display(Name = "Inactive Weekdays :")]
        public System.String Sch3_Weekdays2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_SATURDAY2")]
        [Display(Name = "Inactive Saturday :")]
        public System.String Sch3_Saturday2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_SUNDAY2")]
        [Display(Name = "Inactive Sunday :")]
        public System.String Sch3_Sunday2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_HOLIDAYS2")]
        [Display(Name = "Inactive Holidays :")]
        public System.String Sch3_Holidays2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_TIME_ON2")]
        [Display(Name = "Time on :")]
        public System.Int16? Sch4_TimeOn2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_TIME_OFF2")]
        [Display(Name = "Time off :")]
        public System.Int16? Sch4_TimeOff2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_WEEKDAYS2")]
        [Display(Name = "Inactive Weekdays :")]
        public System.String Sch4_Weekdays2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_SATURDAY2")]
        [Display(Name = "Inactive Saturday :")]
        public System.String Sch4_Saturday2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_SUNDAY2")]
        [Display(Name = "Inactive Sunday :")]
        public System.String Sch4_Sunday2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_HOLIDAYS2")]
        [Display(Name = "Inactive Holidays :")]
        public System.String Sch4_Holidays2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_MONTHUR")]
        [Display(Name = "Inactive Mon-Thur :")]
        public System.String Sch1_MonThur { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_MONTHUR")]
        [Display(Name = "Inactive Mon-Thur :")]
        public System.String Sch2_MonThur { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_MONTHUR")]
        [Display(Name = "Inactive Mon-Thur :")]
        public System.String Sch3_MonThur { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_MONTHUR")]
        [Display(Name = "Inactive Mon-Thur :")]
        public System.String Sch4_MonThur { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_MONTHUR2")]
        [Display(Name = "Inactive Mon-Thur :")]
        public System.String Sch1_MonThur2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_MONTHUR2")]
        [Display(Name = "Inactive Mon-Thur :")]
        public System.String Sch2_MonThur2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_MONTHUR2")]
        [Display(Name = "Inactive Mon-Thur :")]
        public System.String Sch3_MonThur2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_MONTHUR2")]
        [Display(Name = "Inactive Mon-Thur :")]
        public System.String Sch4_MonThur2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_FRISUN")]
        [Display(Name = "Inactive Fri-Sun :")]
        public System.String Sch1_FriSun { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_FRISUN")]
        [Display(Name = "Inactive Fri-Sun :")]
        public System.String Sch2_FriSun { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_FRISUN")]
        [Display(Name = "Inactive Fri-Sun :")]
        public System.String Sch3_FriSun { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_FRISUN")]
        [Display(Name = "Inactive Fri-Sun :")]
        public System.String Sch4_FriSun { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH1_FRISUN2")]
        [Display(Name = "Inactive Fri-Sun :")]
        public System.String Sch1_FriSun2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH2_FRISUN2")]
        [Display(Name = "Inactive Fri-Sun :")]
        public System.String Sch2_FriSun2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH3_FRISUN2")]
        [Display(Name = "Inactive Fri-Sun :")]
        public System.String Sch3_FriSun2 { get; set; }

        [SettingsValidatorAttribute("Capacitor", "SCH4_FRISUN2")]
        [Display(Name = "Inactive Fri-Sun :")]
        public System.String Sch4_FriSun2 { get; set; }
        //INC000004165778 end

        public bool Release { get; set; }
        public List<GISAttributes> GISAttributes { get; set; }

        public SelectList AutoBvrCalcList { get; set; }
        public SelectList ControlTypeList { get; set; }
        public SelectList ControllerUnitModelList { get; set; }
        public SelectList ControllerUnitTypeList { get; set; }
        public SelectList LogIntervalList { get; set; }
        public SelectList DaylightTimeList { get; set; }
        public SelectList EngineeringDocumentList { get; set; }
        //public SelectList BankPositionList { get; set; } //INC000004165708
        public SelectList ScadaRadioManufacturerList { get; set; }
        public SelectList ScadaTypeList { get; set; }
        public SelectList Sch1_ControlStrategyList { get; set; }
        public SelectList Sch2_ControlStrategyList { get; set; }
        public SelectList Sch3_ControlStrategyList { get; set; }
        public SelectList Sch4_ControlStrategyList { get; set; }
        public SelectList Sch1_ScheduleUnitTypeList { get; set; }
        public SelectList Sch2_ScheduleUnitTypeList { get; set; }
        public SelectList Sch3_ScheduleUnitTypeList { get; set; }
        public SelectList Sch4_ScheduleUnitTypeList { get; set; }
        public SelectList SwitchPositionList { get; set; }
        public SelectList TempOverrideList { get; set; }
        public SelectList SeasonOffList { get; set; }  //INC000004112585 

        //INC000004165708 start
        public SelectList Sch1BankPositionList { get; set; }
        public SelectList Sch2BankPositionList { get; set; }
        public SelectList Sch3BankPositionList { get; set; }
        public SelectList Sch4BankPositionList { get; set; }
        //INC000004165708 end

        public string DropDownPostbackScript { get; set; }

        public HashSet<string> FieldsToDisplay { get; set; }

        public void PopulateEntityFromModel(SM_CAPACITOR e)
        {
            e.DEVICE_ID = this.DeviceID;
            e.PREPARED_BY = this.PreparedBy;
            e.CONTROL_SERIAL_NUM = this.ControllerSerialNum;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            //e.NOTES = this.Notes;
            //e.OK_TO_BYPASS = this.OkToBypass;
            e.CONTROL_TYPE = this.ControlType;
            e.SCH1_SCHEDULE = this.Sch1_ScheduleUnitType;
            e.SCH1_CONTROL_STRATEGY = this.Sch1_ControlStrategy;
            e.SCH1_TIME_ON = this.Sch1_TimeOn;
            e.SCH1_TIME_OFF = this.Sch1_TimeOff;
            e.SCH1_LOW_VOLTAGE_SETPOINT = this.Sch1_LowVoltageSetPoint;
            e.SCH1_HIGH_VOLTAGE_SETPOINT = this.Sch1_HighVoltageSetPoint;
            e.SCH1_TEMP_SETPOINT_ON = this.Sch1_TempSetPointOn;
            e.SCH1_TEMP_SETPOINT_OFF = this.Sch1_TempSetPointOff;
            e.SCH1_START_DATE = Utility.ParseDateTime(this.Sch1_StartDate);
            e.SCH1_END_DATE = Utility.ParseDateTime(this.Sch1_EndDate);
            e.SCH1_WEEKDAYS = this.Sch1_Weekdays;
            e.SCH1_SATURDAY = this.Sch1_Saturday;
            e.SCH1_SUNDAY = this.Sch1_Sunday;
            e.SCH1_HOLIDAYS = this.Sch1_Holidays;
            e.SCH1_KVAR_SETPOINT_ON = this.Sch1_KvarSetPointOn;
            e.SCH1_KVAR_SETPOINT_OFF = this.Sch1_KvarSetPointOff;
            e.SCH2_SCHEDULE = this.Sch2_ScheduleUnitType;
            e.SCH2_CONTROL_STRATEGY = this.Sch2_ControlStrategy;
            e.SCH2_TIME_ON = this.Sch2_TimeOn;
            e.SCH2_TIME_OFF = this.Sch2_TimeOff;
            e.SCH2_LOW_VOLTAGE_SETPOINT = this.Sch2_LowVoltageSetPoint;
            e.SCH2_HIGH_VOLTAGE_SETPOINT = this.Sch2_HighVoltageSetPoint;
            e.SCH2_TEMP_SETPOINT_ON = this.Sch2_TempSetPointOn;
            e.SCH2_TEMP_SETPOINT_OFF = this.Sch2_TempSetPointOff;
            e.SCH2_START_DATE = Utility.ParseDateTime(this.Sch2_StartDate);
            e.SCH2_END_DATE = Utility.ParseDateTime(this.Sch2_EndDate);
            e.SCH2_WEEKDAYS = this.Sch2_Weekdays;
            e.SCH2_SATURDAY = this.Sch2_Saturday;
            e.SCH2_SUNDAY = this.Sch2_Sunday;
            e.SCH2_HOLIDAYS = this.Sch2_Holidays;
            e.SCH2_KVAR_SETPOINT_ON = this.Sch2_KvarSetPointOn;
            e.SCH2_KVAR_SETPOINT_OFF = this.Sch2_KvarSetPointOff;
            e.SCH3_SCHEDULE = this.Sch3_ScheduleUnitType;
            e.SCH3_CONTROL_STRATEGY = this.Sch3_ControlStrategy;
            e.SCH3_TIME_ON = this.Sch3_TimeOn;
            e.SCH3_TIME_OFF = this.Sch3_TimeOff;
            e.SCH3_LOW_VOLTAGE_SETPOINT = this.Sch3_LowVoltageSetPoint;
            e.SCH3_HIGH_VOLTAGE_SETPOINT = this.Sch3_HighVoltageSetPoint;
            e.SCH3_TEMP_SETPOINT_ON = this.Sch3_TempSetPointOn;
            e.SCH3_TEMP_SETPOINT_OFF = this.Sch3_TempSetPointOff;
            e.SCH3_START_DATE = Utility.ParseDateTime(this.Sch3_StartDate);
            e.SCH3_END_DATE = Utility.ParseDateTime(this.Sch3_EndDate);
            e.SCH3_WEEKDAYS = this.Sch3_Weekdays;
            e.SCH3_SATURDAY = this.Sch3_Saturday;
            e.SCH3_SUNDAY = this.Sch3_Sunday;
            e.SCH3_HOLIDAYS = this.Sch3_Holidays;
            e.SCH3_KVAR_SETPOINT_ON = this.Sch3_KvarSetPointOn;
            e.SCH3_KVAR_SETPOINT_OFF = this.Sch3_KvarSetPointOff;
            e.SCH4_SCHEDULE = this.Sch4_ScheduleUnitType;
            e.SCH4_CONTROL_STRATEGY = this.Sch4_ControlStrategy;
            e.SCH4_TIME_ON = this.Sch4_TimeOn;
            e.SCH4_TIME_OFF = this.Sch4_TimeOff;
            e.SCH4_LOW_VOLTAGE_SETPOINT = this.Sch4_LowVoltageSetPoint;
            e.SCH4_HIGH_VOLTAGE_SETPOINT = this.Sch4_HighVoltageSetPoint;
            e.SCH4_TEMP_SETPOINT_ON = this.Sch4_TempSetPointOn;
            e.SCH4_TEMP_SETPOINT_OFF = this.Sch4_TempSetPointOff;
            e.SCH4_START_DATE = Utility.ParseDateTime(this.Sch4_StartDate);
            e.SCH4_END_DATE = Utility.ParseDateTime(this.Sch4_EndDate);
            e.SCH4_WEEKDAYS = this.Sch4_Weekdays;
            e.SCH4_SATURDAY = this.Sch4_Saturday;
            e.SCH4_SUNDAY = this.Sch4_Sunday;
            e.SCH4_HOLIDAYS = this.Sch4_Holidays;
            e.SCH4_KVAR_SETPOINT_ON = this.Sch4_KvarSetPointOn;
            e.SCH4_KVAR_SETPOINT_OFF = this.Sch4_KvarSetPointOff;
            //e.CONTROLLER_UNIT_TYPE = this.ControllerUnitType;
            e.CONTROLLER_UNIT_MODEL = this.ControllerUnitModel;
            e.VOLT_VAR_TEAM_MEMBER = this.VoltVarTeamMember;
            e.SWITCH_POSITION = this.SwitchPosition;
            //e.PREFERED_BANK_POSITION = this.BankPosition; //INC000004165708
            e.MAXCYCLES = this.MaxCycles;
            e.DAYLIGHT_SAVINGS_TIME = this.DaylightTime;
            e.EST_VOLTAGE_CHANGE = this.ESTVoltageChangeTime;
            e.VOLTAGE_OVERRIDE_TIME = this.VoltageOverrideTime;
            e.HIGH_VOLTAGE_OVERRIDE_SETPOINT = this.HighVoltageOverrideSetPoint;
            e.LOW_VOLTAGE_OVERRIDE_SETPOINT = this.LowVoltageOverrideSetPoint;
            e.VOLTAGE_CHANGE_TIME = this.VoltageChangeTime;
            e.TEMPERATURE_OVERRIDE = this.TempOverride;
            e.TEMPERATURE_CHANGE_TIME = this.TempChangeTime;
            e.EST_BANK_VOLTAGE_RISE = this.EstBankVoltageRise;
            e.AUTO_BVR_CALC = this.AutoBvrCalc;
            e.DATA_LOGGING_INTERVAL = this.LogInterval;
            e.PULSE_TIME = this.PulseTime;
            e.MIN_SW_VOLTAGE = this.MinSwitchVolt;
            e.TIME_DELAY = this.Delay;
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
            e.SEASON_OFF = this.SeasonOff;//INC000004112585

            //INC000004165708 start
            e.SCH1_BANK_POSITION = this.Sch1BankPosition;
            e.SCH2_BANK_POSITION = this.Sch2BankPosition;
            e.SCH3_BANK_POSITION = this.Sch3BankPosition;
            e.SCH4_BANK_POSITION = this.Sch4BankPosition;
            //INC000004165708 end

            //INC000004165778 start
            e.SCH1_TIME_ON2 = this.Sch1_TimeOn2;
            e.SCH1_TIME_OFF2 = this.Sch1_TimeOff2;
            e.SCH1_WEEKDAYS2 = this.Sch1_Weekdays2;
            e.SCH1_SATURDAY2 = this.Sch1_Saturday2;
            e.SCH1_SUNDAY2 = this.Sch1_Sunday2;
            e.SCH1_HOLIDAYS2 = this.Sch1_Holidays2;
            e.SCH2_TIME_ON2 = this.Sch2_TimeOn2;
            e.SCH2_TIME_OFF2 = this.Sch2_TimeOff2;
            e.SCH2_WEEKDAYS2 = this.Sch2_Weekdays2;
            e.SCH2_SATURDAY2 = this.Sch2_Saturday2;
            e.SCH2_SUNDAY2 = this.Sch2_Sunday2;
            e.SCH2_HOLIDAYS2 = this.Sch2_Holidays2;
            e.SCH3_TIME_ON2 = this.Sch3_TimeOn2;
            e.SCH3_TIME_OFF2 = this.Sch3_TimeOff2;
            e.SCH3_WEEKDAYS2 = this.Sch3_Weekdays2;
            e.SCH3_SATURDAY2 = this.Sch3_Saturday2;
            e.SCH3_SUNDAY2 = this.Sch3_Sunday2;
            e.SCH3_HOLIDAYS2 = this.Sch3_Holidays2;
            e.SCH4_TIME_ON2 = this.Sch4_TimeOn2;
            e.SCH4_TIME_OFF2 = this.Sch4_TimeOff2;
            e.SCH4_WEEKDAYS2 = this.Sch4_Weekdays2;
            e.SCH4_SATURDAY2 = this.Sch4_Saturday2;
            e.SCH4_SUNDAY2 = this.Sch4_Sunday2;
            e.SCH4_HOLIDAYS2 = this.Sch4_Holidays2;
            e.SCH1_MONTHUR = this.Sch1_MonThur;
            e.SCH1_MONTHUR2 = this.Sch1_MonThur2;
            e.SCH2_MONTHUR = this.Sch2_MonThur;
            e.SCH2_MONTHUR2 = this.Sch2_MonThur2;
            e.SCH3_MONTHUR = this.Sch3_MonThur;
            e.SCH3_MONTHUR2 = this.Sch3_MonThur2;
            e.SCH4_MONTHUR = this.Sch4_MonThur;
            e.SCH4_MONTHUR2 = this.Sch4_MonThur2;
            e.SCH1_FRISUN = this.Sch1_FriSun;
            e.SCH1_FRISUN2 = this.Sch1_FriSun2;
            e.SCH2_FRISUN = this.Sch2_FriSun;
            e.SCH2_FRISUN2 = this.Sch2_FriSun2;
            e.SCH3_FRISUN = this.Sch3_FriSun;
            e.SCH3_FRISUN2 = this.Sch3_FriSun2;
            e.SCH4_FRISUN = this.Sch4_FriSun;
            e.SCH4_FRISUN2 = this.Sch4_FriSun2;
            //INC000004165778 end
        }

        public void PopulateModelFromHistoryEntity(SM_CAPACITOR_HIST e)
        {
            this.AutoBvrCalc = e.AUTO_BVR_CALC;
            this.BaudRate = e.BAUD_RATE;
            this.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
            this.ControllerUnitModel = e.CONTROLLER_UNIT_MODEL;
            this.ControlType = e.CONTROL_TYPE;
            this.LogInterval = e.DATA_LOGGING_INTERVAL;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.DaylightTime = e.DAYLIGHT_SAVINGS_TIME;
            this.DeviceID = e.DEVICE_ID;
            //this.EngineeringDocument = e.ENGINEERING_DOCUMENT;
            this.EstBankVoltageRise = e.EST_BANK_VOLTAGE_RISE;
            this.ESTVoltageChangeTime = e.EST_VOLTAGE_CHANGE;
            this.FirmwareVersion = e.FIRMWARE_VERSION;
            this.HighVoltageOverrideSetPoint = e.HIGH_VOLTAGE_OVERRIDE_SETPOINT;
            this.LowVoltageOverrideSetPoint = e.LOW_VOLTAGE_OVERRIDE_SETPOINT;
            this.MasterStation = e.MASTER_STATION;
            this.MaxCycles = e.MAXCYCLES;
            this.MinSwitchVolt = e.MIN_SW_VOLTAGE;
            //this.Notes = e.NOTES;
            //this.OkToBypass = e.OK_TO_BYPASS;
            this.OperatingNumber = e.OPERATING_NUM;
            this.PeerReviewer = e.PEER_REVIEW_BY;
            //this.BankPosition = e.PREFERED_BANK_POSITION; //INC000004165708
            this.PreparedBy = e.PREPARED_BY;
            this.PulseTime = e.PULSE_TIME;
            this.ScadaRadioManufacturer = e.RADIO_MANF_CD;
            this.ScadaRadioModelNum = e.RADIO_MODEL_NUM;
            this.ScadaRadioSerialNum = e.RADIO_SERIAL_NUM;
            this.Repeater = e.REPEATER;
            this.RtuAddress = e.RTU_ADDRESS;
            this.Scada = e.SCADA;
            this.ScadaType = e.SCADA_TYPE;
            this.Sch1_ControlStrategy = e.SCH1_CONTROL_STRATEGY;
            this.Sch1_EndDate = Utility.ParseDateTimeNoYear(e.SCH1_END_DATE);
            this.Sch1_HighVoltageSetPoint = e.SCH1_HIGH_VOLTAGE_SETPOINT;
            this.Sch1_Holidays = e.SCH1_HOLIDAYS;
            this.Sch1_KvarSetPointOff = e.SCH1_KVAR_SETPOINT_OFF;
            this.Sch1_KvarSetPointOn = e.SCH1_KVAR_SETPOINT_ON;
            this.Sch1_LowVoltageSetPoint = e.SCH1_LOW_VOLTAGE_SETPOINT;
            this.Sch1_Saturday = e.SCH1_SATURDAY;
            this.Sch1_ScheduleUnitType = e.SCH1_SCHEDULE;
            this.Sch1_StartDate = Utility.ParseDateTimeNoYear(e.SCH1_START_DATE);
            this.Sch1_Sunday = e.SCH1_SUNDAY;
            this.Sch1_TempSetPointOff = e.SCH1_TEMP_SETPOINT_OFF;
            this.Sch1_TempSetPointOn = e.SCH1_TEMP_SETPOINT_ON;
            this.Sch1_TimeOff = e.SCH1_TIME_OFF;
            this.Sch1_TimeOn = e.SCH1_TIME_ON;
            this.Sch1_Weekdays = e.SCH1_WEEKDAYS;
            this.Sch2_ControlStrategy = e.SCH2_CONTROL_STRATEGY;
            this.Sch2_EndDate = Utility.ParseDateTimeNoYear(e.SCH2_END_DATE);
            this.Sch2_HighVoltageSetPoint = e.SCH2_HIGH_VOLTAGE_SETPOINT;
            this.Sch2_Holidays = e.SCH2_HOLIDAYS;
            this.Sch2_KvarSetPointOff = e.SCH2_KVAR_SETPOINT_OFF;
            this.Sch2_KvarSetPointOn = e.SCH2_KVAR_SETPOINT_ON;
            this.Sch2_LowVoltageSetPoint = e.SCH2_LOW_VOLTAGE_SETPOINT;
            this.Sch2_Saturday = e.SCH2_SATURDAY;
            this.Sch2_ScheduleUnitType = e.SCH2_SCHEDULE;
            this.Sch2_StartDate = Utility.ParseDateTimeNoYear(e.SCH2_START_DATE);
            this.Sch2_Sunday = e.SCH2_SUNDAY;
            this.Sch2_TempSetPointOff = e.SCH2_TEMP_SETPOINT_OFF;
            this.Sch2_TempSetPointOn = e.SCH2_TEMP_SETPOINT_ON;
            this.Sch2_TimeOff = e.SCH2_TIME_OFF;
            this.Sch2_TimeOn = e.SCH2_TIME_ON;
            this.Sch2_Weekdays = e.SCH2_WEEKDAYS;
            this.Sch3_ControlStrategy = e.SCH3_CONTROL_STRATEGY;
            this.Sch3_EndDate = Utility.ParseDateTimeNoYear(e.SCH3_END_DATE);
            this.Sch3_HighVoltageSetPoint = e.SCH3_HIGH_VOLTAGE_SETPOINT;
            this.Sch3_Holidays = e.SCH3_HOLIDAYS;
            this.Sch3_KvarSetPointOff = e.SCH3_KVAR_SETPOINT_OFF;
            this.Sch3_KvarSetPointOn = e.SCH3_KVAR_SETPOINT_ON;
            this.Sch3_LowVoltageSetPoint = e.SCH3_LOW_VOLTAGE_SETPOINT;
            this.Sch3_Saturday = e.SCH3_SATURDAY;
            this.Sch3_ScheduleUnitType = e.SCH3_SCHEDULE;
            this.Sch3_StartDate = Utility.ParseDateTimeNoYear(e.SCH3_START_DATE);
            this.Sch3_Sunday = e.SCH3_SUNDAY;
            this.Sch3_TempSetPointOff = e.SCH3_TEMP_SETPOINT_OFF;
            this.Sch3_TempSetPointOn = e.SCH3_TEMP_SETPOINT_ON;
            this.Sch3_TimeOff = e.SCH3_TIME_OFF;
            this.Sch3_TimeOn = e.SCH3_TIME_ON;
            this.Sch3_Weekdays = e.SCH3_WEEKDAYS;
            this.Sch4_ControlStrategy = e.SCH4_CONTROL_STRATEGY;
            this.Sch4_EndDate = Utility.ParseDateTimeNoYear(e.SCH4_END_DATE);
            this.Sch4_HighVoltageSetPoint = e.SCH4_HIGH_VOLTAGE_SETPOINT;
            this.Sch4_Holidays = e.SCH4_HOLIDAYS;
            this.Sch4_KvarSetPointOff = e.SCH4_KVAR_SETPOINT_OFF;
            this.Sch4_KvarSetPointOn = e.SCH4_KVAR_SETPOINT_ON;
            this.Sch4_LowVoltageSetPoint = e.SCH4_LOW_VOLTAGE_SETPOINT;
            this.Sch4_Saturday = e.SCH4_SATURDAY;
            this.Sch4_ScheduleUnitType = e.SCH4_SCHEDULE;
            this.Sch4_StartDate = Utility.ParseDateTimeNoYear(e.SCH4_START_DATE);
            this.Sch4_Sunday = e.SCH4_SUNDAY;
            this.Sch4_TempSetPointOff = e.SCH4_TEMP_SETPOINT_OFF;
            this.Sch4_TempSetPointOn = e.SCH4_TEMP_SETPOINT_ON;
            this.Sch4_TimeOff = e.SCH4_TIME_OFF;
            this.Sch4_TimeOn = e.SCH4_TIME_ON;
            this.Sch4_Weekdays = e.SCH4_WEEKDAYS;
            this.SoftwareVersion = e.SOFTWARE_VERSION;
            this.SpecialConditions = e.SPECIAL_CONDITIONS;
            this.SwitchPosition = e.SWITCH_POSITION;
            this.TempChangeTime = e.TEMPERATURE_CHANGE_TIME;
            this.TempOverride = e.TEMPERATURE_OVERRIDE;
            this.Delay = e.TIME_DELAY;
            this.TransmitDisableDelay = e.TRANSMIT_DISABLE_DELAY;
            this.TransmitEnableDelay = e.TRANSMIT_ENABLE_DELAY;
            this.VoltVarTeamMember = e.VOLT_VAR_TEAM_MEMBER;
            this.VoltageChangeTime = e.VOLTAGE_CHANGE_TIME;
            this.VoltageOverrideTime = e.VOLTAGE_OVERRIDE_TIME;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
            this.FLISREngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
            this.SeasonOff = e.SEASON_OFF;//INC000004112585

            //INC000004165708 start
            this.Sch1BankPosition = e.SCH1_BANK_POSITION;
            this.Sch2BankPosition = e.SCH2_BANK_POSITION;
            this.Sch3BankPosition = e.SCH3_BANK_POSITION;
            this.Sch4BankPosition = e.SCH4_BANK_POSITION;
            //INC000004165708 end

            //INC000004165778 start
            this.Sch1_TimeOn2 = e.SCH1_TIME_ON2;
            this.Sch1_TimeOff2 = e.SCH1_TIME_OFF2;
            this.Sch1_Weekdays2 = e.SCH1_WEEKDAYS2;
            this.Sch1_Saturday2 = e.SCH1_SATURDAY2;
            this.Sch1_Sunday2 = e.SCH1_SUNDAY2;
            this.Sch1_Holidays2 = e.SCH1_HOLIDAYS2;
            this.Sch2_TimeOn2 = e.SCH2_TIME_ON2;
            this.Sch2_TimeOff2 = e.SCH2_TIME_OFF2;
            this.Sch2_Weekdays2 = e.SCH2_WEEKDAYS2;
            this.Sch2_Saturday2 = e.SCH2_SATURDAY2;
            this.Sch2_Sunday2 = e.SCH2_SUNDAY2;
            this.Sch2_Holidays2 = e.SCH2_HOLIDAYS2;
            this.Sch3_TimeOn2 = e.SCH3_TIME_ON2;
            this.Sch3_TimeOff2 = e.SCH3_TIME_OFF2;
            this.Sch3_Weekdays2 = e.SCH3_WEEKDAYS2;
            this.Sch3_Saturday2 = e.SCH3_SATURDAY2;
            this.Sch3_Sunday2 = e.SCH3_SUNDAY2;
            this.Sch3_Holidays2 = e.SCH3_HOLIDAYS2;
            this.Sch4_TimeOn2 = e.SCH4_TIME_ON2;
            this.Sch4_TimeOff2 = e.SCH4_TIME_OFF2;
            this.Sch4_Weekdays2 = e.SCH4_WEEKDAYS2;
            this.Sch4_Saturday2 = e.SCH4_SATURDAY2;
            this.Sch4_Sunday2 = e.SCH4_SUNDAY2;
            this.Sch4_Holidays2 = e.SCH4_HOLIDAYS2;
            this.Sch1_MonThur = e.SCH1_MONTHUR;
            this.Sch1_MonThur2 = e.SCH1_MONTHUR2;
            this.Sch2_MonThur = e.SCH2_MONTHUR;
            this.Sch2_MonThur2 = e.SCH2_MONTHUR2;
            this.Sch3_MonThur = e.SCH3_MONTHUR;
            this.Sch3_MonThur2 = e.SCH3_MONTHUR2;
            this.Sch4_MonThur = e.SCH4_MONTHUR;
            this.Sch4_MonThur2 = e.SCH4_MONTHUR2;
            this.Sch1_FriSun = e.SCH1_FRISUN;
            this.Sch1_FriSun2 = e.SCH1_FRISUN2;
            this.Sch2_FriSun = e.SCH2_FRISUN;
            this.Sch2_FriSun2 = e.SCH2_FRISUN2;
            this.Sch3_FriSun = e.SCH3_FRISUN;
            this.Sch3_FriSun2 = e.SCH3_FRISUN2;
            this.Sch4_FriSun = e.SCH4_FRISUN;
            this.Sch4_FriSun2 = e.SCH4_FRISUN2;
            //INC000004165778 end
        }

        public void PopulateModelFromEntity(SM_CAPACITOR e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceID = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            this.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            //this.Notes = e.NOTES;
            //this.OkToBypass = e.OK_TO_BYPASS;
            this.ControlType = e.CONTROL_TYPE;
            this.Sch1_ScheduleUnitType = e.SCH1_SCHEDULE;
            this.Sch1_ControlStrategy = e.SCH1_CONTROL_STRATEGY;
            this.Sch1_TimeOn = e.SCH1_TIME_ON;
            this.Sch1_TimeOff = e.SCH1_TIME_OFF;
            this.Sch1_LowVoltageSetPoint = e.SCH1_LOW_VOLTAGE_SETPOINT;
            this.Sch1_HighVoltageSetPoint = e.SCH1_HIGH_VOLTAGE_SETPOINT;
            this.Sch1_TempSetPointOn = e.SCH1_TEMP_SETPOINT_ON;
            this.Sch1_TempSetPointOff = e.SCH1_TEMP_SETPOINT_OFF;
            this.Sch1_StartDate = Utility.ParseDateTimeNoYear(e.SCH1_START_DATE);
            this.Sch1_EndDate = Utility.ParseDateTimeNoYear(e.SCH1_END_DATE);
            this.Sch1_Weekdays = e.SCH1_WEEKDAYS;
            this.Sch1_Saturday = e.SCH1_SATURDAY;
            this.Sch1_Sunday = e.SCH1_SUNDAY;
            this.Sch1_Holidays = e.SCH1_HOLIDAYS;
            this.Sch1_KvarSetPointOn = e.SCH1_KVAR_SETPOINT_ON;
            this.Sch1_KvarSetPointOff = e.SCH1_KVAR_SETPOINT_OFF;
            this.Sch2_ScheduleUnitType = e.SCH2_SCHEDULE;
            this.Sch2_ControlStrategy = e.SCH2_CONTROL_STRATEGY;
            this.Sch2_TimeOn = e.SCH2_TIME_ON;
            this.Sch2_TimeOff = e.SCH2_TIME_OFF;
            this.Sch2_LowVoltageSetPoint = e.SCH2_LOW_VOLTAGE_SETPOINT;
            this.Sch2_HighVoltageSetPoint = e.SCH2_HIGH_VOLTAGE_SETPOINT;
            this.Sch2_TempSetPointOn = e.SCH2_TEMP_SETPOINT_ON;
            this.Sch2_TempSetPointOff = e.SCH2_TEMP_SETPOINT_OFF;
            this.Sch2_StartDate = Utility.ParseDateTimeNoYear(e.SCH2_START_DATE);
            this.Sch2_EndDate = Utility.ParseDateTimeNoYear(e.SCH2_END_DATE);
            this.Sch2_Weekdays = e.SCH2_WEEKDAYS;
            this.Sch2_Saturday = e.SCH2_SATURDAY;
            this.Sch2_Sunday = e.SCH2_SUNDAY;
            this.Sch2_Holidays = e.SCH2_HOLIDAYS;
            this.Sch2_KvarSetPointOn = e.SCH2_KVAR_SETPOINT_ON;
            this.Sch2_KvarSetPointOff = e.SCH2_KVAR_SETPOINT_OFF;
            this.Sch3_ScheduleUnitType = e.SCH3_SCHEDULE;
            this.Sch3_ControlStrategy = e.SCH3_CONTROL_STRATEGY;
            this.Sch3_TimeOn = e.SCH3_TIME_ON;
            this.Sch3_TimeOff = e.SCH3_TIME_OFF;
            this.Sch3_LowVoltageSetPoint = e.SCH3_LOW_VOLTAGE_SETPOINT;
            this.Sch3_HighVoltageSetPoint = e.SCH3_HIGH_VOLTAGE_SETPOINT;
            this.Sch3_TempSetPointOn = e.SCH3_TEMP_SETPOINT_ON;
            this.Sch3_TempSetPointOff = e.SCH3_TEMP_SETPOINT_OFF;
            this.Sch3_StartDate = Utility.ParseDateTimeNoYear(e.SCH3_START_DATE);
            this.Sch3_EndDate = Utility.ParseDateTimeNoYear(e.SCH3_END_DATE);
            this.Sch3_Weekdays = e.SCH3_WEEKDAYS;
            this.Sch3_Saturday = e.SCH3_SATURDAY;
            this.Sch3_Sunday = e.SCH3_SUNDAY;
            this.Sch3_Holidays = e.SCH3_HOLIDAYS;
            this.Sch3_KvarSetPointOn = e.SCH3_KVAR_SETPOINT_ON;
            this.Sch3_KvarSetPointOff = e.SCH3_KVAR_SETPOINT_OFF;
            this.Sch4_ScheduleUnitType = e.SCH4_SCHEDULE;
            this.Sch4_ControlStrategy = e.SCH4_CONTROL_STRATEGY;
            this.Sch4_TimeOn = e.SCH4_TIME_ON;
            this.Sch4_TimeOff = e.SCH4_TIME_OFF;
            this.Sch4_LowVoltageSetPoint = e.SCH4_LOW_VOLTAGE_SETPOINT;
            this.Sch4_HighVoltageSetPoint = e.SCH4_HIGH_VOLTAGE_SETPOINT;
            this.Sch4_TempSetPointOn = e.SCH4_TEMP_SETPOINT_ON;
            this.Sch4_TempSetPointOff = e.SCH4_TEMP_SETPOINT_OFF;
            this.Sch4_StartDate = Utility.ParseDateTimeNoYear(e.SCH4_START_DATE);
            this.Sch4_EndDate = Utility.ParseDateTimeNoYear(e.SCH4_END_DATE);
            this.Sch4_Weekdays = e.SCH4_WEEKDAYS;
            this.Sch4_Saturday = e.SCH4_SATURDAY;
            this.Sch4_Sunday = e.SCH4_SUNDAY;
            this.Sch4_Holidays = e.SCH4_HOLIDAYS;
            this.Sch4_KvarSetPointOn = e.SCH4_KVAR_SETPOINT_ON;
            this.Sch4_KvarSetPointOff = e.SCH4_KVAR_SETPOINT_OFF;
            //this.ControllerUnitType = e.CONTROLLER_UNIT_TYPE;
            this.ControllerUnitModel = e.CONTROLLER_UNIT_MODEL;
            this.VoltVarTeamMember = e.VOLT_VAR_TEAM_MEMBER;
            this.SwitchPosition = e.SWITCH_POSITION;
            //this.BankPosition = e.PREFERED_BANK_POSITION; //INC000004165708
            this.MaxCycles = e.MAXCYCLES;
            this.DaylightTime = e.DAYLIGHT_SAVINGS_TIME;
            this.ESTVoltageChangeTime = e.EST_VOLTAGE_CHANGE;
            this.VoltageOverrideTime = e.VOLTAGE_OVERRIDE_TIME;
            this.HighVoltageOverrideSetPoint = e.HIGH_VOLTAGE_OVERRIDE_SETPOINT;
            this.LowVoltageOverrideSetPoint = e.LOW_VOLTAGE_OVERRIDE_SETPOINT;
            this.VoltageChangeTime = e.VOLTAGE_CHANGE_TIME;
            this.TempOverride = e.TEMPERATURE_OVERRIDE;
            this.TempChangeTime = e.TEMPERATURE_CHANGE_TIME;
            this.EstBankVoltageRise = e.EST_BANK_VOLTAGE_RISE;
            this.AutoBvrCalc = e.AUTO_BVR_CALC;
            this.LogInterval = e.DATA_LOGGING_INTERVAL;
            this.PulseTime = e.PULSE_TIME;
            this.MinSwitchVolt = e.MIN_SW_VOLTAGE;
            this.Delay = e.TIME_DELAY;
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
            this.SeasonOff = e.SEASON_OFF;//INC000004112585

            //INC000004165708 START
            this.Sch1BankPosition = e.SCH1_BANK_POSITION;
            this.Sch2BankPosition = e.SCH2_BANK_POSITION;
            this.Sch3BankPosition = e.SCH3_BANK_POSITION;
            this.Sch4BankPosition = e.SCH4_BANK_POSITION;
            //INC000004165708 END

            //INC000004165778 start
            this.Sch1_TimeOn2 = e.SCH1_TIME_ON2;
            this.Sch1_TimeOff2 = e.SCH1_TIME_OFF2;
            this.Sch1_Weekdays2 = e.SCH1_WEEKDAYS2;
            this.Sch1_Saturday2 = e.SCH1_SATURDAY2;
            this.Sch1_Sunday2 = e.SCH1_SUNDAY2;
            this.Sch1_Holidays2 = e.SCH1_HOLIDAYS2;
            this.Sch2_TimeOn2 = e.SCH2_TIME_ON2;
            this.Sch2_TimeOff2 = e.SCH2_TIME_OFF2;
            this.Sch2_Weekdays2 = e.SCH2_WEEKDAYS2;
            this.Sch2_Saturday2 = e.SCH2_SATURDAY2;
            this.Sch2_Sunday2 = e.SCH2_SUNDAY2;
            this.Sch2_Holidays2 = e.SCH2_HOLIDAYS2;
            this.Sch3_TimeOn2 = e.SCH3_TIME_ON2;
            this.Sch3_TimeOff2 = e.SCH3_TIME_OFF2;
            this.Sch3_Weekdays2 = e.SCH3_WEEKDAYS2;
            this.Sch3_Saturday2 = e.SCH3_SATURDAY2;
            this.Sch3_Sunday2 = e.SCH3_SUNDAY2;
            this.Sch3_Holidays2 = e.SCH3_HOLIDAYS2;
            this.Sch4_TimeOn2 = e.SCH4_TIME_ON2;
            this.Sch4_TimeOff2 = e.SCH4_TIME_OFF2;
            this.Sch4_Weekdays2 = e.SCH4_WEEKDAYS2;
            this.Sch4_Saturday2 = e.SCH4_SATURDAY2;
            this.Sch4_Sunday2 = e.SCH4_SUNDAY2;
            this.Sch4_Holidays2 = e.SCH4_HOLIDAYS2;
            this.Sch1_MonThur = e.SCH1_MONTHUR;
            this.Sch1_MonThur2 = e.SCH1_MONTHUR2;
            this.Sch2_MonThur = e.SCH2_MONTHUR;
            this.Sch2_MonThur2 = e.SCH2_MONTHUR2;
            this.Sch3_MonThur = e.SCH3_MONTHUR;
            this.Sch3_MonThur2 = e.SCH3_MONTHUR2;
            this.Sch4_MonThur = e.SCH4_MONTHUR;
            this.Sch4_MonThur2 = e.SCH4_MONTHUR2;
            this.Sch1_FriSun = e.SCH1_FRISUN;
            this.Sch1_FriSun2 = e.SCH1_FRISUN2;
            this.Sch2_FriSun = e.SCH2_FRISUN;
            this.Sch2_FriSun2 = e.SCH2_FRISUN2;
            this.Sch3_FriSun = e.SCH3_FRISUN;
            this.Sch3_FriSun2 = e.SCH3_FRISUN2;
            this.Sch4_FriSun = e.SCH4_FRISUN;
            this.Sch4_FriSun2 = e.SCH4_FRISUN2;
            //INC000004165778 end
        }

        public void PopulateHistoryFromEntity(SM_CAPACITOR_HIST entityHistory, SM_CAPACITOR e)
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
            //entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
            entityHistory.CONTROL_TYPE = e.CONTROL_TYPE;
            entityHistory.SCH1_SCHEDULE = e.SCH1_SCHEDULE;
            entityHistory.SCH1_CONTROL_STRATEGY = e.SCH1_CONTROL_STRATEGY;
            entityHistory.SCH1_TIME_ON = e.SCH1_TIME_ON;
            entityHistory.SCH1_TIME_OFF = e.SCH1_TIME_OFF;
            entityHistory.SCH1_LOW_VOLTAGE_SETPOINT = e.SCH1_LOW_VOLTAGE_SETPOINT;
            entityHistory.SCH1_HIGH_VOLTAGE_SETPOINT = e.SCH1_HIGH_VOLTAGE_SETPOINT;
            entityHistory.SCH1_TEMP_SETPOINT_ON = e.SCH1_TEMP_SETPOINT_ON;
            entityHistory.SCH1_TEMP_SETPOINT_OFF = e.SCH1_TEMP_SETPOINT_OFF;
            entityHistory.SCH1_START_DATE = e.SCH1_START_DATE;
            entityHistory.SCH1_END_DATE = e.SCH1_END_DATE;
            entityHistory.SCH1_WEEKDAYS = e.SCH1_WEEKDAYS;
            entityHistory.SCH1_SATURDAY = e.SCH1_SATURDAY;
            entityHistory.SCH1_SUNDAY = e.SCH1_SUNDAY;
            entityHistory.SCH1_HOLIDAYS = e.SCH1_HOLIDAYS;
            entityHistory.SCH1_KVAR_SETPOINT_ON = e.SCH1_KVAR_SETPOINT_ON;
            entityHistory.SCH1_KVAR_SETPOINT_OFF = e.SCH1_KVAR_SETPOINT_OFF;
            entityHistory.SCH2_SCHEDULE = e.SCH2_SCHEDULE;
            entityHistory.SCH2_CONTROL_STRATEGY = e.SCH2_CONTROL_STRATEGY;
            entityHistory.SCH2_TIME_ON = e.SCH2_TIME_ON;
            entityHistory.SCH2_TIME_OFF = e.SCH2_TIME_OFF;
            entityHistory.SCH2_LOW_VOLTAGE_SETPOINT = e.SCH2_LOW_VOLTAGE_SETPOINT;
            entityHistory.SCH2_HIGH_VOLTAGE_SETPOINT = e.SCH2_HIGH_VOLTAGE_SETPOINT;
            entityHistory.SCH2_TEMP_SETPOINT_ON = e.SCH2_TEMP_SETPOINT_ON;
            entityHistory.SCH2_TEMP_SETPOINT_OFF = e.SCH2_TEMP_SETPOINT_OFF;
            entityHistory.SCH2_START_DATE = e.SCH2_START_DATE;
            entityHistory.SCH2_END_DATE = e.SCH2_END_DATE;
            entityHistory.SCH2_WEEKDAYS = e.SCH2_WEEKDAYS;
            entityHistory.SCH2_SATURDAY = e.SCH2_SATURDAY;
            entityHistory.SCH2_SUNDAY = e.SCH2_SUNDAY;
            entityHistory.SCH2_HOLIDAYS = e.SCH2_HOLIDAYS;
            entityHistory.SCH2_KVAR_SETPOINT_ON = e.SCH2_KVAR_SETPOINT_ON;
            entityHistory.SCH2_KVAR_SETPOINT_OFF = e.SCH2_KVAR_SETPOINT_OFF;
            entityHistory.SCH3_SCHEDULE = e.SCH3_SCHEDULE;
            entityHistory.SCH3_CONTROL_STRATEGY = e.SCH3_CONTROL_STRATEGY;
            entityHistory.SCH3_TIME_ON = e.SCH3_TIME_ON;
            entityHistory.SCH3_TIME_OFF = e.SCH3_TIME_OFF;
            entityHistory.SCH3_LOW_VOLTAGE_SETPOINT = e.SCH3_LOW_VOLTAGE_SETPOINT;
            entityHistory.SCH3_HIGH_VOLTAGE_SETPOINT = e.SCH3_HIGH_VOLTAGE_SETPOINT;
            entityHistory.SCH3_TEMP_SETPOINT_ON = e.SCH3_TEMP_SETPOINT_ON;
            entityHistory.SCH3_TEMP_SETPOINT_OFF = e.SCH3_TEMP_SETPOINT_OFF;
            entityHistory.SCH3_START_DATE = e.SCH3_START_DATE;
            entityHistory.SCH3_END_DATE = e.SCH3_END_DATE;
            entityHistory.SCH3_WEEKDAYS = e.SCH3_WEEKDAYS;
            entityHistory.SCH3_SATURDAY = e.SCH3_SATURDAY;
            entityHistory.SCH3_SUNDAY = e.SCH3_SUNDAY;
            entityHistory.SCH3_HOLIDAYS = e.SCH3_HOLIDAYS;
            entityHistory.SCH3_KVAR_SETPOINT_ON = e.SCH3_KVAR_SETPOINT_ON;
            entityHistory.SCH3_KVAR_SETPOINT_OFF = e.SCH3_KVAR_SETPOINT_OFF;
            entityHistory.SCH4_SCHEDULE = e.SCH4_SCHEDULE;
            entityHistory.SCH4_CONTROL_STRATEGY = e.SCH4_CONTROL_STRATEGY;
            entityHistory.SCH4_TIME_ON = e.SCH4_TIME_ON;
            entityHistory.SCH4_TIME_OFF = e.SCH4_TIME_OFF;
            entityHistory.SCH4_LOW_VOLTAGE_SETPOINT = e.SCH4_LOW_VOLTAGE_SETPOINT;
            entityHistory.SCH4_HIGH_VOLTAGE_SETPOINT = e.SCH4_HIGH_VOLTAGE_SETPOINT;
            entityHistory.SCH4_TEMP_SETPOINT_ON = e.SCH4_TEMP_SETPOINT_ON;
            entityHistory.SCH4_TEMP_SETPOINT_OFF = e.SCH4_TEMP_SETPOINT_OFF;
            entityHistory.SCH4_START_DATE = e.SCH4_START_DATE;
            entityHistory.SCH4_END_DATE = e.SCH4_END_DATE;
            entityHistory.SCH4_WEEKDAYS = e.SCH4_WEEKDAYS;
            entityHistory.SCH4_SATURDAY = e.SCH4_SATURDAY;
            entityHistory.SCH4_SUNDAY = e.SCH4_SUNDAY;
            entityHistory.SCH4_HOLIDAYS = e.SCH4_HOLIDAYS;
            entityHistory.SCH4_KVAR_SETPOINT_ON = e.SCH4_KVAR_SETPOINT_ON;
            entityHistory.SCH4_KVAR_SETPOINT_OFF = e.SCH4_KVAR_SETPOINT_OFF;
            entityHistory.CONTROL_TYPE = e.CONTROL_TYPE;
            entityHistory.CONTROLLER_UNIT_MODEL = e.CONTROLLER_UNIT_MODEL;
            entityHistory.VOLT_VAR_TEAM_MEMBER = e.VOLT_VAR_TEAM_MEMBER;
            entityHistory.SWITCH_POSITION = e.SWITCH_POSITION;
            //entityHistory.PREFERED_BANK_POSITION = e.PREFERED_BANK_POSITION; //INC000004165708
            entityHistory.MAXCYCLES = e.MAXCYCLES;
            entityHistory.DAYLIGHT_SAVINGS_TIME = e.DAYLIGHT_SAVINGS_TIME;
            entityHistory.EST_VOLTAGE_CHANGE = e.EST_VOLTAGE_CHANGE;
            entityHistory.VOLTAGE_OVERRIDE_TIME = e.VOLTAGE_OVERRIDE_TIME;
            entityHistory.HIGH_VOLTAGE_OVERRIDE_SETPOINT = e.HIGH_VOLTAGE_OVERRIDE_SETPOINT;
            entityHistory.LOW_VOLTAGE_OVERRIDE_SETPOINT = e.LOW_VOLTAGE_OVERRIDE_SETPOINT;
            entityHistory.VOLTAGE_CHANGE_TIME = e.VOLTAGE_CHANGE_TIME;
            entityHistory.TEMPERATURE_OVERRIDE = e.TEMPERATURE_OVERRIDE;
            entityHistory.TEMPERATURE_CHANGE_TIME = e.TEMPERATURE_CHANGE_TIME;
            entityHistory.EST_BANK_VOLTAGE_RISE = e.EST_BANK_VOLTAGE_RISE;
            entityHistory.AUTO_BVR_CALC = e.AUTO_BVR_CALC;
            entityHistory.DATA_LOGGING_INTERVAL = e.DATA_LOGGING_INTERVAL;
            entityHistory.PULSE_TIME = e.PULSE_TIME;
            entityHistory.MIN_SW_VOLTAGE = e.MIN_SW_VOLTAGE;
            entityHistory.TIME_DELAY = e.TIME_DELAY;
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
            entityHistory.SEASON_OFF = e.SEASON_OFF;//INC000004112585

            //INC000004165708 START
            entityHistory.SCH1_BANK_POSITION = e.SCH1_BANK_POSITION;
            entityHistory.SCH2_BANK_POSITION = e.SCH2_BANK_POSITION;
            entityHistory.SCH3_BANK_POSITION = e.SCH3_BANK_POSITION;
            entityHistory.SCH4_BANK_POSITION = e.SCH4_BANK_POSITION;
            //INC000004165708 END

            //INC000004165778 start
            entityHistory.SCH1_TIME_ON2 = e.SCH1_TIME_ON2;
            entityHistory.SCH1_TIME_OFF2 = e.SCH1_TIME_OFF2;
            entityHistory.SCH1_WEEKDAYS2 = e.SCH1_WEEKDAYS2;
            entityHistory.SCH1_SATURDAY2 = e.SCH1_SATURDAY2;
            entityHistory.SCH1_SUNDAY2 = e.SCH1_SUNDAY2;
            entityHistory.SCH1_HOLIDAYS2 = e.SCH1_HOLIDAYS2;
            entityHistory.SCH2_TIME_ON2 = e.SCH2_TIME_ON2;
            entityHistory.SCH2_TIME_OFF2 = e.SCH2_TIME_OFF2;
            entityHistory.SCH2_WEEKDAYS2 = e.SCH2_WEEKDAYS2;
            entityHistory.SCH2_SATURDAY2 = e.SCH2_SATURDAY2;
            entityHistory.SCH2_SUNDAY2 = e.SCH2_SUNDAY2;
            entityHistory.SCH2_HOLIDAYS2 = e.SCH2_HOLIDAYS2;
            entityHistory.SCH3_TIME_ON2 = e.SCH3_TIME_ON2;
            entityHistory.SCH3_TIME_OFF2 = e.SCH3_TIME_OFF2;
            entityHistory.SCH3_WEEKDAYS2 = e.SCH3_WEEKDAYS2;
            entityHistory.SCH3_SATURDAY2 = e.SCH3_SATURDAY2;
            entityHistory.SCH3_SUNDAY2 = e.SCH3_SUNDAY2;
            entityHistory.SCH3_HOLIDAYS2 = e.SCH3_HOLIDAYS2;
            entityHistory.SCH4_TIME_ON2 = e.SCH4_TIME_ON2;
            entityHistory.SCH4_TIME_ON2 = e.SCH4_TIME_OFF2;
            entityHistory.SCH4_WEEKDAYS2 = e.SCH4_WEEKDAYS2;
            entityHistory.SCH4_SATURDAY2 = e.SCH4_SATURDAY2;
            entityHistory.SCH4_SUNDAY2 = e.SCH4_SUNDAY2;
            entityHistory.SCH4_HOLIDAYS2 = e.SCH4_HOLIDAYS2;
            entityHistory.SCH1_MONTHUR = e.SCH1_MONTHUR;
            entityHistory.SCH1_MONTHUR2 = e.SCH1_MONTHUR2;
            entityHistory.SCH2_MONTHUR = e.SCH2_MONTHUR;
            entityHistory.SCH2_MONTHUR2 = e.SCH2_MONTHUR2;
            entityHistory.SCH3_MONTHUR = e.SCH3_MONTHUR;
            entityHistory.SCH3_MONTHUR2 = e.SCH3_MONTHUR2;
            entityHistory.SCH4_MONTHUR = e.SCH4_MONTHUR;
            entityHistory.SCH4_MONTHUR2 = e.SCH4_MONTHUR2;
            entityHistory.SCH1_FRISUN = e.SCH1_FRISUN;
            entityHistory.SCH1_FRISUN2 = e.SCH1_FRISUN2;
            entityHistory.SCH2_FRISUN = e.SCH2_FRISUN;
            entityHistory.SCH2_FRISUN2 = e.SCH2_FRISUN2;
            entityHistory.SCH3_FRISUN = e.SCH3_FRISUN;
            entityHistory.SCH3_FRISUN2 = e.SCH3_FRISUN2;
            entityHistory.SCH4_FRISUN = e.SCH4_FRISUN;
            entityHistory.SCH4_FRISUN2 = e.SCH4_FRISUN2;
            //INC000004165778 end
        }
    }
}