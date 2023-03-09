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
    public class NegotiateStreamRpcServer : RpcServer
    {
        protected override bool RequiresImpersonationOrExplicitCredentials => true;

        private NegotiateStreamRpcServer()
        {
        }

        private protected override Uri MapBaseUri(string uri)
        {
            var port = SettingsProvider.MimServicePort;
            return new Uri($"http://127.0.0.1:{port}");
        }

        private protected override Uri MapApprovalUri(string uri)
        {
            UriBuilder builder = new UriBuilder(uri);
            builder.Host = "127.0.0.1";
            return builder.Uri;
        }

        public static async Task StartNegotiateStreamServerAsync(IPAddress address, int port, CancellationToken cancellationToken)
        {
            var endPoint = new IPEndPoint(address, port);
            var listener = new TcpListener(endPoint);
            listener.Server.DualMode = true;

            listener.Start();
            Logger.LogTrace($"Started listener on port: {port}");
            cancellationToken.Register(listener.Stop);
            Logger.LogTrace($"Authorization group: {SettingsProvider.AuthorizedProxyUsersName}");
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Logger.LogTrace("Waiting for client");

                    var client = await listener.AcceptTcpClientAsync();
                    var server = new NegotiateStreamRpcServer();
                    _ = Task.Run(async () => await server.HandleClientConnectionAsync(client), cancellationToken);
                }
                catch (OperationCanceledException) { }
                catch (ObjectDisposedException) { }
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

                var firstByte = stream.ReadByte();

                if (firstByte == RpcCore.ClientInitialization)
                {
                    Logger.LogTrace("Client requested initialization");
                    stream.WriteByte(RpcCore.ServerAck);
                }
                else
                {
                    Logger.LogError("Unexpected data from client. Closing stream");
                    stream.WriteByte(RpcCore.ServerError);
                    client.Close();
                    return;
                }

                var authStream = await RpcCore.GetServerNegotiateStreamAsync(stream, TokenImpersonationLevel.Impersonation);
                this.impersonationIdentity = authStream.RemoteIdentity as WindowsIdentity;

                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"Client IP: {client.Client.RemoteEndPoint}");
                sb.AppendLine($"Authenticated: {authStream.IsAuthenticated}");
                sb.AppendLine($"RemoteIdentity: {authStream.RemoteIdentity.Name}");
                sb.AppendLine($"AuthType: {authStream.RemoteIdentity.AuthenticationType}");
                sb.AppendLine($"IsEncrypted: {authStream.IsEncrypted}");
                sb.AppendLine($"IsMutuallyAuthenticated: {authStream.IsMutuallyAuthenticated}");
                sb.AppendLine($"IsSigned: {authStream.IsSigned}");
                sb.AppendLine($"ImpersonationLevel: {authStream.ImpersonationLevel}");

                if (authStream.ReadByte() == RpcCore.ClientPostAuthInitialization)
                {
                    if (this.impersonationIdentity.Groups.Contains(SettingsProvider.AuthorizedProxyUsers))
                    {
                        sb.Insert(0, $"A new client was connected and authorized as a member of {SettingsProvider.AuthorizedProxyUsersName}\n");
                        Logger.LogInfo(sb.ToString());

                        authStream.WriteByte(RpcCore.ServerAck);
                    }
                    else
                    {
                        sb.Insert(0, $"A new client was connected but was not an authorized member of the {SettingsProvider.AuthorizedProxyUsersName} group so the connection was terminated\n");
                        Logger.LogWarning(sb.ToString());
                        authStream.WriteByte(RpcCore.AccessDenied);
                        authStream.Close();
                        throw new UnauthorizedAccessException($"The user {authStream.RemoteIdentity.Name} was not authorized to access this service");
                    }
                }
                else
                {
                    sb.Insert(0, "A new client was connected but sent unexpected data so the connection was closed");
                    Logger.LogError(sb.ToString());
                    authStream.WriteByte(RpcCore.ServerError);
                    client.Close();
                    return;
                }

                using (this.impersonationIdentity.Impersonate())
                {
                    await this.SetupRpcServerAsync(RpcCore.GetMessageHandler(authStream, authStream));
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (OperationCanceledException) { }
            catch (IOException e) when (e.InnerException is SocketException s)
            {
                Logger.LogTrace($"Socket was closed: {s.Message}");
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
