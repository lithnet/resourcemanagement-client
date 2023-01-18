using System;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal class ApprovalClient : IApprovalClient
    {
        private const string ApprovedText = "Approved";

        private const string RejectedText = "Rejected";

        public ApprovalClient(IApprovalService approvalService)
        {
            this.ApprovalService = approvalService;
        }

        public IApprovalService ApprovalService { get; }

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
                using (Message responseMessage = await this.ApprovalService.ApproveAsync(endpoint, message).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }
    }
}