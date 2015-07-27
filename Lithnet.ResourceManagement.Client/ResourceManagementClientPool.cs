using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace Lithnet.ResourceManagement.Client
{
    internal static class ResourceManagementClientPool
    {
        private static ConcurrentBag<ResourceManagementClient> pool = new ConcurrentBag<ResourceManagementClient>();

        private static int totalItemsCreated = 0;

        private static int totalReuseRequests = 0;

        public static ResourceManagementClient GetClient()
        {
            if (pool.IsEmpty)
            {
                Interlocked.Increment(ref totalItemsCreated);
                return new ResourceManagementClient();
            }
            else
            {
                ResourceManagementClient client;
                if (pool.TryTake(out client))
                {
                    Interlocked.Increment(ref totalReuseRequests);
                    return client;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public static void DrainPool()
        {
            while (!pool.IsEmpty)
            {
                ResourceManagementClient client;

                if (pool.TryTake(out client))
                {
                    //client.IsPooled = false;
                    //client.Dispose();
                }
            }
        }

        public static void Return(ResourceManagementClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            pool.Add(client);
        }

        public static int CurrentPoolCount
        {
            get
            {
                return pool.Count;
            }
        }

        public static int PoolSize
        {
            get
            {
                return totalItemsCreated;
            }
        }

        public static int CacheHits
        {
            get
            {
                return totalReuseRequests;
            }
        }
    }
}
