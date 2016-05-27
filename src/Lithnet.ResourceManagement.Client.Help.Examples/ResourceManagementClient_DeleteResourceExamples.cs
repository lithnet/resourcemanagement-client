using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client.Help.Examples
{
    class ResourceManagementClient_DeleteResourceExamples
    {
        #region DeleteResource(ResourceObject)
        public void DeleteResourceObject()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            // Get the object to delete
            ResourceObject resource = client.GetResource("2ec7fcd4-8461-4d0b-8e76-3192c12ca48d");

            try
            {
                client.DeleteResource(resource);
            }
            catch (Exception)
            {
                // An error occurred while deleting the resource
                throw;
            }
        }
        #endregion

        #region DeleteResource(Guid)
        public void DeleteResourceByGuid()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            // Get the object to delete
            Guid objectID = new Guid("2ec7fcd4-8461-4d0b-8e76-3192c12ca48d");

            try
            {
                client.DeleteResource(objectID);
            }
            catch (Exception)
            {
                // An error occurred while deleting the resource
                throw;
            }
        }
        #endregion

        #region DeleteResource(String)
        public void DeleteResourceByString()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            try
            {
                client.DeleteResource("2ec7fcd4-8461-4d0b-8e76-3192c12ca48d");
            }
            catch (Exception)
            {
                // An error occurred while deleting the resource
                throw;
            }
        }
        #endregion

        #region DeleteResource(UniqueIdentifier)
        public void DeleteResourceByReference()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            // Get the object with the reference to another object to delete
            ResourceObject resource = client.GetResourceByKey("Person", "AccountName", "user0001");

            if (resource == null)
            {
                return;
            }

            try
            {
                // Delete the object referenced by the 'Manager' attribute on person 'user00001';
                client.DeleteResource(resource.Attributes["Manager"].ReferenceValue);
            }
            catch (Exception)
            {
                // An error occurred while deleting the resource
                throw;
            }
        }
        #endregion

        #region DeleteResources(IEnumerable{ResourceObject})
        public void DeleteAllGroups()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            // We are performing a delete, so we are only interested in the object ID and object type
            List<string> attributesToGet = new List<string>() { "ObjectID", "ObjectType" };

            // Get all the group objects to delete
            ISearchResultCollection resources = client.GetResources("/Group", attributesToGet);

            try
            {
                client.DeleteResources(resources);
            }
            catch (Exception)
            {
                // An error occurred while deleting the resources
                throw;
            }
        }
        #endregion

        #region DeleteResources(IEnumerable{UniqueIdentifier})
        public void DeleteSetMembers()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            ResourceObject resource = client.GetResourceByKey("Set", "DisplayName", "UsersToDelete");

            try
            {
                client.DeleteResources(resource.Attributes["ComputedMember"].ReferenceValues);
            }
            catch (Exception)
            {
                // An error occurred while deleting the resources
                throw;
            }
        }
        #endregion

        #region DeleteResources(IEnumerable{String})
        public void DeleteResourcesByGuidString()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            List<string> objectsToDelete = new List<string>();
            objectsToDelete.Add("71fa5e6b-3f79-4e62-83fe-e6c695edf918");
            objectsToDelete.Add("d2fbed50-caa1-4721-ba77-d0ed51c7fdf7");
            objectsToDelete.Add("adf184ba-aa6d-4691-aea6-55aeee57ea7a");

            try
            {
                client.DeleteResources(objectsToDelete);
            }
            catch (Exception)
            {
                // An error occurred while deleting the resources
                throw;
            }
        }
        #endregion

        #region DeleteResources(IEnumerable{Guid})
        public void DeleteResourcesByGuid()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            List<Guid> objectsToDelete = new List<Guid>();
            objectsToDelete.Add(new Guid("71fa5e6b-3f79-4e62-83fe-e6c695edf918"));
            objectsToDelete.Add(new Guid("d2fbed50-caa1-4721-ba77-d0ed51c7fdf7"));
            objectsToDelete.Add(new Guid("adf184ba-aa6d-4691-aea6-55aeee57ea7a"));

            try
            {
                client.DeleteResources(objectsToDelete);
            }
            catch (Exception)
            {
                // An error occurred while deleting the resources
                throw;
            }
        }
        #endregion
    }
}
