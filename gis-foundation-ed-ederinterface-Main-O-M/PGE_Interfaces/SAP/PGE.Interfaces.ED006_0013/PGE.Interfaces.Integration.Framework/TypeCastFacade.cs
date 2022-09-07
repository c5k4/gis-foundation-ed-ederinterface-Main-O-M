using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


//Should be from Telvent.Delivery.CodeLibrary when it is available from GAC
namespace Telvent.PGE.Framework
{
    /// <summary>
    /// Provides helper methods to translate data between the internal data storage value and the user-display value and vice versa. 
    /// </summary>
    public static class TypeCastFacade
    {
        /// <summary>
        /// Casts a database object into a nullable (structure) data type.
        /// </summary>
        /// <typeparam name="T">The target data type of the conversion.</typeparam>
        /// <param name="obj">The database object to be converted.</param>
        /// <returns>The initialized nullable data.</returns>
        public static Nullable<T> DbCast<T>(object obj) where T : struct
        {
            if (!Convert.IsDBNull(obj))
            {
                if (obj is T) return (Nullable<T>)obj;
                return (Nullable<T>)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
            }
            return default(Nullable<T>);
        }

        /// <summary>
        /// Attempts to casts an object to the type of the given default value. If the object is null
        /// or DBNull, the default value specified will be returned. If the object is
        /// convertable to the type of the default value, the explicit conversion will
        /// be performed.
        /// </summary>
        /// <typeparam name="T">The target data type of the conversion.</typeparam>
        /// <param name="obj">The object to be cast.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// The value of the object cast to the type of the default value.
        /// </returns>        
        public static bool TryCast<T>(object obj, out T result)
        {
            result = default(T);

            try
            {
                result = Cast(obj, result);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Casts an object to the type of the given default value. If the object is null
        /// or DBNull, the default value specified will be returned. If the object is
        /// convertable to the type of the default value, the explicit conversion will
        /// be performed.
        /// </summary>
        /// <typeparam name="T">The target data type of the conversion.</typeparam>
        /// <param name="obj">The object to be cast.</param>
        /// <param name="defaultValue">The default fail back value</param>
        /// <returns>The value of the object cast to the type of the default value.</returns>
        /// <exception cref="System.InvalidCastException">Thrown if the type of the object
        /// cannot be cast to the type of the default value.</exception>
        public static T Cast<T>(object obj, T defaultValue)
        {
            if (obj != null)
            {
                if (obj is T)
                {
                    if (IsEmptyString(obj))
                        return defaultValue;

                    return (T)obj;
                }

                if (IsEmptyString(obj))
                    return defaultValue;

                if (!Convert.IsDBNull(obj))
                {
                    return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
                }
            }

            return defaultValue;
        }


        #region Can be converted to a Linq Extension
        /// <summary>
        /// concatenate the strings in an enumeration separated by the specified delimiter
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="input">The collection of elements to concatenate</param>
        /// <param name="delimiter">The character to insert between the elements</param>
        /// <returns>The elements concatenated together</returns>
        public static string Delimit<T>(IEnumerable<T> input, string delimiter)
        {
            IEnumerator<T> enumerator = input.GetEnumerator();

            if (enumerator.MoveNext())
            {
                StringBuilder builder = new StringBuilder();

                // start off with the first element
                builder.Append(enumerator.Current);

                // append the remaining elements separated by the delimiter
                while (enumerator.MoveNext())
                {
                    builder.Append(delimiter);
                    builder.Append(enumerator.Current);
                }

                return builder.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// concatenate all elements
        /// </summary>
        /// <typeparam name="T">Type of elements in the collection</typeparam>
        /// <param name="input">The collection of elements to concatenate</param>
        /// <returns>The elements concatenated together</returns>
        public static string ToString<T>(IEnumerable<T> input)
        {
            return ToString(input, string.Empty);
        }

        /// <summary>
        /// concatenate all elements separated by a delimiter
        /// </summary>
        /// <typeparam name="T">Type of elements in the collection</typeparam>
        /// <param name="input">The collection of elements to concatenate</param>
        /// <param name="delimiter">The character to insert between the elements</param>
        /// <returns>The elements concatenated together</returns>
        public static string ToString<T>(IEnumerable<T> input, string delimiter)
        {
            return TypeCastFacade.Delimit(input, delimiter);
        }

        #endregion

        /// <summary>
        /// Determines whether the specified object is an empty string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// 	<c>true</c> if the specified object is an empty string; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsEmptyString(object obj)
        {
            if (obj is string)
                if (string.IsNullOrEmpty(obj.ToString().Trim()))
                    return true;

            return false;
        }
    }
}
