//*********** ME Q4 2019 Release - DA#1*******************
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SettingsApp.Models;
using SettingsApp.Common;
using System.Diagnostics;
using System.Drawing;

namespace SettingsApp.Controllers
{
    public class MSOSwitchController : Controller
    { 
        private const string DEVICE_NAME = "MSOSwitch";
        private const string ACTUAL_DEVICE_NAME = "Switch";
        private const string DEVICE_TABLE_NAME = "SM_SWITCH_MSO";
        private const string DEVICE_HIST_TABLE_NAME = "SM_SWITCH_MSO_HIST";

        #region "Action Methods"

        public ActionResult Index(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            MSOSwitchModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
            setUiSettings(HtmlExtensions.PageMode.Current);
            this.initializeDropDowns(model);

            return View(model);
        }

        public ActionResult Future(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            MSOSwitchModel model = getDevice(globalID, HtmlExtensions.PageMode.Future);
            setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

            return View("Index", model);
        }

        public ActionResult GIS(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            int lyID;
            setUiSettings(HtmlExtensions.PageMode.GIS);
            MSOSwitchModel model = new MSOSwitchModel();
            int LayerID;
            Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            if (layerType == "P")
            {
                layerName = "Proposed Switch";


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

                List<History> swi_hist = (from c in db.SM_COMMENT_HIST
                                          join s_h in db.SM_SWITCH_MSO_HIST.Where(s => s.GLOBAL_ID == globalID) on c.HIST_ID equals s_h.ID
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

                ch.CommentHistory.AddRange(swi_hist);
                ch.CommentHistory = ch.CommentHistory.OrderByDescending(c => c.EntryDate).ToList();
            }

            return View("History", ch);
        }

        public ActionResult HistoryDetails(string globalID, string layerName, decimal ID, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            MSOSwitchModel model = new MSOSwitchModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_SWITCH_MSO_HIST e = db.SM_SWITCH_MSO_HIST.FirstOrDefault(s => s.GLOBAL_ID == globalID && s.ID == ID);
                model.PopulateModelFromHistoryEntity(e);
            }

            setUiSettings(HtmlExtensions.PageMode.History);
            this.initializeDropDowns(model);

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
                SM_SWITCH_MSO e = db.SM_SWITCH_MSO.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
                if (e != null && !string.IsNullOrEmpty(e.DISTRICT) && !string.IsNullOrEmpty(e.DIVISION) && !string.IsNullOrEmpty(e.OPERATING_NUM))
                {
                    ViewBag.ShowInsufficientMetaDataRequirement = false;
                    ViewBag.D2SettingsURL = Constants.BuildDocumentumUrl(false, e.DIVISION, e.DISTRICT, ACTUAL_DEVICE_NAME, e.OPERATING_NUM);
                    ViewBag.DBPeerReviewURL = Constants.BuildDocumentumUrl(true, e.DIVISION, e.DISTRICT, ACTUAL_DEVICE_NAME, e.OPERATING_NUM);
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
        public ActionResult SaveFuture(MSOSwitchModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            this.setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

            if (model.SCADA == null || model.SCADA.ToUpper() == "N" || model.FLISRAutomationDevice == null || model.FLISRAutomationDevice == "")
            {
                ModelState.Remove("FLISRAutomationDevice");
                ModelState.Add("FLISRAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FLISRAutomationDevice", new ValueProviderResult("N", "N", null));
                model.FLISRAutomationDevice = "N";
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
                            SM_SWITCH_MSO_HIST history = this.saveHistory(db, globalID, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);
                            db.SM_SWITCH_MSO_HIST.AddObject(history);
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
        public ActionResult CopyToFuture(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            // load the future model from current data
            MSOSwitchModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);

            if (model.SCADA == null || model.SCADA == "" || model.SCADA.ToUpper() == "N")
            {
                ModelState.Remove("FLISRAutomationDevice");
                ModelState.Add("FLISRAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FLISRAutomationDevice", new ValueProviderResult("N", "N", null));
                model.FLISRAutomationDevice = "N";
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
                addComment(db, globalID, SystemEnums.WorkType.NOTE, model.comments);
                db.SaveChanges();
            }

            return RedirectToAction("History", new { globalID = globalID, layerName = layerName, layerType = layerType });
        }

        [HttpPost]
        public ActionResult SwitchTypeChanged(MSOSwitchModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            setUiSettings(HtmlExtensions.PageMode.Future);

            if (model.SCADA == null || model.SCADA.ToUpper() == "N" || model.FLISRAutomationDevice == null || model.FLISRAutomationDevice == "")
            {
                ModelState.Remove("FLISRAutomationDevice");
                ModelState.Add("FLISRAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                ModelState.SetModelValue("FLISRAutomationDevice", new ValueProviderResult("N", "N", null));
                model.FLISRAutomationDevice = "N";
            }

            initializeDropDowns(model);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult ScadaChanged(MSOSwitchModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            setUiSettings(HtmlExtensions.PageMode.Future);

            ModelState.Remove("FLISRAutomationDevice");
            ModelState.Add("FLISRAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
            ModelState.SetModelValue("FLISRAutomationDevice", new ValueProviderResult("N", "N", null));
            model.FLISRAutomationDevice = "N";

            initializeDropDowns(model);
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
                            SM_SWITCH_MSO e = db.SM_SWITCH_MSO.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
                            Documentum.CreateDocument(globalID, ACTUAL_DEVICE_NAME, e.OPERATING_NUM, e.DISTRICT, e.DIVISION, docType, file);
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

        private MSOSwitchModel getDevice(string globalID, HtmlExtensions.PageMode pageMode)
        {
            MSOSwitchModel model = new MSOSwitchModel();

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_SWITCH_MSO e = db.SM_SWITCH_MSO.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError(DEVICE_NAME, globalID));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        private void saveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, MSOSwitchModel model)
        {
            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            SM_SWITCH_MSO entity = db.SM_SWITCH_MSO.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
            entity.CURRENT_FUTURE = currentFuture;
            model.PopulateEntityFromModel(entity);
            entity.DATE_MODIFIED = DateTime.Now;
        }

        private SM_SWITCH_MSO_HIST saveHistory(SettingsEntities db, string globalID, MSOSwitchModel model)
        {
            SM_SWITCH_MSO_HIST history = new SM_SWITCH_MSO_HIST();
            SM_SWITCH_MSO e = db.SM_SWITCH_MSO.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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
                    if (ViewBag.layerType == "P")
                    {

                        ViewBag.Title = "Current Settings for (Proposed) " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "Current Settings for " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    ViewBag.CurrentClass = "selected";
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.Future:
                    if (ViewBag.layerType == "P")
                    {
                        ViewBag.Title = "Future Settings (Proposed)  " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "Future Settings for " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    ViewBag.FutureClass = "selected";
                    ViewBag.IsDisabled = false;
                    break;
                case HtmlExtensions.PageMode.History:
                    if (ViewBag.layerType == "P")
                    {
                        ViewBag.Title = "History for (Proposed) " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "History for " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.GIS:
                    if (ViewBag.layerType == "P")
                    {
                        ViewBag.Title = "GIS Attributes for (Proposed) " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "GIS Attributes for " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    break;
                case HtmlExtensions.PageMode.File:
                    if (ViewBag.layerType == "P")
                    {
                        ViewBag.Title = "Files for (Proposed) " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "Files for " + ACTUAL_DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    break;
                default:
                    break;
            }

            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsDisabled = true;
        }

        private void initializeDropDowns(MSOSwitchModel m)
        {
            m.AtsFeatureList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "ATS_FEATURE"), "Key", "Value");
            m.ScadaTypeList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "SCADA_TYPE"), "Key", "Value");
            m.UnbalanceDetectList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "UNBALANCE_DETECT"), "Key", "Value");
            m.EngineeringDocumentList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "ENGINEERING_DOCUMENT"), "Key", "Value");
            m.OperatingModeList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "OPERATING_MODE"), "Key", "Value");
            m.SCADARadioManufacturerList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "RADIO_MANF_CD"), "Key", "Value");
            m.SwitchTypeList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "SWITCH_TYPE"), "Key", "Value");
            m.ControlUnitTypeList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "CONTROL_UNIT_TYPE", m.SwitchType), "Key", "Value");
            m.SectionalizingFeatureList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "SECTIONALIZING_FEATURE"), "Key", "Value");
            m.ATSCapableList = new SelectList(SiteCache.GetYesNoLookUp, "Key", "Value");
            m.FieldsToDisplay = new HashSet<string>();
            if (m.SwitchType != null && m.ATSCapable != null)
            {
                m.FieldsToDisplay = SiteCache.GetFieldsToDisplayForMSOSwitch(m.SwitchType, m.ATSCapable);
            }
            else
            {
                m.FieldsToDisplay = SiteCache.GetFieldsToDisplayForMSOSwitch("22", "N");
            }

            m.RTUManufactureList = new SelectList(SiteCache.GetLookUpValues("SM_SWITCH_MSO", "RTU_MANF_CD"), "Key", "Value");

            string layerType = "layerType=" + @ViewBag.layerType;

            m.DropDownPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/SwitchTypeChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            m.DropDownPostbackScriptScada = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ScadaChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";

            }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}