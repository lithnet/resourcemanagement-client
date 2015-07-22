using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client
{
    public class UnsupportedDataTypeException : Exception
    {
        public UnsupportedDataTypeException():
            base()
        {
        }

        public UnsupportedDataTypeException(Type expected, Type actual)
            : base(string.Format("Cannot convert data from {0} to {1}", actual.Name, expected.Name))
        {
        }

        public UnsupportedDataTypeException(AttributeType expected, Type actual)
            : base(string.Format("Type {0} is not compatible with attribute data type {0}", actual.Name, expected.ToString()))
        {
        }

        public UnsupportedDataTypeException(string message)
            : base(message)
        {
        }

        public UnsupportedDataTypeException(string message, Exception innerException):
            base(message,innerException)
        {
        }
    }
}
