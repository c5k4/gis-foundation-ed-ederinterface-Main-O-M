using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Objects.DataClasses;

namespace TLM.Common
{
    public class SiteCache
    {
        # region GIS Methods

        public static Tuple<int,string> GetLayerID(string layerName)
        {
            string key = "GISLayers";
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = new GISService().GetLayers();

            Dictionary<string, Tuple<int, string>> layers = HttpContext.Current.Application[key] as Dictionary<string, Tuple<int, string>>;
            return layers[layerName.ToUpper()];
        }
        //Add
        public static Tuple<int, string> GetFSLayerID(string layerName)
        {
            string key = "GISFSLayers";
            if (HttpContext.Current.Application[key] == null)
                HttpContext.Current.Application[key] = new GISService().GetFSLayers();

            Dictionary<string, Tuple<int, string>> FSlayers = HttpContext.Current.Application[key] as Dictionary<string, Tuple<int, string>>;
            return FSlayers[layerName.ToUpper()];
        }
        public static List<GISAttributes> GetLayerAttributes(int layerID, string serviceURL)
        {
            string key = string.Concat("GISLayer_", layerID, "_", serviceURL);
            List<GISAttributes> fields;
            Dictionary<int, List<GISAttributes>> subTypes;
            if (HttpContext.Current.Application[key] == null ||  HttpContext.Current.Application[string.Concat(key,"_SubTypes")] == null)
            {
                new GISService().GetPropertiesOrder(layerID, out fields, out subTypes, serviceURL);
               
               HttpContext.Current.Application[key] = fields;
               HttpContext.Current.Application[string.Concat(key,"_SubTypes")] = subTypes;
            }
            return HttpContext.Current.Application[key] as List<GISAttributes>;
        }

        public static List<GISAttributes> GetLayerSubTypes(int layerID, int subTypeID, string serviceURL)
        {
            
            string key = string.Concat("GISLayer_", layerID, "_",serviceURL,"_SubTypes");
            if (HttpContext.Current.Application[key] == null)

                GetLayerAttributes(layerID, serviceURL);
         
            Dictionary<int, List<GISAttributes>> subTypes = HttpContext.Current.Application[key] as Dictionary<int, List<GISAttributes>>;
            
           
            return subTypes[subTypeID];
        }

        # endregion

    }
}
