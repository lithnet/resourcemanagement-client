using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.ResourceManagement.Client.Help.Examples
{
    class ResourceManagementClient_GetResourceByKeyExamples
    {
        #region GetResourceByKey(String, String, String)
        public ResourceObject GetPersonByUsername(string username)
        {
            ResourceManagementClient client = new ResourceManagementClient();

            // Specify the list of attributes that we are interested in
            List<string> attributesToGet = new List<string>() { "DisplayName", "FirstName", "LastName" };

            try
            {
                ResourceObject resource = client.GetResourceByKey("Person", "AccountName", username, attributesToGet);

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

        #region GetResourceByKey(String, Dictionary{String, String})
        public ResourceObject GetPersonByUsernameAndDomain(string username, string domain)
        {
            ResourceManagementClient client = new ResourceManagementClient();

            Dictionary<string, string> anchorPairs = new Dictionary<string, string>();
            anchorPairs.Add("AccountName", username);
            anchorPairs.Add("Domain", domain);

            // Specify the list of attributes that we are interested in
            List<string> attributesToGet = new List<string>() { "DisplayName", "FirstName", "LastName" };

            try
            {
                ResourceObject resource = client.GetResourceByKey("Person", anchorPairs, attributesToGet);

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
