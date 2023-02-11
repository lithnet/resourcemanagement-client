using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.Host;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Server
{
    public partial class ServiceCore : ServiceBase
    {
        private Task serviceTask;
        private CancellationTokenSource cts;

        public ServiceCore()
        {
            this.InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.StartTcpService();
        }

        protected override void OnStop()
        {
            this.cts.Cancel();
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
            this.serviceTask.Wait();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
        }

        public void StartTcpService()
        {
            this.cts = new CancellationTokenSource();
            this.serviceTask = NegotiateStreamRpcServer.StartNegotiateStreamServerAsync(IPAddress.IPv6Any, 5735, this.cts.Token);
        }

        public void WaitForServiceStop()
        {
            this.serviceTask?.Wait();
        }
    }
}
