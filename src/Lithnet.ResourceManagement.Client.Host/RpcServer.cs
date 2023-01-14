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
            int clientId = 0;
            try
            {
                if (Directory.GetFiles(@"\\.\pipe\").Contains(pipeName))
                {
                    var ex = new SecurityException("The RPC server cannot be started because the named pipe name is already in use. If there is another instance of the service running, close it before starting this instance. If there is not another instance running, this could indicate a security issue and should be investigated");
                    throw ex;
                }

                var pipeSecurity = new PipeSecurity();
                pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkSid, null), PipeAccessRights.FullControl, AccessControlType.Deny));
                pipeSecurity.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.FullControl, AccessControlType.Allow));

                //while (!ct.IsCancellationRequested)
                // {
                NamedPipeServerStream serverPipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.WriteThrough, 0, 0, pipeSecurity);

                await serverPipe.WaitForConnectionAsync(ct).ConfigureAwait(false);
                ct.ThrowIfCancellationRequested();

                await RespondToRpcRequestsAsync(serverPipe, ++clientId).ConfigureAwait(false);
                // }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("RMC", $"Unhandled exception named pipe start \r\n{ex}", EventLogEntryType.Error, 99);
                Trace.WriteLine(ex.ToString());
            }
        }

        private async Task RespondToRpcRequestsAsync(NamedPipeServerStream serverPipe, int clientId)
        {
            using (serverPipe)
            {
                if (serverPipe.ReadByte() != 255)
                {
                    throw new InvalidDataException("The client sent incorrect initialization data");
                }

                Trace.WriteLine("Client has connected to pipe");

                jsonRpcServer = new JsonRpc(RpcCore.GetMessageHandler(serverPipe));
                jsonRpcServer.TraceSource.Switch.Level = SourceLevels.Warning;
                jsonRpcServer.ExceptionStrategy = ExceptionProcessing.ISerializable;
                jsonRpcServer.AllowModificationWhileListening = true;
                jsonRpcServer.AddLocalRpcTarget(this, new JsonRpcTargetOptions() { MethodNameTransform = (x) => $"Control_{x}" });

                jsonRpcServer.Disconnected += (sender, u) => { Trace.WriteLine("Client disconnected"); };
                jsonRpcServer.StartListening();

                Trace.WriteLine("RPC server has connected client and waiting for messages");

                //this.logger.LogTrace($"Rpc listener attached to instance {clientId}. Waiting for requests...");
                await jsonRpcServer.Completion.ConfigureAwait(false);

                Trace.WriteLine("RPC server has terminated");

                // this.logger.LogTrace($"Pipe server instance {clientId} terminated.");
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
            if (initialized)
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

            var wsContextBinding = BindingManager.GetWsHttpContextBinding(recieveTimeout, sendTimeout);
            var wsHttpBinding = BindingManager.GetWsHttpBinding(recieveTimeout, sendTimeout);

            ResourceClient resourceClient = new ResourceClient(wsContextBinding, endpoints.ResourceEndpoint);
            InitializeClient(resourceClient, credentials);

            ResourceFactoryClient resourceFactoryClient = new ResourceFactoryClient(wsContextBinding, endpoints.ResourceFactoryEndpoint);
            InitializeClient(resourceFactoryClient, credentials);

            MetadataExchangeClient metadataClient = new MetadataExchangeClient(wsHttpBinding, endpoints.MetadataEndpoint);
            InitializeClient(metadataClient, credentials);

            SearchClient searchClient = new SearchClient(wsContextBinding, endpoints.SearchEndpoint);
            InitializeClient(searchClient, credentials);

            jsonRpcServer.AddLocalRpcTarget(metadataClient, JsonOptionsFactory.GetTargetOptions(JsonOptionsFactory.MetadataService));
            jsonRpcServer.AddLocalRpcTarget(resourceClient, JsonOptionsFactory.GetTargetOptions(JsonOptionsFactory.ResourceService));
            jsonRpcServer.AddLocalRpcTarget(resourceFactoryClient, JsonOptionsFactory.GetTargetOptions(JsonOptionsFactory.ResourceFactoryService));
            jsonRpcServer.AddLocalRpcTarget(searchClient, JsonOptionsFactory.GetTargetOptions(JsonOptionsFactory.SearchService));

            initialized = true;

            return Task.CompletedTask;
        }
    }
}
