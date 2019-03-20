using System;
using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    public abstract class RequestFault
    {
        protected RequestFault(EndpointReferenceType endpointReference)
        {
            this.EndpointReference = endpointReference;
        }

        [XmlElement(ElementName = "EndpointReference", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        public EndpointReferenceType EndpointReference { get; set; }
    }
}