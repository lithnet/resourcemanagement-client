using System;

namespace Lithnet.ResourceManagement.Client
{
    internal static class UriParser
    {
        public static Uri GetFimServiceHttpUri(string baseUri)
        {
            UriBuilder builder;

            if (string.IsNullOrWhiteSpace(baseUri))
            {
                builder = new UriBuilder($"http://localhost:{ResourceManagementClientOptions.DefaultFimServicePort}");
            }
            else if (Uri.IsWellFormedUriString(baseUri, UriKind.Absolute) && (baseUri.Contains("://")))
            {
                builder = new UriBuilder(baseUri);
            }
            else
            {
                builder = new UriBuilder($"http://{baseUri}");
            }

            if (builder.Port <= 0 || builder.Port == 80)
            {
                builder.Port = ResourceManagementClientOptions.DefaultFimServicePort;
            }

            builder.Scheme = "http";

            return builder.Uri;
        }

        public static Uri GetFimServiceNetTcpUri(string baseUri)
        {
            UriBuilder builder;

            if (string.IsNullOrWhiteSpace(baseUri))
            {
                builder = new UriBuilder($"net.tcp://localhost:{ResourceManagementClientOptions.DefaultNetTcpBindingPort}");
            }
            else if (Uri.IsWellFormedUriString(baseUri, UriKind.Absolute) && (baseUri.Contains("://")))
            {
                builder = new UriBuilder(baseUri);
            }
            else
            {
                builder = new UriBuilder($"net.tcp://{baseUri}");
            }

            if (builder.Port <= 0 || builder.Port == 80 || builder.Port == 808)
            {
                builder.Port = ResourceManagementClientOptions.DefaultNetTcpBindingPort;
            }

            if (builder.Port == ResourceManagementClientOptions.DefaultFimServicePort)
            {
                builder.Port = ResourceManagementClientOptions.DefaultNetTcpBindingPort;
            }

            builder.Scheme = "net.tcp";

            return builder.Uri;
        }

        public static Uri GetRmcProxyUri(string baseUri)
        {
            UriBuilder builder;

            if (string.IsNullOrWhiteSpace(baseUri))
            {
                builder = new UriBuilder($"rmc://localhost:{ResourceManagementClientOptions.DefaultRemoteProxyPort}");
            }
            else if (Uri.IsWellFormedUriString(baseUri, UriKind.Absolute) && (baseUri.Contains("://")))
            {
                builder = new UriBuilder(baseUri);
            }
            else
            {
                builder = new UriBuilder($"rmc://{baseUri}");
            }

            if (builder.Port <= 0 || builder.Port == 80)
            {
                builder.Port = ResourceManagementClientOptions.DefaultRemoteProxyPort;
            }

            if (builder.Port == ResourceManagementClientOptions.DefaultFimServicePort)
            {
                builder.Port = ResourceManagementClientOptions.DefaultRemoteProxyPort;
            }

            builder.Scheme = "rmc";

            return builder.Uri;
        }

        public static Uri GetPipeUri(string baseUri)
        {
            UriBuilder builder;

            if (string.IsNullOrWhiteSpace(baseUri))
            {
                builder = new UriBuilder($"pipe://localhost:{ResourceManagementClientOptions.DefaultFimServicePort}");
            }
            else if (Uri.IsWellFormedUriString(baseUri, UriKind.Absolute) && (baseUri.Contains("://")))
            {
                builder = new UriBuilder(baseUri);
            }
            else
            {
                builder = new UriBuilder($"pipe://{baseUri}");
            }

            if (builder.Port <= 0 || builder.Port == 80)
            {
                builder.Port = ResourceManagementClientOptions.DefaultFimServicePort;
            }

            builder.Scheme = "pipe";

            return builder.Uri;
        }
    }
}
