using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.WebServices;
using System.Globalization;

namespace Lithnet.ResourceManagement.Client
{
    internal static class TypeConverter
    {
        public const string FimServiceDateFormat = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff";

        public const string FimServiceDateFormatZeroedMilliseconds = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'000";
        
        public static bool ToBoolean(object value)
        {
            return Convert.ToBoolean(value);
        }

        public static byte[] ToByte(object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is byte[])
            {
                return (byte[])value;
            }
            else if (value is string)
            {
                return Convert.FromBase64String((string)value);
            }
            else
            {
                throw new UnsupportedDataTypeException(typeof(byte[]), value.GetType());
            }
        }

        public static long ToLong(object value)
        {
            if (value is long || value is int)
            {
                return (long)value;
            }
            else if (value is string)
            {
                return Convert.ToInt64(value);
            }
            else
            {
                throw new UnsupportedDataTypeException(typeof(long), value.GetType());
            }
        }

        public static string ToString(object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is byte[])
            {
                return Convert.ToBase64String((byte[])value);
            }
            else if (value is DateTime)
            {
                return ((DateTime)value).ToResourceManagementServiceDateFormat();
            }
            else if (value is string)
            {
                return (string)value;
            }
            else if (value is bool || value is long || value is int || value is UniqueIdentifier)
            {
                return value.ToString();
            }
            else
            {
                throw new UnsupportedDataTypeException(typeof(string), value.GetType());
            }
        }

        public static UniqueIdentifier ToUniqueIdentifier(object value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value is UniqueIdentifier)
            {
                return (UniqueIdentifier)value;
            }
            else if (value is Guid)
            {
                return new UniqueIdentifier((Guid)value);
            }
            else if (value is string)
            {
                return new UniqueIdentifier((string)value);
            }
            else
            {
                throw new UnsupportedDataTypeException(typeof(UniqueIdentifier), value.GetType());
            }
        }

        public static DateTime ToDateTime(object value)
        {
            if (value is DateTime)
            {
                return (DateTime)value;
            }
            else if (value is string)
            {
                return DateTime.ParseExact((string)value, FimServiceDateFormat, CultureInfo.CurrentCulture);
            }
            else
            {
                throw new UnsupportedDataTypeException(typeof(UniqueIdentifier), value.GetType());
            }
        }
    }
}
