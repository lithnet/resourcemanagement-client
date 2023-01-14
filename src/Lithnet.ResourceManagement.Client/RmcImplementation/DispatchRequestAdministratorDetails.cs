using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "DispatchRequestAdministratorDetails", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class DispatchRequestAdministratorDetails
    {
        [XmlElement(ElementName = "AdditionalTextDetails")]
        public string AdditionalTextDetails { get; set; }

        [XmlElement(ElementName = "DispatchRequestFailureSource")]
        public DispatchRequestFailureSource DispatchRequestFailureSource { get; set; }

        [XmlElement(ElementName = "FailureMessage")]
        public string FailureMessage { get; set; }
    }
}