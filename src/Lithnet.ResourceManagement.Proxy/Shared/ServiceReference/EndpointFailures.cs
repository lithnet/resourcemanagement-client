using System;
using System.Xml.Serialization;
using Lithnet.ResourceManagement.Client.RmcImplementation;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "EndpointFailures", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement"), XmlType(AnonymousType = true)]
    public class EndpointFailures
    {
        [XmlElement(ElementName = "EndpointAdministratorDetails")]
        public EndpointAdministratorDetails AdministratorDetails { get; set; }

        [XmlElement(ElementName = "CorrelationId")]
        public string CorrelationIdentifier { get; set; }

        [XmlElement(ElementName = "EndpointReference", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        public EndpointReferenceType EndpointReference { get; set; }
    }
}