using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using Miner.Server.Client;

namespace ArcFMSilverlight.Controls.Ufm
{
    public class PseudoGroupLayerFilter
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public IList<int> LayerIds { get; private set; }
        public IList<int> LayerIdsOn { get; private set; }
        public IList<int> LayerIdsOff { get; private set; }
        private IDictionary<int, string> _pqlQueries = new Dictionary<int, string>();
        private Map _map;
        private MapTools _mapTools;
        private bool _useIsEnabled = false;
        private bool _turnOff = false;
        private IDictionary<int, LayerDefinition> _layerDefinitions = new Dictionary<int, LayerDefinition>();

        private int _conduitSystemLayerId = -1;
        private ArcGISDynamicMapServiceLayer _mapServiceLayer = null;
        private IDictionary<string, ArcGISDynamicMapServiceLayer> _mapServiceLayers; 

        public ArcGISDynamicMapServiceLayer MapServiceLayer
        {
            get
            {
                return _mapServiceLayer;
            }
        }

        private ESRI.ArcGIS.Client.Toolkit.Legend LegendTOC
        {
            get
            {
                return _mapTools.LayerControl.TOC;
            }
        }


        public PseudoGroupLayerFilter(string name, string url, IList<int> layerIdsOn, IList<int> layerIdsOff, IDictionary<int, string> pqlQueries, Map map, MapTools mapTools, XElement symbolChange50ScaleElement, bool useIsEnabled)
        {
            Name = name;
            Url = url;
            LayerIdsOn = layerIdsOn;
            LayerIdsOff = layerIdsOff;
            _pqlQueries = pqlQueries;
            _map = map;
            _mapTools = mapTools;
            _useIsEnabled = useIsEnabled;
            if (_map != null)
            {
                SetMapServiceLayers();
            }

            if (pqlQueries != null)
            {
                foreach (KeyValuePair<int, string> keyValuePair in pqlQueries)
                {
                    _layerDefinitions.Add(keyValuePair.Key, new LayerDefinition() { Definition = keyValuePair.Value, LayerID = keyValuePair.Key });
                }
            }
            if (symbolChange50ScaleElement != null)
            {
                _conduitSystemLayerId = Convert.ToInt32(symbolChange50ScaleElement.Attribute("ids").Value);
            }
        }

        private void SetMapServiceLayers()
        {
            string currentId =
                _map.Layers.Where(
                    l => l is ArcGISDynamicMapServiceLayer && ((ArcGISDynamicMapServiceLayer) l).Url == Url).First().ID;

            if (currentId.StartsWith("Elec Dist 50"))
            {
                _mapServiceLayers =
                    _map.Layers.Where(l => l is ArcGISDynamicMapServiceLayer && l.ID.StartsWith("Elec Dist 50")).ToDictionary(k => k.ID, v => (ArcGISDynamicMapServiceLayer)v);
            }
            else
            {
                _mapServiceLayers = _map.Layers.Where(
                    l => l is ArcGISDynamicMapServiceLayer && ((ArcGISDynamicMapServiceLayer)l).Url == Url).ToDictionary(k => k.ID, v => (ArcGISDynamicMapServiceLayer)v);
            }
        }

        private void SetCurrentMapService()
        {
            _mapServiceLayer = _mapServiceLayers.Where(l => l.Value.Visible).First().Value;
        }

        public void Apply()
        {
            SetCurrentMapService();
            LayerItemViewModel layerItemViewModel = GetLayerItemViewModelByLayerId(_mapServiceLayer.ID);

            if (_useIsEnabled)
            {
                RestoreIsEnabledLayers(layerItemViewModel.LayerItems, LayerIdsOn.ToArray(), true);
                RestoreIsEnabledLayers(layerItemViewModel.LayerItems, LayerIdsOff.ToArray(), false);
            }
            else
            {
                _mapServiceLayer.VisibleLayers = LayerIdsOn.ToArray();
            }

//            EnactSymbolChange50Scale();
            ApplyDefinitionQueries();
        }

        //void EnactSymbolChange50Scale()
        //{
        //    if (_mapServiceLayer != _mapServiceLayer50 || _conduitSystemLayerId == -1) return;

        //    // Get the DynamicLayerInfoCollection which contains information about the sub-layers of an ArcGISDynamicMapServiceLayer
        //    DynamicLayerInfoCollection myDynamicLayerInfoCollection = _mapServiceLayer.CreateDynamicLayerInfosFromLayerInfos();

        //    // Get the first sub-layer information (i.e. the 'Cities' or [0] sub-layer)
        //    DynamicLayerInfo conduitSystemDynamicLayerInfo = myDynamicLayerInfoCollection[_conduitSystemLayerId];

        //    // Create a LayerDrawingOptionsCollection to house the individual LayerDrawingOptions objects.
        //    LayerDrawingOptionsCollection myLayerDrawingOptionsCollection = new LayerDrawingOptionsCollection();

        //    // Create a new LayerDrawingOptions object which will change the symbology of the existing first sub-layer of 
        //    // the ArcGISDynamicMapServiceLayer.
        //    LayerDrawingOptions myLayerDrawinOptions = new LayerDrawingOptions();
        //    myLayerDrawinOptions.LayerID = conduitSystemDynamicLayerInfo.ID;
        //    myLayerDrawinOptions.Renderer = GetRenderer();
        //    myLayerDrawinOptions.ScaleSymbols = true;
        //    // Apply the new settings of the Dynamic Layer and refresh the ArcGISDynamicMapServiceLayer. The Refresh Method
        //    // will make this client-side request to the ArcGIS Server to re-render the ArcGISDynamicMapServiceLayer using 
        //    // the newly defined symbology.
        //    _mapServiceLayer.LayerDrawingOptions = myLayerDrawingOptionsCollection;
        //    _mapServiceLayer.LayerDrawingOptions.Add(myLayerDrawinOptions);
        //    _mapServiceLayer.Refresh();
        //}

        private UniqueValueRenderer GetRenderer()
        {
            UniqueValueRenderer uniqueValueRenderer = new UniqueValueRenderer() { Field="SUBTYPECD"};

            //TODO: Parameterize this through page.config
            UniqueValueInfo uniqueValueInfo = new UniqueValueInfo()
            {
                Label = "Duct Bank",
                Symbol = new SimpleLineSymbol(Colors.Black, 0.5),
                Value = 1
            };
            uniqueValueRenderer.Infos.Add(uniqueValueInfo);

            UniqueValueInfo uniqueValueInfo2 = new UniqueValueInfo()
            {
                Label = "Conduit CIC",
                Symbol = new SimpleLineSymbol(Color.FromArgb(255, 0, 92, 230), 1.5) { Style = SimpleLineSymbol.LineStyle.Dash },
                Value = 2
            };
            uniqueValueRenderer.Infos.Add(uniqueValueInfo2);
            
            return uniqueValueRenderer;
        }

        private void SetDefaultLayersOn()
        {
            // Bug with the Miner event management of the LegendTOC
            int[] layersOnByDefault =
                _mapServiceLayer.Layers.Where(l => l.DefaultVisibility && l.SubLayerIds == null).Select(l => l.ID).ToArray();
            _mapServiceLayer.VisibleLayers = layersOnByDefault;
            // problem here is setting a leaf node will set its parent (group) on
            int[] grouplayersOffByDefault =
                _mapServiceLayer.Layers.Where(l => !l.DefaultVisibility && l.SubLayerIds != null).Select(l => l.ID).ToArray();
            LayerItemViewModel layerItemViewModel = GetLayerItemViewModelByLayerId(_mapServiceLayer.ID);
            RestoreIsEnabledLayers(layerItemViewModel.LayerItems, grouplayersOffByDefault, false);
            _mapServiceLayer.VisibleLayers = null;
        }

        public void Clear()
        {
            SetCurrentMapService();
            LayerItemViewModel layerItemViewModel = GetLayerItemViewModelByLayerId(_mapServiceLayer.ID);

            RestoreIsEnabledLayers(layerItemViewModel.LayerItems, LayerIdsOn.ToArray(), false);
            RestoreIsEnabledLayers(layerItemViewModel.LayerItems, LayerIdsOff.ToArray(), true);
            //SetDefaultLayersOn();
            //_mapServiceLayer.Refresh();
            //LayerVisibilityTree.InitializeTree(_map);
 
        }


        void ApplyDefinitionQueries()
        {
            foreach (KeyValuePair<int, string> pqlQuery in _pqlQueries)
            {
                _mapServiceLayer.LayerDefinitions.Add(_layerDefinitions[pqlQuery.Key]);
            }
        }

        void ClearDefinitionQueries()
        {
            foreach (KeyValuePair<int, string> pqlQuery in _pqlQueries)
            {
                _mapServiceLayer.LayerDefinitions.Remove(_layerDefinitions[pqlQuery.Key]);
            }
        }

        public LayerItemViewModel GetLayerItemViewModelByLayerId(string id)
        {
            try
            {
                //LegendTOC.UpdateLayout();
                for (int i = 0; i < LegendTOC.LayerIDs.Length; i++)
                {
                    if (LegendTOC.LayerIDs[i] == id)
                    {
                        return LegendTOC.LayerItems[i];
                    }
                }
                return null;
            }
            catch (Exception exception)
            {
                MessageBox.Show("GLTIVWMBI " + exception.ToString());
                throw;
            }
        }



        private void RestoreIsEnabledLayers(ObservableCollection<LayerItemViewModel> layerItems, int[] isEnabledLayers, bool visible)
        {
            foreach (LayerItemViewModel layerItemViewModel in layerItems)
            {
                // If this layer should be enabled but isn't currently then let's enable it
                if (isEnabledLayers.Contains(layerItemViewModel.SubLayerID) && layerItemViewModel.IsEnabled != visible)
                {
                    // NB this changes the TOC and calls an exportMap :(. Not sure how to disable that.
                    layerItemViewModel.IsEnabled = visible;
                }
                if (layerItemViewModel.LayerItems != null)
                {
                    RestoreIsEnabledLayers(layerItemViewModel.LayerItems, isEnabledLayers, visible);
                }
            }
        }

    }
}
