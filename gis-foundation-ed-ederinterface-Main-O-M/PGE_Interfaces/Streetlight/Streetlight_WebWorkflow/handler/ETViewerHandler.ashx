
<%@ WebHandler Language="C#" Class="ETViewerHandler" %>

using System;
using System.Web;
using PGE.Streetlight;
using log4net;
public class ETViewerHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        ILog Log = LogManager.GetLogger(typeof(ETViewerHandler));
        System.Collections.Specialized.NameValueCollection parameters = new System.Collections.Specialized.NameValueCollection();
        context.Response.ContentType = "text/plain";

        try
        {
            parameters = context.Request.Form;
        }

        catch (Exception ex)
        {
            Log.Error("ETViewerHandler ", ex);
        }

        string moduleName = parameters["module"];       
        string _filePathArray = parameters["postData"];
        
        string requestFeedback = "";
       
        switch (moduleName)
        {
            // User profile manages user's setting, bookmarks etc.
            case "User":
                PGE.Streetlight.Login login = new PGE.Streetlight.Login();
                requestFeedback = login.ValidateAndGetLoginInfo(context);
                context.Response.Write(requestFeedback);
                break;

            case "NewJob":
                PGE.Streetlight.NewJob newJob = new PGE.Streetlight.NewJob();
                requestFeedback = newJob.ProcessJob(context);
                context.Response.Write(requestFeedback);
                break;
            case "Review":
                PGE.Streetlight.ReviewJob reviewJob = new PGE.Streetlight.ReviewJob();
                requestFeedback = reviewJob.reviewJob(context);
                context.Response.Write(requestFeedback);
                break;
            case "MainJob":
                PGE.Streetlight.MainJob mainJob = new PGE.Streetlight.MainJob();
                requestFeedback = mainJob.getMainJobData(context);
                context.Response.Write(requestFeedback);
                break;
            default:
                context.Response.Write(requestFeedback);
                break;
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
