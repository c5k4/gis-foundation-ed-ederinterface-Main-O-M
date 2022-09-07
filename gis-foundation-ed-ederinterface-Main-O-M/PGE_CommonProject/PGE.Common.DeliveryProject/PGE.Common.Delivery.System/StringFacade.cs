using System;
using System.Collections.Generic;
using System.Text;

namespace PGE.Common.Delivery.Systems
{
    /// <summary>
    /// Helper methods to work with string object
    /// </summary>
    public class StringFacade
    {
        /// <summary>
        /// Gets an Object and a default value string 
        /// returns default value if the object is null otherwise converts the object to string and returns it
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetDefaultNullString(object value, string defaultValue)
        {
            if (value == null)
                return defaultValue;
            else
                return value.ToString();
        }

        /// <summary>
        /// Given a string will identify if the string is in Data format and will return a string in Date Format "MMM-dd-yyyy".
        /// If the string is not in proper date format will return a EMPTY string if defaultToCurrentDate is false else the CurrentDate in string format "MMM-dd-yyyy"
        /// </summary>
        /// <param name="value">String value to be converted</param>
        /// <param name="defaultToCurrentDate">Boolean to set the default value to currentdate value. If set to yes will return current date in "MMM-dd-yyyy" format. If set to false will return empty string.</param>
        /// <returns>Return string in MMM-dd-yyyy or string.Empty</returns>
        public static string GetDateStringFromString(string value, bool defaultToCurrentDate)
        {
            try
            {
                try
                {
                    DateTime dt = Convert.ToDateTime(value);
                    return dt.ToString("MMM-dd-yyyy");
                }
                catch
                {
                    if (defaultToCurrentDate)
                    {
                        return DateTime.Now.ToString("MMM-dd-yyyy");
                    }
                    else
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Checks if the given string is a valid Integer.
        /// </summary>
        /// <param name="value">String value to evaluate</param>
        /// <returns>Returns true if the given string is a Integer else returns False</returns>
        public static bool IsValidNumberString(string value)
        {
            try
            {
                Convert.ToInt32(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the given string is a valid Double.
        /// </summary>
        /// <param name="value">String value to evaluate</param>
        /// <returns>Returns true if the given string is a Double else returns False</returns>
        public static bool IsValidDoubleString(string value)
        {
            try
            {
                Convert.ToDouble(value);
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Checks if the given string is a valid Date.
        /// </summary>
        /// <param name="value">String value to evaluate</param>
        /// <returns>Returns true if the given string is a Date else returns False</returns>
        public static bool IsValidDateString(string value)
        {
            try
            {
                Convert.ToDateTime(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
