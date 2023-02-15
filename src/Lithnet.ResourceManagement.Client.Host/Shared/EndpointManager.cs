using System;
using System.ServiceModel;

namespace Lithnet.ResourceManagement.Client
{
    internal class EndpointManager
    {
        public EndpointIdentity EndpointSpn { get; private set; }

        public EndpointAddress ResourceFactoryEndpoint { get; private set; }

        public EndpointAddress ResourceEndpoint { get; private set; }

        public EndpointAddress SearchEndpoint { get; private set; }

        public EndpointAddress MetadataEndpoint { get; private set; }

        public EndpointAddress NetTcpResourceFactoryEndpoint { get; private set; }

        public EndpointAddress NetTcpResourceEndpoint { get; private set; }

        public EndpointAddress NetTcpSearchEndpoint { get; private set; }


        public EndpointManager(string baseAddress, string spn)
        {
            this.Configure(baseAddress, spn);
        }

        private void Configure(string baseAddress, string spn)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException("A MIM serivce URI was not provided");
            }

            Uri uri = Uri.IsWellFormedUriString(baseAddress, UriKind.Absolute) ?
                new Uri(baseAddress) :
                new Uri($"http://{baseAddress}:5725");

            EndpointIdentity spnIdentity = string.IsNullOrWhiteSpace(spn) ?
                EndpointManager.SpnIdentityFromUri(uri) :
                EndpointManager.SpnIdentityFromSpn(spn);

            this.Configure(uri, spnIdentity);
        }

        public EndpointManager(Uri baseUri, EndpointIdentity spn)
        {
            this.Configure(baseUri, spn);
        }

        private void Configure(Uri baseUri, EndpointIdentity spn)
        {
            if (!baseUri.IsAbsoluteUri)
            {
                baseUri = new Uri($"http://{baseUri}:5725");
            }

            UriBuilder builder = new UriBuilder(baseUri);

            if (spn == null)
            {
                this.EndpointSpn = EndpointManager.SpnIdentityFromUri(baseUri);
            }
            else
            {
                this.EndpointSpn = spn;
            }

            builder.Path = "ResourceManagementService/Resource";
            this.ResourceEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn);

            builder.Path = "ResourceManagementService/ResourceFactory";
            this.ResourceFactoryEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn);

            builder.Path = "ResourceManagementService/Enumeration";
            this.SearchEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn);

            builder.Path = "ResourceManagementService/MEX";
            this.MetadataEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn);

            builder.Scheme = "net.tcp";
            builder.Port = 5736;
            builder.Path = "ResourceManagementService/Resource";
            this.NetTcpResourceEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn);

            builder.Path = "ResourceManagementService/ResourceFactory";
            this.NetTcpResourceFactoryEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn);

            builder.Path = "ResourceManagementService/Enumeration";
            this.NetTcpSearchEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn);
        }

        public EndpointManager(string baseUri)
        {
            this.Configure(baseUri, null);
        }

        public static EndpointIdentity SpnIdentityFromUri(Uri uri)
        {
#if NETFRAMEWORK
            return EndpointIdentity.CreateSpnIdentity($"FIMService/{uri.Host}");
#else
            return new SpnEndpointIdentity($"FIMService/{uri.Host}");
#endif
        }

        public static EndpointIdentity SpnIdentityFromSpn(string spn)
        {
#if NETFRAMEWORK
            return EndpointIdentity.CreateSpnIdentity(spn);
#else
            return new SpnEndpointIdentity(spn);
#endif
        }

        public static EndpointAddress EndpointFromAddress(string address)
        {
            Uri uri = new Uri(address);
            return new EndpointAddress(uri, EndpointManager.SpnIdentityFromUri(uri));
        }
    }
}