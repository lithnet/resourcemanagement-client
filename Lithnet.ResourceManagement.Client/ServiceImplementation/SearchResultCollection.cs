using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Microsoft.ResourceManagement.WebServices.WSEnumeration;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Provides an enumerator that can iterate through search results from the Resource Management Service. This class provides a synchronous search, where the next page of results is only requested once the enumerator has run out of results.
    /// </summary>
    public class SearchResultCollection : ISearchResultCollection, IEnumerator<ResourceObject>
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
        /// The iterator's current object
        /// </summary>
        private ResourceObject current;

        /// <summary>
        /// The search client used for this search operation
        /// </summary>
        private SearchClient searchClient;

        /// <summary>
        /// The resource management client used for this search operation
        /// </summary>
        private ResourceManagementClient client;

        /// <summary>
        /// The iterator's current index
        /// </summary>
        private int currentIndex = 0;

        /// <summary>
        /// The result set obtained from the Resource Management Service
        /// </summary>
        private List<ResourceObject> resultSet;
        
        /// <summary>
        /// The page size used for the search operation
        /// </summary>
        private int pageSize = 0;

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
        /// Initializes a new instance of the SearchResultCollection class
        /// </summary>
        /// <param name="response">The initial enumeration response from the Resource Management Service</param>
        /// <param name="pageSize">The page size used in the search operation</param>
        /// <param name="searchClient">The client proxy used for performing the search</param>
        /// <param name="client">The client used to convert response data into ResourceObjects</param>
        internal SearchResultCollection(EnumerateResponse response, int pageSize, SearchClient searchClient, ResourceManagementClient client)
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

            this.resultSet = new List<ResourceObject>();
            this.client = client;
            this.context = response.EnumerationContext;
            this.pageSize = pageSize;
            this.details = response.EnumerationDetail;
            this.searchClient = searchClient;
            this.EndOfSequence = response.EndOfSequence != null;
            this.PopulateResultSet(response.Items);
        }

        /// <summary>
        /// Populates the internal result set with the items obtained from the search response
        /// </summary>
        /// <param name="items">The items obtained from the enumeration or pull response</param>
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
        /// Gets an enumerator that can iterate over the search results obtained from the Resource Management Service
        /// </summary>
        /// <returns>An enumerator for the search results</returns>
        public IEnumerator<ResourceObject> GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Gets an enumerator that can iterate over the search results obtained from the Resource Management Service
        /// </summary>
        /// <returns>An enumerator for the search results</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Gets the iterator's current object
        /// </summary>
        public ResourceObject Current
        {
            get
            {
                return this.current;
            }
        }

        /// <summary>
        /// Gets the iterator's current object
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return this.current;
            }
        }

        /// <summary>
        /// Moves the iterator to the next position in the result sett
        /// </summary>
        /// <returns>True if there are more results, false if the end of the collection has been reached</returns>
        public bool MoveNext()
        {
            if (currentIndex < resultSet.Count)
            {
                this.current = resultSet[currentIndex++];
                return true;
            }
            else
            {
                if (this.EndOfSequence == true)
                {
                    this.current = null;
                    return false;
                }

                this.GetNextPage();

                if (currentIndex < resultSet.Count)
                {
                    this.current = resultSet[currentIndex++];
                    return true;
                }
                else
                {
                    this.current = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the next page of search results from the Resource Management Service
        /// </summary>
        private void GetNextPage()
        {
            PullResponse r = this.searchClient.Pull(this.context, this.pageSize);
            if (r.EndOfSequence != null)
            {
                this.EndOfSequence = true;
            }

            this.context = r.EnumerationContext;
            
            this.PopulateResultSet(r.Items);
        }

        /// <summary>
        /// Resets the iterator back to the start of the collection
        /// </summary>
        public void Reset()
        {
            this.current = null;
            this.currentIndex = 0;
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        /// <param name="disposing">Indicates if this object is being disposed from a call to the Dispose() method</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}
