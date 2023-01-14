using System;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(AnonymousType = true), XmlRoot(ElementName = "ModifyRequest", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess", IsNullable = false)]
    public class Put
    {
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (this.Fragments != null)
            {
                foreach (PutFragmentType type in this.Fragments)
                {
                    object innerXml = type.Value;
                    XmlElement element = innerXml as XmlElement;
                    if ((element != null) && element.HasChildNodes)
                    {
                        innerXml = element.FirstChild.InnerXml;
                    }

                    builder.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1} {2}{3}", new object[] {type.Expression, type.Mode, innerXml, Environment.NewLine});
                }
            }

            return builder.ToString();
        }

        [XmlAttribute(AttributeName = "Dialect", DataType = "anyURI")]
        public string Dialect { get; set; }

        [XmlElement("Change")]
        public PutFragmentType[] Fragments { get; set; }
    }
}