using System;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// An XPath expression that involves dereferencing an attribute
    /// </summary>
    public class XPathDereferencedExpression : XPathExpression
    {
        /// <summary>
        /// Gets or sets the name of the attribute to dereference
        /// </summary>
        public string DereferenceAttribute { get; set; }

        public XPathDereferencedExpression() { }

        /// <summary>
        /// Initializes a new instance of the XPathDereferencedExpression class
        /// </summary>
        /// <param name="objectType">The object type used in the expression</param>
        /// <param name="dereferenceAttribute">The name of the attribute to dereference</param>
        /// <param name="query">The query used to build the expression</param>
        public XPathDereferencedExpression(string objectType, string dereferenceAttribute, IXPathQueryObject query)
            : this(objectType, dereferenceAttribute, query, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the XPathDereferencedExpression class
        /// </summary>
        /// <param name="objectType">The object type used in the expression</param>
        /// <param name="dereferenceAttribute">The name of the attribute to dereference</param>
        /// <param name="query">The query used to build the expression</param>
        /// <param name="wrapFilterXml">Indicates if the resulting expression should be wrapped in an XML filter element</param>
        public XPathDereferencedExpression(string objectType, string dereferenceAttribute, IXPathQueryObject query, bool wrapFilterXml)
            : base(objectType, query, wrapFilterXml)
        {
            this.DereferenceAttribute = dereferenceAttribute;
        }

        /// <summary>
        /// Builds the expression using the classes parameters
        /// </summary>
        /// <returns>A string representation of the dereferencing XPath expression</returns>
        private protected override string BuildExpression(IClient clientFactory)
        {
            string baseFilter = base.BuildExpression(clientFactory);

            if (this.DereferenceAttribute == null)
            {
                return baseFilter;
            }

            var attributeName = this.DereferenceAttribute;

            if (clientFactory != null)
            {
                var attribute = AsyncHelper.Run(async () => await clientFactory.SchemaClient.GetAttributeDefinitionAsync(this.DereferenceAttribute));

                if (attribute.Type != AttributeType.Reference)
                {
                    throw new InvalidOperationException("The dereference attribute must be a reference type");
                }

                attributeName = attribute.SystemName;
            }

            return string.Format("{0}/{1}", baseFilter, attributeName);
        }
    }
}
