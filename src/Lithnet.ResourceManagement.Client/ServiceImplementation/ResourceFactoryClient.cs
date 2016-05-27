using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.Exceptions;
using Microsoft.ResourceManagement.WebServices.Faults;
using Microsoft.ResourceManagement.WebServices.WSTransfer;
using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal partial class ResourceFactoryClient : System.ServiceModel.ClientBase<Lithnet.ResourceManagement.Client.ResourceManagementService.ResourceFactory>, Lithnet.ResourceManagement.Client.ResourceManagementService.ResourceFactory
    {
        private ResourceManagementClient client;

        private const string ApprovedText = "Approved";

        private const string RejectedText = "Rejected";

        public void Initialize(ResourceManagementClient client)
        {
            this.DisableContextManager();
            this.client = client;
        }

        public void Create(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            try
            {
                using (Message message = MessageComposer.CreateCreateMessage(resource))
                {
                    using (Message responseMessage = this.Invoke((c) => c.Create(message)))
                    {
                        responseMessage.ThrowOnFault();

                        ResourceCreatedType response = responseMessage.DeserializeMessageWithPayload<ResourceCreatedType>();
                        UniqueIdentifier id = new UniqueIdentifier(response.EndpointReference.ReferenceProperties.ResourceReferenceProperty.Text);
                        resource.CompleteCreateOperation(id);
                    }
                }
            }
            catch (FaultException<RepresentationFailures> e)
            {
                throw new InvalidRepresentationException(e.Detail.AttributeRepresentationFailures[0].AttributeFailureCode, e.Detail.AttributeRepresentationFailures[0].AttributeType, e.Detail.AttributeRepresentationFailures[0].AttributeValue);
            }
        }
        
        public void Approve(UniqueIdentifier workflowInstance, UniqueIdentifier approvalRequest, bool approve, string reason = null)
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
                Decision = approve ? ResourceFactoryClient.ApprovedText : ResourceFactoryClient.RejectedText,
                Reason = reason,
                Approval = approvalRequest.ToString()
            };

           
            using (Message message = MessageComposer.CreateApprovalMessage(workflowInstance, response))
            {
                using (Message responseMessage = this.Invoke((c) => c.Create(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public void Create(IEnumerable<ResourceObject> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            try
            {
                using (Message message = MessageComposer.CreateCreateMessage(resources))
                {
                    using (Message responseMessage = this.Invoke((c) => c.Create(message)))
                    {
                        responseMessage.ThrowOnFault();

                        foreach (ResourceObject resource in resources)
                        {
                            resource.CompleteCreateOperation(resource.ObjectID);
                        }
                    }
                }
            }
            catch (FaultException<RepresentationFailures> e)
            {
                throw new InvalidRepresentationException(e.Detail.AttributeRepresentationFailures[0].AttributeFailureCode, e.Detail.AttributeRepresentationFailures[0].AttributeType, e.Detail.AttributeRepresentationFailures[0].AttributeValue);
            }
        }
    }
}