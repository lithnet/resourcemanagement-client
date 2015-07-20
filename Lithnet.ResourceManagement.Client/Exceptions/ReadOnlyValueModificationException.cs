using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class ReadOnlyValueModificationException : Exception
    {
        public ReadOnlyValueModificationException()
            : base()
        {
        }

        public ReadOnlyValueModificationException(AttributeTypeDefinition attribute)
            : base(string.Format("An attempt was made to modify the read only attribute {0}", attribute.SystemName))
        {
        }

        public ReadOnlyValueModificationException(string message)
            : base(message)
        {
        }

        public ReadOnlyValueModificationException(string message, Exception innerException)
            : base(message, innerException)
        { 
        }
    }
}
