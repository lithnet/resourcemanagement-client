using System;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal class NotImplementedApprovalClient : IApprovalService
    {
        public NotImplementedApprovalClient()
        {
        }

        public Task<Message> ApproveAsync(string endpoint, Message message)
        {
            throw new NotImplementedException($"Approvals cannot be performed with the connection mode currently in use. Please use either {nameof(ConnectionMode.DirectWsHttp)}, {nameof(ConnectionMode.LocalProxy)}, or {nameof(ConnectionMode.RemoteProxy)} mode");
        }
    }
}