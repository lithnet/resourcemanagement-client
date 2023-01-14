using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    internal interface IRpcServer
    {
        Task InitializeClientsAsync(string baseUri, string spn, int concurrentConnectionLimit, int sendTimeout, int recieveTimeout, string username, string password);
    }
}