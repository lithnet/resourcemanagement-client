using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "RequestAdministratorDetails", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class RequestAdministratorDetails
    {
        [XmlElement(ElementName = "AdditionalTextDetails")]
        public string AdditionalTextDetails { get; set; }

        [XmlElement(ElementName = "FailedAttributes")]
        public FailedAttributes FailedAttributes { get; set; }

        [XmlElement(ElementName = "FailureMessage")]
        public string FailureMessage { get; set; }

        [XmlElement(ElementName = "RequestFailureSource")]
        public RequestFailureSource RequestFailureSource { get; set; }
    }
}