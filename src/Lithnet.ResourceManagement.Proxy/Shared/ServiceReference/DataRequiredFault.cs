using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "DataRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = true), XmlType(AnonymousType = true)]
    public class DataRequiredFault
    {
        public DataRequiredFault()
        {
        }

        [XmlElement(ElementName = "InformationToClient")]
        public string InformationToClient { get; set; }

        [XmlElement(ElementName = "RequestedObjectTypeSchema")]
        public string RequestedObjectTypeSchema { get; set; }
    }
}