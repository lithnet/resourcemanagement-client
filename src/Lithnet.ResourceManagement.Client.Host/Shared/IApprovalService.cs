using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    internal interface IApprovalService
    {
        Task<Message> ApproveAsync(string endpoint, Message message);
    }
}
