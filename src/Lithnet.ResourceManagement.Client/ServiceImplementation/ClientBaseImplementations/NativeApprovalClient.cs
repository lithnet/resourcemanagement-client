using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal class NativeApprovalClient : IApprovalService
    {
        private readonly Binding binding;
        private readonly NetworkCredential credentials;

        public NativeApprovalClient(Binding binding, NetworkCredential credentials)
        {
            this.binding = binding;
            this.credentials = credentials;
        }

        public Task<Message> ApproveAsync(string endpoint, Message message)
        {
            var client = new NativeResourceFactoryClient(this.binding, EndpointManager.EndpointFromAddress(endpoint));

            if (this.credentials != null)
            {
                client.ClientCredentials.Windows.ClientCredential = this.credentials;
            }

            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;

            return InternalExtensions.InvokeAsync(client, c => c.CreateAsync(message));
        }
    }
}