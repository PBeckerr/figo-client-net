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
    public class AccessTokenService : FigoClientService
    {
        public AccessTokenService(Configuration configuration) : base(configuration)
        {
        }

        public AccessTokenService(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        ///     Checks if the given token is expired anf if it is attempts to refresh it
        /// </summary>
        /// <param name="existingToken"></param>
        /// <returns>Tuple of AccessToken and if it was refreshed</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<(AccessTokenDto token, bool refreshed)> CheckAndRevalidateIfNeededAsync(AccessTokenDto existingToken)
        {
            if (existingToken == null)
            {
                throw new ArgumentNullException(nameof(existingToken));
            }

            if (!existingToken.IsValid)
            {
                var client = new AuthorizationApi(this.Configuration);
                var refreshRequest =
                    await client.RefreshAccessTokenAsync(new RefreshTokenRequest(RefreshTokenRequest.GrantTypeEnum.Refreshtoken, existingToken.RefreshToken))
                                .ConfigureAwait(false);

                return (AccessTokenDto.FromAccessTokenCreated(refreshRequest), true);
            }

            return (existingToken, false);
        }

        /// <summary>
        ///     Login
        /// </summary>
        /// <param name="authcode"></param>
        /// <param name="redirectUrl"></param>
        /// <returns>AccessToken</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<AccessTokenDto> LoginAsync(string authcode, string redirectUrl)
        {
            if (string.IsNullOrEmpty(authcode))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(authcode));
            }

            if (string.IsNullOrEmpty(redirectUrl))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(redirectUrl));
            }

            var client = new AuthorizationApi(this.Configuration);
            var token = await client
                              .CreateAccessTokenAsync(new AuthorizationCodeRequest(
                                                          AuthorizationCodeRequest.GrantTypeEnum.Authorizationcode,
                                                          authcode,
                                                          redirectUrl))
                              .ConfigureAwait(false);
            return AccessTokenDto.FromAccessTokenCreated(token);
        }
    }
}