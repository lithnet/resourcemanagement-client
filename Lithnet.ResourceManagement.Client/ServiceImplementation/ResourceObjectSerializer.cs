using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Runtime.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    internal class ResourceObjectSerializer : XmlObjectSerializer
    {
        private ResourceObject resource;

        private string objectType;

        public ResourceObjectSerializer(ResourceObject resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            this.resource = resource;
            this.objectType = resource.ObjectTypeName;
        }

        public ResourceObjectSerializer(string objectType)
        {
            if (string.IsNullOrWhiteSpace(objectType))
            {
                throw new ArgumentNullException("objectType");
            }

            this.objectType = objectType;
        }

        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            if (!reader.IsStartElement())
            {
                return false;
            }

            if (reader.LocalName == this.objectType && reader.NamespaceURI == Namespaces.ResourceManagement)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            throw new NotImplementedException();
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            throw new NotImplementedException();
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            throw new NotImplementedException();
        }
    }
}
