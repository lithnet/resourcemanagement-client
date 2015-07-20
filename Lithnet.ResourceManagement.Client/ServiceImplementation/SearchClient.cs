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

        public void Initialize()
        {
            this.DisableContextManager();
        }

        public ISearchResults Enumerate(string filter)
        {
            return this.Enumerate(filter, -1, null, null);
        }

        public ISearchResults Enumerate(string filter, int pageSize)
        {
            return this.Enumerate(filter, pageSize, null, null);
        }

        public ISearchResults Enumerate(string filter, IEnumerable<string> attributesToReturn)
        {
            return this.Enumerate(filter, -1, attributesToReturn, null);
        }

        public ISearchResults Enumerate(string filter, CancellationTokenSource cancellationToken)
        {
            return this.Enumerate(filter, -1, null, cancellationToken);
        }

        public ISearchResults Enumerate(string filter, int pageSize, CancellationTokenSource cancellationToken)
        {
            return this.Enumerate(filter, pageSize, null, cancellationToken);
        }

        public ISearchResults Enumerate(string filter, int pageSize, IEnumerable<string> attributesToReturn)
        {
            return this.Enumerate(filter, pageSize, attributesToReturn, null);
        }

        public ISearchResults Enumerate(string filter, IEnumerable<string> attributesToReturn, CancellationTokenSource cancellationToken)
        {
            return this.Enumerate(filter, -1, attributesToReturn, cancellationToken);
        }

        public ISearchResults Enumerate(string filter, int pageSize, IEnumerable<string> attributesToReturn, CancellationTokenSource cancellationToken)
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

                    if (cancellationToken != null)
                    {
                        return new SearchResultsAsync(response, pageSize, this, cancellationToken.Token);
                    }
                    else
                    {
                        return new SearchResults(response, pageSize, this);
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


        public T Invoke<T>(Func<Search, T> action)
        {

            Search c = this.ChannelFactory.CreateChannel();
            T returnValue;

            try
            {
                ((IClientChannel)c).Open();
                returnValue = action(c);
                ((IClientChannel)c).Close();
                return returnValue;
            }
            catch
            {
                ((IClientChannel)c).Abort();
                throw;
            }
        }
    }
}
