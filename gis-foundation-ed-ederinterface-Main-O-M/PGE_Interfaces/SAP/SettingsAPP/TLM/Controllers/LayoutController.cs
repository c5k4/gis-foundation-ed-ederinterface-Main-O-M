using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TLM.Common.Services;

namespace TLM.Controllers
{
    public class LayoutController : Controller
    {
        //
        // GET: /Layout/
        [ChildActionOnly]
        public ActionResult Header(string GlobalId)
        {
            ViewBag.GlobalId = GlobalId;
            var model = new Models.HeaderViewModel();
            ViewBag.IsGenTabVisible = true;
            if (GlobalId != null)
            {
                var TrfLoadingService = new TransformerLoadingService(GlobalId);
                TrfLoadingService.TransformerScreenInit();
                if (!TrfLoadingService.IsGenerationVisible)
                {
                    ViewBag.IsGenTabVisible = false;
                }
                //s2nn for Primary Meter TLM info. (Make sure that request Equipment is Transformer or Primary Meter)
                //Also set the initial Tabs 
                SetSettingsTab(GlobalId);
            }
            return View(model);
        }

        //s2nn for Primary Meter TLM info.
        private void SetSettingsTab(string GlobalId)
        {
            //s2nn for Primary Meter TLM info.
            if (string.IsNullOrEmpty(GlobalId)) return;
            if (ViewBag.isTransformer = !((GlobalId.Split('}').Length > 1 && GlobalId.Split('}')[1].Length > 0)))
            {
                //ViewBag.isTransformer = false;
                ViewBag.Title = "Primary Meter Settings";
                ViewBag.CGCId = GlobalId.Split('}')[1];
                ViewBag.GlobalId = GlobalId.Split('}')[0] + "}";
            }
        }
    }
}
