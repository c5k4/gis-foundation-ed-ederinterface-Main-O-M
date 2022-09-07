using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using System.Collections.Generic;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using System.Xml.Linq;
using Miner.Server.Client.Query;
using Miner.Server.Client.Tasks;
using Miner.Server.Client.Toolkit;
using System.ComponentModel;
using Miner.Server.Client;
using System.Collections.ObjectModel;

namespace ArcFMSilverlight.Controls.GenerationOnFeeder
{
    public class GetGenOnFeeder
    {
        private List<Graphic> generations = new List<Graphic>();
        private List<Graphic> servicePointsOnTransformer = new List<Graphic>();
        private int servicePointFeatureCount;
        private static string _serviceLocationURL;
        private static IList<string> _genFeederLayerName = new List<string>();
        private static string _servicePointURL;
        private static string _generationInfoURL;
        private static Grid _mapArea;
        private static string _deviceSettingURL;
        private static string _popUpTitle;
        private IEnumerable<IResultSet> Results { get; set; }
        private string _searchType = string.Empty;
        public static string feederZoomGraphicID = "FeederZoomID";
        public IList<Graphic> generationOnFeederData = new List<Graphic>();


        public static IList<string> SearchLayers = new List<string>();

        private SimpleMarkerSymbol _markerSymbol;

        public static GraphicsLayer _graphicsLayer;
        private static Map _map;
        private bool isCalledFromTool = false;
        private FEEDERToolPage _feederToolPage;
        private FeederFromMap _feederFromMap;
        //  private GenerationOnTransformer _GenerationOnTransformer;
        private string _circuitID;
        private ENOSFEEDERTool _enosFEEDERTool;
        private MapTools _mapTools;
        /// <summary>
        /// Fires when the locate starts.
        /// </summary>


        /// <summary>
        /// Fires when the locate ends.
        /// </summary>


        static public void ReadConfiguration(XElement element, Grid MapArea, string DeviceSettingURL, Map map)
        {
            foreach (XElement layerElement in element.Element("Layers").Elements())
            {
                _genFeederLayerName.Add(layerElement.Attribute("LayerName").Value);
            }
            _serviceLocationURL = element.Element("ServiceLocation").Attribute("URL").Value;
            _servicePointURL = element.Element("ServicePoint").Attribute("URL").Value;
            _generationInfoURL = element.Element("GenerationInfo").Attribute("URL").Value;
            _mapArea = MapArea;
            _deviceSettingURL = DeviceSettingURL;
            // _popUpTitle = element.Element("PopUp").Attribute("Title").Value;
            _map = map;
        }


        public static IList<string> GenFeederLayerName
        {

            get
            {
                return _genFeederLayerName;
            }
        }


        public void genGenOnFeeder_Click(string circuitId)
        {
            // _circuitID = circuitId;
            //_feederToolPage = new FEEDERToolPage(_map, _mapTools, "", _enosFEEDERTool);
            // _feederFromMap = new FeederFromMap(_map, _mapArea, _deviceSettingURL);
            ConfigUtility.UpdateStatusBarText("Loading Generation on Feeder...");
            _feederFromMap = new FeederFromMap(_mapArea);
            _feederFromMap.GendatafromMapFeederdata(circuitId);


            //if (circuitId != null)
            //{
            //    ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query(); //query on service point layer          
            //    query.OutFields.AddRange(new string[] { "GLOBALID", "CGC12" });
            //    query.Where = "CIRCUITID='" + circuitId + "'";
            //    QueryTask queryTask = new QueryTask(_serviceLocationURL);
            //    queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(slQueryTask_ExecuteCompleted);
            //    queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(slQueryTask_ExecuteFailed);
            //    queryTask.ExecuteAsync(query);
            //}
        }

        void slQueryTask_ExecuteFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            //_feederToolPage = new FEEDERToolPage(_map, _mapTools, "", _enosFEEDERTool);
            ConfigUtility.UpdateStatusBarText("");
            _feederToolPage.ClearGrid();
        }
        void slQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            string whereInString = string.Empty;
            int iCount = 0;
            if ((e.FeatureSet).Features.Count > 0)
            {
                foreach (var feature in (e.FeatureSet).Features)
                {
                    if (iCount == 0)
                        whereInString = "'" + feature.Attributes["GLOBALID"] + "'";
                    else
                        whereInString = whereInString + ",'" + feature.Attributes["GLOBALID"] + "'";

                    iCount++;
                }

                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query(); //query on service point layer          
                query.OutFields.AddRange(new string[] { "GLOBALID", "SERVICEPOINTID", "STREETNUMBER", "STREETNAME1", "STREETNAME2", "STATE", "CITY", "COUNTY", "SERVICELOCATIONGUID", "TRANSFORMERGUID", "PRIMARYMETERGUID", "METERNUMBER", "CGC12" }); //Adding City to Address 
                query.Where = "SERVICELOCATIONGUID IN (" + whereInString + ")";
                QueryTask queryTask = new QueryTask(_servicePointURL);
                queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(spQueryTask_ExecuteCompleted);
                queryTask.ExecuteAsync(query);

            }
            else
            {
                ConfigUtility.UpdateStatusBarText("");
                _feederToolPage.ClearGrid();
                //ENOSChangeFixes
                MessageBox.Show("No Generation found.", "No Generations Found", MessageBoxButton.OK);
            }
        }

        void spQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            if ((e.FeatureSet).Features.Count > 0)
            {
                // genrationQueryCounter = 0;
                string _servicePtGlobalID = string.Empty;
                servicePointFeatureCount = (e.FeatureSet).Features.Count;

                for (int i = 0; i < (e.FeatureSet).Features.Count; i++)
                {
                    _servicePtGlobalID += "'" + ((e.FeatureSet).Features[i].Attributes)["GLOBALID"].ToString() + "',";

                    servicePointsOnTransformer.Add((e.FeatureSet).Features[i]);
                }
                _servicePtGlobalID = _servicePtGlobalID.Substring(0, _servicePtGlobalID.Length - 1);
                getGenerationInfo(_servicePtGlobalID);

            }
            else
            {
                ConfigUtility.UpdateStatusBarText("");
                _feederToolPage.ClearGrid();
                //ENOSChangeFixes
                MessageBox.Show("No Generation found.", "No Generations Found", MessageBoxButton.OK);
            }
        }

        void getGenerationInfo(string _servicePtGlobalID)
        {
            FeatureLayer pFeatureLayer = new FeatureLayer();
            pFeatureLayer.Url = _generationInfoURL;
            pFeatureLayer.Initialized += new EventHandler<EventArgs>(GenInfoFeatureLayer_Initialized);
            pFeatureLayer.Initialize();

            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query(); //query on generation info layer
            query.ReturnGeometry = true;
            query.OutFields.AddRange(new string[] { "*" });
            query.Where = "SERVICEPOINTGUID IN (" + _servicePtGlobalID + ")";
            QueryTask queryTask = new QueryTask(_generationInfoURL);
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(genInfoQueryTask_ExecuteCompleted);
            queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(genInfoQueryTask_ExecuteFailed);
            queryTask.ExecuteAsync(query);
        }

        private void GenInfoFeatureLayer_Initialized(object sender, System.EventArgs e)
        {
            FeatureLayer _featureLayer = sender as FeatureLayer;
            FeatureLayerInfo pLayerInfo = _featureLayer.LayerInfo;
            foreach (ESRI.ArcGIS.Client.Field field in pLayerInfo.Fields)
            {
                if (field.Domain != null)
                {
                    if (field.Name.ToUpper() == "GENTYPE")
                    {
                        if (field.Domain is ESRI.ArcGIS.Client.FeatureService.CodedValueDomain)
                        {
                            ConfigUtility.DivisionCodedDomains = field.Domain as ESRI.ArcGIS.Client.FeatureService.CodedValueDomain;
                        }
                    }
                    /****************************ENOS Tariff Change- Start***************************/
                    if (field.Name.ToUpper() == "METHODOFLIMITEDEXPORT")
                    {
                        if (field.Domain is ESRI.ArcGIS.Client.FeatureService.CodedValueDomain)
                        {
                            ConfigUtility.GenInfoLimitedExportCodedDomains = field.Domain as ESRI.ArcGIS.Client.FeatureService.CodedValueDomain;
                        }
                    }
                    /****************************ENOS Tariff Change- End****************************/
                }
            }
        }

        void genInfoQueryTask_ExecuteFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.DivisionCodedDomains = null;
            ConfigUtility.UpdateStatusBarText("");
            ConfigUtility.GenInfoLimitedExportCodedDomains = null; //ENOS Tariff Change
        }

        void genInfoQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            // int isGeneration = 0;
            if ((e.FeatureSet).Features.Count > 0)
            {
                for (int i = 0; i < (e.FeatureSet).Features.Count; i++)
                {
                    generations.Add((e.FeatureSet).Features[i]);
                }
            }
            if (generations.Count > 0)
            {
                for (int j = 0; j < generations.Count; j++)
                {

                    for (int i = 0; i < servicePointsOnTransformer.Count; i++)
                    {

                        if (generations[j].Attributes["SERVICEPOINTGUID"].ToString().Replace("}", "").Replace("{", "").ToUpper() == servicePointsOnTransformer[i].Attributes["GLOBALID"].ToString().Replace("}", "").Replace("{", "").ToUpper())
                        {
                            generations[j].Attributes["GENTYPE"] = ConfigUtility.DivisionCodedDomains.CodedValues[generations[j].Attributes["GENTYPE"].ToString()];
                            double genSize = 0;
                            double machineKW = 0;
                            double inverterKW = 0;
                            if (generations[j].Attributes["EFFRATINGMACHKW"] != null && generations[j].Attributes["EFFRATINGINVKW"] != null)
                            {
                                machineKW = Convert.ToDouble(generations[j].Attributes["EFFRATINGMACHKW"]);
                                inverterKW = Convert.ToDouble(generations[j].Attributes["EFFRATINGINVKW"]);
                            }
                            else if (generations[j].Attributes["EFFRATINGMACHKW"] == null && generations[j].Attributes["EFFRATINGINVKW"] != null)
                            {
                                inverterKW = Convert.ToDouble(generations[j].Attributes["EFFRATINGINVKW"]);
                            }
                            else if (generations[j].Attributes["EFFRATINGMACHKW"] != null && generations[j].Attributes["EFFRATINGINVKW"] == null)
                            {
                                machineKW = Convert.ToDouble(generations[j].Attributes["EFFRATINGMACHKW"]);
                            }
                            genSize = machineKW + inverterKW;
                            generations[j].Attributes["GENSIZE"] = genSize.ToString();
                            generations[j].Attributes["PROJECT/REF"] = generations[j].Attributes["SAPEGINOTIFICATION"];
                            generations[j].Attributes["GENGLOBALID"] = generations[j].Attributes["GLOBALID"];
                            //Adding City to Address 
                            generations[j].Attributes["CITY"] = servicePointsOnTransformer[i].Attributes["CITY"];
                            generations[j].Attributes["SERVICEPOINTID"] = servicePointsOnTransformer[i].Attributes["SERVICEPOINTID"];
                            generations[j].Attributes["SERVICELOCATIONGUID"] = servicePointsOnTransformer[i].Attributes["SERVICELOCATIONGUID"];
                            generations[j].Attributes["TRANSFORMERGUID"] = servicePointsOnTransformer[i].Attributes["TRANSFORMERGUID"];
                            generations[j].Attributes["PRIMARYMETERGUID"] = servicePointsOnTransformer[i].Attributes["PRIMARYMETERGUID"];
                            generations[j].Attributes["STREETNUMBER"] = servicePointsOnTransformer[i].Attributes["STREETNUMBER"];
                            generations[j].Attributes["STREETNAME1"] = servicePointsOnTransformer[i].Attributes["STREETNAME1"];
                            generations[j].Attributes["STATE"] = servicePointsOnTransformer[i].Attributes["STATE"];
                            generations[j].Attributes["METERNUMBER"] = servicePointsOnTransformer[i].Attributes["METERNUMBER"];
                            generations[j].Attributes["CGC12"] = servicePointsOnTransformer[i].Attributes["CGC12"];
                            generations[j].Attributes["FEEDERNUMBER"] = _circuitID;

                            /****************************ENOS Tariff Change- Start****************************/
                            generations[i].Attributes["DERATED"] = servicePointsOnTransformer[j].Attributes["DERATED"];
                            if (generations[j].Attributes["METHODOFLIMITEDEXPORT"] != null)
                            {
                                generations[j].Attributes["METHODOFLIMITEDEXPORT"] = ConfigUtility.DivisionCodedDomains.CodedValues[generations[j].Attributes["METHODOFLIMITEDEXPORT"].ToString()];
                            }
                            else
                            {
                                generations[i].Attributes["METHODOFLIMITEDEXPORT"] = servicePointsOnTransformer[j].Attributes["METHODOFLIMITEDEXPORT"];
                            }
                            /****************************ENOS Tariff Change- End****************************/
                        }
                    }

                }
            }
            //showGenInfoData(servicePointsOnTransformer);


            // showGenInfoData(generations);
            servicePointsOnTransformer.Clear();
            ConfigUtility.DivisionCodedDomains = null;

        }

        public void getGenOnFeederMethod(FEEDERToolPage FEEDERToolPage, string _selectedCircuitId, bool calledFromTool)
        {
            _feederToolPage = FEEDERToolPage;
            isCalledFromTool = calledFromTool;
            // genGenOnFeeder_Click(_selectedCircuitId);

        }

        public void showGenInfoData()
        {

            /****************************ENOS2SAP PhaseIII Start****************************/
            GenOnTransformerTool genOnTransformer = new GenOnTransformerTool(_mapArea, generations, _deviceSettingURL, false, _map);
            genOnTransformer._generations = servicePointsOnTransformer;
            genOnTransformer._cgcNumber = "NA";
            //genOnTransformer.OpenDialog("NA"); // ENOSTOEDGIS - Changes for adding CGC to caption 
            /****************************ENOS2SAP PhaseIII End****************************/

        }


        public void LocateServicePoint(string SPID, string SLGUID, string TranGUID, string PMGUID)
        {
            if (SLGUID != "NA")
            {
                ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query(); //query on generation info layer
                query.ReturnGeometry = true;
                query.OutFields.AddRange(new string[] { "*" });
                query.Where = "GLOBALID = '{" + SLGUID + "}'";
                QueryTask queryTask = new QueryTask(SearchLayers[0]);
                queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(LocateQueryTask_ExecuteCompleted);
                //queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(LocatteQueryTask_ExecuteFailed);
                queryTask.ExecuteAsync(query);
            }
            else
            {

                if (TranGUID != "NA")
                {
                    ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query(); //query on generation info layer
                    query.ReturnGeometry = true;
                    query.OutFields.AddRange(new string[] { "*" });
                    query.Where = "GLOBALID = '{" + TranGUID + "}'";
                    QueryTask queryTask = new QueryTask(SearchLayers[1]);
                    queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(LocateQueryTask_ExecuteCompleted);
                    // queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(genInfoQueryTask_ExecuteFailed);
                    queryTask.ExecuteAsync(query);
                }
                else
                {
                    ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query(); //query on generation info layer
                    query.ReturnGeometry = true;
                    query.OutFields.AddRange(new string[] { "*" });
                    query.Where = "GLOBALID = '{" + PMGUID + "}'";
                    QueryTask queryTask = new QueryTask(SearchLayers[2]);
                    queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(LocateQueryTask_ExecuteCompleted);
                    // queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(genInfoQueryTask_ExecuteFailed);
                    queryTask.ExecuteAsync(query);
                }
            }
        }

        void LocateQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            FeatureSet FeatureSet = e.FeatureSet;
            if (FeatureSet.Features.Count > 0)
            {
                ESRI.ArcGIS.Client.Geometry.Geometry selectedGeom = FeatureSet.Features[0].Geometry;
                var feature = FeatureSet.Features[0];
                Envelope envelope = new Envelope();

                envelope.XMax = e.FeatureSet.Features[0].Geometry.Extent.XMax + 200;
                envelope.XMin = e.FeatureSet.Features[0].Geometry.Extent.XMin - 200;
                envelope.YMax = e.FeatureSet.Features[0].Geometry.Extent.YMax + 200;
                envelope.YMin = e.FeatureSet.Features[0].Geometry.Extent.YMin - 200;

                _map.ZoomTo(envelope);
                AddGraphic(selectedGeom);
                ConfigUtility.UpdateStatusBarText("");
            }
            else
            {
                ConfigUtility.UpdateStatusBarText("");
                MessageBox.Show("No features found for this Service Point.");
            }

        }


        private void AddGraphic(ESRI.ArcGIS.Client.Geometry.Geometry geom)
        {
            try
            {
                GraphicsLayer layer = _map.Layers[feederZoomGraphicID] as GraphicsLayer;

                if (layer == null)
                {
                    layer = CreatePoleGraphicLayer();
                    _map.Layers.Add(layer);
                }
                layer.Visible = true;

                layer.Graphics.Clear();

                layer.Graphics.Add(new Graphic
                {
                    Symbol = new SimpleMarkerSymbol
                    {
                        Color = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255)),
                        Size = 18,
                        Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle,

                    },
                    Geometry = geom
                });

            }
            catch (Exception ex)
            {

            }
        }

        //Function to create graphic layer
        private GraphicsLayer CreatePoleGraphicLayer()
        {
            GraphicsLayer lyr = new GraphicsLayer { ID = feederZoomGraphicID };
            return lyr;
        }

    }
}
