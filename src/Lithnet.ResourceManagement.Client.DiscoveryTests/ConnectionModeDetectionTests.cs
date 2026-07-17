using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.DiscoveryTests
{
    /// <summary>
    /// Guards the Auto connection-mode contract: on Windows, the embedded proxy host is always
    /// available (an installed host is preferred, and the embedded copy is extracted otherwise),
    /// so Auto must offer LocalProxy ahead of RemoteProxy. These tests originally documented the
    /// defect where a detection gate probed only on-disk host locations that no shipped package
    /// populates, which made Auto silently skip LocalProxy.
    /// </summary>
    [TestFixture]
    public class ConnectionModeDetectionTests
    {
        [Test]
        public void AutoOffersLocalProxyOnWindows()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Ignore("Windows-only behaviour.");
            }

            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.BaseUri = "http://fimsvc:5725";
            options.ConnectionMode = ConnectionMode.Auto;

            List<ConnectionMode> modes = ClientFactory.DetectConnectionModes(options).ToList();

            Assert.That(modes, Does.Contain(ConnectionMode.LocalProxy),
                "Auto must offer LocalProxy on Windows because the embedded proxy host is always available.");
        }

        [Test]
        public void AutoPrefersLocalProxyOverRemoteProxyOnWindows()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Ignore("Windows-only behaviour.");
            }

            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.BaseUri = "http://fimsvc:5725";
            options.ConnectionMode = ConnectionMode.Auto;

            List<ConnectionMode> modes = ClientFactory.DetectConnectionModes(options).ToList();

            int localIndex = modes.IndexOf(ConnectionMode.LocalProxy);
            int remoteIndex = modes.IndexOf(ConnectionMode.RemoteProxy);

            Assert.That(localIndex, Is.GreaterThanOrEqualTo(0), "Auto did not offer LocalProxy at all.");
            Assert.That(localIndex, Is.LessThan(remoteIndex),
                "Auto must prefer LocalProxy over RemoteProxy on Windows.");
        }

        [Test]
        public void ExplicitModeYieldsExactlyThatMode()
        {
            // When the caller names a mode there must be no fallback chain: detection yields
            // exactly one candidate, so a connection failure surfaces directly.
            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.BaseUri = "http://fimsvc:5725";
            options.ConnectionMode = ConnectionMode.RemoteProxy;

            List<ConnectionMode> modes = ClientFactory.DetectConnectionModes(options).ToList();

            Assert.That(modes, Is.EqualTo(new List<ConnectionMode> { ConnectionMode.RemoteProxy }));
        }
    }
}
