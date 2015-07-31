using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.ResourceManagement.WebServices.WSEnumeration;

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

        //public ISearchResultCollection EnumerateSync(string filter)
        //{
        //    return this.EnumerateSync(filter, -1, null);
        //}

        //public ISearchResultCollection EnumerateSync(string filter, int pageSize)
        //{
        //    return this.EnumerateSync(filter, pageSize, null);
        //}

        //public ISearchResultCollection EnumerateSync(string filter, IEnumerable<string> attributesToReturn)
        //{
        //    return this.EnumerateSync(filter, -1, attributesToReturn);
        //}

        //public ISearchResultCollection EnumerateAsync(string filter)
        //{
        //    return this.EnumerateAsync(filter, -1, null, null);
        //}

        //public ISearchResultCollection EnumerateAsync(string filter, int pageSize)
        //{
        //    return this.EnumerateAsync(filter, pageSize, null, cancellationToken);
        //}

        //public ISearchResultCollection EnumerateAsync(string filter, CancellationTokenSource cancellationToken)
        //{
        //    return this.EnumerateAsync(filter, -1, null, cancellationToken);
        //}

        //public ISearchResultCollection EnumerateAsync(string filter, int pageSize, CancellationTokenSource cancellationToken)
        //{
        //    return this.EnumerateAsync(filter, pageSize, null, cancellationToken);
        //}

        public ISearchResultCollection EnumerateAsync(string filter, int pageSize, IEnumerable<string> attributesToReturn, CancellationTokenSource cancellationToken)
        {
            return this.Enumerate(filter, pageSize, attributesToReturn, cancellationToken, true);
        }

        public ISearchResultCollection EnumerateSync(string filter, int pageSize, IEnumerable<string> attributesToReturn)
        {
            return this.Enumerate(filter, pageSize, attributesToReturn, null, false);
        }

        private ISearchResultCollection Enumerate(string filter, int pageSize, IEnumerable<string> attributesToReturn, CancellationTokenSource cancellationToken, bool searchAsync)
        {
            if (pageSize < 0)
            {
                pageSize = DefaultPageSize;
            }

            using (Message requestMessage = MessageComposer.CreateEnumerateMessage(filter, pageSize, attributesToReturn))
            {
                using (Message responseMessage = this.Invoke((c) => c.Enumerate(requestMessage)))
                {
                    responseMessage.ThrowOnFault();

                    EnumerateResponse response = responseMessage.DeserializeMessageWithPayload<EnumerateResponse>();

                    if (searchAsync)
                    {
                        return new SearchResultCollectionAsync(response, pageSize, this, cancellationToken, this.client);
                    }
                    else
                    {
                        return new SearchResultCollection(response, pageSize, this, this.client);
                    }
                }
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