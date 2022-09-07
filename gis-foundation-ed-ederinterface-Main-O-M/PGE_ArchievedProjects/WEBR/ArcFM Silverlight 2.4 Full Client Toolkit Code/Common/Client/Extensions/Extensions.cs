using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Json;
using System.Linq;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    internal static class Extensions
    {
        public static bool ContainsLayers(this Layer layer)
        {
            if ((layer is GraphicsLayer) || (layer is ElementLayer)) return false;

            return true;
        }

        public static object ToType(this JsonValue source)
        {
            switch (source.JsonType)
            {
                case JsonType.Array:
                    return source as JsonArray;
                case JsonType.Boolean:
                    return Convert.ToBoolean(Utility.DecodeString(source.ToString()));
                case JsonType.Number:
                    if (source.ToString().Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator) || source.ToString().Contains("."))
                    {
                        double value = 0;
                        if(!double.TryParse(source.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture.NumberFormat, out value))
                        {
                            var clonedCultureInfo = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
                            clonedCultureInfo.NumberDecimalSeparator = ".";

                            double.TryParse(source.ToString(), NumberStyles.Any, clonedCultureInfo, out value);
                        }

                        return value;
                    }
                    else
                    {
                        int value = 0;
                        if (int.TryParse(source.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture.NumberFormat, out value))
                        {
                            return value;
                        }
                        else
                        {
                            long longValue;
                            Int64.TryParse(source.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture.NumberFormat, out longValue);
                            return longValue;
                        }
                    }
                case JsonType.String:
                    return Utility.DecodeString(source.ToString());
                default:
                    return source;
            }
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>
        /// An System.Collections.ObjectModel.ObservableCollection whose elements are sorted according
        /// to a key.
        /// </returns>
        public static ObservableCollection<TSource> Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");

            var sortedItemsList = source.OrderBy(keySelector).ToList();

            foreach (var item in sortedItemsList)
            {
                source.Move(source.IndexOf(item), sortedItemsList.IndexOf(item));
            }

            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        public static void Move<TSource>(this ObservableCollection<TSource> source, int oldIndex, int newIndex)
        {
            if (source == null) return;
            if (oldIndex == newIndex) return;

            TSource item = source[oldIndex];
            source.RemoveAt(oldIndex);
            source.Insert(newIndex, item);
        }
    }
}
