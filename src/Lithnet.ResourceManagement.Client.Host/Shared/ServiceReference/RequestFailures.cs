using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(AnonymousType = true), XmlRoot(ElementName = "RequestFailures", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class RequestFailures
    {
        [XmlElement(ElementName = "CorrelationId")]
        public string CorrelationIdentifier { get; set; }

        [XmlElement(ElementName = "EndpointReference", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        public EndpointReferenceType EndpointReference { get; set; }

        [XmlElement(ElementName = "RequestAdministratorDetails")]
        public RequestAdministratorDetails RequestAdministratorDetails { get; set; }
    }
}