using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Json;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using Miner.Silverlight.Logging.Client;
using NLog;

namespace ArcFMSilverlight
{
    public partial class JetToggle : UserControl
    {
        public Map MapControl { get; set; }
        private XElement _element;
        private JetJobsTool _jetJobsTool = null;
        // Cache everything or just element?
        private Grid _mapArea;
        private MapTools _mapTools;
        private Grid _appArea;

        public JetToggle()
        {
            InitializeComponent();
        }

        public void InitializeConfiguration(XElement element, Map mapControl, Grid mapArea, MapTools mapTools)
        {
            MapControl = mapControl;
            _mapArea = mapArea;
            _element = element;
            _mapTools = mapTools;
        }

        public void JobsToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (JobsToggleButton.IsChecked.HasValue)
            {
                if (_jetJobsTool == null)
                {
                    _jetJobsTool = new JetJobsTool(this.MapControl, _mapArea, this.JobsToggleButton, _element, _mapTools);
                }
                else
                {
                    _jetJobsTool.RefreshJobList();
                }
                _jetJobsTool.SetVisible(JobsToggleButton.IsChecked.Value);
            }

        }

    }
}
