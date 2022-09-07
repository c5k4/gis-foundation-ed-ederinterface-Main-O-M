using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NLog;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Actions;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using System.Collections;

namespace ArcFMSilverlight
{
    public partial class RemoveDataControl : UserControl
    {
        #region Variables

        private Logger logger = LogManager.GetCurrentClassLogger();
        private Map _map;
        public List<TempLayer> TempLayerList;
        private MapTools _mapTools;

        #endregion Variables

        #region Constructor

        public RemoveDataControl(Map map, MapTools tools)
        {
            InitializeComponent();
            _map = map;
            _mapTools = tools;
            cboLayerList.ItemsSource = TempLayerList;
        }

        #endregion Constructor

        #region public methods
        // This method is used to fill drop down with temp layers added with Add Data functionality
        public void FillLayerCboBox()
        {
            List<string> layerList = new List<string>();
            foreach (TempLayer layer in TempLayerList)
            {
                layerList.Add(layer.title);
            }
            cboLayerList.ItemsSource = layerList;
            EnableDisableRemoveBtn();
        }
        #endregion public methods

        #region private methods
        private void EnableDisableRemoveBtn()
        {
            if (cboLayerList.SelectedItem != null)
                RemoveLayerBtn.IsEnabled = true;
            else
                RemoveLayerBtn.IsEnabled = false;
        }
        #endregion private methods

        #region events
        // This method get the selected value from drop down and remove it from map instance
        private void RemoveLayerBtn_Click(object sender, RoutedEventArgs e)
        {
             try {
                string selectedLayerName = cboLayerList.SelectedItem.ToString();
                foreach (TempLayer tempLayer in TempLayerList)
                {
                    if (tempLayer.title == selectedLayerName)
                    {
                        Layer selectedLayer = tempLayer.layer;
                        _map.Layers.Remove(selectedLayer);
                        if (tempLayer.title.Split('-').Last() == "(Service)")
                        {
                            ConfigUtility.StatusBar.Text = "Service Removed : " + tempLayer.title.Substring(0, tempLayer.title.LastIndexOf('-'));
                        }
                        else{
                            ConfigUtility.StatusBar.Text = "Layer Removed : " + tempLayer.title.Substring(0, tempLayer.title.LastIndexOf('-')); ;
                        }
                        TempLayerList.Remove(tempLayer);

                        //update TOC - Toggle layers
                        _mapTools.LayerControl.RemoveLayerFromTOC(_mapTools.LayerControl, _map, selectedLayer.ID);

                        break;
                    }
                }
                FillLayerCboBox();
                 //update TOC
            }
            catch (Exception ex) {
                MessageBox.Show("Unable to remove Service/Layer", "Message", MessageBoxButton.OK);
            }
        }

        private void cboLayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableDisableRemoveBtn();
        }

        #endregion events
    }
}
