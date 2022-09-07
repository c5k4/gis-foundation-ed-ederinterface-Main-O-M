using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
using System.Web.Mvc;

namespace SettingsApp.Models
{
    public class TripSaverModel
    {

        [SettingsValidatorAttribute("TripSaver", "OPERATING_NUM")]
        [Display(Name = "Operating Number")]
        public System.String OperatingNumber { get; set; }

        [SettingsValidatorAttribute("TripSaver", "DEVICE_ID")]
        [Display(Name = "Device ID")]
        public System.Int64? DeviceId { get; set; }

        [SettingsValidatorAttribute("TripSaver", "PREPARED_BY")]
        [Display(Name = "Prepared By")]
        public System.String PreparedBy { get; set; }

        [SettingsValidatorAttribute("TripSaver", "DATE_MODIFIED")]
        [Display(Name = "Date modified")]
        public System.String DateModified { get; set; }

        [SettingsValidatorAttribute("TripSaver", "EFFECTIVE_DT")]
        [Display(Name = "Effective Date")]
        public System.String EffectiveDate { get; set; }

        [SettingsValidatorAttribute("TripSaver", "PEER_REVIEW_DT")]
        [Display(Name = "Peer Reviewer Date")]
        public System.String PeerReviewerDate { get; set; }

        [SettingsValidatorAttribute("TripSaver", "PEER_REVIEW_BY")]
        [Display(Name = "Peer Reviewer")]
        public System.String PeerReviewer { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_EMULATED_DEVICE")]
        [Display(Name = "Emulated Device")]
        public System.String IniEmulatedDevice { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_INVERSE_SEGMENT")]
        [Display(Name = "Inverse Segment")]
        public System.String IniInverseSegment { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_SPEED")]
        [Display(Name = "Speed")]
        public System.String IniSpeed { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_AMPERE_RATING")]
        [Display(Name = "Ampere Rating")]
        public System.String IniAmpereRating { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_MIN_TRIP_A")]
        [Display(Name = "Minimum Trip, A")]
        public System.Decimal? IniMinTripA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_TIME_MULTIPLIER")]
        [Display(Name = "Time Multiplier")]
        public System.Decimal? IniTimeMultiplier { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? IniCurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? IniTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_RESET_TYPE")]
        [Display(Name = "Reset Type")]
        public System.String IniResetType { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_RESET_TIME")]
        [Display(Name = "Reset Time, s")]
        public System.Decimal? IniResetTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_LOWCUTOFF")]
        [Display(Name = "Low Cutoff")]
        public System.String IniLowcutoff { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_LOWCUTOFF_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? IniLowcutoffCurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_DEFINITETIME1")]
        [Display(Name = "Definite Time1")]
        public System.String IniDefiniteTime1 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_DEFINITETIME1_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? IniDefiniteTime1CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_DEFINITETIME1_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? IniDefiniteTime1Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_DEFINITETIME2")]
        [Display(Name = "Definite Time 2")]
        public System.String IniDefiniteTime2 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_DEFINITETIME2_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? IniDefiniteTime2CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_DEFINITETIME2_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? IniDefiniteTime2Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_DEFINITETIME3")]
        [Display(Name = "Definite Time 3")]
        public System.String IniDefiniteTime3 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_DEFINITETIME3_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? IniDefiniteTime3CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_DEFINITETIME3_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? IniDefiniteTime3Time { get; set; }


        [SettingsValidatorAttribute("TripSaver", "INI_OPEN_INTERVAL_TIME")]
        [Display(Name = "Open Interval After Initial Trip, s")]
        public System.Decimal? IniOpenIntervalTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "INI_COIL_RATING")]
        [Display(Name = "Coil Rating")]
        public System.String IniCoilRating { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_EMULATED_DEVICE")]
        [Display(Name = "Emulated Device")]
        public System.String Test1EmulatedDevice { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_INVERSE_SEGMENT")]
        [Display(Name = "Inverse Segment")]
        public System.String Test1InverseSegment { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_SPEED")]
        [Display(Name = "Speed")]
        public System.String Test1Speed { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_AMPERE_RATING")]
        [Display(Name = "Ampere Rating")]
        public System.String Test1AmpereRating { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_MIN_TRIP_A")]
        [Display(Name = "Minimum Trip, A")]
        public System.Decimal? Test1MinTripA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_TIME_MULTIPLIER")]
        [Display(Name = "Time Multiplier")]
        public System.Decimal? Test1TimeMultiplier { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test1CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test1Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_RESET_TYPE")]
        [Display(Name = "Reset Type")]
        public System.String Test1ResetType { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_RESET_TIME")]
        [Display(Name = "Reset Time, s")]
        public System.Decimal? Test1ResetTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_LOWCUTOFF")]
        [Display(Name = "Low Cutoff")]
        public System.String Test1Lowcutoff { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_LOWCUTOFF_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test1LowcutoffCurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_DEFINITETIME1")]
        [Display(Name = "Definite Time1")]
        public System.String Test1DefiniteTime1 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_DEFINITETIME1_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test1DefiniteTime1CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_DEFINITETIME1_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test1DefiniteTime1Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_DEFINITETIME2")]
        [Display(Name = "Definite Time 2")]
        public System.String Test1DefiniteTime2 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_DEFINITETIME2_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test1DefiniteTime2CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_DEFINITETIME2_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test1DefiniteTime2Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_DEFINITETIME3")]
        [Display(Name = "Definite Time 3")]
        public System.String Test1DefiniteTime3 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_DEFINITETIME3_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test1DefiniteTime3CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_DEFINITETIME3_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test1DefiniteTime3Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_OPEN_INTERVAL_TIME")]
        [Display(Name = "Open Interval After Test 1, s")]
        public System.Decimal? Test1OpenIntervalTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST1_COIL_RATING")]
        [Display(Name = "Coil Rating")]
        public System.String Test1CoilRating { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_EMULATED_DEVICE")]
        [Display(Name = "Emulated Device")]
        public System.String Test2EmulatedDevice { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_INVERSE_SEGMENT")]
        [Display(Name = "Inverse Segment")]
        public System.String Test2InverseSegment { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_SPEED")]
        [Display(Name = "Speed")]
        public System.String Test2Speed { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_AMPERE_RATING")]
        [Display(Name = "Ampere Rating")]
        public System.String Test2AmpereRating { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_MIN_TRIP_A")]
        [Display(Name = "Minimum Trip, A")]
        public System.Decimal? Test2MinTripA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_TIME_MULTIPLIER")]
        [Display(Name = "Time Multiplier")]
        public System.Decimal? Test2TimeMultiplier { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test2CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test2Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_RESET_TYPE")]
        [Display(Name = "Reset Type")]
        public System.String Test2ResetType { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_RESET_TIME")]
        [Display(Name = "Reset Time, s")]
        public System.Decimal? Test2ResetTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_LOWCUTOFF")]
        [Display(Name = "Low Cutoff")]
        public System.String Test2Lowcutoff { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_LOWCUTOFF_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test2LowcutoffCurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_DEFINITETIME1")]
        [Display(Name = "Definite Time1")]
        public System.String Test2DefiniteTime1 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_DEFINITETIME1_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test2DefiniteTime1CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_DEFINITETIME1_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test2DefiniteTime1Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_DEFINITETIME2")]
        [Display(Name = "Definite Time 2")]
        public System.String Test2DefiniteTime2 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_DEFINITETIME2_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test2DefiniteTime2CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_DEFINITETIME2_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test2DefiniteTime2Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_DEFINITETIME3")]
        [Display(Name = "Definite Time 3")]
        public System.String Test2DefiniteTime3 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_DEFINITETIME3_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test2DefiniteTime3CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_DEFINITETIME3_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test2DefiniteTime3Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_OPEN_INTERVAL_TIME")]
        [Display(Name = "Open Interval After Test 2, s")]
        public System.Decimal? Test2OpenIntervalTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST2_COIL_RATING")]
        [Display(Name = "Coil Rating")]
        public System.String Test2CoilRating { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_EMULATED_DEVICE")]
        [Display(Name = "Emulated Device")]
        public System.String Test3EmulatedDevice { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_INVERSE_SEGMENT")]
        [Display(Name = "Inverse Segment")]
        public System.String Test3InverseSegment { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_SPEED")]
        [Display(Name = "Speed")]
        public System.String Test3Speed { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_AMPERE_RATING")]
        [Display(Name = "Ampere Rating")]
        public System.String Test3AmpereRating { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_MIN_TRIP_A")]
        [Display(Name = "Minimum Trip, A")]
        public System.Decimal? Test3MinTripA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_TIME_MULTIPLIER")]
        [Display(Name = "Time Multiplier")]
        public System.Decimal? Test3TimeMultiplier { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test3CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test3Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_RESET_TYPE")]
        [Display(Name = "Reset Type")]
        public System.String Test3ResetType { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_RESET_TIME")]
        [Display(Name = "Reset Time, s")]
        public System.Decimal? Test3ResetTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_LOWCUTOFF")]
        [Display(Name = "Low Cutoff")]
        public System.String Test3Lowcutoff { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_LOWCUTOFF_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test3LowcutoffCurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_DEFINITETIME1")]
        [Display(Name = "Definite Time1")]
        public System.String Test3DefiniteTime1 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_DEFINITETIME1_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test3DefiniteTime1CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_DEFINITETIME1_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test3DefiniteTime1Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_DEFINITETIME2")]
        [Display(Name = "Definite Time 2")]
        public System.String Test3DefiniteTime2 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_DEFINITETIME2_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test3DefiniteTime2CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_DEFINITETIME2_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test3DefiniteTime2Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_DEFINITETIME3")]
        [Display(Name = "Definite Time 3")]
        public System.String Test3DefiniteTime3 { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_DEFINITETIME3_CURR_A")]
        [Display(Name = "Current, A")]
        public System.Decimal? Test3DefiniteTime3CurrA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_DEFINITETIME3_TIME")]
        [Display(Name = "Time, s")]
        public System.Decimal? Test3DefiniteTime3Time { get; set; }

        [SettingsValidatorAttribute("TripSaver", "SEQUENCE_RESET_TIME")]
        [Display(Name = "Sequence Reset Time, s")]
        public System.Decimal? SeqResetTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "TEST3_COIL_RATING")]
        [Display(Name = "Coil Rating")]
        public System.String Test3CoilRating { get; set; }

        [SettingsValidatorAttribute("TripSaver", "HIGH_CURRENT_CUTOFF")]
        [Display(Name = "High Current Cutoff, A")]
        public System.String HighCurCutOff { get; set; }

        [SettingsValidatorAttribute("TripSaver", "HIGH_CURRENT_CUTOFF_A")]
        [Display(Name = "High Current Cutoff, A")]
        public System.Decimal? HighCurCutOffA { get; set; }

        [SettingsValidatorAttribute("TripSaver", "SEC_MODE")]
        [Display(Name = "Sectionalizing Mode")]
        public System.String SectMode { get; set; }

        [SettingsValidatorAttribute("TripSaver", "SEC_MODE_COUNTS")]
        [Display(Name = "Sectionalizing Mode Counts")]
        public System.Int16? SectModeCounts { get; set; }

        [SettingsValidatorAttribute("TripSaver", "SEC_MODE_RESET_TIME")]
        [Display(Name = "Sectionalizing Mode Reset Time, s")]
        public System.Decimal? SectModeResetTime { get; set; }

        [SettingsValidatorAttribute("TripSaver", "SEC_MODE_STARTING_CURRENT")]
        [Display(Name = "Sectionalizing Mode Starting Current, A")]
        public System.Decimal? SectModeStartingCur { get; set; }

        [SettingsValidatorAttribute("TripSaver", "OK_TO_BYPASS")]
        [Display(Name = "Ok To Bypass :")]
        public System.String OkToBypass { get; set; }

        [SettingsValidatorAttribute("TripSaver", "BYPASS_PLANS")]
        [Display(Name = "Bypass plan :")]
        public System.String BypassPlan { get; set; }

        public bool Release { get; set; }
        public bool IniLowCutOffChk { get; set; }
        public bool IniDefiniteTime1Chk { get; set; }
        public bool IniDefiniteTime2Chk { get; set; }
        public bool IniDefiniteTime3Chk { get; set; }
        public bool Test1LowCutOffChk { get; set; }
        public bool Test1DefiniteTime1Chk { get; set; }
        public bool Test1DefiniteTime2Chk { get; set; }
        public bool Test1DefiniteTime3Chk { get; set; }
        public bool Test2LowCutOffChk { get; set; }
        public bool Test2DefiniteTime1Chk { get; set; }
        public bool Test2DefiniteTime2Chk { get; set; }
        public bool Test2DefiniteTime3Chk { get; set; }
        public bool Test3LowCutOffChk { get; set; }
        public bool Test3DefiniteTime1Chk { get; set; }
        public bool Test3DefiniteTime2Chk { get; set; }
        public bool Test3DefiniteTime3Chk { get; set; }
        public bool Test1Visible { get; set; }
        public bool Test2Visible { get; set; }
        public bool Test3Visible { get; set; }
        public bool HighCurCutoffChk { get; set; }
        public bool CopyInitialToTest1Vis { get; set; }
        public bool CopyTest1ToTest2Vis { get; set; }
        public bool CopyTest2ToTest3Vis { get; set; }

        public List<GISAttributes> GISAttributes { get; set; }

        public HashSet<string> FieldsToDisplay { get; set; }

        public SelectList IniEmulatedDeviceList { get; set; }
        public SelectList IniInverseSegmentList { get; set; }
        public SelectList IniSpeedList { get; set; }
        public SelectList IniAmpereRatingList { get; set; }
        public SelectList IniCoilRatingList { get; set; }
        public SelectList IniResetTypeList { get; set; }

        public SelectList Test1EmulatedDeviceList { get; set; }
        public SelectList Test1InverseSegmentList { get; set; }
        public SelectList Test1SpeedList { get; set; }
        public SelectList Test1AmpereRatingList { get; set; }
        public SelectList Test1CoilRatingList { get; set; }
        public SelectList Test1ResetTypeList { get; set; }

        public SelectList Test2EmulatedDeviceList { get; set; }
        public SelectList Test2InverseSegmentList { get; set; }
        public SelectList Test2SpeedList { get; set; }
        public SelectList Test2AmpereRatingList { get; set; }
        public SelectList Test2CoilRatingList { get; set; }
        public SelectList Test2ResetTypeList { get; set; }

        public SelectList Test3EmulatedDeviceList { get; set; }
        public SelectList Test3InverseSegmentList { get; set; }
        public SelectList Test3SpeedList { get; set; }
        public SelectList Test3AmpereRatingList { get; set; }
        public SelectList Test3CoilRatingList { get; set; }
        public SelectList Test3ResetTypeList { get; set; }

        public SelectList SectModeList { get; set; }

        public string DropDownPostbackScript { get; set; }
        public string IniLowChkPostbackScript { get; set; }
        public string Test1LowChkPostbackScript { get; set; }
        public string Test2LowChkPostbackScript { get; set; }
        public string Test3LowChkPostbackScript { get; set; }
        public string IniDef1ChkPostbackScript { get; set; }
        public string Test1Def1ChkPostbackScript { get; set; }
        public string Test2Def1ChkPostbackScript { get; set; }
        public string Test3Def1ChkPostbackScript { get; set; }
        public string IniDef2ChkPostbackScript { get; set; }
        public string Test1Def2ChkPostbackScript { get; set; }
        public string Test2Def2ChkPostbackScript { get; set; }
        public string Test3Def2ChkPostbackScript { get; set; }
        public string IniDef3ChkPostbackScript { get; set; }
        public string Test1Def3ChkPostbackScript { get; set; }
        public string Test2Def3ChkPostbackScript { get; set; }
        public string Test3Def3ChkPostbackScript { get; set; }
        public string HighCurCutoffPostbackScript { get; set; }
        public string SectModePostbackScript { get; set; }


        public void PopulateEntityFromModel(SM_RECLOSER_TS e)
        {
            e.PREPARED_BY = this.PreparedBy;
            e.DATE_MODIFIED = Utility.ParseDateTime(this.DateModified);
            e.EFFECTIVE_DT = Utility.ParseDateTime(this.EffectiveDate);
            e.PEER_REVIEW_DT = Utility.ParseDateTime(this.PeerReviewerDate);
            e.PEER_REVIEW_BY = this.PeerReviewer;
            e.OK_TO_BYPASS = this.OkToBypass;
            e.BYPASS_PLANS = this.BypassPlan;
            //INITIAL TRIP
            e.INI_EMULATED_DEVICE = this.IniEmulatedDevice;
            e.INI_INVERSE_SEGMENT = this.IniInverseSegment;
            e.INI_SPEED = this.IniSpeed;
            e.INI_AMPERE_RATING = this.IniAmpereRating;
            e.INI_MIN_TRIP_A = this.IniMinTripA;
            e.INI_TIME_MULTIPLIER = this.IniTimeMultiplier;
            e.INI_RESET_TYPE = this.IniResetType;
            e.INI_RESET_TIME = this.IniResetTime;
            e.INI_LOWCUTOFF = this.IniLowcutoff;
            e.INI_LOWCUTOFF_CURR_A = this.IniLowcutoffCurrA;
            e.INI_DEFINITETIME1 = this.IniDefiniteTime1;
            e.INI_DEFINITETIME1_CURR_A = this.IniDefiniteTime1CurrA;
            e.INI_DEFINITETIME1_TIME = this.IniDefiniteTime1Time;
            e.INI_DEFINITETIME2 = this.IniDefiniteTime2;
            e.INI_DEFINITETIME2_CURR_A = this.IniDefiniteTime2CurrA;
            e.INI_DEFINITETIME2_TIME = this.IniDefiniteTime2Time;
            e.INI_OPEN_INTERVAL_TIME = this.IniOpenIntervalTime;
            e.INI_COIL_RATING = this.IniCoilRating;
            e.INI_CURR_A = this.IniCurrA;
            e.INI_TIME = this.IniTime;
            e.INI_DEFINITETIME3 = this.IniDefiniteTime3;
            e.INI_DEFINITETIME3_CURR_A = this.IniDefiniteTime3CurrA;
            e.INI_DEFINITETIME3_TIME = this.IniDefiniteTime3Time;

            //TEST 1
            e.TEST1_EMULATED_DEVICE = this.Test1EmulatedDevice;
            e.TEST1_INVERSE_SEGMENT = this.Test1InverseSegment;
            e.TEST1_SPEED = this.Test1Speed;
            e.TEST1_AMPERE_RATING = this.Test1AmpereRating;
            e.TEST1_MIN_TRIP_A = this.Test1MinTripA;
            e.TEST1_TIME_MULTIPLIER = this.Test1TimeMultiplier;
            e.TEST1_RESET_TYPE = this.Test1ResetType;
            e.TEST1_RESET_TIME = this.Test1ResetTime;
            e.TEST1_LOWCUTOFF = this.Test1Lowcutoff;
            e.TEST1_LOWCUTOFF_CURR_A = this.Test1LowcutoffCurrA;
            e.TEST1_DEFINITETIME1 = this.Test1DefiniteTime1;
            e.TEST1_DEFINITETIME1_CUR_A = this.Test1DefiniteTime1CurrA;
            e.TEST1_DEFINITETIME1_TIME = this.Test1DefiniteTime1Time;
            e.TEST1_DEFINITETIME2 = this.Test1DefiniteTime2;
            e.TEST1_DEFINITETIME2_CURR_A = this.Test1DefiniteTime2CurrA;
            e.TEST1_DEFINITETIME2_TIME = this.Test1DefiniteTime2Time;
            e.TEST1_OPEN_INTERVAL_TIME = this.Test1OpenIntervalTime;
            e.TEST1_COIL_RATING = this.Test1CoilRating;
            e.TEST1_CURR_A = this.Test1CurrA;
            e.TEST1_TIME = this.Test1Time;
            e.TEST1_DEFINITETIME3 = this.Test1DefiniteTime3;
            e.TEST1_DEFINITETIME3_CURR_A = this.Test1DefiniteTime3CurrA;
            e.TEST1_DEFINITETIME3_TIME = this.Test1DefiniteTime3Time;

            //TEST 2
            e.TEST2_EMULATED_DEVICE = this.Test2EmulatedDevice;
            e.TEST2_INVERSE_SEGMENT = this.Test2InverseSegment;
            e.TEST2_SPEED = this.Test2Speed;
            e.TEST2_AMPERE_RATING = this.Test2AmpereRating;
            e.TEST2_MIN_TRIP_A = this.Test2MinTripA;
            e.TEST2_TIME_MULTIPLIER = this.Test2TimeMultiplier;
            e.TEST2_RESET_TYPE = this.Test2ResetType;
            e.TEST2_RESET_TIME = this.Test2ResetTime;
            e.TEST2_LOWCUTOFF = this.Test2Lowcutoff;
            e.TEST2_LOWCUTOFF_CURR_A = this.Test2LowcutoffCurrA;
            e.TEST2_DEFINITETIME1 = this.Test2DefiniteTime1;
            e.TEST2_DEFINITETIME1_CURR_A = this.Test2DefiniteTime1CurrA;
            e.TEST2_DEFINITETIME1_TIME = this.Test2DefiniteTime1Time;
            e.TEST2_DEFINITETIME2 = this.Test2DefiniteTime2;
            e.TEST2_DEFINITETIME2_CURR_A = this.Test2DefiniteTime2CurrA;
            e.TEST2_DEFINITETIME2_TIME = this.Test2DefiniteTime2Time;
            e.TEST2_OPEN_INTERVAL_TIME = this.Test2OpenIntervalTime;
            e.TEST2_COIL_RATING = this.Test2CoilRating;
            e.TEST2_CURR_A = this.Test2CurrA;
            e.TEST2_TIME = this.Test2Time;
            e.TEST2_DEFINITETIME3 = this.Test2DefiniteTime3;
            e.TEST2_DEFINITETIME3_CURR_A = this.Test2DefiniteTime3CurrA;
            e.TEST2_DEFINITETIME3_TIME = this.Test2DefiniteTime3Time;

            //TEST 3
            e.TEST3_EMULATED_DEVICE = this.Test3EmulatedDevice;
            e.TEST3_INVERSE_SEGMENT = this.Test3InverseSegment;
            e.TEST3_SPEED = this.Test3Speed;
            e.TEST3_AMPERE_RATING = this.Test3AmpereRating;
            e.TEST3_MIN_TRIP_A = this.Test3MinTripA;
            e.TEST3_TIME_MULTIPLIER = this.Test3TimeMultiplier;
            e.TEST3_RESET_TYPE = this.Test3ResetType;
            e.TEST3_RESET_TIME = this.Test3ResetTime;
            e.TEST3_LOWCUTOFF = this.Test3Lowcutoff;
            e.TEST3_LOWCUTOFF_CURR_A = this.Test3LowcutoffCurrA;
            e.TEST3_DEFINITETIME1 = this.Test3DefiniteTime1;
            e.TEST3_DEFINITETIME1_CURR_A = this.Test3DefiniteTime1CurrA;
            e.TEST3_DEFINITETIME1_TIME = this.Test3DefiniteTime1Time;
            e.TEST3_DEFINITETIME2 = this.Test3DefiniteTime2;
            e.TEST3_DEFINITETIME2_CURR_A = this.Test3DefiniteTime2CurrA;
            e.TEST3_DEFINITETIME2_TIME = this.Test3DefiniteTime2Time;
            e.TEST3_COIL_RATING = this.Test3CoilRating;
            e.TEST3_CURR_A = this.Test3CurrA;
            e.TEST3_TIME = this.Test3Time;
            e.TEST3_DEFINITETIME3 = this.Test3DefiniteTime3;
            e.TEST3_DEFINITETIME3_CURR_A = this.Test3DefiniteTime3CurrA;
            e.TEST3_DEFINITETIME3_TIME = this.Test3DefiniteTime3Time;
            //if (this.HighCurCutOff == "Y")
            //    this.HighCurCutoffChk = true;
            //else
            //{
            //    this.HighCurCutoffChk = false;
            //    this.HighCurCutOff = "N";
            //}
            e.SEQUENCE_RESET_TIME = this.SeqResetTime;
            e.HIGH_CURRENT_CUTOFF = this.HighCurCutOff;
            e.HIGH_CURRENT_CUTOFF_A = this.HighCurCutOffA;
            e.SEC_MODE = this.SectMode;
            e.SEC_MODE_COUNTS = this.SectModeCounts;
            e.SEC_MODE_RESET_TIME = this.SectModeResetTime;
            e.SEC_MODE_STARTING_CURRENT = this.SectModeStartingCur;
        }

        public void PopulateModelFromEntity(SM_RECLOSER_TS e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.BypassPlan = e.BYPASS_PLANS;
            //initial trip
            this.IniEmulatedDevice = e.INI_EMULATED_DEVICE;
            this.IniInverseSegment = e.INI_INVERSE_SEGMENT;
            this.IniSpeed = e.INI_SPEED;
            this.IniAmpereRating = e.INI_AMPERE_RATING;
            this.IniMinTripA = e.INI_MIN_TRIP_A;
            this.IniTimeMultiplier = e.INI_TIME_MULTIPLIER;
            this.IniResetType = e.INI_RESET_TYPE;
            this.IniResetTime = e.INI_RESET_TIME;
            this.IniLowcutoff = e.INI_LOWCUTOFF;
            this.IniLowcutoffCurrA = e.INI_LOWCUTOFF_CURR_A;
            this.IniDefiniteTime1 = e.INI_DEFINITETIME1;
            this.IniDefiniteTime1CurrA = e.INI_DEFINITETIME1_CURR_A;
            this.IniDefiniteTime1Time = e.INI_DEFINITETIME1_TIME;
            this.IniDefiniteTime2 = e.INI_DEFINITETIME2;
            this.IniDefiniteTime2CurrA = e.INI_DEFINITETIME2_CURR_A;
            this.IniDefiniteTime2Time = e.INI_DEFINITETIME2_TIME;
            this.IniOpenIntervalTime = e.INI_OPEN_INTERVAL_TIME;
            this.IniCoilRating = e.INI_COIL_RATING;
            this.IniCurrA = e.INI_CURR_A;
            this.IniTime = e.INI_TIME;
            this.IniDefiniteTime3 = e.INI_DEFINITETIME3;
            this.IniDefiniteTime3CurrA = e.INI_DEFINITETIME3_CURR_A;
            this.IniDefiniteTime3Time = e.INI_DEFINITETIME3_TIME;

            //test1
            this.Test1EmulatedDevice = e.TEST1_EMULATED_DEVICE;
            this.Test1InverseSegment = e.TEST1_INVERSE_SEGMENT;
            this.Test1Speed = e.TEST1_SPEED;
            this.Test1AmpereRating = e.TEST1_AMPERE_RATING;
            this.Test1MinTripA = e.TEST1_MIN_TRIP_A;
            this.Test1TimeMultiplier = e.TEST1_TIME_MULTIPLIER;
            this.Test1ResetType = e.TEST1_RESET_TYPE;
            this.Test1ResetTime = e.TEST1_RESET_TIME;
            this.Test1Lowcutoff = e.TEST1_LOWCUTOFF;
            this.Test1LowcutoffCurrA = e.TEST1_LOWCUTOFF_CURR_A;
            this.Test1DefiniteTime1 = e.TEST1_DEFINITETIME1;
            this.Test1DefiniteTime1CurrA = e.TEST1_DEFINITETIME1_CUR_A;
            this.Test1DefiniteTime1Time = e.TEST1_DEFINITETIME1_TIME;
            this.Test1DefiniteTime2 = e.TEST1_DEFINITETIME2;
            this.Test1DefiniteTime2CurrA = e.TEST1_DEFINITETIME2_CURR_A;
            this.Test1DefiniteTime2Time = e.TEST1_DEFINITETIME2_TIME;
            this.Test1OpenIntervalTime = e.TEST1_OPEN_INTERVAL_TIME;
            this.Test1CoilRating = e.TEST1_COIL_RATING;
            this.Test1CurrA = e.TEST1_CURR_A;
            this.Test1Time = e.TEST1_TIME;
            this.Test1DefiniteTime3 = e.TEST1_DEFINITETIME3;
            this.Test1DefiniteTime3CurrA = e.TEST1_DEFINITETIME3_CURR_A;
            this.Test1DefiniteTime3Time = e.TEST1_DEFINITETIME3_TIME;

            //test2
            this.Test2EmulatedDevice = e.TEST2_EMULATED_DEVICE;
            this.Test2InverseSegment = e.TEST2_INVERSE_SEGMENT;
            this.Test2Speed = e.TEST2_SPEED;
            this.Test2AmpereRating = e.TEST2_AMPERE_RATING;
            this.Test2MinTripA = e.TEST2_MIN_TRIP_A;
            this.Test2TimeMultiplier = e.TEST2_TIME_MULTIPLIER;
            this.Test2ResetType = e.TEST2_RESET_TYPE;
            this.Test2ResetTime = e.TEST2_RESET_TIME;
            this.Test2Lowcutoff = e.TEST2_LOWCUTOFF;
            this.Test2LowcutoffCurrA = e.TEST2_LOWCUTOFF_CURR_A;
            this.Test2DefiniteTime1 = e.TEST2_DEFINITETIME1;
            this.Test2DefiniteTime1CurrA = e.TEST2_DEFINITETIME1_CURR_A;
            this.Test2DefiniteTime1Time = e.TEST2_DEFINITETIME1_TIME;
            this.Test2DefiniteTime2 = e.TEST2_DEFINITETIME2;
            this.Test2DefiniteTime2CurrA = e.TEST2_DEFINITETIME2_CURR_A;
            this.Test2DefiniteTime2Time = e.TEST2_DEFINITETIME2_TIME;
            this.Test2OpenIntervalTime = e.TEST2_OPEN_INTERVAL_TIME;
            this.Test2CoilRating = e.TEST2_COIL_RATING;
            this.Test2CurrA = e.TEST2_CURR_A;
            this.Test2Time = e.TEST2_TIME;
            this.Test2DefiniteTime3 = e.TEST2_DEFINITETIME3;
            this.Test2DefiniteTime3CurrA = e.TEST2_DEFINITETIME3_CURR_A;
            this.Test2DefiniteTime3Time = e.TEST2_DEFINITETIME3_TIME;

            //test3
            this.Test3EmulatedDevice = e.TEST3_EMULATED_DEVICE;
            this.Test3InverseSegment = e.TEST3_INVERSE_SEGMENT;
            this.Test3EmulatedDevice = e.TEST3_EMULATED_DEVICE;
            this.Test3InverseSegment = e.TEST3_INVERSE_SEGMENT;
            this.Test3Speed = e.TEST3_SPEED;
            this.Test3AmpereRating = e.TEST3_AMPERE_RATING;
            this.Test3MinTripA = e.TEST3_MIN_TRIP_A;
            this.Test3TimeMultiplier = e.TEST3_TIME_MULTIPLIER;
            this.Test3ResetType = e.TEST3_RESET_TYPE;
            this.Test3ResetTime = e.TEST3_RESET_TIME;
            this.Test3Lowcutoff = e.TEST3_LOWCUTOFF;
            this.Test3LowcutoffCurrA = e.TEST3_LOWCUTOFF_CURR_A;
            this.Test3DefiniteTime1 = e.TEST3_DEFINITETIME1;
            this.Test3DefiniteTime1CurrA = e.TEST3_DEFINITETIME1_CURR_A;
            this.Test3DefiniteTime1Time = e.TEST3_DEFINITETIME1_TIME;
            this.Test3DefiniteTime2 = e.TEST3_DEFINITETIME2;
            this.Test3DefiniteTime2CurrA = e.TEST3_DEFINITETIME2_CURR_A;
            this.Test3DefiniteTime2Time = e.TEST3_DEFINITETIME2_TIME;
            this.Test3CoilRating = e.TEST3_COIL_RATING;
            this.Test3CurrA = e.TEST3_CURR_A;
            this.Test3Time = e.TEST3_TIME;
            this.Test3DefiniteTime3 = e.TEST3_DEFINITETIME3;
            this.Test3DefiniteTime3CurrA = e.TEST3_DEFINITETIME3_CURR_A;
            this.Test3DefiniteTime3Time = e.TEST3_DEFINITETIME3_TIME;

            this.SeqResetTime = e.SEQUENCE_RESET_TIME;
            this.HighCurCutOff = e.HIGH_CURRENT_CUTOFF;
            this.HighCurCutOffA = e.HIGH_CURRENT_CUTOFF_A;
            this.SectMode = e.SEC_MODE;
            this.SectModeCounts = e.SEC_MODE_COUNTS;
            this.SectModeResetTime = e.SEC_MODE_RESET_TIME;
            this.SectModeStartingCur = e.SEC_MODE_STARTING_CURRENT;

            //checkboxes
            if (this.IniLowcutoff == "Y")
                this.IniLowCutOffChk = true;
            else
            {
                this.IniLowCutOffChk = false;
                this.IniLowcutoff = "N";
            }
            if (this.IniDefiniteTime1 == "Y")
                this.IniDefiniteTime1Chk = true;
            else
            {
                this.IniDefiniteTime1Chk = false;
                this.IniDefiniteTime1 = "N";
            }
            if (this.IniDefiniteTime2 == "Y")
                this.IniDefiniteTime2Chk = true;
            else
            {
                this.IniDefiniteTime2Chk = false;
                this.IniDefiniteTime2 = "N";
            }
            if (this.IniDefiniteTime3 == "Y")
                this.IniDefiniteTime3Chk = true;
            else
            {
                this.IniDefiniteTime3Chk = false;
                this.IniDefiniteTime3 = "N";
            }

            if (this.Test1Lowcutoff == "Y")
                this.Test1LowCutOffChk = true;
            else
            {
                this.Test1LowCutOffChk = false;
                this.Test1Lowcutoff = "N";
            }
            if (this.Test1DefiniteTime1 == "Y")
                this.Test1DefiniteTime1Chk = true;
            else
            {
                this.Test1DefiniteTime1Chk = false;
                this.Test1DefiniteTime1 = "N";
            }
            if (this.Test1DefiniteTime2 == "Y")
                this.Test1DefiniteTime2Chk = true;
            else
            {
                this.Test1DefiniteTime2Chk = false;
                this.Test1DefiniteTime2 = "N";
            }
            if (this.Test1DefiniteTime3 == "Y")
                this.Test1DefiniteTime3Chk = true;
            else
            {
                this.Test1DefiniteTime3Chk = false;
                this.Test1DefiniteTime3 = "N";
            }

            if (this.Test2Lowcutoff == "Y")
                this.Test2LowCutOffChk = true;
            else
            {
                this.Test2LowCutOffChk = false;
                this.Test2Lowcutoff = "N";
            }
            if (this.Test2DefiniteTime1 == "Y")
                this.Test2DefiniteTime1Chk = true;
            else
            {
                this.Test2DefiniteTime1Chk = false;
                this.Test2DefiniteTime1 = "N"; ;
            }
            if (this.Test2DefiniteTime2 == "Y")
                this.Test2DefiniteTime2Chk = true;
            else
            {
                this.Test2DefiniteTime2Chk = false;
                this.Test2DefiniteTime2 = "N";
            }
            if (this.Test2DefiniteTime3 == "Y")
                this.Test2DefiniteTime3Chk = true;
            else
            {
                this.Test2DefiniteTime3Chk = false;
                this.Test2DefiniteTime3 = "N";
            }

            if (this.Test3Lowcutoff == "Y")
                this.Test3LowCutOffChk = true;
            else
            {
                this.Test3LowCutOffChk = false;
                this.Test3Lowcutoff = "N";
            }
            if (this.Test3DefiniteTime1 == "Y")
                this.Test3DefiniteTime1Chk = true;
            else
            {
                this.Test3DefiniteTime1Chk = false;
                this.Test3DefiniteTime1 = "N";
            }
            if (this.Test3DefiniteTime2 == "Y")
                this.Test3DefiniteTime2Chk = true;
            else
            {
                this.Test3DefiniteTime2Chk = false;
                this.Test3DefiniteTime2 = "N";
            }
            if (this.Test3DefiniteTime3 == "Y")
                this.Test3DefiniteTime3Chk = true;
            else
            {
                this.Test3DefiniteTime3Chk = false;
                this.Test3DefiniteTime3 = "N";
            }

            if (this.HighCurCutOff == "Y")
                this.HighCurCutoffChk = true;
            else
            {
                this.HighCurCutoffChk = false;
                this.HighCurCutOff = "N";
            }

        }

        public void PopulateHistoryFromEntity(SM_RECLOSER_TS_HIST entityHistory, SM_RECLOSER_TS e)
        {
            entityHistory.GLOBAL_ID = e.GLOBAL_ID;
            entityHistory.FEATURE_CLASS_NAME = e.FEATURE_CLASS_NAME;
            entityHistory.OPERATING_NUM = e.OPERATING_NUM;
            entityHistory.DEVICE_ID = e.DEVICE_ID;
            entityHistory.PREPARED_BY = e.PREPARED_BY;

            entityHistory.DATE_MODIFIED = e.DATE_MODIFIED;
            entityHistory.TIMESTAMP = e.TIMESTAMP;
            entityHistory.EFFECTIVE_DT = e.EFFECTIVE_DT;
            entityHistory.PEER_REVIEW_DT = e.PEER_REVIEW_DT;
            entityHistory.PEER_REVIEW_BY = e.PEER_REVIEW_BY;
            entityHistory.DIVISION = e.DIVISION;
            entityHistory.DISTRICT = e.DISTRICT;
            //entityHistory.NOTES = e.NOTES;
            entityHistory.OK_TO_BYPASS = e.OK_TO_BYPASS;
            entityHistory.BYPASS_PLANS = e.BYPASS_PLANS;
            //Intial
            entityHistory.INI_EMULATED_DEVICE = e.INI_EMULATED_DEVICE;
            entityHistory.INI_INVERSE_SEGMENT = e.INI_INVERSE_SEGMENT;
            entityHistory.INI_SPEED = e.INI_SPEED;
            entityHistory.INI_AMPERE_RATING = e.INI_AMPERE_RATING;
            entityHistory.INI_MIN_TRIP_A = e.INI_MIN_TRIP_A;
            entityHistory.INI_TIME_MULTIPLIER = e.INI_TIME_MULTIPLIER;
            entityHistory.INI_RESET_TYPE = e.INI_RESET_TYPE;
            entityHistory.INI_RESET_TIME = e.INI_RESET_TIME;
            entityHistory.INI_LOWCUTOFF = e.INI_LOWCUTOFF;
            entityHistory.INI_LOWCUTOFF_CURR_A = e.INI_LOWCUTOFF_CURR_A;
            entityHistory.INI_DEFINITETIME1 = e.INI_DEFINITETIME1;
            entityHistory.INI_DEFINITETIME1_CURR_A = e.INI_DEFINITETIME1_CURR_A;
            entityHistory.INI_DEFINITETIME1_TIME = e.INI_DEFINITETIME1_TIME;
            entityHistory.INI_DEFINITETIME2 = e.INI_DEFINITETIME2;
            entityHistory.INI_DEFINITETIME2_CURR_A = e.INI_DEFINITETIME2_CURR_A;
            entityHistory.INI_DEFINITETIME2_TIME = e.INI_DEFINITETIME2_TIME;
            entityHistory.INI_OPEN_INTERVAL_TIME = e.INI_OPEN_INTERVAL_TIME;
            entityHistory.INI_COIL_RATING = e.INI_COIL_RATING;
            entityHistory.INI_CURR_A = e.INI_CURR_A;
            entityHistory.INI_TIME = e.INI_TIME;
            entityHistory.INI_DEFINITETIME3 = e.INI_DEFINITETIME3;
            entityHistory.INI_DEFINITETIME3_CURR_A = e.INI_DEFINITETIME3_CURR_A;
            entityHistory.INI_DEFINITETIME3_TIME = e.INI_DEFINITETIME3_TIME;
            //Test1

            entityHistory.TEST1_EMULATED_DEVICE = e.TEST1_EMULATED_DEVICE;
            entityHistory.TEST1_INVERSE_SEGMENT = e.TEST1_INVERSE_SEGMENT;
            entityHistory.TEST1_SPEED = e.TEST1_SPEED;
            entityHistory.TEST1_AMPERE_RATING = e.TEST1_AMPERE_RATING;
            entityHistory.TEST1_MIN_TRIP_A = e.TEST1_MIN_TRIP_A;
            entityHistory.TEST1_TIME_MULTIPLIER = e.TEST1_TIME_MULTIPLIER;
            entityHistory.TEST1_RESET_TYPE = e.TEST1_RESET_TYPE;
            entityHistory.TEST1_RESET_TIME = e.TEST1_RESET_TIME;
            entityHistory.TEST1_LOWCUTOFF = e.TEST1_LOWCUTOFF;
            entityHistory.TEST1_LOWCUTOFF_CURR_A = e.TEST1_LOWCUTOFF_CURR_A;
            entityHistory.TEST1_DEFINITETIME1 = e.TEST1_DEFINITETIME1;
            entityHistory.TEST1_DEFINITETIME1_CUR_A = e.TEST1_DEFINITETIME1_CUR_A;
            entityHistory.TEST1_DEFINITETIME1_TIME = e.TEST1_DEFINITETIME1_TIME;
            entityHistory.TEST1_DEFINITETIME2 = e.TEST1_DEFINITETIME2;
            entityHistory.TEST1_DEFINITETIME2_CURR_A = e.TEST1_DEFINITETIME2_CURR_A;
            entityHistory.TEST1_DEFINITETIME2_TIME = e.TEST1_DEFINITETIME2_TIME;
            entityHistory.TEST1_OPEN_INTERVAL_TIME = e.TEST1_OPEN_INTERVAL_TIME;
            entityHistory.TEST1_COIL_RATING = e.TEST1_COIL_RATING;
            entityHistory.TEST1_TIME = e.TEST1_TIME;
            entityHistory.TEST1_DEFINITETIME3 = e.TEST1_DEFINITETIME3;
            entityHistory.TEST1_DEFINITETIME3_CURR_A = e.TEST1_DEFINITETIME3_CURR_A;
            entityHistory.TEST1_DEFINITETIME3_TIME = e.TEST1_DEFINITETIME3_TIME;
            //Test2
            entityHistory.TEST2_EMULATED_DEVICE = e.TEST2_EMULATED_DEVICE;
            entityHistory.TEST2_INVERSE_SEGMENT = e.TEST2_INVERSE_SEGMENT;
            entityHistory.TEST2_SPEED = e.TEST2_SPEED;
            entityHistory.TEST2_AMPERE_RATING = e.TEST2_AMPERE_RATING;
            entityHistory.TEST2_MIN_TRIP_A = e.TEST2_MIN_TRIP_A;
            entityHistory.TEST2_TIME_MULTIPLIER = e.TEST2_TIME_MULTIPLIER;
            entityHistory.TEST2_RESET_TYPE = e.TEST2_RESET_TYPE;
            entityHistory.TEST2_RESET_TIME = e.TEST2_RESET_TIME;
            entityHistory.TEST2_LOWCUTOFF = e.TEST2_LOWCUTOFF;
            entityHistory.TEST2_LOWCUTOFF_CURR_A = e.TEST2_LOWCUTOFF_CURR_A;
            entityHistory.TEST2_DEFINITETIME1 = e.TEST2_DEFINITETIME1;
            entityHistory.TEST2_DEFINITETIME1_CURR_A = e.TEST2_DEFINITETIME1_CURR_A;
            entityHistory.TEST2_DEFINITETIME1_TIME = e.TEST2_DEFINITETIME1_TIME;
            entityHistory.TEST2_DEFINITETIME2 = e.TEST2_DEFINITETIME2;
            entityHistory.TEST2_DEFINITETIME2_CURR_A = e.TEST2_DEFINITETIME2_CURR_A;
            entityHistory.TEST2_DEFINITETIME2_TIME = e.TEST2_DEFINITETIME2_TIME;
            entityHistory.TEST2_OPEN_INTERVAL_TIME = e.TEST2_OPEN_INTERVAL_TIME;
            entityHistory.TEST2_COIL_RATING = e.TEST2_COIL_RATING;
            entityHistory.TEST2_CURR_A = e.TEST2_CURR_A;
            entityHistory.TEST2_TIME = e.TEST2_TIME;
            entityHistory.TEST2_DEFINITETIME3 = e.TEST2_DEFINITETIME3;
            entityHistory.TEST2_DEFINITETIME3_CURR_A = e.TEST2_DEFINITETIME3_CURR_A;
            entityHistory.TEST2_DEFINITETIME3_TIME = e.TEST2_DEFINITETIME3_TIME;

            //test3
            entityHistory.TEST3_EMULATED_DEVICE = e.TEST3_EMULATED_DEVICE;
            entityHistory.TEST3_INVERSE_SEGMENT = e.TEST3_INVERSE_SEGMENT;
            entityHistory.TEST3_EMULATED_DEVICE = e.TEST3_EMULATED_DEVICE;
            entityHistory.TEST3_INVERSE_SEGMENT = e.TEST3_INVERSE_SEGMENT;
            entityHistory.TEST3_SPEED = e.TEST3_SPEED;
            entityHistory.TEST3_AMPERE_RATING = e.TEST3_AMPERE_RATING;
            entityHistory.TEST3_MIN_TRIP_A = e.TEST3_MIN_TRIP_A;
            entityHistory.TEST3_TIME_MULTIPLIER = e.TEST3_TIME_MULTIPLIER;
            entityHistory.TEST3_RESET_TYPE = e.TEST3_RESET_TYPE;
            entityHistory.TEST3_RESET_TIME = e.TEST3_RESET_TIME;
            entityHistory.TEST3_LOWCUTOFF = e.TEST3_LOWCUTOFF;
            entityHistory.TEST3_LOWCUTOFF_CURR_A = e.TEST3_LOWCUTOFF_CURR_A;
            entityHistory.TEST3_DEFINITETIME1 = e.TEST3_DEFINITETIME1;
            entityHistory.TEST3_DEFINITETIME1_CURR_A = e.TEST3_DEFINITETIME1_CURR_A;
            entityHistory.TEST3_DEFINITETIME1_TIME = e.TEST3_DEFINITETIME1_TIME;
            entityHistory.TEST3_DEFINITETIME2 = e.TEST3_DEFINITETIME2;
            entityHistory.TEST3_DEFINITETIME2_CURR_A = e.TEST3_DEFINITETIME2_CURR_A;
            entityHistory.TEST3_DEFINITETIME2_TIME = e.TEST3_DEFINITETIME2_TIME;
            entityHistory.TEST3_COIL_RATING = e.TEST3_COIL_RATING;
            entityHistory.TEST3_CURR_A = e.TEST3_CURR_A;
            entityHistory.TEST3_TIME = e.TEST3_TIME;
            entityHistory.TEST3_DEFINITETIME3 = e.TEST3_DEFINITETIME3;
            entityHistory.TEST3_DEFINITETIME3_CURR_A = e.TEST3_DEFINITETIME3_CURR_A;
            entityHistory.TEST3_DEFINITETIME3_TIME = e.TEST3_DEFINITETIME3_TIME;

            entityHistory.SEQUENCE_RESET_TIME = e.SEQUENCE_RESET_TIME;
            entityHistory.HIGH_CURRENT_CUTOFF = e.HIGH_CURRENT_CUTOFF;
            entityHistory.HIGH_CURRENT_CUTOFF_A = e.HIGH_CURRENT_CUTOFF_A;
            entityHistory.SEC_MODE = e.SEC_MODE;
            entityHistory.SEC_MODE_COUNTS = e.SEC_MODE_COUNTS;
            entityHistory.SEC_MODE_RESET_TIME = e.SEC_MODE_RESET_TIME;
            entityHistory.SEC_MODE_STARTING_CURRENT = e.SEC_MODE_STARTING_CURRENT;

            //checkboxes
            //if (e.INI_LOWCUTOFF == "Y")
            //    entityHistory.INI_LOWCUTOFF = "Y";

            //else
            //    this.IniLowCutOffChk = false;
            //if (e.INI_DEFINITETIME1 == "Y")
            //    this.IniDefiniteTime1Chk = true;
            //else
            //    this.IniDefiniteTime1Chk = false;
            //if (e.INI_DEFINITETIME2 == "Y")
            //    this.IniDefiniteTime2Chk = true;
            //else
            //    this.IniDefiniteTime2Chk = false;
            //if (e.INI_DEFINITETIME3 == "Y")
            //    this.IniDefiniteTime3Chk = true;
            //else
            //    this.IniDefiniteTime3Chk = false;

            //if (e.TEST1_LOWCUTOFF == "Y")
            //    this.Test1LowCutOffChk = true;
            //else
            //    this.Test1LowCutOffChk = false;
            //if (e.TEST1_DEFINITETIME1 == "Y")
            //    this.Test1DefiniteTime1Chk = true;
            //else
            //    this.Test1DefiniteTime1Chk = false;
            //if (e.TEST1_DEFINITETIME2 == "Y")
            //    this.Test1DefiniteTime2Chk = true;
            //else
            //    this.Test1DefiniteTime2Chk = false;
            //if (e.TEST1_DEFINITETIME3 == "Y")
            //    this.Test1DefiniteTime3Chk = true;
            //else
            //    this.Test1DefiniteTime3Chk = false;

            //if (e.TEST2_LOWCUTOFF == "Y")
            //    this.Test2LowCutOffChk = true;
            //else
            //    this.Test2LowCutOffChk = false;
            //if (e.TEST2_DEFINITETIME1 == "Y")
            //    this.Test2DefiniteTime1Chk = true;
            //else
            //    this.Test2DefiniteTime1Chk = false;
            //if (e.TEST2_DEFINITETIME2 == "Y")
            //    this.Test2DefiniteTime2Chk = true;
            //else
            //    this.Test2DefiniteTime2Chk = false;
            //if (e.TEST2_DEFINITETIME3 == "Y")
            //    this.Test2DefiniteTime3Chk = true;
            //else
            //    this.Test2DefiniteTime3Chk = false;

            //if (e.TEST3_LOWCUTOFF == "Y")
            //    this.Test3LowCutOffChk = true;
            //else
            //    this.Test3LowCutOffChk = false;
            //if (e.TEST3_DEFINITETIME1 == "Y")
            //    this.Test3DefiniteTime1Chk = true;
            //else
            //    this.Test3DefiniteTime1Chk = false;
            //if (e.TEST3_DEFINITETIME2 == "Y")
            //    this.Test3DefiniteTime2Chk = true;
            //else
            //    this.Test3DefiniteTime2Chk = false;
            //if (e.TEST3_DEFINITETIME3 == "Y")
            //    this.Test3DefiniteTime3Chk = true;
            //else
            //    this.Test3DefiniteTime3Chk = false;

            //if (this.HighCurCutOff == "Y")
            //    this.HighCurCutoffChk = true;
            //else
            //    this.HighCurCutoffChk = false;


        }

        public void PopulateModelFromHistoryEntity(SM_RECLOSER_TS_HIST e)
        {
            this.OperatingNumber = e.OPERATING_NUM;
            this.DeviceId = e.DEVICE_ID;
            this.PreparedBy = e.PREPARED_BY;
            this.DateModified = (e.DATE_MODIFIED.HasValue ? e.DATE_MODIFIED.Value.ToShortDateString() : "");
            this.EffectiveDate = (e.EFFECTIVE_DT.HasValue ? e.EFFECTIVE_DT.Value.ToShortDateString() : "");
            this.PeerReviewerDate = (e.PEER_REVIEW_DT.HasValue ? e.PEER_REVIEW_DT.Value.ToShortDateString() : "");
            this.PeerReviewer = e.PEER_REVIEW_BY;
            this.OkToBypass = e.OK_TO_BYPASS;
            this.BypassPlan = e.BYPASS_PLANS;

            //initial trip
            this.IniEmulatedDevice = e.INI_EMULATED_DEVICE;
            this.IniInverseSegment = e.INI_INVERSE_SEGMENT;
            this.IniSpeed = e.INI_SPEED;
            this.IniAmpereRating = e.INI_AMPERE_RATING;
            this.IniMinTripA = e.INI_MIN_TRIP_A;
            this.IniTimeMultiplier = e.INI_TIME_MULTIPLIER;
            this.IniResetType = e.INI_RESET_TYPE;
            this.IniResetTime = e.INI_RESET_TIME;
            this.IniLowcutoff = e.INI_LOWCUTOFF;
            this.IniLowcutoffCurrA = e.INI_LOWCUTOFF_CURR_A;
            this.IniDefiniteTime1 = e.INI_DEFINITETIME1;
            this.IniDefiniteTime1CurrA = e.INI_DEFINITETIME1_CURR_A;
            this.IniDefiniteTime1Time = e.INI_DEFINITETIME1_TIME;
            this.IniDefiniteTime2 = e.INI_DEFINITETIME2;
            this.IniDefiniteTime2CurrA = e.INI_DEFINITETIME2_CURR_A;
            this.IniDefiniteTime2Time = e.INI_DEFINITETIME2_TIME;
            this.IniOpenIntervalTime = e.INI_OPEN_INTERVAL_TIME;
            this.IniCoilRating = e.INI_COIL_RATING;
            this.IniCurrA = e.INI_CURR_A;
            this.IniTime = e.INI_TIME;
            this.IniDefiniteTime3 = e.INI_DEFINITETIME3;
            this.IniDefiniteTime3CurrA = e.INI_DEFINITETIME3_CURR_A;
            this.IniDefiniteTime3Time = e.INI_DEFINITETIME3_TIME;

            //test1
            this.Test1EmulatedDevice = e.TEST1_EMULATED_DEVICE;
            this.Test1InverseSegment = e.TEST1_INVERSE_SEGMENT;
            this.Test1Speed = e.TEST1_SPEED;
            this.Test1AmpereRating = e.TEST1_AMPERE_RATING;
            this.Test1MinTripA = e.TEST1_MIN_TRIP_A;
            this.Test1TimeMultiplier = e.TEST1_TIME_MULTIPLIER;
            this.Test1ResetType = e.TEST1_RESET_TYPE;
            this.Test1ResetTime = e.TEST1_RESET_TIME;
            this.Test1Lowcutoff = e.TEST1_LOWCUTOFF;
            this.Test1LowcutoffCurrA = e.TEST1_LOWCUTOFF_CURR_A;
            this.Test1DefiniteTime1 = e.TEST1_DEFINITETIME1;
            this.Test1DefiniteTime1CurrA = e.TEST1_DEFINITETIME1_CUR_A;
            this.Test1DefiniteTime1Time = e.TEST1_DEFINITETIME1_TIME;
            this.Test1DefiniteTime2 = e.TEST1_DEFINITETIME2;
            this.Test1DefiniteTime2CurrA = e.TEST1_DEFINITETIME2_CURR_A;
            this.Test1DefiniteTime2Time = e.TEST1_DEFINITETIME2_TIME;
            this.Test1OpenIntervalTime = e.TEST1_OPEN_INTERVAL_TIME;
            this.Test1CoilRating = e.TEST1_COIL_RATING;
            this.Test1CurrA = e.TEST1_CURR_A;
            this.Test1Time = e.TEST1_TIME;
            this.Test1DefiniteTime3 = e.TEST1_DEFINITETIME3;
            this.Test1DefiniteTime3CurrA = e.TEST1_DEFINITETIME3_CURR_A;
            this.Test1DefiniteTime3Time = e.TEST1_DEFINITETIME3_TIME;

            //test2
            this.Test2EmulatedDevice = e.TEST2_EMULATED_DEVICE;
            this.Test2InverseSegment = e.TEST2_INVERSE_SEGMENT;
            this.Test2Speed = e.TEST2_SPEED;
            this.Test2AmpereRating = e.TEST2_AMPERE_RATING;
            this.Test2MinTripA = e.TEST2_MIN_TRIP_A;
            this.Test2TimeMultiplier = e.TEST2_TIME_MULTIPLIER;
            this.Test2ResetType = e.TEST2_RESET_TYPE;
            this.Test2ResetTime = e.TEST2_RESET_TIME;
            this.Test2Lowcutoff = e.TEST2_LOWCUTOFF;
            this.Test2LowcutoffCurrA = e.TEST2_LOWCUTOFF_CURR_A;
            this.Test2DefiniteTime1 = e.TEST2_DEFINITETIME1;
            this.Test2DefiniteTime1CurrA = e.TEST2_DEFINITETIME1_CURR_A;
            this.Test2DefiniteTime1Time = e.TEST2_DEFINITETIME1_TIME;
            this.Test2DefiniteTime2 = e.TEST2_DEFINITETIME2;
            this.Test2DefiniteTime2CurrA = e.TEST2_DEFINITETIME2_CURR_A;
            this.Test2DefiniteTime2Time = e.TEST2_DEFINITETIME2_TIME;
            this.Test2OpenIntervalTime = e.TEST2_OPEN_INTERVAL_TIME;
            this.Test2CoilRating = e.TEST2_COIL_RATING;
            this.Test2CurrA = e.TEST2_CURR_A;
            this.Test2Time = e.TEST2_TIME;
            this.Test2DefiniteTime3 = e.TEST2_DEFINITETIME3;
            this.Test2DefiniteTime3CurrA = e.TEST2_DEFINITETIME3_CURR_A;
            this.Test2DefiniteTime3Time = e.TEST2_DEFINITETIME3_TIME;

            //test3
            this.Test3EmulatedDevice = e.TEST3_EMULATED_DEVICE;
            this.Test3InverseSegment = e.TEST3_INVERSE_SEGMENT;
            this.Test3EmulatedDevice = e.TEST3_EMULATED_DEVICE;
            this.Test3InverseSegment = e.TEST3_INVERSE_SEGMENT;
            this.Test3Speed = e.TEST3_SPEED;
            this.Test3AmpereRating = e.TEST3_AMPERE_RATING;
            this.Test3MinTripA = e.TEST3_MIN_TRIP_A;
            this.Test3TimeMultiplier = e.TEST3_TIME_MULTIPLIER;
            this.Test3ResetType = e.TEST3_RESET_TYPE;
            this.Test3ResetTime = e.TEST3_RESET_TIME;
            this.Test3Lowcutoff = e.TEST3_LOWCUTOFF;
            this.Test3LowcutoffCurrA = e.TEST3_LOWCUTOFF_CURR_A;
            this.Test3DefiniteTime1 = e.TEST3_DEFINITETIME1;
            this.Test3DefiniteTime1CurrA = e.TEST3_DEFINITETIME1_CURR_A;
            this.Test3DefiniteTime1Time = e.TEST3_DEFINITETIME1_TIME;
            this.Test3DefiniteTime2 = e.TEST3_DEFINITETIME2;
            this.Test3DefiniteTime2CurrA = e.TEST3_DEFINITETIME2_CURR_A;
            this.Test3DefiniteTime2Time = e.TEST3_DEFINITETIME2_TIME;
            this.Test3CoilRating = e.TEST3_COIL_RATING;
            this.Test3CurrA = e.TEST3_CURR_A;
            this.Test3Time = e.TEST3_TIME;
            this.Test3DefiniteTime3 = e.TEST3_DEFINITETIME3;
            this.Test3DefiniteTime3CurrA = e.TEST3_DEFINITETIME3_CURR_A;
            this.Test3DefiniteTime3Time = e.TEST3_DEFINITETIME3_TIME;

            this.SeqResetTime = e.SEQUENCE_RESET_TIME;
            this.HighCurCutOff = e.HIGH_CURRENT_CUTOFF;
            this.HighCurCutOffA = e.HIGH_CURRENT_CUTOFF_A;
            this.SectMode = e.SEC_MODE;
            this.SectModeCounts = e.SEC_MODE_COUNTS;
            this.SectModeResetTime = e.SEC_MODE_RESET_TIME;
            this.SectModeStartingCur = e.SEC_MODE_STARTING_CURRENT;

            //checkboxes
            if (this.IniLowcutoff == "Y")
                this.IniLowCutOffChk = true;
            else
            {
                this.IniLowCutOffChk = false;
                this.IniLowcutoff = "N";
            }
            if (this.IniDefiniteTime1 == "Y")
                this.IniDefiniteTime1Chk = true;
            else
            {
                this.IniDefiniteTime1Chk = false;
                this.IniDefiniteTime1 = "N";
            }
            if (this.IniDefiniteTime2 == "Y")
                this.IniDefiniteTime2Chk = true;
            else
            {
                this.IniDefiniteTime2Chk = false;
                this.IniDefiniteTime2 = "N";
            }
            if (this.IniDefiniteTime3 == "Y")
                this.IniDefiniteTime3Chk = true;
            else
            {
                this.IniDefiniteTime3Chk = false;
                this.IniDefiniteTime3 = "N";
            }

            if (this.Test1Lowcutoff == "Y")
                this.Test1LowCutOffChk = true;
            else
            {
                this.Test1LowCutOffChk = false;
                this.Test1Lowcutoff = "N";
            }
            if (this.Test1DefiniteTime1 == "Y")
                this.Test1DefiniteTime1Chk = true;
            else
            {
                this.Test1DefiniteTime1Chk = false;
                this.Test1DefiniteTime1 = "N";
            }
            if (this.Test1DefiniteTime2 == "Y")
                this.Test1DefiniteTime2Chk = true;
            else
            {
                this.Test1DefiniteTime2Chk = false;
                this.Test1DefiniteTime2 = "N";
            }
            if (this.Test1DefiniteTime3 == "Y")
                this.Test1DefiniteTime3Chk = true;
            else
            {
                this.Test1DefiniteTime3Chk = false;
                this.Test1DefiniteTime3 = "N";
            }

            if (this.Test2Lowcutoff == "Y")
                this.Test2LowCutOffChk = true;
            else
            {
                this.Test2LowCutOffChk = false;
                this.Test2Lowcutoff = "N";
            }
            if (this.Test2DefiniteTime1 == "Y")
                this.Test2DefiniteTime1Chk = true;
            else
            {
                this.Test2DefiniteTime1Chk = false;
                this.Test2DefiniteTime1 = "N"; ;
            }
            if (this.Test2DefiniteTime2 == "Y")
                this.Test2DefiniteTime2Chk = true;
            else
            {
                this.Test2DefiniteTime2Chk = false;
                this.Test2DefiniteTime2 = "N";
            }
            if (this.Test2DefiniteTime3 == "Y")
                this.Test2DefiniteTime3Chk = true;
            else
            {
                this.Test2DefiniteTime3Chk = false;
                this.Test2DefiniteTime3 = "N";
            }

            if (this.Test3Lowcutoff == "Y")
                this.Test3LowCutOffChk = true;
            else
            {
                this.Test3LowCutOffChk = false;
                this.Test3Lowcutoff = "N";
            }
            if (this.Test3DefiniteTime1 == "Y")
                this.Test3DefiniteTime1Chk = true;
            else
            {
                this.Test3DefiniteTime1Chk = false;
                this.Test3DefiniteTime1 = "N";
            }
            if (this.Test3DefiniteTime2 == "Y")
                this.Test3DefiniteTime2Chk = true;
            else
            {
                this.Test3DefiniteTime2Chk = false;
                this.Test3DefiniteTime2 = "N";
            }
            if (this.Test3DefiniteTime3 == "Y")
                this.Test3DefiniteTime3Chk = true;
            else
            {
                this.Test3DefiniteTime3Chk = false;
                this.Test3DefiniteTime3 = "N";
            }

            if (this.HighCurCutOff == "Y")
                this.HighCurCutoffChk = true;
            else
            {
                this.HighCurCutoffChk = false;
                this.HighCurCutOff = "N";
            }

        }
    }
}




