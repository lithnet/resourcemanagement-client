using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public static class Extensions
    {
        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime)
        {
            return Extensions.ToResourceManagementServiceDateFormat(dateTime, false);
        }

        public static string ToResourceManagementServiceDateFormat(this DateTime dateTime, bool zeroMilliseconds)
        {
            if (zeroMilliseconds)
            {
                return dateTime.ToString(TypeConverter.FimServiceDateFormatZeroedMilliseconds);
            }
            else
            {
                return dateTime.ToString(TypeConverter.FimServiceDateFormat);

            }
        }
    }
}