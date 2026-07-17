using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
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
#if NETFRAMEWORK
            Trace.WriteLine("Initializing native .NET framework factory (netfx)");
            IClient client = new WsHttpClient(p);
            await client.InitializeClientsAsync().ConfigureAwait(false);

            Trace.WriteLine($"Created new client of type {client.GetType().Name} with ID {id}");
            return client;
#else
            // DetectConnectionModes yields an ordered candidate list. When the caller named an
            // explicit connection mode it yields exactly that one mode, so the original exception
            // surfaces directly with no fallback, exactly as it did before the fallback chain
            // existed. Fallback across modes exists only under Auto, and every attempt's exception
            // is retained so an early failure (for example an application control policy blocking
            // the local proxy host) is never masked by a later mode's connection error.
            List<Exception> originalFailures = new List<Exception>();
            List<Exception> describedFailures = new List<Exception>();

            foreach (ConnectionMode mode in DetectConnectionModes(p))
            {
                IClient client = null;

                try
                {
                    client = CreateClientForMode(mode, p);
                    await client.InitializeClientsAsync().ConfigureAwait(false);

                    Trace.WriteLine($"Created new client of type {client.GetType().Name} using {mode} with ID {id}");
                    return client;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Connection mode {mode} failed: {ex}");
                    originalFailures.Add(ex);
                    describedFailures.Add(new ResourceManagementException($"Connection mode {mode} failed: {ex.Message}", ex));

                    if (client != null)
                    {
                        client.Dispose();
                    }
                }
            }

            if (originalFailures.Count == 1)
            {
                ExceptionDispatchInfo.Capture(originalFailures[0]).Throw();
            }

            throw new AggregateException(
                "Unable to connect to the Resource Management Service using any available connection mode. See the inner exceptions for the failure from each mode that was attempted.",
                describedFailures);
#endif
        }

#if !NETFRAMEWORK
        private static IClient CreateClientForMode(ConnectionMode mode, ResourceManagementClientOptions p)
        {
            switch (mode)
            {
                case ConnectionMode.DirectWsHttp:
                    Trace.WriteLine("Attempting direct wshttp mode");
                    return new WsHttpClient(p);

                case ConnectionMode.DirectNetTcp:
                    Trace.WriteLine("Attempting direct nettcp mode");
                    return new NetTcpClient(p);

                case ConnectionMode.LocalProxy:
                    Trace.WriteLine("Attempting local proxy mode");
                    return new PipeRpcClient(p);

                case ConnectionMode.RemoteProxy:
                    Trace.WriteLine("Attempting remote proxy mode");
                    return new NegotiateStreamRpcClient(p);

                default:
                    throw new InvalidOperationException($"Unknown connection mode {mode}");
            }
        }

        internal static IEnumerable<ConnectionMode> DetectConnectionModes(ResourceManagementClientOptions p)
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

                // The embedded proxy host is always available on Windows (an installed host is
                // preferred, and the embedded copy is extracted otherwise), so LocalProxy is always
                // a candidate here.
                yield return ConnectionMode.LocalProxy;
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
