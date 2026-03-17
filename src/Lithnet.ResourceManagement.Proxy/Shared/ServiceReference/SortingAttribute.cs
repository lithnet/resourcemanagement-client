using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = false), XmlType(Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
    public class SortingAttribute
    {
        public SortingAttribute()
        {
        }

        public SortingAttribute(string attributeName)
            : this(attributeName, true)
        {
        }

        public SortingAttribute(string attributeName, bool ascending)
        {
            this.AttributeName = attributeName;
            this.Ascending = ascending;
        }

        [XmlAttribute]
        public bool Ascending { get; set; }

        [XmlText]
        public string AttributeName { get; set; }
    }
}