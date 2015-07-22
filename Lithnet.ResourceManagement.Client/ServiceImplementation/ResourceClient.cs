using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;

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

        public void Put(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            using (Message message = MessageComposer.CreatePutMessage(resource))
            {
                using (Message responseMessage = this.Invoke((c) => c.Put(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public void Put(IEnumerable<ResourceObject> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            using (Message message = MessageComposer.CreatePutMessage(resources))
            {
                using (Message responseMessage = this.Invoke((c) => c.Put(message)))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }

        public ResourceObject Get(UniqueIdentifier id, IEnumerable<string> attributes)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            bool partialResponse = attributes != null;

            GetResponse r = new GetResponse();

            using (Message message = MessageComposer.CreateGetMessage(id, attributes))
            {
                using (Message responseMessage = this.Invoke((c) => c.Get(message)))
                {
                    responseMessage.ThrowOnFault();

                    if (partialResponse)
                    {
                        GetResponse response = responseMessage.DeserializeMessageWithPayload<GetResponse>();
                        return new ResourceObject(response.Results.OfType<XmlElement>(), this.client);
                    }
                    else
                    {
                        XmlDictionaryReader fullObject = responseMessage.GetReaderAtBodyContents();
                        return new ResourceObject(fullObject, this.client);
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

        internal ResourceObject Get(UniqueIdentifier id)
        {
            return this.Get(id, null);
        }

        internal XmlDictionaryReader GetFullObjectForUpdate(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            GetResponse r = new GetResponse();

            using (Message message = MessageComposer.CreateGetMessage(resource.ObjectID))
            {
                using (Message responseMessage = this.Invoke((c) => c.Get(message)))
                {
                    responseMessage.ThrowOnFault();

                    return responseMessage.GetReaderAtBodyContents();
                }
            }
        }

        public T Invoke<T>(Func<Resource, T> action)
        {
            Resource c = this.ChannelFactory.CreateChannel();
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
