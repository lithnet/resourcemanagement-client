using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class XPathDereferencedExpression : XPathExpression
    {
        public string DereferenceAttribute { get; set; }

        public XPathDereferencedExpression(string objectType, string dereferenceAttribute, IXPathPredicateComponent component)
            : base(objectType, component)
        {
            this.DereferenceAttribute = dereferenceAttribute;
        }

        protected override string BuildFilter()
        {
            string baseFilter = base.BuildFilter();

            return string.Format("{0}/{1}", baseFilter, this.DereferenceAttribute);
        }
    }
}
