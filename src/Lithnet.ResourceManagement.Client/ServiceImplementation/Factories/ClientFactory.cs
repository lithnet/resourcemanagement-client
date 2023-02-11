using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.Hosts;
using Nito.AsyncEx;

namespace Lithnet.ResourceManagement.Client
{
    internal static class ClientFactory
    {
        private static ConcurrentDictionary<string, IClient> existingFactories = new ConcurrentDictionary<string, IClient>();

        public static IClient GetOrCreateFactory(ResourceManagementClientOptions p)
        {
            var clientId = GetClientId(p);
            Trace.WriteLine($"Getting client with id {clientId}");
            var factory = existingFactories.GetOrAdd(clientId, (id) => CreateClient(id, p));

            if (!factory.IsFaulted)
            {
                return factory;
            }
            else
            {
                factory.Dispose();
            }

            Trace.WriteLine($"Client {clientId} is in a faulted state and will be rebuilt");

            return existingFactories.AddOrUpdate(clientId,
                (id) => CreateClient(id, p),
                (id, _) => CreateClient(id, p));
        }

        private static IClient CreateClient(string id, ResourceManagementClientOptions p)
        {
            return AsyncContext.Run(async () => await CreateClientAsync(id, p).ConfigureAwait(false));
        }

        private static async Task<IClient> CreateClientAsync(string id, ResourceManagementClientOptions p)
        {
            IClient factory;
            Trace.WriteLine($"New client required for {id}");

#if NETFRAMEWORK
            Trace.WriteLine("Initializing native .NET framework factory (netfx)");
            factory = new NativeClientFactory();
#else
            factory = GetClient(p);
#endif
            await factory.InitializeClientsAsync().ConfigureAwait(false);
            return factory;
        }

        private static IClient GetClient(ResourceManagementClientOptions p)
        {
            var connectionModes = DetectConnectionModes(p.ConnectionMode).ToList();

            if (connectionModes.Contains(ConnectionMode.Direct))
            {
                Trace.WriteLine("Using direct connection mode");
                return new NativeClient(p);
            }

            if (connectionModes.Contains(ConnectionMode.LocalProxy))
            {
                Trace.WriteLine("Using local proxy");
                return new PipeRpcClient(p);
            }

            Trace.WriteLine("Using remote proxy");
            return new NegotitateStreamRpcClient(p);
        }

        private static IEnumerable<ConnectionMode> DetectConnectionModes(ConnectionMode connectionMode)
        {
            if (connectionMode != ConnectionMode.Auto &&
                !(connectionMode == ConnectionMode.Direct && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)))
            {
                Trace.WriteLine($"Using connection mode from configuration: {connectionMode}");
                yield return connectionMode;
                yield break;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (FrameworkUtilities.IsFramework)
                {
                    yield return ConnectionMode.Direct;
                }

                if (ExePipeHost.HasHostExe())
                {
                    yield return ConnectionMode.LocalProxy;
                }
            }

            yield return ConnectionMode.RemoteProxy;
        }

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
            sb.Append(p.RemoteHostSpn);
            sb.Append(p.RemoteProxyHost);
            sb.Append(p.RemoteProxyPort);
            sb.Append(p.ConnectionMode);

            byte[] rawBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var hasher = SHA256.Create();
            return Convert.ToBase64String(hasher.ComputeHash(rawBytes));
        }
    }
}
