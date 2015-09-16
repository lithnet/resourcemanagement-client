using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class LdapFilterParserException : Exception
    {
        internal LdapFilterParserException()
            :base ()
        {
        }

        internal LdapFilterParserException(TokenError error)
            : base (error.ToString())
        {
        }

        internal LdapFilterParserException(string message)
            :base (message)
        {
        }

        internal LdapFilterParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
