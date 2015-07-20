using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class TooManyResultsException : Exception
    {
        public TooManyResultsException()
            :base ()
        {
        }

        public TooManyResultsException(string message)
            : base(message)

        { 
        }

        public TooManyResultsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
