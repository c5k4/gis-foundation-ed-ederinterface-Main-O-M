using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;
using TLM.EntityFramework;
using TLM.Models;
using TLM.Common.Interfaces;
using System.Text;

namespace TLM.Common.Services
{
    public class TransformerLoadingService
    {
        private string GlobalId { get; set; }
        private string NameplateKVA { get; set; }
        public decimal TransformerId { get; set; }
        public string ClimateZoneCD { get; set; }
        public short? SecondaryVoltage { get; set; }
        public string SecondaryVoltageDesc { get; set; }
        public decimal CGCId { get; set; }
        public bool IsGeneration { get; set; }
        public bool IsGenerationVisible { get; set; }
        public DateTime? DataStartDate { get; set; }
        readonly string LIGHTER = "LIGHTER";
        readonly string POWER = "POWER";
        readonly string WINDING = " WINDING";
        readonly string UNDETERMINED = "UNDETERMINED";
        readonly string SECVOLCODETYPE = "SECONDARY_VLTG";
        public int TotalUnits { get; set; }
        public StringBuilder InfoMessages = new StringBuilder();
        private string IsWUndetermined { get; set; }
        private string IsSUndetermined { get; set; }
        public Dictionary<string, string> CodesList { get; set; }
        private Dictionary<string, string> CustomerTypesList { get; set; }
        private string[] Seasons { get; set; }
        public TransformerLoadingService(string GlobalId)
        {
            this.GlobalId = GlobalId;
            this.InfoMessages.Clear();
            PopulateTrfInfo();

        }
        #region public methods
        public TransformerPeak GetTransformerLoadingInfo(bool IsGeneration)
        {
            try
            {
                this.IsGeneration = IsGeneration;
                var TrfPeakModel = PopulateTransformerPeakModelFromDB();
                return TrfPeakModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void TransformerScreenInit()
        {
            using (var Context = new TLMEntities())
            {
                var TrfPeakGen = from t in Context.TRF_PEAK_GEN
                                 where t.TRF_ID == this.TransformerId
                                 select t.TRF_ID;

                if (TrfPeakGen.Any())
                {
                    IsGenerationVisible = true;
                }
                else
                {
                    IsGenerationVisible = false;
                }

            }
        }

        #endregion
        #region private methods

        private void PopulateCodes(TLMEntities Context)
        {
            CodesList = new Dictionary<string, string>();
            var Codes = Context.CODE_LOOKUP.Where(c => c.CODE_TYP == UNDETERMINED).Select(c => new { CodeId = c.CODE, CodeDesc = c.DESC_LONG });
            if (Codes.Any())
            {
                foreach (var Code in Codes)
                {
                    CodesList.Add(Code.CodeId, Code.CodeDesc);
                }
            }
        }

        private void PopulateCustomerTypesAndSeasons(TLMEntities Context)
        {
            var SeasonCustTypeCodes = (from s in Context.CODE_LOOKUP
                                       where s.CODE_TYP == "SEASON" || s.CODE_TYP == "CUST_TYPE"
                                       select new { Code = s.CODE, Name = s.DESC_SHORT, CodeType = s.CODE_TYP }).ToList();
            this.Seasons = SeasonCustTypeCodes.Where(s => s.CodeType == "SEASON").OrderBy(s => s.Code).Select(s => s.Code).ToArray<string>();
            this.CustomerTypesList = SeasonCustTypeCodes.Where(ct => ct.CodeType == "CUST_TYPE").Select(ct => new { Code = ct.Code, Name = ct.Name }).ToDictionary(i => i.Code, i => i.Name);

        }
        private List<TransformerPeakByCustType> GetTransfPeakCustTypeInfo(TLMEntities Context, TransformerPeak Model)
        {

            var TrfCustTypes = new List<TransformerCustType>();
            var TrfPeakByCustTypeInfoList = new List<TransformerPeakByCustType>();
            string[] Seasons;
            Dictionary<string, string> CustomerTypes;
            int DisplayOrder = 0;

            IQueryable<TransformerCustType> TrfPeakCustTypeInfo = null;

            var SeasonCustTypeCodes = (from s in Context.CODE_LOOKUP
                                       where s.CODE_TYP == "SEASON" || s.CODE_TYP == "CUST_TYPE"
                                       select new { Code = s.CODE, Name = s.DESC_SHORT, CodeType = s.CODE_TYP }).ToList();
            Seasons = SeasonCustTypeCodes.Where(s => s.CodeType == "SEASON").OrderBy(s => s.Code).Select(s => s.Code).ToArray<string>();
            CustomerTypes = SeasonCustTypeCodes.Where(ct => ct.CodeType == "CUST_TYPE").Select(ct => new { Code = ct.Code, Name = ct.Name }).ToDictionary(i => i.Code, i => i.Name);

            if (Model.TrfPeakIds != null)
            {
                if (!this.IsGeneration)
                {
                    TrfPeakCustTypeInfo = from t in Context.TRF_PEAK_BY_CUST_TYP
                                          where Model.TrfPeakIds.Contains(t.TRF_PEAK_ID)
                                          select new TransformerCustType { Season = t.SEASON, CustomerType = t.CUST_TYP, Qty = t.SEASON_CUST_CNT, KVA = t.SEASON_TOTAL_KVA };
                }
                else
                {
                    TrfPeakCustTypeInfo = from t in Context.TRF_PEAK_GEN_BY_CUST_TYP
                                          where Model.TrfPeakIds.Contains(t.TRF_PEAK_GEN_ID)
                                          select new TransformerCustType { Season = t.SEASON, CustomerType = t.CUST_TYP, Qty = t.SEASON_CUST_CNT, KVA = t.SEASON_TOTAL_KVA };
                }
            }
            //if (TrfPeakCustTypeInfo == null)
            //{
            //    return TrfPeakByCustTypeInfoList;
            //}
            if (TrfPeakCustTypeInfo!=null && TrfPeakCustTypeInfo.Any())
            {
                TrfCustTypes = TrfPeakCustTypeInfo.ToList();
            }
            if (TrfCustTypes != null)
            {
                Model.CustomerCount = TrfCustTypes.Count;
            }
            foreach (var CustomerType in CustomerTypes)
            {
                decimal? WinterQty = null;
                decimal? WinterKVA = null;
                decimal? SummerQty = null;
                decimal? SummerKVA = null;
                /* order by */
                switch (CustomerType.Key.ToLower())
                {
                    case "agr":
                        DisplayOrder = 4;
                        break;
                    case "com":
                        DisplayOrder = 2;
                        break;
                    case "dom":
                        DisplayOrder = 1;
                        break;
                    case "ind":
                        DisplayOrder = 3;
                        break;
                    case "oth":
                        DisplayOrder = 5;
                        break;
                    default:
                        break;
                }

                if (TrfCustTypes != null && TrfCustTypes.Count > 0)
                {
                    var CustTypeSummer = TrfCustTypes.Where(i => i.CustomerType == CustomerType.Key && i.Season == Seasons[0]).FirstOrDefault();
                    var CustTypeWinter = TrfCustTypes.Where(i => i.CustomerType == CustomerType.Key && i.Season == Seasons[1]).FirstOrDefault();

                    if (CustTypeWinter != null)
                    {
                        WinterQty = CustTypeWinter.Qty;
                        if (IsGeneration == true)
                            WinterKVA = CustTypeWinter.KVA * (-1);
                        else
                            WinterKVA = CustTypeWinter.KVA;
                    }
                    if (CustTypeSummer != null)
                    {
                        SummerQty = CustTypeSummer.Qty;
                        if (IsGeneration == true)
                            SummerKVA = CustTypeSummer.KVA * (-1);
                        else
                            SummerKVA = CustTypeSummer.KVA;
                    }

                }
                TrfPeakByCustTypeInfoList.Add(new TransformerPeakByCustType { DisplayOrder = DisplayOrder, CustomerType = CustomerType.Value, WinterQty = WinterQty, WinterKVA = WinterKVA, SummerQty = SummerQty, SummerKVA = SummerKVA });
            }

            TrfPeakByCustTypeInfoList = TrfPeakByCustTypeInfoList.OrderBy(t => t.DisplayOrder).ToList();
            return TrfPeakByCustTypeInfoList;
        }


        private void PopulateTrfInfo()
        {
            if (string.IsNullOrEmpty(this.GlobalId))
                return;

            using (var Context = new TLMEntities())
            {
                var TransformerInfo = Context.TRANSFORMERs.Where(t => t.GLOBAL_ID == this.GlobalId).Select(t => new { t.ID, t.LOWSIDE_VOLTAGE, t.CGC_ID, t.CLIMATE_ZONE_CD });
                //t.PHASE_CD,

                if (TransformerInfo.Any())
                {
                    foreach (var Transformer in TransformerInfo)
                    {
                        this.TransformerId = Transformer.ID;
                        this.CGCId = Transformer.CGC_ID;
                        this.ClimateZoneCD = Transformer.CLIMATE_ZONE_CD;
                        this.SecondaryVoltage = Transformer.LOWSIDE_VOLTAGE;                             
                        var Codes = (from cl in Context.CODE_LOOKUP
                                     where cl.CODE_TYP == SECVOLCODETYPE
                                     select new { cl.DESC_LONG, cl.CODE });
                        if (Codes.Any())
                        {
                            var CodesList = Codes.ToList();
                            if (this.SecondaryVoltage != null)
                            {
                                this.SecondaryVoltageDesc = CodesList.Where(cl => cl.CODE == this.SecondaryVoltage.ToString()).FirstOrDefault().DESC_LONG;
                            }
                        }
                    }
                }
                PopulateCodes(Context);
            }
        }

        private TransformerPeak PopulateTrfInfo(TransformerPeak Model)
        {
            if (this.TransformerId > 0)
            {
                Model.TransformerId = this.TransformerId;
                Model.CGCId = this.CGCId;
                Model.ClimateZoneCD = this.ClimateZoneCD;
                Model.SecondaryVoltage = this.SecondaryVoltage;
                Model.SecondaryVoltageDesc = this.SecondaryVoltageDesc;
            }
            return Model;
        }

        private TransformerPeak PopulateTrfBankInfo(TLMEntities Context, TransformerPeak Model)
        {
            var TransformerBankInfo = from tb in Context.TRANSFORMER_BANK
                                      where tb.TRF_ID == Model.TransformerId
                                      select new { tb.ID, tb.NP_KVA, tb.REC_STATUS };
            if (TransformerBankInfo.Any())
            {
                var Count = TransformerBankInfo.Count();
                var BankIds = new decimal[Count];
                var NPKVAs = new StringBuilder();
                int i = 0;
                foreach (var Bank in TransformerBankInfo.OrderByDescending(t => t.NP_KVA))
                {
                    BankIds[i] = Bank.ID;

                    //26_Nov_2019 Change  for TLM UI Enhancement 
                    if (Bank.REC_STATUS != "D")
                    {
                        //if (i == Count - 1)
                        //    NPKVAs.AppendFormat("{0:n1}", Bank.NP_KVA);
                        //else
                            NPKVAs.AppendFormat("{0:n1}/", Bank.NP_KVA);
                    }                    
                    i++;
                }
                if (NPKVAs.Length > 0) {
                    if (NPKVAs[NPKVAs.Length - 1].ToString() == "/")
                    {
                        NPKVAs.Remove(NPKVAs.Length - 1, 1);
                    }
                }
                
                Model.NameplateKVA = NPKVAs.ToString();
                //26_Nov_2019 Change  for TLM UI Enhancement Requirement - Put NamePlate should be null when status is Delete
                //var status = TransformerBankInfo.Select(t => t.REC_STATUS).FirstOrDefault();
                //if (Convert.ToString(status) == "D")
                //{
                //    Model.NameplateKVA = "";
                //}
                //else
                //{
                //    Model.NameplateKVA = NPKVAs.ToString();
                //}
               
                Model.BankIds = BankIds;
            }
            return Model;
        }

        private TransformerPeak PopulateTrfPeakInfo(TLMEntities Context, TransformerPeak Model)
        {
            IQueryable<TRF_PEAK_COMMON> TransformerPeakInfo;
            if (!this.IsGeneration)
            {
                TransformerPeakInfo = from tp in Context.TRF_PEAK
                                      where tp.TRF_ID == Model.TransformerId
                                      select new TRF_PEAK_COMMON
                                      {
                                          TRF_ID = tp.TRF_ID,
                                          SMR_PEAK_DATE = tp.SMR_PEAK_DATE,
                                          WNTR_PEAK_DATE = tp.WNTR_PEAK_DATE,
                                          SMR_PEAK_SM_CUST_CNT = tp.SMR_PEAK_SM_CUST_CNT,
                                          SMR_PEAK_TOTAL_CUST_CNT = tp.SMR_PEAK_TOTAL_CUST_CNT,
                                          WNTR_PEAK_SM_CUST_CNT = tp.WNTR_PEAK_SM_CUST_CNT,
                                          WNTR_PEAK_TOTAL_CUST_CNT = tp.WNTR_PEAK_TOTAL_CUST_CNT,
                                          DATA_START_DATE = tp.DATA_START_DATE,
                                          ID = tp.ID,
                                          SMR_LF = tp.SMR_LF,
                                          WNTR_LF = tp.WNTR_LF,
                                          SMR_LOAD_UNDETERMINED = tp.SMR_LOAD_UNDETERMINED,
                                          WNTR_LOAD_UNDETERMINED = tp.WNTR_LOAD_UNDETERMINED
                                      };
            }
            else
            {
                TransformerPeakInfo = from tp in Context.TRF_PEAK_GEN
                                      where tp.TRF_ID == Model.TransformerId
                                      select new TRF_PEAK_COMMON
                                      {
                                          TRF_ID = tp.TRF_ID,
                                          SMR_PEAK_DATE = tp.SMR_PEAK_DATE,
                                          WNTR_PEAK_DATE = tp.WNTR_PEAK_DATE,
                                          SMR_PEAK_SM_CUST_CNT = tp.SMR_PEAK_SM_CUST_CNT,
                                          SMR_PEAK_TOTAL_CUST_CNT = tp.SMR_PEAK_TOTAL_CUST_CNT,
                                          WNTR_PEAK_SM_CUST_CNT = tp.WNTR_PEAK_SM_CUST_CNT,
                                          WNTR_PEAK_TOTAL_CUST_CNT = tp.WNTR_PEAK_TOTAL_CUST_CNT,
                                          DATA_START_DATE = tp.DATA_START_DATE,
                                          ID = tp.ID,
                                          SMR_LF = tp.SMR_LF,
                                          WNTR_LF = tp.WNTR_LF,
                                          SMR_LOAD_UNDETERMINED = tp.SMR_LOAD_UNDETERMINED,
                                          WNTR_LOAD_UNDETERMINED = tp.WNTR_LOAD_UNDETERMINED
                                      };
            }
            decimal[] TrfPeakIds = null;
            if (TransformerPeakInfo.Any())
            {
                Model.DataStartDate = TransformerPeakInfo.GroupBy(g => g.TRF_ID).Select(grp => grp.Min(d => d.DATA_START_DATE)).FirstOrDefault();
                TrfPeakIds = new decimal[TransformerPeakInfo.Count()];
                int i = 0;
                foreach (var TrfPeak in TransformerPeakInfo)
                {
                    TrfPeakIds[i] = TrfPeak.ID;
                    i++;
                    Model.SummerPeakDate = TrfPeak.SMR_PEAK_DATE;
                    Model.SummerSMCustCount = TrfPeak.SMR_PEAK_SM_CUST_CNT;
                    Model.SummerTotalCustCount = TrfPeak.SMR_PEAK_TOTAL_CUST_CNT;
                    //Model.SummerLoadFactor = TrfPeak.SMR_LF;
                    Model.SummerLoadUndetermined = TrfPeak.SMR_LOAD_UNDETERMINED;

                    Model.WinterPeakDate = TrfPeak.WNTR_PEAK_DATE;
                    Model.WinterSMCustCount = TrfPeak.WNTR_PEAK_SM_CUST_CNT;
                    Model.WinterTotalCustCount = TrfPeak.WNTR_PEAK_TOTAL_CUST_CNT;
                    //Model.WinterLoadFactor = TrfPeakTrfPeak.WNTR_LF;
                    Model.WinterLoadUndetermined = TrfPeak.WNTR_LOAD_UNDETERMINED;


                    Model.WinterSMCustToTotalCust = string.Format("{0}/{1}", Convert.ToInt32(Model.WinterSMCustCount).ToString(), Convert.ToInt32(Model.WinterTotalCustCount).ToString());
                    Model.SummerSMCustToTotalCust = string.Format("{0}/{1}", Convert.ToInt32(Model.SummerSMCustCount).ToString(), Convert.ToInt32(Model.SummerTotalCustCount).ToString());
                    Model.WinterCustRatio = Model.WinterTotalCustCount == null || Model.WinterTotalCustCount == 0 ? null : (Model.WinterSMCustCount / Model.WinterTotalCustCount) * 100;
                    Model.SummerCustRatio = Model.SummerTotalCustCount == null || Model.SummerTotalCustCount == 0 ? null : (Model.SummerSMCustCount / Model.SummerTotalCustCount) * 100;
                    Model.WinterCustomersRatio = string.Format("{0}%", Convert.ToInt32(Model.WinterCustRatio).ToString());
                    Model.SummerCustomersRatio = string.Format("{0}%", Convert.ToInt32(Model.SummerCustRatio).ToString());
                    Model.WinterLoadFactor = TrfPeak.WNTR_LF != null ? (TrfPeak.WNTR_LF > 100 ? " Bad Data" : string.Format("{0}%", TrfPeak.WNTR_LF.ToString())) : "NA";
                    Model.SummerLoadFactor = TrfPeak.SMR_LF != null ? (TrfPeak.SMR_LF > 100 ? " Bad Data" : string.Format("{0}%",TrfPeak.SMR_LF.ToString())) : "NA";
                    //Model.LoadFactor = string.Format("Winter {0}% / Summer {1}%", WinterLFTextToDisplay, SummerLFTextToDisplay);
                }
            }
            Model.TrfPeakIds = TrfPeakIds;
            return Model;
        }
        private TransformerPeak PopulateTrfBankPeakInfo(TLMEntities Context, TransformerPeak Model)
        {
            var BankIds = Model.BankIds;
            var TrfPeakIds = Model.TrfPeakIds;
            IQueryable<TRF_BANK_PEAK_COMMON> TrfBankPeakData = null;
            List<TRF_BANK_PEAK_COMMON> TransformerBankPeakInfo = null;
            if (!this.IsGeneration)
            {
                TrfBankPeakData = from tbp in Context.TRF_BANK_PEAK
                                  join tb in Context.TRANSFORMER_BANK
                                  on tbp.TRF_BANK_ID equals tb.ID
                                  where BankIds.Contains(tbp.TRF_BANK_ID)
                                  && TrfPeakIds.Contains(tbp.TRF_PEAK_ID)
                                  orderby tb.NP_KVA descending
                                  select new TRF_BANK_PEAK_COMMON
                                  {
                                      NameplateKVAs = tbp.NP_KVA,
                                      ID = tbp.ID,
                                      SMR_KVA = tbp.SMR_KVA,
                                      WNTR_KVA = tbp.WNTR_KVA,
                                      SMR_CAP = tbp.SMR_CAP,
                                      WNTR_CAP = tbp.WNTR_CAP,
                                      SMR_PCT = tbp.SMR_PCT,
                                      WNTR_PCT = tbp.WNTR_PCT,
                                      DATA_START_DATE = tbp.DATA_START_DATE,
                                      TRF_PEAK_ID = tbp.TRF_PEAK_ID,
                                      TransformerType = tb.TRF_TYP,
                                      TRF_BANK_ID = tbp.TRF_BANK_ID
                                  };

            }
            else
            {
                TrfBankPeakData = from tbp in Context.TRF_BANK_PEAK_GEN
                                  join tb in Context.TRANSFORMER_BANK
                                  on tbp.TRF_BANK_ID equals tb.ID
                                  where BankIds.Contains(tbp.TRF_BANK_ID)
                                  && TrfPeakIds.Contains(tbp.TRF_PEAK_GEN_ID)
                                  orderby tb.NP_KVA descending
                                  select new TRF_BANK_PEAK_COMMON
                                  {
                                      NameplateKVAs = tbp.NP_KVA,
                                      ID = tbp.ID,
                                      SMR_KVA = (tbp.SMR_KVA) * (-1),
                                      WNTR_KVA = (tbp.WNTR_KVA) * (-1),
                                      SMR_CAP = tbp.SMR_CAP,
                                      WNTR_CAP = tbp.WNTR_CAP,
                                      SMR_PCT = tbp.SMR_PCT,
                                      WNTR_PCT = tbp.WNTR_PCT,
                                      DATA_START_DATE = tbp.DATA_START_DATE,
                                      TRF_PEAK_ID = tbp.TRF_PEAK_GEN_ID,
                                      TransformerType = tb.TRF_TYP,
                                      TRF_BANK_ID = tbp.TRF_BANK_ID
                                  };
            }

            if (TrfBankPeakData.Any())
            {
                TransformerBankPeakInfo = TrfBankPeakData.ToList();
                int BankCnt = 0;
                int TotalItems = TransformerBankPeakInfo.Count();
                Model.TotalUnits = TotalItems;
                if (TotalItems <= 3)
                {

                    Model.WinterBankTypes = new string[TotalItems];
                    Model.WinterPeakMonths = new DateTime?[TotalItems];
                    Model.WinterNamePlateKVAs = new decimal?[TotalItems];
                    Model.WinterCalcCaps = new decimal?[TotalItems];
                    Model.WinterKVAPeaks = new decimal?[TotalItems];
                    Model.WinterPercLoadings = new string[TotalItems];

                    Model.SummerBankTypes = new string[TotalItems];
                    Model.SummerPeakMonths = new DateTime?[TotalItems];
                    Model.SummerNamePlateKVAs = new decimal?[TotalItems];
                    Model.SummerCalcCaps = new decimal?[TotalItems];
                    Model.SummerKVAPeaks = new decimal?[TotalItems];
                    Model.SummerPercLoadings = new string[TotalItems];
                    Model.WinterLoadStatus = string.Empty;
                    Model.SummerLoadStatus =string.Empty;

                    
                    TransformerBankPeakInfo = TransformerBankPeakInfo.OrderByDescending(bp => bp.NameplateKVAs).OrderByDescending(bp => bp.SMR_KVA).ToList();

                    

                    BankCnt = 0;
                    var Winding = string.Empty;
                    foreach (var PeakInfo in TransformerBankPeakInfo)
                    {

                        if (PeakInfo.TransformerType == "21" || PeakInfo.TransformerType == "32")
                        {
                            Winding = WINDING;
                        }
                        switch (BankCnt)
                        {
                            case 0:
                                Model.SummerBankTypes[BankCnt] = LIGHTER + Winding;
                                break;
                            default:
                                Model.SummerBankTypes[BankCnt] = POWER + Winding;
                                break;
                        }
                        Model.SummerNamePlateKVAs[BankCnt] = PeakInfo.NameplateKVAs;
                        Model.SummerPeakMonths[BankCnt] = Model.SummerPeakDate;
                        Model.SummerCalcCaps[BankCnt] = PeakInfo.SMR_CAP;
                        Model.SummerKVAPeaks[BankCnt] = PeakInfo.SMR_KVA;
                        Model.SummerPercLoadings[BankCnt] = PeakInfo.SMR_PCT != null ? string.Format("{0:n1}%", PeakInfo.SMR_PCT) : string.Empty;

                        if(PeakInfo.SMR_PCT!=null)
                        {
                            if(PeakInfo.SMR_PCT<=100)
                            {
                                Model.SummerLoadStatus = "Correct";
                            }
                            else
                            {
                                Model.SummerLoadStatus = "Overloaded";
                            }
                        }
                        
                        BankCnt++;
                    }


                    TransformerBankPeakInfo = TransformerBankPeakInfo.OrderByDescending(bp => bp.NameplateKVAs).OrderByDescending(bp => bp.WNTR_KVA).ToList();
                    BankCnt = 0;
                    foreach (var PeakInfo in TransformerBankPeakInfo)
                    {
                        if (PeakInfo.TransformerType == "21" || PeakInfo.TransformerType == "32")
                        {
                            Winding = WINDING;
                        }
                        
                        switch (BankCnt)
                        {
                            case 0:
                                Model.WinterBankTypes[BankCnt] = LIGHTER + Winding;
                                break;
                            default:
                                Model.WinterBankTypes[BankCnt] = POWER + Winding;
                                break;
                        }
                        Model.WinterNamePlateKVAs[BankCnt] = PeakInfo.NameplateKVAs;
                        Model.WinterPeakMonths[BankCnt] = Model.WinterPeakDate;
                        Model.WinterCalcCaps[BankCnt] = PeakInfo.WNTR_CAP;
                        Model.WinterKVAPeaks[BankCnt] = PeakInfo.WNTR_KVA;
                        Model.WinterPercLoadings[BankCnt] = PeakInfo.WNTR_PCT != null ? string.Format("{0:n1}%", PeakInfo.WNTR_PCT) : string.Empty;
                        if (PeakInfo.WNTR_PCT != null)
                        {
                            if (PeakInfo.WNTR_PCT <= 100)
                            {
                                Model.WinterLoadStatus = "Correct";
                            }
                            else
                            {
                                Model.WinterLoadStatus = "Overloaded";
                            }
                        }
                        
                        BankCnt++;
                    }

                }
            }
            CheckIfUndetermined(Model);
            return Model;
        }

        private TransformerPeak PopulateTransformerPeakModelFromDB()
        {
            var Model = new TransformerPeak();

            using (var Context = new TLMEntities())
            {

                Model = PopulateTrfInfo(Model);

                if (Model.TransformerId != 0)
                {
                    Model = PopulateTrfBankInfo(Context, Model);
                    Model = PopulateTrfPeakInfo(Context, Model);
                    if (Model.BankIds != null && Model.TrfPeakIds != null)
                    {
                        if (Model.BankIds.Length > 0 && Model.TrfPeakIds.Length > 0)
                        {
                            Model = PopulateTrfBankPeakInfo(Context, Model);
                        }
                    }
                    //if (Model.TrfPeakIds != null && Model.TrfPeakIds.Length > 0)
                   // {
                        Model.TrfInfoByCustType = GetTransfPeakCustTypeInfo(Context, Model);
                        if (Model.CustomerCount <= 0)
                        {
                            Model.WinterLoadStatus = "No Customers";
                            Model.SummerLoadStatus = "No Customers";
                            Model.LoadFactor = "Summer NA / Winter NA";
                        }
                        
                   // }
                }

            }
            return Model;
        }


        private TransformerPeak CheckIfUndetermined(TransformerPeak Model)
        {

            if (!string.IsNullOrEmpty(Model.WinterLoadUndetermined))
            {
                Model.WinterBankTypes = new string[0];
                Model.WinterPeakMonths = new DateTime?[0];
                Model.WinterNamePlateKVAs = new decimal?[0];
                Model.WinterCalcCaps = new decimal?[0];
                Model.WinterKVAPeaks = new decimal?[0];
                Model.WinterPercLoadings = new string[0];
                Model.WinterLoadStatus = "<span class=\"undetermined\">Indeterminable (" + Model.WinterLoadUndetermined + "**)</span>";
                Model.WinterCodeDescription = "**  " + Model.WinterLoadUndetermined + " - " + ((CodesList != null && CodesList.Count >= 1) ? CodesList[Model.WinterLoadUndetermined] : string.Empty);
                Model.WinterLoadFactor = "NA";
            }

            if (!string.IsNullOrEmpty(Model.SummerLoadUndetermined))
            {
                Model.SummerBankTypes = new string[0];
                Model.SummerPeakMonths = new DateTime?[0];
                Model.SummerNamePlateKVAs = new decimal?[0];
                Model.SummerCalcCaps = new decimal?[0];
                Model.SummerKVAPeaks = new decimal?[0];
                Model.SummerPercLoadings = new string[0];
                Model.SummerLoadStatus = "<span class=\"undetermined\">Indeterminable (" + Model.SummerLoadUndetermined + "***)</span>";
                Model.SummerCodeDescription = "*** " + Model.SummerLoadUndetermined + " - " + ((CodesList != null && CodesList.Count >= 1) ? CodesList[Model.SummerLoadUndetermined] : string.Empty);
                Model.SummerLoadFactor = " NA";
            }
            Model.LoadFactor = string.Format("Winter {0} / Summer {1}", Model.WinterLoadFactor, Model.SummerLoadFactor);
            return Model;
        }
        #endregion

    }
    public class TransformerCustType
    {
        public string CustomerType { get; set; }
        public string Season { get; set; }
        public decimal Qty { get; set; }
        public decimal KVA { get; set; }

    }
    public class TRF_BANK_PEAK_COMMON
    {
        public decimal ID { get; set; }
        public decimal? SMR_KVA { get; set; }
        public decimal? WNTR_KVA { get; set; }
        public decimal? SMR_CAP { get; set; }
        public decimal? WNTR_CAP { get; set; }
        public decimal? SMR_PCT { get; set; }
        public decimal? WNTR_PCT { get; set; }
        public DateTime? DATA_START_DATE { get; set; }
        public decimal? TRF_PEAK_ID { get; set; }
        public decimal? TRF_BANK_ID { get; set; }
        public decimal? NameplateKVAs { get; set; }
        public string TransformerType { get; set; }

    }
    public class TRF_PEAK_COMMON
    {
        public decimal TRF_ID { get; set; }
        public DateTime? SMR_PEAK_DATE { get; set; }
        public DateTime? WNTR_PEAK_DATE { get; set; }
        public decimal SMR_KVA { get; set; }
        public decimal WNTR_KVA { get; set; }
        public decimal SMR_CAP { get; set; }
        public decimal WNTR_CAP { get; set; }
        public decimal? SMR_PCT { get; set; }
        public decimal? WNTR_PCT { get; set; }
        public decimal? SMR_PEAK_SM_CUST_CNT { get; set; }
        public decimal? SMR_PEAK_TOTAL_CUST_CNT { get; set; }
        public decimal? WNTR_PEAK_SM_CUST_CNT { get; set; }
        public decimal? WNTR_PEAK_TOTAL_CUST_CNT { get; set; }
        public DateTime? DATA_START_DATE { get; set; }
        public decimal ID { get; set; }
        public short? SMR_LF { get; set; }
        public short? WNTR_LF { get; set; }
        public string SMR_LOAD_UNDETERMINED { get; set; }
        public string WNTR_LOAD_UNDETERMINED { get; set; }
    }
}
