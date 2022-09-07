using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Net;
using System.IO;
using System.ServiceModel.Configuration;
using System.Web.Hosting;

namespace SettingsApp.Common
{
    public class Documentum
    {
        public static void CreateDocument(string globalID, string deviceType, string operatingNum, string district, string division, string docType, HttpPostedFileBase file)
        {
            var _url = Constants.DocumentumServiceURI;
            var _action = string.Format("{0}?op={1}", _url, "CreateDocument");
            ValidateData(globalID, deviceType, operatingNum, district, division, docType);
            //TODO:
            /* HC - capitalize the operating #, district, division*/
            district = district.Replace("/", "-").Trim().ToUpper();
            division = division.Replace("/", "-").Trim().ToUpper();
            operatingNum = operatingNum.Trim().ToUpper();
            deviceType = deviceType.Trim().ToUpper();
            string fileName = "";
            string currentUserDisplayName = Security.CurrentUserName;
            string currentUserLogin = Security.CurrentUser;
            using (HostingEnvironment.Impersonate())
            {
                if (saveFile(file, out fileName))
                {
                    XmlDocument soapEnvelopeXml = CreateSoapEnvelope(globalID, deviceType, operatingNum, district, division, docType, fileName, currentUserDisplayName, currentUserLogin);
                    HttpWebRequest webRequest = CreateWebRequest(_url, _action);
                    InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
                    string soapResult, errorMessage = string.Empty;
                    try
                    {
                        using (WebResponse webResponse = webRequest.GetResponse())
                        {
                            using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                            {
                                soapResult = rd.ReadToEnd();

                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(soapResult);

                                XmlNamespaceManager mgr = new XmlNamespaceManager(doc.NameTable);
                                mgr.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
                                mgr.AddNamespace("cre", "com/pge/ei/documentmanagement/createdocumentservice");
                                mgr.AddNamespace("doc", "com/pge/ei/documentmanagement");

                                foreach (XmlNode row in doc.SelectNodes("/soapenv:Envelope/soapenv:Body/cre:CreateDocumentV1RequestAcknowledgement/cre:RequestStatus", mgr))
                                {
                                    errorMessage = row.SelectSingleNode("doc:Status", mgr).InnerText;
                                }

                                if (errorMessage.ToUpper() != "SUCCESS")
                                {
                                    Utility.WriteLog(soapResult.ToString());
                                    throw new ApplicationException("Upload of document failed.");
                                }
                            }
                            Console.Write(soapResult);
                        }
                    }
                    catch (WebException wex)
                    {
                        Utility.WriteLog(wex.ToString());
                        throw new ApplicationException(new StreamReader(wex.Response.GetResponseStream()).ReadToEnd());
                    }
                }
            }
        }



        private static void ValidateData(string globalID, string deviceType, string operatingNum, string district, string division, string docType)
        {
            if (string.IsNullOrEmpty(globalID))
                throw new ApplicationException(string.Format("{0} is required for documentum.", "GlobalID"));
            if (string.IsNullOrEmpty(deviceType))
                throw new ApplicationException(string.Format("{0} is required for documentum.", "DeviceType"));
           
            if (string.IsNullOrEmpty(operatingNum))
                throw new ApplicationException(string.Format("{0} is required for documentum.", "Operating Number"));
            if (string.IsNullOrEmpty(district))
                throw new ApplicationException(string.Format("{0} is required for documentum.", "District"));
            if (string.IsNullOrEmpty(division))
                throw new ApplicationException(string.Format("{0} is required for documentum.", "Division"));
            if (string.IsNullOrEmpty(docType))
                throw new ApplicationException(string.Format("{0} is required for documentum.", "Document Type"));


        }

        private static bool saveFile(HttpPostedFileBase file, out string fileName)
        {
            bool retVal = false;
            fileName = "";
            if (file.ContentLength > 0)
            {
                
                fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                fileName = fileName + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss") + extension;
                var path = Path.Combine(Constants.Documentum_File_Drop, fileName);
                file.SaveAs(path);
                retVal = true;
            }
            return retVal;
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(string globalID, string deviceType, string operatingNum, string district, string division, string docType, string fileName, string currentUserDiplayName, string currentUserLogin)
        {
            globalID = globalID.Remove(0, 1);
            globalID = globalID.Remove(globalID.Length - 1, 1);

            System.Text.StringBuilder soapMessage = new System.Text.StringBuilder();
            soapMessage.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            soapMessage.Append(buildStartElenemt("soap:Envelope", true));
            soapMessage.Append(buildAttribute("xmlns:soap", "http://schemas.xmlsoap.org/soap/envelope/"));
            soapMessage.Append(buildAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance"));
            soapMessage.Append(buildAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema"));
            soapMessage.Append(" >");

            soapMessage.Append(buildStartElenemt("soap:Header", false));

            soapMessage.Append(buildStartElenemt("TrackingInfo", true));
            soapMessage.Append(buildAttribute("xmlns", "com/pge/ei/documentmanagement"));
            soapMessage.Append(" >");

            soapMessage.Append(buildElenemt("TrackingId", string.Concat("EDGIS:", Guid.NewGuid())));
            soapMessage.Append(buildElenemt("SourceSystemName", Constants.Documentum_Tracking_SourceSystemName));
            soapMessage.Append(buildElenemt("CreationMatrixKey", Constants.Documentum_Tracking_CreationMatrixKey));
            soapMessage.Append(buildElenemt("RequestedByUser", currentUserLogin));
            soapMessage.Append(buildElenemt("SecurityClassification", Constants.Documentum_Tracking_SecurityClassification));

            soapMessage.Append(buildEndElenemt("TrackingInfo"));
            soapMessage.Append(buildEndElenemt("soap:Header"));

            soapMessage.Append(buildStartElenemt("soap:Body", false));

            soapMessage.Append(buildStartElenemt("CreateDocumentV1Request", true));
            soapMessage.Append(buildAttribute("xmlns", "com/pge/ei/documentmanagement/createdocumentservice"));
            soapMessage.Append(" >");

            soapMessage.Append(buildStartElenemt("RequestHeader", true));
            soapMessage.Append(" />");

            soapMessage.Append(buildStartElenemt("DocumentList", true));
            soapMessage.Append(buildAttribute("xmlns", "com/pge/ei/documentmanagement"));
            soapMessage.Append(" >");

            soapMessage.Append(buildStartElenemt("Document", false));
            soapMessage.Append(buildElenemt("SkipFileTransfer", "Yes"));
            soapMessage.Append(buildElenemt("ContentFileLocation", string.Concat(Constants.Documentum_File_Path, "\\", fileName))); //filepath
            soapMessage.Append(buildElenemt("ContentType", System.IO.Path.GetExtension(string.Concat(Constants.Documentum_File_Drop, "\\", fileName)).TrimStart('.').ToLower()));

            soapMessage.Append(buildStartElenemt("AttributeList", false));

            soapMessage.Append(buildDocumentumAttribute("EQUIP_NBR", operatingNum));
            soapMessage.Append(buildDocumentumAttribute("DIVISION_DESC", division));
            soapMessage.Append(buildDocumentumAttribute("DOC_DESC", fileName));
            //TODO:
            // added FILE_NAME base on suggestion from UVA
            soapMessage.Append(buildDocumentumAttribute("FILE_NAME", fileName));
            soapMessage.Append(buildDocumentumAttribute("DOC_CREATE_DATE", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")));
            soapMessage.Append(buildDocumentumAttribute("BUSS_DOC_TYPE", docType));
            soapMessage.Append(buildDocumentumAttribute("ASSET_LOC_DISTRICT", district));
            soapMessage.Append(buildDocumentumAttribute("ASSET_EQUIPMENT_TYPE", deviceType.Replace("TRIPSAVER", "RECLOSER")));
            soapMessage.Append(buildDocumentumAttribute("DOCUMENT_OWNERS", currentUserDiplayName));
            soapMessage.Append(buildDocumentumAttribute("DOCUMENT_AUTHORS", currentUserDiplayName));
            soapMessage.Append(buildDocumentumAttribute("ASSET_FAMILY", "Electric Distribution"));
            soapMessage.Append(buildDocumentumAttribute("ASSET_TYPE", deviceType.Replace("TRIPSAVER","RECLOSER")));
            soapMessage.Append(buildDocumentumAttribute("ASSET_NAME", operatingNum));
            soapMessage.Append(buildDocumentumAttribute("ASSET_NUM", globalID));

            soapMessage.Append(buildEndElenemt("AttributeList"));
            soapMessage.Append(buildEndElenemt("Document"));
            soapMessage.Append(buildEndElenemt("DocumentList"));

            soapMessage.Append(buildEndElenemt("CreateDocumentV1Request"));
            soapMessage.Append(buildEndElenemt("soap:Body"));
            soapMessage.Append(buildEndElenemt("soap:Envelope"));

            Utility.WriteLog(soapMessage.ToString());

            XmlDocument soapEnvelop = new XmlDocument();
            soapEnvelop.LoadXml(soapMessage.ToString());
            return soapEnvelop;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        private static string buildAttribute(string key, string value)
        {
            return string.Format("{0}=\"{1}\" ", key, System.Security.SecurityElement.Escape(value));
        }

        private static string buildElenemt(string key, string value)
        {
            return string.Format("<{0}>{1}</{0}>", key, System.Security.SecurityElement.Escape(value));
        }

        private static string buildStartElenemt(string key, bool hasAttribute)
        {
            if (hasAttribute)
                return string.Format("<{0} ", key);
            else
                return string.Format("<{0}>", key);
        }

        private static string buildEndElenemt(string key)
        {
            return string.Format("</{0}>", key);
        }

        private static string buildDocumentumAttribute(string key, string value)
        {
            System.Text.StringBuilder message = new System.Text.StringBuilder();
            message.Append(buildStartElenemt("Attribute", false));
            message.Append(buildElenemt("AttributeName", key));
            message.Append(buildElenemt("AttributeValue", System.Security.SecurityElement.Escape(value))); // value of doc
            message.Append(buildEndElenemt("Attribute"));
            return message.ToString();
        }

    }
}