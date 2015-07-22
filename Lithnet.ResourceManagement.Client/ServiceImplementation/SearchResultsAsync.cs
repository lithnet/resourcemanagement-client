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
    public class SearchResultsAsync : ISearchResults
    {
        private EnumerationContextType context;

        private EnumerationDetailType details;

        private SearchClient searchClient;

        private ResourceManagementClient client;

        private BlockingCollection<ResourceObject> resultSet;

        private int pageSize = 0;

        private IEnumerable<ResourceObject> consumingEnumerable;

        private CancellationToken token;

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

        internal SearchResultsAsync(EnumerateResponse response, int pageSize, SearchClient searchClient, CancellationToken token, ResourceManagementClient client)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

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
            this.token = token;
            this.context = response.EnumerationContext;
            this.pageSize = pageSize;
            this.details = response.EnumerationDetail;
            this.searchClient = searchClient;
            this.EndOfSequence = response.EndOfSequence != null;
            this.PopulateResultSet(response.Items);
            Debug.WriteLine("Enumeration started. End of request: {0}. Expected results: {1}", this.EndOfSequence, this.Count);
            this.ExecuteProducer();

        }

        private void ExecuteProducer()
        {
            Task task = new Task(() =>
                {
                    while (this.EndOfSequence == false)
                    {
                        if (this.token.IsCancellationRequested)
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

        private void ReleaseEnumerationContext()
        {
            try
            {
                this.searchClient.Release(this.context);
            }
            catch
            { }
        }

        public IEnumerator<ResourceObject> GetEnumerator()
        {
            return this.consumingEnumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.consumingEnumerable.GetEnumerator();
        }
    }
}
