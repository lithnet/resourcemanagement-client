using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Lithnet.ResourceManagement.Client
{
    internal static class ServiceFaultTranslator
    {
        public static Exception GetExceptionFromFaultMessage(MessageFault fault)
        {
            if (fault.Code?.SubCode == null)
            {
                return new FaultException(fault, string.Empty);
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

                case "CannotProcessFilter":
                    return GetCannotProcessFilterException(fault);

                default:
                    break;
            }

            if (fault.HasDetail)
            {
                return new FaultException(fault, fault.GetReaderAtDetailContents().ReadOuterXml());
            }
            else
            {
                return new FaultException(fault, string.Empty);
            }
        }

        public static Exception GetCannotProcessFilterException(MessageFault fault)
        {
            return new CannotProcessFilterException(fault);
        }

        public static Exception GetUnwillingToPerformException(MessageFault fault)
        {
            DispatchRequestFailures failure = fault.DeserializeMessageWithPayload<DispatchRequestFailures>();

            if (failure?.AdministratorDetails == null)
            {
                return new FaultException(fault, string.Empty);
            }

            throw new UnwillingToPerformException(failure);
        }

        public static Exception GetInvalidRepresentationException(MessageFault fault)
        {
            RepresentationFailures failure = fault.DeserializeMessageWithPayload<RepresentationFailures>();

            if (failure == null)
            {
                return new FaultException(fault, string.Empty);
            }

            return InvalidRepresentationException.GetException(failure);
        }

        public static Exception GetPermissionDeniedException(MessageFault fault)
        {
            RequestFailures failure = fault.DeserializeMessageWithPayload<RequestFailures>();

            if (failure == null)
            {
                return new FaultException(fault, string.Empty);
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
                return new FaultException(fault, string.Empty);
            }

            return new AuthorizationRequiredException(failure);
        }
    }
}
