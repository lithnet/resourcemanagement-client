using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(AnonymousType = true), XmlRoot(ElementName = "Pull", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", IsNullable = false)]
    public class Pull : IPullControl
    {

        // Properties
        [XmlAnyElement]
        public XmlElement[] Any { get; set; }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        [XmlElement(ElementName = "EnumerationContext")]
        public EnumerationContextType EnumerationContext { get; set; } = new EnumerationContextType();

        [XmlElement(ElementName = "MaxCharacters", DataType = "positiveInteger")]
        public string MaxCharacters{ get; set; }

        [XmlElement(ElementName = "MaxElements", DataType = "positiveInteger")]
        public string MaxElements { get; set; }

        [XmlElement(ElementName = "MaxTime", DataType = "duration")]
        public string MaxTime { get; set; }

        [XmlElement(ElementName = "PullAdjustment", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        public PullAdjustmentType PullAdjustment { get; set; }
    }
}