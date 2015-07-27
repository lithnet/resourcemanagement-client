using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// The exception that is thrown when an resource could not be found
    /// </summary>
    [Serializable]
    public class ResourceNotFound : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ResourceNotFound class
        /// </summary>
        public ResourceNotFound()
            : base(string.Format("The specified object was not found"))
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceNotFound class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        public ResourceNotFound(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceNotFound class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        public ResourceNotFound(string message, Exception innerException) :
            base(message,innerException)
        {
        }
    }
}
