using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// A utility class that provides the ability to build XPath queries
    /// </summary>
    public static class XPathFilterBuilder
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
            return XPathFilterBuilder.CreateAndFilter(objectType, dictionary);
        }

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with an 'and' operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <returns>An XPath query string</returns>
        public static string CreateAndFilter(string objectType, Dictionary<string, string> keyValuePairs)
        {
            return XPathFilterBuilder.CreateFilter(objectType, QueryOperator.And, XPathFilterBuilder.DictionaryToPredicates(keyValuePairs, XPathOperator.Equals));
        }

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with an 'or' operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <returns>An XPath query string</returns>
        public static string CreateOrFilter(string objectType, Dictionary<string, string> keyValuePairs)
        {
            return XPathFilterBuilder.CreateFilter(objectType, QueryOperator.Or, XPathFilterBuilder.DictionaryToPredicates(keyValuePairs, XPathOperator.Equals));
        }
        
        internal static IEnumerable<XPathFilterPredicate> DictionaryToPredicates(Dictionary<string,string> keyValuePairs, XPathOperator op)
        {
            List<XPathFilterPredicate> predicates = new List<XPathFilterPredicate>();

            foreach (KeyValuePair<string,string> kvp in keyValuePairs)
            {
                predicates.Add(new XPathFilterPredicate(kvp.Key, op, kvp.Value));
            }

            return predicates;
        }

        internal static string CreateFilter(string objectType, QueryOperator queryOperator, IEnumerable<XPathFilterPredicateGroup> queryGroups)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("/{0}[", objectType);

            foreach (XPathFilterPredicateGroup group in queryGroups)
            {
                sb.AppendFormat("({0})", group.ToString());

                if (group != queryGroups.Last())
                {
                    sb.AppendFormat(" {0} ", queryOperator);
                }
            }

            sb.AppendFormat("]");

            return sb.ToString();

        }

        internal static string CreateFilter(string objectType, QueryOperator queryOperator, IEnumerable<XPathFilterPredicate> predicates)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("/{0}[", objectType);

            XPathFilterPredicateGroup group = new XPathFilterPredicateGroup(predicates, queryOperator);
            sb.AppendFormat(group.ToString());

            sb.AppendFormat("]");

            return sb.ToString();

        }

        internal static string CreateFilter(string objectType, QueryOperator queryOperator, params XPathFilterPredicate[] predicates)
        {
            return XPathFilterBuilder.CreateFilter(objectType, queryOperator, (IEnumerable<XPathFilterPredicate>)predicates);
        }
    }
}
