using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.ServiceModel.Activation;
using System.Configuration;
using System.Threading;
using NLog;


namespace WCF_EESGIS
{
     [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    public class Service1 : EESGISService
    {
        
        string UrlEncode(string url)
        {
            Dictionary<string, string> toBeEncoded = new Dictionary<string, string>() { { ",", "%2C" }, { "\\", "%5C" } };
            Regex replaceRegex = new Regex(@"[,\\]");
            MatchEvaluator matchEval = match => toBeEncoded[match.Value];
            string encoded = replaceRegex.Replace(url, matchEval);
            return encoded;
        }
        public Stream GetEESGISData(string OrderNumber, string SystemType)
        {
           
            string qpostData = string.Empty;
            try
            {
                if (OrderNumber != "" && SystemType != "")
                {
                    Helper.LogHelper.Log("Info", "Requesting Data started with Order No: "+OrderNumber+ " System Type:"+SystemType);
                    //string URL = "http://DEETGISGENWC007/SAPGIS/service/data/?OrderNo=30695029&System=ED";
                    string apiurl = ConfigurationManager.AppSettings["APIURL"].ToString();
                    string URL = string.Format(apiurl, OrderNumber, "&", SystemType);
                    //URL = UrlEncode(URL);
                    //URL = GPServiceURL + URL;

                    HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                    request.Method = "GET";
                    request.Expect = "application/json";
                    request.ContentType = "application/json";

                    request.ContentType = "application/json; charset=utf-8";
                    request.Timeout = 300000;
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                    using (StreamReader responseReader = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        // dumps the HTML from the response into a string variable
                        qpostData = responseReader.ReadToEnd();
                        Helper.LogHelper.Log("Info", "Response Received: "+ qpostData);
                    }
                    //qpostData = JsonConvert.SerializeObject(qpostData);
                    string finalReturn = string.Empty;
                    //finalReturn = qpostData.Replace("\"", string.Empty);
                    WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
                    return new MemoryStream(Encoding.UTF8.GetBytes(qpostData));
                    // EESGISDataset jointPoleGpObj = JsonConvert.DeserializeObject<EESGISDataset>(qpostData);

                   // return finalReturn;
                }
                else
                {

                    return null;
                }
            }
            catch (Exception e)
            {
                Helper.LogHelper.Log("Error", "Error In Operation: " + DateTime.Now, e.Message);
                string r = e.Message;
                byte[] byteArray = Encoding.ASCII.GetBytes(r);
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";
                MemoryStream stream = new MemoryStream(byteArray);
                
                return stream;
            }
           // return null;
        }
    }
}
