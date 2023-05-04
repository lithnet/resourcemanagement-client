using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client;

namespace Lithnet.ResourceManagement.Proxy
{
    public class PipeRpcServer : RpcServer
    {
        private readonly string pipeName;

        protected override bool RequiresImpersonationOrExplicitCredentials => false;

        public PipeRpcServer(string pipeName)
        {
            this.pipeName = string.Format(RpcCore.PipeNameFormatTemplate, pipeName);
        }

        private protected override Uri MapBaseUri(string uri)
        {
            UriBuilder builder = new UriBuilder(uri);
            if (builder.Scheme != "http")
            {
                builder.Scheme = "http";
            }

            if (builder.Port <= 0 || builder.Port == 80)
            {
                builder.Port = 5725;
            }

            return builder.Uri;
        }

        private protected override Uri MapApprovalUri(string baseUri)
        {
            return new Uri(baseUri);
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

                this.impersonationIdentity = WindowsIdentity.GetCurrent();
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
                if (serverPipe.ReadByte() != RpcCore.ClientInitialization)
                {
                    throw new InvalidDataException("The client sent incorrect initialization data");
                }

                serverPipe.WriteByte(RpcCore.ServerAck);

                Logger.LogTrace("Client has connected to the Pipe");

                // using GZipStream sendingStream = new GZipStream(serverPipe, CompressionMode.Compress);
                //  using GZipStream receivingStream = new GZipStream(serverPipe, CompressionMode.Decompress);

                await this.SetupRpcServerAsync(RpcCore.GetMessageHandler(serverPipe, serverPipe));
            }
        }
    }
}
