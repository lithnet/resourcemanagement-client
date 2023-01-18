// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [XmlRoot(ElementName = MetadataStrings.MetadataExchangeStrings.MetadataReference, Namespace = MetadataStrings.MetadataExchangeStrings.Namespace)]
    public class MetadataReference : IXmlSerializable
    {
        private Collection<XmlAttribute> _attributes = new Collection<XmlAttribute>();
        private static XmlDocument s_document = new XmlDocument();

        public MetadataReference()
        {
        }

        public MetadataReference(EndpointAddress address, AddressingVersion addressVersion)
        {
            this.Address = address;
            this.AddressVersion = addressVersion;
        }

        public EndpointAddress Address
        {
            get; set;
        }

        public AddressingVersion AddressVersion
        {
            get; set;
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Address = EndpointAddress.ReadFrom(this.AddressVersion, XmlDictionaryReader.CreateDictionaryReader(reader));
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.Address != null)
            {
                XmlDictionaryWriter w = (XmlDictionaryWriter)XmlWriter.Create(writer);
                this.Address.WriteContentsTo(this.AddressVersion, w);
            }
        }
    }
}
