using System;
using System.Collections;
using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// Provides an enumerator that can iterate through search results from the Resource Management Service. This class provides a synchronous search, where the next page of results is only requested once the enumerator has run out of results.
    /// </summary>
    public class SearchResultCollection : ISearchResultCollection
    {
        /// <summary>
        /// The search pager used for result enumeration
        /// </summary>
        private SearchResultPager pager;

        /// <summary>
        /// The result set obtained from the Resource Management Service
        /// </summary>
        private List<ResourceObject> resultSet;

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
        /// Initializes a new instance of the SearchResultCollection class
        /// </summary>
        /// <param name="pager">The search pager used for result enumeration</param>
        internal SearchResultCollection(SearchResultPager pager)
        {
            if (pager == null)
            {
                throw new ArgumentNullException("pager");
            }

            this.pager = pager;
            this.resultSet = new List<ResourceObject>();
        }

        /// <summary>
        /// Gets the next page of the search results
        /// </summary>
        private void GetNextPage()
        {
            if (this.pager.HasMoreItems)
            {
                this.resultSet.AddRange(this.pager.GetNextPage());
            }
        }

        /// <summary>
        /// Gets the object at the specified index in the result collection
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal ResourceObject GetObjectAtIndex(int index)
        {
            if (index >= this.Count)
            {
                throw new ArgumentOutOfRangeException(string.Format("The specified index was out of range of the allowed values. Index: {0}, Total Count: {1}", index, this.Count));
            }

            if ((index >= this.resultSet.Count))
            {
                if (this.pager.HasMoreItems)
                {
                    this.GetNextPage();
                    return this.GetObjectAtIndex(index);
                }
                else
                {
                    throw new InvalidOperationException("There are no more pages in the result set");
                }
            }

            return this.resultSet[index];
        }

        /// <summary>
        /// Gets a value that indicates if there are more items in the result set after the specified index
        /// </summary>
        /// <param name="index">The value to query if there are proceeding items in the collection</param>
        /// <returns>True if there are more items in the results set</returns>
        internal bool HasMoreItems(int index)
        {
            return index < this.Count;
        }

        /// <summary>
        /// Gets an enumerator that can iterate over the search results obtained from the Resource Management Service
        /// </summary>
        /// <returns>An enumerator for the search results</returns>
        public IEnumerator<ResourceObject> GetEnumerator()
        {
            return new SearchResultEnumerator(this);
        }

        /// <summary>
        /// Gets an enumerator that can iterate over the search results obtained from the Resource Management Service
        /// </summary>
        /// <returns>An enumerator for the search results</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SearchResultEnumerator(this);
        }
    }
}