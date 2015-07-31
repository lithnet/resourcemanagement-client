using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// A class used to build a standard XPath expression, containing one or more queries
    /// </summary>
    public class XPathExpression 
    {
        /// <summary>
        /// Gets or sets the type of object to query
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the query component for this expression. This may be a <c>XPathQuery</c> or <c>XPathQueryGroup</c>
        /// </summary>
        public IXPathQueryObject Query { get; set; }

        /// <summary>
        /// Initializes a new instance of the XPathExpression class
        /// </summary>
        /// <param name="objectType">The type of object to query for</param>
        /// <param name="query">The query component for this expression</param>
        public XPathExpression(string objectType, IXPathQueryObject query)
        {
            this.ObjectType = objectType;
            this.Query = query;
        }

        /// <summary>
        /// Builds the XPath expression
        /// </summary>
        /// <returns>The string representation of the expression</returns>
        protected virtual string BuildExpression()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("/{0}[", this.ObjectType);

            sb.AppendFormat("{0}", this.Query.BuildQueryString());
            
            sb.AppendFormat("]");
           
            return sb.ToString();
        }

        /// <summary>
        /// Gets a string that represents this object
        /// </summary>
        /// <returns>The string representation of the expression</returns>
        public override string ToString()
        {
            return this.BuildExpression();
        }
    }
}
