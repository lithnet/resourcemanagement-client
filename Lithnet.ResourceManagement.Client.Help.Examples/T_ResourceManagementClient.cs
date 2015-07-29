using System;
using Lithnet.ResourceManagement.Client;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an instance of the resource management defaultCredentialClient
            ResourceManagementClient client = new ResourceManagementClient();

            // Get the resource with the account name 'testuser'
            ResourceObject resource = client.GetResourceByKey("Person", "AccountName", "testuser");

            // Write the object to the console
            Console.WriteLine(resource.ToString());

            // Get a single attribute
            Console.WriteLine(resource.Attributes["AccountName"].StringValue);

            // Change an attribute
            resource.Attributes["AccountName"].SetValue("NewUsername");

            // Save the resource to the fim service
            resource.Save();

            // Create a new object
            ResourceObject newResource = client.CreateResource("Person");
            newResource.Attributes["AccountName"].SetValue("MyNewAccount");
            newResource.Attributes["Domain"].SetValue("FIM-DEV1");

            // Save the new object to the fim service
            newResource.Save();

            // Search for the newly created object by anchor
            ResourceObject foundObject = client.GetResourceByKey("Person", "AccountName", "MyNewAccount");

            // Delete the object
            client.DeleteResource(foundObject);

            // Print the values of the object
            Console.WriteLine(foundObject.ToString());

            // Search for all Sets
            foreach (ResourceObject result in client.GetResources("/Set"))
            {
                Console.WriteLine(result.ToString());
            }
        }
    }
}
