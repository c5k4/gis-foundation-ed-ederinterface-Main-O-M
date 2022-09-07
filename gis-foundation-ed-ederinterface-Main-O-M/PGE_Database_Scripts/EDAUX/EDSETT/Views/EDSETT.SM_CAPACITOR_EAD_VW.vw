Prompt drop View SM_CAPACITOR_EAD_VW;
DROP VIEW EDSETT.SM_CAPACITOR_EAD_VW
/

/* Formatted on 7/1/2019 10:06:01 PM (QP5 v5.313) */
PROMPT View SM_CAPACITOR_EAD_VW;
--
-- SM_CAPACITOR_EAD_VW  (View)
--

CREATE OR REPLACE FORCE VIEW EDSETT.SM_CAPACITOR_EAD_VW
(
    GLOBAL_ID,
    FEATURE_CLASS_NAME,
    OPERATING_NUM,
    DEVICE_ID,
    PREPARED_BY,
    RELAY_TYPE,
    DATE_MODIFIED,
    TIMESTAMP,
    EFFECTIVE_DT,
    PEER_REVIEW_DT,
    PEER_REVIEW_BY,
    DIVISION,
    DISTRICT,
    CURRENT_FUTURE,
    FIRMWARE_VERSION,
    SOFTWARE_VERSION,
    CONTROL_SERIAL_NUM,
    PROCESSED_FLAG,
    RELEASED_BY,
    SCH1_SCHEDULE,
    SCH1_CONTROL_STRATEGY,
    SCH1_TIME_ON,
    SCH1_TIME_OFF,
    SCH1_LOW_VOLTAGE_SETPOINT,
    SCH1_HIGH_VOLTAGE_SETPOINT,
    SCH1_TEMP_SETPOINT_ON,
    SCH1_TEMP_SETPOINT_OFF,
    SCH1_START_DATE,
    SCH1_END_DATE,
    SCH1_WEEKDAYS,
    SCH1_SATURDAY,
    SCH1_SUNDAY,
    SCH1_HOLIDAYS,
    SCH1_KVAR_SETPOINT_ON,
    SCH1_KVAR_SETPOINT_OFF,
    SCH2_SCHEDULE,
    SCH2_CONTROL_STRATEGY,
    SCH2_TIME_ON,
    SCH2_TIME_OFF,
    SCH2_LOW_VOLTAGE_SETPOINT,
    SCH2_HIGH_VOLTAGE_SETPOINT,
    SCH2_TEMP_SETPOINT_ON,
    SCH2_TEMP_SETPOINT_OFF,
    SCH2_START_DATE,
    SCH2_END_DATE,
    SCH2_WEEKDAYS,
    SCH2_SATURDAY,
    SCH2_SUNDAY,
    SCH2_HOLIDAYS,
    SCH2_KVAR_SETPOINT_ON,
    SCH2_KVAR_SETPOINT_OFF,
    SCH3_SCHEDULE,
    SCH3_CONTROL_STRATEGY,
    SCH3_TIME_ON,
    SCH3_TIME_OFF,
    SCH3_LOW_VOLTAGE_SETPOINT,
    SCH3_HIGH_VOLTAGE_SETPOINT,
    SCH3_TEMP_SETPOINT_ON,
    SCH3_TEMP_SETPOINT_OFF,
    SCH3_START_DATE,
    SCH3_END_DATE,
    SCH3_WEEKDAYS,
    SCH3_SATURDAY,
    SCH3_SUNDAY,
    SCH3_HOLIDAYS,
    SCH3_KVAR_SETPOINT_ON,
    SCH3_KVAR_SETPOINT_OFF,
    SCH4_SCHEDULE,
    SCH4_CONTROL_STRATEGY,
    SCH4_TIME_ON,
    SCH4_TIME_OFF,
    SCH4_LOW_VOLTAGE_SETPOINT,
    SCH4_HIGH_VOLTAGE_SETPOINT,
    SCH4_TEMP_SETPOINT_ON,
    SCH4_TEMP_SETPOINT_OFF,
    SCH4_START_DATE,
    SCH4_END_DATE,
    SCH4_WEEKDAYS,
    SCH4_SATURDAY,
    SCH4_SUNDAY,
    SCH4_HOLIDAYS,
    SCH4_KVAR_SETPOINT_ON,
    SCH4_KVAR_SETPOINT_OFF,
    CONTROL_TYPE,
    CONTROLLER_UNIT_MODEL,
    VOLT_VAR_TEAM_MEMBER,
    SWITCH_POSITION,
    MAXCYCLES,
    DAYLIGHT_SAVINGS_TIME,
    EST_VOLTAGE_CHANGE,
    VOLTAGE_OVERRIDE_TIME,
    HIGH_VOLTAGE_OVERRIDE_SETPOINT,
    LOW_VOLTAGE_OVERRIDE_SETPOINT,
    VOLTAGE_CHANGE_TIME,
    TEMPERATURE_OVERRIDE,
    TEMPERATURE_CHANGE_TIME,
    EST_BANK_VOLTAGE_RISE,
    AUTO_BVR_CALC,
    DATA_LOGGING_INTERVAL,
    PULSE_TIME,
    MIN_SW_VOLTAGE,
    TIME_DELAY,
    SCADA,
    SCADA_TYPE,
    MASTER_STATION,
    BAUD_RATE,
    TRANSMIT_ENABLE_DELAY,
    TRANSMIT_DISABLE_DELAY,
    RTU_ADDRESS,
    REPEATER,
    SPECIAL_CONDITIONS,
    RADIO_MANF_CD,
    RADIO_MODEL_NUM,
    RADIO_SERIAL_NUM,
    ID,
    ENGINEERING_COMMENTS,
    FLISR_ENGINEERING_COMMENTS,
    RTU_EXIST,
    RTU_MODEL_NUM,
    RTU_SERIAL_NUM,
    RTU_FIRMWARE_VERSION,
    RTU_SOFTWARE_VERSION,
    RTU_MANF_CD,
    SEASON_OFF,
    SCH1_BANK_POSITION,
    SCH2_BANK_POSITION,
    SCH3_BANK_POSITION,
    SCH4_BANK_POSITION,
    SCH1_TIME_ON2,
    SCH1_TIME_OFF2,
    SCH1_WEEKDAYS2,
    SCH1_SATURDAY2,
    SCH1_SUNDAY2,
    SCH1_HOLIDAYS2,
    SCH2_TIME_ON2,
    SCH2_TIME_OFF2,
    SCH2_WEEKDAYS2,
    SCH2_SATURDAY2,
    SCH2_SUNDAY2,
    SCH2_HOLIDAYS2,
    SCH3_TIME_ON2,
    SCH3_TIME_OFF2,
    SCH3_WEEKDAYS2,
    SCH3_SATURDAY2,
    SCH3_SUNDAY2,
    SCH3_HOLIDAYS2,
    SCH4_TIME_ON2,
    SCH4_TIME_OFF2,
    SCH4_WEEKDAYS2,
    SCH4_SATURDAY2,
    SCH4_SUNDAY2,
    SCH4_HOLIDAYS2,
    SCH1_MONTHUR,
    SCH1_FRISUN,
    SCH1_MONTHUR2,
    SCH1_FRISUN2,
    SCH2_MONTHUR,
    SCH2_FRISUN,
    SCH2_MONTHUR2,
    SCH2_FRISUN2,
    SCH3_MONTHUR,
    SCH3_FRISUN,
    SCH3_MONTHUR2,
    SCH3_FRISUN2,
    SCH4_MONTHUR,
    SCH4_FRISUN,
    SCH4_MONTHUR2,
    SCH4_FRISUN2
)
AS
    SELECT "GLOBAL_ID",
           "FEATURE_CLASS_NAME",
           "OPERATING_NUM",
           "DEVICE_ID",
           "PREPARED_BY",
           "RELAY_TYPE",
           "DATE_MODIFIED",
           "TIMESTAMP",
           "EFFECTIVE_DT",
           "PEER_REVIEW_DT",
           "PEER_REVIEW_BY",
           "DIVISION",
           "DISTRICT",
           "CURRENT_FUTURE",
           "FIRMWARE_VERSION",
           "SOFTWARE_VERSION",
           "CONTROL_SERIAL_NUM",
           "PROCESSED_FLAG",
           "RELEASED_BY",
           "SCH1_SCHEDULE",
           "SCH1_CONTROL_STRATEGY",
           "SCH1_TIME_ON",
           "SCH1_TIME_OFF",
           "SCH1_LOW_VOLTAGE_SETPOINT",
           "SCH1_HIGH_VOLTAGE_SETPOINT",
           "SCH1_TEMP_SETPOINT_ON",
           "SCH1_TEMP_SETPOINT_OFF",
           "SCH1_START_DATE",
           "SCH1_END_DATE",
           "SCH1_WEEKDAYS",
           "SCH1_SATURDAY",
           "SCH1_SUNDAY",
           "SCH1_HOLIDAYS",
           "SCH1_KVAR_SETPOINT_ON",
           "SCH1_KVAR_SETPOINT_OFF",
           "SCH2_SCHEDULE",
           "SCH2_CONTROL_STRATEGY",
           "SCH2_TIME_ON",
           "SCH2_TIME_OFF",
           "SCH2_LOW_VOLTAGE_SETPOINT",
           "SCH2_HIGH_VOLTAGE_SETPOINT",
           "SCH2_TEMP_SETPOINT_ON",
           "SCH2_TEMP_SETPOINT_OFF",
           "SCH2_START_DATE",
           "SCH2_END_DATE",
           "SCH2_WEEKDAYS",
           "SCH2_SATURDAY",
           "SCH2_SUNDAY",
           "SCH2_HOLIDAYS",
           "SCH2_KVAR_SETPOINT_ON",
           "SCH2_KVAR_SETPOINT_OFF",
           "SCH3_SCHEDULE",
           "SCH3_CONTROL_STRATEGY",
           "SCH3_TIME_ON",
           "SCH3_TIME_OFF",
           "SCH3_LOW_VOLTAGE_SETPOINT",
           "SCH3_HIGH_VOLTAGE_SETPOINT",
           "SCH3_TEMP_SETPOINT_ON",
           "SCH3_TEMP_SETPOINT_OFF",
           "SCH3_START_DATE",
           "SCH3_END_DATE",
           "SCH3_WEEKDAYS",
           "SCH3_SATURDAY",
           "SCH3_SUNDAY",
           "SCH3_HOLIDAYS",
           "SCH3_KVAR_SETPOINT_ON",
           "SCH3_KVAR_SETPOINT_OFF",
           "SCH4_SCHEDULE",
           "SCH4_CONTROL_STRATEGY",
           "SCH4_TIME_ON",
           "SCH4_TIME_OFF",
           "SCH4_LOW_VOLTAGE_SETPOINT",
           "SCH4_HIGH_VOLTAGE_SETPOINT",
           "SCH4_TEMP_SETPOINT_ON",
           "SCH4_TEMP_SETPOINT_OFF",
           "SCH4_START_DATE",
           "SCH4_END_DATE",
           "SCH4_WEEKDAYS",
           "SCH4_SATURDAY",
           "SCH4_SUNDAY",
           "SCH4_HOLIDAYS",
           "SCH4_KVAR_SETPOINT_ON",
           "SCH4_KVAR_SETPOINT_OFF",
           "CONTROL_TYPE",
           "CONTROLLER_UNIT_MODEL",
           "VOLT_VAR_TEAM_MEMBER",
           "SWITCH_POSITION",
           "MAXCYCLES",
           "DAYLIGHT_SAVINGS_TIME",
           "EST_VOLTAGE_CHANGE",
           "VOLTAGE_OVERRIDE_TIME",
           "HIGH_VOLTAGE_OVERRIDE_SETPOINT",
           "LOW_VOLTAGE_OVERRIDE_SETPOINT",
           "VOLTAGE_CHANGE_TIME",
           "TEMPERATURE_OVERRIDE",
           "TEMPERATURE_CHANGE_TIME",
           "EST_BANK_VOLTAGE_RISE",
           "AUTO_BVR_CALC",
           "DATA_LOGGING_INTERVAL",
           "PULSE_TIME",
           "MIN_SW_VOLTAGE",
           "TIME_DELAY",
           "SCADA",
           "SCADA_TYPE",
           "MASTER_STATION",
           "BAUD_RATE",
           "TRANSMIT_ENABLE_DELAY",
           "TRANSMIT_DISABLE_DELAY",
           "RTU_ADDRESS",
           "REPEATER",
           "SPECIAL_CONDITIONS",
           "RADIO_MANF_CD",
           "RADIO_MODEL_NUM",
           "RADIO_SERIAL_NUM",
           "ID",
           "ENGINEERING_COMMENTS",
           "FLISR_ENGINEERING_COMMENTS",
           "RTU_EXIST",
           "RTU_MODEL_NUM",
           "RTU_SERIAL_NUM",
           "RTU_FIRMWARE_VERSION",
           "RTU_SOFTWARE_VERSION",
           "RTU_MANF_CD",
           "SEASON_OFF",
           "SCH1_BANK_POSITION",
           "SCH2_BANK_POSITION",
           "SCH3_BANK_POSITION",
           "SCH4_BANK_POSITION",
           "SCH1_TIME_ON2",
           "SCH1_TIME_OFF2",
           "SCH1_WEEKDAYS2",
           "SCH1_SATURDAY2",
           "SCH1_SUNDAY2",
           "SCH1_HOLIDAYS2",
           "SCH2_TIME_ON2",
           "SCH2_TIME_OFF2",
           "SCH2_WEEKDAYS2",
           "SCH2_SATURDAY2",
           "SCH2_SUNDAY2",
           "SCH2_HOLIDAYS2",
           "SCH3_TIME_ON2",
           "SCH3_TIME_OFF2",
           "SCH3_WEEKDAYS2",
           "SCH3_SATURDAY2",
           "SCH3_SUNDAY2",
           "SCH3_HOLIDAYS2",
           "SCH4_TIME_ON2",
           "SCH4_TIME_OFF2",
           "SCH4_WEEKDAYS2",
           "SCH4_SATURDAY2",
           "SCH4_SUNDAY2",
           "SCH4_HOLIDAYS2",
           "SCH1_MONTHUR",
           "SCH1_FRISUN",
           "SCH1_MONTHUR2",
           "SCH1_FRISUN2",
           "SCH2_MONTHUR",
           "SCH2_FRISUN",
           "SCH2_MONTHUR2",
           "SCH2_FRISUN2",
           "SCH3_MONTHUR",
           "SCH3_FRISUN",
           "SCH3_MONTHUR2",
           "SCH3_FRISUN2",
           "SCH4_MONTHUR",
           "SCH4_FRISUN",
           "SCH4_MONTHUR2",
           "SCH4_FRISUN2"
      FROM sm_capacitor
     WHERE current_future = 'C'
/
