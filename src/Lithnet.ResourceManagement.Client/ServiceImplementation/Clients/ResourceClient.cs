using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal class ResourceClient : IResourceClient
    {
        private readonly IClient client;

        public ResourceClient(IClient client)
        {
            this.client = client;
        }

        public async Task PutAsync(ResourceObject resource, CultureInfo locale)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            using (Message message = MessageComposer.CreatePutMessage(resource, locale))
            {
                if (message == null)
                {
                    return;
                }

                var channel = await this.client.GetResourceChannelAsync();
                using (Message responseMessage = await channel.PutAsync(message).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public async Task PutAsync(IEnumerable<ResourceObject> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            using (Message message = MessageComposer.CreatePutMessage(resources.ToArray()))
            {
                if (message == null)
                {
                    return;
                }

                var channel = await this.client.GetResourceChannelAsync();
                using (Message responseMessage = await channel.PutAsync(message).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public async Task<ResourceObject> GetAsync(UniqueIdentifier id, IEnumerable<string> attributes, CultureInfo locale, bool getPermissions)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            List<string> fixedAttributes = new List<string>();
            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    fixedAttributes.Add(await this.client.SchemaClient.GetCorrectAttributeNameCaseAsync(attribute));
                }
            }

            bool partialResponse = attributes != null;

            using (Message message = MessageComposer.CreateGetMessage(id, fixedAttributes.ToArray(), locale, getPermissions))
            {
                var channel = await this.client.GetResourceChannelAsync();

                using (Message responseMessage = await channel.GetAsync(message).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();

                    if (partialResponse)
                    {
                        GetResponse response = responseMessage.DeserializeMessageWithPayload<GetResponse>();
                        return new ResourceObject(response.Results.OfType<XmlElement>(), this.client, locale);
                    }
                    else
                    {
                        XmlDictionaryReader fullObject = responseMessage.GetReaderAtBodyContents();
                        return new ResourceObject(fullObject, this.client, locale);
                    }
                }
            }
        }

        public async Task DeleteAsync(IEnumerable<ResourceObject> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            await this.DeleteAsync(resources.Select(t => t.ObjectID)).ConfigureAwait(false);
        }

        public async Task DeleteAsync(IEnumerable<UniqueIdentifier> resourceIDs)
        {
            if (resourceIDs == null)
            {
                throw new ArgumentNullException(nameof(resourceIDs));
            }

            UniqueIdentifier[] ids = resourceIDs.ToArray();

            if (ids.Length == 0)
            {
                return;
            }

            using (Message message = MessageComposer.CreateDeleteMessage(ids))
            {
                var channel = await this.client.GetResourceChannelAsync();

                using (Message responseMessage = await channel.DeleteAsync(message).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public async Task DeleteAsync(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            await this.DeleteAsync(resource.ObjectID).ConfigureAwait(false);
        }

        public async Task DeleteAsync(UniqueIdentifier id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            using (Message message = MessageComposer.CreateDeleteMessage(id))
            {
                var channel = await this.client.GetResourceChannelAsync();

                using (Message responseMessage = await channel.DeleteAsync(message).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public async Task<XmlDictionaryReader> GetFullObjectForUpdateAsync(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            using (Message message = MessageComposer.CreateGetMessage(resource.ObjectID, null, resource.Locale, resource.HasPermissionHints))
            {
                var channel = await this.client.GetResourceChannelAsync();

                using (Message responseMessage = await channel.GetAsync(message).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();

                    return responseMessage.GetReaderAtBodyContents();
                }
            }
        }
    }
}