namespace Lithnet.ResourceManagement.Client
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
    }
}
