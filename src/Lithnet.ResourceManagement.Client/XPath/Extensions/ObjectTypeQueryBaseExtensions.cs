namespace Lithnet.ResourceManagement.Client.XPath
{
    public static class ObjectTypeQueryBaseExtensions
    {
        public static IChainableQuery StartOrGroup(this IObjectTypeQueryBase q)
        {
            ((XPathFluentBuilder)q).StartOrGroup();
            return (IChainableQuery)q;
        }

        public static IChainableQuery StartAndGroup(this IObjectTypeQueryBase q)
        {
            ((XPathFluentBuilder)q).StartAndGroup();
            return (IChainableQuery)q;
        }

        public static IChainableQuery StartNotAndGroup(this IObjectTypeQueryBase q)
        {
            ((XPathFluentBuilder)q).StartNotAndGroup();
            return (IChainableQuery)q;
        }

        public static IChainableQuery StartNotOrGroup(this IObjectTypeQueryBase q)
        {
            ((XPathFluentBuilder)q).StartNotOrGroup();
            return (IChainableQuery)q;
        }

        public static IObjectTypeQueryBase EndGroup(this IObjectTypeQueryBase q)
        {
            ((XPathFluentBuilder)q).EndGroup();
            return (IObjectTypeQueryBase)q;
        }

        public static IExpressionDereferenced Dereference(this IObjectTypeQueryBase q, string attributeName)
        {
            ((XPathFluentBuilder)q).Dereference(attributeName);
            return (IExpressionDereferenced)q;
        }

        public static IAttributeConditionFinal WhereAttribute(this IObjectTypeQueryBase q, string attributeName)
        {
            ((XPathFluentBuilder)q).NewQuery(attributeName);
            return (IAttributeConditionFinal)q;
        }
    }
}