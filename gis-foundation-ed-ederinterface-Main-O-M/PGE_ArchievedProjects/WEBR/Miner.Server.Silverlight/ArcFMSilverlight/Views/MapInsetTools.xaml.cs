using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using ESRI.ArcGIS.Client;

using ArcFMSilverlight.ViewModels;

namespace ArcFMSilverlight
{
    /// <summary>
    ///   MagnifyingTools control supporting pan, zoom and identify in an inset map.
    /// </summary>
    public partial class MapInsetTools : UserControl
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "MapInsetTools" /> class.
        /// </summary>
        public MapInsetTools()
        {
            InitializeComponent();

            if (!DesignerProperties.IsInDesignTool)
            {
                ViewModel = new MapInsetToolsViewModel();
            }
        }

        public MapInsetToolsViewModel ViewModel
        {
            get { return DataContext as MapInsetToolsViewModel; }
            set { DataContext = value; }
        }

        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(MapInsetTools),
            new PropertyMetadata(OnMapChanged));

        public Map InsetMap
        {
            get { return (Map)GetValue(InsetMapProperty); }
            set { SetValue(InsetMapProperty, value); }
        }

        public static readonly DependencyProperty InsetMapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(MapInsetTools),
            new PropertyMetadata(OnInsetMapChanged));

        public double Resolution
        {
            get { return (double)GetValue(ResolutionProperty); }
            set { SetValue(ResolutionProperty, value); }
        }

        public static readonly DependencyProperty ResolutionProperty = DependencyProperty.Register(
            "Resolution",
            typeof(double),
            typeof(MapInsetTools),
            new PropertyMetadata(OnResolutionChanged));

        public double UnscaledMarkerSize
        {
            get { return (double)GetValue(UnscaledMarkerSizeProperty); }
            set { SetValue(UnscaledMarkerSizeProperty, value); }
        }

        public static readonly DependencyProperty UnscaledMarkerSizeProperty = DependencyProperty.Register(
            "UnscaledMarkerSize",
            typeof(double),
            typeof(MapInsetTools),
            new PropertyMetadata(OnUnscaledMarkerSizeChanged));

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                var tools = (MapInsetTools)d;
                tools.ViewModel.Map = e.NewValue as Map;
            }
        }

        private static void OnInsetMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                var tools = (MapInsetTools)d;
                tools.InsetTOC.Map = tools.ViewModel.InsetMap = e.NewValue as Map;
                tools.ViewModel.InsetTOC = tools.InsetTOC;
            }
        }

        private static void OnResolutionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                var tools = (MapInsetTools)d;
                tools.ViewModel.Resolution = (double)e.NewValue;
            }
        }

        private static void OnUnscaledMarkerSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                var tools = (MapInsetTools)d;
                tools.ViewModel.UnscaledMarkerSize = (double)e.NewValue;
            }
        }
    }
}