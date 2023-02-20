#if !NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    
    public class UriMappingTests
    {
        [Test]
        public void TestFimServiceUriParser1()
        {
            var uri = UriParser.GetFimServiceHttpUri("http://xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5725, uri.Port);
            Assert.AreEqual("http", uri.Scheme);
        }

        [Test]
        public void TestFimServiceUriParser2()
        {
            var uri = UriParser.GetFimServiceHttpUri("http://xxx:1234");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("http", uri.Scheme);
        }

        [Test]
        public void TestFimServiceUriParser3()
        {
            var uri = UriParser.GetFimServiceHttpUri("zxfvasdf:1234");

            Assert.AreEqual("zxfvasdf", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("http", uri.Scheme);
        }

        [Test]
        public void TestFimServiceUriParser4()
        {
            var uri = UriParser.GetFimServiceHttpUri("xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5725, uri.Port);
            Assert.AreEqual("http", uri.Scheme);
        }

        [Test]
        public void TestPipeUriParser1()
        {
            var uri = UriParser.GetPipeUri("pipe://xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5725, uri.Port);
            Assert.AreEqual("pipe", uri.Scheme);
        }

        [Test]
        public void TestPipeUriParser2()
        {
            var uri = UriParser.GetPipeUri("pipe://xxx:1234");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("pipe", uri.Scheme);
        }

        [Test]
        public void TestPipeUriParser3()
        {
            var uri = UriParser.GetPipeUri("zxfvasdf:1234");

            Assert.AreEqual("zxfvasdf", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("pipe", uri.Scheme);
        }

        [Test]
        public void TestPipeUriParser4()
        {
            var uri = UriParser.GetPipeUri("xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5725, uri.Port);
            Assert.AreEqual("pipe", uri.Scheme);
        }

        [Test]
        public void TestPipeUriParser5()
        {
            var uri = UriParser.GetPipeUri("http://xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5725, uri.Port);
            Assert.AreEqual("pipe", uri.Scheme);
        }

        [Test]
        public void TestPipeUriParser6()
        {
            var uri = UriParser.GetPipeUri("http://xxx:1234");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("pipe", uri.Scheme);
        }

        [Test]
        public void TestNetTcpUriParser1()
        {
            var uri = UriParser.GetFimServiceNetTcpUri("net.tcp://xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5736, uri.Port);
            Assert.AreEqual("net.tcp", uri.Scheme);
        }

        [Test]
        public void TestNetTcpUriParser2()
        {
            var uri = UriParser.GetFimServiceNetTcpUri("net.tcp://xxx:1234");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("net.tcp", uri.Scheme);
        }

        [Test]
        public void TestNetTcpUriParser3()
        {
            var uri = UriParser.GetFimServiceNetTcpUri("zxfvasdf:1234");

            Assert.AreEqual("zxfvasdf", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("net.tcp", uri.Scheme);
        }

        [Test]
        public void TestNetTcpUriParser4()
        {
            var uri = UriParser.GetFimServiceNetTcpUri("xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5736, uri.Port);
            Assert.AreEqual("net.tcp", uri.Scheme);
        }


        [Test]
        public void TestNetTcpUriParser5()
        {
            var uri = UriParser.GetFimServiceNetTcpUri("http://xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5736, uri.Port);
            Assert.AreEqual("net.tcp", uri.Scheme);
        }

        [Test]
        public void TestNetTcpUriParser6()
        {
            var uri = UriParser.GetFimServiceNetTcpUri("http://xxx:1234");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("net.tcp", uri.Scheme);
        }

        [Test]
        public void TestProxyUriParser1()
        {
            var uri = UriParser.GetRmcProxyUri("rmc://xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5735, uri.Port);
            Assert.AreEqual("rmc", uri.Scheme);
        }

        [Test]
        public void TestProxyUriParser2()
        {
            var uri = UriParser.GetRmcProxyUri("rmc://xxx:1234");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("rmc", uri.Scheme);
        }

        [Test]
        public void TestProxyUriParser3()
        {
            var uri = UriParser.GetRmcProxyUri("zxfvasdf:1234");

            Assert.AreEqual("zxfvasdf", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("rmc", uri.Scheme);
        }

        [Test]
        public void TestProxyUriParser4()
        {
            var uri = UriParser.GetRmcProxyUri("xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5735, uri.Port);
            Assert.AreEqual("rmc", uri.Scheme);
        }

        [Test]
        public void TestProxyUriParser5()
        {
            var uri = UriParser.GetRmcProxyUri("http://xxx");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(5735, uri.Port);
            Assert.AreEqual("rmc", uri.Scheme);
        }

        [Test]
        public void TestProxyUriParser6()
        {
            var uri = UriParser.GetRmcProxyUri("http://xxx:1234");

            Assert.AreEqual("xxx", uri.Host);
            Assert.AreEqual(1234, uri.Port);
            Assert.AreEqual("rmc", uri.Scheme);
        }
    }
}
#endif