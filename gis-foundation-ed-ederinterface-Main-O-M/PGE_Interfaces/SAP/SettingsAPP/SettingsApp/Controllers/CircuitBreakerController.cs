using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;

using SettingsApp.Models;
using SettingsApp.Common;

namespace SettingsApp.Controllers
{

    public class CircuitBreakerController : Controller
    {
        private const string DEVICE_NAME = "CircuitBreaker";
        private const string DEVICE_NAME_DOCUMENTUM = "Circuit Breaker";
        private const string DEVICE_TABLE_NAME = "SM_CIRCUIT_BREAKER";
        private const string DEVICE_HIST_TABLE_NAME = "SM_CIRCUIT_BREAKER_HIST";

        #region "Action Methods"

        public ActionResult Index(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            CircuitBreakerModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);

            setUiSettings(HtmlExtensions.PageMode.Current);
            this.initializeDropDowns(model);



            return View(model);
        }

        public ActionResult Future(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            CircuitBreakerModel model = getDevice(globalID, HtmlExtensions.PageMode.Future);
            setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

            return View("Index", model);
        }

        public ActionResult GIS(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            setUiSettings(HtmlExtensions.PageMode.GIS);
            CircuitBreakerModel model = new CircuitBreakerModel();

            Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            Dictionary<string, string> attributeValues = new GISService().GetProperties(globalID, layer.Item1, layer.Item2);


            if (attributeValues.ContainsKey("SUBTYPECD"))
            {
                if (!string.IsNullOrEmpty(attributeValues["SUBTYPECD"]))
                {
                    model.GISAttributes = SiteCache.GetLayerSubTypes(layer.Item1, int.Parse(attributeValues["SUBTYPECD"]), layer.Item2);
                }
            }
            else
            {
                model.GISAttributes = SiteCache.GetLayerAttributes(layer.Item1, layer.Item2);
            }
            foreach (GISAttributes a in model.GISAttributes)
            {
                if (attributeValues.ContainsKey(a.FieldName))
                    a.Value = attributeValues[a.FieldName];
            }
            return View(model);
        }

        public ActionResult History(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            setUiSettings(HtmlExtensions.PageMode.History);

            CommentHistoryModel ch = new CommentHistoryModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                ch.CommentHistory = (from c in db.SM_COMMENT_HIST
                                     select new History
                                     {
                                         ID = c.ID,
                                         GlobalID = c.GLOBAL_ID,
                                         DeviceTableName = c.DEVICE_TABLE_NAME,
                                         DeviceHistoryTableName = c.DEVICE_HIST_TABLE_NAME,
                                         Note = c.COMMENTS,
                                         WorkDate = c.WORK_DATE,
                                         WorkType = c.WORK_TYPE,
                                         EntryDate = c.ENTRY_DATE,
                                         HistoryID = c.HIST_ID,
                                         PerformedBy = c.PERFORMED_BY
                                     }).Where(c => c.GlobalID == globalID).ToList();

                List<History> cir_hist = (from c in db.SM_COMMENT_HIST
                                          join c_h in db.SM_CIRCUIT_BREAKER_HIST.Where(s => s.GLOBAL_ID == globalID) on c.HIST_ID equals c_h.ID
                                          select new History
                                          {
                                              ID = c.ID,
                                              GlobalID = c.GLOBAL_ID,
                                              DeviceTableName = c.DEVICE_TABLE_NAME,
                                              DeviceHistoryTableName = c.DEVICE_HIST_TABLE_NAME,
                                              Note = c.COMMENTS,
                                              WorkDate = c.WORK_DATE,
                                              WorkType = c.WORK_TYPE,
                                              EntryDate = c.ENTRY_DATE,
                                              HistoryID = c.HIST_ID,
                                              PerformedBy = c.PERFORMED_BY
                                          }).Where(c => c.DeviceHistoryTableName == DEVICE_HIST_TABLE_NAME).ToList();

                ch.CommentHistory.AddRange(cir_hist);
                ch.CommentHistory = ch.CommentHistory.OrderByDescending(c => c.EntryDate).ToList();
            }

            return View("History", ch);
        }

        public ActionResult HistoryDetails(string globalID, string layerName, decimal ID)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;

            CircuitBreakerModel model = new CircuitBreakerModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_CIRCUIT_BREAKER_HIST e = db.SM_CIRCUIT_BREAKER_HIST.FirstOrDefault(s => s.GLOBAL_ID == globalID && s.ID == ID);
                model.PopulateModelFromHistoryEntity(e);
            }

            setUiSettings(HtmlExtensions.PageMode.History);
            this.initializeDropDowns(model);

            return View("HistoryData", model);
        }

        public ActionResult DeleteComment(string globalID, string layerName, decimal ID)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_COMMENT_HIST history = db.SM_COMMENT_HIST.First(c => c.ID == ID);
                db.SM_COMMENT_HIST.DeleteObject(history);
                db.SaveChanges();
            }

            return RedirectToAction("History", new { globalID = globalID, layerName = layerName });
        }

        public ActionResult Files(string globalID, string layerName, string settingsError = null, string peerReviewError = null)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.SettingsFileError = (settingsError == null ? "" : settingsError);
            ViewBag.PeerReviewError = (peerReviewError == null ? "" : peerReviewError);
            ViewBag.SettingsFileUploaded = (settingsError != null && settingsError == "SUCESS" ? true : false);
            ViewBag.PeerReviewFileUploaded = (peerReviewError != null && peerReviewError == "SUCESS" ? true : false); ;

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_CIRCUIT_BREAKER e = db.SM_CIRCUIT_BREAKER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");

                if (e != null && !string.IsNullOrEmpty(e.DISTRICT) && !string.IsNullOrEmpty(e.DIVISION) && !string.IsNullOrEmpty(e.OPERATING_NUM))
                {
                    ViewBag.ShowInsufficientMetaDataRequirement = false;
                    string district = e.DISTRICT;
                    string division = e.DIVISION;
                    System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                    district = textInfo.ToTitleCase(district);
                    division = textInfo.ToTitleCase(division);

                    ViewBag.D2SettingsURL = Constants.BuildDocumentumUrl(false, division, district, DEVICE_NAME_DOCUMENTUM, e.OPERATING_NUM);
                    ViewBag.DBPeerReviewURL = Constants.BuildDocumentumUrl(true, division, district, DEVICE_NAME_DOCUMENTUM, e.OPERATING_NUM);
                }
                else
                    ViewBag.ShowInsufficientMetaDataRequirement = true;
            }
            ViewBag.DeviceType = DEVICE_NAME;
            setUiSettings(HtmlExtensions.PageMode.File);
            return View("Files");
        }

        public ActionResult EngineeringInfo(string globalID, string layerName, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;
            ViewBag.ControllerName = "CircuitBreaker";
            ViewBag.AddNewItemErrorMessage = string.Empty;
            ViewBag.SaveErrorMessage = string.Empty;

            var model = new CircuitLoadModel();
            decimal CircuitBreakerId = 0;
            CircuitBreakerId = GetCircuitBreakerRowIdForLoadInfo(globalID);
            if (CircuitBreakerId == 0)
            {
                ViewBag.NoDataFoundMsg = "No data found in the system. Please contact an administrator.";
                return View("EngineeringInfo", model);
            }
            model = GetLoadInfo(CircuitBreakerId);
            setUiSettings(HtmlExtensions.PageMode.EngineeringInfo);
            return View("EngineeringInfo", model);
        }
        #endregion


        #region "Post Methods"
        [HttpPost]
        public ActionResult SaveLoadInfo(CircuitLoadModel model, string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            
            ViewBag.ControllerName = "CircuitBreaker";
            ViewBag.IsDisabled = false;
            ViewBag.AddNewItemErrorMessage = string.Empty;
            ViewBag.SaveErrorMessage = string.Empty;
            try
            {
                
                if (ModelState.ContainsKey("NewLoadMonth"))
                    ModelState["NewLoadMonth"].Errors.Clear();
                if (ModelState.ContainsKey("NewLoadTime"))
                    ModelState["NewLoadTime"].Errors.Clear();
                if (ModelState.ContainsKey("NewLoadTotalKW"))
                    ModelState["NewLoadTotalKW"].Errors.Clear();
                
                 if (ModelState.IsValid)
                {
                    var ErrorMessage = model.ValidateRecentPeakLoadInfo();
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        ViewBag.ShowPageError = true;
                        ViewBag.SaveErrorMessage = ErrorMessage;
                        return View("EngineeringInfo", model);
                    }
                    using (SettingsEntities db = new SettingsEntities())
                    {
                        var CircuitLoadData = db.SM_CIRCUIT_LOAD.Where(t => t.REF_DEVICE_ID == model.CircuitBreakerRowId).FirstOrDefault();
                        if (CircuitLoadData == null)
                        {
                            CircuitLoadData = new SM_CIRCUIT_LOAD();
                            var MaxLoadId = (db.SM_CIRCUIT_LOAD.Max(t =>(decimal?) t.ID))??Decimal.Zero;
                            CircuitLoadData.ID = MaxLoadId + 1;
                            model.PopulateEntityFromModel(CircuitLoadData,null);
                            db.SM_CIRCUIT_LOAD.AddObject(CircuitLoadData);
                        }
                        else
                        {
                            var CircuitLoadHistData = db.SM_CIRCUIT_LOAD_HIST.Where(t => t.SM_CIRCUIT_LOAD_ID == CircuitLoadData.ID).OrderByDescending(t => t.PEAK_DTM);
                            model.PopulateEntityFromModel(CircuitLoadData, CircuitLoadHistData);
                        }
                        
                        db.SaveChanges();
                        ViewBag.ShowSaveSucessful = true;
                        ModelState.Clear();
                        model = GetLoadInfo(model.CircuitBreakerRowId);
                        TempData["ShowSaveSuccessful"] = true;
                        return RedirectToAction("EngineeringInfo", new { globalID = globalID, layerName = layerName });

                    }
                }
                else
                {
                    ViewBag.ShowPageError = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("EngineeringInfo", model);
        }

        [HttpPost]
        public ActionResult AddNewItem(CircuitLoadModel model, string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;

            ViewBag.ControllerName = "CircuitBreaker";
            ViewBag.AddNewItemErrorMessage = string.Empty;
            ViewBag.SaveErrorMessage = string.Empty;
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    var ErrorMessage = model.ValidateNewPeakLoadInfo();
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        ViewBag.ShowPageError = true;
                        ViewBag.AddNewItemErrorMessage = ErrorMessage;
                        return View("EngineeringInfo", model);
                        //return RedirectToAction("EngineeringInfo", new { globalID = globalID, layerName = layerName, units = units, model = model });
                    }
                    using (SettingsEntities db = new SettingsEntities())
                    {
                        SM_CIRCUIT_LOAD_HIST CircuitLoadHist = new SM_CIRCUIT_LOAD_HIST();
                        CircuitLoadHist.PEAK_DTM = model.GetValidPeakDateTime(model.NewLoadMonth, model.NewLoadTime);
                        CircuitLoadHist.TOTAL_KW_LOAD = model.NewLoadTotalKW;
                        var MaxHistoryId = (db.SM_CIRCUIT_LOAD_HIST.Max(t => (decimal?)t.ID))??Decimal.Zero;
                        CircuitLoadHist.ID = MaxHistoryId + 1;
                        db.SM_CIRCUIT_LOAD_HIST.AddObject(CircuitLoadHist);
                        var CircuitLoadData = db.SM_CIRCUIT_LOAD.Where(t => t.REF_DEVICE_ID == model.CircuitBreakerRowId).FirstOrDefault();
                        CircuitLoadHist.SM_CIRCUIT_LOAD_ID = CircuitLoadData.ID;
                        var WinterMonths = new int[] { 1, 2, 3, 11, 12 };
                        if (CircuitLoadHist.PEAK_DTM != null)
                        {
                            if (WinterMonths.Contains(CircuitLoadHist.PEAK_DTM.Value.Month))
                            {
                                CircuitLoadData.WINTER_UPDT_DTM = DateTime.Now;
                            }
                            else
                            {
                                CircuitLoadData.SUMMER_UPDT_DTM = DateTime.Now;
                            }
                        }
                        db.SaveChanges();
                        var CircuitLoadLoadHistData = db.SM_CIRCUIT_LOAD_HIST.Where(t => t.SM_CIRCUIT_LOAD_ID == CircuitLoadData.ID).OrderByDescending(t => t.PEAK_DTM);
                        model.PopulateModelFromEntity(CircuitLoadData, CircuitLoadLoadHistData.ToList());
                        ViewBag.ShowAddSucessful = true;
                        ModelState.Clear();
                        model = GetLoadInfo(model.CircuitBreakerRowId);
                        TempData["ShowAddSuccessful"] = true;
                        return RedirectToAction("EngineeringInfo", new { globalID = globalID, layerName = layerName });
                        //model.NewLoadMonth = null;
                        //model.NewLoadTime = null;
                        //model.NewLoadTotalKW = null;
                    }
                }
                else
                {
                    setUiSettings(HtmlExtensions.PageMode.EngineeringInfo);
                    ViewBag.ShowPageError = true;
                    return View("EngineeringInfo", model);
                }
            }
            else
            {
                setUiSettings(HtmlExtensions.PageMode.EngineeringInfo);
                ViewBag.ShowPageError = true;
                return View("EngineeringInfo", model);
            }
            

        }

        [HttpPost]
        public ActionResult SaveFuture(CircuitBreakerModel model, string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            this.setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

            if (model.Scada == null || model.Scada.ToUpper() == "N" || model.FlisrAutomationDevice == null || model.FlisrAutomationDevice == "")
            {
                ModelState.Remove("FlisrAutomationDevice");
                ModelState.Add("FlisrAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FlisrAutomationDevice", new ValueProviderResult("N", "N", null));
                model.FlisrAutomationDevice = "N";
            }

            try
            {
                if (ModelState.IsValid)
                {
                    SettingsEntities db = new SettingsEntities();
                    db.Connection.Open();
                    using (var transaction = db.Connection.BeginTransaction())
                    {
                        if (model.Release)
                        {
                            SM_CIRCUIT_BREAKER_HIST history = this.saveHistory(db, globalID, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);
                            db.SM_CIRCUIT_BREAKER_HIST.AddObject(history);
                            db.SaveChanges();
                            this.addComment(db, globalID, SystemEnums.WorkType.SETT, "Release to Current", history.ID);
                        }
                        else
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);

                        try
                        {
                            db.SaveChanges();
                            transaction.Commit();
                            ModelState.Clear();
                            model = this.getDevice(globalID, HtmlExtensions.PageMode.Future);
                            this.initializeDropDowns(model);
                            ViewBag.ShowSaveSucessful = true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            db.Connection.Close();
                            throw ex;
                        }
                    }

                    return View("Index", model);
                }
                else
                {
                    ViewBag.ShowPageError = true;
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                return View("Index", model);
            }
        }

        [HttpPost]
        public ActionResult CopyToFuture(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;

            // load the future model from current data
            CircuitBreakerModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);

            if (model.Scada == null || model.Scada == "" || model.Scada.ToUpper() == "N")
            {
                ModelState.Remove("FlisrAutomationDevice");
                ModelState.Add("FlisrAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FlisrAutomationDevice", new ValueProviderResult("N", "N", null));
                model.FlisrAutomationDevice = "N";
            }

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult ScadaChanged(CircuitBreakerModel model, string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            setUiSettings(HtmlExtensions.PageMode.Future);

            ModelState.Remove("FlisrAutomationDevice");
            ModelState.Add("FlisrAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
            ModelState.SetModelValue("FlisrAutomationDevice", new ValueProviderResult("N", "N", null));
            model.FlisrAutomationDevice = "N";

            initializeDropDowns(model);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult AddComment(CommentHistoryModel model, string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;

            using (SettingsEntities db = new SettingsEntities())
            {
                addComment(db, globalID, SystemEnums.WorkType.NOTE, model.comments);
                db.SaveChanges();
            }

            return RedirectToAction("History", new { globalID = globalID, layerName = layerName });
        }

        [HttpPost]
        public ActionResult SettingsFiles(string globalID, string layerName)
        {
            HttpPostedFileBase file = Request.Files["fileSettings"];
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            string err = "SUCESS";
            if (file.FileName != null && file.FileName.Length > 0)
            {
                uploadFile(file, globalID, "Device Settings-Settings Configuration", out err);
            }
            else
            {
                err = Constants.SelectFileError;
            }
            return RedirectToAction("Files", new { globalID = globalID, layerName = layerName, settingsError = err, peerReviewError = "" });
        }

        [HttpPost]
        public ActionResult PeerReviewFiles(string globalID, string layerName)
        {
            HttpPostedFileBase file = Request.Files["filePeerReview"];
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            string err = "SUCESS";
            if (file.FileName != null && file.FileName.Length > 0)
            {
                uploadFile(file, globalID, "Device Settings-Peer Review", out err);
            }
            else
            {
                err = Constants.SelectFileError;
            }
            return RedirectToAction("Files", new { globalID = globalID, layerName = layerName, settingsError = "", peerReviewError = err });
        }

        #endregion


        #region "LoadSaveDevice"

        private void uploadFile(HttpPostedFileBase file, string globalID, string docType, out string error)
        {
            bool debug = false;
            error = "SUCESS";
            if (file.ContentLength > 0)
            {
                try
                {
                    if (!debug)
                    {
                        using (SettingsEntities db = new SettingsEntities())
                        {
                            SM_CIRCUIT_BREAKER e = db.SM_CIRCUIT_BREAKER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
                            string district = e.DISTRICT;
                            string division = e.DIVISION;

                            System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
                            district = textInfo.ToTitleCase(district);
                            division = textInfo.ToTitleCase(division);

                            Documentum.CreateDocument(globalID, DEVICE_NAME_DOCUMENTUM, e.OPERATING_NUM, district, division, docType, file);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utility.WriteLog(ex.ToString());
                    error = Constants.FileUploadError;
                }
            }
            else
            {
                error = Constants.FileContentError;
            }
        }


        private CircuitBreakerModel getDevice(string globalID, HtmlExtensions.PageMode pageMode)
        {
            CircuitBreakerModel model = new CircuitBreakerModel();

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_CIRCUIT_BREAKER e = db.SM_CIRCUIT_BREAKER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError(DEVICE_NAME, globalID));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        private void saveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, CircuitBreakerModel model)
        {
            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            SM_CIRCUIT_BREAKER entity = db.SM_CIRCUIT_BREAKER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
            entity.CURRENT_FUTURE = currentFuture;
            model.PopulateEntityFromModel(entity);
            entity.DATE_MODIFIED = DateTime.Now;
        }

        private SM_CIRCUIT_BREAKER_HIST saveHistory(SettingsEntities db, string globalID, CircuitBreakerModel model)
        {
            SM_CIRCUIT_BREAKER_HIST history = new SM_CIRCUIT_BREAKER_HIST();
            SM_CIRCUIT_BREAKER e = db.SM_CIRCUIT_BREAKER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
            model.PopulateHistoryFromEntity(history, e);
            history.GLOBAL_ID = globalID;
            return history;
        }

        private void addComment(SettingsEntities db, string globalID, SystemEnums.WorkType commentType, string comments, decimal historyID = 0)
        {
            SM_COMMENT_HIST comment = new SM_COMMENT_HIST();
            comment.PERFORMED_BY = Security.CurrentUser;
            comment.WORK_DATE = DateTime.Now;
            comment.ENTRY_DATE = comment.WORK_DATE;
            comment.WORK_TYPE = commentType.ToString();

            comment.COMMENTS = comments;
            comment.HIST_ID = historyID;

            if (commentType == SystemEnums.WorkType.SETT)
            {
                comment.DEVICE_HIST_TABLE_NAME = DEVICE_HIST_TABLE_NAME;
            }
            else
            {
                comment.GLOBAL_ID = globalID;
                comment.DEVICE_TABLE_NAME = DEVICE_TABLE_NAME;
            }

            db.SM_COMMENT_HIST.AddObject(comment);
        }

        #endregion


        #region "Load UI"

        private void setUiSettings(HtmlExtensions.PageMode pageMode)
        {
            ViewBag.PageMode = pageMode.ToString().ToUpper();
            ViewBag.ControllerName = DEVICE_NAME;
            ViewBag.OperatingNum = SiteCache.OperatingNumber(ViewBag.GlobalID, DEVICE_NAME);
            switch (pageMode)
            {
                case HtmlExtensions.PageMode.Current:
                    ViewBag.Title = "Current Settings for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    ViewBag.CurrentClass = "selected";
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.Future:
                    ViewBag.Title = "Future Settings for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    ViewBag.FutureClass = "selected";
                    ViewBag.IsDisabled = false;
                    break;
                case HtmlExtensions.PageMode.History:
                    ViewBag.Title = "History for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.GIS:
                    ViewBag.Title = "GIS Attributes for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    break;
                case HtmlExtensions.PageMode.File:
                    ViewBag.Title = "Files for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    break;
                case HtmlExtensions.PageMode.EngineeringInfo:
                    ViewBag.Title = ViewBag.OperatingNum;
                    break;
                default:
                    break;
            }

            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsDisabled = true;
        }

        private void initializeDropDowns(CircuitBreakerModel sectModel)
        {
            sectModel.DistributionPlanningAreaList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "DPA_CD"), "Key", "Value");
            sectModel.EngineeringDocumentList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ENGINEERING_DOCUMENT"), "Key", "Value");
            sectModel.GrdBkRelayCodeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_BK_RELAY_CD"), "Key", "Value");
            sectModel.GrdBkRelayTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_BK_RELAY_TYPE"), "Key", "Value");
            sectModel.GrdPrRelayCodeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_PR_RELAY_CD"), "Key", "Value");
            sectModel.GrdPrRelayTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_PR_RELAY_TYPE"), "Key", "Value");
            sectModel.PhaBkRelayCodeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_BK_RELAY_CD"), "Key", "Value");
            sectModel.PhaBkRelayTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_BK_RELAY_TYPE"), "Key", "Value");
            sectModel.PhaRrRelayCodeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_PR_RELAY_CD"), "Key", "Value");
            sectModel.PhaRrRelayTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_PR_RELAY_TYPE"), "Key", "Value");
            sectModel.ScadaRadioManufacturerList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "RADIO_MANF_CD"), "Key", "Value");
            sectModel.ScadaTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "SCADA_TYPE"), "Key", "Value");

            sectModel.RTUManufactureList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "RTU_MANF_CD"), "Key", "Value");
            sectModel.OperatingModeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OPERATING_MODE"), "Key", "Value");
            //add
            sectModel.GRDPROPSTOLOCKOUTList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_PR_OPS_TO_LOCKOUT"), "Key", "Value");
            sectModel.PHA_PR_OPS_TO_LOCKOUTList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_PR_OPS_TO_LOCKOUT"), "Key", "Value");
            sectModel.PHA_BK_OPS_TO_LOCKOUTList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_BK_OPS_TO_LOCKOUT"), "Key", "Value");
            sectModel.GRD_BK_OPS_TO_LOCKOUTList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_BK_OPS_TO_LOCKOUT"), "Key", "Value");
           
//
//
//
            sectModel.DropDownPostbackScriptScada = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ScadaChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"';form.submit();";
        }

        private CircuitLoadModel GetLoadInfo(decimal CircuitBreakerRowId)
        {
            CircuitLoadModel model = new CircuitLoadModel();
            model.CircuitBreakerRowId = CircuitBreakerRowId;
            using (SettingsEntities db = new SettingsEntities())
            {
                var CircuitLoadData = db.SM_CIRCUIT_LOAD.Where(t => t.REF_DEVICE_ID == CircuitBreakerRowId).FirstOrDefault();
                if (CircuitLoadData == null)
                {
                    ViewBag.NoDataFoundMsg = "NO_DATA_FOUND";
                    return model;

                } var CircuitLoadHistData = db.SM_CIRCUIT_LOAD_HIST.Where(t => t.SM_CIRCUIT_LOAD_ID == CircuitLoadData.ID).OrderByDescending(t => t.PEAK_DTM);
                
                model.PopulateModelFromEntity(CircuitLoadData, CircuitLoadHistData!=null?CircuitLoadHistData.ToList():null);

                return model;
            }
        }

        private bool GetHighSideVoltage(string layerName, string globalID)
        {
            int? HighSideVoltage = null;
            var IsFedStationChecked = false;
            var KEY = "HIGHSIDEVOLTAGE";
            Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            Dictionary<string, string> attributeValues = new GISService().GetProperties(globalID, layer.Item1, layer.Item2);

            if (attributeValues.ContainsKey(KEY))
            {
                HighSideVoltage = Convert.ToInt32(attributeValues[KEY]);
            }

            if (HighSideVoltage != null)
            {
                if (HighSideVoltage < 50)
                {
                    IsFedStationChecked = true;
                }
            }

            return IsFedStationChecked;
        }

        private decimal GetCircuitBreakerRowIdForLoadInfo(string GlobalID)
        {
            decimal CircuitBreakerId = 0;
            if (!string.IsNullOrEmpty(GlobalID))
            {
                using (SettingsEntities db = new SettingsEntities())
                {
                    var CircuitBreakerData = db.SM_CIRCUIT_BREAKER.Where(r => r.GLOBAL_ID == GlobalID && r.CURRENT_FUTURE == "C");
                    if (CircuitBreakerData != null)
                    {
                        CircuitBreakerId = CircuitBreakerData.FirstOrDefault().ID;
                    }
                }
            }
            return CircuitBreakerId;
        }
        #endregion


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
