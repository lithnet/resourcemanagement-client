using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client
{
    internal class NativeClientFactory : IClientFactory
    {
        private EndpointManager endpointManager;
        private Binding wsHttpContextBinding;
        private Binding wsHttpBinding;

        public IResourceClient ResourceClient
        {
            get; private set;
        }

        public IResourceFactoryClient ResourceFactoryClient
        {
            get; private set;
        }

        public ISearchClient SearchClient
        {
            get; private set;
        }

        public ISchemaClient SchemaClient
        {
            get; private set;
        }

        public string BaseUri
        {
            get; set;
        }

        public string Spn
        {
            get; set;
        }

        public int ConcurrentConnections
        {
            get; set;
        }

        public TimeSpan ConnectTimeout
        {
            get; set;
        }

        public int SendTimeout
        {
            get; set;
        }

        public int RecieveTimeout
        {
            get; set;
        }

        public NetworkCredential Credentials
        {
            get; set;
        }

        public Task InitializeClientsAsync(ResourceManagementClient rmc)
        {
#if NETFRAMEWORK
            this.endpointManager = new EndpointManager(new Uri(this.BaseUri), EndpointIdentity.CreateSpnIdentity(this.Spn));
#else
            this.endpointManager = new EndpointManager(new Uri(this.BaseUri), new SpnEndpointIdentity(this.Spn));
#endif

            this.wsHttpContextBinding = BindingManager.GetWsHttpContextBinding(this.RecieveTimeout, this.SendTimeout);
            this.wsHttpBinding = BindingManager.GetWsHttpBinding(this.RecieveTimeout, this.SendTimeout);

            var nativeResourceClient = new NativeResourceClient(this.wsHttpContextBinding, this.endpointManager.ResourceEndpoint);
            var nativeResourceFactoryClient = new NativeResourceFactoryClient(this.wsHttpContextBinding, this.endpointManager.ResourceFactoryEndpoint);
            var nativeSearchClient = new NativeSearchClient(this.wsHttpContextBinding, this.endpointManager.SearchEndpoint);
            var nativeSchemaClient = new NativeSchemaClient(this.wsHttpBinding, this.endpointManager.MetadataEndpoint);

            this.ConfigureChannelCredentials(nativeResourceClient);
            this.ConfigureChannelCredentials(nativeResourceFactoryClient);
            this.ConfigureChannelCredentials(nativeSearchClient);

            this.ResourceClient = new ResourceClient(rmc, nativeResourceClient);
            this.ResourceFactoryClient = new ResourceFactoryClient(nativeResourceFactoryClient);
            this.SearchClient = new SearchClient(rmc, nativeSearchClient);
            this.SchemaClient = new SchemaClient(nativeSchemaClient);

            return Task.CompletedTask;
        }

        public IResourceFactoryClient CreateApprovalClient(string endpoint)
        {
            var nativeResourceFactoryClient = new NativeResourceFactoryClient(this.wsHttpContextBinding, new EndpointAddress(endpoint));
            this.ConfigureChannelCredentials(nativeResourceFactoryClient);

            return new ResourceFactoryClient(nativeResourceFactoryClient);
        }

        private void ConfigureChannelCredentials<T>(ClientBase<T> channel) where T : class
        {
            if (this.Credentials != null)
            {
                channel.ClientCredentials.Windows.ClientCredential = this.Credentials;
            }

            channel.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
        }
    }
}
