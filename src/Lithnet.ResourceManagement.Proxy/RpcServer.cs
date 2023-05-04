using System;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Proxy
{
    public abstract class RpcServer : IRpcServer
    {
        private JsonRpc jsonRpcServer;
        private bool initialized;

        protected WindowsIdentity impersonationIdentity;

        protected abstract bool RequiresImpersonationOrExplicitCredentials { get; }

        protected RpcServer()
        {
        }

        public async Task SetupRpcServerAsync(IJsonRpcMessageHandler messageHandler)
        {
            this.jsonRpcServer = new RmcJsonRpc(messageHandler);
            this.jsonRpcServer.TraceSource.Switch.Level = SourceLevels.Warning;
            this.jsonRpcServer.ExceptionStrategy = ExceptionProcessing.ISerializable;
            this.jsonRpcServer.AllowModificationWhileListening = true;
            this.jsonRpcServer.AddLocalRpcTarget(this, new JsonRpcTargetOptions() { MethodNameTransform = (x) => $"Control_{x}" });

            this.jsonRpcServer.Disconnected += (sender, u) => Logger.LogTrace("Client has disconnected");
            this.jsonRpcServer.StartListening();

            Logger.LogTrace("RPC server has connected client and waiting for messages");
            await this.jsonRpcServer.Completion.ConfigureAwait(false);

            Logger.LogTrace("RPC server has completed");
        }

        private protected void InitializeClient<T>(ClientBase<T> client, NetworkCredential credentials) where T : class
        {
            if (credentials != null)
            {
                client.ClientCredentials.Windows.ClientCredential = credentials;
            }
            else
            {
                client.ClientCredentials.Windows.ClientCredential = (NetworkCredential)CredentialCache.DefaultCredentials;
            }

            client.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Identification;
        }

        private protected abstract Uri MapApprovalUri(string uri);

        private protected abstract Uri MapBaseUri(string uri);

        public Task InitializeClientsAsync(string baseUri, string spn, int concurrentConnectionLimit, int sendTimeout, int recieveTimeout, string username, string password)
        {
            if (this.initialized)
            {
                throw new InvalidOperationException("The server has already been initialized");
            }

            if (concurrentConnectionLimit > 0)
            {
                ServicePointManager.DefaultConnectionLimit = concurrentConnectionLimit;
            }

            NetworkCredential credentials = null;

            if (username != null)
            {
                credentials = new NetworkCredential(username, password);
            }

            if (this.RequiresImpersonationOrExplicitCredentials)
            {
                if (credentials == null && this.impersonationIdentity?.ImpersonationLevel != TokenImpersonationLevel.Impersonation)
                {
                    throw new UnauthorizedAccessException($"Credentials were not provided, and an valid impersonation identity was not available. Impersonation level {this.impersonationIdentity?.ImpersonationLevel}");
                }
            }

            var uri = this.MapBaseUri(baseUri);
            Trace.WriteLine($"Mapped {baseUri} to {uri} with supplied SPN {spn}");

            var endpoints = new EndpointManager(uri, spn);

            sendTimeout = Math.Max(10, sendTimeout);
            recieveTimeout = Math.Max(10, recieveTimeout);

            var wsHttpAuthenticatedBinding = BindingManager.GetWsAuthenticatedBinding(recieveTimeout, sendTimeout);
            var wsHttpBinding = BindingManager.GetWsHttpBinding(recieveTimeout, sendTimeout);
            var wsHttpAuthenticatedContextBinding = BindingManager.GetWsHttpContextBinding(recieveTimeout, sendTimeout);

            NativeResourceClient resourceClient = new NativeResourceClient(wsHttpAuthenticatedBinding, endpoints.ResourceEndpoint);
            this.InitializeClient(resourceClient, credentials);

            NativeResourceFactoryClient resourceFactoryClient = new NativeResourceFactoryClient(wsHttpAuthenticatedBinding, endpoints.ResourceFactoryEndpoint);
            this.InitializeClient(resourceFactoryClient, credentials);

            NativeSchemaClient metadataClient = new NativeSchemaClient(wsHttpBinding, endpoints.MetadataEndpoint);
            this.InitializeClient(metadataClient, credentials);

            NativeSearchClient searchClient = new NativeSearchClient(wsHttpAuthenticatedBinding, endpoints.SearchEndpoint);
            this.InitializeClient(searchClient, credentials);

            ApprovalService approvalService = new ApprovalService(wsHttpAuthenticatedContextBinding, credentials, this.impersonationIdentity, this.MapApprovalUri);

            metadataClient.Open();
            resourceClient.Open();
            resourceFactoryClient.Open();
            searchClient.Open();

            this.jsonRpcServer.AddLocalRpcTarget(metadataClient, JsonOptionsFactory.GetTargetOptions(JsonOptionsFactory.MetadataService));
            this.jsonRpcServer.AddLocalRpcTarget(resourceClient, JsonOptionsFactory.GetTargetOptions(JsonOptionsFactory.ResourceService));
            this.jsonRpcServer.AddLocalRpcTarget(resourceFactoryClient, JsonOptionsFactory.GetTargetOptions(JsonOptionsFactory.ResourceFactoryService));
            this.jsonRpcServer.AddLocalRpcTarget(searchClient, JsonOptionsFactory.GetTargetOptions(JsonOptionsFactory.SearchService));
            this.jsonRpcServer.AddLocalRpcTarget(approvalService, JsonOptionsFactory.GetTargetOptions(JsonOptionsFactory.ApprovalService));

            this.initialized = true;

            return Task.CompletedTask;
        }
    }
}
