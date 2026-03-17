using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlType(TypeName = "ResourceReferencePropertyType", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement"), XmlRoot(ElementName = "ResourceReferenceProperty", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false)]
    public class ResourceReferencePropertyType
    {
        public ResourceReferencePropertyType()
        {
        }

        public ResourceReferencePropertyType(UniqueIdentifier objectId)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            this.Text = objectId.ToString();
        }

        [XmlText]
        public string Text { get; set; }
    }
}