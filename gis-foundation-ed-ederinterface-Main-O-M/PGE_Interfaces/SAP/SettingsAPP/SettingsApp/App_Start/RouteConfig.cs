using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SettingsApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{globalID}/{layerName}/{ID}",
                defaults: new { controller = "Home", action = "Index", globalID = UrlParameter.Optional, layerName = UrlParameter.Optional, ID = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Regulator",
                url: "Regulator/{action}/{globalID}/{layerName}/{units}",
                defaults: new { controller = "Home", action = "Index", globalID = UrlParameter.Optional, layerName = UrlParameter.Optional, units = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "RegulatorHistory",
                url: "Regulator/HistoryDetails/{globalID}/{layerName}/{units}/{ID}",
                defaults: new { controller = "Regulator", action = "HistoryDetails", globalID = UrlParameter.Optional, layerName = UrlParameter.Optional, units = UrlParameter.Optional, ID = UrlParameter.Optional }
            );
        }
    }
}