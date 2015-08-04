using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.ResourceManagement.Client;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// A utility class that provides the ability to build XPath queries
    /// </summary>
    public class XPathFilterBuilder
    {
        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute value pair
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="attributeName">The name of the attribute to query</param>
        /// <param name="attributeValue">The value of the attribute to query</param>
        /// <returns>An XPath query string</returns>
        public static string CreateFilter(string objectType, string attributeName, object attributeValue)
        {
            AttributeValuePairCollection dictionary = new AttributeValuePairCollection();
            dictionary.Add(attributeName, attributeValue);
            return XPathFilterBuilder.CreateFilter(objectType, dictionary, ComparisonOperator.Equals, GroupOperator.And);
        }

        public static string CreateFilter(string objectType, string attributeName, ComparisonOperator comparisonOperator, object attributeValue)
        {
            AttributeValuePairCollection dictionary = new AttributeValuePairCollection();
            
            dictionary.Add(attributeName, attributeValue);
            return XPathFilterBuilder.CreateFilter(objectType, dictionary, comparisonOperator, GroupOperator.And);
        }

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with an 'and' operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <returns>An XPath query string</returns>
        public static string CreateFilter(string objectType, Dictionary<string, object> keyValuePairs, ComparisonOperator valueComparisonOperator, GroupOperator groupOperator)
        {
            return XPathFilterBuilder.CreateFilter(objectType, new XPathQueryGroup(groupOperator, valueComparisonOperator, keyValuePairs));
        }

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with an 'and' operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <returns>An XPath query string</returns>
        public static string CreateFilter(string objectType, AttributeValuePairCollection keyValuePairs, ComparisonOperator valueComparisonOperator, GroupOperator groupOperator)
        {
            return XPathFilterBuilder.CreateFilter(objectType, new XPathQueryGroup(groupOperator, valueComparisonOperator, keyValuePairs));
        }

        public static string CreateFilter(string objectType, IXPathQueryObject query)
        {
            return new XPathExpression(objectType, query).ToString();
        }

        public static string CreateUnionFilter(params XPathExpression[] expressions)
        {
            return expressions.Select(t => t.ToString()).ToSeparatedString(" | ");
        }

        public static string CreateDereferenceFilter(string searchObjectType, string searchAttributeName, object searchAttributeValue, string referenceAttributeName)
        {
            XPathQuery predicate = new XPathQuery(searchAttributeName, ComparisonOperator.Equals, searchAttributeValue);
            return XPathFilterBuilder.CreateDereferenceFilter(searchObjectType, predicate, referenceAttributeName);
        }

        public static string CreateDereferenceFilter(string searchObjectType, AttributeValuePairCollection searchAttributes, ComparisonOperator valueComparisonOperator, GroupOperator groupOperator, string referenceAttributeName)
        {
            XPathQueryGroup predicate = new XPathQueryGroup(groupOperator, valueComparisonOperator, searchAttributes);
            return XPathFilterBuilder.CreateDereferenceFilter(searchObjectType, predicate, referenceAttributeName);
        }

        public static string CreateDereferenceFilter(string searchObjectType, Dictionary<string, object> searchAttributes, ComparisonOperator valueComparisonOperator, GroupOperator groupOperator, string referenceAttributeName)
        {
            XPathQueryGroup predicate = new XPathQueryGroup(groupOperator, valueComparisonOperator, searchAttributes);
            return XPathFilterBuilder.CreateDereferenceFilter(searchObjectType, predicate, referenceAttributeName);
        }

        public static string CreateDereferenceFilter(string searchObjectType, IXPathQueryObject query, string referenceAttributeName)
        {
            XPathDereferencedExpression expression = new XPathDereferencedExpression(searchObjectType, referenceAttributeName, query);
            return expression.ToString();
        }

        internal static string CreateFilter(string objectType, GroupOperator queryOperator, params IXPathQueryObject[] queries)
        {
            return XPathFilterBuilder.CreateFilter(objectType, queryOperator, (IEnumerable<IXPathQueryObject>)queries);
        }

        internal static string CreateFilter(string objectType, GroupOperator queryOperator, IEnumerable<IXPathQueryObject> queries)
        {
            XPathQueryGroup group = new XPathQueryGroup(queryOperator, queries);
            group.GroupOperator = queryOperator;
            return XPathFilterBuilder.CreateFilter(objectType, group);
        }
    }
}