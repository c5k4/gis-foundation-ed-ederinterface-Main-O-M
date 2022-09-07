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
using System.Configuration;

namespace SettingsApp.Common
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

        public List<string> GetRegulatorUnit(string globalID, string layerName)
        {
            string prefix = ConfigurationManager.AppSettings["Environment"];
            string SubVoltageRegulatorLayerName = ConfigurationManager.AppSettings["SubVoltageRegulatorLayerName"];
            string TrfBankLayerNames = ConfigurationManager.AppSettings["TrfBankLayerName"];
            string[] TrfBankLayerNameArray = TrfBankLayerNames.Split(';'); 
            string layerID = string.Empty;
            Tuple<int, string> layer = SiteCache.GetLayerID(layerName);
            layerID = layer.Item1.ToString();

            Dictionary<string, string> retVal = new Dictionary<string, string>();
            string webServiceURL = string.Empty;
            string url = string.Empty;
            string unitLayerName = string.Empty;

            if (layerName == SubVoltageRegulatorLayerName)
            {
                unitLayerName = ConfigurationManager.AppSettings["SubVoltageRegulatorUnitLayerName"];
                webServiceURL = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "GISSubStationService")];
                layerID = this.getRegulatorUnitLayerID(unitLayerName, webServiceURL);
                url = string.Format("{0}/{1}/query?where=VOLTAGEREGULATORGUID='{2}'&text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=GLOBALID&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=json", webServiceURL, layerID, globalID);
            }
            else if (TrfBankLayerNameArray.Contains(layerName))
            {
                unitLayerName = ConfigurationManager.AppSettings["TrfBankUnitLayerName"];
                webServiceURL = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "GISSubStationService")];
                layerID = this.getRegulatorUnitLayerID(unitLayerName, webServiceURL);
                url = string.Format("{0}/{1}/query?where=TRANSFORMERBANKGUID='{2}'&text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=GLOBALID&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=json", webServiceURL, layerID, globalID);
            }
            else
            {
                unitLayerName = ConfigurationManager.AppSettings["VoltageRegulatorUnitLayerName"];
                webServiceURL = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "GISService")];
                layerID = this.getRegulatorUnitLayerID(unitLayerName, webServiceURL);
                url = string.Format("{0}/{1}/query?where=REGULATORGUID='{2}'&text=&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=GLOBALID&returnGeometry=true&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&f=json", webServiceURL, layerID, globalID);
            }

            // get a single unit guid
            Uri uri = new Uri(url);
            JObject response = JsonConvert.DeserializeObject<JObject>(GetResponse(uri));

            JArray myArray = response.Value<JArray>("features");

            List<string> globalIdList = new List<string>();

            if (layerName == SubVoltageRegulatorLayerName)    //INC000004128536

            {
                foreach (JObject o in myArray.Children<JObject>())
                {
                    foreach (JProperty p in o.Value<JObject>("attributes").Properties())
                    {
                        if (p.Name.ToUpper() == "GLOBALID")
                        {
                            globalIdList.Add(p.Value.ToString());
                        }
                    }                       
                }
            }
            else
            {
                foreach (JObject o in myArray.Children<JObject>())
                {
                    foreach (JProperty p in o.Value<JObject>("attributes").Properties())
                    {
                        retVal.Add(p.Name.ToUpper(), p.Value.ToString());
                    }
                    if (retVal.ContainsKey("GLOBALID"))
                    {
                        globalIdList.Add(retVal["GLOBALID"]);
                        break;
                    }
                }
            }

            if (globalIdList.Count > 0)
                return globalIdList;
            else
                throw new Exception("No Units Found for Voltage Regulator with GUID " + globalID + ".");

        }

        private string getRegulatorUnitLayerID(string unitLayerName, string url)
        {
            string layerID = string.Empty;
            string prefix = ConfigurationManager.AppSettings["Environment"];
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            Uri uri = new Uri(url);
            
            try
            {
                string resp = GetResponse(uri);

                int xx = resp.IndexOf(unitLayerName);
                string temp = resp.Substring(resp.IndexOf(unitLayerName) + unitLayerName.Length, 10);

                temp = temp.Remove(temp.IndexOf(")"));
                temp = temp.Substring(temp.IndexOf("(") + 1);

                bool parseSuccess = false;
                int intLayerID = 0;
                parseSuccess = int.TryParse(temp, out intLayerID);

                if (parseSuccess)
                    layerID = intLayerID.ToString();

                // TODO: refactor using json object
                //JObject response = JsonConvert.DeserializeObject<JObject>(GetResponse(uri));
                //JArray myArray = response.Value<JArray>("features");

                //foreach (JObject o in myArray.Children<JObject>())
                //{
                //    foreach (JProperty p in o.Value<JObject>("attributes").Properties())
                //    {
                //        retVal.Add(p.Name.ToUpper(), p.Value.ToString());
                //    }
                //    break;
                //}

                //if (retVal.ContainsKey(SubVoltageRegulatorUnitLayerName))
                //    layerID = retVal[SubVoltageRegulatorUnitLayerName];
            }
            catch (Exception ex)
            {
            }

            return layerID;
        }

        public Dictionary<string, Tuple<int, string>> GetLayers()
        {

            List<string> layersToConsider = SiteCache.DistinctLayers();
            
            Dictionary<string, Tuple<int, string>> returnVal = new Dictionary<string, Tuple<int, string>>(); ;
            //Uri uri = new Uri(string.Format("{0}?f=json", Constants.GISServiceURI));
            getLayers(layersToConsider, returnVal, Constants.GISServiceURI);

            //uri = new Uri(string.Format("{0}?f=json", Constants.GISSubStationServiceURI));
            getLayers(layersToConsider, returnVal, Constants.GISSubStationServiceURI);

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
                if (t.HasValues)
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
        }

        public Dictionary<string, string> GetProperties(string globalID, int layerID, string serviceURL)
        {
           
            //layerID = layerID + 1;
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

        public void GetPropertiesOrder(int layerID, out List<GISAttributes> fields, out Dictionary<int, List<GISAttributes>> subTypes, string serviceURL)
        {
            //layerID = layerID + 1;
            Dictionary<string, Dictionary<string, string>> domainValues = this.getDomainValues(layerID,serviceURL);
            subTypes = new Dictionary<int, List<GISAttributes>>();
            fields = new List<GISAttributes>();
            string url = string.Format("{0}/exts/ArcFMMapServer/id/{1}?f=json", serviceURL, layerID);
            Uri uri = new Uri(url);
            JObject response = JsonConvert.DeserializeObject<JObject>(GetResponse(uri));
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