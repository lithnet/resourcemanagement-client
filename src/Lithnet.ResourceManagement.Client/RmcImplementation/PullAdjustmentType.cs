using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(TypeName = "PullAdjustmentType", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement"), XmlRoot(ElementName = "PullAdjustment", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false)]
    public class PullAdjustmentType
    {
        [XmlElement(ElementName = "EnumerationDirection")]
        public EnumerationDirectionType EnumerationDirection { get; set; }

        [XmlElement(ElementName = "StartingIndex", DataType = "positiveInteger")]
        public string StartingIndex { get; set; }
    }
}