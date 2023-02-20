#if !NETFRAMEWORK

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    
    public class ConfigTests
    {
        private readonly string hostname;

        public ConfigTests()
        {
            var options = UnitTestHelper.ServiceProvider.GetRequiredService<IOptions<ResourceManagementClientOptions>>().Value;
            UriBuilder builder = new UriBuilder(options.BaseUri);
            this.hostname = builder.Host;
        }

        [Test]
        public void TestNetTcpBindingWithPort()
        {
            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.BaseUri = $"net.tcp://{this.hostname}:5736";
            options.ConnectionMode = ConnectionMode.Auto;

            var client = new ResourceManagementClient(options);

            Assert.AreEqual("DisplayName", client.GetCorrectAttributeName("displayname"));
        }

        [Test]
        public void TestNetTcpBindingWithoutPort()
        {
            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.BaseUri = $"net.tcp://{this.hostname}";
            options.ConnectionMode = ConnectionMode.Auto;

            var client = new ResourceManagementClient(options);

            Assert.AreEqual("DisplayName", client.GetCorrectAttributeName("displayname"));
        }

        [Test]
        public void TestRmcBindingWithPort()
        {
            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.BaseUri = $"rmc://{this.hostname}:5735";
            options.ConnectionMode = ConnectionMode.Auto;

            var client = new ResourceManagementClient(options);

            Assert.AreEqual("DisplayName", client.GetCorrectAttributeName("displayname"));
        }

        [Test]
        public void TestRmcBindingWithoutPort()
        {
            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.BaseUri = $"rmc://{this.hostname}";
            options.ConnectionMode = ConnectionMode.Auto;

            var client = new ResourceManagementClient(options);

            Assert.AreEqual("DisplayName", client.GetCorrectAttributeName("displayname"));
        }

        [Test]
        public void TestPipeBindingWithPort()
        {
            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.BaseUri = $"pipe://{this.hostname}:5725";
            options.ConnectionMode = ConnectionMode.Auto;

            var client = new ResourceManagementClient(options);

            Assert.AreEqual("DisplayName", client.GetCorrectAttributeName("displayname"));
        }

        [Test]
        public void TestPipeBindingWithoutPort()
        {
            ResourceManagementClientOptions options = new ResourceManagementClientOptions();
            options.BaseUri = $"pipe://{this.hostname}";
            options.ConnectionMode = ConnectionMode.Auto;

            var client = new ResourceManagementClient(options);

            Assert.AreEqual("DisplayName", client.GetCorrectAttributeName("displayname"));
        }
    }
}

#endif