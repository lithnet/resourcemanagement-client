using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using StreamJsonRpc;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal class RpcServer : IRpcServer
    {
        private readonly string pipeName;
        private JsonRpc jsonRpcServer;
        private bool initialized;

        public RpcServer(string pipeName)
        {
            this.pipeName = string.Format(RpcCore.PipeNameFormatTemplate, pipeName);
        }

        internal async Task StartNamedPipeServerAsync(CancellationToken ct)
        {
            try
            {
                if (Directory.GetFiles(@"\\.\pipe\").Contains(this.pipeName))
                {
                    var ex = new SecurityException("The RPC server cannot be started because the named pipe name is already in use. If there is another instance of the service running, close it before starting this instance. If there is not another instance running, this could indicate a security issue and should be investigated");
                    throw ex;
                }

                var pipeSecurity = new PipeSecurity();
                pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkSid, null), PipeAccessRights.FullControl, AccessControlType.Deny));
                pipeSecurity.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.FullControl, AccessControlType.Allow));

                NamedPipeServerStream serverPipe = new NamedPipeServerStream(this.pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.WriteThrough, 0, 0, pipeSecurity);

                await serverPipe.WaitForConnectionAsync(ct).ConfigureAwait(false);
                ct.ThrowIfCancellationRequested();

                await this.RespondToRpcRequestsAsync(serverPipe).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unhandled exception in pipe startup");
            }
        }

        private async Task RespondToRpcRequestsAsync(NamedPipeServerStream serverPipe)
        {
            using (serverPipe)
            {
                if (serverPipe.ReadByte() != 255)
                {
                    throw new InvalidDataException("The client sent incorrect initialization data");
                }

                Logger.LogTrace("Client has connected to pipe");

                this.jsonRpcServer = new JsonRpc(RpcCore.GetMessageHandler(serverPipe));
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
        }

        private void InitializeClient<T>(ClientBase<T> client, NetworkCredential credentials) where T : class
        {
            if (credentials != null)
            {
                client.ClientCredentials.Windows.ClientCredential = credentials;
            }

            client.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;
        }

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

            var spnIdentity = string.IsNullOrEmpty(spn) ? null : EndpointIdentity.CreateSpnIdentity(spn);

            Uri uri = baseUri == null ? new Uri("http://localhost:5725") : new Uri(baseUri);

            var endpoints = new EndpointManager(uri, spnIdentity);
            sendTimeout = Math.Max(10, sendTimeout);
            recieveTimeout = Math.Max(10, recieveTimeout);

            var wsHttpAuthenticatedBinding = BindingManager.GetWsAuthenticatedBinding(recieveTimeout, sendTimeout);
            var wsHttpBinding = BindingManager.GetWsHttpBinding(recieveTimeout, sendTimeout);
            var wsHttpAuthenticatedContextBinding = BindingManager.GetWsHttpContextBinding(recieveTimeout, sendTimeout);

            ResourceClient resourceClient = new ResourceClient(wsHttpAuthenticatedBinding, endpoints.ResourceEndpoint);
            this.InitializeClient(resourceClient, credentials);

            ResourceFactoryClient resourceFactoryClient = new ResourceFactoryClient(wsHttpAuthenticatedBinding, endpoints.ResourceFactoryEndpoint);
            this.InitializeClient(resourceFactoryClient, credentials);

            MetadataExchangeClient metadataClient = new MetadataExchangeClient(wsHttpBinding, endpoints.MetadataEndpoint);
            this.InitializeClient(metadataClient, credentials);

            SearchClient searchClient = new SearchClient(wsHttpAuthenticatedBinding, endpoints.SearchEndpoint);
            this.InitializeClient(searchClient, credentials);

            ApprovalService approvalService = new ApprovalService(wsHttpAuthenticatedContextBinding, credentials);

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
