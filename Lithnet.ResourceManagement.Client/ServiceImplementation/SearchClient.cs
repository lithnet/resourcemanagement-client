using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.ResourceManagement.WebServices.WSEnumeration;
using Lithnet.ResourceManagement.Client.ServiceImplementation;
using System.Xml;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal partial class SearchClient : System.ServiceModel.ClientBase<Lithnet.ResourceManagement.Client.ResourceManagementService.Search>, Lithnet.ResourceManagement.Client.ResourceManagementService.Search
    {
        private const int DefaultPageSize = 200;

        private ResourceManagementClient client;

        public void Initialize(ResourceManagementClient client)
        {
            this.DisableContextManager();
            this.client = client;
        }

        public ISearchResultCollection EnumerateAsync(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes, CancellationTokenSource cancellationToken)
        {
            return this.Enumerate(filter, pageSize, attributesToReturn, sortingAttributes, cancellationToken, true);
        }

        public ISearchResultCollection EnumerateSync(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes)
        {
            return this.Enumerate(filter, pageSize, attributesToReturn, sortingAttributes, null, false);
        }

        public SearchResultPager EnumeratePaged(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes)
        {
            if (pageSize < 0)
            {
                pageSize = DefaultPageSize;
            }

            var response = this.Enumerate(filter, 0, attributesToReturn, sortingAttributes);
            return new SearchResultPager(response, pageSize, this, this.client);
        }

        public DataPage<ResourceObject> EnumeratePagedSync(string filter, int pageNumber, int pageSize, IEnumerable<string> attributesToGet,
            string sortingAttributeName, bool sortingAscending)
        {

            // first request - only to get enumeration context and total rows count
            // do not fetch any items - total count only; however this cannot be set to 0 because then total count is not returned (is always set to 0)
            // get only ObjectID attribute to minimize overhead caused by this operation which returns elements not used for further processing
            var enumerateResponse = this.Enumerate(filter, 1, new List<string> { AttributeNames.ObjectID }, null);
            int totalCount = Convert.ToInt32(enumerateResponse.EnumerationDetail.Count);

            // second request - to actually get desired data
            var context = enumerateResponse.EnumerationContext;
            context.Selection = attributesToGet != null ? attributesToGet.ToArray() : null;

            if (pageSize <= 0)
            {
                pageSize = DefaultPageSize;
            }

            if (pageNumber > 0)
            {
                context.CurrentIndex = (pageNumber - 1) * pageSize;
            }
            else
            {
                context.CurrentIndex = 0;
            }

            if (!String.IsNullOrWhiteSpace(sortingAttributeName))
            {
                var sortingAttribute = new SortingAttribute(sortingAttributeName, sortingAscending);
                context.Sorting = new Sorting()
                {
                    SortingAttributes = new SortingAttribute[]
                    {
                    sortingAttribute
                    }
                };
            }

            var results = this.Pull(context, pageSize);
            var downloadedRecords = new List<ResourceObject>();
            if (results.Items != null)
            {
                foreach (var item in results.Items.Any.OfType<XmlElement>())
                {
                    downloadedRecords.Add(new ResourceObject(item, this.client));
                }
            }

            return new DataPage<ResourceObject>(downloadedRecords, totalCount);
        }

        private EnumerateResponse Enumerate(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes)
        {
            if (pageSize < 0)
            {
                pageSize = DefaultPageSize;
            }

            using (Message requestMessage = MessageComposer.CreateEnumerateMessage(filter, pageSize, attributesToReturn, sortingAttributes))
            {
                using (Message responseMessage = this.Invoke((c) => c.Enumerate(requestMessage)))
                {
                    responseMessage.ThrowOnFault();

                    EnumerateResponse response = responseMessage.DeserializeMessageWithPayload<EnumerateResponse>();
                    return response;
                }
            }
        }

        private ISearchResultCollection Enumerate(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes, CancellationTokenSource cancellationToken, bool searchAsync)
        {
            if (pageSize < 0)
            {
                pageSize = DefaultPageSize;
            }

            var response = this.Enumerate(filter, pageSize, attributesToReturn, sortingAttributes);
            if (searchAsync)
            {
                return new SearchResultCollectionAsync(response, pageSize, this, cancellationToken, this.client);
            }
            else
            {
                return new SearchResultCollection(response, pageSize, this, this.client);
            }
        }

        internal PullResponse Pull(EnumerationContextType context, int pageSize)
        {
            using (Message pullRequest = MessageComposer.GeneratePullMessage(context, pageSize))
            {
                using (Message responseMessage = this.Invoke((c) => c.Pull(pullRequest)))
                {
                    responseMessage.ThrowOnFault();

                    PullResponse pullResponseTyped = responseMessage.DeserializeMessageWithPayload<PullResponse>();
                    return pullResponseTyped;
                }
            }
        }

        internal void Release(EnumerationContextType context)
        {
            using (Message releaseRequest = MessageComposer.GenerateReleaseMessage(context))
            {
                using (Message responseMessage = Release(releaseRequest))
                {
                    releaseRequest.ThrowOnFault();
                }
            }
        }
    }
}