using System;
using Lithnet.ResourceManagement.Client;

namespace ConsoleApplication1
{
    class ResourceManagementClientCtorExamples
    {
        #region ResourceManagementClient()
        private ResourceManagementClient client;

        public void InitializeClient()
        {
            this.client = new ResourceManagementClient();
        }
        #endregion

        #region ResourceManagementClient(System.Net.NetworkCredentials)

        private ResourceManagementClient specifiedCredentialClient;

        public void InitializeClientWithCredentials()
        {
            // Set the credentials that this specific instance of the resource management client will use
            System.Net.NetworkCredential creds = new System.Net.NetworkCredential("username2", "password2");
            this.specifiedCredentialClient = new ResourceManagementClient(creds);
        }
        #endregion
    }
}