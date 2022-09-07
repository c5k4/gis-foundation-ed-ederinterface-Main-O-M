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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ArcFMSilverlight.Controls.Ufm;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using Miner.Server.Client;
using Geometry = System.Windows.Media.Geometry;
using TriggerBase = System.Windows.TriggerBase;

namespace ArcFMSilverlight
{
    public partial class UfmRibbonPanel : UserControl
    {
        private const string NO_FILTER_STATE = "All (Default)";
        private const string SELECT_PLACEHOLDER = "--Select--";

        private IDictionary<string, PseudoGroupLayerFilter> _pseudoGroupLayerFilters = new Dictionary<string, PseudoGroupLayerFilter>(); 

        private Map _map;
        private MapTools _mapTools;
        private TabItem _ufmTabItem;
        private UfmPrintTool _ufmPrintTool;
        private PseudoGroupLayerFilter _currentPseudoGroupLayerFilter = null;
        private PseudoGroupLayerFilter _electricDistribution50Filter = null;
        private PseudoGroupLayerFilter _priSec50EdmFilter = null;
        private PseudoGroupLayerFilter _ufm50EdmFilter = null;
        private PseudoGroupLayerFilter _ufmFilter = null;
        private string _currentStoredDisplayName = "";
        // This should be configuration, not in the code
        private IDictionary<string, bool> _edmasterFilterApplied = new Dictionary<string, bool>()
        {
            {"Elec Dist 50 Scale",false},
            {"Elec Dist 50 Scale Primary View",false},
            {"Elec Dist 50 Scale Secondary View",false},
            {"Elec Dist 50 Scale Duct View",false}
        };

        private bool _isResetting = false;

        public UfmRibbonPanel()
        {
            InitializeComponent();
        }

        public void ResetUiState()
        {

            _isResetting = true;
            rdoDefault.IsChecked = true;
            _isResetting = false;
        }


        public void SetUfmVisibility(string storedDisplayName)
        {
            _ufmTabItem.Visibility = Visibility.Visible;
            _isResetting = true;
            _currentStoredDisplayName = storedDisplayName;

            if (storedDisplayName.StartsWith("Elec Dist 50"))
            {
                if (storedDisplayName.Contains("Primary"))
                {
                    rdoPrimary.IsChecked = true;
                }
                else if (storedDisplayName.Contains("Secondary"))
                {
                    rdoSecondary.IsChecked = true;
                }
                else if (storedDisplayName.Contains("Duct"))
                {
                    rdoUfm.IsChecked = true;
                    _ufmFilter.Apply();
                }
                else
                {
                    rdoDefault.IsChecked = true;
                }
                RestoreEdMasterCheckbox(storedDisplayName);
            }
            else // something else chosen
            {
                ResetUiState();
                _ufmTabItem.Visibility = Visibility.Collapsed;
            }

            _isResetting = false;
        }

        private IList<int> GetLayerIds(XElement pseudoGroupLayerElement, bool layersOn)
        {
            string pseudoGroupLayerIDCSV;
            IList<string> pseudoGroupLayersString = null;
            IList<int> pseudoGroupLayerLayerIds = new List<int>();
            string layersOnOff = layersOn ? "LayersOn" : "LayersOff";

            if (pseudoGroupLayerElement.Elements(layersOnOff).Any())
            {
                pseudoGroupLayerIDCSV = pseudoGroupLayerElement.Element(layersOnOff).Attribute("ids").Value;
                pseudoGroupLayersString = pseudoGroupLayerIDCSV.Split(',').ToList();
                pseudoGroupLayerLayerIds = pseudoGroupLayersString.Select(x => Int32.Parse(x)).ToList();
            }

            return pseudoGroupLayerLayerIds;
        }

        public void InitializeConfiguration(Map map, MapTools mapTools, XElement element, TabItem ufmTabItem, Grid mapArea)
        {
            _ufmTabItem = ufmTabItem;
            _map = map;
            _mapTools = mapTools;
            PseudoGroupLayerFilter filter = new PseudoGroupLayerFilter(SELECT_PLACEHOLDER, null, null, null, null, null, null, null, false);
            _pseudoGroupLayerFilters.Add(SELECT_PLACEHOLDER, filter);

            foreach (XElement pseudoGroupLayerElement in element.Descendants("PseudoGroupLayer"))
            {
                string pseudoGroupLayerName = pseudoGroupLayerElement.Attribute("name").Value;
                string pseudoGroupLayerUrl = pseudoGroupLayerElement.Attribute("url").Value;
                bool useIsEnabled = Convert.ToBoolean(pseudoGroupLayerElement.Attribute("useIsEnabled").Value);
                IList<int> pseudoGroupLayersOnIds = GetLayerIds(pseudoGroupLayerElement, true);
                IList<int> pseudoGroupLayersOffIds = GetLayerIds(pseudoGroupLayerElement, false); 

                IDictionary<int, string> queries = new Dictionary<int, string>();

                foreach (XElement definitionQueryElement in pseudoGroupLayerElement.Descendants("DefinitionQuery"))
                {
                    int layerId = Convert.ToInt32(definitionQueryElement.Attribute("LayerId").Value);
                    string query = definitionQueryElement.Attribute("query").Value;
                    queries.Add(layerId, query);
                }
                XElement symbolChange50ScaleElement = pseudoGroupLayerElement.Element("SymbolChange50Scale");
                PseudoGroupLayerFilter pseudoGroupLayerFilter = new PseudoGroupLayerFilter(pseudoGroupLayerName, pseudoGroupLayerUrl,
                    pseudoGroupLayersOnIds, pseudoGroupLayersOffIds, queries, _map, _mapTools, symbolChange50ScaleElement, useIsEnabled);
                _pseudoGroupLayerFilters.Add(pseudoGroupLayerName, pseudoGroupLayerFilter);

            }
            _electricDistribution50Filter = _pseudoGroupLayerFilters["ElectricDistribution50EdmFilter"];
            _priSec50EdmFilter = _pseudoGroupLayerFilters["PriSec50EdmFilter"];
            _ufm50EdmFilter = _pseudoGroupLayerFilters["Ufm50EdmFilter"];
            _ufmFilter = _pseudoGroupLayerFilters["Ufm50Filter"];
            _ufmPrintTool = new UfmPrintTool(UFMPrintToggleButton, map, "", mapArea);
        }


        private void RdoPrimary_OnChecked(object sender, RoutedEventArgs e)
        {
            _mapTools.StoredViewControl.SetStoredViewByName("Elec Dist 50 Scale Primary View");
        }

        private void RdoDefault_OnChecked(object sender, RoutedEventArgs e)
        {
            if (rdoDefault == null || _isResetting) return;

            _mapTools.StoredViewControl.SetStoredViewByName("Elec Dist 50 Scale");
        }

        private void chkEdMasterFilter_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_isResetting) return;

            _edmasterFilterApplied[_currentStoredDisplayName] = true;

            PseudoGroupLayerFilter pseudoGroupLayerFilter = GetSelectedPseudoGroupLayerFilter();
            pseudoGroupLayerFilter.Apply();
        }

        private void RestoreEdMasterCheckbox(string storedDisplayName)
        {
            _isResetting = true;
            chkEdMasterFilter.IsChecked = _edmasterFilterApplied[storedDisplayName];
            _isResetting = false;
        }
        private void chkEdMasterFilter_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (_isResetting) return;

            _edmasterFilterApplied[_currentStoredDisplayName] = false;

            PseudoGroupLayerFilter pseudoGroupLayerFilter = GetSelectedPseudoGroupLayerFilter();
            pseudoGroupLayerFilter.Clear();
        }

        private PseudoGroupLayerFilter GetSelectedPseudoGroupLayerFilter()
        {
            if (rdoDefault.IsChecked.Value)
            {
                return _electricDistribution50Filter;
            }
            if (rdoPrimary.IsChecked.Value || rdoSecondary.IsChecked.Value)
            {
                return _priSec50EdmFilter;
            }

            return _ufm50EdmFilter;
        }

        private void RdoSecondary_OnChecked(object sender, RoutedEventArgs e)
        {
            _mapTools.StoredViewControl.SetStoredViewByName("Elec Dist 50 Scale Secondary View");
        }

        private void RdoUfm_OnChecked(object sender, RoutedEventArgs e)
        {
            _mapTools.StoredViewControl.SetStoredViewByName("Elec Dist 50 Scale Duct View");
        }
    }


}
