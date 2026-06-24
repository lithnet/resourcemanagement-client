using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client
{
    public interface IAttributeValueCollection : IEnumerable<AttributeValue>
    {
        AttributeValue this[string attributeName] { get; }

        bool ContainsAttribute(string attributeName);
    }
}
