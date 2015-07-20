using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.ResourceManagement.WebServices.Exceptions;
using Microsoft.ResourceManagement.WebServices.Faults;

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

                default:
                    break;
            }

            return new FaultException(fault);
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
    }
}
