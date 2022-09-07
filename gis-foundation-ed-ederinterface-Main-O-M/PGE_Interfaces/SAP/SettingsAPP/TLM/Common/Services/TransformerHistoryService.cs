using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TLM.EntityFramework;
using System.Data.Objects;
using TLM.Models;
using System.Globalization;
using System.Data.EntityClient;
using System.Text;

namespace TLM.Common.Services
{
    public class TransformerHistoryService
    {
        #region private members
        private string GlobalId { get; set; }
        public decimal TransformerId { get; set; }
        private int SelectedMonth { get; set; }
        public DateTime? DataStartDate { get; set; }
        private DateTime FromDate { get; set; }
        private DateTime ToDate { get; set; }
        private DateTime? TrfPeakTime { get; set; }
        private decimal? TrfPeakKVA { get; set; }
        private DateTime LastDate { get; set; }
        public long TransformerCGC { get; set; }
        public DateTime? EarliestDataStartDate { get; set; }
        public DateTime? LatestDataStartDate { get; set; }
        public bool IsAllMetersLegacy { get; set; }
        #endregion
        public StringBuilder InfoMessages = new StringBuilder();
        #region public constructor
        public TransformerHistoryService(string GlobalId, DateTime FromDate, DateTime ToDate, int SelectedMonth, DateTime DataStartDate)
        {
            this.GlobalId = GlobalId;
            this.FromDate = FromDate;
            this.ToDate = ToDate;
            this.SelectedMonth = SelectedMonth;
            this.DataStartDate = DataStartDate;
            this.InfoMessages.Clear();
            this.TrfPeakKVA = null;
            this.TrfPeakTime = null;
            this.IsAllMetersLegacy = true;
            GetAndSetTransformerId();
            InfoMessages.Clear();
            
        }
        public TransformerHistoryService(string GlobalId)
        {
            this.GlobalId = GlobalId;
            this.IsAllMetersLegacy = true;
            GetAndSetTransformerId();
        }
        public TransformerHistoryService(decimal TransformerId)
        {
            this.TransformerId = TransformerId;
        }
        #endregion
        #region public methods
        public TransformerLoadHistory GetTransformerHistory()
        {
            var IsEstimated = string.Empty;
            GetDataStartDate();
            var MeterRatiosInfoList = GetMeterRatiosInfo();
            var TrfSPInfoList = GetServicePointInfo(out IsEstimated);
            var TrfPeakHistoryModel = PopulateTrfPeakHistoryModel(MeterRatiosInfoList, TrfSPInfoList, IsEstimated);
            return TrfPeakHistoryModel;
        }
        public TransformerLoadHistory GetTransformerHistoryGeneration()
        {
            var IsEstimated = string.Empty;
            GetDataStartDateGeneration();
            var MeterRatiosInfoList = GetMeterRatiosInfoGeneration();
            var TrfSPInfoList = GetServicePointInfoGeneration(out IsEstimated);
            var TrfPeakHistoryModel = PopulateTrfPeakHistoryModel(MeterRatiosInfoList, TrfSPInfoList, IsEstimated);
            return TrfPeakHistoryModel;
        }
        public void GetDataStartDate()
        {
            
            using (var Context = new TLMEntities())
            {
                var data = Context.TRF_PEAK_HIST.Where(t => t.TRF_ID == TransformerId).GroupBy(g => g.TRF_ID).Select(grp => new { MinDate = grp.Min(d => d.BATCH_DATE), MaxDate = grp.Max(d => d.BATCH_DATE) } );
                if(data.Any())
                {
                    if (data.Count() > 0)
                    {
                        this.DataStartDate = data.First().MinDate;
                        this.EarliestDataStartDate = data.First().MinDate;
                        this.LatestDataStartDate = data.First().MaxDate;
                    }
                }
            }
            if (this.DataStartDate == null)
            {
                InfoMessages.AppendLine(string.Format("No Data Available for selected Transformer!"));
            }
        }

        public void GetDataStartDateGeneration()
        {
            using (var Context = new TLMEntities())
            {
                var data = Context.TRF_PEAK_GEN_HIST.Where(t => t.TRF_ID == TransformerId).GroupBy(g => g.TRF_ID).Select(grp => new { MinDate = grp.Min(d => d.BATCH_DATE), MaxDate = grp.Max(d => d.BATCH_DATE) });
                if (data.Any())
                {
                    if (data.Count() > 0)
                    {
                        this.DataStartDate = data.First().MinDate;
                        this.EarliestDataStartDate = data.First().MinDate;
                        this.LatestDataStartDate = data.First().MaxDate;
                    }
                }
            }
            if (this.DataStartDate == null)
            {
                InfoMessages.AppendLine(string.Format("Unable to find data start date for Transformer!"));
            }
        }

        public void GetAndSetTransformerId()
        {
            using (var context = new TLMEntities())
            {
                var transformer = context.TRANSFORMERs.Where(t => t.GLOBAL_ID == GlobalId).FirstOrDefault();
                if (transformer != null)
                {
                    TransformerId = transformer.ID;
                    TransformerCGC = transformer.CGC_ID;
                }
                TransformerId = context.TRANSFORMERs.Where(t => t.GLOBAL_ID == GlobalId).Select(y => y.ID).FirstOrDefault();
            }

        }
        #endregion
        #region private methods

        private List<Tuple<string, string, string>> GetMeterRatiosInfo()
        {

            var MeterRatiosList = new List<Tuple<string, string, string>>();
            using (var Context = new TLMEntities())
            {
                var MeterRatiosInfo = (Context.TRF_PEAK_HIST.Where(x => x.BATCH_DATE >= FromDate && x.BATCH_DATE <= ToDate && x.TRF_ID == TransformerId))
                    .Select(y => new { y.TRF_PEAK_KVA, y.TRF_PEAK_TIME, y.BATCH_DATE, y.SM_CUST_TOTAL, y.CCB_CUST_TOTAL }).ToList()
                                            .Select(t => new
                                            {
                                                MeterReadingMonth = t.BATCH_DATE.ToString("MMMyy"),
                                                MeterRatioValue = ((t.SM_CUST_TOTAL == null ? string.Empty : t.SM_CUST_TOTAL.ToString()) + "/" + (t.CCB_CUST_TOTAL == null ? string.Empty : t.CCB_CUST_TOTAL.ToString())),
                                                //MeterRatioValue = t.SM_CUST_TOTAL.ToString() + "/" + t.CCB_CUST_TOTAL.ToString(),
                                                ColorCode = t.BATCH_DATE.Month <= 3 || t.BATCH_DATE.Month >= 11 ? "WinterCode" : "SummerCode",
                                                TrfPeakKVA = t.TRF_PEAK_KVA,
                                                TrfPeakTime = t.TRF_PEAK_TIME,
                                                Batch_Date = t.BATCH_DATE
                                            });

                if (MeterRatiosInfo == null)
                {
                   // InfoMessages.AppendLine(string.Format("Unable to find meter ratios information for Transformer!"));
                    InfoMessages.AppendLine(string.Format("No data available for meter ratios in Transformer for selected date!"));
                    return null;
                }
                else if (MeterRatiosInfo.Count() <= 0)
                {
                  //  InfoMessages.AppendLine(string.Format("Unable to find meter ratios information for Transformer!"));
                    InfoMessages.AppendLine(string.Format("No data available for meter ratios in Transformer for selected date!!"));
                }
                else if (MeterRatiosInfo.Count() > 0)
                    LastDate = MeterRatiosInfo.OrderByDescending(t => t.Batch_Date).First().Batch_Date;
                foreach (var MeterRatio in MeterRatiosInfo.OrderBy(t => t.Batch_Date))
                {
                    var MeterRationItem = new Tuple<string, string, string>(MeterRatio.MeterReadingMonth,
                        MeterRatio.MeterRatioValue, MeterRatio.ColorCode);
                    MeterRatiosList.Add(MeterRationItem);
                }
                var TrfPeakKVAAndTime = MeterRatiosInfo.OrderByDescending(x =>x.TrfPeakKVA).ThenByDescending(x=>x.Batch_Date).Select(y => new { y.TrfPeakKVA, y.TrfPeakTime }).FirstOrDefault();
                if (TrfPeakKVAAndTime != null)
                {
                    TrfPeakKVA = TrfPeakKVAAndTime.TrfPeakKVA;
                    TrfPeakTime = TrfPeakKVAAndTime.TrfPeakTime;
                }
            }
            return MeterRatiosList;
        }

        private List<Tuple<string, string, string>> GetMeterRatiosInfoGeneration()
        {

            var MeterRatiosList = new List<Tuple<string, string, string>>();
            using (var Context = new TLMEntities())
            {
                var MeterRatiosInfo = (Context.TRF_PEAK_GEN_HIST.Where(x => x.BATCH_DATE >= FromDate && x.BATCH_DATE <= ToDate && x.TRF_ID == TransformerId))
                    .Select(y => new { y.TRF_PEAK_KVA, y.TRF_PEAK_TIME, y.BATCH_DATE, y.SM_CUST_TOTAL, y.CCB_CUST_TOTAL }).ToList()
                                            .Select(t => new
                                            {
                                                MeterReadingMonth = t.BATCH_DATE.ToString("MMMyy"),
                                                MeterRatioValue = ((t.SM_CUST_TOTAL == null ? string.Empty : t.SM_CUST_TOTAL.ToString()) + "/" + (t.CCB_CUST_TOTAL == null ? string.Empty : t.CCB_CUST_TOTAL.ToString())),
                                                //MeterRatioValue = t.SM_CUST_TOTAL.ToString() + "/" + t.CCB_CUST_TOTAL.ToString(),
                                                ColorCode = t.BATCH_DATE.Month <= 3 || t.BATCH_DATE.Month >= 11 ? "WinterCode" : "SummerCode",
                                                TrfPeakKVA = t.TRF_PEAK_KVA,
                                                TrfPeakTime = t.TRF_PEAK_TIME,
                                                Batch_Date = t.BATCH_DATE
                                            });

                if (MeterRatiosInfo == null)
                {
                    InfoMessages.AppendLine(string.Format("Unable to find meter ratios information for Transformer!"));
                    return null;
                }
                else if (MeterRatiosInfo.Count() <= 0)
                {
                    InfoMessages.AppendLine(string.Format("Unable to find meter ratios information for Transformer!"));
                }
                else if (MeterRatiosInfo.Count() > 0)
                    LastDate = MeterRatiosInfo.OrderByDescending(t => t.Batch_Date).First().Batch_Date;

                foreach (var MeterRatio in MeterRatiosInfo.OrderBy(t => t.Batch_Date))
                {
                    var MeterRationItem = new Tuple<string, string, string>(MeterRatio.MeterReadingMonth,
                        MeterRatio.MeterRatioValue, MeterRatio.ColorCode);
                    MeterRatiosList.Add(MeterRationItem);
                }
                var TrfPeakKVAAndTime = MeterRatiosInfo.OrderByDescending(x => x.TrfPeakKVA).Select(y => new { y.TrfPeakKVA, y.TrfPeakTime }).FirstOrDefault();
                if (TrfPeakKVAAndTime != null)
                {
                    TrfPeakKVA = TrfPeakKVAAndTime.TrfPeakKVA;
                    TrfPeakTime = TrfPeakKVAAndTime.TrfPeakTime;
                }
            }
            return MeterRatiosList;
        }

        private List<TransformerServicePointInfo> GetServicePointInfo(out string IsEstimated)
        {
            var TrfSPInfoList = new List<TransformerServicePointInfo>();
            IsEstimated = string.Empty;

            using (var Context = new TLMEntities())
            {
                var SPHistoryMeterInfo1 = (from sp in Context.SP_PEAK_HIST
                                          join trf in Context.TRF_PEAK_HIST on sp.TRF_PEAK_HIST_ID equals trf.ID
                                          join met in Context.METERs on sp.SERVICE_POINT_ID equals met.SERVICE_POINT_ID
                                          where trf.BATCH_DATE >= FromDate
                                                  && trf.BATCH_DATE <= ToDate
                                                  && trf.TRF_ID == TransformerId
                                          orderby met.SVC_ST_NAME,met.SVC_ST_NUM ascending
                                          select new TransformerServicePointInfo
                                          {
                                              ServicePointId = sp.SERVICE_POINT_ID,
                                              IsConnected = null,
                                              IsSmartMeter = sp.SM_FLG,
                                              Interval = sp.INT_LEN,
                                              KVATrfPeak = sp.SP_KVA_TRF_PEAK,
                                              PeakKVA = sp.SP_PEAK_KVA,
                                              PeakTimestamp = sp.SP_PEAK_TIME,
                                              MeterNumber = met.METER_NUMBER,
                                              TransformerHistoryId = sp.TRF_PEAK_HIST_ID,
                                              ServicePointRowNumber = sp.ID,
                                              IsEstimated = sp.VEE_TRF_KW_FLG,
                                              StNo=met.SVC_ST_NUM,
                                              StName = met.SVC_ST_NAME,
                                              CustomerType = sp.CUST_TYP
                                          }).ToList();

                var SPHistoryMeterInfo = (from x in SPHistoryMeterInfo1
                                          
                                          select new TransformerServicePointInfo
                                          {
                                              ServicePointId = x.ServicePointId,
                                              IsConnected = null,
                                              IsSmartMeter =x.IsSmartMeter,
                                              Interval = x.Interval,
                                              KVATrfPeak = x.KVATrfPeak,
                                              PeakKVA = x.PeakKVA,
                                              PeakTimestamp = x.PeakTimestamp,
                                              MeterNumber = x.MeterNumber,
                                              TransformerHistoryId = x.TransformerHistoryId,
                                              ServicePointRowNumber = x.ServicePointRowNumber,
                                              IsEstimated = x.IsEstimated,
                                              Address = x.StNo + " " + x.StName,
                                              CustomerType = x.CustomerType
                                          }).ToList();
                if (SPHistoryMeterInfo == null)
                {
                   // InfoMessages.AppendLine(string.Format("Unable to find service points' information for Transformer!"));
                    InfoMessages.AppendLine(string.Format("No data available for service points in Transformer for selected date!"));
                    return null;
                }
                else if (SPHistoryMeterInfo.Count <= 0)
                {
                    //InfoMessages.AppendLine(string.Format("Unable to find service points' information for Transformer!"));
                    InfoMessages.AppendLine(string.Format("No data available for service points in Transformer for selected date!"));
                }

                var MaxKVABySPInfo = SPHistoryMeterInfo.GroupBy(x => new { x.ServicePointId })
                    .Select(t => new { PeakKVA = t.Max(s => s.PeakKVA), ServicePointId = t.Key.ServicePointId }).ToList();

                if (SPHistoryMeterInfo.Where(sp => sp.IsEstimated == "E").Count() >= 1)
                    IsEstimated = "E";

                

                foreach (var MaxKVABySPItem in MaxKVABySPInfo)
                {
                    var SPIsEstimated = string.Empty;

                    var EstimatedData = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == MaxKVABySPItem.ServicePointId && sp.IsEstimated == "E");
                    if (EstimatedData.Any())
                    {
                        SPIsEstimated = "E";
                    }
                    TrfSPInfoList.Add(SPHistoryMeterInfo.Where(sp => sp.ServicePointId == MaxKVABySPItem.ServicePointId && sp.PeakKVA == MaxKVABySPItem.PeakKVA).OrderByDescending(s => s.PeakTimestamp).FirstOrDefault());
                    var obj = TrfSPInfoList.FirstOrDefault(x => x.ServicePointId ==MaxKVABySPItem.ServicePointId);
                    if (obj != null) obj.IsEstimated = SPIsEstimated;
                }

                int DisplayOrder = 1;
                var TrfPeakTimeSt = TrfPeakTime==null?null:Convert.ToDateTime(TrfPeakTime).ToString("MM/yyyy");
                foreach (var TrfSPInfoItem in TrfSPInfoList)
                {
                    TrfSPInfoItem.DisplayOrder = DisplayOrder++;
                    var SMFlagOfLastAvailList = SPHistoryMeterInfo.Where(t => t.ServicePointId == TrfSPInfoItem.ServicePointId).OrderByDescending(p => p.PeakTimestamp).Select(sp => new { sp.IsSmartMeter,sp.Interval });

                    var SMFlagOfRecentList = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == TrfSPInfoItem.ServicePointId && sp.PeakTimestamp.ToString("MM/yyyy") == LastDate.ToString("MM/yyyy")).Select(t => t.IsSmartMeter);
                    var KVATrfPeakList = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == TrfSPInfoItem.ServicePointId && sp.PeakTimestamp.ToString("MM/yyyy") == TrfPeakTimeSt).Select(t => t.KVATrfPeak);
                    var ConnectedFlagList = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == TrfSPInfoItem.ServicePointId && sp.PeakTimestamp.ToString("MM/yyyy") == ToDate.ToString("MM/yyyy"));

                    if(KVATrfPeakList!=null)
                    {
                        TrfSPInfoItem.KVATrfPeak = KVATrfPeakList.FirstOrDefault();
                    }
                    if(ConnectedFlagList!=null)
                    {
                        TrfSPInfoItem.IsConnected = ConnectedFlagList.Count() >= 1 ? "Y" : "N";
                    }
                    if(SMFlagOfRecentList!=null && SMFlagOfRecentList.Count()>=1)
                    {
                        TrfSPInfoItem.IsSmartMeter = SMFlagOfRecentList.First() == "S" ? "Y" : "N";
                    }
                    else
                    {
                        if (SMFlagOfLastAvailList != null)
                        {
                            TrfSPInfoItem.IsSmartMeter = SMFlagOfLastAvailList.FirstOrDefault().IsSmartMeter== "S" ? "Y" : "N";
                        }
                    }
                    if (TrfSPInfoItem.IsSmartMeter == "Y")
                    {
                        this.IsAllMetersLegacy = false;
                        if(TrfSPInfoItem.Interval==null && SMFlagOfLastAvailList!=null)
                        {
                            TrfSPInfoItem.Interval = SMFlagOfLastAvailList.FirstOrDefault().Interval;
                        }
                    }
                 }
            }
            return TrfSPInfoList;
        }



        private List<TransformerServicePointInfo> GetServicePointInfoGeneration(out string IsEstimated)
        {

            var TrfSPInfoList = new List<TransformerServicePointInfo>();
            IsEstimated = string.Empty;

            using (var Context = new TLMEntities())
            {
                var SPHistoryMeterInfo = (from sp in Context.SP_PEAK_GEN_HIST
                                          join trf in Context.TRF_PEAK_GEN_HIST on sp.TRF_PEAK_GEN_HIST_ID equals trf.ID
                                          join met in Context.METERs on sp.SERVICE_POINT_ID equals met.SERVICE_POINT_ID
                                          where trf.BATCH_DATE >= FromDate
                                                  && trf.BATCH_DATE <= ToDate
                                                  && trf.TRF_ID == TransformerId
                                          orderby met.SVC_ST_NAME, met.SVC_ST_NUM ascending
                                          select new TransformerServicePointInfo
                                          {
                                              ServicePointId = sp.SERVICE_POINT_ID,
                                              IsConnected = null,
                                              IsSmartMeter = sp.SM_FLG,
                                              Interval = sp.INT_LEN,
                                              KVATrfPeak = sp.SP_KVA_TRF_PEAK,
                                              PeakKVA = sp.SP_PEAK_KVA,
                                              PeakTimestamp = sp.SP_PEAK_TIME,
                                              MeterNumber = met.METER_NUMBER,
                                              TransformerHistoryId = sp.TRF_PEAK_GEN_HIST_ID,
                                              ServicePointRowNumber = sp.ID,
                                              IsEstimated = sp.VEE_TRF_KW_FLG,
                                              Address = met.SVC_ST_NUM + " " + met.SVC_ST_NAME,
                                              CustomerType = sp.CUST_TYP
                                          }).ToList();


                if (SPHistoryMeterInfo == null)
                {
                   // InfoMessages.AppendLine(string.Format("Unable to find service points' information for Transformer!"));
                    InfoMessages.AppendLine(string.Format("No data available for service points in Transformer for selected date!"));
                    return null;
                }
                else if (SPHistoryMeterInfo.Count <= 0)
                {
                    //InfoMessages.AppendLine(string.Format("Unable to find service points' information for Transformer!"));
                    InfoMessages.AppendLine(string.Format("No data available for service points in Transformer for selected date!"));
                }
                var MaxKVABySPInfo = SPHistoryMeterInfo.GroupBy(x => new { x.ServicePointId })
                    .Select(t => new { PeakKVA = t.Max(s => s.PeakKVA), ServicePointId = t.Key.ServicePointId }).ToList();

                if (SPHistoryMeterInfo.Where(sp => sp.IsEstimated == "E").Count() >= 1)
                    IsEstimated = "E";


                foreach (var MaxKVABySPItem in MaxKVABySPInfo)
                {
                    var SPIsEstimated = string.Empty;

                    var EstimatedData = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == MaxKVABySPItem.ServicePointId && sp.IsEstimated == "E");
                    if (EstimatedData.Any())
                    {
                        SPIsEstimated = "E";
                    }
                    TrfSPInfoList.Add(SPHistoryMeterInfo.Where(sp => sp.ServicePointId == MaxKVABySPItem.ServicePointId && sp.PeakKVA == MaxKVABySPItem.PeakKVA).OrderByDescending(s => s.PeakTimestamp).FirstOrDefault());
                    var obj = TrfSPInfoList.FirstOrDefault(x => x.ServicePointId == MaxKVABySPItem.ServicePointId);
                    if (obj != null) obj.IsEstimated = SPIsEstimated;
                }


                int DisplayOrder = 1;
                var TrfPeakTimeSt = TrfPeakTime == null ? null : Convert.ToDateTime(TrfPeakTime).ToString("MM/yyyy");
                foreach (var TrfSPInfoItem in TrfSPInfoList)
                {
                    TrfSPInfoItem.DisplayOrder = DisplayOrder++;
                    var SMFlagOfLastAvailList = SPHistoryMeterInfo.Where(t => t.ServicePointId == TrfSPInfoItem.ServicePointId).OrderByDescending(p => p.PeakTimestamp).Select(sp => sp.IsSmartMeter);

                    var SMFlagOfRecentList = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == TrfSPInfoItem.ServicePointId && sp.PeakTimestamp.ToString("MM/yyyy") == LastDate.ToString("MM/yyyy")).Select(t => t.IsSmartMeter);
                    var KVATrfPeakList = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == TrfSPInfoItem.ServicePointId && sp.PeakTimestamp.ToString("MM/yyyy") == TrfPeakTimeSt).Select(t => t.KVATrfPeak);
                    var ConnectedFlagList = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == TrfSPInfoItem.ServicePointId && sp.PeakTimestamp.ToString("MM/yyyy") == ToDate.ToString("MM/yyyy"));

                    if (KVATrfPeakList.Any())
                    {
                        TrfSPInfoItem.KVATrfPeak = KVATrfPeakList.First();
                    }
                    if (ConnectedFlagList.Any())
                    {
                        TrfSPInfoItem.IsConnected = ConnectedFlagList.Count() >= 1 ? "Y" : "N";
                    }
                    if (SMFlagOfRecentList.Any())
                    {
                        TrfSPInfoItem.IsSmartMeter = SMFlagOfRecentList.First() == "S" ? "Y" : "N";
                        if (TrfSPInfoItem.IsSmartMeter == "Y")
                        {

                        }
                    }
                    else
                    {
                        if (SMFlagOfLastAvailList.Any())
                        {
                            TrfSPInfoItem.IsSmartMeter = SMFlagOfLastAvailList.First() == "S" ? "Y" : "N";
                        }
                    }
                    if (TrfSPInfoItem.IsSmartMeter == "Y")
                    {
                        this.IsAllMetersLegacy = false;
                    }

                }

            }
            return TrfSPInfoList;
        }
        //private List<TransformerServicePointInfo> GetServicePointInfoGeneration(out string IsEstimated)
        //{

        //    var TrfSPInfoList = new List<TransformerServicePointInfo>();
        //    IsEstimated = string.Empty;

        //    using (var Context = new TLMEntities())
        //    {
        //        var SPHistoryMeterInfo1 = (from sp in Context.SP_PEAK_HIST
        //                                   join trf in Context.TRF_PEAK_HIST on sp.TRF_PEAK_HIST_ID equals trf.ID
        //                                   join met in Context.METERs on sp.SERVICE_POINT_ID equals met.SERVICE_POINT_ID
        //                                   where trf.BATCH_DATE >= FromDate
        //                                           && trf.BATCH_DATE <= ToDate
        //                                           && trf.TRF_ID == TransformerId
        //                                   orderby met.SVC_ST_NAME, met.SVC_ST_NUM ascending
        //                                   select new TransformerServicePointInfo
        //                                   {
        //                                       ServicePointId = sp.SERVICE_POINT_ID,
        //                                       IsConnected = null,
        //                                       IsSmartMeter = sp.SM_FLG,
        //                                       Interval = sp.INT_LEN,
        //                                       KVATrfPeak = sp.SP_KVA_TRF_PEAK,
        //                                       PeakKVA = sp.SP_PEAK_KVA,
        //                                       PeakTimestamp = sp.SP_PEAK_TIME,
        //                                       MeterNumber = met.METER_NUMBER,
        //                                       TransformerHistoryId = sp.TRF_PEAK_HIST_ID,
        //                                       ServicePointRowNumber = sp.ID,
        //                                       IsEstimated = sp.VEE_TRF_KW_FLG,
        //                                       StNo = met.SVC_ST_NUM,
        //                                       StName = met.SVC_ST_NAME,
        //                                       CustomerType = sp.CUST_TYP
        //                                   }).ToList();

        //        var SPHistoryMeterInfo = (from x in SPHistoryMeterInfo1

        //                                  select new TransformerServicePointInfo
        //                                  {
        //                                      ServicePointId = x.ServicePointId,
        //                                      IsConnected = null,
        //                                      IsSmartMeter = x.IsSmartMeter,
        //                                      Interval = x.Interval,
        //                                      KVATrfPeak = x.KVATrfPeak,
        //                                      PeakKVA = x.PeakKVA,
        //                                      PeakTimestamp = x.PeakTimestamp,
        //                                      MeterNumber = x.MeterNumber,
        //                                      TransformerHistoryId = x.TransformerHistoryId,
        //                                      ServicePointRowNumber = x.ServicePointRowNumber,
        //                                      IsEstimated = x.IsEstimated,
        //                                      Address = x.StNo + " " + x.StName,
        //                                      CustomerType = x.CustomerType
        //                                  }).ToList();
               
        //        if (SPHistoryMeterInfo == null)
        //        {
        //            InfoMessages.AppendLine(string.Format("Unable to find service points' information for Transformer!"));
        //            return null;
        //        }
        //        else if (SPHistoryMeterInfo.Count <= 0)
        //        {
        //            InfoMessages.AppendLine(string.Format("Unable to find service points' information for Transformer!"));
        //        }
        //        var MaxKVABySPInfo = SPHistoryMeterInfo.GroupBy(x => new { x.ServicePointId })
        //            .Select(t => new { PeakKVA = t.Max(s => s.PeakKVA), ServicePointId = t.Key.ServicePointId }).ToList();

        //        if (SPHistoryMeterInfo.Where(sp => sp.IsEstimated == "E").Count() >= 1)
        //            IsEstimated = "E";


        //        foreach (var MaxKVABySPItem in MaxKVABySPInfo)
        //        {
        //            var SPIsEstimated = string.Empty;

        //            var EstimatedData = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == MaxKVABySPItem.ServicePointId && sp.IsEstimated == "E");
        //            if (EstimatedData.Any())
        //            {
        //                SPIsEstimated = "E";
        //            }
        //            TrfSPInfoList.Add(SPHistoryMeterInfo.Where(sp => sp.ServicePointId == MaxKVABySPItem.ServicePointId && sp.PeakKVA == MaxKVABySPItem.PeakKVA).OrderByDescending(s => s.PeakTimestamp).FirstOrDefault());
        //            var obj = TrfSPInfoList.FirstOrDefault(x => x.ServicePointId == MaxKVABySPItem.ServicePointId);
        //            if (obj != null) obj.IsEstimated = SPIsEstimated;
        //        }


        //        int DisplayOrder = 1;
        //        var TrfPeakTimeSt = TrfPeakTime == null ? null : Convert.ToDateTime(TrfPeakTime).ToString("MM/yyyy");
        //        foreach (var TrfSPInfoItem in TrfSPInfoList)
        //        {
        //            TrfSPInfoItem.DisplayOrder = DisplayOrder++;
        //            var SMFlagOfLastAvailList = SPHistoryMeterInfo.Where(t => t.ServicePointId == TrfSPInfoItem.ServicePointId).OrderByDescending(p => p.PeakTimestamp).Select(sp => sp.IsSmartMeter);

        //            var SMFlagOfRecentList = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == TrfSPInfoItem.ServicePointId && sp.PeakTimestamp.ToString("MM/yyyy") == LastDate.ToString("MM/yyyy")).Select(t => t.IsSmartMeter);
        //            var KVATrfPeakList = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == TrfSPInfoItem.ServicePointId && sp.PeakTimestamp.ToString("MM/yyyy") == TrfPeakTimeSt).Select(t => t.KVATrfPeak);
        //            var ConnectedFlagList = SPHistoryMeterInfo.Where(sp => sp.ServicePointId == TrfSPInfoItem.ServicePointId && sp.PeakTimestamp.ToString("MM/yyyy") == ToDate.ToString("MM/yyyy"));

        //            if (KVATrfPeakList.Any())
        //            {
        //                TrfSPInfoItem.KVATrfPeak = KVATrfPeakList.First();
        //            }
        //            if (ConnectedFlagList.Any())
        //            {
        //                TrfSPInfoItem.IsConnected = ConnectedFlagList.Count() >= 1 ? "Y" : "N";
        //            }
        //            if (SMFlagOfRecentList.Any())
        //            {
        //                TrfSPInfoItem.IsSmartMeter = SMFlagOfRecentList.First() == "S" ? "Y" : "N";
        //                if (TrfSPInfoItem.IsSmartMeter == "Y")
        //                {
 
        //                }
        //            }
        //            else
        //            {
        //                if (SMFlagOfLastAvailList.Any())
        //                {
        //                    TrfSPInfoItem.IsSmartMeter = SMFlagOfLastAvailList.First() == "S" ? "Y" : "N";
        //                }
        //            }
        //            if (TrfSPInfoItem.IsSmartMeter == "Y")
        //            {
        //                this.IsAllMetersLegacy = false;
        //            }
                 
        //        }

        //    }
        //    return TrfSPInfoList;
        //}
        private TransformerLoadHistory PopulateTrfPeakHistoryModel(List<Tuple<string, string, string>> MeterRatiosInfoList,
                                                                    List<TransformerServicePointInfo> TrfSPInfoList, string IsEstimated)
        {
            var TrfMonthlyLoadHist = new TransformerLoadHistory();
            TrfMonthlyLoadHist.DataStartDate = this.DataStartDate!=null?this.DataStartDate.Value.ToString("MM/yyyy"):string.Empty;
            TrfMonthlyLoadHist.EarliestLoadDate = this.EarliestDataStartDate;
            TrfMonthlyLoadHist.LatestLoadDate = this.LatestDataStartDate;
            TrfMonthlyLoadHist.SelectedMonth = SelectedMonth;
            TrfMonthlyLoadHist.StartMonth = FromDate.ToString("MM/yyyy");
            TrfMonthlyLoadHist.SearchFromDate = FromDate;
            TrfMonthlyLoadHist.SearchToDate = ToDate;
            TrfMonthlyLoadHist.MeterRatios = MeterRatiosInfoList;
            TrfMonthlyLoadHist.ServicePointInfo = TrfSPInfoList;
            TrfMonthlyLoadHist.DatePeaked = TrfPeakTime;
            TrfMonthlyLoadHist.IsAllMetersLegacy = this.IsAllMetersLegacy;
            TrfMonthlyLoadHist.PeakKVADemand = string.Format("{0} {1}", TrfPeakKVA != null ? string.Format("{0:N1}", TrfPeakKVA) : string.Empty, IsEstimated);
            return TrfMonthlyLoadHist;

        }

        #endregion
    }
}