namespace Lithnet.ResourceManagement.Client
{
    public class ResourceManagementClientOptions
    {
        public const int DefaultFimServicePort = 5725;
        public const int DefaultRemoteProxyPort = 5735;
        public const int DefaultNetTcpBindingPort = 5736;

        /// <summary>
        /// The URI or hostname of the MIM Service.
        /// </summary>
        public string BaseUri { get; set; } = $"http://localhost:{DefaultFimServicePort}";

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
        /// Gets or sets the location of the Resource Management Client Host executable
        /// </summary>
        /// <remarks>
        /// The host executable is a .NET Framework application that is used to connect to the MIM service when the client is running on .NET Core on Windows. It not needed when the client is running on .NET Framework, or when the client is configured to a <see cref="ConnectionMode.DirectNetTcp"/>, or <see cref="ConnectionMode.RemoteProxy"/> connection mode.
        /// </remarks>
        public string RmcHostExe { get; set; }

        /// <summary>
        /// Creates a new instance, and copies the values from the original instance
        /// </summary>
        /// <param name="original">The options instance to copy the values from</param>
        /// <returns>A copy of the original instance</returns>
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
                SendTimeoutSeconds = original.SendTimeoutSeconds,
                Spn = original.Spn,
                Username = original.Username
            };
        }
    }
}
