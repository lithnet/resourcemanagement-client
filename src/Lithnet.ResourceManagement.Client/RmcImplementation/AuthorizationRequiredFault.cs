using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "AuthorizationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = true), XmlType(TypeName = "AuthorizationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IncludeInSchema = false)]
    public class AuthorizationRequiredFault : RequestFault
    {
        public AuthorizationRequiredFault() : this(null)
        {
        }

        public AuthorizationRequiredFault(EndpointReferenceType endpointReference) : base(endpointReference)
        {
        }
    }
}
