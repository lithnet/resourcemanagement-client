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
    public class SearchResults : ISearchResults, IEnumerator<ResourceObject>
    {
        private EnumerationContextType context;

        private EnumerationDetailType details;

        private ResourceObject current;

        private SearchClient searchClient;

        private ResourceManagementClient client;

        private int currentIndex = 0;

        private List<ResourceObject> resultSet;
        
        private int pageSize = 0;

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

        private bool EndOfSequence = false;

        internal SearchResults(EnumerateResponse response, int pageSize, SearchClient searchClient, ResourceManagementClient client)
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

        private void PopulateResultSet(ItemListType items)
        {
            if (items != null)
            {
                int startCount = resultSet.Count;

                foreach (XmlElement item in items.Any.OfType<XmlElement>())
                {
                    this.resultSet.Add(new ResourceObject(item, this.client));
                }
            }
        }

        public IEnumerator<ResourceObject> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public ResourceObject Current
        {
            get
            {
                return this.current;
            }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get
            {
                return this.current;
            }
        }

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

        public void Reset()
        {
            this.current = null;
            this.currentIndex = 0;
        }
    }
}
