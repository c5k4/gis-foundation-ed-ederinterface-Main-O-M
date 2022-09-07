using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
#if WPF
using System.IO;
using System.Text;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Symbols
#elif WPF
namespace Miner.Mobile.Client.Symbols
#endif
{
    /// <summary>
    /// Custom Text Symbol for scaling and rotation
    /// </summary>
    public class CustomTextSymbol : BasicCustomPointSymbol
    {
        private const string NameSpace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        public CustomTextSymbol()
        {
            Visibility = Visibility.Collapsed;

            string template =
              @"<ControlTemplate xmlns=""" + NameSpace + @"""
                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                 xmlns:miner=""http://schemas.miner.com/arcfm/client/2010""" +
#if SILVERLIGHT
              @"                 xmlns:controls=""clr-namespace:Miner.Server.Client.Controls;assembly=Miner.Server.Client"">" +
#elif WPF
              @"                 xmlns:controls=""clr-namespace:Miner.Mobile.Client.Controls;assembly=Miner.Mobile.Client"">" +
#endif
              @"    <Grid RenderTransformOrigin=""0.5,0.5"">
                        <Grid.Resources>
                            <miner:PointSizeToMarginConverter x:Key=""PointSizeToMarginConverter"" />
                        </Grid.Resources>

                        <Grid.RenderTransform>
                            <RotateTransform Angle=""{Binding Symbol.Angle}"" />
                        </Grid.RenderTransform>

                        <Grid.Margin>
                            <Binding Path=""Symbol.Size"" Converter=""{StaticResource PointSizeToMarginConverter}"" />
                        </Grid.Margin>

                        <controls:LayoutTransform Xaml=""{Binding Symbol.Xaml}"" ScaleFactor=""{Binding Symbol.ScaleFactor}"" Visibility=""{Binding Symbol.Visibility}"" />
                    </Grid>
                </ControlTemplate>";

#if SILVERLIGHT
            ControlTemplate = (ControlTemplate)XamlReader.Load(template);
#elif WPF
            var array = Encoding.UTF8.GetBytes(template);
            var stream = new MemoryStream(array);
            ControlTemplate = (ControlTemplate)XamlReader.Load(stream);
#endif
        }

        public event EventHandler SourceChanged;

        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)base.GetValue(FontFamilyProperty);
            }
            set
            {
                base.SetValue(FontFamilyProperty, value);
            }
        }

        public short FontSize
        {
            get
            {
                return (short)base.GetValue(FontSizeProperty);
            }
            set
            {
                base.SetValue(FontSizeProperty, value);
            }
        }

        public Brush Foreground
        {
            get
            {
                return (Brush)base.GetValue(ForegroundProperty);
            }
            set
            {
                base.SetValue(ForegroundProperty, value);
            }
        }

        public Brush Background
        {
            get
            {
                return (Brush)base.GetValue(BackgroundProperty);
            }
            set
            {
                base.SetValue(BackgroundProperty, value);
            }
        }

        public Brush BorderBrush
        {
            get
            {
                return (Brush)base.GetValue(BorderBrushProperty);
            }
            set
            {
                base.SetValue(BorderBrushProperty, value);
            }
        }

        public string Xaml
        {
            get
            {
                return (string)base.GetValue(XamlProperty);
            }
            set
            {
                base.SetValue(XamlProperty, value);
            }
        }

        public string UncompressedXaml
        {
            get
            {
                return (string)base.GetValue(UncompressedXamlProperty);
            }
            set
            {
                base.SetValue(UncompressedXamlProperty, value);
            }
        }

        public string CompressedXaml
        {
            get
            {
                return (string)base.GetValue(CompressedXamlProperty);
            }
            set
            {
                base.SetValue(CompressedXamlProperty, value);
            }
        }

        public double ScaleFactor
        {
            get
            {
                return (double)base.GetValue(ScaleFactorProperty);
            }
            set
            {
                base.SetValue(ScaleFactorProperty, value);
            }
        }

        public bool Selected
        {
            get
            {
                return (bool)base.GetValue(SelectedProperty);
            }
            set
            {
                base.SetValue(SelectedProperty, value);
            }
        }

        public Visibility Visibility
        {
            get
            {
                return (Visibility)base.GetValue(VisibilityProperty);
            }
            set
            {
                base.SetValue(VisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
            "FontFamily",
            typeof(FontFamily),
            typeof(CustomTextSymbol),
            null);

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize",
            typeof(short),
            typeof(CustomTextSymbol),
            null);

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground",
            typeof(Brush),
            typeof(CustomTextSymbol),
            null);

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(Brush),
            typeof(CustomTextSymbol),
            null);

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush",
            typeof(Brush),
            typeof(CustomTextSymbol),
            null);

        public static readonly DependencyProperty XamlProperty = DependencyProperty.Register(
            "Xaml",
            typeof(string),
            typeof(CustomTextSymbol),
            new PropertyMetadata(OnTextChanged));

        public static readonly DependencyProperty CompressedXamlProperty = DependencyProperty.Register(
            "CompressedXaml",
            typeof(string),
            typeof(CustomTextSymbol),
            new PropertyMetadata(OnCompressedXamlChanged));

        public static readonly DependencyProperty UncompressedXamlProperty = DependencyProperty.Register(
            "UncompressedXaml",
            typeof(string),
            typeof(CustomTextSymbol),
            null);

        public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.Register(
            "ScaleFactor",
            typeof(double),
            typeof(CustomTextSymbol),
            null);

        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
            "Selected",
            typeof(bool),
            typeof(CustomTextSymbol),
            null);

        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register(
            "Visibility",
            typeof(Visibility),
            typeof(CustomTextSymbol),
            null);

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var symbol = (CustomTextSymbol)d;

            if (e.NewValue.ToString() != "")
            {
                symbol.Visibility = Visibility.Visible;
            }
        }

        private static void OnCompressedXamlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var symbol = (CustomTextSymbol)d;

            try
            {
                var convertedXaml = symbol.CompressedXaml.Replace("(", "<").Replace(")", ">").Replace("&lb;", "(").Replace("&rb;", ")");
                XElement.Parse(convertedXaml);
                symbol.UncompressedXaml = UncompressXaml(convertedXaml);
            }
            catch
            {
                var xaml = symbol.CompressedXaml.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r\n", "&#13;").Replace("\r", "&#13;").Replace("\n", "&#13;");
                xaml = xaml.Replace("&#13;", @"</Run></Paragraph><Paragraph FontFamily=""Arial"" FontSize=""12"" Foreground=""Black""><Run>");
                symbol.UncompressedXaml = @"<Section xml:space=""preserve"" HasTrailingParagraphBreakOnPaste=""False"" xmlns=""" + CustomTextSymbol.NameSpace + @"""><Paragraph FontFamily=""Arial"" FontSize=""12"" Foreground=""Black""><Run>" + xaml + @"</Run></Paragraph></Section>";
            }
          
            symbol.Xaml = ConvertXaml(symbol.UncompressedXaml);
        }

        internal void GenerateTextImage()
        {
            var imageTemplate =
              @"<UserControl xmlns=""" + NameSpace + @"""
                             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                    <Border x:Name=""SymbolBorder"" />
                </UserControl>";

#if SILVERLIGHT
            var control = (Control)XamlReader.Load(imageTemplate);
            (control.FindName("SymbolBorder") as Border).Child = (TextBlock)XamlReader.Load(Xaml);

            var bitmap = new WriteableBitmap(control, null);
#elif WPF
            var array = Encoding.UTF8.GetBytes(imageTemplate);
            var stream = new MemoryStream(array);
            var control = (Control)XamlReader.Load(stream);

            array = Encoding.UTF8.GetBytes(Xaml);
            stream = new MemoryStream(array);
            (control.FindName("SymbolBorder") as Border).Child = (TextBlock)XamlReader.Load(stream);

            var bitmap = new RenderTargetBitmap((int)control.RenderSize.Width, (int)control.RenderSize.Height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(control);
#endif
            Source = bitmap;

            EventHandler handler = this.SourceChanged;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        private static string ConvertXaml(string xaml)
        {
            if (xaml.IndexOf("TextBlock") >= 0)
            {
                return xaml;
            }

            var blocks = new XElement(XName.Get("TextBlock", NameSpace));
            blocks.SetAttributeValue("FontFamily", "Arial");
            blocks.SetAttributeValue("FontSize", "12");
            blocks.SetAttributeValue("Foreground", "Black");

            XElement section = XElement.Parse(xaml);
            foreach (var paragraph in section.Elements())
            {
                foreach (var element in paragraph.Elements().Where(e => e.Name.LocalName == "Run"))
                {
                    foreach (var attribute in paragraph.Attributes())
                    {
                        var originalAttribute = element.Attributes().FirstOrDefault(a => a.Name.LocalName == attribute.Name.LocalName);
                        if (originalAttribute != null)
                        {
                            element.SetAttributeValue(attribute.Name, originalAttribute.Value);
                        }
                        else
                        {
                            element.SetAttributeValue(attribute.Name, attribute.Value);
                        }
                    }

                    blocks.Add(element);
                }

                if (paragraph != section.Elements().Last())
                {
                    blocks.Add(new XElement(XName.Get("LineBreak", NameSpace)));
                }
            }

            return blocks.ToString().Replace(@" xmlns=""""", "").Replace("\r\n  ", "").Replace(">  ", ">");
        }

        private static string UncompressXaml(string xaml)
        {
            var blocks = new XElement(XName.Get("Section", NameSpace));
            blocks.SetAttributeValue(XNamespace.Xml + "space", "preserve");
            blocks.SetAttributeValue("HasTrailingParagraphBreakOnPaste", "False");

            XElement section = XElement.Parse(xaml);
            foreach (var paragraph in section.Elements())
            {
                var line = new XElement("Paragraph");
                foreach (var element in paragraph.Elements().Where(e => e.Name.LocalName == "R"))
                {
                    var run = new XElement("Run");
                    run.Value = element.Value.Replace(" ", "&#160;");

                    foreach (var attribute in element.Attributes())
                    {
                        switch (attribute.Name.LocalName)
                        {
                            case "NM":
                                run.SetAttributeValue("FontFamily", attribute.Value);
                                break;
                            case "SZ":
                                run.SetAttributeValue("FontSize", attribute.Value);
                                break;
                            case "FG":
                                run.SetAttributeValue("Foreground", attribute.Value);
                                break;
                            case "WT":
                                run.SetAttributeValue("FontWeight", attribute.Value);
                                break;
                            case "ST":
                                run.SetAttributeValue("FontStyle", attribute.Value);
                                break;
                            case "DC":
                                run.SetAttributeValue("TextDecorations", attribute.Value);
                                break;
                        };
                    }

                    line.Add(run);
                }

                blocks.Add(line);
            }

            return blocks.ToString().Replace(@" xmlns=""""", "").Replace("\r\n  ", "").Replace(">  ", ">").Replace("&amp;#160;", "&#160;");
        }
    }
}