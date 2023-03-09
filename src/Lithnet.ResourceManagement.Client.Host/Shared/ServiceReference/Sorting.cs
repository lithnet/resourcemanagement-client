using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = true), XmlType(Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class Sorting
    {
        [XmlAttribute(AttributeName = "Dialect", DataType = "anyURI")]
        public string Dialect { get; set; } = "http://schemas.microsoft.com/2006/11/ResourceManagement";

        [XmlElement(ElementName = "SortingAttribute")]
        public SortingAttribute[] SortingAttributes { get; set; }
    }
}