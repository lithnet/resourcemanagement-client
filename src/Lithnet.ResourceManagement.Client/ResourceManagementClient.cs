namespace Lithnet.ResourceManagement.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Lithnet.ResourceManagement.Client.ResourceManagementService;
    using Microsoft.ResourceManagement.WebServices;
    using Microsoft.ResourceManagement.WebServices.WSEnumeration;
    using System.Threading;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using Lithnet.ResourceManagement.Client;
    using System.Globalization;

    /// <summary>
    /// The main class used to create, update, delete, and search for objects in the resource management service
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    /// <example>
    /// <code language="cs" title="Using the Resource Management Client" source="..\Lithnet.ResourceManagement.Client.Help.Examples\T_ResourceManagementClient.cs" />
    /// </example>
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
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
        private static Binding wsHttpContextBinding;

        /// <summary>
        /// The explicit credentials for this client
        /// </summary>
        private NetworkCredential creds;

        /// <summary>
        /// The endpoint manager for the client
        /// </summary>
        private EndpointManager endpointManager;

        /// <summary>
        /// Gets the username of the current user
        /// </summary>
        private string UserName
        {
            get
            {
                string value;

                if (this.creds != null)
                {
                    value = this.creds.UserName;
                }
                else
                {
                    value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }

                if (value.IndexOf("\\", StringComparison.Ordinal) >= 0)
                {
                    string[] split = value.Split('\\');
                    return split[1];
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Gets the domain of the current user
        /// </summary>
        private string Domain
        {
            get
            {
                string value;

                if (this.creds != null)
                {
                    value = this.creds.Domain ?? this.creds.UserName;
                }
                else
                {
                    value = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }

                if (value.IndexOf("\\", StringComparison.Ordinal) >= 0)
                {
                    string[] split = value.Split('\\');
                    return split[0];
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Gets the credentials used to connect to the resource management service as specified in the configuration file
        /// </summary>
        /// <remarks>All instances of the <c>ResourceManagementClient</c> that are not loaded with a specific set of credentials will use the credentials specified in the configuration file. If not credentials are specified in the file, then the credentials of the current user are used</remarks>
        private static NetworkCredential NetworkCredentials { get; set; }

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

            InitialResourceManagementClientDefaults();
        }

        private static void InitialResourceManagementClientDefaults()
        {
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
            ResourceManagementClient.wsHttpContextBinding = BindingManager.GetWsHttpContextBinding();

            if (!string.IsNullOrWhiteSpace(Configuration.Username))
            {
                ResourceManagementClient.NetworkCredentials = new NetworkCredential(Configuration.Username, Configuration.Password);
            }
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementClient class
        /// </summary>
        /// <example>
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClientCtorExamples.cs" region="ResourceManagementClient()"/>
        /// </example>
        public ResourceManagementClient()
        {
            this.InitializeClients(ResourceManagementClient.EndpointManager, ResourceManagementClient.NetworkCredentials, !Configuration.ForceKerberos);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementClient class
        /// </summary>
        /// <param name="credentials">The credentials to use to connect to the service</param>
        /// <example>
        /// The following example shows how to load an instance of the <c>ResourceManagementClient</c> using a specific set of credentials
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClientCtorExamples.cs" region="ResourceManagementClient(System.Net.NetworkCredentials)"/>
        /// </example>
        public ResourceManagementClient(NetworkCredential credentials)
        {
            this.InitializeClients(ResourceManagementClient.EndpointManager, credentials, !Configuration.ForceKerberos);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementClient class
        /// </summary>
        /// <param name="baseAddress">The full address of the Resource Management Service Endpoint</param>
        public ResourceManagementClient(string baseAddress)
        {
            EndpointManager e = new EndpointManager(baseAddress);
            this.InitializeClients(e, null, true);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementClient class
        /// </summary>
        /// <param name="baseAddress">The full address of the Resource Management Service Endpoint</param>
        /// <param name="credentials">The credentials to use to connect to the service</param>
        public ResourceManagementClient(string baseAddress, NetworkCredential credentials)
        {
            EndpointManager e = new EndpointManager(baseAddress);
            this.InitializeClients(e, credentials, true);
        }

        /// <summary>
        /// Initializes a new instance of the ResourceManagementClient class
        /// </summary>
        /// <param name="baseAddress">The URI of the Resource Management Service Endpoint</param>
        /// <param name="credentials">The credentials to use to connect to the service</param>
        /// <param name="servicePrincipalName">The service principal name of the Resource Management Service</param>
        /// <param name="allowNtlm">Specifies if NTLM authentication should be attempted if kerberos authentication fails</param>
        public ResourceManagementClient(Uri baseAddress, NetworkCredential credentials, string servicePrincipalName, bool allowNtlm)
        {
            EndpointIdentity id;

            if (servicePrincipalName != null)
            {
                id = EndpointIdentity.CreateSpnIdentity(servicePrincipalName);
            }
            else
            {
                id = EndpointIdentity.CreateSpnIdentity(string.Format("FIMService/{0}", baseAddress.Host));

            }

            EndpointManager e = new EndpointManager(baseAddress, id);
            this.InitializeClients(e, credentials, allowNtlm);
        }

        /// <summary>
        /// Deletes the specified resources from the resource management service as a single composite operation
        /// </summary>
        /// <param name="resourceIDs">A collection of object IDs in GUID format to delete to delete</param>
        /// <remarks>
        /// Note that when using this method, the objects are passed to the Resource Management Service in a single request. In the Request History in the portal, this will appear as a single request, and the individual object IDs that were deleted will not be visible.
        /// </remarks>
        /// <example>
        /// The following example deletes a set of objects using a list of known GUIDs
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_DeleteResourceExamples.cs" region="DeleteResources(IEnumerable{Guid})"/>
        /// </example>
        public void DeleteResources(IEnumerable<Guid> resourceIDs)
        {
            this.resourceClient.Delete(resourceIDs.Select(t => new UniqueIdentifier(t)));
        }

        /// <summary>
        /// Deletes the specified resources from the resource management service as a single composite operation
        /// </summary>
        /// <param name="resourceIDs">A collection of object IDs in string format to delete to delete</param>
        /// <remarks>
        /// Note that when using this method, the objects are passed to the Resource Management Service in a single request. In the Request History in the portal, this will appear as a single request, and the individual object IDs that were deleted will not be visible.
        /// </remarks>
        /// <example>
        /// The following example deletes a set of objects using a list of known GUIDs represented in string format
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_DeleteResourceExamples.cs" region="DeleteResources(IEnumerable{String})"/>
        /// </example>
        public void DeleteResources(IEnumerable<string> resourceIDs)
        {
            this.resourceClient.Delete(resourceIDs.Select(t => new UniqueIdentifier(t)));
        }

        /// <summary>
        /// Deletes the specified resources from the resource management service as a single composite operation
        /// </summary>
        /// <param name="resourceIDs">A collection of references to delete</param>
        /// <remarks>
        /// Note that when using this method, the objects are passed to the Resource Management Service in a single request. In the Request History in the portal, this will appear as a single request, and the individual object IDs that were deleted will not be visible.
        /// </remarks>
        /// <example>
        /// The following example gets all the members of a set, and passes the reference attribute containing the objects to the delete function
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_DeleteResourceExamples.cs" region="DeleteResources(IEnumerable{UniqueIdentifier})"/>
        /// </example>
        public void DeleteResources(IEnumerable<UniqueIdentifier> resourceIDs)
        {
            this.resourceClient.Delete(resourceIDs);
        }

        /// <summary>
        /// Deletes the specified resources from the resource management service as a single composite operation
        /// </summary>
        /// <param name="resources">A collection of resources to delete</param>
        /// <remarks>
        /// Note that when using this method, the objects are passed to the Resource Management Service in a single request. In the Request History in the portal, this will appear as a single request, and the individual object IDs that were deleted will not be visible.
        /// </remarks>
        /// <example>
        /// The following example performs a search for all Group objects, and passes the resulting enumerable to the delete function
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_DeleteResourceExamples.cs" region="DeleteResources(IEnumerable{ResourceObject})"/>
        /// </example>
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
        /// <example>
        /// The following example deletes the specified resource object
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_DeleteResourceExamples.cs" region="DeleteResource(ResourceObject)"/>
        /// </example>
        public void DeleteResource(ResourceObject resource)
        {
            this.DeleteResource(resource.ObjectID);
            resource.IsDeleted = true;
        }

        /// <summary>
        /// Deletes the specified resource from the resource management service
        /// </summary>
        /// <param name="id">The reference to the object to delete</param>
        /// <example>
        /// The following example shows how to delete an object using a reference attribute
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_DeleteResourceExamples.cs" region="DeleteResource(UniqueIdentifier)"/>
        /// </example>
        public void DeleteResource(UniqueIdentifier id)
        {
            this.resourceClient.Delete(id);
        }

        /// <summary>
        /// Deletes the specified resource from the resource management service
        /// </summary>
        /// <param name="id">The ID of the object to delete</param>
        /// <example>
        /// The following example shows how to delete an object using a known GUID
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_DeleteResourceExamples.cs" region="DeleteResource(Guid)"/>
        /// </example>
        public void DeleteResource(Guid id)
        {
            this.DeleteResource(new UniqueIdentifier(id));
        }

        /// <summary>
        /// Deletes the specified resource from the resource management service
        /// </summary>
        /// <param name="id">The ID of the object to delete, in GUID format, with or without the urn: prefix</param>
        /// <example>
        /// The following example shows how to delete an object using a string representation of a GUID
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_DeleteResourceExamples.cs" region="DeleteResource(String)"/>
        /// </example>
        public void DeleteResource(string id)
        {
            this.DeleteResource(new UniqueIdentifier(id));
        }

        /// <summary>
        /// Saves the specified resources in the resource management service. Updates and Adds are committed as a single composite operation.
        /// </summary>
        /// <param name="resources">The resources to update</param>
        public void SaveResources(params ResourceObject[] resources)
        {
            this.SaveResources(resources);
        }

        /// <summary>
        /// Saves the specified resources in the resource management service. Updates and Adds are committed as a single composite operation.
        /// </summary>
        /// <param name="resources">The collection of resources to update</param>
        public void SaveResources(IEnumerable<ResourceObject> resources)
        {
            IList<ResourceObject> resourceObjects = resources as IList<ResourceObject> ?? resources.ToList();

            if (resourceObjects.Any(t => t.Locale != null))
            {
                throw new InvalidOperationException("Cannot perform a composite save on a localized resource");
            }

            List<ResourceObject> objectsToCreate = resourceObjects.Where(t => t.ModificationType == OperationType.Create).ToList();
            if (objectsToCreate.Count > 0)
            {
                if (objectsToCreate.Count > 1)
                {
                    this.CreateResources(objectsToCreate);
                }
                else
                {
                    this.CreateResource(objectsToCreate.FirstOrDefault());
                }
            }

            List<ResourceObject> objectsToDelete = resourceObjects.Where(t => t.ModificationType == OperationType.Delete).ToList();
            List<ResourceObject> objectsToUpdate = resourceObjects.Where(t => t.ModificationType == OperationType.Update).ToList();

            if (objectsToDelete.Count > 0)
            {
                this.DeleteResources(objectsToDelete);
            }

            if (objectsToUpdate.Count > 0)
            {
                this.resourceClient.Put(objectsToUpdate);

                foreach (ResourceObject resource in objectsToUpdate)
                {
                    resource.CommitChanges();
                }
            }
        }

        /// <summary>
        /// Saves the specified resources in the resource management service. Each update is performed on its own thread
        /// </summary>
        /// <remarks>
        /// This method will reorder the operations to perform creates first, followed by updates and finally deletes
        /// </remarks>
        /// <param name="resources">The resources to save</param>
        public void SaveResourcesParallel(IEnumerable<ResourceObject> resources)
        {
            this.SaveResourcesParallel(resources, 0, null);

        }

        /// <summary>
        /// Saves the specified resources in the resource management service. Each update is performed on its own thread
        /// </summary>
        /// <remarks>
        /// This method will reorder the operations to perform creates first, followed by updates and finally deletes
        /// </remarks>
        /// <param name="maxDegreeOfParallelism">The maximum number of threads to use for the operation</param>
        /// <param name="resources">The resources to save</param>
        public void SaveResourcesParallel(IEnumerable<ResourceObject> resources, int maxDegreeOfParallelism)
        {
            this.SaveResourcesParallel(resources, maxDegreeOfParallelism, null);
        }

        /// <summary>
        /// Saves the specified resources in the resource management service. Each update is performed on its own thread
        /// </summary>
        /// <remarks>
        /// This method will reorder the operations to perform creates first, followed by updates and finally deletes
        /// </remarks>
        /// <param name="maxDegreeOfParallelism">The maximum number of threads to use for the operation</param>
        /// <param name="locale">The localization culture to use when saving the object</param>
        /// <param name="resources">The resources to save</param>
        public void SaveResourcesParallel(IEnumerable<ResourceObject> resources, int maxDegreeOfParallelism, CultureInfo locale)
        {
            ConcurrentQueue<ResourceObject> createResources = new ConcurrentQueue<ResourceObject>(resources.Where(t => t.ModificationType == OperationType.Create));
            ConcurrentQueue<ResourceObject> updateResources = new ConcurrentQueue<ResourceObject>(resources.Where(t => t.ModificationType == OperationType.Update));
            ConcurrentQueue<ResourceObject> deleteResources = new ConcurrentQueue<ResourceObject>(resources.Where(t => t.ModificationType == OperationType.Delete));

            ParallelOptions op = new ParallelOptions();
            if (maxDegreeOfParallelism > 0)
            {
                op.MaxDegreeOfParallelism = maxDegreeOfParallelism;
            }

            Parallel.ForEach(createResources, op, (resource =>
            {
                this.SaveResource(resource, locale);
            }));

            Parallel.ForEach(updateResources, op, (resource =>
            {
                this.SaveResource(resource, locale);
            }));

            Parallel.ForEach(deleteResources, op, (resource =>
            {
                this.SaveResource(resource, locale);
            }));
        }

        /// <summary>
        /// Saves the specified resource in the resource management service
        /// </summary>
        /// <param name="resource">The resource to save</param>
        public void SaveResource(ResourceObject resource)
        {
            this.SaveResource(resource, null);
        }

        /// <summary>
        /// Saves the specified resource in the resource management service
        /// </summary>
        /// <param name="resource">The resource to save</param>
        /// <param name="locale">The localization culture to use when saving the object</param>
        public void SaveResource(ResourceObject resource, CultureInfo locale)
        {
            switch (resource.ModificationType)
            {
                case OperationType.None:
                    return;

                case OperationType.Create:
                    this.CreateResource(resource);
                    break;

                case OperationType.Update:
                    this.PutResource(resource, locale);
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
        /// <example>
        /// The following example shows how to create a new resource in the Resource Management Service
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_CreateResourceExamples.cs" region="CreateResource(String)"/>
        /// </example>
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
        /// <example>
        /// The following example shows how to create a blank template of an object used to update the Resource Management Service
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_CreateResourceExamples.cs" region="CreateResourceTemplateForUpdate(String, UniqueIdentifier)"/>
        /// </example>
        public ResourceObject CreateResourceTemplateForUpdate(string objectType, UniqueIdentifier id)
        {
            return new ResourceObject(objectType, id, this);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a known GUID value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(Guid)"/>
        /// </example>
        public ResourceObject GetResource(Guid id)
        {
            return this.GetResource(new UniqueIdentifier(id), null, null);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(Guid id, CultureInfo locale)
        {
            return this.GetResource(new UniqueIdentifier(id), null, locale);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a known GUID value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(Guid)"/>
        /// </example>
        public ResourceObject GetResource(Guid id, IEnumerable<string> attributesToGet)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet);
        }


        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(Guid id, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet, locale);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a known GUID value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(String)"/>
        /// </example>
        public ResourceObject GetResource(string id)
        {
            return this.GetResource(new UniqueIdentifier(id), null, null);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <param name="getPermissionHints">Gets the permission hints for each attribute of the resource</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a known GUID value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(String)"/>
        /// </example>
        public ResourceObject GetResource(string id, bool getPermissionHints)
        {
            return this.GetResource(new UniqueIdentifier(id), null, null, getPermissionHints);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(string id, CultureInfo locale)
        {
            return this.GetResource(new UniqueIdentifier(id), null, locale);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <param name="getPermissionHints">Gets the permission hints for each attribute of the resource</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(string id, CultureInfo locale, bool getPermissionHints)
        {
            return this.GetResource(new UniqueIdentifier(id), null, locale, getPermissionHints);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a known GUID value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(String)"/>
        /// </example>
        public ResourceObject GetResource(string id, IEnumerable<string> attributesToGet)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="getPermissionHints">Gets the permission hints for each attribute of the resource</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a known GUID value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(String)"/>
        /// </example>
        public ResourceObject GetResource(string id, IEnumerable<string> attributesToGet, bool getPermissionHints)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet, getPermissionHints);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(string id, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet, locale);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get as a GUID in string format</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <param name="getPermissionHints">Gets the permission hints for each attribute of the resource</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(string id, IEnumerable<string> attributesToGet, CultureInfo locale, bool getPermissionHints)
        {
            return this.GetResource(new UniqueIdentifier(id), attributesToGet, locale, getPermissionHints);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a reference value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(UniqueIdentifier)"/>
        /// </example>
        public ResourceObject GetResource(UniqueIdentifier id)
        {
            return this.resourceClient.Get(id, null, null, false);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="getPermissionHints">Gets the permission hints for each attribute of the resource</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a reference value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(UniqueIdentifier)"/>
        /// </example>
        public ResourceObject GetResource(UniqueIdentifier id, bool getPermissionHints)
        {
            return this.resourceClient.Get(id, null, null, getPermissionHints);
        }

        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(UniqueIdentifier id, CultureInfo locale)
        {
            return this.resourceClient.Get(id, null, locale, false);
        }


        /// <summary>
        /// Gets a resource from the resource management service, retrieving all attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <param name="getPermissionHints">Gets the permission hints for each attribute of the resource</param>
        /// <returns>The resource represented by the specified ID</returns>
        public ResourceObject GetResource(UniqueIdentifier id, CultureInfo locale, bool getPermissionHints)
        {
            return this.resourceClient.Get(id, null, locale, getPermissionHints);
        }


        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a reference value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(UniqueIdentifier)"/>
        /// </example>
        public ResourceObject GetResource(UniqueIdentifier id, IEnumerable<string> attributesToGet)
        {
            return this.resourceClient.Get(id, attributesToGet, null, false);
        }


        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="getPermissionHints">Gets the permission hints for each attribute of the resource</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a reference value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(UniqueIdentifier)"/>
        /// </example>
        public ResourceObject GetResource(UniqueIdentifier id, IEnumerable<string> attributesToGet, bool getPermissionHints)
        {
            return this.resourceClient.Get(id, attributesToGet, null, getPermissionHints);
        }


        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a reference value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(UniqueIdentifier)"/>
        /// </example>
        public ResourceObject GetResource(UniqueIdentifier id, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            return this.resourceClient.Get(id, attributesToGet, locale, false);
        }


        /// <summary>
        /// Gets a resource from the resource management service, retrieving only a specified set of attributes for the resource
        /// </summary>
        /// <param name="id">The ID of the resource to get</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <param name="getPermissionHints">Gets the permission hints for each attribute of the resource</param>
        /// <returns>The resource represented by the specified ID</returns>
        /// <example>
        /// The following example shows how to get an object from a reference value
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceExamples.cs" region="GetResource(UniqueIdentifier)"/>
        /// </example>
        public ResourceObject GetResource(UniqueIdentifier id, IEnumerable<string> attributesToGet, CultureInfo locale, bool getPermissionHints)
        {
            return this.resourceClient.Get(id, attributesToGet, locale, getPermissionHints);
        }


        /// <summary>
        /// Gets a resource from the resource management service using a unique attribute and value combination, retrieving all attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attributeName">The name of the attribute used as the key</param>
        /// <param name="value">The value of the attribute</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        /// <example>
        /// The following example shows how get a user by its AccountName attribute
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceByKeyExamples.cs" region="GetResourceByKey(String, String, String)"/>
        /// </example>
        public ResourceObject GetResourceByKey(string objectType, string attributeName, object value)
        {
            return this.GetResourceByKey(objectType, attributeName, value, null, null);
        }

        /// <summary>
        /// Gets a resource from the resource management service using a unique attribute and value combination, retrieving all attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attributeName">The name of the attribute used as the key</param>
        /// <param name="value">The value of the attribute</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        /// <example>
        /// The following example shows how get a user by its AccountName attribute
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceByKeyExamples.cs" region="GetResourceByKey(String, String, String)"/>
        /// </example>
        public ResourceObject GetResourceByKey(string objectType, string attributeName, object value, CultureInfo locale)
        {
            return this.GetResourceByKey(objectType, attributeName, value, null, locale);
        }

        /// <summary>
        /// Gets a resource from the resource management service using a unique attribute and value combination, retrieving the specified attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attributeName">The name of the attribute used as the key</param>
        /// <param name="value">The value of the attribute</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        /// <example>
        /// The following example shows how get a user by its AccountName attribute
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceByKeyExamples.cs" region="GetResourceByKey(String, String, String)"/>
        /// </example>
        public ResourceObject GetResourceByKey(string objectType, string attributeName, object value, IEnumerable<string> attributesToGet)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(attributeName, value);

            return this.GetResourceByKey(objectType, values, attributesToGet);
        }

        /// <summary>
        /// Gets a resource from the resource management service using a unique attribute and value combination, retrieving the specified attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attributeName">The name of the attribute used as the key</param>
        /// <param name="value">The value of the attribute</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        /// <example>
        /// The following example shows how get a user by its AccountName attribute
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceByKeyExamples.cs" region="GetResourceByKey(String, String, String)"/>
        /// </example>
        public ResourceObject GetResourceByKey(string objectType, string attributeName, object value, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(attributeName, value);

            return this.GetResourceByKey(objectType, values, attributesToGet, locale);
        }

        /// <summary>
        /// Gets a resource from the resource management service using a set of unique attribute and value combinations, retrieving all attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attributeValuePairs">A list of attribute value pairs that make this object unique</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        /// <example>
        /// The following example shows how get a user by using the AccountName and Domain pair of anchor attributes
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceByKeyExamples.cs" region="GetResourceByKey(String, Dictionary{String, String})"/>
        /// </example>
        public ResourceObject GetResourceByKey(string objectType, Dictionary<string, object> attributeValuePairs)
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
        /// <example>
        /// The following example shows how get a user by using the AccountName and Domain pair of anchor attributes
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceByKeyExamples.cs" region="GetResourceByKey(String, Dictionary{String, String})"/>
        /// </example>
        public ResourceObject GetResourceByKey(string objectType, Dictionary<string, object> attributeValuePairs, IEnumerable<string> attributesToGet)
        {
            return this.GetResourceByKey(objectType, attributeValuePairs, attributesToGet, null);
        }

        /// <summary>
        /// Gets a resource from the resource management service using a set of unique attribute and value combinations, retrieving the specified attributes for the resource
        /// </summary>
        /// <param name="objectType">The type of object to retrieve</param>
        /// <param name="attributeValuePairs">A list of attribute value pairs that make this object unique</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the object</param>
        /// <returns>A resource that matches the specified criteria, or null of no object was found</returns>
        /// <exception cref="TooManyResultsException">The method will throw this exception when more that one match was found for the specified criteria</exception>
        /// <example>
        /// The following example shows how get a user by using the AccountName and Domain pair of anchor attributes
        /// <code language="cs" title="Example" source="..\Lithnet.ResourceManagement.Client.Help.Examples\ResourceManagementClient_GetResourceByKeyExamples.cs" region="GetResourceByKey(String, Dictionary{String, String})"/>
        /// </example>
        public ResourceObject GetResourceByKey(string objectType, Dictionary<string, object> attributeValuePairs, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            string filter = XPathFilterBuilder.CreateFilter(objectType, attributeValuePairs, ComparisonOperator.Equals, GroupOperator.And);

            if (attributesToGet == null)
            {
                attributesToGet = ResourceManagementSchema.GetObjectType(objectType).Attributes.Select(t => t.SystemName);
            }

            ISearchResultCollection results = this.searchClient.EnumerateSync(filter, 1, attributesToGet, null, locale);

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
            return this.searchClient.EnumerateSync(filter, -1, null, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the default page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, CultureInfo locale)
        {
            return this.searchClient.EnumerateSync(filter, -1, null, null, locale);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the default page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, IEnumerable<string> attributesToGet)
        {
            return this.searchClient.EnumerateSync(filter, -1, attributesToGet, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the default page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            return this.searchClient.EnumerateSync(filter, -1, attributesToGet, null, locale);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttribute">The name of the attribute to sort the search results by</param>
        /// <param name="sortAscending">Indicates if the attribute sort order should be ascending or descending</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, IEnumerable<string> attributesToGet, string sortAttribute, bool sortAscending)
        {
            if (string.IsNullOrWhiteSpace(sortAttribute))
            {
                throw new ArgumentNullException("sortAttribute");
            }

            SortingAttribute attribute = new SortingAttribute(sortAttribute, sortAscending);
            return this.searchClient.EnumerateSync(filter, -1, attributesToGet, new SortingAttribute[] { attribute }, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateSync(filter, -1, attributesToGet, sortAttributes, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes, CultureInfo locale)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateSync(filter, -1, attributesToGet, sortAttributes, locale);
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
            return this.searchClient.EnumerateSync(filter, pageSize, null, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, int pageSize, IEnumerable<string> attributesToGet)
        {
            return this.searchClient.EnumerateSync(filter, pageSize, attributesToGet, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, int pageSize, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            return this.searchClient.EnumerateSync(filter, pageSize, attributesToGet, null, locale);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttribute">The name of the attribute to sort the search results by</param>
        /// <param name="sortAscending">Indicates if the attribute sort order should be ascending or descending</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, int pageSize, IEnumerable<string> attributesToGet, string sortAttribute, bool sortAscending)
        {
            if (string.IsNullOrWhiteSpace(sortAttribute))
            {
                throw new ArgumentNullException(nameof(sortAttribute));
            }

            SortingAttribute attribute = new SortingAttribute(sortAttribute, sortAscending);
            return this.searchClient.EnumerateSync(filter, pageSize, attributesToGet, new SortingAttribute[] { attribute }, null);
        }

        /// <summary>
        /// Returns approval requests of the specified type for the currently connected user
        /// </summary>
        /// <param name="status">Specifies the types of approvals to return</param>
        /// <returns>A collection of pending approvals</returns>
        public ISearchResultCollection GetApprovals(ApprovalStatus status)
        {
            string approvalStatusString = string.Empty;

            if (status != ApprovalStatus.Unknown)
            {
                approvalStatusString = $"ApprovalStatus = '{status}' and ";
            }

            if (this.UserName == null || this.Domain == null)
            {
                throw new InvalidOperationException("The username or domain parameters were unknown");
            }

            string xpath = $"/Approval[{approvalStatusString}Approver=/Person[AccountName = '{this.UserName}' and Domain = '{this.Domain}']]";
            return this.GetResources(xpath, ResourceManagementSchema.ObjectTypes[ObjectTypeNames.Approval].Attributes.Select(t => t.SystemName));
        }

        /// <summary>
        /// Returns approval requests of the specified type for the specified user
        /// </summary>
        /// <param name="status">Specifies the types of approvals to return</param>
        /// <param name="userID">The unique identifer of the user</param>
        /// <returns>A collection of pending approvals</returns>
        public ISearchResultCollection GetApprovals(ApprovalStatus status, UniqueIdentifier userID)
        {
            string approvalStatusString = string.Empty;

            if (status != ApprovalStatus.Unknown)
            {
                approvalStatusString = $"ApprovalStatus = '{status}' and ";
            }

            string xpath = $"/Approval[{approvalStatusString}Approver='{userID.Value}']";
            return this.GetResources(xpath, ResourceManagementSchema.ObjectTypes[ObjectTypeNames.Approval].Attributes.Select(t => t.SystemName));
        }

        /// <summary>
        /// Approves or rejects a pending request
        /// </summary>
        /// <remarks>It is recommended to use the <see cref="GetApprovals(ApprovalStatus)"/> method to obtain the pending approval request</remarks>
        /// <param name="approvalRequest">The approval object to process.The object must be in the 'pending' state</param>
        /// <param name="approve">A value indicating is the request should be approved</param>
        /// <param name="reason">An optional reason for the approval or rejection</param>
        public void Approve(ResourceObject approvalRequest, bool approve, string reason = null)
        {
            if (approvalRequest.ObjectTypeName != ObjectTypeNames.Approval)
            {
                throw new ArgumentException("The specified object was not an approval request", nameof(approvalRequest));
            }

            if (!approvalRequest.Attributes.ContainsAttribute(AttributeNames.WorkflowInstance))
            {
                throw new ArgumentException("The approval object did not contain the 'WorkflowInstance' attribute value. Ensure the attribute was selected as part of the GET request");
            }

            if (!approvalRequest.Attributes.ContainsAttribute(AttributeNames.EndpointAddress))
            {
                throw new ArgumentException("The approval object did not contain the 'EndpointAddress' attribute value. Ensure the attribute was selected as part of the GET request");
            }

            if (!approvalRequest.Attributes.ContainsAttribute(AttributeNames.ApprovalStatus))
            {
                throw new ArgumentException("The approval object did not contain the 'EndpointAddress' attribute value. Ensure the attribute was selected as part of the GET request");
            }

            if (approvalRequest.Attributes[AttributeNames.ApprovalStatus].StringValue != "Pending")
            {
                throw new InvalidOperationException("The specified approval request is not in a pending state");
            }

            UniqueIdentifier approval = approvalRequest.Attributes[AttributeNames.ObjectID].ReferenceValue;
            UniqueIdentifier workflowInstance = approvalRequest.Attributes[AttributeNames.WorkflowInstance].ReferenceValue;
            string endpointAddress = approvalRequest.Attributes[AttributeNames.EndpointAddress].StringValues.FirstOrDefault(t => t.StartsWith("http", StringComparison.OrdinalIgnoreCase));

            if (endpointAddress == null)
            {
                throw new InvalidOperationException("The endpoint address was not of a supported type");
            }

            using (ResourceFactoryClient client = this.CreateApprovalClient(endpointAddress))
            {
                client.Approve(workflowInstance, approval, approve, reason);
            }
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, int pageSize, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateSync(filter, pageSize, attributesToGet, sortAttributes, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, synchronously, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResources(string filter, int pageSize, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes, CultureInfo locale)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateSync(filter, pageSize, attributesToGet, sortAttributes, locale);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter)
        {
            return this.searchClient.EnumerateAsync(filter, -1, null, null, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, CultureInfo locale)
        {
            return this.searchClient.EnumerateAsync(filter, -1, null, null, locale, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet)
        {
            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, null, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, null, locale, null);
        }


        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttribute">The name of the attribute to sort the search results by</param>
        /// <param name="sortAscending">Indicates if the attribute sort order should be ascending or descending</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet, string sortAttribute, bool sortAscending)
        {
            if (string.IsNullOrWhiteSpace(sortAttribute))
            {
                throw new ArgumentNullException("sortAttribute");
            }

            SortingAttribute attribute = new SortingAttribute(sortAttribute, sortAscending);
            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, new SortingAttribute[] { attribute }, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, sortAttributes, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes, CultureInfo locale)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, sortAttributes, locale, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize)
        {
            return this.searchClient.EnumerateAsync(filter, pageSize, null, null, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, CultureInfo locale)
        {
            return this.searchClient.EnumerateAsync(filter, pageSize, null, null, locale, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet)
        {
            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, null, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, null, locale, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttribute">The name of the attribute to sort the search results by</param>
        /// <param name="sortAscending">Indicates if the attribute sort order should be ascending or descending</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet, string sortAttribute, bool sortAscending)
        {
            if (string.IsNullOrWhiteSpace(sortAttribute))
            {
                throw new ArgumentNullException("sortAttribute");
            }

            SortingAttribute attribute = new SortingAttribute(sortAttribute, sortAscending);
            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, new SortingAttribute[] { attribute }, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, sortAttributes, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes, CultureInfo locale)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, sortAttributes, locale, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.EnumerateAsync(filter, -1, null, null, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, CultureInfo locale, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.EnumerateAsync(filter, -1, null, null, locale, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, null, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the default page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet, CultureInfo locale, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, null, locale, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttribute">The name of the attribute to sort the search results by</param>
        /// <param name="sortAscending">Indicates if the attribute sort order should be ascending or descending</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet, string sortAttribute, bool sortAscending, CancellationTokenSource cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sortAttribute))
            {
                throw new ArgumentNullException("sortAttribute");
            }

            SortingAttribute attribute = new SortingAttribute(sortAttribute, sortAscending);
            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, new SortingAttribute[] { attribute }, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes, CancellationTokenSource cancellationToken)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, sortAttributes, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes, CultureInfo locale, CancellationTokenSource cancellationToken)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateAsync(filter, -1, attributesToGet, sortAttributes, locale, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.EnumerateAsync(filter, pageSize, null, null, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size
        /// WARNING: Due to the way that the resource management client processes unconstrained XPath queries, this function can cause excessive load on the underlying database. If the object type or attributes are known, use the overload of this function that supports specifying the attributes to get
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, CultureInfo locale, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.EnumerateAsync(filter, pageSize, null, null, locale, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, null, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet, CultureInfo locale, CancellationTokenSource cancellationToken)
        {
            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, null, locale, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttribute">The name of the attribute to sort the search results by</param>
        /// <param name="sortAscending">Indicates if the attribute sort order should be ascending or descending</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet, string sortAttribute, bool sortAscending, CancellationTokenSource cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sortAttribute))
            {
                throw new ArgumentNullException("sortAttribute");
            }

            SortingAttribute attribute = new SortingAttribute(sortAttribute, sortAscending);
            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, new SortingAttribute[] { attribute }, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes, CancellationTokenSource cancellationToken)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, sortAttributes, null, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, retrieving all results asynchronously on a separate thread, using the specified page size, and retrieving the specified attributes
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <param name="cancellationToken">A cancellation object that can be used to terminate the search</param>
        /// <returns>A collection of matching resource objects</returns>
        public ISearchResultCollection GetResourcesAsync(string filter, int pageSize, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes, CultureInfo locale, CancellationTokenSource cancellationToken)
        {
            if (sortAttributes == null)
            {
                throw new ArgumentNullException("sortAttributes");
            }

            return this.searchClient.EnumerateAsync(filter, pageSize, attributesToGet, sortAttributes, locale, cancellationToken);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, using a SearchResultPager to navigate through the result set
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <returns>An object that can be used to navigate through the search results</returns>
        public SearchResultPager GetResourcesPaged(string filter, int pageSize)
        {
            return this.searchClient.EnumeratePaged(filter, pageSize, null, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, using a SearchResultPager to navigate through the result set
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>An object that can be used to navigate through the search results</returns>
        public SearchResultPager GetResourcesPaged(string filter, int pageSize, CultureInfo locale)
        {
            return this.searchClient.EnumeratePaged(filter, pageSize, null, null, locale);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, using a SearchResultPager to navigate through the result set
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <returns>An object that can be used to navigate through the search results</returns>
        public SearchResultPager GetResourcesPaged(string filter, int pageSize, IEnumerable<string> attributesToGet)
        {
            return this.searchClient.EnumeratePaged(filter, pageSize, attributesToGet, null, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, using a SearchResultPager to navigate through the result set
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>An object that can be used to navigate through the search results</returns>
        public SearchResultPager GetResourcesPaged(string filter, int pageSize, IEnumerable<string> attributesToGet, CultureInfo locale)
        {
            return this.searchClient.EnumeratePaged(filter, pageSize, attributesToGet, null, locale);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, using a SearchResultPager to navigate through the result set
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <returns>An object that can be used to navigate through the search results</returns>
        public SearchResultPager GetResourcesPaged(string filter, int pageSize, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes)
        {
            return this.searchClient.EnumeratePaged(filter, pageSize, attributesToGet, sortAttributes, null);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, using a SearchResultPager to navigate through the result set
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttributes">A collection of attribute names and sort directions to order the results with</param>
        /// <param name="locale">The culture to use to request a localized version of the objects</param>
        /// <returns>An object that can be used to navigate through the search results</returns>
        public SearchResultPager GetResourcesPaged(string filter, int pageSize, IEnumerable<string> attributesToGet, IEnumerable<SortingAttribute> sortAttributes, CultureInfo locale)
        {
            return this.searchClient.EnumeratePaged(filter, pageSize, attributesToGet, sortAttributes, locale);
        }

        /// <summary>
        /// Uses the specified XPath filter to find matching objects in the resource management service, using a SearchResultPager to navigate through the result set
        /// </summary>
        /// <param name="filter">The XPath filter defining the search criteria</param>
        /// <param name="pageSize">The number of results to request from the server at a time</param>
        /// <param name="attributesToGet">The list of attributes to retrieve</param>
        /// <param name="sortAttribute">The name of the attribute to sort the search results by</param>
        /// <param name="sortAscending">Indicates if the attribute sort order should be ascending or descending</param>
        /// <returns>An object that can be used to navigate through the search results</returns>
        public SearchResultPager GetResourcesPaged(string filter, int pageSize, IEnumerable<string> attributesToGet, string sortAttribute, bool sortAscending)
        {
            if (string.IsNullOrWhiteSpace(sortAttribute))
            {
                throw new ArgumentNullException("sortAttribute");
            }

            SortingAttribute attribute = new SortingAttribute(sortAttribute, sortAscending);
            return this.searchClient.EnumeratePaged(filter, pageSize, attributesToGet, new SortingAttribute[] { attribute }, null);
        }

        /// <summary>
        /// Gets the number of resources that match a specified criteria
        /// </summary>
        /// <param name="filter">The XPath filter to use in the search</param>
        /// <returns>The number of resources that match the specified criteria</returns>
        public int GetResourceCount(string filter)
        {
            ISearchResultCollection result = this.searchClient.EnumerateSync(filter, 0, new List<string>(), null, null);
            return result.Count;
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
        /// Submits a list of resources template to the resource management service for creation
        /// </summary>
        /// <param name="resources">A collection of resource objects to create</param>
        internal void CreateResources(IEnumerable<ResourceObject> resources)
        {
            this.resourceFactoryClient.Create(resources);
        }

        /// <summary>
        /// Submits a resource template to the resource management service for update
        /// </summary>
        /// <param name="resource">The resource to update</param>
        /// <param name="locale">The culture to use when saving a localized version of the object</param>
        internal void PutResource(ResourceObject resource, CultureInfo locale)
        {
            this.resourceClient.Put(resource, locale);
        }

        /// <summary>
        /// Reloads the schema from the Resource Management Service
        /// </summary>
        public void RefreshSchema()
        {
            ResourceManagementSchema.RefreshSchema(this.endpointManager);
        }

        /// <summary>
        /// Initializes the WCF bindings, endpoints, and proxy objects
        /// </summary>
        private void InitializeClients(EndpointManager endpointManager, NetworkCredential credentials, bool allowNtlm)
        {
            this.endpointManager = endpointManager;

            this.resourceClient = new ResourceClient(ResourceManagementClient.wsHttpContextBinding, this.endpointManager.ResourceEndpoint);
            this.resourceFactoryClient = new ResourceFactoryClient(ResourceManagementClient.wsHttpContextBinding, this.endpointManager.ResourceFactoryEndpoint);
            this.searchClient = new SearchClient(ResourceManagementClient.wsHttpContextBinding, this.endpointManager.SearchEndpoint);
            this.creds = credentials;

            if (credentials != null)
            {
                this.resourceClient.ClientCredentials.Windows.ClientCredential = credentials;
                this.resourceFactoryClient.ClientCredentials.Windows.ClientCredential = credentials;
                this.searchClient.ClientCredentials.Windows.ClientCredential = credentials;
            }

            this.resourceClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
            this.resourceFactoryClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
            this.searchClient.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
#pragma warning disable 0618
            this.resourceClient.ClientCredentials.Windows.AllowNtlm = allowNtlm;
            this.resourceFactoryClient.ClientCredentials.Windows.AllowNtlm = allowNtlm;
            this.searchClient.ClientCredentials.Windows.AllowNtlm = allowNtlm;
#pragma warning restore 0618

            this.resourceClient.Initialize(this);
            this.resourceFactoryClient.Initialize(this);
            this.searchClient.Initialize(this);

            this.resourceClient.Open();
            this.resourceFactoryClient.Open();
            this.searchClient.Open();

            ResourceManagementSchema.LoadSchema(endpointManager);
        }

        private ResourceFactoryClient CreateApprovalClient(string endpoint)
        {
            return this.CreateApprovalClient(EndpointManager.EndpointFromAddress(endpoint));
        }

        private ResourceFactoryClient CreateApprovalClient(EndpointAddress endpoint)
        {
            ResourceFactoryClient client = new ResourceFactoryClient(ResourceManagementClient.wsHttpContextBinding, endpoint);

            if (this.creds != null)
            {
                client.ClientCredentials.Windows.ClientCredential = this.creds;
            }

#pragma warning disable 0618
            client.ClientCredentials.Windows.AllowNtlm = this.resourceFactoryClient.ClientCredentials.Windows.AllowNtlm;
#pragma warning restore 0618

            client.Initialize(this);
            client.Open();

            return client;
        }
    }
}
