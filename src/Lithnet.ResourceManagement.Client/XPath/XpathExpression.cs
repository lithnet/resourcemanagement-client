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
        /// Gets or sets a value indicating whether the resulting expression should be wrapped in a filter XML element
        /// </summary>
        public bool WrapFilterXml { get; set; }


        /// <summary>
        /// Initializes a new instance of the XPathExpression class
        /// </summary>
        public XPathExpression()
            : this(null, null, false)
        {
        }
        /// <summary>
        /// Initializes a new instance of the XPathExpression class
        /// </summary>
        /// <param name="objectType">The type of object to query for</param>
        public XPathExpression(string objectType)
            : this(objectType, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XPathExpression class
        /// </summary>
        /// <param name="objectType">The type of object to query for</param>
        /// <param name="query">The query component for this expression</param>
        public XPathExpression(string objectType, IXPathQueryObject query)
            : this(objectType, query, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XPathExpression class
        /// </summary>
        /// <param name="objectType">The type of object to query for</param>
        /// <param name="query">The query component for this expression</param>
        /// <param name="wrapFilterXml">Indicates if the resulting expression should be wrapped in a filter XML element</param>
        public XPathExpression(string objectType, IXPathQueryObject query, bool wrapFilterXml)
        {
            this.ObjectType = objectType;
            this.Query = query;
            this.WrapFilterXml = wrapFilterXml;
        }

        /// <summary>
        /// Builds the XPath expression
        /// </summary>
        /// <returns>The string representation of the expression</returns>
        protected virtual string BuildExpression()
        {
            StringBuilder sb = new StringBuilder();

            if (this.ObjectType != "*")
            {
                ResourceManagementSchema.ValidateObjectTypeName(this.ObjectType);
            }

            sb.AppendFormat("/{0}", this.ObjectType);

            if (this.Query != null)
            {
                sb.AppendFormat("[{0}]", this.Query.BuildQueryString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the string representation of the expression
        /// </summary>
        /// <returns>The string representation of the expression</returns>
        public override string ToString()
        {
            return this.ToString(this.WrapFilterXml);
        }

        /// <summary>
        /// Gets the string representation of the expression
        /// </summary>
        /// <param name="wrapFilterXml">A value that indicates if the expression should be wrapped in an XML filter element</param>
        /// <returns>The string representation of the expression</returns>
        public string ToString(bool wrapFilterXml)
        {
            if (wrapFilterXml)
            {
                return this.BuildExpression().ToResourceManagementFilterXml();
            }
            else
            {
                return this.BuildExpression();
            }
        }
    }
}
