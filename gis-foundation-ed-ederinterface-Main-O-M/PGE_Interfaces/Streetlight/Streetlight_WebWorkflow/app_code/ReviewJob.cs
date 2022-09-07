using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.UI;
using System.Data;
using log4net;
using log4net.Config;

namespace PGE.Streetlight
{
    /// <summary>
    /// Summary description for ReviewJob
    /// </summary>
    public class ReviewJob
    {
        private static string _destinationFolder = ConfigurationManager.AppSettings["DESTINATION_PATH"];
        private static string CONNECTION_STRING = ConfigurationManager.AppSettings["CONNECTION_STRING"];
          private static readonly ILog logger = LogManager.GetLogger(typeof(ReviewJob));

          public ReviewJob()
        {
            XmlConfigurator.Configure();
        }
        public string reviewJob(HttpContext context)
        {
            bool isInsertedTransAssignTb = false;
            try
            {
                System.Collections.Specialized.NameValueCollection parameters = new System.Collections.Specialized.NameValueCollection();
                context.Response.ContentType = "text/plain";
                parameters = context.Request.Form;
                string jobId = parameters["jobID"];
                string approveRejectFlag = parameters["approveRejectFlag"];
                string userComments = parameters["comments"];
                string jobDesc = parameters["jobDesc"];
                string userLanId = parameters["userLanId"];
                string fromTaskID = parameters["fromTaskID"];
                bool isJobInitiated = false;
                DataTable dtResponse = new DataTable();
                NewJob newJob = new NewJob();

                isInsertedTransAssignTb = newJob.TransAssignedInsertion(userComments, userLanId, jobDesc, approveRejectFlag, jobId, fromTaskID, isJobInitiated);
            }catch(Exception ex){
                logger.Error("Error in function reviewJob:", ex);
            }
            return (JsonConvert.SerializeObject(isInsertedTransAssignTb));
        }
       
       

    }
}