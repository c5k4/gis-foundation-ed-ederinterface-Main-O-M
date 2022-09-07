using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Controllers
{
    public class TestController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.Accepted, "Hello World");
        }

        public HttpResponseMessage Post(JToken item)
        {
            Emplyoee emp = new Emplyoee();
            object sysobj = JsonConvert.DeserializeObject(item.ToString());
                    //convert object into JArray
            // ReSharper disable once CollectionNeverUpdated.Local
                    JArray jarr = new JArray(sysobj);
                    //Parse Jarray to SAPRWNotificationDetail Object
                    Emplyoee objEmp = jarr[0].ToObject(emp.GetType()) as Emplyoee;

            if (objEmp != null)
                return Request.CreateResponse(HttpStatusCode.Accepted, "Hello " + objEmp.FirstName + " " + objEmp.LastName);
            return null;
        }

        public class Emplyoee
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

        }
        public HttpResponseMessage Put(JToken item)
        {
            Emplyoee emp = new Emplyoee
            {
                FirstName = "saurabh",
                LastName = "gupta"
            };

            string json = JsonConvert.SerializeObject(emp);

            return Request.CreateResponse(HttpStatusCode.Accepted, json);
        }
    }
}
