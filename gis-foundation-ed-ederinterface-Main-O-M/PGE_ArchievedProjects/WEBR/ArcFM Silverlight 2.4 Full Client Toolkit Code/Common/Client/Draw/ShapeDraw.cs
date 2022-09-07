using System;
using System.Windows;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    internal class ShapeDraw : Draw
    {
        public ShapeDraw()
            : base()
        {
            DrawComplete += new EventHandler<DrawEventArgs>(ShapeDraw_DrawComplete);
        }

        public ShapeDraw(Map map, DrawMode mode)
            : base(map)
        {
            DrawMode = mode;
            DrawComplete += new EventHandler<DrawEventArgs>(ShapeDraw_DrawComplete);
        }

        public event EventHandler<ShapeDrawEventArgs> ShapeDrawComplete;

        public bool IsShapeEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsShapeEnabledProperty =
            DependencyProperty.Register("IsShapeEnabled", typeof(bool), typeof(Draw), new PropertyMetadata(OnIsShapeEnabledChanged));

        private static readonly DependencyProperty ActiveDrawProperty = DependencyProperty.RegisterAttached("ActiveDraw", typeof(ShapeDraw), typeof(ShapeDraw), null);

        private static void OnIsShapeEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShapeDraw draw = (ShapeDraw)d;
            draw.IsEnabled = (bool)e.NewValue;
        }

        void ShapeDraw_DrawComplete(object sender, DrawEventArgs e)
        {
            if (e.Geometry == null)
            {
                return;
            }

            ShapeDrawEventArgs args = new ShapeDrawEventArgs();
            args.Geometry = e.Geometry;
            this.OnDrawComplete(args);
        }

        private void OnDrawComplete(ShapeDrawEventArgs args)
        {
            EventHandler<ShapeDrawEventArgs> handler = this.ShapeDrawComplete;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}
