using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Initializes a new instance of the XPathDereferencedExpression class
        /// </summary>
        /// <param name="objectType">The object type used in the expression</param>
        /// <param name="dereferenceAttribute">The name of the attribute to dereference</param>
        /// <param name="query">The query used to build the expression</param>
        public XPathDereferencedExpression(string objectType, string dereferenceAttribute, IXPathQueryObject query)
            : base(objectType, query)
        {
            this.DereferenceAttribute = dereferenceAttribute;
        }

        /// <summary>
        /// Builds the expression using the classes parameters
        /// </summary>
        /// <returns>A string representation of the dereferencing XPath expression</returns>
        protected override string BuildExpression()
        {
            string baseFilter = base.BuildExpression();

            return string.Format("{0}/{1}", baseFilter, this.DereferenceAttribute);
        }
    }
}
