using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TLM.Models;
using TLM.EntityFramework;
using System.Collections;
using TLM.Common.Services;
using System.Globalization;
using System.Text;
using TLM.Common;
using System.IO;

namespace TLM.Controllers
{
    public class TransformerController : Controller
    {

        string LogPath = string.Empty;
        string ErrorMsg = string.Empty;
       
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            
            var GlobalIdValue = requestContext.RouteData.Values["GlobalId"];
            if (GlobalIdValue != null)
            {
                SetGenerationTab(GlobalIdValue.ToString());                
            }

        }
        public ActionResult GIS(string GlobalId)
        {
          
            string layerName = "Transformer";
            ViewBag.GlobalID = GlobalId;
            ViewBag.LayerName = layerName;
            List<GISAttributes> fields = new List<GISAttributes>();
            var TrfLoadingService = new TransformerLoadingService(GlobalId);
          
            ViewBag.Title = string.Format("GIS Attributes for Transformer CGC# - {0}", TrfLoadingService.CGCId);
             
            
            Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            Dictionary<string, string> attributeValues = new GISService().GetProperties(GlobalId, layer.Item1, layer.Item2);
            //Add for PublicationFS
            Tuple<int, string> FSlayer = SiteCache.GetFSLayerID(layerName);

            Dictionary<string, string> attributeValues1 = new GISService().GetFSProperties(TrfLoadingService.CGCId.ToString(), FSlayer.Item1, FSlayer.Item2);
            string FEEDERNAME = attributeValues1.ElementAt(4).Value.ToString();
            //
            TLM.Models.GIS model = new GIS();
            List<GISAttributes> f1 = new List<GISAttributes>();
            if (attributeValues.ContainsKey("SUBTYPECD"))
            {
                if (!string.IsNullOrEmpty(attributeValues["SUBTYPECD"]))
                {
                    
                   
                    model.GISAttributes = SiteCache.GetLayerSubTypes(layer.Item1, int.Parse(attributeValues["SUBTYPECD"]), layer.Item2);
                   
              
            }
            else
            {
              
                model.GISAttributes = SiteCache.GetLayerAttributes(layer.Item1, layer.Item2);
              
            }

                f1.Add(new GISAttributes()
                {
                    DisplayName = "Feeder Name",
                    FieldName = "FEEDERNAME",
                    Type = "",
                    Value = ""
                });
             


            }
            //Add for  FeederName from PublicationFS
            if (model.GISAttributes[5].FieldName != "FEEDERNAME")
            {
                model.GISAttributes.Insert(5, f1[0]);
            }
            
            model.GISAttributes = model.GISAttributes.GroupBy(x => x.DisplayName).Select(y => y.First()).ToList();
            attributeValues.Add(attributeValues1.ElementAt(4).Key, attributeValues1.ElementAt(4).Value);

            foreach (GISAttributes a in model.GISAttributes)
            {
                if (attributeValues.ContainsKey(a.FieldName))
                    a.Value = attributeValues[a.FieldName];
                
            }
          
            return View(model);
        }
       
        public ActionResult InputLoad(string GlobalId)
        {
            var model = new TransformerPeak();

            try
            {
                //s2nn for Primary Meter TLM info.
                if (string.IsNullOrEmpty(GlobalId)) return View(model);
                if (GlobalId.Split('}').Length > 1 && GlobalId.Split('}')[1].Length > 0) GlobalId = CGCtoGlobalID(GlobalId.Split('}')[1]);

                ViewData["PageTitle"] = "Transformer Loading";
                ViewData["GlobalId"] = GlobalId;
                if (string.IsNullOrEmpty(GlobalId))
                {
                    ViewData["ErrorMessage"] = "Global Id can't be empty!";
                    return View(model);
                }

                var TrfLoadingService = new TransformerLoadingService(GlobalId);
                //SetGenerationTab(GlobalId);
                model = TrfLoadingService.GetTransformerLoadingInfo(false);
                if (model.TransformerId <= 0)
                {
                    ViewData["ErrorMessage"] = "Transformer doesn't exists! Please verify Global Id!";
                    return View(model);
                }

                if (TrfLoadingService.InfoMessages != null)
                {
                    if (TrfLoadingService.InfoMessages.Length >= 1)
                        ViewData["InfoMessage"] = TrfLoadingService.InfoMessages.ToString().Replace(Environment.NewLine, "<br/>");
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            return View(model);
        }

        public ActionResult OutputLoad(string GlobalId)
        {
            var model = new TransformerPeak();
            try
            {
                ViewData["PageTitle"] = "Generation on Transformer";
                ViewData["Warning"] = "Only to be used by Engineering - Smart Meter only";
                ViewData["GlobalId"] = GlobalId;
                if (string.IsNullOrEmpty(GlobalId))
                {
                    ViewData["ErrorMessage"] = "Global Id can't be empty!";
                    return View("InputLoad", model);
                }
                var TrfLoadingService = new TransformerLoadingService(GlobalId);
                //SetGenerationTab(GlobalId);
                model = TrfLoadingService.GetTransformerLoadingInfo(true);
                if (model.TransformerId <= 0)
                {
                    ViewData["ErrorMessage"] = "Transformer doesn't exists! Please verify Global Id!";
                    return View("InputLoad", model);
                }
                if (TrfLoadingService.InfoMessages != null)
                {
                    if (TrfLoadingService.InfoMessages.Length >= 1)
                        ViewData["InfoMessage"] = TrfLoadingService.InfoMessages.ToString().Replace(Environment.NewLine, "<br/>");
                }
            }
            catch (Exception ex)
            {
                //ExceptionLogger Logger = new ExceptionLogger();
                //Logger.LogException(GlobalId,ex, LogPath, "Transformer", "OutputLoad");
                throw ex;
            }
            return View("InputLoad", model);
        }

        public ActionResult SpecialLoad(string GlobalId)
        {
            ViewBag.SettingsDomain = TML.Common.Constants.SettingsURL;
            //s2nn for Primary Meter TLM info.
            if (string.IsNullOrEmpty(GlobalId)) return View();
            if (GlobalId.Split('}').Length > 1 && GlobalId.Split('}')[1].Length > 0)
            {
                ViewBag.isTransformer = false;
                ViewBag.Title = "Primary Meter Settings";
                ViewBag.CGCId = GlobalId.Split('}')[1];
                ViewBag.GlobalId = GlobalId.Split('}')[0] + "}";
            }
            else
            {
                ViewBag.isTransformer = true;
                ViewBag.Title = "Transformer Special Load";                
                if (!string.IsNullOrEmpty(GlobalId))
                {
                    using (TLMEntities db = new TLMEntities())
                    {
                        var CGCId = db.TRANSFORMERs.Where(t => t.GLOBAL_ID.ToUpper() == GlobalId.ToUpper()).Select(t => t.CGC_ID).FirstOrDefault();
                        ViewBag.CGCId = CGCId;
                        ViewBag.GlobalId = GlobalId;
                    }
                }
            }
            return View();
        }       

        [HttpGet]
        public ActionResult History(string GlobalId)
        {
            var model = new TransformerLoadHistory();
            model.SelectedMonth = 12;
          
            //s2nn for Primary Meter TLM info.
            if (string.IsNullOrEmpty(GlobalId)) return View(model);
            if (GlobalId.Split('}').Length > 1 && GlobalId.Split('}')[1].Length > 0) GlobalId = CGCtoGlobalID(GlobalId.Split('}')[1]);

            bool IsModelFromSession = false;
            ViewData["GlobalId"] = GlobalId;
            string msg=TML.Common.Constants.Warningmsg;
            if ( msg== "1")
            {
                ViewBag.Remark = TML.Common.Constants.Warningmsgtext;
            }

            try
            {
                model.StartMonth = (DateTime.Now - new TimeSpan(1095, 0, 0, 0, 0)).ToString("MM/yyyy");

                if (string.IsNullOrEmpty(GlobalId))
                {
                    model.ErrorMessage = "Global Id can't be empty!";
                    return View(model);
                }
                var TrfHistoryService = new TransformerHistoryService(GlobalId);
                model.TransformerId = TrfHistoryService.TransformerId;
                model.TransformerCGC = TrfHistoryService.TransformerCGC;
                if (model.TransformerId <= 0)
                {
                    model.ErrorMessage = "Transformer doesn't exists! Please verify Global Id!";
                    return View(model);
                }
                if (Session["ViewBagModel"] != null)
                {
                    TransformerLoadHistory ViewBagModel = (TransformerLoadHistory)Session["ViewBagModel"];
                    if (ViewBagModel != null)
                    {
                        if (ViewBagModel.TransformerId == model.TransformerId)
                        {
                            model = ViewBagModel;
                            IsModelFromSession = true;
                        }
                    }
                }
                //SetGenerationTab(GlobalId);
                if (ViewBag.IsGenTabVisible != null && ViewBag.IsGenTabVisible == false)
                {
                    model.GenerationFilters = new List<string>() { "Delivered" };
                }

                if (IsModelFromSession)
                    return View(model);
                DateTime EarliestLoadDate;
                DateTime LatestLoadDate;
                TrfHistoryService.GetDataStartDate();
                
                var DataStartDate = TrfHistoryService.DataStartDate;
                model.EarliestLoadDate = TrfHistoryService.EarliestDataStartDate;
                model.LatestLoadDate = TrfHistoryService.LatestDataStartDate;
               
                if (model.EarliestLoadDate.HasValue && model.LatestLoadDate.HasValue)
                {
                    EarliestLoadDate = model.EarliestLoadDate.Value;
                    LatestLoadDate = model.LatestLoadDate.Value;
                    if ((((LatestLoadDate.Year - EarliestLoadDate.Year) * 12) + (LatestLoadDate.Month - EarliestLoadDate.Month)) >= 11)
                    {
                        model.StartMonth = LatestLoadDate.AddMonths(-11).ToString("MM/yyyy");
                    }
                    else
                    {
                        model.StartMonth = EarliestLoadDate.ToString("MM/yyyy");
                       
                    }
                }

                if (!DataStartDate.HasValue)
                {
                    model.DataStartDate = string.Empty;
                    //model.LatestLoadDate=(DateTime?)null;
                    //model.LatestLoadDate = DateTime.MaxValue;//tlm changes 02/19/2020 for data in trf_peak_hist table
                    model.DataLastDate = string.Empty;
                }
                else
                {
                    //model.DataStartDateDT = DataStartDate.Value;
                    model.DataStartDate = DataStartDate.Value.ToString("MM/yyyy");
                    model.DataLastDate = model.LatestLoadDate.Value.ToString("MM/yyyy");
                }
                if (TrfHistoryService.InfoMessages != null)
                {
                    if (TrfHistoryService.InfoMessages.Length >= 1)
                        model.InfoMessage = TrfHistoryService.InfoMessages.ToString().Replace(Environment.NewLine, "<br/>");
                }
            }
            catch (Exception ex)
            {
                //ExceptionLogger Logger = new ExceptionLogger();
                //Logger.LogException(GlobalId,ex, LogPath, "Transformer", "HistoryGET");
                throw ex;
                
            }
            // var s = model.LatestLoadDate.Value;
            //string sa = s.ToString("MM/yyyy");
            if (model.LatestLoadDate.HasValue)
            {

                model.DataLastDate = model.LatestLoadDate.Value.ToString("MM/yyyy");
            }
            else
            {
                model.DataLastDate = "";
            }

            // Session["HeaderDisplayLatestDate"] = model.LatestLoadDate.Value.ToString("MM/yyyy");
           // Session["HeaderDisplayLatestDate"]=  (string.IsNullOrEmpty(model.LatestLoadDate.Value.ToString("MM/yyyy")) ?  null : model.LatestLoadDate.Value.ToString("MM/yyyy"));
             return View(model);
            //}
        }

        [HttpPost]
        public ActionResult History(string GlobalId, TransformerLoadHistory model)
        {
            try
            {
                //s2nn for Primary Meter TLM info.
                if (string.IsNullOrEmpty(GlobalId)) return PartialView("_History", model);
                if (GlobalId.Split('}').Length > 1 && GlobalId.Split('}')[1].Length > 0) GlobalId = CGCtoGlobalID(GlobalId.Split('}')[1]);

                int SelectedMonth = model.SelectedMonth;
                string StartMonth = model.StartMonth;
                string DataStartDate = model.DataStartDate;
                string ShowRecOrGen = model.ShowRecOrGen;
                var ResultDisplayMsg = string.Format(@"Displaying ""{0}"" Data for Start Month {1}  & for {2} Month(s)", ShowRecOrGen, StartMonth, SelectedMonth.ToString());
                ViewData["GlobalId"] = GlobalId;
                model.MeterRatios = new List<Tuple<string, string, string>>();
                model.ServicePointInfo = new List<TransformerServicePointInfo>();
                model.DataDisplayMessage = ResultDisplayMsg;
                var ErrorMessages = new StringBuilder();
                if (string.IsNullOrEmpty(GlobalId))
                    ErrorMessages.AppendLine("Global Id can't be empty!");
                if (string.IsNullOrEmpty(StartMonth))
                    ErrorMessages.AppendLine("Start Month can't be empty!");
                if (string.IsNullOrEmpty(DataStartDate))
                    ErrorMessages.AppendLine("Data Start Date can't be empty!");
                if (ErrorMessages.Length >= 1)
                {
                    model.ErrorMessage = ErrorMessages.ToString().Replace(Environment.NewLine, "<br/>");
                    return View(model);
                }
                //ViewData["DataDisplayMessage"] 
                StartMonth = string.Concat("01/", StartMonth);
                DataStartDate = string.Concat("01/", DataStartDate);

                var DataStartDateDT = DateTime.ParseExact(DataStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var FromDate = DateTime.ParseExact(StartMonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var ToDate = FromDate.AddMonths(SelectedMonth - 1);
                if (model.LatestLoadDate != null && model.LatestLoadDate.HasValue)
                {
                    if (ToDate > model.LatestLoadDate)
                    {
                        ToDate = model.LatestLoadDate.Value;
                        model.SelectedMonth = ((ToDate.Year - FromDate.Year) * 12) + (ToDate.Month - FromDate.Month) + 1;
                        SelectedMonth = model.SelectedMonth;
                        ResultDisplayMsg = model.DataDisplayMessage = string.Format(@"Displaying ""{0}"" Data for Start Month {1}  & for {2} Month(s)", ShowRecOrGen, FromDate.ToString("MM/yyyy"), SelectedMonth.ToString());
                    }
                }

                var TrfHistoryService = new TransformerHistoryService(GlobalId, FromDate, ToDate, SelectedMonth, DataStartDateDT);
                var TransformerId = TrfHistoryService.TransformerId;

                if (TransformerId <= 0)
                {
                    model.ErrorMessage = "Transformer doesn't exists! Please verify Global Id!";
                    return View(model);
                }

                if (ShowRecOrGen.ToLower() == "delivered")
                {
                    model = TrfHistoryService.GetTransformerHistory();
                }
                else if (ShowRecOrGen.ToLower() == "generation")
                {
                    model = TrfHistoryService.GetTransformerHistoryGeneration();
                }
                //tlm enhancement 14/01/2020
                if (TrfHistoryService.InfoMessages != null)
                {
                    if (TrfHistoryService.InfoMessages.Length >= 1)
                    {
                        ErrorMsg=TrfHistoryService.InfoMessages.ToString();// 12/11/2019


                        if (ErrorMsg.Contains("meter") && !ErrorMsg.Contains("service"))
                        {
                            model.InfoMessage = "No  History data available for selected date!";
                        }
                        else if (ErrorMsg.Contains("service") && !ErrorMsg.Contains("meter"))
                        {
                            model.InfoMessage = "No  History data available for selected date!";
                        }
                         else
                         {
                                model.InfoMessage = "No data available for selected date!";
                         }
                }}
                //tlm enhancement 14/01/2020
                if (model.InfoMessage == "No data available for selected date!")
                {
                    model.DataDisplayMessage = "No Data Delivered to display for selected date";
                    
                }
                else 
                {
                    model.DataDisplayMessage = ResultDisplayMsg;
                }
                model.TransformerId = TransformerId;
                model.TransformerCGC = TrfHistoryService.TransformerCGC;
                model.ShowRecOrGen = ShowRecOrGen;
                //SetGenerationTab(GlobalId);
                if (ViewBag.IsGenTabVisible != null && ViewBag.IsGenTabVisible == false)
                {
                    model.GenerationFilters = new List<string>() { "Delivered" };
                }
                else
                {
                    model.GenerationFilters = new List<string>() { "Delivered", "Generation" };
                }
                Session["ViewBagModel"] = model;
            }
            catch (Exception ex)
            {
                //ExceptionLogger Logger = new ExceptionLogger();
                //Logger.LogException(GlobalId,ex, LogPath, "Transformer", "HistoryPOST");
                throw ex;
            }
            return PartialView("_History",model);
        }

        private void SetGenerationTab(string GlobalId)
        {
            //s2nn for Primary Meter TLM info.
            if (string.IsNullOrEmpty(GlobalId)) return;
            if (GlobalId.Split('}').Length > 1 && GlobalId.Split('}')[1].Length > 0) GlobalId = CGCtoGlobalID(GlobalId.Split('}')[1]);

            Dictionary<string, bool> IsGenTabVisible = new Dictionary<string, bool>();
            if (Session["IsGenTabVisible"] == null)
            {
                var TrfLoadingService = new TransformerLoadingService(GlobalId);
                TrfLoadingService.TransformerScreenInit();
                IsGenTabVisible.Clear();
                IsGenTabVisible.Add(GlobalId, TrfLoadingService.IsGenerationVisible);
                Session["IsGenTabVisible"] = IsGenTabVisible;
            }
            else
            {
                IsGenTabVisible = (Dictionary<string, bool>)Session["IsGenTabVisible"];
                if (IsGenTabVisible.ContainsKey(GlobalId))
                {
                    ViewBag.IsGenTabVisible = IsGenTabVisible[GlobalId];
                }
                else
                {
                    var TrfLoadingService = new TransformerLoadingService(GlobalId);
                    TrfLoadingService.TransformerScreenInit();
                    IsGenTabVisible.Clear();
                    IsGenTabVisible.Add(GlobalId, TrfLoadingService.IsGenerationVisible);
                    Session["IsGenTabVisible"] = IsGenTabVisible;
                }
            }
            if (IsGenTabVisible.Count > 0)
            {
                ViewBag.IsGenTabVisible = IsGenTabVisible[GlobalId];
            }
        }

        public ActionResult GetTransformerInfo(string TransfomerID = "1")
        {
            var model = new TransformerInfo()
            {
                SwitchPosition = "automatic",
                Vendor = "Derived from corelogic 2011",
                Maxcycles = 4,
                VoltageChangeTime = 1.5,
                PulseTime = 7,
                MinSwitchVolt = 120,
                EstBankVoltRise = 1.5,
                AutoBVRCalc = "Enabled",
                LowVoltSetPoint = 120,
                HighVoltSetPoint = 130,
                VoltOverrideTime = 60,
                VoltChangeTime = 3
            };

            return PartialView("_TransformerInfo", model);
        }

        public ActionResult Export(decimal TransformerId)
        {
            TransformerLoadHistory Model = new TransformerLoadHistory();
            Model = null;
            if (Session["ViewBagModel"] != null)
            {
                TransformerLoadHistory ViewBagModel = (TransformerLoadHistory)Session["ViewBagModel"];
                if (ViewBagModel != null)
                {
                    if (ViewBagModel.TransformerId == TransformerId)
                    {
                        Model = ViewBagModel;
                    }
                }
            }
            //SetGenerationTab(GlobalId);
            if (ViewBag.IsGenTabVisible != null && ViewBag.IsGenTabVisible == false)
            {
                Model.GenerationFilters = new List<string>() { "Delivered" };
            }

            if (Model == null) return Content("No Data found to export to CSV. Please click back to previous page, retrieve the data and try again.");
            return File(new System.Text.UTF8Encoding().GetBytes(FormatDoc(Model)), "text/csv", "Report.csv");

            //System.IO.File.WriteAllText("test.csv", FormatDoc(Model));
        }

        private string FormatDoc(TransformerLoadHistory Model)
        {
            StringBuilder sb = new StringBuilder();
            var NegativeSign = string.Empty;
            if (Model.ShowRecOrGen.ToLower() == "generation")
            {
                NegativeSign = "-";
            }
            if (Model != null)
            {
                sb.AppendLine(string.Format("Transformer CGC# : {0},,,,,,Run On : {1}", Model.TransformerCGC, DateTime.Now));
                sb.AppendLine(string.Format("Data Available From: {0}", Model.DataStartDate));
                sb.AppendLine(string.Format("Selected Search Range:, {0} thru {1},{2} Data", Model.SearchFromDate.ToString("MM/yyyy", CultureInfo.InvariantCulture), Model.SearchToDate.ToString("MM/yyyy", CultureInfo.InvariantCulture), Model.ShowRecOrGen));

                sb.AppendLine(string.Format("Peak kVA Demand: {0}{1},Date Peaked: {2}", NegativeSign, Model.PeakKVADemand, Model.DatePeaked));

                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Meter Ratios");
                var MonthNames = new StringBuilder();
                var Ratios = new StringBuilder();
                if (Model.MeterRatios != null)
                {
                    foreach (var MonthItem in Model.MeterRatios)
                    {
                        MonthNames.AppendFormat(" {0} ,", MonthItem.Item1);
                        Ratios.AppendFormat(" {0} ,", MonthItem.Item2);
                    }
                }
                sb.AppendLine(MonthNames.ToString());
                sb.AppendLine(Ratios.ToString());
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Service Point Info");
                sb.AppendLine("#,Connected,Smart Meter,Service Point Id, Meter #, Peak Timestamp,Peak kVA, kVA at Trf Peak, Interval, Customer Type, Address");
                if (Model.ServicePointInfo != null)
                {
                    foreach (var SP in Model.ServicePointInfo)
                    {
                        sb.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{11}{6},{11}{7},{8},{9},{10}", SP.DisplayOrder, SP.IsConnected, SP.IsSmartMeter, SP.ServicePointId, SP.MeterNumber, SP.PeakTimestamp, string.Format("{0} {1}", string.Format("{0:n1}", SP.PeakKVA), (SP.IsEstimated == "E" ? "E" : string.Empty)), string.Format("{0} {1}", string.Format("{0:n1}", SP.KVATrfPeak), (SP.IsEstimated == "E" ? "E" : string.Empty)), SP.Interval, SP.CustomerType, SP.Address, NegativeSign));
                    }
                }
            }
            return sb.ToString();
        }

        [HttpPost]
        public JsonResult GetRecOrGenData(string GlobalId, string GenOrRec)
        {
            var model = new TransformerLoadHistory();

            //s2nn for Primary Meter TLM info.
            if (string.IsNullOrEmpty(GlobalId)) return Json(model);
            if (GlobalId.Length == 12) GlobalId = CGCtoGlobalID(GlobalId);

            ViewData["GlobalId"] = GlobalId;
            var ShowRecOrGen = GenOrRec;
            var TrfHistoryService = new TransformerHistoryService(GlobalId);
            if (ShowRecOrGen.ToLower() == "delivered")
            {
                TrfHistoryService.GetDataStartDate();
            }
            else if (ShowRecOrGen.ToLower() == "generation")
            {
                TrfHistoryService.GetDataStartDateGeneration();
            }

            var DataStartDate = TrfHistoryService.DataStartDate;
            ModelState.Remove("EarliestLoadDate");
            model.EarliestLoadDate = TrfHistoryService.EarliestDataStartDate;

            ModelState.Remove("LatestLoadDate");
            model.LatestLoadDate = TrfHistoryService.LatestDataStartDate;

            DateTime EarliestLoadDate;
            DateTime LatestLoadDate;
            if (!DataStartDate.HasValue)
            {
                model.DataStartDate = string.Empty;
            }
            else
            {
                model.DataStartDate = DataStartDate.Value.ToString("MM/yyyy");
            }
            if (model.EarliestLoadDate.HasValue && model.LatestLoadDate.HasValue)
            {
                EarliestLoadDate = model.EarliestLoadDate.Value;
                LatestLoadDate = model.LatestLoadDate.Value;

                if ((((LatestLoadDate.Year - EarliestLoadDate.Year) * 12) + (LatestLoadDate.Month - EarliestLoadDate.Month)) >= 11)
                {
                    model.StartMonth = LatestLoadDate.AddMonths(-11).ToString("MM/yyyy");
                }
                else
                {
                    model.StartMonth = EarliestLoadDate.ToString("MM/yyyy");
                }
            }
            return Json(model);
            //return View("History",model);
        }

        private string CGCtoGlobalID(string sCGC)
        {
            if (string.IsNullOrEmpty(sCGC))
                return sCGC;

            using (var Context = new TLMEntities())
            {
                Int64 CGC = Convert.ToInt64(sCGC);
                var TransformerInfo = Context.TRANSFORMERs.Where(t => t.CGC_ID == CGC).Select(t => new { t.GLOBAL_ID });

                if (TransformerInfo.Any())
                {
                    foreach (var Transformer in TransformerInfo)
                    {
                        return Transformer.GLOBAL_ID;                        
                    }
                }
            }
            return string.Empty;
        }

    }
    public class FeederAttributes
    {
        public string DisplayName { get; set; }
        public string FieldName { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        
    }
}
