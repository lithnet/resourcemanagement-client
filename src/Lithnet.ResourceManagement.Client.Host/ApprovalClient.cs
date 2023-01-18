using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client.Host
{
    internal class ApprovalService
    {
        private readonly Binding binding;
        private readonly NetworkCredential credentials;

        public ApprovalService(Binding binding, NetworkCredential credentials)
        {
            this.binding = binding;
            this.credentials = credentials;
        }

        public async Task<Message> ApproveAsync(string endpoint, Message message)
        {
            ResourceFactoryClient client = new ResourceFactoryClient(this.binding, new EndpointAddress(endpoint));

            if (credentials != null)
            {
                client.ClientCredentials.Windows.ClientCredential = credentials;
            }

            client.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Delegation;

            return await client.CreateAsync(message);
        }
    }
}
