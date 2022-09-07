using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using NLog;

namespace ArcFMSilverlight.Web.Services
{
    /// <summary>
    /// Summary description for nLogReceiverService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class nLogReceiverService : System.Web.Services.WebService
    {
        [WebMethod]
        public void logmessage()
        {
            Logger serverSideLogger = LogManager.GetLogger("ServerSideLog");

            String userName = HttpContext.Current.Request.ServerVariables["LOGON_USER"];
            String hostAddress = HttpContext.Current.Request.UserHostAddress;
            String hostName = HttpContext.Current.Request.UserHostName;
            String browserName = HttpContext.Current.Request.Browser.Type;
            String browserVersion = HttpContext.Current.Request.Browser.Version;

            if (HttpContext.Current.Request.Form.Count > 1)
            {
                String pLevel = HttpContext.Current.Request.Form[0];
                String pMessage = HttpUtility.HtmlDecode(HttpContext.Current.Request.Form[1]);

                pMessage = userName.Trim() + ", " + browserName.Trim() + ", " + browserVersion + ", " +
                           hostAddress.Trim() + ", " + hostName.Trim() + ", " + pMessage;
                switch (pLevel)
                {
                    case "Info":
                        serverSideLogger.Info(pMessage);
                        break;
                    case "Warn":
                        serverSideLogger.Warn(pMessage);
                        break;
                    case "Error":
                        serverSideLogger.Error(pMessage);
                        break;
                    default:
                        serverSideLogger.Error(pMessage);
                        break;
                }
            }
        }
    }
}
