using System;

namespace Lithnet.ResourceManagement.Client.XPath
{
    public static class AttributeConditionFinalExtensions
    {
        public static ICompletedExpression IsPresent(this IAttributeConditionFinal q)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.IsPresent, null);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression IsNotPresent(this IAttributeConditionFinal q)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.IsNotPresent, null);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueEquals(this IAttributeConditionFinal q, string value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Equals, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueEquals(this IAttributeConditionFinal q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Equals, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueEquals(this IAttributeConditionFinal q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Equals, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueEquals(this IAttributeConditionFinal q, bool value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Equals, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueIsGreaterThan(this IAttributeConditionFinal q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.GreaterThan, value);
            return (ICompletedExpression)q;
        }
        public static ICompletedExpression ValueIsGreaterThan(this IAttributeConditionFinal q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.GreaterThan, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueIsGreaterThanOrEqualTo(this IAttributeConditionFinal q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.GreaterThanOrEquals, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueIsGreaterThanOrEqualTo(this IAttributeConditionFinal q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.GreaterThanOrEquals, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueIsLessThan(this IAttributeConditionFinal q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.LessThan, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueIsLessThan(this IAttributeConditionFinal q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.LessThan, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueIsLessThanOrEqualTo(this IAttributeConditionFinal q, long value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.LessThanOrEquals, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueIsLessThanOrEqualTo(this IAttributeConditionFinal q, DateTime value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.LessThanOrEquals, value);
            return (ICompletedExpression)q;
        }

        public static ICompletedExpression ValueContainsString(this IAttributeConditionFinal q, string value)
        {
            ((XPathFluentBuilder)q).CompleteQuery(ComparisonOperator.Contains, value);
            return (ICompletedExpression)q;
        }

        public static IExpressionRoot ReferenceValueMatchesSubExpression(this IAttributeConditionFinal q)
        {
            var x = ((XPathFluentBuilder)q);
            var y = x.CreateExpression();
            x.CompleteQuery(ComparisonOperator.Equals, y);
            x.StartExpression(y);
            return (IExpressionRoot)q;
        }
    }
}