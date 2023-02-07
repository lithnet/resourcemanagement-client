using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
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
            Trace.WriteLine("Started listener");
            cancellationToken.Register(listener.Stop);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Trace.WriteLine("Waiting for client");

                    var client = await listener.AcceptTcpClientAsync();

                    Trace.WriteLine("Got client");

                    var server = new StreamRpcServer();
                    _ = Task.Run(async () => await server.HandleClientConnectionAsync(client), cancellationToken);
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
        }

        private async Task HandleClientConnectionAsync(TcpClient client)
        {
            try
            {
                Trace.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

                using var stream = client.GetStream();

                //if (stream.ReadByte() != 255)
                //{
                //    throw new InvalidDataException("The client sent incorrect initialization data");
                //}

                var authStream = await RpcCore.GetServerNegotiateStreamAsync(stream);

                Trace.WriteLine($"Authenticated: {authStream.IsAuthenticated}");
                Trace.WriteLine($"RemoteIdentity: {authStream.RemoteIdentity.Name}");
                Trace.WriteLine($"AuthType: {authStream.RemoteIdentity.AuthenticationType}");
                Trace.WriteLine($"IsEncrypted: {authStream.IsEncrypted}");
                Trace.WriteLine($"IsMutuallyAuthenticated: {authStream.IsMutuallyAuthenticated}");
                Trace.WriteLine($"IsSigned: {authStream.IsSigned}");
                Trace.WriteLine($"ImpersonationLevel: {authStream.ImpersonationLevel}");

                this.impersonationIdentity = authStream.RemoteIdentity as WindowsIdentity;
                await WindowsIdentity.RunImpersonated(this.impersonationIdentity.AccessToken, async () =>
                {
                    Trace.WriteLine($"Current user is now {System.Security.Principal.WindowsIdentity.GetCurrent().Name} on thread {Thread.CurrentThread.ManagedThreadId}");
                    await this.SetupRpcServerAsync(RpcCore.GetMessageHandler(authStream));

                });
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
