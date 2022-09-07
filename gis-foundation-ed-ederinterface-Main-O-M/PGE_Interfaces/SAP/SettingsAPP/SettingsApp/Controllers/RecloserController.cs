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

    public class RecloserController : Controller
    {
        private const string DEVICE_NAME = "Recloser";
        private const string DEVICE_TABLE_NAME = "SM_RECLOSER";
        private const string DEVICE_HIST_TABLE_NAME = "SM_RECLOSER_HIST";

        #region "Action Methods"

        public ActionResult Index(string globalID, string layerName, string layerType)
        {
            if (layerName == "Proposed Dynamic Protective Device")
            {
                layerType = "P";
            }
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;

            RecloserModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);

            getSubstationName();  //July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Current);
            this.initializeDropDowns(model);
            SetDefaultValues(model);

            return View(model);
        }

        public ActionResult Future(string globalID, string layerName, string layerType)
        {
            if (layerName == "Proposed Dynamic Protective Device")
            {
                layerType = "P";
            }
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            RecloserModel model = getDevice(globalID, HtmlExtensions.PageMode.Future);
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);
            SetDefaultValues(model);

            return View("Index", model);
        }

        public ActionResult GIS(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.GIS);
            RecloserModel model = new RecloserModel();

            Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            int LayerID;
            int lyID = SiteCache.GetLayerID(layerName).Item1;
            if (layerType == "P")
            {
                layerName = "Proposed Dynamic Protective Device";


                lyID = SiteCache.GetLayerID(layerName).Item1;
            }
            else
            {

                lyID = SiteCache.GetLayerID(layerName).Item1;

            }
            if (layerType == "C")
            {
                LayerID = lyID + 1;
            }
            else if (layerType == "O")
            {
                LayerID = lyID + 2;
            }
            else
            {
                LayerID = lyID + 0;
            }
            Dictionary<string, string> attributeValues = new GISService().GetProperties(globalID, LayerID, layer.Item2);


            if (attributeValues.ContainsKey("SUBTYPECD"))
            {
                if (!string.IsNullOrEmpty(attributeValues["SUBTYPECD"]))
                {
                    model.GISAttributes = SiteCache.GetLayerSubTypes(LayerID, int.Parse(attributeValues["SUBTYPECD"]), layer.Item2);
                }
            }
            else
            {
                model.GISAttributes = SiteCache.GetLayerAttributes(LayerID, layer.Item2);
            }
            foreach (GISAttributes a in model.GISAttributes)
            {
                if (attributeValues.ContainsKey(a.FieldName))
                    a.Value = attributeValues[a.FieldName];
            }
            return View(model);
        }

        public ActionResult History(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            getSubstationName();//July 2017 release
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

                List<History> rec_hist = (from c in db.SM_COMMENT_HIST
                                          join r_h in db.SM_RECLOSER_HIST.Where(s => s.GLOBAL_ID == globalID) on c.HIST_ID equals r_h.ID
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

                ch.CommentHistory.AddRange(rec_hist);
                ch.CommentHistory = ch.CommentHistory.OrderByDescending(c => c.EntryDate).ToList();
            }

            return View("History", ch);
        }

        public ActionResult HistoryDetails(string globalID, string layerName, decimal ID)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;

            RecloserModel model = new RecloserModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_RECLOSER_HIST e = db.SM_RECLOSER_HIST.FirstOrDefault(s => s.GLOBAL_ID == globalID && s.ID == ID);
                model.PopulateModelFromHistoryEntity(e);
            }

            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.History);
            this.initializeDropDowns(model);

            SetDefaultValues(model);
            return View("HistoryData", model);
        }

        public ActionResult DeleteComment(string globalID, string layerName, string layerType, decimal ID)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_COMMENT_HIST history = db.SM_COMMENT_HIST.First(c => c.ID == ID);
                db.SM_COMMENT_HIST.DeleteObject(history);
                db.SaveChanges();
            }

            return RedirectToAction("History", new { globalID = globalID, layerName = layerName, layerType = layerType });
        }

        public ActionResult Files(string globalID, string layerName, string layerType, string settingsError = null, string peerReviewError = null)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            ViewBag.SettingsFileError = (settingsError == null ? "" : settingsError);
            ViewBag.PeerReviewError = (peerReviewError == null ? "" : peerReviewError);
            ViewBag.SettingsFileUploaded = (settingsError != null && settingsError == "SUCESS" ? true : false);
            ViewBag.PeerReviewFileUploaded = (peerReviewError != null && peerReviewError == "SUCESS" ? true : false); ;

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_RECLOSER e = db.SM_RECLOSER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
                if (e != null && !string.IsNullOrEmpty(e.DISTRICT) && !string.IsNullOrEmpty(e.DIVISION) && !string.IsNullOrEmpty(e.OPERATING_NUM))
                {
                    ViewBag.ShowInsufficientMetaDataRequirement = false;
                    ViewBag.D2SettingsURL = Constants.BuildDocumentumUrl(false, e.DIVISION, e.DISTRICT, DEVICE_NAME, e.OPERATING_NUM);
                    ViewBag.DBPeerReviewURL = Constants.BuildDocumentumUrl(true, e.DIVISION, e.DISTRICT, DEVICE_NAME, e.OPERATING_NUM);
                }
                else
                    ViewBag.ShowInsufficientMetaDataRequirement = true;
            }
            ViewBag.DeviceType = DEVICE_NAME;
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.File);
            return View("Files");
        }
        #endregion


        #region "Post Methods"

        [HttpPost]
        public ActionResult SaveFuture(RecloserModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            getSubstationName();//July 2017 release
            this.setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);
            SetDefaultValues(model);

            if (model.Scada == null || model.Scada.ToUpper() == "N" || model.FlisrDevice == null || model.FlisrDevice == "")
            {
                ModelState.Remove("FlisrDevice");
                ModelState.Add("FlisrDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FlisrDevice", new ValueProviderResult("N", "N", null));
                model.FlisrDevice = "N";
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
                            SM_RECLOSER_HIST history = this.saveHistory(db, globalID, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);
                            db.SM_RECLOSER_HIST.AddObject(history);
                            db.SaveChanges();
                            this.addComment(db, globalID, SystemEnums.WorkType.SETT, "Release to Current", layerType, history.ID);
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
                            SetDefaultValues(model);
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
        public ActionResult CopyToFuture(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            // load the future model from current data
            RecloserModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);
            SetDefaultValues(model);

            if (model.Scada == null || model.Scada == "" || model.Scada.ToUpper() == "N")
            {
                ModelState.Remove("FlisrDevice");
                ModelState.Add("FlisrDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FlisrDevice", new ValueProviderResult("N", "N", null));
                model.FlisrDevice = "N";
            }

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult AddComment(CommentHistoryModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            using (SettingsEntities db = new SettingsEntities())
            {
                addComment(db, globalID, SystemEnums.WorkType.NOTE, model.comments, layerType);
                db.SaveChanges();
            }

            return RedirectToAction("History", new { globalID = globalID, layerName = layerName, layerType = layerType });
        }

        [HttpPost]
        public ActionResult ControlTypeChanged(RecloserModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);

            if (model.Scada == null || model.Scada.ToUpper() == "N" || model.FlisrDevice == null || model.FlisrDevice == "")
            {
                ModelState.Remove("FlisrDevice");
                ModelState.Add("FlisrDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FlisrDevice", new ValueProviderResult("N", "N", null));
                model.FlisrDevice = "N";
            }

            initializeDropDowns(model);
            SetDefaultValues(model);
            return View("Index", model);

            //if (model.ControllerUnitType == "BECK" && model.ActiveProfile == "SEC")
            //{
            //    //if(model.FaultCurrentOnly
            //}

        }

        //[HttpPost]
        //public ActionResult ScadaChanged(RecloserModel model, string globalID, string layerName, string layerType)
        //{
        //    ViewBag.GlobalID = globalID;
        //    ViewBag.LayerName = layerName;
        //    ViewBag.layerType = layerType;
        //    getSubstationName();//July 2017 release
        //    setUiSettings(HtmlExtensions.PageMode.Future);

        //    ModelState.Remove("FlisrDevice");
        //    ModelState.Add("FlisrDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
        //    ModelState.SetModelValue("FlisrDevice", new ValueProviderResult("N", "N", null));
        //    model.FlisrDevice = "N";

        //    initializeDropDowns(model);
        //    return View("Index", model);
        //}
        [HttpPost]
        public ActionResult SfgCdNormalChanged(RecloserModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);
            SetDefaultValues(model);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult SfgCdAlt1Changed(RecloserModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);
            SetDefaultValues(model);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult SfgCdAlt3Changed(RecloserModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);
            SetDefaultValues(model);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult BlockingCutChanged(RecloserModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);
            SetDefaultValues(model);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult SettingsFiles(string globalID, string layerName, string layerType)
        {
            HttpPostedFileBase file = Request.Files["fileSettings"];
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
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
        public ActionResult PeerReviewFiles(string globalID, string layerName, string layerType)
        {
            HttpPostedFileBase file = Request.Files["filePeerReview"];
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
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

        //INC000004115222 start
        [HttpPost]
        public ActionResult CopyNormalToAlt1(RecloserModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            if (model.Scada == null || model.Scada.ToUpper() == "N" || model.FlisrDevice == null || model.FlisrDevice == "")
            {
                ModelState.Remove("FlisrDevice");
                ModelState.Add("FlisrDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FlisrDevice", new ValueProviderResult("N", "N", null));
                model.FlisrDevice = "N";
            }
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);
            SetDefaultValues(model);
            //Copy Phase fields
            ModelState.Remove("AltPhaMinTrip");
            model.AltPhaMinTrip = model.PhaMinTrip;
            ModelState.Remove("AltPhaInstTrip");
            model.AltPhaInstTrip = model.PhaInstTrip;
            ModelState.Remove("AltTcc1FastCurvesUsed");
            model.AltTcc1FastCurvesUsed = model.Tcc1FastCurvesUsed;
            ModelState.Remove("AltPhaFastCurveOps");
            model.AltPhaFastCurveOps = model.PhaFastCurveOps;
            ModelState.Remove("AltPhaFastCurve");
            model.AltPhaFastCurve = model.PhaFastCurve;
            ModelState.Remove("AltPhaVmulFast");
            model.AltPhaVmulFast = model.PhaTmulFast;
            ModelState.Remove("AltPhaTaddFast");
            model.AltPhaTaddFast = model.PhaTaddFast;
            ModelState.Remove("AltTcc2SlowCurvesUsed");
            model.AltTcc2SlowCurvesUsed = model.Tcc2SlowCurvesUsed;
            ModelState.Remove("AltPhaSlowCurveOps");
            model.AltPhaSlowCurveOps = model.PhaSlowCurveOps;
            ModelState.Remove("AltPhaSlowCurve");
            model.AltPhaSlowCurve = model.PhaSlowCurve;
            ModelState.Remove("AltPhaVmulSlow");
            model.AltPhaVmulSlow = model.PhaTmulSlow;
            ModelState.Remove("AltPhaTaddSlow");
            model.AltPhaTaddSlow = model.PhaTaddSlow;
            ModelState.Remove("AltPhaResponseTime");
            model.AltPhaResponseTime = model.PhaResponseTime;

            //Copy Ground fields
            ModelState.Remove("AltGrdMinTrip");
            model.AltGrdMinTrip = model.GrdMinTrip;
            ModelState.Remove("AltGrdInstTrip");
            model.AltGrdInstTrip = model.GrdInstTrip;
            ModelState.Remove("AltGrdFastCurveOps");
            model.AltGrdFastCurveOps = model.GrdOpFastCurveOps;
            ModelState.Remove("AltGrdFastCurve");
            model.AltGrdFastCurve = model.GrdFastCurve;
            ModelState.Remove("AltGrdVmulFast");
            model.AltGrdVmulFast = model.GrdTmulFast;
            ModelState.Remove("AltGrdTaddFast");
            model.AltGrdTaddFast = model.GrdTaddFast;
            ModelState.Remove("AltGrdSlowCurveOps");
            model.AltGrdSlowCurveOps = model.GrdSlowCurveOps;
            ModelState.Remove("AltGrdSlowCurve");
            model.AltGrdSlowCurve = model.GrdSlowCurve;
            ModelState.Remove("AltGrdVmulSlow");
            model.AltGrdVmulSlow = model.GrdTmulSlow;
            ModelState.Remove("AltGrdTaddSlow");
            model.AltGrdTaddSlow = model.GrdTaddSlow;
            ModelState.Remove("AltGrdResponseTime");
            model.AltGrdResponseTime = model.GrdResponseTime;
            ModelState.Remove("AltPhaOpsToLockout");
            model.AltPhaOpsToLockout = model.PhaOpsToLockout;

            ModelState.Remove("AltTotOpsToLockout");
            model.AltTotOpsToLockout = model.TotOpsToLockout;
            ModelState.Remove("AltReclose1Time");
            model.AltReclose1Time = model.Reclose1Time;
            ModelState.Remove("AltReclose2Time");
            model.AltReclose2Time = model.Reclose2Time;
            ModelState.Remove("AltReclose3Time");
            model.AltReclose3Time = model.Reclose3Time;
            ModelState.Remove("AltResetTime");
            model.AltResetTime = model.ResetTime;
            ModelState.Remove("AltRecloseRetryEnabled");
            model.AltRecloseRetryEnabled = model.RecloseRetryEnabled;
            ModelState.Remove("AltGrdOpsToLockout");
            model.AltGrdOpsToLockout = model.GrdOpsToLockout;

            ModelState.Remove("AltSfg");
            model.AltSfg = model.SgfCd;
            ModelState.Remove("AltSgfMinTripPercent");
            model.AltSgfMinTripPercent = model.SgfMinTripPercent;
            ModelState.Remove("AltSgfTimeDelay");
            model.AltSgfTimeDelay = model.SgfTimeDelay;
            ModelState.Remove("AltSgfLockout");
            model.AltSgfLockout = model.SgfLockout;

            ModelState.Remove("AltHighCurrentLockoutUsed");
            model.AltHighCurrentLockoutUsed = model.HighCurrentLockoutUsed;
            ModelState.Remove("AltHighCurrentLockoutPhase");
            model.AltHighCurrentLockoutPhase = model.HighCurrentLockoutPhase;
            ModelState.Remove("AltHighCurrentLockoutGround");
            model.AltHighCurrentLockoutGround = model.HighCurrentLockoutGround;

            ModelState.Remove("AltColdLoadPliUsed");
            model.AltColdLoadPliUsed = model.ColdLoadPliUsed;
            ModelState.Remove("AltColdLoadPliPhase");
            model.AltColdLoadPliPhase = model.ColdLoadPliPhase;
            ModelState.Remove("AltColdLoadPliCurvePha");
            model.AltColdLoadPliCurvePha = model.ColdLoadPliCurvePhase;
            ModelState.Remove("AltColdLoadPliGround");
            model.AltColdLoadPliGround = model.ColdLoadPliGround;
            ModelState.Remove("AltColdLoadPliCurveGround");
            model.AltColdLoadPliCurveGround = model.ColdLoadPliCurveGround;

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult CopyNormalToAlt2(RecloserModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            if (model.Scada == null || model.Scada.ToUpper() == "N" || model.FlisrDevice == null || model.FlisrDevice == "")
            {
                ModelState.Remove("FlisrDevice");
                ModelState.Add("FlisrDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FlisrDevice", new ValueProviderResult("N", "N", null));
                model.FlisrDevice = "N";
            }
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);
            SetDefaultValues(model);
            //Copy Phase fields
            ModelState.Remove("Alt2LsResetTime");
            model.Alt2LsResetTime = model.ResetTime;
            ModelState.Remove("Alt2LsOpsToLockout");
            model.Alt2LsOpsToLockout = model.TotOpsToLockout;
            ModelState.Remove("Alt2PhaMinTrip");
            model.Alt2PhaMinTrip = model.PhaMinTrip;
            ModelState.Remove("Alt2GrdMinTrip");
            model.Alt2GrdMinTrip = model.GrdMinTrip;
            ModelState.Remove("Alt2PhaInstTrip");
            model.Alt2PhaInstTrip = model.PhaInstTrip;
            ModelState.Remove("Alt2GrdInstTrip");
            model.Alt2GrdInstTrip = model.GrdInstTrip;

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult CopyNormalToAlt3(RecloserModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            if (model.Scada == null || model.Scada.ToUpper() == "N" || model.FlisrDevice == null || model.FlisrDevice == "")
            {
                ModelState.Remove("FlisrDevice");
                ModelState.Add("FlisrDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FlisrDevice", new ValueProviderResult("N", "N", null));
                model.FlisrDevice = "N";
            }
            getSubstationName();//July 2017 release
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);
            SetDefaultValues(model);
            //Copy Phase fields
            ModelState.Remove("Alt3PhaMinTrip");
            model.Alt3PhaMinTrip = model.PhaMinTrip;
            ModelState.Remove("Alt3PhaInstTrip");
            model.Alt3PhaInstTrip = model.PhaInstTrip;
            ModelState.Remove("Alt3Tcc1FastCurvesUsed");
            model.Alt3Tcc1FastCurvesUsed = model.Tcc1FastCurvesUsed;
            ModelState.Remove("Alt3PhaFastCurveOps");
            model.Alt3PhaFastCurveOps = model.PhaFastCurveOps;
            ModelState.Remove("Alt3PhaFastCurve");
            model.Alt3PhaFastCurve = model.PhaFastCurve;
            ModelState.Remove("Alt3PhaVmulFast");
            model.Alt3PhaVmulFast = model.PhaTmulFast;
            ModelState.Remove("Alt3PhaTaddFast");
            model.Alt3PhaTaddFast = model.PhaTaddFast;
            ModelState.Remove("Alt3Tcc2SlowCurvesUsed");
            model.Alt3Tcc2SlowCurvesUsed = model.Tcc2SlowCurvesUsed;
            ModelState.Remove("Alt3PhaSlowCurveOps");
            model.Alt3PhaSlowCurveOps = model.PhaSlowCurveOps;
            ModelState.Remove("Alt3PhaSlowCurve");
            model.Alt3PhaSlowCurve = model.PhaSlowCurve;
            ModelState.Remove("Alt3PhaVmulSlow");
            model.Alt3PhaVmulSlow = model.PhaTmulSlow;
            ModelState.Remove("Alt3PhaTaddSlow");
            model.Alt3PhaTaddSlow = model.PhaTaddSlow;

            //Copy Ground fields
            ModelState.Remove("Alt3GrdMinTrip");
            model.Alt3GrdMinTrip = model.GrdMinTrip;
            ModelState.Remove("Alt3GrdInstTrip");
            model.Alt3GrdInstTrip = model.GrdInstTrip;
            ModelState.Remove("Alt3GrdFastCurveOps");
            model.Alt3GrdFastCurveOps = model.GrdOpFastCurveOps;
            ModelState.Remove("Alt3GrdFastCurve");
            model.Alt3GrdFastCurve = model.GrdFastCurve;
            ModelState.Remove("Alt3GrdVmulFast");
            model.Alt3GrdVmulFast = model.GrdTmulFast;
            ModelState.Remove("Alt3GrdTaddFast");
            model.Alt3GrdTaddFast = model.GrdTaddFast;
            ModelState.Remove("Alt3GrdSlowCurveOps");
            model.Alt3GrdSlowCurveOps = model.GrdSlowCurveOps;
            ModelState.Remove("Alt3GrdSlowCurve");
            model.Alt3GrdSlowCurve = model.GrdSlowCurve;
            ModelState.Remove("Alt3GrdVmulSlow");
            model.Alt3GrdVmulSlow = model.GrdTmulSlow;
            ModelState.Remove("Alt3GrdTaddSlow");
            model.Alt3GrdTaddSlow = model.GrdTaddSlow;

            ModelState.Remove("Alt3TotOpsToLockout");
            model.Alt3TotOpsToLockout = model.TotOpsToLockout;
            ModelState.Remove("Alt3Reclose1Time");
            model.Alt3Reclose1Time = model.Reclose1Time;
            ModelState.Remove("Alt3Reclose2Time");
            model.Alt3Reclose2Time = model.Reclose2Time;
            ModelState.Remove("Alt3Reclose3Time");
            model.Alt3Reclose3Time = model.Reclose3Time;
            ModelState.Remove("Alt3ResetTime");
            model.Alt3ResetTime = model.ResetTime;
            ModelState.Remove("Alt3RecloseRetryEnabled");
            model.Alt3RecloseRetryEnabled = model.RecloseRetryEnabled;

            ModelState.Remove("Alt3SensitiveGroundFault");
            model.Alt3SensitiveGroundFault = model.SgfCd;
            ModelState.Remove("Alt3SgfMinTripPercent");
            model.Alt3SgfMinTripPercent = model.SgfMinTripPercent;
            ModelState.Remove("Alt3SgfTimeDelay");
            model.Alt3SgfTimeDelay = model.SgfTimeDelay;
            ModelState.Remove("Alt3SgfLockout");
            model.Alt3SgfLockout = model.SgfLockout;

            ModelState.Remove("Alt3HighCurrentLockoutUsed");
            model.Alt3HighCurrentLockoutUsed = model.HighCurrentLockoutUsed;
            ModelState.Remove("Alt3HighCurrentLockoutPhase");
            model.Alt3HighCurrentLockoutPhase = model.HighCurrentLockoutPhase;
            ModelState.Remove("Alt3HighCurrentLockoutGround");
            model.Alt3HighCurrentLockoutGround = model.HighCurrentLockoutGround;

            ModelState.Remove("Alt3ColdLoadPliUsed");
            model.Alt3ColdLoadPliUsed = model.ColdLoadPliUsed;
            ModelState.Remove("Alt3ColdLoadPliPhase");
            model.Alt3ColdLoadPliPhase = model.ColdLoadPliPhase;
            ModelState.Remove("Alt3ColdLoadPliCurvePhase");
            model.Alt3ColdLoadPliCurvePhase = model.ColdLoadPliCurvePhase;
            ModelState.Remove("Alt3ColdLoadPliGround");
            model.Alt3ColdLoadPliGround = model.ColdLoadPliGround;
            ModelState.Remove("Alt3ColdLoadPliCurveGround");
            model.Alt3ColdLoadPliCurveGround = model.ColdLoadPliCurveGround;

            return View("Index", model);
        }
        //INC000004115222 end  

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
                            SM_RECLOSER e = db.SM_RECLOSER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
                            Documentum.CreateDocument(globalID, DEVICE_NAME, e.OPERATING_NUM, e.DISTRICT, e.DIVISION, docType, file);
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

        private RecloserModel getDevice(string globalID, HtmlExtensions.PageMode pageMode)
        {
            RecloserModel model = new RecloserModel();

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_RECLOSER e = db.SM_RECLOSER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError(DEVICE_NAME, globalID));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        private void saveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, RecloserModel model)
        {

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            SM_RECLOSER entity = db.SM_RECLOSER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
            entity.CURRENT_FUTURE = currentFuture;
            model.PopulateEntityFromModel(entity);
            entity.DATE_MODIFIED = DateTime.Now;
        }

        private SM_RECLOSER_HIST saveHistory(SettingsEntities db, string globalID, RecloserModel model)
        {
            SM_RECLOSER_HIST history = new SM_RECLOSER_HIST();
            SM_RECLOSER e = db.SM_RECLOSER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
            model.PopulateHistoryFromEntity(history, e);
            history.GLOBAL_ID = globalID;
            return history;
        }

        private void addComment(SettingsEntities db, string globalID, SystemEnums.WorkType commentType, string comments, string layerType, decimal historyID = 0)
        {
            ViewBag.layerType = layerType;
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

        //July 2017 release - Settings for Substation Reclosers
        private void getSubstationName()
        {
            string substationName = "";
            if (ViewBag.LayerName.ToUpper() == "Interrupting Device - Recloser".ToUpper() || ViewBag.LayerName == "Interrupting Device - Recloser - Open".ToUpper())
            {
                Tuple<int, string> layer = SiteCache.GetLayerID(ViewBag.LayerName);
                var subName = new GISService().GetProperties(ViewBag.GlobalID, layer.Item1, layer.Item2)["SUBSTATIONNAME"];

                if (subName != null)
                    substationName = subName.ToString() + "-";
            }
            ViewBag.SubstationName = substationName;
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
                    if (ViewBag.layerType == "P")
                    {

                        ViewBag.Title = "Current Settings for (Proposed) " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "Current Settings for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    ViewBag.CurrentClass = "selected";
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.Future:
                    if (ViewBag.layerType == "P")
                    {
                        ViewBag.Title = "Future Settings (Proposed)  " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "Future Settings for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    ViewBag.FutureClass = "selected";
                    ViewBag.IsDisabled = false;
                    break;
                case HtmlExtensions.PageMode.History:
                    if (ViewBag.layerType == "P")
                    {
                        ViewBag.Title = "History for (Proposed) " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "History for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.GIS:
                    if (ViewBag.layerType == "P")
                    {
                        ViewBag.Title = "GIS Attributes for (Proposed) " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "GIS Attributes for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    break;
                case HtmlExtensions.PageMode.File:
                    if (ViewBag.layerType == "P")
                    {
                        ViewBag.Title = "Files for (Proposed) " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "Files for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    break;
                default:
                    break;
            }


            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsDisabled = true;
        }

        private void initializeDropDowns(RecloserModel sectModel)
        {
            sectModel.ControllerUnitTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "CONTROL_TYPE"), "Key", "Value");
            sectModel.OperatingModeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OPERATING_AS_CD"), "Key", "Value");
            sectModel.EngineeringDocumentList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ENGINEERING_DOCUMENT"), "Key", "Value");
            // sectModel.AltPhaInstTripList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_PHA_INST_TRIP_CD", sectModel.ControllerUnitType), "Key", "Value");
            // sectModel.Alt3PhaInstTripList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT3_PHA_INST_TRIP_CD", sectModel.ControllerUnitType), "Key", "Value");
            // sectModel.PhaInstTripList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_INST_TRIP_CD", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.AltPhaFastCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_PHA_FAST_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.Alt3PhaFastCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT3_PHA_FAST_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.PhaFastCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_FAST_CRV", sectModel.ControllerUnitType), "Key", "Value");
            //sectModel.Alt2PhaInstTripList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT2_PHA_INST_TRIP_CD", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.Alt2PhaCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT2_PHA_FAST_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.AltPhaSlowCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_PHA_SLOW_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.Alt3PhaSlowCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT3_PHA_SLOW_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.PhaSlowCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_SLOW_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.AltPhaResponseTimeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_PHA_RESP_TIME", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.PhaResponseTimeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_RESP_TIME", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.ScadaTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "SCADA_TYPE"), "Key", "Value");
            sectModel.ScadaRadioManufacturerList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "RADIO_MANF_CD"), "Key", "Value");
            // sectModel.AltGrdInstTripList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_GRD_INST_TRIP_CD", sectModel.ControllerUnitType), "Key", "Value");
            //sectModel.Alt3GrdInstTripList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT3_GRD_INST_TRIP_CD", sectModel.ControllerUnitType), "Key", "Value");
            // sectModel.GrdInstTripList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_INST_TRIP_CD", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.AltGrdFastCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_GRD_FAST_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.Alt3GrdFastCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT3_GRD_FAST_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.GrdFastCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_FAST_CRV", sectModel.ControllerUnitType), "Key", "Value");
            //sectModel.Alt2GrdInstTripList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT2_GRD_INST_TRIP_CD", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.Alt2GrdCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT2_GRD_FAST_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.AltGrdSlowCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_GRD_SLOW_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.Alt3GrdSlowCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT3_GRD_SLOW_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.GrdSlowCurveList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_SLOW_CRV", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.AltGrdResponseTimeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_GRD_RESP_TIME", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.GrdResponseTimeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_RESP_TIME", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.AltSfgList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_SGF_CD"), "Key", "Value");
            sectModel.Alt3SensitiveGroundFaultList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT3_SGF_CD"), "Key", "Value");
            sectModel.SgfCdList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "SGF_CD"), "Key", "Value", sectModel.ControllerUnitType);
            sectModel.AltHighCurrentLockoutPhaseList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_HIGH_CURRENT_LOCKOUT_PHA"), "Key", "Value");
            sectModel.Alt3HighCurrentLockoutPhaseList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT3_HIGH_CURRENT_LOCKOUT_PHA"), "Key", "Value");
            sectModel.HighCurrentLockoutPhaseList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "HIGH_CURRENT_LOCKOUT_PHA"), "Key", "Value");
            sectModel.AltHighCurrentLockoutGroundList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_HIGH_CURRENT_LOCKUOUT_GRD"), "Key", "Value");
            sectModel.Alt3HighCurrentLockoutGroundList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT3_HIGH_CURRENT_LOCKUOUT_GRD"), "Key", "Value");
            sectModel.HighCurrentLockoutGroundList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "HIGH_CURRENT_LOCKUOUT_GRD"), "Key", "Value");

            //DA# 200101 - ME Q1 2020 
            if (sectModel.ControllerUnitType == "BECK")
                sectModel.ActiveProfileList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ACTIVE_PROFILE", sectModel.ControllerUnitType), "Key", "Value");
            else
                sectModel.ActiveProfileList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ACTIVE_PROFILE"), "Key", "Value");
            sectModel.GrdCurveColdLoadPUList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PU_CURVE_GROU", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.PhaCurveColdLoadPUList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PU_CURVE_PHA", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.CountToTripVoltList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "COUT_T_TRIP_VOLT_LOSS", sectModel.ControllerUnitType), "Key", "Value");

            sectModel.GrdOpsToLockoutList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "GRD_LOCKOUT_OPS", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.PhaOpsToLockoutList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PHA_LOCKOUT_OPS", sectModel.ControllerUnitType), "Key", "Value");

            sectModel.RTUManufactureList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "RTU_MANF_CD"), "Key", "Value");

            //INC000004115018 START
            sectModel.AltGrdOpsToLockoutList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_GRD_LOCKOUT_OPS", sectModel.ControllerUnitType), "Key", "Value");
            sectModel.AltPhaOpsToLockoutList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ALT_PHA_LOCKOUT_OPS", sectModel.ControllerUnitType), "Key", "Value");
            //INC000004115018 END
            string layerType = "layerType=" + @ViewBag.layerType;

            //DA# 200101 - ME Q1 2020
            // sectModel.FieldsToDisplay = SiteCache.GetFieldsToDisplayForRecloser(sectModel.ControllerUnitType);
            if (sectModel.ControllerUnitType == "BECK" && sectModel.ActiveProfile != null && SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ACTIVE_PROFILE", "BECK").Keys.Contains(sectModel.ActiveProfile) == true)
            {
                sectModel.FieldsToDisplay = SiteCache.GetFieldsToDisplayForRecloser(sectModel.ControllerUnitType, sectModel.ActiveProfile);
            }
            else
            {
                sectModel.FieldsToDisplay = SiteCache.GetFieldsToDisplayForRecloser(sectModel.ControllerUnitType, "All");
            }

            sectModel.DropDownPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ControlTypeChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            sectModel.DropDownPostbackScriptScada = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ScadaChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        public JsonResult NormalRPG(string controlType)
        {
            SelectList SgfCdList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "SGF_CD"), "Key", "Value");
            Dictionary<string, SelectList> response = new Dictionary<string, SelectList>();
            response.Add("NormalSPG", SgfCdList);


            return Json(response);
        }

        private void SetDefaultValues(RecloserModel recModel)
        {
            if (recModel.ResetTimeFrmLock == null)
                recModel.ResetTimeFrmLock = 45;
            if (recModel.PhaFirstRec == null)
                recModel.PhaFirstRec = Convert.ToDecimal(10.00);
            if (recModel.GrdFirstRec == null)
                recModel.GrdFirstRec = Convert.ToDecimal(10.00);
            if (recModel.PhaSecondRec == null)
                recModel.PhaSecondRec = Convert.ToDecimal(15.00);
            if (recModel.GrdSecondRec == null)
                recModel.GrdSecondRec = Convert.ToDecimal(15.00);
            if (recModel.PhaDefiniteTime == null)
                recModel.PhaDefiniteTime = Convert.ToDecimal(0.01);
            if (recModel.GrdDefiniteTime == null)
                recModel.GrdDefiniteTime = Convert.ToDecimal(0.01);
            if (recModel.Phase == null)
                recModel.Phase = 450;
            if (recModel.Ground == null)
                recModel.Ground = 100;
            if (recModel.PhaOpsToLock == null)
                recModel.PhaOpsToLock = 3;
            if (recModel.GrdOpsToLock == null)
                recModel.GrdOpsToLock = 3;

            if (recModel.ResetTimeBeck == null)
                recModel.ResetTimeBeck = 45;
            if (recModel.ResetTimeFrmLock == null)
                recModel.ResetTimeFrmLock = 45;
            if (recModel.VoltLossDelay == null)
                recModel.VoltLossDelay = 5;
            if (recModel.CountToTripVolt == null)
                recModel.CountToTripVolt = 2;
            if (recModel.ResetTimerSecMode == null)
                recModel.ResetTimerSecMode = 45;
            if (recModel.FaultCurrentOnly == null && recModel.VoltageLossOnly == null && recModel.FaultCurVoltLoss == null)
                recModel.FaultCurVoltLoss = "Y";
            if (recModel.HLTEnabled == false)
                recModel.HLTEnabled = true;
        }

    }
}
