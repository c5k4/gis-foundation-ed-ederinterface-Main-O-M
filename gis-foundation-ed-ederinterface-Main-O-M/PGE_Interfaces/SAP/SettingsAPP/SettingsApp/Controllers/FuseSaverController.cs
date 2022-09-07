using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;
using System.Web.Script.Serialization; 
using SettingsApp.Models;
using SettingsApp.Common;

namespace SettingsApp.Controllers
{

    public class FuseSaverController : Controller
    {
        private const string DEVICE_NAME = "FuseSaver";
        private const string DEVICE_TABLE_NAME = "SM_RECLOSER_FS";
        private const string DEVICE_HIST_TABLE_NAME = "SM_RECLOSER_FS_HIST";

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

            FuseSaverModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);

            setUiSettings(HtmlExtensions.PageMode.Current);
            this.initializeDropDowns(model);
            if (model.PolicyType == null || model.PolicyType.ToString() == "")
            //  if (string.IsNullOrEmpty(model.NORMALMINTRIPCM) == true)
            {
                model.PolicyType = "OCO";
            }
            //String a = Convert.ToDecimal(model.NORMALMINTRIPCM);
            if (model.NORMALMINTRIPCM == null || model.NORMALMINTRIPCM.ToString() == "")
            //  if (string.IsNullOrEmpty(model.NORMALMINTRIPCM) == true)
            {
                model.NORMALMINTRIPCM = "2";
            }
            if (model.NORMALMAXFT == null || model.NORMALMAXFT.ToString() == "")
            // if (string.IsNullOrEmpty(model.NORMALMAXFT.ToString()) == true)
            {
                model.NORMALMAXFT = "5";
            }
            if (model.NORMALINSTTS == null || model.NORMALINSTTS.ToString() == "")
            //  if (string.IsNullOrEmpty(model.NORMALINSTTS) == true)
            {
                model.NORMALINSTTS = "OFF";
            }
            if (model.NORMALMINTT == null || model.NORMALMINTT.ToString() == "")
            // if (string.IsNullOrEmpty(model.NORMALMINTT) == true)
            {
                model.NORMALMINTT = "DIS";
            }
            //Fast
            if (model.FASTMODEMINTCM == null || model.FASTMODEMINTCM.ToString() == "")
            // if (string.IsNullOrEmpty(model.FASTMODEMINTCM) == true)
            {
                model.FASTMODEMINTCM = "2";
            }
            if (model.FASTMODEINSTTS == null || model.FASTMODEINSTTS.ToString() == "")
            //  if (string.IsNullOrEmpty(model.FASTMODEINSTTS) == true)
            {
                model.FASTMODEINSTTS = ".1";
            }
            if (model.FASTMODEMAXFT == null || model.FASTMODEMAXFT.ToString() == "")
            //  if (string.IsNullOrEmpty(model.FASTMODEMAXFT) == true)
            {
                model.FASTMODEMAXFT = "3";
            }
            if (model.FASTMODEMINTT == null || model.FASTMODEMINTT.ToString() == "")
            //if (string.IsNullOrEmpty(model.FASTMODEMINTT) == true)
            {
                model.FASTMODEMINTT = "DIS";
            }
            //OPS
            if (model.OSDEADTS == null || model.OSDEADTS.ToString() == "")
            //  if (model.OSDEADTS == null || model.OSDEADTS.ToString() == "")
            {
                model.OSDEADTS = 10;
            }
            if (model.OSENABLEEXTLEV == null || model.OSENABLEEXTLEV.ToString() == "")
            //  if (string.IsNullOrEmpty(model.OSENABLEEXTLEV) == true)
            {
                model.OSENABLEEXTLEV = "DIS";
            }
            if (model.OSMANUALGANOP == null || model.OSMANUALGANOP.ToString() == "")
            //  if (string.IsNullOrEmpty(model.OSENABLEEXTLEV) == true)
            // if (string.IsNullOrEmpty(model.OSMANUALGANOP) == true)
            {
                model.OSMANUALGANOP = "DIS";
            }
            if (model.OSDEENERLT == null || model.OSDEENERLT.ToString() == "")
            //  if (string.IsNullOrEmpty(model.OSENABLEEXTLEV) == true)
            //if (string.IsNullOrEmpty(model.OSDEENERLT) == true)
            {
                model.OSDEENERLT = "60";
            }
            if (model.OSRECLAIMTIME.ToString() == "" || model.OSRECLAIMTIME.ToString() == null)
            {
                model.OSRECLAIMTIME = 45;
            }
            if (model.OSINRUSHRM == null || model.OSINRUSHRM.ToString() == "")
            // if (string.IsNullOrEmpty(model.OSINRUSHRM) == true)
            {
                model.OSINRUSHRM = "3";
            }

            if (string.IsNullOrEmpty(model.OSINRUSHRT.ToString()) == true || model.OSINRUSHRT.ToString() == "0")
            {
                model.OSINRUSHRT = 20;
            }
            if (string.IsNullOrEmpty(model.OSCAPCHARGE) == true)
            {
                model.OSCAPCHARGE = "ALWD";// "ALLOWED";
            }
            //Level
            if (string.IsNullOrEmpty(model.LEVERUPPM) == true)
            {
                model.LEVERUPPM = "NN";
            }
            if (string.IsNullOrEmpty(model.LEVERUPDEELPM) == true)
            {
                model.LEVERUPDEELPM = "NS";// "NORMAL-SINGLE";
            }
            if (model.LEVERUPPSEDO3PH == "" || model.LEVERUPPSEDO3PH == null)
            {
                model.LEVERUPPSEDO3PH = "DIS";
            }
            if (string.IsNullOrEmpty(model.LEVERUP3PHLOCK) == true)
            {
                model.LEVERUP3PHLOCK = "DIS";
            }
            if (string.IsNullOrEmpty(model.LEVERDOWNPM) == true)
            {
                model.LEVERDOWNPM = "FS";
            }
            if (string.IsNullOrEmpty(model.LEVERDOWNDEELPM) == true)
            {
                model.LEVERDOWNDEELPM = "FS";
            }
            if (string.IsNullOrEmpty(model.LEVERDOWNPSEDO3PH) == true)
            {
                model.LEVERDOWNPSEDO3PH = "DIS";
            }
            if (string.IsNullOrEmpty(model.LEVERDOWN3PHLOCK) == true)
            {
                model.LEVERDOWN3PHLOCK = "ENB";// "ENABLED";
            }
            if (string.IsNullOrEmpty(model.LEVERDOWNMI) == true)
            {
                model.LEVERDOWNMI = "MIN";// "MANUAL INHIBIT";
            }
            //oth
            // if (model.OTHLOADPP.ToString() == "" || model.OTHLOADPP.ToString() == null)
            if (string.IsNullOrEmpty(model.OTHLOADPP) == true)
            {
                model.OTHLOADPP = "15";
            }
            // if (model.OTHLINEFREQ.ToString() == "" || model.OTHLINEFREQ.ToString() == null)
            if (string.IsNullOrEmpty(model.OTHLINEFREQ) == true)
            {
                model.OTHLINEFREQ = "60";
            }
            if (string.IsNullOrEmpty(model.OTHENDLIFEPOLICY) == true)
            {
                model.OTHENDLIFEPOLICY = "SO";
            }
            //if (model.OTHTIMEZONEDISPLAY.ToString() == "" || model.OTHTIMEZONEDISPLAY.ToString() == null)
            if (string.IsNullOrEmpty(model.OTHTIMEZONEDISPLAY) == true)
            {
                model.OTHTIMEZONEDISPLAY = "PST";
            }
            // if (model.OTHFAULTPI.ToString() == "" || model.OTHFAULTPI.ToString() == null)
            if (string.IsNullOrEmpty(model.OTHFAULTPI) == true)
            {
                model.OTHFAULTPI = "7";
            }
          
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
            FuseSaverModel model = getDevice(globalID, HtmlExtensions.PageMode.Future);
            setUiSettings(HtmlExtensions.PageMode.Future);
          this.initializeDropDowns(model);
            //String a = Convert.ToDecimal(model.NORMALMINTRIPCM);
          if (model.NORMALMINTRIPCM == null || model.NORMALMINTRIPCM.ToString() == "")
          //  if (string.IsNullOrEmpty(model.NORMALMINTRIPCM) == true)
            {
                model.NORMALMINTRIPCM = "3";
            }
          if (model.NORMALMAXFT == null || model.NORMALMAXFT.ToString() == "")
           // if (string.IsNullOrEmpty(model.NORMALMAXFT.ToString()) == true)
            {
                model.NORMALMAXFT = "3";
            }
          if (model.NORMALINSTTS == null || model.NORMALINSTTS.ToString() == "")
          //  if (string.IsNullOrEmpty(model.NORMALINSTTS) == true)
            {
                model.NORMALINSTTS = "OFF";
            }
          if (model.NORMALMINTT == null || model.NORMALMINTT.ToString() == "")
           // if (string.IsNullOrEmpty(model.NORMALMINTT) == true)
            {
                model.NORMALMINTT = "DIS";
            }
            //Fast
          if (model.FASTMODEMINTCM == null || model.FASTMODEMINTCM.ToString() == "")
           // if (string.IsNullOrEmpty(model.FASTMODEMINTCM) == true)
            {
                model.FASTMODEMINTCM = "2";
            }
          if (model.FASTMODEINSTTS == null || model.FASTMODEINSTTS.ToString() == "")
          //  if (string.IsNullOrEmpty(model.FASTMODEINSTTS) == true)
            {
                model.FASTMODEINSTTS = "3";
            }
          if (model.FASTMODEMAXFT == null || model.FASTMODEMAXFT.ToString() == "")
          //  if (string.IsNullOrEmpty(model.FASTMODEMAXFT) == true)
            {
                model.FASTMODEMAXFT = "3";
            }
          if (model.FASTMODEMINTT == null || model.FASTMODEMINTT.ToString() == "")
            //if (string.IsNullOrEmpty(model.FASTMODEMINTT) == true)
            {
                model.FASTMODEMINTT = "DIS";
            }
                //OPS
          if (model.OSDEADTS == null || model.OSDEADTS.ToString() == "")
              //  if (model.OSDEADTS == null || model.OSDEADTS.ToString() == "")
                {
                    model.OSDEADTS = 10;
                }
          if (model.OSENABLEEXTLEV == null || model.OSENABLEEXTLEV.ToString() == "")
              //  if (string.IsNullOrEmpty(model.OSENABLEEXTLEV) == true)
                {
                    model.OSENABLEEXTLEV = "DIS";
                }
          if (model.OSMANUALGANOP == null || model.OSMANUALGANOP.ToString() == "")
              //  if (string.IsNullOrEmpty(model.OSENABLEEXTLEV) == true)
               // if (string.IsNullOrEmpty(model.OSMANUALGANOP) == true)
                {
                    model.OSMANUALGANOP = "DIS";
                }
          if (model.OSDEENERLT == null || model.OSDEENERLT.ToString() == "")
              //  if (string.IsNullOrEmpty(model.OSENABLEEXTLEV) == true)
                //if (string.IsNullOrEmpty(model.OSDEENERLT) == true)
                {
                    model.OSDEENERLT = "60";
                }
                if (model.OSRECLAIMTIME.ToString() == "" || model.OSRECLAIMTIME.ToString() == null)
                {
                    model.OSRECLAIMTIME = 60;
                }
                if (model.OSINRUSHRM == null || model.OSINRUSHRM.ToString() == "")
               // if (string.IsNullOrEmpty(model.OSINRUSHRM) == true)
                {
                    model.OSINRUSHRM = "2";
                }

                if (string.IsNullOrEmpty(model.OSINRUSHRT.ToString()) == true || model.OSINRUSHRT.ToString()=="0")
                {
                    model.OSINRUSHRT = 500;
                }
                if (string.IsNullOrEmpty(model.OSCAPCHARGE) == true)
                {
                    model.OSCAPCHARGE = "ALWD";// "ALLOWED";
                }
                //Level
                if (string.IsNullOrEmpty(model.LEVERUPPM) == true)
                {
                    model.LEVERUPPM = "NN";
                }
                if (string.IsNullOrEmpty(model.LEVERUPDEELPM) == true)
                {
                    model.LEVERUPDEELPM = "NS";// "NORMAL-SINGLE";
                }
                if (model.LEVERUPPSEDO3PH == "" || model.LEVERUPPSEDO3PH == null)
                {
                    model.LEVERUPPSEDO3PH = "DIS";
                }
                if (string.IsNullOrEmpty(model.LEVERUP3PHLOCK) == true)
                {
                    model.LEVERUP3PHLOCK = "DIS";
                }
                if (string.IsNullOrEmpty(model.LEVERDOWNPM) == true)
                {
                    model.LEVERDOWNPM = "FS";
                }
                if (string.IsNullOrEmpty(model.LEVERDOWNDEELPM) == true)
                {
                    model.LEVERDOWNDEELPM = "FS";
                }
                if (string.IsNullOrEmpty(model.LEVERDOWNPSEDO3PH) == true)
                {
                    model.LEVERDOWNPSEDO3PH = "DIS";
                }
                if (string.IsNullOrEmpty(model.LEVERDOWN3PHLOCK) == true)
                {
                    model.LEVERDOWN3PHLOCK = "ENB";// "ENABLED";
                }
                if (string.IsNullOrEmpty(model.LEVERDOWNMI) == true)
                {
                    model.LEVERDOWNMI = "MIN";// "MANUAL INHIBIT";
                }
                //oth
               // if (model.OTHLOADPP.ToString() == "" || model.OTHLOADPP.ToString() == null)
               if (string.IsNullOrEmpty(model.OTHLOADPP) == true)
                {
                    model.OTHLOADPP = "15";
                }
               // if (model.OTHLINEFREQ.ToString() == "" || model.OTHLINEFREQ.ToString() == null)
                if (string.IsNullOrEmpty(model.OTHLINEFREQ) == true)
                {
                    model.OTHLINEFREQ = "50";
                }
                if (string.IsNullOrEmpty(model.OTHENDLIFEPOLICY) == true)
                {
                    model.OTHENDLIFEPOLICY="SO";
                }
                //if (model.OTHTIMEZONEDISPLAY.ToString() == "" || model.OTHTIMEZONEDISPLAY.ToString() == null)
                if (string.IsNullOrEmpty(model.OTHTIMEZONEDISPLAY) == true)
                {
                    model.OTHTIMEZONEDISPLAY = "PST";
                }
               // if (model.OTHFAULTPI.ToString() == "" || model.OTHFAULTPI.ToString() == null)
               if (string.IsNullOrEmpty(model.OTHFAULTPI) == true)
                {
                    model.OTHFAULTPI = "4";
                }
                //if (model.FUSETYPE.ToString() == "")
                //{
                //    model.FUSETYPE = "ENABLED";
                //}
               // FuseSaverModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
                GetFuseRateList(model.FUSETYPE,model);
            
            return View("Index", model);
        }

        public ActionResult GIS(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            setUiSettings(HtmlExtensions.PageMode.GIS);
            FuseSaverModel model = new FuseSaverModel();

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
                                          join r_h in db.SM_RECLOSER_FS_HIST.Where(s => s.GLOBAL_ID == globalID) on c.HIST_ID equals r_h.ID
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

            FuseSaverModel model = new FuseSaverModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_RECLOSER_FS_HIST e = db.SM_RECLOSER_FS_HIST.FirstOrDefault(s => s.GLOBAL_ID == globalID && s.ID == ID);
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
                SM_RECLOSER_FS e = db.SM_RECLOSER_FS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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
        public ActionResult SaveFuture(FuseSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
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
                            SM_RECLOSER_FS_HIST history = this.saveHistory(db, globalID, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);
                            db.SM_RECLOSER_FS_HIST.AddObject(history);
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
            FuseSaverModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
            setUiSettings(HtmlExtensions.PageMode.Future);
            initializeDropDowns(model);

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

        //[HttpPost]
        //public ActionResult ControlTypeChanged(FuseSaverModel model, string globalID, string layerName, string layerType)
        //{
        //    ViewBag.GlobalID = globalID;
        //    ViewBag.LayerName = layerName;
        //    ViewBag.layerType = layerType;
        //    setUiSettings(HtmlExtensions.PageMode.Future);

        //    initializeDropDowns(model);
        //    return View("Index", model);
        //}

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
                            SM_RECLOSER_FS e = db.SM_RECLOSER_FS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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

        private FuseSaverModel getDevice(string globalID, HtmlExtensions.PageMode pageMode)
        {
            FuseSaverModel model = new FuseSaverModel();

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_RECLOSER_FS e = db.SM_RECLOSER_FS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError(DEVICE_NAME, globalID));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        private void saveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, FuseSaverModel model)
        {

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            SM_RECLOSER_FS entity = db.SM_RECLOSER_FS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
            entity.CURRENT_FUTURE = currentFuture;
            model.PopulateEntityFromModel(entity);
            entity.DATE_MODIFIED = DateTime.Now;
        }

        private SM_RECLOSER_FS_HIST saveHistory(SettingsEntities db, string globalID, FuseSaverModel model)
        {
            SM_RECLOSER_FS_HIST history = new SM_RECLOSER_FS_HIST();
            SM_RECLOSER_FS e = db.SM_RECLOSER_FS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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



       // [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetFuseRateList(string FuseType, FuseSaverModel fsModel)
        {


          

            Dictionary<string, string> result;
        
            using (SettingsEntities db = new SettingsEntities())
            {
                result = SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "FUSE_RATING", FuseType);
                //fsModel.FUSE_RATING_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "FUSE_RATING", FuseType), "Key", "Value");
            

                //var _FRateval = (db.SM_TABLE_LOOKUP.Where(x => x.CONTROL_TYPE == FuseType && x.DEVICE_NAME == "SM_RECLOSER_FS")).ToList(); 
             
                //JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                //result = javaScriptSerializer.Serialize(fsModel.FUSE_RATING_list);
            }
          
            return Json(result, JsonRequestBehavior.AllowGet);
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

        private void initializeDropDowns(FuseSaverModel fsModel)
        {
            string layerType = "layerType=" + @ViewBag.layerType;
            fsModel.PolicyTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "POLICY_TYPE"), "Key", "Value");
            fsModel.ScadaTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "SCADA_TYPE"), "Key", "Value");
            fsModel.ScadaRadioManufacturerList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "RADIO_MANF_CD"), "Key", "Value");
            fsModel.RTUManufactureList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "RTU_MANF_CD"), "Key", "Value");

            fsModel.NORMAL_MIN_TRIP_CM_List = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "NORMAL_MIN_TRIP_CM"), "Key", "Value");
            fsModel.NORMAL_MAX_FT_List = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "NORMAL_MAX_FT"), "Key", "Value");
            fsModel.NORMAL_INST_TS_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "NORMAL_INST_TS"), "Key", "Value");
            fsModel.NORMAL_MIN_TT_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "NORMAL_MIN_TT"), "Key", "Value");

            fsModel.FASTMODE_MIN_TCM_List = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "FASTMODE_MIN_TCM"), "Key", "Value");
            fsModel.FASTMODE_MAX_FT_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "FASTMODE_MAX_FT"), "Key", "Value");
            fsModel.FASTMODE_INST_TS_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "FASTMODE_INST_TS"), "Key", "Value");
            fsModel.FASTMODE_MIN_TT_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "FASTMODE_MIN_TT"), "Key", "Value");

            fsModel.OS_DEAD_TS_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OS_DEAD_TS"), "Key", "Value");
            fsModel.OS_ENABLE_EXT_LEV_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OS_ENABLE_EXT_LEV"), "Key", "Value");
            fsModel.OS_MANUAL_GAN_OP_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OS_MANUAL_GAN_OP"), "Key", "Value");
            fsModel.OS_DE_ENER_LT_list = new SelectList(SiteCache.GetIntegerLookUp(1,300,1), "Key", "Value");
            fsModel.OS_RECLAIM_TIME_list = new SelectList(SiteCache.GetIntegerLookUp(40,100,1), "Key", "Value");
            fsModel.OS_INRUSH_RM_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OS_INRUSH_RM"), "Key", "Value");
            fsModel.OS_INRUSH_RT_list = new SelectList(SiteCache.GetIntegerLookUp(20,1000,20), "Key", "Value");
            fsModel.OS_CAP_CHARGE_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OS_CAP_CHARGE"), "Key", "Value");

            fsModel.LEVER_UP_PM_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LEVER_UP_PM"), "Key", "Value");
            fsModel.LEVER_UP_DE_EL_PM_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LEVER_UP_DE_EL_PM"), "Key", "Value");
            fsModel.LEVER_UP_PSEDO_3PH_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LEVER_UP_PSEDO_3PH"), "Key", "Value");
            fsModel.LEVER_UP_3PH_LOCK_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LEVER_UP_3PH_LOCK"), "Key", "Value");

            fsModel.LEVER_DOWN_PM_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LEVER_DOWN_PM"), "Key", "Value");
            fsModel.LEVER_DOWN_DE_EL_PM_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LEVER_DOWN_DE_EL_PM"), "Key", "Value");
            fsModel.LEVER_DOWN_PSEDO_3PH_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LEVER_DOWN_PSEDO_3PH"), "Key", "Value");
            fsModel.LEVER_DOWN_3PH_LOCK_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LEVER_DOWN_3PH_LOCK"), "Key", "Value");
            fsModel.LEVER_DOWN_MI_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LEVER_DOWN_MI"), "Key", "Value");

            fsModel.OTH_LOAD_PP_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OTH_LOAD_PP"), "Key", "Value");
            fsModel.OTH_ENDLIFE_POLICY_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OTH_ENDLIFE_POLICY"), "Key", "Value");
            fsModel.OTH_LINE_FREQ_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OTH_LINE_FREQ"), "Key", "Value");
            fsModel.OTH_TIMEZONE_DISPLAY_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OTH_TIMEZONE_DISPLAY"), "Key", "Value");
            fsModel.OTH_FAULT_PI_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "OTH_FAULT_PI"), "Key", "Value");

            fsModel.FUSE_TYPE_List = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "FUSE_TYPE"), "Key", "Value");
            fsModel.FUSE_RATING_list = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "FUSE_RATING", fsModel.FUSETYPE), "Key", "Value");

            fsModel.FieldsToDisplay = SiteCache.GetFieldsToDisplayForFuseSaver();
            //tsModel.HighCurCutoffPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/HighCurChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            //tsModel.SectModePostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/SectModeChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";

        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
