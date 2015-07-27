namespace Lithnet.ResourceManagement.Client
{
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
    using System.Xml;

    /// <summary>
    /// The main class used to create, update, delete, and search for objects in the resource management service
    /// </summary>
    public class ResourceManagementClient
    {
        /// <summary>
        /// The local instance of the Resource proxy
        /// </summary>
        private ResourceClient resourceClient;

        /// <summary>
        /// The local instance of the ResourceFactory proxy
        /// </summary>
        private ResourceFactoryClient resourceFactoryClient;

        /// <summary>
        /// The local instance of the Search proxy
        /// </summary>
        private SearchClient searchClient;

        /// <summary>
        /// The binding used to connect to the resource management service
        /// </summary>
        private static Binding WsHttpContextBinding;

        /// <summary>
        /// Gets or sets the credentials used to connect to the resource management service
        /// </summary>
        public static NetworkCredential NetworkCredentials { get; set; }

        /// <summary>
        /// Gets the instance of the endpoint manager used to build the endpoint configuration for the resource management service
        /// </summary>
        internal static EndpointManager EndpointManager { get; private set; }

        /// <summary>
        /// Gets the instance of the lithnetResourceManagementClient section from the configuration file
        /// </summary>
        internal static ClientConfigurationSection Configuration { get; private set; }

        /// <summary>
        /// Initializes the static members of the ResourceManagementClient class
        /// </summary>
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
            ResourceManagementClient.WsHttpContextBinding = BindingManager.GetWsHttpContextBinding();

            if (Configuration.Username != null)
            {
                ResourceManagementClient.NetworkCredentials = new NetworkCredential(Configuration.Username, Configuration.Password);
            }
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementClient class
        /// </summary>
        public ResourceManagementClient()
        {
            this.InitializeClients();
        }

        /// <summary>
        /// Deletes the specified resources from the resource management service as a single composite operation
        /// </summary>
        /// <param name="resourceIDs">A collection of references to delete</param>
        public void DeleteResources(IEnumerable<UniqueIdentifier> resourceIDs)
        {
            this.resourceClient.Delete(resourceIDs);
        }

        /// <summary>
        /// Deletes the specified resources from the resource management service as a single composite operation
        /// </summary>
        /// <param name="resources">A collection of resources to delete</param>
        public void DeleteResources(IEnumerable<ResourceObject> resources)
        {
            this.resourceClient.Delete(resources);

            foreach (ResourceObject resource in resources)
            {
                resource.IsDeleted = true;
            }
        }

        /// <summary>
        /// Deletes the specified resource from the resource management service
        /// </summary>
        /// <param name="resource">The resource to delete</param>
        public void DeleteResource(ResourceObject resource)
        {
            this.DeleteResource(resource.ObjectID);
            resource.IsDeleted = true;
        }

        /// <summary>
        /// Deletes the specified resource from the resource management service
        /// </summary>
        /// <param name="id">The reference to the object to delete</param>
        public void DeleteResource(UniqueIdentifier id)
        {
            this.resourceClient.Delete(id);
        }

        /// <summary>
        /// Deletes the specified resource from the resource management service
        /// </summary>
        /// <param name="id">The ID of the object to delete</param>
        public void DeleteResource(Guid id)
        {
            this.DeleteResource(new UniqueIdentifier(id));
        }

        /// <summary>
        /// Deletes the specified resource from the resource management service
        /// </summary>
        /// <param name="id">The ID of the object to delete, in GUID format, with or without the urn: prefix</param>
        /// <example>
        /// <remarks>This example shows how to delete a resource using its ID</remarks>
        /// <code>
        /// ResourceManagementClient client = new ResourceManagementClient();
        /// client.DeleteResource("8d86fb71-e4d0-4af8-af5b-8a124a836fc6");
        /// </code>
        /// </example>
        public void DeleteResource(string id)
        {
            this.DeleteResource(new UniqueIdentifier(id));
        }

        /// <summary>
        /// Saves the specified resources in the resource management service. Updates are committed as a single composite operation, while adds are processed individually
        /// </summary>
        /// <param name="resources">The resources to update</param>
        public void SaveResources(params ResourceObject[] resources)
        {
            this.SaveResources(resources);
        }

        /// <summary>
        /// Saves the specified resources in the resource management service. Updates are committed as a single composite operation, while adds are processed individually
        /// </summary>
        /// <param name="resources">The collection of resources to update</param>
        public void SaveResources(IEnumerable<ResourceObject> resources)
        {
            foreach (ResourceObject resource in resources.Where(t => t.ModificationType == OperationType.Create))
            {
                this.CreateResource(resource);
            }

            List<ResourceObject> objectsToDelete = resources.Where(t => t.ModificationType == OperationType.Delete).ToList();
            List<ResourceObject> objectsToUpdate = resources.Where(t => t.ModificationType == OperationType.Update).ToList();

            if (objectsToDelete.Count > 0)
            {
                this.DeleteResources(objectsToDelete);
            }

            if (objectsToUpdate.Count > 0)
            {
                this.resourceClient.Put(objectsToUpdate);

                foreach (ResourceObject resource in resources)
                {
                    resource.CommitChanges();
                }
            }
        }

        /// <summary>
        /// Saves the specified resource in the resource management service
        /// </summary>
        /// <param name="resource">The resource to save</param>
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

        /// <summary>
        /// Creates a template of a new object that can be used to save to the resource management service at a later stage
        /// </summary>
        /// <param name="objectType">The object type to create</param>
        /// <returns>An empty template of a new object that will be created in the resource management service when saved</returns>
        public ResourceObject CreateResource(string objectType)
        {
            return new ResourceObject(objectType, this);
        }

        /// <summary>
        /// Creates a template of an existing object that can be used to save to the resource management service at a later stage.
        /// As this is an empty template, there is no. validation performed on value changes. It is recommended to use <c ref="GetResource"/>
        /// and perform the update against the real object
        /// </summary>
        /// <param name="objectType">The type of object to create the template for</param>
        /// <param name="id">The ID of the object to update</param>
        /// <returns>An empty template of an existing object that can be updated in the resource management service when saved</returns>
        public ResourceObject CreateResourceTemplateForUpdate(string objectType, UniqueIdentifier id)
        {
            return new ResourceObject(objectType, id, this);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(Guid id)
        {
            return this.GetResource(new UniqueIdentifier(id), null);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(Guid id, IEnumerable<string> attributesToGet)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(string id)
        {
            return this.GetResource(new UniqueIdentifier(id), null);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        public ResourceObject GetResource(string id, IEnumerable<string> attributesToGet)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(UniqueIdentifier id)
        {
            return this.resourceClient.Get(id, null);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(UniqueIdentifier id, IEnumerable<string> attributesToGet)
        {
            return this.resourceClient.Get(id, attributesToGet);
        }

        /// <summary>
        /// Gets a resource from the resource management service using a unique attribute and value combination, retrieving all attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attribute">The name of the attribute used as the key</param>
        /// <param name="value">The value of the attribute</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        public ResourceObject GetResourceByKey(string objectType, string attribute, string value)
        {
            return this.GetResourceByKey(objectType, attribute, value, null);
        }

        /// <summary>
        /// Gets a resource from the resource management service using a unique attribute and value combination, retrieving the specified attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attribute">The name of the attribute used as the key</param>
        /// <param name="value">The value of the attribute</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        public ResourceObject GetResourceByKey(string objectType, string attribute, string value, IEnumerable<string> attributesToGet)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add(attribute, value);

            return this.GetResourceByKey(objectType, values, attributesToGet);
        }

        /// <summary>
        /// Gets a resource from the resource management service using a set of unique attribute and value combinations, retrieving all attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attributeValuePairs">A list of attribute value pairs that make this object unique</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        public ResourceObject GetResourceByKey(string objectType, Dictionary<string, string> attributeValuePairs)
        {
            return this.GetResourceByKey(objectType, attributeValuePairs, null);
        }

        /// <summary>
        /// Gets a resource from the resource management service using a set of unique attribute and value combinations, retrieving the specified attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attributeValuePairs">A list of attribute value pairs that make this object unique</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        public ResourceObject GetResourceByKey(string objectType, Dictionary<string, string> attributeValuePairs, IEnumerable<string> attributesToGet)
        {
            string filter = XpathFilterBuilder.CreateAndFilter(objectType, attributeValuePairs);

            if (attributesToGet == null)
            {
                attributesToGet = ResourceManagementSchema.ObjectTypes[objectType].Attributes.Select(t => t.SystemName);
            }

            ISearchResultCollection results = this.searchClient.Enumerate(filter, 1, attributesToGet);

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

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the default page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter)
        {
            return this.searchClient.Enumerate(filter, -1, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.Enumerate(filter, -1, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the default page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToRetrieve">The list of attributes to retrieve</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, IEnumerable<string> attributesToRetrieve)
        {
            return this.searchClient.Enumerate(filter, -1, attributesToRetrieve, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToRetrieve">The list of attributes to retrieve</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, IEnumerable<string> attributesToRetrieve, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.Enumerate(filter, -1, attributesToRetrieve, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, int pageSize)
        {
            return this.searchClient.Enumerate(filter, pageSize, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, int pageSize, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.Enumerate(filter, pageSize, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToRetrieve">The list of attributes to retrieve</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, int pageSize, IEnumerable<string> attributesToRetrieve)
        {
            return this.searchClient.Enumerate(filter, pageSize, attributesToRetrieve, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToRetrieve">The list of attributes to retrieve</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, int pageSize, IEnumerable<string> attributesToRetrieve, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.Enumerate(filter, pageSize, attributesToRetrieve, cancellationToken);
        }

        /// <summary>
        /// Performs an refresh of an object by updating the resource from the resource management service
        /// </summary>
        /// <param name="resource">The object to refresh</param>
        /// <returns>An XML representation of the resource</returns>
        internal XmlDictionaryReader RefreshResource(ResourceObject resource)
        {
            return this.resourceClient.GetFullObjectForUpdate(resource);
        }

        /// <summary>
        /// Submits a resource template to the resource management service for creation
        /// </summary>
        /// <param name="resource">The resource template to create</param>
        internal void CreateResource(ResourceObject resource)
        {
            this.resourceFactoryClient.Create(resource);
        }

        /// <summary>
        /// Submits a resource template to the resource management service for update
        /// </summary>
        /// <param name="resource">The resource to update</param>
        internal void PutResource(ResourceObject resource)
        {
            this.resourceClient.Put(resource);
        }

        /// <summary>
        /// Initializes the WCF bindings, endpoints, and proxy objects
        /// </summary>
        private void InitializeClients()
        {
            this.resourceClient = new ResourceClient(ResourceManagementClient.WsHttpContextBinding, ResourceManagementClient.EndpointManager.ResourceEndpoint);
            this.resourceFactoryClient = new ResourceFactoryClient(ResourceManagementClient.WsHttpContextBinding, ResourceManagementClient.EndpointManager.ResourceFactoryEndpoint);
            this.searchClient = new SearchClient(ResourceManagementClient.WsHttpContextBinding, ResourceManagementClient.EndpointManager.SearchEndpoint);

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

            this.resourceClient.Initialize(this);
            this.resourceFactoryClient.Initialize(this);
            this.searchClient.Initialize(this);

            this.resourceClient.Open();
            this.resourceFactoryClient.Open();
            this.searchClient.Open();

            ResourceManagementSchema.LoadSchema();
        }
    }
}