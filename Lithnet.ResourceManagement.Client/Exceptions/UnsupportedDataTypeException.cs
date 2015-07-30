using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// The exception that is thrown when an attributeName is made to set an attributeName value that cannot be converted into the native data type for that attributeName
    /// </summary>
    [Serializable]
    public class UnsupportedDataTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the UnsupportedDataTypeException class
        /// </summary>
        public UnsupportedDataTypeException():
            base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnsupportedDataTypeException class
        /// </summary>
        /// <param name="expected">The data type that was expected</param>
        /// <param name="actual">The data type that was provided</param>
        public UnsupportedDataTypeException(Type expected, Type actual)
            : base(string.Format("Cannot convert data from {0} to {1}", actual.Name, expected.Name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnsupportedDataTypeException class
        /// </summary>
        /// <param name="expected">The data type that was expected</param>
        /// <param name="actual">The data type that was provided</param>
        public UnsupportedDataTypeException(AttributeType expected, Type actual)
            : base(string.Format("Type {0} is not compatible with attributeName data type {0}", actual.Name, expected.ToString()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnsupportedDataTypeException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        public UnsupportedDataTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the UnsupportedDataTypeException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        public UnsupportedDataTypeException(string message, Exception innerException):
            base(message,innerException)
        {
        }
    }
}
