using System;

namespace Lithnet.ResourceManagement.Client.XPath
{
    public static class AttributeConditionChainableExtensions
    {
        public static IChainableQuery IsPresent(this IAttributeConditionChainable q)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.IsPresent, null);
            return (IChainableQuery)q;
        }

        public static IChainableQuery IsNotPresent(this IAttributeConditionChainable q)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.IsNotPresent, null);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueEquals(this IAttributeConditionChainable q, string value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Equals, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueEquals(this IAttributeConditionChainable q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Equals, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueEquals(this IAttributeConditionChainable q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Equals, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueEquals(this IAttributeConditionChainable q, bool value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Equals, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueIsGreaterThan(this IAttributeConditionChainable q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.GreaterThan, value);
            return (IChainableQuery)q;
        }
        public static IChainableQuery ValueIsGreaterThan(this IAttributeConditionChainable q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.GreaterThan, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueIsGreaterThanOrEqualTo(this IAttributeConditionChainable q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.GreaterThanOrEquals, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueIsGreaterThanOrEqualTo(this IAttributeConditionChainable q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.GreaterThanOrEquals, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueIsLessThan(this IAttributeConditionChainable q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.LessThan, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueIsLessThan(this IAttributeConditionChainable q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.LessThan, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueIsLessThanOrEqualTo(this IAttributeConditionChainable q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.LessThanOrEquals, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueIsLessThanOrEqualTo(this IAttributeConditionChainable q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.LessThanOrEquals, value);
            return (IChainableQuery)q;
        }

        public static IChainableQuery ValueContainsString(this IAttributeConditionChainable q, string value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Contains, value);
            return (IChainableQuery)q;
        }

        public static IExpressionRoot ReferenceValueMatchesSubExpression(this IAttributeConditionChainable q)
        {
            var x = ((XPathFluentBuilder)q);
            var y = x.CreateExpression();
            x.CompleteQuery(ComparisonOperator.Equals, y);
            x.StartExpression(y);
            return (IExpressionRoot)q;
        }
    }
}