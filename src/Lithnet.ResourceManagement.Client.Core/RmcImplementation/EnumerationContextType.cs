using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "EnumerationContext", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", IsNullable = false), XmlType(TypeName = "EnumerationContext", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration")]
    public class EnumerationContextType
    {
        [XmlElement(ElementName = "CurrentIndex", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false)]
        public int CurrentIndex { get; set; }

        [XmlElement(ElementName = "EnumerationDirection", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false)]
        public EnumerationDirectionType EnumerationDirection { get; set; }

        [XmlElement(ElementName = "Expires", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false)]
        public DateTime Expires { get; set; }

        [XmlElement(ElementName = "Filter", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false)]
        public string Filter { get; set; }

        [XmlArray(Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", ElementName = "LocalePreferences", IsNullable = true)]
        public LocalePreferenceType[] LocalePreferences { get; set; }

        [XmlArray(Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", ElementName = "Selection", IsNullable = true)]
        public string[] Selection { get; set; }

        [XmlElement(ElementName = "Sorting", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = true)]
        public Sorting Sorting { get; set; }

        [XmlElement(ElementName = "TemporalView", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false)]
        public TimeRestriction TemporalView { get; set; }
    }
}