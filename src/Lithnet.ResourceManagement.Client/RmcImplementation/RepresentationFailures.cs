using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(AnonymousType = true), XmlRoot(ElementName = "RepresentationFailures", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class RepresentationFailures
    {
        [XmlElement(ElementName = "AttributeRepresentationFailure")]
        public AttributeRepresentationFailure[] AttributeRepresentationFailures { get; set; }

        [XmlElement(ElementName = "CorrelationId")]
        public string CorrelationIdentifier { get; set; }

        [XmlElement(ElementName = "MessageRepresentationFailure")]
        public MessageRepresentationFailure[] MessageRepresentationFailures { get; set; }
    }
}