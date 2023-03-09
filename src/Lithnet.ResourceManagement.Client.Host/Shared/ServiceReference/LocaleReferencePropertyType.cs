using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "Locale", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false), XmlType(TypeName = "LocaleReferencePropertyType", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class LocaleReferencePropertyType
    {
        public LocaleReferencePropertyType()
        {
        }

        public LocaleReferencePropertyType(CultureInfo locale)
        {
            if (locale == null)
            {
                throw new ArgumentNullException(nameof(locale));
            }

            this.Text = locale.ToString();
        }

        [XmlText]
        public string Text { get; set; }
    }
}