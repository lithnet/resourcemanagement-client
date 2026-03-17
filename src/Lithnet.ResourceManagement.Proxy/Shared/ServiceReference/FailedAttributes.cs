using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "FailedAttributes", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class FailedAttributes
    {
        [XmlElement(ElementName = "AttributeType")]
        public string[] AttributeType { get; set; }
    }
}