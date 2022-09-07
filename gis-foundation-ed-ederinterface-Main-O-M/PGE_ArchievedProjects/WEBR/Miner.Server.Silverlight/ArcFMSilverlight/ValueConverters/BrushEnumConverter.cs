using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ArcFMSilverlight.ValueConverters
{
    public class BrushEnumConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            
            var brush = value as SolidColorBrush;
            if (brush == null) return 0;

            var color = brush.Color;
            if (color == Colors.LightGray)
            {
                return 0;
            }
            else if (color == Colors.Gray)
            {
                return 1;
            }
            else if (color == Colors.DarkGray)
            {
                return 2;
            }
            else if (color == Colors.Black)
            {
                return 3;
            }
            else if (color == Colors.White)
            {
                return 4;
            }
            else if (color == Colors.Red)
            {
                return 5;
            }
            else if (color == Colors.Brown)
            {
                return 6;
            }
            else if (color == Colors.Orange)
            {
                return 7;
            }
            else if (color == Colors.Yellow)
            {
                return 8;
            }
            else if (color == Colors.Green)
            {
                return 9;
            }
            else if (color == Colors.Blue)
            {
                return 10;
            }
            else if (color == Colors.Cyan)
            {
                return 11;
            }
            else if (color == Colors.Purple)
            {
                return 12;
            }
            else if (color == Colors.Magenta)
            {
                return 13;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Colors.LightGray);

            switch ((int)value)
            {
                case 0:
                    return new SolidColorBrush(Colors.LightGray);
                case 1:
                    return new SolidColorBrush(Colors.Gray);
                case 2:
                    return new SolidColorBrush(Colors.DarkGray);
                case 3:
                    return new SolidColorBrush(Colors.Black);
                case 4:
                    return new SolidColorBrush(Colors.White);
                case 5:
                    return new SolidColorBrush(Colors.Red);
                case 6:
                    return new SolidColorBrush(Colors.Brown);
                case 7:
                    return new SolidColorBrush(Colors.Orange);
                case 8:
                    return new SolidColorBrush(Colors.Yellow);
                case 9:
                    return new SolidColorBrush(Colors.Green);
                case 10:
                    return new SolidColorBrush(Colors.Blue);
                case 11:
                    return new SolidColorBrush(Colors.Cyan);
                case 12:
                    return new SolidColorBrush(Colors.Purple);
                case 13:
                    return new SolidColorBrush(Colors.Magenta);
                default:
                    return new SolidColorBrush(Colors.LightGray);
            }
        }

        #endregion
    }
}
