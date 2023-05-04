using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    [ServiceContract(Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer", ConfigurationName = "Resource")]
    internal interface IResource
    {
        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/transfer/GetResponse")]
        [FaultContract(typeof(RepresentationFailures), Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault", Name = "InvalidRepresentation")]
        [FaultContract(typeof(RequestFailures), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "PermissionDenied", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(DispatchRequestFailures), Action = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess/fault", Name = "UnwillingToPerform", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
        [FaultContract(typeof(EndpointFailures), Action = "http://www.w3.org/2005/08/addressing/fault", Name = "EndpointUnavailable", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        Task<Message> GetAsync(Message request);

        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Put", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/transfer/PutResponse")]
        [FaultContract(typeof(AnonymousInteractionRequiredFault), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "AnonymousInteractionRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(RepresentationFailures), Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault", Name = "InvalidRepresentation")]
        [FaultContract(typeof(AuthenticationRequiredFault), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "AuthenticationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(AuthorizationRequiredFault), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "AuthorizationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(DispatchRequestFailures), Action = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess/fault", Name = "UnwillingToPerform", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
        [FaultContract(typeof(EndpointFailures), Action = "http://www.w3.org/2005/08/addressing/fault", Name = "EndpointUnavailable", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        [FaultContract(typeof(RequestFailures), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "PermissionDenied", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        Task<Message> PutAsync(Message request);

        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/Delete", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/transfer/DeleteResponse")]
        [FaultContract(typeof(EndpointFailures), Action = "http://www.w3.org/2005/08/addressing/fault", Name = "EndpointUnavailable", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        [FaultContract(typeof(AuthorizationRequiredFault), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "AuthorizationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(DispatchRequestFailures), Action = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess/fault", Name = "UnwillingToPerform", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
        [FaultContract(typeof(AuthenticationRequiredFault), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "AuthenticationRequiredFault", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(RequestFailures), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "PermissionDenied", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(RepresentationFailures), Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault", Name = "InvalidRepresentation")]
        Task<Message> DeleteAsync(Message request);
    }
}
