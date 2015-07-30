using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class XPathExpression 
    {
        public string ObjectType { get; set; }

        public IXPathPredicateComponent Predicate { get; set; }

        public XPathExpression(string objectType, IXPathPredicateComponent predicate)
        {
            this.ObjectType = objectType;
            this.Predicate = predicate;
        }

        protected virtual string BuildFilter()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("/{0}[", this.ObjectType);

            sb.AppendFormat("{0}", this.Predicate.ToString());
            
            sb.AppendFormat("]");
           
            return sb.ToString();
        }

        public override string ToString()
        {
            return this.BuildFilter();
        }
    }
}
