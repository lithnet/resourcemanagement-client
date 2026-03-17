using System.Collections;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    internal class ConnectionModeSources : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return ConnectionMode.LocalProxy;
            //if (!FrameworkUtilities.IsFramework)
            {
                yield return ConnectionMode.RemoteProxy;
            }
            yield return ConnectionMode.DirectNetTcp;
#if NETFRAMEWORK
            yield return ConnectionMode.DirectWsHttp;
#endif
        }
    }

    internal class ConnectionModeSourcesApprovals : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return ConnectionMode.LocalProxy;
            if (!FrameworkUtilities.IsFramework)
            {
                yield return ConnectionMode.RemoteProxy;
            }
#if NETFRAMEWORK
            yield return ConnectionMode.DirectWsHttp;
#endif
        }
    }
}
