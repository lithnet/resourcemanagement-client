using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal interface IRpcServer
    {
        Task InitializeClientsAsync(string baseUri, string spn, int concurrentConnectionLimit, int sendTimeout, int recieveTimeout, string username, string password);
    }
}