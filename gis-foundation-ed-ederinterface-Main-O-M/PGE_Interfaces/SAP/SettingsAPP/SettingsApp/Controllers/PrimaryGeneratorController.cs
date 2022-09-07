using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SettingsApp.Models;
using SettingsApp.Common;

namespace SettingsApp.Controllers
{
    public class PrimaryGeneratorController : Controller
    {
        private const string DEVICE_TABLE_NAME = "SM_PRIMARY_GEN";
        private const string GENERATOR_TABLE_NAME = "SM_PRIMARY_GEN_DTL";
        private const string CONTROLLER_NAME = "PrimaryGenerator";
        private const string OBJECTID_KEY = "OBJECTID";
        private const string LAYER_NAME = "Primary Generation";
        private const string DEVICE_NAME = "PrimaryGenerator";
        private const string DEVICE_DISPLAY_NAME = "Primary Generator";
        private const string DEVICE_HIST_TABLE_NAME = "SM_PRIMARY_GEN_HIST";

        #region "Action Methods"

        public ActionResult Index(string globalID, string layerName)
        {
            layerName = layerName ?? LAYER_NAME;
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

            PrimaryGenerationSettingsModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);

            setUiSettings(HtmlExtensions.PageMode.Current);
            this.initializeDropDowns(model);



            return View(model);
        }

        public ActionResult Future(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

            PrimaryGenerationSettingsModel model = getDevice(globalID, HtmlExtensions.PageMode.Future);
            setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

            return View("Index", model);
        }



        public ActionResult GIS(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            var LayerName = !string.IsNullOrEmpty(layerName) ? layerName : LAYER_NAME;
            ViewBag.LayerName = LayerName;
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

            setUiSettings(HtmlExtensions.PageMode.GIS);
            var model = new PrimaryGenerationSettingsModel();

            Tuple<int, string> layer = SiteCache.GetLayerID(LayerName);



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


        public ActionResult Generator(string globalID, string layerName)
        {
            Session.Clear();
            layerName = layerName ?? LAYER_NAME;
            decimal DeviceId = getGenerationId(globalID);
            PrimaryGeneratorModel model = getGenerator(DeviceId);

            setGeneratorUI(model.DeviceId, globalID, layerName);
            this.initializeGeneratorDD(model);

            return View(model);
        }
        #endregion


        #region "Post Methods"

        public ActionResult GeneratorTypeChanged(PrimaryGeneratorModel model, string globalID, string layerName)
        {
            var GeneratorType = model.GeneratorType;
            var OldGeneratorTypeVal = string.Format("OldGeneratorTypeVal{0}",model.DeviceId);
            var GeneratorModel = string.Format("GeneratorModel{0}",model.DeviceId);
            if (!string.IsNullOrEmpty(model.GeneratorType) && !string.IsNullOrWhiteSpace(model.GeneratorType))
            {

                if (Session[OldGeneratorTypeVal] == null)
                {
                    Session[OldGeneratorTypeVal] = GeneratorType;
                    Session[GeneratorModel] = model;
                }
                else
                {
                    GeneratorType = (string)Session[OldGeneratorTypeVal];
                    Session[OldGeneratorTypeVal] = model.GeneratorType;
                    if (Session[GeneratorModel] == null)
                        Session[GeneratorModel] = model;
                    var OldModel = (PrimaryGeneratorModel)Session[GeneratorModel];
                    model.PopulateGeneratorTypebasedModels(GeneratorType, model, OldModel);
                    model = OldModel;
                }
            }
            setGeneratorUI(model.DeviceId, globalID, layerName);
            initializeGeneratorDD(model);
            return View("Generator", model);
        }

        [HttpPost]
        public ActionResult SaveGenerator(PrimaryGeneratorModel model, string globalID, string layerName)
        {
            this.setGeneratorUI(model.DeviceId, globalID, layerName);
            this.initializeGeneratorDD(model);
            if (string.IsNullOrWhiteSpace(model.GeneratorType) || string.IsNullOrEmpty(model.GeneratorType))
            {
                ViewBag.ShowPageError = true;
                return View("Generator", model);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    using (SettingsEntities db = new SettingsEntities())
                    {
                        db.Connection.Open();
                        using (var Transaction = db.Connection.BeginTransaction())
                        {
                            try
                            {
                                //Added to update the modified date if Generator_Detail is updated Change made for change detection on 9/10/15
                                IQueryable<SM_PRIMARY_GEN> GeneratorEntities = db.SM_PRIMARY_GEN.Where(s => s.GLOBAL_ID == globalID);
                                if (GeneratorEntities != null && GeneratorEntities.Any())
                                {
                                    foreach(var GeneratorEntity in GeneratorEntities)
                                    {
                                        if (GeneratorEntity != null)
                                            GeneratorEntity.DATE_MODIFIED = DateTime.Now;
                                    }
                                }
                                
                                SM_PRIMARY_GEN_DTL entity = db.SM_PRIMARY_GEN_DTL.SingleOrDefault(s => s.PRIMARY_GEN_ID == model.DeviceId);
                                model.PopulateEntityFromModel(entity);
                                db.SaveChanges();
                                Transaction.Commit();
                                ModelState.Clear();
                                model = this.getGenerator(model.DeviceId);
                                this.initializeGeneratorDD(model);
                                ViewBag.ShowSaveSucessful = true;
                                Session.Clear();
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Connection.Close();
                                throw ex;

                            }
                        }
                    }
                    return View("Generator", model);
                }
                else
                {
                    ViewBag.ShowPageError = true;
                    return View("Generator", model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("Generator", model);
        }

        [HttpPost]
        public ActionResult SaveFuture(PrimaryGenerationSettingsModel model, string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

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
                            SM_PRIMARY_GEN_HIST history = this.saveHistory(db, globalID, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model);
                            this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);
                            db.SM_PRIMARY_GEN_HIST.AddObject(history);
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
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);


            // load the future model from current data
            PrimaryGenerationSettingsModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
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
        public ActionResult ScadaChanged(PrimaryGenerationSettingsModel model, string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

            setUiSettings(HtmlExtensions.PageMode.Future);

            ModelState.Remove("FlisrAutomationDevice");
            ModelState.Add("FlisrAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
            ModelState.SetModelValue("FlisrAutomationDevice", new ValueProviderResult("N", "N", null));
            model.FlisrAutomationDevice = "N";

            initializeDropDowns(model);
            return View("Index", model);
        }


        #endregion


        #region "LoadSaveDevice"


        private PrimaryGenerationSettingsModel getDevice(string globalID, HtmlExtensions.PageMode pageMode)
        {
            PrimaryGenerationSettingsModel model = new PrimaryGenerationSettingsModel();

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_PRIMARY_GEN e = db.SM_PRIMARY_GEN.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError(DEVICE_NAME, globalID));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        private void saveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, PrimaryGenerationSettingsModel model)
        {
            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            SM_PRIMARY_GEN entity = db.SM_PRIMARY_GEN.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
            entity.CURRENT_FUTURE = currentFuture;
            model.PopulateEntityFromModel(entity);
            entity.DATE_MODIFIED = DateTime.Now;
        }


        private SM_PRIMARY_GEN_HIST saveHistory(SettingsEntities db, string globalID, PrimaryGenerationSettingsModel model)
        {
            SM_PRIMARY_GEN_HIST history = new SM_PRIMARY_GEN_HIST();
            SM_PRIMARY_GEN e = db.SM_PRIMARY_GEN.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == "C");
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
        #region "Load and Save Primary Generator"
        private PrimaryGeneratorModel getGenerator(decimal DeviceID)
        {
            var model = new PrimaryGeneratorModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_PRIMARY_GEN_DTL e = db.SM_PRIMARY_GEN_DTL.SingleOrDefault(s => s.PRIMARY_GEN_ID == DeviceID);
                if (e == null)
                    throw new Exception("Generator information is not found in the system.");
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        //private void saveGenerator(SettingsEntities db, decimal DeviceID, PrimaryGeneratorModel model)
        //{
        //    SM_PRIMARY_GEN_DTL entity = db.SM_PRIMARY_GEN_DTL.SingleOrDefault(s => s.ID==DeviceID);
        //    model.PopulateEntityFromModel(entity);
        //}
        #endregion

        #region "Load UI"

        private void setUiSettings(HtmlExtensions.PageMode pageMode)
        {
            ViewBag.PageMode = pageMode.ToString().ToUpper();
            ViewBag.ControllerName = CONTROLLER_NAME;

            switch (pageMode)
            {
                case HtmlExtensions.PageMode.Current:
                    ViewBag.Title = "Current Settings for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    ViewBag.CurrentClass = "selected";
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.Future:
                    ViewBag.Title = "Future Settings for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    ViewBag.FutureClass = "selected";
                    ViewBag.IsDisabled = false;
                    break;
                case HtmlExtensions.PageMode.History:
                    ViewBag.Title = "History for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.GIS:
                    ViewBag.Title = "GIS Attributes for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    break;
                case HtmlExtensions.PageMode.File:
                    ViewBag.Title = "Files for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    break;
                case HtmlExtensions.PageMode.EngineeringInfo:
                    ViewBag.Title = ViewBag.ObjectId;
                    break;
                default:
                    break;
            }

            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsDisabled = true;
        }

        private void initializeDropDowns(PrimaryGenerationSettingsModel sectModel)
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

            sectModel.DropDownPostbackScriptScada = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ScadaChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"';form.submit();";
        }



        #endregion
        #region "Load Primary Generator UI"
        private void setGeneratorUI(decimal DeviceId, string globalID, string layerName)
        {
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.IsDisabled = false;
            ViewBag.DeviceId = DeviceId;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            ViewBag.Title = "Generator for " + ViewBag.ObjectId;
            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsDisabled = true;
        }

        private void initializeGeneratorDD(PrimaryGeneratorModel model)
        {
            model.GeneratorTypeList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "GENERATOR_TYPE"), "KEY", "VALUE");
            model.AnsiMotorGroupList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "ANSI_MOTOR_GROUP"), "KEY", "VALUE");
            model.ConfigurationList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "CONFIGURATION"), "KEY", "VALUE");
            model.EstimatedMethodsList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "ESTIMATION_METHOD"), "KEY", "VALUE");
            model.ReactivePowerCapabilityList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "REACTIVE_POWER_CAPABILITY"), "KEY", "VALUE");
            model.RotorTypeList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "ROTOR_TYPE"), "KEY", "VALUE");
            model.SyncGenModelList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "SYNC_GEN_MODEL"), "KEY", "VALUE");
            model.DropDownPostbackScriptGeneratorType = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/GeneratorTypeChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"';form.submit();";
        }

        #endregion
        #region private members
        private decimal getGenerationId(string globalID)
        {
            decimal DeviceId = 0;
            using (SettingsEntities db = new SettingsEntities())
            {
                DeviceId = db.SM_PRIMARY_GEN.SingleOrDefault(g => g.GLOBAL_ID == globalID && g.CURRENT_FUTURE == "C").ID;
            }
            return DeviceId;
        }


        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
