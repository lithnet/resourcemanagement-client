using System;

namespace Lithnet.ResourceManagement.Client.XPath
{
    public static class ChainableQueryExtensions
    {
        public static IAttributeConditionChainable WhereAttribute(this IChainableQuery q, string attributeName)
        {
            ((XPathFluentBuilder)q).NewQuery(attributeName);
            return (IAttributeConditionChainable)q;
        }

        public static IChainableQuery EndSubExpression(this IChainableQuery q)
        {
            var x = ((XPathFluentBuilder)q).EndExpression();
            return (IChainableQuery)x;
        }

        public static IChainableQuery StartOrGroup(this IChainableQuery q)
        {
            ((XPathFluentBuilder)q).StartOrGroup();
            return (IChainableQuery)q;
        }

        public static IChainableQuery StartAndGroup(this IChainableQuery q)
        {
            ((XPathFluentBuilder)q).StartAndGroup();
            return (IChainableQuery)q;
        }

        public static IChainableQuery StartNotAndGroup(this IChainableQuery q)
        {
            ((XPathFluentBuilder)q).StartNotAndGroup();
            return (IChainableQuery)q;
        }

        public static IChainableQuery StartNotOrGroup(this IChainableQuery q)
        {
            ((XPathFluentBuilder)q).StartNotOrGroup();
            return (IChainableQuery)q;
        }

        public static IChainableQuery EndGroup(this IChainableQuery q)
        {
            ((XPathFluentBuilder)q).EndGroup();
            return (IChainableQuery)q;
        }
    }
}