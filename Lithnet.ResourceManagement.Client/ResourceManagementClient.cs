using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Microsoft.ResourceManagement.WebServices;
using System.Threading;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Lithnet.ResourceManagement.Client
{
    public class ResourceManagementClient
    {
        private ResourceClient resourceClient;

        private ResourceFactoryClient resourceFactoryClient;

        private SearchClient searchClient;

        public static bool UseNewChannelPerCall { get; set; }

        public static NetworkCredential NetworkCredentials { get; set; }

        internal static EndpointManager EndpointManager { get; private set; }

        internal static ClientConfigurationSection Configuration { get; private set; }

        private static Binding HttpContextBinding;

        static ResourceManagementClient()
        {
            ResourceManagementClient.Configuration = ClientConfigurationSection.GetConfiguration();

            if (ResourceManagementClient.Configuration.ConcurrentConnectionLimit > 0)
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = ResourceManagementClient.Configuration.ConcurrentConnectionLimit;
            }

            Uri baseAddress;

            if (Configuration.ResourceManagementServiceBaseAddress == null)
            {
                baseAddress = new Uri("http://localhost:5725");
            }
            else
            {
                baseAddress = Configuration.ResourceManagementServiceBaseAddress;
            }

            EndpointIdentity spn = null;

            if (!string.IsNullOrWhiteSpace(Configuration.ServicePrincipalName))
            {
                spn = EndpointIdentity.CreateSpnIdentity(Configuration.ServicePrincipalName);
            }

            ResourceManagementClient.EndpointManager = new EndpointManager(baseAddress, spn);
            ResourceManagementClient.HttpContextBinding = BindingManager.GetWsHttpContextBinding();

            if (Configuration.Username != null)
            {
                ResourceManagementClient.NetworkCredentials = new NetworkCredential(Configuration.Username, Configuration.Password);
            }
        }

        public ResourceManagementClient()
        {
            this.InitializeClients();
        }

        public void DeleteResources(IEnumerable<UniqueIdentifier> resourceIDs)
        {
            this.resourceClient.Delete(resourceIDs);
        }

        public void DeleteResources(IEnumerable<ResourceObject> resources)
        {
            this.resourceClient.Delete(resources);

            foreach (ResourceObject resource in resources)
            {
                resource.IsDeleted = true;
            }
        }

        public void DeleteResource(ResourceObject resource)
        {
            this.DeleteResource(resource.ObjectID);
            resource.IsDeleted = true;
        }

        public void DeleteResource(UniqueIdentifier id)
        {
            this.resourceClient.Delete(id);
        }

        public void DeleteResource(Guid id)
        {
            this.DeleteResource(new UniqueIdentifier(id));
        }

        public void DeleteResource(string id)
        {
            this.DeleteResource(new UniqueIdentifier(id));
        }

        public void SaveResources(IEnumerable<ResourceObject> resourceObjects)
        {
            if (resourceObjects.Any(t => t.ModificationType != OperationType.Update)) 
            {
                throw new InvalidOperationException("Batch operations can only be performed when all objects have a modification type of Update");
            }

            this.resourceClient.Put(resourceObjects);

            foreach (ResourceObject resource in resourceObjects)
            {
                resource.CommitChanges();
            }
        }

        public void SaveResource(ResourceObject resource)
        {
            switch (resource.ModificationType)
            {
                case OperationType.None:
                    return;

                case OperationType.Create:
                    this.CreateResource(resource);
                    break;

                case OperationType.Update:
                    this.PutResource(resource);
                    resource.CommitChanges();
                    break;

                case OperationType.Delete:
                    this.DeleteResource(resource);
                    break;

                default:
                    break;
            }

        }

        public ResourceObject CreateResource(string objectType)
        {
            return new ResourceObject(objectType);
        }

        public ResourceObject CreateResourceTemplateForUpdate(string objectType, UniqueIdentifier id)
        {
            return new ResourceObject(objectType, id);
        }

        public ResourceObject GetResource(Guid id)
        {
            return this.GetResource(new UniqueIdentifier(id), null);
        }

        public ResourceObject GetResource(Guid id, IEnumerable<string> attributesToGet)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet);
        }

        public ResourceObject GetResource(string id)
        {
            return this.GetResource(new UniqueIdentifier(id), null);
        }

        public ResourceObject GetResource(string id, IEnumerable<string> attributesToGet)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet);
        }

        public ResourceObject GetResource(UniqueIdentifier id)
        {
            return this.resourceClient.Get(id, null);
        }

        public ResourceObject GetResource(UniqueIdentifier id, IEnumerable<string> attributesToGet)
        {
            return this.resourceClient.Get(id, attributesToGet);
        }

        public ResourceObject GetResourceByKey(string objectType, string attribute, string value)
        {
            return this.GetResourceByKey(objectType, attribute, value, null);
        }

        public ResourceObject GetResourceByKey(string objectType, string attribute, string value, IEnumerable<string> attributesToGet)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add(attribute, value);

            return this.GetResourceByKey(objectType, values, attributesToGet);
        }

        public ResourceObject GetResourceByKey(string objectType, Dictionary<string, string> attributeValuePairs)
        {
            return this.GetResourceByKey(objectType, attributeValuePairs, null);
        }

        public ResourceObject GetResourceByKey(string objectType, Dictionary<string, string> attributeValuePairs, IEnumerable<string> attributesToGet)
        {
            string filter = XpathFilterBuilder.GetAndFilterText(objectType, attributeValuePairs);

            if (attributesToGet == null)
            {
                attributesToGet = Schema.ObjectTypes[objectType].Select(t => t.SystemName);
            }

            ISearchResults results = this.searchClient.Enumerate(filter, 1, attributesToGet);

            if (results.Count == 0)
            {
                return null;
            }
            else if (results.Count > 1)
            {
                throw new TooManyResultsException("The query returned more than one result");
            }
            else
            {
                return results.FirstOrDefault();
            }
        }

        public ISearchResults GetResources(string filter)
        {
            return this.searchClient.Enumerate(filter, -1, null, null);
        }

        public ISearchResults GetResources(string filter, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.Enumerate(filter, -1, null, cancellationToken);
        }

        public ISearchResults GetResources(string filter, IEnumerable<string> attributesToRetrieve)
        {
            return this.searchClient.Enumerate(filter, -1, attributesToRetrieve, null);
        }

        public ISearchResults GetResources(string filter, IEnumerable<string> attributesToRetrieve, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.Enumerate(filter, -1, attributesToRetrieve, cancellationToken);
        }

        public ISearchResults GetResources(string filter, int pageSize)
        {
            return this.searchClient.Enumerate(filter, pageSize, null, null);
        }

        public ISearchResults GetResources(string filter, int pageSize, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.Enumerate(filter, pageSize, null, cancellationToken);
        }

        public ISearchResults GetResources(string filter, int pageSize, IEnumerable<string> attributesToRetrieve)
        {
            return this.searchClient.Enumerate(filter, pageSize, attributesToRetrieve, null);
        }

        public ISearchResults GetResources(string filter, int pageSize, IEnumerable<string> attributesToRetrieve, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.Enumerate(filter, pageSize, attributesToRetrieve, cancellationToken);
        }

        internal void CreateResource(ResourceObject resource)
        {
            this.resourceFactoryClient.Create(resource);
        }

        internal void PutResource(ResourceObject resource)
        {
            this.resourceClient.Put(resource);
        }

        private void InitializeClients()
        {
            this.resourceClient = new ResourceClient(ResourceManagementClient.HttpContextBinding, ResourceManagementClient.EndpointManager.ResourceEndpoint);
            this.resourceFactoryClient = new ResourceFactoryClient(ResourceManagementClient.HttpContextBinding, ResourceManagementClient.EndpointManager.ResourceFactoryEndpoint);
            this.searchClient = new SearchClient(ResourceManagementClient.HttpContextBinding, ResourceManagementClient.EndpointManager.SearchEndpoint);

            if (ResourceManagementClient.NetworkCredentials != null)
            {
                this.resourceClient.ClientCredentials.Windows.ClientCredential = ResourceManagementClient.NetworkCredentials;
                this.resourceFactoryClient.ClientCredentials.Windows.ClientCredential = ResourceManagementClient.NetworkCredentials;
                this.searchClient.ClientCredentials.Windows.ClientCredential = ResourceManagementClient.NetworkCredentials;
            }

#pragma warning disable 0618
            this.resourceClient.ClientCredentials.Windows.AllowNtlm = !Configuration.ForceKerberos;
            this.resourceClient.ClientCredentials.Windows.AllowNtlm = !Configuration.ForceKerberos;
            this.resourceClient.ClientCredentials.Windows.AllowNtlm = !Configuration.ForceKerberos;
#pragma warning restore 0618

            this.resourceClient.Initialize();
            this.resourceFactoryClient.Initialize();
            this.searchClient.Initialize();

            this.resourceClient.Open();
            this.resourceFactoryClient.Open();
            this.searchClient.Open();

            Schema.LoadSchema();
        }
    }
}