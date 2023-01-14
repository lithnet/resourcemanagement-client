using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// An enumeration of resource objects returned from a search
    /// </summary>
    public interface ISearchResultCollection : IEnumerable<ResourceObject>
    {
        /// <summary>
        /// The number of objects returned from the search
        /// </summary>
        int Count { get; }
    }
}
