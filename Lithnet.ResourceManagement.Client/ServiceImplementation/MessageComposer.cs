using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using Microsoft.ResourceManagement.WebServices.WSEnumeration;
using Microsoft.ResourceManagement;
using Microsoft.ResourceManagement.WebServices;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Lithnet.ResourceManagement.Client;
using System.Configuration;
using System.Globalization;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal static class MessageComposer
    {
        private const string DefaultMaxCharacters = "3668672";
        private const string DefaultPageSize = "200";

        internal static Message CreateGetMessage(UniqueIdentifier id)
        {
            return CreateGetMessage(id, null);
        }

        internal static Message CreateGetMessage(UniqueIdentifier id, IEnumerable<string> attributes)
        {
            Get op = null;

            if (attributes != null && attributes.Any())
            {
                op = new Get();
                op.Dialect = Namespaces.RMIdentityAttributeType;
                op.Expressions = MessageComposer.AddMandatoryAttributes(attributes).ToArray();
            }

            Message message;

            if (op == null)
            {
                message = Message.CreateMessage(MessageVersion.Default, Namespaces.Get);
            }
            else
            {
                message = Message.CreateMessage(MessageVersion.Default, Namespaces.Get, new SerializerBodyWriter(op));
                message.AddHeader(Namespaces.IdMDirectoryAccess, HeaderConstants.IdentityManagementOperation, null, true);
            }

            message.AddHeader("Locale", CultureInfo.CurrentCulture);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, id.ToString());

            return message;

        }

        internal static Message CreateCreateMessage(ResourceObject resource)
        {
            Create op = new Create();

            op.Dialect = Namespaces.RMIdentityAttributeType;
            op.Fragments = resource.GetPutFragements().ToArray();

            Message message;

            message = Message.CreateMessage(MessageVersion.Default, Namespaces.Create, new SerializerBodyWriter(op));
            message.AddHeader(Namespaces.IdMDirectoryAccess, HeaderConstants.IdentityManagementOperation, null, true);

            return message;
        }

        internal static Message CreatePutMessage(ResourceObject resource)
        {
            Put op = new Put();

            op.Dialect = Namespaces.RMIdentityAttributeType;
            op.Fragments = resource.GetPutFragements().ToArray();

            Message message;

            message = Message.CreateMessage(MessageVersion.Default, Namespaces.Put, new SerializerBodyWriter(op));
            message.AddHeader(Namespaces.IdMDirectoryAccess, HeaderConstants.IdentityManagementOperation, null, true);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, resource.ObjectID.ToString());

            return message;
        }

        internal static Message CreatePutMessage(IEnumerable<ResourceObject> resources)
        {
            int count = resources.Count();
            if (count == 0)
            {
                throw new ArgumentNullException("resources");
            }
            else if (count == 1)
            {
                return MessageComposer.CreatePutMessage(resources.First());
            }

            Put op = new Put();

            op.Dialect = Namespaces.RMIdentityAttributeType;
            List<PutFragmentType> fragments = new List<PutFragmentType>();
            foreach (ResourceObject resource in resources)
            {
                foreach(PutFragmentType fragment in resource.GetPutFragements())
                {
                    fragment.TargetIdentifier = resource.ObjectID.Value;
                    fragments.Add(fragment);
                }
            }

            op.Fragments = fragments.ToArray();
            Message message;

            message = Message.CreateMessage(MessageVersion.Default, Namespaces.Put, new SerializerBodyWriter(op));
            message.AddHeader(Namespaces.IdMDirectoryAccess, HeaderConstants.IdentityManagementOperation, null, true);
            message.AddHeader(Namespaces.ResourceManagement, HeaderConstants.CompositeTypeOperation, null);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, resources.Select(t => t.ObjectID.ToString()).ToCommaSeparatedString());

            return message;
        }

        internal static Message CreateDeleteMessage(UniqueIdentifier id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            Message message;
            message = Message.CreateMessage(MessageVersion.Default, Namespaces.Delete);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, id.ToString());

            return message;
        }

        internal static Message CreateDeleteMessage(IEnumerable<UniqueIdentifier> ids)
        {
            int count = ids.Count();
            if (count == 0)
            {
                throw new ArgumentNullException("ids");
            }
            else if (count == 1)
            {
                return MessageComposer.CreateDeleteMessage(ids.First());
            }
            
            Message message;
            message = Message.CreateMessage(MessageVersion.Default, Namespaces.Delete);
            message.AddHeader(Namespaces.ResourceManagement, HeaderConstants.CompositeTypeOperation, null);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, ids.Select(t => t.ToString()).ToCommaSeparatedString());

            return message;
        }

        internal static Message CreateEnumerateMessage(string filter, int pageSize, IEnumerable<string> attributes)
        {
            Enumerate request = new Enumerate();
            request.Filter = new FilterType(filter);
            request.MaxCharacters = MessageComposer.DefaultMaxCharacters;
            request.MaxElements = pageSize < 0 ? MessageComposer.DefaultPageSize : pageSize.ToString();

            if (attributes != null)
            {
                request.Selection = MessageComposer.AddMandatoryAttributes(attributes).ToArray();
            }

            Message requestMessage = Message.CreateMessage(MessageVersion.Default, Namespaces.Enumerate, new SerializerBodyWriter(request));
            requestMessage.AddHeader(Namespaces.ResourceManagement, "IncludeCount", null);

            return requestMessage;
        }

        internal static Message GeneratePullMessage(EnumerationContextType context, int pageSize)
        {
            Pull op = new Pull();
            op.EnumerationContext = context;
            op.MaxElements = pageSize.ToString();

            return Message.CreateMessage(MessageVersion.Soap12WSAddressing10, Namespaces.Pull, new SerializerBodyWriter(op));
        }

        internal static Message GenerateReleaseMessage(EnumerationContextType context)
        {
            Release op = new Release();
            op.EnumerationContext = context;

            return Message.CreateMessage(MessageVersion.Soap12WSAddressing10, Namespaces.Release, new SerializerBodyWriter(op));
        }

        private static HashSet<string> AddMandatoryAttributes(IEnumerable<string> attributes)
        {
            HashSet<string> set = new HashSet<string>();

            foreach (string attribute in attributes)
            {
                set.Add(attribute);
            }

            foreach(string item in ResourceManagementSchema.MandatoryAttributes)
            {
                set.Add(item);
            }

            return set;
        }
    }
}


