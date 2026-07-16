using System.ServiceModel.Channels;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    [JsonRpcContract]
    internal partial interface IApprovalService
    {
        Task<Message> ApproveAsync(string endpoint, Message message);
    }
}
