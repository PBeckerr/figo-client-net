using System;
using System.Threading.Tasks;
using Figo.Client.Abstractions;
using Figo.Client.Core.Api;
using Figo.Client.Core.Client;
using Figo.Client.Core.Model;
using Figo.Client.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Figo.Client
{
    public class StandingOrderService : FigoClientService
    {
        public StandingOrderService(Configuration configuration) : base(configuration)
        {
        }

        public StandingOrderService(IConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
        }

        /// <summary>
        ///     Retrieve a single standing order associated to a specific account by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accessToken"></param>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>Task of StandingOrder</returns>
        public async Task<StandingOrder> GetAccountDetailAsync(AccessTokenDto accessToken, string accountId, string standingOrderId)
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

            if (standingOrderId == null)
            {
                throw new ArgumentNullException(nameof(standingOrderId));
            }

            this.Configuration.AccessToken = accessToken.AccessToken;
            var standingOrdersApi = new StandingOrdersApi(this.Configuration, this.Logger);
            return await standingOrdersApi
                         .GetStandingOrderOfAccountAsync(accountId, standingOrderId)
                         .ConfigureAwait(false);
        }

        /// <summary>
        ///     Get a list of the standing orders of a specific account. You can additionally
        ///     constrain the amount of standing orders being returned by using the query parameters described below as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accessToken"></param>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of StandingOrdersResponse</returns>
        public async Task<StandingOrdersResponse> GetAllAsync(AccessTokenDto accessToken, string accountId, bool cents = false)
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
            var standingOrdersApi = new StandingOrdersApi(this.Configuration, this.Logger);
            return await standingOrdersApi
                         .ListStandingOrdersOfAccountAsync(accountId, cents).ConfigureAwait(false);
        }

        /// <summary>
        ///     Get a list of the standing orders of all accounts. You can additionally constrain the amount
        ///     of standing orders being returned by using the query parameters described as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <returns>Task of StandingOrdersResponse</returns>
        public async Task<StandingOrdersResponse> GetAllAsync(AccessTokenDto accessToken)
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
            var standingOrdersApi = new StandingOrdersApi(this.Configuration, this.Logger);
            return await standingOrdersApi.ListStandingOrdersAsync().ConfigureAwait(false);
        }

        /// <summary>
        ///     Retrieve a single standing order associated to a specific account by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accessToken"></param>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>Task of StandingOrder</returns>
        public async Task<StandingOrder> GetDetailAsync(AccessTokenDto accessToken, string accountId, string standingOrderId)
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

            if (standingOrderId == null)
            {
                throw new ArgumentNullException(nameof(standingOrderId));
            }

            this.Configuration.AccessToken = accessToken.AccessToken;
            var standingOrdersApi = new StandingOrdersApi(this.Configuration, this.Logger);
            return await standingOrdersApi
                         .GetStandingOrderOfAccountAsync(accountId, standingOrderId)
                         .ConfigureAwait(false);
        }
    }
}