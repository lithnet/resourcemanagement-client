using System.Collections.Generic;

namespace Lithnet.ResourceManagement.Client
{
    internal class SearchResultEnumerator : IEnumerator<IResourceObject>
    {
        private int index = -1;

        private SearchResultCollection collection;

        public SearchResultEnumerator(SearchResultCollection collection)
        {
            this.collection = collection;
        }

        public IResourceObject Current
        {
            get
            {
                return AsyncHelper.Run(async () => await this.collection.GetObjectAtIndexAsync(this.index).ConfigureAwait(false));
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

            return this.collection.HasMoreItems(this.index);
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
