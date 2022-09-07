using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SettingsApp.Models;
using SettingsApp.Common;

namespace SettingsApp.Controllers
{
    public class ConductorController : Controller
    {
        //
        // GET: /Conductor/
        private const string DEVICE_TYPE = "CONDUCTOR";
        private const string DEVICE_TABLE_NAME = "SM_SPECIAL_LOAD";
        private const string CONTROLLER_NAME = "conductor";
        private const string OBJECTID_KEY = "OBJECTID";
        //private const string LAYER_NAME = "Primary Overhead Conductor";
        public ActionResult GIS(string GlobalId, string ObjectId,string LayerName)
        {
            Dictionary<string, string> attributeValues = new Dictionary<string, string>();
            ViewBag.LayerName = LayerName;
            ViewBag.GlobalID = GlobalId;
            ViewBag.ControllerName = CONTROLLER_NAME;
            var model = new SpecialLoadModel();

            Tuple<int, string> layer = SiteCache.GetLayerID(LayerName);

            if (Session[GlobalId] == null)
                GetGISAttributes(GlobalId,LayerName);

            attributeValues = (Dictionary<string, string>)Session[GlobalId];

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


            ViewBag.ObjectId = ObjectId;
            ViewBag.Title = string.Format("GIS Attributes for Conductor-{0}", ObjectId);
            return View(model);
        }

        public ActionResult SpecialLoad(string GlobalId,string ObjectId,string LayerName)
        {
            var model = new SpecialLoadModel();
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.GlobalId = GlobalId;
            ViewBag.LayerName = LayerName;
            if (Session[GlobalId] == null)
                GetGISAttributes(GlobalId,LayerName);
            Dictionary<string, string> attributeValues = (Dictionary<string, string>)Session[GlobalId];
            if (string.IsNullOrEmpty(ObjectId)) ObjectId = SpecialLoadService.GetValueFromAttributes(attributeValues, OBJECTID_KEY);

            ViewBag.ObjectId = ObjectId;
            ViewBag.Title = string.Format("Special Load for Conductor-{0}", ObjectId);
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
            catch (Exception ex)
            {
                throw ex;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult SaveSpecialLoad(SpecialLoadModel model, string GlobalId,string ObjectId,string LayerName)
        {
            ViewBag.ControllerName = CONTROLLER_NAME;
            ViewBag.GlobalId = GlobalId;
            ViewBag.ObjectId = ObjectId;
            ViewBag.LayerName = LayerName;
            ViewBag.Title = string.Format("Special Load for Conductor-{0}", ObjectId);
            try
            {
                if (model != null)
                {
                    if (ModelState.IsValid)
                    {
                        model.PopulateEntityFromModel(GlobalId, DEVICE_TYPE);
                        model.PopulateModelFromEntity(GlobalId, DEVICE_TYPE);
                        TempData["ShowAddSuccessful"] = true;
                        return Redirect(string.Format("/{0}/{1}?globalid={2}&ObjectId={3}&LayerName={4}",CONTROLLER_NAME,"specialload",GlobalId,ObjectId,LayerName));
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

        private void GetGISAttributes(string GlobalId,string LayerName)
        {
            Tuple<int, string> layer = SiteCache.GetLayerID(LayerName);
            Dictionary<string, string> attributeValues = new GISService().GetProperties(GlobalId, layer.Item1, layer.Item2);
            if (Session[GlobalId] == null)
            {
                Session.Add(GlobalId, attributeValues);
            }
        }

    }
}
