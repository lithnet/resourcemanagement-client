namespace Lithnet.ResourceManagement.Client
{
    using System;
    using Microsoft.ResourceManagement.WebServices.Exceptions;
    using Microsoft.ResourceManagement.WebServices.Faults;

    /// <summary>
    /// An exception thrown by the resource management service when permission to perform an action is denied
    /// </summary>
    [Serializable]
    public class PermissionDeniedException : ResourceManagementException
    {
        /// <summary>
        /// The string used to construct the exception message
        /// </summary>
        private const string messageFormat = "Failure message: {0}\nAttributes: {1}\nSource: {2}\n Additional details: {3}\nCorrelation ID:{4}";

        /// <summary>
        /// The internal representation of the failure
        /// </summary>
        private RequestAdministratorDetails details;

        /// <summary>
        /// Gets the message describing the failure
        /// </summary>
        public string FailureMessage
        {
            get
            {
                return this.details.FailureMessage;
            }
        }

        /// <summary>
        /// Gets additional details provided by the server regarding the failure
        /// </summary>
        public string AdditionalTextDetails
        {
            get
            {
                return this.details.AdditionalTextDetails;
            }
        }

        /// <summary>
        /// Gets the source of the failure
        /// </summary>
        public RequestFailureSource FailureSource
        {
            get
            {
                return this.details.RequestFailureSource;
            }
        }

        /// <summary>
        /// Gets a list of attributes that caused the failure
        /// </summary>
        public string[] Attributes
        {
            get
            {
                return this.details.FailedAttributes?.AttributeType;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PermissionDeniedException class
        /// </summary>
        /// <param name="failure">The object containing the details of the failure</param>
        public PermissionDeniedException(RequestFailures failure)
            : base(GetMessage(failure, failure.CorrelationIdentifier), failure.CorrelationIdentifier)
        {
            this.details = failure.RequestAdministratorDetails;
        }

        /// <summary>
        /// Gets a string representation of the exception
        /// </summary>
        /// <param name="failure">The object containing details of the failure</param>
        /// <param name="correlationID">The ID of the failed request</param>
        /// <returns>A string containing the details of the exception</returns>
        private static string GetMessage(RequestFailures failure, string correlationID)
        {
            string attributes = string.Empty;

            RequestAdministratorDetails details = failure.RequestAdministratorDetails;

            if (details != null)
            {
                if (details.FailedAttributes != null)
                {
                    attributes = string.Join(",", details.FailedAttributes);
                }

                return string.Format(
                    PermissionDeniedException.messageFormat,
                    details.FailureMessage,
                    attributes,
                    details.RequestFailureSource,
                    details.AdditionalTextDetails,
                    correlationID
                    );
            }
            else
            {
                return string.Format("A generic permission denied failure was returned from the server that did not contain detailed information. Correlation ID: {0}", correlationID ?? string.Empty);
            }
        }
    }
}
