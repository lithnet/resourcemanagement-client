using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(TypeName = "ItemList", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration")]
    public class ItemListType
    {
        public ItemListType()
        {
        }

        [XmlAnyElement]
        public XmlNode[] Any { get; set; }
    }
}