using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "Release", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", IsNullable = false), XmlType(AnonymousType = true)]
    public class Release
    {
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        [XmlElement(ElementName = "EnumerationContext")]
        public EnumerationContextType EnumerationContext { get; set; } = new EnumerationContextType();
    }
}