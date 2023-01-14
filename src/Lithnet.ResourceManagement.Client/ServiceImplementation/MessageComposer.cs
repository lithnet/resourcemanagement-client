﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Channels;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal static class MessageComposer
    {
        private const string DefaultPageSize = "200";

        private static HashSet<UniqueIdentifier> builtInResources;

        static MessageComposer()
        {
            MessageComposer.builtInResources = new HashSet<UniqueIdentifier>()
            {
                new UniqueIdentifier("7fb2b853-24f0-4498-9534-4e10589723c4"), // Built-in admin account
                new UniqueIdentifier("fb89aefa-5ea1-47f1-8890-abe7797d6497"), // Sync service account
                new UniqueIdentifier("e05d1f1b-3d5e-4014-baa6-94dee7d68c89"), // FIM service account
                new UniqueIdentifier("b0b36673-d43b-4cfa-a7a2-aff14fd90522") // anonymous resource
            };
        }

        internal static Message CreateGetMessage(UniqueIdentifier id)
        {
            return CreateGetMessage(id, null, null, false);
        }

        internal static Message CreateGetMessage(UniqueIdentifier id, string[] attributes, CultureInfo locale, bool includePermissions)
        {
            Get op = null;

            if (attributes != null && attributes.Length > 0)
            {
                op = new Get
                {
                    Dialect = Namespaces.RMIdentityAttributeType,
                    Expressions = MessageComposer.AddMandatoryAttributes(attributes, locale).ToArray()
                };
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

            if (locale != null)
            {
                message.AddHeader(AttributeNames.Locale, locale);
            }

            if (includePermissions)
            {
                message.AddHeader(HeaderConstants.IncludePermissionHints, null);
            }

            message.AddHeader(HeaderConstants.ResourceReferenceProperty, id.ToString());

            return message;
        }

        internal static Message CreateCreateMessage(ResourceObject resource)
        {
            Create op = new Create
            {
                Dialect = Namespaces.RMIdentityAttributeType,
                Fragments = resource.GetPutFragements().ToArray<FragmentType>()
            };

            Message message = Message.CreateMessage(MessageVersion.Default, Namespaces.Create, new SerializerBodyWriter(op));
            message.AddHeader(Namespaces.IdMDirectoryAccess, HeaderConstants.IdentityManagementOperation, null, true);

            return message;
        }

        internal static Message CreateCreateMessage(ResourceObject[] resources)
        {
            Create op = new Create();
            op.Dialect = Namespaces.RMIdentityAttributeType;

            if (resources == null || resources.Length == 0)
            {
                throw new ArgumentNullException(nameof(resources));
            }
            else if (resources.Length == 1)
            {
                return MessageComposer.CreateCreateMessage(resources[0]);
            }

            List<FragmentType> fragments = new List<FragmentType>();

            foreach (ResourceObject resource in resources)
            {
                foreach (PutFragmentType fragment in resource.GetPutFragements())
                {
                    fragment.TargetIdentifier = resource.ObjectID.ToString(false);
                    fragments.Add(fragment);
                }

                // Add Object ID
                PutFragmentType fragmentObjectID = new PutFragmentType(
                    AttributeNames.ObjectID,
                    ModeType.Insert,
                    AttributeNames.ObjectID,
                    null,
                    false,
                    resource.ObjectID.ToString(false));

                fragmentObjectID.TargetIdentifier = resource.ObjectID.ToString(false);
                fragments.Add(fragmentObjectID);
            }

            if (fragments.Count == 0)
            {
                return null;
            }

            op.Fragments = fragments.ToArray();

            Message message = Message.CreateMessage(MessageVersion.Default, Namespaces.Create, new SerializerBodyWriter(op));
            message.AddHeader(Namespaces.IdMDirectoryAccess, HeaderConstants.IdentityManagementOperation, null, true);
            message.AddHeader(Namespaces.ResourceManagement, HeaderConstants.CompositeTypeOperation, null);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, resources.Select(t => t.ObjectID.ToString(false)).ToCommaSeparatedString());

            return message;
        }

        internal static Message CreatePutMessage(ResourceObject resource, CultureInfo locale)
        {
            Put op = new Put
            {
                Dialect = Namespaces.RMIdentityAttributeType,
                Fragments = resource.GetPutFragements().ToArray()
            };

            if (op.Fragments == null || op.Fragments.Length == 0)
            {
                return null;
            }

            Message message = Message.CreateMessage(MessageVersion.Default, Namespaces.Put, new SerializerBodyWriter(op));
            message.AddHeader(Namespaces.IdMDirectoryAccess, HeaderConstants.IdentityManagementOperation, null, true);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, resource.ObjectID.ToString());

            if (locale != null || resource.Locale != null)
            {
                message.AddHeader(AttributeNames.Locale, locale ?? resource.Locale);
            }

            return message;
        }

        internal static Message CreatePutMessage(ResourceObject[] resources)
        {
            if (resources == null || resources.Length == 0)
            {
                throw new ArgumentNullException(nameof(resources));
            }
            else if (resources.Length == 1)
            {
                return MessageComposer.CreatePutMessage(resources[0], null);
            }

            Put op = new Put();

            op.Dialect = Namespaces.RMIdentityAttributeType;
            List<PutFragmentType> fragments = new List<PutFragmentType>();

            foreach (ResourceObject resource in resources)
            {
                foreach (PutFragmentType fragment in resource.GetPutFragements())
                {
                    fragment.TargetIdentifier = resource.ObjectID.Value;
                    fragments.Add(fragment);
                }
            }

            if (fragments.Count == 0)
            {
                return null;
            }

            op.Fragments = fragments.ToArray();

            Message message = Message.CreateMessage(MessageVersion.Default, Namespaces.Put, new SerializerBodyWriter(op));
            message.AddHeader(Namespaces.IdMDirectoryAccess, HeaderConstants.IdentityManagementOperation, null, true);
            message.AddHeader(Namespaces.ResourceManagement, HeaderConstants.CompositeTypeOperation, null);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, resources.Select(t => t.ObjectID.ToString()).ToCommaSeparatedString());

            return message;
        }

        internal static Message CreateDeleteMessage(UniqueIdentifier id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            MessageComposer.ThrowOnDeleteBuiltInResource(id);

            Message message = Message.CreateMessage(MessageVersion.Default, Namespaces.Delete);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, id.ToString());

            return message;
        }

        internal static Message CreateDeleteMessage(UniqueIdentifier[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                throw new ArgumentNullException(nameof(ids));
            }
            else if (ids.Length == 1)
            {
                return MessageComposer.CreateDeleteMessage(ids.First());
            }

            MessageComposer.ThrowOnDeleteBuiltInResource(ids);

            Message message = Message.CreateMessage(MessageVersion.Default, Namespaces.Delete);
            message.AddHeader(Namespaces.ResourceManagement, HeaderConstants.CompositeTypeOperation, null);
            message.AddHeader(HeaderConstants.ResourceReferenceProperty, ids.Select(t => t.ToString()).ToCommaSeparatedString());

            return message;
        }

        internal static Message CreateEnumerateMessage(string filter, int pageSize, string[] attributes, SortingAttribute[] sortingAttributes, CultureInfo locale)
        {
            Enumerate request = new Enumerate
            {
                Filter = new FilterType(filter),
                MaxElements = pageSize < 0 ? MessageComposer.DefaultPageSize : pageSize.ToString()
            };

            if (attributes != null && attributes.Length > 0)
            {
                request.Selection = MessageComposer.AddMandatoryAttributes(attributes, locale).ToArray();
            }

            if (sortingAttributes != null && sortingAttributes.Length > 0)
            {
                request.Sorting = new Sorting
                {
                    Dialect = Namespaces.ResourceManagement,
                    SortingAttributes = sortingAttributes
                };
            }

            if (locale != null)
            {
                request.LocalePreferences = new LocalePreferenceType[1];
                request.LocalePreferences[0] = new LocalePreferenceType(locale, 1);
            }

            Message requestMessage = Message.CreateMessage(MessageVersion.Default, Namespaces.Enumerate, new SerializerBodyWriter(request));
            requestMessage.AddHeader(Namespaces.ResourceManagement, "IncludeCount", null);

            return requestMessage;
        }

        internal static Message GeneratePullMessage(EnumerationContextType context, int pageSize)
        {
            Pull op = new Pull
            {
                EnumerationContext = context,
                MaxElements = pageSize.ToString()
            };

            return Message.CreateMessage(MessageVersion.Soap12WSAddressing10, Namespaces.Pull, new SerializerBodyWriter(op));
        }

        internal static Message GenerateReleaseMessage(EnumerationContextType context)
        {
            Release op = new Release { EnumerationContext = context };

            return Message.CreateMessage(MessageVersion.Soap12WSAddressing10, Namespaces.Release, new SerializerBodyWriter(op));
        }

        internal static Message CreateApprovalMessage(UniqueIdentifier workflowID, ApprovalResponse response)
        {
            Message message = Message.CreateMessage(MessageVersion.Default, Namespaces.Create, new SerializerBodyWriter(response));
            message.Headers.Add(new ContextHeader(workflowID.Value));

            return message;
        }

        private static IEnumerable<string> AddMandatoryAttributes(string[] attributes, CultureInfo locale)
        {
            HashSet<string> set = new HashSet<string>(attributes);

            foreach (string item in SchemaConstants.MandatoryAttributes)
            {
                set.Add(item);
            }

            if (locale != null)
            {
                set.Add(AttributeNames.Locale);
            }

            return set;
        }

        private static void ThrowOnDeleteBuiltInResource(IEnumerable<UniqueIdentifier> ids)
        {
            foreach (UniqueIdentifier id in ids)
            {
                MessageComposer.ThrowOnDeleteBuiltInResource(id);
            }
        }

        private static void ThrowOnDeleteBuiltInResource(UniqueIdentifier id)
        {
            if (MessageComposer.builtInResources.Contains(id))
            {
                throw new InvalidOperationException($"A request to delete a built-in resource has been stopped by the client library. Resource: {id}");
            }
        }
    }
}


