using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml.Linq;
using System.Web.Configuration;
using NLog;

namespace ArcFMSilverlight.Web
{
    /// <summary>
    /// Summary description for FileUpload
    /// </summary>
    public class FileUpload : IHttpHandler
    {
        //Logger serverSideLogger = LogManager.GetLogger("ServerSideLog");

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";
                var module = context.Request.QueryString["module"];
                var filename = context.Request.QueryString["filename"];
                var guid = context.Request.QueryString["guid"];
                var server = context.Request.QueryString["server"];
                var lanid = context.Request.QueryString["LANID"];

                if (module != null && module.ToString() == "DeleteFileFromServer")
                {
                    string folderPath = WebConfigurationManager.AppSettings["StandardPrintFileUploadDC2"] + "\\";
                    DeleteFileFromServer(lanid.ToString(), folderPath);
                }
                else
                {
                    string fileLocation = string.Empty;

                    if (module != null && module.ToString() == "AddFileToServer")  //INC000004479909
                    {
                        fileLocation = WebConfigurationManager.AppSettings["StandardPrintFileUploadDC2"] + "\\";
                    }
                    else  //SAP RW Notification
                    {
                        string FileUploadDC1 = WebConfigurationManager.AppSettings["SAPFileUploadDC1"];
                        string FileUploadDC2 = WebConfigurationManager.AppSettings["SAPFileUploadDC2"];


                        if (server.ToString() == "DC1")
                            fileLocation = FileUploadDC1 + guid.ToString() + "\\";
                        else
                            fileLocation = FileUploadDC2 + guid.ToString() + "\\";
                    }

                    if (!Directory.Exists(fileLocation))
                        Directory.CreateDirectory(fileLocation);

                    byte[] buffer = new byte[10096];
                    int bytesRead;
                    using (FileStream fs = File.Create(fileLocation + filename.ToString()))
                    {
                        //serverSideLogger.Info("Uploading file " + filename + " to " + fileLocation );
                        while ((bytesRead = context.Request.InputStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            fs.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //serverSideLogger.Error("Error in uploading the files." +  ex.StackTrace.ToString());  
                //throw;
            }
                       
        }

        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[10096];
            int bytesRead;
             
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }

        //INC000004479909
        private void DeleteFileFromServer(string lanid, string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] Files = Directory.GetFiles(folderPath);

                foreach (string file in Files)
                {
                    if (file.Split(new string[] { "\\" }, StringSplitOptions.None).Last().ToString().IndexOf(lanid.ToUpper() + "_") == 0)
                    {
                        File.Delete(file);
                    }
                } 
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
}