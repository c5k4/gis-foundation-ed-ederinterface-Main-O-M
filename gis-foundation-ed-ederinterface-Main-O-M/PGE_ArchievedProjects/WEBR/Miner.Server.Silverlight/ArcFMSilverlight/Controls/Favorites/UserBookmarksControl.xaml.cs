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
using ArcFMSilverlight.Controls.Bookmarks;
using ArcFMSilverlight.Controls.Favorites;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using Microsoft.Expression.Shapes;
using Miner.Server.Client;
using LayerVisibility = Miner.Server.Client.Toolkit.LayerVisibility;

namespace ArcFMSilverlight
{
    public partial class UserBookmarksControl : UserControl
    {

        public UserBookmarksControl()
        {
            InitializeComponent();
        }



        public void Initialize(XElement element, MapTools mapTools, Map map)
        {
            FavoritesControl.Initialize(element, mapTools, map);
            BookmarksControl.Initialize(element, mapTools.StoredViewControl, map);
        }

        public void Load()
        {
            FavoritesControl.Load();
            BookmarksControl.Load();
        }

        /// <summary>
        /// Gets the identifier for the <see cref="Map"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(UserBookmarksControl),
            null);

        /// <summary>
        /// Gets or sets the Map.
        /// </summary>
        public Map Map
        {
            get
            {
                return (Map)GetValue(MapProperty);
            }
            set
            {

                SetValue(MapProperty, value);
                //InitialSetting();
            }
        }
   }
}
