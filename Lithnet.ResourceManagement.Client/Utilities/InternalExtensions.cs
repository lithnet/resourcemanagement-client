// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Ryan Newington">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.ResourceManagement.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.ComponentModel;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Defines extension methods used in the application
    /// </summary>
    internal static class InternalExtensions
    {
        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToCommaSeparatedString(this IEnumerable<string> strings)
        {
            string newString = string.Empty;

            if (strings != null)
            {
                foreach (string text in strings)
                {
                    newString = newString.AppendWithCommaSeparator(text);
                }
            }

            return newString;
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <param name="separator">The character or string to use to separate the strings</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToSeparatedString(this IEnumerable<string> strings, string separator)
        {
            string newString = string.Empty;

            foreach (string text in strings)
            {
                newString = newString.AppendWithSeparator(separator, text);
            }

            return newString;
        }

        /// <summary>
        /// Converts an enumeration of strings into a comma separated list
        /// </summary>
        /// <param name="strings">The enumeration of string objects</param>
        /// <returns>The comma separated list of strings</returns>
        public static string ToNewLineSeparatedString(this IEnumerable<string> strings)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string text in strings)
            {
                builder.AppendLine(text);
            }

            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Appends two string together with a comma and a space
        /// </summary>
        /// <param name="text">The original string</param>
        /// <param name="textToAppend">The string to append</param>
        /// <returns>The concatenated string</returns>
        public static string AppendWithCommaSeparator(this string text, string textToAppend)
        {
            string newString = string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                text += ", ";
            }
            else
            {
                text = string.Empty;
            }

            newString = text + textToAppend;
            return newString;
        }

        /// <summary>
        /// Appends two string together with a comma and a space
        /// </summary>
        /// <param name="text">The original string</param>
        /// <param name="separator">The character or string to use to separate the strings</param>
        /// <param name="textToAppend">The string to append</param>
        /// <returns>The concatenated string</returns>
        public static string AppendWithSeparator(this string text, string separator, string textToAppend)
        {
            string newString = string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                text += separator;
            }
            else
            {
                text = string.Empty;
            }

            newString = text + textToAppend;
            return newString;
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object</returns>
        public static string ToSmartString(this object obj)
        {
            if (obj is byte[])
            {
                byte[] cast = (byte[])obj;
                return Convert.ToBase64String(cast);
            }
            else if (obj is long)
            {
                return ((long)obj).ToString();
            }
            else if (obj is string)
            {
                return ((string)obj).ToString();
            }
            else if (obj is bool)
            {
                return ((bool)obj).ToString();
            }
            else if (obj is Guid)
            {
                return ((Guid)obj).ToString();
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).ToString(TypeConverter.FimServiceDateFormat);
            }
            else if (obj == null)
            {
                return "null";
            }
            else
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object, or a null value if the object is null</returns>
        public static string ToSmartStringOrNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else
            {
                return obj.ToSmartString();
            }
        }

        /// <summary>
        /// Gets an informative string representation of an object
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>An informative string representation of an object, or a null value if the object is null</returns>
        public static string ToSmartStringOrEmptyString(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                return obj.ToSmartString();
            }
        }

        /// <summary>
        /// Truncates a string to the specified length
        /// </summary>
        /// <param name="obj">The string to truncate</param>
        /// <param name="totalLength">The length to truncate to</param>
        /// <returns></returns>
        public static string TruncateString(this string obj, int totalLength)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                return obj;
            }

            if (totalLength <= 3)
            {
                throw new ArgumentException("The maxlength value must be greater than 3", "totalLength");
            }

            if (obj.Length > totalLength)
            {
                return obj.Substring(0, totalLength - 3) + "...";
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        /// Gets a value indicating whether two enumerations contain the same elements, even if they are in different orders
        /// </summary>
        /// <typeparam name="T">The type of items in the enumerations</typeparam>
        /// <param name="enumeration1">The first list to compare</param>
        /// <param name="enumeration2">The second list to compare</param>
        /// <returns>A value indicating if the two enumerations contain the same objects</returns>
        public static bool ContainsSameElements<T>(this IEnumerable<T> enumeration1, IEnumerable<T> enumeration2)
        {
            List<T> list1 = enumeration1.ToList();
            List<T> list2 = enumeration2.ToList();

            if (list1.Count != list2.Count)
            {
                return false;
            }

            return list1.Intersect(list2).Count() == list1.Count;
        }
      
        /// <summary>
        /// <para>Truncates a DateTime to a specified resolution.</para>
        /// <para>A convenient source for resolution is TimeSpan.TicksPerXXXX constants.</para>
        /// </summary>
        /// <param name="date">The DateTime object to truncate</param>
        /// <param name="resolution">e.g. to round to nearest second, TimeSpan.TicksPerSecond</param>
        /// <returns>Truncated DateTime</returns>
        public static DateTime Truncate(this DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }

        /// <summary>
        /// Disables the context manager for the specified client
        /// </summary>
        /// <typeparam name="T">The type of client proxy</typeparam>
        /// <param name="client">The client proxy to disable the context manager for</param>
        public static void DisableContextManager<T>(this ClientBase<T> client)  where T : class
        {
               IContextManager property = client.ChannelFactory.GetProperty<IContextManager>();
               if (property != null)
               {
                   property.Enabled = false;
               }
        }
    }
}
