Prompt drop View SM_SWITCH_ERD_VW;
DROP VIEW EDSETT.SM_SWITCH_ERD_VW
/

/* Formatted on 7/1/2019 10:06:38 PM (QP5 v5.313) */
PROMPT View SM_SWITCH_ERD_VW;
--
-- SM_SWITCH_ERD_VW  (View)
--

CREATE OR REPLACE FORCE VIEW EDSETT.SM_SWITCH_ERD_VW
(
    GLOBAL_ID,
    FEATURE_CLASS_NAME,
    OPERATING_NUM,
    DEVICE_ID,
    PREPARED_BY,
    SWITCH_TYPE,
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
    CONTROL_UNIT_TYPE,
    PROCESSED_FLAG,
    RELEASED_BY,
    OK_TO_BYPASS,
    SECTIONALIZING_FEATURE,
    ATS_CAPABLE,
    ATS_FEATURE,
    PHA_FAULT_CUR_LEVEL,
    GRD_FAULT_CUR_LEVEL,
    PHA_FAULT_DURATION,
    GRD_FAULT_DURATION,
    PHA_INRUSH_TIME,
    GRD_INRUSH_TIME,
    PHA_INRUSH_MULT,
    GRD_INRUSH_MULT,
    SECT_RESET_TIME,
    RECLOSE_RESET_TIME,
    OC_TO_VOLT_TIME,
    FAULT_CUR_LOSS,
    RECL_COUNT_TO_TRIP,
    SHOTS_REQ_LOCKOUT,
    SHOTS_TO_LOCKOUT_TIME,
    OVERC_SHOTS_TO_LO_OPER,
    VOLT_LOSS_THRESH,
    TIME_THRESH,
    CURR_THRESH,
    AUTO_RECLOSE_TIME,
    ATS_PREFERRED_FEED,
    ATS_ALTERNATE_FEED,
    SELECT_PREFERRED,
    UNBALANCE_DETECT,
    SELECT_RETURN,
    SELECT_TRANSACTION,
    DWELL_TIMER,
    NORMALIZE_LEFT,
    NORMALIZE_RIGHT,
    SET_BASE_LEFT,
    SET_BASE_RIGHT,
    ACCESS_CODE,
    COMM_O_BIT_RATE,
    LOCKOUT_LEVEL,
    LOSS_OF_SOURCE,
    RETURN_TO_SOURCE_VOLT,
    OVERVOLT_DETECT,
    UNBALANCE_DETECT_VOLT,
    LOSS_OF_LEFT_SOURCE,
    LOSS_OF_RIGHT_SOURCE,
    RETURN_TO_SOURCE_TIME,
    LOCKOUT_RESET,
    OC_LOCKOUT_PICKUP,
    TRANSITION_DWELL,
    WINDOW_BEGIN,
    WINDOW_LENGTH,
    FLISR,
    SUMMER_LOAD_LIMIT,
    WINTER_LOAD_LIMIT,
    LIMITING_FACTOR,
    ENGINEERING_COMMENTS,
    OPERATING_MODE,
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
    RTU_EXIST,
    RTU_MODEL_NUM,
    RTU_SERIAL_NUM,
    RTU_FIRMWARE_VERSION,
    RTU_SOFTWARE_VERSION,
    RTU_MANF_CD,
    FLISR_ENGINEERING_COMMENTS
)
AS
    SELECT "GLOBAL_ID",
           "FEATURE_CLASS_NAME",
           "OPERATING_NUM",
           "DEVICE_ID",
           "PREPARED_BY",
           "SWITCH_TYPE",
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
           "CONTROL_UNIT_TYPE",
           "PROCESSED_FLAG",
           "RELEASED_BY",
           "OK_TO_BYPASS",
           "SECTIONALIZING_FEATURE",
           "ATS_CAPABLE",
           "ATS_FEATURE",
           "PHA_FAULT_CUR_LEVEL",
           "GRD_FAULT_CUR_LEVEL",
           "PHA_FAULT_DURATION",
           "GRD_FAULT_DURATION",
           "PHA_INRUSH_TIME",
           "GRD_INRUSH_TIME",
           "PHA_INRUSH_MULT",
           "GRD_INRUSH_MULT",
           "SECT_RESET_TIME",
           "RECLOSE_RESET_TIME",
           "OC_TO_VOLT_TIME",
           "FAULT_CUR_LOSS",
           "RECL_COUNT_TO_TRIP",
           "SHOTS_REQ_LOCKOUT",
           "SHOTS_TO_LOCKOUT_TIME",
           "OVERC_SHOTS_TO_LO_OPER",
           "VOLT_LOSS_THRESH",
           "TIME_THRESH",
           "CURR_THRESH",
           "AUTO_RECLOSE_TIME",
           "ATS_PREFERRED_FEED",
           "ATS_ALTERNATE_FEED",
           "SELECT_PREFERRED",
           "UNBALANCE_DETECT",
           "SELECT_RETURN",
           "SELECT_TRANSACTION",
           "DWELL_TIMER",
           "NORMALIZE_LEFT",
           "NORMALIZE_RIGHT",
           "SET_BASE_LEFT",
           "SET_BASE_RIGHT",
           "ACCESS_CODE",
           "COMM_O_BIT_RATE",
           "LOCKOUT_LEVEL",
           "LOSS_OF_SOURCE",
           "RETURN_TO_SOURCE_VOLT",
           "OVERVOLT_DETECT",
           "UNBALANCE_DETECT_VOLT",
           "LOSS_OF_LEFT_SOURCE",
           "LOSS_OF_RIGHT_SOURCE",
           "RETURN_TO_SOURCE_TIME",
           "LOCKOUT_RESET",
           "OC_LOCKOUT_PICKUP",
           "TRANSITION_DWELL",
           "WINDOW_BEGIN",
           "WINDOW_LENGTH",
           "FLISR",
           "SUMMER_LOAD_LIMIT",
           "WINTER_LOAD_LIMIT",
           "LIMITING_FACTOR",
           "ENGINEERING_COMMENTS",
           "OPERATING_MODE",
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
           RTU_EXIST,
           RTU_MODEL_NUM,
           RTU_SERIAL_NUM,
           RTU_FIRMWARE_VERSION,
           RTU_SOFTWARE_VERSION,
           RTU_MANF_CD,
           FLISR_ENGINEERING_COMMENTS
      FROM SM_SWITCH
     WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
           AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
           AND CURRENT_FUTURE = 'C'
/


Prompt Grants on VIEW SM_SWITCH_ERD_VW TO GIS_I to GIS_I;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDSETT.SM_SWITCH_ERD_VW TO GIS_I
/