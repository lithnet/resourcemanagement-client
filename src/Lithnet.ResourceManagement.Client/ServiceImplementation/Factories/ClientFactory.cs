using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Lithnet.ResourceManagement.Client
{
    internal static class ClientFactory
    {
        private static ConcurrentDictionary<string, IClientFactory> existingFactories = new ConcurrentDictionary<string, IClientFactory>();

        public static IClientFactory GetOrCreateFactory(ResourceManagementClientOptions p)
        {
            var factoryId = GetFactoryId(p);
            Trace.WriteLine($"Getting client with id {factoryId}");
            var factory = existingFactories.GetOrAdd(factoryId, (id) => CreateClientFactory(id, p));

            if (!factory.IsFaulted)
            {
                return factory;
            }

            Trace.WriteLine($"Factory {factoryId} is in a faulted state and will be rebuilt");

            return existingFactories.AddOrUpdate(factoryId,
                (id) => CreateClientFactory(id, p),
                (id, _) => CreateClientFactory(id, p));
        }

        private static IClientFactory CreateClientFactory(string id, ResourceManagementClientOptions p)
        {
            return AsyncContext.Run(async () => await CreateClientFactoryAsync(id, p).ConfigureAwait(false));
        }

        private static async Task<IClientFactory> CreateClientFactoryAsync(string id, ResourceManagementClientOptions p)
        {
            IClientFactory factory;
            Trace.WriteLine($"New client required for {id}");

#if NETFRAMEWORK
            Trace.WriteLine("Initializing native .NET framework factory (netfx)");
            factory = new NativeClientFactory();
#else
            if (FrameworkUtilities.IsFramework)
            {
                Trace.WriteLine("Initializing native .NET framework factory (netstandard2.0)");
                factory = new NativeClientFactory(p);
            }
            else
            {
                Trace.WriteLine("Initializing RPC factory");
                factory = new RpcClientFactory(p);
            }
#endif
            await factory.InitializeClientsAsync().ConfigureAwait(false);
            return factory;
        }

        private static string GetFactoryId(ResourceManagementClientOptions p)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(p.BaseUri);
            sb.Append(p.Spn);
            sb.Append(p.ConcurrentConnectionLimit.ToString());
            sb.Append(p.ConnectTimeoutSeconds.ToString());
            sb.Append(p.Username);
            sb.Append(p.Password);
            sb.Append(p.RecieveTimeoutSeconds.ToString());
            sb.Append(p.RecieveTimeoutSeconds.ToString());

            byte[] rawBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var hasher = SHA256.Create();
            return Convert.ToBase64String(hasher.ComputeHash(rawBytes));
        }
    }
}
