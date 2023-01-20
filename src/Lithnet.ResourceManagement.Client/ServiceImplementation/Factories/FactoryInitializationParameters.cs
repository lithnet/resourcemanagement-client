using System;
using System.Net;

namespace Lithnet.ResourceManagement.Client
{
    internal class FactoryInitializationParameters
    {
        public string BaseUri { get; set; }

        public string Spn { get; set; }

        public int ConcurrentConnections { get; set; } = 10;

        public TimeSpan ConnectTimeout { get; set; } = new TimeSpan(0, 0, 30);

        public NetworkCredential Credentials { get; set; }

        public int RecieveTimeout { get; set; } = 30;

        public int SendTimeout { get; set; } = 30;
    }
}
