using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lithnet.ResourceManagement.Client.ResourceManagementService;
using Microsoft.ResourceManagement.WebServices.WSEnumeration;
using System.Globalization;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Provides an enumerator that can iterate through search results from the Resource Management Service in a paged manner
    /// </summary>
    public class SearchResultPager 
    {
        private CultureInfo locale;

        /// <summary>
        /// The enumeration context object provided by the Resource Management Service
        /// </summary>
        private EnumerationContextType context;

        /// <summary>
        /// The search client used for this search operation
        /// </summary>
        private SearchClient searchClient;

        /// <summary>
        /// The resource management client used for this search operation
        /// </summary>
        private ResourceManagementClient client;

        /// <summary>
        /// The page size used for the search operation
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets the number of results in the search response
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there are more results on the server
        /// </summary>
        public bool HasMoreItems
        {
            get
            {
                return this.TotalCount > 0 && !this.EndOfSequence;
            }
        }

        /// <summary>
        /// Gets the current starting index of the paged search
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return this.context.CurrentIndex;
            }
            set
            {
                this.context.CurrentIndex = value;
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
        /// <param name="locale">The localization culture that the search results are represented as</param>
        internal SearchResultPager(EnumerateResponse response, int pageSize, SearchClient searchClient, ResourceManagementClient client, CultureInfo locale)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (pageSize < 0)
            {
                throw new ArgumentException("The page size must be zero or greater", nameof(pageSize));
            }

            if (searchClient == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            this.TotalCount = Convert.ToInt32(response.EnumerationDetail.Count);
            this.client = client;
            this.locale = locale;
            this.context = response.EnumerationContext;
            this.context.CurrentIndex = 0;
            this.PageSize = pageSize;
            this.searchClient = searchClient;
            this.EndOfSequence = response.EndOfSequence != null;
        }

        /// <summary>
        /// Populates the internal result set with the items obtained from the search response
        /// </summary>
        /// <param name="items">The items obtained from the enumeration or pull response</param>
        private IEnumerable<ResourceObject> EnumerateResultSet(ItemListType items)
        {
            if (items != null)
            {
                foreach (XmlElement item in items.Any.OfType<XmlElement>())
                {
                    yield return new ResourceObject(item, this.client, this.locale);
                }
            }
        }

        /// <summary>
        /// Gets the next page of search results from the Resource Management Service
        /// </summary>
        /// <returns>An enumeration of the ResourceObjects in the page</returns>
        public IEnumerable<ResourceObject> GetNextPage()
        {
            PullResponse r = this.searchClient.Pull(this.context, this.PageSize);
            if (r.EndOfSequence != null)
            {
                this.EndOfSequence = true;
            }

            if (r.EnumerationContext != null)
            {
                this.context = r.EnumerationContext;
            }

            return this.EnumerateResultSet(r.Items);
        }
    }
}