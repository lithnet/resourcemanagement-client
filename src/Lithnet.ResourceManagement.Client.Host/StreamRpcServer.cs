using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.Host
{
    public class StreamRpcServer : RpcServer
    {
        private StreamRpcServer()
        {
        }

        public static async Task StartNegotiateStreamServerAsync(IPAddress address, int port, CancellationToken cancellationToken)
        {
            var endPoint = new IPEndPoint(address, port);
            var listener = new TcpListener(endPoint);
            listener.Server.DualMode = true;

            listener.Start();
            Logger.LogTrace("Started listener");
            cancellationToken.Register(listener.Stop);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Logger.LogTrace("Waiting for client");

                    var client = await listener.AcceptTcpClientAsync();
                    var server = new StreamRpcServer();
                    _ = Task.Run(async () => await server.HandleClientConnectionAsync(client), cancellationToken);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "An error occured setting up the client connection");
                }
            }
        }

        private async Task HandleClientConnectionAsync(TcpClient client)
        {
            try
            {
                Logger.LogTrace($"Client connected: {client.Client.RemoteEndPoint}");

                using var stream = client.GetStream();


                var authStream = await RpcCore.GetServerNegotiateStreamAsync(stream);
                this.impersonationIdentity = authStream.RemoteIdentity as WindowsIdentity;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("A new client was connected");
                sb.AppendLine($"Client IP: {client.Client.RemoteEndPoint}");
                sb.AppendLine($"Authenticated: {authStream.IsAuthenticated}");
                sb.AppendLine($"RemoteIdentity: {authStream.RemoteIdentity.Name}");
                sb.AppendLine($"AuthType: {authStream.RemoteIdentity.AuthenticationType}");
                sb.AppendLine($"IsEncrypted: {authStream.IsEncrypted}");
                sb.AppendLine($"IsMutuallyAuthenticated: {authStream.IsMutuallyAuthenticated}");
                sb.AppendLine($"IsSigned: {authStream.IsSigned}");
                sb.AppendLine($"Stream ImpersonationLevel: {authStream.ImpersonationLevel}");
                sb.AppendLine($"Identity ImpersonationLevel: {this.impersonationIdentity.ImpersonationLevel}");

                Logger.LogInfo(sb.ToString());

                if (this.impersonationIdentity != null && this.impersonationIdentity.ImpersonationLevel == TokenImpersonationLevel.Impersonation)
                {
                    using (this.impersonationIdentity.Impersonate())
                    {
                        await this.SetupRpcServerAsync(RpcCore.GetMessageHandler(authStream));
                    }
                }
                else
                {
                    await this.SetupRpcServerAsync(RpcCore.GetMessageHandler(authStream));
                }

                //}
            }
            catch (OperationCanceledException) { }
            catch (IOException e) when (e.InnerException is SocketException s)
            {
                Logger.LogError($"Socket was closed: {s.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while setting up a client connection");
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
