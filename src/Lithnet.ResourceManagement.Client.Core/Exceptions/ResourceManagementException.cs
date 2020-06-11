namespace Lithnet.ResourceManagement.Client
{
    using System;

    /// <summary>
    /// Represents an exception thrown by the resource management service
    /// </summary>
    [Serializable]
    public class ResourceManagementException : Exception
    {
        /// <summary>
        /// Gets the correlation ID of the failed request 
        /// </summary>
        public string CorrelationID { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementException class
        /// </summary>
        public ResourceManagementException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        public ResourceManagementException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="correlationID">The ID of the failed request </param>
        public ResourceManagementException(string message, string correlationID)
            : base(message)
        {
            this.CorrelationID = correlationID;
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        public ResourceManagementException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="correlationID">The ID of the failed request </param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        public ResourceManagementException(string message, string correlationID, Exception innerException)
           : base(message, innerException)
        {
            this.CorrelationID = correlationID;
        }
    }
}
