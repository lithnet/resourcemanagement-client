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

        public EndpointManager(Uri baseUri, EndpointIdentity spn)
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
        }

        public EndpointManager(string baseUri)
            : this(new Uri(baseUri), null)
        {
        }

        public static EndpointIdentity SpnIdentityFromUri(Uri uri)
        {
#if NETFRAMEWORK
            return EndpointIdentity.CreateSpnIdentity($"FIMService/{uri.Host}");
#else
            return new SpnEndpointIdentity($"FIMService/{uri.Host}");
#endif
        }

        public static EndpointAddress EndpointFromAddress(string address)
        {
            Uri uri = new Uri(address);
            return new EndpointAddress(uri, EndpointManager.SpnIdentityFromUri(uri));
        }
    }
}