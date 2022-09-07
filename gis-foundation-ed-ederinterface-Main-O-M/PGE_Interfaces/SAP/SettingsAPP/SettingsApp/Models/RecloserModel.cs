using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class RecloserModel
    {
        [SettingsValidatorAttribute("Recloser", "OPERATING_NUM")]
        [Display(Name = "Operating Number :")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("Recloser", "DEVICE_ID")]
        [Display(Name = "Device ID :")]
        public System.Int64? DeviceId { get; set; }

        [SettingsValidatorAttribute("Recloser", "PREPARED_BY")]
        [Display(Name = "Prepared By :")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("Recloser", "CONTROL_SERIAL_NUM")]
        [Display(Name = "Controller Serial # :")]
        public System.String ControllerSerialNum { get; set; }

        [SettingsValidatorAttribute("Recloser", "DATE_MODIFIED")]
        [Display(Name = "Date modified :")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("Recloser", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date :")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("Recloser", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date :")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("Recloser", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer :")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("Recloser", "NOTES")]
        [Display(Name = "Notes :")]
        public System.String Notes { get; set; }

        [SettingsValidatorAttribute("Recloser", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        [SettingsValidatorAttribute("Recloser", "CONTROL_TYPE")]
        [Display(Name = "Controller Unit Type :")]
        public System.String ControllerUnitType { get; set; }

        [SettingsValidatorAttribute("Recloser", "BYPASS_PLANS")]
        [Display(Name = "Bypass plan :")]
        public System.String BypassPlan { get; set; }



        [SettingsValidatorAttribute("Recloser", "OPERATING_AS_CD")]
        [Display(Name = "Operating As :")]
        public System.String OperatingMode { get; set; }

        [SettingsValidatorAttribute("Recloser", "GRD_MIN_TRIP")]
        [Display(Name = "Minimum Trip (Ground) :")]
        public System.Int16? GrdMinTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_MIN_TRIP")]
        [Display(Name = "Minimum Trip (Phase) :")]
        public System.Int16? PhaMinTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "GRD_INST_TRIP_CD")]
        //[Range(0, 99.99, ErrorMessage = "Enter number between 0 to 99.99")]
        [Display(Name = "Inst. Trip (Ground) :")]
        public System.Decimal? GrdInstTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_INST_TRIP_CD")]
        //[Range(0, 12500, ErrorMessage = "Enter number between 0 to 12500")]
        [Display(Name = "Inst. Trip (Phase) :")]
        public System.Decimal? PhaInstTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "TCC1_FAST_CURVES_USED")]
        [Display(Name = "TCC1: Fast curves used :")]
        public System.String Tcc1FastCurvesUsed { get; set; }

        //[SettingsValidatorAttribute("Recloser", "ALT_TCC1_FAST_CURVES_USED")]
        //[Display(Name = "TCC1: Fast curves used :")]
        //public System.String AltTcc1FastCurvesUsed { get; set; }

        //[SettingsValidatorAttribute("Recloser", "ALT_TCC2_SLOW_CURVES_USED")]
        //[Display(Name = "TCC2: Slow curves used :")]
        //public System.String AltTcc2SlowCurvesUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "TCC2_SLOW_CURVES_USED")]
        [Display(Name = "TCC2: Slow curves used :")]
        public System.String Tcc2SlowCurvesUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "GRD_OP_F_CRV")]
        [Display(Name = "Fast curve ops. (Ground) :")]
        public System.Int16? GrdOpFastCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_OP_F_CRV")]
        [Display(Name = "Fast curve ops. (Phase) :")]
        public System.Int16? PhaFastCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "GRD_RESP_TIME")]
        [Display(Name = "Response time (Ground) :")]
        public System.Decimal? GrdResponseTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_RESP_TIME")]
        [Display(Name = "Response time (Phase) :")]
        public System.Decimal? PhaResponseTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "GRD_FAST_CRV")]
        [Display(Name = "Fast curve (Ground) :")]
        public System.String GrdFastCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_FAST_CRV")]
        [Display(Name = "Fast curve (Phase) :")]
        public System.String PhaFastCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "GRD_SLOW_CRV")]
        [Display(Name = "Slow curve (Ground) :")]
        public System.String GrdSlowCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_SLOW_CRV")]
        [Display(Name = "Slow curve (Phase) :")]
        public System.String PhaSlowCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_SLOW_CRV_OPS")]
        [Display(Name = "Slow curve ops (Phase) :")]
        public System.Int16? PhaSlowCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "GRD_SLOW_CRV_OPS")]
        [Display(Name = "Slow curve ops (Ground) :")]
        public System.Int16? GrdSlowCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "GRD_TMUL_FAST")]
        [Display(Name = "TMul Fast (Ground) :")]
        public System.Decimal? GrdTmulFast { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_TMUL_FAST")]
        [Display(Name = "TMul Fast (Phase) :")]
        public System.Decimal? PhaTmulFast { get; set; }

        [SettingsValidatorAttribute("Recloser", "COLD_PHA_TMUL_FAST")]
        [Display(Name = "TMul Fast (Ground) :")]
        public System.Decimal? ColdGrdTmulFast { get; set; }

        [SettingsValidatorAttribute("Recloser", "COLD_GRD_TMUL_FAST")]
        [Display(Name = "TMul Fast (Phase) :")]
        public System.Decimal? ColdPhaTmulFast { get; set; }


        [SettingsValidatorAttribute("Recloser", "GRD_TMUL_SLOW")]
        [Display(Name = "TMul Slow (Ground) :")]
        public System.Decimal? GrdTmulSlow { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_TMUL_SLOW")]
        [Display(Name = "TMul Slow (Phase) :")]
        public System.Decimal? PhaTmulSlow { get; set; }

        //[SettingsValidatorAttribute("Recloser", "GRD_TADD_FAST")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Fast (Ground) :")]
        public System.Decimal? GrdTaddFast { get; set; }

        // [SettingsValidatorAttribute("Recloser", "PHA_TADD_FAST")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Fast (Phase) :")]
        public System.Decimal? PhaTaddFast { get; set; }

        // [SettingsValidatorAttribute("Recloser", "GRD_TADD_SLOW")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Slow (Ground) :")]
        public System.Decimal? GrdTaddSlow { get; set; }

        // [SettingsValidatorAttribute("Recloser", "PHA_TADD_SLOW")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Slow (Phase) :")]
        public System.Decimal? PhaTaddSlow { get; set; }

        [SettingsValidatorAttribute("Recloser", "GRD_LOCKOUT_OPS")]
        [Display(Name = "Ops to lockout (Ground)  :")]
        public System.Int16? GrdOpsToLockout { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHA_LOCKOUT_OPS")]
        [Display(Name = "Ops to lockout (Phase)  :")]
        public System.Int16? PhaOpsToLockout { get; set; }

        [SettingsValidatorAttribute("Recloser", "TOT_LOCKOUT_OPS")]
        [Display(Name = "Ops to lockout :")]
        public System.Int16? TotOpsToLockout { get; set; }

        [SettingsValidatorAttribute("Recloser", "RECLOSE1_TIME")]
        [Display(Name = "1st reclose :")]
        public System.Decimal? Reclose1Time { get; set; }

        [SettingsValidatorAttribute("Recloser", "RECLOSE2_TIME")]
        [Display(Name = "2nd reclose :")]
        public System.Decimal? Reclose2Time { get; set; }

        [SettingsValidatorAttribute("Recloser", "RECLOSE3_TIME")]
        [Display(Name = "3rd reclose :")]
        public System.Decimal? Reclose3Time { get; set; }

        [SettingsValidatorAttribute("Recloser", "RESET")]
        [Display(Name = "Reset Time :")]
        public System.Int16? ResetTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "RECLOSE_RETRY_ENABLED")]
        [Display(Name = "Reclose retry enabled :")]
        public System.String RecloseRetryEnabled { get; set; }

        [SettingsValidatorAttribute("Recloser", "SGF_CD")]
        [Display(Name = "Sensitive Ground Fault (SGF) :")]
        public System.String SgfCd { get; set; }

        [SettingsValidatorAttribute("Recloser", "SGF_MIN_TRIP_PERCENT")]
        [Display(Name = "Min. Trip % :")]
        public System.Decimal? SgfMinTripPercent { get; set; }

        [SettingsValidatorAttribute("Recloser", "SGF_TIME_DELAY")]
        [Display(Name = "Time Delay :")]
        public System.Decimal? SgfTimeDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "SGF_LOCKOUT")]
        [Display(Name = "Lockout :")]
        public System.Int16? SgfLockout { get; set; }

        [SettingsValidatorAttribute("Recloser", "HIGH_CURRENT_LOCKOUT_USED")]
        [Display(Name = "High current lockout used :")]
        public System.String HighCurrentLockoutUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "HIGH_CURRENT_LOCKOUT_PHA")]
        [Display(Name = "Phase :")]
        public System.String HighCurrentLockoutPhase { get; set; }

        [SettingsValidatorAttribute("Recloser", "HIGH_CURRENT_LOCKUOUT_GRD")]
        [Display(Name = "Ground :")]
        public System.String HighCurrentLockoutGround { get; set; }

        [SettingsValidatorAttribute("Recloser", "COLD_LOAD_PLI_USED")]
        [Display(Name = "Cold load PU used :")]
        public System.String ColdLoadPliUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "COLD_LOAD_PLI_PHA")]
        [Display(Name = "Phase :")]
        public System.Int64? ColdLoadPliPhase { get; set; }

        [SettingsValidatorAttribute("Recloser", "COLD_LOAD_PLI_GRD")]
        [Display(Name = "Ground :")]
        public System.Int64? ColdLoadPliGround { get; set; }

        [SettingsValidatorAttribute("Recloser", "COLD_LOAD_PLI_CURVE_PHA")]
        [Display(Name = "Curve (Phase) :")]
        public System.String ColdLoadPliCurvePhase { get; set; }

        [SettingsValidatorAttribute("Recloser", "COLD_LOAD_PLI_CURVE_GRD")]
        [Display(Name = "Curve (Ground) :")]
        public System.String ColdLoadPliCurveGround { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_GRD_MIN_TRIP")]
        [Display(Name = "Minimum Trip (Ground) :")]
        public System.Int16? AltGrdMinTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_PHA_MIN_TRIP")]
        [Display(Name = "Minimum Trip (Phase) :")]
        public System.Int16? AltPhaMinTrip { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT_GRD_INST_TRIP_CD")]
        [Range(0, 99.99, ErrorMessage = "Enter number between 0 to 99.99")]
        [Display(Name = "Inst. Trip (Ground) :")]
        public System.Decimal? AltGrdInstTrip { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT_PHA_INST_TRIP_CD")]
        [Range(0, 99.99, ErrorMessage = "Enter number between 0 to 99.99")]
        [Display(Name = "Inst. Trip (Phase) :")]
        public System.Decimal? AltPhaInstTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_GRD_OP_F_CRV")]
        [Display(Name = "Fast curve ops. (Ground) :")]
        public System.Int16? AltGrdFastCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_PHA_OP_F_CRV")]
        [Display(Name = "Fast curve ops. (Phase) :")]
        public System.Int16? AltPhaFastCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_TCC1_FAST_CURVES_USED")]
        [Display(Name = "TCC1: Fast curves used :")]
        public System.String AltTcc1FastCurvesUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_TCC2_SLOW_CURVES_USED")]
        [Display(Name = "TCC2: Slow curves used :")]
        public System.String AltTcc2SlowCurvesUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_GRD_RESP_TIME")]
        [Display(Name = "Response time (Ground) :")]
        public System.Decimal? AltGrdResponseTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_PHA_RESP_TIME")]
        [Display(Name = "Response time (Phase) :")]
        public System.Decimal? AltPhaResponseTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_GRD_FAST_CRV")]
        [Display(Name = "Fast curve (Ground) :")]
        public System.String AltGrdFastCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_PHA_FAST_CRV")]
        [Display(Name = "Fast curve (Phase) :")]
        public System.String AltPhaFastCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_GRD_SLOW_CRV")]
        [Display(Name = "Slow curve (Ground) :")]
        public System.String AltGrdSlowCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_PHA_SLOW_CRV")]
        [Display(Name = "Slow curve (Phase) :")]
        public System.String AltPhaSlowCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_GRD_VMUL_FAST")]
        [Display(Name = "TMul Fast (Ground) :")]
        public System.Decimal? AltGrdVmulFast { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_PHA_SLOW_CRV_OPS")]
        [Display(Name = "Slow curve ops (Phase) :")]
        public System.Int16? AltPhaSlowCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_GRD_SLOW_CRV_OPS")]
        [Display(Name = "Slow curve ops (Ground) :")]
        public System.Int16? AltGrdSlowCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_PHA_VMUL_FAST")]
        [Display(Name = "TMul Fast (Phase) :")]
        public System.Decimal? AltPhaVmulFast { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_GRD_VMUL_SLOW")]
        [Display(Name = "TMul Slow (Ground) :")]
        public System.Decimal? AltGrdVmulSlow { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_PHA_VMUL_SLOW")]
        [Display(Name = "TMul Slow (Phase) :")]
        public System.Decimal? AltPhaVmulSlow { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT_GRD_TADD_FAST")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Fast (Ground) :")]
        public System.Decimal? AltGrdTaddFast { get; set; }

        //  [SettingsValidatorAttribute("Recloser", "ALT_PHA_TADD_FAST")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Fast (Phase) :")]
        public System.Decimal? AltPhaTaddFast { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT_GRD_TADD_SLOW")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Slow (Ground) :")]
        public System.Decimal? AltGrdTaddSlow { get; set; }

        //[SettingsValidatorAttribute("Recloser", "ALT_PHA_TADD_SLOW")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Slow (Phase) :")]
        public System.Decimal? AltPhaTaddSlow { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_TOT_LOCKOUT_OPS")]
        [Display(Name = "Ops to lockout :")]
        public System.Int16? AltTotOpsToLockout { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_RECLOSE1_TIME")]
        [Display(Name = "1st reclose :")]
        public System.Decimal? AltReclose1Time { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_RECLOSE2_TIME")]
        [Display(Name = "2nd reclose :")]
        public System.Decimal? AltReclose2Time { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_RECLOSE3_TIME")]
        [Display(Name = "3rd reclose :")]
        public System.Decimal? AltReclose3Time { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_RESET")]
        [Display(Name = "Reset Time :")]
        public System.Int16? AltResetTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_RECLOSE_RETRY_ENABLED")]
        [Display(Name = "Reclose retry enabled :")]
        public System.String AltRecloseRetryEnabled { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_SGF_CD")]
        [Display(Name = "Sensitive Ground Fault (SGF) :")]
        public System.String AltSfg { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_SGF_MIN_TRIP_PERCENT")]
        [Display(Name = "Min. Trip % :")]
        public System.Decimal? AltSgfMinTripPercent { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_SGF_TIME_DELAY")]
        [Display(Name = "Time Delay :")]
        public System.Decimal? AltSgfTimeDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_SGF_LOCKOUT")]
        [Display(Name = "Lockout :")]
        public System.Int16? AltSgfLockout { get; set; }


        [SettingsValidatorAttribute("Recloser", "ALT_HIGH_CURRENT_LOCKOUT_USED")]
        [Display(Name = "High current lockout used :")]
        public System.String AltHighCurrentLockoutUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_HIGH_CURRENT_LOCKOUT_PHA")]
        [Display(Name = "Phase :")]
        public System.String AltHighCurrentLockoutPhase { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_HIGH_CURRENT_LOCKUOUT_GRD")]
        [Display(Name = "Ground :")]
        public System.String AltHighCurrentLockoutGround { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_COLD_LOAD_PLI_USED")]
        [Display(Name = "Cold load PU used :")]
        public System.String AltColdLoadPliUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_COLD_LOAD_PLI_PHA")]
        [Display(Name = "Phase :")]
        public System.Int64? AltColdLoadPliPhase { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_COLD_LOAD_PLI_GRD")]
        [Display(Name = "Ground :")]
        public System.Int64? AltColdLoadPliGround { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_COLD_LOAD_PLI_CURVE_PHA")]
        [Display(Name = "Curve (Phase) :")]
        public System.String AltColdLoadPliCurvePha { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_COLD_LOAD_PLI_CURVE_GRD")]
        [Display(Name = "Curve (Ground) :")]
        public System.String AltColdLoadPliCurveGround { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_PERMIT_LS_ENABLING")]
        [Display(Name = "Permit LS Enabling :")]
        public System.String Alt2PermitLsEnabling { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_GRD_ARMING_THRESHOLD")]
        [Display(Name = "Arming Threshold (ground) :")]
        public System.Int16? Alt2GrdArmingThreshold { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_PHA_ARMING_THRESHOLD")]
        [Display(Name = "Arming Threshold (phase) :")]
        public System.Int16? Alt2PhaArmingThreshold { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_GRD_INRUSH_THRESHOLD")]
        [Display(Name = "Inrush Threshold (ground) :")]
        public System.Int16? Alt2GrdInrushThreshold { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_PHA_INRUSH_THRESHOLD")]
        [Display(Name = "Inrush Threshold (phase) :")]
        public System.Int16? Alt2PhaInrushThreshold { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_INRUSH_DURATION")]
        [Display(Name = "Inrush duration :")]
        public System.Int16? Alt2InrushDuration { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_LS_LOCKOUT_OPS")]
        [Display(Name = "Ops to Lockout :")]
        public System.Int16? Alt2LsOpsToLockout { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_LS_RESET_TIME")]
        [Display(Name = "Reset Time :")]
        public System.Int16? Alt2LsResetTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_GRD_MIN_TRIP")]
        [Display(Name = "Minimum Trip (Ground) :")]
        public System.Int16? Alt2GrdMinTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_PHA_MIN_TRIP")]
        [Display(Name = "Minimum Trip (Phase) :")]
        public System.Int16? Alt2PhaMinTrip { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT2_GRD_INST_TRIP_CD")]
        [Range(0, 99.99, ErrorMessage = "Enter number between 0 to 99.99")]
        [Display(Name = "Inst. Trip (Ground) :")]
        public System.Decimal? Alt2GrdInstTrip { get; set; }

        //[SettingsValidatorAttribute("Recloser", "ALT2_PHA_INST_TRIP_CD")]
        [Range(0, 99.99, ErrorMessage = "Enter number between 0 to 99.99")]
        [Display(Name = "Inst. Trip (Phase) :")]
        public System.Decimal? Alt2PhaInstTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_GRD_FAST_CRV")]
        [Display(Name = "Curve (Ground) :")]
        public System.String Alt2GrdCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_PHA_FAST_CRV")]
        [Display(Name = "Curve (Phase) :")]
        public System.String Alt2PhaCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_PHA_VMUL_FAST")]
        [Display(Name = "TMul (Phase) :")]
        public System.Decimal? Alt2PhaVmul { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT2_GRD_VMUL_SLOW")]
        [Display(Name = "TMul (Ground) :")]
        public System.Decimal? Alt2GrdVmul { get; set; }

        //  [SettingsValidatorAttribute("Recloser", "ALT2_GRD_TADD_FAST")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd (Ground) :")]
        public System.Decimal? Alt2GrdTadd { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT2_PHA_TADD_FAST")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd (Phase) :")]
        public System.Decimal? Alt2PhaTadd { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_GRD_MIN_TRIP")]
        [Display(Name = "Minimum Trip (Ground) :")]
        public System.Int16? Alt3GrdMinTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_PHA_MIN_TRIP")]
        [Display(Name = "Minimum Trip (Phase) :")]
        public System.Int16? Alt3PhaMinTrip { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT3_GRD_INST_TRIP_CD")]
        [Range(0, 99.99, ErrorMessage = "Enter number between 0 to 99.99")]
        [Display(Name = "Inst. Trip (Ground) :")]
        public System.Decimal? Alt3GrdInstTrip { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT3_PHA_INST_TRIP_CD")]
        [Range(0, 99.99, ErrorMessage = "Enter number between 0 to 99.99")]
        [Display(Name = "Inst. Trip (Phase) :")]
        public System.Decimal? Alt3PhaInstTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_GRD_OP_F_CRV")]
        [Display(Name = "Fast curve ops. (Ground) :")]
        public System.Int16? Alt3GrdFastCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_PHA_OP_F_CRV")]
        [Display(Name = "Fast curve ops. (Phase) :")]
        public System.Int16? Alt3PhaFastCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_TCC1_FAST_CURVES_USED")]
        [Display(Name = "TCC1: Fast curves used :")]
        public System.String Alt3Tcc1FastCurvesUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_TCC2_SLOW_CURVES_USED")]
        [Display(Name = "TCC2: Slow curves used :")]
        public System.String Alt3Tcc2SlowCurvesUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_GRD_FAST_CRV")]
        [Display(Name = "Fast curve (Ground) :")]
        public System.String Alt3GrdFastCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_PHA_FAST_CRV")]
        [Display(Name = "Fast curve (Phase) :")]
        public System.String Alt3PhaFastCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_GRD_SLOW_CRV")]
        [Display(Name = "Slow curve (Ground) :")]
        public System.String Alt3GrdSlowCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_PHA_SLOW_CRV")]
        [Display(Name = "Slow curve (Phase) :")]
        public System.String Alt3PhaSlowCurve { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_GRD_VMUL_FAST")]
        [Display(Name = "TMul Fast (Ground) :")]
        public System.Decimal? Alt3GrdVmulFast { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_PHA_SLOW_CRV_OPS")]
        [Display(Name = "Slow curve ops (Phase) :")]
        public System.Int16? Alt3PhaSlowCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_GRD_SLOW_CRV_OPS")]
        [Display(Name = "Slow curve ops (Ground) :")]
        public System.Int16? Alt3GrdSlowCurveOps { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_PHA_VMUL_FAST")]
        [Display(Name = "TMul Fast (Phase) :")]
        public System.Decimal? Alt3PhaVmulFast { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_GRD_VMUL_SLOW")]
        [Display(Name = "TMul Slow (Ground) :")]
        public System.Decimal? Alt3GrdVmulSlow { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_PHA_VMUL_SLOW")]
        [Display(Name = "TMul Slow (Phase) :")]
        public System.Decimal? Alt3PhaVmulSlow { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT3_GRD_TADD_FAST")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Fast (Ground) :")]
        public System.Decimal? Alt3GrdTaddFast { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT3_PHA_TADD_FAST")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Fast (Phase) :")]
        public System.Decimal? Alt3PhaTaddFast { get; set; }

        // [SettingsValidatorAttribute("Recloser", "ALT3_GRD_TADD_SLOW")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Slow (Ground) :")]
        public System.Decimal? Alt3GrdTaddSlow { get; set; }

        //[SettingsValidatorAttribute("Recloser", "ALT3_PHA_TADD_SLOW")]
        [Range(0, 1.0, ErrorMessage = "Enter number between 0 to 1")]
        [Display(Name = "TAdd Slow (Phase) :")]
        public System.Decimal? Alt3PhaTaddSlow { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_PHA_DELAY")]
        [Display(Name = "Delay (Ground) :")]
        public System.Decimal? Alt3PhaDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_GRD_DELAY")]
        [Display(Name = "Delay (Phase) :")]
        public System.Decimal? Alt3GrdDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_TOT_LOCKOUT_OPS")]
        [Display(Name = "Ops to lockout :")]
        public System.Int16? Alt3TotOpsToLockout { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_RECLOSE1_TIME")]
        [Display(Name = "1st reclose :")]
        public System.Decimal? Alt3Reclose1Time { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_RECLOSE2_TIME")]
        [Display(Name = "2nd reclose :")]
        public System.Decimal? Alt3Reclose2Time { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_RECLOSE3_TIME")]
        [Display(Name = "3rd reclose :")]
        public System.Decimal? Alt3Reclose3Time { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_RESET")]
        [Display(Name = "Reset Time :")]
        public System.Int16? Alt3ResetTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_RECLOSE_RETRY_ENABLED")]
        [Display(Name = "Reclose retry enabled :")]
        public System.String Alt3RecloseRetryEnabled { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_SGF_CD")]
        [Display(Name = "Sensitive Ground Fault (SGF) :")]
        public System.String Alt3SensitiveGroundFault { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_SGF_MIN_TRIP_PERCENT")]
        [Display(Name = "Min. Trip % :")]
        public System.Decimal? Alt3SgfMinTripPercent { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_SGF_TIME_DELAY")]
        [Display(Name = "Time Delay :")]
        public System.Decimal? Alt3SgfTimeDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_SGF_LOCKOUT")]
        [Display(Name = "Lockout :")]
        public System.Int16? Alt3SgfLockout { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_HIGH_CURRENT_LOCKOUT_USED")]
        [Display(Name = "High current lockout used :")]
        public System.String Alt3HighCurrentLockoutUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_HIGH_CURRENT_LOCKOUT_PHA")]
        [Display(Name = "Phase :")]
        public System.String Alt3HighCurrentLockoutPhase { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_HIGH_CURRENT_LOCKUOUT_GRD")]
        [Display(Name = "Ground :")]
        public System.String Alt3HighCurrentLockoutGround { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_COLD_LOAD_PLI_USED")]
        [Display(Name = "Cold load PU used :")]
        public System.String Alt3ColdLoadPliUsed { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_COLD_LOAD_PLI_PHA")]
        [Display(Name = "Phase :")]
        public System.Int64? Alt3ColdLoadPliPhase { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_COLD_LOAD_PLI_GRD")]
        [Display(Name = "Ground :")]
        public System.Int64? Alt3ColdLoadPliGround { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_COLD_LOAD_PLI_CURVE_PHA")]
        [Display(Name = "Curve (Phase) :")]
        public System.String Alt3ColdLoadPliCurvePhase { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT3_COLD_LOAD_PLI_CURVE_GRD")]
        [Display(Name = "Curve (Ground) :")]
        public System.String Alt3ColdLoadPliCurveGround { get; set; }

        [SettingsValidatorAttribute("Recloser", "ACTIVE_PROFILE")]
        [Display(Name = "Active Profile :")]
        public System.String ActiveProfile { get; set; }

        [SettingsValidatorAttribute("Recloser", "ENGINEERING_COMMENTS")]
        [Display(Name = "Engineering Comments :")]
        public System.String EngineeringComments { get; set; }

        [SettingsValidatorAttribute("Recloser", "FLISR_Engineering_Comments")]
        [Display(Name = "FLISR Comments :")]
        public System.String FlisrEngineeringComments { get; set; }

        [SettingsValidatorAttribute("Recloser", "FLISR")]
        [Display(Name = "FLISR Device :")]
        public System.String FlisrDevice { get; set; }

        [SettingsValidatorAttribute("Recloser", "Summer_Load_Limit")]
        [Display(Name = "Summer Load Limit (amps) :")]
        public System.Int64? SummerLoadLimit { get; set; }

        [SettingsValidatorAttribute("Recloser", "Winter_Load_Limit")]
        [Display(Name = "Winter Load Limit (amps) :")]
        public System.Int64? WinterLoadLimit { get; set; }

        [SettingsValidatorAttribute("Recloser", "Limiting_Factor")]
        [Display(Name = "Limiting Factor :")]
        public System.String LimitingFactor { get; set; }

        [SettingsValidatorAttribute("Recloser", "FIRMWARE_VERSION")]
        [Display(Name = "Firmware Version :")]
        public System.String FirmwareVersion { get; set; }

        [SettingsValidatorAttribute("Recloser", "SOFTWARE_VERSION")]
        [Display(Name = "Software Version :")]
        public System.String SoftwareVersion { get; set; }

        [SettingsValidatorAttribute("Recloser", "ENGINEERING_DOCUMENT")]
        [Display(Name = "Engineering Document :")]
        public System.String EngineeringDocument { get; set; }

        [SettingsValidatorAttribute("Recloser", "SCADA")]
        [Display(Name = "SCADA :")]
        public System.String Scada { get; set; }

        [SettingsValidatorAttribute("Recloser", "SCADA_TYPE")]
        [Display(Name = "SCADA Type :")]
        public System.String ScadaType { get; set; }

        [SettingsValidatorAttribute("Recloser", "MASTER_STATION")]
        [Display(Name = "Master Station :")]
        public System.String MasterStation { get; set; }

        [SettingsValidatorAttribute("Recloser", "BAUD_RATE")]
        [Display(Name = "Baud Rate :")]
        public System.Int32? BaudRate { get; set; }

        [SettingsValidatorAttribute("Recloser", "TRANSMIT_ENABLE_DELAY")]
        [Display(Name = "Transmit Enable Delay :")]
        public System.Int32? TransmitEnableDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "TRANSMIT_DISABLE_DELAY")]
        [Display(Name = "Transmit Disable Delay :")]
        public System.Int32? TransmitDisableDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "RTU_ADDRESS")]
        [Display(Name = "RTU Address :")]
        public System.String RtuAddress { get; set; }

        [SettingsValidatorAttribute("Recloser", "REPEATER")]
        [Display(Name = "(if applicable) Repeater :")]
        public System.String Repeater { get; set; }

        [SettingsValidatorAttribute("Recloser", "SPECIAL_CONDITIONS")]
        [Display(Name = "Special conditions :")]
        public System.String SpecialConditions { get; set; }

        [SettingsValidatorAttribute("Recloser", "RADIO_MANF_CD")]
        [Display(Name = "SCADA radio manufacturer :")]
        public System.String ScadaRadioManufacturer { get; set; }

        [SettingsValidatorAttribute("Recloser", "RADIO_MODEL_NUM")]
        [Display(Name = "SCADA radio model # :")]
        public System.String ScadaRadioModelNum { get; set; }

        [SettingsValidatorAttribute("Recloser", "PERMIT_RB_CUTIN")]
        [Display(Name = "Permit RB Cut-In :")]
        public System.String PermitRbCutIn { get; set; }

        [SettingsValidatorAttribute("Recloser", "RADIO_SERIAL_NUM")]
        [Display(Name = "SCADA radio serial # :")]
        public System.String ScadaRadioSerialNum { get; set; }

        [SettingsValidatorAttribute("Recloser", "DIRECT_TRANSFER_TRIP")]
        [Display(Name = "Transfer Trip :")]
        public System.String DirectTransferTrip { get; set; }

        [SettingsValidatorAttribute("Recloser", "BOC_VOLTAGE")]
        [Display(Name = "BOC Voltage :")]
        public System.Int16? BocVoltage { get; set; }

        [SettingsValidatorAttribute("Recloser", "RB_CUTOUT_TIME")]
        [Display(Name = "RB C/OUT Time :")]
        public System.Int16? RbCutoutTime { get; set; }



        /*New fields Settingsgaps*/
        [SettingsValidatorAttribute("Recloser", "RTU_EXISTS")]
        [Display(Name = "Is Multi-Functional? :")]
        public System.String IsMultiFunctional { get; set; }

        [SettingsValidatorAttribute("Recloser", "RTU_EXISTS")]
        [Display(Name = "External RTU :")]
        public System.String RTUExists { get; set; }


        [SettingsValidatorAttribute("Recloser", "RTU_MANF_CD")]
        [Display(Name = "RTU Manufacturer :")]
        public System.String RTUManufacture { get; set; }


        [SettingsValidatorAttribute("Recloser", "RTU_MODEL_NUM")]
        [Display(Name = "RTU Model # :")]
        public System.String RTUModelNumber { get; set; }


        [SettingsValidatorAttribute("Recloser", "RTU_SERIAL_NUM")]
        [Display(Name = "RTU Serial # :")]
        public System.String RTUSerialNumber { get; set; }


        [SettingsValidatorAttribute("Recloser", "RTU_SOFTWARE_VERSION")]
        [Display(Name = "RTU Software version :")]
        public System.String RTUSoftwareVersion { get; set; }


        [SettingsValidatorAttribute("Recloser", "RTU_FIRMWARE_VERSION")]
        [Display(Name = "RTU Firmware version :")]
        public System.String RTUFirmwareVersion { get; set; }

        //INC000004115018 START
        [SettingsValidatorAttribute("Recloser", "ALT_GRD_LOCKOUT_OPS")]
        [Display(Name = "Ops to lockout (Ground)  :")]
        public System.Int16? AltGrdOpsToLockout { get; set; }

        [SettingsValidatorAttribute("Recloser", "ALT_PHA_LOCKOUT_OPS")]
        [Display(Name = "Ops to lockout (Phase)  :")]
        public System.Int16? AltPhaOpsToLockout { get; set; }
        //INC000004115018 END

        public SelectList RTUManufactureList { get; set; }

        /* end of changes */

        //DA# 200101 - ME Q1 2020---START
        [SettingsValidatorAttribute("Recloser", "DEF_TIME_PHA")]
        [Display(Name = "Definite Time (Phase) :")]
        public System.Decimal? PhaDefiniteTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "DEF_TIME_GROUND")]
        [Display(Name = "Definite Time (Ground) :")]
        public System.Decimal? GrdDefiniteTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "MIN_RESP_TADDR_PHA")]
        [Display(Name = "Min. Response TAddr (Phase) :")]
        public System.Decimal? PhaMinResponseTAddr { get; set; }

        [SettingsValidatorAttribute("Recloser", "MIN_RESP_TADDR_GRO")]
        [Display(Name = "Min. Response TAddr (Ground) :")]
        public System.Decimal? GrdMinResponseTAddr { get; set; }

        [SettingsValidatorAttribute("Recloser", "PHASE")]
        [Display(Name = "Phase :")]
        public System.Int32? Phase { get; set; }

        [SettingsValidatorAttribute("Recloser", "GROUND")]
        [Display(Name = "Ground :")]
        public System.Int16? Ground { get; set; }

        [SettingsValidatorAttribute("Recloser", "OPT_LOCK_PHA")]
        [Display(Name = "Ops to lockout :")]
        public System.Int16? PhaOpsToLock { get; set; }

        [SettingsValidatorAttribute("Recloser", "OPT_LOCK_GRO")]
        [Display(Name = "Ops to lockout :")]
        public System.Int16? GrdOpsToLock { get; set; }

        [SettingsValidatorAttribute("Recloser", "FIRST_RECLO_PHA")]
        [Display(Name = "1st reclose :")]
        public System.Decimal? PhaFirstRec { get; set; }

        [SettingsValidatorAttribute("Recloser", "FIRST_RECLO_GRO")]
        [Display(Name = "1st reclose :")]
        public System.Decimal? GrdFirstRec { get; set; }

        [SettingsValidatorAttribute("Recloser", "SEC_RECLO_PHA")]
        [Display(Name = "2nd reclose :")]
        public System.Decimal? PhaSecondRec { get; set; }

        [SettingsValidatorAttribute("Recloser", "SEC_RECLO_GRO")]
        [Display(Name = "2nd reclose :")]
        public System.Decimal? GrdSecondRec { get; set; }

        [SettingsValidatorAttribute("Recloser", "THIRD_RECLO_PHA")]
        [Display(Name = "3rd reclose :")]
        public System.Decimal? PhaThirdRec { get; set; }

        [SettingsValidatorAttribute("Recloser", "THIRD_RECLO_GRO")]
        [Display(Name = "3rd reclose :")]
        public System.Decimal? GrdThirdRec { get; set; }

        [SettingsValidatorAttribute("Recloser", "RESET_TIME")]
        [Display(Name = "Reset Time :")]
        public System.Int16? ResetTimeBeck { get; set; }

        [SettingsValidatorAttribute("Recloser", "RESET_TIME_LOCK")]
        [Display(Name = "Reset time From Lockout :")]
        public System.Int16? ResetTimeFrmLock { get; set; }

        [SettingsValidatorAttribute("Recloser", "MTT_PHA")]
        [Display(Name = "MTT :")]
        public System.Int32? PhaMTTHighCurLock { get; set; }

        [SettingsValidatorAttribute("Recloser", "MTT_GROU")]
        [Display(Name = "MTT :")]
        public System.Int32? GrdMTTHighCurLock { get; set; }

        [SettingsValidatorAttribute("Recloser", "TIME_DEL_PHA")]
        [Display(Name = "Time Delay :")]
        public System.Decimal? PhaTimeDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "TIME_DEL_GROU")]
        [Display(Name = "Time Delay :")]
        public System.Decimal? GrdTimeDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "PU_MTT_PHA")]
        [Display(Name = "MTT :")]
        public System.Int16? PhaMTTPU { get; set; }

        [SettingsValidatorAttribute("Recloser", "PU_MTT_GROU")]
        [Display(Name = "MTT :")]
        public System.Int16? GrdMTTPU { get; set; }

        [SettingsValidatorAttribute("Recloser", "PU_CURVE_PHA")]
        [Display(Name = "Curve :")]
        public System.String PhaCurvePU { get; set; }

        [SettingsValidatorAttribute("Recloser", "PU_CURVE_GROU")]
        [Display(Name = "Curve :")]
        public System.String GrdCurvePU { get; set; }

        [SettingsValidatorAttribute("Recloser", "TADDR_PHA")]
        [Display(Name = "TAddr :")]
        public System.Decimal? PhaTAddr { get; set; }

        [SettingsValidatorAttribute("Recloser", "TADDR_GROU")]
        [Display(Name = "TAddr :")]
        public System.Decimal? GrdTAddr { get; set; }

        [SettingsValidatorAttribute("Recloser", "MIN_RESP_TADDR_T_D_PHA")]
        [Display(Name = "Min Resp TAddr Time Delay :")]
        public System.Decimal? PhaMinRespTAddrTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "MIN_RESP_TADDR_T_D_GR")]
        [Display(Name = "Min Resp TAddr Time Delay :")]
        public System.Decimal? GrdMinRespTAddrTime { get; set; }

        [SettingsValidatorAttribute("Recloser", "TIME_T_ACTI")]
        [Display(Name = "Time Locked out to Activate CL :")]
        public System.Int32? TimeToActivate { get; set; }

        [SettingsValidatorAttribute("Recloser", "FAULT_CURR_ONLY")]
        [Display(Name = "Fault Current Only")]
        public System.String FaultCurrentOnly { get; set; }

        [SettingsValidatorAttribute("Recloser", "VOLT_LOSS_ONLY")]
        [Display(Name = "Voltage Loss Only")]
        public System.String VoltageLossOnly { get; set; }

        [SettingsValidatorAttribute("Recloser", "FAULT_CURR_W_VOL_LOSS")]
        [Display(Name = "Fault Current with Voltage Loss (Voltage Restraint)")]
        public System.String FaultCurVoltLoss { get; set; }

        [SettingsValidatorAttribute("Recloser", "VOLT_LOSS_DISP")]
        [Display(Name = "Voltage Loss Delay :")]
        public System.Decimal? VoltLossDelay { get; set; }

        [SettingsValidatorAttribute("Recloser", "COUT_T_TRIP_VOLT_LOSS")]
        [Display(Name = "Counts to Trip Voltage Loss (Voltage Restraint) :")]
        public System.Int16? CountToTripVolt { get; set; }

        [SettingsValidatorAttribute("Recloser", "RESET_TIMERR")]
        [Display(Name = "Reset Timer :")]
        public System.Decimal? ResetTimerSecMode { get; set; }

        public SelectList PhaCurveColdLoadPUList { get; set; }
        public SelectList GrdCurveColdLoadPUList { get; set; }
        public SelectList CountToTripVoltList { get; set; }

        public string DropDownPostbackScriptActiveProfile { get; set; }
        public bool HLTEnabled { get; set; }
        public bool EnabSwitchMode { get; set; }
        public bool SeqCoordMode { get; set; }
        //DA# 200101 - ME Q1 2020---END




        //DA# 200101
        public bool Release { get; set; }
        public List<GISAttributes> GISAttributes { get; set; }

        public string DropDownPostbackScript { get; set; }
        public string DropDownPostbackScriptScada { get; set; }

        public HashSet<string> FieldsToDisplay { get; set; }

        public SelectList ControllerUnitTypeList { get; set; }
        public SelectList OperatingModeList { get; set; }
        public SelectList EngineeringDocumentList { get; set; }
        public SelectList AltPhaInstTripList { get; set; }
        public SelectList Alt3PhaInstTripList { get; set; }
        public SelectList PhaInstTripList { get; set; }
        public SelectList AltPhaFastCurveList { get; set; }
        public SelectList Alt3PhaFastCurveList { get; set; }
        public SelectList PhaFastCurveList { get; set; }
        public SelectList Alt2PhaInstTripList { get; set; }
        public SelectList Alt2PhaCurveList { get; set; }
        public SelectList AltPhaSlowCurveList { get; set; }
        public SelectList Alt3PhaSlowCurveList { get; set; }
        public SelectList PhaSlowCurveList { get; set; }
        public SelectList AltPhaResponseTimeList { get; set; }
        public SelectList PhaResponseTimeList { get; set; }
        public SelectList ScadaTypeList { get; set; }
        public SelectList ScadaRadioManufacturerList { get; set; }
        public SelectList AltGrdInstTripList { get; set; }
        public SelectList Alt3GrdInstTripList { get; set; }
        public SelectList GrdInstTripList { get; set; }
        public SelectList AltGrdFastCurveList { get; set; }
        public SelectList Alt3GrdFastCurveList { get; set; }
        public SelectList GrdFastCurveList { get; set; }
        public SelectList Alt2GrdInstTripList { get; set; }
        public SelectList Alt2GrdCurveList { get; set; }
        public SelectList AltGrdSlowCurveList { get; set; }
        public SelectList Alt3GrdSlowCurveList { get; set; }
        public SelectList GrdSlowCurveList { get; set; }
        public SelectList AltGrdResponseTimeList { get; set; }
        public SelectList GrdResponseTimeList { get; set; }
        public SelectList AltSfgList { get; set; }
        public SelectList Alt3SensitiveGroundFaultList { get; set; }
        public SelectList SgfCdList { get; set; }
        public SelectList AltHighCurrentLockoutPhaseList { get; set; }
        public SelectList Alt3HighCurrentLockoutPhaseList { get; set; }
        public SelectList HighCurrentLockoutPhaseList { get; set; }
        public SelectList AltHighCurrentLockoutGroundList { get; set; }
        public SelectList Alt3HighCurrentLockoutGroundList { get; set; }
        public SelectList HighCurrentLockoutGroundList { get; set; }
        public SelectList ActiveProfileList { get; set; }
        public SelectList GrdOpsToLockoutList { get; set; }
        public SelectList PhaOpsToLockoutList { get; set; }

        //INC000004115018 START
        public SelectList AltGrdOpsToLockoutList { get; set; }
        public SelectList AltPhaOpsToLockoutList { get; set; }
        //INC000004115018 END


        public void PopulateEntityFromModel(SM_RECLOSER e)
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
            e.CONTROL_TYPE = this.ControllerUnitType;
            e.BYPASS_PLANS = this.BypassPlan;
            e.OPERATING_AS_CD = this.OperatingMode;
            e.GRD_MIN_TRIP = this.GrdMinTrip;
            e.PHA_MIN_TRIP = this.PhaMinTrip;
            e.GRD_INST_TRIP_CD = this.GrdInstTrip;
            e.PHA_INST_TRIP_CD = this.PhaInstTrip;
            e.TCC1_FAST_CURVES_USED = this.Tcc1FastCurvesUsed;
            e.TCC2_SLOW_CURVES_USED = this.Tcc2SlowCurvesUsed;
            e.GRD_OP_F_CRV = this.GrdOpFastCurveOps;
            e.PHA_OP_F_CRV = this.PhaFastCurveOps;
            e.GRD_RESP_TIME = this.GrdResponseTime;
            e.PHA_RESP_TIME = this.PhaResponseTime;
            e.GRD_FAST_CRV = this.GrdFastCurve;
            e.PHA_FAST_CRV = this.PhaFastCurve;
            e.GRD_SLOW_CRV = this.GrdSlowCurve;
            e.PHA_SLOW_CRV = this.PhaSlowCurve;
            e.PHA_SLOW_CRV_OPS = this.PhaSlowCurveOps;
            e.GRD_SLOW_CRV_OPS = this.GrdSlowCurveOps;
            e.GRD_TMUL_FAST = this.GrdTmulFast;
            e.PHA_TMUL_FAST = this.PhaTmulFast;
            e.GRD_TMUL_SLOW = this.GrdTmulSlow;
            e.PHA_TMUL_SLOW = this.PhaTmulSlow;
            //ME Q2 
            e.COLD_PHA_TMUL_FAST = this.ColdGrdTmulFast;
            e.COLD_PHA_TMUL_FAST = this.ColdPhaTmulFast;
            e.GRD_TADD_FAST = this.GrdTaddFast;
            e.PHA_TADD_FAST = this.PhaTaddFast;
            e.GRD_TADD_SLOW = this.GrdTaddSlow;
            e.PHA_TADD_SLOW = this.PhaTaddSlow;
            e.TOT_LOCKOUT_OPS = this.TotOpsToLockout;
            e.PHA_LOCKOUT_OPS = this.PhaOpsToLockout;
            e.GRD_LOCKOUT_OPS = this.GrdOpsToLockout;
            e.RECLOSE1_TIME = this.Reclose1Time;
            e.RECLOSE2_TIME = this.Reclose2Time;
            e.RECLOSE3_TIME = this.Reclose3Time;
            e.RESET = this.ResetTime;
            e.RECLOSE_RETRY_ENABLED = this.RecloseRetryEnabled;
            e.SGF_CD = this.SgfCd;
            e.SGF_MIN_TRIP_PERCENT = this.SgfMinTripPercent;
            e.SGF_TIME_DELAY = this.SgfTimeDelay;
            e.SGF_LOCKOUT = this.SgfLockout;
            e.HIGH_CURRENT_LOCKOUT_USED = this.HighCurrentLockoutUsed;
            e.HIGH_CURRENT_LOCKOUT_PHA = this.HighCurrentLockoutPhase;
            e.HIGH_CURRENT_LOCKUOUT_GRD = this.HighCurrentLockoutGround;
            e.COLD_LOAD_PLI_USED = this.ColdLoadPliUsed;
            e.COLD_LOAD_PLI_PHA = this.ColdLoadPliPhase;
            e.COLD_LOAD_PLI_GRD = this.ColdLoadPliGround;
            e.COLD_LOAD_PLI_CURVE_PHA = this.ColdLoadPliCurvePhase;
            e.COLD_LOAD_PLI_CURVE_GRD = this.ColdLoadPliCurveGround;
            e.ALT_GRD_MIN_TRIP = this.AltGrdMinTrip;
            e.ALT_PHA_MIN_TRIP = this.AltPhaMinTrip;
            e.ALT_GRD_INST_TRIP_CD = this.AltGrdInstTrip;
            e.ALT_PHA_INST_TRIP_CD = this.AltPhaInstTrip;
            e.ALT_GRD_OP_F_CRV = this.AltGrdFastCurveOps;
            e.ALT_PHA_OP_F_CRV = this.AltPhaFastCurveOps;
            e.ALT_TCC1_FAST_CURVES_USED = this.AltTcc1FastCurvesUsed;
            e.ALT_TCC2_SLOW_CURVES_USED = this.AltTcc2SlowCurvesUsed;
            e.ALT_GRD_RESP_TIME = this.AltGrdResponseTime;
            e.ALT_PHA_RESP_TIME = this.AltPhaResponseTime;
            e.ALT_GRD_FAST_CRV = this.AltGrdFastCurve;
            e.ALT_PHA_FAST_CRV = this.AltPhaFastCurve;
            e.ALT_GRD_SLOW_CRV = this.AltGrdSlowCurve;
            e.ALT_PHA_SLOW_CRV = this.AltPhaSlowCurve;
            e.ALT_GRD_VMUL_FAST = this.AltGrdVmulFast;
            e.ALT_PHA_SLOW_CRV_OPS = this.AltPhaSlowCurveOps;
            e.ALT_GRD_SLOW_CRV_OPS = this.AltGrdSlowCurveOps;
            e.ALT_PHA_VMUL_FAST = this.AltPhaVmulFast;
            e.ALT_GRD_VMUL_SLOW = this.AltGrdVmulSlow;
            e.ALT_PHA_VMUL_SLOW = this.AltPhaVmulSlow;
            e.ALT_GRD_TADD_FAST = this.AltGrdTaddFast;
            e.ALT_PHA_TADD_FAST = this.AltPhaTaddFast;
            e.ALT_GRD_TADD_SLOW = this.AltGrdTaddSlow;
            e.ALT_PHA_TADD_SLOW = this.AltPhaTaddSlow;
            e.ALT_TOT_LOCKOUT_OPS = this.AltTotOpsToLockout;
            e.ALT_RECLOSE1_TIME = this.AltReclose1Time;
            e.ALT_RECLOSE2_TIME = this.AltReclose2Time;
            e.ALT_RECLOSE3_TIME = this.AltReclose3Time;
            e.ALT_RESET = this.AltResetTime;
            e.ALT_RECLOSE_RETRY_ENABLED = this.AltRecloseRetryEnabled;
            e.ALT_SGF_CD = this.AltSfg;
            e.ALT_SGF_MIN_TRIP_PERCENT = this.AltSgfMinTripPercent;
            e.ALT_SGF_TIME_DELAY = this.AltSgfTimeDelay;
            e.ALT_SGF_LOCKOUT = this.AltSgfLockout;
            e.ALT_HIGH_CURRENT_LOCKOUT_USED = this.AltHighCurrentLockoutUsed;
            e.ALT_HIGH_CURRENT_LOCKOUT_PHA = this.AltHighCurrentLockoutPhase;
            e.ALT_HIGH_CURRENT_LOCKUOUT_GRD = this.AltHighCurrentLockoutGround;
            e.ALT_COLD_LOAD_PLI_USED = this.AltColdLoadPliUsed;
            e.ALT_COLD_LOAD_PLI_PHA = this.AltColdLoadPliPhase;
            e.ALT_COLD_LOAD_PLI_GRD = this.AltColdLoadPliGround;
            e.ALT_COLD_LOAD_PLI_CURVE_PHA = this.AltColdLoadPliCurvePha;
            e.ALT_COLD_LOAD_PLI_CURVE_GRD = this.AltColdLoadPliCurveGround;
            e.ALT2_PERMIT_LS_ENABLING = this.Alt2PermitLsEnabling;
            e.ALT2_GRD_ARMING_THRESHOLD = this.Alt2GrdArmingThreshold;
            e.ALT2_PHA_ARMING_THRESHOLD = this.Alt2PhaArmingThreshold;
            e.ALT2_GRD_INRUSH_THRESHOLD = this.Alt2GrdInrushThreshold;
            e.ALT2_PHA_INRUSH_THRESHOLD = this.Alt2PhaInrushThreshold;
            e.ALT2_INRUSH_DURATION = this.Alt2InrushDuration;
            e.ALT2_LS_LOCKOUT_OPS = this.Alt2LsOpsToLockout;
            e.ALT2_LS_RESET_TIME = this.Alt2LsResetTime;
            e.ALT2_GRD_MIN_TRIP = this.Alt2GrdMinTrip;
            e.ALT2_PHA_MIN_TRIP = this.Alt2PhaMinTrip;
            e.ALT2_GRD_INST_TRIP_CD = this.Alt2GrdInstTrip;
            e.ALT2_PHA_INST_TRIP_CD = this.Alt2PhaInstTrip;
            e.ALT2_GRD_FAST_CRV = this.Alt2GrdCurve;
            e.ALT2_PHA_FAST_CRV = this.Alt2PhaCurve;
            e.ALT2_PHA_VMUL_FAST = this.Alt2PhaVmul;
            e.ALT2_GRD_VMUL_SLOW = this.Alt2GrdVmul;
            e.ALT2_GRD_TADD_FAST = this.Alt2GrdTadd;
            e.ALT2_PHA_TADD_FAST = this.Alt2PhaTadd;
            e.ALT3_GRD_MIN_TRIP = this.Alt3GrdMinTrip;
            e.ALT3_PHA_MIN_TRIP = this.Alt3PhaMinTrip;
            e.ALT3_GRD_INST_TRIP_CD = this.Alt3GrdInstTrip;
            e.ALT3_PHA_INST_TRIP_CD = this.Alt3PhaInstTrip;
            e.ALT3_GRD_OP_F_CRV = this.Alt3GrdFastCurveOps;
            e.ALT3_PHA_OP_F_CRV = this.Alt3PhaFastCurveOps;
            e.ALT3_TCC1_FAST_CURVES_USED = this.Alt3Tcc1FastCurvesUsed;
            e.ALT3_TCC2_SLOW_CURVES_USED = this.Alt3Tcc2SlowCurvesUsed;
            e.ALT3_GRD_FAST_CRV = this.Alt3GrdFastCurve;
            e.ALT3_PHA_FAST_CRV = this.Alt3PhaFastCurve;
            e.ALT3_GRD_SLOW_CRV = this.Alt3GrdSlowCurve;
            e.ALT3_PHA_SLOW_CRV = this.Alt3PhaSlowCurve;
            e.ALT3_GRD_VMUL_FAST = this.Alt3GrdVmulFast;
            e.ALT3_PHA_SLOW_CRV_OPS = this.Alt3PhaSlowCurveOps;
            e.ALT3_GRD_SLOW_CRV_OPS = this.Alt3GrdSlowCurveOps;
            e.ALT3_PHA_VMUL_FAST = this.Alt3PhaVmulFast;
            e.ALT3_GRD_VMUL_SLOW = this.Alt3GrdVmulSlow;
            e.ALT3_PHA_VMUL_SLOW = this.Alt3PhaVmulSlow;
            e.ALT3_GRD_TADD_FAST = this.Alt3GrdTaddFast;
            e.ALT3_PHA_TADD_FAST = this.Alt3PhaTaddFast;
            e.ALT3_GRD_TADD_SLOW = this.Alt3GrdTaddSlow;
            e.ALT3_PHA_TADD_SLOW = this.Alt3PhaTaddSlow;
            e.ALT3_PHA_DELAY = this.Alt3PhaDelay;
            e.ALT3_GRD_DELAY = this.Alt3GrdDelay;
            e.ALT3_TOT_LOCKOUT_OPS = this.Alt3TotOpsToLockout;
            e.ALT3_RECLOSE1_TIME = this.Alt3Reclose1Time;
            e.ALT3_RECLOSE2_TIME = this.Alt3Reclose2Time;
            e.ALT3_RECLOSE3_TIME = this.Alt3Reclose3Time;
            e.ALT3_RESET = this.Alt3ResetTime;
            e.ALT3_RECLOSE_RETRY_ENABLED = this.Alt3RecloseRetryEnabled;
            e.ALT3_SGF_CD = this.Alt3SensitiveGroundFault;
            e.ALT3_SGF_MIN_TRIP_PERCENT = this.Alt3SgfMinTripPercent;
            e.ALT3_SGF_TIME_DELAY = this.Alt3SgfTimeDelay;
            e.ALT3_SGF_LOCKOUT = this.Alt3SgfLockout;
            e.ALT3_HIGH_CURRENT_LOCKOUT_USED = this.Alt3HighCurrentLockoutUsed;
            e.ALT3_HIGH_CURRENT_LOCKOUT_PHA = this.Alt3HighCurrentLockoutPhase;
            e.ALT3_HIGH_CURRENT_LOCKUOUT_GRD = this.Alt3HighCurrentLockoutGround;
            e.ALT3_COLD_LOAD_PLI_USED = this.Alt3ColdLoadPliUsed;
            e.ALT3_COLD_LOAD_PLI_PHA = this.Alt3ColdLoadPliPhase;
            e.ALT3_COLD_LOAD_PLI_GRD = this.Alt3ColdLoadPliGround;
            e.ALT3_COLD_LOAD_PLI_CURVE_PHA = this.Alt3ColdLoadPliCurvePhase;
            e.ALT3_COLD_LOAD_PLI_CURVE_GRD = this.Alt3ColdLoadPliCurveGround;
            e.ACTIVE_PROFILE = this.ActiveProfile;
            e.ENGINEERING_COMMENTS = this.EngineeringComments;
            e.FLISR = this.FlisrDevice;
            e.Summer_Load_Limit = this.SummerLoadLimit;
            e.Winter_Load_Limit = this.WinterLoadLimit;
            e.Limiting_Factor = this.LimitingFactor;
            e.FLISR_ENGINEERING_COMMENTS = this.FlisrEngineeringComments;
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
            e.PERMIT_RB_CUTIN = this.PermitRbCutIn;
            e.DIRECT_TRANSFER_TRIP = this.DirectTransferTrip;
            e.BOC_VOLTAGE = this.BocVoltage;
            e.RB_CUTOUT_TIME = this.RbCutoutTime;
            e.RTU_EXIST = this.RTUExists;
            e.RTU_MANF_CD = this.RTUManufacture;
            e.RTU_FIRMWARE_VERSION = this.RTUFirmwareVersion;
            e.RTU_MODEL_NUM = this.RTUModelNumber;
            e.RTU_SERIAL_NUM = this.RTUSerialNumber;
            e.RTU_SOFTWARE_VERSION = this.RTUSoftwareVersion;
            e.MULTI_FUNCTIONAL = this.IsMultiFunctional;

            //INC000004115018 start
            e.ALT_PHA_LOCKOUT_OPS = this.AltPhaOpsToLockout;
            e.ALT_GRD_LOCKOUT_OPS = this.AltGrdOpsToLockout;
            //INC000004115018 end

            //DA# 200101 - ME Q1 2020---START
            e.DEF_TIME_PHA = this.PhaDefiniteTime;
            e.DEF_TIME_GROUND = this.GrdDefiniteTime;
            e.MIN_RESP_TADDR_PHA = this.PhaMinResponseTAddr;
            e.MIN_RESP_TADDR_GRO = this.GrdMinResponseTAddr;
            if (this.HLTEnabled == true)
                e.HLT_ENAB = "Y";
            else
                e.HLT_ENAB = "N";
            e.PHASE = this.Phase;
            e.GROUND = this.Ground;
            e.OPT_LOCK_PHA = this.PhaOpsToLock;
            e.OPT_LOCK_GRO = this.GrdOpsToLock;
            e.FIRST_RECLO_PHA = this.PhaFirstRec;
            e.FIRST_RECLO_GRO = this.GrdFirstRec;
            e.SEC_RECLO_PHA = this.PhaSecondRec;
            e.SEC_RECLO_GRO = this.GrdSecondRec;
            e.THIRD_RECLO_PHA = this.PhaThirdRec;
            e.THIRD_RECLO_GRO = this.GrdThirdRec;
            e.RESET_TIME_LOCK = this.ResetTimeFrmLock;
            e.MTT_PHA = this.PhaMTTHighCurLock;
            e.MTT_GROU = this.GrdMTTHighCurLock;
            e.TIME_DEL_PHA = this.PhaTimeDelay;
            e.TIME_DEL_GROU = this.GrdTimeDelay;
            e.PU_MTT_PHA = this.PhaMTTPU;
            e.PU_MTT_GROU = this.GrdMTTPU;
            e.PU_CURVE_PHA = this.PhaCurvePU;
            e.PU_CURVE_GROU = this.GrdCurvePU;
            e.TADDR_PHA = this.PhaTAddr;
            e.TADDR_GROU = this.GrdTAddr;
            e.MIN_RESP_TADDR_T_D_PHA = this.PhaMinRespTAddrTime;
            e.MIN_RESP_TADDR_T_D_GR = this.GrdMinRespTAddrTime;
            e.TIME_T_ACTI = this.TimeToActivate;
            e.FAULT_CURR_ONLY = this.FaultCurrentOnly;
            e.VOLT_LOSS_ONLY = this.VoltageLossOnly;
            e.FAULT_CURR_W_VOL_LOSS = this.FaultCurVoltLoss;
            e.VOLT_LOSS_DISP = this.VoltLossDelay;
            if (this.EnabSwitchMode == true)
                e.ENA_SWIT_MOD = "Y";
            else
                e.ENA_SWIT_MOD = "N";
            e.COUT_T_TRIP_VOLT_LOSS = this.CountToTripVolt;
            e.RESET_TIMERR = this.ResetTimerSecMode;
            if (this.SeqCoordMode == true)
                e.SEQ_COORDI_MODE = "Y";
            else
                e.SEQ_COORDI_MODE = "N";

            //DA# 200101 - ME Q1 2020---END
        }

        public void PopulateModelFromEntity(SM_RECLOSER e)
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
            this.ControllerUnitType = e.CONTROL_TYPE;
            this.BypassPlan = e.BYPASS_PLANS;
            this.OperatingMode = e.OPERATING_AS_CD;
            this.GrdMinTrip = e.GRD_MIN_TRIP;
            this.PhaMinTrip = e.PHA_MIN_TRIP;
            this.GrdInstTrip = e.GRD_INST_TRIP_CD;
            this.PhaInstTrip = e.PHA_INST_TRIP_CD;
            this.Tcc1FastCurvesUsed = e.TCC1_FAST_CURVES_USED;
            this.Tcc2SlowCurvesUsed = e.TCC2_SLOW_CURVES_USED;
            this.GrdOpFastCurveOps = e.GRD_OP_F_CRV;
            this.PhaFastCurveOps = e.PHA_OP_F_CRV;
            this.GrdResponseTime = e.GRD_RESP_TIME;
            this.PhaResponseTime = e.PHA_RESP_TIME;
            this.GrdFastCurve = e.GRD_FAST_CRV;
            this.PhaFastCurve = e.PHA_FAST_CRV;
            this.GrdSlowCurve = e.GRD_SLOW_CRV;
            this.PhaSlowCurve = e.PHA_SLOW_CRV;
            this.PhaSlowCurveOps = e.PHA_SLOW_CRV_OPS;
            this.GrdSlowCurveOps = e.GRD_SLOW_CRV_OPS;
            this.GrdTmulFast = e.GRD_TMUL_FAST;
            this.PhaTmulFast = e.PHA_TMUL_FAST;
            //ME Q2
            this.ColdGrdTmulFast = e.COLD_PHA_TMUL_FAST;
            this.ColdPhaTmulFast = e.COLD_PHA_TMUL_FAST;

            this.GrdTmulSlow = e.GRD_TMUL_SLOW;
            this.PhaTmulSlow = e.PHA_TMUL_SLOW;
            this.GrdTaddFast = e.GRD_TADD_FAST;
            this.PhaTaddFast = e.PHA_TADD_FAST;
            this.GrdTaddSlow = e.GRD_TADD_SLOW;
            this.PhaTaddSlow = e.PHA_TADD_SLOW;
            this.TotOpsToLockout = e.TOT_LOCKOUT_OPS;
            this.GrdOpsToLockout = e.GRD_LOCKOUT_OPS;
            this.PhaOpsToLockout = e.PHA_LOCKOUT_OPS;
            this.Reclose1Time = e.RECLOSE1_TIME;
            this.Reclose2Time = e.RECLOSE2_TIME;
            this.Reclose3Time = e.RECLOSE3_TIME;
            this.ResetTime = e.RESET;
            this.RecloseRetryEnabled = e.RECLOSE_RETRY_ENABLED;
            this.SgfCd = e.SGF_CD;
            this.SgfMinTripPercent = e.SGF_MIN_TRIP_PERCENT;
            this.SgfTimeDelay = e.SGF_TIME_DELAY;
            this.SgfLockout = e.SGF_LOCKOUT;
            this.HighCurrentLockoutUsed = e.HIGH_CURRENT_LOCKOUT_USED;
            this.HighCurrentLockoutPhase = e.HIGH_CURRENT_LOCKOUT_PHA;
            this.HighCurrentLockoutGround = e.HIGH_CURRENT_LOCKUOUT_GRD;
            this.ColdLoadPliUsed = e.COLD_LOAD_PLI_USED;
            this.ColdLoadPliPhase = e.COLD_LOAD_PLI_PHA;
            this.ColdLoadPliGround = e.COLD_LOAD_PLI_GRD;
            this.ColdLoadPliCurvePhase = e.COLD_LOAD_PLI_CURVE_PHA;
            this.ColdLoadPliCurveGround = e.COLD_LOAD_PLI_CURVE_GRD;
            this.AltGrdMinTrip = e.ALT_GRD_MIN_TRIP;
            this.AltPhaMinTrip = e.ALT_PHA_MIN_TRIP;
            this.AltGrdInstTrip = e.ALT_GRD_INST_TRIP_CD;
            this.AltPhaInstTrip = e.ALT_PHA_INST_TRIP_CD;
            this.AltGrdFastCurveOps = e.ALT_GRD_OP_F_CRV;
            this.AltPhaFastCurveOps = e.ALT_PHA_OP_F_CRV;
            this.AltTcc1FastCurvesUsed = e.ALT_TCC1_FAST_CURVES_USED;
            this.AltTcc2SlowCurvesUsed = e.ALT_TCC2_SLOW_CURVES_USED;
            this.AltGrdResponseTime = e.ALT_GRD_RESP_TIME;
            this.AltPhaResponseTime = e.ALT_PHA_RESP_TIME;
            this.AltGrdFastCurve = e.ALT_GRD_FAST_CRV;
            this.AltPhaFastCurve = e.ALT_PHA_FAST_CRV;
            this.AltGrdSlowCurve = e.ALT_GRD_SLOW_CRV;
            this.AltPhaSlowCurve = e.ALT_PHA_SLOW_CRV;
            this.AltGrdVmulFast = e.ALT_GRD_VMUL_FAST;
            this.AltPhaSlowCurveOps = e.ALT_PHA_SLOW_CRV_OPS;
            this.AltGrdSlowCurveOps = e.ALT_GRD_SLOW_CRV_OPS;
            this.AltPhaVmulFast = e.ALT_PHA_VMUL_FAST;
            this.AltGrdVmulSlow = e.ALT_GRD_VMUL_SLOW;
            this.AltPhaVmulSlow = e.ALT_PHA_VMUL_SLOW;
            this.AltGrdTaddFast = e.ALT_GRD_TADD_FAST;
            this.AltPhaTaddFast = e.ALT_PHA_TADD_FAST;
            this.AltGrdTaddSlow = e.ALT_GRD_TADD_SLOW;
            this.AltPhaTaddSlow = e.ALT_PHA_TADD_SLOW;
            this.AltTotOpsToLockout = e.ALT_TOT_LOCKOUT_OPS;
            this.AltReclose1Time = e.ALT_RECLOSE1_TIME;
            this.AltReclose2Time = e.ALT_RECLOSE2_TIME;
            this.AltReclose3Time = e.ALT_RECLOSE3_TIME;
            this.AltResetTime = e.ALT_RESET;
            this.AltRecloseRetryEnabled = e.ALT_RECLOSE_RETRY_ENABLED;
            this.AltSfg = e.ALT_SGF_CD;
            this.AltSgfMinTripPercent = e.ALT_SGF_MIN_TRIP_PERCENT;
            this.AltSgfTimeDelay = e.ALT_SGF_TIME_DELAY;
            this.AltSgfLockout = e.ALT_SGF_LOCKOUT;
            this.AltHighCurrentLockoutUsed = e.ALT_HIGH_CURRENT_LOCKOUT_USED;
            this.AltHighCurrentLockoutPhase = e.ALT_HIGH_CURRENT_LOCKOUT_PHA;
            this.AltHighCurrentLockoutGround = e.ALT_HIGH_CURRENT_LOCKUOUT_GRD;
            this.AltColdLoadPliUsed = e.ALT_COLD_LOAD_PLI_USED;
            this.AltColdLoadPliPhase = e.ALT_COLD_LOAD_PLI_PHA;
            this.AltColdLoadPliGround = e.ALT_COLD_LOAD_PLI_GRD;
            this.AltColdLoadPliCurvePha = e.ALT_COLD_LOAD_PLI_CURVE_PHA;
            this.AltColdLoadPliCurveGround = e.ALT_COLD_LOAD_PLI_CURVE_GRD;
            this.Alt2PermitLsEnabling = e.ALT2_PERMIT_LS_ENABLING;
            this.Alt2GrdArmingThreshold = e.ALT2_GRD_ARMING_THRESHOLD;
            this.Alt2PhaArmingThreshold = e.ALT2_PHA_ARMING_THRESHOLD;
            this.Alt2GrdInrushThreshold = e.ALT2_GRD_INRUSH_THRESHOLD;
            this.Alt2PhaInrushThreshold = e.ALT2_PHA_INRUSH_THRESHOLD;
            this.Alt2InrushDuration = e.ALT2_INRUSH_DURATION;
            this.Alt2LsOpsToLockout = e.ALT2_LS_LOCKOUT_OPS;
            this.Alt2LsResetTime = e.ALT2_LS_RESET_TIME;
            this.Alt2GrdMinTrip = e.ALT2_GRD_MIN_TRIP;
            this.Alt2PhaMinTrip = e.ALT2_PHA_MIN_TRIP;
            this.Alt2GrdInstTrip = e.ALT2_GRD_INST_TRIP_CD;
            this.Alt2PhaInstTrip = e.ALT2_PHA_INST_TRIP_CD;
            this.Alt2GrdCurve = e.ALT2_GRD_FAST_CRV;
            this.Alt2PhaCurve = e.ALT2_PHA_FAST_CRV;
            this.Alt2PhaVmul = e.ALT2_PHA_VMUL_FAST;
            this.Alt2GrdVmul = e.ALT2_GRD_VMUL_SLOW;
            this.Alt2GrdTadd = e.ALT2_GRD_TADD_FAST;
            this.Alt2PhaTadd = e.ALT2_PHA_TADD_FAST;
            this.Alt3GrdMinTrip = e.ALT3_GRD_MIN_TRIP;
            this.Alt3PhaMinTrip = e.ALT3_PHA_MIN_TRIP;
            this.Alt3GrdInstTrip = e.ALT3_GRD_INST_TRIP_CD;
            this.Alt3PhaInstTrip = e.ALT3_PHA_INST_TRIP_CD;
            this.Alt3GrdFastCurveOps = e.ALT3_GRD_OP_F_CRV;
            this.Alt3PhaFastCurveOps = e.ALT3_PHA_OP_F_CRV;
            this.Alt3Tcc1FastCurvesUsed = e.ALT3_TCC1_FAST_CURVES_USED;
            this.Alt3Tcc2SlowCurvesUsed = e.ALT3_TCC2_SLOW_CURVES_USED;
            this.Alt3GrdFastCurve = e.ALT3_GRD_FAST_CRV;
            this.Alt3PhaFastCurve = e.ALT3_PHA_FAST_CRV;
            this.Alt3GrdSlowCurve = e.ALT3_GRD_SLOW_CRV;
            this.Alt3PhaSlowCurve = e.ALT3_PHA_SLOW_CRV;
            this.Alt3GrdVmulFast = e.ALT3_GRD_VMUL_FAST;
            this.Alt3PhaSlowCurveOps = e.ALT3_PHA_SLOW_CRV_OPS;
            this.Alt3GrdSlowCurveOps = e.ALT3_GRD_SLOW_CRV_OPS;
            this.Alt3PhaVmulFast = e.ALT3_PHA_VMUL_FAST;
            this.Alt3GrdVmulSlow = e.ALT3_GRD_VMUL_SLOW;
            this.Alt3PhaVmulSlow = e.ALT3_PHA_VMUL_SLOW;
            this.Alt3GrdTaddFast = e.ALT3_GRD_TADD_FAST;
            this.Alt3PhaTaddFast = e.ALT3_PHA_TADD_FAST;
            this.Alt3GrdTaddSlow = e.ALT3_GRD_TADD_SLOW;
            this.Alt3PhaTaddSlow = e.ALT3_PHA_TADD_SLOW;
            this.Alt3PhaDelay = e.ALT3_PHA_DELAY;
            this.Alt3GrdDelay = e.ALT3_GRD_DELAY;
            this.Alt3TotOpsToLockout = e.ALT3_TOT_LOCKOUT_OPS;
            this.Alt3Reclose1Time = e.ALT3_RECLOSE1_TIME;
            this.Alt3Reclose2Time = e.ALT3_RECLOSE2_TIME;
            this.Alt3Reclose3Time = e.ALT3_RECLOSE3_TIME;
            this.Alt3ResetTime = e.ALT3_RESET;
            this.Alt3RecloseRetryEnabled = e.ALT3_RECLOSE_RETRY_ENABLED;
            this.Alt3SensitiveGroundFault = e.ALT3_SGF_CD;
            this.Alt3SgfMinTripPercent = e.ALT3_SGF_MIN_TRIP_PERCENT;
            this.Alt3SgfTimeDelay = e.ALT3_SGF_TIME_DELAY;
            this.Alt3SgfLockout = e.ALT3_SGF_LOCKOUT;
            this.Alt3HighCurrentLockoutUsed = e.ALT3_HIGH_CURRENT_LOCKOUT_USED;
            this.Alt3HighCurrentLockoutPhase = e.ALT3_HIGH_CURRENT_LOCKOUT_PHA;
            this.Alt3HighCurrentLockoutGround = e.ALT3_HIGH_CURRENT_LOCKUOUT_GRD;
            this.Alt3ColdLoadPliUsed = e.ALT3_COLD_LOAD_PLI_USED;
            this.Alt3ColdLoadPliPhase = e.ALT3_COLD_LOAD_PLI_PHA;
            this.Alt3ColdLoadPliGround = e.ALT3_COLD_LOAD_PLI_GRD;
            this.Alt3ColdLoadPliCurvePhase = e.ALT3_COLD_LOAD_PLI_CURVE_PHA;
            this.Alt3ColdLoadPliCurveGround = e.ALT3_COLD_LOAD_PLI_CURVE_GRD;
            this.ActiveProfile = e.ACTIVE_PROFILE;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
            this.FlisrDevice = e.FLISR;
            this.SummerLoadLimit = e.Summer_Load_Limit;
            this.WinterLoadLimit = e.Winter_Load_Limit;
            this.LimitingFactor = e.Limiting_Factor;
            this.FlisrEngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
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
            this.PermitRbCutIn = e.PERMIT_RB_CUTIN;
            this.DirectTransferTrip = e.DIRECT_TRANSFER_TRIP;
            this.BocVoltage = e.BOC_VOLTAGE;
            this.RbCutoutTime = e.RB_CUTOUT_TIME;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
            this.IsMultiFunctional = e.MULTI_FUNCTIONAL;

            //INC000004115018 start
            this.AltPhaOpsToLockout = e.ALT_PHA_LOCKOUT_OPS;
            this.AltGrdOpsToLockout = e.ALT_GRD_LOCKOUT_OPS;
            //INC000004115018 end

            //DA# 200101 - ME Q1 2020---START
            this.PhaDefiniteTime = e.DEF_TIME_PHA;
            this.GrdDefiniteTime = e.DEF_TIME_GROUND;
            this.PhaMinResponseTAddr = e.MIN_RESP_TADDR_PHA;
            this.GrdMinResponseTAddr = e.MIN_RESP_TADDR_GRO;
            if (e.HLT_ENAB == "Y")
                this.HLTEnabled = true;
            else
                this.HLTEnabled = false;
            this.Phase = e.PHASE;
            this.Ground = e.GROUND;
            this.PhaOpsToLock = e.OPT_LOCK_PHA;
            this.GrdOpsToLock = e.OPT_LOCK_GRO;
            this.PhaFirstRec = e.FIRST_RECLO_PHA;
            this.GrdFirstRec = e.FIRST_RECLO_GRO;
            this.PhaSecondRec = e.SEC_RECLO_PHA;
            this.GrdSecondRec = e.SEC_RECLO_GRO;
            this.PhaThirdRec = e.THIRD_RECLO_PHA;
            this.GrdThirdRec = e.THIRD_RECLO_GRO;
            this.ResetTimeFrmLock = e.RESET_TIME_LOCK;
            this.PhaMTTHighCurLock = e.MTT_PHA;
            this.GrdMTTHighCurLock = e.MTT_GROU;
            this.PhaTimeDelay = e.TIME_DEL_PHA;
            this.GrdTimeDelay = e.TIME_DEL_GROU;
            this.PhaMTTPU = e.PU_MTT_PHA;
            this.GrdMTTPU = e.PU_MTT_GROU;
            this.PhaCurvePU = e.PU_CURVE_PHA;
            this.GrdCurvePU = e.PU_CURVE_GROU;
            this.PhaTAddr = e.TADDR_PHA;
            this.GrdTAddr = e.TADDR_GROU;
            this.PhaMinRespTAddrTime = e.MIN_RESP_TADDR_T_D_PHA;
            this.GrdMinRespTAddrTime = e.MIN_RESP_TADDR_T_D_GR;
            this.TimeToActivate = e.TIME_T_ACTI;
            this.FaultCurrentOnly = e.FAULT_CURR_ONLY;
            this.VoltageLossOnly = e.VOLT_LOSS_ONLY;
            this.FaultCurVoltLoss = e.FAULT_CURR_W_VOL_LOSS;
            this.VoltLossDelay = e.VOLT_LOSS_DISP;
            if (e.ENA_SWIT_MOD == "Y")
                this.EnabSwitchMode = true;
            else
                this.EnabSwitchMode = false;
            this.Phase = e.PHASE;
            this.CountToTripVolt = e.COUT_T_TRIP_VOLT_LOSS;
            this.ResetTimerSecMode = e.RESET_TIMERR;
            if (e.SEQ_COORDI_MODE == "Y")
                this.SeqCoordMode = true;
            else
                this.SeqCoordMode = false;
            //DA# 200101 - ME Q1 2020---END
        }

        public void PopulateHistoryFromEntity(SM_RECLOSER_HIST entityHistory, SM_RECLOSER e)
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
            entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
            entityHistory.CONTROL_TYPE = e.CONTROL_TYPE;
            entityHistory.BYPASS_PLANS = e.BYPASS_PLANS;
            entityHistory.OPERATING_AS_CD = e.OPERATING_AS_CD;
            entityHistory.GRD_MIN_TRIP = e.GRD_MIN_TRIP;
            entityHistory.PHA_MIN_TRIP = e.PHA_MIN_TRIP;
            entityHistory.GRD_INST_TRIP_CD = e.GRD_INST_TRIP_CD;
            entityHistory.PHA_INST_TRIP_CD = e.PHA_INST_TRIP_CD;
            entityHistory.TCC1_FAST_CURVES_USED = e.TCC1_FAST_CURVES_USED;
            entityHistory.TCC2_SLOW_CURVES_USED = e.TCC2_SLOW_CURVES_USED;
            entityHistory.GRD_OP_F_CRV = e.GRD_OP_F_CRV;
            entityHistory.PHA_OP_F_CRV = e.PHA_OP_F_CRV;
            entityHistory.GRD_RESP_TIME = e.GRD_RESP_TIME;
            entityHistory.PHA_RESP_TIME = e.PHA_RESP_TIME;
            entityHistory.GRD_FAST_CRV = e.GRD_FAST_CRV;
            entityHistory.PHA_FAST_CRV = e.PHA_FAST_CRV;
            entityHistory.GRD_SLOW_CRV = e.GRD_SLOW_CRV;
            entityHistory.PHA_SLOW_CRV = e.PHA_SLOW_CRV;
            entityHistory.PHA_SLOW_CRV_OPS = e.PHA_SLOW_CRV_OPS;
            entityHistory.GRD_SLOW_CRV_OPS = e.GRD_SLOW_CRV_OPS;
            entityHistory.GRD_TMUL_FAST = e.GRD_TMUL_FAST;
            entityHistory.PHA_TMUL_FAST = e.PHA_TMUL_FAST;
            //ME Q2
            entityHistory.COLD_GRD_TMUL_FAST = e.COLD_GRD_TMUL_FAST;
            entityHistory.COLD_PHA_TMUL_FAST = e.COLD_PHA_TMUL_FAST;

            entityHistory.GRD_TMUL_SLOW = e.GRD_TMUL_SLOW;
            entityHistory.PHA_TMUL_SLOW = e.PHA_TMUL_SLOW;
            entityHistory.GRD_TADD_FAST = e.GRD_TADD_FAST;
            entityHistory.PHA_TADD_FAST = e.PHA_TADD_FAST;
            entityHistory.GRD_TADD_SLOW = e.GRD_TADD_SLOW;
            entityHistory.PHA_TADD_SLOW = e.PHA_TADD_SLOW;
            entityHistory.TOT_LOCKOUT_OPS = e.TOT_LOCKOUT_OPS;
            entityHistory.PHA_LOCKOUT_OPS = e.PHA_LOCKOUT_OPS;
            entityHistory.GRD_LOCKOUT_OPS = e.GRD_LOCKOUT_OPS;
            entityHistory.RECLOSE1_TIME = e.RECLOSE1_TIME;
            entityHistory.RECLOSE2_TIME = e.RECLOSE2_TIME;
            entityHistory.RECLOSE3_TIME = e.RECLOSE3_TIME;
            entityHistory.RESET = e.RESET;
            entityHistory.RECLOSE_RETRY_ENABLED = e.RECLOSE_RETRY_ENABLED;
            entityHistory.SGF_CD = e.SGF_CD;
            entityHistory.SGF_MIN_TRIP_PERCENT = e.SGF_MIN_TRIP_PERCENT;
            entityHistory.SGF_TIME_DELAY = e.SGF_TIME_DELAY;
            entityHistory.SGF_LOCKOUT = e.SGF_LOCKOUT;
            entityHistory.HIGH_CURRENT_LOCKOUT_USED = e.HIGH_CURRENT_LOCKOUT_USED;
            entityHistory.HIGH_CURRENT_LOCKOUT_PHA = e.HIGH_CURRENT_LOCKOUT_PHA;
            entityHistory.HIGH_CURRENT_LOCKUOUT_GRD = e.HIGH_CURRENT_LOCKUOUT_GRD;
            entityHistory.COLD_LOAD_PLI_USED = e.COLD_LOAD_PLI_USED;
            entityHistory.COLD_LOAD_PLI_PHA = e.COLD_LOAD_PLI_PHA;
            entityHistory.COLD_LOAD_PLI_GRD = e.COLD_LOAD_PLI_GRD;
            entityHistory.COLD_LOAD_PLI_CURVE_PHA = e.COLD_LOAD_PLI_CURVE_PHA;
            entityHistory.COLD_LOAD_PLI_CURVE_GRD = e.COLD_LOAD_PLI_CURVE_GRD;
            entityHistory.ALT_GRD_MIN_TRIP = e.ALT_GRD_MIN_TRIP;
            entityHistory.ALT_PHA_MIN_TRIP = e.ALT_PHA_MIN_TRIP;
            entityHistory.ALT_GRD_INST_TRIP_CD = e.ALT_GRD_INST_TRIP_CD;
            entityHistory.ALT_PHA_INST_TRIP_CD = e.ALT_PHA_INST_TRIP_CD;
            entityHistory.ALT_GRD_OP_F_CRV = e.ALT_GRD_OP_F_CRV;
            entityHistory.ALT_PHA_OP_F_CRV = e.ALT_PHA_OP_F_CRV;
            entityHistory.ALT_TCC1_FAST_CURVES_USED = e.ALT_TCC1_FAST_CURVES_USED;
            entityHistory.ALT_TCC2_SLOW_CURVES_USED = e.ALT_TCC2_SLOW_CURVES_USED;
            entityHistory.ALT_GRD_RESP_TIME = e.ALT_GRD_RESP_TIME;
            entityHistory.ALT_PHA_RESP_TIME = e.ALT_PHA_RESP_TIME;
            entityHistory.ALT_GRD_FAST_CRV = e.ALT_GRD_FAST_CRV;
            entityHistory.ALT_PHA_FAST_CRV = e.ALT_PHA_FAST_CRV;
            entityHistory.ALT_GRD_SLOW_CRV = e.ALT_GRD_SLOW_CRV;
            entityHistory.ALT_PHA_SLOW_CRV = e.ALT_PHA_SLOW_CRV;
            entityHistory.ALT_GRD_VMUL_FAST = e.ALT_GRD_VMUL_FAST;
            entityHistory.ALT_PHA_SLOW_CRV_OPS = e.ALT_PHA_SLOW_CRV_OPS;
            entityHistory.ALT_GRD_SLOW_CRV_OPS = e.ALT_GRD_SLOW_CRV_OPS;
            entityHistory.ALT_PHA_VMUL_FAST = e.ALT_PHA_VMUL_FAST;
            entityHistory.ALT_GRD_VMUL_SLOW = e.ALT_GRD_VMUL_SLOW;
            entityHistory.ALT_PHA_VMUL_SLOW = e.ALT_PHA_VMUL_SLOW;
            entityHistory.ALT_GRD_TADD_FAST = e.ALT_GRD_TADD_FAST;
            entityHistory.ALT_PHA_TADD_FAST = e.ALT_PHA_TADD_FAST;
            entityHistory.ALT_GRD_TADD_SLOW = e.ALT_GRD_TADD_SLOW;
            entityHistory.ALT_PHA_TADD_SLOW = e.ALT_PHA_TADD_SLOW;
            entityHistory.ALT_TOT_LOCKOUT_OPS = e.ALT_TOT_LOCKOUT_OPS;
            entityHistory.ALT_RECLOSE1_TIME = e.ALT_RECLOSE1_TIME;
            entityHistory.ALT_RECLOSE2_TIME = e.ALT_RECLOSE2_TIME;
            entityHistory.ALT_RECLOSE3_TIME = e.ALT_RECLOSE3_TIME;
            entityHistory.ALT_RESET = e.ALT_RESET;
            entityHistory.ALT_RECLOSE_RETRY_ENABLED = e.ALT_RECLOSE_RETRY_ENABLED;
            entityHistory.ALT_SGF_CD = e.ALT_SGF_CD;
            entityHistory.ALT_SGF_MIN_TRIP_PERCENT = e.ALT_SGF_MIN_TRIP_PERCENT;
            entityHistory.ALT_SGF_TIME_DELAY = e.ALT_SGF_TIME_DELAY;
            entityHistory.ALT_SGF_LOCKOUT = e.ALT_SGF_LOCKOUT;
            entityHistory.ALT_HIGH_CURRENT_LOCKOUT_USED = e.ALT_HIGH_CURRENT_LOCKOUT_USED;
            entityHistory.ALT_HIGH_CURRENT_LOCKOUT_PHA = e.ALT_HIGH_CURRENT_LOCKOUT_PHA;
            entityHistory.ALT_HIGH_CURRENT_LOCKUOUT_GRD = e.ALT_HIGH_CURRENT_LOCKUOUT_GRD;
            entityHistory.ALT_COLD_LOAD_PLI_USED = e.ALT_COLD_LOAD_PLI_USED;
            entityHistory.ALT_COLD_LOAD_PLI_PHA = e.ALT_COLD_LOAD_PLI_PHA;
            entityHistory.ALT_COLD_LOAD_PLI_GRD = e.ALT_COLD_LOAD_PLI_GRD;
            entityHistory.ALT_COLD_LOAD_PLI_CURVE_PHA = e.ALT_COLD_LOAD_PLI_CURVE_PHA;
            entityHistory.ALT_COLD_LOAD_PLI_CURVE_GRD = e.ALT_COLD_LOAD_PLI_CURVE_GRD;
            entityHistory.ALT2_PERMIT_LS_ENABLING = e.ALT2_PERMIT_LS_ENABLING;
            entityHistory.ALT2_GRD_ARMING_THRESHOLD = e.ALT2_GRD_ARMING_THRESHOLD;
            entityHistory.ALT2_PHA_ARMING_THRESHOLD = e.ALT2_PHA_ARMING_THRESHOLD;
            entityHistory.ALT2_GRD_INRUSH_THRESHOLD = e.ALT2_GRD_INRUSH_THRESHOLD;
            entityHistory.ALT2_PHA_INRUSH_THRESHOLD = e.ALT2_PHA_INRUSH_THRESHOLD;
            entityHistory.ALT2_INRUSH_DURATION = e.ALT2_INRUSH_DURATION;
            entityHistory.ALT2_LS_LOCKOUT_OPS = e.ALT2_LS_LOCKOUT_OPS;
            entityHistory.ALT2_LS_RESET_TIME = e.ALT2_LS_RESET_TIME;
            entityHistory.ALT2_GRD_MIN_TRIP = e.ALT2_GRD_MIN_TRIP;
            entityHistory.ALT2_PHA_MIN_TRIP = e.ALT2_PHA_MIN_TRIP;
            entityHistory.ALT2_GRD_INST_TRIP_CD = e.ALT2_GRD_INST_TRIP_CD;
            entityHistory.ALT2_PHA_INST_TRIP_CD = e.ALT2_PHA_INST_TRIP_CD;
            entityHistory.ALT2_GRD_FAST_CRV = e.ALT2_GRD_FAST_CRV;
            entityHistory.ALT2_PHA_FAST_CRV = e.ALT2_PHA_FAST_CRV;
            entityHistory.ALT2_PHA_VMUL_FAST = e.ALT2_PHA_VMUL_FAST;
            entityHistory.ALT2_GRD_VMUL_SLOW = e.ALT2_GRD_VMUL_SLOW;
            entityHistory.ALT2_GRD_TADD_FAST = e.ALT2_GRD_TADD_FAST;
            entityHistory.ALT2_PHA_TADD_FAST = e.ALT2_PHA_TADD_FAST;
            entityHistory.ALT3_GRD_MIN_TRIP = e.ALT3_GRD_MIN_TRIP;
            entityHistory.ALT3_PHA_MIN_TRIP = e.ALT3_PHA_MIN_TRIP;
            entityHistory.ALT3_GRD_INST_TRIP_CD = e.ALT3_GRD_INST_TRIP_CD;
            entityHistory.ALT3_PHA_INST_TRIP_CD = e.ALT3_PHA_INST_TRIP_CD;
            entityHistory.ALT3_GRD_OP_F_CRV = e.ALT3_GRD_OP_F_CRV;
            entityHistory.ALT3_PHA_OP_F_CRV = e.ALT3_PHA_OP_F_CRV;
            entityHistory.ALT3_TCC1_FAST_CURVES_USED = e.ALT3_TCC1_FAST_CURVES_USED;
            entityHistory.ALT3_TCC2_SLOW_CURVES_USED = e.ALT3_TCC2_SLOW_CURVES_USED;
            entityHistory.ALT3_GRD_FAST_CRV = e.ALT3_GRD_FAST_CRV;
            entityHistory.ALT3_PHA_FAST_CRV = e.ALT3_PHA_FAST_CRV;
            entityHistory.ALT3_GRD_SLOW_CRV = e.ALT3_GRD_SLOW_CRV;
            entityHistory.ALT3_PHA_SLOW_CRV = e.ALT3_PHA_SLOW_CRV;
            entityHistory.ALT3_GRD_VMUL_FAST = e.ALT3_GRD_VMUL_FAST;
            entityHistory.ALT3_PHA_SLOW_CRV_OPS = e.ALT3_PHA_SLOW_CRV_OPS;
            entityHistory.ALT3_GRD_SLOW_CRV_OPS = e.ALT3_GRD_SLOW_CRV_OPS;
            entityHistory.ALT3_PHA_VMUL_FAST = e.ALT3_PHA_VMUL_FAST;
            entityHistory.ALT3_GRD_VMUL_SLOW = e.ALT3_GRD_VMUL_SLOW;
            entityHistory.ALT3_PHA_VMUL_SLOW = e.ALT3_PHA_VMUL_SLOW;
            entityHistory.ALT3_GRD_TADD_FAST = e.ALT3_GRD_TADD_FAST;
            entityHistory.ALT3_PHA_TADD_FAST = e.ALT3_PHA_TADD_FAST;
            entityHistory.ALT3_GRD_TADD_SLOW = e.ALT3_GRD_TADD_SLOW;
            entityHistory.ALT3_PHA_TADD_SLOW = e.ALT3_PHA_TADD_SLOW;
            entityHistory.ALT3_PHA_DELAY = e.ALT3_PHA_DELAY;
            entityHistory.ALT3_GRD_DELAY = e.ALT3_GRD_DELAY;
            entityHistory.ALT3_TOT_LOCKOUT_OPS = e.ALT3_TOT_LOCKOUT_OPS;
            entityHistory.ALT3_RECLOSE1_TIME = e.ALT3_RECLOSE1_TIME;
            entityHistory.ALT3_RECLOSE2_TIME = e.ALT3_RECLOSE2_TIME;
            entityHistory.ALT3_RECLOSE3_TIME = e.ALT3_RECLOSE3_TIME;
            entityHistory.ALT3_RESET = e.ALT3_RESET;
            entityHistory.ALT3_RECLOSE_RETRY_ENABLED = e.ALT3_RECLOSE_RETRY_ENABLED;
            entityHistory.ALT3_SGF_CD = e.ALT3_SGF_CD;
            entityHistory.ALT3_SGF_MIN_TRIP_PERCENT = e.ALT3_SGF_MIN_TRIP_PERCENT;
            entityHistory.ALT3_SGF_TIME_DELAY = e.ALT3_SGF_TIME_DELAY;
            entityHistory.ALT3_SGF_LOCKOUT = e.ALT3_SGF_LOCKOUT;
            entityHistory.ALT3_HIGH_CURRENT_LOCKOUT_USED = e.ALT3_HIGH_CURRENT_LOCKOUT_USED;
            entityHistory.ALT3_HIGH_CURRENT_LOCKOUT_PHA = e.ALT3_HIGH_CURRENT_LOCKOUT_PHA;
            entityHistory.ALT3_HIGH_CURRENT_LOCKUOUT_GRD = e.ALT3_HIGH_CURRENT_LOCKUOUT_GRD;
            entityHistory.ALT3_COLD_LOAD_PLI_USED = e.ALT3_COLD_LOAD_PLI_USED;
            entityHistory.ALT3_COLD_LOAD_PLI_PHA = e.ALT3_COLD_LOAD_PLI_PHA;
            entityHistory.ALT3_COLD_LOAD_PLI_GRD = e.ALT3_COLD_LOAD_PLI_GRD;
            entityHistory.ALT3_COLD_LOAD_PLI_CURVE_PHA = e.ALT3_COLD_LOAD_PLI_CURVE_PHA;
            entityHistory.ALT3_COLD_LOAD_PLI_CURVE_GRD = e.ALT3_COLD_LOAD_PLI_CURVE_GRD;
            entityHistory.ACTIVE_PROFILE = e.ACTIVE_PROFILE;
            entityHistory.ENGINEERING_COMMENTS = e.ENGINEERING_COMMENTS;
            entityHistory.FLISR = e.FLISR;
            entityHistory.Summer_Load_Limit = e.Summer_Load_Limit;
            entityHistory.Winter_Load_Limit = e.Winter_Load_Limit;
            entityHistory.Limiting_Factor = e.Limiting_Factor;
            entityHistory.FLISR_ENGINEERING_COMMENTS = e.FLISR_ENGINEERING_COMMENTS;
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
            entityHistory.FLISR_ENGINEERING_COMMENTS = e.FLISR_ENGINEERING_COMMENTS;
            entityHistory.FLISR_OPERATING_MODE = e.FLISR_OPERATING_MODE;
            entityHistory.PERMIT_RB_CUTIN = e.PERMIT_RB_CUTIN;
            entityHistory.DIRECT_TRANSFER_TRIP = e.DIRECT_TRANSFER_TRIP;
            entityHistory.BOC_VOLTAGE = e.BOC_VOLTAGE;
            entityHistory.RB_CUTOUT_TIME = e.RB_CUTOUT_TIME;
            entityHistory.RTU_EXIST = e.RTU_EXIST;
            entityHistory.RTU_MANF_CD = e.RTU_MANF_CD;
            entityHistory.RTU_FIRMWARE_VERSION = e.RTU_FIRMWARE_VERSION;
            entityHistory.RTU_MODEL_NUM = e.RTU_MODEL_NUM;
            entityHistory.RTU_SERIAL_NUM = e.RTU_SERIAL_NUM;
            entityHistory.RTU_SOFTWARE_VERSION = e.RTU_SOFTWARE_VERSION;
            entityHistory.ENGINEERING_COMMENTS = e.ENGINEERING_COMMENTS;
            entityHistory.MULTI_FUNCTIONAL = e.MULTI_FUNCTIONAL;

            //INC000004115018 start
            entityHistory.ALT_PHA_LOCKOUT_OPS = e.ALT_PHA_LOCKOUT_OPS;
            entityHistory.ALT_GRD_LOCKOUT_OPS = e.ALT_GRD_LOCKOUT_OPS;
            //INC000004115018 end

            //DA# 200101 - ME Q1 2020---START
            entityHistory.DEF_TIME_PHA = e.DEF_TIME_PHA;
            entityHistory.DEF_TIME_GROUND = e.DEF_TIME_GROUND;
            entityHistory.MIN_RESP_TADDR_PHA = e.MIN_RESP_TADDR_PHA;
            entityHistory.MIN_RESP_TADDR_GRO = e.MIN_RESP_TADDR_GRO;
            entityHistory.HLT_ENAB = e.HLT_ENAB;
            entityHistory.PHASE = e.PHASE;
            entityHistory.GROUND = e.GROUND;
            entityHistory.OPT_LOCK_PHA = e.OPT_LOCK_PHA;
            entityHistory.OPT_LOCK_GRO = e.OPT_LOCK_GRO;
            entityHistory.FIRST_RECLO_PHA = e.FIRST_RECLO_PHA;
            entityHistory.FIRST_RECLO_GRO = e.FIRST_RECLO_GRO;
            entityHistory.SEC_RECLO_PHA = e.SEC_RECLO_PHA;
            entityHistory.SEC_RECLO_GRO = e.SEC_RECLO_GRO;
            entityHistory.THIRD_RECLO_PHA = e.THIRD_RECLO_PHA;
            entityHistory.THIRD_RECLO_GRO = e.THIRD_RECLO_GRO;
            entityHistory.RESET_TIME_LOCK = e.RESET_TIME_LOCK;
            entityHistory.MTT_PHA = e.MTT_PHA;
            entityHistory.MTT_GROU = e.MTT_GROU;
            entityHistory.TIME_DEL_PHA = e.TIME_DEL_PHA;
            entityHistory.TIME_DEL_GROU = e.TIME_DEL_GROU;
            entityHistory.PU_MTT_PHA = e.PU_MTT_PHA;
            entityHistory.PU_MTT_GROU = e.PU_MTT_GROU;
            entityHistory.PU_CURVE_PHA = e.PU_CURVE_PHA;
            entityHistory.PU_CURVE_GROU = e.PU_CURVE_GROU;
            entityHistory.TADDR_PHA = e.TADDR_PHA;
            entityHistory.TADDR_GROU = e.TADDR_GROU;
            entityHistory.MIN_RESP_TADDR_T_D_PHA = e.MIN_RESP_TADDR_T_D_PHA;
            entityHistory.MIN_RESP_TADDR_T_D_GR = e.MIN_RESP_TADDR_T_D_GR;
            entityHistory.TIME_T_ACTI = e.TIME_T_ACTI;
            entityHistory.FAULT_CURR_ONLY = e.FAULT_CURR_ONLY;
            entityHistory.VOLT_LOSS_ONLY = e.VOLT_LOSS_ONLY;
            entityHistory.FAULT_CURR_W_VOL_LOSS = e.FAULT_CURR_W_VOL_LOSS;
            entityHistory.VOLT_LOSS_DISP = e.VOLT_LOSS_DISP;
            entityHistory.ENA_SWIT_MOD = e.ENA_SWIT_MOD;
            entityHistory.COUT_T_TRIP_VOLT_LOSS = e.COUT_T_TRIP_VOLT_LOSS;
            entityHistory.RESET_TIMERR = e.RESET_TIMERR;
            entityHistory.SEQ_COORDI_MODE = e.SEQ_COORDI_MODE;
            //DA# 200101 - ME Q1 2020---END
        }

        public void PopulateModelFromHistoryEntity(SM_RECLOSER_HIST e)
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
            this.ControllerUnitType = e.CONTROL_TYPE;
            this.BypassPlan = e.BYPASS_PLANS;
            this.OperatingMode = e.OPERATING_AS_CD;
            this.GrdMinTrip = e.GRD_MIN_TRIP;
            this.PhaMinTrip = e.PHA_MIN_TRIP;
            this.GrdInstTrip = e.GRD_INST_TRIP_CD;
            this.PhaInstTrip = e.PHA_INST_TRIP_CD;
            this.Tcc1FastCurvesUsed = e.TCC1_FAST_CURVES_USED;
            this.Tcc2SlowCurvesUsed = e.TCC2_SLOW_CURVES_USED;
            this.GrdOpFastCurveOps = e.GRD_OP_F_CRV;
            this.PhaFastCurveOps = e.PHA_OP_F_CRV;
            this.GrdResponseTime = e.GRD_RESP_TIME;
            this.PhaResponseTime = e.PHA_RESP_TIME;
            this.GrdFastCurve = e.GRD_FAST_CRV;
            this.PhaFastCurve = e.PHA_FAST_CRV;
            this.GrdSlowCurve = e.GRD_SLOW_CRV;
            this.PhaSlowCurve = e.PHA_SLOW_CRV;
            this.PhaSlowCurveOps = e.PHA_SLOW_CRV_OPS;
            this.GrdSlowCurveOps = e.GRD_SLOW_CRV_OPS;
            this.GrdTmulFast = e.GRD_TMUL_FAST;
            this.PhaTmulFast = e.PHA_TMUL_FAST;
            //ME Q2
            this.ColdGrdTmulFast = e.COLD_GRD_TMUL_FAST;
            this.ColdPhaTmulFast = e.COLD_PHA_TMUL_FAST;

            this.GrdTmulSlow = e.GRD_TMUL_SLOW;
            this.PhaTmulSlow = e.PHA_TMUL_SLOW;
            this.GrdTaddFast = e.GRD_TADD_FAST;
            this.PhaTaddFast = e.PHA_TADD_FAST;
            this.GrdTaddSlow = e.GRD_TADD_SLOW;
            this.PhaTaddSlow = e.PHA_TADD_SLOW;
            this.TotOpsToLockout = e.TOT_LOCKOUT_OPS;
            this.GrdOpsToLockout = e.GRD_LOCKOUT_OPS;
            this.PhaOpsToLockout = e.PHA_LOCKOUT_OPS;
            this.Reclose1Time = e.RECLOSE1_TIME;
            this.Reclose2Time = e.RECLOSE2_TIME;
            this.Reclose3Time = e.RECLOSE3_TIME;
            this.ResetTime = e.RESET;
            this.RecloseRetryEnabled = e.RECLOSE_RETRY_ENABLED;
            this.SgfCd = e.SGF_CD;
            this.SgfMinTripPercent = e.SGF_MIN_TRIP_PERCENT;
            this.SgfTimeDelay = e.SGF_TIME_DELAY;
            this.SgfLockout = e.SGF_LOCKOUT;
            this.HighCurrentLockoutUsed = e.HIGH_CURRENT_LOCKOUT_USED;
            this.HighCurrentLockoutPhase = e.HIGH_CURRENT_LOCKOUT_PHA;
            this.HighCurrentLockoutGround = e.HIGH_CURRENT_LOCKUOUT_GRD;
            this.ColdLoadPliUsed = e.COLD_LOAD_PLI_USED;
            this.ColdLoadPliPhase = e.COLD_LOAD_PLI_PHA;
            this.ColdLoadPliGround = e.COLD_LOAD_PLI_GRD;
            this.ColdLoadPliCurvePhase = e.COLD_LOAD_PLI_CURVE_PHA;
            this.ColdLoadPliCurveGround = e.COLD_LOAD_PLI_CURVE_GRD;
            this.AltGrdMinTrip = e.ALT_GRD_MIN_TRIP;
            this.AltPhaMinTrip = e.ALT_PHA_MIN_TRIP;
            this.AltGrdInstTrip = e.ALT_GRD_INST_TRIP_CD;
            this.AltPhaInstTrip = e.ALT_PHA_INST_TRIP_CD;
            this.AltGrdFastCurveOps = e.ALT_GRD_OP_F_CRV;
            this.AltPhaFastCurveOps = e.ALT_PHA_OP_F_CRV;
            this.AltTcc1FastCurvesUsed = e.ALT_TCC1_FAST_CURVES_USED;
            this.AltTcc2SlowCurvesUsed = e.ALT_TCC2_SLOW_CURVES_USED;
            this.AltGrdResponseTime = e.ALT_GRD_RESP_TIME;
            this.AltPhaResponseTime = e.ALT_PHA_RESP_TIME;
            this.AltGrdFastCurve = e.ALT_GRD_FAST_CRV;
            this.AltPhaFastCurve = e.ALT_PHA_FAST_CRV;
            this.AltGrdSlowCurve = e.ALT_GRD_SLOW_CRV;
            this.AltPhaSlowCurve = e.ALT_PHA_SLOW_CRV;
            this.AltGrdVmulFast = e.ALT_GRD_VMUL_FAST;
            this.AltPhaSlowCurveOps = e.ALT_PHA_SLOW_CRV_OPS;
            this.AltGrdSlowCurveOps = e.ALT_GRD_SLOW_CRV_OPS;
            this.AltPhaVmulFast = e.ALT_PHA_VMUL_FAST;
            this.AltGrdVmulSlow = e.ALT_GRD_VMUL_SLOW;
            this.AltPhaVmulSlow = e.ALT_PHA_VMUL_SLOW;
            this.AltGrdTaddFast = e.ALT_GRD_TADD_FAST;
            this.AltPhaTaddFast = e.ALT_PHA_TADD_FAST;
            this.AltGrdTaddSlow = e.ALT_GRD_TADD_SLOW;
            this.AltPhaTaddSlow = e.ALT_PHA_TADD_SLOW;
            this.AltTotOpsToLockout = e.ALT_TOT_LOCKOUT_OPS;
            this.AltReclose1Time = e.ALT_RECLOSE1_TIME;
            this.AltReclose2Time = e.ALT_RECLOSE2_TIME;
            this.AltReclose3Time = e.ALT_RECLOSE3_TIME;
            this.AltResetTime = e.ALT_RESET;
            this.AltRecloseRetryEnabled = e.ALT_RECLOSE_RETRY_ENABLED;
            this.AltSfg = e.ALT_SGF_CD;
            this.AltSgfMinTripPercent = e.ALT_SGF_MIN_TRIP_PERCENT;
            this.AltSgfTimeDelay = e.ALT_SGF_TIME_DELAY;
            this.AltSgfLockout = e.ALT_SGF_LOCKOUT;
            this.AltHighCurrentLockoutUsed = e.ALT_HIGH_CURRENT_LOCKOUT_USED;
            this.AltHighCurrentLockoutPhase = e.ALT_HIGH_CURRENT_LOCKOUT_PHA;
            this.AltHighCurrentLockoutGround = e.ALT_HIGH_CURRENT_LOCKUOUT_GRD;
            this.AltColdLoadPliUsed = e.ALT_COLD_LOAD_PLI_USED;
            this.AltColdLoadPliPhase = e.ALT_COLD_LOAD_PLI_PHA;
            this.AltColdLoadPliGround = e.ALT_COLD_LOAD_PLI_GRD;
            this.AltColdLoadPliCurvePha = e.ALT_COLD_LOAD_PLI_CURVE_PHA;
            this.AltColdLoadPliCurveGround = e.ALT_COLD_LOAD_PLI_CURVE_GRD;
            this.Alt2PermitLsEnabling = e.ALT2_PERMIT_LS_ENABLING;
            this.Alt2GrdArmingThreshold = e.ALT2_GRD_ARMING_THRESHOLD;
            this.Alt2PhaArmingThreshold = e.ALT2_PHA_ARMING_THRESHOLD;
            this.Alt2GrdInrushThreshold = e.ALT2_GRD_INRUSH_THRESHOLD;
            this.Alt2PhaInrushThreshold = e.ALT2_PHA_INRUSH_THRESHOLD;
            this.Alt2InrushDuration = e.ALT2_INRUSH_DURATION;
            this.Alt2LsOpsToLockout = e.ALT2_LS_LOCKOUT_OPS;
            this.Alt2LsResetTime = e.ALT2_LS_RESET_TIME;
            this.Alt2GrdMinTrip = e.ALT2_GRD_MIN_TRIP;
            this.Alt2PhaMinTrip = e.ALT2_PHA_MIN_TRIP;
            this.Alt2GrdInstTrip = e.ALT2_GRD_INST_TRIP_CD;
            this.Alt2PhaInstTrip = e.ALT2_PHA_INST_TRIP_CD;
            this.Alt2GrdCurve = e.ALT2_GRD_FAST_CRV;
            this.Alt2PhaCurve = e.ALT2_PHA_FAST_CRV;
            this.Alt2PhaVmul = e.ALT2_PHA_VMUL_FAST;
            this.Alt2GrdVmul = e.ALT2_GRD_VMUL_SLOW;
            this.Alt2GrdTadd = e.ALT2_GRD_TADD_FAST;
            this.Alt2PhaTadd = e.ALT2_PHA_TADD_FAST;
            this.Alt3GrdMinTrip = e.ALT3_GRD_MIN_TRIP;
            this.Alt3PhaMinTrip = e.ALT3_PHA_MIN_TRIP;
            this.Alt3GrdInstTrip = e.ALT3_GRD_INST_TRIP_CD;
            this.Alt3PhaInstTrip = e.ALT3_PHA_INST_TRIP_CD;
            this.Alt3GrdFastCurveOps = e.ALT3_GRD_OP_F_CRV;
            this.Alt3PhaFastCurveOps = e.ALT3_PHA_OP_F_CRV;
            this.Alt3Tcc1FastCurvesUsed = e.ALT3_TCC1_FAST_CURVES_USED;
            this.Alt3Tcc2SlowCurvesUsed = e.ALT3_TCC2_SLOW_CURVES_USED;
            this.Alt3GrdFastCurve = e.ALT3_GRD_FAST_CRV;
            this.Alt3PhaFastCurve = e.ALT3_PHA_FAST_CRV;
            this.Alt3GrdSlowCurve = e.ALT3_GRD_SLOW_CRV;
            this.Alt3PhaSlowCurve = e.ALT3_PHA_SLOW_CRV;
            this.Alt3GrdVmulFast = e.ALT3_GRD_VMUL_FAST;
            this.Alt3PhaSlowCurveOps = e.ALT3_PHA_SLOW_CRV_OPS;
            this.Alt3GrdSlowCurveOps = e.ALT3_GRD_SLOW_CRV_OPS;
            this.Alt3PhaVmulFast = e.ALT3_PHA_VMUL_FAST;
            this.Alt3GrdVmulSlow = e.ALT3_GRD_VMUL_SLOW;
            this.Alt3PhaVmulSlow = e.ALT3_PHA_VMUL_SLOW;
            this.Alt3GrdTaddFast = e.ALT3_GRD_TADD_FAST;
            this.Alt3PhaTaddFast = e.ALT3_PHA_TADD_FAST;
            this.Alt3GrdTaddSlow = e.ALT3_GRD_TADD_SLOW;
            this.Alt3PhaTaddSlow = e.ALT3_PHA_TADD_SLOW;
            this.Alt3PhaDelay = e.ALT3_PHA_DELAY;
            this.Alt3GrdDelay = e.ALT3_GRD_DELAY;
            this.Alt3TotOpsToLockout = e.ALT3_TOT_LOCKOUT_OPS;
            this.Alt3Reclose1Time = e.ALT3_RECLOSE1_TIME;
            this.Alt3Reclose2Time = e.ALT3_RECLOSE2_TIME;
            this.Alt3Reclose3Time = e.ALT3_RECLOSE3_TIME;
            this.Alt3ResetTime = e.ALT3_RESET;
            this.Alt3RecloseRetryEnabled = e.ALT3_RECLOSE_RETRY_ENABLED;
            this.Alt3SensitiveGroundFault = e.ALT3_SGF_CD;
            this.Alt3SgfMinTripPercent = e.ALT3_SGF_MIN_TRIP_PERCENT;
            this.Alt3SgfTimeDelay = e.ALT3_SGF_TIME_DELAY;
            this.Alt3SgfLockout = e.ALT3_SGF_LOCKOUT;
            this.Alt3HighCurrentLockoutUsed = e.ALT3_HIGH_CURRENT_LOCKOUT_USED;
            this.Alt3HighCurrentLockoutPhase = e.ALT3_HIGH_CURRENT_LOCKOUT_PHA;
            this.Alt3HighCurrentLockoutGround = e.ALT3_HIGH_CURRENT_LOCKUOUT_GRD;
            this.Alt3ColdLoadPliUsed = e.ALT3_COLD_LOAD_PLI_USED;
            this.Alt3ColdLoadPliPhase = e.ALT3_COLD_LOAD_PLI_PHA;
            this.Alt3ColdLoadPliGround = e.ALT3_COLD_LOAD_PLI_GRD;
            this.Alt3ColdLoadPliCurvePhase = e.ALT3_COLD_LOAD_PLI_CURVE_PHA;
            this.Alt3ColdLoadPliCurveGround = e.ALT3_COLD_LOAD_PLI_CURVE_GRD;
            this.ActiveProfile = e.ACTIVE_PROFILE;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
            this.FlisrDevice = e.FLISR;
            this.SummerLoadLimit = e.Summer_Load_Limit;
            this.WinterLoadLimit = e.Winter_Load_Limit;
            this.LimitingFactor = e.Limiting_Factor;
            this.FlisrEngineeringComments = e.FLISR_ENGINEERING_COMMENTS;
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
            this.PermitRbCutIn = e.PERMIT_RB_CUTIN;
            this.DirectTransferTrip = e.DIRECT_TRANSFER_TRIP;
            this.BocVoltage = e.BOC_VOLTAGE;
            this.RbCutoutTime = e.RB_CUTOUT_TIME;
            this.RTUExists = e.RTU_EXIST;
            this.RTUFirmwareVersion = e.RTU_FIRMWARE_VERSION;
            this.RTUManufacture = e.RTU_MANF_CD;
            this.RTUModelNumber = e.RTU_MODEL_NUM;
            this.RTUSerialNumber = e.RTU_SERIAL_NUM;
            this.RTUSoftwareVersion = e.RTU_SOFTWARE_VERSION;
            this.EngineeringComments = e.ENGINEERING_COMMENTS;
            this.IsMultiFunctional = e.MULTI_FUNCTIONAL;

            //INC000004115018 start
            this.AltPhaOpsToLockout = e.ALT_PHA_LOCKOUT_OPS;
            this.AltGrdOpsToLockout = e.ALT_GRD_LOCKOUT_OPS;
            //INC000004115018 end

            //DA# 200101 - ME Q1 2020---START
            this.PhaDefiniteTime = e.DEF_TIME_PHA;
            this.GrdDefiniteTime = e.DEF_TIME_GROUND;
            this.PhaMinResponseTAddr = e.MIN_RESP_TADDR_PHA;
            this.GrdMinResponseTAddr = e.MIN_RESP_TADDR_GRO;
            if (e.HLT_ENAB == "Y")
                this.HLTEnabled = true;
            else
                this.HLTEnabled = false;
            this.Phase = e.PHASE;
            this.Ground = e.GROUND;
            this.PhaOpsToLock = e.OPT_LOCK_PHA;
            this.GrdOpsToLock = e.OPT_LOCK_GRO;
            this.PhaFirstRec = e.FIRST_RECLO_PHA;
            this.GrdFirstRec = e.FIRST_RECLO_GRO;
            this.PhaSecondRec = e.SEC_RECLO_PHA;
            this.GrdSecondRec = e.SEC_RECLO_GRO;
            this.PhaThirdRec = e.THIRD_RECLO_PHA;
            this.GrdThirdRec = e.THIRD_RECLO_GRO;
            this.ResetTimeFrmLock = e.RESET_TIME_LOCK;
            this.PhaMTTHighCurLock = e.MTT_PHA;
            this.GrdMTTHighCurLock = e.MTT_GROU;
            this.PhaTimeDelay = e.TIME_DEL_PHA;
            this.GrdTimeDelay = e.TIME_DEL_GROU;
            this.PhaMTTPU = e.PU_MTT_PHA;
            this.GrdMTTPU = e.PU_MTT_GROU;
            this.PhaCurvePU = e.PU_CURVE_PHA;
            this.GrdCurvePU = e.PU_CURVE_GROU;
            this.PhaTAddr = e.TADDR_PHA;
            this.GrdTAddr = e.TADDR_GROU;
            this.PhaMinRespTAddrTime = e.MIN_RESP_TADDR_T_D_PHA;
            this.GrdMinRespTAddrTime = e.MIN_RESP_TADDR_T_D_GR;
            this.TimeToActivate = e.TIME_T_ACTI;
            this.FaultCurrentOnly = e.FAULT_CURR_ONLY;
            this.VoltageLossOnly = e.VOLT_LOSS_ONLY;
            this.FaultCurVoltLoss = e.FAULT_CURR_W_VOL_LOSS;
            this.VoltLossDelay = e.VOLT_LOSS_DISP;
            if (e.ENA_SWIT_MOD == "Y")
                this.EnabSwitchMode = true;
            else
                this.EnabSwitchMode = false;
            this.CountToTripVolt = e.COUT_T_TRIP_VOLT_LOSS;
            this.ResetTimerSecMode = e.RESET_TIMERR;
            if (e.SEQ_COORDI_MODE == "Y")
                this.SeqCoordMode = true;
            else
                this.SeqCoordMode = false;
            //DA# 200101 - ME Q1 2020---END
        }
    }

    public class RadioButtonItem
    {
        public string Name { get; set; }
        public bool Selected { get; set; }
        public string Value { get; set; }
        public bool Visible { get; set; }
    }
}