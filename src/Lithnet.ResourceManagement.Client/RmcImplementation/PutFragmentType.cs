using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlInclude(typeof(PutFragmentType)), XmlType(TypeName = "Change", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess"), XmlRoot(ElementName = "Change", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess", IsNullable = false)]
    public class PutFragmentType : FragmentType
    {
        public PutFragmentType()
        {
        }

        public PutFragmentType(string expression, ModeType mode, object value)
            : base(expression, value)
        {
            this.Mode = mode;
        }

        public PutFragmentType(string expression, ModeType mode, string elementName, string typeName, bool multivalued, object value)
            : base(expression, elementName, typeName, multivalued, value)
        {
            this.Mode = mode;
        }

        public PutFragmentType(string expression, ModeType mode, string elementName, string typeName, bool multivalued, object[] value)
            : base(expression, elementName, typeName, multivalued, value)
        {
            this.Mode = mode;
        }

        public PutFragmentType(string expression, ModeType mode, string elementName, string typeName, bool multivalued, object value, UniqueIdentifier targetIdentifier)
            : base(expression, elementName, typeName, multivalued, value, targetIdentifier)
        {
            this.Mode = mode;
        }

        [XmlIgnore]
        public Guid ChangeLogIdentifier { get; set; }

        [XmlIgnore]
        public long ChangeLogSequenceNumber { get; set; }

        [XmlAttribute(AttributeName = "Operation")]
        public ModeType Mode { get; set; }

        [XmlIgnore]
        public long RequestKey { get; set; }

        [XmlIgnore]
        public bool Synchronized { get; set; }
    }
}
