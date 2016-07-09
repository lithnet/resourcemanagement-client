using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using System.Globalization;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal partial class ResourceClient : ClientBase<Resource>, Resource
    {
        private ResourceManagementClient client;

        public void Initialize(ResourceManagementClient client)
        {
            this.DisableContextManager();
            this.client = client;
        }

        public void Put(ResourceObject resource, CultureInfo locale)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            using (Message message = MessageComposer.CreatePutMessage(resource, locale))
            {
                if (message == null)
                {
                    return;
                }

                using (Message responseMessage = this.Invoke((c) => c.Put(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public void Put(IEnumerable<ResourceObject> resources, CultureInfo locale)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            if (resources.Any(t => t.Locale != null && t.Locale != locale))
            {
                foreach (ResourceObject resource in resources)
                {
                    this.Put(resource, locale);
                }

                return;
            }

            using (Message message = MessageComposer.CreatePutMessage(resources, locale))
            {
                if (message == null)
                {
                    return;
                }

                using (Message responseMessage = this.Invoke((c) => c.Put(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public ResourceObject Get(UniqueIdentifier id, IEnumerable<string> attributes, CultureInfo locale)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            bool partialResponse = attributes != null;
            
            GetResponse r = new GetResponse();

            using (Message message = MessageComposer.CreateGetMessage(id, attributes, locale))
            {
                using (Message responseMessage = this.Invoke((c) => c.Get(message)))
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

        public void Delete(IEnumerable<ResourceObject> resources)
        {
            this.Delete(resources.Select(t => t.ObjectID));
        }

        public void Delete(IEnumerable<UniqueIdentifier> resourceIDs)
        {
            if (!resourceIDs.Any())
            {
                return;
            }

            using (Message message = MessageComposer.CreateDeleteMessage(resourceIDs))
            {
                using (Message responseMessage = this.Invoke((c) => c.Delete(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public void Delete(ResourceObject resource)
        {
            this.Delete(resource.ObjectID);
        }

        public void Delete(UniqueIdentifier id)
        {
            using (Message message = MessageComposer.CreateDeleteMessage(id))
            {
                using (Message responseMessage = this.Invoke((c) => c.Delete(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        internal XmlDictionaryReader GetFullObjectForUpdate(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            GetResponse r = new GetResponse();

            using (Message message = MessageComposer.CreateGetMessage(resource.ObjectID, null, resource.Locale))
            {
                using (Message responseMessage = this.Invoke((c) => c.Get(message)))
                {
                    responseMessage.ThrowOnFault();

                    return responseMessage.GetReaderAtBodyContents();
                }
            }
        }
    }
}