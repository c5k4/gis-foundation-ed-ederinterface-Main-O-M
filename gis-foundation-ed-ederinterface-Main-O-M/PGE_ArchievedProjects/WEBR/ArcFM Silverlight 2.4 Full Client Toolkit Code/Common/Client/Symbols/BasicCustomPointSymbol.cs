using System.Windows;
using System.Windows.Media;

using ESRI.ArcGIS.Client.Symbols;

#if SILVERLIGHT
namespace Miner.Server.Client.Symbols
#elif WPF
namespace Miner.Mobile.Client.Symbols
#endif
{
    /// <summary>
    /// Base class for Custom Point Symbols
    /// </summary>
    public class BasicCustomPointSymbol : MarkerSymbol
    {
        public ImageSource Source
        {
            get
            {
                return (ImageSource)base.GetValue(SourceProperty);
            }
            set
            {
                base.SetValue(SourceProperty, value);
            }
        }

        public double Width
        {
            get
            {
                return (double)base.GetValue(WidthProperty);
            }
            set
            {
                base.SetValue(WidthProperty, value);

                if (!double.IsInfinity(value))
                {
                    base.SetValue(SizeProperty, new Point(value, Size.Y));
                }
            }
        }

        public double Height
        {
            get
            {
                return (double)base.GetValue(HeightProperty);
            }
            set
            {
                base.SetValue(HeightProperty, value);

                if (!double.IsInfinity(value))
                {
                    base.SetValue(SizeProperty, new Point(Size.X, value));
                }
            }
        }

        public Point Size
        {
            get
            {
                return (Point)base.GetValue(SizeProperty);
            }
            set
            {
                base.SetValue(SizeProperty, value);
            }
        }

        public double Angle
        {
            get
            {
                return (double)base.GetValue(AngleProperty);
            }
            set
            {
                base.SetValue(AngleProperty, value);
            }
        }

        public Brush SelectionColor
        {
            get
            {
                return (Brush)base.GetValue(SelectionColorProperty);
            }
            set
            {
                base.SetValue(SelectionColorProperty, value);
            }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source",
            typeof(ImageSource),
            typeof(BasicCustomPointSymbol),
            null);

        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width",
            typeof(double),
            typeof(BasicCustomPointSymbol),
            null);

        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height",
            typeof(double),
            typeof(BasicCustomPointSymbol),
            null);

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size",
            typeof(Point),
            typeof(BasicCustomPointSymbol),
            null);

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            "Angle",
            typeof(double),
            typeof(BasicCustomPointSymbol),
            null);

        public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register(
            "SelectionColor",
            typeof(Brush),
            typeof(BasicCustomPointSymbol),
            null);
    }
}