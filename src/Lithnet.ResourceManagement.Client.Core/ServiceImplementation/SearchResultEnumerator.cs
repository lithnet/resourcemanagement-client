using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client
{
    internal class SearchResultEnumerator : IEnumerator<ResourceObject>
    {
        private int index = -1;

        private SearchResultCollection collection;

        public SearchResultEnumerator(SearchResultCollection collection)
        {
            this.collection = collection;
        }

        public ResourceObject Current
        {
            get
            {
                return this.collection.GetObjectAtIndex(index);
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public bool MoveNext()
        {
            this.index++;

            return this.collection.HasMoreItems(index);
        }

        public void Reset()
        {
            this.index = -1;
        }

        public void Dispose()
        {
        }
    }
}
