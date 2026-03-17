using System;
using System.Threading.Tasks;
using Lithnet.ResourceManagement.Client.ResourceManagementService;

namespace Lithnet.ResourceManagement.Client
{
    internal interface IClient : IDisposable
    {
        string DisplayName { get; }
        
        IResourceClient ResourceClient { get; }

        IResourceFactoryClient ResourceFactoryClient { get; }

        ISchemaClient SchemaClient { get; }

        ISearchClient SearchClient { get; }

        IApprovalClient ApprovalClient { get; }

        bool IsFaulted { get; }

        Task InitializeClientsAsync();

        Task<IResource> GetResourceChannelAsync();

        Task<IResourceFactory> GetResourceFactoryChannelAsync();

        Task<ISearch> GetSearchChannelAsync();

        Task<IMetadataExchange> GetSchemaChannelAsync();

        Task<IApprovalService> GetApprovalChannelAsync();
    }
}