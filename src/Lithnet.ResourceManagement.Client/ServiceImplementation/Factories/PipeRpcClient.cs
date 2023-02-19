using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.Hosts;

namespace Lithnet.ResourceManagement.Client
{
    internal class PipeRpcClient : RpcClient
    {
        private NamedPipeClientStream clientPipe;
        private string pipeName;
        private IPipeHost pipeHost;
        private string displayName;

        public override bool IsFaulted => !this.clientPipe?.IsConnected ?? false;

        public PipeRpcClient(ResourceManagementClientOptions p) : base(p)
        {
        }

        public override string DisplayName => this.displayName;

        protected override string MapUri(string baseUri)
        {
            return UriParser.GetFimServiceHttpUri(baseUri).ToString();
        }

        protected override async Task<Stream> GetStreamAsync()
        {
            var client = await this.GetOrCreateClientPipeAsync(TimeSpan.FromSeconds(this.parameters.ConnectTimeoutSeconds), CancellationToken.None).ConfigureAwait(false);

            var uri = this.parameters.GetFimServiceUri();
            this.displayName = $"Local proxy {this.pipeHost.HostLocation} connected to {uri}";

            return client;
        }

        private async Task<NamedPipeClientStream> GetOrCreateClientPipeAsync(TimeSpan timeout, CancellationToken token)
        {
            if (this.clientPipe?.IsConnected != true)
            {
                this.clientPipe = await this.CreateClientPipeAsync(timeout, token).ConfigureAwait(false);
                this.clientPipe.WriteByte(RpcCore.ClientInitialization);
                var firstByte = this.clientPipe.ReadByte();

                if (firstByte != RpcCore.ServerAck)
                {
                    throw new InvalidDataException("The server did not provide the correct initialization repsonse");
                }
            }

            return this.clientPipe;
        }

        private async Task<NamedPipeClientStream> CreateClientPipeAsync(TimeSpan timeout, CancellationToken token)
        {
            this.pipeName = Guid.NewGuid().ToString();
            this.pipeHost = new ExePipeHost();
            this.pipeHost.OpenPipe(this.pipeName, this.parameters);

            var pipe = new NamedPipeClientStream(".", string.Format(RpcCore.PipeNameFormatTemplate, this.pipeName), PipeDirection.InOut, PipeOptions.Asynchronous, System.Security.Principal.TokenImpersonationLevel.Delegation);
            await pipe.ConnectAsync((int)timeout.TotalMilliseconds, token).ConfigureAwait(false);

            return pipe;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.clientPipe.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
