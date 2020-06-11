using System;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// The exception that is thrown when more than the expected number of search results was returned
    /// </summary>
    [Serializable]
    public class TooManyResultsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the TooManyResultsException class
        /// </summary>
        public TooManyResultsException()
            :base ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TooManyResultsException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        public TooManyResultsException(string message)
            : base(message)

        { 
        }

        /// <summary>
        /// Initializes a new instance of the TooManyResultsException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        public TooManyResultsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
