using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "EndpointReference", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = false), XmlType(TypeName = "EndpointReference", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
    public class EndpointReferenceType
    {
        [XmlElement(ElementName = "Address")]
        public AttributedURIType Address { get; set; }

        [XmlAnyElement]
        public XmlElement[] Any { get; set; }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        [XmlElement(ElementName = "Metadata")]
        public MetadataType Metadata { get; set; }

        [XmlElement(ElementName = "ReferenceProperties")]
        public ReferencePropertiesType ReferenceProperties { get; set; }
    }
}