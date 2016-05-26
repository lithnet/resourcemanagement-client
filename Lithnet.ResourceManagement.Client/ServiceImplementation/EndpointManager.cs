using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Lithnet.ResourceManagement.Client
{
    internal class EndpointManager
    {
        private Uri baseUri;

        public EndpointIdentity EndpointSpn { get; private set; }

        public EndpointAddress ResourceFactoryEndpoint { get; private set; }

        public EndpointAddress ResourceEndpoint { get; private set; }

        public EndpointAddress SearchEndpoint { get; private set; }

        public EndpointAddress MetadataEndpoint { get; private set; }

        public EndpointManager(Uri baseUri, EndpointIdentity spn)
        {
            UriBuilder builder = new UriBuilder(baseUri);

            this.baseUri = baseUri;

            if (spn == null)
            {
                this.EndpointSpn = EndpointManager.SpnIdentityFromUri(baseUri);
            }
            else
            {
                this.EndpointSpn = spn;
            }

            builder.Path = "ResourceManagementService/Resource";
            this.ResourceEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn, new AddressHeader[0]);

            builder.Path = "ResourceManagementService/ResourceFactory";
            this.ResourceFactoryEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn, new AddressHeader[0]);

            builder.Path = "ResourceManagementService/Enumeration";
            this.SearchEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn, new AddressHeader[0]);

            builder.Path = "ResourceManagementService/MEX";
            this.MetadataEndpoint = new EndpointAddress(builder.Uri, this.EndpointSpn, new AddressHeader[0]);
        }

        public EndpointManager(string baseUri)
            : this(new Uri(baseUri), null)
        {
        }


        public static EndpointIdentity SpnIdentityFromUri(Uri uri)
        {
            return EndpointIdentity.CreateSpnIdentity($"FIMService/{uri.Host}");
        }

        public static EndpointAddress EndpointFromAddress(string address)
        {
            Uri uri = new Uri(address);
            return new EndpointAddress(uri, EndpointManager.SpnIdentityFromUri(uri), new AddressHeader[0]);
        }
    }
}

