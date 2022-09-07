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
    public class TripSaverController : Controller
    {
        //
        // GET: /TripSaver/

        private const string DEVICE_NAME = "TripSaver";
        private const string DEVICE_TABLE_NAME = "SM_RECLOSER_TS";
        private const string DEVICE_HIST_TABLE_NAME = "SM_RECLOSER_TS_HIST";

        #region "Action Methods"

        public ActionResult Index(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            TripSaverModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);

            setUiSettings(HtmlExtensions.PageMode.Current);
            this.initializeDropDowns(model);
            // model.IniEmulatedDevice = "MR";
            // model.IniInverseSegment = "MR";
            return View(model);
        }

        public ActionResult GIS(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            setUiSettings(HtmlExtensions.PageMode.GIS);
            TripSaverModel model = new TripSaverModel();

            //Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            //Dictionary<string, string> attributeValues = new GISService().GetProperties(globalID, layer.Item1, layer.Item2);

            Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            int LayerID;
            int lyID = SiteCache.GetLayerID(layerName).Item1;
            if (layerType == "C")
            {
                LayerID = lyID + 1;
            }
            else
            {
                LayerID = lyID + 2;
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

        public ActionResult Future(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            TripSaverModel model = getDevice(globalID, HtmlExtensions.PageMode.Future);

            setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);
            //model.IniEmulatedDevice = "MR";
            // model.IniInverseSegment = "MR";
            return View("Index", model);
        }

        public ActionResult History(string globalID, string layerName,string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType=layerType;
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

                List<History> ts_hist = (from c in db.SM_COMMENT_HIST
                                         join ts_h in db.SM_RECLOSER_TS_HIST.Where(s => s.GLOBAL_ID == globalID) on c.HIST_ID equals ts_h.ID
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

                ch.CommentHistory.AddRange(ts_hist);
                ch.CommentHistory = ch.CommentHistory.OrderByDescending(c => c.EntryDate).ToList();
            }

            return View("History", ch);
        }

        public ActionResult HistoryDetails(string globalID, string layerName, decimal ID, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            TripSaverModel model = new TripSaverModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_RECLOSER_TS_HIST e = db.SM_RECLOSER_TS_HIST.FirstOrDefault(s => s.GLOBAL_ID == globalID && s.ID == ID);
                model.PopulateModelFromHistoryEntity(e);
            }

            setUiSettings(HtmlExtensions.PageMode.History);
            this.initializeDropDowns(model);

            return View("HistoryData", model);
        }

        public ActionResult DeleteComment(string globalID, string layerName,string layerType, decimal ID)
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

        public ActionResult Files(string globalID, string layerName,string layerType, string settingsError = null, string peerReviewError = null)
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
                SM_RECLOSER_TS e = db.SM_RECLOSER_TS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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
        public ActionResult SaveFuture(TripSaverModel model, string globalID, string layerName, string layerType)
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
                            SM_RECLOSER_TS_HIST history = this.saveHistory(db, globalID, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);
                            db.SM_RECLOSER_TS_HIST.AddObject(history);
                            db.SaveChanges();
                            this.addComment(db, globalID, SystemEnums.WorkType.SETT, "Release to Current",layerType, history.ID);
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
            TripSaverModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
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

        [HttpPost]
        public ActionResult ControlTypeChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            setUiSettings(HtmlExtensions.PageMode.Future);

            if (model.IniEmulatedDevice != null || model.Test2EmulatedDevice != null)
            {
                //ModelState.Remove("FlisrDevice");
                //ModelState.Add("FlisrDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
                //ModelState.SetModelValue("FlisrDevice", new ValueProviderResult("N", "N", null));

            }

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

        [HttpPost]
        public ActionResult AddTest(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            return null;
        }

        [HttpPost]
        public ActionResult RemoveTest(string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            return null;
        }


        //
        [HttpPost]
        public ActionResult CopyInitialToTest1(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            setUiSettings(HtmlExtensions.PageMode.Future);

            ModelState.Remove("Test1EmulatedDevice");
            model.Test1EmulatedDevice = model.IniEmulatedDevice;
            ModelState.Remove("Test1InverseSegment");
            model.Test1InverseSegment = model.IniInverseSegment;
            ModelState.Remove("Test1Speed");
            model.Test1Speed = model.IniSpeed;
            ModelState.Remove("Test1AmpereRating");
            model.Test1AmpereRating = model.IniAmpereRating;
            ModelState.Remove("Test1CoilRating");
            model.Test1CoilRating = model.IniCoilRating;
            ModelState.Remove("Test1ResetType");
            model.Test1ResetType = model.IniResetType;
            ModelState.Remove("Test1MinTripA");
            model.Test1MinTripA = model.IniMinTripA;
            ModelState.Remove("Test1TimeMultiplier");
            model.Test1TimeMultiplier = model.IniTimeMultiplier;
            ModelState.Remove("Test1CurrA");
            model.Test1CurrA = model.IniCurrA;
            ModelState.Remove("Test1Time");
            model.Test1Time = model.IniTime;
            ModelState.Remove("Test1ResetTime");
            model.Test1ResetTime = model.IniResetTime;
            ModelState.Remove("Test1Lowcutoff");
            model.Test1Lowcutoff = model.IniLowcutoff;
            ModelState.Remove("Test1LowCutOffChk");
            if (model.Test1Lowcutoff == "Y")
                model.Test1LowCutOffChk = true;
            else
                model.Test1LowCutOffChk = false;
            ModelState.Remove("Test1LowcutoffCurrA");
            model.Test1LowcutoffCurrA = model.IniLowcutoffCurrA;

            ModelState.Remove("Test1DefiniteTime1");
            model.Test1DefiniteTime1 = model.IniDefiniteTime1;
            ModelState.Remove("Test1DefiniteTime1Chk");
            if (model.Test1DefiniteTime1 == "Y")
                model.Test1DefiniteTime1Chk = true;
            else
                model.Test1DefiniteTime1Chk = false;
            ModelState.Remove("Test1DefiniteTime1CurrA");
            model.Test1DefiniteTime1CurrA = model.IniDefiniteTime1CurrA;
            ModelState.Remove("Test1DefiniteTime1Time");
            model.Test1DefiniteTime1Time = model.IniDefiniteTime1Time;

            ModelState.Remove("Test1DefiniteTime2");
            model.Test1DefiniteTime2 = model.IniDefiniteTime2;
            ModelState.Remove("Test1DefiniteTime2Chk");
            if (model.Test1DefiniteTime2 == "Y")
                model.Test1DefiniteTime2Chk = true;

            else
                model.Test1DefiniteTime2Chk = false;
            ModelState.Remove("Test1DefiniteTime2CurrA");
            model.Test1DefiniteTime2CurrA = model.IniDefiniteTime2CurrA;
            ModelState.Remove("Test1DefiniteTime2Time");
            model.Test1DefiniteTime2Time = model.IniDefiniteTime2Time;

            ModelState.Remove("Test1DefiniteTime3");
            model.Test1DefiniteTime3 = model.IniDefiniteTime3;
            ModelState.Remove("Test1DefiniteTime3Chk");
            if (model.Test1DefiniteTime3 == "Y")
                model.Test1DefiniteTime3Chk = true;
            else
                model.Test1DefiniteTime3Chk = false;
            ModelState.Remove("Test1DefiniteTime3CurrA");
            model.Test1DefiniteTime3CurrA = model.IniDefiniteTime3CurrA;
            ModelState.Remove("Test1DefiniteTime3Time");
            model.Test1DefiniteTime3Time = model.IniDefiniteTime3Time;

            initializeDropDowns(model);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult CopyTest1ToTest2(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            setUiSettings(HtmlExtensions.PageMode.Future);


            ModelState.Remove("Test2EmulatedDevice");
            model.Test2EmulatedDevice = model.Test1EmulatedDevice;
            ModelState.Remove("Test2InverseSegment");
            model.Test2InverseSegment = model.Test1InverseSegment;
            ModelState.Remove("Test2Speed");
            model.Test2Speed = model.Test1Speed;
            ModelState.Remove("Test2AmpereRating");
            model.Test2AmpereRating = model.Test1AmpereRating;
            ModelState.Remove("Test2CoilRating");
            model.Test2CoilRating = model.Test1CoilRating;
            ModelState.Remove("Test2ResetType");
            model.Test2ResetType = model.Test1ResetType;
            ModelState.Remove("Test2MinTripA");
            model.Test2MinTripA = model.Test1MinTripA;
            ModelState.Remove("Test2TimeMultiplier");
            model.Test2TimeMultiplier = model.Test1TimeMultiplier;
            ModelState.Remove("Test2CurrA");
            model.Test2CurrA = model.Test1CurrA;
            ModelState.Remove("Test2Time");
            model.Test2Time = model.Test1Time;
            ModelState.Remove("Test2ResetTime");
            model.Test2ResetTime = model.Test1ResetTime;
            ModelState.Remove("Test2Lowcutoff");
            model.Test2Lowcutoff = model.Test1Lowcutoff;
            ModelState.Remove("Test2LowCutOffChk");
            if (model.Test2Lowcutoff == "Y")
                model.Test2LowCutOffChk = true;
            else
                model.Test2LowCutOffChk = false;
            ModelState.Remove("Test2LowcutoffCurrA");
            model.Test2LowcutoffCurrA = model.Test1LowcutoffCurrA;

            ModelState.Remove("Test2DefiniteTime1");
            model.Test2DefiniteTime1 = model.Test1DefiniteTime1;
            ModelState.Remove("Test2DefiniteTime1Chk");
            if (model.Test2DefiniteTime1 == "Y")
                model.Test2DefiniteTime1Chk = true;
            else
                model.Test2DefiniteTime1Chk = false;
            ModelState.Remove("Test2DefiniteTime1CurrA");
            model.Test2DefiniteTime1CurrA = model.Test1DefiniteTime1CurrA;
            ModelState.Remove("Test2DefiniteTime1Time");
            model.Test2DefiniteTime1Time = model.Test1DefiniteTime1Time;

            ModelState.Remove("Test2DefiniteTime2");
            model.Test2DefiniteTime2 = model.Test1DefiniteTime2;
            ModelState.Remove("Test2DefiniteTime2Chk");
            if (model.Test2DefiniteTime2 == "Y")
                model.Test2DefiniteTime2Chk = true;

            else
                model.Test2DefiniteTime2Chk = false;
            ModelState.Remove("Test2DefiniteTime2CurrA");
            model.Test2DefiniteTime2CurrA = model.Test1DefiniteTime2CurrA;
            ModelState.Remove("Test2DefiniteTime2Time");
            model.Test2DefiniteTime2Time = model.Test1DefiniteTime2Time;

            ModelState.Remove("Test2DefiniteTime3");
            model.Test2DefiniteTime3 = model.Test1DefiniteTime3;
            ModelState.Remove("Test2DefiniteTime3Chk");
            if (model.Test2DefiniteTime3 == "Y")
                model.Test2DefiniteTime3Chk = true;
            else
                model.Test2DefiniteTime3Chk = false;
            ModelState.Remove("Test2DefiniteTime3CurrA");
            model.Test2DefiniteTime3CurrA = model.Test1DefiniteTime3CurrA;
            ModelState.Remove("Test2DefiniteTime3Time");
            model.Test2DefiniteTime3Time = model.Test1DefiniteTime3Time;

            initializeDropDowns(model);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult CopyTest2ToTest3(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.layerType = layerType;
            setUiSettings(HtmlExtensions.PageMode.Future);

            ModelState.Remove("Test3EmulatedDevice");
            model.Test3EmulatedDevice = model.Test2EmulatedDevice;
            ModelState.Remove("Test3InverseSegment");
            model.Test3InverseSegment = model.Test2InverseSegment;
            ModelState.Remove("Test3Speed");
            model.Test3Speed = model.Test2Speed;
            ModelState.Remove("Test3AmpereRating");
            model.Test3AmpereRating = model.Test2AmpereRating;
            ModelState.Remove("Test3CoilRating");
            model.Test3CoilRating = model.Test2CoilRating;
            ModelState.Remove("Test3ResetType");
            model.Test3ResetType = model.Test2ResetType;
            ModelState.Remove("Test3MinTripA");
            model.Test3MinTripA = model.Test2MinTripA;
            ModelState.Remove("Test3TimeMultiplier");
            model.Test3TimeMultiplier = model.Test2TimeMultiplier;
            ModelState.Remove("Test3CurrA");
            model.Test3CurrA = model.Test2CurrA;
            ModelState.Remove("Test3Time");
            model.Test3Time = model.Test2Time;
            ModelState.Remove("Test3ResetTime");
            model.Test3ResetTime = model.Test2ResetTime;
            ModelState.Remove("Test3Lowcutoff");
            model.Test3Lowcutoff = model.Test2Lowcutoff;
            ModelState.Remove("Test3LowCutOffChk");
            if (model.Test3Lowcutoff == "Y")
                model.Test3LowCutOffChk = true;
            else
                model.Test3LowCutOffChk = false;
            ModelState.Remove("Test3LowcutoffCurrA");
            model.Test3LowcutoffCurrA = model.Test2LowcutoffCurrA;

            ModelState.Remove("Test3DefiniteTime1");
            model.Test3DefiniteTime1 = model.Test2DefiniteTime1;
            ModelState.Remove("Test3DefiniteTime1Chk");
            if (model.Test3DefiniteTime1 == "Y")
                model.Test3DefiniteTime1Chk = true;
            else
                model.Test3DefiniteTime1Chk = false;
            ModelState.Remove("Test3DefiniteTime1CurrA");
            model.Test3DefiniteTime1CurrA = model.Test2DefiniteTime1CurrA;
            ModelState.Remove("Test3DefiniteTime1Time");
            model.Test3DefiniteTime1Time = model.Test2DefiniteTime1Time;

            ModelState.Remove("Test3DefiniteTime2");
            model.Test3DefiniteTime2 = model.Test2DefiniteTime2;
            ModelState.Remove("Test3DefiniteTime2Chk");
            if (model.Test3DefiniteTime2 == "Y")
                model.Test3DefiniteTime2Chk = true;

            else
                model.Test3DefiniteTime2Chk = false;
            ModelState.Remove("Test3DefiniteTime2CurrA");
            model.Test3DefiniteTime2CurrA = model.Test2DefiniteTime2CurrA;
            ModelState.Remove("Test3DefiniteTime2Time");
            model.Test3DefiniteTime2Time = model.Test2DefiniteTime2Time;

            ModelState.Remove("Test3DefiniteTime3");
            model.Test3DefiniteTime3 = model.Test2DefiniteTime3;
            ModelState.Remove("Test3DefiniteTime3Chk");
            if (model.Test3DefiniteTime3 == "Y")
                model.Test3DefiniteTime3Chk = true;
            else
                model.Test3DefiniteTime3Chk = false;
            ModelState.Remove("Test3DefiniteTime3CurrA");
            model.Test3DefiniteTime3CurrA = model.Test2DefiniteTime3CurrA;
            ModelState.Remove("Test3DefiniteTime3Time");
            model.Test3DefiniteTime3Time = model.Test2DefiniteTime3Time;

            initializeDropDowns(model);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult IniLowCutOffChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("IniLowcutoff");
            if (model.IniLowCutOffChk)
                model.IniLowcutoff = "Y";
            else
            {
                model.IniLowcutoff = "N";
                ModelState.Remove("IniLowcutoffCurrA");
                model.IniLowcutoffCurrA = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult IniDef1ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("IniDefiniteTime1");

            if (model.IniDefiniteTime1Chk)
                model.IniDefiniteTime1 = "Y";
            else
            {
                model.IniDefiniteTime1 = "N";
                ModelState.Remove("IniDefiniteTime2");
                model.IniDefiniteTime2 = "N";
                ModelState.Remove("IniDefiniteTime2Chk");
                model.IniDefiniteTime2Chk = false;
                ModelState.Remove("IniDefiniteTime1CurrA");
                model.IniDefiniteTime1CurrA = null;
                ModelState.Remove("IniDefiniteTime1Time");
                model.IniDefiniteTime1Time = null;
                ModelState.Remove("IniDefiniteTime2CurrA");
                model.IniDefiniteTime2CurrA = null;
                ModelState.Remove("IniDefiniteTime2Time");
                model.IniDefiniteTime2Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult IniDef2ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("IniDefiniteTime2");
            if (model.IniDefiniteTime2Chk)
                model.IniDefiniteTime2 = "Y";
            else
            {
                model.IniDefiniteTime2 = "N";
                ModelState.Remove("IniDefiniteTime3");
                model.IniDefiniteTime3 = "N";
                ModelState.Remove("IniDefiniteTime3Chk");
                model.IniDefiniteTime3Chk = false;
                ModelState.Remove("IniDefiniteTime2CurrA");
                model.IniDefiniteTime2CurrA = null;
                ModelState.Remove("IniDefiniteTime2Time");
                model.IniDefiniteTime2Time = null;
                ModelState.Remove("IniDefiniteTime3CurrA");
                model.IniDefiniteTime3CurrA = null;
                ModelState.Remove("IniDefiniteTime3Time");
                model.IniDefiniteTime3Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName, layerType);
        }

        [HttpPost]
        public ActionResult IniDef3ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("IniDefiniteTime3");
            if (model.IniDefiniteTime3Chk)
                model.IniDefiniteTime3 = "Y";
            else
            {
                model.IniDefiniteTime3 = "N";
                ModelState.Remove("IniDefiniteTime3CurrA");
                model.IniDefiniteTime3CurrA = null;
                ModelState.Remove("IniDefiniteTime3Time");
                model.IniDefiniteTime3Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test1LowCutOffChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test1Lowcutoff");
            if (model.Test1LowCutOffChk)
                model.Test1Lowcutoff = "Y";
            else
            {
                model.Test1Lowcutoff = "N";
                ModelState.Remove("Test1LowcutoffCurrA");
                model.Test1LowcutoffCurrA = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test1Def1ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test1DefiniteTime1");
            if (model.Test1DefiniteTime1Chk)
                model.Test1DefiniteTime1 = "Y";
            else
            {
                model.Test1DefiniteTime1 = "N";
                ModelState.Remove("Test1DefiniteTime2");
                model.Test1DefiniteTime2 = "N";
                ModelState.Remove("Test1DefiniteTime2Chk");
                model.Test1DefiniteTime2Chk = false;
                ModelState.Remove("Test1DefiniteTime1CurrA");
                model.Test1DefiniteTime1CurrA = null;
                ModelState.Remove("Test1DefiniteTime1Time");
                model.Test1DefiniteTime1Time = null;
                ModelState.Remove("Test1DefiniteTime2CurrA");
                model.Test1DefiniteTime2CurrA = null;
                ModelState.Remove("Test1DefiniteTime2Time");
                model.Test1DefiniteTime2Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test1Def2ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test1DefiniteTime2");
            if (model.Test1DefiniteTime2Chk)
                model.Test1DefiniteTime2 = "Y";
            else
            {
                model.Test1DefiniteTime2 = "N";
                ModelState.Remove("Test1DefiniteTime3");
                model.Test1DefiniteTime3 = "N";
                ModelState.Remove("Test1DefiniteTime3Chk");
                model.Test1DefiniteTime3Chk = false;
                ModelState.Remove("Test1DefiniteTime2CurrA");
                model.Test1DefiniteTime2CurrA = null;
                ModelState.Remove("Test1DefiniteTime2Time");
                model.Test1DefiniteTime2Time = null;
                ModelState.Remove("Test1DefiniteTime3CurrA");
                model.Test1DefiniteTime3CurrA = null;
                ModelState.Remove("Test1DefiniteTime3Time");
                model.Test1DefiniteTime3Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test1Def3ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test1DefiniteTime3");
            if (model.Test1DefiniteTime3Chk)
                model.Test1DefiniteTime3 = "Y";
            else
            {
                model.Test1DefiniteTime3 = "N";
                ModelState.Remove("Test1DefiniteTime3CurrA");
                model.Test1DefiniteTime3CurrA = null;
                ModelState.Remove("Test1DefiniteTime3Time");
                model.Test1DefiniteTime3Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test2LowCutOffChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test2Lowcutoff");
            if (model.Test2LowCutOffChk)
                model.Test2Lowcutoff = "Y";
            else
            {
                model.Test2Lowcutoff = "N";
                ModelState.Remove("Test2LowcutoffCurrA");
                model.Test2LowcutoffCurrA = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test2Def1ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test2DefiniteTime1");
            if (model.Test2DefiniteTime1Chk)
                model.Test2DefiniteTime1 = "Y";
            else
            {
                model.Test2DefiniteTime1 = "N";
                ModelState.Remove("Test2DefiniteTime2");
                model.Test2DefiniteTime2 = "N";
                ModelState.Remove("Test2DefiniteTime2Chk");
                model.Test2DefiniteTime2Chk = false;
                ModelState.Remove("Test2DefiniteTime1CurrA");
                model.Test2DefiniteTime1CurrA = null;
                ModelState.Remove("Test2DefiniteTime1Time");
                model.Test2DefiniteTime1Time = null;
                ModelState.Remove("Test2DefiniteTime2CurrA");
                model.Test2DefiniteTime2CurrA = null;
                ModelState.Remove("Test2DefiniteTime2Time");
                model.Test2DefiniteTime2Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test2Def2ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test2DefiniteTime2");
            if (model.Test2DefiniteTime2Chk)
                model.Test2DefiniteTime2 = "Y";
            else
            {
                model.Test2DefiniteTime2 = "N";
                ModelState.Remove("Test2DefiniteTime3");
                model.Test2DefiniteTime3 = "N";
                ModelState.Remove("Test2DefiniteTime3Chk");
                model.Test2DefiniteTime3Chk = false;
                ModelState.Remove("Test2DefiniteTime2CurrA");
                model.Test2DefiniteTime2CurrA = null;
                ModelState.Remove("Test2DefiniteTime2Time");
                model.Test2DefiniteTime2Time = null;
                ModelState.Remove("Test2DefiniteTime3CurrA");
                model.Test2DefiniteTime3CurrA = null;
                ModelState.Remove("Test2DefiniteTime3Time");
                model.Test2DefiniteTime3Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test2Def3ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test2DefiniteTime3");
            if (model.Test2DefiniteTime3Chk)
                model.Test2DefiniteTime3 = "Y";
            else
            {
                model.Test2DefiniteTime3 = "N";
                ModelState.Remove("Test2DefiniteTime3CurrA");
                model.Test2DefiniteTime3CurrA = null;
                ModelState.Remove("Test2DefiniteTime3Time");
                model.Test2DefiniteTime3Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test3LowCutOffChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test3Lowcutoff");
            if (model.Test3LowCutOffChk)
                model.Test3Lowcutoff = "Y";
            else
            {
                model.Test3Lowcutoff = "N";
                ModelState.Remove("Test3Lowcutoff");
                model.Test3Lowcutoff = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test3Def1ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test3DefiniteTime1");
            if (model.Test3DefiniteTime1Chk)
                model.Test3DefiniteTime1 = "Y";
            else
            {
                model.Test3DefiniteTime1 = "N";
                ModelState.Remove("Test3DefiniteTime2");
                model.Test3DefiniteTime2 = "N";
                ModelState.Remove("Test3DefiniteTime2Chk");
                model.Test3DefiniteTime2Chk = false;
                ModelState.Remove("Test3DefiniteTime1CurrA");
                model.Test3DefiniteTime1CurrA = null;
                ModelState.Remove("Test3DefiniteTime1Time");
                model.Test3DefiniteTime1Time = null;
                ModelState.Remove("Test3DefiniteTime2CurrA");
                model.Test3DefiniteTime2CurrA = null;
                ModelState.Remove("Test3DefiniteTime2Time");
                model.Test3DefiniteTime2Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName, layerType);
        }

        [HttpPost]
        public ActionResult Test3Def2ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test3DefiniteTime2");
            if (model.Test3DefiniteTime2Chk)
                model.Test3DefiniteTime2 = "Y";
            else
            {

                model.Test3DefiniteTime2 = "N";
                ModelState.Remove("Test3DefiniteTime3");
                model.Test3DefiniteTime3 = "N";
                ModelState.Remove("Test3DefiniteTime3Chk");
                model.Test3DefiniteTime3Chk = false;
                ModelState.Remove("Test3DefiniteTime2CurrA");
                model.Test3DefiniteTime2CurrA = null;
                ModelState.Remove("Test3DefiniteTime2Time");
                model.Test3DefiniteTime2Time = null;
                ModelState.Remove("Test3DefiniteTime3CurrA");
                model.Test3DefiniteTime3CurrA = null;
                ModelState.Remove("Test3DefiniteTime3Time");
                model.Test3DefiniteTime3Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult Test3Def3ChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("Test3DefiniteTime3");
            if (model.Test3DefiniteTime3Chk)
                model.Test3DefiniteTime3 = "Y";
            else
            {
                model.Test3DefiniteTime3 = "N";
                ModelState.Remove("Tes3DefiniteTime3CurrA");
                model.Test3DefiniteTime3CurrA = null;
                ModelState.Remove("Test3DefiniteTime3Time");
                model.Test3DefiniteTime3Time = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult HighCurChkChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("HighCurCutOff");
            if (model.HighCurCutoffChk)
                model.HighCurCutOff = "Y";
            else
            {
                model.HighCurCutOff = "N";
                ModelState.Remove("HighCurCutOffA");
                model.HighCurCutOffA = null;
            }
            return ControlTypeChanged(model, globalID, layerName,layerType);
        }

        [HttpPost]
        public ActionResult SectModeChanged(TripSaverModel model, string globalID, string layerName, string layerType)
        {
            ViewBag.layerType = layerType;
            ModelState.Remove("SectModeCounts");
            ModelState.Remove("SectModeResetTime");
            ModelState.Remove("SectModeStartingCur");
            model.SectModeCounts = null;
            model.SectModeResetTime = null;
            model.SectModeStartingCur = null;

            return ControlTypeChanged(model, globalID, layerName,layerType);
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
                            SM_RECLOSER_TS e = db.SM_RECLOSER_TS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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

        private TripSaverModel getDevice(string globalID, HtmlExtensions.PageMode pageMode)
        {
            TripSaverModel model = new TripSaverModel();

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_RECLOSER_TS e = db.SM_RECLOSER_TS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError(DEVICE_NAME, globalID));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }



        private void saveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, TripSaverModel model)
        {
            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            SM_RECLOSER_TS entity = db.SM_RECLOSER_TS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
            entity.CURRENT_FUTURE = currentFuture;
            model.PopulateEntityFromModel(entity);
            entity.DATE_MODIFIED = DateTime.Now;
        }

        private SM_RECLOSER_TS_HIST saveHistory(SettingsEntities db, string globalID, TripSaverModel model)
        {
           
            SM_RECLOSER_TS_HIST history = new SM_RECLOSER_TS_HIST();
            SM_RECLOSER_TS e = db.SM_RECLOSER_TS.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
            model.PopulateHistoryFromEntity(history, e);
            history.GLOBAL_ID = globalID;
            return history;
        }
        private void addComment(SettingsEntities db, string globalID, SystemEnums.WorkType commentType, string comments,string layerType, decimal historyID = 0)
        {
            ViewBag.layerType = layerType;
            SM_COMMENT_HIST comment = new SM_COMMENT_HIST();
            comment.PERFORMED_BY =  Security.CurrentUser;
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
            };

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
                default:
                    break;
            }

            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsDisabled = true;
        }

        private void initializeDropDowns(TripSaverModel tsModel)
        {
            string layerType = "layerType=" + @ViewBag.layerType;
            tsModel.IniEmulatedDeviceList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "INI_EMULATED_DEVICE"), "Key", "Value");
            tsModel.IniSpeedList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "INI_SPEED", tsModel.IniEmulatedDevice), "Key", "Value");
            tsModel.IniAmpereRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "INI_AMPERE_RATING", tsModel.IniSpeed), "Key", "value");
            tsModel.IniInverseSegmentList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "INI_INVERSE_SEGMENT", tsModel.IniEmulatedDevice), "Key", "value");
            tsModel.IniCoilRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "INI_COIL_RATING", tsModel.IniInverseSegment), "Key", "value");

            tsModel.DropDownPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ControlTypeChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";

            tsModel.IniResetTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "INI_RESET_TYPE"), "Key", "Value");

            tsModel.Test1EmulatedDeviceList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST1_EMULATED_DEVICE"), "Key", "Value");
            tsModel.Test1InverseSegmentList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST1_INVERSE_SEGMENT", tsModel.Test1EmulatedDevice), "Key", "Value");
            tsModel.Test1SpeedList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST1_SPEED", tsModel.Test1EmulatedDevice), "Key", "Value");
            tsModel.Test1AmpereRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST1_AMPERE_RATING", tsModel.Test1Speed), "Key", "Value");
            tsModel.Test1CoilRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST1_COIL_RATING", tsModel.Test1InverseSegment), "Key", "Value");
            tsModel.Test1ResetTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST1_RESET_TYPE"), "Key", "Value");

            tsModel.Test2EmulatedDeviceList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST2_EMULATED_DEVICE"), "Key", "Value");
            tsModel.Test2InverseSegmentList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST2_INVERSE_SEGMENT", tsModel.Test2EmulatedDevice), "Key", "Value");
            tsModel.Test2SpeedList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST2_SPEED", tsModel.Test2EmulatedDevice), "Key", "Value");
            tsModel.Test2AmpereRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST2_AMPERE_RATING", tsModel.Test2Speed), "Key", "Value");
            tsModel.Test2CoilRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST2_COIL_RATING", tsModel.Test2InverseSegment), "Key", "Value");
            tsModel.Test2ResetTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST2_RESET_TYPE"), "Key", "Value");

            tsModel.Test3EmulatedDeviceList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST3_EMULATED_DEVICE"), "Key", "Value");
            tsModel.Test3InverseSegmentList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST3_INVERSE_SEGMENT", tsModel.Test3EmulatedDevice), "Key", "Value");
            tsModel.Test3SpeedList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST3_SPEED", tsModel.Test3EmulatedDevice), "Key", "Value");
            tsModel.Test3AmpereRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST3_AMPERE_RATING", tsModel.Test3Speed), "Key", "Value");
            tsModel.Test3CoilRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST3_COIL_RATING", tsModel.Test3InverseSegment), "Key", "Value");
            tsModel.Test3ResetTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "TEST3_RESET_TYPE"), "Key", "Value");

            tsModel.SectModeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "SEC_MODE"), "Key", "Value");
           

           

            tsModel.IniLowChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/IniLowCutOffChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test1LowChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test1LowCutOffChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test2LowChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test2LowCutOffChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test3LowChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test3LowCutOffChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";

            tsModel.IniDef1ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/IniDef1ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test1Def1ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test1Def1ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test2Def1ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test2Def1ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test3Def1ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test3Def1ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";

            tsModel.IniDef2ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/IniDef2ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test1Def2ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test1Def2ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test2Def2ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test2Def2ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test3Def2ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test3Def2ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";

            tsModel.IniDef3ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/IniDef3ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test1Def3ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test1Def3ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test2Def3ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test2Def3ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.Test3Def3ChkPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/Test3Def3ChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";

            tsModel.HighCurCutoffPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/HighCurChkChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";
            tsModel.SectModePostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/SectModeChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"?" + layerType + @"';form.submit();";

        }

        #endregion

        public JsonResult InEMULATED_DEVICE(string DeviceType)
        {
            Dictionary<string, SelectList> response = new Dictionary<string, SelectList>();
            if (DeviceType == "FL")
            {
                SelectList IniSpeedList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "INI_SPEED", DeviceType), "Key", "Value");
                response.Add("SpeedList", IniSpeedList);
            }
          
            else
            {
                SelectList IniInverseSegmentList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "INI_INVERSE_SEGMENT", DeviceType), "Key", "Value");


                response.Add("InverseSeg", IniInverseSegmentList);
                //if (DeviceType == "HR")
                //{
                //    SelectList IniCoilRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "INI_COIL_RATING", DeviceType), "Key", "value");
                //    response.Add("CoilRate", IniCoilRatingList);
                //}
                


            }
            return Json(response);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

