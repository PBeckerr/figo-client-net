using System;
using System.Threading.Tasks;
using Figo.Client.Abstractions;
using Figo.Client.Core.Api;
using Figo.Client.Core.Client;
using Figo.Client.Core.Model;
using Figo.Client.Models;
using Microsoft.Extensions.Configuration;

namespace Figo.Client
{
    public class AccountService : FigoClientService

    {
        public AccountService(Configuration configuration) : base(configuration)
        {
        }

        public AccountService(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        ///     Retrieve the balance of an account identified by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="accessToken"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>AccountBalance</returns>
        public async Task<AccountBalance> GetAccountBalanceAsync(AccessTokenDto accessToken, string accountId, bool cents = false)
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
            var accountService = new AccountsApi(this.Configuration);
            return await accountService.GetAccountBalanceAsync(accountId, cents).ConfigureAwait(false);
        }

        /// <summary>
        ///     Get a list of all available accounts to which the user has granted access.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accessToken"></param>
        /// <returns>AccountsApiResponse</returns>
        public async Task<AccountsApiResponse> GetAllAsync(AccessTokenDto accessToken)
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
            var accountService = new AccountsApi(this.Configuration);
            return await accountService.ListAccountsAsync().ConfigureAwait(false);
        }

        /// <summary>
        ///     Retrieve detailed information about an account identified by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accessToken"></param>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Account</returns>
        public async Task<Account> GetDetailAsync(AccessTokenDto accessToken, string accountId, bool cents = false)
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
            var accountService = new AccountsApi(this.Configuration);
            return await accountService.GetAccountAsync(accountId, cents).ConfigureAwait(false);
        }
    }
}