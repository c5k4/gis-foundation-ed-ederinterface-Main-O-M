using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ESRI.ArcGIS.Client;
#if WPF
using System;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Toolkit
#elif WPF
namespace Miner.Mobile.Client.Toolkit
#endif
{
    /// <summary>
    /// Internal class. Must be public for Silverlight XAML
    /// </summary>
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class MapHelper : UserControl
    {
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Map.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register("Map", typeof(Map), typeof(MapHelper), new PropertyMetadata(OnMapChanged));

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;

            MapHelper helper = (MapHelper)d;
            
            Map oldMap = e.OldValue as Map;
            if (oldMap != null)
            {
                oldMap.Loaded -= helper.Map_Loaded;
                if (VisualTreeHelper.GetChildrenCount(oldMap) > 0)
                {
                    Grid grid = VisualTreeHelper.GetChild(oldMap, 0) as Grid;
                    grid.MouseLeftButtonDown -= helper.UnhandleMouseLeftButton;
                    grid.MouseLeftButtonUp -= helper.UnhandleMouseLeftButton;
                }
            }
            Map newMap = e.NewValue as Map;
            if (newMap != null)
            {
                newMap.Loaded += helper.Map_Loaded;
            }
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            Map map = (Map)sender;
            if (VisualTreeHelper.GetChildrenCount(map) > 0)
            {
                Grid grid = VisualTreeHelper.GetChild(map, 0) as Grid;
                grid.MouseLeftButtonDown += UnhandleMouseLeftButton;
                grid.MouseLeftButtonUp += UnhandleMouseLeftButton;
            }
        }

        // Hack to get the map taking care of the MouseLeftButtonDown event even if
        // this event is handled by the a parent element such as a scrollviewer
        private void UnhandleMouseLeftButton(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            Dispatcher.BeginInvoke(() => e.Handled = false);
#elif WPF
            Dispatcher.BeginInvoke(new Action(() => { e.Handled = false; }));
#endif
        }
    }
}
