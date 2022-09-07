using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PGE.Interfaces.ProxyForSAPRWNotification.Models;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Controllers
{
    public class SaprwNotificationsController : ApiController
    {
        
        private static readonly ISaprwNotificationDetail Repository = new SaprwNotificationRepo();
        private readonly ITraceWriter _tracer ;

        
        public SaprwNotificationsController()
        {
            _tracer = GlobalConfiguration.Configuration.Services.GetTraceWriter();
        }

        // GET api/saprwnotifications
        #region GET function

        public HttpResponseMessage Get()
        {
            _tracer.Info(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, "Getting Data");
            string[] filepathname = new string[2];
            filepathname[0] = "1";
            filepathname[1] = "2";

            NotificationDetail objnotdel = new NotificationDetail
            {
                NotificationId = "",
                Lanid = "S4GA",
                MapNumber = "mapnumber",
                SubStation = "Substation",
                CircuitId = "circuitID",
                SubmitDate = new DateTime(2008, 12, 28),
                Division = "division",
                Department = "department",
                ConstructionType = "const",
                CorrectionType = "corr",
                Status = 3
            };

            NotificationLocation notloc = new NotificationLocation
            {
                NotificationId = "",
                MapLocationNum = 234,
                MapLocation = "maploc",
                Comments = "comments",
                Latitude = 0.0,
                Longitude = 0.0,
                MapJobId = "mapjobid",
                FileName = filepathname,
                MapCorrectionType = "mapcorr"
            };

            List<NotificationLocation> lstnotloc = new List<NotificationLocation> {notloc, notloc};





            SaprwNotification obj = new SaprwNotification
            {
                Notificationdetails = objnotdel,
                NotificationLocations = lstnotloc
            };
            //obj.FilePathName2 = filepathname;

            string json = JsonConvert.SerializeObject(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, json);
        }

        #endregion

        //POST api/saprwnotifications
        public async Task<HttpResponseMessage> Post(JToken item)
        {
            var isExecute = false;
            _tracer.Info(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, "Starting post method.");
            await Task.Delay(10000);
         
            _tracer.Info(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, "Parsing the Json.");

            //Check User is authenticate
            if (User.Identity.IsAuthenticated)
            {
                
                try
                {
                    var objsaprwnotification = new SaprwNotification();

                    //Deserlialize the input Json 
                    var sysobj = JsonConvert.DeserializeObject(item.ToString());
                    if (sysobj == null) throw new ArgumentNullException("item");

                    //convert object into JArray
                    var objjarr = new JArray(sysobj);

                    //Parse Jarray to SAPRWNotificationDetail Object
                    var objRwNotification = objjarr[0].ToObject(objsaprwnotification.GetType()) as SaprwNotification;
                _tracer.Info(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, "Json Has Parsed.");

                    //Caling the ProcessSAPRWNOTIFICATION to insert the notification detail and send the records to EI
                    isExecute = Repository.ProcessSaprwNotification(objRwNotification);
                    if (!isExecute)
                    {
                        _tracer.Info(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, "Post method has ended");
                        return Request.CreateResponse(HttpStatusCode.OK, false);
                    }
                    _tracer.Info(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, "Post method has ended.");
                    return Request.CreateResponse(HttpStatusCode.OK, true);
                }
                catch (Exception ex)
                {
                   _tracer.Error(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, ex.Message);
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, isExecute);
                }
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized, false);
        }

        // only for test purpose
        #region Put function
        /*
        public async Task<HttpResponseMessage> Put(JToken item)
        {
            await Task.Delay(1);
            string[] filepathname = new string[2];
            filepathname[0] = "1";
            filepathname[1] = "2";

            NotificationDetail objnotdel = new NotificationDetail
            {
                NotificationId = "",
                Lanid = "S4GA",
                MapNumber = "mapnumber",
                SubStation = "Substation",
                CircuitId = "circuitID",
                SubmitDate = new DateTime(2008, 12, 28),
                Division = "division",
                Department = "department",
                ConstructionType = "const",
                CorrectionType = "corr",
                Status = 3
            };

            NotificationLocation notloc = new NotificationLocation
            {
                NotificationId = "",
                MapLocationNum = 234,
                MapLocation = "maploc",
                Comments = "comments",
                Latitude = 0.0,
                Longitude = 0.0,
                MapJobId = "mapjobid",
                FileName = filepathname,
                MapCorrectionType = "mapcorr"
            };

            List<NotificationLocation> lstnotloc = new List<NotificationLocation> { notloc, notloc };





            SaprwNotification obj = new SaprwNotification
            {
                Notificationdetails = objnotdel,
                NotificationLocations = lstnotloc
            };
            obj.FilePathName2 = filepathname;

            string json = JsonConvert.SerializeObject(obj);
            object sysobj = JsonConvert.DeserializeObject(item.ToString());
            JArray Jarr = new JArray(sysobj);
            SAPRWNotificationDetail obj2 = Jarr[0].ToObject(obj.GetType()) as SAPRWNotificationDetail;



            JObject jsonobj = sysobj as JObject;
            jsonobj.ToObject(obj.GetType());

            obj.FilePathName2 = ((Newtonsoft.Json.Linq.JObject)((new System.Linq.SystemCore_EnumerableDebugView<Newtonsoft.Json.Linq.JToken>(((Newtonsoft.Json.Linq.JArray)(sysobj)))).Items[0]))["FilePathName2"];


            return Request.CreateResponse(HttpStatusCode.OK, json);
        }*/

        #endregion


        
    }
}
