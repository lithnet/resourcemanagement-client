namespace Lithnet.ResourceManagement.Client.XPath
{
    public interface ICompletedExpression
    {
        string BuildQuery();

        string BuildFilter();
    }
}