using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client
{
    internal abstract class RpcClient : IClient
    {
        protected readonly ResourceManagementClientOptions parameters;
        protected JsonRpc jsonClient;
        private bool disposedValue;

        private IResource resourceChannel;
        private IResourceFactory resourceFactoryChannel;
        private ISearch searchChannel;
        private IMetadataExchange schemaChannel;
        private IApprovalService approvalChannel;

        public IResourceClient ResourceClient { get; private set; }

        public IResourceFactoryClient ResourceFactoryClient { get; private set; }

        public ISearchClient SearchClient { get; private set; }

        public ISchemaClient SchemaClient { get; private set; }

        public IApprovalClient ApprovalClient { get; private set; }

        public abstract bool IsFaulted { get; }

        public RpcClient(ResourceManagementClientOptions p)
        {
            this.parameters = p;
        }

        public async Task InitializeClientsAsync()
        {
            this.jsonClient = await this.GetJsonRpcClientAsync();

            this.jsonClient.TraceSource.Switch.Level = SourceLevels.Verbose;
            this.jsonClient.Disconnected += this.JsonClient_Disconnected;
            this.jsonClient.ExceptionStrategy = ExceptionProcessing.ISerializable;
            this.jsonClient.AllowModificationWhileListening = true;
            this.jsonClient.StartListening();

            var server = this.jsonClient.Attach<IRpcServer>(new JsonRpcProxyOptions { MethodNameTransform = x => $"Control_{x}" });

            await server.InitializeClientsAsync(this.parameters.BaseUri, this.parameters.Spn, this.parameters.ConcurrentConnectionLimit, this.parameters.SendTimeoutSeconds, this.parameters.RecieveTimeoutSeconds, this.parameters.Username, this.parameters.Password).ConfigureAwait(false);

            this.resourceChannel = this.jsonClient.Attach<IResource>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ResourceService));
            this.resourceFactoryChannel = this.jsonClient.Attach<IResourceFactory>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ResourceFactoryService));
            this.searchChannel = this.jsonClient.Attach<ISearch>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.SearchService));
            this.schemaChannel = this.jsonClient.Attach<IMetadataExchange>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.MetadataService));
            this.approvalChannel = this.jsonClient.Attach<IApprovalService>(JsonOptionsFactory.GetProxyOptions(JsonOptionsFactory.ApprovalService));

            this.ResourceClient = new ResourceClient(this);
            this.ResourceFactoryClient = new ResourceFactoryClient(this);
            this.SearchClient = new SearchClient(this);
            this.SchemaClient = new SchemaClient(this);
            this.ApprovalClient = new ApprovalClient(this);
        }
        
        private async Task<JsonRpc> GetJsonRpcClientAsync()
        {
            var stream = await this.GetStreamAsync();

            // GZipStream sendingStream = new GZipStream(stream, CompressionMode.Compress);
            // GZipStream receivingStream = new GZipStream(stream, CompressionMode.Decompress);

            return new JsonRpc(RpcCore.GetMessageHandler(stream, stream));
        }
        protected abstract Task<Stream> GetStreamAsync();

        private void JsonClient_Disconnected(object sender, JsonRpcDisconnectedEventArgs e)
        {
            Trace.WriteLine("Server disconnected");
        }

        private async Task EnsureConnectedAsync()
        {
            if (this.IsFaulted)
            {
                await this.InitializeClientsAsync();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.jsonClient?.Dispose();
                }

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        public async Task<IResource> GetResourceChannelAsync()
        {
            await this.EnsureConnectedAsync();
            return this.resourceChannel;
        }

        public async Task<IResourceFactory> GetResourceFactoryChannelAsync()
        {
            await this.EnsureConnectedAsync();
            return this.resourceFactoryChannel;
        }

        public async Task<ISearch> GetSearchChannelAsync()
        {
            await this.EnsureConnectedAsync();
            return this.searchChannel;
        }

        public async Task<IMetadataExchange> GetSchemaChannelAsync()
        {
            await this.EnsureConnectedAsync();
            return this.schemaChannel;
        }

        public async Task<IApprovalService> GetApprovalChannelAsync()
        {
            await this.EnsureConnectedAsync();
            return this.approvalChannel;
        }
    }
}
