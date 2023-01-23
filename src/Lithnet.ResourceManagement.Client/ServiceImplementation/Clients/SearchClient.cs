using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal class SearchClient : ISearchClient
    {
        private const int DefaultPageSize = 200;

        private IClientFactory client;
        private ISearch channel;

        public SearchClient(IClientFactory client, ISearch channel)
        {
            this.client = client;
            this.channel = channel;
        }

        public async Task<ISearchResultCollection> EnumerateGreedyAsync(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes, CultureInfo locale, CancellationTokenSource cancellationToken)
        {
            return new SearchResultCollectionAsync(await this.EnumeratePagedAsync(filter, pageSize, attributesToReturn, sortingAttributes, locale).ConfigureAwait(false), cancellationToken);
        }

        public async Task<ISearchResultCollection> EnumerateSyncAsync(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes, CultureInfo locale)
        {
            return new SearchResultCollection(await this.EnumeratePagedAsync(filter, pageSize, attributesToReturn, sortingAttributes, locale).ConfigureAwait(false));
        }

        public async Task<SearchResultPager> EnumeratePagedAsync(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes, CultureInfo locale)
        {
            if (pageSize < 0)
            {
                pageSize = DefaultPageSize;
            }

            var response = await this.EnumerateAsync(filter, 0, attributesToReturn, sortingAttributes, locale).ConfigureAwait(false);
            return new SearchResultPager(response, pageSize, this.client, locale);
        }

        private async Task<EnumerateResponse> EnumerateAsync(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes, CultureInfo locale)
        {
            if (pageSize < 0)
            {
                pageSize = DefaultPageSize;
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            List<string> fixedAttributesToReturn = new List<string>();
            if (attributesToReturn != null)
            {
                foreach (var attribute in attributesToReturn)
                {
                    fixedAttributesToReturn.Add(await this.client.SchemaClient.GetCorrectAttributeNameCaseAsync(attribute));
                }
            }

            if (sortingAttributes != null)
            {
                foreach (var attribute in sortingAttributes)
                {
                    attribute.AttributeName = await this.client.SchemaClient.GetCorrectAttributeNameCaseAsync(attribute.AttributeName);
                }
            }

            using (Message requestMessage = MessageComposer.CreateEnumerateMessage(filter, pageSize, fixedAttributesToReturn?.ToArray(), sortingAttributes?.ToArray(), locale))
            {
                using (Message responseMessage = await this.channel.EnumerateAsync(requestMessage).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();

                    return responseMessage.DeserializeMessageWithPayload<EnumerateResponse>();
                }
            }
        }

        public async Task<PullResponse> PullAsync(EnumerationContextType context, int pageSize)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            using (Message pullRequest = MessageComposer.GeneratePullMessage(context, pageSize))
            {
                using (Message responseMessage = await this.channel.PullAsync(pullRequest).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();

                    return responseMessage.DeserializeMessageWithPayload<PullResponse>();
                }
            }
        }

        public async Task ReleaseAsync(EnumerationContextType context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            using (Message releaseRequest = MessageComposer.GenerateReleaseMessage(context))
            {
                using (Message responseMessage = await this.channel.ReleaseAsync(releaseRequest).ConfigureAwait(false))
                {
                    responseMessage.ThrowOnFault();
                }
            }
        }
    }
}