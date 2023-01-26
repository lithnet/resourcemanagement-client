namespace Lithnet.ResourceManagement.Client.XPath
{
    public static class ExpressionRootExtensions
    {
        public static IObjectTypeQueryBase FindObjectsOfType(this IExpressionRoot q, string objectType)
        {
            ((XPathFluentBuilder)q).SetObjectType(objectType);
            return (IObjectTypeQueryBase)q;
        }

        public static IObjectTypeQueryBase FindObjectsOfAnyType(this IExpressionRoot q)
        {
            ((XPathFluentBuilder)q).SetAnyObjectType();
            return (IObjectTypeQueryBase)q;
        }
    }
}