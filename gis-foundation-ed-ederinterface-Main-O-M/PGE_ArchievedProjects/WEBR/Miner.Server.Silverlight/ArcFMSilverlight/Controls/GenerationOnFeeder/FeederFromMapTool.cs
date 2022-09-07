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
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit.Events;
using System.Collections.Generic;
using ESRI.ArcGIS.Client;
using System.ComponentModel;
using ArcFMSilverlight.Controls.GenerationOnFeeder;

namespace ArcFMSilverlight
{

    public class FeederFromMapTool : Control, IActiveControl
    {

        private FeederFromMap _FeederFromMapTool;
        public FloatableWindow _fw = null;
        private Grid _mapArea;
        private Map _map;
        private string _popUpTitle;


        //public FeederFromMapTool(Grid mapArea,Map map)
        //{
        //    _mapArea = mapArea;
        //    _map = map;

        //}

        // ENOSTOEDGIS - Changes for adding CGC to caption
        public void OpenDialog(Grid mapArea, Map map, FeederFromMap _FeederFromMapTool, string CircuitId)
        {
            _map = map;
            _fw = new FloatableWindow();
            _fw.ParentLayoutRoot = mapArea;
            //_fw.Width = 557;
            // _fw.Height = 290;
            //_fw.Height = 210;           
            if (!string.IsNullOrEmpty(CircuitId))
            {
                _popUpTitle = "Generation On Feeder (Feeder Number: " + CircuitId + ")"; // ENOS Tariff Change- Feeder number parameter added on grid title
            }
            else
            {
                _popUpTitle = "Generation On Feeder";
            }
            //  _popUpTitle = "Generation On Feeder";

            _fw.Title = _popUpTitle;
            // _fw.Title = "Generation On Transformer";
            _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            _fw.ResizeMode = ResizeMode.NoResize;
            _fw.Content = _FeederFromMapTool;
            _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
            _fw.Show();

        }

        private void _fw_Closing(object sender, CancelEventArgs e)
        {
            string layerId = GetGenOnFeeder.feederZoomGraphicID;
            // GraphicsLayer layer = this.Map.Layers[layerId] as GraphicsLayer;
            GraphicsLayer layer = _map.Layers[layerId] as GraphicsLayer;
            if (layer != null)
            {
                layer.Graphics.Clear();
            }


            this.IsActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            ConfigUtility.UpdateStatusBarText("");
        }
        #region IActiveControl Members

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set { setActive(value); }
        }

        private void setActive(bool isActive)
        {
            _isActive = isActive;
            if (_fw == null) return;

            if (!isActive)
            {
                // Clear everything
                //disable draw polygon

                _fw.Visibility = System.Windows.Visibility.Collapsed;

            }
            else // Active
            {

                _fw.Visibility = System.Windows.Visibility.Visible;
            }

        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(FeederFromMapTool), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var genOnTransformerTool = (FeederFromMapTool)d;

            var isActive = (bool)e.NewValue;

            if (isActive == false)
            {
                //                measure.MeasureMode = null;
            }

        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsActive = false;
        }

        #endregion IActiveControl Members

    }


}
