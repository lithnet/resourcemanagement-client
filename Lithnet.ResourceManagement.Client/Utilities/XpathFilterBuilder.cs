using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    internal static class XpathFilterBuilder
    {
        public static string GetFilter(string objectType, Dictionary<string, string> keyValuePairs)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("/{0}[", objectType);

            foreach (KeyValuePair<string, string> anchor in keyValuePairs)
            {
                sb.AppendFormat("({0} = '{1}')", anchor.Key, anchor.Value);

                if (anchor.Key != keyValuePairs.Last().Key)
                {
                    sb.AppendFormat(" and ");
                }
            }

            sb.AppendFormat("]");

            return sb.ToString();
        }
    }
}
