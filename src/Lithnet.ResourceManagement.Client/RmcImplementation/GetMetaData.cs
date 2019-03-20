using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [XmlRoot(ElementName = "GetMetadata", Namespace = "http://schemas/xmlsoap.org/ws/2004/09/mex", IsNullable = false)]
    public class GetMetadata
    {
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttribute { get; set; }

        [XmlElement(ElementName = "Dialect", DataType = "anyURI")]
        public string Dialect { get; set; }

        [XmlElement(ElementName = "Identifier", DataType = "anyURI")]
        public string Identifier { get; set; }
    }
}