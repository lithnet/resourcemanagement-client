using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "MessageRepresentationFailure", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class MessageRepresentationFailure
    {
        [XmlElement(ElementName = "AdditionalTextDetails")]
        public string AdditionalTextDetails { get; set; }

        [XmlElement(ElementName = "FailureMessage")]
        public string FailureMessage { get; set; }

        [XmlElement(ElementName = "MessageFailureCode")]
        public MessageFailureCode MessageFailureCode { get; set; }
    }
}