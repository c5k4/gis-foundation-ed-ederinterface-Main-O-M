Prompt drop View SM_REGULATOR_EAD_VW;
DROP VIEW EDSETT.SM_REGULATOR_EAD_VW
/

/* Formatted on 7/1/2019 10:06:28 PM (QP5 v5.313) */
PROMPT View SM_REGULATOR_EAD_VW;
--
-- SM_REGULATOR_EAD_VW  (View)
--

CREATE OR REPLACE FORCE VIEW EDSETT.SM_REGULATOR_EAD_VW
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
    OK_TO_BYPASS,
    CONTROL_TYPE,
    "MODE",
    PRIMARY_CT_RATING,
    BAND_WIDTH,
    PT_RATIO,
    RANGE_UNBLOCKED,
    BLOCKED_PCT,
    STEPS,
    PEAK_LOAD,
    MIN_LOAD,
    PVD_MAX,
    PVD_MIN,
    SVD_MIN,
    POWER_FACTOR,
    LOAD_CYCLE,
    RISE_RATING,
    TIMER,
    VOLT_VAR_TEAM_MEMBER,
    REV_THRESHOLD,
    HIGH_VOLTAGE_LIMIT,
    LOW_VOLTAGE_LIMIT,
    FWD_A_STATUS,
    FWD_A_VOLT,
    FWD_A_RESET,
    FWD_A_XSET,
    FWD_B_STATUS,
    FWD_B_VOLT,
    FWD_B_RESET,
    FWD_B_XSET,
    FWD_C_STATUS,
    FWD_C_VOLT,
    FWD_C_RESET,
    FWD_C_XSET,
    REV_A_VOLT,
    REV_A_RESET,
    REV_A_XSET,
    REV_B_VOLT,
    REV_B_RESET,
    REV_B_XSET,
    REV_C_VOLT,
    REV_C_RESET,
    REV_C_XSET,
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
    USE_RX,
    ID,
    REVERSIBLE,
    BANK_CD,
    RTU_EXIST,
    RTU_MODEL_NUM,
    RTU_SERIAL_NUM,
    RTU_FIRMWARE_VERSION,
    RTU_SOFTWARE_VERSION,
    RTU_MANF_CD,
    SEASON_OFF,
    SWITCH_POSITION,
    EMERGENCY_ONLY,
    ENGINEERING_COMMENTS,
    FLISR_ENGINEERING_COMMENTS
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
           "OK_TO_BYPASS",
           "CONTROL_TYPE",
           "MODE",
           "PRIMARY_CT_RATING",
           "BAND_WIDTH",
           "PT_RATIO",
           "RANGE_UNBLOCKED",
           "BLOCKED_PCT",
           "STEPS",
           "PEAK_LOAD",
           "MIN_LOAD",
           "PVD_MAX",
           "PVD_MIN",
           "SVD_MIN",
           "POWER_FACTOR",
           "LOAD_CYCLE",
           "RISE_RATING",
           "TIMER",
           "VOLT_VAR_TEAM_MEMBER",
           "REV_THRESHOLD",
           "HIGH_VOLTAGE_LIMIT",
           "LOW_VOLTAGE_LIMIT",
           "FWD_A_STATUS",
           "FWD_A_VOLT",
           "FWD_A_RESET",
           "FWD_A_XSET",
           "FWD_B_STATUS",
           "FWD_B_VOLT",
           "FWD_B_RESET",
           "FWD_B_XSET",
           "FWD_C_STATUS",
           "FWD_C_VOLT",
           "FWD_C_RESET",
           "FWD_C_XSET",
           "REV_A_VOLT",
           "REV_A_RESET",
           "REV_A_XSET",
           "REV_B_VOLT",
           "REV_B_RESET",
           "REV_B_XSET",
           "REV_C_VOLT",
           "REV_C_RESET",
           "REV_C_XSET",
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
           "USE_RX",
           "ID",
           "REVERSIBLE",
           "BANK_CD",
           RTU_EXIST,
           RTU_MODEL_NUM,
           RTU_SERIAL_NUM,
           RTU_FIRMWARE_VERSION,
           RTU_SOFTWARE_VERSION,
           RTU_MANF_CD,
           SEASON_OFF,
           SWITCH_POSITION,
           EMERGENCY_ONLY,
           ENGINEERING_COMMENTS,
           FLISR_ENGINEERING_COMMENTS
      FROM SM_REGULATOR
     WHERE CURRENT_FUTURE = 'C'
/
