using System.Text;
using Nito.AsyncEx;

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
        {
        }

        /// <summary>
        /// Initializes a new instance of the XPathExpression class
        /// </summary>
        /// <param name="objectType">The type of object to query for</param>
        public XPathExpression(ObjectTypeDefinition objectType)
            : this(objectType.SystemName, null, false)
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
        public XPathExpression(ObjectTypeDefinition objectType, IXPathQueryObject query)
            : this(objectType.SystemName, query, false)
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
        public XPathExpression(ObjectTypeDefinition objectType, IXPathQueryObject query, bool wrapFilterXml)
            : this(objectType.SystemName, query, false)
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
        private protected virtual string BuildExpression(IClientFactory clientFactory)
        {
            StringBuilder sb = new StringBuilder();

            string ot;

            if (string.IsNullOrWhiteSpace(this.ObjectType) || this.ObjectType == "*")
            {
                ot = "*";
            }
            else
            {
                ot = clientFactory == null
                    ? this.ObjectType
                    : AsyncContext.Run(async () => await clientFactory.SchemaClient.GetCorrectObjectTypeNameCaseAsync(this.ObjectType));
            }

            sb.AppendFormat("/{0}", ot);

            if (this.Query != null)
            {
                sb.AppendFormat("[{0}]", this.Query.BuildQueryString());
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return this.ToString(null, this.WrapFilterXml);
        }

        internal string BuildExpression(IClientFactory factory, bool wrapFilterXml)
        {
            if (wrapFilterXml)
            {
                return this.BuildExpression(factory).ToResourceManagementFilterXml();
            }
            else
            {
                return this.BuildExpression(factory);
            }
        }

        /// <summary>
        /// Gets the string representation of the expression
        /// </summary>
        /// <returns>The string representation of the expression</returns>
        public string ToString(ResourceManagementClient client)
        {
            return this.BuildExpression(client?.ClientFactory, this.WrapFilterXml);
        }

        /// <summary>
        /// Gets the string representation of the expression
        /// </summary>
        /// <param name="wrapFilterXml">A value that indicates if the expression should be wrapped in an XML filter element</param>
        /// <returns>The string representation of the expression</returns>
        public string ToString(ResourceManagementClient client, bool wrapFilterXml)
        {
            return this.BuildExpression(client?.ClientFactory, wrapFilterXml);
        }
    }
}
