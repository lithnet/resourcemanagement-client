using System;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal class ApprovalClient : IApprovalClient
    {
        private readonly IClient client;

        private const string ApprovedText = "Approved";

        private const string RejectedText = "Rejected";

        public ApprovalClient(IClient client)
        {
            this.client = client;
        }

        public async Task ApproveAsync(string endpoint, UniqueIdentifier workflowInstance, UniqueIdentifier approvalRequest, bool approve, string reason = null)
        {
            if (workflowInstance == null)
            {
                throw new ArgumentNullException(nameof(workflowInstance));
            }

            if (approvalRequest == null)
            {
                throw new ArgumentNullException(nameof(approvalRequest));
            }

            ApprovalResponse response = new ApprovalResponse
            {
                Decision = approve ? ApprovedText : RejectedText,
                Reason = reason,
                Approval = approvalRequest.ToString()
            };

            using (Message message = MessageComposer.CreateApprovalMessage(workflowInstance, response))
            {
                var channel = await this.client.GetApprovalChannelAsync();

                using (Message responseMessage = await channel.ApproveAsync(endpoint, message).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }
    }
}