using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Lithnet.ResourceManagement.Client.ResourceManagementService
{
    internal interface ISearchClient
    {
        Task<ISearchResultCollection> EnumerateGreedyAsync(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes, CultureInfo locale, CancellationTokenSource cancellationToken);

        Task<SearchResultPager> EnumeratePagedAsync(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes, CultureInfo locale);

        Task<ISearchResultCollection> EnumerateSyncAsync(string filter, int pageSize, IEnumerable<string> attributesToReturn, IEnumerable<SortingAttribute> sortingAttributes, CultureInfo locale);

        Task<PullResponse> PullAsync(EnumerationContextType context, int pageSize);

        Task ReleaseAsync(EnumerationContextType context);
    }
}