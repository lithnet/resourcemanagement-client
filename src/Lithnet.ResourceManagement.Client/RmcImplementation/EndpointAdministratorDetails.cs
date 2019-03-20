using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client.RmcImplementation
{
    [Serializable, XmlRoot(ElementName = "EndpointAdministratorDetails", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class EndpointAdministratorDetails
    {
        [XmlElement(ElementName = "AdditionalTextDetails")]
        public string AdditionalTextDetails { get; set; }

        [XmlElement(ElementName = "FailureMessage")]
        public string FailureMessage { get; set; }

        [XmlElement(ElementName = "HealthFailureSource")]
        public HealthFailureSource HealthFailureSource { get; set; }
    }
}