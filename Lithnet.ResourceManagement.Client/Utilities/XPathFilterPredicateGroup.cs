using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class XPathFilterPredicateGroup
    {
        public List<XPathFilterPredicate> Predicates { get; private set; }

        public QueryOperator GroupOperator { get; set; }

        public XPathFilterPredicateGroup()
        {
            this.Predicates = new List<XPathFilterPredicate>();
        }

        public XPathFilterPredicateGroup(IEnumerable<XPathFilterPredicate> predicates, QueryOperator groupOperator)
        {
            this.Predicates = predicates.ToList();
            this.GroupOperator = groupOperator;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (XPathFilterPredicate predicate in this.Predicates)
            {
                sb.AppendFormat("({0})", predicate.ToString());

                if (predicate != this.Predicates.Last())
                {
                    string op;

                    switch (this.GroupOperator)
                    {
                        case QueryOperator.And:
                            op = "and";
                            break;
                        case QueryOperator.Or:
                            op = "or";
                            break;
                        
                        default:
                            throw new InvalidOperationException("Unknown group operator");
                    }

                    sb.AppendFormat(" {0} ", op);
                }
            }

            return sb.ToString();
        }
    }
}
