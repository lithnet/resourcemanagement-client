using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal interface IResourceClient
    {
        Task DeleteAsync(IEnumerable<IResourceObject> resources);

        Task DeleteAsync(IEnumerable<UniqueIdentifier> resourceIDs);

        Task DeleteAsync(IResourceObject resource);

        Task DeleteAsync(UniqueIdentifier id);

        Task<ResourceObject> GetAsync(UniqueIdentifier id, IEnumerable<string> attributes, CultureInfo locale, bool getPermissions);

        Task<XmlDictionaryReader> GetFullObjectForUpdateAsync(IResourceObject resource);

        Task PutAsync(IEnumerable<IResourceObject> resources);

        Task PutAsync(IResourceObject resource, CultureInfo locale);
    }
}