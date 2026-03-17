using System;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlInclude(typeof(PutFragmentType)), XmlType(TypeName = "AttributeTypeAndValue", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess"), XmlRoot(ElementName = "AttributeTypeAndValue", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess", IsNullable = false)]
    public class FragmentType
    {
        private const int defaultLocaleKey = 0x7f;

        public FragmentType()
        {
        }

        public FragmentType(string expression, object value) : this(expression, value, defaultLocaleKey)
        {
        }

        public FragmentType(string expression, object value, int localeKey)
        {
            this.Expression = expression;
            this.Value = value;
            this.LocaleKey = localeKey;
        }

        public FragmentType(string expression, string elementName, string typeName, bool multivalued, object value)
        {
            XmlDocument document = new XmlDocument();
            if (value != null)
            {
                string str = value.ToString();

                XmlElement newChild = document.CreateElement("AttributeValue", "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess");
                document.AppendChild(newChild);
                XmlElement element2 = document.CreateElement("rm", elementName, "http://schemas.microsoft.com/2006/11/ResourceManagement");
                element2.InnerText = str;
                newChild.AppendChild(element2);


                this.Expression = expression;
                this.Value = newChild.CloneNode(true);
                return;
            }

            this.Expression = expression;
        }

        public FragmentType(string expression, string elementName, string typeName, bool multivalued, object[] value)
        {
            if (value == null)
            {
                this.Expression = expression;
            }
            else
            {
                XmlDocument document = new XmlDocument();
                XmlElement newChild = null;

                newChild = document.CreateElement("AttributeValue", "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess");
                document.AppendChild(newChild);
                foreach (object t in value)
                {
                    if (t != null)
                    {
                        string str = t.ToString();

                        XmlElement element2 = document.CreateElement("rm", elementName, "http://schemas.microsoft.com/2006/11/ResourceManagement");
                        element2.InnerText = str;
                        newChild.AppendChild(element2);
                    }
                }

                this.Expression = expression;
                this.Value = newChild.CloneNode(true);
            }
        }

        public FragmentType(string expression, string elementName, string typeName, bool multivalued, object value, UniqueIdentifier targetIdentifier) : this(expression, elementName, typeName, multivalued, value)
        {
            if (targetIdentifier != null)
            {
                this.TargetIdentifier = targetIdentifier?.ToString();
            }
        }

        [XmlElement(ElementName = "AttributeType")]
        public string Expression { get; set; }

        [XmlIgnore]
        public int LocaleKey { get; set; }

        [XmlAttribute(AttributeName = "TargetIdentifier")]
        public string TargetIdentifier { get; set; }

        [XmlAnyElement(Name = "AttributeValue")]
        public object Value { get; set; }
    }
}
