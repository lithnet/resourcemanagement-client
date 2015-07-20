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
            return dateTime.ToString(TypeConverter.FimServiceDateFormat);
        }
    }
}
