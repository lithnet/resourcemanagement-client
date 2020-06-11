namespace Lithnet.ResourceManagement.Client
{
    using System;

    /// <summary>
    /// An exception thrown by the resource management service when an unsupported request is submitted
    /// </summary>
    [Serializable]
    public class InvalidRepresentationException : ResourceManagementException
    {
        /// <summary>
        /// Initializes a new instance of the AttributeRepresentationFailureException class
        /// </summary>
        /// <param name="message">The message text containing details of the failure</param>
        /// <param name="correlationID">The ID of the failed request </param>
        public InvalidRepresentationException(string message, string correlationID)
            : base(message, correlationID)
        {
        }

        /// <summary>
        /// Gets an exception object that is appropriate for the specified representation failure
        /// </summary>
        /// <param name="failures">The object representing the failure</param>
        /// <returns>This method returns either an AttributeRepresentationFailureException, MessageRepresentationFailureException, or generic InvalidRepresentationException</returns>
        internal static InvalidRepresentationException GetException(RepresentationFailures failures)
        {
            if (failures == null)
            {
                throw new ArgumentNullException(nameof(failures));
            }

            if (failures.AttributeRepresentationFailures != null && failures.AttributeRepresentationFailures.Length > 0)
            {
                return new AttributeRepresentationFailureException(failures.AttributeRepresentationFailures[0], failures.CorrelationIdentifier);
            }

            if (failures.MessageRepresentationFailures != null && failures.MessageRepresentationFailures.Length > 0)
            {
                return new MessageRepresentationFailureException(failures.MessageRepresentationFailures[0], failures.CorrelationIdentifier);
            }

            return new InvalidRepresentationException("A generic representation failure was returned from the server that did not contain detailed information", failures.CorrelationIdentifier);
        }
    }
}
