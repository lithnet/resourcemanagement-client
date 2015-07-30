using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// The exception that is thrown when an attempt to modify a read-only attributeName is made
    /// </summary>
    [Serializable]
    public class ReadOnlyValueModificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ReadOnlyValueModificationException class
        /// </summary>
        public ReadOnlyValueModificationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyValueModificationException class
        /// </summary>
        /// <param name="attributeName">The attributeName that was illegally modified</param>
        public ReadOnlyValueModificationException(AttributeTypeDefinition attribute)
            : base(string.Format("An attempt was made to modify the read only attributeName {0}", attribute.SystemName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyValueModificationException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        public ReadOnlyValueModificationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyValueModificationException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        public ReadOnlyValueModificationException(string message, Exception innerException)
            : base(message, innerException)
        { 
        }
    }
}
