﻿namespace Lithnet.ResourceManagement.Client
{
    public class ResourceManagementClientOptions
    {
        /// <summary>
        /// The URI or hostname of the MIM Service.
        /// </summary>
        public string BaseUri { get; set; } = "http://localhost:5725";

        /// <summary>
        /// The name of the user to connect to the MIM service as. Leave blank to connect as the currently logged on user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The password of the user to connect to the MIM service as. Ignored if the username is null.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Optional. The service principal name of the MIM Service. Defaults to 'FIMService/{hostname}`
        /// </summary>
        public string Spn { get; set; }

        /// <summary>
        /// The maximum number of connections that can be made to the MIM service
        /// </summary>
        public int ConcurrentConnectionLimit { get; set; } = 10000;

        /// <summary>
        /// The maximum number of seconds to allow for a connection attempt to succeed
        /// </summary>
        public int ConnectTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// The maximum amount of seconds to wait for incoming data
        /// </summary>
        public int RecieveTimeoutSeconds { get; set; } = 60 * 20;

        /// <summary>
        /// The maximum amount of time to wait for outgoing data
        /// </summary>
        public int SendTimeoutSeconds { get; set; } = 60 * 20;

        /// <summary>
        /// Gets or sets the type of connection to make to the MIM service
        /// </summary>
        public ConnectionMode ConnectionMode { get; set; }

        /// <summary>
        /// Gets or sets the hostname of the RMC proxy service, used for connection scenarios involving dotnet core hosts. If not specified, this defaults to the name of the MIM service
        /// </summary>
        public string RemoteProxyHost { get; set; }

        /// <summary>
        /// Gets or sets the port that the RMC proxy server is listening on. Defaults to 5735.
        /// </summary>
        public int RemoteProxyPort { get; set; } = 5735;

        /// <summary>
        /// Gets or sets the SPN used by the remote proxy service
        /// </summary>
        public string RemoteHostSpn { get; set; }
        
        public static ResourceManagementClientOptions Clone(ResourceManagementClientOptions original)
        {
            return new ResourceManagementClientOptions
            {
                BaseUri = original.BaseUri,
                ConcurrentConnectionLimit = original.ConcurrentConnectionLimit,
                ConnectionMode = original.ConnectionMode,
                ConnectTimeoutSeconds = original.ConnectTimeoutSeconds,
                Password = original.Password,
                RecieveTimeoutSeconds = original.RecieveTimeoutSeconds,
                RemoteHostSpn = original.RemoteHostSpn,
                RemoteProxyHost = original.RemoteProxyHost,
                RemoteProxyPort = original.RemoteProxyPort,
                SendTimeoutSeconds = original.SendTimeoutSeconds,
                Spn = original.Spn,
                Username = original.Username
            };
        }
    }
}