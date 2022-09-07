using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace TLM
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);
        }

        //void Application_Error(object sender, EventArgs e)
        //{
            
        //    Exception exc = Server.GetLastError();

        //    // Handle HTTP errors
        //    if (exc.GetType() == typeof(HttpException))
        //    {
               
        //        if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
        //            return;

              
        //        //Server.Transfer("HttpErrorPage.aspx");
        //    }


           
        //    // Clear the error from the server
        //    Server.ClearError();
        //}

        protected void Application_Error()
        {
            var exception = Server.GetLastError();

            var httpException = (HttpException)exception;
            Response.StatusCode = httpException.GetHttpCode();

        }

    }
}