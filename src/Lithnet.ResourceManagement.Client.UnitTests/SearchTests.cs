using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.UnitTests
{
    [TestClass]
    public class SearchTests
    {
        public SearchTests()
        {
        //    var x = UnitTestHelper.client;
        }

        [TestMethod]
        public async Task SearchTestAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            ISearchResultCollection results = await c.GetResourcesAsync("/Set", 200).ConfigureAwait(false);
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
        public void SearchBadFilter()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            try
            {
                ISearchResultCollection results = c.GetResources("!not a filter!", 200);
                Debug.WriteLine("Getting {0} results", results.Count);

                foreach (ResourceObject o in results)
                {
                    Debug.WriteLine("UT got object " + o.ObjectID);
                }
            }
            catch (CannotProcessFilterException)
            {
                return;
            }

            Assert.Fail("The expected exception was not thrown");
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
        public async Task SearchTestAsyncSortedAscAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", true));
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);

            ISearchResultCollection results = await c.GetResourcesAsync(query, -1, new string[] { "AccountName" }, sortAttributes).ConfigureAwait(false);
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
        public async Task SearchTestAsyncSortedDescAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", false));
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);

            ISearchResultCollection results = await c.GetResourcesAsync(query, -1, new string[] { "AccountName" }, sortAttributes).ConfigureAwait(false);
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
        public async Task SearchTestAsyncNoResultsAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            ISearchResultCollection results = await c.GetResourcesAsync("Set[DisplayName='...!!!...']", 200).ConfigureAwait(false);
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
        public async Task SearchTestPagedResultsSortAscAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            List<string> attributesToGet = new List<string>() { AttributeNames.AccountName };
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", true));

            SearchResultPager pager = await c.GetResourcesPagedAsync(query, 2, attributesToGet, sortAttributes).ConfigureAwait(false);

            Assert.AreEqual(pager.TotalCount, 6);

            var results = await pager.GetNextPageAsync().ToListAsync().ConfigureAwait(false);

            // Move forward through the result set, getting two items per page
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(true, pager.HasMoreItems);
            Assert.AreEqual("reftest1", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", results[1].Attributes[AttributeNames.AccountName].StringValue);

            results = await pager.GetNextPageAsync().ToListAsync().ConfigureAwait(false);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(true, pager.HasMoreItems);
            Assert.AreEqual("reftest3", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", results[1].Attributes[AttributeNames.AccountName].StringValue);

            results = await pager.GetNextPageAsync().ToListAsync().ConfigureAwait(false);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(false, pager.HasMoreItems);
            Assert.AreEqual("reftest5", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest6", results[1].Attributes[AttributeNames.AccountName].StringValue);

            /// Jump back in the result set and change the page size
            pager.CurrentIndex = 2;
            pager.PageSize = 4;
            results = await pager.GetNextPageAsync().ToListAsync().ConfigureAwait(false);
            Assert.AreEqual(4, results.Count);
            Assert.AreEqual(false, pager.HasMoreItems);
            Assert.AreEqual("reftest3", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", results[1].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest5", results[2].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest6", results[3].Attributes[AttributeNames.AccountName].StringValue);
        }

        [TestMethod]
        public async Task SearchTestPagedResultsSortDescAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            string query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            List<string> attributesToGet = new List<string>() { AttributeNames.AccountName };
            List<SortingAttribute> sortAttributes = new List<SortingAttribute>();
            sortAttributes.Add(new SortingAttribute("AccountName", false));

            SearchResultPager pager = await c.GetResourcesPagedAsync(query, 2, attributesToGet, sortAttributes).ConfigureAwait(false);

            Assert.AreEqual(pager.TotalCount, 6);

            var results = await pager.GetNextPageAsync().ToListAsync().ConfigureAwait(false);

            // Move forward through the result set, getting two items per page
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(true, pager.HasMoreItems);
            Assert.AreEqual("reftest6", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest5", results[1].Attributes[AttributeNames.AccountName].StringValue);

            results = await pager.GetNextPageAsync().ToListAsync().ConfigureAwait(false);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(true, pager.HasMoreItems);
            Assert.AreEqual("reftest4", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest3", results[1].Attributes[AttributeNames.AccountName].StringValue);

            results = await pager.GetNextPageAsync().ToListAsync().ConfigureAwait(false);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(false, pager.HasMoreItems);
            Assert.AreEqual("reftest2", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest1", results[1].Attributes[AttributeNames.AccountName].StringValue);

            /// Jump back in the result set and change the page size
            pager.CurrentIndex = 2;
            pager.PageSize = 4;
            results = await pager.GetNextPageAsync().ToListAsync    ().ConfigureAwait(false);
            Assert.AreEqual(4, results.Count);
            Assert.AreEqual(false, pager.HasMoreItems);
            Assert.AreEqual("reftest4", results[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest3", results[1].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", results[2].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest1", results[3].Attributes[AttributeNames.AccountName].StringValue);
        }

        [TestMethod]
        public async Task SearchTestPagedDefaultPageAndSizeSortedDescendingAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            var query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            var attributesToGet = new List<string>();
            attributesToGet.Add(AttributeNames.AccountName);

            SearchResultPager results = await c.GetResourcesPagedAsync(query, 100, attributesToGet, AttributeNames.AccountName, false).ConfigureAwait(false);
            Assert.AreEqual(results.TotalCount, 6);

            var arrayResults = await results.GetNextPageAsync().ToListAsync().ConfigureAwait(false);

            Assert.AreEqual(6, arrayResults.Count);
            Assert.AreEqual("reftest6", arrayResults[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest5", arrayResults[1].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", arrayResults[2].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest3", arrayResults[3].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", arrayResults[4].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest1", arrayResults[5].Attributes[AttributeNames.AccountName].StringValue);
        }

        [TestMethod]
        public async Task SearchTestPagedSortedAscendingAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            var query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            var attributesToGet = new List<string>();
            attributesToGet.Add(AttributeNames.AccountName);

            SearchResultPager results = await c.GetResourcesPagedAsync(query, 2, attributesToGet, AttributeNames.AccountName, true).ConfigureAwait(false);
            Assert.AreEqual(6, results.TotalCount);
            results.CurrentIndex = 2;
            var arrayResults = await results.GetNextPageAsync().ToListAsync().ConfigureAwait(false);

            Assert.AreEqual(2, arrayResults.Count);
            Assert.AreEqual("reftest3", arrayResults[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest4", arrayResults[1].Attributes[AttributeNames.AccountName].StringValue);
        }

        [TestMethod]
        public async Task SearchTestPagedSortedDescendingAsync()
        {
            ResourceManagementClient c = new ResourceManagementClient();

            var query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);
            var attributesToGet = new List<string>();
            attributesToGet.Add(AttributeNames.AccountName);

            SearchResultPager results = await c.GetResourcesPagedAsync(query, 2, attributesToGet, AttributeNames.AccountName, false).ConfigureAwait(false);
            Assert.AreEqual(6, results.TotalCount);
            results.CurrentIndex = 3;
            var arrayResults = await results.GetNextPageAsync().ToListAsync().ConfigureAwait(false);

            Assert.AreEqual(2, arrayResults.Count);
            Assert.AreEqual("reftest3", arrayResults[0].Attributes[AttributeNames.AccountName].StringValue);
            Assert.AreEqual("reftest2", arrayResults[1].Attributes[AttributeNames.AccountName].StringValue);
        }

        [TestMethod]
        public void SearchTestResultCount()
        {
            ResourceManagementClient c = new ResourceManagementClient();
            var query = String.Format("/{0}[starts-with('{1}', 'reftest')]", UnitTestHelper.ObjectTypeUnitTestObjectName, AttributeNames.AccountName);

            Assert.AreEqual(6, c.GetResourceCount(query));
        }
    }
}