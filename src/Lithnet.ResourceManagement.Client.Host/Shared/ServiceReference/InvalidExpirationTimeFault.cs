using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(TypeName = "InvalidExpirationTime", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", IncludeInSchema = false), XmlRoot(ElementName = "InvalidExpirationTime", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", IsNullable = true)]
    public class InvalidExpirationTimeFault
    {
    }
}