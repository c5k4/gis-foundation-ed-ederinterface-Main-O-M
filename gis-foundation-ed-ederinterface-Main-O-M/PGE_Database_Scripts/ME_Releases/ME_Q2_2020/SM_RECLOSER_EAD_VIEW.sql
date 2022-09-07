spool "D:\Temp\SM_RECLOSER_EAD_VIEW.txt"
Prompt drop View SM_RECLOSER_EAD_VW;
DROP VIEW EDSETT.SM_RECLOSER_EAD_VW
/

/* Formatted on 6/15/2020 12:25:24 AM (QP5 v5.313) */
PROMPT View SM_RECLOSER_EAD_VW;
--
-- SM_RECLOSER_EAD_VW  (View)
--

CREATE OR REPLACE FORCE VIEW EDSETT.SM_RECLOSER_EAD_VW
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
    BYPASS_PLANS,
    OPERATING_AS_CD,
    GRD_MIN_TRIP,
    PHA_MIN_TRIP,
    GRD_INST_TRIP_CD,
    PHA_INST_TRIP_CD,
    TCC1_FAST_CURVES_USED,
    TCC2_SLOW_CURVES_USED,
    GRD_OP_F_CRV,
    PHA_OP_F_CRV,
    GRD_RESP_TIME,
    PHA_RESP_TIME,
    GRD_FAST_CRV,
    PHA_FAST_CRV,
    GRD_SLOW_CRV,
    PHA_SLOW_CRV,
    PHA_SLOW_CRV_OPS,
    GRD_SLOW_CRV_OPS,
    GRD_TMUL_FAST,
    PHA_TMUL_FAST,
    GRD_TMUL_SLOW,
    PHA_TMUL_SLOW,
    GRD_TADD_FAST,
    PHA_TADD_FAST,
    GRD_TADD_SLOW,
    PHA_TADD_SLOW,
    TOT_LOCKOUT_OPS,
    RECLOSE1_TIME,
    RECLOSE2_TIME,
    RECLOSE3_TIME,
    RESET,
    RECLOSE_RETRY_ENABLED,
    SGF_CD,
    SGF_MIN_TRIP_PERCENT,
    SGF_TIME_DELAY,
    HIGH_CURRENT_LOCKOUT_USED,
    HIGH_CURRENT_LOCKOUT_PHA,
    HIGH_CURRENT_LOCKUOUT_GRD,
    COLD_LOAD_PLI_USED,
    COLD_LOAD_PLI_PHA,
    COLD_LOAD_PLI_GRD,
    COLD_LOAD_PLI_CURVE_PHA,
    COLD_LOAD_PLI_CURVE_GRD,
    ALT_GRD_MIN_TRIP,
    ALT_PHA_MIN_TRIP,
    ALT_GRD_INST_TRIP_CD,
    ALT_PHA_INST_TRIP_CD,
    ALT_GRD_OP_F_CRV,
    ALT_PHA_OP_F_CRV,
    ALT_TCC1_FAST_CURVES_USED,
    ALT_TCC2_SLOW_CURVES_USED,
    ALT_GRD_RESP_TIME,
    ALT_PHA_RESP_TIME,
    ALT_GRD_FAST_CRV,
    ALT_PHA_FAST_CRV,
    ALT_GRD_SLOW_CRV,
    ALT_PHA_SLOW_CRV,
    ALT_GRD_VMUL_FAST,
    ALT_PHA_SLOW_CRV_OPS,
    ALT_GRD_SLOW_CRV_OPS,
    ALT_PHA_VMUL_FAST,
    ALT_GRD_VMUL_SLOW,
    ALT_PHA_VMUL_SLOW,
    ALT_GRD_TADD_FAST,
    ALT_PHA_TADD_FAST,
    ALT_GRD_TADD_SLOW,
    ALT_PHA_TADD_SLOW,
    ALT_TOT_LOCKOUT_OPS,
    ALT_RECLOSE1_TIME,
    ALT_RECLOSE2_TIME,
    ALT_RECLOSE3_TIME,
    ALT_RESET,
    ALT_RECLOSE_RETRY_ENABLED,
    ALT_SGF_CD,
    ALT_SGF_MIN_TRIP_PERCENT,
    ALT_SGF_TIME_DELAY,
    ALT_HIGH_CURRENT_LOCKOUT_USED,
    ALT_HIGH_CURRENT_LOCKOUT_PHA,
    ALT_HIGH_CURRENT_LOCKUOUT_GRD,
    ALT_COLD_LOAD_PLI_USED,
    ALT_COLD_LOAD_PLI_PHA,
    ALT_COLD_LOAD_PLI_GRD,
    ALT_COLD_LOAD_PLI_CURVE_PHA,
    ALT_COLD_LOAD_PLI_CURVE_GRD,
    ALT2_PERMIT_LS_ENABLING,
    ALT2_GRD_ARMING_THRESHOLD,
    ALT2_PHA_ARMING_THRESHOLD,
    ALT2_GRD_INRUSH_THRESHOLD,
    ALT2_PHA_INRUSH_THRESHOLD,
    ALT2_INRUSH_DURATION,
    ALT2_LS_LOCKOUT_OPS,
    ALT2_LS_RESET_TIME,
    ALT2_GRD_MIN_TRIP,
    ALT2_PHA_MIN_TRIP,
    ALT2_GRD_INST_TRIP_CD,
    ALT2_PHA_INST_TRIP_CD,
    ALT2_GRD_FAST_CRV,
    ALT2_PHA_FAST_CRV,
    ALT2_PHA_VMUL_FAST,
    ALT2_GRD_VMUL_SLOW,
    ALT2_GRD_TADD_FAST,
    ALT2_PHA_TADD_FAST,
    ALT3_GRD_MIN_TRIP,
    ALT3_PHA_MIN_TRIP,
    ALT3_GRD_INST_TRIP_CD,
    ALT3_PHA_INST_TRIP_CD,
    ALT3_GRD_OP_F_CRV,
    ALT3_PHA_OP_F_CRV,
    ALT3_TCC1_FAST_CURVES_USED,
    ALT3_TCC2_SLOW_CURVES_USED,
    ALT3_GRD_FAST_CRV,
    ALT3_PHA_FAST_CRV,
    ALT3_GRD_SLOW_CRV,
    ALT3_PHA_SLOW_CRV,
    ALT3_GRD_VMUL_FAST,
    ALT3_PHA_SLOW_CRV_OPS,
    ALT3_GRD_SLOW_CRV_OPS,
    ALT3_PHA_VMUL_FAST,
    ALT3_GRD_VMUL_SLOW,
    ALT3_PHA_VMUL_SLOW,
    ALT3_GRD_TADD_FAST,
    ALT3_PHA_TADD_FAST,
    ALT3_GRD_TADD_SLOW,
    ALT3_PHA_TADD_SLOW,
    ALT3_PHA_DELAY,
    ALT3_GRD_DELAY,
    ALT3_TOT_LOCKOUT_OPS,
    ALT3_RECLOSE1_TIME,
    ALT3_RECLOSE2_TIME,
    ALT3_RECLOSE3_TIME,
    ALT3_RESET,
    ALT3_RECLOSE_RETRY_ENABLED,
    ALT3_SGF_CD,
    ALT3_SGF_MIN_TRIP_PERCENT,
    ALT3_SGF_TIME_DELAY,
    ALT3_HIGH_CURRENT_LOCKOUT_USED,
    ALT3_HIGH_CURRENT_LOCKOUT_PHA,
    ALT3_HIGH_CURRENT_LOCKUOUT_GRD,
    ALT3_COLD_LOAD_PLI_USED,
    ALT3_COLD_LOAD_PLI_PHA,
    ALT3_COLD_LOAD_PLI_GRD,
    ALT3_COLD_LOAD_PLI_CURVE_PHA,
    ALT3_COLD_LOAD_PLI_CURVE_GRD,
    ACTIVE_PROFILE,
    ENGINEERING_COMMENTS,
    FLISR,
    "Summer_Load_Limit",
    "Winter_Load_Limit",
    "Limiting_Factor",
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
    FLISR_ENGINEERING_COMMENTS,
    FLISR_OPERATING_MODE,
    ID,
    PERMIT_RB_CUTIN,
    DIRECT_TRANSFER_TRIP,
    BOC_VOLTAGE,
    RB_CUTOUT_TIME,
    RTU_EXIST,
    RTU_MODEL_NUM,
    RTU_SERIAL_NUM,
    RTU_FIRMWARE_VERSION,
    RTU_SOFTWARE_VERSION,
    RTU_MANF_CD,
    MULTI_FUNCTIONAL,
    ALT_GRD_LOCKOUT_OPS,
    ALT_PHA_LOCKOUT_OPS,
    DEF_TIME_PHA,
    DEF_TIME_GROUND,
    MIN_RESP_TADDR_PHA,
    MIN_RESP_TADDR_GRO,
    HLT_ENAB,
    PHASE,
    GROUND,
    OPT_LOCK_PHA,
    OPT_LOCK_GRO,
    FIRST_RECLO_PHA,
    FIRST_RECLO_GRO,
    SEC_RECLO_PHA,
    SEC_RECLO_GRO,
    THIRD_RECLO_PHA,
    THIRD_RECLO_GRO,
    RESET_TIME,
    RESET_TIME_LOCK,
    MTT_PHA,
    MTT_GROU,
    TIME_DEL_PHA,
    TIME_DEL_GROU,
    PU_MTT_PHA,
    PU_MTT_GROU,
    PU_CURVE_PHA,
    PU_CURVE_GROU,
    TADDR_PHA,
    TADDR_GROU,
    MIN_RESP_TADDR_T_D_PHA,
    MIN_RESP_TADDR_T_D_GR,
    TIME_T_ACTI,
    FAULT_CURR_ONLY,
    VOLT_LOSS_ONLY,
    FAULT_CURR_W_VOL_LOSS,
    VOLT_LOSS_DISP,
    ENA_SWIT_MOD,
    COUT_T_TRIP_VOLT_LOSS,
    RESET_TIMERR,
    SEQ_COORDI_MODE,
	COLD_PHA_TMUL_FAST,
	COLD_GRD_TMUL_FAST
)
BEQUEATH DEFINER
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
           "BYPASS_PLANS",
           "OPERATING_AS_CD",
           "GRD_MIN_TRIP",
           "PHA_MIN_TRIP",
           "GRD_INST_TRIP_CD",
           "PHA_INST_TRIP_CD",
           "TCC1_FAST_CURVES_USED",
           "TCC2_SLOW_CURVES_USED",
           "GRD_OP_F_CRV",
           "PHA_OP_F_CRV",
           "GRD_RESP_TIME",
           "PHA_RESP_TIME",
           "GRD_FAST_CRV",
           "PHA_FAST_CRV",
           "GRD_SLOW_CRV",
           "PHA_SLOW_CRV",
           "PHA_SLOW_CRV_OPS",
           "GRD_SLOW_CRV_OPS",
           "GRD_TMUL_FAST",
           "PHA_TMUL_FAST",
           "GRD_TMUL_SLOW",
           "PHA_TMUL_SLOW",
           "GRD_TADD_FAST",
           "PHA_TADD_FAST",
           "GRD_TADD_SLOW",
           "PHA_TADD_SLOW",
           "TOT_LOCKOUT_OPS",
           "RECLOSE1_TIME",
           "RECLOSE2_TIME",
           "RECLOSE3_TIME",
           "RESET",
           "RECLOSE_RETRY_ENABLED",
           "SGF_CD",
           "SGF_MIN_TRIP_PERCENT",
           "SGF_TIME_DELAY",
           "HIGH_CURRENT_LOCKOUT_USED",
           "HIGH_CURRENT_LOCKOUT_PHA",
           "HIGH_CURRENT_LOCKUOUT_GRD",
           "COLD_LOAD_PLI_USED",
           "COLD_LOAD_PLI_PHA",
           "COLD_LOAD_PLI_GRD",
           "COLD_LOAD_PLI_CURVE_PHA",
           "COLD_LOAD_PLI_CURVE_GRD",
           "ALT_GRD_MIN_TRIP",
           "ALT_PHA_MIN_TRIP",
           "ALT_GRD_INST_TRIP_CD",
           "ALT_PHA_INST_TRIP_CD",
           "ALT_GRD_OP_F_CRV",
           "ALT_PHA_OP_F_CRV",
           "ALT_TCC1_FAST_CURVES_USED",
           "ALT_TCC2_SLOW_CURVES_USED",
           "ALT_GRD_RESP_TIME",
           "ALT_PHA_RESP_TIME",
           "ALT_GRD_FAST_CRV",
           "ALT_PHA_FAST_CRV",
           "ALT_GRD_SLOW_CRV",
           "ALT_PHA_SLOW_CRV",
           "ALT_GRD_VMUL_FAST",
           "ALT_PHA_SLOW_CRV_OPS",
           "ALT_GRD_SLOW_CRV_OPS",
           "ALT_PHA_VMUL_FAST",
           "ALT_GRD_VMUL_SLOW",
           "ALT_PHA_VMUL_SLOW",
           "ALT_GRD_TADD_FAST",
           "ALT_PHA_TADD_FAST",
           "ALT_GRD_TADD_SLOW",
           "ALT_PHA_TADD_SLOW",
           "ALT_TOT_LOCKOUT_OPS",
           "ALT_RECLOSE1_TIME",
           "ALT_RECLOSE2_TIME",
           "ALT_RECLOSE3_TIME",
           "ALT_RESET",
           "ALT_RECLOSE_RETRY_ENABLED",
           "ALT_SGF_CD",
           "ALT_SGF_MIN_TRIP_PERCENT",
           "ALT_SGF_TIME_DELAY",
           "ALT_HIGH_CURRENT_LOCKOUT_USED",
           "ALT_HIGH_CURRENT_LOCKOUT_PHA",
           "ALT_HIGH_CURRENT_LOCKUOUT_GRD",
           "ALT_COLD_LOAD_PLI_USED",
           "ALT_COLD_LOAD_PLI_PHA",
           "ALT_COLD_LOAD_PLI_GRD",
           "ALT_COLD_LOAD_PLI_CURVE_PHA",
           "ALT_COLD_LOAD_PLI_CURVE_GRD",
           "ALT2_PERMIT_LS_ENABLING",
           "ALT2_GRD_ARMING_THRESHOLD",
           "ALT2_PHA_ARMING_THRESHOLD",
           "ALT2_GRD_INRUSH_THRESHOLD",
           "ALT2_PHA_INRUSH_THRESHOLD",
           "ALT2_INRUSH_DURATION",
           "ALT2_LS_LOCKOUT_OPS",
           "ALT2_LS_RESET_TIME",
           "ALT2_GRD_MIN_TRIP",
           "ALT2_PHA_MIN_TRIP",
           "ALT2_GRD_INST_TRIP_CD",
           "ALT2_PHA_INST_TRIP_CD",
           "ALT2_GRD_FAST_CRV",
           "ALT2_PHA_FAST_CRV",
           "ALT2_PHA_VMUL_FAST",
           "ALT2_GRD_VMUL_SLOW",
           "ALT2_GRD_TADD_FAST",
           "ALT2_PHA_TADD_FAST",
           "ALT3_GRD_MIN_TRIP",
           "ALT3_PHA_MIN_TRIP",
           "ALT3_GRD_INST_TRIP_CD",
           "ALT3_PHA_INST_TRIP_CD",
           "ALT3_GRD_OP_F_CRV",
           "ALT3_PHA_OP_F_CRV",
           "ALT3_TCC1_FAST_CURVES_USED",
           "ALT3_TCC2_SLOW_CURVES_USED",
           "ALT3_GRD_FAST_CRV",
           "ALT3_PHA_FAST_CRV",
           "ALT3_GRD_SLOW_CRV",
           "ALT3_PHA_SLOW_CRV",
           "ALT3_GRD_VMUL_FAST",
           "ALT3_PHA_SLOW_CRV_OPS",
           "ALT3_GRD_SLOW_CRV_OPS",
           "ALT3_PHA_VMUL_FAST",
           "ALT3_GRD_VMUL_SLOW",
           "ALT3_PHA_VMUL_SLOW",
           "ALT3_GRD_TADD_FAST",
           "ALT3_PHA_TADD_FAST",
           "ALT3_GRD_TADD_SLOW",
           "ALT3_PHA_TADD_SLOW",
           "ALT3_PHA_DELAY",
           "ALT3_GRD_DELAY",
           "ALT3_TOT_LOCKOUT_OPS",
           "ALT3_RECLOSE1_TIME",
           "ALT3_RECLOSE2_TIME",
           "ALT3_RECLOSE3_TIME",
           "ALT3_RESET",
           "ALT3_RECLOSE_RETRY_ENABLED",
           "ALT3_SGF_CD",
           "ALT3_SGF_MIN_TRIP_PERCENT",
           "ALT3_SGF_TIME_DELAY",
           "ALT3_HIGH_CURRENT_LOCKOUT_USED",
           "ALT3_HIGH_CURRENT_LOCKOUT_PHA",
           "ALT3_HIGH_CURRENT_LOCKUOUT_GRD",
           "ALT3_COLD_LOAD_PLI_USED",
           "ALT3_COLD_LOAD_PLI_PHA",
           "ALT3_COLD_LOAD_PLI_GRD",
           "ALT3_COLD_LOAD_PLI_CURVE_PHA",
           "ALT3_COLD_LOAD_PLI_CURVE_GRD",
           "ACTIVE_PROFILE",
           "ENGINEERING_COMMENTS",
           "FLISR",
           "Summer_Load_Limit",
           "Winter_Load_Limit",
           "Limiting_Factor",
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
           "FLISR_ENGINEERING_COMMENTS",
           "FLISR_OPERATING_MODE",
           "ID",
           "PERMIT_RB_CUTIN",
           "DIRECT_TRANSFER_TRIP",
           "BOC_VOLTAGE",
           "RB_CUTOUT_TIME",
           RTU_EXIST,
           RTU_MODEL_NUM,
           RTU_SERIAL_NUM,
           RTU_FIRMWARE_VERSION,
           RTU_SOFTWARE_VERSION,
           RTU_MANF_CD,
           MULTI_FUNCTIONAL,
           ALT_GRD_LOCKOUT_OPS,
           ALT_PHA_LOCKOUT_OPS,
           "DEF_TIME_PHA",
           "DEF_TIME_GROUND",
           "MIN_RESP_TADDR_PHA",
           "MIN_RESP_TADDR_GRO",
           "HLT_ENAB",
           "PHASE",
           "GROUND",
           "OPT_LOCK_PHA",
           "OPT_LOCK_GRO",
           "FIRST_RECLO_PHA",
           "FIRST_RECLO_GRO",
           "SEC_RECLO_PHA",
           "SEC_RECLO_GRO",
           "THIRD_RECLO_PHA",
           "THIRD_RECLO_GRO",
           "RESET_TIME",
           "RESET_TIME_LOCK",
           "MTT_PHA",
           "MTT_GROU",
           "TIME_DEL_PHA",
           "TIME_DEL_GROU",
           "PU_MTT_PHA",
           "PU_MTT_GROU",
           "PU_CURVE_PHA",
           "PU_CURVE_GROU",
           "TADDR_PHA",
           "TADDR_GROU",
           "MIN_RESP_TADDR_T_D_PHA",
           "MIN_RESP_TADDR_T_D_GR",
           "TIME_T_ACTI",
           "FAULT_CURR_ONLY",
           "VOLT_LOSS_ONLY",
           "FAULT_CURR_W_VOL_LOSS",
           "VOLT_LOSS_DISP",
           "ENA_SWIT_MOD",
           "COUT_T_TRIP_VOLT_LOSS",
           "RESET_TIMERR",
           "SEQ_COORDI_MODE",
		   "COLD_PHA_TMUL_FAST",
		   "COLD_GRD_TMUL_FAST"
      FROM SM_RECLOSER
     WHERE CURRENT_FUTURE = 'C'
/

spool off;