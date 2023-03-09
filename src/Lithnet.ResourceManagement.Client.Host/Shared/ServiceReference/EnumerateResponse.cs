using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    [Serializable, XmlType(AnonymousType = true), XmlRoot(ElementName = "EnumerateResponse", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", IsNullable = false)]
    public class EnumerateResponse : IPullResponse
    {
        [XmlAnyElement]
        public XmlElement[] Any { get; set; }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        [XmlElement(ElementName = "EndOfSequence")]
        public object EndOfSequence { get; set; }

        [XmlElement(ElementName = "EnumerationContext")]
        public EnumerationContextType EnumerationContext { get; set; }

        [XmlElement(ElementName = "EnumerationDetail", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        public EnumerationDetailType EnumerationDetail { get; set; }

        [XmlElement(ElementName = "Expires")]
        public string Expires { get; set; }

        [XmlElement(ElementName = "Items")]
        public ItemListType Items { get; set; }
    }
}