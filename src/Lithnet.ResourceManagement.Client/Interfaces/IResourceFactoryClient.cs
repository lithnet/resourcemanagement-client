using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal interface IResourceFactoryClient
    {
        Task ApproveAsync(UniqueIdentifier workflowInstance, UniqueIdentifier approvalRequest, bool approve, string reason = null);

        Task CreateAsync(IEnumerable<ResourceObject> resources);

        Task CreateAsync(ResourceObject resource);
    }
}