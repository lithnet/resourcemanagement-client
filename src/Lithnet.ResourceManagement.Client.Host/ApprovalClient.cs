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

        public ApprovalService(Binding binding, NetworkCredential credentials, WindowsIdentity impersonationIdentity)
        {
            this.binding = binding;
            this.credentials = credentials;
            this.impersonationIdentity = impersonationIdentity;
        }

        public async Task<Message> ApproveAsync(string endpoint, Message message)
        {
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
