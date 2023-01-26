namespace Lithnet.ResourceManagement.Client.XPath
{
    public static class CompletedExpressionExtensions
    {
        public static IObjectTypeQueryBase AlsoFindObjectsOfType(this ICompletedExpression q, string objectType)
        {
            ((XPathFluentBuilder)q).AddAdditionalExpression(objectType);
            return (IObjectTypeQueryBase)q;
        }

        public static IObjectTypeQueryBase AlsoFindObjectsOfAnyType(this ICompletedExpression q)
        {
            ((XPathFluentBuilder)q).AddAdditionalExpression("*");
            return (IObjectTypeQueryBase)q;
        }

        public static IChainableQuery EndSubExpression(this ICompletedExpression q)
        {
            var x = ((XPathFluentBuilder)q).EndExpression();
            return (IChainableQuery)q;
        }

        public static IExpressionDereferenced Dereference(this ICompletedExpression q, string attributeName)
        {
            ((XPathFluentBuilder)q).Dereference(attributeName);
            return (IExpressionDereferenced)q;
        }
    }
}