using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.WebServices;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Contains various helper extensions for use with the this library
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime)
        {
            return Extensions.ToResourceManagementServiceDateFormat(dateTime, false);
        }

        /// <summary>
        /// Converts a date time to the ISO 8601 date string required by the Resource Management Service
        /// </summary>
        /// <param name="dateTime">The date and time to convert</param>
        /// <param name="zeroMilliseconds">A value indicating whether the millisecond component of the date should be zeroed to avoid rounding/round-trip issues</param>
        /// <returns>An ISO 8601 date format string</returns>
        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime, bool zeroMilliseconds)
        {
            DateTime convertedDateTime = dateTime.ToUniversalTime();

            if (zeroMilliseconds)
            {
                return convertedDateTime.ToString(TypeConverter.FimServiceDateFormatZeroedMilliseconds);
            }
            else
            {
                return convertedDateTime.ToString(TypeConverter.FimServiceDateFormat);

            }
        }

        /// <summary>
        /// Add the specified value to the collection of values of a multivalued attribute, or sets the value of a single-valued attribute
        /// </summary>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">The value to add</param>
        public static void AddValue(this ResourceObject rObject, string attributeName, object value)
        {
            if (rObject != null)
                rObject.Attributes[attributeName].AddValue(value);
        }

        /// <summary>
        /// Returns a value indicating whether the specified value is present on the specified attribute
        /// </summary>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">The value to check</param>
        /// <returns>Returns false if ResourceObject is null or true or false depending on if the value exists on the given attribute</returns>
        public static bool HasValue(this ResourceObject rObject, string attributeName, object value)
        {
            return rObject != null ? rObject.Attributes[attributeName].HasValue(value) : false;
        }

        /// <summary>
        /// Removes a specific value from the specifed attribute
        /// </summary>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">The value to remove</param>
        public static void RemoveValue(this ResourceObject rObject, string attributeName, object value)
        {
            if (rObject != null)
                rObject.Attributes[attributeName].RemoveValue(value);
        }

        /// <summary>
        /// Sets the value of the attribute, overwriting any existing values present on the object
        /// </summary>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">The value to set</param>
        public static void SetValue(this ResourceObject rObject, string attributeName, object value)
        {
            if (rObject != null)
                rObject.Attributes[attributeName].SetValue(value);
        }

    }
}