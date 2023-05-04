using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    [Serializable, XmlRoot(ElementName = "PullResponse", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", IsNullable = false), XmlType(AnonymousType = true)]
    public class PullResponse : IPullResponse
    {
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        [XmlElement(ElementName = "EndOfSequence")]
        public object EndOfSequence { get; set; }

        [XmlElement(ElementName = "EnumerationContext")]
        public EnumerationContextType EnumerationContext { get; set; }

        [XmlElement(ElementName = "Items")]
        public ItemListType Items { get; set; }
    }
}