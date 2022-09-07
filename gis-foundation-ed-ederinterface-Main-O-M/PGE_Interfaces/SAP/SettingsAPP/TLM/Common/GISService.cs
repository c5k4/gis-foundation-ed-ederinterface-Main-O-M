using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using TML.Common;

namespace TLM.Common
{
    public class GISService
    {
        private void GetPOSTResponse(Uri uri, string data)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.Method = "POST";
            webrequest.ContentType = "application/json";
            webrequest.ContentLength = 0;
            Stream stream = webrequest.GetRequestStream();
            stream.Close();
            string result;
            using (WebResponse response = webrequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }
        }

        private string GetResponse(Uri uri)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/json";
            webrequest.ContentLength = 0;
            //Stream stream = webrequest.GetRequestStream();
            //stream.Close();
            string result;
            using (WebResponse response = webrequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

        public Dictionary<string, Tuple<int, string>> GetLayers()
        {

            List<string> layersToConsider = new List<string> { "TRANSFORMER" };

            Dictionary<string, Tuple<int, string>> returnVal = new Dictionary<string, Tuple<int, string>>(); ;
            //Uri uri = new Uri(string.Format("{0}?f=json", Constants.GISServiceURI));
            getLayers(layersToConsider, returnVal, Constants.GISServiceURI);

            //uri = new Uri(string.Format("{0}?f=json", Constants.GISSubStationServiceURI));
            getLayers(layersToConsider, returnVal, Constants.GISSubStationServiceURI);

            return returnVal;
        }
       // Add
        public Dictionary<string, Tuple<int, string>> GetFSLayers()
        {

            List<string> layersToConsider = new List<string> { "TRANSFORMER" };

            Dictionary<string, Tuple<int, string>> returnVal = new Dictionary<string, Tuple<int, string>>(); ;

            getLayers(layersToConsider, returnVal, Constants.GISFSServiceURI);

          

            return returnVal;
        }
        private void getLayers(List<string> layersToConsider, Dictionary<string, Tuple<int, string>> returnVal, string url)
        {
            Uri uri = new Uri(string.Format("{0}?f=json", url));
            JObject response = JsonConvert.DeserializeObject<JObject>(GetResponse(uri));

            JToken token;
            response.TryGetValue("layers", out token);
            getTokenValues(layersToConsider, returnVal, token, url);

            response.TryGetValue("tables", out token);
            getTokenValues(layersToConsider, returnVal, token, url);
        }

        private static void getTokenValues(List<string> layersToConsider, Dictionary<string, Tuple<int, string>> returnVal, JToken token, string serviceURL)
        {
            string layerName = string.Empty;
            foreach (JToken t in token)
            {
                layerName = t.Value<string>("name").ToUpper();
                if (layersToConsider.Contains(layerName))
                {
                    layerName = layerName.Replace(':', '!');
                    if (!returnVal.ContainsKey(layerName))
                        returnVal.Add(layerName, new Tuple<int, string>(t.Value<int>("id"), serviceURL));
                    //Debug.WriteLine(string.Concat("id:",t.Value<string>("id"), "   name:",t.Value<string>("name")));
                }
            }
        }

        public Dictionary<string, string> GetProperties(string globalID, int layerID, string serviceURL)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            string url = string.Format("{0}/{1}/query?where=globalid='{2}'&outFields=*&f=json", serviceURL, layerID, globalID);
            Uri uri = new Uri(url);
            JObject response = JsonConvert.DeserializeObject<JObject>(GetResponse(uri));

            JArray myArray = response.Value<JArray>("features");

            foreach (JObject o in myArray.Children<JObject>())
            {
                foreach (JProperty p in o.Value<JObject>("attributes").Properties())
                {
                    retVal.Add(p.Name, p.Value.ToString());
                }
            }
            return retVal;
        }
        //Add
        public Dictionary<string, string> GetFSProperties(string CGC, int layerID, string serviceURL)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            string url = string.Format("{0}/{1}/query?where=CGC12='{2}'&outFields=*&f=json", serviceURL, layerID, CGC);
            Uri uri = new Uri(url);
            JObject response = JsonConvert.DeserializeObject<JObject>(GetResponse(uri));

            JArray myArray = response.Value<JArray>("features");

            foreach (JObject o in myArray.Children<JObject>())
            {
                foreach (JProperty p in o.Value<JObject>("attributes").Properties())
                {
                    retVal.Add(p.Name, p.Value.ToString());
                }
            }
            return retVal;
        }
        public void GetPropertiesOrder(int layerID, out List<GISAttributes> fields, out Dictionary<int, List<GISAttributes>> subTypes, string serviceURL)
        {
            Dictionary<string, Dictionary<string, string>> domainValues = this.getDomainValues(layerID,serviceURL);
            subTypes = new Dictionary<int, List<GISAttributes>>();
            fields = new List<GISAttributes>();
            string url = string.Format("{0}/exts/ArcFMMapServer/id/{1}?f=json", serviceURL, layerID);
            Uri uri = new Uri(url);
            JObject response = JsonConvert.DeserializeObject<JObject>(GetResponse(uri));
            //string j = response.ToString();

            //var jsonResponse = JObject.Parse(j);

          
            //var FeederJson = JObject.Parse(@"{ ""name"": ""FEEDERNAME"", ""type"": ""esriFieldTypeString"",""alias"": ""Feeder Name""}");
          
            //jsonResponse["fields"][4].AddAfterSelf(FeederJson);
            JToken token;
            string key = string.Empty;
            response.TryGetValue("fields", out token);
            
           
            foreach (JToken t in token)
            {
                var attribute = new GISAttributes();
                attribute.DisplayName = t.Value<string>("alias");
                attribute.FieldName = t.Value<string>("name");
                attribute.Type = t.Value<string>("type");
                key = string.Concat(layerID, "_", attribute.FieldName);
                if (domainValues.ContainsKey(key))
                    attribute.lookUp = domainValues[key];
                fields.Add(attribute);
               
                
            }
            
            bool process = true;
            int counter = 1;
            int subTypeID;
            string subTypeKey = string.Empty;
            while (process)
            {
                token = null;

                response.TryGetValue(counter.ToString(), out token);
              
                
                if (token == null)
                    process = false;
                else
                {
                    counter++;
                    subTypeID = token.Value<int>("id");
                    subTypeKey = string.Concat(layerID, "_", subTypeID, "_", "SUBTYPECD");
                    if (!domainValues.ContainsKey(subTypeKey))
                        domainValues[subTypeKey] = new Dictionary<string, string>();

                    domainValues[subTypeKey].Add(subTypeID.ToString(), token.Value<string>("name"));
                    
                    subTypes.Add(subTypeID, new List<GISAttributes>());

                    foreach (JToken t in token.Value<JToken>("domains"))
                    {
                        var attribute = new GISAttributes();
                        attribute.DisplayName = t.Value<string>("alias");
                        attribute.FieldName = t.Value<string>("name");
                        attribute.Type = t.Value<string>("type");
                        key = string.Concat(layerID, "_", token.Value<int>("id"), "_", attribute.FieldName);
                        if (domainValues.ContainsKey(key))
                            attribute.lookUp = domainValues[key];
                        else
                        {
                            key = string.Concat(layerID, "_", attribute.FieldName);
                            if (domainValues.ContainsKey(key))
                                attribute.lookUp = domainValues[key];
                        }

                        fields.Add(attribute);

                        subTypes[token.Value<int>("id")].Add(attribute);
                    }
                }
            }
        }

        private Dictionary<string, Dictionary<string, string>> getDomainValues(int layerID, string serviceUrl)
        {
            Dictionary<string, Dictionary<string, string>> domainValues = new Dictionary<string, Dictionary<string, string>>();
            string dictKey = string.Empty;

            Uri uri = new Uri(string.Format("{0}/{1}?f=json", serviceUrl, layerID));
            JObject response = JsonConvert.DeserializeObject<JObject>(GetResponse(uri));


            JToken token;
            response.TryGetValue("fields", out token);

            foreach (JToken t in token)
            {

                JObject d = t.Value<JObject>("domain");
                if (d != null)
                {
                    JArray myArray = d.Value<JArray>("codedValues");
                    if (myArray != null)
                    {
                        foreach (JToken o in myArray)
                        {
                            dictKey = string.Concat(layerID, "_", t.Value<string>("name"));
                            if (!domainValues.ContainsKey(dictKey))
                            {
                                domainValues.Add(dictKey, new Dictionary<string, string>());
                            }
                            domainValues[dictKey][o.Value<string>("code")] = o.Value<string>("name");

                        }
                    }
                }
            }

            response.TryGetValue("types", out token);
            if (token != null)
            {
                JToken t1 = null;
                for (int x = 0; x < token.Count(); x++)
                {
                    t1 = token[x];
                    foreach (JProperty p in t1.Value<JToken>("domains"))
                    {
                        JToken myToken = p.Value;
                        if (myToken.Value<string>("type") != "inherited")
                        {
                            JArray myArray = myToken.Value<JArray>("codedValues");
                            if (myArray != null)
                            {
                                foreach (JToken o in myArray)
                                {
                                    dictKey = string.Concat(layerID, "_", t1.Value<string>("id"), "_", p.Name);
                                    if (!domainValues.ContainsKey(dictKey))
                                    {
                                        domainValues.Add(dictKey, new Dictionary<string, string>());
                                    }
                                    domainValues[dictKey][o.Value<string>("code")] = o.Value<string>("name");
                                }
                            }
                        }
                    }
                    
                }
            }
            return domainValues;
        }
    }
}