using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal interface IApprovalClient
    {
        Task ApproveAsync(string endpoint, UniqueIdentifier workflowInstance, UniqueIdentifier approvalRequest, bool approve, string reason = null);
    }
}