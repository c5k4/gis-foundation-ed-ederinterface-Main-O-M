using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SettingsApp.Models;

namespace SettingsApp.Controllers
{
    public class TransformerController : Controller
    {
        //
        // GET: /Transformer/
        private const string DEVICE_TYPE = "TRANSFORMER";
        private const string DEVICE_TABLE_NAME = "SM_SPECIAL_LOAD";
        private const string CONTROLLER_NAME = "transformer";
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SpecialLoad(string GlobalId,string CGCId)
        {
            var model = new SpecialLoadModel();
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.GlobalId = GlobalId;
            model.CGCId = CGCId;
            ViewBag.Title = "Transformer Special Load";
            try
            {
                if (!string.IsNullOrEmpty(GlobalId))
                {
                     model.PopulateModelFromEntity(GlobalId, DEVICE_TYPE);
                }
                else
                {
                    ViewBag.ShowPageError = true;
                    ViewBag.ErrorMessages = "Invalid global id!";
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveSpecialLoad(SpecialLoadModel model,string GlobalId,string CGCId)
        {
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.GlobalId = GlobalId;
            model.CGCId = CGCId;
            ViewBag.Title = "Transformer Special Load";
            try
            {
                if (model != null)
                {
                    if (ModelState.IsValid)
                    {
                        model.PopulateEntityFromModel(GlobalId, DEVICE_TYPE);
                        model.PopulateModelFromEntity(GlobalId, DEVICE_TYPE);
                        TempData["ShowAddSuccessful"]= true;
                        return Redirect(string.Format("/{0}/{1}?globalid={2}&CGCId={3}", CONTROLLER_NAME, "specialload", GlobalId,CGCId));
                        //return RedirectToAction("SpecialLoad", new { GlobalId = GlobalId, CGCId = CGCId });
                    }
                    else
                    {
                        ViewBag.ShowPageError = true;
                    }
                }
            }
            catch(Exception ex)
            {
                ViewBag.ShowPageError = true;
                ViewBag.ErrorMessages = ex.Message;
                
            }
            return View("SpecialLoad", model);
        }

    }
}
