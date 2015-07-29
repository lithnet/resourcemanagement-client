using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.collection.GetObjectAtIndex(index);
            }
        }

        public bool MoveNext()
        {
            this.index++;

            return this.index != this.collection.Count;
        }

        public void Reset()
        {
            this.index = -1;
        }
    }
}
