using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(TypeName = "AuthenticationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IncludeInSchema = false), XmlRoot(ElementName = "AuthenticationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = true)]
    public class AuthenticationRequiredFault
    {
        [XmlElement(ElementName = "SecurityTokenServiceAddress")]
        public string SecurityTokenServiceAddress { get; set; }

        [XmlElement(ElementName = "UserLockedOut")]
        public bool? UserLockedOut { get; set; }

        [XmlElement(ElementName = "UserRegistered")]
        public bool? UserRegistered { get; set; }
    }
}