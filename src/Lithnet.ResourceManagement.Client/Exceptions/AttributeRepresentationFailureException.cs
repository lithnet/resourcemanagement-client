namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// An exception that is thrown when an unsupported attempt is made to modify an attribute
    /// </summary>
    public class AttributeRepresentationFailureException : InvalidRepresentationException
    {
        /// <summary>
        /// The string used to construct the exception message
        /// </summary>
        private const string messageFormat = "Failure message: {0}\nAttribute: {1}\nValue: {2}\nFailure code: {3}\n Additional details: {4}\nCorrelation ID:{5}";

        /// <summary>
        /// The internal representation of the failure
        /// </summary>
        private AttributeRepresentationFailure failure;

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
        public AttributeFailureCode AttributeFailureCode
        {
            get
            {
                return this.failure.AttributeFailureCode;
            }
        }

        /// <summary>
        /// Gets the name of the attribute
        /// </summary>
        public string AttributeType
        {
            get
            {
                return this.failure.AttributeType;
            }
        }

        /// <summary>
        /// Gets the value of the attribute
        /// </summary>
        public string AttributeValue
        {
            get
            {
                return this.failure.AttributeValue;
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
        /// Initializes a new instance of the AttributeRepresentationFailureException class
        /// </summary>
        /// <param name="failure">The object containing the details of the failure</param>
        /// <param name="correlationID">The ID of the failed request </param>
        public AttributeRepresentationFailureException(AttributeRepresentationFailure failure, string correlationID)
            : base(AttributeRepresentationFailureException.GetMessage(failure, correlationID),  correlationID)
        {
            this.failure = failure;
        }

        /// <summary>
        /// Gets a string representation of the exception
        /// </summary>
        /// <param name="failure">The object containing details of the failure</param>
        /// <param name="correlationID">The ID of the failed request</param>
        /// <returns>A string containing the details of the exception</returns>
        private static string GetMessage(AttributeRepresentationFailure failure, string correlationID)
        {
            return string.Format(
                AttributeRepresentationFailureException.messageFormat,
                failure.FailureMessage,
                failure.AttributeType, 
                failure.AttributeValue, 
                failure.AttributeFailureCode,
                failure.AdditionalTextDetails, correlationID);
        }
    }
}
