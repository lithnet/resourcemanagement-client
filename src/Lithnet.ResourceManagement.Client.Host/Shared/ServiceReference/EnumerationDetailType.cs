using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    [Serializable, XmlRoot(ElementName = "EnumerationDetail", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false), XmlType(TypeName = "EnumerationDetail", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class EnumerationDetailType
    {
        public EnumerationDetailType()
        {
        }

        public EnumerationDetailType(int count)
        {
            this.Count = count.ToString(NumberFormatInfo.InvariantInfo);
        }

        [XmlElement(ElementName = "Count", DataType = "positiveInteger")]
        public string Count { get; set; }
    }
}