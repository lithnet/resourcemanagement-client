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
        /// The enumeration context object provided by the Resource Management Service
        /// </summary>
        private EnumerationContextType context;

        /// <summary>
        /// The details of the enumeration operation
        /// </summary>
        private EnumerationDetailType details;

        /// <summary>
        /// The search client used for this search operation
        /// </summary>
        private SearchClient searchClient;

        /// <summary>
        /// The resource management client used for this search operation
        /// </summary>
        private ResourceManagementClient client;

        /// <summary>
        /// The result set obtained from the Resource Management Service
        /// </summary>
        private BlockingCollection<ResourceObject> resultSet;

        /// <summary>
        /// The page size used for the search operation
        /// </summary>
        private int pageSize = 0;

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
                if (this.details != null && this.details.Count != null)
                {
                    return Convert.ToInt32(this.details.Count);
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Indicates if the Resource Management Service has provided all the results and signaled the end of sequence flag
        /// </summary>
        private bool EndOfSequence = false;

        /// <summary>
        /// Initializes a new instance of the SearchResultCollectionAsync class
        /// </summary>
        /// <param name="response">The initial enumeration response from the Resource Management Service</param>
        /// <param name="pageSize">The page size used in the search operation</param>
        /// <param name="searchClient">The client proxy used for performing the search</param>
        /// <param name="tokenSource">The cancellation token source that can be used to abort the async operation</param>
        /// <param name="client">The client used to convert response data into ResourceObjects</param>
        internal SearchResultCollectionAsync(EnumerateResponse response, int pageSize, SearchClient searchClient, CancellationTokenSource tokenSource, ResourceManagementClient client)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (pageSize < 0)
            {
                throw new ArgumentException("The page size must be zero or greater", "pageSize");
            }

            if (searchClient == null)
            {
                throw new ArgumentNullException("client");
            }

            this.resultSet = new BlockingCollection<ResourceObject>();
            this.client = client;
            this.consumingEnumerable = this.resultSet.GetConsumingEnumerable();

            if (tokenSource != null)
            {
                this.token = tokenSource.Token;
            }

            this.context = response.EnumerationContext;
            this.pageSize = pageSize;
            this.details = response.EnumerationDetail;
            this.searchClient = searchClient;
            this.EndOfSequence = response.EndOfSequence != null;
            this.PopulateResultSet(response.Items);
            Debug.WriteLine("Enumeration started. End of request: {0}. Expected results: {1}", this.EndOfSequence, this.Count);
            this.ExecuteProducer();

        }

        /// <summary>
        /// Starts the producer thread which enumerates the results from the resource management service
        /// </summary>
        private void ExecuteProducer()
        {
            Task task = new Task(() =>
                {
                    while (this.EndOfSequence == false)
                    {
                        if (this.token != null && this.token.IsCancellationRequested)
                        {
                            this.ReleaseEnumerationContext();
                            break; 
                        }

                        PullResponse r = this.searchClient.Pull(this.context, this.pageSize);

                        if (r.EndOfSequence != null)
                        {
                            this.EndOfSequence = true;
                        }

                        this.context = r.EnumerationContext;

                        //this.currentIndex = 0;
                        this.PopulateResultSet(r.Items);
                    }

                    this.resultSet.CompleteAdding();
                });

            task.Start();
        }

        /// <summary>
        /// Populates the result set from the items returned from the enumeration call
        /// </summary>
        /// <param name="items">The items contained in the response from the server</param>
        private void PopulateResultSet(ItemListType items)
        {
            if (items != null)
            {
                foreach (XmlElement item in items.Any.OfType<XmlElement>())
                {
                    this.resultSet.Add(new ResourceObject(item, this.client));
                }
            }
        }

        /// <summary>
        /// Signals to the resource management service that we are done with the enumeration context
        /// </summary>
        private void ReleaseEnumerationContext()
        {
            try
            {
                this.searchClient.Release(this.context);
            }
            catch
            { }
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
