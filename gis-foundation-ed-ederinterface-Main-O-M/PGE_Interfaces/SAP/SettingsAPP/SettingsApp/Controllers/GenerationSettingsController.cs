using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SettingsApp.Models;
using SettingsApp.Common;
using System.Configuration;

//ENOS2EDGIS
namespace SettingsApp.Controllers
{
    public class GenerationSettingsController : Controller
    {
        //
        // GET: /GenerationSettings/

        private const string DEVICE_TABLE_NAME = "SM_GENERATION";
        private const string GENERATION_TABLE_NAME = "SM_PRIMARY_GEN_DTL";
        private const string PROTECTION_TABLE_NAME = "SM_PROTECTION";
        private const string RELAY_TABLE_NAME = "SM_RELAY";
        private const string GENERATOR_TABLE_NAME = "SM_GENERATOR";
        private const string EQUIPMENT_TABLE_NAME = "SM_GEN_EQUIPMENT";
        private const string CONTROLLER_NAME = "GenerationSettings";
        private const string OBJECTID_KEY = "OBJECTID";
        private const string LAYER_NAME = "EDGIS.GENERATIONINFO";
        private const string DEVICE_NAME = "Generation";
        private const string DEVICE_DISPLAY_NAME = "Generation";
        private const string DEVICE_HIST_TABLE_NAME = "SM_GENERATION_HIST";
        private string Prot_ParentType_Gen = ConfigurationManager.AppSettings["PROTECTION_ParentType_Gen"];

        public ActionResult Index(string globalID,string layerName)
        {
            layerName = layerName ?? LAYER_NAME;
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            Session["ObjectId"] = ViewBag.ObjectId;
            GenerationModel model = GetDevice(globalID, HtmlExtensions.PageMode.Generation);
            ViewBag.ParentType = "Generation";
            ViewBag.ParentId = model.ID;
            ViewBag.GenerationID = model.ID;
            ViewBag.ProjectName = model.ProjectName;
            ViewBag.GlobalID = model.GlobalID;
            //Session["SuperuserOn"] = false;
            Security.IsSuperUserActive = false;
            Session["SuperUserButtonText"] = "Activate Super User";
            ViewBag.IsDisabled = true;
            
            setUiSettings(HtmlExtensions.PageMode.Generation);
            Session["CurrentGeneration"] = model;
            ViewBag.ControllerName = CONTROLLER_NAME;
            this.initializeDropDowns(model);
            //******************************ENOS2SAP PhaseIII Start**************************************/
            List<ProtectionModel> PModelList = GetProtectionLists(model);
            foreach (ProtectionModel pm in PModelList)
            {
                List<GeneratorModel> genrList = GetGenerators(pm);
                model.NameplateRating = Convert.ToString(genrList.Sum(x => x.NameplateCapacity));
            }
            /******************************ENOS2SAP PhaseIII End**************************************/

            var tuple = new Tuple<GenerationModel, List<ProtectionModel>>(model,GetProtectionLists(model));
            return View("Generation",tuple);
        }

        private void GetProtections(GenerationModel genModel)
        {
            genModel.ListOfProtection = new List<ProtectionViewModel>();
            List<ProtectionViewModel> protections = new List<ProtectionViewModel>();
            using (SettingsEntities db = new SettingsEntities())
            {
                ProtectionViewModel viewModel = null;
                ProtectionModel model = null;
                IQueryable<SM_PROTECTION> e = db.SM_PROTECTION.Where(s => s.PARENT_ID == genModel.ID);
                if (e != null && e.Count() > 0)
                {
                    foreach (SM_PROTECTION pro in e)
                    {
                        model = new ProtectionModel();
                        viewModel = new ProtectionViewModel();
                        model.PopulateModelFromEntity(pro);
                        viewModel.ActiveProtection = model;
                        protections.Add(viewModel); 
                    }
                    var orderedList = protections.OrderBy(x => x.ActiveProtection.ID).ToList();
                    genModel.ListOfProtection = orderedList;

                }//else
                    //throw new Exception(Constants.GetNoDeviceFoundError("Protection", genModel.ID.ToString()));
            }
        }

        private List<ProtectionModel> GetProtectionLists(GenerationModel genModel)
        {
            List<ProtectionModel> prots = new List<ProtectionModel>();
            using (SettingsEntities db = new SettingsEntities())
            {
                ProtectionModel model = null;
                IQueryable<SM_PROTECTION> e = db.SM_PROTECTION.Where(s => s.PARENT_ID == genModel.ID && s.PARENT_TYPE == Prot_ParentType_Gen);
                if (e != null && e.Count() > 0)
                {
                    foreach (SM_PROTECTION pro in e)
                    {
                        model = new ProtectionModel();
                        model.PopulateModelFromEntity(pro);
                        prots.Add(model);
                    }
                    var orderedList = prots.OrderBy(x => x.ID).ToList();
                    return orderedList;
                    //foreach (ProtectionModel prot in orderedList)
                    //{
                    //    ProtectionViewModel viewModel = new ProtectionViewModel();
                    //    viewModel.ActiveProtection = prot;
                    //    protectionViews.Add(viewModel);
                    //}
                }
                else
                {
                    return prots;
                    //throw new Exception(Constants.GetNoDeviceFoundError("Protection", genModel.ID.ToString()));
                }
            }
        }

        [HttpPost]
        public ActionResult ToggleSuperUser(string genID)
        {
            ViewBag.Layername = "Generation";
            if (Security.IsSuperUserActive)
            {
                Security.IsSuperUserActive = false;
                Session["SuperUserButtonText"] = "Activate Super User";
                ViewBag.IsDisabled = true;
            }
            else
            {
                Security.IsSuperUserActive = true;
                Session["SuperUserButtonText"] = "Deactivate Super User";
                ViewBag.IsDisabled = false;
            }

            if (TempData["unsaved"] == null)
            {
                TempData["unsaved"] = "false";
            }

            return RedirectToAction("Generation", new { generationId=genID });
        }

        private void SaveGenereationHistory(SM_GENERATION smGen, GenerationModel model, SettingsEntities db)
        {
            try
            {
                long MaxProtectionId = (db.SM_GENERATION_HIST.Max(t => (long?)t.ID)) ?? 0;
                SM_GENERATION_HIST genHist = new SM_GENERATION_HIST();
                model.PopulateHistoryFromEntity(genHist, smGen);
                genHist.ID = MaxProtectionId + 1;
                db.SM_GENERATION_HIST.AddObject(genHist);
            }
            catch(Exception ex)
            {

            }
        }

        public ActionResult Generation(string generationId, string layerName)
        {
            layerName = layerName ?? LAYER_NAME;
            ViewBag.GenerationID = generationId;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = generationId;

            GenerationModel model = GetGenerationByID(generationId);
            ViewBag.ParentType = "Generation";
            ViewBag.ParentId = model.ID;
            ViewBag.GenerationID = model.ID;
            ViewBag.ProjectName = model.ProjectName;
            ViewBag.GlobalID = model.GlobalID;

            //******************************ENOS2SAP PhaseIII Start**************************************/
            List<ProtectionModel> PModelList = GetProtectionLists(model);
            foreach (ProtectionModel pm in PModelList)
            {
                List<GeneratorModel> genrList = GetGenerators(pm);
                model.NameplateRating = Convert.ToString(genrList.Sum(x => x.NameplateCapacity));
            }
            /******************************ENOS2SAP PhaseIII End**************************************/

            setUiSettings(HtmlExtensions.PageMode.Generation);
            Session["CurrentGeneration"] = model;
            this.initializeDropDowns(model);
            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();
            var tuple = new Tuple<GenerationModel, List<ProtectionModel>>(model, GetProtectionLists(model));
            return View("Generation", tuple);
           

        }
        public void Generationdata(string generationId, string layerName)
        {
            layerName = layerName ?? LAYER_NAME;
            ViewBag.GenerationID = generationId;
            ViewBag.LayerName = layerName;
            ViewBag.ObjectId = generationId;

            GenerationModel model = GetGenerationByID(generationId);
            ViewBag.ParentType = "Generation";
            ViewBag.ParentId = model.ID;
            ViewBag.GenerationID = model.ID;
            ViewBag.ProjectName = model.ProjectName;
            ViewBag.GlobalID = model.GlobalID;

            setUiSettings(HtmlExtensions.PageMode.Generation);
            Session["CurrentGeneration"] = model;
            this.initializeDropDowns(model);
            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();
           
        }
        private GenerationModel GetGenerationByID(string generationId)
        {
            GenerationModel model = new GenerationModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                var parentIdConv = Convert.ToInt64(generationId); ;
                SM_GENERATION e = db.SM_GENERATION.SingleOrDefault(s => s.ID == parentIdConv);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError("Generation", generationId));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }



        private GenerationModel GetGeneratorByID(string generationId, HtmlExtensions.PageMode pageMode)
        {
            GenerationModel model = new GenerationModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                var parentIdConv = Convert.ToInt64(generationId); ;
                SM_GENERATION e = db.SM_GENERATION.SingleOrDefault(s => s.ID == parentIdConv);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError("Protection", generationId));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        public ActionResult Protection(string parentType, string parentID)
        {
            //layerName = layerName ?? LAYER_NAME;
            //ViewBag.ProtectionLayerName = layerName;
            //ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            ViewBag.ProtectionParentType = parentType;
            ViewBag.ProtectionParentId = parentID;
            ProtectionModel model = GetProtection(parentID, HtmlExtensions.PageMode.Protection);

            setUiSettings(HtmlExtensions.PageMode.Protection);
            this.initializeDropDowns(model);
            return View("Protection",model);
        }

        private ProtectionModel GetProtection(string parentID, HtmlExtensions.PageMode pageMode)
        {
            ProtectionModel model = new ProtectionModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                var parentIdConv = Convert.ToInt64(parentID); ;
                SM_PROTECTION e = db.SM_PROTECTION.SingleOrDefault(s => s.PARENT_ID == parentIdConv);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError("Protection", parentID));
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        #region "Load UI"

        private void setUiSettings(HtmlExtensions.PageMode pageMode)
        {
            ViewBag.PageMode = pageMode.ToString().ToUpper();
            ViewBag.ControllerName = CONTROLLER_NAME;
          
            ViewBag.GenerationID = ViewBag.ObjectId;
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
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.Generation:
                    if (ViewBag.ProjectName == null || ViewBag.ProjectName == "")
                    {

                        ViewBag.Title = "Generation - " + ViewBag.ObjectId;
                    }
                    else
                    {
                        ViewBag.Title = "Generation - " + ViewBag.ProjectName;
                    }
                   // ViewBag.Title = "Generation - " + Session["ObjectId"];
                    ViewBag.Generation = "selected";
                    ViewBag.IsDisabled = true;
                    break;
                case HtmlExtensions.PageMode.Protection:
                    ViewBag.Title = "Protection for Generation ID : " + ViewBag.ProtectionParentId;
                    ViewBag.Protection = "selected";
                    ViewBag.IsDisabled = true;
                    break;
                default:
                    break;
            }

            if (Security.IsSuperUserActive)
            {
                ViewBag.IsDisabled = false;
            }
            else
            {
                //if (!SettingsApp.Common.Security.IsInAdminGroup)
                    ViewBag.IsDisabled = true;
            }
        }

        private void initializeDropDowns(object sectModel)
        {
            if (sectModel.GetType() == typeof(ProtectionModel))
            {
                ((ProtectionModel)sectModel).ProtectionTypeList = new SelectList(SiteCache.GetLookUpValues("SM_PROTECTION", "PROTECTION_TYPE"), "Key", "Value");
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
            }
            else if (sectModel is GenEquipmentModel)
            {
                ((GenEquipmentModel)sectModel).GenTechCodeList = new SelectList(SiteCache.GetLookUpValues("SM_GEN_EQUIPMENT", "GEN_TECH_CD"), "Key", "Value");
            }

            //sectModel.DropDownPostbackScriptScada = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ScadaChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"';form.submit();";
        }

        #endregion

        #region "Load Generation UI"

        private void SetGenerationUI(decimal DeviceId, string globalID, string layerName)
        {
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.GlobalID = globalID;
            ViewBag.LayerName = layerName;
            ViewBag.IsDisabled = false;
            ViewBag.DeviceId = DeviceId;
            //ViewBag.ObjectId = SiteCache.GetObjectIdFromGIS(layerName, globalID, OBJECTID_KEY);
            ViewBag.Title = "Generator for " + ViewBag.ObjectId;
            
            //ENOS2EDGIS -ST - issue resolution
            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsDisabled = true;

            //if (!(SettingsApp.Common.Security.IsInAdminGroup || Security.IsInSuperUserGroup))
            //    ViewBag.IsDisabled = true;

        }

        private void InitializeGeneratorDD(GenerationModel model)
        {
            //model.GeneratorTypeList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "GENERATOR_TYPE"), "KEY", "VALUE");
            //model.AnsiMotorGroupList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "ANSI_MOTOR_GROUP"), "KEY", "VALUE");
            //model.ConfigurationList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "CONFIGURATION"), "KEY", "VALUE");
            //model.EstimatedMethodsList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "ESTIMATION_METHOD"), "KEY", "VALUE");
            //model.ReactivePowerCapabilityList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "REACTIVE_POWER_CAPABILITY"), "KEY", "VALUE");
            //model.RotorTypeList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "ROTOR_TYPE"), "KEY", "VALUE");
            //model.SyncGenModelList = new SelectList(SiteCache.GetLookUpValues(GENERATOR_TABLE_NAME, "SYNC_GEN_MODEL"), "KEY", "VALUE");
            //model.DropDownPostbackScriptGeneratorType = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/GeneratorTypeChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"';form.submit();";
        }

        #endregion

        #region "Load and Save Generation"
        private GenerationModel GetGeneration(decimal DeviceID)
        {
            var model = new GenerationModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_GENERATION e = db.SM_GENERATION.SingleOrDefault(s => s.ID == DeviceID);
                if (e == null)
                    throw new Exception("Generator information is not found in the system.");
                model.PopulateModelFromEntity(e);
            }
            return model;
        }

        #endregion

        #region "LoadSaveDevice"


        private GenerationModel GetDevice(string globalID, HtmlExtensions.PageMode pageMode)
        {
            GenerationModel model = new GenerationModel();

            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_GENERATION e = db.SM_GENERATION.SingleOrDefault(s => s.GLOBAL_ID == globalID);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError(DEVICE_NAME, globalID));
                model.PopulateModelFromEntity(e);
                model.CurrentOrFuture = "C";
            }
            return model;
        }

        private void SaveDevice(SettingsEntities db, string globalID, HtmlExtensions.PageMode pageMode, GenerationModel model)
        {
            string currentFuture = (pageMode == HtmlExtensions.PageMode.Future) ? "F" : "C";
            SM_GENERATION entity = db.SM_GENERATION.SingleOrDefault(s => s.GLOBAL_ID == globalID);
            model.PopulateEntityFromModel(entity);
            entity.DATE_MODIFIED = DateTime.Now;
        }


        private SM_GENERATION_HIST saveHistory(SettingsEntities db, string globalID, GenerationModel model)
        {
            SM_GENERATION_HIST history = new SM_GENERATION_HIST();
            SM_GENERATION e = db.SM_GENERATION.SingleOrDefault(s => s.GLOBAL_ID == globalID);
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

        private void SetSuperUser()
        {
            if (!Security.IsSuperUserActive)
            {
                Session["SuperUserButtonText"] = "Activate Super User";
                ViewBag.IsDisabled = true;
            }
            else
            {
                ViewBag.IsDisabled = false;
                Session["SuperUserButtonText"] = "Deactivate Super User";
            }
        }

        [HttpPost]
        public ActionResult SaveGeneration([Bind(Prefix = "Item1")]GenerationModel model)
        {
            ViewBag.Generation = "selected";
            try
            {
                //User is a Super user, and Super user edit mode is active
                if (ModelState.IsValid)
                {
                    using (SettingsEntities db = new SettingsEntities())
                    {
                        db.Connection.Open();
                        using (var Transaction = db.Connection.BeginTransaction())
                        {
                            try
                            {
                                SM_GENERATION entity = db.SM_GENERATION.SingleOrDefault(s => s.GLOBAL_ID == model.GlobalID);
                                if (entity != null)
                                {
                                    bool canSave = false;
                                    if (Security.IsInSuperUserGroup && Security.IsSuperUserActive)
                                    {
                                        //Save all edits made in the genearion tab
                                        SaveGenereationHistory(entity, model, db);
                                        model.PopulateEntityFromModel(entity);
                                        canSave = true;
                                    }
                                    else if (Security.IsInAdminGroup)
                                    {
                                        //Save ONLY Notes field value
                                        SaveGenereationHistory(entity, model, db);
                                        entity.NOTES = model.Notes;
                                        entity.DIRECT_TRANSFER_TRIP = model.DirectTransferTrip ? "Y" : "N";
                                        entity.GRD_FAULT_DETECTION_CD = model.GrdFaultDetectionCd ? "Y" : "N";
                                        canSave = true;
                                    }
                                    else
                                    {
                                        TempData["msg"] = "<script>alert('User is not allowed to edit data in the current page');</script>";
                                        TempData["unsaved"] = "false";
                                        canSave = false;
                                        return RedirectToAction("Generation", new { generationId = model.ID.ToString() });
                                    }
                                    if(canSave)
                                    {
                                        
                                        entity.DATE_MODIFIED = DateTime.Now;
                                        entity.MODIFIEDBY = Security.CurrentUser;
                                        db.SaveChanges();
                                        Transaction.Commit();
                                        ModelState.Clear();
                                        TempData["unsaved"] = "false";
                                        Session.Clear();
                                        TempData["ShowSaveSucessful"] = true;
                                        setUiSettings(HtmlExtensions.PageMode.Generation);
                                        return RedirectToAction("Generation", new { generationId = model.ID.ToString() });
                                       // return View("Generation", new { generationId = model.ID.ToString() });
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ViewBag.ShowPageError = true;
                                Transaction.Rollback();
                                db.Connection.Close();
                                TempData["unsaved"] = "true";
                                TempData["ShowPageError"] = true;
                                ViewBag.IsDisabled = false;
                                ViewBag.ShowPageError = true;
                                return RedirectToAction("Generation", new { generationId = model.ID.ToString() });
                            }
                        }
                      }
                    }
                    else
                    {
                        ViewBag.ShowPageError = true;
                        ViewBag.ParentType = "Generation";
                        ViewBag.ParentId = model.ID;
                        ViewBag.GenerationID = model.ID;
                        ViewBag.ProjectName = model.ProjectName;
                        ViewBag.GlobalID = model.GlobalID;
                        ViewBag.ObjectId = model.ID;
                        setUiSettings(HtmlExtensions.PageMode.Generation);
                        Session["CurrentGeneration"] = model;
                        this.initializeDropDowns(model);
                        ViewBag.ControllerName = CONTROLLER_NAME;
                        SetSuperUser();
                        var tuple = new Tuple<GenerationModel, List<ProtectionModel>>(model, GetProtectionLists(model));
                        return View("Generation", tuple);
                    }
               
            }
            catch (Exception ex)
            {
                ViewBag.ShowPageError = true;
                TempData["unsaved"] = "false";
                return RedirectToAction("Generation", new { generationId = model.ID.ToString() });
            }
            return RedirectToAction("Generation", new { generationId = model.ID.ToString() });
            
        }


        /******************************ENOS2SAP PhaseIII Start**************************************/
        public List<GeneratorModel> GetGenerators(ProtectionModel model)
        {
            List<GeneratorModel> lstofGenerators = new List<GeneratorModel>();
            GeneratorModel genrModel = null;
            using (SettingsEntities db = new SettingsEntities())
            {
                IQueryable<SM_GENERATOR> e = db.SM_GENERATOR.Where(s => s.PROTECTION_ID == model.ID);
                if (e != null && e.Count() > 0)
                {
                    foreach (SM_GENERATOR pro in e)
                    {
                        genrModel = new GeneratorModel();
                        genrModel.PopulateModelFromEntity(pro);
                        lstofGenerators.Add(genrModel);
                    }
                    var orderedList = lstofGenerators.OrderBy(x => x.ID).ToList();
                    return orderedList;
                }
                else
                {
                    return lstofGenerators;
                }
            }
        }
        /******************************ENOS2SAP PhaseIII End**************************************/
    }

}
