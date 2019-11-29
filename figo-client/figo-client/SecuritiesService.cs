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
    public class SecuritiesService : FigoClientService
    {
        public SecuritiesService(Configuration configuration) : base(configuration)
        {
        }

        public SecuritiesService(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        ///     Get a list of the securities of all accounts. You can additionally constrain the amount of
        ///     securities being returned by using the query parameters described below as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accessToken"></param>
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
        /// </param>
        /// <returns>SecuritiesApiResponse</returns>
        public async Task<SecuritiesApiResponse> GetAllAsync(AccessTokenDto accessToken, SecurityListFilter listFilter = default)
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
            var securitiesApi = new SecuritiesApi(this.Configuration);
            return await securitiesApi.ListSecuritiesAsync(null, listFilter?.Count, listFilter?.Offset, listFilter?.Since?.ToString("O"), listFilter?.SinceType)
                                      .ConfigureAwait(false);
        }

        /// <summary>
        ///     Get a list of the securities of account per ID. You can additionally constrain the amount of
        ///     securities being returned by using the query parameters described below as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accessToken"></param>
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
        /// </param>
        /// <returns>SecuritiesApiResponse</returns>
        public async Task<SecuritiesApiResponse> GetAllAsync(AccessTokenDto accessToken, string accountId, SecurityListFilter listFilter = default)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }

            if (!accessToken.IsValid)
            {
                throw new ArgumentException($"{nameof(accessToken)} is expired.");
            }

            this.Configuration.AccessToken = accessToken.AccessToken;
            var securitiesApi = new SecuritiesApi(this.Configuration);
            return await securitiesApi
                         .ListSecuritiesOfAccountAsync(accountId,
                                                       listFilter?.Count,
                                                       listFilter?.Offset,
                                                       listFilter?.Since?.ToString("O"),
                                                       listFilter?.SinceType)
                         .ConfigureAwait(false);
        }

        /// <summary>
        ///     Retrieve a single security associated to a specific account by its security-id.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="securityId"></param>
        /// <returns>Security</returns>
        public async Task<Security> GetDetail(AccessTokenDto accessToken, string accountId, string securityId)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (!accessToken.IsValid)
            {
                throw new ArgumentException($"{nameof(accessToken)} is expired.");
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(accountId));
            }

            this.Configuration.AccessToken = accessToken.AccessToken;
            var securitiesApi = new SecuritiesApi(this.Configuration);
            return await securitiesApi.GetSecurityOfAccountAsync(accountId, securityId).ConfigureAwait(false);
        }
    }
}