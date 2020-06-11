using System;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// An exception that is thrown when the LDAP filter encounters an unrecoverable error
    /// </summary>
    [Serializable]
    public class LdapFilterParserException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the LdapFilterParserException class
        /// </summary>
        internal LdapFilterParserException()
            :base ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LdapFilterParserException class
        /// </summary>
        /// <param name="error">The description of the parser error</param>
        internal LdapFilterParserException(TokenError error)
            : base (error.ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the LdapFilterParserException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        internal LdapFilterParserException(string message)
            :base (message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LdapFilterParserException class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that was the cause of this exception</param>
        internal LdapFilterParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
