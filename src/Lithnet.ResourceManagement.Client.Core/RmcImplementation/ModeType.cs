using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [XmlType(TypeName = "ModeType", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
    public enum ModeType
    {
        [XmlEnum(Name = "Add")]
        Insert = 1,
        [XmlEnum(Name = "Replace")]
        Modify = 0,
        [XmlEnum(Name = "Delete")]
        Remove = 2
    }
}
