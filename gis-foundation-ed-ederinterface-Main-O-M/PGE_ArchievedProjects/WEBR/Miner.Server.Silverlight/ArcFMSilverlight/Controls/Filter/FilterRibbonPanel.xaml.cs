using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using Geometry = System.Windows.Media.Geometry;
using SimpleLineSymbol = ESRI.ArcGIS.Client.Symbols.SimpleLineSymbol;
using TriggerBase = System.Windows.TriggerBase;

namespace ArcFMSilverlight
{
    public partial class FilterRibbonPanel : UserControl
    {
        private string _circuitSourceURL;

        //private const string FILTERRENDER_ATTRIBUTE = "FILTERRENDER";
        private const string DEFINITION_CIRCUIT = "CIRCUITID='{0}'";
        private const int FONT_SIZE = 35;
        private FontFamily _fontFamily = new FontFamily("Arial");

        private Envelope _primaryUGLayerExtentGeometry = null;
        private Envelope _primaryOHLayerExtentGeometry = null;
        private bool _primaryUGReturned;
        private bool _primaryOHReturned;
        private bool _circuitPointReturned;
        private Map _map;
        private IDictionary<string, string> _circuitIDDictionary = new Dictionary<string, string>();

        private string _primaryOhUrl = "";
        private string _primaryUgUrl = "";
        private string _circuitPointUrl = "";

        public IList<string> MapServiceLayers { get; set; }
        private IList<Graphic> _primaryUgGraphics;
        private IList<Graphic> _primaryOhGraphics;
        private Graphic _circuitPointGraphic = null;  //INC000004272706
        private GraphicsLayer _graphicsLayer = new GraphicsLayer();
        private GraphicsLayer _graphicsLayerPoint = new GraphicsLayer();
        private GraphicsLayer _graphicsLayerText = new GraphicsLayer();
        private bool _createNewSelection = false;
        private bool _zoomToAllSelected = false;

        private Envelope _selectedEnvelope = null;

        private string _circuitIdSelected = "";

        //INC000004272706------WEBR: Circuit Highlight Color selection
        private IList<Graphic> _lineGraphics = new List<Graphic>();
        Color _colorObj = new Color();
        bool _nullCircuitColor = false;
        private List<string> _circuitIDList = new List<string>();
        private Dictionary<string, string> _allCircuitColors = new Dictionary<string, string>();
        private List<string> _circuitColorList = new List<string>();
        Random random = new Random();

        //INC000004371220-----Highlight secondary network in case primary conductors are not available.
        private Envelope _secondaryUGLayerExtentGeometry = null;
        private Envelope _secondaryOHLayerExtentGeometry = null;
        private Envelope _secondaryBusBarLayerExtentGeometry = null;
        private bool _secondaryUGReturned;
        private bool _secondaryOHReturned;
        private bool _secondaryBusBarReturned;
        private string _secondaryOhUrl = "";
        private string _secondaryUgUrl = "";
        private string _secondaryBusBarUrl = "";
        private IList<Graphic> _secondaryUgGraphics;
        private IList<Graphic> _secondaryOhGraphics;
        private IList<Graphic> _secondaryBusBarGraphics;

        public FilterRibbonPanel()
        {
            InitializeComponent();
        }


        public void InitializeConfiguration(Map map, XElement element)
        {
            _map = map;
            _primaryOhUrl = element.Element("PrimaryOhUrl").Attribute("Url").Value + "/" +
                            element.Element("PrimaryOhUrl").Attribute("LayerId").Value;
            _primaryUgUrl = element.Element("PrimaryUgUrl").Attribute("Url").Value + "/" +
                            element.Element("PrimaryUgUrl").Attribute("LayerId").Value;

            //INC000004371220---START
            _secondaryOhUrl = element.Element("SecondaryOhUrl").Attribute("Url").Value + "/" +
                           element.Element("SecondaryOhUrl").Attribute("LayerId").Value;
            _secondaryUgUrl = element.Element("SecondaryUgUrl").Attribute("Url").Value + "/" +
                           element.Element("SecondaryUgUrl").Attribute("LayerId").Value;
            _secondaryBusBarUrl = element.Element("SecondaryBusBarUrl").Attribute("Url").Value + "/" +
                          element.Element("SecondaryUgUrl").Attribute("LayerId").Value;
            //INC000004371220---END

            _circuitSourceURL = element.Element("CircuitSourceUrl").Attribute("Url").Value + "/" +
                                element.Element("CircuitSourceUrl").Attribute("LayerId").Value;
            _circuitPointUrl = element.Element("CircuitPointUrl").Attribute("Url").Value + "/" +
                                element.Element("CircuitPointUrl").Attribute("LayerId").Value;

            //INC000004272706
            _allCircuitColors.Clear();
            foreach (XElement scaleElement in element.Element("CircuitColor").Elements())
            {
                _allCircuitColors.Add(scaleElement.Attribute("code").Value, scaleElement.Attribute("value").Value);
            }

            _graphicsLayer.ID = "CircuitGraphics";
            _graphicsLayerPoint.ID = "CircuitPointGraphic";

            _map.Layers.Add(_graphicsLayer);
            _map.Layers.Add(_graphicsLayerPoint);
            _map.Layers.Add(_graphicsLayerText);

            MapServiceLayers = (element.Attribute("mapServiceLayerIds").Value.Split(',').ToList());

        }

        public void SetStateFromStoredDisplayChange(string storedDisplayName)
        {
            if (!MapServiceLayers.Contains(storedDisplayName))
            {
                IsEnabled = false;
                ResetFilterUi(_createNewSelection);
            }
            else
            {
                IsEnabled = true;
            }
        }

        public void ResetFilterUi(bool resetGraphics = true)
        {
            PART_CircuitPopup.IsOpen = false;
            PART_FilterAutoCompleteTextBlock.Text = "";
            FilterTextBlock.Text = "None";
            CircuitFilterButton.IsChecked = false;
            if (resetGraphics)
            {
                ResetFilterGraphics();
            }
        }

        private void ClearFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            ResetFilterUi();
        }


        private void CircuitFilterButton_OnClick(object sender, RoutedEventArgs e)
        {
            PART_CircuitPopup.IsOpen = !PART_CircuitPopup.IsOpen;
        }

        private bool SearchStringEndsWithCircuitID(string searchString)
        {
            int n;
            return (searchString.Contains(" ") &&
                    int.TryParse(searchString.Substring(searchString.LastIndexOf(" ") + 1), out n));
        }

        private void PART_AutoCompleteTextBlock_OnPopulating(object sender, PopulatingEventArgs e)
        {
            var queryTask =
                new QueryTask(_circuitSourceURL);
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTask_ExecuteCompleted);
            queryTask.Failed += new EventHandler<TaskFailedEventArgs>(queryTask_Failed);
            Query query = new Query();
            string userInput = e.Parameter.TrimStart().ToUpper();
            //TODO: Fix BUG 24559. Input could be MANCHESTER 1101 or SAN CARLOS 1101, needs to search after SAN CARLOS
            string substationLike = userInput.Contains(" ") ? userInput.Substring(0, userInput.LastIndexOf(" ")) : userInput;
            query.Where = "SUBSTATIONNAME like '" + substationLike + "%' and LENGTH(circuitid) = 9 ";
            if (SearchStringEndsWithCircuitID(userInput))
            {
                string circuitIDLike = userInput.Substring(userInput.LastIndexOf(" ") + 1);
                query.Where += " and substr(CIRCUITID, -4, 4) like '" + circuitIDLike + "%'";
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
            PART_FilterAutoCompleteTextBlock.ItemsSource = circuitIdList;
            PART_FilterAutoCompleteTextBlock.PopulateComplete();
        }


        private void PART_AutoCompleteTextBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    e.Handled = true;
                    PART_CircuitPopup.IsOpen = false;
                    break;
                case Key.Enter:
                    break;
                default:
                    break;
            }
        }

        private void ResetFilterGraphics()
        {
            _graphicsLayer.ClearGraphics();
            _graphicsLayerPoint.ClearGraphics();
            _graphicsLayerText.ClearGraphics();
            _selectedEnvelope = null;
            //_renderColor = 0;
            _circuitIdSelected = "";

            //INC000004272706
            _circuitPointGraphic = null;
            _lineGraphics.Clear();
            _circuitIDList.Clear();
            _nullCircuitColor = false;
            _circuitColorList.Clear();
        }

        private void PART_AutoCompleteTextBlock_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string circuitId = _circuitIDDictionary[e.AddedItems[0].ToString()];
                ZoomToCircuit(circuitId, e.AddedItems[0].ToString());
            }
        }


        public void ZoomToCircuit(string circuitId, string circuitName, bool resetGraphics = false)
        {

            if (_createNewSelection || resetGraphics)
            {
                ResetFilterGraphics();
            }
            ConfigUtility.UpdateStatusBarText("Zooming to Circuit [ " + circuitName + " ]...");
            FilterTextBlock.Text = circuitName;
            _circuitIdSelected = circuitId;
            getCircuitColor();   //INC000004272706
            //getCircuitPoint();    
            // SetExtentFromDefinitionQuery(string.Format(DEFINITION_CIRCUIT, circuitId));
        }

        //INC000004272706---START
        private void getCircuitColor()
        {
            var queryTask = new QueryTask(_circuitSourceURL);
            queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskColor_ExecuteCompleted);
            queryTask.Failed += new EventHandler<TaskFailedEventArgs>(queryTask_Failed);
            Query query = new Query();
            query.Where = "CIRCUITID='" + _circuitIdSelected + "'";
            query.OutFields.Add("CIRCUITCOLOR");
            queryTask.ExecuteAsync(query);
        }

        private void queryTaskColor_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            if (e != null)
            {
                if (e.FeatureSet.Features.Count > 0 && (e.FeatureSet).Features[0].Attributes["CIRCUITCOLOR"] != null)
                {
                    string _circuitColor = (e.FeatureSet).Features[0].Attributes["CIRCUITCOLOR"].ToString();
                    if (_circuitColor.Trim().Length > 0)
                    {
                        _colorObj = getColorObject((e.FeatureSet).Features[0].Attributes["CIRCUITCOLOR"].ToString());
                        _nullCircuitColor = false;
                    }
                    else
                    {
                        _nullCircuitColor = true;
                    }
                }
                else
                {
                    _nullCircuitColor = true;
                }
                getCircuitPoint();
            }
            else
                ConfigUtility.UpdateStatusBarText("Error in highlighting circuit.");
        }

        private Color getColorObject(string cktcolor)
        {
            Color colorObj = new Color();
            if (_allCircuitColors.ContainsKey(cktcolor))
            {
                string[] RGBValues = _allCircuitColors[cktcolor].Split(',');
                Color _tempColor = Color.FromArgb(255, (byte)Convert.ToInt16(RGBValues[0]), (byte)Convert.ToInt16(RGBValues[1]), (byte)Convert.ToInt16(RGBValues[2]));
                if (!checkDuplicate(_tempColor.ToString()))
                {
                    colorObj = _tempColor;
                }
                else
                {
                    colorObj = getDifferentColor();
                }
            }
            else
            {
                colorObj = getDifferentColor();
            }

            return colorObj;
        }

        private Color getDifferentColor()
        {
            Color colorObj = new Color();
            bool random = true;
            foreach (string rgbValues in _allCircuitColors.Values)
            {
                string[] RGBValues = rgbValues.Split(',');
                Color _tempColor = Color.FromArgb(255, (byte)Convert.ToInt16(RGBValues[0]), (byte)Convert.ToInt16(RGBValues[1]), (byte)Convert.ToInt16(RGBValues[2]));
                if (_circuitColorList.IndexOf(_tempColor.ToString()) < 0)
                {
                    colorObj = _tempColor;
                    random = false;
                    break;
                }
            }
            if (random)
            {
                colorObj = getRandomColor();
            }
            return colorObj;
        }

        private Color getRandomColor()
        {
            Color color = new Color();

            string hexColor = String.Format("#{0:X6}", random.Next(0x1000000));
            if (checkDuplicate(hexColor))
            {
                getRandomColor();
            }
            else
                color = colorFromHex(hexColor);

            return color;
        }

        private Color colorFromHex(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6) throw new Exception("Color not valid");

            return Color.FromArgb(255,
                byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        }

        private bool checkDuplicate(string hexColor)
        {
            if (_circuitColorList.IndexOf(hexColor) >= 0)
                return true;
            else
                return false;
        }
        //INC000004272706---END

        private void SetExtentFromDefinitionQuery(string definition)
        {
            _primaryUGLayerExtentGeometry = null;
            _primaryOHLayerExtentGeometry = null;
            _primaryOHReturned = false;
            _primaryUGReturned = false;
            _primaryOhGraphics = null;
            _primaryUgGraphics = null;
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

        private void getCircuitPoint()
        {
            _circuitPointReturned = false;
            Query query = new Query();
            query.ReturnGeometry = true;
            query.Where = string.Format(DEFINITION_CIRCUIT, _circuitIdSelected);
            QueryTask queryTaskCP = new QueryTask(_circuitPointUrl);
            queryTaskCP.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskCP_ExecuteCompleted);
            queryTaskCP.ExecuteAsync(query);
        }

        //INC000004272706---START
        private void SetRenderColor(IList<Graphic> graphics)
        {
            if (!_nullCircuitColor)
            {
                foreach (Graphic graphic in graphics)
                {
                    Graphic lineGraphic = new Graphic();
                    lineGraphic.Geometry = graphic.Geometry;
                    lineGraphic.Symbol = new SimpleLineSymbol()   //INC000004191953-LineSymbol changed to SimpleLineSymbol
                    {
                        Color = new SolidColorBrush(_colorObj),
                        Width = 4
                    };
                    _lineGraphics.Add(lineGraphic);
                }
            }
        }
        //INC000004272706----END

        private void queryTaskUg_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _primaryUGReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _primaryUGLayerExtentGeometry = GetGeometry(featureSet);
            _primaryUgGraphics = featureSet.Features;
            SetRenderColor(_primaryUgGraphics);
            if (_primaryOHReturned)
                CheckForSecondaryNetwork();  //INC000004371220
        }

        private void queryTaskOh_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _primaryOHReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _primaryOHLayerExtentGeometry = GetGeometry(featureSet);
            _primaryOhGraphics = featureSet.Features;
            SetRenderColor(_primaryOhGraphics);
            if (_primaryUGReturned)
                CheckForSecondaryNetwork();  //INC000004371220
        }

        //INC000004371220----START
        private void CheckForSecondaryNetwork()
        {
            if (_primaryOhGraphics.Count > 0 || _primaryUgGraphics.Count > 0)
                ZoomToExtent(_primaryOHLayerExtentGeometry, _primaryUGLayerExtentGeometry, null);
            else
            {
                _secondaryUGLayerExtentGeometry = null;
                _secondaryOHLayerExtentGeometry = null;
                _secondaryBusBarLayerExtentGeometry = null;
                _secondaryOHReturned = false;
                _secondaryUGReturned = false;
                _secondaryBusBarReturned = false;
                _secondaryOhGraphics = null;
                _secondaryUgGraphics = null;
                _secondaryBusBarGraphics = null;
                Query query = new Query();
                query.ReturnGeometry = true;
                query.Where = string.Format(DEFINITION_CIRCUIT, _circuitIdSelected);
                QueryTask queryTaskSecOh = new QueryTask(_secondaryOhUrl);
                queryTaskSecOh.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskSecOh_ExecuteCompleted);
                queryTaskSecOh.ExecuteAsync(query);
                QueryTask queryTaskSecUg = new QueryTask(_secondaryUgUrl);
                queryTaskSecUg.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskSecUg_ExecuteCompleted);
                queryTaskSecUg.ExecuteAsync(query);
                query.Where = string.Format(DEFINITION_CIRCUIT, _circuitIdSelected) + " AND SUBTYPECD=2"; //SUBTYPECD=2 for Secondary
                QueryTask queryTaskSecBusBar = new QueryTask(_secondaryBusBarUrl);
                queryTaskSecBusBar.ExecuteCompleted += new EventHandler<QueryEventArgs>(queryTaskSecBusBar_ExecuteCompleted);
                queryTaskSecBusBar.ExecuteAsync(query);
            }
        }

        private void queryTaskSecUg_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _secondaryUGReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _secondaryUGLayerExtentGeometry = GetGeometry(featureSet);
            _secondaryUgGraphics = featureSet.Features;
            SetRenderColor(_secondaryUgGraphics);
            if (_secondaryOHReturned && _secondaryBusBarReturned)
                ZoomToExtent(_secondaryOHLayerExtentGeometry, _secondaryUGLayerExtentGeometry, _secondaryBusBarLayerExtentGeometry);
        }

        private void queryTaskSecOh_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _secondaryOHReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _secondaryOHLayerExtentGeometry = GetGeometry(featureSet);
            _secondaryOhGraphics = featureSet.Features;
            SetRenderColor(_secondaryOhGraphics);
            if (_secondaryUGReturned && _secondaryBusBarReturned)
                ZoomToExtent(_secondaryOHLayerExtentGeometry, _secondaryUGLayerExtentGeometry, _secondaryBusBarLayerExtentGeometry);
        }

        private void queryTaskSecBusBar_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _secondaryBusBarReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            _secondaryBusBarLayerExtentGeometry = GetGeometry(featureSet);
            _secondaryBusBarGraphics = featureSet.Features;
            SetRenderColor(_secondaryBusBarGraphics);
            if (_secondaryUGReturned && _secondaryOHReturned)
                ZoomToExtent(_secondaryOHLayerExtentGeometry, _secondaryUGLayerExtentGeometry, _secondaryBusBarLayerExtentGeometry);
        }
        //INC000004371220----END

        private void queryTaskCP_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            _circuitPointReturned = true;
            FeatureSet featureSet = e.FeatureSet;
            //_circuitPointGraphic = featureSet.Features;
            if (featureSet.Features.Count > 0)   //INC000004272706
                _circuitPointGraphic = featureSet.Features[0];
            SetExtentFromDefinitionQuery(string.Format(DEFINITION_CIRCUIT, _circuitIdSelected));
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

        private void SetSelectedEnvelope(Envelope newEnvelope)
        {
            if (_selectedEnvelope != null && _zoomToAllSelected)
            {
                _selectedEnvelope = _selectedEnvelope.Union(newEnvelope);
            }
            else
            {
                _selectedEnvelope = newEnvelope;
            }

        }

        private void ZoomToExtent(Envelope envelope1, Envelope envelope2, Envelope envelope3)
        {
            const double EXPAND_FACTOR = 1.25;

            //_renderColor++;
            if (envelope1 != null)
            {
                SetSelectedEnvelope(envelope1);
                if (envelope2 != null)
                {
                    _selectedEnvelope = _selectedEnvelope.Union(envelope2);
                }
                if (envelope3 != null)
                {
                    _selectedEnvelope = _selectedEnvelope.Union(envelope3);
                }
            }
            else if (envelope2 != null) // envelope1 is null
            {
                SetSelectedEnvelope(envelope2);
                if (envelope3 != null)
                {
                    _selectedEnvelope = _selectedEnvelope.Union(envelope3);
                }
            }
            else if (envelope3 != null) // envelopes 1 and 2 are null
            {
                SetSelectedEnvelope(envelope3);
            }

            if (_selectedEnvelope != null)
            {
                _map.ZoomTo(_selectedEnvelope.Expand(EXPAND_FACTOR));
            }
            else
            {
                MessageBox.Show("No features found for this CircuitID");
            }

            if (_lineGraphics.Count > 0)
            {
                CreateCircuitGraphics();
            }
            ConfigUtility.UpdateStatusBarText("");
            if (_nullCircuitColor)
                MessageBox.Show("There is no color available for entered circuit. Please contact Mapping Team. Thanks");
        }

        private void CreateCircuitGraphics()
        {
            //INC000004272706
            if (!_circuitIDList.Contains(_circuitIdSelected))
            {
                _graphicsLayer.Graphics.AddRange(_lineGraphics);
                _circuitColorList.Add(_colorObj.ToString());
                _circuitIDList.Add(_circuitIdSelected);

                if (_circuitPointGraphic != null && !_nullCircuitColor)
                {
                    Graphic pointGraphic = new Graphic();
                    pointGraphic.Geometry = _circuitPointGraphic.Geometry;
                    pointGraphic.Symbol = new ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol()
                    {
                        Color = new SolidColorBrush(_colorObj),
                        Size = 40,
                        Style = ESRI.ArcGIS.Client.Symbols.SimpleMarkerSymbol.SimpleMarkerStyle.Triangle
                    };
                    if (!(_graphicsLayerPoint.Graphics.Contains(pointGraphic)))
                        _graphicsLayerPoint.Graphics.Add(pointGraphic);
                }

                // Need centroid of extent
                if (_selectedEnvelope != null && !_nullCircuitColor)
                {
                    TextSymbol textSymbol = new TextSymbol()
                    {
                        FontFamily = _fontFamily,
                        Foreground = new SolidColorBrush(_colorObj),  //INC000004272706
                        FontSize = FONT_SIZE,
                        Text = FilterTextBlock.Text
                    };

                    Size textSize = CADExportControl.GetTextSize(textSymbol.Text, _fontFamily, FONT_SIZE);
                    textSymbol.OffsetX = textSize.Width / 2;
                    textSymbol.OffsetY = textSize.Height / 2;

                    Graphic textGraphic = new Graphic() { Symbol = textSymbol, Geometry = _selectedEnvelope.GetCenter() };
                    textGraphic.Geometry = _selectedEnvelope.GetCenter();
                    _graphicsLayerText.Graphics.Add(textGraphic);
                }
            }

            _lineGraphics.Clear();
        }

    }
}


