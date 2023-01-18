using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.Hosts;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    internal class RpcClientFactory : IClientFactory
    {
        private NamedPipeClientStream clientPipe;
        private string pipeName;
        private IPipeHost pipeHost;

        public IResourceClient ResourceClient { get; private set; }

        public IResourceFactoryClient ResourceFactoryClient { get; private set; }

        public ISearchClient SearchClient { get; private set; }

        public ISchemaClient SchemaClient { get; private set; }

        public IApprovalClient ApprovalClient { get; private set; }

        public string BaseUri { get; set; }

        public string Spn { get; set; }

        public int ConcurrentConnections { get; set; }

        public TimeSpan ConnectTimeout { get; set; }

        public int SendTimeout { get; set; }

        public int RecieveTimeout { get; set; }

        public NetworkCredential Credentials { get; set; }

        public async Task InitializeClientsAsync(ResourceManagementClient rmc)
        {
            var pipe = await this.GetOrCreateClientPipeAsync(this.ConnectTimeout, CancellationToken.None).ConfigureAwait(false);
            var jsonClient = new JsonRpc(RpcCore.GetMessageHandler(pipe));

            jsonClient.TraceSource.Switch.Level = SourceLevels.Warning;
            jsonClient.Disconnected += this.JsonClient_Disconnected;
            jsonClient.ExceptionStrategy = ExceptionProcessing.ISerializable;
            jsonClient.AllowModificationWhileListening = true;
            jsonClient.StartListening();

            var server = jsonClient.Attach<IRpcServer>(new JsonRpcProxyOptions { MethodNameTransform = x => $"Control_{x}" });

            await server.InitializeClientsAsync(this.BaseUri, this.Spn, this.ConcurrentConnections, this.SendTimeout, this.RecieveTimeout, this.Credentials?.UserName, this.Credentials?.Password).ConfigureAwait(false);

            var resourceChannel = jsonClient.Attach<IResource>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ResourceService));
            var resourceFactoryChannel = jsonClient.Attach<IResourceFactory>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ResourceFactoryService));
            var searchChannel = jsonClient.Attach<ISearch>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.SearchService));
            var schemaChannel = jsonClient.Attach<IMetadataExchange>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.MetadataService));
            var approvalChannel = jsonClient.Attach<IApprovalService>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ApprovalService));

            this.ResourceClient = new ResourceClient(rmc, resourceChannel);
            this.ResourceFactoryClient = new ResourceFactoryClient(resourceFactoryChannel);
            this.SearchClient = new SearchClient(rmc, searchChannel);
            this.SchemaClient = new SchemaClient(schemaChannel);
            this.ApprovalClient = new ApprovalClient(approvalChannel);
        }

        private void JsonClient_Disconnected(object sender, JsonRpcDisconnectedEventArgs e)
        {
            Trace.WriteLine("Server disconnected");
        }

        private async Task<NamedPipeClientStream> GetOrCreateClientPipeAsync(TimeSpan timeout, CancellationToken token)
        {
            if (this.clientPipe?.IsConnected != true)
            {
                this.clientPipe = await this.CreateClientPipeAsync(timeout, token).ConfigureAwait(false);
                this.clientPipe.WriteByte(255);
            }

            return this.clientPipe;
        }

        private async Task<NamedPipeClientStream> CreateClientPipeAsync(TimeSpan timeout, CancellationToken token)
        {
            this.pipeName = Guid.NewGuid().ToString();
            this.pipeHost = RmcConfiguration.UseComHost ? new ComPipeHost() : (IPipeHost)new ExePipeHost();
            this.pipeHost.OpenPipe(this.pipeName);

            var pipe = new NamedPipeClientStream(".", string.Format(RpcCore.PipeNameFormatTemplate, this.pipeName), PipeDirection.InOut, PipeOptions.Asynchronous, System.Security.Principal.TokenImpersonationLevel.Delegation);
            await pipe.ConnectAsync((int)timeout.TotalMilliseconds, token).ConfigureAwait(false);

            return pipe;
        }
    }
}
