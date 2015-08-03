using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// The exception that is thrown when an object type is requested that does not exist in the schema
    /// </summary>
    [Serializable]
    public class NoSuchObjectTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NoSuchObjectTypeException class
        /// </summary>
        public NoSuchObjectTypeException():
            base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the NoSuchObjectTypeException class
        /// </summary>
        /// <param name="attributeName">The name of the object type that was not found</param>
        public NoSuchObjectTypeException(string objectTypeName)
            : base(string.Format("The object type '{0}' does not exist in the schema", objectTypeName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the NoSuchObjectTypeException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        public NoSuchObjectTypeException(string message, Exception innerException) :
            base(message ,innerException)
        {
        }
    }
}
