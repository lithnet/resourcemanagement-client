using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client
{
    internal class NativeClient : IClient
    {
        private readonly ResourceManagementClientOptions parameters;
        private bool disposedValue;
        private NativeResourceClient resourceChannel;
        private NativeResourceFactoryClient resourceFactoryChannel;
        private NativeSearchClient searchChannel;
        private NativeSchemaClient schemaChannel;
        private NativeApprovalClient approvalChannel;

        public IResourceClient ResourceClient { get; private set; }

        public IResourceFactoryClient ResourceFactoryClient { get; private set; }

        public ISearchClient SearchClient { get; private set; }

        public ISchemaClient SchemaClient { get; private set; }

        public IApprovalClient ApprovalClient { get; private set; }

        public bool IsFaulted => false;

        public NativeClient(ResourceManagementClientOptions p)
        {
            this.parameters = p;
        }

        public Task InitializeClientsAsync()
        {
            var endpointManager = new EndpointManager(this.parameters.BaseUri, this.parameters.Spn);

            NetworkCredential creds = null;

            if (!string.IsNullOrWhiteSpace(this.parameters.Username))
            {
                creds = new NetworkCredential(this.parameters.Username, this.parameters.Password);
            }

            var wsHttpAuthenticatedContextBinding = BindingManager.GetWsHttpContextBinding(this.parameters.RecieveTimeoutSeconds, this.parameters.SendTimeoutSeconds);
            var wsHttpAuthenticatedBinding = BindingManager.GetWsAuthenticatedBinding(this.parameters.RecieveTimeoutSeconds, this.parameters.SendTimeoutSeconds);
            var wsHttpBinding = BindingManager.GetWsHttpBinding(this.parameters.RecieveTimeoutSeconds, this.parameters.SendTimeoutSeconds);

            this.resourceChannel = new NativeResourceClient(wsHttpAuthenticatedBinding, endpointManager.ResourceEndpoint);
            this.resourceFactoryChannel = new NativeResourceFactoryClient(wsHttpAuthenticatedBinding, endpointManager.ResourceFactoryEndpoint);
            this.searchChannel = new NativeSearchClient(wsHttpAuthenticatedBinding, endpointManager.SearchEndpoint);
            this.schemaChannel = new NativeSchemaClient(wsHttpBinding, endpointManager.MetadataEndpoint);
            this.approvalChannel = new NativeApprovalClient(wsHttpAuthenticatedContextBinding, creds);

            this.ConfigureChannelCredentials(this.resourceChannel, creds);
            this.ConfigureChannelCredentials(this.resourceFactoryChannel, creds);
            this.ConfigureChannelCredentials(this.searchChannel, creds);

            this.ResourceClient = new ResourceClient(this);
            this.ResourceFactoryClient = new ResourceFactoryClient(this);
            this.SearchClient = new SearchClient(this);
            this.SchemaClient = new SchemaClient(this);
            this.ApprovalClient = new ApprovalClient(this);

            return Task.CompletedTask;
        }

        private void ConfigureChannelCredentials<T>(ClientBase<T> channel, NetworkCredential creds) where T : class
        {
            if (creds != null)
            {
                channel.ClientCredentials.Windows.ClientCredential = creds;
            }

            channel.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
        }

        private Task EnsureConnectedAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
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
