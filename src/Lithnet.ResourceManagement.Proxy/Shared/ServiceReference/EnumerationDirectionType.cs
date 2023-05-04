using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(TypeName = "EnumerationDirection", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement"), XmlRoot(ElementName = "EnumerationDirection", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false)]
    public enum EnumerationDirectionType
    {
        Forwards,
        Backwards
    }
}