using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SettingsApp.Common;
using System.Configuration;

namespace SettingsApp.Controllers
{
    public class HomeController : Controller
    {
        int LayerID;
        string layerType;
        public ActionResult Index()
        {
            //return RedirectToAction("Index", "Regulator", new
            //{
            //    globalID = "{7CD20D26-59DD-477E-A6A2-2A448A7EB16A}",
            //    layerName = "Voltage Regulator",
            //    units = "{0E3A27E0-85F8-4B54-A7DB-2E3ADE787A37}!{0DDCDAC7-EC17-4468-8618-DFE76B77BDAB}!{D8083530-A2C8-4060-B72D-3000AB4B3909}",
            //    id = "na"
            //});

            //return RedirectToAction("Index", "Regulator", new
            //{
            //    globalID = "{7CD20D26-59DD-477E-A6A2-2A448A7EB16A}",
            //    units = "{0DDCDAC7-EC17-4468-8618-DFE76B77BDAB}",
            //    id = "na"
            //});


            //string unitID = new GISService().GetRegulatorUnit("{12E0F1B6-3AD7-4245-9F88-59C475629B2B}", "Sub Voltage Regulator");
            //string unitID = new GISService().GetRegulatorUnit("{7CD20D26-59DD-477E-A6A2-2A448A7EB16A}", "Sub Voltage Regulator (ID: 74)");

            Dictionary<string, string> devices = new Dictionary<string, string>();

            using (SettingsEntities db = new SettingsEntities())
            {

                var swi = db.SM_SWITCH.FirstOrDefault(s => s.DEVICE_ID.Value == 1240033036);
                if (swi != null)
                    devices.Add("Switch", swi.GLOBAL_ID);

                var sec = db.SM_SECTIONALIZER.FirstOrDefault(s => s.DEVICE_ID.Value == 1140033998);
                if (sec != null)
                    devices.Add("Sectionalizer", sec.GLOBAL_ID);

                //var r = db.SM_REGULATOR.FirstOrDefault(s => s.DEVICE_ID.Value == 1140016429);
                //if (r != null)
                //    devices.Add("Regulator", r.GLOBAL_ID);

                //devices.Add("Regulator", "{9CBDC0C9-0A99-4820-9854-F24D95B39BFB}");

                var rec = db.SM_RECLOSER.FirstOrDefault(s => s.DEVICE_ID.Value == 1340005177);
                if (rec != null)
                    devices.Add("Reclosure", rec.GLOBAL_ID);

                var i = db.SM_INTERRUPTER.FirstOrDefault(s => s.DEVICE_ID.Value == 1240034971);
                if (i != null)
                    devices.Add("Interrupter", i.GLOBAL_ID);

                var c = db.SM_CIRCUIT_BREAKER.FirstOrDefault(s => s.DEVICE_ID.Value == 1040022979);
                if (c != null)
                    devices.Add("CircuitBreaker", c.GLOBAL_ID);

                var cap = db.SM_CIRCUIT_BREAKER.FirstOrDefault(s => s.DEVICE_ID.Value == 1240027305);
                if (cap != null)
                    devices.Add("Capacitor", cap.GLOBAL_ID);

                var n = db.SM_SWITCH.FirstOrDefault(s => s.DEVICE_ID.Value == 1340000227);
                if (n != null)
                    devices.Add("NetworkProtector", n.GLOBAL_ID);

            }
            ViewBag.Equipments = devices;
            return View();
        }

        public ActionResult Integration()
        {
            string globalID = Request.QueryString["globalID"];
            string layerName = Request.QueryString["layerName"];
             layerType = Request.QueryString["layerType"];
           
            Tuple<int,string> layer = SiteCache.GetLayerID(Utility.CleanPathString(layerName));
            string settingsAppTable = string.Empty;
            string key = layerName.ToUpper();
            if (key == "DYNAMIC PROTECTIVE DEVICE" || key == "SWITCH")
            {


                int lyID = SiteCache.GetLayerID(layerName).Item1;
                if (layerType == "C")
                {
                    LayerID = lyID + 1;
                }

                else
                {
                    LayerID = lyID + 2;
                }
            }
            else
            {
                LayerID = SiteCache.GetLayerID(layerName).Item1;
            }
            if (SiteCache.LayerTableMapping().ContainsKey(key))
                settingsAppTable = SiteCache.LayerTableMapping()[key];
            else
            {
                
                Dictionary<string, string> attributes = new GISService().GetProperties(globalID, LayerID, layer.Item2);

                //ME Q4 2019 - DA#1
                if (key == "SWITCH" && attributes.ContainsKey("SYMBOLNUMBER"))
                {
                    if (attributes["SYMBOLNUMBER"].ToString() == "40" || attributes["SYMBOLNUMBER"].ToString() == "41" ||
                        attributes["SYMBOLNUMBER"].ToString() == "42" || attributes["SYMBOLNUMBER"].ToString() == "43")
                        settingsAppTable = "SM_SWITCH_MSO";
                }
                if (attributes.ContainsKey("SUBTYPECD") && string.IsNullOrEmpty(settingsAppTable))
                {
                    key = string.Concat(layerName, "_", attributes["SUBTYPECD"]).ToUpper();
                    if ((key == "DYNAMIC PROTECTIVE DEVICE_3" || key == "PROPOSED DYNAMIC PROTECTIVE DEVICE_3") && (attributes["DEVICETYPE"]).ToUpper() == "TS")
                        settingsAppTable = "SM_RECLOSER_TS";
                    else if ((key == "DYNAMIC PROTECTIVE DEVICE_3" || key == "PROPOSED DYNAMIC PROTECTIVE DEVICE_3") && (attributes["DEVICETYPE"]).ToUpper() == "FS")
                        settingsAppTable = "SM_RECLOSER_FS";
                    else if ((key == "DYNAMIC PROTECTIVE DEVICE_3" || key == "PROPOSED DYNAMIC PROTECTIVE DEVICE_3"))
                        settingsAppTable = "SM_RECLOSER";
                    else if (key == "PROPOSED CAPACITOR BANK_2")
                        settingsAppTable = "SM_CAPACITOR";
                    else if (SiteCache.LayerSubTypeTableMapping().ContainsKey(key))
                        settingsAppTable = SiteCache.LayerSubTypeTableMapping()[key];
                }
            }
            return RedirectToController(globalID, Utility.CleanPathString(layerName), settingsAppTable);

            // test query string
            // home/integration?globalID={411C654A-B0DB-4C82-BC15-6627E263C0D3}&layerName=DYNAMIC PROTECTIVE DEVICE
        }

       

        private ActionResult RedirectToController(string globalID, string layerName, string settingsAppTable)
        {
            string controllerName = string.Empty;
            string vrUnits = string.Empty;
           // settingsAppTable = "SM_PRIMARY_METER";
            if (layerName.Contains("Proposed"))
            {
                layerName = layerName.Substring(9);
                layerType = "P";
            }
            switch (settingsAppTable.ToUpper())
            {
                case "SM_CIRCUIT_BREAKER":
                    controllerName = "CircuitBreaker";
                    break;
                case "SM_INTERRUPTER":
                    controllerName = "Interrupter";
                    break;
                case "SM_NETWORK_PROTECTOR":
                    controllerName = "NetworkProtector";
                    break;
                case "SM_RECLOSER":
                    controllerName = "Recloser";
                    break;
                case "SM_REGULATOR":
                    controllerName = "Regulator";
                    break;
                case "SM_SECTIONALIZER":
                    controllerName = "Sectionalizer";
                    break;
                case "SM_SWITCH":
                    controllerName = "Switch";
                    break;
                case "SM_CAPACITOR":
                    controllerName = "Capacitor";
                    break;
                case "SM_PRIMARY_METER":
                    controllerName = "PrimaryMeter";
                    break;
                case "SM_PRIMARY_GEN":
                    controllerName = "PrimaryGenerator";
                    break;
                case "SM_RECLOSER_TS":
                    controllerName = "TripSaver";
                    break;
                case "SM_RECLOSER_FS":
                    controllerName = "FuseSaver";
                    break;
                //************ENOS2EDGIS Start****************
                case "SM_GENERATION":
                    controllerName = "GenerationSettings";
                    break;
                //*************ENOS2EDGIS End******************
                //************ME Q4 2019 - DA#1 Start****************
                case "SM_SWITCH_MSO":
                    controllerName = "MSOSwitch";
                    break;
                //************ME Q4 2019 - DA#1 End****************
                
            }
            if (settingsAppTable.ToUpper() == "SM_REGULATOR")
            {
                //if (layerType == "P")
                //{
                //    layerName = "Proposed " + layerName;
                //}
                var TrfBankLayerNames = ConfigurationManager.AppSettings["TrfBankLayerName"];
                var TrfBankLayerNamesArray = TrfBankLayerNames.Split(';');
                if (layerName == ConfigurationManager.AppSettings["VoltageRegulatorLayerName"] ||
                layerName == ConfigurationManager.AppSettings["SubVoltageRegulatorLayerName"] || TrfBankLayerNamesArray.Contains(layerName))
                {
                    List<string> unitGlobaiID = new GISService().GetRegulatorUnit(globalID, layerName);
                    //INC000004128536
                    if (layerName == ConfigurationManager.AppSettings["SubVoltageRegulatorLayerName"])  
                        vrUnits = getSubVRUnits(unitGlobaiID, globalID);
                    else
                        vrUnits = getVRUnits(unitGlobaiID[0], globalID);
                    return RedirectToAction("Index", controllerName, new { globalID = globalID, layerName = layerName, units = vrUnits, layerType = layerType });
                }
                else
                {
                    // if other than one that are associated with units, pass global id
                    return RedirectToAction("Index", controllerName, new { globalID = globalID, layerName = layerName, units = globalID, layerType = layerType });
                }
            }
            else if (layerName == "Dynamic Protective Device" || layerName == "Switch" || layerName == "Capacitor Bank")
                
            {
                return RedirectToAction("Index", controllerName, new { globalID = globalID, layerName = layerName, layerType = layerType });
            }
            else
                return RedirectToAction("Index", controllerName, new { globalID = globalID, layerName = layerName });
        }


        private string getVRUnits(string unitGlobalID, string globalID)
        {
            string retVal = string.Empty;
            string[] ids = new string[0];
            using (SettingsEntities db = new SettingsEntities())
            {
                // get the unit
                SM_REGULATOR e = db.SM_REGULATOR.SingleOrDefault(s => s.GLOBAL_ID == unitGlobalID && s.CURRENT_FUTURE == "C");

                if (e == null)
                    throw new Exception(Constants.GetNoDeviceFoundError("VoltageRegulator", unitGlobalID));

                if (string.IsNullOrEmpty(e.OPERATING_NUM) || string.IsNullOrEmpty(e.DISTRICT) || string.IsNullOrEmpty(e.DIVISION))
                    throw new Exception("At least one of the Operating Number, District, or Division is empty and cannot continue for Voltage Regulator with GUID " + globalID + ".");

                //var regulatorUnits = db.SM_REGULATOR.Where(s => s.OPERATING_NUM == e.OPERATING_NUM &&
                //s.DISTRICT == e.DISTRICT && s.DIVISION == e.DIVISION && s.CURRENT_FUTURE == "C");
                var regulatorUnits = db.SM_REGULATOR.Where(s => s.OPERATING_NUM == e.OPERATING_NUM && s.GLOBAL_ID == unitGlobalID &&
                s.DISTRICT == e.DISTRICT && s.DIVISION == e.DIVISION && s.CURRENT_FUTURE == "C");

                if (regulatorUnits.Count() > 3)
                    throw new Exception("More than 3 units returned for Voltage Regulator with GUID " + globalID + ".  Total of " + regulatorUnits.Count().ToString() + " returned.");

                if (regulatorUnits.Count() > 0)
                {
                    ids = regulatorUnits.Select(s => s.GLOBAL_ID).ToArray();
                    retVal = string.Join("!", ids);
                }
                // get all the units
            }


            return retVal;
        }

        private string getSubVRUnits(List<string> unitGlobalID, string globalID)    //INC000004128536
        {
            string retVal = string.Empty;
            string[] ids = new string[0];
            using (SettingsEntities db = new SettingsEntities())
            {
                // get the unit
                 var regulatorUnits = db.SM_REGULATOR.Where(s => unitGlobalID.Contains(s.GLOBAL_ID) && s.CURRENT_FUTURE == "C");

                 if (regulatorUnits == null)
                 {
                     string guids = string.Join(",", unitGlobalID.ToArray());
                     throw new Exception(Constants.GetNoDeviceFoundError("VoltageRegulator", guids));
                 }

                if (regulatorUnits.Count() > 3)
                    throw new Exception("More than 3 units returned for Voltage Regulator with GUID " + globalID + ".  Total of " + regulatorUnits.Count().ToString() + " returned.");

                if (regulatorUnits.Count() > 0)
                {
                    ids = regulatorUnits.Select(s => s.GLOBAL_ID).ToArray();
                    retVal = string.Join("!", ids);
                }
                // get all the units
            }


            return retVal;
        }
    }

}
