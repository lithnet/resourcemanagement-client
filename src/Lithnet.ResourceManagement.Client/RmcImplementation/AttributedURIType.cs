using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "MessageID", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = false), XmlType(TypeName = "AttributedURI", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
    public class AttributedURIType
    {
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        [XmlText(DataType = "anyURI")]
        public string Value { get; set; }
    }
}