using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace TML.Common
{
    public class Constants
    {
        public static string prefix = ConfigurationManager.AppSettings["Environment"];
        public static string GISServiceURI = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "GISService")];
        public static string GISFSServiceURI = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "GISFSService")];
        public static string GISSubStationServiceURI = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "GISSubStationService")];
        public static string SettingsURL = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "SettingsDomain")];

        public static string Warningmsg = ConfigurationManager.AppSettings["WarningDisplayMessage"];
        public static string Warningmsgtext = ConfigurationManager.AppSettings["WarningDisplayMessageText"];
    }
}