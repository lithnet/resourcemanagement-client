using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// A utility class that provides the ability to build XPath queries
    /// </summary>
    public static class XpathFilterBuilder
    {
        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with an 'and' operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <returns></returns>
        public static string CreateAndFilter(string objectType, Dictionary<string, string> keyValuePairs)
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

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with an 'or' operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <returns></returns>
        public static string CreateOrFilter(string objectType, Dictionary<string, string> keyValuePairs)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("/{0}[", objectType);

            foreach (KeyValuePair<string, string> anchor in keyValuePairs)
            {
                sb.AppendFormat("({0} = '{1}')", anchor.Key, anchor.Value);

                if (anchor.Key != keyValuePairs.Last().Key)
                {
                    sb.AppendFormat(" or ");
                }
            }

            sb.AppendFormat("]");

            return sb.ToString();
        }
    }
}
