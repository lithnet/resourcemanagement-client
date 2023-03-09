using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "Filter", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", IsNullable = false), XmlType(TypeName = "Filter", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration")]
    public class FilterType
    {
        public FilterType()
        {
            this.Dialect = "http://schemas.microsoft.com/2006/11/XPathFilterDialect";
        }

        public FilterType(string text)
            : this()
        {
            this.Text = text;
        }

        internal FilterType(string dialect, string text)
        {
            this.Dialect = dialect;
            this.Text = text;
        }

        [XmlAttribute(AttributeName = "Dialect", DataType = "anyURI")]
        public string Dialect { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}