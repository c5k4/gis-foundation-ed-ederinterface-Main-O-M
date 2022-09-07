using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SettingsApp.Models;
using SettingsApp.Common;
using System.Configuration;

namespace SettingsApp.Controllers
{
    public class PrimaryMeterController : Controller
    {
        //
        // GET: /PrimaryMeter/
        private const string DEVICE_TYPE = "PRIMARYMETER";
        private const string SPECIAL_LOAD_TABLE_NAME = "SM_SPECIAL_LOAD";
        private const string DEVICE_TABLE_NAME = "SM_PRIMARY_METER";
        private const string CONTROLLER_NAME = "primarymeter";
        private const string OBJECTID_KEY = "OBJECTID";
        private const string LAYER_NAME = "Primary Meter";
        private const string DEVICE_NAME = "PrimaryMeter";
        private const string DEVICE_DISPLAY_NAME = "Primary Meter";
        private const string DEVICE_HIST_TABLE_NAME = "SM_PRIMARY_METER_HIST";
        //**************ENOS2EDGIS start**********************************************
        private string RelayCd_PPHA = ConfigurationManager.AppSettings["RelayCd_PPHA"];
        private string RelayCd_PGRD = ConfigurationManager.AppSettings["RelayCd_PGRD"];
        private string RelayCd_BPHA = ConfigurationManager.AppSettings["RelayCd_BPHA"];
        private string RelayCd_BGRD = ConfigurationManager.AppSettings["RelayCd_BGRD"];
        private string Prot_ParentType_PM = ConfigurationManager.AppSettings["PROTECTION_ParentType_PM"];
        private string PROTECTION_Type_RELY = ConfigurationManager.AppSettings["PROTECTION_Type_RELY"];
        private string CurrentOrFuture_C = ConfigurationManager.AppSettings["CurrentOrFuture_C"];
        private string CurrentOrFuture_F = ConfigurationManager.AppSettings["CurrentOrFuture_F"];
        //**************ENOS2EDGIS end**********************************************
        
        string OPERATING_NUM;
        #region Initialization
        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    try
        //    {
        //        base.Initialize(requestContext);
        //        var ObjectId = requestContext.RouteData.Values["ObjectId"] != null ? requestContext.RouteData.Values["ObjectId"].ToString() : string.Empty;
        //        var GlobalId = requestContext.RouteData.Values["GlobalId"] != null ? requestContext.RouteData.Values["GlobalId"].ToString() : string.Empty;
        //        var LayerName = requestContext.RouteData.Values["LayerName"] != null ? requestContext.RouteData.Values["LayerName"].ToString() : string.Empty;
        //        if (string.IsNullOrEmpty(ObjectId))
        //        {

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        #endregion
        #region Special Load
        public ActionResult SpecialLoad(string globalID, string LayerName)
        {
            var model = new SpecialLoadModel();
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.GlobalId = globalID;
            var layerName = LayerName ?? LAYER_NAME;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            ViewBag.Title = string.Format("Special Load for Device {0} - {1}", DEVICE_DISPLAY_NAME, ViewBag.ObjectId);
            var attributeValues = new Dictionary<string, string>();
            try
            {
                if (!string.IsNullOrEmpty(globalID))
                {
                    model.PopulateModelFromEntity(globalID, DEVICE_TYPE);
                }
                else
                {
                    ViewBag.ShowPageError = true;
                    ViewBag.ErrorMessages = "Invalid global id!";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveSpecialLoad(SpecialLoadModel model, string globalID, string LayerName)
        {
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.GlobalId = globalID;
            var layerName = !string.IsNullOrEmpty(LayerName) ? LayerName : LAYER_NAME;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            ViewBag.Title = string.Format("Special Load for Device {0} - {1}", DEVICE_DISPLAY_NAME, ViewBag.ObjectId);

            ViewBag.LayerName = layerName;

            try
            {
                if (model != null)
                {
                    if (ModelState.IsValid)
                    {
                        model.PopulateEntityFromModel(globalID, DEVICE_TYPE);
                        model.PopulateModelFromEntity(globalID, DEVICE_TYPE);
                        TempData["ShowAddSuccessful"] = true;
                        return Redirect(string.Format("/{0}/{1}?globalid={2}&LayerName={3}", CONTROLLER_NAME, "specialload", globalID, LayerName));
                        //return RedirectToAction("SpecialLoad", new { GlobalId = GlobalId });
                    }
                    else
                    {
                        ViewBag.ShowPageError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ShowPageError = true;
                ViewBag.ErrorMessages = ex.Message;

            }
            return View("SpecialLoad", model);
        }

        #endregion



        #region "Action Methods"

        //public ActionResult Index(string globalID, string layerName)
        //{
        //    layerName = layerName ?? LAYER_NAME;
        //    ViewBag.GlobalID = globalID;
        //    ViewBag.LayerName = layerName;
        //    ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

        //    PrimaryMeterModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);

        //    setUiSettings(HtmlExtensions.PageMode.Current);
        //    this.initializeDropDowns(model);
        //    return View(model);
        //}

        public ActionResult Future(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

            PrimaryMeterModel model = getDevice(globalID, HtmlExtensions.PageMode.Future);
            setUiSettings(HtmlExtensions.PageMode.Future);
            this.initializeDropDowns(model);

            return View("Index", model);
        }

        public ActionResult Index(string globalID, string layerName)
        {
            layerName = layerName ?? LAYER_NAME;
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

            PrimaryMeterModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
            //ENOS2EDGIS, Modified code to add protection for primary meter data
            model.DateModified = DateTime.Now.ToString("MM/dd/yyyy");
            GetProtection(model);

            setUiSettings(HtmlExtensions.PageMode.Protection);
            this.initializeDropDowns(model);
            ViewBag.Protection = "selected";
            ViewBag.ProtectionParentId = model.ID.ToString();
            return View(model);
        }
        /// <summary>
        /// ENOS2EDGIS, Executed when user click 'Save' Button.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="globalID"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveProtection(PrimaryMeterModel model, string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            ViewBag.Protection = "selected";

            try
            {
                if (ModelState.IsValid)
                {
                    SettingsEntities db = new SettingsEntities();
                    db.Connection.Open();
                    using (var transaction = db.Connection.BeginTransaction())
                    {
                        if (Security.IsInAdminGroup)
                        {
                            if (SaveRelays(model))
                            {
                                model = SaveCurrentProtection(model);
                            }
                        }
                        else
                        {
                            TempData["msg"] = "<script>alert('Activate Super User to edit data');</script>";
                            return PrepareView(model);
                        }
                        SM_PRIMARY_METER_HIST history = this.saveHistory(db, globalID, model);
                        this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model);
                        // this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);
                        db.SM_PRIMARY_METER_HIST.AddObject(history);
                        //db.SaveChanges();
                        //this.addComment(db, globalID, SystemEnums.WorkType.SETT, "Release to Current", history.ID);

                        try
                        {
                            db.SaveChanges();
                            transaction.Commit();
                            ModelState.Clear();
                            model = this.getDevice(globalID, HtmlExtensions.PageMode.Current);
                            GetProtection(model);
                            ViewBag.ShowSaveSucessful = true;
                            TempData["unsaved"] = "false";
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            db.Connection.Close();
                            ViewBag.ShowPageError = true;
                            TempData["unsaved"] = "true";
                            return PrepareView(model);
                        }
                    }
                }
                else
                {
                    ViewBag.ShowPageError = true;
                    TempData["unsaved"] = "true";
                }
                return PrepareView(model);
            }
            catch (Exception ex)
            {
                ViewBag.ShowPageError = true;
                TempData["unsaved"] = "true";
                return PrepareView(model);
            }
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ActionResult PrepareView(PrimaryMeterModel model)
        {
            model.DateModified = DateTime.Now.ToString("MM/dd/yyyy");
            this.initializeDropDowns(model);
            this.initializeDropDowns(model.ActiveProtection);
            setUiSettings(HtmlExtensions.PageMode.Protection);
            GetRelays(model);
            return View("Index", model);
        }
        /// <summary>
        ///ENOS2EDGIS, post method that saves data for protection
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        private PrimaryMeterModel SaveCurrentProtection(PrimaryMeterModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    SM_PROTECTION smProtection = db.SM_PROTECTION.FirstOrDefault(x => x.ID == model.ActiveProtection.ID && x.PARENT_TYPE == Prot_ParentType_PM);
                    if (smProtection == null)
                    {
                        smProtection = new SM_PROTECTION();
                        model.ActiveProtection.PopulateEntityFromModel(smProtection);
                        smProtection.DATECREATED = DateTime.Now;
                        smProtection.CREATEDBY = Security.CurrentUser;
                        db.SM_PROTECTION.AddObject(smProtection);
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                            ProtectionModel proModel = GetProtectionbyID(model.ActiveProtection.ID.ToString(), HtmlExtensions.PageMode.Protection);
                            this.initializeDropDowns(proModel);
                            model.ActiveProtection = proModel;
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                            return null;
                        }

                    }
                    else
                    {
                        model.ActiveProtection.PopulateEntityFromModel(smProtection);
                        smProtection.DATE_MODIFIED = DateTime.Now;
                        smProtection.MODIFIEDBY = Security.CurrentUser;
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                            ProtectionModel proModel = GetProtectionbyID(model.ActiveProtection.ID.ToString(), HtmlExtensions.PageMode.Protection);
                            this.initializeDropDowns(proModel);
                            model.ActiveProtection = proModel;
                            return model;
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                            return null;
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// ENOS2EDGIS, method to save relay's data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool SaveRelays(PrimaryMeterModel model)
        {
            bool savedAllRealys = true;
            try
            {
                if (Session["PreviousProtectionType"] != null && Session["PreviousProtectionType"].ToString() == PROTECTION_Type_RELY)
                {
                    savedAllRealys = DeleteRelays(model);
                }
                else
                {
                    if (model.ActivePrimaryPhaseRelay != null)
                    {
                        savedAllRealys = SaveRelay(model.ActivePrimaryPhaseRelay);
                    }
                    if (model.ActivePrimaryGroundRelay != null)
                    {
                        savedAllRealys = SaveRelay(model.ActivePrimaryGroundRelay);
                    }
                    if (model.ActiveBackupPhaseRelay != null)
                    {
                        savedAllRealys = SaveRelay(model.ActiveBackupPhaseRelay);
                    }
                    if (model.ActiveBackupGroundRelay != null)
                    {
                        savedAllRealys = SaveRelay(model.ActiveBackupGroundRelay);
                    }
                    return savedAllRealys;
                }
                return savedAllRealys;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ENOS2EDGIS, code to add backup relay for existing main relay
        /// </summary>
        /// <param name="model"></param>
        /// <param name="globalID"></param>
        /// <param name="layerName"></param>
        /// <param name="protectionID"></param>
        /// <param name="backupType"></param>
        /// <returns></returns>
        public ActionResult AddBackupRelay(PrimaryMeterModel model, string globalID, string layerName, string protectionID, string backupType)
        {
            ViewBag.AddBackup = true;
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            ViewBag.Protection = "selected";

            GetRelays(model);
            CreateRelays(model, backupType);

            if (model.Relays != null && model.Relays.Count > 0)
            {
               
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
               
            }           
            model.DateModified = DateTime.Now.ToString("MM/dd/yyyy");
            setUiSettings(HtmlExtensions.PageMode.Current);
            this.initializeDropDowns(model.ActiveProtection);
            ViewBag.ControllerName = CONTROLLER_NAME;
            return View("Index", model);
        }
        /// <summary>
        /// ENOS2EDGIS, method to delete back up relay
        /// </summary>
        /// <param name="model"></param>
        /// <param name="globalID"></param>
        /// <param name="layerName"></param>
        /// <param name="protectionID"></param>
        /// <param name="relayID"></param>
        /// <returns></returns>
        public ActionResult DeleteBackupRelay(PrimaryMeterModel model, string globalID, string layerName, string protectionID, string relayID)
        {

            ViewBag.DeleteBackup = true;
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            ViewBag.Protection = "selected";


            GetRelays(model);
            DeleteRelay(model, relayID);

            if (model.Relays != null && model.Relays.Count > 0)
            {
                
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
                
            }
            model.DateModified = DateTime.Now.ToString("MM/dd/yyyy");
            setUiSettings(HtmlExtensions.PageMode.Current);
            ViewBag.ControllerName = CONTROLLER_NAME;
            this.initializeDropDowns(model.ActiveProtection);
            return View("Index", model);
        }


        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="primaryModel"></param>
        /// <param name="relayID"></param>
        private void DeleteRelay(PrimaryMeterModel primaryModel, string relayID)
        {
            if (primaryModel.Relays != null && primaryModel.Relays.Count > 0)
            {
                int relayConId = Convert.ToInt32(relayID);
                int exisitngRelIdx = primaryModel.Relays.FindIndex(x => x.ID == relayConId);
                if (exisitngRelIdx != -1)
                {
                    RelayModel relModel = primaryModel.Relays[exisitngRelIdx];
                    if (DeleteRelay(relModel))
                    {
                        primaryModel.Relays.RemoveAt(exisitngRelIdx);
                    }
                }
            }
        }
        /// <summary>
        /// ENOS2EDGIS, main method which actually deletes relays from database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool DeleteRelays(ProtectionViewModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    IQueryable<SM_RELAY> smRelays = db.SM_RELAY.Where(x => x.PROTECTION_ID == model.ActiveProtection.ID);
                    if (smRelays != null && smRelays.Count() > 0)
                    {
                        foreach (SM_RELAY smRelay in smRelays)
                        {
                            db.DeleteObject(smRelay);
                        }
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="smRelay"></param>
        /// <param name="model"></param>
        /// <param name="db"></param>
        private void SaveRelayHistory(SM_RELAY smRelay, RelayModel model, SettingsEntities db)
        {
            try
            {
                long MaxProtectionId = (db.SM_RELAY_HIST.Max(t => (long?)t.ID)) ?? 0;
                SM_RELAY_HIST relayHist = new SM_RELAY_HIST();
                model.PopulateHistoryFromEntity(relayHist, smRelay);
                relayHist.ID = MaxProtectionId + 1;
                db.SM_RELAY_HIST.AddObject(relayHist);
                db.SaveChanges();
            }
            catch
            {

            }
        }

        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="smProtection"></param>
        /// <param name="model"></param>
        /// <param name="db"></param>
        private void SaveProtectionHistory(SM_PROTECTION smProtection, ProtectionModel model, SettingsEntities db)
        {
            try
            {
                long MaxProtectionId = (db.SM_PROTECTION_HIST.Max(t => (long?)t.ID)) ?? 0;
                SM_PROTECTION_HIST proHist = new SM_PROTECTION_HIST();
                model.PopulateHistoryFromEntity(proHist, smProtection);
                proHist.ID = MaxProtectionId + 1;
                db.SM_PROTECTION_HIST.AddObject(proHist);
                db.SaveChanges();
            }
            catch
            {

            }
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pageMode"></param>
        /// <returns></returns>
        private ProtectionModel GetProtectionbyParentID(string ID, HtmlExtensions.PageMode pageMode)
        {
            ProtectionModel model = new ProtectionModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                var proIdConv = Convert.ToInt64(ID);
                //ENOS2EDGIS start
                //SM_PROTECTION e = db.SM_PROTECTION.SingleOrDefault(s => s.PARENT_ID == proIdConv && s.PARENT_TYPE == "PRIMARYMETER"); 
                SM_PROTECTION e = db.SM_PROTECTION.FirstOrDefault(s => s.PARENT_ID == proIdConv && s.PARENT_TYPE == Prot_ParentType_PM);
                //ENOS2EDGIS end
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError("Protection", ID));
                model.PopulateModelFromEntity(e);
                this.initializeDropDowns(model);
            }
            return model;
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pageMode"></param>
        /// <returns></returns>
        private ProtectionModel GetProtectionbyID(string ID, HtmlExtensions.PageMode pageMode)
        {
            ProtectionModel model = new ProtectionModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                var proIdConv = Convert.ToInt64(ID);
                SM_PROTECTION e = db.SM_PROTECTION.SingleOrDefault(s => s.ID == proIdConv && s.PARENT_TYPE == Prot_ParentType_PM);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError("Protection", ID));
                model.PopulateModelFromEntity(e);
                this.initializeDropDowns(model);
            }
            return model;
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="model"></param>
        private void GetProtection(PrimaryMeterModel model)
        {
            ProtectionModel proModel = GetProtectionbyParentID(model.ID.ToString(), HtmlExtensions.PageMode.Protection);
            this.initializeDropDowns(proModel);
            model.ActiveProtection = proModel;
            GetRelays(model);
            if (model.Relays != null && model.Relays.Count > 0)
            {
               
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
               
            }
        }        
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="model"></param>
        private void GetRelays(PrimaryMeterModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                IQueryable<SM_RELAY> e = db.SM_RELAY.Where(s => s.PROTECTION_ID == model.ActiveProtection.ID);
                if (e != null && e.Count() > 0)
                {
                    model.Relays = new List<RelayModel>();
                    if (Session["PreviousProtectionType"] != null && Session["PreviousProtectionType"].ToString() == PROTECTION_Type_RELY)
                    {
                        //The current protectio have some relays, user changed it to something else, so delete the existing relays
                        DeleteRelays(model);
                    }
                    else
                    {
                        //If there are any active relays in the model, add them first, because those will contain the updates from user
                        AddActiveRelays(model);
                        foreach (SM_RELAY relay in e)
                        {
                            if (model.Relays.FindIndex(x => x.ID == relay.ID) == -1)
                            {
                                RelayModel relModel = new RelayModel();
                                relModel.CurrentOrFuture = CurrentOrFuture_C;
                                relModel.PopulateModelFromEntity(relay);
                                this.initializeDropDowns(relModel);
                                model.Relays.Add(relModel);
                            }
                        }
                    }
                }
                else
                {
                    if (Session["PreviousProtectionType"] != null && Session["PreviousProtectionType"].ToString() != PROTECTION_Type_RELY && model.ActiveProtection.ProtectionType == PROTECTION_Type_RELY)
                    {
                        //this means user want to add RELAY as protection type, so create both primary phase & primary ground relays
                        CreateRelays(model, string.Empty);
                    }
                    //throw new Exception(Constants.GetNoDeviceFoundError("Generator", model.ID.ToString()));
                }
            }
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="model"></param>
        private void AddActiveRelays(PrimaryMeterModel model)
        {
            if (model.Relays == null)
                model.Relays = new List<RelayModel>();
            if (model.ActivePrimaryPhaseRelay != null)
            {
                if (model.Relays.FindIndex(x => x.ID == model.ActivePrimaryPhaseRelay.ID) == -1)
                {
                    this.initializeDropDowns(model.ActivePrimaryPhaseRelay);
                    model.Relays.Add(model.ActivePrimaryPhaseRelay);
                }
            }
            if (model.ActivePrimaryGroundRelay != null)
            {
                if (model.Relays.FindIndex(x => x.ID == model.ActivePrimaryGroundRelay.ID) == -1)
                {
                    this.initializeDropDowns(model.ActivePrimaryGroundRelay);
                    model.Relays.Add(model.ActivePrimaryGroundRelay);
                }
            }
            if (model.ActiveBackupPhaseRelay != null)
            {
                if (model.Relays.FindIndex(x => x.ID == model.ActiveBackupPhaseRelay.ID) == -1)
                {
                    this.initializeDropDowns(model.ActiveBackupPhaseRelay);
                    model.Relays.Add(model.ActiveBackupPhaseRelay);
                }
            }
            if (model.ActiveBackupGroundRelay != null)
            {
                if (model.Relays.FindIndex(x => x.ID == model.ActiveBackupGroundRelay.ID) == -1)
                {
                    this.initializeDropDowns(model.ActiveBackupGroundRelay);
                    model.Relays.Add(model.ActiveBackupGroundRelay);
                }
            }
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="model"></param>
        /// <param name="relayType"></param>
        private void CreateRelays(PrimaryMeterModel model, string relayType)
        {
            if (model.Relays == null && string.IsNullOrEmpty(relayType))
            {
                model.Relays = new List<RelayModel>();
                if (model.ActivePrimaryPhaseRelay != null)
                {
                    this.initializeDropDowns(model.ActivePrimaryPhaseRelay);
                    model.Relays.Add(model.ActivePrimaryPhaseRelay);
                }
                else
                    model.Relays.Add(CreateRelay(RelayCd_PPHA, model));

                if (model.ActivePrimaryGroundRelay != null)
                {
                    this.initializeDropDowns(model.ActivePrimaryGroundRelay);
                    model.Relays.Add(model.ActivePrimaryGroundRelay);
                }
                else
                    model.Relays.Add(CreateRelay(RelayCd_PGRD, model));

                if (model.ActiveBackupPhaseRelay != null)
                {
                    this.initializeDropDowns(model.ActiveBackupPhaseRelay);
                    model.Relays.Add(model.ActiveBackupPhaseRelay);
                }

                if (model.ActiveBackupGroundRelay != null)
                {
                    this.initializeDropDowns(model.ActiveBackupGroundRelay);
                    model.Relays.Add(model.ActiveBackupGroundRelay);
                }
            }
            if (ViewBag.AddBackup != null && (bool)ViewBag.AddBackup)
            {
                if (model.ActiveBackupPhaseRelay != null)
                {
                    this.initializeDropDowns(model.ActiveBackupPhaseRelay);
                    model.Relays.Add(model.ActiveBackupPhaseRelay);
                }
                else
                {
                    if (relayType == RelayCd_BPHA)
                    {
                        model.Relays.Add(CreateRelay(RelayCd_BPHA, model));
                    }
                }

                if (model.ActiveBackupGroundRelay != null)
                {
                    this.initializeDropDowns(model.ActiveBackupGroundRelay);
                    model.Relays.Add(model.ActiveBackupGroundRelay);
                }
                else
                {
                    if (relayType == RelayCd_BGRD)
                    {
                        model.Relays.Add(CreateRelay(RelayCd_BGRD, model));
                    }
                }
            }
        }

        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="relayCD"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private RelayModel CreateRelay(string relayCD, PrimaryMeterModel model)
        {
            RelayModel relModel = new RelayModel();
            long mxGeneratorId;
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                if (model.Relays != null && model.Relays.Count > 0)
                {
                    mxGeneratorId = model.Relays != null && model.Relays.Count > 0 ? (long)model.Relays.Max(x => x.ID) : (long)db.SM_RELAY.Max(t => (long?)t.ID);
                }
                else
                {
                    mxGeneratorId = 0;
                }
                relModel.DateCreated = DateTime.Now.ToShortDateString();
                relModel.CreatedBy = Security.CurrentUser;
                relModel.RelayCd = relayCD;
                relModel.RelayType = "1";
                relModel.CurveType = "1";
                if (relayCD == RelayCd_PPHA || relayCD == RelayCd_BPHA)
                {
                    relModel.RestraintType = "0";
                }
                relModel.CurrentOrFuture = CurrentOrFuture_C;
                //if (model.Relays != null && model.Relays.Count > 0 && (long)model.Relays.Max(x => x.ID) == mxGeneratorId + 1)
                //    mxGeneratorId = (long)model.Relays.Max(x => x.ID);
                relModel.ID = mxGeneratorId + 1;
                relModel.ProtectionId = model.ActiveProtection.ID;
            }
            this.initializeDropDowns(relModel);
            return relModel;
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool DeleteRelays(PrimaryMeterModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    IQueryable<SM_RELAY> smRelays = db.SM_RELAY.Where(x => x.PROTECTION_ID == model.ActiveProtection.ID);
                    if (smRelays != null && smRelays.Count() > 0)
                    {
                        foreach (SM_RELAY smRelay in smRelays)
                        {
                            db.DeleteObject(smRelay);
                        }
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        private bool SaveRelay(RelayModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    SM_RELAY smRelay = db.SM_RELAY.FirstOrDefault(x => x.ID == model.ID);
                    if (smRelay == null)
                    {
                        smRelay = new SM_RELAY();
                        model.PopulateEntityFromModel(smRelay);
                        smRelay.DATECREATED = DateTime.Now;
                        smRelay.CREATEDBY = Security.CurrentUser;
                        db.SM_RELAY.AddObject(smRelay);
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                            return false;
                        }

                    }
                    else
                    {
                        SaveRelayHistory(smRelay, model, db);
                        model.PopulateEntityFromModel(smRelay);
                        smRelay.DATE_MODIFIED = DateTime.Now;
                        smRelay.MODIFIEDBY = Security.CurrentUser;
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        private bool DeleteRelay(RelayModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    SM_RELAY smRelay = db.SM_RELAY.FirstOrDefault(x => x.ID == model.ID);
                    if (smRelay == null)
                    {
                        //nothing to delete
                        return true;
                    }
                    else
                    {
                        SaveRelayHistory(smRelay, model, db);
                        db.DeleteObject(smRelay);
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                            return false;
                        }
                    }
                }
            }
            return false;
        }



        public ActionResult GIS(string globalID, string layerName)
        {
            ViewBag.GlobalID = globalID;
            var LayerName = !string.IsNullOrEmpty(layerName) ? layerName : LAYER_NAME;
            ViewBag.LayerName = LayerName;
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

            setUiSettings(HtmlExtensions.PageMode.GIS);
            var model = new PrimaryMeterModel();

            Tuple<int, string> layer = SiteCache.GetLayerID(LayerName);

            //TO-DO:

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
        public ActionResult Files(string globalID, string layerName, string settingsError = null, string peerReviewError = null)
        {
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.SettingsFileError = (settingsError == null ? "" : settingsError);
            ViewBag.PeerReviewError = (peerReviewError == null ? "" : peerReviewError);
            ViewBag.SettingsFileUploaded = (settingsError != null && settingsError == "SUCESS" ? true : false);
            ViewBag.PeerReviewFileUploaded = (peerReviewError != null && peerReviewError == "SUCESS" ? true : false); ;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            OPERATING_NUM = ViewBag.ObjectId;
            // ViewBag.OPERATING_NUM = OPERATING_NUM;
            using (SettingsEntities db = new SettingsEntities())
            {
                //***************************ENOS2EDGIS start******************************************************************
                SM_PRIMARY_METER e = db.SM_PRIMARY_METER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == CurrentOrFuture_C);
                //***************************ENOS2EDGIS end******************************************************************
                if (e != null && !string.IsNullOrEmpty(e.DISTRICT) && !string.IsNullOrEmpty(e.DIVISION) && !string.IsNullOrEmpty(OPERATING_NUM))
                {
                    ViewBag.ShowInsufficientMetaDataRequirement = false;
                    ViewBag.D2SettingsURL = Constants.BuildDocumentumUrl(false, e.DIVISION, e.DISTRICT, DEVICE_NAME, OPERATING_NUM);
                    ViewBag.DBPeerReviewURL = Constants.BuildDocumentumUrl(true, e.DIVISION, e.DISTRICT, DEVICE_NAME, OPERATING_NUM);
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

        //[HttpPost]
        //public ActionResult SaveFuture(PrimaryMeterModel model, string globalID, string layerName)
        //{
        //    ViewBag.GlobalID = globalID;
        //    ViewBag.LayerName = layerName;
        //    ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

        //    this.setUiSettings(HtmlExtensions.PageMode.Future);

        //    this.initializeDropDowns(model);

        //    if (model.Scada == null || model.Scada.ToUpper() == "N" || model.FlisrAutomationDevice == null || model.FlisrAutomationDevice == "")
        //    {
        //        ModelState.Remove("FlisrAutomationDevice");
        //        ModelState.Add("FlisrAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
        //        ModelState.SetModelValue("FlisrAutomationDevice", new ValueProviderResult("N", "N", null));
        //        model.FlisrAutomationDevice = "N";
        //    }

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            SettingsEntities db = new SettingsEntities();
        //            db.Connection.Open();
        //            using (var transaction = db.Connection.BeginTransaction())
        //            {
        //                if (model.Release)
        //                {
        //                    SM_PRIMARY_METER_HIST history = this.saveHistory(db, globalID, model);
        //                    this.saveDevice(db, globalID, HtmlExtensions.PageMode.Current, model);
        //                    this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);
        //                    db.SM_PRIMARY_METER_HIST.AddObject(history);
        //                    db.SaveChanges();
        //                    this.addComment(db, globalID, SystemEnums.WorkType.SETT, "Release to Current", history.ID);
        //                }
        //                else
        //                    this.saveDevice(db, globalID, HtmlExtensions.PageMode.Future, model);

        //                try
        //                {
        //                    db.SaveChanges();
        //                    transaction.Commit();
        //                    ModelState.Clear();
        //                    model = this.getDevice(globalID, HtmlExtensions.PageMode.Future);
        //                    this.initializeDropDowns(model);
        //                    ViewBag.ShowSaveSucessful = true;
        //                }
        //                catch (Exception ex)
        //                {
        //                    transaction.Rollback();
        //                    db.Connection.Close();
        //                    throw ex;
        //                }
        //            }

        //            return View("Index", model);
        //        }
        //        else
        //        {
        //            ViewBag.ShowPageError = true;
        //            return View("Index", model);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("Index", model);
        //    }
        //}

        //[HttpPost]
        //public ActionResult CopyToFuture(string globalID, string layerName)
        //{
        //    ViewBag.GlobalID = globalID;
        //    ViewBag.LayerName = layerName;
        //    ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);


        //    // load the future model from current data
        //    PrimaryMeterModel model = getDevice(globalID, HtmlExtensions.PageMode.Current);
        //    setUiSettings(HtmlExtensions.PageMode.Future);
        //    initializeDropDowns(model);

        //    if (model.Scada == null || model.Scada == "" || model.Scada.ToUpper() == "N")
        //    {
        //        ModelState.Remove("FlisrAutomationDevice");
        //        ModelState.Add("FlisrAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
        //        ModelState.SetModelValue("FlisrAutomationDevice", new ValueProviderResult("N", "N", null));
        //        model.FlisrAutomationDevice = "N";
        //    }

        //    return View("Index", model);
        //}

        //[HttpPost]
        //public ActionResult ScadaChanged(PrimaryMeterModel model, string globalID, string layerName)
        //{
        //    ViewBag.GlobalID = globalID;
        //    ViewBag.LayerName = layerName;
        //    ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);

        //    setUiSettings(HtmlExtensions.PageMode.Future);

        //    ModelState.Remove("FlisrAutomationDevice");
        //    ModelState.Add("FlisrAutomationDevice", new ModelState { Value = new ValueProviderResult("N", "N", null) });
        //    ModelState.SetModelValue("FlisrAutomationDevice", new ValueProviderResult("N", "N", null));
        //    model.FlisrAutomationDevice = "N";

        //    initializeDropDowns(model);
        //    return View("Index", model);
        //}

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


        private PrimaryMeterModel getDevice(string globalID, HtmlExtensions.PageMode pageMode)
        {
            PrimaryMeterModel model = new PrimaryMeterModel();
            //***************************ENOS2EDGIS start******************************************************************
            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? CurrentOrFuture_F : CurrentOrFuture_C;
            //***************************ENOS2EDGIS end******************************************************************
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_PRIMARY_METER e = db.SM_PRIMARY_METER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
                if (e == null)
                {
                    throw new Exception(Constants.GetNoDeviceFoundError(DEVICE_NAME, globalID));
                    /*
                    var CurrentFutureValues = new string[] { CurrentOrFuture_C, CurrentOrFuture_F };
                    foreach (var CurrentOrFuture in CurrentFutureValues)
                    {
                        e = new SM_PRIMARY_METER();
                        e.GLOBAL_ID = globalID;
                        e.CURRENT_FUTURE = CurrentOrFuture;
                        db.SM_PRIMARY_METER.AddObject(e);
                        db.SaveChanges();
                    }*/

                }

                model.PopulateModelFromEntity(e);
            }
            return model;
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="model"></param>
        /// <param name="previousProtectionType"></param>
        /// <param name="globalID"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProtectionTypeChanged(PrimaryMeterModel model, string previousProtectionType, string globalID, string layerName)
        {
            ViewBag.PrimaryMeterID = model.ActiveProtection.ParentID;
            ViewBag.GlobalID = model.GlobalId;
            layerName = !string.IsNullOrEmpty(layerName) ? layerName : LAYER_NAME;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, model.GlobalId, OBJECTID_KEY);

            if (!string.IsNullOrEmpty(previousProtectionType))
            {
                Session["PreviousProtectionType"] = previousProtectionType;
            }
            //GetRelays(model);
            model.DateModified = DateTime.Now.ToString("MM/dd/yyyy");
            //PrimaryMeterModel newModel = getDevice(model.GlobalId, HtmlExtensions.PageMode.Current);
            //GetProtection(model);
            GetRelays(model);
            if (model.Relays != null && model.Relays.Count > 0)
            {
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }
            ViewBag.CurrentProtectionId = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = model.ActiveProtection.ID.ToString();
            setUiSettings(HtmlExtensions.PageMode.Current);
            this.initializeDropDowns(model.ActiveProtection);
            TempData["unsaved"] = "true";
            ViewBag.Protection = "selected";
            return View("Index", model);
        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="db"></param>
        /// <param name="globalID"></param>
        /// <param name="pageMode"></param>
        /// <param name="model"></param>
        private void saveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, PrimaryMeterModel model)
        {
            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? CurrentOrFuture_F : CurrentOrFuture_C;
            model.CurrentOrFuture = currentFuture;
            SM_PRIMARY_METER entity = db.SM_PRIMARY_METER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == currentFuture);
            model.PopulateEntityFromModel(entity);
            //entity.CURRENT_FUTURE = currentFuture;
            entity.DATE_MODIFIED = DateTime.Now;
        }


        private SM_PRIMARY_METER_HIST saveHistory(SettingsEntities db, string globalID, PrimaryMeterModel model)
        {
            SM_PRIMARY_METER_HIST history = new SM_PRIMARY_METER_HIST();
            //***************************ENOS2EDGIS start******************************************************************
            SM_PRIMARY_METER e = db.SM_PRIMARY_METER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == CurrentOrFuture_C);
            //***************************ENOS2EDGIS end******************************************************************
            model.PopulateHistoryFromEntity(history, e);
            history.GLOBAL_ID = globalID;
            long MaxProtectionId = (db.SM_PRIMARY_METER_HIST.Max(t => (long?)t.ID)) ?? 0;
            history.ID = MaxProtectionId + 1;
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

            //ViewBag.OperatingNum = SiteCache.OperatingNumber(ViewBag.GlobalID, DEVICE_NAME);
            switch (pageMode)
            {
                case HtmlExtensions.PageMode.Current:

                    if (ViewBag.layerName == "Proposed Primary Meter")
                    {

                        ViewBag.Title = "Current Settings for (Proposed) " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    }
                    else
                    {
                        ViewBag.Title = "Current Settings for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    }
                  
                    ViewBag.CurrentClass = "selected";
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.Future:
                    if (ViewBag.layerName == "Proposed Primary Meter")
                    {

                        ViewBag.Title = "Future Settings for (Proposed) " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    }
                    else
                    {
                        ViewBag.Title = "Future Settings for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    }
                   // ViewBag.Title = "Future Settings for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    ViewBag.FutureClass = "selected";
                    ViewBag.IsDisabled = false;
                    break;
                case HtmlExtensions.PageMode.History:

                    if (ViewBag.layerName == "Proposed Primary Meter")
                    {

                        ViewBag.Title = "History for (Proposed) " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    }
                    else
                    {
                        ViewBag.Title = "History for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    }
                   // ViewBag.Title = "History for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.GIS:
                    if (ViewBag.layerName == "Proposed Primary Meter")
                    {

                        ViewBag.Title = "GIS Attributes for (Proposed) " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    }
                    else
                    {
                        ViewBag.Title = "GIS Attributes for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    }
                   // ViewBag.Title = "GIS Attributes for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    break;
                case HtmlExtensions.PageMode.File:
                    ViewBag.Title = "Files for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;

                    break;
                case HtmlExtensions.PageMode.EngineeringInfo:
                    ViewBag.Title = ViewBag.ObjectId;
                    break;
                //ENOS2EDGIS
                case HtmlExtensions.PageMode.Generation:
                    ViewBag.Title = "Generation - " + ViewBag.ProjectName;
                    ViewBag.Generation = "selected";
                    break;
                //ENOS2EDGIS
                case HtmlExtensions.PageMode.Protection:
                    ViewBag.Title = "Protection for " + DEVICE_DISPLAY_NAME + "-" + ViewBag.ObjectId;
                    ViewBag.GeneratorTitle = ViewBag.ProjectName;
                    ViewBag.Protection = "selected";
                    break;
                default:
                    break;
            }

            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsDisabled = true;
        }
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
                            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS("Primary Meter", globalID, OBJECTID_KEY);
                            string OPERATING_NUM = ViewBag.ObjectId;
                            //***************************ENOS2EDGIS start******************************************************************
                            SM_PRIMARY_METER e = db.SM_PRIMARY_METER.SingleOrDefault(s => s.GLOBAL_ID == globalID && s.CURRENT_FUTURE == CurrentOrFuture_C);
                            //***************************ENOS2EDGIS end******************************************************************
                            Documentum.CreateDocument(globalID, DEVICE_NAME, OPERATING_NUM, e.DISTRICT, e.DIVISION, docType, file);
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
        private void initializeDropDowns(PrimaryMeterModel sectModel)
        {

        }
        /// <summary>
        /// ENOS2EDGIS
        /// </summary>
        /// <param name="sectModel"></param>
        private void initializeDropDowns(object sectModel)
        {
            if (sectModel.GetType() == typeof(ProtectionModel))
            {
                ((ProtectionModel)sectModel).ProtectionTypeList = new SelectList(SiteCache.GetLookUpValues("SM_PROTECTION", "PROTECTION_TYPE", "PRIMARYMETER"), "Key", "Value");
                ((ProtectionModel)sectModel).FuseTypeList = new SelectList(SiteCache.GetLookUpValues("SM_PROTECTION", "FUSE_TYPE"), "Key", "Value");
                ((ProtectionModel)sectModel).RecloseCurveList = new SelectList(SiteCache.GetLookUpValues("SM_PROTECTION", "PHA_SLOW_CURVE"), "Key", "Value");
                ((ProtectionModel)sectModel).ProtectionTypeDropDownPostbackScript = @"var form = document.forms[0]; form.action='/PrimaryMeter/ProtectionTypeChanged?previousProtectionType=" + ((ProtectionModel)sectModel).ProtectionType + "&globalID=" + ViewBag.GlobalID + "&layerName=" + ViewBag.LayerName + @"';form.submit();";
            }
            else if (sectModel.GetType() == typeof(GenerationModel))
            {
                ((GenerationModel)sectModel).ProgramTypeList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATION", "PROGRAM_TYPE"), "Key", "Value");
                ((GenerationModel)sectModel).TechnologyTypeList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATION", "FUEL_TECH_TYPE"), "Key", "Value");
            }
            else if (sectModel is RelayModel)
            {
                ((RelayModel)sectModel).RelayCodeList = new SelectList(SiteCache.GetLookUpValues("SM_RELAY", "RELAY_CD"), "Key", "Value");
                ((RelayModel)sectModel).RelayTypeList = new SelectList(SiteCache.GetLookUpValues("SM_RELAY", "RELAY_TYPE"), "Key", "Value");
                ((RelayModel)sectModel).CurveTypeList = new SelectList(SiteCache.GetLookUpValues("SM_RELAY", "CURVE_TYPE"), "Key", "Value");
                ((RelayModel)sectModel).RestraintList = new SelectList(SiteCache.GetLookUpValues("SM_RELAY", "RESTRAINT_TYPE"), "Key", "Value");
            }
            else if (sectModel is GeneratorModel)
            {
                ((GeneratorModel)sectModel).ProgramTypeList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "PROGRAM_TYPE"), "Key", "Value");
                ((GeneratorModel)sectModel).TechnologyTypeList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "FUEL_TECH_TYPE"), "Key", "Value");
                ((GeneratorModel)sectModel).ConnectionTypeList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "CONNECTION_CD"), "Key", "Value");
                ((GeneratorModel)sectModel).StatusList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "STATUS_CD"), "Key", "Value");
                ((GeneratorModel)sectModel).GenTechCodeList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "GEN_TECH_CD"), "Key", "Value");
                ((GeneratorModel)sectModel).ControlList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "CONTROL_CD"), "Key", "Value");
                ((GeneratorModel)sectModel).ModeOfInverterList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "MODE_OF_INVERTER"), "Key", "Value");

            }
            else if (sectModel is GenEquipmentModel)
            {
                ((GenEquipmentModel)sectModel).GenTechCodeList = new SelectList(SiteCache.GetLookUpValues("SM_GEN_EQUIPMENT", "GEN_TECH_CD"), "Key", "Value");
            }
            //sectModel.DropDownPostbackScriptScada = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ScadaChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"';form.submit();";
        }



        #endregion


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
