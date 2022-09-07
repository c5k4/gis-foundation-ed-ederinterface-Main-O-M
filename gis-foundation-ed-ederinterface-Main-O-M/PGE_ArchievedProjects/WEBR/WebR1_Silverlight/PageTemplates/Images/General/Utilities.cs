using System;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Toolkit;
using System.Windows.Media;
using System.Reflection;
using ESRI.ArcGIS.Client.Symbols;
using System.Xml.Linq;

namespace PageTemplates
{
    public class Utilities
    {
        internal static esriUnits GetMapUnits(Map templateMap)
        {
            if (templateMap == null) return esriUnits.esriUnknownUnits;

            esriUnits mapUnits = esriUnits.esriUnknownUnits;

            try
            {
                foreach (Layer layer in templateMap.Layers)
                {
                    if (layer is ArcGISTiledMapServiceLayer)
                    {
                        mapUnits = (esriUnits)Enum.Parse(typeof(esriUnits), ((ArcGISTiledMapServiceLayer)layer).Units, false);
                        break;
                    }

                    if (!(layer is ArcGISDynamicMapServiceLayer)) continue;
                    mapUnits = (esriUnits)Enum.Parse(typeof(esriUnits), ((ArcGISDynamicMapServiceLayer)layer).Units, false);
                    break;
                }
            }
            catch
            {
                return mapUnits;
            }

            return mapUnits;
        }

        internal static ScaleLine.ScaleLineUnit EsriUnitsToScaleBarUnits(esriUnits mapUnit)
        {
            switch (mapUnit)
            {
                case esriUnits.esriInches:
                    return ScaleLine.ScaleLineUnit.Inches;
                case esriUnits.esriFeet:
                    return ScaleLine.ScaleLineUnit.Feet;
                case esriUnits.esriYards:
                    return ScaleLine.ScaleLineUnit.Yards;
                case esriUnits.esriMiles:
                    return ScaleLine.ScaleLineUnit.Miles;
                case esriUnits.esriNauticalMiles:
                    return ScaleLine.ScaleLineUnit.NauticalMiles;
                case esriUnits.esriMillimeters:
                    return ScaleLine.ScaleLineUnit.Millimeters;
                case esriUnits.esriCentimeters:
                    return ScaleLine.ScaleLineUnit.Centimeters;
                case esriUnits.esriDecimeters:
                    return ScaleLine.ScaleLineUnit.Decimeters;
                case esriUnits.esriMeters:
                    return ScaleLine.ScaleLineUnit.Meters;
                case esriUnits.esriKilometers:
                    return ScaleLine.ScaleLineUnit.Kilometers;
                case esriUnits.esriDecimalDegrees:
                    return ScaleLine.ScaleLineUnit.DecimalDegrees;
                default:
                    return ScaleLine.ScaleLineUnit.Undefined;
            }
        }
        public static TextSymbol ConvertTextXamltoTextSymbol(string textXaml)
        {
            TextSymbol textSymbol = new TextSymbol();
            textSymbol.FontSize = 0.0;

            XElement topElement = XElement.Parse(textXaml);
            if (topElement.HasAttributes)
            {
                var attribute = topElement.Attribute("FontFamily");
                if (attribute != null)
                {
                    textSymbol.FontFamily = new FontFamily(attribute.Value);
                    //textSymbol.FontFamily = new FontFamily("Arial Bold Italic");
                }
                attribute = topElement.Attribute("FontSize");
                if (attribute != null)
                {
                    textSymbol.FontSize = Convert.ToDouble(attribute.Value);
                }
                attribute = topElement.Attribute("Foreground");
                if (attribute != null)
                {
                    var solidColorBrush = new SolidColorBrush(GetThisColor(attribute.Value));
                    textSymbol.Foreground = solidColorBrush;
                }
            }

            foreach (XElement element in topElement.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "Run":
                        if (element.HasAttributes)
                        {
                            var attribute = element.Attribute("FontSize");
                            if (attribute != null)
                            {
                                textSymbol.FontSize = Convert.ToDouble(attribute.Value);
                            }
                            attribute = element.Attribute("FontFamily");
                            if (attribute != null)
                            {
                                //var ff = new FontFamily("Arial");

                                textSymbol.FontFamily = new FontFamily(attribute.Value);
                                //textSymbol.FontFamily = new FontFamily("Arial Bold");
                            }
                            attribute = element.Attribute("Foreground");
                            if (attribute != null)
                            {
                                var solidColorBrush = new SolidColorBrush(GetColorFromHex(attribute.Value));
                                textSymbol.Foreground = solidColorBrush;
                            }
                        }

                        textSymbol.Text += element.Value;
                        break;
                    case "LineBreak":
                        textSymbol.Text += "\r\n";
                        break;
                }
            }

            return textSymbol;
        }

        private static Color GetColorFromHex(string hex)
        {
            //strip out any # if they exist
            hex = hex.Replace("#", string.Empty);
            if (hex.Length == 8)
            {
                byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));

                return Color.FromArgb(a, r, g, b);
            }
            else
            {
                return GetThisColor(hex);
            }
        }

        private static Color GetThisColor(string colorString)
        {
            Type colorType = (typeof(System.Windows.Media.Colors));
            if (colorType.GetProperty(colorString) != null)
            {
                object o = colorType.InvokeMember(colorString,
                BindingFlags.GetProperty, null, null, null); if (o != null)
                {
                    return (Color)o;
                }
            }
            return Colors.Transparent;
        } 
    }
}
