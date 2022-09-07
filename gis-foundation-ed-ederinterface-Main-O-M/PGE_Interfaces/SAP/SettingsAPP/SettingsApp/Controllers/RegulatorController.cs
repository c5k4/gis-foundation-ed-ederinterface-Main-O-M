using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;

using SettingsApp.Models;
using SettingsApp.Common;
using SettingsApp;

namespace SettingsApp.Controllers
{
    [SetLoadTabVisibility()]
    public class RegulatorController : Controller
    {
        private const string DEVICE_NAME = "Regulator";
        private const string DEVICE_TABLE_NAME = "SM_REGULATOR";
        private const string DEVICE_HIST_TABLE_NAME = "SM_REGULATOR_HIST";

        #region "Action Methods"

        public ActionResult Index(string globalID, string layerName, string units, string layerType )
        {
            if (layerType =="P")
            {
                layerName = "Proposed Voltage Regulator";
            }
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;
            ViewBag.layerType = layerType;

            RegulatorModel model = getDevice(units, HtmlExtensions.PageMode.Current);
            setUiSettings(HtmlExtensions.PageMode.Current);
            this.initializeDropDowns(model);
            //model.ControlType = "";
            return View(model);
        }

        public ActionResult Future(string globalID, string layerName, string units, string layerType)
        {
            if (layerType == "P")
            {
                layerName = "Proposed Voltage Regulator";
            }
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;
            var existingModel = TempData["RegModel"];
            RegulatorModel model = null;
            if (existingModel == null)
            {
                model = getDevice(units, HtmlExtensions.PageMode.Future);
            }
            else
            {
                model = (RegulatorModel)existingModel;
            }

            setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

            return View("Index", model);
        }

        public ActionResult GIS(string globalID, string layerName, string units, string layerType)
        {

            if (layerType == "P")
            {
                layerName = "Proposed Voltage Regulator";
            }
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;

            setUiSettings(HtmlExtensions.PageMode.GIS);
            RegulatorModel model = new RegulatorModel();

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

        
        public ActionResult History(string globalID, string layerName, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;

            string[] regulatorUnits = this.getUnitIDs(units);

            setUiSettings(HtmlExtensions.PageMode.History);

            CommentHistoryModel ch_all = new CommentHistoryModel();
            List<History> historyList = new List<History>();
            ch_all.CommentHistory = historyList;

            using (SettingsEntities db = new SettingsEntities())
            {
                if (regulatorUnits == null) return View("History", ch_all);
                foreach (string unitID in regulatorUnits)
                {
                    CommentHistoryModel ch = new CommentHistoryModel();
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
                                         }).Where(c => c.GlobalID == unitID).ToList();

                    List<History> reg_hist = (from c in db.SM_COMMENT_HIST
                                              join r_h in db.SM_REGULATOR_HIST.Where(s => s.GLOBAL_ID == unitID) on c.HIST_ID equals r_h.ID
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

                    ch.CommentHistory.AddRange(reg_hist);
                    ch_all.CommentHistory.AddRange(ch.CommentHistory);

                }
                ch_all.CommentHistory = ch_all.CommentHistory.OrderByDescending(c => c.EntryDate).ToList();
            }

            return View("History", ch_all);
        }

        public ActionResult HistoryDetails(string globalID, string layerName, string units, decimal ID)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;

            string[] regulatorUnitIDs = this.getUnitIDs(units);

            RegulatorModel model = new RegulatorModel();
            if (regulatorUnitIDs == null) return View("HistoryData", model);
            using (SettingsEntities db = new SettingsEntities())
            {
                
                List<RegulatorUnit> regulatorUnits = new List<RegulatorUnit>();
                SM_REGULATOR_HIST e = new SM_REGULATOR_HIST();
                
                foreach (string unitID in regulatorUnitIDs)
                {
                    e = db.SM_REGULATOR_HIST.FirstOrDefault(s => s.GLOBAL_ID == unitID && s.ID == ID);
                    if (e != null)
                        break;
                }

                model = new RegulatorModel();
                //SM_REGULATOR_HIST e = db.SM_REGULATOR_HIST.FirstOrDefault(s => s.GLOBAL_ID == unitID && s.ID == ID);
                model.PopulateModelFromHistoryEntity(e);


                RegulatorUnit unit = new RegulatorUnit();
                unit.GlobalID = e.GLOBAL_ID;
                unit.ControlType = e.CONTROL_TYPE;
                unit.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
                unit.SoftwareVersion = e.SOFTWARE_VERSION;
                unit.FirmwareVersion = e.FIRMWARE_VERSION;
                unit.BankCode = e.BANK_CD;
                regulatorUnits.Add(unit);

                model.RegulatorUnits = regulatorUnits.OrderBy(r => r.BankCode).ToList();
            }

            setUiSettings(HtmlExtensions.PageMode.History);
            this.initializeDropDowns(model);

            return View("HistoryData", model);
        }

        public ActionResult DeleteComment(string globalID, string layerName, string units, decimal ID)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;

            string[] regulatorUnits = this.getUnitIDs(units);

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_COMMENT_HIST history = db.SM_COMMENT_HIST.First(c => c.ID == ID);
                db.SM_COMMENT_HIST.DeleteObject(history);
                db.SaveChanges();
            }

            return RedirectToAction("History", new { globalID = globalID, layerName = layerName, units = units });
        }

        public ActionResult Files(string globalID, string layerName, string units, string settingsError = null, string peerReviewError = null)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;

            ViewBag.SettingsFileError = (settingsError == null ? "" : settingsError);
            ViewBag.PeerReviewError = (peerReviewError == null ? "" : peerReviewError);
            ViewBag.SettingsFileUploaded = (settingsError != null && settingsError == "SUCESS" ? true : false);
            ViewBag.PeerReviewFileUploaded = (peerReviewError != null && peerReviewError == "SUCESS" ? true : false); ;

            using (SettingsEntities db = new SettingsEntities())
            {
                string[] regulatorUnits = getUnitIDs(units);
                string unitID = regulatorUnits[0];

                SM_REGULATOR e = db.SM_REGULATOR.SingleOrDefault(s => s.GLOBAL_ID == unitID && s.CURRENT_FUTURE == "C");
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


        public ActionResult EngineeringInfo(string globalID, string layerName, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;
            ViewBag.ControllerName = "Regulator";
            ViewBag.AddNewItemErrorMessage = string.Empty;
            ViewBag.SaveErrorMessage = string.Empty;

            var model = new TrfBankLoadModel();
            decimal RegulatorId=0;
            using (SettingsEntities db = new SettingsEntities())
            {
                string[] regulatorUnitIDs = this.getUnitIDs(units);
                
                RegulatorId = GetRegIdFromUnitsForLoadInfo(regulatorUnitIDs);
                if (RegulatorId == 0)
                {
                    ViewBag.NoDataFoundMsg = "No data found in the system. Please contact an administrator.";
                    return View("EngineeringInfo", model);
                }
                
                //var TrfBankLoadData = db.SM_SUBTRF_BANK_LOAD.Where(t => t.REF_DEVICE_ID == RegulatorId).FirstOrDefault();
                //if (TrfBankLoadData == null)
                //    return View("EngineeringInfo", model);

                //var TrfBankLoadHistData = db.SM_SUBTRF_BANK_LOAD_HIST.Where(t => t.SM_SUBTRF_BANK_LOAD_ID == TrfBankLoadData.ID).OrderByDescending(t => t.PEAK_DTM);
                
                
            }
            model = GetLoadInfo(RegulatorId,layerName,globalID);
            model.BankNo = GetBankCode(layerName, globalID);
            setUiSettings(HtmlExtensions.PageMode.EngineeringInfo);
            return View("EngineeringInfo", model);
        }


        private string GetKeyValueFromGISTab(string layerName, string globalID,string KEY)
        {
            var KeyValue = string.Empty;
            Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            Dictionary<string, string> attributeValues = new GISService().GetProperties(globalID, layer.Item1, layer.Item2);

            if (attributeValues.ContainsKey(KEY))
            {
                KeyValue = attributeValues[KEY];
            }
            return KeyValue;
        }

        private bool GetHighSideVoltage(string layerName, string globalID)
        {
            var KEY = "HIGHSIDEVOLTAGE";
            var IsFedStationChecked = false;
            double HighSideVoltage;
            string KeyValue = GetKeyValueFromGISTab(layerName, globalID, KEY);
            if (KeyValue != null)
            {
                if(double.TryParse(KeyValue,out HighSideVoltage))
                {
                    if (HighSideVoltage < 50)
                    {
                        IsFedStationChecked = true;
                    }
                }
            }

            return IsFedStationChecked;
        }

        private bool IsSubTypeBooster(string layerName, string globalID)
        {
            var KEY = "SUBTYPECD";
            var IsSubTypeBooster = false;
            int SubTypeCD;
            string KeyValue = GetKeyValueFromGISTab(layerName, globalID, KEY);
            if (KeyValue != null)
            {
                if (int.TryParse(KeyValue, out SubTypeCD))
                {
                    if (SubTypeCD == 3)
                    {
                        IsSubTypeBooster = true;
                    }
                }
            }

            return IsSubTypeBooster;
        }
        private short? GetBankCode(string layerName, string globalID)
        {
            var KEY = "BANKCD";
            short BankCode;
            string KeyValue = GetKeyValueFromGISTab(layerName, globalID, KEY);
            if (KeyValue != null)
            {
                if (short.TryParse(KeyValue, out BankCode))
                {
                    return BankCode;
                }
            }

            return null;
        }

        private decimal GetRegIdFromUnitsForLoadInfo(string[] RegulatorUnitIds)
        {
            decimal RegulatorId = 0;
            if (RegulatorUnitIds != null && RegulatorUnitIds.Length > 0)
            {
                var UnitGlobalId  = RegulatorUnitIds.Last();
                using (SettingsEntities db = new SettingsEntities())
                {
                    var RegulatorData = db.SM_REGULATOR.Where(r => r.GLOBAL_ID == UnitGlobalId && r.CURRENT_FUTURE == "C");
                    if (RegulatorData != null)
                    {
                        RegulatorId = RegulatorData.FirstOrDefault().ID;
                    }
                }
            }
            return RegulatorId;
        }

        #endregion


        #region "Post Methods"

        [HttpPost]
        public ActionResult SaveLoadInfo(TrfBankLoadModel model, string globalID, string layerName, string id, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.Units = units;
            ViewBag.ControllerName = "Regulator";
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
                model.DistributionFedStation = GetHighSideVoltage(layerName, globalID);
                if (ModelState.IsValid)
                {
                    var ErrorMessage = model.ValidateRecentPeakLoadInfo();
                    if(!string.IsNullOrEmpty(ErrorMessage))
                    {
                        ViewBag.ShowPageError = true;
                        ViewBag.SaveErrorMessage = ErrorMessage;
                        return View("EngineeringInfo", model);
                    }
                    using (SettingsEntities db = new SettingsEntities())
                    {
                        var TrfBankLoadData = db.SM_SUBTRF_BANK_LOAD.Where(t => t.REF_DEVICE_ID == model.RegulatorRowId).FirstOrDefault();
                        if (TrfBankLoadData == null)
                        {
                            TrfBankLoadData = new SM_SUBTRF_BANK_LOAD();
                            var MaxLoadId= db.SM_SUBTRF_BANK_LOAD.Select(t => t.ID).Max();
                            TrfBankLoadData.ID = MaxLoadId + 1;
                            model.PopulateEntityFromModel(TrfBankLoadData);
                            db.SM_SUBTRF_BANK_LOAD.AddObject(TrfBankLoadData);
                        }
                        else
                        {
                            var TrfBankLoadHistData = db.SM_SUBTRF_BANK_LOAD_HIST.Where(t => t.SM_SUBTRF_BANK_LOAD_ID == TrfBankLoadData.ID).OrderByDescending(t => t.PEAK_DTM);
                            model.PopulateEntityFromModel(TrfBankLoadData, TrfBankLoadHistData);
                        }
                        db.SaveChanges();
                        ViewBag.ShowSaveSucessful = true;
                        ModelState.Clear();
                        model = GetLoadInfo(model.RegulatorRowId, layerName,globalID);
                        TempData["ShowSaveSuccessful"] = true;
                        return RedirectToAction("EngineeringInfo", new { globalID = globalID, layerName = layerName, units = units });
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


        /*[HttpPost]
        public ActionResult CustomerCount(TrfBankLoadModel model, string globalID, string layerName, string id, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            units = id;
            ViewBag.Units = units;
            ViewBag.ControllerName = "Regulator";
            if (model == null)
            {
                model = SetupEngInfoTestData();
            }

            ViewBag.IsDisabled = false;
            setUiSettings(HtmlExtensions.PageMode.EngineeringInfo);
            return RedirectToAction("EngineeringInfo", new { globalID = globalID, layerName = layerName, units = units, model = model });
        }
        */

        [HttpPost]
        public ActionResult AddNewItem(TrfBankLoadModel model, string globalID, string layerName, string id, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            
            ViewBag.Units = units;
            ViewBag.ControllerName = "Regulator";
            ViewBag.AddNewItemErrorMessage = string.Empty;
            ViewBag.SaveErrorMessage = string.Empty;
            if (model != null)
            {
                model.DistributionFedStation = GetHighSideVoltage(layerName, globalID);
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
                        SM_SUBTRF_BANK_LOAD_HIST TrfBankHist = new SM_SUBTRF_BANK_LOAD_HIST();
                        TrfBankHist.PEAK_DTM = model.GetValidPeakDateTime(model.NewLoadMonth, model.NewLoadTime);
                        TrfBankHist.TOTAL_KW_LOAD = model.NewLoadTotalKW;
                        var MaxHistoryId = db.SM_SUBTRF_BANK_LOAD_HIST.Select(t => t.ID).Max();
                        TrfBankHist.ID = MaxHistoryId + 1;
                        db.SM_SUBTRF_BANK_LOAD_HIST.AddObject(TrfBankHist);
                        var TrfBankLoadData = db.SM_SUBTRF_BANK_LOAD.Where(t => t.REF_DEVICE_ID == model.RegulatorRowId).FirstOrDefault();
                        TrfBankHist.SM_SUBTRF_BANK_LOAD_ID = TrfBankLoadData.ID;
                        var WinterMonths = new int[] { 1, 2, 3, 11, 12 };
                        if (TrfBankHist.PEAK_DTM != null)
                        {
                            if (WinterMonths.Contains(TrfBankHist.PEAK_DTM.Value.Month))
                            {
                                TrfBankLoadData.WINTER_UPDT_DTM = DateTime.Now;
                            }
                            else
                            {
                                TrfBankLoadData.SUMMER_UPDT_DTM = DateTime.Now;
                            }
                        }
                        db.SaveChanges();
                        var TrfBankLoadHistData = db.SM_SUBTRF_BANK_LOAD_HIST.Where(t => t.SM_SUBTRF_BANK_LOAD_ID == TrfBankLoadData.ID).OrderByDescending(t => t.PEAK_DTM);
                        model.PopulateModelFromEntity(TrfBankLoadData, TrfBankLoadHistData.ToList(), GetHighSideVoltage(layerName, globalID));
                        ViewBag.ShowAddSucessful = true;
                        ModelState.Clear();
                        model = GetLoadInfo(model.RegulatorRowId,layerName,globalID);
                        TempData["ShowAddSuccessful"] = true;
                        return RedirectToAction("EngineeringInfo", new { globalID = globalID, layerName = layerName, units = units });
                        //model.NewLoadMonth = null;
                        //model.NewLoadTime = null;
                        //model.NewLoadTotalKW = null;
                    }
                }
                else
                {
                    ViewBag.ShowPageError = true;
                    setUiSettings(HtmlExtensions.PageMode.EngineeringInfo);
                    return View("EngineeringInfo", model);
                }
            }
            else
            {
                ViewBag.ShowPageError = true;
                setUiSettings(HtmlExtensions.PageMode.EngineeringInfo);
                return View("EngineeringInfo", model);
            }
            
            //return View("EngineeringInfo", model);
            
        }

        private TrfBankLoadModel GetLoadInfo(decimal RegulatorRowId,string layerName,string globalID)
        {
            TrfBankLoadModel model = new TrfBankLoadModel();
            model.RegulatorRowId = RegulatorRowId;
            bool IsFedStationChecked = GetHighSideVoltage(layerName, globalID);
                
            using (SettingsEntities db = new SettingsEntities())
            {
                var TrfBankLoadData = db.SM_SUBTRF_BANK_LOAD.Where(t => t.REF_DEVICE_ID == RegulatorRowId).FirstOrDefault();
                if (TrfBankLoadData == null)
                {
                    ViewBag.NoDataFoundMsg = "NO_DATA_FOUND";
                    model.DistributionFedStation = IsFedStationChecked;
                    return model;
                }
                var TrfBankLoadHistData = db.SM_SUBTRF_BANK_LOAD_HIST.Where(t => t.SM_SUBTRF_BANK_LOAD_ID == TrfBankLoadData.ID).OrderByDescending(t => t.PEAK_DTM);
                model.PopulateModelFromEntity(TrfBankLoadData, TrfBankLoadHistData.ToList(), IsFedStationChecked);
                return model;
            }
        }
        [HttpPost]
        public ActionResult SaveFuture(RegulatorModel model, string globalID, string layerName, string id, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            units = id;
            ViewBag.Units = units;

            this.setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

            string[] regulatorUnits = this.getUnitIDs(units);

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
                            //SM_REGULATOR_HIST history = this.saveHistory(db, globalID, model, regulatorUnits);
                            this.saveHistory(db, globalID, model, regulatorUnits);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model, regulatorUnits);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model, regulatorUnits);


                        }
                        else
                        {
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model, regulatorUnits);
                        }

                        try
                        {
                            db.SaveChanges();
                            transaction.Commit();
                            ModelState.Clear();

                            model = this.getDevice(units, HtmlExtensions.PageMode.Future);

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
        public ActionResult CopyToFuture(string globalID, string layerName, string id, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            units = id;
            ViewBag.Units = units;

            // load the future model from current data
            RegulatorModel model = getDevice(units, HtmlExtensions.PageMode.Current);

            setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

            return View("Index", model);
        }

        // this function retired.
        //[HttpPost]
        //public ActionResult CopyToFutureByOperatingNumber(RegulatorModel oldModel, string globalID, string layerName)
        //{
        //    ViewBag.LayerName = layerName;
        //    SM_REGULATOR newEntity = null;
        //    SM_REGULATOR oldEntity = null;
        //    RegulatorModel newModel = new RegulatorModel();

        //    if (!string.IsNullOrEmpty(oldModel.GlobalIDCopy))
        //    {
        //        using (SettingsEntities db = new SettingsEntities())
        //        {
        //            newEntity = db.SM_REGULATOR.SingleOrDefault(s => s.GLOBAL_ID == oldModel.GlobalIDCopy && s.CURRENT_FUTURE == "C");
        //            oldEntity = db.SM_REGULATOR.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");

        //            if (newEntity != null)
        //            {
        //                ModelState.Clear();
        //                //newModel.PopulateModelFromEntity(newEntity);
        //                newEntity = newModel.PopulateEntityFromEnityCopy(newEntity, oldEntity);
        //                //newModel.PopulateModelFromEntity(newEntity);

        //                ViewBag.GlobalID = oldModel.GlobalIDCopy;
        //                setUiSettings(HtmlExtensions.PageMode.Future);
        //                ViewBag.ShowCopyError = false;
        //            }
        //            else
        //            {
        //                ViewBag.GlobalID = globalID;
        //                setUiSettings(HtmlExtensions.PageMode.Current);
        //                ViewBag.ShowCopyError = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //            ViewBag.GlobalID = globalID;
        //            setUiSettings(HtmlExtensions.PageMode.Current);
        //            ViewBag.ShowCopyError = true;
        //    }

        //    initializeDropDowns(newModel);

        //    return View("Index", newModel);
        //}

        [HttpPost]
        public ActionResult AddComment(CommentHistoryModel model, string globalID, string layerName, string id, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            units = id;
            ViewBag.Units = units;

            string[] regulatorUnits = this.getUnitIDs(units);

            using (SettingsEntities db = new SettingsEntities())
            {
                foreach (string unitID in regulatorUnits)
                {
                    addComment(db, unitID, SystemEnums.WorkType.NOTE, model.comments);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("History", new { globalID = globalID, layerName = layerName, units = units });
        }

        [HttpPost]
        public ActionResult CopySamePhaseSettings(RegulatorModel model, string globalID, string layerName, string id, string units)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            units = id;
            ViewBag.Units = units;

            setUiSettings(HtmlExtensions.PageMode.Future);

            if (model.ForwardBStatus != null && model.ForwardBStatus.ToUpper() == "Y")
                {
                    model.ReverseBVoltage = model.ReverseAVoltage;
                    model.ReverseBRset = model.ReverseARset;
                    model.ReverseBXset = model.ReverseAXset;
                    model.ForwardBVoltage = model.ForwardAVoltage;
                    model.ForwardBRset = model.ForwardARset;
                    model.ForwardBXset = model.ForwardAXset;
                }

                if (model.ForwardCStatus != null && model.ForwardCStatus.ToUpper() == "Y")
                {
                    model.ReverseCVoltage = model.ReverseAVoltage;
                    model.ReverseCRset = model.ReverseARset;
                    model.ReverseCXset = model.ReverseAXset;
                    model.ForwardCVoltage = model.ForwardAVoltage;
                    model.ForwardCRset = model.ForwardARset;
                    model.ForwardCXset = model.ForwardAXset;
                }
                TempData["RegModel"] = model;
            

            initializeDropDowns(model);
            return RedirectToAction("Future", new { globalID = globalID, layerName = layerName, units = units });
            //return View("Index", model);
        }

        [HttpPost]
        public ActionResult SettingsFiles(string globalID, string layerName, string id, string units)
        {
            HttpPostedFileBase file = Request.Files["fileSettings"];
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            units = id;
            string[] regulatorUnits = this.getUnitIDs(units);
            string err = "SUCESS";

            if (file.FileName != null && file.FileName.Length > 0)
            {
                uploadFile(file, regulatorUnits[0], "Device Settings-Settings Configuration", out err);
            }
            else
            {
                err = Constants.SelectFileError;
            }
            return RedirectToAction("Files", new { globalID = globalID, layerName = layerName, units = units, settingsError = err, peerReviewError = "" });
        }

        [HttpPost]
        public ActionResult PeerReviewFiles(string globalID, string layerName, string id, string units)
        {
            HttpPostedFileBase file = Request.Files["filePeerReview"];
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            units = id;
            string[] regulatorUnits = this.getUnitIDs(units);
            string err = "SUCESS";

            if (file.FileName != null && file.FileName.Length > 0)
            {
                uploadFile(file, regulatorUnits[0], "Device Settings-Peer Review", out err);
            }
            else
            {
                err = Constants.SelectFileError;
            }
            return RedirectToAction("Files", new { globalID = globalID, layerName = layerName, units = units, settingsError = "", peerReviewError = err });
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
                            SM_REGULATOR e = db.SM_REGULATOR.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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

        private RegulatorModel getDevice(string unitIDs, HtmlExtensions.PageMode pageMode)
        {
            RegulatorModel model = new RegulatorModel();
            string[] regulatorUnitIDs = this.getUnitIDs(unitIDs);
            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            if (regulatorUnitIDs == null) return model;
            using (SettingsEntities db = new SettingsEntities())
            {
                List<RegulatorUnit> regulatorUnits = new List<RegulatorUnit>();

                // build domain model
                foreach (string unitID in regulatorUnitIDs)
                {
                    model = new RegulatorModel();

                    SM_REGULATOR e = db.SM_REGULATOR.SingleOrDefault(s => s.GLOBAL_ID == unitID && s.CURRENT_FUTURE == currentFuture);
                    model.PopulateModelFromEntity(e);

                    RegulatorUnit unit = new RegulatorUnit();
                    unit.GlobalID = e.GLOBAL_ID;
                    unit.ControlType = e.CONTROL_TYPE;
                    unit.ControllerSerialNum = e.CONTROL_SERIAL_NUM;
                    unit.SoftwareVersion = e.SOFTWARE_VERSION;
                    unit.FirmwareVersion = e.FIRMWARE_VERSION;
                    unit.BankCode = e.BANK_CD;
                    regulatorUnits.Add(unit);
                }

                model.RegulatorUnits = regulatorUnits.OrderBy(r => r.BankCode).ToList();
            }
            return model;
        }

        private void saveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, RegulatorModel model, string[] unitIDs)
        {
            int i = 0;
            foreach (RegulatorUnit unit in model.RegulatorUnits)
            {
                string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
                string unitID = unitIDs[i];

                var entity = (from r in db.SM_REGULATOR
                              where r.GLOBAL_ID == unitID && r.CURRENT_FUTURE == currentFuture
                              select r).FirstOrDefault();

                //SM_REGULATOR entity = db.SM_REGULATOR.SingleOrDefault(s => s.GLOBAL_ID == unitID && s.CURRENT_FUTURE == currentFuture);

                // update with unit info
                model.PopulateEntityFromModel(entity);
                entity.CONTROL_TYPE = unit.ControlType;
                entity.CONTROL_SERIAL_NUM = unit.ControllerSerialNum;
                entity.SOFTWARE_VERSION = unit.SoftwareVersion;
                entity.FIRMWARE_VERSION = unit.FirmwareVersion;

                entity.CURRENT_FUTURE = currentFuture;


                entity.DATE_MODIFIED = DateTime.Now;

                db.SaveChanges();
                i++;
            }
        }

        private void saveHistory(SettingsEntities db, string globalID, RegulatorModel model, string[] unitIDs)
        {
            int i = 0;
            foreach (RegulatorUnit unit in model.RegulatorUnits)
            {
                string unitID = unitIDs[i];
                SM_REGULATOR_HIST history = new SM_REGULATOR_HIST();
                SM_REGULATOR e = db.SM_REGULATOR.SingleOrDefault(s => s.GLOBAL_ID == unitID && s.CURRENT_FUTURE == "C");
                model.PopulateHistoryFromEntity(history, e);
                db.SM_REGULATOR_HIST.AddObject(history);
                db.SaveChanges();
                this.addComment(db, unitID, SystemEnums.WorkType.SETT, "Release to Current", history.ID);
                db.SaveChanges();
                i++;
                //return history;
            }
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

            ViewBag.OperatingNum = SiteCache.OperatingNumber(this.getUnitID(), DEVICE_NAME);
            switch (pageMode)
            {
                case HtmlExtensions.PageMode.Current:
                    if (ViewBag.layerName == "Proposed Voltage Regulator")
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
                    if (ViewBag.layerName == "Proposed Voltage Regulator")
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
                    if (ViewBag.layerName == "Proposed Voltage Regulator")
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
                    if (ViewBag.layerName == "Proposed Voltage Regulator")
                    {
                        ViewBag.Title = "GIS Attributes for (Proposed) " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }
                    else
                    {
                        ViewBag.Title = "GIS Attributes for " + DEVICE_NAME + " - " + ViewBag.OperatingNum;
                    }

                    break;
                case HtmlExtensions.PageMode.File:
                    if (ViewBag.layerName == "Proposed Voltage Regulator")
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

        private void initializeDropDowns(RegulatorModel sectModel)
        {
            if (sectModel.RegulatorUnits == null)
            {
                List<RegulatorUnit> units = new List<RegulatorUnit>();
                RegulatorUnit unit1 = new RegulatorUnit();
                units.Add(unit1);
                sectModel.RegulatorUnits = units;
            }

            sectModel.ControlTypeDictionary = SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "CONTROL_TYPE");
            //sectModel.RegulatorUnits[0].ControlTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "CONTROL_TYPE"), "Key", "Value", sectModel.RegulatorUnits[0].ControlType);

            sectModel.BlockedPctList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "BLOCKED_PCT"), "Key", "Value");
            sectModel.ControlTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "CONTROL_TYPE"), "Key", "Value", sectModel.RegulatorUnits[0].ControlType);
            sectModel.EngineeringDocumentList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "ENGINEERING_DOCUMENT"), "Key", "Value");
            sectModel.LoadCycleList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "LOAD_CYCLE"), "Key", "Value");
            sectModel.ModeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "MODE"), "Key", "Value");
            sectModel.PtRatioList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "PT_RATIO"), "Key", "Value");
            sectModel.ScadaRadioManufacturerList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "RADIO_MANF_CD"), "Key", "Value");
            sectModel.RiseRatingList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "RISE_RATING"), "Key", "Value");
            sectModel.ScadaTypeList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "SCADA_TYPE"), "Key", "Value");
            sectModel.RTUManufactureList = new SelectList(SiteCache.GetLookUpValues(DEVICE_TABLE_NAME, "RTU_MANF_CD"), "Key", "Value");
            sectModel.SamePhaseSettingsPostbackScript = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/SamePhaseSettingsChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"';form.submit();";

        }


        #endregion


        private string getUnitID()
        {
            string unitID = ViewBag.Units;
            if (unitID == null) return null;
            return unitID.Split('!')[0];
        }

        private string[] getUnitIDs(string unitIDs)
        {
            if (unitIDs == null) return null;
            string[] regulatorUnitIDs = unitIDs.Split('!');
            List<string> SortedRegulatorUnitIDs = new List<string>();
            using (SettingsEntities db = new SettingsEntities())
            {
                var UnitsInfo = (from RegUnits in db.SM_REGULATOR where regulatorUnitIDs.Contains(RegUnits.GLOBAL_ID) orderby RegUnits.BANK_CD select new { RegUnits.BANK_CD, RegUnits.GLOBAL_ID }).ToList();
                foreach (var Unit in UnitsInfo)
                {
                    SortedRegulatorUnitIDs.Add(Unit.GLOBAL_ID);
                }
            }

            //            Array.Sort(regulatorUnitIDs);
            return SortedRegulatorUnitIDs.Distinct().ToArray();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
public class SetLoadTabVisibility : ActionFilterAttribute
{
    private const string FEATURE_CLASS_NAME = "EDGIS.SUBLoadTapChanger";

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        base.OnActionExecuted(filterContext);
        var Units = string.Empty;
        Units = filterContext.HttpContext.Request.QueryString["units"];
        if (string.IsNullOrEmpty(Units))
            Units = (string)(filterContext.RouteData.Values["units"]);
        if (!string.IsNullOrEmpty(Units))
        {
            var UnitIDs  = Units.Split('!');
            if (UnitIDs != null)
            {
                using(SettingsApp.SettingsEntities db = new SettingsApp.SettingsEntities())
                {
                    foreach(var UnitID in UnitIDs)
                    {
                        SM_REGULATOR e = db.SM_REGULATOR.SingleOrDefault(s => s.GLOBAL_ID == UnitID && s.CURRENT_FUTURE == "C");
                        if (e.FEATURE_CLASS_NAME.ToUpper() == FEATURE_CLASS_NAME.ToUpper())
                        {
                            filterContext.Controller.ViewBag.IsLoadTabVisible = true;
                        }
                        else
                        {
                            filterContext.Controller.ViewBag.IsLoadTabVisible = false;
                        }
                    }
                }
            }

        }
    }
}
