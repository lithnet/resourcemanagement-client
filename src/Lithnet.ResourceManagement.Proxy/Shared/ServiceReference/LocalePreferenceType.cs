using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(TypeName = "LocalePreference", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement"), XmlRoot(ElementName = "LocalePreference", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false)]
    public class LocalePreferenceType
    {
        private CultureInfo localeField;

        public LocalePreferenceType()
        {
        }

        public LocalePreferenceType(CultureInfo locale, int preferenceValue)
        {
            this.localeField = locale;
            this.PreferenceValue = preferenceValue.ToString(this.localeField);
        }

        public LocaleReferencePropertyType Locale
        {
            get => new LocaleReferencePropertyType(this.localeField);
            set
            {
                LocaleReferencePropertyType type = value;
                if ((type != null) && !string.IsNullOrEmpty(type.Text))
                {
                    this.localeField = CultureInfo.GetCultureInfo(type.Text);
                }
            }
        }

        [XmlElement(DataType = "positiveInteger")]
        public string PreferenceValue { get; set; }
    }
}