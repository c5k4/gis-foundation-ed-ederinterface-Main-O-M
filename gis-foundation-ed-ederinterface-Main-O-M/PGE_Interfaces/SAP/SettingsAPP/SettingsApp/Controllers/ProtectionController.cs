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
    public class ProtectionController : Controller
    {
        //
        // GET: /Protection/
        private const string CONTROLLER_NAME = "Protection";
        private const string DEVICE_DISPLAY_NAME = "Protection";        
        private string GENTECHCD_INVEXT = ConfigurationManager.AppSettings["GENTECHCD_INVEXT"];
        private string GENTECHCD_INVINC = ConfigurationManager.AppSettings["GENTECHCD_INVINC"];
        private string GENTECHCD_SYNCH = ConfigurationManager.AppSettings["GENTECHCD_SYNCH"];
        private string GENTECHCD_INDCT = ConfigurationManager.AppSettings["GENTECHCD_INDCT"];
        private string RelayCd_PPHA = ConfigurationManager.AppSettings["RelayCd_PPHA"];
        private string RelayCd_PGRD = ConfigurationManager.AppSettings["RelayCd_PGRD"];
        private string RelayCd_BPHA = ConfigurationManager.AppSettings["RelayCd_BPHA"];
        private string RelayCd_BGRD = ConfigurationManager.AppSettings["RelayCd_BGRD"];
        private string PROTECTION_Type_RELY = ConfigurationManager.AppSettings["PROTECTION_Type_RELY"];
        private string Prot_ParentType_Gen = ConfigurationManager.AppSettings["PROTECTION_ParentType_Gen"];
        private string PROTECTION_Type_UNSP = ConfigurationManager.AppSettings["PROTECTION_Type_UNSP"];
        private string CurrentOrFuture_C = ConfigurationManager.AppSettings["CurrentOrFuture_C"];  
        private string Certification = ConfigurationManager.AppSettings["Certification"];  
        private string ModeOfInverter = ConfigurationManager.AppSettings["ModeOfInverter"];
        private string GENTECHCD_BATT = ConfigurationManager.AppSettings["GENTECHCD_BATT"]; 
        private string RelayType_1 = ConfigurationManager.AppSettings["RelayType_1"]; 
        private string CurveType_1 = ConfigurationManager.AppSettings["CurveType_1"];
        private string RestraintType_0 = ConfigurationManager.AppSettings["RestraintType_0"];  

        public ActionResult Index(string parentType, string parentID)
        {
            ViewBag.ProtectionParentType = parentType;
            ViewBag.ProtectionParentId = parentID;

            ProtectionModel model = GetProtection(parentID, HtmlExtensions.PageMode.Protection);
            ProtectionViewModel viewModel = new ProtectionViewModel();
            viewModel.ActiveProtection = model;
            ViewBag.ProtectionId = viewModel.ActiveProtection.ID.ToString();
            PopulateRelatedData(viewModel);
            SetSuperUser();
            ViewBag.ControllerName = CONTROLLER_NAME;
            this.initializeDropDowns(model);
            return View(model);
        }


        public ActionResult IndexByID(string parentType, string protectionId)
        {
            ViewBag.ProtectionParentType = parentType;
            ProtectionViewModel viewModel = new ProtectionViewModel();
            ProtectionModel model = GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);
            ViewBag.ProtectionParentId = model.ParentID.ToString();
            viewModel.ActiveProtection = model;
            ViewBag.ProtectionId = viewModel.ActiveProtection.ID.ToString();
            PopulateRelatedData(viewModel);
            SetSuperUser();
            ViewBag.CurrentProtectionId = model.ID.ToString();
            ViewBag.ProtectionID = model.ID.ToString();
            this.initializeDropDowns(model);
            return View("Index", viewModel);
        }

        [HttpPost]
        public ActionResult SaveProtection(ProtectionViewModel model)
        {
            string alertMessage = string.Empty;
            //ProtectionViewModel model = Session["CurrentProtection"] as ProtectionViewModel;
            GeneratorModel genModel = Session["CurrentGenerator"] as GeneratorModel;
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    //Save Generator data first, if user an administrator engineer, udpate only few feilds in generator                   
                    if (Security.IsInAdminGroup || Security.IsInSuperUserGroup)
                    {
                        if (SaveGenerator(model.ActiveGenerator, model, ref alertMessage))
                        {
                            if (SaveRelays(model, ref alertMessage))
                            {
                                model = SaveCurrentProtection(model, ref alertMessage);
                                TempData["unsaved"] = "false";
                            }
                            else
                            {
                                TempData["unsaved"] = "true";
                            }
                        }
                        else
                        {
                            TempData["unsaved"] = "true";
                        }
                    }
                    else
                    {
                        //ViewBag.IsDisabled = true;
                        TempData["msg"] = "<script>alert('Activate Super User to edit data');</script>";
                        return RedirectToAction("IndexByID", "Protection", new { parentType = "Generation", protectionId = model.ActiveProtection.ID.ToString() });
                    }
                }
                else
                {
                    ViewBag.ShowPageError = true;
                    TempData["unsaved"] = "true";
                }
            }
            else
            {
            }

            PopulateRelatedData(model);
            SetSuperUser();
            ViewBag.ProtectionParentType = "Generation";
            ViewBag.ProtectionParentId = model.ActiveProtection.ParentID.ToString();
            ViewBag.CurrentProtectionId = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = model.ActiveProtection.ID.ToString();
            this.initializeDropDowns(model.ActiveProtection);
            this.initializeDropDowns(model.ActiveGenerator);
            this.initializeDropDowns(model.ActiveEquipment);
            if (!string.IsNullOrEmpty(alertMessage))
            {
                TempData["msg"] = alertMessage;
            }
            return View("Index", model);
        }
        [HttpPost]
        private bool SaveRelays(ProtectionViewModel model, ref string alertMessage)
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
                        savedAllRealys = SaveRelay(model.ActivePrimaryPhaseRelay, ref alertMessage);
                    }
                    if (model.ActivePrimaryGroundRelay != null)
                    {
                        savedAllRealys = SaveRelay(model.ActivePrimaryGroundRelay, ref alertMessage);
                    }
                    if (model.ActiveBackupPhaseRelay != null)
                    {
                        savedAllRealys = SaveRelay(model.ActiveBackupPhaseRelay, ref alertMessage);
                    }
                    if (model.ActiveBackupGroundRelay != null)
                    {
                        savedAllRealys = SaveRelay(model.ActiveBackupGroundRelay, ref alertMessage);
                    }
                    if (savedAllRealys)
                    { ViewBag.ShowSaveSucessful = true; }
                    else
                        ViewBag.ShowPageError = true;
                    return savedAllRealys;
                }
                return savedAllRealys;
            }
            catch
            {
                return false;
            }
        }

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
                            SaveRelayHistory(smRelay, new RelayModel(), db);
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

        [HttpPost]
        private bool SaveRelay(RelayModel model, ref string alertMessage)
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
                            TempData["unsaved"] = "true";
                            alertMessage = "<script>alert('Error occured while saving new relays');</script>";
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
                            TempData["unsaved"] = "true";
                            alertMessage = "<script>alert('Error occured while saving Current Relays');</script>";
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        [HttpPost]
        private void SaveRelayHistory(SM_RELAY smRelay, RelayModel model, SettingsEntities db)
        {
            try
            {
                long MaxId = (db.SM_RELAY_HIST.Max(t => (long?)t.ID)) ?? 0;
                SM_RELAY_HIST relayHist = new SM_RELAY_HIST();
                model.PopulateHistoryFromEntity(relayHist, smRelay);
                relayHist.ID = MaxId + 1;
                db.SM_RELAY_HIST.AddObject(relayHist);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        [HttpPost]
        private void SaveGeneratorHistory(SM_GENERATOR smGenerator, GeneratorModel model, SettingsEntities db)
        {
            try
            {
                long MaxId = (db.SM_GENERATOR_HIST.Max(t => (long?)t.ID)) ?? 0;
                SM_GENERATOR_HIST genHist = new SM_GENERATOR_HIST();
                model.PopulateHistoryFromEntity(genHist, smGenerator);
                genHist.ID = MaxId + 1;
                db.SM_GENERATOR_HIST.AddObject(genHist);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        [HttpPost]
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
            catch (Exception ex)
            {

            }
        }
        [HttpPost]
        private void SaveEquipmentHistory(SM_GEN_EQUIPMENT smEquipment, GenEquipmentModel model, SettingsEntities db)
        {
            try
            {
                long MaxId = (db.SM_GEN_EQUIPMENT_HIST.Max(t => (long?)t.ID)) ?? 0;
                SM_GEN_EQUIPMENT_HIST equipHist = new SM_GEN_EQUIPMENT_HIST();
                model.PopulateHistoryFromEntity(equipHist, smEquipment);
                equipHist.ID = MaxId + 1;
                db.SM_GEN_EQUIPMENT_HIST.AddObject(equipHist);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
        }


        [HttpPost]
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

        [HttpPost]
        private ProtectionViewModel SaveCurrentProtection(ProtectionViewModel model, ref string alertMessage)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    SM_PROTECTION smProtection = db.SM_PROTECTION.FirstOrDefault(x => x.ID == model.ActiveProtection.ID);
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
                            ViewBag.ShowSaveSucessful = true;
                            model.ActiveProtection = proModel;
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                            ViewBag.ShowPageError = true;
                            alertMessage = "<script>alert('Error occured while saving new Protection');</script>";
                            return null;
                        }

                    }
                    else
                    {
                        SaveProtectionHistory(smProtection, model.ActiveProtection, db);
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
                            ViewBag.ShowSaveSucessful = true;
                            return model;
                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                            ProtectionModel proModel = GetProtectionbyID(model.ActiveProtection.ID.ToString(), HtmlExtensions.PageMode.Protection);
                            this.initializeDropDowns(proModel);
                            model.ActiveProtection = proModel;
                            alertMessage = "<script>alert('Error occured while saving current Protection');</script>";
                            ViewBag.ShowPageError = true;
                            return model;
                        }
                    }
                }
            }
            return null;
        }

        [HttpPost]
        public ActionResult DeleteEquipment(GenEquipmentModel activeEquipment, GeneratorModel activeGenerator, ProtectionViewModel activeProModel, string protectionId, string gensapEquipmentId, string equipmentSapID, int ActiveProtectionIndex, int ActiveGeneratorIndex, int ActiveEquipmentIndex)
        {
            if (!ModelState.IsValid)
            {
                //For any deleting record, we dont need to validate errors
                foreach (var modelValue in ModelState.Values)
                {
                    modelValue.Errors.Clear();
                }
            }
            if (Security.IsSuperUserActive && Security.IsInSuperUserGroup)
            {
                TempData["GeneratorIndex"] = ActiveGeneratorIndex;
                TempData["EquipmentIndex"] = ActiveEquipmentIndex;
                TempData["ProtectionIndex"] = ActiveProtectionIndex;
                if (!DeleteIndividualEquipment(activeEquipment))
                {
                    TempData["msg"] = "<script>alert('Unable to delete current equipment');</script>";
                }
            }
            else
            {
                TempData["msg"] = "<script>alert('Current user does not have sufficient privileges to delete equipment');</script>";
            }
            //Prepare new model after delete
            //ProtectionModel currentPro = GetProtectionbyID(!string.IsNullOrEmpty(protectionId) ? protectionId : activeProModel.ActiveProtection.ID.ToString(), HtmlExtensions.PageMode.Protection);
            ProtectionViewModel proviewModel = new ProtectionViewModel();
            proviewModel.ActiveProtection = activeProModel.ActiveProtection != null ? activeProModel.ActiveProtection : GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);
            ViewBag.GenerationID = proviewModel.ActiveProtection.ParentID;
            GetProtectionsRelatedParentGen(proviewModel);

            if (proviewModel.Protections != null && proviewModel.Protections.Count > 0)
            {
                proviewModel.ActiveProtectionIndex = proviewModel.Protections.FindIndex(x => x.ID == proviewModel.ActiveProtection.ID);
            }

            GetGenerators(proviewModel);

            if (proviewModel.Generators != null && proviewModel.Generators.Count > 0)
            {
                AssignActiveGeneratorIndex(proviewModel);
                ViewBag.ActiveGeneratorIndex = proviewModel.ActiveGeneratorIndex;
            }
            else
            {
                proviewModel.ActiveGeneratorIndex = -1;
            }
            if (proviewModel.ActiveGeneratorIndex != -1)
            {
                proviewModel.ActiveGenerator = activeProModel.ActiveGenerator != null ? activeProModel.ActiveGenerator : proviewModel.Generators[activeProModel.ActiveGeneratorIndex];
                if (proviewModel.ActiveGenerator != null)
                {
                    this.initializeDropDowns(proviewModel.ActiveGenerator);
                    ViewBag.ProjectName = proviewModel.ActiveGenerator.ProjectName;
                    GetEquipments(proviewModel, proviewModel.ActiveGenerator);
                    if (proviewModel.Equipments != null && proviewModel.Equipments.Count > 0)
                    {
                        AssignActiveEquipmentIndex(proviewModel);
                        int toBeActiveIndex = 0;
                        if (proviewModel.ActiveEquipmentIndex == proviewModel.Equipments.Count)
                        {
                            toBeActiveIndex = proviewModel.ActiveEquipmentIndex - 1;
                        }
                        proviewModel.ActiveEquipment = proviewModel.Equipments[toBeActiveIndex];
                        this.initializeDropDowns(proviewModel.ActiveEquipment);
                        proviewModel.ActiveEquipmentIndex = toBeActiveIndex;
                        ViewBag.ActiveEquipmentIndex = toBeActiveIndex;
                    }
                    else
                        proviewModel.ActiveEquipmentIndex = -1;
                }
            }


            GetRelays(proviewModel);

            if (proviewModel.Relays != null && proviewModel.Relays.Count > 0)
            {
                proviewModel.ActivePrimaryPhaseRelay = proviewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                proviewModel.ActivePrimaryGroundRelay = proviewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                proviewModel.ActiveBackupPhaseRelay = proviewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                proviewModel.ActiveBackupGroundRelay = proviewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }


            Session["CurrentProtection"] = proviewModel;
            setUiSettings(HtmlExtensions.PageMode.Protection);
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.ProtectionParentId = proviewModel.ActiveProtection.ParentID.ToString();
            SetSuperUser();
            this.initializeDropDowns(proviewModel.ActiveProtection);
            ModelState.Clear();
            return View("Index", proviewModel);
        }

        private bool DeleteIndividualEquipment(GenEquipmentModel activeEquipment)
        {
            bool newlyCreated = false;
            if (TempData["NewEquipment"] != null)
            {
                newlyCreated = true;
            }
            if (newlyCreated)
            {
                return true;
            }
            else
            {
                //we dont need to delete the newly created equipment database, as it is not yet saved.
                using (SettingsEntities db = new SettingsEntities())
                {
                    db.Connection.Open();
                    using (var Transaction = db.Connection.BeginTransaction())
                    {
                        SM_GEN_EQUIPMENT existingEquip = db.SM_GEN_EQUIPMENT.FirstOrDefault(x => x.SAP_EQUIPMENT_ID == activeEquipment.SAPEquipmentID);
                        if (existingEquip != null)
                        {
                            db.DeleteObject(existingEquip);
                            try
                            {
                                db.SaveChanges();
                                Transaction.Commit();
                                return true;
                            }
                            catch
                            {
                                Transaction.Rollback();
                                return false;
                            }
                        }
                        else
                            return false;
                    }
                }
            }

        }

        private bool DeleteIndividualGenerator(GeneratorModel activeGenerator, ProtectionViewModel proViewModel)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    SM_GENERATOR existingGen;
                    if (activeGenerator.SapEquipmentID != null)
                    {
                        existingGen = db.SM_GENERATOR.FirstOrDefault(x => x.SAP_EQUIPMENT_ID == activeGenerator.SapEquipmentID);
                    }
                    else
                    {
                        existingGen = db.SM_GENERATOR.FirstOrDefault(x => x.ID == activeGenerator.ID);
                    }
                    if (existingGen != null)
                    {
                        db.DeleteObject(existingGen);
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            Transaction.Rollback();
                            return false;
                        }
                    }
                    else
                        return false;
                }
            }
        }

        [HttpGet]
        public PartialViewResult RelocateGenerators(string generationID, int activeProtectionIndex, string currentProtectionId)
        {
            MoveGeneratorModel model;
            long genID = Convert.ToInt32(generationID);
            using (SettingsEntities db = new SettingsEntities())
            {
                IEnumerable<SM_PROTECTION> protections = (from b in db.SM_PROTECTION.Where(x => x.PARENT_ID == genID && x.PARENT_TYPE == Prot_ParentType_Gen) orderby b.ID select b).ToList();

                model = new MoveGeneratorModel
                {
                    GeneratorID = Convert.ToInt32(generationID),
                    CurrentProtectionIndex = activeProtectionIndex,
                    CurrentProtectionId = currentProtectionId,
                    GeneratorListBoxItems1 = new List<SelectListItem>(),
                    GeneratorListBoxItems2 = new List<SelectListItem>(),
                    ProtectionNamesList = protections.Select((x, i) => new SelectListItem
                    {
                        Value = x.ID.ToString(),
                        Text = "Protection " + (i + 1).ToString()
                    })
                };
            }

            return PartialView("~/views/Protection/_MoveGenerators.cshtml", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MoveGenerators(MoveGeneratorModel activeModel, string selectedProtection1, string generatorList1Updated, string selectedProtection2, string generatorList2Updated)
        {
            bool allSaved = true;
            if (!string.IsNullOrEmpty(activeModel.Dropdown1SelectedProtection))
            {
                if (!string.IsNullOrEmpty(generatorList1Updated))
                {
                    activeModel.SelectedValuesInGeneratorLb1 = generatorList1Updated.Split(',');
                }
                if (activeModel.SelectedValuesInGeneratorLb1 != null && activeModel.SelectedValuesInGeneratorLb1.Count() > 0)
                {
                    foreach (string generatorID in activeModel.SelectedValuesInGeneratorLb1)
                    {
                        if (allSaved)
                            UpdateParentIdOfGenerator(generatorID, activeModel.Dropdown1SelectedProtection);
                        else
                            break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(activeModel.Dropdown2SelectedProtection))
            {
                if (!string.IsNullOrEmpty(generatorList2Updated))
                {
                    activeModel.SelectedValuesInGeneratorLb2 = generatorList2Updated.Split(',');
                }
                if (activeModel.SelectedValuesInGeneratorLb2 != null && activeModel.SelectedValuesInGeneratorLb2.Count() > 0)
                {
                    foreach (string generatorID in activeModel.SelectedValuesInGeneratorLb2)
                    {
                        if (allSaved)
                            UpdateParentIdOfGenerator(generatorID, activeModel.Dropdown2SelectedProtection);
                        else
                            break;
                    }
                }
            }
            if (allSaved)
                return Json(new { message = "Relocated Successfully" }); // success message
            else
                return Json(new { message = "Error in Moving..." }); // fail message

        }

        [HttpPost]
        private bool UpdateParentIdOfGenerator(string generatorID, string protectId)
        {
            long protectionID = Convert.ToInt32(protectId);
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    long genIdConv = Convert.ToInt32(generatorID);
                    SM_GENERATOR smGen = db.SM_GENERATOR.FirstOrDefault(x => x.ID == genIdConv);
                    if (smGen != null && smGen.PROTECTION_ID != protectionID)
                    {
                        smGen.PROTECTION_ID = protectionID;
                        smGen.DATE_MODIFIED = DateTime.Now;
                        smGen.MODIFIEDBY = Security.CurrentUser;
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            Transaction.Rollback();
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteGenerator(ProtectionViewModel activeProModel, string protectionId, string gensapEquipmentId, int ActiveProtectionIndex, int ActiveGeneratorIndex, int ActiveEquipmentIndex)
        {
            ProtectionViewModel newactiveProModel = new ProtectionViewModel();
            bool newlyCreated = false;
            bool deletedAll = false;
            TempData["GeneratorIndex"] = ActiveGeneratorIndex;
            TempData["EquipmentIndex"] = ActiveEquipmentIndex;
            TempData["ProtectionIndex"] = ActiveProtectionIndex;
            if (TempData["NewGenerator"] != null)
            {
                newlyCreated = true;
            }
            if (newlyCreated)
            {
                //If it is newly created Generator, dont worry about errors, delete it.
                if (!ModelState.IsValid)
                {
                    //For any deleting record, we dont need to validate errors
                    foreach (var modelValue in ModelState.Values)
                    {
                        modelValue.Errors.Clear();
                    }
                }
                deletedAll = true;
                ViewData["DeleteNew"] = true;
            }
            else
            {
                if (Security.IsSuperUserActive && Security.IsInSuperUserGroup)
                {
                    GetEquipments(activeProModel, activeProModel.ActiveGenerator);
                    if (activeProModel.Equipments != null && activeProModel.Equipments.Count > 0)
                    {
                        int equpmentCnt = activeProModel.Equipments.Count;
                        int deletedCnt = 0;
                        foreach (GenEquipmentModel activeEquipment in activeProModel.Equipments)
                        {
                            if (DeleteIndividualEquipment(activeEquipment))
                            {
                                deletedCnt++;
                            }
                        }
                        if (equpmentCnt == deletedCnt)
                        {
                            //Successfuly deleted child Equipments, now delete individual generator
                            deletedAll = DeleteIndividualGenerator(activeProModel.ActiveGenerator, activeProModel);
                        }
                        else
                        {
                            deletedAll = false;
                            TempData["msg"] = "<script>alert('Unable to delete all equipments related to the current generation, please delete them manually');</script>";
                        }
                    }
                    else
                    {
                        if (activeProModel.ActiveGenerator != null)
                        {
                            deletedAll = DeleteIndividualGenerator(activeProModel.ActiveGenerator, activeProModel);
                        }
                    }
                }
                else
                {
                    TempData["msg"] = "<script>alert('Current user does not have sufficient privileges to delete equipment');</script>";
                }
            }


            //return RedirectToAction("IndexByID", "Protection", new { parentType = "Generation", protectionId = activeProModel.ActiveProtection.ID.ToString() });
            ProtectionViewModel newProModel = new ProtectionViewModel();
            newProModel.ActiveProtection = activeProModel.ActiveProtection != null ? activeProModel.ActiveProtection : GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);

            ViewBag.GenerationID = newProModel.ActiveProtection.ParentID;
            GetProtectionsRelatedParentGen(newProModel);

            if (newProModel.Protections != null && newProModel.Protections.Count > 0)
            {
                newProModel.ActiveProtectionIndex = newProModel.Protections.FindIndex(x => x.ID == newProModel.ActiveProtection.ID);
            }

            GetGenerators(newProModel);

            if (newProModel.Generators != null && newProModel.Generators.Count > 0)
            {
                AssignActiveGeneratorIndex(newProModel);
                ViewBag.ActiveGeneratorIndex = newProModel.ActiveGeneratorIndex;
                UpdateBackupGenerationFieldInGeneration(activeProModel.ActiveGenerator, newProModel);   
            }
            else
            {
                newProModel.ActiveGeneratorIndex = -1;
            }
            if (newProModel.ActiveGeneratorIndex != -1)
            {
                newProModel.ActiveGenerator = newProModel.Generators[newProModel.ActiveGeneratorIndex];
                if (newProModel.ActiveGenerator != null)
                {
                    this.initializeDropDowns(newProModel.ActiveGenerator);
                    ViewBag.ProjectName = newProModel.ActiveGenerator.ProjectName;
                    if (!string.IsNullOrEmpty(newProModel.ActiveGenerator.SapEquipmentID))
                    {
                        GetEquipments(newProModel, newProModel.ActiveGenerator);
                        if (newProModel.Equipments != null && newProModel.Equipments.Count > 0)
                        {
                            AssignActiveEquipmentIndex(newProModel);
                            int toBeActiveIndex = 0;
                            if (newProModel.ActiveEquipmentIndex == newProModel.Equipments.Count)
                            {
                                toBeActiveIndex = newProModel.ActiveEquipmentIndex - 1;
                            }
                            newProModel.ActiveEquipment = newProModel.Equipments[toBeActiveIndex];
                            this.initializeDropDowns(newProModel.ActiveEquipment);
                            newProModel.ActiveEquipmentIndex = toBeActiveIndex;
                            ViewBag.ActiveEquipmentIndex = toBeActiveIndex;
                        }
                        else
                            newProModel.ActiveEquipmentIndex = -1;
                    }
                }
            }


            GetRelays(newProModel);

            if (newProModel.Relays != null && newProModel.Relays.Count > 0)
            {
                newProModel.ActivePrimaryPhaseRelay = newProModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                newProModel.ActivePrimaryGroundRelay = newProModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                newProModel.ActiveBackupPhaseRelay = newProModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                newProModel.ActiveBackupGroundRelay = newProModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }


            Session["CurrentProtection"] = newProModel;
            setUiSettings(HtmlExtensions.PageMode.Protection);
            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();
            ViewBag.ProtectionParentId = newProModel.ActiveProtection.ParentID.ToString();
            this.initializeDropDowns(newProModel.ActiveProtection);
            //ModelState.Remove("ActiveGenerator.SapEquipmentID");
            ModelState.Clear();
            //foreach (var modelValue in ModelState.Keys)
            //{
            //    ModelState.Remove(modelValue);
            //}
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("IndexByID", new { parentType = "Generation", protectionId = newProModel.ActiveProtection.ID.ToString() });
            return Json(new { Url = redirectUrl });
            //return View("Index",newProModel);
        }


        [HttpPost]
        private bool SaveGenerator(GeneratorModel genModel, ProtectionViewModel model, ref string alertMessage)
        {
            bool newlyCreated = false;

            if (genModel != null)
            {
                using (SettingsEntities db = new SettingsEntities())
                {
                    db.Connection.Open();
                    if (TempData["NewGenerator"] != null)
                    {
                        newlyCreated = true;
                    }
                    else
                    {
                        bool isthisExist = db.SM_GENERATOR.Any(x => x.ID == genModel.ID);
                        newlyCreated = isthisExist ? false : true;
                    }
                    using (var Transaction = db.Connection.BeginTransaction())
                    {
                        //insert code to check the newly inserted values
                        if (newlyCreated)
                        {
                            //Check the whether SAP EQUIPMENT ID is Unique or not 
                            if (!IsSapEquipmentIdAvailble(genModel.SapEquipmentID))
                            {
                                //Check whether SAP Queu Number is unique or not
                                //if (!IsQueueAvailble(genModel.SapQueueNumber))
                                //{
                                SM_GENERATOR smGenerator = new SM_GENERATOR();
                                genModel.PopulateEntityFromModel(smGenerator);
                                smGenerator.DATECREATED = DateTime.Now;
                                smGenerator.CREATEDBY = Security.CurrentUser;
                                db.SM_GENERATOR.AddObject(smGenerator);
                                try
                                {
                                    db.SaveChanges();
                                    Transaction.Commit();
                                    // ENOSTOSAP Phase 2 Req-4
                                    UpdateBackupGenerationFieldInGeneration(genModel, model);
                                    //end
                                    ViewBag.ShowGeneratorSaveSucessful = true;
                                    if (model.ActiveEquipment != null)
                                    {
                                        return Security.IsSuperUserActive && SaveEquipment(model.ActiveEquipment, ref alertMessage);
                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Transaction.Rollback();
                                    db.Connection.Close();
                                    ViewBag.ShowGeneratorPageError = true;
                                    TempData["unsaved"] = "true";
                                    alertMessage = "<script>alert('Exception occured while saving generator data');</script>";
                                    return false;
                                }
                                //}
                                //else
                                //{
                                //    //Sap Equipment ID Already exist
                                //    ViewBag.ShowGeneratorPageError = true;
                                //    ModelState.AddModelError("ActiveGenerator.SapQueueNumber", "Generator SAP Queue Number should be Unique");
                                //    TempData["unsaved"] = "true";
                                //    alertMessage = "<script>alert('Entered SAP Queue Number '" + genModel.SapQueueNumber + "' Already Exist, it shoud be unique ');</script>";
                                //    return false;
                                //}
                            }
                            else
                            {
                                //Sap Equipment ID Already exist
                                ViewBag.ShowGeneratorPageError = true;
                                ModelState.AddModelError("ActiveGenerator.SapEquipmentID", "Generator SAP Equipment ID should be Unique");
                                TempData["unsaved"] = "true";
                                alertMessage = "<script>alert('Entered SAP Equipment ID '" + genModel.SapEquipmentID + "' Already Exist, it shoud be unique ');</script>";
                                return false;
                            }
                        }
                        else
                        {
                            //If this is an existing generator, then update Generator depending up on the user role
                            // SM_GENERATOR smGenerator = db.SM_GENERATOR.FirstOrDefault(x => x.SAP_EQUIPMENT_ID == genModel.SapEquipmentID);
                            // ** offshore team change logic -SAP_EQUIPMENT_ID to ID
                            SM_GENERATOR smGenerator = db.SM_GENERATOR.FirstOrDefault(x => x.ID == genModel.ID);

                            // ENOSTOSAP Phase 2 Req-4
                            //GetGenerators(model);
                            //List<GeneratorModel> genModelList = model.Generators; //end
                            if (smGenerator != null)
                            {
                                bool removeEquipment = false;
                                SaveGeneratorHistory(smGenerator, genModel, db);
                                if (Security.IsSuperUserActive)
                                {
                                    if (smGenerator.GEN_TECH_CD != genModel.GenTechCd)
                                        UpdateFieldsPerGenType(genModel, smGenerator, ref removeEquipment);
                                    genModel.PopulateEntityFromModel(smGenerator);

                                }
                                else
                                {
                                    //if the user is an engineer, update only the below.
                                    //Add code by offshore-9th May17
                                    smGenerator.CONNECTION_CD = genModel.ConnectionCD;
                                    smGenerator.CONTROL_CD = genModel.ControlCD;
                                    smGenerator.STATUS_CD = genModel.StatusCD;
                                    smGenerator.NOTES = genModel.Notes;
                                    smGenerator.SS_REACTANCE = genModel.SSReactance;
                                    smGenerator.SS_RESISTANCE = genModel.SSResistance;
                                    smGenerator.SUBTRANS_REACTANCE = genModel.SubTransReactance;
                                    smGenerator.SUBTRANS_RESISTANCE = genModel.SubTransResistance;
                                    smGenerator.TRANS_REACTANCE = genModel.TransReactance;
                                    smGenerator.TRANS_RESISTNACE = genModel.TransResistance;
                                    smGenerator.ZERO_REACTANCE = genModel.ZeroReactance;
                                    smGenerator.ZERO_RESISTANCE = genModel.ZeroResistance;
                                    smGenerator.NEG_REACTANCE = genModel.NegativeReactance;
                                    smGenerator.NEG_RESISTANCE = genModel.NegativeResistance;
                                    smGenerator.GRD_REACTANCE = genModel.GroundReactance;
                                    smGenerator.GRD_RESISTANCE = genModel.GroundResistance;
                                    smGenerator.INVERTER_EFFICIENCY = genModel.InverterEfficiency;

                                }
                                smGenerator.DATE_MODIFIED = DateTime.Now;
                                smGenerator.MODIFIEDBY = Security.CurrentUser;
                                try
                                {
                                    db.SaveChanges();
                                    Transaction.Commit();
                                    // ENOSTOSAP Phase 2 Req-4
                                    UpdateBackupGenerationFieldInGeneration(genModel, model); //end
                                    ViewBag.ShowGeneratorSaveSucessful = true;

                                    if (model.ActiveEquipment != null)
                                    {
                                        return SaveEquipment(model.ActiveEquipment, ref  alertMessage);
                                    }
                                    else
                                    {
                                        if (removeEquipment)
                                        {
                                            return DeleteEquipment(model);
                                        }
                                        return true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Transaction.Rollback();
                                    db.Connection.Close();
                                    ViewBag.ShowGeneratorPageError = true;
                                    alertMessage = "<script>alert('Exception occured while saving generator data');</script>";
                                    TempData["unsaved"] = "true";
                                    return false;
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
            else
            {
                //this means its a new protection without any new generation
                return true;
            }
            return false;
        }

        private void SaveHistory<T>(T genObject)
        {
            try
            {
                using (SettingsEntities db = new SettingsEntities())
                {
                    db.Connection.Open();
                    if (typeof(T) is GenerationModel)
                    {
                        GenerationModel generationModel = genObject as GenerationModel;
                        SM_GENERATION_HIST genHist = new SM_GENERATION_HIST();
                        SM_GENERATION existingGen = db.SM_GENERATION.FirstOrDefault(s => s.GLOBAL_ID == generationModel.GlobalID && s.CURRENT_FUTURE == CurrentOrFuture_C);
                        if (existingGen != null)
                        {
                            generationModel.PopulateHistoryFromEntity(genHist, existingGen);
                            db.SM_GENERATION_HIST.AddObject(genHist);
                        }
                    }
                    else if (typeof(T) is ProtectionModel)
                    {
                        ProtectionModel proModel = genObject as ProtectionModel;
                        SM_PROTECTION_HIST proHist = new SM_PROTECTION_HIST();
                        SM_PROTECTION existingPro = db.SM_PROTECTION.FirstOrDefault(s => s.ID == proModel.ID && s.CURRENT_FUTURE == CurrentOrFuture_C);
                        if (existingPro != null)
                        {
                            proModel.PopulateHistoryFromEntity(proHist, existingPro);
                            db.SM_PROTECTION_HIST.AddObject(proHist);
                        }
                    }
                    else if (typeof(T) is RelayModel)
                    {
                        RelayModel relayModel = genObject as RelayModel;
                        SM_RELAY_HIST relHist = new SM_RELAY_HIST();
                        SM_RELAY existingRel = db.SM_RELAY.FirstOrDefault(s => s.ID == relayModel.ID && s.CURRENT_FUTURE == CurrentOrFuture_C);
                        if (existingRel != null)
                        {
                            relayModel.PopulateHistoryFromEntity(relHist, existingRel);
                            db.SM_RELAY_HIST.AddObject(relHist);
                        }
                    }
                    else if (typeof(T) is GeneratorModel)
                    {
                        GeneratorModel generatorModel = genObject as GeneratorModel;
                        SM_GENERATOR_HIST geneHist = new SM_GENERATOR_HIST();
                        SM_GENERATOR existingGene = db.SM_GENERATOR.FirstOrDefault(s => s.ID == generatorModel.ID && s.CURRENT_FUTURE == CurrentOrFuture_C);
                        if (existingGene != null)
                        {
                            generatorModel.PopulateHistoryFromEntity(geneHist, existingGene);
                            db.SM_GENERATOR_HIST.AddObject(geneHist);
                        }
                    }
                    else if (typeof(T) is GenEquipmentModel)
                    {
                        GenEquipmentModel genEquipmentModel = genObject as GenEquipmentModel;
                        SM_GEN_EQUIPMENT_HIST equipHist = new SM_GEN_EQUIPMENT_HIST();
                        SM_GEN_EQUIPMENT existingEquip = db.SM_GEN_EQUIPMENT.FirstOrDefault(s => s.ID == genEquipmentModel.ID && s.CURRENT_FUTURE == CurrentOrFuture_C);
                        if (existingEquip != null)
                        {
                            genEquipmentModel.PopulateHistoryFromEntity(equipHist, existingEquip);
                            db.SM_GEN_EQUIPMENT_HIST.AddObject(equipHist);
                        }
                    }
                    using (var Transaction = db.Connection.BeginTransaction())
                    {
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();
                        }
                        catch
                        {
                            //
                        }
                    }
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// Used when a super user modifies the Gen type of an existing generator with Inverter type to Synchronus or Induction type.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool DeleteEquipment(ProtectionViewModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                SM_GENERATOR smGen = null;
                if (!string.IsNullOrEmpty(model.ActiveGenerator.SapEquipmentID.Trim()))
                {
                    smGen = db.SM_GENERATOR.FirstOrDefault(x => x.SAP_EQUIPMENT_ID == model.ActiveGenerator.SapEquipmentID);
                }
                else if (smGen == null && model.ActiveGenerator.ID != null)
                {
                    smGen = db.SM_GENERATOR.FirstOrDefault(x => x.ID == model.ActiveGenerator.ID);
                }
                if (smGen != null)
                {
                    using (var Transaction = db.Connection.BeginTransaction())
                    {
                        IQueryable<SM_GEN_EQUIPMENT> smGenEquips = db.SM_GEN_EQUIPMENT.Where(x => x.GENERATOR_ID == smGen.ID);
                        if (smGenEquips != null && smGenEquips.Count() > 0)
                        {
                            foreach (SM_GEN_EQUIPMENT genEquip in smGenEquips)
                            {
                                SaveEquipmentHistory(genEquip, new GenEquipmentModel(), db);
                                db.DeleteObject(genEquip);
                            }
                        }
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();

                            return true;
                        }
                        catch
                        {
                            Transaction.Rollback();
                            return false;
                        }
                    }
                }
                else
                    return true;
            }

        }

        private void UpdateFieldsPerGenType(GeneratorModel genModel, SM_GENERATOR smGenerator, ref bool removeEquipment)
        {
            if ((smGenerator.GEN_TECH_CD == GENTECHCD_INVEXT || smGenerator.GEN_TECH_CD == GENTECHCD_INVINC) && (genModel.GenTechCd == GENTECHCD_SYNCH || genModel.GenTechCd == GENTECHCD_INDCT))
            {
                genModel.Certification = Certification;
                genModel.ModeOfInverter = ModeOfInverter;
                genModel.InverterEfficiency = 0;
                removeEquipment = true;
            }
            else if ((smGenerator.GEN_TECH_CD == GENTECHCD_SYNCH || smGenerator.GEN_TECH_CD == GENTECHCD_INDCT) && (genModel.GenTechCd == GENTECHCD_INVEXT || genModel.GenTechCd == GENTECHCD_INVINC))
            {
                genModel.SSReactance = 0;
                genModel.SSResistance = 0;
                genModel.SubTransReactance = 0;
                genModel.SubTransResistance = 0;
                genModel.TransReactance = 0;
                genModel.TransResistance = 0;
                genModel.NegativeReactance = 0;
                genModel.NegativeResistance = 0;
                genModel.ZeroReactance = 0;
                genModel.ZeroResistance = 0;
                genModel.GroundReactance = 0;
                genModel.GroundResistance = 0;
                removeEquipment = false;
            }
        }

        private bool SaveEquipment(GenEquipmentModel genEquipmentModel, ref string alertMessage)
        {
            bool newlyCreated = false;

            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                if (TempData["NewEquipment"] != null)
                {
                    newlyCreated = true;
                }
                else
                {
                    bool isThisExist = db.SM_GEN_EQUIPMENT.Any(x => x.ID == genEquipmentModel.ID);
                    newlyCreated = isThisExist ? false : true;
                }
                if (newlyCreated)
                {
                    if (!IsEquipmentIDAvailble(genEquipmentModel.SAPEquipmentID))
                    {
                        using (var Transaction = db.Connection.BeginTransaction())
                        {
                            //If it is newly created Equipment 
                            SM_GEN_EQUIPMENT smEquipment = new SM_GEN_EQUIPMENT();
                            genEquipmentModel.PopulateEntityFromModel(smEquipment);
                            smEquipment.CREATEDBY = Security.CurrentUser;
                            smEquipment.DATECREATED = DateTime.Now;
                            db.SM_GEN_EQUIPMENT.AddObject(smEquipment);
                            try
                            {
                                db.SaveChanges();
                                Transaction.Commit();
                                //SaveHistory<GenEquipmentModel>(genEquipmentModel);
                                ViewBag.ShowEquipmentSaveSucessful = true;
                                return true;
                            }
                            catch (Exception ex)
                            {
                                Transaction.Rollback();
                                db.Connection.Close();
                                ViewBag.ShowEquipmentPageError = true;
                                alertMessage = "<script>alert('Exception occured while saving Equipment data');</script>";
                                TempData["unsaved"] = "true";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //Equipment Sap Equipment ID Already exist
                        ViewBag.ShowGeneratorPageError = true;
                        ModelState.AddModelError("ActiveEquipment.SAPEquipmentID", "Equipment SAP Equipment ID should be Unique, Entered value already exist in Database");
                        TempData["unsaved"] = "true";
                        alertMessage = "<script>alert('Entered SAP Equipment ID '" + genEquipmentModel.SAPEquipmentID + "' For Equipment Already Exist, it shoud be unique ');</script>";
                        return false;
                    }
                }
                else
                {
                    //If user updating existing data , update equipment fields on the basis of user role
                    using (var Transaction = db.Connection.BeginTransaction())
                    {
                        SM_GEN_EQUIPMENT smEquipment = db.SM_GEN_EQUIPMENT.FirstOrDefault(x => x.ID == genEquipmentModel.ID);
                        if (smEquipment != null)
                        {
                            bool canWeModifyEquipment = true;
                            //Save existing data to history table
                            SaveEquipmentHistory(smEquipment, genEquipmentModel, db);
                            if (Security.IsSuperUserActive)
                            {
                                if (smEquipment.GEN_TECH_CD != genEquipmentModel.GenTechCD)
                                {
                                    UpdateFieldsPerGenEquipType(genEquipmentModel, smEquipment);
                                }
                                genEquipmentModel.PopulateEntityFromModel(smEquipment);
                                canWeModifyEquipment = true;
                            }
                            else
                            {
                                //As of now, only NOTES field is editable by non super users & admins.
                                if (smEquipment.NOTES != genEquipmentModel.Notes)
                                {
                                    smEquipment.NOTES = genEquipmentModel.Notes;
                                    canWeModifyEquipment = true;
                                }
                                else
                                    canWeModifyEquipment = false;
                            }
                            if (canWeModifyEquipment)
                            {
                                smEquipment.MODIFIEDBY = Security.CurrentUser;
                                smEquipment.DATE_MODIFIED = DateTime.Now;
                                try
                                {
                                    db.SaveChanges();
                                    Transaction.Commit();
                                    ViewBag.ShowEquipmentSaveSucessful = true;
                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    Transaction.Rollback();
                                    db.Connection.Close();
                                    ViewBag.ShowEquipmentPageError = true;
                                    alertMessage = "<script>alert('Exception occured while saving Equipment data');</script>";
                                    TempData["unsaved"] = "true";
                                    return false;
                                }
                            }
                            else
                            {
                                //Nothing to udpate
                                return true;
                            }

                        }
                    }
                }
                return false;
            }
        }

        private void UpdateFieldsPerGenEquipType(GenEquipmentModel genEquipmentModel, SM_GEN_EQUIPMENT smEquipment)
        {
            if (smEquipment.GEN_TECH_CD == GENTECHCD_BATT && genEquipmentModel.GenTechCD != GENTECHCD_BATT)
            {
                genEquipmentModel.PTCRatedkW = 0;
                genEquipmentModel.MaxStorageCapacity = 0;
                genEquipmentModel.NameplateRating = 0;
                genEquipmentModel.ChargeDemandkW = 0;
                genEquipmentModel.GridCharged = string.Empty;

            }
        }


        public ActionResult AddNewProtection(string parentType, string generationID)
        {
            ViewBag.ParentGenerationID = generationID;
            ProtectionModel model = CreateItem(parentType, Convert.ToInt32(generationID));
            this.initializeDropDowns(model);
            ProtectionViewModel viewModel = new ProtectionViewModel();
            viewModel.ActiveProtection = model;
            ViewBag.ProtectionId = viewModel.ActiveProtection.ID.ToString();
            PopulateRelatedData(viewModel);
            ViewBag.ProtectionParentId = viewModel.ActiveProtection.ParentID.ToString();
            SetSuperUser();
            return View("Index", viewModel);
        }

        public ActionResult DeleteProtection(string protectionId, int ActiveProtectionIndex)
        {
            ViewBag.PreviousProtectionDeleted = protectionId;
            ProtectionModel proModel = GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);
            //this.initializeDropDowns(proModel);
            ProtectionViewModel proViewModel = new ProtectionViewModel();
            proViewModel.ActiveProtection = proModel;
            PopulateRelatedData(proViewModel);

            int currentActiveIndex = 0;
            int toBeActiveIndex = 0;
            if (proViewModel != null)
            {
                protectionId = proViewModel.ActiveProtection.ID.ToString();
                currentActiveIndex = proViewModel.ActiveProtectionIndex;
                if (currentActiveIndex == proViewModel.Protections.Count - 1)
                {
                    toBeActiveIndex = currentActiveIndex - 1;
                }
            }
            ProtectionModel nextAvailableProtection = null;
            //if (Session["PreviousProtectionType"] != null && Session["PreviousProtectionType"].ToString() == "RELY")
            //{
            //    //The current protectio have some relays, user changed it to something else, so delete the existing relays
            //   // DeleteRelays(proViewModel);
            //}

            //code add
            DeleteRelays(proViewModel);
            if (DeleteItem(protectionId))
            {
                proViewModel.Protections.RemoveAt(currentActiveIndex);
                nextAvailableProtection = proViewModel.Protections[toBeActiveIndex];
                this.initializeDropDowns(nextAvailableProtection);
                proViewModel = new ProtectionViewModel();
                proViewModel.ActiveProtection = nextAvailableProtection;
                PopulateRelatedData(proViewModel);
                proViewModel.ActiveProtectionIndex = toBeActiveIndex;
                this.initializeDropDowns(proViewModel.ActiveProtection);
            }

            SetSuperUser();
            ViewBag.ProtectionParentId = proViewModel.ActiveProtection.ParentID.ToString();
            return View("Index", proViewModel);
        }

        [HttpPost]
        private bool DeleteItem(string protectionId)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    long proId = long.Parse(protectionId);
                    SM_PROTECTION SMPro = db.SM_PROTECTION.FirstOrDefault(x => x.ID == proId);
                    if (SMPro != null)
                    {
                        db.SM_PROTECTION.DeleteObject(SMPro);
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
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            return false;
        }

        [HttpPost]
        private ProtectionModel CreateItem(string parentType, int generationID)
        {
            ProtectionModel model = null;
            if (model == null)
            {
                model = new ProtectionModel();
                model.ParentID = generationID;
                model.ParentType = parentType;
                model.ProtectionType = PROTECTION_Type_UNSP;
                model.CurrentOrFuture = CurrentOrFuture_C;
                model.ParentType = Prot_ParentType_Gen;

                using (SettingsEntities db = new SettingsEntities())
                {
                    db.Connection.Open();
                    using (var Transaction = db.Connection.BeginTransaction())
                    {

                        SM_PROTECTION SMPro = new SM_PROTECTION();

                        long MaxProtectionId = (db.SM_PROTECTION.Max(t => (long?)t.ID)) ?? 0;
                        model.ID = MaxProtectionId + 1;
                        model.PopulateEntityFromModel(SMPro);
                        SMPro.DATECREATED = DateTime.Now;
                        SMPro.CREATEDBY = Security.CurrentUser;

                        //db.SM_PROTECTION.ApplyCurrentValues(e);

                        db.SM_PROTECTION.AddObject(SMPro);
                        try
                        {
                            db.SaveChanges();
                            Transaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            Transaction.Rollback();
                            db.Connection.Close();
                        }
                        this.initializeDropDowns(model);
                    }
                }
            }
            return model;
        }


        public ActionResult AddNewGenerator(ProtectionViewModel proViewModel, string parentType, string protectionId, int? protectionIndex)
        {
            //ProtectionViewModel model = Session["CurrentProtection"] as ProtectionViewModel;
            ViewBag.ProtectionParentType = "Generation";
            ProtectionModel model1 = GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);
            proViewModel.ActiveProtection = model1;
            ViewBag.ProtectionID = proViewModel.ActiveProtection.ID;
            ViewBag.GenerationID = proViewModel.ActiveProtection.ParentID;

            GetProtectionsRelatedParentGen(proViewModel);
            if (proViewModel.Protections != null && proViewModel.Protections.Count > 0)
            {
                proViewModel.ActiveProtectionIndex = proViewModel.Protections.FindIndex(x => x.ID == model1.ID); ;
            }

            GetGenerators(proViewModel);
            GeneratorModel newGenerator = CreateNewGenerator(proViewModel);
            if (proViewModel.Generators != null && proViewModel.Generators.Count > 0)
            {
                proViewModel.ActiveGeneratorIndex = proViewModel.Generators.FindIndex(x => x.ID == newGenerator.ID);
            }
            else
            {
                proViewModel.Generators = new List<GeneratorModel>();
                proViewModel.Generators.Add(newGenerator);
                proViewModel.ActiveGeneratorIndex = 0;
            }

            GetRelays(proViewModel);
            if (proViewModel.Relays != null && proViewModel.Relays.Count > 0)
            {
                proViewModel.ActivePrimaryPhaseRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                proViewModel.ActivePrimaryGroundRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                proViewModel.ActiveBackupPhaseRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                proViewModel.ActiveBackupGroundRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }
            //TempData["GeneratorIndex"] = ActiveGeneratorIndex;
            //TempData["EquipmentIndex"] = ActiveEquipmentIndex;
            //TempData["ProtectionIndex"] = ActiveProtectionIndex;
            //PopulateRelatedData(model);
            setUiSettings(HtmlExtensions.PageMode.Protection);
            SetSuperUser();
            this.initializeDropDowns(proViewModel.ActiveProtection);
            ViewBag.ProtectionParentId = proViewModel.ActiveProtection.ParentID.ToString();
            TempData["unsaved"] = "true";
            ModelState.Clear();
            return View("Index", proViewModel);
        }

        public ActionResult AddNewEquipment(ProtectionViewModel proViewModel, GeneratorModel genModel, string genId, string protectionId, int? protectionIndex, int? generatorIndex)
        {
            ViewBag.ProtectionParentType = "Generation";
            //ProtectionViewModel viewModel = new ProtectionViewModel();
            ProtectionModel model1 = GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);
            proViewModel.ActiveProtection = model1;
            ViewBag.ProtectionID = proViewModel.ActiveProtection.ID;
            ViewBag.GenerationID = proViewModel.ActiveProtection.ParentID;
            GetProtectionsRelatedParentGen(proViewModel);

            if (proViewModel.Protections != null && proViewModel.Protections.Count > 0)
            {
                proViewModel.ActiveProtectionIndex = proViewModel.Protections.FindIndex(x => x.ID == model1.ID); ;
            }

            GetGenerators(proViewModel);
            if (proViewModel.Generators != null && proViewModel.Generators.Count > 0)
            {
                long genIdConv = Convert.ToInt32(genId);
                proViewModel.ActiveGeneratorIndex = proViewModel.Generators.FindIndex(x => x.ID == genIdConv);

            }
            else
            {
                proViewModel.ActiveGeneratorIndex = 0;
            }
            try
            {
                proViewModel.ActiveGenerator = proViewModel.Generators[proViewModel.ActiveGeneratorIndex];
            }
            catch
            {
                string alertMessage = string.Empty;
                alertMessage = "<script>alert('Need first save generator data');</script>";
                // proViewModel.ActiveGenerator = 0;
            }
            if (proViewModel.ActiveGenerator != null)
            {
                GetEquipments(proViewModel, proViewModel.ActiveGenerator);
                GenEquipmentModel newEquipment = CreateNewEquipment(proViewModel, proViewModel.ActiveGenerator);
                if (proViewModel.Equipments != null && proViewModel.Equipments.Count > 0)
                {
                    proViewModel.ActiveEquipmentIndex = proViewModel.Equipments.FindIndex(x => x.ID == newEquipment.ID);
                }
                else
                {
                    proViewModel.ActiveEquipmentIndex = 0;
                }
            }

            GetRelays(proViewModel);
            if (proViewModel.Relays != null && proViewModel.Relays.Count > 0)
            {
                proViewModel.ActivePrimaryPhaseRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                proViewModel.ActivePrimaryGroundRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                proViewModel.ActiveBackupPhaseRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                proViewModel.ActiveBackupGroundRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }

            TempData["GeneratorIndex"] = generatorIndex;
            TempData["ProtectionIndex"] = protectionIndex;
            //PopulateRelatedData(model);
            setUiSettings(HtmlExtensions.PageMode.Protection);
            SetSuperUser();
            this.initializeDropDowns(proViewModel.ActiveProtection);
            ViewBag.ProtectionParentId = proViewModel.ActiveProtection.ParentID.ToString();
            TempData["unsaved"] = "true";
            return View("Index", proViewModel);
        }

        private GenEquipmentModel CreateNewEquipment(ProtectionViewModel model, GeneratorModel activeGenerator)
        {
            GenEquipmentModel genEquipModel = new GenEquipmentModel();
            SM_GEN_EQUIPMENT smGenEquip = new SM_GEN_EQUIPMENT();
            using (SettingsEntities db = new SettingsEntities())
            {
                long mxGeneratorId = (db.SM_GEN_EQUIPMENT.Max(t => (long?)t.ID)) ?? 0;
                genEquipModel.ID = mxGeneratorId + 1;
                genEquipModel.SAPEquipmentID = string.Empty;
                genEquipModel.CurrentOrFuture = CurrentOrFuture_C;
                genEquipModel.GenTechCD = "DG";
                genEquipModel.DateCreated = DateTime.Now.ToShortDateString();
                genEquipModel.CreatedBy = Security.CurrentUser;
                //PXYK,Instead of storing SAP Equipment ID of Generator, Generator ID field in Equipment table will now store ID field value of Generator
                genEquipModel.GeneratorID = activeGenerator.ID;

                genEquipModel.PopulateEntityFromModel(smGenEquip);
                this.initializeDropDowns(genEquipModel);
                if (model.Equipments == null)
                {
                    model.Equipments = new List<GenEquipmentModel>();
                }
                model.Equipments.Add(genEquipModel);
                model.ActiveEquipment = genEquipModel;
                TempData["NewEquipment"] = true;
            }
            return genEquipModel;

        }

        private GeneratorModel CreateNewGenerator(ProtectionViewModel model)
        {
            GeneratorModel genModel = new GeneratorModel();
            SM_GENERATOR smGen = new SM_GENERATOR();
            using (SettingsEntities db = new SettingsEntities())
            {
                long mxGeneratorId = (db.SM_GENERATOR.Max(t => (long?)t.ID)) ?? 0;
                genModel.ID = mxGeneratorId + 1;
                genModel.SapEquipmentID = string.Empty;
                genModel.SapQueueNumber = string.Empty;
                genModel.SapEgiNotification = string.Empty;
                genModel.CurrentOrFuture = CurrentOrFuture_C;
                //genModel.GenTechCd = "INVE";
                genModel.DateCreated = DateTime.Now.ToShortDateString();
                genModel.CreatedBy = Security.CurrentUser;
                genModel.ProtectionId = model.ActiveProtection.ID;
                // genModel.ModeOfInverter = "SMART";
                // genModel.ControlCD = "NONE";
                // genModel.ConnectionCD = "DELT";
                /// genModel.StatusCD = "OFF";

                genModel.PopulateEntityFromModel(smGen);
                this.initializeDropDowns(genModel);
                //GenEquipmentModel genEquipment = new GenEquipmentModel();
                //SM_GEN_EQUIPMENT gen_Equipment = new SM_GEN_EQUIPMENT();
                //long maxEquipmentId = (db.SM_GEN_EQUIPMENT.Max(t => (long?)t.ID)) ?? 0;
                //genModel.ID = maxEquipmentId + 1;
                //genEquipment.ID = maxEquipmentId;
                //genEquipment.GeneratorID = genModel.SapEquipmentID;
                //genEquipment.SAPEquipmentID = "abc456xyz";
                //genEquipment.CurrentOrFuture = CurrentOrFuture_C;
                //genEquipment.DateCreated = DateTime.Now.ToShortDateString();
                //genEquipment.CreatedBy = Security.CurrentUser;
                //genEquipment.PopulateEntityFromModel(gen_Equipment);

                //model.ActiveEquipment = genEquipment;
                //model.Equipments = new List<GenEquipmentModel>();
                //model.Equipments.Add(genEquipment);
                if (model.Generators == null)
                {
                    model.Generators = new List<GeneratorModel>();
                }

                model.Generators.Add(genModel);
                model.ActiveGenerator = genModel;
                TempData["NewGenerator"] = true;
            }
            return genModel;

        }


        [HttpPost]
        public ActionResult ToggleSuperUser(string protectionID, int ActiveProtectionIndex, int ActiveGeneratorIndex, int ActiveEquipmentIndex)
        {

            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsAdminUser = true;
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
            TempData["unsaved"] = "false";
            TempData["GeneratorIndex"] = ActiveGeneratorIndex;
            TempData["EquipmentIndex"] = ActiveEquipmentIndex;
            TempData["ProtectionIndex"] = ActiveProtectionIndex;


            var redirectUrl = new UrlHelper(Request.RequestContext).Action("IndexByID", new { parentType = "Generation", protectionId = protectionID });
            return Json(new { Url = redirectUrl });
            //return RedirectToAction("IndexByID", "Protection", new { parentType = "Generation" , protectionId = protectionID });

        }


        private void PopulateRelatedData(ProtectionViewModel viewModel)
        {
            ViewBag.GenerationID = viewModel.ActiveProtection.ParentID;
            GetProtectionsRelatedParentGen(viewModel);

            if (viewModel.Protections != null && viewModel.Protections.Count > 0)
            {
                viewModel.ActiveProtectionIndex = viewModel.Protections.FindIndex(x => x.ID == viewModel.ActiveProtection.ID);
            }

            GetGenerators(viewModel);

            if (viewModel.Generators != null && viewModel.Generators.Count > 0)
            {
                //Incase if newly added active generator is not in list of generators, add it to the list
                if (viewModel.ActiveGenerator != null)
                {
                    try
                    {
                        int activeIndex = viewModel.Generators.FindIndex(x => x.ID == viewModel.ActiveGenerator.ID);
                        if (activeIndex == -1)
                        {
                            //this means its a new generator which is not yet added in database, so add it to the list of generators, but check whether user click 'delete' button
                            //for the newly added Generator
                            if (ViewData["DeleteNew"] == null || (bool)ViewData["DeleteNew"] == false)
                            {
                                viewModel.Generators.Add(viewModel.ActiveGenerator);
                            }
                        }
                    }
                    catch
                    {

                    }
                }
                AssignActiveGeneratorIndex(viewModel);
                ViewBag.ActiveGeneratorIndex = viewModel.ActiveGeneratorIndex;
                PopulateNameplateRating(viewModel);
            }
            else if (viewModel.ActiveGenerator != null)
            {
                viewModel.Generators = new List<GeneratorModel>();
                viewModel.Generators.Add(viewModel.ActiveGenerator);
                this.initializeDropDowns(viewModel.ActiveGenerator);
                viewModel.ActiveGeneratorIndex = 0;
            }
            else
            {
                viewModel.ActiveGeneratorIndex = -1;
            }
            if (viewModel.ActiveGeneratorIndex != -1)
            {
                viewModel.ActiveGenerator = viewModel.Generators[viewModel.ActiveGeneratorIndex];
                if (viewModel.ActiveGenerator != null)
                {
                    this.initializeDropDowns(viewModel.ActiveGenerator);
                    ViewBag.ProjectName = viewModel.ActiveGenerator.ProjectName;
                    //some times for a newly created generator, SAP Equipment ID might be empty, so no need to check equipments
                    if (!string.IsNullOrEmpty(viewModel.ActiveGenerator.SapEquipmentID) && !string.IsNullOrWhiteSpace(viewModel.ActiveGenerator.SapEquipmentID))
                    {
                        GetEquipments(viewModel, viewModel.ActiveGenerator);
                        if (viewModel.Equipments != null && viewModel.Equipments.Count > 0)
                        {
                            try
                            {
                                AssignActiveEquipmentIndex(viewModel);
                                viewModel.ActiveEquipment = viewModel.Equipments[viewModel.ActiveEquipmentIndex];
                                this.initializeDropDowns(viewModel.ActiveEquipment);
                                ViewBag.ActiveEquipmentIndex = viewModel.ActiveEquipmentIndex;
                            }
                            catch { }
                        }
                        else if (viewModel.ActiveEquipment != null)
                        {
                            viewModel.Equipments = new List<GenEquipmentModel>();
                            viewModel.Equipments.Add(viewModel.ActiveEquipment);
                            this.initializeDropDowns(viewModel.ActiveEquipment);
                            viewModel.ActiveEquipmentIndex = 0;
                        }
                        else
                        {
                            viewModel.ActiveEquipmentIndex = -1;
                        }
                    }
                }
            }

            GetRelays(viewModel);

            if (viewModel.Relays != null && viewModel.Relays.Count > 0)
            {
                viewModel.ActivePrimaryPhaseRelay = viewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                viewModel.ActivePrimaryGroundRelay = viewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                viewModel.ActiveBackupPhaseRelay = viewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                viewModel.ActiveBackupGroundRelay = viewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }


            Session["CurrentProtection"] = viewModel;
            ViewBag.ProtectionParentId = viewModel.ActiveProtection.ParentID.ToString();
            setUiSettings(HtmlExtensions.PageMode.Protection);
            ViewBag.ControllerName = CONTROLLER_NAME;
        }

        private void AssignActiveGeneratorIndex(ProtectionViewModel viewModel)
        {
            if (TempData["GeneratorIndex"] != null && Convert.ToInt32(TempData["GeneratorIndex"]) != -1)
            {
                viewModel.ActiveGeneratorIndex = Convert.ToInt32(TempData["GeneratorIndex"]);
                if (viewModel.ActiveGeneratorIndex == viewModel.Generators.Count)
                    viewModel.ActiveGeneratorIndex = viewModel.ActiveGeneratorIndex - 1;
                if (viewModel.ActiveGeneratorIndex > viewModel.Generators.Count)
                    viewModel.ActiveGeneratorIndex = 0;
            }
            else
            {
                if (viewModel.ActiveGenerator != null && viewModel.Generators != null && viewModel.Generators.Count > 0)
                {
                    if (viewModel.ActiveGenerator.SapEquipmentID != null)
                    {
                        viewModel.ActiveGeneratorIndex = viewModel.Generators.FindIndex(x => x.SapEquipmentID == viewModel.ActiveGenerator.SapEquipmentID);
                    }
                    else if (viewModel.ActiveGenerator.ID != null && viewModel.ActiveGenerator.ID > 0)
                    {
                        viewModel.ActiveGeneratorIndex = viewModel.Generators.FindIndex(x => x.ID == viewModel.ActiveGenerator.ID);
                    }
                    else
                    {
                        viewModel.ActiveGeneratorIndex = 0;
                    }
                    if (viewModel.ActiveGeneratorIndex > 1 && viewModel.ActiveGeneratorIndex == viewModel.Generators.Count)
                        viewModel.ActiveGeneratorIndex = viewModel.ActiveGeneratorIndex - 1;

                }
                else if (viewModel.ActiveGeneratorIndex == -1)
                {
                    viewModel.ActiveGeneratorIndex = 0;
                }
            }
        }

        private void AssignActiveEquipmentIndex(ProtectionViewModel viewModel)
        {
            if (TempData["EquipmentIndex"] != null && Convert.ToInt32(TempData["EquipmentIndex"]) != -1)
            {
                viewModel.ActiveEquipmentIndex = Convert.ToInt32(TempData["EquipmentIndex"]);
            }
            else
            {
                if (viewModel.ActiveEquipment != null && viewModel.Equipments != null && viewModel.Equipments.Count > 0)
                {
                    if (viewModel.ActiveEquipment.SAPEquipmentID != null)
                    {
                        viewModel.ActiveEquipmentIndex = viewModel.Equipments.FindIndex(x => x.SAPEquipmentID == viewModel.ActiveEquipment.SAPEquipmentID);
                    }
                    else if (viewModel.ActiveEquipment.ID != null && viewModel.ActiveEquipment.ID > 0)
                    {
                        viewModel.ActiveEquipmentIndex = viewModel.Equipments.FindIndex(x => x.ID == viewModel.ActiveEquipment.ID);
                    }
                    if (viewModel.ActiveEquipmentIndex > 1 && viewModel.ActiveEquipmentIndex == viewModel.Equipments.Count)
                        viewModel.ActiveEquipmentIndex = viewModel.ActiveEquipmentIndex - 1;

                }
                else if (viewModel.ActiveEquipmentIndex == -1)
                {
                    viewModel.ActiveEquipmentIndex = 0;
                }
            }
        }

        [HttpPost]
        public ActionResult GenEquipTypeChanged(ProtectionViewModel model, string previousGenType)
        {
            /***************ENOS2EDGIS Start****************/
            if (model.ActiveGenerator.Quantity != null && model.ActiveGenerator.NameplateRating != null)
            {
                model.ActiveGenerator.NameplateCapacity = (model.ActiveGenerator.NameplateRating) * (model.ActiveGenerator.Quantity);
                
            }
            if (model.ActiveEquipment.Quantity != null && model.ActiveEquipment.NameplateRating != null)
            {
                model.ActiveEquipment.NameplateCapacity = (model.ActiveEquipment.NameplateRating) * (model.ActiveEquipment.Quantity);
                
            }
            /***************ENOS2EDGIS End****************/
            ViewBag.GenerationID = model.ActiveProtection.ParentID;
            //Add code from offshore team
            if (string.IsNullOrEmpty(model.ActiveEquipment.SAPEquipmentID))
            {
                model.ActiveEquipment.SAPEquipmentID = "0";
            }
            else
            {
            }
            if (!string.IsNullOrEmpty(previousGenType))
            {
                Session["PreviousGenEquipType"] = previousGenType;
            }
            GetProtectionsRelatedParentGen(model);
            if (model.Protections != null && model.Protections.Count > 0)
            {
                model.ActiveProtectionIndex = model.Protections.FindIndex(x => x.ID == model.ActiveProtection.ID);
                this.initializeDropDowns(model.ActiveProtection);
            }

            GetGenerators(model);
            if (model.Generators != null && model.Generators.Count > 0)
            {
                model.ActiveGeneratorIndex = model.Generators.FindIndex(x => x.ID == model.ActiveGenerator.ID);
                this.initializeDropDowns(model.ActiveGenerator);
            }

            if (model.ActiveGenerator != null && (model.ActiveGenerator.GenTechCd == GENTECHCD_INVEXT || model.ActiveGenerator.GenTechCd == GENTECHCD_INVINC))
            {
                GetEquipments(model, model.ActiveGenerator);
                if (model.Equipments != null && model.Equipments.Count > 0)
                {
                    model.ActiveEquipmentIndex = model.Equipments.FindIndex(x => x.ID == model.ActiveEquipment.ID);
                    this.initializeDropDowns(model.ActiveEquipment);
                }
                else if (model.ActiveEquipment != null)
                {
                    model.ActiveEquipmentIndex = 0;
                }
                Session["CurrentGenerator"] = model.ActiveGenerator;
                ViewBag.ProjectName = model.ActiveGenerator.ProjectName;
            }

            GetRelays(model);

            if (model.Relays != null && model.Relays.Count > 0)
            {
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }

            Session["CurrentProtection"] = model;

            setUiSettings(HtmlExtensions.PageMode.Protection);

            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();

            ViewBag.CurrentProtectionId = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = model.ActiveProtection.ID.ToString();
            this.initializeDropDowns(model.ActiveProtection);
            ViewBag.ProtectionParentId = model.ActiveProtection.ParentID.ToString();
            TempData["unsaved"] = "true";
            return View("Index", model);

        }

        [HttpPost]
        public ActionResult GenTypeChanged(ProtectionViewModel model, string previousGenType)
        {
            /***************ENOS2EDGIS Start****************/
            if (model.ActiveGenerator.Quantity != null && model.ActiveGenerator.NameplateRating != null)
            {
                model.ActiveGenerator.NameplateCapacity = (model.ActiveGenerator.NameplateRating) * (model.ActiveGenerator.Quantity);
            }                
               
            /***************ENOS2EDGIS End****************/
            ViewBag.GenerationID = model.ActiveProtection.ParentID;
            if (!string.IsNullOrEmpty(previousGenType))
            {
                Session["PreviousGenType"] = previousGenType;
            }
            GetProtectionsRelatedParentGen(model);
            if (model.Protections != null && model.Protections.Count > 0)
            {
                model.ActiveProtectionIndex = model.Protections.FindIndex(x => x.ID == model.ActiveProtection.ID);
                this.initializeDropDowns(model.ActiveProtection);
            }

            GetGenerators(model);
            if (model.Generators != null && model.Generators.Count > 0)
            {
                model.ActiveGeneratorIndex = model.Generators.FindIndex(x => x.ID == model.ActiveGenerator.ID);
                this.initializeDropDowns(model.ActiveGenerator);
            }

            if (model.ActiveGenerator != null && (model.ActiveGenerator.GenTechCd == GENTECHCD_INVEXT || model.ActiveGenerator.GenTechCd == GENTECHCD_INVINC))
            {
                GetEquipments(model, model.ActiveGenerator);
                if (model.Equipments != null && model.Equipments.Count > 0)
                {
                    //model.ActiveEquipmentIndex = !string.IsNullOrEmpty(activeEquipmentIdx) ? Convert.ToInt32(activeEquipmentIdx) : model.ActiveEquipmentIndex; 
                    if (model.ActiveEquipmentIndex != -1)
                    {
                        model.ActiveEquipment = model.Equipments[model.ActiveEquipmentIndex];
                    }
                    else
                    {
                        model.ActiveEquipment = model.Equipments[0];
                        model.ActiveEquipmentIndex = 0;
                    }
                }
                Session["CurrentGenerator"] = model.ActiveGenerator;
                ViewBag.ProjectName = model.ActiveGenerator.ProjectName;
            }

            GetRelays(model);

            if (model.Relays != null && model.Relays.Count > 0)
            {
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }

            Session["CurrentProtection"] = model;

            setUiSettings(HtmlExtensions.PageMode.Protection);

            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();

            ViewBag.CurrentProtectionId = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = model.ActiveProtection.ID.ToString();
            this.initializeDropDowns(model.ActiveProtection);
            ViewBag.ProtectionParentId = model.ActiveProtection.ParentID.ToString();
            TempData["unsaved"] = "true";
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult ProtectionTypeChanged(ProtectionViewModel model, string previousProtectionType)
        {
            ViewBag.GenerationID = model.ActiveProtection.ParentID;
            if (!string.IsNullOrEmpty(previousProtectionType))
            {
                Session["PreviousProtectionType"] = previousProtectionType;
            }

            GetProtectionsRelatedParentGen(model);
            if (model.Protections != null && model.Protections.Count > 0)
            {
                model.ActiveProtectionIndex = model.Protections.FindIndex(x => x.ID == model.ActiveProtection.ID);
                this.initializeDropDowns(model.ActiveProtection);
            }

            GetGenerators(model);
            if (model.Generators != null && model.Generators.Count > 0)
            {
                //model.ActiveGeneratorIndex = !string.IsNullOrEmpty(activeGeneratoridx) ? Convert.ToInt32(activeGeneratoridx) : model.ActiveGeneratorIndex;
                if (model.ActiveGeneratorIndex != -1)
                    model.ActiveGenerator = model.Generators[model.ActiveGeneratorIndex];
                else
                {
                    model.ActiveGenerator = model.Generators[0];
                    model.ActiveGeneratorIndex = 0;
                }
            }

            if (model.ActiveGenerator != null)
            {
                GetEquipments(model, model.ActiveGenerator);
                if (model.Equipments != null && model.Equipments.Count > 0)
                {
                    //model.ActiveEquipmentIndex = !string.IsNullOrEmpty(activeEquipmentIdx) ? Convert.ToInt32(activeEquipmentIdx) : model.ActiveEquipmentIndex; 
                    if (model.ActiveEquipmentIndex != -1)
                    {
                        model.ActiveEquipment = model.Equipments[model.ActiveEquipmentIndex];
                    }
                    else
                    {
                        model.ActiveEquipment = model.Equipments[0];
                        model.ActiveEquipmentIndex = 0;
                    }
                }
                Session["CurrentGenerator"] = model.ActiveGenerator;
                ViewBag.ProjectName = model.ActiveGenerator.ProjectName;
            }

            GetRelays(model);

            if (model.Relays != null && model.Relays.Count > 0)
            {
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }

            Session["CurrentProtection"] = model;



            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();

            ViewBag.CurrentProtectionId = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = model.ActiveProtection.ID.ToString();
            this.initializeDropDowns(model.ActiveProtection);
            ViewBag.ProtectionParentId = model.ActiveProtection.ParentID.ToString();
            setUiSettings(HtmlExtensions.PageMode.Protection);
            TempData["unsaved"] = "true";
            return View("Index", model);
        }


        private void SetSuperUser()
        {
            if (Security.IsSuperUserActive == false)
            {
                //Session["SuperuserOn"] = false;
                Session["SuperUserButtonText"] = "Activate Super User";
                ViewBag.IsDisabled = true;
            }
            else
            {
                ViewBag.IsDisabled = false;
                Session["SuperUserButtonText"] = "Deactivate Super User";
                //if ((bool)Session["SuperuserOn"])
                //{

                //}
                //else
                //{
                //    ViewBag.IsDisabled = true;
                //    Session["SuperUserButtonText"] = "Activate Super User";
                //}
            }
            if (!SettingsApp.Common.Security.IsInAdminGroup)
                ViewBag.IsAdminUser = true;

        }

        private void initializeDropDowns(object sectModel)
        {
            try
            {
                if (sectModel.GetType() == typeof(ProtectionModel))
                {
                    ((ProtectionModel)sectModel).ProtectionTypeList = new SelectList(SiteCache.GetLookUpValues("SM_PROTECTION", "PROTECTION_TYPE", "GENERATION"), "Key", "Value");
                    ((ProtectionModel)sectModel).FuseTypeList = new SelectList(SiteCache.GetLookUpValues("SM_PROTECTION", "FUSE_TYPE"), "Key", "Value");
                    ((ProtectionModel)sectModel).RecloseCurveList = new SelectList(SiteCache.GetLookUpValues("SM_PROTECTION", "PHA_SLOW_CURVE"), "Key", "Value");
                    ((ProtectionModel)sectModel).ProtectionTypeDropDownPostbackScript = @"var form = document.forms[0]; form.action='/Protection/ProtectionTypeChanged?previousProtectionType=" + ((ProtectionModel)sectModel).ProtectionType + @"';form.submit();";
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
                    ((GeneratorModel)sectModel).TechnologyTypeList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "TECH_TYPE_CD"), "Key", "Value");
                    ((GeneratorModel)sectModel).ConnectionTypeList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "CONNECTION_CD"), "Key", "Value");
                    ((GeneratorModel)sectModel).StatusList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "STATUS_CD"), "Key", "Value");
                    ((GeneratorModel)sectModel).GenTechCodeList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "GEN_TECH_CD"), "Key", "Value");
                    ((GeneratorModel)sectModel).ControlList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "CONTROL_CD"), "Key", "Value");
                    ((GeneratorModel)sectModel).ModeOfInverterList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "MODE_OF_INVERTER"), "Key", "Value");
                    ((GeneratorModel)sectModel).PowerSourceList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "POWER_SOURCE"), "Key", "Value");
                   // ((GeneratorModel)sectModel).CertificationList = new SelectList(SiteCache.GetLookUpValues("SM_GENERATOR", "CERTIFICATION"), "Key", "Value");

                    /***************************************ENOS2EDGIS Start**********************************************/
                    //((GeneratorModel)sectModel).CertificationList = new SelectList(new Dictionary<string, string>(){
                    //                                                         { "UL1741", "UL1741" },
                    //                                                         { "UL1741SA", "UL1741SA" },
                    //                                                         { "", "" }
                    //                                                        }, "Key", "Value");

                    string certificationValues = ConfigurationManager.AppSettings["CertificationValues"];
                    string[] certificationValuesArray = certificationValues.Split('$');                 
                    Dictionary<string, string> certificationDictionary = new Dictionary<string, string>();
                    for (int i = 0; i < certificationValuesArray.Length; i++) {
                        certificationDictionary.Add(certificationValuesArray[i].Split(',')[0], certificationValuesArray[i].Split(',')[1]);
                    }                                    
                    ((GeneratorModel)sectModel).CertificationList = new SelectList(certificationDictionary, "Key", "Value");
                    /***************************************ENOS2EDGIS End**********************************************/                                       
                   
                    ((GeneratorModel)sectModel).GenTypeDropDownPostbackScript = @"var form = document.forms[0]; form.action='/Protection/GenTypeChanged?previousGenType=" + ((GeneratorModel)sectModel).GenTechCd + @"';form.submit();";

                }
                else if (sectModel is GenEquipmentModel)
                {
                    ((GenEquipmentModel)sectModel).ProgramTypeList = new SelectList(SiteCache.GetLookUpValues("SM_GEN_EQUIPMENT", "PROGRAM_TYPE"), "Key", "Value");
                    ((GenEquipmentModel)sectModel).GenTechCodeList = new SelectList(SiteCache.GetLookUpValues("SM_GEN_EQUIPMENT", "GEN_TECH_CD"), "Key", "Value");
                    ((GenEquipmentModel)sectModel).GenEquipTypeDropDownPostbackScript = @"var form = document.forms[0]; form.action='/Protection/GenEquipTypeChanged?previousGenType=" + ((GenEquipmentModel)sectModel).GenTechCD + @"';form.submit();";
                }
                //sectModel.DropDownPostbackScriptScada = @"var form = document.forms[0]; form.action='/" + ViewBag.ControllerName + @"/ScadaChanged/" + ViewBag.GlobalID + @"/" + ViewBag.LayerName + @"';form.submit();";

            }
            catch { }
        }

        public JsonResult GetGeneratorNames(string protectionID)
        {
            long conProtectionId = Convert.ToInt32(protectionID);
            SettingsEntities db = new SettingsEntities();

            return Json(new
            {
                Generators = from u in db.SM_GENERATOR.Where(x => x.PROTECTION_ID == conProtectionId).OrderBy(x => x.ID)
                             select new { Description = u.PROJECT_NAME + " " + u.MANUFACTURER, ID = u.ID }
            }, JsonRequestBehavior.AllowGet);
        }

        private void GetRelays(ProtectionViewModel model)
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

        private void CreateRelays(ProtectionViewModel model, string relayType)
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

        private void AddActiveRelays(ProtectionViewModel model)
        {
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


        private RelayModel CreateRelay(string relayCD, ProtectionViewModel model)
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
                relModel.RelayType = RelayType_1;
                relModel.CurveType = CurveType_1;
                if (relayCD == RelayCd_PPHA || relayCD == RelayCd_BPHA)
                {
                    relModel.RestraintType = RestraintType_0;
                }
                relModel.CurrentOrFuture = CurrentOrFuture_C;
                //if (model.Relays != null && model.Relays.Count > 0 && (long)model.Relays.Max(x => x.ID) == mxGeneratorId + 1)
                //    mxGeneratorId = (long)model.Relays.Max(x => x.ID);
                relModel.ID = mxGeneratorId + 1;
                relModel.ProtectionId = model.ActiveProtection.ID;
            }
            setUiSettings(HtmlExtensions.PageMode.Protection);
            TempData["unsaved"] = "true";
            this.initializeDropDowns(relModel);
            return relModel;
        }

        public ActionResult GetGeneratorBySapEQuipmetId(string protectionId, string sapEquipmentID)
        {
            ProtectionViewModel model = null;
            if (!string.IsNullOrEmpty(protectionId))
            {
                ProtectionModel proModel = GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);
                if (proModel != null)
                {
                    this.initializeDropDowns(proModel);
                    model = new ProtectionViewModel();
                    model.ActiveProtection = proModel;
                }
            }
            else
            {
                model = Session["CurrentProtection"] as ProtectionViewModel;
            }

            ViewBag.GenerationID = model.ActiveProtection.ParentID;

            GetProtectionsRelatedParentGen(model);
            if (model.Protections != null && model.Protections.Count > 0)
            {
                model.ActiveProtectionIndex = model.Protections.FindIndex(x => x.ID == model.ActiveProtection.ID);
            }

            GetGenerators(model);

            if (model.Generators != null && model.Generators.Count > 0)
            {
                model.ActiveGeneratorIndex = model.Generators.FindIndex(x => x.SapEquipmentID == sapEquipmentID);
                if (model.ActiveGeneratorIndex != -1)
                    model.ActiveGenerator = model.Generators[model.ActiveGeneratorIndex];
                else
                {
                    model.ActiveGenerator = model.Generators[0];
                    model.ActiveGeneratorIndex = 0;
                }
            }
            if (model.ActiveGenerator != null)
            {
                GetEquipments(model, model.ActiveGenerator);
                if (model.Equipments != null && model.Equipments.Count > 0)
                {
                    model.ActiveEquipment = model.Equipments[0];
                    model.ActiveEquipmentIndex = 0;
                }
                Session["CurrentGenerator"] = model.ActiveGenerator;
                ViewBag.ProjectName = model.ActiveGenerator.ProjectName;
            }

            GetRelays(model);
            if (model.Relays != null && model.Relays.Count > 0)
            {
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }

            Session["CurrentProtection"] = model;
            setUiSettings(HtmlExtensions.PageMode.Protection);
            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();

            ViewBag.CurrentProtectionId = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = model.ActiveProtection.ID.ToString();
            this.initializeDropDowns(model.ActiveProtection);
            ViewBag.ProtectionParentId = model.ActiveProtection.ParentID.ToString();
            return View("Index", model);

        }

        public ActionResult GetGeneratorById(string protectionId, string genID)
        {
            ProtectionViewModel model = null;
            if (!string.IsNullOrEmpty(protectionId))
            {
                ProtectionModel proModel = GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);
                if (proModel != null)
                {
                    this.initializeDropDowns(proModel);
                    model = new ProtectionViewModel();
                    model.ActiveProtection = proModel;
                }
            }
            else
            {
                model = Session["CurrentProtection"] as ProtectionViewModel;
            }

            ViewBag.GenerationID = model.ActiveProtection.ParentID;

            GetProtectionsRelatedParentGen(model);
            if (model.Protections != null && model.Protections.Count > 0)
            {
                model.ActiveProtectionIndex = model.Protections.FindIndex(x => x.ID == model.ActiveProtection.ID);
            }

            GetGenerators(model);

            if (model.Generators != null && model.Generators.Count > 0)
            {
                long genIdConv = Convert.ToInt32(genID);

                model.ActiveGeneratorIndex = model.Generators.FindIndex(x => x.ID == genIdConv);
                if (model.ActiveGeneratorIndex != -1)
                    model.ActiveGenerator = model.Generators[model.ActiveGeneratorIndex];
                else
                {
                    model.ActiveGenerator = model.Generators[0];
                    model.ActiveGeneratorIndex = 0;
                }
            }
            if (model.ActiveGenerator != null)
            {
                //For newly created generator, we don't need to search for equipments
                if (!string.IsNullOrWhiteSpace(model.ActiveGenerator.SapEquipmentID) && !string.IsNullOrEmpty(model.ActiveGenerator.SapEquipmentID))
                {
                    GetEquipments(model, model.ActiveGenerator);
                    if (model.Equipments != null && model.Equipments.Count > 0)
                    {
                        model.ActiveEquipment = model.Equipments[0];
                        model.ActiveEquipmentIndex = 0;
                    }
                    Session["CurrentGenerator"] = model.ActiveGenerator;
                }
                ViewBag.ProjectName = model.ActiveGenerator.ProjectName;
            }

            GetRelays(model);
            if (model.Relays != null && model.Relays.Count > 0)
            {
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }

            Session["CurrentProtection"] = model;
            setUiSettings(HtmlExtensions.PageMode.Protection);
            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();

            ViewBag.CurrentProtectionId = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = model.ActiveProtection.ID.ToString();
            this.initializeDropDowns(model.ActiveProtection);
            ViewBag.ProtectionParentId = model.ActiveProtection.ParentID.ToString();
            return View("Index", model);

        }

        public ActionResult AddBackupRelay(ProtectionViewModel proViewModel, string protectionID, string backupType, int ActiveProtectionIndex, int ActiveGeneratorIndex, int ActiveEquipmentIndex)
        {
            ViewBag.AddBackup = true;
            ViewBag.GenerationID = proViewModel.ActiveProtection.ParentID;

            GetProtectionsRelatedParentGen(proViewModel);
            if (proViewModel.Protections != null && proViewModel.Protections.Count > 0)
            {
                if (ActiveProtectionIndex != -1)
                    proViewModel.ActiveProtectionIndex = ActiveProtectionIndex;
                else
                {
                    if (!string.IsNullOrEmpty(protectionID))
                    {
                        long proid = Convert.ToInt32(protectionID);
                        proViewModel.ActiveProtectionIndex = proViewModel.Protections.FindIndex(x => x.ID == proid);
                    }
                    else
                    {
                        proViewModel.ActiveProtectionIndex = 0;
                    }
                }
                this.initializeDropDowns(proViewModel.ActiveProtection);
            }

            GetGenerators(proViewModel);
            if (proViewModel.Generators != null && proViewModel.Generators.Count > 0)
            {
                proViewModel.ActiveGeneratorIndex = ActiveGeneratorIndex;
                if (proViewModel.ActiveGeneratorIndex != -1)
                    proViewModel.ActiveGenerator = proViewModel.Generators[proViewModel.ActiveGeneratorIndex];
                else
                {
                    proViewModel.ActiveGenerator = proViewModel.Generators[0];
                    proViewModel.ActiveGeneratorIndex = 0;
                }
            }

            if (proViewModel.ActiveGenerator != null)
            {
                Session["CurrentGenerator"] = proViewModel.ActiveGenerator;
                ViewBag.ProjectName = proViewModel.ActiveGenerator.ProjectName;
                GetEquipments(proViewModel, proViewModel.ActiveGenerator);
                if (proViewModel.Equipments != null && proViewModel.Equipments.Count > 0)
                {
                    proViewModel.ActiveEquipmentIndex = ActiveEquipmentIndex;
                    if (proViewModel.ActiveEquipmentIndex != -1)
                        proViewModel.ActiveEquipment = proViewModel.Equipments[proViewModel.ActiveEquipmentIndex];
                    else
                    {
                        proViewModel.ActiveGenerator = proViewModel.Generators[0];
                        proViewModel.ActiveGeneratorIndex = 0;
                    }

                }
            }

            GetRelays(proViewModel);
            CreateRelays(proViewModel, backupType);

            if (proViewModel.Relays != null && proViewModel.Relays.Count > 0)
            {
                proViewModel.ActivePrimaryPhaseRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                proViewModel.ActivePrimaryGroundRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                proViewModel.ActiveBackupPhaseRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                proViewModel.ActiveBackupGroundRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }

            Session["CurrentProtection"] = proViewModel;

            setUiSettings(HtmlExtensions.PageMode.Protection);

            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();

            ViewBag.CurrentProtectionId = proViewModel.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = proViewModel.ActiveProtection.ID.ToString();
            this.initializeDropDowns(proViewModel.ActiveProtection);
            ViewBag.ProtectionParentId = proViewModel.ActiveProtection.ParentID.ToString();
            return View("Index", proViewModel);
        }

        public ActionResult DeleteBackupRelay(ProtectionViewModel proViewModel, string protectionID, string relayID, int ActiveProtectionIndex, int ActiveGeneratorIndex, int ActiveEquipmentIndex)
        {


            ViewBag.GenerationID = proViewModel.ActiveProtection.ParentID;
            GetGenerators(proViewModel);
            GetRelays(proViewModel);
            DeleteRelay(proViewModel, relayID);
            GetProtectionsRelatedParentGen(proViewModel);
            if (proViewModel.Generators != null && proViewModel.Generators.Count > 0)
            {
                try
                {
                    proViewModel.ActiveGeneratorIndex = ActiveGeneratorIndex;
                    if (proViewModel.ActiveGeneratorIndex != -1)
                        proViewModel.ActiveGenerator = proViewModel.Generators[proViewModel.ActiveGeneratorIndex];
                    else
                    {
                        proViewModel.ActiveGenerator = proViewModel.Generators[0];
                        proViewModel.ActiveGeneratorIndex = 0;
                    }
                    if (proViewModel.Equipments != null && proViewModel.Equipments.Count > 0)
                    {
                        proViewModel.ActiveEquipmentIndex = ActiveEquipmentIndex;
                        if (proViewModel.ActiveEquipmentIndex != -1)
                            proViewModel.ActiveEquipment = proViewModel.Equipments[proViewModel.ActiveEquipmentIndex];
                        else
                        {
                            proViewModel.ActiveGenerator = proViewModel.Generators[0];
                            proViewModel.ActiveGeneratorIndex = 0;
                        }

                    }
                    Session["CurrentGenerator"] = proViewModel.ActiveGenerator;
                    ViewBag.ProjectName = proViewModel.ActiveGenerator.ProjectName;
                }
                catch { }
            }
            if (proViewModel.Relays != null && proViewModel.Relays.Count > 0)
            {
                proViewModel.ActivePrimaryPhaseRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                proViewModel.ActivePrimaryGroundRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                proViewModel.ActiveBackupPhaseRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                proViewModel.ActiveBackupGroundRelay = proViewModel.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }
            if (proViewModel.Protections != null && proViewModel.Protections.Count > 0)
            {
                proViewModel.ActiveProtectionIndex = proViewModel.Protections.FindIndex(x => x.ID == proViewModel.ActiveProtection.ID);
            }
            Session["CurrentProtection"] = proViewModel;

            setUiSettings(HtmlExtensions.PageMode.Protection);

            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();

            ViewBag.CurrentProtectionId = proViewModel.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = proViewModel.ActiveProtection.ID.ToString();
            this.initializeDropDowns(proViewModel.ActiveProtection);
            this.initializeDropDowns(proViewModel.ActiveGenerator);
            this.initializeDropDowns(proViewModel.ActiveEquipment);
            ViewBag.ProtectionParentId = proViewModel.ActiveProtection.ParentID.ToString();
            return View("Index", proViewModel);
        }

        private void DeleteRelay(ProtectionViewModel proViewModel, string relayID)
        {
            if (proViewModel.Relays != null && proViewModel.Relays.Count > 0)
            {
                int relayConId = Convert.ToInt32(relayID);
                int exisitngRelIdx = proViewModel.Relays.FindIndex(x => x.ID == relayConId);
                if (exisitngRelIdx != -1)
                {
                    RelayModel relModel = proViewModel.Relays[exisitngRelIdx];
                    if (DeleteRelay(relModel))
                    {
                        proViewModel.Relays.RemoveAt(exisitngRelIdx);
                    }
                }
            }
        }


        public ActionResult GetEquipmentBySapEQuipmetId(string protectionId, string genSapEquipmentID, string sapEquipmentID)
        {
            ProtectionViewModel model = null;
            if (!string.IsNullOrEmpty(protectionId))
            {
                ProtectionModel proModel = GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);
                if (proModel != null)
                {
                    this.initializeDropDowns(proModel);
                    model = new ProtectionViewModel();
                    model.ActiveProtection = proModel;
                }
            }
            else
            {
                model = Session["CurrentProtection"] as ProtectionViewModel;
            }

            ViewBag.GenerationID = model.ActiveProtection.ParentID;

            GetProtectionsRelatedParentGen(model);
            if (model.Protections != null && model.Protections.Count > 0)
            {
                model.ActiveProtectionIndex = model.Protections.FindIndex(x => x.ID == model.ActiveProtection.ID);
            }

            GetGenerators(model);

            if (model.Generators != null && model.Generators.Count > 0)
            {
                model.ActiveGeneratorIndex = model.Generators.FindIndex(x => x.SapEquipmentID == genSapEquipmentID);
                if (model.ActiveGeneratorIndex != -1)
                    model.ActiveGenerator = model.Generators[model.ActiveGeneratorIndex];
                else
                {
                    model.ActiveGenerator = model.Generators[0];
                    model.ActiveGeneratorIndex = 0;
                }
            }

            GetEquipments(model, model.ActiveGenerator);

            if (model.Equipments != null && model.Equipments.Count > 0)
            {
                model.ActiveEquipmentIndex = model.Equipments.FindIndex(x => x.SAPEquipmentID == sapEquipmentID);
                if (model.ActiveEquipmentIndex != -1)
                {
                    model.ActiveEquipment = model.Equipments[model.ActiveEquipmentIndex];
                }
                else
                {
                    model.ActiveEquipment = model.Equipments[0];
                    model.ActiveEquipmentIndex = 0;
                }
            }
            Session["CurrentGenerator"] = model.ActiveGenerator;
            ViewBag.ProjectName = model.ActiveGenerator.ProjectName;

            GetRelays(model);
            if (model.Relays != null && model.Relays.Count > 0)
            {
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }

            Session["CurrentProtection"] = model;
            setUiSettings(HtmlExtensions.PageMode.Protection);
            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();

            ViewBag.CurrentProtectionId = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = model.ActiveProtection.ID.ToString();
            this.initializeDropDowns(model);
            ViewBag.ProtectionParentId = model.ActiveProtection.ParentID.ToString();
            return View("Index", model);

        }

        public ActionResult GetEquipmentById(string protectionId, string genSapEquipmentID, string sapID)
        {
            ProtectionViewModel model = null;
            if (!string.IsNullOrEmpty(protectionId))
            {
                ProtectionModel proModel = GetProtectionbyID(protectionId, HtmlExtensions.PageMode.Protection);
                if (proModel != null)
                {
                    this.initializeDropDowns(proModel);
                    model = new ProtectionViewModel();
                    model.ActiveProtection = proModel;
                }
            }
            else
            {
                model = Session["CurrentProtection"] as ProtectionViewModel;
            }

            ViewBag.GenerationID = model.ActiveProtection.ParentID;

            GetProtectionsRelatedParentGen(model);
            if (model.Protections != null && model.Protections.Count > 0)
            {
                model.ActiveProtectionIndex = model.Protections.FindIndex(x => x.ID == model.ActiveProtection.ID);
            }

            GetGenerators(model);

            if (model.Generators != null && model.Generators.Count > 0)
            {
                model.ActiveGeneratorIndex = model.Generators.FindIndex(x => x.SapEquipmentID == genSapEquipmentID);
                if (model.ActiveGeneratorIndex != -1)
                    model.ActiveGenerator = model.Generators[model.ActiveGeneratorIndex];
                else
                {
                    model.ActiveGenerator = model.Generators[0];
                    model.ActiveGeneratorIndex = 0;
                }
            }

            GetEquipments(model, model.ActiveGenerator);

            if (model.Equipments != null && model.Equipments.Count > 0)
            {
                long sapIDConv = Convert.ToInt32(sapID);
                model.ActiveEquipmentIndex = model.Equipments.FindIndex(x => x.ID == sapIDConv);
                if (model.ActiveEquipmentIndex != -1)
                {
                    model.ActiveEquipment = model.Equipments[model.ActiveEquipmentIndex];
                }
                else
                {
                    model.ActiveEquipment = model.Equipments[0];
                    model.ActiveEquipmentIndex = 0;
                }
            }
            Session["CurrentGenerator"] = model.ActiveGenerator;
            ViewBag.ProjectName = model.ActiveGenerator.ProjectName;

            GetRelays(model);
            if (model.Relays != null && model.Relays.Count > 0)
            {
                model.ActivePrimaryPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PPHA);
                model.ActivePrimaryGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_PGRD);
                model.ActiveBackupPhaseRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BPHA);
                model.ActiveBackupGroundRelay = model.Relays.FirstOrDefault(x => x.RelayCd == RelayCd_BGRD);
            }

            Session["CurrentProtection"] = model;
            setUiSettings(HtmlExtensions.PageMode.Protection);
            ViewBag.ControllerName = CONTROLLER_NAME;
            SetSuperUser();

            ViewBag.CurrentProtectionId = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionID = model.ActiveProtection.ID.ToString();
            ViewBag.ProtectionParentId = model.ActiveProtection.ParentID.ToString();
            this.initializeDropDowns(model);
            return View("Index", model);

        }

        private void GetGenerators(ProtectionViewModel model)
        {
            List<GeneratorModel> lstofGenerators = new List<GeneratorModel>();
            using (SettingsEntities db = new SettingsEntities())
            {
                IQueryable<SM_GENERATOR> e = db.SM_GENERATOR.Where(s => s.PROTECTION_ID == model.ActiveProtection.ID);
                if (e != null && e.Count() > 0)
                {
                    model.Generators = new List<GeneratorModel>();
                    int i = 0;
                    foreach (SM_GENERATOR generator in e)
                    {
                        GeneratorModel genModel = new GeneratorModel();
                        genModel.PopulateModelFromEntity(generator);
                        this.initializeDropDowns(genModel);
                        lstofGenerators.Add(genModel);
                    }
                    if (model.ActiveGenerator != null && model.ActiveGenerator.ID != null && lstofGenerators.FindIndex(x => x.ID == model.ActiveGenerator.ID) == -1)
                    {
                        //this means its new Generator not yet saved, so add it to list
                        this.initializeDropDowns(model.ActiveGenerator);
                        lstofGenerators.Add(model.ActiveGenerator);
                    }

                    if (lstofGenerators.Count > 0)
                    {
                        var orderedList = lstofGenerators.OrderBy(x => x.ID).ToList();
                        model.Generators = orderedList;
                    }
                }
                else
                {
                    if (model.ActiveGenerator != null && model.ActiveGenerator.ID != null)
                    {
                        //this means its new Generator not yet saved, so add it to list
                        this.initializeDropDowns(model.ActiveGenerator);
                        lstofGenerators.Add(model.ActiveGenerator);
                        model.Generators = lstofGenerators;
                    }
                    //throw new Exception(Constants.GetNoDeviceFoundError("Generator", model.ID.ToString()));
                }
            }
        }

        private void GetEquipments(ProtectionViewModel proModel, GeneratorModel genModel)
        {
            List<GenEquipmentModel> lstOfEquipments = new List<GenEquipmentModel>();
            if (!string.IsNullOrEmpty(genModel.SapEquipmentID))
            {
                using (SettingsEntities db = new SettingsEntities())
                {
                    //ENOS2EDGIS issue 
                    IQueryable<SM_GEN_EQUIPMENT> e = db.SM_GEN_EQUIPMENT.Where(s => s.GENERATOR_ID == genModel.ID);
                    //SM_GEN_EQUIPMENT e1 = db.SM_GEN_EQUIPMENT.FirstOrDefault(s => s.GENERATOR_ID == genModel.ID);
                    if (e != null && e.Count() > 0)
                    {
                        proModel.Equipments = new List<GenEquipmentModel>();
                        foreach (SM_GEN_EQUIPMENT equipment in e)
                        {
                            GenEquipmentModel genEquipModel = new GenEquipmentModel();
                            genEquipModel.PopulateModelFromEntity(equipment);
                            this.initializeDropDowns(genEquipModel);
                            lstOfEquipments.Add(genEquipModel);
                        }
                        if (proModel.ActiveEquipment != null && proModel.ActiveEquipment.ID != null && lstOfEquipments.FindIndex(x => x.ID == proModel.ActiveEquipment.ID) == -1)
                        {
                            //this means its new Equipment not yet saved, so add it to list
                            lstOfEquipments.Add(proModel.ActiveEquipment);
                        }
                        if (lstOfEquipments.Count > 0)
                        {
                            var orderedList = lstOfEquipments.OrderBy(x => x.ID).ToList();
                            proModel.Equipments = orderedList;
                        }
                    }
                    else
                    {
                        if (proModel.ActiveEquipment != null && proModel.ActiveEquipment.ID != null)
                        {
                            //this means its new Equipment not yet saved, so add it to list
                            lstOfEquipments.Add(proModel.ActiveEquipment);
                            proModel.Equipments = lstOfEquipments;
                        }

                        //throw new Exception(Constants.GetNoDeviceFoundError("Generator", model.ID.ToString()));
                    }
                }
            }
        }

        private void GetProtectionsRelatedParentGen(ProtectionViewModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                SM_GENERATION e = db.SM_GENERATION.SingleOrDefault(s => s.ID == model.ActiveProtection.ParentID);
                if (e != null)
                {
                    GenerationModel genModel = new GenerationModel();
                    genModel.PopulateModelFromEntity(e);
                    this.initializeDropDowns(genModel);
                    Session["CurrentGeneration"] = genModel;
                    TempData["GenerationID"] = genModel.ID;
                    model.Protections = new List<ProtectionModel>();
                    GetProtections(genModel, model);
                }
                else
                {
                    //throw new Exception(Constants.GetNoDeviceFoundError("Protection", model.ParentID.ToString()));
                }
            }
        }

        private void GetProtections(GenerationModel genModel, ProtectionViewModel currentProModel)
        {
            List<ProtectionModel> protections = new List<ProtectionModel>();
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
                        this.initializeDropDowns(model);
                        protections.Add(model);
                    }
                    var orderedList = protections.OrderBy(x => x.ID).ToList();
                    currentProModel.Protections = orderedList;
                }
                else
                    throw new Exception(Constants.GetNoDeviceFoundError("Protection", genModel.ID.ToString()));
            }
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
            this.initializeDropDowns(model);
            return model;
        }

        private ProtectionModel GetProtectionbyID(string ID, HtmlExtensions.PageMode pageMode)
        {
            ProtectionModel model = new ProtectionModel();

            using (SettingsEntities db = new SettingsEntities())
            {
                var proIdConv = Convert.ToInt64(ID);
                SM_PROTECTION e = db.SM_PROTECTION.SingleOrDefault(s => s.ID == proIdConv);
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError("Protection", ID));
                model.PopulateModelFromEntity(e);
                this.initializeDropDowns(model);
            }
            return model;
        }

        private ProtectionModel CreateDefaultProtection(int generationID, string parentType)
        {
            ProtectionModel model = new ProtectionModel();
            model.ParentID = generationID;
            model.ParentType = parentType;
            model.ProtectionType = PROTECTION_Type_UNSP;
            model.CurrentOrFuture = CurrentOrFuture_C;

            using (SettingsEntities db = new SettingsEntities())
            {
                SM_PROTECTION e = db.SM_PROTECTION.CreateObject();
                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError("Protection", ""));

                long MaxProtectionId = (db.SM_PROTECTION.Max(t => (long?)t.ID)) ?? 0;
                model.ID = MaxProtectionId + 1;
                model.PopulateEntityFromModel(e);
                //db.SM_PROTECTION.ApplyCurrentValues(e);

                db.SM_PROTECTION.AddObject(e);
                //model.PopulateModelFromEntity(e);
            }
            return model;
        }


        private GenerationModel GetGeneration(string generationId, HtmlExtensions.PageMode pageMode)
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

        private void setUiSettings(HtmlExtensions.PageMode pageMode)
        {
            ViewBag.PageMode = pageMode.ToString().ToUpper();
            ViewBag.ControllerName = CONTROLLER_NAME;
            int P = ViewBag.GenerationID;
            string PP = ViewBag.ProtectionParentId;

            switch (pageMode)
            {
                case HtmlExtensions.PageMode.Current:
                    ViewBag.Title = "Current Settings for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    ViewBag.CurrentClass = "selected";
                    break;
                case HtmlExtensions.PageMode.Future:
                    ViewBag.Title = "Future Settings for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
                    ViewBag.FutureClass = "selected";
                    break;
                case HtmlExtensions.PageMode.History:
                    ViewBag.Title = "History for " + DEVICE_DISPLAY_NAME + " - " + ViewBag.ObjectId;
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
                case HtmlExtensions.PageMode.Generation:
                    if (ViewBag.ProjectName == null || ViewBag.ProjectName == "")
                    {

                        ViewBag.Title = "Generation - " + ViewBag.ObjectId;
                    }
                    else
                    {
                        ViewBag.Title = "Generation - " + ViewBag.ProjectName;
                    }
                    ViewBag.Generation = "selected";
                    break;
                case HtmlExtensions.PageMode.Protection:
                    // ViewBag.Title = "Protection for Generation ID : " + ViewBag.ProtectionParentId;
                    if (ViewBag.ProjectName == null || ViewBag.ProjectName == "")
                    {

                        ViewBag.Title = "Protection for Generation ID : " + ViewBag.GenerationID;
                    }
                    else
                    {
                        ViewBag.Title = "Protection for Generation ID : " + ViewBag.ProjectName;
                    }
                    //  ViewBag.GeneratorTitle = ViewBag.ProjectName;
                    ViewBag.Protection = "selected";
                    break;
                default:
                    break;
            }
        }

        public bool IsGenTechEquipmentAvailble(string GenTechEquipment)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                return db.SM_GENERATOR.Any(m => m.GEN_TECH_EQUIPMENT == GenTechEquipment);
            }
        }

        public bool IsSapEquipmentIdAvailble(string SapEquipmentID)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                return db.SM_GENERATOR.Any(m => m.SAP_EQUIPMENT_ID == SapEquipmentID);
            }
        }

        public bool IsQueueAvailble(string SapQueueNumber)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                return db.SM_GENERATOR.Any(m => m.SAP_QUEUE_NUMBER == SapQueueNumber);
            }
        }

        public bool IsNotificationAvailble(string SapEgiNotification)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                return db.SM_GENERATOR.Any(m => m.SAP_EGI_NOTIFICATION == SapEgiNotification);
            }
        }


        public bool IsEquipmentIDAvailble(string SAPEquipmentID)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                return db.SM_GEN_EQUIPMENT.Any(m => m.SAP_EQUIPMENT_ID == SAPEquipmentID);
            }
        }


        // ENOSTOSAP Phase 2 Req-4-Add BackUp Gen
        private void UpdateBackupGenerationFieldInGeneration(GeneratorModel gen, ProtectionViewModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    try
                    {
                        db.SaveChanges();
                        Transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Transaction.Rollback();
                        db.Connection.Close();
                    }
                    using (var Transaction1 = db.Connection.BeginTransaction())
                    {
                        try
                        {
                            GetGenerators(model);
                            List<GeneratorModel> genModelList = model.Generators;
                            if (genModelList.Any(p => String.Equals(p.BackupGeneration, true)))
                            {
                                SM_GENERATION smGeneration = db.SM_GENERATION.FirstOrDefault(x => x.SAP_EGI_NOTIFICATION == gen.SapEgiNotification);
                                if (smGeneration != null)
                                {
                                    smGeneration.BACKUP_GENERATION = true ? "Y" : "N";
                                }
                            }
                            else
                            {
                                SM_GENERATION smGeneration = db.SM_GENERATION.FirstOrDefault(x => x.SAP_EGI_NOTIFICATION == gen.SapEgiNotification);
                                if (smGeneration != null)
                                {
                                    smGeneration.BACKUP_GENERATION = false ? "Y" : "N";
                                }
                            }
                            try
                            {
                                db.SaveChanges();
                                Transaction1.Commit();
                            }
                            catch (Exception ex)
                            {
                                Transaction1.Rollback();
                                db.Connection.Close();
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        private void PopulateNameplateRating(ProtectionViewModel model)
        {
            using (SettingsEntities db = new SettingsEntities())
            {
                db.Connection.Open();
                using (var Transaction = db.Connection.BeginTransaction())
                {
                    try
                    {
                        db.SaveChanges();
                        Transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        Transaction.Rollback();
                        db.Connection.Close();
                    }
                    using (var Transaction1 = db.Connection.BeginTransaction())
                    {
                        try
                        {
                            GetGenerators(model);
                            List<GeneratorModel> genModelList = model.Generators;
                            model.ActiveProtection.NameplateRating = Convert.ToString(genModelList.Sum(x => x.NameplateCapacity));
                           

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }
    }
}
