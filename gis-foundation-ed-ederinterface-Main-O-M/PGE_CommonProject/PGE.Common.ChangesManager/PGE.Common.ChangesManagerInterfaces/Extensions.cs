using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.ChangesManagerShared
{
    public static class Extensions
    {
        static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        // Works in C#3/VS2008:
        // Returns a new dictionary of this ... others merged leftward.
        // Keeps the type of 'this', which must be default-instantiable.
        // Example: 
        //   result = map.MergeLeft(other1, other2, ...)
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                // ^-- echk. Not quite there type-system.
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }

        public static string GetOracleDateString(this DateTime dateTime)
        {

            return "to_date('" + dateTime.ToString("yyyy-MM-dd HH:mm:ss") +
                               "','YYYY-MM-DD HH24:MI:SS')";
        }

        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        public static string GetValueAsString(this IRow row, string fieldName)
        {
            int valueNameFieldIndex = row.Table.FindField(fieldName);
            string versionName = "";
            if (valueNameFieldIndex > -1)
            {
                object versionNameObject = row.get_Value(valueNameFieldIndex);
                versionName = versionNameObject == DBNull.Value ? "" : versionNameObject.ToString();
            }
            return versionName;
        }

        public static string GetRowDescription(this IRow row, bool includeGlobalID = true)
        {
            if (row == null) return "Row is Null";
            if (includeGlobalID)
                return "Row [ " + ((IDataset) row.Table).Name + " ] OID [ " + row.OID + " ]" +
                   " GLOBALID [ " + row.GetValueAsString("GLOBALID");
            else
            {
                return "Row [ " + ((IDataset) row.Table).Name + " ] OID [ " + row.OID + " ]";
            }
        }

        /// <summary>
        /// Get the description for the coded value domain
        /// </summary>
        /// <param name="pObj">pass object</param>
        /// <param name="pFieldIndex">pass the field Index for the field</param>
        /// <param name="pErrorMessage">out error message if domain is null</param>
        /// <returns>Returns description for the field value</returns>
        public static String GetDomainDescription(this IObject pObj, int pFieldIndex, out String pErrorMessage)
        {
            // Varibale Declarations
            pErrorMessage = string.Empty;
            String valueResult = string.Empty;
            ICodedValueDomain pCodedValueDomain = null;

            // Check for field index
            if (pFieldIndex == -1)
            {
                pErrorMessage = ("Field index should not be -1. Please check the field exist in layer or not.");
                return string.Empty;
            }


            IField pField = pObj.Fields.get_Field(pFieldIndex);

            // if domain is null return message
            if (pField.Domain == null)
            {
                pErrorMessage = "Domain is not assigned to field '" + pField.Name + "'.";
                return string.Empty;
            }

            IDomain pDomain = pField.Domain;

            //Check for domain type
            if (pDomain.Type == esriDomainType.esriDTCodedValue)
            {
                pCodedValueDomain = pDomain as ICodedValueDomain;
                string featureValue = pObj.get_Value(pFieldIndex).ToString();

                //Get coded value name and value and check with given value
                for (int j = 0; j < pCodedValueDomain.CodeCount; j++)
                {
                    string codedValueName = pCodedValueDomain.get_Name(j);
                    string codedValueValue = pCodedValueDomain.get_Value(j).ToString();

                    if (codedValueValue.ToString().Equals(featureValue))
                    {
                        valueResult = codedValueName;
                        break;
                    }
                }
            }
            else
            {
                pErrorMessage = "Domain is not codedvalue.";
                return string.Empty;
            }
            return valueResult;

        }


        /// <summary>
        /// Gets the description (aka the name) of this instance's subtype.
        /// </summary>
        /// <param name="self">The instance to get the subtype description from.</param>
        /// <returns>The description of the subtype.</returns>
        public static string GetSubtypeDescription(this IObject self)
        {
            var subtypes = self.Class as ISubtypes;
            if (subtypes == null || !subtypes.HasSubtype) return null;
            IRowSubtypes rowSubtypes = (IRowSubtypes)self;
            int selfSubtypeCode = rowSubtypes.SubtypeCode;
            return subtypes.SubtypeName[selfSubtypeCode];
        }
        /// <summary>
        /// Gets the description (aka the name) of this instance's subtype.
        /// </summary>
        /// <param name="self">The instance to get the subtype description from.</param>
        /// <returns>The description of the subtype.</returns>
        public static int GetSubtypeCode(this IObject self)
        {
            var subtypes = self.Class as ISubtypes;
            if (subtypes == null || !subtypes.HasSubtype) return -1;
            IRowSubtypes rowSubtypes = (IRowSubtypes)self;
            int selfSubtypeCode = rowSubtypes.SubtypeCode;
            return selfSubtypeCode;
        }

        /// <summary>
        ///   Returns the given field's value, or null if no field with the given name is found.
        /// </summary>
        /// <param name="fieldName"> The name of the field to look for. </param>
        /// <param name="useDomain"> true if the field's domain should be used; otherwise false. </param>
        /// <returns>The value of the given field.</returns>
        public static object GetFieldValue(this IObject self, string fieldName, bool useDomain = true)
        {

            IField field;
            int pos;

            pos = self.Class.FindField(fieldName);
            if (pos == -1) return null;
            _logger.Debug("Field index of " + fieldName + " is " + pos);

            field = self.Fields.Field[pos];
            var valueAsObject = self.Value[pos];
            if (valueAsObject == null)
            {
                _logger.Debug(fieldName + " = <null>");
                return null;
            }

            _logger.Debug(valueAsObject.GetType().Name + ' ' + fieldName + " = " + valueAsObject.ToString());

            if (!useDomain)
                return valueAsObject;
            _logger.Debug("Looking up domain for field " + fieldName);
            // If there is a domain configured and it's specified to use a domain then get the description.
            var valueAsText = valueAsObject.Convert(String.Empty);
            if (valueAsText != String.Empty)
            {
                //if the domain is not assigned for each sub-type then ArcObjects throws an error while trying to get the 
                //domain by sub-type.
                //resolution is to check the domain on field first and if not found then for subtypes
                var domain = field.Domain;
                if (domain == null)
                {
                    domain = ((ISubtypes)self.Table).HasSubtype
                        ? ((ISubtypes)self.Table).get_Domain(((IRowSubtypes)self).SubtypeCode, fieldName)
                        : null;
                }
                var result = domain == null ? valueAsObject : domain.Lookup(valueAsText) ?? valueAsObject;
                _logger.Debug(fieldName + " domain result is " + result.ToString());
                return result;
            }
            return null;
        }

        /// <summary>
        /// Attempts to match the domain code to a name, optionally returning a default string if the code is not found.
        /// If no default value string is provided, null is returned.
        /// </summary>
        /// <param name="domain">The domain to search on.</param>
        /// <param name="domainCode">The code to look for in the domain.</param>
        /// <param name="defaultValue">An optional default value to return in liu of a result.</param>
        /// <returns></returns>
        private static object Lookup(this IDomain domain, string domainCode, string defaultValue = null)
        {
            var codedValueDomain = domain as ICodedValueDomain;
            if (codedValueDomain != null)
            {
                for (var i = 0; i < codedValueDomain.CodeCount; i++)
                {
                    var code = codedValueDomain.Value[i].Convert(String.Empty);
                    if (domainCode.Equals(code)) return codedValueDomain.Name[i];
                }
            }
            return defaultValue;
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

        /// <summary>
        /// Indicates whether the specified string is null or an <see cref="F:System.String.Empty"/> string.
        /// </summary>
        /// <returns>
        /// true if the <paramref name="value"/> parameter is null or an empty string (""); otherwise, false.
        /// </returns>
        /// <param name="value">The string to test. </param>
        /// <filterpriority>1</filterpriority>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
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


    }
    public static class StringExtension
    {
        public static string GetDatabaseStringValue(this string value, bool useQuotes = true)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "NULL";
            }
            if (useQuotes)
                return "'" + value.Replace("'", "''") + "'";
            else
                return value;
        }
        public static string GetDatabaseStringValue(this int? value)
        {
            if (value.HasValue)
            {
                return Convert.ToString(value);
            }
            return "NULL";
        }
    }

}
