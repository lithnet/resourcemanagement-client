namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// An exception thrown by the resource management service when an unsupported request message is submitted
    /// </summary>
    public class MessageRepresentationFailureException : InvalidRepresentationException
    {
        /// <summary>
        /// The string used to construct the exception message
        /// </summary>
        private const string messageFormat = "Failure message: {0}\nFailure code: {1}\n Additional details: {2}\nCorrelation ID:{3}";

        /// <summary>
        /// The internal representation of the failure
        /// </summary>
        private MessageRepresentationFailure failure;

        /// <summary>
        /// Gets additional details provided by the server regarding the failure
        /// </summary>
        public string AdditionalTextDetails
        {
            get
            {
                return this.failure.AdditionalTextDetails;
            }
        }

        /// <summary>
        /// Gets the code representing the type of the failure
        /// </summary>
        public MessageFailureCode MessageFailureCode
        {
            get
            {
                return this.failure.MessageFailureCode;
            }
        }

        /// <summary>
        /// Gets the message describing the failure
        /// </summary>
        public string FailureMessage
        {
            get
            {
                return this.failure.FailureMessage;
            }
        }

        /// <summary>
        /// Initializes a new instance of the MessageRepresentationFailureException class
        /// </summary>
        /// <param name="failure">The object containing the details of the failure</param>
        /// <param name="correlationID">The ID of the failed request </param>
        public MessageRepresentationFailureException(MessageRepresentationFailure failure, string correlationID)
            : base(GetMessage(failure, correlationID), correlationID)
        {
            this.failure = failure;
        }

        /// <summary>
        /// Gets a string representation of the exception
        /// </summary>
        /// <param name="failure">The object containing details of the failure</param>
        /// <param name="correlationID">The ID of the failed request</param>
        /// <returns>A string containing the details of the exception</returns>
        private static string GetMessage(MessageRepresentationFailure failure, string correlationID)
        {
            return string.Format(
                MessageRepresentationFailureException.messageFormat,
                failure.FailureMessage,
                failure.MessageFailureCode,
                failure.AdditionalTextDetails,
                correlationID);
        }
    }
}
