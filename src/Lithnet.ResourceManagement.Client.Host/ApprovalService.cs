using System;
using System.Net;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal class ApprovalService
    {
        private readonly Binding binding;
        private readonly NetworkCredential credentials;
        private readonly WindowsIdentity impersonationIdentity;
        private readonly bool translateEndpointsToLocalHost;

        public ApprovalService(Binding binding, NetworkCredential credentials, WindowsIdentity impersonationIdentity, bool translateEndpointsToLocalHost)
        {
            this.binding = binding;
            this.credentials = credentials;
            this.impersonationIdentity = impersonationIdentity;
            this.translateEndpointsToLocalHost = translateEndpointsToLocalHost;
        }

        public async Task<Message> ApproveAsync(string endpoint, Message message)
        {
            if (this.translateEndpointsToLocalHost)
            {
                UriBuilder builder = new UriBuilder(endpoint);
                builder.Host = "127.0.0.1";
                endpoint = builder.ToString();
            }

            ResourceFactoryClient client = new ResourceFactoryClient(this.binding, EndpointManager.EndpointFromAddress(endpoint));

            if (this.credentials != null)
            {
                client.ClientCredentials.Windows.ClientCredential = this.credentials;
            }

            client.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;

            if (this.impersonationIdentity != null)
            {
                using (this.impersonationIdentity.Impersonate())
                {
                    client.Open();
                }
            }

            return await client.CreateAsync(message);
        }
    }
}
