using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.DiscoveryTests
{
    /// <summary>
    /// Guards the RemoteProxy authentication SPN contract: the default targets the host computer
    /// account (host/), matching the proxy service's Network Service identity, and an explicitly
    /// configured SPN always wins.
    /// </summary>
    [TestFixture]
    public class RemoteProxySpnTests
    {
        [Test]
        public void DefaultsToHostSpn()
        {
            Assert.That(NegotiateStreamRpcClient.GetAuthenticationSpn(null, "mim1.contoso.com"),
                Is.EqualTo("host/mim1.contoso.com"));
        }

        [Test]
        public void EmptyConfiguredSpnFallsBackToHostSpn()
        {
            Assert.That(NegotiateStreamRpcClient.GetAuthenticationSpn(" ", "mim1.contoso.com"),
                Is.EqualTo("host/mim1.contoso.com"));
        }

        [Test]
        public void ConfiguredSpnAlwaysWins()
        {
            Assert.That(NegotiateStreamRpcClient.GetAuthenticationSpn("FIMService/mim1.contoso.com", "mim1.contoso.com"),
                Is.EqualTo("FIMService/mim1.contoso.com"));
        }
    }
}
