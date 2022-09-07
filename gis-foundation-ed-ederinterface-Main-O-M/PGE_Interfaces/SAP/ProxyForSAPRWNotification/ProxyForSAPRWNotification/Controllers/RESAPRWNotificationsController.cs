using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Tracing;
using PGE.Interfaces.ProxyForSAPRWNotification.Models;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Controllers
{
    public class ResaprwNotificationsController : ApiController
    {
        private static readonly IReSaprwNotification Repository = new ReSaprwNotificationRepo();
        private readonly ITraceWriter _tracer = GlobalConfiguration.Configuration.Services.GetTraceWriter();


        public HttpResponseMessage Get(string notificationid)
        {
            _tracer.Info(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, "start processing ReSapRWNotificationController");
            if (notificationid == null)
            {
                throw new ArgumentNullException("notificationid");
            }
            return Request.CreateResponse(HttpStatusCode.OK, Repository.Reprocess(notificationid));
        }
    }
}
