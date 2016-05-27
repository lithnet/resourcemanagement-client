using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Microsoft.ResourceManagement.WebServices.WSEnumeration;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Provides an enumerator that can iterate through search results from the Resource Management Service. This class obtains all result pages on a separate thread, without waiting for the iterator to reach the end of a page before requesting a new one.
    /// </summary>
    public class SearchResultCollectionAsync : ISearchResultCollection
    {
        /// <summary>
        /// The search pager used for the results enumeration
        /// </summary>
        private SearchResultPager pager;

        /// <summary>
        /// The result set obtained from the Resource Management Service
        /// </summary>
        private BlockingCollection<ResourceObject> resultSet;

        /// <summary>
        /// The internal enumerable that consumes results as they are generated
        /// </summary>
        private IEnumerable<ResourceObject> consumingEnumerable;

        /// <summary>
        /// The cancellation token used to abort the operation
        /// </summary>
        private CancellationToken token;

        /// <summary>
        /// Gets the number of results in the search response
        /// </summary>
        public int Count
        {
            get
            {
                return this.pager.TotalCount;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SearchResultCollectionAsync class
        /// </summary>
        /// <param name="pager">The search pager used for the results enumeration</param>
        /// <param name="tokenSource">The cancellation token source that can be used to abort the async operation</param>
        internal SearchResultCollectionAsync(SearchResultPager pager, CancellationTokenSource tokenSource)
        {
            if (pager == null)
            {
                throw new ArgumentNullException("pager");
            }

            this.resultSet = new BlockingCollection<ResourceObject>();
            this.consumingEnumerable = this.resultSet.GetConsumingEnumerable();

            if (tokenSource != null)
            {
                this.token = tokenSource.Token;
            }

            this.pager = pager;
            this.ExecuteProducer();
        }

        /// <summary>
        /// Starts the producer thread which enumerates the results from the resource management service
        /// </summary>
        private void ExecuteProducer()
        {
            Task task = new Task(() =>
                {
                    while (this.pager.HasMoreItems)
                    {
                        foreach(ResourceObject result in this.pager.GetNextPage())
                        {
                            this.resultSet.Add(result);
                        }
                    }

                    this.resultSet.CompleteAdding();
                }, this.token);
            
            task.Start();
        }

        /// <summary>
        /// Gets an enumerator that can iterate over the search results obtained from the Resource Management Service
        /// </summary>
        /// <returns>An enumerator for the search results</returns>
        public IEnumerator<ResourceObject> GetEnumerator()
        {
            return this.consumingEnumerable.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that can iterate over the search results obtained from the Resource Management Service
        /// </summary>
        /// <returns>An enumerator for the search results</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.consumingEnumerable.GetEnumerator();
        }
    }
}
