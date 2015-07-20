using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public interface ISearchResults : IEnumerable<ResourceObject>
    {
        int Count { get; }
    }
}
