using System;
using System.Net;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client
{
    internal interface IClientFactory
    {
        string BaseUri { get; set; }

        int ConcurrentConnections { get; set; }

        TimeSpan ConnectTimeout { get; set; }

        NetworkCredential Credentials { get; set; }

        int RecieveTimeout { get; set; }

        IResourceClient ResourceClient { get; }

        IResourceFactoryClient ResourceFactoryClient { get; }

        ISchemaClient SchemaClient { get; }

        ISearchClient SearchClient { get; }

        IApprovalClient ApprovalClient { get; } 

        int SendTimeout { get; set; }

        string Spn { get; set; }

        Task InitializeClientsAsync(ResourceManagementClient rmc);
    }
}