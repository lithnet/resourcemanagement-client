using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.Host
{
    public class PipeRpcServer : RpcServer
    {
        private readonly string pipeName;

        public PipeRpcServer(string pipeName)
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

                Logger.LogTrace("Client has connected to the Pipe");

                await this.SetupRpcServerAsync(RpcCore.GetMessageHandler(serverPipe));
            }
        }
    }
}
