using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client
{
    [ServiceContract(Namespace = "http://schemas.xmlsoap.org/ws/2004/09/enumeration", ConfigurationName = "Search")]
    internal interface ISearch
    {
        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/Enumerate", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/EnumerateResponse")]
        [FaultContract(typeof(CannotProcessFilterFault), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "CannotProcessFilter")]
        [FaultContract(typeof(SupportedDialect), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "FilterDialectRequestedUnavailable")]
        [FaultContract(typeof(RepresentationFailures), Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault", Name = "InvalidRepresentation", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer")]
        [FaultContract(typeof(InvalidExpirationTimeFault), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "InvalidExpirationTime")]
        [FaultContract(typeof(UnsupportedExpirationTypeFault), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "UnsupportedExpirationType")]
        [FaultContract(typeof(DispatchRequestFailures), Action = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess/fault", Name = "UnwillingToPerform", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
        [FaultContract(typeof(EndpointFailures), Action = "http://www.w3.org/2005/08/addressing/fault", Name = "EndpointUnavailable", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        [FaultContract(typeof(RequestFailures), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "PermissionDenied", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        Task<Message> EnumerateAsync(Message request);

        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/Pull", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/PullResponse")]
        [FaultContract(typeof(RepresentationFailures), Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault", Name = "InvalidRepresentation", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer")]
        [FaultContract(typeof(InvalidEnumerationContextFault), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "InvalidEnumerationContext")]
        [FaultContract(typeof(DispatchRequestFailures), Action = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess/fault", Name = "UnwillingToPerform", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
        [FaultContract(typeof(EndpointFailures), Action = "http://www.w3.org/2005/08/addressing/fault", Name = "EndpointUnavailable", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        [FaultContract(typeof(RequestFailures), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "PermissionDenied", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        Task<Message> PullAsync(Message request);

        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/Renew", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/RenewResponse")]
        [FaultContract(typeof(RequestFailures), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "PermissionDenied", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(UnsupportedExpirationTypeFault), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "UnsupportedExpirationType")]
        [FaultContract(typeof(DispatchRequestFailures), Action = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess/fault", Name = "UnwillingToPerform", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
        [FaultContract(typeof(RepresentationFailures), Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault", Name = "InvalidRepresentation", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer")]
        [FaultContract(typeof(EndpointFailures), Action = "http://www.w3.org/2005/08/addressing/fault", Name = "EndpointUnavailable", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        [FaultContract(typeof(InvalidEnumerationContextFault), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "InvalidEnumerationContext")]
        [FaultContract(typeof(InvalidExpirationTimeFault), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "InvalidExpirationTime")]
        Task<Message> RenewAsync(Message request);

        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/GetStatus", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/GetStatusResponse")]
        [FaultContract(typeof(EndpointFailures), Action = "http://www.w3.org/2005/08/addressing/fault", Name = "EndpointUnavailable", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        [FaultContract(typeof(RepresentationFailures), Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault", Name = "InvalidRepresentation", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer")]
        [FaultContract(typeof(InvalidEnumerationContextFault), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "InvalidEnumerationContext")]
        [FaultContract(typeof(RequestFailures), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "PermissionDenied", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(DispatchRequestFailures), Action = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess/fault", Name = "UnwillingToPerform", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
        Task<Message> GetStatusAsync(Message request);

        [OperationContract(Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/Release", ReplyAction = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/ReleaseResponse")]
        [FaultContract(typeof(RequestFailures), Action = "http://schemas.microsoft.com/2006/11/ResourceManagement/fault", Name = "PermissionDenied", Namespace = "http://schemas.microsoft.com/2006/11/ResourceManagement")]
        [FaultContract(typeof(EndpointFailures), Action = "http://www.w3.org/2005/08/addressing/fault", Name = "EndpointUnavailable", Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing")]
        [FaultContract(typeof(InvalidEnumerationContextFault), Action = "http://schemas.xmlsoap.org/ws/2004/09/enumeration/fault", Name = "InvalidEnumerationContext")]
        [FaultContract(typeof(DispatchRequestFailures), Action = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess/fault", Name = "UnwillingToPerform", Namespace = "http://schemas.microsoft.com/2006/11/IdentityManagement/DirectoryAccess")]
        [FaultContract(typeof(RepresentationFailures), Action = "http://schemas.xmlsoap.org/ws/2004/09/transfer/fault", Name = "InvalidRepresentation", Namespace = "http://schemas.xmlsoap.org/ws/2004/09/transfer")]
        Task<Message> ReleaseAsync(Message request);
    }
}
