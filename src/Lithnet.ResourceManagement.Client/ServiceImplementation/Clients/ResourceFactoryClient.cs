using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Rmc = Lithnet.ResourceManagement.Client;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal class ResourceFactoryClient : IResourceFactoryClient
    {
        private readonly IClient client;

        public ResourceFactoryClient(IClient client)
        {
            this.client = client;
        }

        public async Task CreateAsync(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            try
            {
                using (Message message = MessageComposer.CreateCreateMessage(resource))
                {
                    var channel = await this.client.GetResourceFactoryChannelAsync();

                    using (Message responseMessage = await channel.CreateAsync(message).ConfigureAwait(false))
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
                throw InvalidRepresentationException.GetException(e.Detail);
            }
        }

        public async Task CreateAsync(IEnumerable<ResourceObject> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            ResourceObject[] resourceArray = resources.ToArray();

            try
            {
                using (Message message = MessageComposer.CreateCreateMessage(resourceArray))
                {
                    var channel = await this.client.GetResourceFactoryChannelAsync();

                    using (Message responseMessage = await channel.CreateAsync(message).ConfigureAwait(false))
                    {
                        responseMessage.ThrowOnFault();

                        foreach (ResourceObject resource in resourceArray)
                        {
                            resource.CompleteCreateOperation(resource.ObjectID);
                        }
                    }
                }
            }
            catch (FaultException<Rmc.RepresentationFailures> e)
            {
                throw InvalidRepresentationException.GetException(e.Detail);
            }
        }
    }
}