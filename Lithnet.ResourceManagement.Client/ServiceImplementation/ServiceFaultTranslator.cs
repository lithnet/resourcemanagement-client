using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ResourceManagement.WebServices.Exceptions;
using Microsoft.ResourceManagement.WebServices.Faults;
using System.Xml;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    internal static class ServiceFaultTranslator
    {
        public static Exception GetExceptionFromFaultMessage(MessageFault fault)
        {
            if (fault.Code == null || fault.Code.SubCode == null)
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

                default:
                    break;
            }

            return new FaultException(fault, fault.GetReaderAtDetailContents().ReadOuterXml());
        }

        public static Exception GetUnwillingToPerformException(MessageFault fault)
        {
            DispatchRequestFailures failure = fault.DeserializeMessageWithPayload<DispatchRequestFailures>();

            if (failure == null || failure.AdministratorDetails == null)
            {
                return new FaultException(fault);
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(failure.AdministratorDetails.FailureMessage);
            builder.AppendLine("Details: " + failure.AdministratorDetails.AdditionalTextDetails);
            builder.AppendLine("Source: " + failure.AdministratorDetails.DispatchRequestFailureSource);
            throw new UnwillingToPerformException(builder.ToString());
        }

        public static Exception GetInvalidRepresentationException(MessageFault fault)
        {
            RepresentationFailures representationFailures = fault.DeserializeMessageWithPayload<RepresentationFailures>();

            if (representationFailures == null)
            {
                return new FaultException(fault);
            }

            if (representationFailures.AttributeRepresentationFailures == null || representationFailures.AttributeRepresentationFailures.Length == 0)
            {
                if (representationFailures.MessageRepresentationFailures != null && representationFailures.MessageRepresentationFailures.Length > 0)
                {
                    return new InvalidRepresentationException(representationFailures.MessageRepresentationFailures[0].MessageFailureCode);
                }
                else
                {
                    return new FaultException(fault);
                }
            }

            AttributeRepresentationFailure failure = representationFailures.AttributeRepresentationFailures[0];

            return new InvalidRepresentationException(failure.AttributeFailureCode, failure.AttributeType, failure.AttributeValue);
        }

        public static Exception GetPermissionDeniedException(MessageFault fault)
        {
            RequestFailures failures = fault.DeserializeMessageWithPayload<RequestFailures>();

            if (failures == null || failures.RequestAdministratorDetails == null)
            {
                return new PermissionDeniedException(fault.Reason.ToString());
            }

            string[] failedAttributeNames = failures.RequestAdministratorDetails.FailedAttributes == null ? null : failures.RequestAdministratorDetails.FailedAttributes.AttributeType;
            string attributes = string.Empty;

            if (failedAttributeNames != null)
            {
                attributes = failures.RequestAdministratorDetails.FailedAttributes == null ? null : failures.RequestAdministratorDetails.FailedAttributes.AttributeType.ToCommaSeparatedString();
            }

            if (failures.RequestAdministratorDetails.RequestFailureSource == RequestFailureSource.ResourceIsMissing)
            {
                return new ResourceNotFoundException();
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(fault.Reason.ToString());
            builder.AppendLine(fault.Code.Name);
            builder.AppendFormat("Source: {0}\n", failures.RequestAdministratorDetails.RequestFailureSource.ToString());

            if (attributes != null)
            {
                builder.AppendFormat("Attributes: {0}\n", attributes);
            }

            PermissionDeniedException ex = new PermissionDeniedException(failures.RequestAdministratorDetails.RequestFailureSource, null, failedAttributeNames);
            return new PermissionDeniedException(builder.ToString(), ex);
        }
    }
}
