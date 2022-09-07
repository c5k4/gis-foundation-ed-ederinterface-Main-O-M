using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using Geometry = System.Windows.Media.Geometry;
using TriggerBase = System.Windows.TriggerBase;

namespace ArcFMSilverlight
{
    public partial class SchematicsRibbonPanel : UserControl
    {
        private string _circuitSourceURL;
       
       
        private const string DEFINITION_ANNO_RESET = "FeatureID in (select objectid from edgis.ZPGEVW_{0} {1})";
        private const string DEFINITION_ANNO = "FeatureID in (select objectid from edgis.{0} {1} and status >0)";
        private const string DEFINITION_CIRCUIT = "CIRCUITID='{0}'";
        private const string DEFINITION_VOLTAGE = "OPERATINGVOLTAGE={0}";
        private const string DEFINITION_VOLTAGE_BY_CIRCUITID = "SUBSTR(CIRCUITID,-4, 2)='{0}'";
        private const string DEFINITION_FEEDER_TYPE = "{0}={1}";
        private const string DEFINITION_VOLTAGE_BY_NOMINALVOLTAGE = "NOMINALVOLTAGE={0}";  //INC000004444604


        private Envelope _primaryUGLayerExtentGeometry = null;
        private Envelope _primaryOHLayerExtentGeometry = null;
        private bool _primaryUGReturned;
        private bool _primaryOHReturned;
        private Map _map;
        private IDictionary<string, string> _circuitIDDictionary = new Dictionary<string, string>();

        private string _circuitIDVoltageSubstringQuery = "";
        private string _nominalVoltageSubstringQuery = "";  //INC000004444604

        private string _primaryOhUrl = "";
        private string _primaryUgUrl = "";
        private int _substationSchemAnnoLayerId;

        private string _domainFieldName = "";
        private IDictionary<object, string> _codedValues = null;

        private string _annotationGroupLayer = "Annotation";
        private IDictionary<string, string> _layerFCPairs = new Dictionary<string, string>();

        private string _layerDefinition; // Bizarrely this is not available through REST -- or coming up empty
        private CADExportControl _cadExportTool;
        public CADExportControl CadExportTool
        {
            get
            {
                return _cadExportTool;

            }

            set
            {
                _cadExportTool = value;
            }

        }
        private ArcGISDynamicMapServiceLayer _schematicsDynamicMapServiceLayer;

        public ArcGISDynamicMapServiceLayer SchematicsDynamicMapServiceLayer
        {
            get
            {
                return _schematicsDynamicMapServiceLayer;
            }
            set
            {
                _schematicsDynamicMapServiceLayer = value;
                InitializeAnnotationLayerList(value.Layers);
                InitializeSchemFeederTypeDropDown();
            }

        }

        private IList<int> _filterLayerList;
        private IList<int> _voltageFilterByCircuitIDList;
        private IList<int> _voltageFilterAnnoByCircuitIDList;
        private IList<int> _annotationLayerList;
        private IList<int> _annotationNonProposedLayerList;
        private IList<int> _excludeFromVoltageFilterList;
        private IList<int> _excludeFromProposedAnnoList;
        private IList<int> _voltageFilterByNominalVoltList;   //INC000004444604

        public SchematicsRibbonPanel()
        {
            InitializeComponent();
        }

        private void InitializeAnnotationLayerList(LayerInfo[] layerInfos)
        {
            foreach (LayerInfo layerInfo in layerInfos)
            {
                if (layerInfo.Name == _annotationGroupLayer)
                {
                    _annotationLayerList = layerInfo.SubLayerIds.ToList();
                    _annotationNonProposedLayerList = _annotationLayerList.Except(_excludeFromProposedAnnoList).ToList();
                }
            }
        }

        public void InitializeConfiguration(Map map, XElement element)
        {
            _map = map;
            _layerDefinition = element.Element("LayerDefinition").Value;
            string layersCSV = element.Element("SchematicsFilter").Attribute("ids").Value;
            IList<string> layersArray = layersCSV.Split(',').ToList();
            _filterLayerList = layersArray.Select(x => Int32.Parse(x)).ToList();

            string excludeFromVoltageFilterLayerIDsCSV = element.Element("SchematicsFilter").Element("ExcludeFromVoltageFilterLayerIDsCSV").Attribute("ids").Value;
            IList<string> excludeLayersArray = excludeFromVoltageFilterLayerIDsCSV.Split(',').ToList();
            _excludeFromVoltageFilterList = excludeLayersArray.Select(x => Int32.Parse(x)).ToList();

            string excludeFromProposedAnnoLayerIDsCSV = element.Element("SchematicsFilter").Element("ExcludeFromProposedAnnoList").
                Attribute("ids").Value;
            excludeLayersArray = excludeFromProposedAnnoLayerIDsCSV.Split(',').ToList();
            _excludeFromProposedAnnoList = excludeLayersArray.Select(x => Int32.Parse(x)).ToList();

            string voltageFilterByCircuitIDCSV = element.Element("SchematicsFilter").Element("VoltageFilterByCircuitID").Attribute("ids").Value;
            IList<string> vlayersArray = voltageFilterByCircuitIDCSV.Split(',').ToList();
            _voltageFilterByCircuitIDList = vlayersArray.Select(x => Int32.Parse(x)).ToList();

            string voltageFilterAnnoByCircuitIDCSV = element.Element("SchematicsFilter").Element("VoltageFilterAnnoByCircuitID").Attribute("ids").Value;
            vlayersArray = voltageFilterAnnoByCircuitIDCSV.Split(',').ToList();
            _voltageFilterAnnoByCircuitIDList = vlayersArray.Select(x => Int32.Parse(x)).ToList();

            string codedDescriptionsCSV =
                element.Element("SchematicsFilter")
                    .Element("PrimaryVoltageDomain")
                    .Attribute("CodedDescriptionsCSV")
                    .Value;
            string codedValuesCSV =
                element.Element("SchematicsFilter").Element("PrimaryVoltageDomain").Attribute("CodedValuesCSV").Value;
            IList<string> codedDescriptions = codedDescriptionsCSV.Split(',').ToList();
            IList<string> codedValues = codedValuesCSV.Split(',').ToList();
            Dictionary<string, string> primaryVoltageDomainDictionary =
                codedValues.Zip(codedDescriptions, (k, v) => new { k, v })
                    .ToDictionary(x => x.k, x => x.v);
            Dictionary<string, string> placeHolder = new Dictionary<string, string>() {{"", "--Select--"}
        };
            Dictionary<string, string> voltageDictionary = placeHolder.Concat(primaryVoltageDomainDictionary).ToDictionary(x => x.Key, x => x.Value);
            PART_SchemVoltageDropDown.ItemsSource = voltageDictionary;
            PART_SchemVoltageDropDown.SelectedIndex = 0;
            _circuitSourceURL = element.Element("SchematicsFilter").Element("CircuitSourceService").Attribute("url").Value + "/" +
                element.Element("SchematicsFilter").Element("CircuitSourceService").Attribute("LayerId").Value;

            string layerFCPairsCSV =
                element.Element("SchematicsFilter").Element("CircuitFilter").Attribute("LayerFCPairs").Value;
            _layerFCPairs = layerFCPairsCSV.Split(',')
                .ToDictionary(l => l.Substring(0, l.IndexOf("-")), l => l.Substring(l.IndexOf("-") + 1));
            _primaryOhUrl = element.Element("PrimaryOh").Attribute("Url").Value + "/" + element.Element("PrimaryOh").Attribute("LayerId").Value;
            _primaryUgUrl = element.Element("PrimaryUg").Attribute("Url").Value + "/" + element.Element("PrimaryUg").Attribute("LayerId").Value;

            _domainFieldName = element.Element("SchematicsFilter").Element("FeederTypeFilter").Attribute("domainFieldName").Value;

            //INC000004444604
            string voltageFilterByNominalVoltCSV = element.Element("SchematicsFilter").Element("VoltageFilterByNominalVoltage").Attribute("ids").Value;
            IList<string> nominalVoltLayersArray = voltageFilterByNominalVoltCSV.Split(',').ToList();
            _voltageFilterByNominalVoltList = nominalVoltLayersArray.Select(x => Int32.Parse(x)).ToList();
        }


        private void SchemClearFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            PART_SchemCircuitPopup.IsOpen = false;
            PART_SchemVoltagePopup.IsOpen = false;
            PART_SchemFeederTypePopup.IsOpen = false;
            PART_SchematicsAutoCompleteTextBlock.Text = "";
            UnsetLayerDefinitionQuery(true);
            SchemFilterTextBlock.Text = "No Filter";
            PART_SchemVoltageDropDown.SelectedIndex = 0;
            PART_SchemFeederTypeDropDown.SelectedIndex = 0;
            ComboBoxItem cbItem = (ComboBoxItem)_cadExportTool.cboExportFormat.SelectedItem;
            string selectedExportFormat = cbItem.Content.ToString();

            if (_cadExportTool != null)
            {
                _cadExportTool.lblFilterType.Visibility = System.Windows.Visibility.Collapsed;
                _cadExportTool.lblFilterValue.Visibility = System.Windows.Visibility.Collapsed;
                _cadExportTool.lblFilterType.Content = "";
                _cadExportTool.lblFilterValue.Content = "";
            }
         
        }

        public void ClearSchematicFilter()
        {
            PART_SchemCircuitPopup.IsOpen = false;
            PART_SchemVoltagePopup.IsOpen = false;
            PART_SchemFeederTypePopup.IsOpen = false;
            PART_SchematicsAutoCompleteTextBlock.Text = "";
            UnsetLayerDefinitionQuery(true);
            SchemFilterTextBlock.Text = "No Filter";
            PART_SchemVoltageDropDown.SelectedIndex = 0;
            PART_SchemFeederTypeDropDown.SelectedIndex = 0;
            
        }

        private void SchemVoltageFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            PART_SchemCircuitPopup.IsOpen = false;
            PART_SchemFeederTypePopup.IsOpen = false;
            PART_SchemVoltagePopup.IsOpen = !PART_SchemVoltagePopup.IsOpen;
        }

        private void SchemCircuitFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            PART_SchemVoltagePopup.IsOpen = false;
            PART_SchemFeederTypePopup.IsOpen = false;
            PART_SchemCircuitPopup.IsOpen = !PART_SchemCircuitPopup.IsOpen;
        }

        private void PART_SchematicsAutoCompleteTextBlock_OnPopulating(object sender, PopulatingEventArgs e)
        {
            var queryTask =
                new QueryTask(_circuitSourceURL);
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTask_ExecuteCompleted);
            queryTask.Failed += new EventHandler<TaskFailedEventArgs>(queryTask_Failed);
            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            string userInput = e.Parameter.Trim().ToUpper();

            string substationLike = userInput.Contains(" ") ? userInput.Substring(0, userInput.IndexOf(" ")) : userInput;
            string circuitIDLike = userInput.Contains(" ") ? userInput.Substring(userInput.IndexOf(" ") + 1) : "";
            query.Where = "SUBSTATIONNAME like '" + substationLike + "%' and LENGTH(circuitid) = 9 ";
            if (circuitIDLike != "")
            {
                query.Where += " and substr(CIRCUITID, -4, 4) like '" + e.Parameter + "%'";
            }

            query.OutFields.Add("CIRCUITID");
            query.OutFields.Add("SUBSTATIONNAME");
            queryTask.ExecuteAsync(query);
        }

        private void queryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            // Log it??
        }

        private void queryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            //Weed out duplicates e.g. MILLBRAE 0401 either with distinct or groupby/first
            List<string> circuitIdList = e.FeatureSet.Features.
                Select(f => Convert.ToString(f.Attributes["SUBSTATIONNAME"]) + " " + Convert.ToString(f.Attributes["CIRCUITID"]).
                        Substring(Convert.ToString(f.Attributes["CIRCUITID"]).Length - 4)).ToList();
            circuitIdList = circuitIdList.Distinct().ToList();
            circuitIdList.Sort();
            _circuitIDDictionary = e.FeatureSet.Features.GroupBy(g => g.Attributes["SUBSTATIONNAME"] + Convert.ToString(g.Attributes["CIRCUITID"]).
                        Substring(Convert.ToString(g.Attributes["CIRCUITID"]).Length - 4)).Select(y => y.First()).
                ToDictionary(f => Convert.ToString(f.Attributes["SUBSTATIONNAME"]) + " " + Convert.ToString(f.Attributes["CIRCUITID"]).
                        Substring(Convert.ToString(f.Attributes["CIRCUITID"]).Length - 4), f => Convert.ToString(f.Attributes["CIRCUITID"]));
            PART_SchematicsAutoCompleteTextBlock.ItemsSource = circuitIdList;
            PART_SchematicsAutoCompleteTextBlock.PopulateComplete();
        }


        private void PART_SchematicsAutoCompleteTextBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    e.Handled = true;
                    PART_SchemCircuitPopup.IsOpen = false;
                    break;
                default:
                    break;
            }
        }

        private void PART_SchematicsAutoCompleteTextBlock_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string circuitId = _circuitIDDictionary[e.AddedItems[0].ToString()];
                SetLayerDefinitionQuery(string.Format(DEFINITION_CIRCUIT, circuitId), true);
                SchemFilterTextBlock.Text = e.AddedItems[0].ToString();
                //_cadExportTool.PopulateCircuitIds();
                 ComboBoxItem cbItem = (ComboBoxItem)_cadExportTool.cboExportFormat.SelectedItem;
                string selectedExportFormat = cbItem.Content.ToString();
                if (_cadExportTool != null)
                {
                    _cadExportTool.lblFilterType.Content = "Circuit ID";
                    _cadExportTool.lblFilterValue.Content = circuitId.ToString();
                    if (selectedExportFormat == "PNG")
                    {
                        _cadExportTool.lblFilterType.Visibility = System.Windows.Visibility.Visible;
                       
                        _cadExportTool.lblFilterValue.Visibility = System.Windows.Visibility.Visible;
                     
                    }
                }
            }
        }

        public void SetLayerDeffromDWG(string circuitId)
        {
            SetLayerDefinitionQuery(circuitId, false);

        }

        private void PART_SchemVoltageDropDown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && ((KeyValuePair<string, string>)e.AddedItems[0]).Key != "")
            {
                int codedValue = Convert.ToInt32(((KeyValuePair<string, string>)e.AddedItems[0]).Key);
                this.SchematicsDynamicMapServiceLayer.DisableClientCaching = true;
                string circuitIDVoltageSubstring =
                    VoltageToCircuitIDPrefix(((KeyValuePair<string, string>)e.AddedItems[0]).Value);
                _circuitIDVoltageSubstringQuery = string.Format(DEFINITION_VOLTAGE_BY_CIRCUITID,
                    circuitIDVoltageSubstring);
                _nominalVoltageSubstringQuery = string.Format(DEFINITION_VOLTAGE_BY_NOMINALVOLTAGE, codedValue);  //INC000004444604
                SetLayerDefinitionQuery(string.Format(DEFINITION_VOLTAGE, codedValue));
                this.SchematicsDynamicMapServiceLayer.DisableClientCaching = false;
                SchemFilterTextBlock.Text = ((KeyValuePair<string, string>)e.AddedItems[0]).Value;
                ComboBoxItem cbItem = (ComboBoxItem)_cadExportTool.cboExportFormat.SelectedItem;
                string selectedExportFormat = cbItem.Content.ToString();
                if (_cadExportTool != null)
                {
                    _cadExportTool.lblFilterType.Content = "Voltage";
                    _cadExportTool.lblFilterValue.Content = ((KeyValuePair<string, string>)e.AddedItems[0]).Value;
                    if (selectedExportFormat == "PNG")
                    {
                        _cadExportTool.lblFilterType.Visibility = System.Windows.Visibility.Visible;
                       
                        _cadExportTool.lblFilterValue.Visibility = System.Windows.Visibility.Visible;
                       
                        
                    }
                }
            }
            else
            {
                
                UnsetLayerDefinitionQuery(true);
                SchemFilterTextBlock.Text = "No Filter";
            }
        }

        private void SetAnnotationDefinitionQuery(string whereClause)
        {
            foreach (int layerID in _annotationLayerList)
            {
                string layerName = _schematicsDynamicMapServiceLayer.Layers[layerID].Name;
                LayerDefinition layerDefinition = new LayerDefinition();
                layerDefinition.LayerID = layerID;
                if (_voltageFilterAnnoByCircuitIDList.Contains(layerID))
                {
                    layerDefinition.Definition = GetAnnoDefinition(layerName, _circuitIDVoltageSubstringQuery);
                }
                else
                {
                    layerDefinition.Definition = GetAnnoDefinition(layerName, whereClause);
                }
                this.SchematicsDynamicMapServiceLayer.LayerDefinitions.Add(layerDefinition);
            }

        }

        private string GetAnnoDefinition(string layerName, string whereClause = "")
        {
            string definition;
            string layerNameRedux = layerName.Substring(0, layerName.IndexOf("Schem"));
            string featureClass = _layerFCPairs.ContainsKey(layerNameRedux)
                ? _layerFCPairs[layerNameRedux]
                : layerNameRedux;
            if (whereClause != "")
            {
                definition = DEFINITION_ANNO;
                whereClause = " where " + whereClause;
            }
            else
            {
                definition = DEFINITION_ANNO_RESET;
            }
            definition = String.Format(definition, featureClass, whereClause);

            return definition;
        }

        private void UnsetLayerDefinitionQuery(bool refresh = false)
        {
            if (SchematicsDynamicMapServiceLayer == null) return;
            if (SchematicsDynamicMapServiceLayer.LayerDefinitions.Count > 0)
            {
                SchematicsDynamicMapServiceLayer.LayerDefinitions.Clear();
                RestoreAnnoDefinitions();
                /*cadschematicfilter reset*/
                _cadExportTool.SchematicsFilterDefinitionquery = null;
            }
            if (refresh)
            {
                SchematicsDynamicMapServiceLayer.DisableClientCaching = true;
                SchematicsDynamicMapServiceLayer.Refresh();
                SchematicsDynamicMapServiceLayer.DisableClientCaching = false;
            }
        }

        private void RestoreAnnoDefinitions()
        {
            foreach (int layerId in _annotationNonProposedLayerList)
            {
                LayerDefinition layerDefinition = new LayerDefinition();
                layerDefinition.LayerID = layerId;
                // How to get the layerName?
                string layerName = _schematicsDynamicMapServiceLayer.Layers[layerId].Name;
                layerDefinition.Definition = GetAnnoDefinition(layerName);
                SchematicsDynamicMapServiceLayer.LayerDefinitions.Add(layerDefinition);
            }
        }

        private void SetLayerDefinitionQuery(string definition, bool setExtent = false)
        {
            UnsetLayerDefinitionQuery();
            //string []filterDenfination={};
            
            bool definitionQueryIsVoltage = definition.ToUpper().StartsWith("OPERATINGVOLTAGE");

            foreach (int layerID in _filterLayerList)
            {
                if (definitionQueryIsVoltage && _excludeFromVoltageFilterList.Contains(layerID))
                {
                    continue;
                }
                LayerDefinition layerDefinition = new LayerDefinition();
                layerDefinition.LayerID = layerID;
                
                // This is a bit of an a(d)h(o)c last minute request to have some layers work on CircuitID
                if (definitionQueryIsVoltage && _voltageFilterByCircuitIDList.Contains(layerID))
                {
                    layerDefinition.Definition = _circuitIDVoltageSubstringQuery;
                }
                else if (definitionQueryIsVoltage && _voltageFilterByNominalVoltList.Contains(layerID))    //INC000004444604
                {
                    layerDefinition.Definition = _nominalVoltageSubstringQuery;
                }
                else
                {
                    layerDefinition.Definition = definition;
                }
                layerDefinition.Definition += " and " + _layerDefinition;
 
               
                SchematicsDynamicMapServiceLayer.LayerDefinitions.Add(layerDefinition);
            }
            
            
            // This requires the standardized queries = false property in arcgisserver
            SetAnnotationDefinitionQuery(definition);
            Dictionary<string, string> DefinitionDictionary = new Dictionary<string, string>();
            var items = new List<KeyValuePair<string, string>>();
            
           
           
            foreach (LayerDefinition layer in this.SchematicsDynamicMapServiceLayer.LayerDefinitions)
            {
                //INC000004444604 - to handle CAD export
                if (definitionQueryIsVoltage && _voltageFilterByNominalVoltList.Contains(layer.LayerID))
                {
                    items.Add(new KeyValuePair<string, string>(layer.LayerID.ToString(), definition + " and " + _layerDefinition));
                }
                else
                {
                    items.Add(new KeyValuePair<string, string>(layer.LayerID.ToString(), layer.Definition));
                }
            }

    
           
            string defnJson = MyDictionaryToJson(items);
            

            _cadExportTool.SchematicsFilterDefinitionquery = defnJson;
            this.SchematicsDynamicMapServiceLayer.Refresh();
            if (setExtent)
            {
                SetExtentFromDefinitionQuery(definition);
            }
        }

        string MyDictionaryToJson(List<KeyValuePair<string, string>> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
            return "{" + string.Join(",", entries) + "}";
        }

        

        private void SetExtentFromDefinitionQuery(string definition)
        {
            _primaryUGLayerExtentGeometry = null;
            _primaryOHLayerExtentGeometry = null;
            _primaryOHReturned = false;
            _primaryUGReturned = false;
            Query query = new Query();
            query.ReturnGeometry = true;
            query.Where = definition;
            QueryTask queryTaskOh = new QueryTask(_primaryOhUrl);
            queryTaskOh.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskOh_ExecuteCompleted);
            queryTaskOh.ExecuteAsync(query);
            QueryTask queryTaskUg = new QueryTask(_primaryUgUrl);
            queryTaskUg.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskUg_ExecuteCompleted);
            queryTaskUg.ExecuteAsync(query);

        }

        /// <summary>
        /// Converts voltage description e.g. "4.2 KV" and returns "04"
        /// </summary>
        /// <param name="voltageDescription"></param>
        /// <returns></returns>
        private string VoltageToCircuitIDPrefix(string voltageDescription)
        {
            string circuitIDPrefix =
                ((int)Convert.ToDouble(voltageDescription.Substring(0, voltageDescription.IndexOf(" ")))).ToString("D2");

            if (circuitIDPrefix == "12")
            {
                circuitIDPrefix = "11";
            }

            return circuitIDPrefix;
        }

        private void queryTaskUg_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _primaryUGReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _primaryUGLayerExtentGeometry = GetGeometry(featureSet);

            if (_primaryOHReturned)
            {
                ZoomToExtent(_primaryOHLayerExtentGeometry, _primaryUGLayerExtentGeometry);
            }
        }

        private void queryTaskOh_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _primaryOHReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _primaryOHLayerExtentGeometry = GetGeometry(featureSet);
            if (_primaryUGReturned)
            {
                ZoomToExtent(_primaryOHLayerExtentGeometry, _primaryUGLayerExtentGeometry);
            }
        }

        private Envelope GetGeometry(FeatureSet featureSet)
        {
            Envelope env = null;
            foreach (Graphic feature in featureSet.Features)
            {
                if (env == null)
                    env = feature.Geometry.Extent.Clone();
                else
                    env = env.Union(feature.Geometry.Extent);
            }

            return env;
        }

        private void ZoomToExtent(Envelope envelope1, Envelope envelope2)
        {
            Envelope env = null;
            if (envelope1 != null)
            {
                env = envelope1;
                if (envelope2 != null)
                {
                    env.Union(envelope2);
                }
            }
            else if (envelope2 != null) // envelope1 is null
            {
                env = envelope2;
            }
            if (env != null)
            {
                _map.ZoomTo(env);
                //                _map.Extent = env;
            }
            else
            {
                MessageBox.Show("No features found for this CircuitID");
            }
        }

        void InitializeSchemFeederTypeDropDown()
        {
            _schematicsDynamicMapServiceLayer.GetDetails(_filterLayerList[0],
                OnGetDetails);
        }

        private void OnGetDetails(FeatureLayerInfo featureLayerInfo, Exception exception)
        {
            Domain domain = featureLayerInfo.Fields.Where(f => f.Name == _domainFieldName).First().Domain;

            CodedValueDomain codedValueDomain = domain as CodedValueDomain;
            _codedValues = codedValueDomain.CodedValues;

            Dictionary<object, string> placeHolder = new Dictionary<object, string>() {{"", "--Select--"}
        };
            Dictionary<object, string> values = placeHolder.Concat(_codedValues).ToDictionary(x => x.Key, x => x.Value);
            PART_SchemFeederTypeDropDown.ItemsSource = values;
            PART_SchemFeederTypeDropDown.SelectedIndex = 0;

        }

        private void SchemFeederTypeFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            PART_SchemVoltagePopup.IsOpen = false;
            PART_SchemCircuitPopup.IsOpen = false;
            PART_SchemFeederTypePopup.IsOpen = !PART_SchemFeederTypePopup.IsOpen;
        }

        private void PART_SchemFeederTypeDropDown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && ((KeyValuePair<object, string>)e.AddedItems[0]).Key.ToString() != "")
            {
                int feederType = Convert.ToInt32(((KeyValuePair<object, string>)e.AddedItems[0]).Key);
                SetLayerDefinitionQuery(string.Format(DEFINITION_FEEDER_TYPE, _domainFieldName, feederType));
                SchemFilterTextBlock.Text = ((KeyValuePair<object, string>)e.AddedItems[0]).Value;
                ComboBoxItem cbItem = (ComboBoxItem)_cadExportTool.cboExportFormat.SelectedItem;
                string selectedExportFormat = cbItem.Content.ToString();
                if (_cadExportTool != null)
                {
                    _cadExportTool.lblFilterType.Content = "Feeder Type";
                    _cadExportTool.lblFilterValue.Content = ((KeyValuePair<object, string>)e.AddedItems[0]).Value;
                    if (selectedExportFormat == "PNG")
                    {
                        _cadExportTool.lblFilterType.Visibility = System.Windows.Visibility.Visible;
                       
                        _cadExportTool.lblFilterValue.Visibility = System.Windows.Visibility.Visible;
                       
                      
                        _cadExportTool.Height = 320;
                        _cadExportTool.stdwgPanel.Height = 320;
                        _cadExportTool.DwgFloatWindow.Height = 330;
                    }
                }

            }
            else
            {
               
                UnsetLayerDefinitionQuery(true);
                SchemFilterTextBlock.Text = "No Filter";
            }
        }
    }

    public class UpperCaseBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs args)
        {
            var selectionStart = AssociatedObject.SelectionStart;
            var selectionLength = AssociatedObject.SelectionLength;

            AssociatedObject.Text = AssociatedObject.Text.ToUpper();

            AssociatedObject.SelectionStart = selectionStart;
            AssociatedObject.SelectionLength = selectionLength;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
            base.OnDetaching();
        }
    }

}
