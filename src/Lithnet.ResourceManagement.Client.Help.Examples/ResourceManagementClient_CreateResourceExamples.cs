using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ResourceManagement.WebServices;

namespace Lithnet.ResourceManagement.Client.Help.Examples
{
    class ResourceManagementClient_CreateResourceExamples
    {
        #region CreateResource(String)
        public void CreateResouruce()
        {
            ResourceManagementClient client = new ResourceManagementClient();

            // Create a template for a resource object
            ResourceObject resource = client.CreateResource("Person");

            resource.Attributes["AccountName"].SetValue("user0001");
            resource.Attributes["Domain"].SetValue("FIM-DEV1");
            resource.Attributes["DisplayName"].SetValue("Test user 1");

            try
            {
                resource.Save();
            }
            catch (Exception)
            {
                // An error occurred while creating the resource
                throw;
            }
        }
        #endregion

        #region CreateResourceTemplateForUpdate(String, UniqueIdentifier)
        public void CreateResource()
        {
            ResourceManagementClient client = new ResourceManagementClient();
            UniqueIdentifier knownID = new UniqueIdentifier("5f491363-4d59-42f9-8f6c-8fd92cd1fb92");

            // Create a template to update a resource with a known ID
            ResourceObject resource = client.CreateResourceTemplateForUpdate("Person", knownID);

            resource.Attributes["DisplayName"].SetValue("Test User 3");

            try
            {
                resource.Save();
            }
            catch (Exception)
            {
                // An error occurred while updating the resource
                throw;
            }
        }
        #endregion
    }
}
