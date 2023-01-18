using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "Metadata", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = false), XmlType(TypeName = "MetadataType", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
    public class MetadataType
    {
        [XmlAnyElement]
        public XmlElement[] Any { get; set; }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }
    }
}