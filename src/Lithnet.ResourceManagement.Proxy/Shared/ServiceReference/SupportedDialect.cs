using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "SupportedDialect", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration"), XmlType(AnonymousType = true)]
    public class SupportedDialect
    {
        public SupportedDialect()
            : this(string.Empty)
        {
        }

        public SupportedDialect(string dialect)
        {
            this.Text = dialect;
        }

        [XmlText(DataType = "anyURI")]
        public string Text { get; set; }
    }
}