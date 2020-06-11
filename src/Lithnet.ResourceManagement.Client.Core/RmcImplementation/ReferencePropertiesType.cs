using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(TypeName = "ReferencePropertiesType", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing"), XmlRoot(ElementName = "ReferenceProperties", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing", IsNullable = false)]
    public class ReferencePropertiesType
    {

        public ReferencePropertiesType()
        {
        }

        public ReferencePropertiesType(UniqueIdentifier objectId)
        {
            if (objectId != null)
            {
                this.ResourceReferenceProperty = new ResourceReferencePropertyType(objectId);
            }
        }

        [XmlAnyElement]
        public XmlNode[] Any { get; set; }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        [XmlElement(ElementName = "Locale")]
        public LocaleReferencePropertyType Locale { get; set; }

        [XmlElement(ElementName = "ResourceReferenceProperty", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        public ResourceReferencePropertyType ResourceReferenceProperty { get; set; }
    }
}