namespace Lithnet.ResourceManagement.Client
{
    using System;
    using Microsoft.ResourceManagement.WebServices.Exceptions;
    using Microsoft.ResourceManagement.WebServices.Faults;

    /// <summary>
    /// An exception thrown by the resource management service when an invalid request is submitted
    /// </summary>
    [Serializable]
    public class UnwillingToPerformException : ResourceManagementException
    {
        /// <summary>
        /// The string used to construct the exception message
        /// </summary>
        private const string messageFormat = "Failure Message: {0}\nFailure Source: {1}\nDetails: {2}\nCorrelation ID:{3}";

        /// <summary>
        /// The internal representation of the failure
        /// </summary>
        private DispatchRequestFailures failure;

        /// <summary>
        /// Gets the message describing the failure
        /// </summary>
        public string FailureMessage
        {
            get
            {
                return this.failure.AdministratorDetails.FailureMessage;
            }
        }

        /// <summary>
        /// Gets additional details provided by the server regarding the failure
        /// </summary>
        public string AdditionalTextDetails
        {
            get
            {
                return this.failure.AdministratorDetails.AdditionalTextDetails;
            }
        }

        /// <summary>
        /// Gets the source of the failure
        /// </summary>       
        public DispatchRequestFailureSource FailureSource
        {
            get
            {
                return this.failure.AdministratorDetails.DispatchRequestFailureSource;
            }
        }

        /// <summary>
        /// Initializes a new instance of the UnwillingToPerformException class
        /// </summary>
        /// <param name="failure">The object containing the details of the failure</param>
        public UnwillingToPerformException(DispatchRequestFailures failure)
            :base(GetMessage(failure, failure.CorrelationIdentifier),  failure.CorrelationIdentifier)
        {
            this.failure = failure;
        }

        /// <summary>
        /// Gets a string representation of the exception
        /// </summary>
        /// <param name="failure">The object containing details of the failure</param>
        /// <param name="correlationID">The ID of the failed request</param>
        /// <returns>A string containing the details of the exception</returns>
        private static string GetMessage(DispatchRequestFailures failure, string correlationID)
        {
            return string.Format(
                UnwillingToPerformException.messageFormat,
                failure.AdministratorDetails?.FailureMessage,
                failure.AdministratorDetails?.DispatchRequestFailureSource,
                failure.AdministratorDetails?.AdditionalTextDetails,
                failure.CorrelationIdentifier);
        }
    }
}
