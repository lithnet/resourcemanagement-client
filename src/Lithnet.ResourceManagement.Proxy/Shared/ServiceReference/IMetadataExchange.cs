using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    [ServiceContract(Namespace = "http://schemas.microsoft.com/2006/04/mex", ConfigurationName = "Lithnet.ResourceManagement.Client.ResourceManagementService.IMetadataExchange")]
    internal interface IMetadataExchange
    {
        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/transfer/GetResponse")]
        Task<Message> GetAsync(Message request);
    }
}
