Prompt drop View SM_RECLOSER_TS_ERD_VW;
DROP VIEW EDSETT.SM_RECLOSER_TS_ERD_VW
/

/* Formatted on 7/1/2019 10:06:26 PM (QP5 v5.313) */
PROMPT View SM_RECLOSER_TS_ERD_VW;
--
-- SM_RECLOSER_TS_ERD_VW  (View)
--

CREATE OR REPLACE FORCE VIEW EDSETT.SM_RECLOSER_TS_ERD_VW
(
    ID,
    GLOBAL_ID,
    FEATURE_CLASS_NAME,
    OPERATING_NUM,
    DEVICE_ID,
    PREPARED_BY,
    DATE_MODIFIED,
    TIMESTAMP,
    EFFECTIVE_DT,
    PEER_REVIEW_DT,
    PEER_REVIEW_BY,
    DIVISION,
    DISTRICT,
    CURRENT_FUTURE,
    INI_EMULATED_DEVICE,
    INI_INVERSE_SEGMENT,
    INI_SPEED,
    INI_AMPERE_RATING,
    INI_MIN_TRIP_A,
    INI_TIME_MULTIPLIER,
    INI_RESET_TYPE,
    INI_RESET_TIME,
    INI_LOWCUTOFF,
    INI_LOWCUTOFF_CURR_A,
    INI_DEFINITETIME1,
    INI_DEFINITETIME1_CURR_A,
    INI_DEFINITETIME1_TIME,
    INI_DEFINITETIME2,
    INI_DEFINITETIME2_CURR_A,
    INI_DEFINITETIME2_TIME,
    INI_OPEN_INTERVAL_TIME,
    INI_COIL_RATING,
    TEST1_EMULATED_DEVICE,
    TEST1_INVERSE_SEGMENT,
    TEST1_SPEED,
    TEST1_AMPERE_RATING,
    TEST1_MIN_TRIP_A,
    TEST1_TIME_MULTIPLIER,
    TEST1_RESET_TYPE,
    TEST1_RESET_TIME,
    TEST1_LOWCUTOFF,
    TEST1_LOWCUTOFF_CURR_A,
    TEST1_DEFINITETIME1,
    TEST1_DEFINITETIME1_CUR_A,
    TEST1_DEFINITETIME1_TIME,
    TEST1_DEFINITETIME2,
    TEST1_DEFINITETIME2_CURR_A,
    TEST1_DEFINITETIME2_TIME,
    TEST1_OPEN_INTERVAL_TIME,
    TEST1_COIL_RATING,
    TEST2_EMULATED_DEVICE,
    TEST2_INVERSE_SEGMENT,
    TEST2_SPEED,
    TEST2_AMPERE_RATING,
    TEST2_MIN_TRIP_A,
    TEST2_TIME_MULTIPLIER,
    TEST2_RESET_TYPE,
    TEST2_RESET_TIME,
    TEST2_LOWCUTOFF,
    TEST2_LOWCUTOFF_CURR_A,
    TEST2_DEFINITETIME1,
    TEST2_DEFINITETIME1_CURR_A,
    TEST2_DEFINITETIME1_TIME,
    TEST2_DEFINITETIME2,
    TEST2_DEFINITETIME2_CURR_A,
    TEST2_DEFINITETIME2_TIME,
    TEST2_OPEN_INTERVAL_TIME,
    TEST2_COIL_RATING,
    TEST3_EMULATED_DEVICE,
    TEST3_INVERSE_SEGMENT,
    TEST3_SPEED,
    TEST3_AMPERE_RATING,
    TEST3_MIN_TRIP_A,
    TEST3_TIME_MULTIPLIER,
    TEST3_RESET_TYPE,
    TEST3_RESET_TIME,
    TEST3_LOWCUTOFF,
    TEST3_LOWCUTOFF_CURR_A,
    TEST3_DEFINITETIME1,
    TEST3_DEFINITETIME1_CURR_A,
    TEST3_DEFINITETIME1_TIME,
    TEST3_DEFINITETIME2,
    TEST3_DEFINITETIME2_CURR_A,
    TEST3_DEFINITETIME2_TIME,
    TEST3_COIL_RATING,
    HIGH_CURRENT_CUTOFF,
    HIGH_CURRENT_CUTOFF_A,
    SEC_MODE_COUNTS,
    SEC_MODE,
    SEC_MODE_RESET_TIME,
    SEC_MODE_STARTING_CURRENT,
    SEQUENCE_RESET_TIME,
    INI_TIME,
    INI_CURR_A,
    INI_DEFINITETIME3,
    INI_DEFINITETIME3_CURR_A,
    INI_DEFINITETIME3_TIME,
    TEST1_TIME,
    TEST1_CURR_A,
    TEST1_DEFINITETIME3,
    TEST1_DEFINITETIME3_CURR_A,
    TEST1_DEFINITETIME3_TIME,
    TEST2_TIME,
    TEST2_CURR_A,
    TEST2_DEFINITETIME3,
    TEST2_DEFINITETIME3_CURR_A,
    TEST2_DEFINITETIME3_TIME,
    TEST3_TIME,
    TEST3_CURR_A,
    TEST3_DEFINITETIME3,
    TEST3_DEFINITETIME3_CURR_A,
    TEST3_DEFINITETIME3_TIME,
    OK_TO_BYPASS,
    BYPASS_PLANS
)
AS
    SELECT "ID",
           "GLOBAL_ID",
           "FEATURE_CLASS_NAME",
           "OPERATING_NUM",
           "DEVICE_ID",
           "PREPARED_BY",
           "DATE_MODIFIED",
           "TIMESTAMP",
           "EFFECTIVE_DT",
           "PEER_REVIEW_DT",
           "PEER_REVIEW_BY",
           "DIVISION",
           "DISTRICT",
           "CURRENT_FUTURE",
           "INI_EMULATED_DEVICE",
           "INI_INVERSE_SEGMENT",
           "INI_SPEED",
           "INI_AMPERE_RATING",
           "INI_MIN_TRIP_A",
           "INI_TIME_MULTIPLIER",
           "INI_RESET_TYPE",
           "INI_RESET_TIME",
           "INI_LOWCUTOFF",
           "INI_LOWCUTOFF_CURR_A",
           "INI_DEFINITETIME1",
           "INI_DEFINITETIME1_CURR_A",
           "INI_DEFINITETIME1_TIME",
           "INI_DEFINITETIME2",
           "INI_DEFINITETIME2_CURR_A",
           "INI_DEFINITETIME2_TIME",
           "INI_OPEN_INTERVAL_TIME",
           "INI_COIL_RATING",
           "TEST1_EMULATED_DEVICE",
           "TEST1_INVERSE_SEGMENT",
           "TEST1_SPEED",
           "TEST1_AMPERE_RATING",
           "TEST1_MIN_TRIP_A",
           "TEST1_TIME_MULTIPLIER",
           "TEST1_RESET_TYPE",
           "TEST1_RESET_TIME",
           "TEST1_LOWCUTOFF",
           "TEST1_LOWCUTOFF_CURR_A",
           "TEST1_DEFINITETIME1",
           "TEST1_DEFINITETIME1_CUR_A",
           "TEST1_DEFINITETIME1_TIME",
           "TEST1_DEFINITETIME2",
           "TEST1_DEFINITETIME2_CURR_A",
           "TEST1_DEFINITETIME2_TIME",
           "TEST1_OPEN_INTERVAL_TIME",
           "TEST1_COIL_RATING",
           "TEST2_EMULATED_DEVICE",
           "TEST2_INVERSE_SEGMENT",
           "TEST2_SPEED",
           "TEST2_AMPERE_RATING",
           "TEST2_MIN_TRIP_A",
           "TEST2_TIME_MULTIPLIER",
           "TEST2_RESET_TYPE",
           "TEST2_RESET_TIME",
           "TEST2_LOWCUTOFF",
           "TEST2_LOWCUTOFF_CURR_A",
           "TEST2_DEFINITETIME1",
           "TEST2_DEFINITETIME1_CURR_A",
           "TEST2_DEFINITETIME1_TIME",
           "TEST2_DEFINITETIME2",
           "TEST2_DEFINITETIME2_CURR_A",
           "TEST2_DEFINITETIME2_TIME",
           "TEST2_OPEN_INTERVAL_TIME",
           "TEST2_COIL_RATING",
           "TEST3_EMULATED_DEVICE",
           "TEST3_INVERSE_SEGMENT",
           "TEST3_SPEED",
           "TEST3_AMPERE_RATING",
           "TEST3_MIN_TRIP_A",
           "TEST3_TIME_MULTIPLIER",
           "TEST3_RESET_TYPE",
           "TEST3_RESET_TIME",
           "TEST3_LOWCUTOFF",
           "TEST3_LOWCUTOFF_CURR_A",
           "TEST3_DEFINITETIME1",
           "TEST3_DEFINITETIME1_CURR_A",
           "TEST3_DEFINITETIME1_TIME",
           "TEST3_DEFINITETIME2",
           "TEST3_DEFINITETIME2_CURR_A",
           "TEST3_DEFINITETIME2_TIME",
           "TEST3_COIL_RATING",
           "HIGH_CURRENT_CUTOFF",
           "HIGH_CURRENT_CUTOFF_A",
           "SEC_MODE_COUNTS",
           "SEC_MODE",
           "SEC_MODE_RESET_TIME",
           "SEC_MODE_STARTING_CURRENT",
           "SEQUENCE_RESET_TIME",
           "INI_TIME",
           "INI_CURR_A",
           "INI_DEFINITETIME3",
           "INI_DEFINITETIME3_CURR_A",
           "INI_DEFINITETIME3_TIME",
           "TEST1_TIME",
           "TEST1_CURR_A",
           "TEST1_DEFINITETIME3",
           "TEST1_DEFINITETIME3_CURR_A",
           "TEST1_DEFINITETIME3_TIME",
           "TEST2_TIME",
           "TEST2_CURR_A",
           "TEST2_DEFINITETIME3",
           "TEST2_DEFINITETIME3_CURR_A",
           "TEST2_DEFINITETIME3_TIME",
           "TEST3_TIME",
           "TEST3_CURR_A",
           "TEST3_DEFINITETIME3",
           "TEST3_DEFINITETIME3_CURR_A",
           "TEST3_DEFINITETIME3_TIME",
           "OK_TO_BYPASS",
           "BYPASS_PLANS"
      FROM SM_RECLOSER_TS
     WHERE     DATE_MODIFIED >= SM_EXPOSE_DATA_PKG.GET_DATEFROM
           AND DATE_MODIFIED <= SM_EXPOSE_DATA_PKG.GET_DATETO
           AND CURRENT_FUTURE = 'C'
/