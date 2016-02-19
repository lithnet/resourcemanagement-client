using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Lithnet.ResourceManagement.Client;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using Microsoft.ResourceManagement.WebServices;
using System.Collections.Generic;
using System.Diagnostics;
using Lithnet.ResourceManagement.Client.ServiceImplementation;
using System.Linq;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class SearchTests
    {
        [TestMethod]
        public void SearchTestBatchedAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            ISearchResultCollection results = c.GetResourcesAsync("/Set", 200);
            Debug.WriteLine("Getting {0} results", results.Count);

            int count = 0;

            foreach (ResourceObject o in results)
            {
                Debug.WriteLine("UT got object " + o.ObjectID);
                count++;
            }

            Assert.AreEqual(results.Count, count);
        }

        [TestMethod]
        public void SearchTestBatched()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            ISearchResultCollection results = c.GetResources("/Set", 200);
            Debug.WriteLine("Getting {0} results", results.Count);

            int count = 0;

            foreach (ResourceObject o in results)
            {
                Debug.WriteLine("UT got object " + o.ObjectID);
                count++;
            }

            Assert.AreEqual(results.Count, count);

        }

        [TestMethod]
        public void SearchTestAsyncNoResults()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            ISearchResultCollection results = c.GetResourcesAsync("Set[DisplayName='...!!!...']", 200);
            Debug.WriteLine("Getting {0} results", results.Count);

            int count = 0;

            foreach (ResourceObject o in results)
            {
                Debug.WriteLine("UT got object " + o.ObjectID);
                count++;
            }

            Assert.AreEqual(results.Count, count);
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void SearchTestNoResults()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            ISearchResultCollection results = c.GetResources("/Set[DisplayName='...!!!...']", 200);
            Debug.WriteLine("Getting {0} results", results.Count);

            int count = 0;

            foreach (ResourceObject o in results)
            {
                Debug.WriteLine("UT got object " + o.ObjectID);
                count++;
            }

            Assert.AreEqual(results.Count, count);
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void SearchTestBatchedRestrictedAttributeList()
        {
            ResourceManagementClient c = new ResourceManagementClient();


            List<string> attributesToGet = new List<string>();

            attributesToGet.Add("ObjectID");
            attributesToGet.Add("ObjectType");
            attributesToGet.Add("DisplayName");

            ISearchResultCollection results = c.GetResources("/Group", 200, attributesToGet);
            Debug.WriteLine("Getting {0} results", results.Count);
            int count = 0;

            foreach (ResourceObject o in results)
            {
                Debug.WriteLine("UT got object " + o.ObjectID);
                count++;
            }

            Assert.AreEqual(results.Count, count);
        }

        [TestMethod]
        public void SearchTestPagedDefaultPageAndSizeSortedDescending()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            var query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            var attributesToGet = new List<string>();
            attributesToGet.Add(AttributeNames.AccountName);

            DataPage<ResourceObject> results = c.GetPagedResources(query, 0, 0, attributesToGet, AttributeNames.AccountName, false);
            Assert.AreEqual(results.TotalItemsCount, 6);

            var arrayResults = results.Items.ToArray();
            Assert.AreEqual(results.Items.Count(), 6);
            Assert.AreEqual(arrayResults[0].Attributes[AttributeNames.AccountName].StringValue, "reftest6");
            Assert.AreEqual(arrayResults[1].Attributes[AttributeNames.AccountName].StringValue, "reftest5");
            Assert.AreEqual(arrayResults[2].Attributes[AttributeNames.AccountName].StringValue, "reftest4");
            Assert.AreEqual(arrayResults[3].Attributes[AttributeNames.AccountName].StringValue, "reftest3");
            Assert.AreEqual(arrayResults[4].Attributes[AttributeNames.AccountName].StringValue, "reftest2");
            Assert.AreEqual(arrayResults[5].Attributes[AttributeNames.AccountName].StringValue, "reftest1");
        }

        [TestMethod]
        public void SearchTestPagedSortedAscending()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            var query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            var attributesToGet = new List<string>();
            attributesToGet.Add(AttributeNames.AccountName);

            DataPage<ResourceObject> results = c.GetPagedResources(query, 2, 2, attributesToGet, AttributeNames.AccountName, true);
            Assert.AreEqual(results.TotalItemsCount, 6);

            var arrayResults = results.Items.ToArray();
            Assert.AreEqual(results.Items.Count(), 2);
            Assert.AreEqual(arrayResults[0].Attributes[AttributeNames.AccountName].StringValue, "reftest3");
            Assert.AreEqual(arrayResults[1].Attributes[AttributeNames.AccountName].StringValue, "reftest4");
        }

        [TestMethod]
        public void SearchTestPagedSortedDescending()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            var query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            var attributesToGet = new List<string>();
            attributesToGet.Add(AttributeNames.AccountName);

            DataPage<ResourceObject> results = c.GetPagedResources(query, 3, 2, attributesToGet, AttributeNames.AccountName, false);
            Assert.AreEqual(results.TotalItemsCount, 6);

            var arrayResults = results.Items.ToArray();
            Assert.AreEqual(results.Items.Count(), 2);
            Assert.AreEqual(arrayResults[0].Attributes[AttributeNames.AccountName].StringValue, "reftest2");
            Assert.AreEqual(arrayResults[1].Attributes[AttributeNames.AccountName].StringValue, "reftest1");
        }


        [TestMethod]
        public void SearchTestPagedShouldNotThrowExceptionIfNoAttrsSpecified()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            var query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            var attributesToGet = new List<string>();
            attributesToGet.Add(AttributeNames.AccountName);

            DataPage<ResourceObject> results = c.GetPagedResources(query, 2, 2, null, null, true);
            Assert.AreEqual(results.TotalItemsCount, 6);

            var arrayResults = results.Items.ToArray();
            Assert.AreEqual(results.Items.Count(), 2);
            Assert.AreEqual(arrayResults[0].Attributes[AttributeNames.AccountName].StringValue, "reftest3");
            Assert.AreEqual(arrayResults[1].Attributes[AttributeNames.AccountName].StringValue, "reftest4");
        }

    }
}
