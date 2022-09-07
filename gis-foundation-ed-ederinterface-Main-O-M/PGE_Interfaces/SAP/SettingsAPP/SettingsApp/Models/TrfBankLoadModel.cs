using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SettingsApp.Common;
namespace SettingsApp.Models
{
    public class TrfBankLoadModel
    {
        public decimal RegulatorRowId { get; set; }
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD", "TRANSMISSN_BUS_NBR")]
        [Display(Name = "Transmission bus number : ")]
        public int? TransmissionBusNo { get; set; }
        [Display(Name = "Bank code : ")]
        public short? BankNo { get; set; }
        [Display(Name = "Distribution Fed Station")]
        public bool DistributionFedStation { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD", "POS_SEQ_REACT")]
        [Display(Name = "Positive Sequence Reactance :")]
        public decimal? PositiveSeqReactance { get; set; }
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD", "POS_SEQ_RESIST")]
        [Display(Name = "Positive Sequence Resistance :")]
        public decimal? PositiveSeqResistance { get; set; }
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD", "ZERO_SEQ_REACT")]
        [Display(Name = "Zero Sequence Reactance :")]
        public decimal? ZeroSeqReactance { get; set; }
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD", "ZERO_SEQ_RESIST")]
        [Display(Name = "Zero Sequence Resistance :")]
        public decimal? ZeroSeqResistance { get; set; }
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD", "SUMMER_KVA_CAP")]
        [Display(Name = "kVA Capability :")]
        public int? SummerKVACapability { get; set; }
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD", "SUMMER_BASE_FACTOR")]
        [Display(Name = "Base Power Factor :")]
        public decimal? SummerBaseFactor { get; set; }
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD", "WINTER_KVA_CAP")]
        [Display(Name = "kVA Capability :")]
        public int? WinterKVACapability { get; set; }
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD", "WINTER_BASE_FACTOR")]
        [Display(Name = "Base Power Factor :")]
        public decimal? WinterBaseFactor { get; set; }

        [Display(Name = "Total :")]
        public int TotalCustomersCount { get; set; }
        [Display(Name = "Dom :")]
        public int DomesticCustomersCount { get; set; }
        [Display(Name = "Com :")]
        public int CommercialCustomersCount { get; set; }
        [Display(Name = "Ind :")]
        public int IndustrialCustomersCount { get; set; }
        [Display(Name = "Agr :")]
        public int AgriculturalCustomersCount { get; set; }
        [Display(Name = "Other :")]
        public int OtherCustomersCount { get; set; }

        [Display(Name = "Date of peak")]
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD_HIST", "PEAKDATE")]
        public DateTime? NewLoadMonth { get; set; }
        [Display(Name = "Time of peak")]
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD_HIST", "PEAKTIME")]
        public string NewLoadTime { get; set; }
        [Display(Name = "Total KW load")]
        [SettingsValidatorAttribute("SUBTRF_BANK_LOAD_HIST", "TOTAL_KW_LOAD")]
        public int? NewLoadTotalKW { get; set; }
        public List<TrfBankPeakModel> RecentMonthsData { get; set; }
        public List<TrfBankPeakModel> HistoricalMonthsData { get; set; }

        //sequence - id,peakDate,peakTime,

        [Display(Name = "Last Updated (Summer) : ")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? SummerLastUpdated { get; set; }
        [Display(Name = "Last Updated (Winter) : ")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? WinterLastUpdated { get; set; }

        //public long[] MonthRowId { get; set; }
        //public DateTime[] DateOfPeak { get; set; }
        //public int[] DayOfPeak { get; set; }
        //public DateTime TimeOfPeak { get; set; }
        //public long TotalKWLoad { get; set; }

        public void PopulateModelFromEntity(SM_SUBTRF_BANK_LOAD e, List<SM_SUBTRF_BANK_LOAD_HIST> LoadHistoryList, bool IsFedStationChecked)
        {
            //this.BankNo = e.BANK_CD;
            this.TransmissionBusNo = e.TRANSMISSN_BUS_NBR;


            if (IsFedStationChecked)
                this.DistributionFedStation = true;
            else
                this.DistributionFedStation = false;

            this.PositiveSeqReactance = e.POS_SEQ_REACT;
            this.PositiveSeqResistance = e.POS_SEQ_RESIST;
            this.ZeroSeqReactance = e.ZERO_SEQ_REACT;
            this.ZeroSeqResistance = e.ZERO_SEQ_RESIST;

            this.SummerBaseFactor = e.SUMMER_BASE_FACTOR;
            this.SummerKVACapability = e.SUMMER_KVA_CAP;
            this.SummerLastUpdated = e.SUMMER_UPDT_DTM;

            this.WinterBaseFactor = e.WINTER_BASE_FACTOR;
            this.WinterKVACapability = e.WINTER_KVA_CAP;
            this.WinterLastUpdated = e.WINTER_UPDT_DTM;

            var Counter = 0;
            this.HistoricalMonthsData = new List<TrfBankPeakModel>();
            this.RecentMonthsData = new List<TrfBankPeakModel>();
            if (LoadHistoryList != null)
            {
                foreach (var HistoryRow in LoadHistoryList)
                {
                    string PeakTime = null;

                    if (HistoryRow.PEAK_DTM != null)
                    {
                        if (HistoryRow.PEAK_DTM.HasValue)
                        {
                            PeakTime = HistoryRow.PEAK_DTM.Value.ToString("HHmm");
                        }
                    }
                    TrfBankPeakModel Load = new TrfBankPeakModel();
                    Load.Id = HistoryRow.ID;
                    Load.PeakDate = HistoryRow.PEAK_DTM;
                    Load.PeakTime = PeakTime;
                    Load.TotalKWLoad = HistoryRow.TOTAL_KW_LOAD;
                    if (Counter > 11)
                    {
                        this.HistoricalMonthsData.Add(Load);
                    }
                    else
                    {
                        this.RecentMonthsData.Add(Load);
                    }
                    Counter++;
                }
            }

        }

        public void PopulateEntityFromModel(SM_SUBTRF_BANK_LOAD e)
        {
            //e.BANK_CD = this.BankNo;
            e.REF_DEVICE_ID = this.RegulatorRowId;
            e.TRANSMISSN_BUS_NBR = this.TransmissionBusNo;
            e.POS_SEQ_REACT = this.PositiveSeqReactance;
            e.POS_SEQ_RESIST = this.PositiveSeqResistance;
            e.ZERO_SEQ_REACT = this.ZeroSeqReactance;
            e.ZERO_SEQ_RESIST = this.ZeroSeqResistance;
            e.SUMMER_BASE_FACTOR = this.SummerBaseFactor;
            e.SUMMER_KVA_CAP = this.SummerKVACapability;
            e.SUMMER_UPDT_DTM = DateTime.Now;
            e.WINTER_BASE_FACTOR = this.WinterBaseFactor;
            e.WINTER_KVA_CAP = this.WinterKVACapability;
            e.WINTER_UPDT_DTM = DateTime.Now;
        }
        public void PopulateEntityFromModel(SM_SUBTRF_BANK_LOAD e, IQueryable<SM_SUBTRF_BANK_LOAD_HIST> LoadHistoryList)
        {
            //e.BANK_CD = this.BankNo;
            e.TRANSMISSN_BUS_NBR = this.TransmissionBusNo;
            e.POS_SEQ_REACT = this.PositiveSeqReactance;
            e.POS_SEQ_RESIST = this.PositiveSeqResistance;
            e.ZERO_SEQ_REACT = this.ZeroSeqReactance;
            e.ZERO_SEQ_RESIST = this.ZeroSeqResistance;

            if (e.SUMMER_BASE_FACTOR != this.SummerBaseFactor || e.SUMMER_KVA_CAP != this.SummerKVACapability)
            {
                e.SUMMER_UPDT_DTM = DateTime.Now;
            }
            e.SUMMER_BASE_FACTOR = this.SummerBaseFactor;
            e.SUMMER_KVA_CAP = this.SummerKVACapability;

            if (e.WINTER_BASE_FACTOR != this.WinterBaseFactor || e.WINTER_KVA_CAP != this.WinterKVACapability)
            {
                e.WINTER_UPDT_DTM = DateTime.Now;
            }
            e.WINTER_BASE_FACTOR = this.WinterBaseFactor;
            e.WINTER_KVA_CAP = this.WinterKVACapability;

            //e.WINTER_UPDT_DTM = this.WinterLastUpdated;
            List<TrfBankPeakModel> AllLoadHistoryList = new List<TrfBankPeakModel>();
            if(this.RecentMonthsData!=null && this.RecentMonthsData.Count>0)
                AllLoadHistoryList.AddRange(this.RecentMonthsData);
            if (this.HistoricalMonthsData != null && this.HistoricalMonthsData.Count > 0)
                AllLoadHistoryList.AddRange(this.HistoricalMonthsData);
            foreach (var row in AllLoadHistoryList)
            {
                var ValidPeakDateTime = GetValidPeakDateTime(row.PeakDate, row.PeakTime);
                var eh = LoadHistoryList.Where(h => h.ID == row.Id).FirstOrDefault();
                if (eh != null)
                {
                    if (CheckIfMonthlyDataChanged(ValidPeakDateTime, row.TotalKWLoad, eh.PEAK_DTM, eh.TOTAL_KW_LOAD))
                    {
                        var WinterMonths = new int[] { 1, 2, 3, 11, 12 };
                        if (ValidPeakDateTime != null)
                        {
                            if (WinterMonths.Contains(ValidPeakDateTime.Value.Month))
                            {
                                e.WINTER_UPDT_DTM = DateTime.Now;
                            }
                            else
                            {
                                e.SUMMER_UPDT_DTM = DateTime.Now;
                            }
                        }
                    }
                    eh.PEAK_DTM = ValidPeakDateTime;
                    eh.TOTAL_KW_LOAD = row.TotalKWLoad;
                }
            }
        }


        public bool CheckIfMonthlyDataChanged(DateTime? NewPeakDateTime, int? NewTotalKWLoad, DateTime? OldPeakDateTime, int? OldTotalKWLoad)
        {
            bool IsChanged = false;

            if (NewPeakDateTime != OldPeakDateTime || NewTotalKWLoad != OldTotalKWLoad)
            {
                IsChanged = true;
            }
            return IsChanged;

        }

        public DateTime? GetValidPeakDateTime(DateTime? PeakDate, string PeakTime)
        {

            var NewValidPeakDateTime = new DateTime?();
            NewValidPeakDateTime = null;
            if (string.IsNullOrEmpty(PeakTime))
                PeakTime = "0000";
            if (PeakDate != null && PeakTime != null)
            {
                var ActualPeakDate = PeakDate.Value;
                if (PeakTime.Length == 4)
                {
                    int hour = int.Parse(PeakTime.Substring(0, 2));
                    int min = int.Parse(PeakTime.Substring(2, 2));
                    if (hour >= 0 && hour <= 23 && min >= 0 && min <= 59)
                    {
                        PeakDate = NewValidPeakDateTime = new DateTime(ActualPeakDate.Year, ActualPeakDate.Month, ActualPeakDate.Day, hour, min, 00);
                    }
                }
            }
            return NewValidPeakDateTime;
        }

        public bool ValidateIfNewPeakDateExists()
        {
            var IsPeakDateNotExists = true;
            List<TrfBankPeakModel> AllLoadHistoryList = new List<TrfBankPeakModel>();
            if (this.RecentMonthsData != null && this.RecentMonthsData.Count > 0)
                AllLoadHistoryList.AddRange(this.RecentMonthsData);
            if (this.HistoricalMonthsData != null && this.HistoricalMonthsData.Count > 0)
                AllLoadHistoryList.AddRange(this.HistoricalMonthsData);
            
            if (AllLoadHistoryList != null)
            {
                var PeakDates = AllLoadHistoryList.Select(t => t.PeakDate.Value.ToString("MM/yyyy"));
                
                if (PeakDates == null)
                {
                    IsPeakDateNotExists = true;
                }
                else if (this.NewLoadMonth==null || PeakDates.Contains(this.NewLoadMonth.Value.ToString("MM/yyyy")))
                {
                    IsPeakDateNotExists = false;
                }
            }
            return IsPeakDateNotExists;
        }

        public string ValidateNewPeakLoadInfo()
        {
            var ErrorMessage = string.Empty;
            if (string.IsNullOrEmpty(this.NewLoadTime))
            {
                this.NewLoadTime = "0000";
            }
            if (this.NewLoadMonth == null)
            {
                ErrorMessage = "Peak date/time can't be null!";
            }
            else
            {
                if (!ValidateIfNewPeakDateExists())
                {
                    ErrorMessage = "Peak date already exists!";
                }
                else
                {
                    if (GetValidPeakDateTime(this.NewLoadMonth, this.NewLoadTime) == null)
                    {
                        ErrorMessage = "Invalid peak date/time!";
                    }
                }
            }
            return ErrorMessage;
        }

        public string ValidateRecentPeakLoadInfo()
        {
            var ErrorMessage = string.Empty;

            List<TrfBankPeakModel> AllLoadHistoryList = new List<TrfBankPeakModel>();
            if (this.RecentMonthsData != null && this.RecentMonthsData.Count > 0)
                AllLoadHistoryList.AddRange(this.RecentMonthsData);
            if (this.HistoricalMonthsData != null && this.HistoricalMonthsData.Count > 0)
                AllLoadHistoryList.AddRange(this.HistoricalMonthsData);
            
            var RecentMonthsLoadList = this.RecentMonthsData;

            if (RecentMonthsLoadList != null && AllLoadHistoryList!=null)
            {
                var AllDates = AllLoadHistoryList.Select(t => (t.PeakDate!=null?t.PeakDate.Value.ToString("MM/yyyy"):string.Empty));
                foreach (var LoadInfo in RecentMonthsLoadList)
                {
                    if (LoadInfo.PeakDate != null)
                    {
                        var PeakDateString = LoadInfo.PeakDate.Value.ToShortDateString();
                        DateTime ValidPeakDate;
                        if (DateTime.TryParse(PeakDateString, out ValidPeakDate))
                        {

                            var SelectedDates = AllDates.Where(t => t == LoadInfo.PeakDate.Value.ToString("MM/yyyy"));
                            if (SelectedDates != null)
                            {
                                if (SelectedDates.Count() > 1)
                                {
                                    ErrorMessage = "Duplicate peak dates found!";
                                }
                            }
                        }
                        else
                        {
                            ErrorMessage = "Invalid peak date(s)!";
                        }
                    }
                    else
                    {
                        ErrorMessage = "Peak date(s) can't be empty!";
                    }
                }
            }

            return ErrorMessage;
        }

    }
}