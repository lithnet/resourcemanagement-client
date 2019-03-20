using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable, XmlRoot(ElementName = "AnonymousInteractionRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IsNullable = true), XmlType(TypeName = "AnonymousInteractionRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement", IncludeInSchema = false)]
    public class AnonymousInteractionRequiredFault
    {
        public AnonymousInteractionRequiredFault()
            : this(null)
        {
        }

        public AnonymousInteractionRequiredFault(List<InteractiveWorkflowAddress> endpointAddressList)
        {
            this.EndpointAddresses = endpointAddressList;
        }

        [XmlElement(ElementName = "EndpointAddresses")]
        public List<InteractiveWorkflowAddress> EndpointAddresses { get; set; }
    }
}