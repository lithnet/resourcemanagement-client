using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// The exception that is thrown when an attribute is requested that does not exist on a particular object class
    /// </summary>
    [Serializable]
    public class NoSuchAttributeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NoSuchAttributeException class
        /// </summary>
        public NoSuchAttributeException():
            base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the NoSuchAttributeException class
        /// </summary>
        /// <param name="attributeName">The name of the attribute that was not found</param>
        public NoSuchAttributeException(string attributeName)
            : base(string.Format("The attribute '{0}' does not exist on this object", attributeName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the NoSuchAttributeException class
        /// </summary>
        /// <param name="attributeName">The name of the attribute that was not found</param>
        /// <param name="objectType">The name of the object type that did not contain the attribute</param>
        public NoSuchAttributeException(string attributeName, string objectType)
            : base(string.Format("The attribute '{0}' does not exist on the object type {1}", attributeName, objectType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the NoSuchAttributeException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        public NoSuchAttributeException(string message, Exception innerException) :
            base(message ,innerException)
        {
        }
    }
}
