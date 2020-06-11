using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [XmlType(AnonymousType = true), XmlRoot(ElementName = "BaseObjectSearchRequest", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess", IsNullable = false)]
    public class Get
    {
        public override string ToString()
        {
            if (this.Expressions != null)
            {
                return string.Join(",", this.Expressions);
            }

            return string.Empty;
        }

        [XmlAttribute(AttributeName = "Dialect", DataType = "anyURI")]
        public string Dialect { get; set; }

        [XmlElement(ElementName = "AttributeType")]
        public string[] Expressions { get; set; }
    }
}