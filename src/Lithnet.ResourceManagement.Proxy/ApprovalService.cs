using System;
using System.Net;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Proxy
{
    internal class ApprovalService
    {
        private readonly Binding binding;
        private readonly NetworkCredential credentials;
        private readonly WindowsIdentity impersonationIdentity;
        private readonly Func<string, Uri> endpointerMapper;

        public ApprovalService(Binding binding, NetworkCredential credentials, WindowsIdentity impersonationIdentity, Func<string, Uri> endpointMapper)
        {
            this.binding = binding;
            this.credentials = credentials;
            this.impersonationIdentity = impersonationIdentity;
            this.endpointerMapper = endpointMapper;
        }

        public async Task<Message> ApproveAsync(string endpoint, Message message)
        {
            if (this.endpointerMapper != null)
            {
                endpoint = this.endpointerMapper(endpoint).ToString();
            }

            NativeResourceFactoryClient client = new NativeResourceFactoryClient(this.binding, EndpointManager.EndpointFromAddress(endpoint));

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
