using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.Exceptions;
using Microsoft.ResourceManagement.WebServices.Faults;
using Microsoft.ResourceManagement.WebServices.WSTransfer;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal partial class ResourceFactoryClient : System.ServiceModel.ClientBase<Lithnet.ResourceManagement.Client.ResourceManagementService.ResourceFactory>, Lithnet.ResourceManagement.Client.ResourceManagementService.ResourceFactory
    {
        public void Initialize()
        {
            this.DisableContextManager();
        }

        public void Create(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
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

        public T Invoke<T>(Func<ResourceFactory, T> action)
        {
            ResourceFactory c = this.ChannelFactory.CreateChannel();
            T returnValue;

            try
            {
                ((IClientChannel)c).Open();
                returnValue = action(c);
                ((IClientChannel)c).Close();
                return returnValue;
            }
            catch
            {
                ((IClientChannel)c).Abort();
                throw;
            }
        }
    }
}