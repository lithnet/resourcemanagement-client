using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    internal class NegotiateStreamRpcClient : RpcClient
    {
        private TcpClient client;
        private string displayName;

        public NegotiateStreamRpcClient(ResourceManagementClientOptions p) : base(p)
        {
        }

        public override bool IsFaulted => !this.client?.Connected ?? false;

        protected override string MapUri(string baseUri)
        {
            return null;
        }

        public override string DisplayName => this.displayName;

        protected override async Task<Stream> GetStreamAsync()
        {
            var proxyUri = this.parameters.GetProxyUri();
            var host = proxyUri.Host;
            var port = proxyUri.Port;

            this.displayName = $"RPC connection to {proxyUri}";

            this.client = new TcpClient();

            try
            {
                await this.client.ConnectAsync(host, port);
            }
            catch (Exception ex)
            {
                throw new RmcProxyConnectionException($"Could not connect to the RMC proxy on {host}:{port}. Ensure the Lithnet Resource Management Proxy service has been installed and is running on the host. See https://go.lithnet.io/fwlink/XXXXXX for more information", ex);
            }

            var stream = this.client.GetStream();

            var impersonationLevel = TokenImpersonationLevel.Impersonation;

            stream.WriteByte(RpcCore.ClientInitialization);
            var response = stream.ReadByte();

            if (response != RpcCore.ServerAck)
            {
                throw new InvalidDataException("The server did not respond with the correct acknowledgement of the initialization request");
            }

            var spn = this.parameters.Spn ?? $"FIMService/{host}";

            NetworkCredential credentials;

            if (string.IsNullOrWhiteSpace(this.parameters.Username))
            {
                credentials = (NetworkCredential)CredentialCache.DefaultCredentials;
            }
            else
            {
                credentials = new NetworkCredential(this.parameters.Username, this.parameters.Password);
            }

            ValidateNegotiateStreamSupport();

            Trace.WriteLine($"Attempting to connect to remote proxy {host}:{port}:{spn}");

            var protectedStream = await RpcCore.GetClientNegotiateStreamAsync(stream, credentials, spn, impersonationLevel);

            protectedStream.WriteByte(RpcCore.ClientPostAuthInitialization);

            response = protectedStream.ReadByte();

            if (response != RpcCore.ServerAck)
            {
                if (response == RpcCore.AccessDenied)
                {
                    throw new UnauthorizedAccessException($"Access to the RMC proxy on {host}:{port} was denied.");
                }
                else
                {
                    throw new InvalidDataException("The server did not respond with the correct acknowledgement of the initialization request");
                }
            }

            return protectedStream;
        }

        private static void ValidateNegotiateStreamSupport()
        {
#if !NETFRAMEWORK
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || FrameworkUtilities.IsFramework)
            {
                return;
            }

            if (System.Environment.Version < new Version(8, 0))
            {
                throw new PlatformNotSupportedException($"Using '{nameof(ConnectionMode.RemoteProxy)}' requires .NET 8 or later");
            }
#endif
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.client?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}