using System.IO;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.DiscoveryTests
{
    /// <summary>
    /// Tests for the single host-resolution function. The registry reads are substituted through
    /// the resolver's provider seams so the tests run without elevation and independently of the
    /// state of the machine they run on.
    /// </summary>
    [TestFixture]
    public class ProxyHostResolverTests
    {
        private string existingExe;
        private string hostDirectory;
        private string savedFxHostPath;

        [SetUp]
        public void Initialize()
        {
            this.existingExe = Path.Combine(Path.GetTempPath(), "rmc-resolver-test.exe");
            File.WriteAllBytes(this.existingExe, new byte[] { 0x4D, 0x5A });

            this.hostDirectory = Path.Combine(Path.GetTempPath(), "rmc-resolver-test-dir");
            Directory.CreateDirectory(this.hostDirectory);
            File.WriteAllBytes(Path.Combine(this.hostDirectory, ProxyHostResolver.HostExeName), new byte[] { 0x4D, 0x5A });

            this.savedFxHostPath = RmcConfiguration.FxHostPath;
            RmcConfiguration.FxHostPath = null;

            ProxyHostResolver.RegistryHostPathProvider = () => null;
            ProxyHostResolver.UserFxHostPathProvider = () => null;
            ProxyHostResolver.MachineFxHostPathProvider = () => null;
        }

        [TearDown]
        public void Cleanup()
        {
            ProxyHostResolver.RegistryHostPathProvider = ProxyHostResolver.ReadRegistryHostPath;
            ProxyHostResolver.UserFxHostPathProvider = ProxyHostResolver.ReadUserFxHostPath;
            ProxyHostResolver.MachineFxHostPathProvider = ProxyHostResolver.ReadMachineFxHostPath;
            RmcConfiguration.FxHostPath = this.savedFxHostPath;

            File.Delete(this.existingExe);
            Directory.Delete(this.hostDirectory, true);
        }

        [Test]
        public void ReturnsNullWhenNoHostIsInstalled()
        {
            Assert.That(ProxyHostResolver.ResolveInstalledHost(new ResourceManagementClientOptions()), Is.Null);
        }

        [Test]
        public void PrefersExplicitHostExeOverRegistry()
        {
            ProxyHostResolver.RegistryHostPathProvider = () => "C:\\registry\\host.exe";
            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.RmcHostExe = this.existingExe;

            Assert.That(ProxyHostResolver.ResolveInstalledHost(options), Is.EqualTo(this.existingExe));
        }

        [Test]
        public void ReturnsRegistryHostWhenInstalled()
        {
            ProxyHostResolver.RegistryHostPathProvider = () => this.existingExe;

            Assert.That(ProxyHostResolver.ResolveInstalledHost(new ResourceManagementClientOptions()), Is.EqualTo(this.existingExe));
        }

        [Test]
        public void ReturnsHostFromUserFxHostPathDirectory()
        {
            ProxyHostResolver.UserFxHostPathProvider = () => this.hostDirectory;

            string expected = Path.Combine(this.hostDirectory, ProxyHostResolver.HostExeName);
            Assert.That(ProxyHostResolver.ResolveInstalledHost(new ResourceManagementClientOptions()), Is.EqualTo(expected));
        }

        [Test]
        public void ReturnsHostFromMachineFxHostPathDirectory()
        {
            ProxyHostResolver.MachineFxHostPathProvider = () => this.hostDirectory;

            string expected = Path.Combine(this.hostDirectory, ProxyHostResolver.HostExeName);
            Assert.That(ProxyHostResolver.ResolveInstalledHost(new ResourceManagementClientOptions()), Is.EqualTo(expected));
        }

        [Test]
        public void ThrowsWhenExplicitHostExeDoesNotExist()
        {
            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.RmcHostExe = "C:\\does\\not\\exist.exe";

            Assert.Throws<FileNotFoundException>(() => ProxyHostResolver.ResolveInstalledHost(options));
        }

        [Test]
        public void ThrowsWhenRegistryHostPathIsStale()
        {
            // A registry value naming a missing file means a broken install. Silently extracting
            // to TEMP would mask it, and on an application-control machine the extracted copy is
            // blocked anyway, so the user would get a baffling error instead of "your proxy
            // install is broken".
            ProxyHostResolver.RegistryHostPathProvider = () => "C:\\does\\not\\exist.exe";

            Assert.Throws<FileNotFoundException>(() => ProxyHostResolver.ResolveInstalledHost(new ResourceManagementClientOptions()));
        }

        [Test]
        public void ThrowsWhenConfiguredFxHostPathDirectoryHasNoHost()
        {
            string emptyDirectory = Path.Combine(Path.GetTempPath(), "rmc-resolver-test-empty");
            Directory.CreateDirectory(emptyDirectory);

            try
            {
                RmcConfiguration.FxHostPath = emptyDirectory;

                Assert.Throws<FileNotFoundException>(() => ProxyHostResolver.ResolveInstalledHost(new ResourceManagementClientOptions()));
            }
            finally
            {
                Directory.Delete(emptyDirectory, true);
            }
        }
    }
}
