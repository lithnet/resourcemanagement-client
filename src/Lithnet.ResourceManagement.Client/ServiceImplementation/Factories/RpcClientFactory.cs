using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
        private readonly ResourceManagementClientOptions parameters;

        public IResourceClient ResourceClient { get; private set; }

        public IResourceFactoryClient ResourceFactoryClient { get; private set; }

        public ISearchClient SearchClient { get; private set; }

        public ISchemaClient SchemaClient { get; private set; }

        public IApprovalClient ApprovalClient { get; private set; }

        public bool IsFaulted => !this.clientPipe?.IsConnected ?? false;

        public RpcClientFactory(ResourceManagementClientOptions p)
        {
            this.parameters = p;
        }

        public async Task InitializeClientsAsync()
        {
            var jsonClient = await this.GetJsonRpcClientAsync();

            jsonClient.TraceSource.Switch.Level = SourceLevels.Verbose;
            jsonClient.Disconnected += this.JsonClient_Disconnected;
            jsonClient.ExceptionStrategy = ExceptionProcessing.ISerializable;
            jsonClient.AllowModificationWhileListening = true;
            jsonClient.StartListening();

            var server = jsonClient.Attach<IRpcServer>(new JsonRpcProxyOptions { MethodNameTransform = x => $"Control_{x}" });

            await server.InitializeClientsAsync(this.parameters.BaseUri, this.parameters.Spn, this.parameters.ConcurrentConnectionLimit, this.parameters.SendTimeoutSeconds, this.parameters.RecieveTimeoutSeconds, this.parameters.Username, this.parameters.Password).ConfigureAwait(false);

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

        private async Task<JsonRpc> GetJsonRpcClientAsync()
        {
            var stream = await this.GetStreamAsync();
            return new JsonRpc(RpcCore.GetMessageHandler(stream));
        }

        private async Task<Stream> GetStreamAsync()
        {
            ConnectionMode selectedConnectionMode = this.parameters.ConnectionMode;

            Trace.WriteLine($"Configured connection mode: {selectedConnectionMode}");

            if (selectedConnectionMode == ConnectionMode.Auto)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (FrameworkUtilities.IsFramework)
                    {
                        selectedConnectionMode = ConnectionMode.Direct;
                    }
                    else
                    {
                        selectedConnectionMode = ConnectionMode.LocalProxy;
                    }
                }
                else
                {
                    selectedConnectionMode = ConnectionMode.RemoteProxy;
                }
            }

            Trace.WriteLine($"Selected connection mode: {selectedConnectionMode}");

            switch (selectedConnectionMode)
            {
                case ConnectionMode.Direct:
                case ConnectionMode.LocalProxy:
                    return await this.GetOrCreateClientPipeAsync(TimeSpan.FromSeconds(this.parameters.ConnectTimeoutSeconds), CancellationToken.None).ConfigureAwait(false);

                case ConnectionMode.RemoteProxy:
                    return await this.CreateNegotiateStreamAsync();

                default:
                    throw new Exception("Unknown connection mode");
            }
        }

        private async Task<NegotiateStream> CreateNegotiateStreamAsync()
        {
            var uri = new Uri(this.parameters.BaseUri, UriKind.RelativeOrAbsolute);
            var host = this.parameters.RemoteProxyHost ?? uri.Host;
            var port = this.parameters.RemoteProxyPort;

            TcpClient client = new TcpClient();
            await client.ConnectAsync(host, port);
            var stream = client.GetStream();
            var spn = this.parameters.RemoteHostSpn ?? $"LithnetRMC/{host}";

            NetworkCredential credentials;

            if (string.IsNullOrWhiteSpace(this.parameters.Username))
            {
                credentials = (NetworkCredential)CredentialCache.DefaultCredentials;
            }
            else
            {
                credentials = new NetworkCredential(this.parameters.Username, this.parameters.Password);
            }

            Trace.WriteLine($"Attempting to connect to remote proxy {host}:{port}:{spn}");

            return await RpcCore.GetClientNegotiateStreamAsync(stream, credentials, spn);
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
