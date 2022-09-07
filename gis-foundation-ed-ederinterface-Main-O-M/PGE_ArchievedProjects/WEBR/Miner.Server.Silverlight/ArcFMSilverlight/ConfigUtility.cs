using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Bing;
using ESRI.ArcGIS.Client.Bing.GeocodeService;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit.DataSources;
using ESRI.ArcGIS.Client.WebMap;

using Miner.Server.Client;
using Miner.Server.Client.Query;
using Miner.Server.Client.Tasks;
using System.ServiceModel.DomainServices.Client.ApplicationServices;

using ArcFM.Silverlight.PGE.CustomTools;
using System.Json;
using System.Text.RegularExpressions;
using System.Net;
using System.Windows.Controls;
using ArcFMSilverlight.Controls.Tracing;
using System.Windows;
using System.Windows.Browser;
using Miner.Server.Client.Toolkit;
using ESRI.ArcGIS.Client.FeatureService;

namespace ArcFMSilverlight
{
    public static class ConfigUtility
    {
        /// <summary>
        /// This specifies the object class ID mapping. Dictionary structure is Dictionary<MapserverName, Dictionary<ObjectClassID, List<LayerIDs>>>
        /// </summary>
        public static Dictionary<string, List<Relationship>> MapURLToRelationshipsMap = new Dictionary<string, List<Relationship>>();
        public static Dictionary<int, string> ObjectClassIDToSubtypeFieldMap = new Dictionary<int, string>();
        public static Dictionary<string, Dictionary<int, List<int>>> ObjectClassIDMapping = new Dictionary<string, Dictionary<int, List<int>>>();
        public static Dictionary<string, Dictionary<int, List<string>>> LayerIDToFieldMapping = new Dictionary<string, Dictionary<int, List<string>>>();
        public static Dictionary<string, string> IdentifyFieldMapping = new Dictionary<string, string>();
        public static TextBlock StatusBar { get; set; }
        public static bool TracingClassIDMapBuilt = false;
        public static Map CurrentMap = null;
        private static string CurrentInitLayerURL = "";
        public static ArcGISDynamicMapServiceLayer tracingLayer = null;
        public static string LocatorService = "";
        public static int WKID = 0;

        //Alias field name mapping.  Name of Domain, Domain Value, Domain Description
        public static Dictionary<string, Dictionary<string, string>> AliasFieldNameMapping = new Dictionary<string, Dictionary<string, string>>();

        //Coded domain values.  Name of Domain, Domain Value, Domain Description
        public static Dictionary<string, Dictionary<string, string>> CodedDomainValues = new Dictionary<string, Dictionary<string, string>>();
        public static Dictionary<string, string> DomainNamesByFieldsAndSubtype = new Dictionary<string, string>();
        public static ESRI.ArcGIS.Client.FeatureService.CodedValueDomain DivisionCodedDomains = null;
        public static ESRI.ArcGIS.Client.FeatureService.CodedValueDomain GenInfoLimitedExportCodedDomains = null; //ENOS Tariff Change
        //PONS Application variable
        public static string SERVICEPOINTID = "SERVICEPOINTID";
        public static string SERVICELOCATIONGUID;

        public static bool HasSpecialCharacters(string str)
        {
            string specialCharacters = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
            //string specialCharacters = @"%@$%&*()><'|}]{[_~`+=-";
            char[] specialCharactersArray = specialCharacters.ToCharArray();

            int index = str.IndexOfAny(specialCharactersArray);
            //index == -1 no special characters
            if (index == -1)
                return false;
            else
                return true;
        }

        public static bool IsItNumber(string inputvalue)
        {
            Regex isnumber = new Regex("[^0-9]");
            return !isnumber.IsMatch(inputvalue);

        }




        //End
        public static void AddRelationships(string URL, int layerID, List<Relationship> relationships)
        {
            string key = URL.ToUpper() + "-" + layerID;
            if (!MapURLToRelationshipsMap.ContainsKey(key))
            {
                MapURLToRelationshipsMap.Add(key, relationships);
            }
        }

        /// <summary>
        /// Returns a list of all relationships layer IDs for the specified feature layer ID
        /// </summary>
        /// <param name="URL">URL of the service to query</param>
        /// <param name="layerID">Layer ID of the desired feature layer</param>
        /// <returns></returns>
        public static List<Relationship> GetRelatedTableLayerIDs(string URL, int layerID)
        {
            string key = URL.ToUpper() + "-" + layerID;
            if (MapURLToRelationshipsMap.ContainsKey(key))
            {
                return MapURLToRelationshipsMap[key];
            }
            return new List<Relationship>();
        }

        /// <summary>
        /// Returns the request relationship layer ID with the requested relationship name
        /// </summary>
        /// <param name="URL">URL of the service to query</param>
        /// <param name="layerID">Layer ID of feature layer with relationships</param>
        /// <param name="relationshipName">Name of the desired relationship</param>
        /// <returns></returns>
        public static Relationship GetRelationshipByName(string URL, int layerID, string relationshipName)
        {
            string key = URL.ToUpper() + "-" + layerID;
            if (MapURLToRelationshipsMap.ContainsKey(key))
            {
                foreach (Relationship rel in MapURLToRelationshipsMap[key])
                {
                    if (rel.Name.ToUpper() == relationshipName.ToUpper())
                    {
                        return rel;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the request relationship layer ID with the requested relationship name
        /// </summary>
        /// <param name="URL">URL of the service to query</param>
        /// <param name="layerID">Layer ID of feature layer with relationships</param>
        /// <param name="relationshipName">Name of the desired relationship</param>
        /// <returns></returns>
        public static Relationship GetRelationshipByRelatedLayerID(string URL, int layerID, int relationshipLayerID)
        {
            string key = URL.ToUpper() + "-" + layerID;
            if (MapURLToRelationshipsMap.ContainsKey(key))
            {
                foreach (Relationship rel in MapURLToRelationshipsMap[key])
                {
                    if (rel.RelatedTableId == relationshipLayerID)
                    {
                        return rel;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Add a new subtype field name for a class ID
        /// </summary>
        /// <param name="classID">Class ID of the feature class</param>
        /// <param name="subtypeFieldName">Subtype field name</param>
        public static void AddNewSubtypeField(int classID, string subtypeFieldName)
        {
            if (!ObjectClassIDToSubtypeFieldMap.ContainsKey(classID))
            {
                ObjectClassIDToSubtypeFieldMap.Add(classID, subtypeFieldName);
            }
        }

        /// <summary>
        /// Get the subtype field name for a provided class ID
        /// </summary>
        /// <param name="classID">Class ID</param>
        /// <returns></returns>
        public static string GetSubtypeFieldName(int classID)
        {
            if (ObjectClassIDToSubtypeFieldMap.ContainsKey(classID))
            {
                return ObjectClassIDToSubtypeFieldMap[classID];
            }
            return "";
        }

        /// <summary>
        /// Add a new field alias name to the cache
        /// </summary>
        /// <param name="Url">MapService URL</param>
        /// <param name="layerID">Layer ID being queried</param>
        /// <param name="FieldName">Field Name</param>
        /// <param name="description"></param>
        public static void AddAliasFieldNameMap(string Url, int layerID, string FieldName, string aliasName)
        {
            string searchValue = (Url + "-" + layerID + "-" + FieldName).ToUpper();

            //Something failed so we'll perform checks before adding here.
            if (!AliasFieldNameMapping.ContainsKey(searchValue))
            {
                AliasFieldNameMapping.Add(searchValue, new Dictionary<string, string>());
            }
            if (!AliasFieldNameMapping[searchValue].ContainsKey(FieldName))
            {
                AliasFieldNameMapping[searchValue].Add(FieldName, aliasName);
            }
        }

        /// <summary>
        /// Returns a field name alias if one exists in the cache
        /// </summary>
        /// <param name="Url">MapService URL</param>
        /// <param name="layerID">Layer ID being queried</param>
        /// <param name="FieldName">Field Name</param>
        /// <returns></returns>
        public static string GetFieldNameAlias(string Url, int layerID, string FieldName)
        {
            string searchValue = (Url + "-" + layerID + "-" + FieldName).ToUpper();

            if (AliasFieldNameMapping.ContainsKey(searchValue))
            {
                if (AliasFieldNameMapping[searchValue].ContainsKey(FieldName))
                {
                    return AliasFieldNameMapping[searchValue][FieldName];
                }
            }
            //If there was no matching field name alias, just return the field name
            return FieldName;
        }

        /// <summary>
        /// Add a new domain code and description value to the cached list
        /// </summary>
        /// <param name="Url">MapService URL</param>
        /// <param name="layerID">Layer ID being queried</param>
        /// <param name="subtypeValue">Subtype value</param>
        /// <param name="FieldName">Field Name</param>
        /// <param name="domainName">Name of the Domain</param>
        /// <param name="code">Domain Code</param>
        /// <param name="description"></param>
        public static void AddCodedValueDomain(string Url, int layerID, string subtypeValue, string FieldName, string domainName, object code, string description)
        {
            string searchValue = (Url + "-" + layerID + "-" + subtypeValue + "-" + FieldName).ToUpper();

            //Something failed so we'll perform checks before adding here.
            if (!CodedDomainValues.ContainsKey(searchValue))
            {
                CodedDomainValues.Add(searchValue, new Dictionary<string, string>());
            }
            if (!CodedDomainValues[searchValue].ContainsKey(code.ToString()))
            {
                CodedDomainValues[searchValue].Add(code.ToString(), description);
            }
        }

        /// <summary>
        /// Returns a domain description if one exists in the cache
        /// </summary>
        /// <param name="Url">MapService URL</param>
        /// <param name="layerID">Layer ID being queried</param>
        /// <param name="subtypeValue">Subtype value</param>
        /// <param name="FieldName">Field Name</param>
        /// <param name="code">Domain Code</param>
        /// <returns></returns>
        public static string GetDomainDescription(string Url, int layerID, string subtypeValue, string FieldName, object code)
        {
            string searchValue = (Url + "-" + layerID + "-" + subtypeValue + "-" + FieldName).ToUpper();

            if (CodedDomainValues.ContainsKey(searchValue))
            {
                if (CodedDomainValues[searchValue].ContainsKey(code.ToString()))
                {
                    return CodedDomainValues[searchValue][code.ToString()];
                }
            }
            //If there was no matching domain code, just return the code
            return code.ToString();
        }


        public static void AddLayerIDToMap(string MapServerURL, int classID, int layerID)
        {
            if (!ObjectClassIDMapping.ContainsKey(MapServerURL))
            {
                ObjectClassIDMapping.Add(MapServerURL, new Dictionary<int, List<int>>());
            }
            if (!ObjectClassIDMapping[MapServerURL].ContainsKey(classID))
            {
                ObjectClassIDMapping[MapServerURL].Add(classID, new List<int>());
            }
            if (!ObjectClassIDMapping[MapServerURL][classID].Contains(layerID))
            {
                ObjectClassIDMapping[MapServerURL][classID].Add(layerID);
            }
        }

        public static int GetLayerIDFromLayerName(Map currentMap, string mapServerURL, string LayerName)
        {
            foreach (Layer layer in currentMap.Layers)
            {
                if (layer is ArcGISDynamicMapServiceLayer)
                {
                    ArcGISDynamicMapServiceLayer dynamicMapServiceLayer = layer as ArcGISDynamicMapServiceLayer;
                    if (dynamicMapServiceLayer.Url == mapServerURL)
                    {
                        foreach (LayerInfo layerInfo in dynamicMapServiceLayer.Layers)
                        {
                            if (layerInfo.Name == LayerName)
                            {
                                return layerInfo.ID;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        public static string GetLayerNameFromLayerID(ArcGISDynamicMapServiceLayer dynamicLayer, int layerID)
        {
            foreach (LayerInfo layerInfo in dynamicLayer.Layers)
            {
                if (layerInfo.ID == layerID)
                {
                    return layerInfo.Name;
                }
            }
            return "";
        }

        public static List<string> GetLayerIDToFieldMapping(string mapServerURL, int LayerID)
        {
            if (LayerIDToFieldMapping.ContainsKey(mapServerURL))
            {
                if (LayerIDToFieldMapping[mapServerURL].ContainsKey(LayerID))
                {
                    return LayerIDToFieldMapping[mapServerURL][LayerID];
                }
            }
            return new List<string>();
        }

        public static List<int> GetLayerIDFromClassID(string mapServerURL, int classID)
        {
            if (ObjectClassIDMapping.ContainsKey(mapServerURL))
            {
                if (ObjectClassIDMapping[mapServerURL].ContainsKey(classID))
                {
                    return ObjectClassIDMapping[mapServerURL][classID];
                }
            }
            return new List<int>();
        }

        public static int GetClassIDFromLayerID(string mapServerURL, int layerID)
        {
            if (ObjectClassIDMapping.ContainsKey(mapServerURL))
            {
                Dictionary<int, List<int>> classIDToLayerIDsMap = ObjectClassIDMapping[mapServerURL];
                foreach (KeyValuePair<int, List<int>> classIDToLayer in classIDToLayerIDsMap)
                {
                    foreach (int ID in classIDToLayer.Value)
                    {
                        if (ID == layerID) { return classIDToLayer.Key; }
                    }
                }
            }
            return -1;
        }

        public static void GetClassIDToLayerIDMapping(Action<bool> callback, string mapServerURL, Map currentMap, string statusMessage, bool isSchematics)
        {
            CurrentCallBack = callback;
            currentStatusMessage = statusMessage;
            CurrentMap = currentMap;
            CurrentInitLayerURL = mapServerURL;
            if (!TracingClassIDMapBuilt)
            {
                tracingLayer = new ArcGISDynamicMapServiceLayer();
                tracingLayer.Url = TracingHelper.LoadingInformationTracingTableURL;
                ClassIDsStillToLoad++;
                TotalLayersToLoad += 1.0;

                StatusBar.Text = currentStatusMessage + ": " + "0.0%";
                StatusBar.UpdateLayout();

                tracingLayer.Initialized += new EventHandler<EventArgs>(tracingLayer_Initialized);
                tracingLayer.InitializationFailed += new EventHandler<EventArgs>(tracingLayer_InitializationFailed);
                tracingLayer.Initialize();
                TracingClassIDMapBuilt = true;
            }
            else
            {
                foreach (Layer layer in currentMap.Layers)
                {
                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        ArcGISDynamicMapServiceLayer dynamicMapServiceLayer = layer as ArcGISDynamicMapServiceLayer;
                        if (dynamicMapServiceLayer.Url == mapServerURL)
                        {
                            if (!ObjectClassIDMapping.ContainsKey(mapServerURL))
                            {
                                lock (LoadingClassIDsLock)
                                {
                                    StatusBar.Text = currentStatusMessage + ": " + "0.0%";
                                    StatusBar.UpdateLayout();
                                    foreach (LayerInfo layerInfo in dynamicMapServiceLayer.Layers)
                                    {
                                        try
                                        {
                                            if (layerInfo.SubLayerIds == null || layerInfo.SubLayerIds.Count() < 1)
                                            {
                                                WebClient webClient = new WebClient();
                                                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_ObjectClassIDQueryFinished);
                                                string tokenURL = dynamicMapServiceLayer.Url + "/exts/ArcFMMapServer/id/" + layerInfo.ID + "?f=pjson";
                                                ClassIDsStillToLoad++;
                                                TotalLayersToLoad += 1.0;
                                                webClient.DownloadStringAsync(new Uri(tokenURL), dynamicMapServiceLayer.Url);
                                            }
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        static void tracingLayer_InitializationFailed(object sender, EventArgs e)
        {
            UpdateStatusBarText("Tracing Layer intialization failed");
            TracingClassIDMapBuilt = false;
        }

        public static void UpdateStatusBarText(string status)
        {
            StatusBar.Text = status;
            StatusBar.UpdateLayout();
        }

        static void tracingLayer_Initialized(object sender, EventArgs e)
        {
            lock (LoadingClassIDsLock)
            {
                ClassIDsStillToLoad--;
                ArcGISDynamicMapServiceLayer tracingLayer = sender as ArcGISDynamicMapServiceLayer;
                if (!ObjectClassIDMapping.ContainsKey(tracingLayer.Url))
                {
                    foreach (LayerInfo layerInfo in tracingLayer.Layers)
                    {
                        try
                        {
                            if (layerInfo.SubLayerIds == null || layerInfo.SubLayerIds.Count() < 1)
                            {
                                WebClient webClient = new WebClient();
                                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_ObjectClassIDQueryFinished);
                                string tokenURL = tracingLayer.Url + "/exts/ArcFMMapServer/id/" + layerInfo.ID + "?f=pjson";
                                ClassIDsStillToLoad++;
                                TotalLayersToLoad += 1.0;
                                webClient.DownloadStringAsync(new Uri(tokenURL), tracingLayer.Url);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }

                foreach (Layer layer in CurrentMap.Layers)
                {
                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        ArcGISDynamicMapServiceLayer dynamicMapServiceLayer = layer as ArcGISDynamicMapServiceLayer;
                        if (dynamicMapServiceLayer.Url == CurrentInitLayerURL)
                        {
                            if (!ObjectClassIDMapping.ContainsKey(CurrentInitLayerURL))
                            {
                                foreach (LayerInfo layerInfo in dynamicMapServiceLayer.Layers)
                                {
                                    try
                                    {
                                        if (layerInfo.SubLayerIds == null || layerInfo.SubLayerIds.Count() < 1)
                                        {
                                            WebClient webClient = new WebClient();
                                            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_ObjectClassIDQueryFinished);
                                            string tokenURL = dynamicMapServiceLayer.Url + "/exts/ArcFMMapServer/id/" + layerInfo.ID + "?f=pjson";
                                            ClassIDsStillToLoad++;
                                            TotalLayersToLoad += 1.0;
                                            webClient.DownloadStringAsync(new Uri(tokenURL), dynamicMapServiceLayer.Url);
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                if (ClassIDsStillToLoad == 0)
                {
                    CurrentCallBack(true);
                }
            }
        }

        public static bool IsLayerVisible(string mapServerUrl, Map currentMap, int LayerID)
        {
            foreach (Layer layer in currentMap.Layers)
            {
                if (layer is ArcGISDynamicMapServiceLayer && layer.Visible)
                {
                    ArcGISDynamicMapServiceLayer dynamicMapLayer = layer as ArcGISDynamicMapServiceLayer;

                    if (dynamicMapLayer.Url != mapServerUrl) { continue; }

                    if (!dynamicMapLayer.GetLayerVisibility(LayerID)) { return false; }

                    return IsLayerVisible(currentMap, dynamicMapLayer, LayerID);
                }
            }
            return false;
        }

        private static bool IsLayerVisible(Map currentMap, ArcGISDynamicMapServiceLayer mapLayer, int LayerID)
        {
            foreach (LayerInfo layerInfo in mapLayer.Layers)
            {
                if (layerInfo.ID == LayerID)
                {
                    if (layerInfo.MinScale < currentMap.Scale && layerInfo.MinScale != 0)
                    {
                        return false;
                    }
                }
            }

            //Determine if parents are visible also
            foreach (LayerInfo layerInfo in mapLayer.Layers)
            {
                if (layerInfo.SubLayerIds != null && layerInfo.SubLayerIds.Contains(LayerID))
                {
                    //If the min scale is defined and less than the current map scale then it isn't visible
                    if (layerInfo.MinScale < currentMap.Scale && layerInfo.MinScale != 0)
                    {
                        return false;
                    }
                    else
                    {
                        return IsLayerVisible(currentMap, mapLayer, layerInfo.ID);
                    }
                }
            }
            //No parent layer was found so we will assume 
            return true;
        }

        private static Action<bool> CurrentCallBack = null;
        private static Object LoadingClassIDsLock = new Object();
        private static int ClassIDsStillToLoad = 0;
        private static double TotalLayersToLoad = 0;
        private static double LayersLoaded = 0;
        private static string currentStatusMessage = "";
        static void webClient_ObjectClassIDQueryFinished(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    return;
                }

                JsonValue jsonReponse = JsonObject.Parse(e.Result);
                //a.Result.Close();

                if (jsonReponse.ContainsKey("objectClassID"))
                {
                    int objectClassID = -1;
                    int layerID = -1;
                    try
                    {
                        string jsonResponseString = jsonReponse.ToString();
                        string[] jsonArray = Regex.Split(jsonResponseString, ",");

                        foreach (string item in jsonArray)
                        {
                            if (item.Contains("\"objectClassID\":"))
                            {
                                objectClassID = Int32.Parse(item.Substring(item.LastIndexOf(":") + 1));
                            }
                            else if (item.Contains("\"id\":"))
                            {
                                layerID = Int32.Parse(item.Substring(item.LastIndexOf(":") + 1));
                            }
                            if (layerID != -1 && objectClassID != -1) { break; }
                        }
                    }
                    catch { return; }

                    //Determine the URI that made this request so we can see which mapserver this layer belongs to
                    string mapserver = e.UserState as string;
                    //This response has an objectclassID so let's add it to our list.
                    lock (LoadingClassIDsLock)
                    {
                        //Now we can add this item to our dictionary
                        if (!ConfigUtility.ObjectClassIDMapping.ContainsKey(mapserver))
                        {
                            ConfigUtility.ObjectClassIDMapping.Add(mapserver, new Dictionary<int, List<int>>());
                        }
                        if (!ConfigUtility.ObjectClassIDMapping[mapserver].ContainsKey(objectClassID))
                        {
                            ConfigUtility.ObjectClassIDMapping[mapserver].Add(objectClassID, new List<int>());
                        }
                        ConfigUtility.ObjectClassIDMapping[mapserver][objectClassID].Add(layerID);
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {
                lock (LoadingClassIDsLock)
                {
                    ClassIDsStillToLoad--;
                    LayersLoaded += 1.0;
                    double percent = Math.Round((LayersLoaded / TotalLayersToLoad), 2) * 100;
                    StatusBar.Text = currentStatusMessage + ": " + percent + "%";
                    StatusBar.UpdateLayout();
                    if (ClassIDsStillToLoad == 0)
                    {
                        CurrentCallBack(true);
                    }
                }
            }
        }

        static void webClient_SchemObjectClassIDQueryFinished(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null) return;

                List<int> startingParens = new List<int>();
                List<int> endingParens = new List<int>();
                JsonValue jsonReponse = JsonObject.Parse(e.Result);
                string response = jsonReponse.ToString();

                //Parse through our response for the classIDs
                Stack<int> OpenBrackets = new Stack<int>();
                for (int i = 0; i < response.Length; i++)
                {
                    switch (response[i])
                    {
                        case '{':
                            OpenBrackets.Push(i);
                            break;
                        case '}':
                            startingParens.Add(OpenBrackets.Pop());
                            endingParens.Add(i);
                            break;
                        default:
                            break;
                    }
                }

                Dictionary<int, int> matchingParensDict = new Dictionary<int, int>();

                for (int i = 0; i < startingParens.Count; i++)
                {
                    matchingParensDict.Add(startingParens[i], endingParens[i]);
                }

                startingParens.Sort();
                endingParens.Clear();
                for (int i = 0; i < startingParens.Count; i++)
                {
                    endingParens.Add(matchingParensDict[startingParens[i]]);
                }

                List<string> matchedSections = new List<string>();
                //Build the sections with substrings
                for (int i = 0; i < startingParens.Count; i++)
                {
                    int startingParen = startingParens[i];
                    int endingParen = endingParens[i];
                    int increment = 1;
                    if (((i + 1) < startingParens.Count) && endingParen > startingParens[i + 1])
                    {
                        increment = 0;
                        endingParen = startingParens[i + 1];
                    }

                    matchedSections.Add(response.Substring(startingParen, endingParen - startingParen + increment));
                }

                Dictionary<int, List<int>> oidToLayerMap = new Dictionary<int, List<int>>();
                foreach (string layerProperty in matchedSections)
                {
                    if (layerProperty.Contains("\"featureClassID\""))
                    {
                        int id = -1;
                        int featClassID = -1;
                        string input = layerProperty.Replace("{", "").Replace("}", "");
                        string[] layers = Regex.Split(input, ",");
                        for (int j = 0; j < layers.Length; j++)
                        {
                            string[] values = Regex.Split(layers[j], ":");

                            for (int i = 0; i < values.Length; i++)
                            {
                                if (values[i] == "\"id\"")
                                {
                                    id = Int32.Parse(values[i + 1]);
                                }
                                else if (values[i] == "\"featureClassID\"")
                                {
                                    featClassID = Int32.Parse(values[i + 1]);
                                }
                            }
                        }
                        if (id == -1 || featClassID == -1) { continue; }
                        if (!oidToLayerMap.ContainsKey(featClassID))
                        {
                            oidToLayerMap.Add(featClassID, new List<int>());
                        }
                        oidToLayerMap[featClassID].Add(id);
                    }
                }

                //Determine the URI that made this request so we can see which mapserver this layer belongs to
                string mapserver = e.UserState as string;

                //This response has an objectclassID so let's add it to our list.
                lock (LoadingClassIDsLock)
                {
                    foreach (KeyValuePair<int, List<int>> kvp in oidToLayerMap)
                    {
                        //Now we can add this item to our dictionary
                        if (!ConfigUtility.ObjectClassIDMapping.ContainsKey(mapserver))
                        {
                            ConfigUtility.ObjectClassIDMapping.Add(mapserver, new Dictionary<int, List<int>>());
                        }
                        if (!ConfigUtility.ObjectClassIDMapping[mapserver].ContainsKey(kvp.Key))
                        {
                            ConfigUtility.ObjectClassIDMapping[mapserver].Add(kvp.Key, new List<int>());
                        }
                        ConfigUtility.ObjectClassIDMapping[mapserver][kvp.Key] = kvp.Value;
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {
                lock (LoadingClassIDsLock)
                {
                    CurrentCallBack(true);
                }
            }
        }

        public static string GeometryServiceURL { get; set; }

        public static string BingKey { get; set; }

        public static SearchItem SearchItemFromXElement(XElement element)
        {
            const string addressSearchType = "AddressSearch";
            const string bingAddressSearchType = "BingAddressSearch";
            const string latlongSearchType = "LatLongSearch";
            const string cgcSearch = "CGCSearch";
            const string pgeSearch = "PGESearch";
            const string pgeSearchMm = "PGESearchMM";
            const string transformerSearch = "TransformerSearch";

            List<TableSearch> tableSearchList = new List<TableSearch>();
            if (element.ToString().Contains("SearchTable")) tableSearchList = new List<TableSearch>();

            if (element == null) return null;

            string elementName = element.Name.LocalName;

            SearchItem item;
            if (elementName == addressSearchType)
            {
                item = new CustomAddressSearch(new LocateTask()); //item = new AddressSearch();
            }
            else if (elementName == bingAddressSearchType)
            {
                item = new BingAddressSearch();
            }
            else if (elementName == latlongSearchType)
            {
                item = new CustomLatLongSearch(new LocateTask());

            }
            else if (elementName == cgcSearch)
            {
                item = new CustomCGCSearch(new LocateTask());
            }
            else if (elementName == pgeSearch)
            {
                item = new PGESearch(new LocateTask());
            }
            else if (elementName == pgeSearchMm)
            {
                item = new PGESearchMM(new LocateTask());
            }
            else if (elementName == transformerSearch)
            {
                item = new TransformerSearch(new LocateTask());
            }
            else
            {
                item = new Search();
            }

            // Parse common attributes
            var attribute = element.Attribute("Title");
            if (attribute != null)
            {
                item.Title = attribute.Value;
            }

            attribute = element.Attribute("Description");
            if (attribute != null)
            {
                item.Description += attribute.Value;
            }

            attribute = element.Attribute("MaxRecords");
            if (attribute != null)
            {
                int maxRecords;
                if (int.TryParse(attribute.Value, out maxRecords))
                {
                    item.MaxRecords = maxRecords;
                }
            }

            attribute = element.Attribute("Type");
            if (attribute != null)
            {
                if ((attribute.Value.Length > 0) && (attribute.Value == "Custom"))
                {
                    item.IsCustom = true;
                }
            }

            if (item is Search || item is PGESearch || item is PGESearchMM)
            {
                if (element.HasElements)
                {
                    foreach (var search in element.Elements())
                    {
                        if (search.Name == "SearchLayer")
                        {
                            var searchLayer = SearchLayerFromXElement(search);
                            if (searchLayer != null)
                                item.SearchLayers.Add(searchLayer);
                        }
                        else if (search.Name == "SearchTable")
                        {
                            TableSearch tableSearch = SearchTableFromXElement(search);
                            if (tableSearch != null)
                            {
                                tableSearch.Title = item.Title;
                                tableSearch.Description = item.Description;
                                tableSearch.MaxRecords = item.MaxRecords;
                                if (tableSearchList != null) tableSearchList.Add(tableSearch);
                                tableSearch.TableSearchCollction = tableSearchList;
                                tableSearch.IsCustom = item.IsCustom;
                                item = tableSearch;
                            }
                        }
                        else if (search.Name == "SearchService")
                        {
                            ServiceSearch serviceSearch = SearchServiceFromXElement(search);
                            if (serviceSearch != null)
                            {
                                serviceSearch.Title = item.Title;
                                serviceSearch.Description = item.Description;
                                serviceSearch.MaxRecords = item.MaxRecords;
                                serviceSearch.IsCustom = item.IsCustom;
                                item = serviceSearch;
                            }
                        }
                    }
                }
            }

            if (item is AddressSearch)
            {
                AddressSearch tempItem = (AddressSearch)item;
                attribute = element.Attribute("AddressScore");
                if (attribute != null)
                {
                    int addressScore;
                    if (int.TryParse(attribute.Value, out addressScore))
                    {
                        tempItem.AddressScore = addressScore;
                    }
                }

                attribute = element.Attribute("Url");
                if (attribute != null)
                {
                    tempItem.Url = attribute.Value;
                }

                attribute = element.Attribute("Fields");
                if (attribute != null)
                {
                    tempItem.Fields = attribute.Value;
                }

                attribute = element.Attribute("AddressDefault");
                if (attribute != null)
                {
                    tempItem.AddressDefault = attribute.Value;
                }

                attribute = element.Attribute("ToolTip");
                if (attribute != null)
                {
                    tempItem.ToolTip = attribute.Value;
                }
            }


            if (item is CustomAddressSearch)
            {
                CustomAddressSearch tempItem = (CustomAddressSearch)item;

                attribute = element.Attribute("AddressScore");
                if (attribute != null)
                {
                    int addressScore;
                    if (int.TryParse(attribute.Value, out addressScore))
                    {
                        tempItem.AddressScore = addressScore;
                    }
                }

                attribute = element.Attribute("Url");
                if (attribute != null)
                {
                    tempItem.Service = attribute.Value;
                    LocatorService = attribute.Value;
                }

                attribute = element.Attribute("Fields");
                if (attribute != null)
                {
                    tempItem.Fields = attribute.Value;
                }

            }

            if (item is CustomLatLongSearch)
            {
                CustomLatLongSearch tempItem = (CustomLatLongSearch)item;
                attribute = element.Attribute("Url");
                if (attribute != null)
                {
                    tempItem.Service = attribute.Value;
                }

                if (Application.Current.Host.InitParams.ContainsKey("Config"))
                {
                    string config = Application.Current.Host.InitParams["Config"];
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement elements = XElement.Parse(config);
                    XElement searchExtentElement = null;

                    foreach (XElement ele in elements.Elements())
                    {
                        if (ele.Name.LocalName == "SearchExtents")
                        {
                            searchExtentElement = ele;
                            break;
                        }
                    }

                    foreach (XElement extentElement in searchExtentElement.Elements())
                    {
                        if (extentElement.Name.LocalName == "LatLongExtent")
                        {
                            tempItem.LatLongExtent = ConfigUtility.EnvelopeFromXElement(extentElement, "Extent");
                        }
                        else if (extentElement.Name.LocalName == "XYExtent")
                        {
                            tempItem.XYExtent = ConfigUtility.EnvelopeFromXElement(extentElement, "Extent");
                        }
                    }
                }

            }

            if (item is CustomCGCSearch)
            {
                CustomCGCSearch tempItem = (CustomCGCSearch)item;

                attribute = element.Attribute("ClassId");
                if (attribute != null)
                {
                    //tempItem.ClassID = Convert.ToInt16(attribute.Value);
                    tempItem.ClassID = Convert.ToString(attribute.Value);           //ME Q4 2019 - DA# 190906
                }

            }

            if (item is TransformerSearch)
            {
                TransformerSearch tempItem = (TransformerSearch)item;
                tempItem.custAddFields = element.Attribute("CustFields").Value.ToString();
                tempItem.relationField = element.Attribute("RelationField").Value.ToString();

                if (element.HasElements)
                {
                    foreach (var search in element.Elements())
                    {
                        if (search.Name == "SearchCustAddLayer")
                        {
                            var searchLayer = SearchLayerFromXElement(search);
                            if (searchLayer != null)
                                tempItem.SearchCustAddLayer = searchLayer;
                        }
                        else if (search.Name == "SearchLayer")
                        {
                            var searchLayer = SearchLayerFromXElement(search);
                            if (searchLayer != null)
                                tempItem.SearchTransLayer = searchLayer;
                        }
                    }
                }

            }

            if (item is BingAddressSearch)
            {
                BingAddressSearch bingItem = (BingAddressSearch)item;
                if (string.IsNullOrEmpty(BingKey) == false)
                {
                    bingItem.Key = BingKey;
                }

                attribute = element.Attribute("MinimumConfidence");
                if (attribute != null)
                {
                    Confidence conf;
                    if (Confidence.TryParse(attribute.Value, true, out conf) == true)
                    {
                        bingItem.MinConfidence = conf;
                    }
                    else
                    {
                        bingItem.MinConfidence = Confidence.High;
                    }
                }
            }

            return item;
        }

        private static CustomLatLongSearch LatLongServiceFromXElement(XElement latlong)
        {
            if (latlong == null) return null;
            ILocateTask locate = new LocateTask();
            var latlongService = new CustomLatLongSearch(locate);
            XAttribute attribute = latlong.Attribute("Url");
            if (attribute != null) latlongService.Service = attribute.Value;
            return latlongService;
        }

        private static ServiceSearch SearchServiceFromXElement(XElement search)
        {
            if (search == null) return null;
            ILocateTask locate = new LocateTask();
            var searchService = new ServiceSearch(locate);
            XAttribute attribute = search.Attribute("Url");
            if (attribute != null) searchService.Service = attribute.Value;

            attribute = search.Attribute("LayerIds");
            if (attribute != null) searchService.Layers = attribute.Value;

            attribute = search.Attribute("Fields");
            if (attribute != null) searchService.Fields = attribute.Value;

            return searchService;
        }

        private static TableSearch SearchTableFromXElement(XElement element)
        {
            if (element == null) return null;
            ILocateTask locate = new LocateTask();
            var search = new TableSearch(locate);

            XAttribute attribute = element.Attribute("Url");
            if (attribute != null) search.Service = attribute.Value;

            attribute = element.Attribute("ID");
            if (attribute != null)
            {
                int tableIndex;
                if (int.TryParse(attribute.Value, out tableIndex))
                {
                    search.TableIndex = tableIndex;
                }
            }

            attribute = element.Attribute("LayerType");
            if (attribute != null) search.layerType = attribute.Value;

            attribute = element.Attribute("Fields");
            if (attribute != null) search.Fields = attribute.Value;

            search.isExactMatch = true;

            return search;
        }

        public static SearchLayer SearchLayerFromXElement(XElement element)
        {
            if (element == null) return null;
            SearchLayer layer = new SearchLayer();

            XAttribute attribute = element.Attribute("Url");
            if (attribute != null)
            {
                layer.Url = attribute.Value;
            }
            attribute = element.Attribute("ProxyUrl");
            if (attribute != null)
            {
                layer.ProxyUrl = attribute.Value;
            }
            attribute = element.Attribute("LayerId");
            if (attribute != null)
            {
                int layerID;
                if (int.TryParse(attribute.Value, out layerID))
                {
                    layer.ID = layerID;
                }
            }
            attribute = element.Attribute("Fields");
            if (attribute != null)
            {
                foreach (string field in attribute.Value.Split(','))
                {
                    layer.Fields.Add(field);
                }
            }
            if (element.HasElements)
            {
                layer.SearchRelationship = SearchRelationshipFromXElement(element.Elements().First());
            }
            return layer;
        }

        public static SearchRelationship SearchRelationshipFromXElement(XElement element)
        {
            if (element == null) return null;
            SearchRelationship relationship = new SearchRelationship();

            XAttribute attribute = element.Attribute("Table");
            if (attribute != null)
            {
                relationship.TableName = attribute.Value;
            }
            attribute = element.Attribute("Path");
            if (attribute != null)
            {
                relationship.Path = attribute.Value;
            }
            attribute = element.Attribute("Fields");
            if (attribute != null)
            {
                foreach (string field in attribute.Value.Split(','))
                {
                    relationship.Fields.Add(field);
                }
            }
            return relationship;
        }

        public static Layer LayerFromXElement(XElement element)
        {
            if (element == null) return null;

            string url = null;
            string type = null;
            string proxyUrl = null;
            string noData = null;
            string layerStyle = null;
            string key = null;
            string version = null;
            double minRes = double.NaN;
            double maxRes = double.NaN;

            bool isDisableClientCaching = false;

            var attribute = element.Attribute("Type");
            if (attribute != null)
            {
                type = attribute.Value;
            }

            attribute = element.Attribute("MinRes");
            if (attribute != null)
            {
                minRes = Double.Parse(attribute.Value);
            }

            attribute = element.Attribute("MaxRes");
            if (attribute != null)
            {
                maxRes = Double.Parse(attribute.Value);
            }

            attribute = element.Attribute("Url");
            if (attribute != null)
            {
                url = attribute.Value;
            }

            attribute = element.Attribute("ProxyUrl");
            if (attribute != null)
            {
                proxyUrl = attribute.Value;
            }

            attribute = element.Attribute("NoData");
            if (attribute != null)
            {
                noData = attribute.Value;
            }

            attribute = element.Attribute("LayerStyle");
            if (attribute != null)
            {
                layerStyle = attribute.Value;
            }

            if (string.IsNullOrEmpty(BingKey) == false)
            {
                key = BingKey;
            }

            attribute = element.Attribute("Version");
            if (attribute != null)
            {
                version = attribute.Value;
            }

            attribute = element.Attribute("DisableClientCaching");
            if (attribute != null)
            {
                bool.TryParse(attribute.Value, out isDisableClientCaching);
            }

            var layer = type == "BingTileLayer" ? CreateBingLayer(layerStyle, key) : CreateLayer(type, url, proxyUrl, noData, version, isDisableClientCaching, minRes, maxRes);
            if (layer != null)
            {
                attribute = element.Attribute("Visible");
                if (attribute != null)
                {
                    bool layerVisible;
                    if (bool.TryParse(attribute.Value, out layerVisible))
                    {
                        layer.Visible = layerVisible;
                    }
                }
                attribute = element.Attribute("MapServiceName");
                if (attribute != null)
                {
                    layer.ID = attribute.Value;
                }
                attribute = element.Attribute("Opacity");
                if (attribute != null)
                {
                    double value;
                    if (double.TryParse(attribute.Value, out value))
                        layer.Opacity = value;
                }

                if (layer is FeatureLayer)
                {
                    attribute = element.Attribute("AutoSave");
                    if (attribute != null)
                    {
                        bool autoSave;
                        if (bool.TryParse(attribute.Value, out autoSave))
                        {
                            ((FeatureLayer)layer).AutoSave = autoSave;
                        }
                    }
                    attribute = element.Attribute("OutFields");
                    if (attribute != null)
                    {
                        char[] sep = { ',' };
                        var values = attribute.Value.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string value in values)
                        {
                            ((FeatureLayer)layer).OutFields.Add(value);
                        }
                    }
                    attribute = element.Attribute("Mode");
                    if (attribute != null)
                    {
                        if (attribute.Value != null)
                        {
                            if (attribute.Value.Equals("OnDemand"))
                            {
                                ((FeatureLayer)layer).Mode = FeatureLayer.QueryMode.OnDemand;
                            }
                        }
                    }

                }

            }
            return layer;
        }

        public static void WebmapDocFromXElement(XElement webmap, out Document doc, out string mapID, out bool privateMap, ref string tokenServerUrl, ref bool visible)
        {
            privateMap = false;
            doc = null;
            mapID = null;

            var attribute = webmap.Attribute("ID");
            if (attribute == null)
            {
                return;
            }
            mapID = attribute.Value;
            doc = new Document();

            attribute = webmap.Attribute("ProxyUrl");
            if (attribute != null)
            {
                doc.ProxyUrl = attribute.Value;
            }

            attribute = webmap.Attribute("ServerBaseUrl");
            if (attribute != null)
            {
                doc.ServerBaseUrl = attribute.Value;
            }

            attribute = webmap.Attribute("Private");
            if (attribute != null)
            {
                bool.TryParse(attribute.Value, out privateMap);
            }

            attribute = webmap.Attribute("TokenServerUrl");
            if (attribute != null && attribute.Value.Trim() != string.Empty)
            {
                tokenServerUrl = attribute.Value.Trim();
            }

            attribute = webmap.Attribute("Visible");
            if (attribute != null)
            {
                bool temp;
                if (bool.TryParse(attribute.Value, out temp))
                    visible = temp;
            }

            if (string.IsNullOrEmpty(BingKey) == false)
            {
                doc.BingToken = BingKey;
            }

        }

        public static void WebmapDocFromXElement(XElement webmap, out Document doc, out string mapID, out bool privateMap, ref string tokenServerUrl, ref bool visible, out string mapDisplayName)
        {
            privateMap = false;
            doc = null;
            mapID = null;
            mapDisplayName = null;

            var attribute = webmap.Attribute("ID");
            if (attribute == null)
            {
                return;
            }
            mapID = attribute.Value;
            doc = new Document();

            attribute = webmap.Attribute("DisplayName");
            if (attribute != null)
            {
                mapDisplayName = attribute.Value;
            }

            attribute = webmap.Attribute("ProxyUrl");
            if (attribute != null)
            {
                doc.ProxyUrl = attribute.Value;
            }

            attribute = webmap.Attribute("ServerBaseUrl");
            if (attribute != null)
            {
                doc.ServerBaseUrl = attribute.Value;
            }

            attribute = webmap.Attribute("Private");
            if (attribute != null)
            {
                bool.TryParse(attribute.Value, out privateMap);
            }

            attribute = webmap.Attribute("TokenServerUrl");
            if (attribute != null && attribute.Value.Trim() != string.Empty)
            {
                tokenServerUrl = attribute.Value.Trim();
            }

            attribute = webmap.Attribute("Visible");
            if (attribute != null)
            {
                bool temp;
                if (bool.TryParse(attribute.Value, out temp))
                    visible = temp;
            }

            if (string.IsNullOrEmpty(BingKey) == false)
            {
                doc.BingToken = BingKey;
            }

        }

        public static RelationshipInformation RelatedDataFromXElement(XElement element)
        {
            if (element == null) return null;

            RelationshipInformation data = new RelationshipInformation();

            XAttribute attribute = element.Attribute("LayerId");
            if (attribute != null)
            {
                int layerID;
                if (int.TryParse(attribute.Value, out layerID))
                {
                    data.LayerId = layerID;
                }
            }
            attribute = element.Attribute("Url");
            if (attribute != null)
            {
                data.Service = attribute.Value;
            }
            attribute = element.Attribute("ProxyUrl");
            if (attribute != null)
            {
                data.ProxyUrl = attribute.Value;
            }
            attribute = element.Attribute("RelationshipIds");
            if (attribute != null)
            {
                foreach (string id in attribute.Value.Split(','))
                {
                    int relId;
                    if (int.TryParse(id, out relId))
                    {
                        data.RelationshipIds.Add(relId);
                    }
                }
            }

            //*************ENOS2EDGIS Start*******************
            //ENOS2EDGIS, in page.config, we do have Alias names for relationship, so we are utlising them there
            attribute = element.Attribute("RelationshipAlias");
            if (attribute != null)
            {
                foreach (string id in attribute.Value.Split(','))
                {
                    data.RelationshipNames.Add(id);
                }
            }
            //*************ENOS2EDGIS End**********************
            return data;
        }

        public static Envelope EnvelopeFromXElement(XElement element, string attributeName)
        {
            if (element == null) return null;
            if (string.IsNullOrEmpty(attributeName)) return null;

            XAttribute attribute = element.Attribute(attributeName);
            if (attribute == null) return null;

            List<double> corners = new List<double>();
            foreach (string curString in attribute.Value.Split(','))
            {
                double corner;
                if (double.TryParse(curString, out corner))
                {
                    corners.Add(corner);
                }
            }

            if (corners.Count == 4)
            {
                return new Envelope(corners[0], corners[1], corners[2], corners[3]);
            }

            return null;
        }

        public static string GetExtentLayer(XElement element)
        {
            XAttribute attribute = element.Attribute("ExtentLayer");
            if (attribute != null)
            {
                return attribute.Value;
            }
            return string.Empty;
        }

        private static Layer CreateLayer(string type, string url, string proxyUrl, string noData, string version, bool isDisableClientCaching, double minRes, double maxRes)
        {
            switch (type)
            {
                case "ArcGISDynamicMapServiceLayer":
                    return new ArcGISDynamicMapServiceLayer { MinimumResolution = minRes, MaximumResolution = maxRes, Url = url, ProxyURL = proxyUrl, DisableClientCaching = isDisableClientCaching };
                case "ArcGISTiledMapServiceLayer":
                    return new ArcGISTiledMapServiceLayer { MinimumResolution = minRes, MaximumResolution = maxRes, Url = url, ProxyURL = proxyUrl };
                case "ArcGISImageServiceLayer":
                    ArcGISImageServiceLayer imageServiceLayer = new ArcGISImageServiceLayer { MinimumResolution = minRes, MaximumResolution = maxRes, Url = url, ProxyURL = proxyUrl, DisableClientCaching = isDisableClientCaching };
                    if (string.IsNullOrEmpty(noData) == false)
                    {
                        imageServiceLayer.ImageFormat = ArcGISImageServiceLayer.ImageServiceImageFormat.JPGPNG;
                        imageServiceLayer.NoData = Convert.ToDouble(noData);
                    }
                    return imageServiceLayer;
                case "FeatureLayer":
                    return new FeatureLayer { MinimumResolution = minRes, MaximumResolution = maxRes, Url = url, ProxyUrl = proxyUrl, DisableClientCaching = isDisableClientCaching };
                case "WmsLayer":
                    return new WmsLayer { MinimumResolution = minRes, MaximumResolution = maxRes, Url = url, ProxyUrl = proxyUrl, Version = version };
            }
            return null;
        }

        private static Layer CreateBingLayer(string layerStyle, string token)
        {
            var layer = new TileLayer
            {
                Token = token
            };

            TileLayer.LayerType layerType;
            Enum.TryParse<TileLayer.LayerType>(layerStyle, true, out layerType);
            if (layerType != null)
            {
                layer.LayerStyle = layerType;
            }

            return layer;
        }

        /// <summary>
        /// Checks whether an XElement object is secured and allowed to be used.  Looks for a 'GroupFilter' attribute.
        /// If one does not exist, the item is considered an unsecure item and returns true.
        /// If one does exist, the priciple is check to see whether the user is in on of the specified groups.
        /// </summary>
        /// <param name="element">the element to check for security</param>
        /// <returns>whether the item is allowed for the user</returns>
        public static bool IsItemAllowed(XElement element)
        {
            XAttribute attribute = element.Attribute("GroupFilter");

            if (attribute != null)
            {
                if (WebContext.Current == null) throw new Exception("The web context was not initialized.");
                if (WebContext.Current.User == null) throw new Exception("The user cannot be determined.");
                if (!WebContext.Current.User.IsAuthenticated) throw new Exception("The user " + WebContext.Current.User.Name + " is not authenticated.");

                string groups = attribute.Value;
                string[] groupArray = groups.Split(',');
                //TODO: uncomment
                //return true;
                return groupArray.Contains("Everyone") || groupArray.Any(s => WebContext.Current.User.IsInRole(s));
            }

            return true;

        }
    }
}
