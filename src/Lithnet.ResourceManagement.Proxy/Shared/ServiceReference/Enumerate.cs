using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "Enumerate", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", IsNullable = false), XmlType(AnonymousType = true)]
    public class Enumerate : IPullControl
    {
        [XmlAnyElement]
        public XmlElement[] Any { get; set; }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        [XmlElement(ElementName = "EndTo")]
        public EndpointReferenceType EndTo { get; set; }

        [XmlElement(ElementName = "Expires")]
        public string Expires { get; set; }

        [XmlElement(ElementName = "Filter")]
        public FilterType Filter { get; set; }

        [XmlArray(Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", ElementName = "LocalePreferences")]
        public LocalePreferenceType[] LocalePreferences { get; set; }

        [XmlElement(ElementName = "MaxCharacters", DataType = "positiveInteger")]
        public string MaxCharacters { get; set; }
       
        [XmlElement(ElementName = "MaxElements", DataType = "positiveInteger")]
        public string MaxElements { get; set; }
      
        [XmlElement(ElementName = "MaxTime", DataType = "duration")]
        public string MaxTime { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        public string[] Selection { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        public Sorting Sorting { get; set; }

        [XmlElement(ElementName = "Time", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        public DateTime? Time { get; set; }
    }
}