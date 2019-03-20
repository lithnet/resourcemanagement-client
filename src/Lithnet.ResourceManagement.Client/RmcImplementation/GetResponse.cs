using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "BaseObjectSearchResponse", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess", IsNullable = false), XmlType(AnonymousType = true)]
    public class GetResponse
    {

        public GetResponse()
        {
        }

        [XmlAnyElement(Name = "PartialAttribute")]
        public object[] Results { get; set; }
    }
}