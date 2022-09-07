using System;
using System.Collections.Generic;
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
using ArcFM.Silverlight.PGE.CustomTools;
using ArcFMSilverlight.Controls.Favorites;
using ArcFMSilverlight.Controls.Legend;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using Miner.Server.Client;

namespace ArcFMSilverlight
{
    public partial class LegendToggle : UserControl
    {
        private LegendTool _legendTool;

        public LegendToggle()
        {
            InitializeComponent();
        }

        public void Initialize(LegendTool legendTool)
        {
            _legendTool = legendTool;
            LegendToggleButton.IsEnabled = true;
        }

        private void LegendToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (LegendToggleButton.IsChecked.HasValue)
            {
                _legendTool.IsActive = LegendToggleButton.IsChecked.Value;
            }
        }

    }
}
