using System;

namespace Lithnet.ResourceManagement.Client
{

    [Serializable]
    public class RmcProxyConnectionException : Exception
    {
        public RmcProxyConnectionException() { }

        public RmcProxyConnectionException(string message) : base(message) { }

        public RmcProxyConnectionException(string message, Exception inner) : base(message, inner) { }

        protected RmcProxyConnectionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
