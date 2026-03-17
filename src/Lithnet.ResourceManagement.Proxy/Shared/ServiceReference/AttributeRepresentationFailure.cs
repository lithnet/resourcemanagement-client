using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "AttributeRepresentationFailure", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class AttributeRepresentationFailure
    {
        [XmlElement(ElementName = "AdditionalTextDetails")]
        public string AdditionalTextDetails { get; set; }

        [XmlElement(ElementName = "AttributeFailureCode")]
        public AttributeFailureCode AttributeFailureCode { get; set; }

        [XmlElement(ElementName = "AttributeType")]
        public string AttributeType { get; set; }

        [XmlElement(ElementName = "AttributeValue")]
        public string AttributeValue { get; set; }

        [XmlElement(ElementName = "FailureMessage")]
        public string FailureMessage { get; set; }
    }
}
