using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using System.Globalization;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal partial class ResourceClient 
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
                throw new ArgumentNullException(nameof(resource));
            }

            using (Message message = MessageComposer.CreatePutMessage(resource, locale))
            {
                if (message == null)
                {
                    return;
                }

                using (Message responseMessage = this.Invoke(c => c.Put(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public void Put(IEnumerable<ResourceObject> resources)
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

                using (Message responseMessage = this.Invoke((c) => c.Put(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public ResourceObject Get(UniqueIdentifier id, IEnumerable<string> attributes, CultureInfo locale, bool getPermissions)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            bool partialResponse = attributes != null;
            
            using (Message message = MessageComposer.CreateGetMessage(id, attributes?.ToArray(), locale, getPermissions))
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
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            this.Delete(resources.Select(t => t.ObjectID));
        }

        public void Delete(IEnumerable<UniqueIdentifier> resourceIDs)
        {
            if (resourceIDs == null)
            {
                throw new ArgumentNullException(nameof(resourceIDs));
            }

            using (Message message = MessageComposer.CreateDeleteMessage(resourceIDs.ToArray()))
            {
                using (Message responseMessage = this.Invoke((c) => c.Delete(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public void Delete(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            this.Delete(resource.ObjectID);
        }

        public void Delete(UniqueIdentifier id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

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
                throw new ArgumentNullException(nameof(resource));
            }

            using (Message message = MessageComposer.CreateGetMessage(resource.ObjectID, null, resource.Locale, resource.HasPermissionHints))
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