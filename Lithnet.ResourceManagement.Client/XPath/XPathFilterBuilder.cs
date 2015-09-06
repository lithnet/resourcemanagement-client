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

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute value pair
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="attributeName">The name of the attribute to query</param>
        /// <param name="comparisonOperator">The operator used to compare the attribute and value</param>
        /// <param name="attributeValue">The value of the attribute to query</param>
        /// <returns>An XPath query string</returns>
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
        /// <param name="valueComparisonOperator">The operator used to compare the attribute and value pairs</param>
        /// <param name="groupOperator">The operator to use to join the attribute value pair comparisons together</param>
        /// <returns>An XPath query string</returns>
        public static string CreateFilter(string objectType, Dictionary<string, object> keyValuePairs, ComparisonOperator valueComparisonOperator, GroupOperator groupOperator)
        {
            return XPathFilterBuilder.CreateFilter(objectType, new XPathQueryGroup(groupOperator, keyValuePairs, valueComparisonOperator));
        }

        /// <summary>
        /// Creates an XPath filter for the specified object type and attribute/value pairs. Multiple pairs are joined with an 'and' operator
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <param name="valueComparisonOperator">The operator used to compare the attribute and value pairs</param>
        /// <param name="groupOperator">The operator to use to join the attribute value pair comparisons together</param>
        /// <returns>An XPath query string</returns>
        public static string CreateFilter(string objectType, AttributeValuePairCollection keyValuePairs, ComparisonOperator valueComparisonOperator, GroupOperator groupOperator)
        {
            return XPathFilterBuilder.CreateFilter(objectType, new XPathQueryGroup(groupOperator, keyValuePairs, valueComparisonOperator));
        }

        /// <summary>
        /// Creates an XPath filter using the specified query object and object type
        /// </summary>
        /// <param name="objectType">The type of object to query</param>
        /// <param name="query">The IXPathQueryObject that contains the query logic to use in the expression</param>
        /// <returns>An XPath query string</returns>
        public static string CreateFilter(string objectType, IXPathQueryObject query)
        {
            return new XPathExpression(objectType, query).ToString();
        }

        /// <summary>
        /// Creates a filter that is a union of the results of one or more expressions. This function joins the expressions together with a logical or operator.
        /// </summary>
        /// <param name="expressions">The XPathExpression objects to union</param>
        /// <returns>An XPath query string</returns>
        public static string CreateUnionFilter(params XPathExpression[] expressions)
        {
            return expressions.Select(t => t.ToString()).ToSeparatedString(" | ");
        }

        /// <summary>
        /// Creates a filter that dereferences a matching expression, and returns the resulting values from the referenced attribute
        /// </summary>
        /// <param name="searchObjectType">The object type to query</param>
        /// <param name="searchAttributeName">The name of the attribute to query</param>
        /// <param name="searchAttributeValue">The value of the attribute to query</param>
        /// <param name="referenceAttributeName">The name of the attribute to dereference</param>
        /// <returns>An XPath query string</returns>
        public static string CreateDereferenceFilter(string searchObjectType, string searchAttributeName, object searchAttributeValue, string referenceAttributeName)
        {
            XPathQuery predicate = new XPathQuery(searchAttributeName, ComparisonOperator.Equals, searchAttributeValue);
            return XPathFilterBuilder.CreateDereferenceFilter(searchObjectType, predicate, referenceAttributeName);
        }

        /// <summary>
        /// Creates a filter that dereferences a matching expression, and returns the resulting values from the referenced attribute
        /// </summary>
        /// <param name="searchObjectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <param name="valueComparisonOperator">The operator used to compare the attribute and value pairs</param>
        /// <param name="groupOperator">The operator to use to join the attribute value pair comparisons together</param>
        /// <param name="referenceAttributeName">The name of the attribute used to dereference the expression</param>
        /// <returns>An XPath query string</returns>
        public static string CreateDereferenceFilter(string searchObjectType, AttributeValuePairCollection keyValuePairs, ComparisonOperator valueComparisonOperator, GroupOperator groupOperator, string referenceAttributeName)
        {
            XPathQueryGroup predicate = new XPathQueryGroup(groupOperator, keyValuePairs, valueComparisonOperator);
            return XPathFilterBuilder.CreateDereferenceFilter(searchObjectType, predicate, referenceAttributeName);
        }

        /// <summary>
        /// Creates a filter that dereferences a matching expression, and returns the resulting values from the referenced attribute
        /// </summary>
        /// <param name="searchObjectType">The object type to query</param>
        /// <param name="keyValuePairs">The list to attribute and value pairs to query for</param>
        /// <param name="valueComparisonOperator">The operator used to compare the attribute and value pairs</param>
        /// <param name="groupOperator">The operator to use to join the attribute value pair comparisons together</param>
        /// <param name="referenceAttributeName">The name of the attribute used to dereference the expression</param>
        /// <returns>An XPath query string</returns>
        public static string CreateDereferenceFilter(string searchObjectType, Dictionary<string, object> keyValuePairs, ComparisonOperator valueComparisonOperator, GroupOperator groupOperator, string referenceAttributeName)
        {
            XPathQueryGroup predicate = new XPathQueryGroup(groupOperator, keyValuePairs, valueComparisonOperator);
            return XPathFilterBuilder.CreateDereferenceFilter(searchObjectType, predicate, referenceAttributeName);
        }

        /// <summary>
        /// Creates a filter that dereferences a matching expression, and returns the resulting values from the referenced attribute
        /// </summary>
        /// <param name="searchObjectType">The type of object to query</param>
        /// <param name="query">The IXPathQueryObject that contains the query logic to use in the expression</param>
        /// <param name="referenceAttributeName">The name of the attribute used to dereference the expression</param>
        /// <returns>An XPath query string</returns>
        public static string CreateDereferenceFilter(string searchObjectType, IXPathQueryObject query, string referenceAttributeName)
        {
            XPathDereferencedExpression expression = new XPathDereferencedExpression(searchObjectType, referenceAttributeName, query);
            return expression.ToString();
        }

        /// <summary>
        /// Creates a filter using the specified query objects
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="queryOperator">The operator used to compare the query elements</param>
        /// <param name="queries">The query elements</param>
        /// <returns>An XPath query string</returns>
        internal static string CreateFilter(string objectType, GroupOperator queryOperator, params IXPathQueryObject[] queries)
        {
            return XPathFilterBuilder.CreateFilter(objectType, queryOperator, (IEnumerable<IXPathQueryObject>)queries);
        }

        /// <summary>
        /// Creates a filter using the specified query objects
        /// </summary>
        /// <param name="objectType">The object type to query</param>
        /// <param name="queryOperator">The operator used to compare the query elements</param>
        /// <param name="queries">The query elements</param>
        /// <returns>An XPath query string</returns>
        internal static string CreateFilter(string objectType, GroupOperator queryOperator, IEnumerable<IXPathQueryObject> queries)
        {
            XPathQueryGroup group = new XPathQueryGroup(queryOperator, queries);
            group.GroupOperator = queryOperator;
            return XPathFilterBuilder.CreateFilter(objectType, group);
        }
    }
}