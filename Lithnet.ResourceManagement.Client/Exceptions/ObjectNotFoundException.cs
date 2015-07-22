using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    [Serializable]
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException()
            : base(string.Format("The specified object was not found"))
        {
        }

        public ObjectNotFoundException(string message)
            : base(message)
        {
        }

        public ObjectNotFoundException(string message, Exception innerException) :
            base(message,innerException)
        {
        }
    }
}
