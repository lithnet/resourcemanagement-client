using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client
{
    internal class NativeClientFactory : IClientFactory
    {
        private ResourceManagementClientOptions parameters;

        public IResourceClient ResourceClient { get; private set; }

        public IResourceFactoryClient ResourceFactoryClient { get; private set; }

        public ISearchClient SearchClient { get; private set; }

        public ISchemaClient SchemaClient { get; private set; }

        public IApprovalClient ApprovalClient { get; private set; }

        public bool IsFaulted => false;

        public NativeClientFactory(ResourceManagementClientOptions p)
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

            var nativeResourceClient = new NativeResourceClient(wsHttpAuthenticatedBinding, endpointManager.ResourceEndpoint);
            var nativeResourceFactoryClient = new NativeResourceFactoryClient(wsHttpAuthenticatedBinding, endpointManager.ResourceFactoryEndpoint);
            var nativeSearchClient = new NativeSearchClient(wsHttpAuthenticatedBinding, endpointManager.SearchEndpoint);
            var nativeSchemaClient = new NativeSchemaClient(wsHttpBinding, endpointManager.MetadataEndpoint);
            var nativeApprovalClient = new NativeApprovalClient(wsHttpAuthenticatedContextBinding, creds);

            this.ConfigureChannelCredentials(nativeResourceClient, creds);
            this.ConfigureChannelCredentials(nativeResourceFactoryClient, creds);
            this.ConfigureChannelCredentials(nativeSearchClient, creds);

            this.ResourceClient = new ResourceClient(this, nativeResourceClient);
            this.ResourceFactoryClient = new ResourceFactoryClient(nativeResourceFactoryClient);
            this.SearchClient = new SearchClient(this, nativeSearchClient);
            this.SchemaClient = new SchemaClient(nativeSchemaClient);
            this.ApprovalClient = new ApprovalClient(nativeApprovalClient);

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
    }
}
