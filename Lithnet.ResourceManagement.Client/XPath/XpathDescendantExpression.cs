using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class XPathDescendantExpression : XPathExpression
    {
        public string DecendantsOfAttribute { get; set; }

        public XPathDescendantExpression(string objectType, string descendantsOfAttribute, IXPathPredicateComponent component)
            : base(objectType, component)
        {
            this.DecendantsOfAttribute = descendantsOfAttribute;
        }

        protected override string BuildFilter()
        {
            string baseFilter = base.BuildFilter();

            return string.Format("/{0}[descendant-in('{1}', {2})]", this.ObjectType, this.DecendantsOfAttribute, baseFilter);
        }
    }
}
