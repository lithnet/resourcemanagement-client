using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Lithnet.ResourceManagement.Client;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using Microsoft.ResourceManagement.WebServices;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class SearchTests
    {
        [TestMethod]
        public void SearchTestBatchedAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            System.Threading.CancellationTokenSource source = new System.Threading.CancellationTokenSource();

            ISearchResults results = c.GetResources("/Group", 200, source);
            Debug.WriteLine("Getting {0} results", results.Count);

            foreach (ResourceObject o in results)
            {
                Debug.WriteLine("UT got object " + o.ObjectID);
            }
        }

        [TestMethod]
        public void SearchTestBatched()
        {
            SearchClient c = new SearchClient();
            c.Open();

            ISearchResults results = c.Enumerate("/Group", 200);
            Debug.WriteLine("Getting {0} results", results.Count);

            foreach (ResourceObject o in results)
            {
                Debug.WriteLine("UT got object " + o.ObjectID);
            }
        }

        [TestMethod]
        public void SearchTestBatchedRestrictedAttributeList()
        {
            SearchClient c = new SearchClient();
            c.Open();

            List<string> attributesToGet = new List<string>();

            attributesToGet.Add("ObjectID");
            attributesToGet.Add("ObjectType");
            attributesToGet.Add("DisplayName");

            ISearchResults results = c.Enumerate("/Group", 200, attributesToGet);
            Debug.WriteLine("Getting {0} results", results.Count);

            foreach (ResourceObject o in results)
            {
                Debug.WriteLine("UT got object " + o.ObjectID);
            }
        }

    }
}
