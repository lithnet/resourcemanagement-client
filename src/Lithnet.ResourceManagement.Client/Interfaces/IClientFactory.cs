using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client
{
    internal interface IClientFactory
    {
        IResourceClient ResourceClient { get; }

        IResourceFactoryClient ResourceFactoryClient { get; }

        ISchemaClient SchemaClient { get; }

        ISearchClient SearchClient { get; }

        IApprovalClient ApprovalClient { get; }

        bool IsFaulted { get; }

        Task InitializeClientsAsync();
    }
}