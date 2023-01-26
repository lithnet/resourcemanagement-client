using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Represents a group of predicates within an XPath expression
    /// </summary>
    public class XPathQueryGroup : IXPathQueryObject
    {
        /// <summary>
        /// Gets the list of query objects used in the group
        /// </summary>
        public List<IXPathQueryObject> Queries { get; private set; }

        /// <summary>
        /// Gets the logical operator to apply to queries within this group
        /// </summary>
        public GroupOperator GroupOperator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the resulting group should be negated
        /// </summary>
        public bool Negate { get; set; }

        /// <summary>
        /// Initializes a new instance of the XpathQueryGroup class
        /// </summary>
        internal XPathQueryGroup()
        {
            this.GroupOperator = GroupOperator.And;
            this.Queries = new List<IXPathQueryObject>();
        }

        /// <summary>
        /// Initializes a new instance of the XpathQueryGroup class
        /// </summary>
        /// <param name="groupOperator">The logical operator to apply to queries within this group</param>
        public XPathQueryGroup(GroupOperator groupOperator)
            : this(groupOperator, new List<IXPathQueryObject>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the XpathQueryGroup class
        /// </summary>
        /// <param name="groupOperator">The logical operator to apply to queries within this group</param>
        /// <param name="queries">The query objects used in the group</param>
        public XPathQueryGroup(GroupOperator groupOperator, params IXPathQueryObject[] queries)
            : this(groupOperator, (IEnumerable<IXPathQueryObject>)queries)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XpathQueryGroup class
        /// </summary>
        /// <param name="groupOperator">The logical operator to apply to queries within this group</param>
        /// <param name="queries">The query objects used in the group</param>
        public XPathQueryGroup(GroupOperator groupOperator, IEnumerable<IXPathQueryObject> queries)
        {
            this.Queries = queries.ToList<IXPathQueryObject>();
            this.GroupOperator = groupOperator;
        }

        /// <summary>
        /// Initializes a new instance of the XpathQueryGroup class
        /// </summary>
        /// <param name="groupOperator">The logical operator to apply to queries within this group</param>
        /// <param name="attributeValuePairs">The attribute and value pairs to query</param>
        /// <param name="valueComparisonOperator">The operator to apply to the individual attribute and value pairs</param>
        public XPathQueryGroup(GroupOperator groupOperator, AttributeValuePairCollection attributeValuePairs, ComparisonOperator valueComparisonOperator) 
        {
            this.Queries = new List<IXPathQueryObject>();

            foreach (AttributeValuePair value in attributeValuePairs)
            {
                this.Queries.Add(new XPathQuery(value.Attribute, valueComparisonOperator, value.Value));
            }

            this.GroupOperator = groupOperator;
        }

        /// <summary>
        /// Get the XPath query string
        /// </summary>
        /// <returns>The string representation of the query group</returns>
        public override string ToString()
        {
            return this.BuildQueryString();
        }

        /// <summary>
        /// Builds the XPath query string, and returns the resulting string
        /// </summary>
        /// <returns>The string representation of the query group</returns>
        public string BuildQueryString()
        {
            if (this.Queries == null || this.Queries.Count == 0)
            {
                throw new InvalidOperationException("There were no predicates in the group");
            }

            StringBuilder sb = new StringBuilder();

            foreach (IXPathQueryObject query in this.Queries)
            {
                sb.AppendFormat("{0}", query.ToString());

                if (query != this.Queries.Last())
                {
                    sb.AppendFormat(" {0} ", this.GroupOperator.ToString().ToLower());
                }
            }

            if (this.Queries.Count > 1)
            {
                if (this.Negate)
                {
                    return string.Format("(not({0}))", sb.ToString());
                }
                else
                {
                    return string.Format("({0})", sb.ToString());
                }
            }
            else
            {
                if (this.Negate)
                {
                    return string.Format("(not{0})", sb.ToString());
                }
                else
                {
                    return sb.ToString();
                }
            }
        }

        private string NegateGroup(string value)
        {
            if (this.Negate)
            {
                return string.Format("(not({0}))", value);
            }
            else
            {
                return value;
            }
        }
    }
}
