using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.WebServices;

 namespace Lithnet.ResourceManagement.Client.Help.Examples
{
    class ResourceManagementClient_GetResourceExamples
    {
        #region GetResource(Guid)
        public ResourceObject GetResourceByGuid(Guid id)
        {
            ResourceManagementClient client = new ResourceManagementClient();

            // Specify the list of attributes that we are interested in
            List<string> attributesToGet = new List<string>() { "DisplayName", "FirstName", "LastName" };

            try
            {
                ResourceObject resource = client.GetResource(id);

                if (resource == null)
                {
                    // The resource was not found, throw an exception
                    throw new ResourceNotFoundException();
                }

                return resource;
            }
            catch (TooManyResultsException)
            {
                // More than one match was found
                throw;
            }
        }
        #endregion

        
        #region GetResource(UniqueIdentifier)
        public ResourceObject GetResourceByReference()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            // Specify the list of attributes that we are interested in
            List<string> attributesToGet = new List<string>() { "DisplayName", "FirstName", "LastName" };

            try
            {
                ResourceObject resource = client.GetResourceByKey("Person", "AccountName", "user0001", attributesToGet);

                if (resource == null)
                {
                    // The resource was not found, throw an exception
                    throw new ResourceNotFoundException();
                }

                // Return the resource referenced by user0001's Manager attributeName

                return client.GetResource(resource.Attributes["Manager"].ReferenceValue);
            }
            catch (TooManyResultsException)
            {
                // More than one match was found
                throw;
            }
        }
        #endregion

        
        #region GetResource(String)
        public ResourceObject GetResourceByGuidString(string id)
        {
            ResourceManagementClient client = new ResourceManagementClient();

            // Specify the list of attributes that we are interested in
            List<string> attributesToGet = new List<string>() { "DisplayName", "FirstName", "LastName" };

            try
            {
                ResourceObject resource = client.GetResource(id);

                if (resource == null)
                {
                    // The resource was not found, throw an exception
                    throw new ResourceNotFoundException();
                }

                return resource;
            }
            catch (TooManyResultsException)
            {
                // More than one match was found
                throw;
            }
        }
        #endregion
      
    }
}
