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
        /// Creates an XPath filter for the specified object type and attribute value pair
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="attributeName">The name of the attribute to query</param>
        /// <param name="attributeValue">The value of the attribute to query</param>
        /// <returns>An XPath query string</returns>
        public static string CreateFilter(string objectType, string attributeName, string attributeValue)
        {
            Dictionary<string, string> dictionary = new Dictionary<string,string>();
            dictionary.Add(attributeName, attributeValue);
            return XpathFilterBuilder.CreateAndFilter(objectType, dictionary);
        }

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with an 'and' operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <returns>An XPath query string</returns>
        public static string CreateAndFilter(string objectType, Dictionary<string, string> keyValuePairs)
        {
            return XpathFilterBuilder.CreateFilter(objectType, "and", keyValuePairs);
        }

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with an 'or' operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <returns>An XPath query string</returns>
        public static string CreateOrFilter(string objectType, Dictionary<string, string> keyValuePairs)
        {
            return XpathFilterBuilder.CreateFilter(objectType, "or", keyValuePairs);
        }

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with with specified operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="op">The operator to apply to the elements in the filter</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <returns>An XPath query string</returns>
        internal static string CreateFilter(string objectType, string op, Dictionary<string, string> keyValuePairs)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("/{0}[", objectType);

            foreach (KeyValuePair<string, string> anchor in keyValuePairs)
            {
                sb.AppendFormat("({0} = '{1}')", anchor.Key, anchor.Value);

                if (anchor.Key != keyValuePairs.Last().Key)
                {
                    sb.AppendFormat(" {0} ", op);
                }
            }

            sb.AppendFormat("]");

            return sb.ToString();
        }
    }
}
