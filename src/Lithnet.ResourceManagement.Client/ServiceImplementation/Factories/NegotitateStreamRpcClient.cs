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
    internal class NegotitateStreamRpcClient : RpcClient
    {
        private TcpClient client;

        public NegotitateStreamRpcClient(ResourceManagementClientOptions p) : base(p)
        {
        }

        public override bool IsFaulted => !this.client?.Connected ?? false;

        protected override async Task<Stream> GetStreamAsync()
        {
            var host = this.parameters.GetProxyHostName();
            var port = this.parameters.GetProxyPort();

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

            var impersonationLevel = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                TokenImpersonationLevel.Impersonation : TokenImpersonationLevel.Identification;

            stream.WriteByte(RpcCore.ClientInitialization);
            var response = stream.ReadByte();

            if (response != RpcCore.ServerAck)
            {
                throw new InvalidDataException("The server did not respond with the correct acknowledgement of the impersonation request");
            }

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

            return await RpcCore.GetClientNegotiateStreamAsync(stream, credentials, spn, impersonationLevel);
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
