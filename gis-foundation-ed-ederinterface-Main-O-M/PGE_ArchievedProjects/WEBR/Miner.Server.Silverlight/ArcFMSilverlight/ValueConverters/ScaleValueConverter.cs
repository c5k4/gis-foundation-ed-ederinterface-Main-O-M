using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

using ESRI.ArcGIS.Client.Toolkit;

namespace ArcFMSilverlight.ValueConverters
{
    public class ScaleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (!(value is string)) throw new ArgumentException("Value is not a string", "value");

            if (parameter == null) return string.Empty;
            if (!(parameter is TextBlock)) throw new ArgumentException("Parameter is not a TextBlock", "value");

            var unitTextBlock = parameter as TextBlock;

            if (string.IsNullOrEmpty(unitTextBlock.Text)) return string.Empty;

            ScaleLine.ScaleLineUnit units;

            try
            {
                units = (ScaleLine.ScaleLineUnit)Enum.Parse(typeof(ScaleLine.ScaleLineUnit), unitTextBlock.Text, true);
            }
            catch (Exception)
            {
                throw new ArgumentException("Parameter is not a valid ScaleLine.ScaleLineUnit", "parameter");
            }

            var originalValue = value.ToString();
            var v = ValueToDouble(originalValue);
            if (double.IsNaN(v)) return string.Empty;

            var scientificValue = originalValue.Substring(0, originalValue.IndexOf(' '));
            if (!scientificValue.Contains('E'))
            {
                scientificValue = "";
            }

            string finalValueEnding;

            switch (units)
            {
                case ScaleLine.ScaleLineUnit.Feet:
                    if (v >= 5286)
                    {
                        v = v/5286;
                        finalValueEnding = "mi";
                    }
                    else
                    {
                        finalValueEnding = "ft";
                    } 

                    break;
                case ScaleLine.ScaleLineUnit.Miles:
                    if ((v < 1) && (v > 0))
                    {
                        v = v * 5286;
                        finalValueEnding = "ft";
                    }
                    else
                    {
                        finalValueEnding = "mi";
                    }

                    if (scientificValue != "")
                    {
                        finalValueEnding = "mi";
                    }
                    break;
                case ScaleLine.ScaleLineUnit.Meters:
                    if (v >= 1000)
                    {
                        v = v / 1000;
                        finalValueEnding = "km";
                    }
                    else
                    {
                        finalValueEnding = "m";
                    } 
                    break;
                case ScaleLine.ScaleLineUnit.DecimalDegrees:
                case ScaleLine.ScaleLineUnit.Kilometers:
                    if ((v < 1) && (v > 0))
                    {
                        v = v * 1000;
                        finalValueEnding = "m";
                    }
                    else
                    {
                        finalValueEnding = "km";
                    }

                    if (scientificValue != "")
                    {
                        finalValueEnding = "km";
                    }
                    break;
                default:
                    finalValueEnding = string.Empty;
                    break;
            }

            v = Math.Round(v, 4);
            return (scientificValue == "" ? v.ToString() : scientificValue) + finalValueEnding;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static double ValueToDouble(string value)
        {
            if (string.IsNullOrEmpty(value)) return double.NaN;
            if (value.Contains("NaN")) return double.NaN;

            // Remove any abreviation periods for the units.
            if (value.Trim().EndsWith(".")) value = value.Substring(0, value.Length - 2);

            var chars = value.ToCharArray();
            var nums = new List<Char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };

            // Get only the numbers and any decimal point
            var val = new string(chars.Where(nums.Contains).ToArray());

            return System.Convert.ToDouble(val);

        }

    }
}
