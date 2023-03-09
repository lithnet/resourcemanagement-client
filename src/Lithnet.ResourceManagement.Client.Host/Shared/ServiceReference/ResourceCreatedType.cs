using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "ResourceCreated", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer", IsNullable = false), XmlType(TypeName = "ResourceCreated", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer")]
    public class ResourceCreatedType
    {
        [XmlIgnore]
        public AttributedURIType Address
        {
            get => this.EndpointReference.Address;
            set => this.EndpointReference.Address = value;
        }

        [XmlIgnore]
        public XmlElement[] Any
        {
            get => this.EndpointReference.Any;
            set => this.EndpointReference.Any = value;
        }

        [XmlIgnore]
        public XmlAttribute[] AnyAttr
        {
            get => this.EndpointReference.AnyAttr;
            set => this.EndpointReference.AnyAttr = value;
        }

        [XmlElement(ElementName = "EndpointReference", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        public EndpointReferenceType EndpointReference { get; set; } = new EndpointReferenceType();

        [XmlIgnore]
        public MetadataType Metadata
        {
            get => this.EndpointReference.Metadata;
            set => this.EndpointReference.Metadata = value;
        }

        [XmlIgnore]
        public ReferencePropertiesType ReferenceProperties
        {
            get => this.EndpointReference.ReferenceProperties;
            set => this.EndpointReference.ReferenceProperties = value;
        }
    }
}