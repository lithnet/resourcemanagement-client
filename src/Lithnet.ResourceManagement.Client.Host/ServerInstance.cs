using System.Threading;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal struct ServerInstance
    {
        public RpcServer Server { get; set; }

        public Task ServerTask { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public string PipeName { get; set; }
    }
}
