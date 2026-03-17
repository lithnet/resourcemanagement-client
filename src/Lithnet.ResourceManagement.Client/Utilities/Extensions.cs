using System;

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
            return dateTime.ToResourceManagementServiceDateFormat(false);
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
        /// <param name="resource">The resource to add the value to</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">The value to add</param>
        public static void AddValue(this ResourceObject resource, string attributeName, object value)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            resource.Attributes[attributeName].AddValue(value);
        }

        /// <summary>
        /// Returns a value indicating whether the specified value is present on the specified attribute
        /// </summary>
        /// <param name="resource">The resource to check if the value exists on</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">The value to check</param>
        /// <returns>Returns false if ResourceObject is null or true or false depending on if the value exists on the given attribute</returns>
        public static bool HasValue(this ResourceObject resource, string attributeName, object value)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            return resource.Attributes[attributeName].HasValue(value);
        }

        /// <summary>
        /// Removes a specific value from the specified attribute
        /// </summary>
        /// <param name="resource">The resource to remove the value from</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">The value to remove</param>
        public static void RemoveValue(this ResourceObject resource, string attributeName, object value)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            resource.Attributes[attributeName].RemoveValue(value);
        }

        /// <summary>
        /// Sets the value of the specified attribute, overwriting any existing values present on the object
        /// </summary>
        /// <param name="resource">The resource to set the value on</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="value">The value to set</param>
        public static void SetValue(this ResourceObject resource, string attributeName, object value)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            resource.Attributes[attributeName].SetValue(value);
        }

        /// <summary>
        /// Gets the value of the specified attribute or null if the attribute doesn't exist on the object
        /// </summary>
        /// <param name="resource">The resource to set the value on</param>
        /// <param name="attributeName">The name of the attribute</param>
        public static object GetValueOrDefault(this ResourceObject resource, string attributeName)
        {
            return resource.GetValueOrDefault(attributeName, null);
        }

        /// <summary>
        /// Gets the value of the specified attribute, or the specified default value if the attribute doesn't exist on the object or it's value is null
        /// </summary>
        /// <param name="resource">The resource to set the value on</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="defaultValue">The defaultValue to return if attribute doesn't exist</param>
        public static object GetValueOrDefault(this ResourceObject resource, string attributeName, object defaultValue)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (attributeName == null)
            {
                throw new ArgumentNullException(nameof(attributeName));
            }

            if (resource.HasValue(attributeName))
            {
                return resource.Attributes[attributeName].Value;
            }
            else
            {
                return defaultValue;
            }
        }

        internal static Uri GetProxyUri(this ResourceManagementClientOptions options)
        {
            return UriParser.GetRmcProxyUri(options.BaseUri);
        }

        internal static Uri GetNetTcpUri(this ResourceManagementClientOptions options)
        {
            return UriParser.GetFimServiceNetTcpUri(options.BaseUri);
        }

        internal static Uri GetFimServiceUri(this ResourceManagementClientOptions options)
        {
            return UriParser.GetFimServiceHttpUri(options.BaseUri);
        }
    }
}