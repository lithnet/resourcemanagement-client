using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Microsoft.ResourceManagement.WebServices;
using System.Threading;
using System.Net;
using System.ServiceModel;

namespace Lithnet.ResourceManagement.Client
{
    public class ResourceManagementClient //: IDisposable
    {
        private ResourceClient resourceClient;

        private ResourceFactoryClient resourceFactoryClient;

        private SearchClient searchClient;

        //internal bool IsPooled { get; set; }

        public static bool UseNewChannelPerCall { get; set; }

        public static NetworkCredential NetworkCredentials { get; set; }

        public static int ConcurrentConnectionLimit { get; set; }

        public ResourceManagementClient()
        {
            this.InitializeClients();
        }

        //internal ResourceManagementClient(bool pooled)
        //{
        //    this.IsPooled = pooled;
        //    this.InitializeClients();
        //}

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
            this.resourceClient.Put(resourceObjects);

            foreach(ResourceObject resource in resourceObjects)
            {
                resource.CommitChanges();
            }
        }

        public void SaveResource(ResourceObject resource)
        {
            this.resourceClient.Put(resource);
            resource.CommitChanges();
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
            string filter = XpathFilterBuilder.GetFilter(objectType, attributeValuePairs);

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
            if (ResourceManagementClient.ConcurrentConnectionLimit > 0)
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = ResourceManagementClient.ConcurrentConnectionLimit;
            }

            System.Net.ServicePointManager.DefaultConnectionLimit = 10000;

            this.resourceClient = new ResourceClient();
            this.resourceFactoryClient = new ResourceFactoryClient();
            this.searchClient = new SearchClient();

            if (ResourceManagementClient.NetworkCredentials != null)
            {
                this.resourceClient.ClientCredentials.Windows.ClientCredential = ResourceManagementClient.NetworkCredentials;
                this.resourceFactoryClient.ClientCredentials.Windows.ClientCredential = ResourceManagementClient.NetworkCredentials;
                this.searchClient.ClientCredentials.Windows.ClientCredential = ResourceManagementClient.NetworkCredentials;
            }

            this.resourceClient.Initialize();
            this.resourceFactoryClient.Initialize();
            this.searchClient.Initialize();

            this.resourceClient.Open();
            this.resourceFactoryClient.Open();
            this.searchClient.Open();

            Schema.LoadSchema();
            
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        // release other disposable objects
        //    }
        //}

        //~ResourceManagementClient()
        //{
        //    this.Dispose(false);
        //}

        //public void Dispose()
        //{
        //    //if (!this.IsPooled)
        //    //{
        //        this.Dispose(true);
        //        GC.SuppressFinalize(this);
        //    //}
        //    //else
        //    //{
        //        ResourceManagementClientPool.Return(this);
        //    //}
        //}
    }
}
