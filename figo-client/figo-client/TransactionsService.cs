using System;
using System.Threading.Tasks;
using Figo.Client.Abstractions;
using Figo.Client.Core.Api;
using Figo.Client.Core.Client;
using Figo.Client.Core.Model;
using Figo.Client.Models;
using Figo.Client.Models.Filter;
using Microsoft.Extensions.Configuration;

namespace Figo.Client
{
    public class TransactionsService : FigoClientService

    {
        public TransactionsService(Configuration configuration) : base(configuration)
        {
        }

        public TransactionsService(IConfiguration configuration) : base(configuration)
        {
        }


        /// <summary>
        ///     Get a list of the transactions associated with a specific account. You can
        ///     additionally constrain the amount of transactions being returned by using the query parameters described below as
        ///     filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="listFilter">
        ///     (optional)
        ///     count:
        ///     Limit the number of returned items. In combination with the offset parameter this can be used to
        ///     paginate the result list. (optional, default to 1000)
        ///     offset:
        ///     Skip this number of transactions in the response. In combination with the count parameter this can
        ///     be used to paginate the result list. (optional, default to 0)
        ///     since:
        ///     Return only transactions after this date based on since_type. This parameter can either
        ///     be a transaction ID or a date. Given at least one transaction matches the filter criterion, if provided as
        ///     transaction ID the result will *not* contain this ID. If provided as ISO date, the result *will* contain this date.
        ///     This behavior may change in the future. (optional)
        ///     sinceType:
        ///     This parameter defines how the parameter since will be interpreted. (optional,
        ///     default to created)
        ///     filter:
        ///     Filter transactions by given key:value combination. Possible keys:    - date (maps to
        ///     booked_at, please use ISO date here, not datetime)   - person (maps to payer/payee name)   - purpose
        ///     - amount  Values are interpreted using wildcards: person:John Doe will match %John Doe%.
        ///     (optional)
        ///     includePending:
        ///     This flag indicates whether pending transactions should be included in the response.
        ///     Pending transactions are always included as a complete set, regardless of the since parameter. (optional, default
        ///     to false)
        ///     sort
        ///     Determines whether results will be sorted in ascending or descending order. (optional)
        ///     until:
        ///     Return only transactions which were booked on or before this date. Please provide as ISO date. It
        ///     is not possible to use the since_type semantics with until. (optional)
        ///     includeStatistics:
        ///     If true includes statistics on the returned transactions: maximum deposit,
        ///     total deposits, maximum expense, total expenses.  (optional, default to false)
        ///     filter:
        ///     Filter transactions by given key:value combination. Possible keys:    - date (maps to
        ///     booked_at, please use ISO date here, not datetime)   - person (maps to payer/payee name)   - purpose
        ///     - amount  Values are interpreted using wildcards: person:John Doe will match %John Doe%.
        ///     (optional)
        /// </param>
        /// <returns>ApiResponse of TransactionList</returns>
        public async Task<TransactionList> GetAllAsync(AccessTokenDto accessToken, string accountId, AccountListFilter listFilter = default)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (!accessToken.IsValid)
            {
                throw new ArgumentException($"{nameof(accessToken)} is expired.");
            }

            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            this.Configuration.AccessToken = accessToken.AccessToken;
            var transactionsApi = new TransactionsApi(this.Configuration);
            return await transactionsApi
                         .ListTransactionsOfAccountAsync(
                             accountId,
                             null,
                             listFilter?.Filter,
                             listFilter?.Count,
                             listFilter?.Offset,
                             listFilter?.IncludePending,
                             listFilter?.Sort,
                             listFilter?.Since?.ToString("O"),
                             listFilter?.Until?.ToString("O"),
                             listFilter?.SinceType,
                             listFilter?.Types,
                             listFilter?.Cents,
                             listFilter?.IncludeStatistics).ConfigureAwait(false);
        }

        public async Task<TransactionList> GetAllAsync(AccessTokenDto accessToken, AccountListFilter listFilter = default)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (!accessToken.IsValid)
            {
                throw new ArgumentException($"{nameof(accessToken)} is expired.");
            }

            this.Configuration.AccessToken = accessToken.AccessToken;
            var transactionsApi = new TransactionsApi(this.Configuration);
            return await transactionsApi.ListTransactionsAsync(
                null,
                listFilter?.Filter,
                null,
                listFilter?.Count,
                listFilter?.Offset,
                listFilter?.Sort,
                listFilter?.Since?.ToString("O"),
                listFilter?.Until?.ToString("O"),
                listFilter?.SinceType,
                listFilter?.Types,
                listFilter?.Cents,
                listFilter?.IncludePending,
                listFilter?.IncludeStatistics).ConfigureAwait(false);
        }

        public async Task<Transaction> GetDetailAsync(AccessTokenDto accessToken, string accountId, string transactionId)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (!accessToken.IsValid)
            {
                throw new ArgumentException($"{nameof(accessToken)} is expired.");
            }

            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (transactionId == null)
            {
                throw new ArgumentNullException(nameof(transactionId));
            }

            this.Configuration.AccessToken = accessToken.AccessToken;
            var transactionsApi = new TransactionsApi(this.Configuration);
            return await transactionsApi
                         .GetTransctionOfAccountAsync(accountId, transactionId)
                         .ConfigureAwait(false);
        }

        public async Task<Transaction> GetDetailAsync(AccessTokenDto accessToken, string transactionId)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (!accessToken.IsValid)
            {
                throw new ArgumentException($"{nameof(accessToken)} is expired.");
            }

            if (transactionId == null)
            {
                throw new ArgumentNullException(nameof(transactionId));
            }

            this.Configuration.AccessToken = accessToken.AccessToken;
            var transactionsApi = new TransactionsApi(this.Configuration);
            return await transactionsApi
                         .GetTransactionAsync(transactionId)
                         .ConfigureAwait(false);
        }
    }
}