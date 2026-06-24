using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal interface IResourceFactoryClient
    {
        Task CreateAsync(IEnumerable<IResourceObject> resources);

        Task CreateAsync(IResourceObject resource);
    }
}