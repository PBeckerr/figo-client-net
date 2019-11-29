using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Figo.Client.Abstractions;
using Figo.Client.Core.Api;
using Figo.Client.Core.Client;
using Figo.Client.Core.Model;
using Microsoft.Extensions.Configuration;

namespace Figo.Client
{
    public class InitiationService : FigoClientService
    {
        public InitiationService(Configuration configuration) : base(configuration)
        {
        }

        public InitiationService(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        ///     Inits and requests a redirectUrl for a payment
        /// </summary>
        /// <param name="stateToken"></param>
        /// <param name="redirectUri"></param>
        /// <param name="accountId"></param>
        /// <param name="payment"></param>
        /// <param name="tanSchemeId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Uri> GetRedirectUriForPaymentAsync(
            string stateToken,
            string redirectUri,
            string accountId,
            Payment payment,
            string tanSchemeId)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment));
            }

            if (string.IsNullOrEmpty(redirectUri))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(redirectUri));
            }

            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(accountId));
            }

            if (string.IsNullOrEmpty(tanSchemeId))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(tanSchemeId));
            }

            var init = new InitiationApi(this.Configuration);
            var shieldToken = await init
                                    .CreatePaymentShieldTokenAsync(new ShieldTokenPISRequest(
                                                                       stateToken,
                                                                       redirectUri,
                                                                       accountId,
                                                                       payment,
                                                                       tanSchemeId)).ConfigureAwait(false);

            return new Uri(shieldToken.Location);
        }

        /// <summary>
        ///     Inits and requests a redirectUrl for a shieldToken
        /// </summary>
        /// <param name="stateToken"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Uri> GetRedirectUriForShieldTokenAsync(string stateToken, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(returnUrl));
            }

            var init = new InitiationApi(this.Configuration);
            var shieldToken = await init.CreateConnectShieldTokenAsync(new ShieldTokenRequest(stateToken, returnUrl)).ConfigureAwait(false);

            return new Uri(shieldToken.Location);
        }

        /// <summary>
        ///     Inits and requests a redirectUrl for bank account sychronisation
        /// </summary>
        /// <param name="stateToken"></param>
        /// <param name="returnUrl"></param>
        /// <param name="accountIds"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Uri> GetRedirectUriForSyncAccountsAsync(string stateToken, string returnUrl, List<string> accountIds)
        {
            if (accountIds == null)
            {
                throw new ArgumentNullException(nameof(accountIds));
            }

            if (accountIds.Count == 0)
            {
                throw new ArgumentException(nameof(accountIds) + " empty.");
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(returnUrl));
            }

            var init = new InitiationApi(this.Configuration);
            var shieldToken = await init.CreateSyncShieldTokenAsync(new ShieldTokenSyncRequest(
                                                                        stateToken,
                                                                        returnUrl,
                                                                        accountIds))
                                        .ConfigureAwait(false);

            return new Uri(shieldToken.Location);
        }
    }
}