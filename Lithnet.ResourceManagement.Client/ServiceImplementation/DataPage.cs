using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client.ServiceImplementation
{
    /// <summary>
    /// Provides structure for returning paged data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataPage<T>
    {
        /// <summary>
        /// Item elements for specific page
        /// </summary>
        public IEnumerable<T> Items { get; private set; }

        /// <summary>
        /// Total number of items for collection
        /// </summary>
        public int TotalItemsCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items">Item elements for specific page</param>
        /// <param name="totalItemsCount">The total number of items for given collection</param>
        public DataPage(IEnumerable<T> items, int totalItemsCount)
        {
            Items = items;
            TotalItemsCount = totalItemsCount;
        }
    }
}
