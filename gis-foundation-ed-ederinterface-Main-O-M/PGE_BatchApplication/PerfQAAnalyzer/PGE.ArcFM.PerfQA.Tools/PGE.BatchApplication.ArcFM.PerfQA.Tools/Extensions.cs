using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.ArcFM_PerfQA_Tools
{
    public static class Extensions
    {
        /// <summary>
        /// Provides an enumeration for the given set.
        /// </summary>
        /// <param name="self">The instance of ISet to enumerate on.</param>
        public static IEnumerable<IObject> AsEnumerable(this ISet set)
        {
            set.Reset();
            IObject unitObj;
            while ((unitObj = set.Next() as IObject) != null)
                yield return unitObj;
        }

        public static IEnumerable<IRelationshipClass> AsEnumerable(this IEnumRelationshipClass self)
        {
            self.Reset();
            IRelationshipClass current;
            while ((current = self.Next()) != null)
            {
                yield return current;
            }
        }
        

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <returns>
        /// true if the <paramref name="value"/> parameter is null or <see cref="F:System.String.Empty"/>, or if <paramref name="value"/> consists exclusively of white-space characters.
        /// </returns>
        /// <param name="value">The string to test.</param>
        public static bool IsNullOrWhitespace(this string value)
        {
            return string.IsNullOrEmpty(value) || value.All(char.IsWhiteSpace);
        }

        /// <summary>
        ///   Concatenates the given enumeration of strings, optionally with a separator between each.
        /// </summary>
        /// <param name="items"> </param>
        /// <param name="seperator"> </param>
        /// <returns> </returns>
        public static string Concatenate(this IEnumerable<string> items, string seperator = "")
        {
            if (seperator == null) seperator = "";
            var builder = new StringBuilder();
            builder.Append(items.FirstOrDefault() ?? "");
            foreach (var item in items.Skip(1))
            {
                builder.Append(seperator);
                builder.Append(item);
            }
            return builder.ToString();
        }


        /// <summary>
        ///   Casts or converts this instance to the given type if possible.
        ///   Otherwise returns a default value.
        /// </summary>
        /// <typeparam name="T"> The type to cast this instance to. </typeparam>
        /// <param name="obj"> The object to cast. </param>
        /// <param name="defaultValue"> If the cast fails, yield this value. </param>
        /// <returns> This instance converted to T; otherwise defaultValue. </returns>
        public static T Convert<T>(this object obj, T defaultValue = default(T))
        {
            if (obj != null && obj != DBNull.Value)
            {
                string objString = obj.ToString();
                if (objString.IsNullOrWhitespace())
                {
                    return defaultValue;
                }
                else
                {
                    if (obj is T)
                    {
                        return (T)obj;
                    }
                    else
                    {
                        return (T)System.Convert.ChangeType(obj, typeof(T), System.Globalization.CultureInfo.InvariantCulture);

                    }
                }
            }

            return defaultValue;
        }

    }
}
