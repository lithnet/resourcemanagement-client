using System.Threading.Tasks;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    [JsonRpcContract]
    internal partial interface IRpcServer
    {
        Task InitializeClientsAsync(string baseUri, string spn, int concurrentConnectionLimit, int sendTimeout, int recieveTimeout, string username, string password);
    }
}