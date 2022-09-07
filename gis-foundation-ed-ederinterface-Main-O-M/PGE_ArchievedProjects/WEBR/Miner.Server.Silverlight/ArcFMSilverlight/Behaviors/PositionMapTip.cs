using System;
using System.Collections.Specialized;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ArcFMSilverlight.Controls.ShowRolloverInfo;
using ESRI.ArcGIS.Client;

namespace ArcFMSilverlight.Behaviors
{
    public class PositionMapTip : Behavior<Map>
    {
        private Point _mousePos; // Track the position of the mouse on the Map

        /// Distance between the MapTip and the boundary of the map
        public double Margin { get; set; }

        // Called after the behavior is attached to an AssociatedObject.
        // Override this to hook up functionality to the AssociatedObject.
        protected override void OnAttached()
        {
            base.OnAttached();

            if (this.AssociatedObject == null || this.AssociatedObject.Layers == null)
                return;

            // Wire layer collection changed handler to monitor adding/removal of GraphicsLayers
            this.AssociatedObject.Layers.CollectionChanged += Layers_CollectionChanged;

            //foreach (Layer layer in this.AssociatedObject.Layers)
            //    wireHandlers(layer as GraphicsLayer);
        }

        // Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        // Override this to unhook functionality from the AssociatedObject.
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject == null || this.AssociatedObject.Layers == null)
                return;

            foreach (Layer layer in this.AssociatedObject.Layers)
                removeHandlers(layer as GraphicsLayer);
        }

        // Add/remove MapTip positioning handlers for added/removed GraphicsLayers
        void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Layer layer in e.NewItems)
                {
                    if (layer is FeatureLayer && layer.ID != null &&
                        layer.ID.StartsWith(ShowRolloverInfo.ROLLOVER_FEATURELAYER_PREFIX))
                    {
                        wireHandlers(layer as GraphicsLayer);
                    }
                }

            if (e.OldItems != null)
                foreach (Layer layer in e.OldItems)
                    if (layer is FeatureLayer && layer.ID != null &&
                        layer.ID.StartsWith(ShowRolloverInfo.ROLLOVER_FEATURELAYER_PREFIX))
                    {
                        removeHandlers(layer as GraphicsLayer);
                    }
        }

        // Add MapTip positioning handlers
        public void wireHandlers(GraphicsLayer graphicsLayer)
        {
            if (graphicsLayer == null) return;

            // Add handlers to get the position of the mouse on the map
            graphicsLayer.MouseEnter += GraphicsLayer_MouseEnterOrMove;
//            graphicsLayer.MouseMove += GraphicsLayer_MouseEnterOrMove;

            if (graphicsLayer.MapTip != null)
                graphicsLayer.MapTip.SizeChanged += MapTip_SizeChanged;
        }

        // Remove MapTip positioning handlers
        private void removeHandlers(GraphicsLayer graphicsLayer)
        {
            if (graphicsLayer == null) return;
            graphicsLayer.MouseEnter -= GraphicsLayer_MouseEnterOrMove;
//            graphicsLayer.MouseMove -= GraphicsLayer_MouseEnterOrMove;

            if (graphicsLayer.MapTip != null)
                graphicsLayer.MapTip.SizeChanged -= MapTip_SizeChanged;
        }

        // Get mouse position
        void GraphicsLayer_MouseEnterOrMove(object sender, GraphicMouseEventArgs args)
        {
            _mousePos = args.GetPosition(AssociatedObject);
        }

        // Position and size MapTip
        void MapTip_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AssociatedObject == null)
                return;

            FrameworkElement mapTip = sender as FrameworkElement;
            if (mapTip == null)
                return;

            // Determine what quadrant of the Map the mouse is in
            bool upper = _mousePos.Y < AssociatedObject.ActualHeight / 2;
            bool right = _mousePos.X > AssociatedObject.ActualWidth / 2;

            if (mapTip.ActualHeight > 0)
            {
                double maxHeight;
                double maxWidth;
                double horizontalOffset;
                double verticalOffset;

                // Calculate max dimensions
                maxHeight = upper ? AssociatedObject.ActualHeight - _mousePos.Y - Margin :
                    _mousePos.Y - Margin;
                maxWidth = right ? _mousePos.X - Margin : AssociatedObject.ActualWidth
                    - _mousePos.X - Margin;


                //Calculate offsets for MapTip
                verticalOffset = upper ? 0 : (int)(0 - mapTip.ActualHeight);
                horizontalOffset = right ? (int)(0 - mapTip.ActualWidth) : 0;

                if (verticalOffset != 0 || horizontalOffset != 0)
                {
                    // Apply dimensions and offsets.  MapTip should not extend outside the map.
                    mapTip.MaxHeight = maxHeight;
                    mapTip.MaxWidth = maxWidth;

                    // Set horizontal and vertical offset dependency properties on the MapTip
                    mapTip.SetValue(GraphicsLayer.MapTipHorizontalOffsetProperty, horizontalOffset);
                    mapTip.SetValue(GraphicsLayer.MapTipVerticalOffsetProperty, verticalOffset);
                }
            }
        }
    }
}
