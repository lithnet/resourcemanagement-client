using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class NoSuchAttributeException : Exception
    {
        public NoSuchAttributeException():
            base()
        {
        }

        public NoSuchAttributeException(string attributeName)
            : base(string.Format("The attribute '{0}' does not exist on this object", attributeName))
        {
        }

        public NoSuchAttributeException(string attributeName, string objectType)
            : base(string.Format("The attribute '{0}' does not exist on the object type {1}", attributeName, objectType))
        {
        }

        public NoSuchAttributeException(string message, Exception innerException) :
            base(message,innerException)
        {
        }
    }
}
