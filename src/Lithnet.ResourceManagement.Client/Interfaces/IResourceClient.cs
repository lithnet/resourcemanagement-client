using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal interface IResourceClient
    {
        Task DeleteAsync(IEnumerable<ResourceObject> resources);

        Task DeleteAsync(IEnumerable<UniqueIdentifier> resourceIDs);

        Task DeleteAsync(ResourceObject resource);

        Task DeleteAsync(UniqueIdentifier id);

        Task<ResourceObject> GetAsync(UniqueIdentifier id, IEnumerable<string> attributes, CultureInfo locale, bool getPermissions);

        Task<XmlDictionaryReader> GetFullObjectForUpdateAsync(ResourceObject resource);

        Task PutAsync(IEnumerable<ResourceObject> resources);

        Task PutAsync(ResourceObject resource, CultureInfo locale);
    }
}