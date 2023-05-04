using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    [ServiceContract(Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer", ConfigurationName = "ResourceFactory")]
    internal interface IResourceFactory
    {
        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Create", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/transfer/CreateResponse")]
        [FaultContract(typeof(DataRequiredFault), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "DataRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(RepresentationFailures), Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault", Name = "InvalidRepresentation")]
        [FaultContract(typeof(AuthorizationRequiredFault), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "AuthorizationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(AuthenticationRequiredFault), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "AuthenticationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(DispatchRequestFailures), Action = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess/fault", Name = "UnwillingToPerform", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
        [FaultContract(typeof(RequestFailures), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "PermissionDenied", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(EndpointFailures), Action = "http://www.w3.org/2005/08/addressing/fault", Name = "EndpointUnavailable", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        Task<Message> CreateAsync(Message request);
    }
}
