using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SettingsApp.Common;
namespace SettingsApp
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
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "Messages";
            DefaultModelBinder.ResourceClassKey = "Messages";

            ModelBinders.Binders.Add(typeof(System.Int16), new Integer16Binder());
            ModelBinders.Binders.Add(typeof(System.Int16?), new Integer16NullableBinder());
            ModelBinders.Binders.Add(typeof(System.Int32), new Integer32Binder());
            ModelBinders.Binders.Add(typeof(System.Int32?), new Integer32NullableBinder());
            ModelBinders.Binders.Add(typeof(System.Int64), new Integer64Binder());
            ModelBinders.Binders.Add(typeof(System.Int64?), new Integer64NullableBinder());
            ModelBinders.Binders.Add(typeof(System.Decimal), new DecimalBinder());
            ModelBinders.Binders.Add(typeof(System.Decimal?), new DecimalNullableBinder());
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();

                var httpException = (HttpException)exception;
                Response.StatusCode = httpException.GetHttpCode();

        }


        //void Application_Error(object sender, EventArgs e)
        //{
        //    Exception exc = Server.GetLastError();
        //    SettingsApp.Models.ErrorModel err = new Models.ErrorModel();

        //    // Handle HTTP errors
        //    //if (exc.GetType() == typeof(HttpException) || exc.GetType() == typeof(HttpUnhandledException))
        //    //{
        //        err.ErrorObject = exc;
        //        //Response.Redirect(String.Format("~/Error/{0}/?message={1}", action, exception.Message));
        //        //return;
        //    //}
        //    // Clear the error from the server

        //        Server.Transfer("~/error/index");
        //}
    }
}