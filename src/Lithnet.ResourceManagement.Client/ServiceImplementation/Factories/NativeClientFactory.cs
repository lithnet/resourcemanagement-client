using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client
{
    internal class NativeClientFactory : IClientFactory
    {
        private FactoryInitializationParameters parameters;

        public IResourceClient ResourceClient { get; private set; }

        public IResourceFactoryClient ResourceFactoryClient { get; private set; }

        public ISearchClient SearchClient { get; private set; }

        public ISchemaClient SchemaClient { get; private set; }

        public IApprovalClient ApprovalClient { get; private set; }

        public bool IsFaulted => false;

        public NativeClientFactory(FactoryInitializationParameters p)
        {
            this.parameters = p;
        }

        public Task InitializeClientsAsync()
        {
#if NETFRAMEWORK
            var endpointManager = new EndpointManager(new Uri(parameters.BaseUri), EndpointIdentity.CreateSpnIdentity(parameters.Spn));
#else
            var endpointManager = new EndpointManager(new Uri(this.parameters.BaseUri), new SpnEndpointIdentity(this.parameters.Spn));
#endif

            var wsHttpAuthenticatedContextBinding = BindingManager.GetWsHttpContextBinding(this.parameters.RecieveTimeout, this.parameters.SendTimeout);
            var wsHttpAuthenticatedBinding = BindingManager.GetWsAuthenticatedBinding(this.parameters.RecieveTimeout, this.parameters.SendTimeout);
            var wsHttpBinding = BindingManager.GetWsHttpBinding(this.parameters.RecieveTimeout, this.parameters.SendTimeout);

            var nativeResourceClient = new NativeResourceClient(wsHttpAuthenticatedBinding, endpointManager.ResourceEndpoint);
            var nativeResourceFactoryClient = new NativeResourceFactoryClient(wsHttpAuthenticatedBinding, endpointManager.ResourceFactoryEndpoint);
            var nativeSearchClient = new NativeSearchClient(wsHttpAuthenticatedBinding, endpointManager.SearchEndpoint);
            var nativeSchemaClient = new NativeSchemaClient(wsHttpBinding, endpointManager.MetadataEndpoint);
            var nativeApprovalClient = new NativeApprovalClient(wsHttpAuthenticatedContextBinding, this.parameters.Credentials);

            this.ConfigureChannelCredentials(nativeResourceClient);
            this.ConfigureChannelCredentials(nativeResourceFactoryClient);
            this.ConfigureChannelCredentials(nativeSearchClient);

            this.ResourceClient = new ResourceClient(this, nativeResourceClient);
            this.ResourceFactoryClient = new ResourceFactoryClient(nativeResourceFactoryClient);
            this.SearchClient = new SearchClient(this, nativeSearchClient);
            this.SchemaClient = new SchemaClient(nativeSchemaClient);
            this.ApprovalClient = new ApprovalClient(nativeApprovalClient);

            return Task.CompletedTask;
        }

        private void ConfigureChannelCredentials<T>(ClientBase<T> channel) where T : class
        {
            if (this.parameters.Credentials != null)
            {
                channel.ClientCredentials.Windows.ClientCredential = this.parameters.Credentials;
            }

            channel.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
        }
    }
}
