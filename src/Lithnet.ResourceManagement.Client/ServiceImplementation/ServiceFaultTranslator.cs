using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ResourceManagement.WebServices.Exceptions;
using Microsoft.ResourceManagement.WebServices.Faults;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace Lithnet.ResourceManagement.Client
{
    internal static class ServiceFaultTranslator
    {
        public static Exception GetExceptionFromFaultMessage(MessageFault fault)
        {
            if (fault.Code?.SubCode == null)
            {
                return new FaultException(fault);
            }
            
            switch (fault.Code.SubCode.Name)
            {
                case "InvalidRepresentation":
                    return GetInvalidRepresentationException(fault);

                case "PermissionDenied":
                    return GetPermissionDeniedException(fault);

                case "UnwillingToPerform":
                    return GetUnwillingToPerformException(fault);

                case "AuthorizationRequiredFault":
                    return GetAuthorizationRequiredException(fault);

                default:
                    break;
            }
            
            return new FaultException(fault, fault.GetReaderAtDetailContents().ReadOuterXml());
        }

        public static Exception GetUnwillingToPerformException(MessageFault fault)
        {
            DispatchRequestFailures failure = fault.DeserializeMessageWithPayload<DispatchRequestFailures>();

            if (failure?.AdministratorDetails == null)
            {
                return new FaultException(fault);
            }
            
            throw new UnwillingToPerformException(failure);
        }

        public static Exception GetInvalidRepresentationException(MessageFault fault)
        {
            RepresentationFailures failure = fault.DeserializeMessageWithPayload<RepresentationFailures>();

            if (failure == null)
            {
                return new FaultException(fault);
            }

            return InvalidRepresentationException.GetException(failure);
        }

        public static Exception GetPermissionDeniedException(MessageFault fault)
        {
            RequestFailures failure = fault.DeserializeMessageWithPayload<RequestFailures>();

            if (failure == null)
            {
                return new FaultException(fault);
            }

            if (failure.RequestAdministratorDetails?.RequestFailureSource == RequestFailureSource.ResourceIsMissing)
            {
                return new ResourceNotFoundException();
            }

            return new PermissionDeniedException(failure);
        }

        public static Exception GetAuthorizationRequiredException(MessageFault fault)
        {
            AuthorizationRequiredFault failure = fault.DeserializeMessageWithPayload<AuthorizationRequiredFault>();

            if (failure == null)
            {
                return new FaultException(fault);
            }

            return new AuthorizationRequiredException(failure);
        }
    }
}
