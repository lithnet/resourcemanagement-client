using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class XPathAscendantExpression : XPathExpression
    {
        public string AscendantsOfAttribute { get; set; }

        public XPathAscendantExpression(string objectType, string ascendantsOfAttribute, IXPathPredicateComponent component)
            : base(objectType, component)
        {
            this.AscendantsOfAttribute = ascendantsOfAttribute;
        }

        protected override string BuildFilter()
        {
            string baseFilter = base.BuildFilter();
            return string.Format("descendants({0}, '{1}')", baseFilter, this.AscendantsOfAttribute);
        }
    }
}
