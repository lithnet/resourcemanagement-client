using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Lithnet.ResourceManagement.Client;
using Microsoft.ResourceManagement.WebServices.IdentityManagementOperation;
using Microsoft.ResourceManagement.WebServices;
using Microsoft.ResourceManagement.WebServices.WSEnumeration;
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
        public void SearchTestAsync()
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
        public void SearchTestSync()
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
        public void SearchTestSyncSortedAsc()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", true));
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);

            ISearchResultCollection results = c.GetResources(query, -1, new string[] { "AccountName" }, sortAttributes);
            ResourceObject[] arrayResults = results.ToArray();

            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("reftest1", arrayResults[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", arrayResults[1].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest3", arrayResults[2].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", arrayResults[3].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest5", arrayResults[4].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest6", arrayResults[5].Attributes[AttributeNames.AccountName].StringValue);
        }

        [TestMethod]
        public void SearchTestSyncSortedDesc()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", false));
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);

            ISearchResultCollection results = c.GetResources(query, -1, new string[] { "AccountName" }, sortAttributes);
            ResourceObject[] arrayResults = results.ToArray();

            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("reftest6", arrayResults[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest5", arrayResults[1].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", arrayResults[2].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest3", arrayResults[3].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", arrayResults[4].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest1", arrayResults[5].Attributes[AttributeNames.AccountName].StringValue);
        }

        [TestMethod]
        public void SearchTestAsyncSortedAsc()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", true));
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);

            ISearchResultCollection results = c.GetResourcesAsync(query, -1, new string[] { "AccountName" }, sortAttributes);
            ResourceObject[] arrayResults = results.ToArray();

            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("reftest1", arrayResults[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", arrayResults[1].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest3", arrayResults[2].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", arrayResults[3].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest5", arrayResults[4].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest6", arrayResults[5].Attributes[AttributeNames.AccountName].StringValue);
        }

        [TestMethod]
        public void SearchTestAsyncSortedDesc()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", false));
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);

            ISearchResultCollection results = c.GetResourcesAsync(query, -1, new string[] { "AccountName" }, sortAttributes);
            ResourceObject[] arrayResults = results.ToArray();

            Assert.AreEqual(6, results.Count);
            Assert.AreEqual("reftest6", arrayResults[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest5", arrayResults[1].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", arrayResults[2].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest3", arrayResults[3].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", arrayResults[4].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest1", arrayResults[5].Attributes[AttributeNames.AccountName].StringValue);
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
        public void SearchTestSyncNoResults()
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
        public void SearchTestSyncRestrictedAttributeList()
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
        public void SearchTestPagedResultsSortAsc()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            List<string> attributesToGet = new List<string>() { AttributeNames.AccountName };
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", true));

            SearchResultPager pager = c.GetResourcesPaged(query, 2, attributesToGet, sortAttributes);

            Assert.AreEqual(pager.TotalCount, 6);

            ResourceObject[] results = pager.GetNextPage().ToArray();

            // Move forward through the result set, getting two items per page
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(true, pager.HasMoreItems());
            Assert.AreEqual("reftest1", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", results[1].Attributes[AttributeNames.AccountName].StringValue);

            results = pager.GetNextPage().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(true, pager.HasMoreItems());
            Assert.AreEqual("reftest3", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", results[1].Attributes[AttributeNames.AccountName].StringValue);

            results = pager.GetNextPage().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(false, pager.HasMoreItems());
            Assert.AreEqual("reftest5", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest6", results[1].Attributes[AttributeNames.AccountName].StringValue);

            /// Jump back in the result set and change the page size
            pager.CurrentIndex = 2;
            pager.PageSize = 4;
            results = pager.GetNextPage().ToArray();
            Assert.AreEqual(4, results.Length);
            Assert.AreEqual(false, pager.HasMoreItems());
            Assert.AreEqual("reftest3", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", results[1].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest5", results[2].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest6", results[3].Attributes[AttributeNames.AccountName].StringValue);
        }

        [TestMethod]
        public void SearchTestPagedResultsSortDesc()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            List<string> attributesToGet = new List<string>() { AttributeNames.AccountName };
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", false));

            SearchResultPager pager = c.GetResourcesPaged(query, 2, attributesToGet, sortAttributes);

            Assert.AreEqual(pager.TotalCount, 6);

            ResourceObject[] results = pager.GetNextPage().ToArray();

            // Move forward through the result set, getting two items per page
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(true, pager.HasMoreItems());
            Assert.AreEqual("reftest6", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest5", results[1].Attributes[AttributeNames.AccountName].StringValue);

            results = pager.GetNextPage().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(true, pager.HasMoreItems());
            Assert.AreEqual("reftest4", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest3", results[1].Attributes[AttributeNames.AccountName].StringValue);

            results = pager.GetNextPage().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(false, pager.HasMoreItems());
            Assert.AreEqual("reftest2", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest1", results[1].Attributes[AttributeNames.AccountName].StringValue);

            /// Jump back in the result set and change the page size
            pager.CurrentIndex = 2;
            pager.PageSize = 4;
            results = pager.GetNextPage().ToArray();
            Assert.AreEqual(4, results.Length);
            Assert.AreEqual(false, pager.HasMoreItems());
            Assert.AreEqual("reftest4", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest3", results[1].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", results[2].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest1", results[3].Attributes[AttributeNames.AccountName].StringValue);
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

        [TestMethod]
        public void SearchTestResultCount()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            var query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);

            Assert.AreEqual(6, c.GetResourceCount(query));
        }

        [TestMethod]
        public void TestDataPagePerformance()
        {
            int page = 0;
            int count = 0;
            var query = String.Format("/Person");
            ResourceManagementClient c = new ResourceManagementClient();
            DataPage<ResourceObject> results;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            do
            {
                timer.Restart();
                page++;
                results = c.GetPagedResources(query, page, 10, new List<string>(), null, true);

                foreach(var result in results.Items)
                {
                    count++;
                }

                Debug.WriteLine("Page time: {0}", timer.Elapsed.TotalSeconds);
            }
            while (results.Items.Count() > 0);
        }

        [TestMethod]
        public void TestSearchPagerPerformance()
        {
            var query = String.Format("/Person");
            ResourceManagementClient c = new ResourceManagementClient();
            var results = c.GetResourcesPaged(query, 10, new List<string>(), null);
            int count = 0;
            Stopwatch timer = new Stopwatch();
            timer.Start();

            while (results.HasMoreItems())
            {
                timer.Restart();

                foreach (var result in results.GetNextPage())
                {
                    count++;  
                }

                Debug.WriteLine("Page time: {0}", timer.Elapsed.TotalSeconds);
            }
        }
    }
}