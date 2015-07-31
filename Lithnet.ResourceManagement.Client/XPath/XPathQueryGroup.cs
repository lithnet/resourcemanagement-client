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
        /// Initializes a new instance of the XpathQueryGroup class
        /// </summary>
        public XPathQueryGroup()
            :this (GroupOperator.And, new List<IXPathQueryObject>())
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

        public XPathQueryGroup(GroupOperator groupOperator, ComparisonOperator valueComparisonOperator, AttributeValuePairCollection attributeValuePairs)
        {
            this.Queries = new List<IXPathQueryObject>();

            foreach(AttributeValuePair value in attributeValuePairs)
            {
                this.Queries.Add(new XPathQuery(value.AttributeName, valueComparisonOperator, value.Value));
            }

            this.GroupOperator = groupOperator;
        }

        public XPathQueryGroup(GroupOperator groupOperator, ComparisonOperator valueComparisonOperator, Dictionary<string, object> attributeValuePairs)
        {
            this.Queries = new List<IXPathQueryObject>();

            foreach (KeyValuePair<string, object> value in attributeValuePairs)
            {
                this.Queries.Add(new XPathQuery(value.Key, valueComparisonOperator, value.Value));
            }

            this.GroupOperator = groupOperator;
        }

        public override string ToString()
        {
            return this.BuildQueryString();
        }

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
                return string.Format("({0})", sb.ToString());
            }
            else
            {
                return sb.ToString();

            }
        }
    }
}
