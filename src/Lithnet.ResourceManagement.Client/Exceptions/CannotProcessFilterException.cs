using System.ServiceModel.Channels;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace Lithnet.ResourceManagement.Client
{
    using System;
    using Microsoft.ResourceManagement.WebServices.Exceptions;
    using Microsoft.ResourceManagement.WebServices.Faults;

    /// <summary>
    /// An exception thrown by the resource management service when an invalid request is submitted
    /// </summary>
    [Serializable]
    public class CannotProcessFilterException : ResourceManagementException
    {
        /// <summary>
        /// The string used to construct the exception message
        /// </summary>
        private const string messageFormat = "Failure Message: {0}";

        /// <summary>
        /// Initializes a new instance of the UnwillingToPerformException class
        /// </summary>
        /// <param name="failure">The object containing the details of the failure</param>
        public CannotProcessFilterException(MessageFault fault)
            :base(GetMessage(fault))
        {
        }

        /// <summary>
        /// Gets a string representation of the exception
        /// </summary>
        /// <param name="failure">The object containing details of the failure</param>
        /// <param name="correlationID">The ID of the failed request</param>
        /// <returns>A string containing the details of the exception</returns>
        private static string GetMessage(MessageFault fault)
        {
            return string.Format(
                CannotProcessFilterException.messageFormat,
                fault.Reason.ToString());
        }
    }
}
