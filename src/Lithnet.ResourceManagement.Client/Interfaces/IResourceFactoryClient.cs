using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal interface IResourceFactoryClient
    {
        Task CreateAsync(IEnumerable<ResourceObject> resources);

        Task CreateAsync(ResourceObject resource);
    }
}