using System;
using System.Diagnostics;
using System.IO.Pipes;
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
        private readonly FactoryInitializationParameters parameters;

        public IResourceClient ResourceClient { get; private set; }

        public IResourceFactoryClient ResourceFactoryClient { get; private set; }

        public ISearchClient SearchClient { get; private set; }

        public ISchemaClient SchemaClient { get; private set; }

        public IApprovalClient ApprovalClient { get; private set; }

        public bool IsFaulted => !this.clientPipe.IsConnected;

        public RpcClientFactory(FactoryInitializationParameters p)
        {
            this.parameters = p;
        }

        public async Task InitializeClientsAsync()
        {
            var pipe = await this.GetOrCreateClientPipeAsync(parameters.ConnectTimeout, CancellationToken.None).ConfigureAwait(false);
            var jsonClient = new JsonRpc(RpcCore.GetMessageHandler(pipe));

            jsonClient.TraceSource.Switch.Level = SourceLevels.Warning;
            jsonClient.Disconnected += this.JsonClient_Disconnected;
            jsonClient.ExceptionStrategy = ExceptionProcessing.ISerializable;
            jsonClient.AllowModificationWhileListening = true;
            jsonClient.StartListening();

            var server = jsonClient.Attach<IRpcServer>(new JsonRpcProxyOptions { MethodNameTransform = x => $"Control_{x}" });

            await server.InitializeClientsAsync(parameters.BaseUri, parameters.Spn, parameters.ConcurrentConnections, parameters.SendTimeout, parameters.RecieveTimeout, parameters.Credentials?.UserName, parameters.Credentials?.Password).ConfigureAwait(false);

            var resourceChannel = jsonClient.Attach<IResource>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ResourceService));
            var resourceFactoryChannel = jsonClient.Attach<IResourceFactory>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ResourceFactoryService));
            var searchChannel = jsonClient.Attach<ISearch>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.SearchService));
            var schemaChannel = jsonClient.Attach<IMetadataExchange>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.MetadataService));
            var approvalChannel = jsonClient.Attach<IApprovalService>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ApprovalService));

            this.ResourceClient = new ResourceClient(this, resourceChannel);
            this.ResourceFactoryClient = new ResourceFactoryClient(resourceFactoryChannel);
            this.SearchClient = new SearchClient(this, searchChannel);
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
