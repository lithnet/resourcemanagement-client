using System.Xml.Serialization;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Class to support serialization of approval response data
    /// </summary>
    [XmlRoot(ElementName = "ApprovalResponse", Namespace = Namespaces.ResourceManagement)]
    public class ApprovalResponse
    {
        /// <summary>
        /// Gets or sets the object ID of the approval request
        /// </summary>
        [XmlElement(ElementName = "Approval", Namespace = Namespaces.ResourceManagement)]
        public string Approval { get; set; }
        
        /// <summary>
        /// Gets or sets the approval decision ("Approved" or "Rejected")
        /// </summary>
        [XmlElement(ElementName = "Decision", Namespace = Namespaces.ResourceManagement)]
        public string Decision { get; set; }

        /// <summary>
        /// Gets or sets the object type
        /// </summary>
        [XmlElement(ElementName = "ObjectType", Namespace = Namespaces.ResourceManagement)]
        public string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the approval reason
        /// </summary>
        [XmlElement(ElementName = "Reason", Namespace = Namespaces.ResourceManagement)]
        public string Reason { get; set; }

        /// <summary>
        /// Initializes a new instance of the approval response class
        /// </summary>
        public ApprovalResponse()
        {
            this.ObjectType = ObjectTypeNames.ApprovalResponse;
        }
    }
}
