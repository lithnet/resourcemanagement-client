using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    internal static class ClientFactory
    {
        private static ConcurrentDictionary<string, IClient> existingFactories = new ConcurrentDictionary<string, IClient>();

        public static IClient GetOrCreateClient(ResourceManagementClientOptions p)
        {
            var clientId = GetClientId(p);
            Trace.WriteLine($"Getting client with id {clientId}");
            var client = existingFactories.GetOrAdd(clientId, (id) => CreateClient(id, p));

            if (!client.IsFaulted)
            {
                Trace.WriteLine($"Returning client {clientId} - {client.DisplayName}");
                return client;
            }
            else
            {
                client.Dispose();
            }

            Trace.WriteLine($"Client {clientId} is in a faulted state and will be rebuilt");

            return existingFactories.AddOrUpdate(clientId,
                (id) => CreateClient(id, p),
                (id, _) => CreateClient(id, p));
        }

        private static IClient CreateClient(string id, ResourceManagementClientOptions p)
        {
            return AsyncHelper.Run(async () => await CreateClientAsync(id, p).ConfigureAwait(false));
        }

        private static async Task<IClient> CreateClientAsync(string id, ResourceManagementClientOptions p)
        {
            IClient client;

#if NETFRAMEWORK
            Trace.WriteLine("Initializing native .NET framework factory (netfx)");
            client = new WsHttpClient(p);
#else
            client = GetClient(p);
#endif
            await client.InitializeClientsAsync().ConfigureAwait(false);

            Trace.WriteLine($"Created new client of type {client.GetType().Name} with ID {id}");
            return client;
        }

#if !NETFRAMEWORK
        private static IClient GetClient(ResourceManagementClientOptions p)
        {
            var connectionModes = DetectConnectionModes(p).ToList();

            if (connectionModes.Contains(ConnectionMode.DirectWsHttp))
            {
                Trace.WriteLine("Using direct wshttp mode");
                return new WsHttpClient(p);
            }

            if (connectionModes.Contains(ConnectionMode.DirectNetTcp))
            {
                Trace.WriteLine("Using direct nettcp mode");
                return new NetTcpClient(p);
            }

            if (connectionModes.Contains(ConnectionMode.LocalProxy))
            {
                Trace.WriteLine("Using local proxy");
                return new PipeRpcClient(p);
            }

            Trace.WriteLine("Using remote proxy");
            return new NegotiateStreamRpcClient(p);
        }

        private static IEnumerable<ConnectionMode> DetectConnectionModes(ResourceManagementClientOptions p)
        {
            if (p.ConnectionMode != ConnectionMode.Auto &&
                !(p.ConnectionMode == ConnectionMode.DirectWsHttp && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)))
            {
                Trace.WriteLine($"Using connection mode from configuration: {p.ConnectionMode}");
                yield return p.ConnectionMode;
                yield break;
            }

            if (p.BaseUri != null)
            {
                if (p.BaseUri.StartsWith("net.tcp://"))
                {
                    yield return ConnectionMode.DirectNetTcp;
                    yield break;
                }

                if (p.BaseUri.StartsWith("rmc://"))
                {
                    yield return ConnectionMode.RemoteProxy;
                    yield break;
                }

                if (p.BaseUri.StartsWith("pipe://"))
                {
                    yield return ConnectionMode.LocalProxy;
                    yield break;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (FrameworkUtilities.IsFramework)
                {
                    yield return ConnectionMode.DirectWsHttp;
                }

                if (ExePipeHost.HasHostExe())
                {
                    yield return ConnectionMode.LocalProxy;
                }
            }

            yield return ConnectionMode.RemoteProxy;
        }

#endif

        private static string GetClientId(ResourceManagementClientOptions p)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(p.BaseUri);
            sb.Append(p.Spn);
            sb.Append(p.ConcurrentConnectionLimit.ToString());
            sb.Append(p.ConnectTimeoutSeconds.ToString());
            sb.Append(p.Username);
            sb.Append(p.Password);
            sb.Append(p.RecieveTimeoutSeconds.ToString());
            sb.Append(p.SendTimeoutSeconds.ToString());
            sb.Append(p.ConnectionMode);

            byte[] rawBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var hasher = SHA256.Create();
            return Convert.ToBase64String(hasher.ComputeHash(rawBytes));
        }
    }
}
