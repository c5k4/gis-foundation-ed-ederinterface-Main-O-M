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

    public class NetworkProtectorController : Controller
    {
        private const string DEVICE_NAME = "NetworkProtector";
        private const string DEVICE_TABLE_NAME = "SM_NETWORK_PROTECTOR";
        private const string DEVICE_HIST_TABLE_NAME = "SM_NETWORK_PROTECTOR_HIST";

        #region "Action Methods"

        public ActionResult Index(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            NetworkProtectorModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);

            setUiSettings(HtmlExtensions.PageMode.Current);
            // model.WattVarTrip = 0; 
            this.initializeDropDowns(model);
            if (model.ClosingMode == null)
                model.ClosingMode = "CCC";
            return View(model);
        }

        public ActionResult Future(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;

            NetworkProtectorModel model = getDevice(globalID, HtmlExtensions.PageMode.Future);

            //  model.WattVarTrip = 1; 
            setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);
            if (model.ClosingMode == null)
                model.ClosingMode = "CCC";
            return View("Index", model);
        }

        public ActionResult GIS(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            setUiSettings(HtmlExtensions.PageMode.GIS);
            NetworkProtectorModel model = new NetworkProtectorModel();

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

                List<History> net_hist = (from c in db.SM_COMMENT_HIST
                                          join n_h in db.SM_NETWORK_PROTECTOR_HIST.Where(s => s.GLOBAL_ID == globalID) on c.HIST_ID equals n_h.ID
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

                ch.CommentHistory.AddRange(net_hist);
                ch.CommentHistory = ch.CommentHistory.OrderByDescending(c => c.EntryDate).ToList();
            }

            return View("History", ch);
        }

        public ActionResult HistoryDetails(string globalID, string layerName, decimal ID)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;

            NetworkProtectorModel model = new NetworkProtectorModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_NETWORK_PROTECTOR_HIST e = db.SM_NETWORK_PROTECTOR_HIST.FirstOrDefault(s => s.GLOBAL_ID == globalID && s.ID == ID);
                model.PopulateModelFromHistoryEntity(e);
            }

            setUiSettings(HtmlExtensions.PageMode.History);
            this.initializeDropDowns(model);
            if (model.ClosingMode == null)
                model.ClosingMode = "CCC";
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
                SM_NETWORK_PROTECTOR e = db.SM_NETWORK_PROTECTOR.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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
            setUiSettings(HtmlExtensions.PageMode.File);
            return View("Files");
        }
        #endregion


        #region "Post Methods"

        [HttpPost]
        public ActionResult SaveFuture(NetworkProtectorModel model, string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            this.setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

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
                            SM_NETWORK_PROTECTOR_HIST history = this.saveHistory(db, globalID, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);
                            db.SM_NETWORK_PROTECTOR_HIST.AddObject(history);
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
            NetworkProtectorModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);
            if (model.ClosingMode == null)
                model.ClosingMode = "CCC";
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
                            SM_NETWORK_PROTECTOR e = db.SM_NETWORK_PROTECTOR.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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

        private NetworkProtectorModel getDevice(string globalID, HtmlExtensions.PageMode pageMode)
        {
            NetworkProtectorModel model = new NetworkProtectorModel();

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_NETWORK_PROTECTOR e = db.SM_NETWORK_PROTECTOR.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError(DEVICE_NAME, globalID));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        private void saveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, NetworkProtectorModel model)
        {
            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            SM_NETWORK_PROTECTOR entity = db.SM_NETWORK_PROTECTOR.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
            entity.CURRENT_FUTURE = currentFuture;
            model.PopulateEntityFromModel(entity);
            entity.DATE_MODIFIED = DateTime.Now;
        }

        private SM_NETWORK_PROTECTOR_HIST saveHistory(SettingsEntities db, string globalID, NetworkProtectorModel model)
        {
            SM_NETWORK_PROTECTOR_HIST history = new SM_NETWORK_PROTECTOR_HIST();
            SM_NETWORK_PROTECTOR e = db.SM_NETWORK_PROTECTOR.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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
                    ViewBag.Title = "Current Settings for Network Protector" + " - " + ViewBag.OperatingNum;
                    ViewBag.CurrentClass = "selected";
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.Future:
                    ViewBag.Title = "Future Settings for Network Protector" + " - " + ViewBag.OperatingNum;
                    ViewBag.FutureClass = "selected";
                    ViewBag.IsDisabled = false;
                    break;
                case HtmlExtensions.PageMode.History:
                    ViewBag.Title = "History for Network Protector" + " - " + ViewBag.OperatingNum;
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.GIS:
                    ViewBag.Title = "GIS Attributes for Network Protector" + " - " + ViewBag.OperatingNum;
                    break;
                case HtmlExtensions.PageMode.File:
                    ViewBag.Title = "Files for NetworkProtector" + " - " + ViewBag.OperatingNum;
                    break;
                default:
                    break;
            }

            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsDisabled = true;
        }
        [HttpPost]
        public ActionResult ControlTypeChanged(NetworkProtectorModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            setUiSettings(HtmlExtensions.PageMode.Future);

            //if (model.IniEmulatedDevice != null || model.Test2EmulatedDevice != null)
            //{
            //    //ModelState.Remove("FlisrDevice");
            //    //ModelState.Add("FlisrDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
            //    //ModelState.SetModelValue("FlisrDevice", new ValueProviderResult("N", "N", null));

            //}

            initializeDropDowns(model);
            //if (model.ClosingMode == null)
            //    model.ClosingMode = "CCC";
            return View("Index", model);
        }
        [HttpPost]
        public ActionResult IniLowCutOffChkChanged(NetworkProtectorModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("WattVarTrip");
            if (model.WattVarTrip == 0)
            {
                model.WattVarTrip = 0;
            }
            else
            {
                model.WattVarTrip = 1;
                //ModelState.Remove("WattVarTrip");
                //model.IniLowcutoffCurrA = null;
            }
            return ControlTypeChanged(model, globalID, layerName, layerType);
        }
        private void initializeDropDowns(NetworkProtectorModel sectModel)
        {
            string layerType = "layerType=" + @ViewBag.layerType;
            sectModel.ClosingModeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "CLOSEMODE"), "Key", "Value");
            // sectModel.CtRatioList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "CT_Ratio"), "Key", "Value");

            sectModel.TripModeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TRIPMODE"), "Key", "Value");
            //  sectModel.WattVarTripCharList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "WATTVARTRIP"), "Key", "Value");

            sectModel.DropDownPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ControlTypeChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            sectModel.IniLowChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/IniLowCutOffChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
