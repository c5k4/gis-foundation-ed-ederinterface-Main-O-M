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
using System.Collections.Generic;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using System.Xml.Linq;

namespace ArcFMSilverlight.Controls.GenerationOnTransformer
{
    public class GetGeneration
    {
        private int servicePointFeatureCount;
        private List<Graphic> generations = new List<Graphic>();
        private List<Graphic> servicePointsOnTransformer = new List<Graphic>();
        private GenOnTransformerTool genOnTransformer;

        //private string _servicePointURL;
        //private string _generationInfoURL;
        //private Grid _mapArea;
        //private string _deviceSettingURL;

        // private static string _serviceLocationURL;
        private static IList<string> _genFeederLayerName = new List<string>();
        private static string _servicePointURL;
        private static string _generationInfoURL;
        private static Grid _mapArea;
        private static Map _map;
        private static string _deviceSettingURL;
        private static string _popUpTitle;
        // ENOSTOEDGIS - Changes for adding CGC to caption 
        private string _trCGC;
        public static void ReadConfiguration(Map map)
        {
            _map = map;
        }


        public void showGenMenu_Click(string globalId, string CGCNumber, string servicePointURL, string generationInfoURL, Grid mapArea, string deviceSettingURL)
        {

            _servicePointURL = servicePointURL;
            _generationInfoURL = generationInfoURL;
            _mapArea = mapArea;
            _deviceSettingURL = deviceSettingURL;
            // ENOSTOEDGIS - Changes for adding CGC to caption Starts
            try
            {
                _trCGC = CGCNumber;
                if (string.IsNullOrEmpty(_trCGC) || string.IsNullOrWhiteSpace(_trCGC))
                {
                    _trCGC = "NA";
                }
            }
            catch (Exception exp)
            {
            }
            //ENOSTOEDGIS - Changes for adding CGC to caption Ends

            if (globalId != null)
            {
                Query query = new Query(); //query on service point layer          
                query.OutFields.AddRange(new string[] { "GLOBALID", "SERVICEPOINTID", "STREETNUMBER", "STREETNAME1", "STREETNAME2", "CITY", "STATE", "COUNTY" });//Adding City to Address 
                query.Where = "TRANSFORMERGUID='" + globalId + "'";
                QueryTask queryTask = new QueryTask(_servicePointURL);
                queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(spQueryTask_ExecuteCompleted);
                queryTask.ExecuteAsync(query);
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

            Query query = new Query(); //query on generation info layer
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
                        if (field.Domain is CodedValueDomain)
                        {
                            ConfigUtility.DivisionCodedDomains = field.Domain as CodedValueDomain;
                        }
                    }

                    /****************************ENOS Tariff Change- Start***************************/
                    if (field.Name.ToUpper() == "METHODOFLIMITEDEXPORT")
                    {
                        if (field.Domain is CodedValueDomain)
                        {
                            ConfigUtility.GenInfoLimitedExportCodedDomains = field.Domain as CodedValueDomain;
                        }
                    }
                    /****************************ENOS Tariff Change- End****************************/
                }
            }
        }
        void genInfoQueryTask_ExecuteFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.DivisionCodedDomains = null;
            ConfigUtility.GenInfoLimitedExportCodedDomains = null; //ENOS Tariff Change
        }

        void genInfoQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            int isGeneration = 0;
            if ((e.FeatureSet).Features.Count > 0)
            {
                for (int i = 0; i < (e.FeatureSet).Features.Count; i++)
                {
                    generations.Add((e.FeatureSet).Features[i]);
                }
            }
            if (generations.Count > 0)
            {
                //foreach (var spGraphic in servicePointsOnTransformer)
                for (int i = 0; i < servicePointsOnTransformer.Count; i++)
                {
                    //foreach (var graphic in generations)
                    for (int j = 0; j < generations.Count; j++)
                    {

                        if (servicePointsOnTransformer[i].Attributes["GLOBALID"].ToString().Replace("}", "").Replace("{", "").ToUpper() == generations[j].Attributes["SERVICEPOINTGUID"].ToString().Replace("}", "").Replace("{", "").ToUpper())
                        {
                            servicePointsOnTransformer[i].Attributes["GENTYPE"] = ConfigUtility.DivisionCodedDomains.CodedValues[generations[j].Attributes["GENTYPE"].ToString()];
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
                            servicePointsOnTransformer[i].Attributes["GENSIZE"] = genSize.ToString();
                            servicePointsOnTransformer[i].Attributes["PROJECT/REF"] = generations[j].Attributes["SAPEGINOTIFICATION"];
                            servicePointsOnTransformer[i].Attributes["GENGLOBALID"] = generations[j].Attributes["GLOBALID"];
                            servicePointsOnTransformer[i].Attributes["FEEDERNUMBER"] = null;

                            /****************************ENOS Tariff Change- Start****************************/
                            servicePointsOnTransformer[i].Attributes["DERATED"] = generations[j].Attributes["DERATED"];
                            if (generations[j].Attributes["METHODOFLIMITEDEXPORT"] != null)
                            {
                                servicePointsOnTransformer[i].Attributes["METHODOFLIMITEDEXPORT"] = ConfigUtility.GenInfoLimitedExportCodedDomains.CodedValues[generations[j].Attributes["METHODOFLIMITEDEXPORT"].ToString()];
                            }
                            else
                            {
                                servicePointsOnTransformer[i].Attributes["METHODOFLIMITEDEXPORT"] = generations[j].Attributes["METHODOFLIMITEDEXPORT"];
                            }
                            /****************************ENOS Tariff Change- End****************************/

                            break;
                        }
                        else
                        {
                            isGeneration++;

                        }
                        if (isGeneration == generations.Count)
                        {
                            isGeneration = 0;
                            servicePointsOnTransformer[i].Attributes["GENTYPE"] = null;
                            servicePointsOnTransformer[i].Attributes["GENSIZE"] = null;
                            servicePointsOnTransformer[i].Attributes["PROJECT/REF"] = null;
                            servicePointsOnTransformer[i].Attributes["GENGLOBALID"] = null;
                            servicePointsOnTransformer[i].Attributes["FEEDERNUMBER"] = null;
                        }
                    }

                }
            }
            showGenInfoData(servicePointsOnTransformer);
            // servicePointsOnTransformer.Clear();
            ConfigUtility.DivisionCodedDomains = null;

        }

        void showGenInfoData(IList<Graphic> generations)
        {
            //ENOS2SAP PhaseIII Start
            genOnTransformer = new GenOnTransformerTool(_mapArea, generations, _deviceSettingURL, true, _map);
            genOnTransformer._generations = servicePointsOnTransformer;
            genOnTransformer._cgcNumber = _trCGC;
            // ENOSTOEDGIS - Changes for adding CGC to caption 
            // genOnTransformer.OpenDialog(_trCGC);
            //ENOS2SAP PhaseIII End
        }

        /*****************************ENOSChange Ends*******************************/
    }
}
