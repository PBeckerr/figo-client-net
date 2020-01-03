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
    public class VerificationService : FigoClientService
    {
        public VerificationService(Configuration configuration) : base(configuration)
        {
        }

        public VerificationService(IConfiguration configuration, ILogger logger) : base(configuration, logger)
        {
        }

        /// <summary>
        ///     Get a payment receipt Get a payment receipt that indicates if the user has initiated the payment identified by
        ///     payment-id. This call might return a 404 as long as the user is still being guided through
        ///     the flow but has not yet executed the payment at the bank. The same holds true if the user aborts the flow.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accessToken"></param>
        /// <param name="paymentId"></param>
        /// <returns>PaymentReceipt</returns>
        public async Task<PaymentReceipt> GetPaymentReceipt(AccessTokenDto accessToken, string paymentId)
        {
            if (accessToken == null)
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (!accessToken.IsValid)
            {
                throw new ArgumentException($"{nameof(accessToken)} is expired.");
            }

            if (paymentId == null)
            {
                throw new ArgumentNullException(nameof(paymentId));
            }

            this.Configuration.AccessToken = accessToken.AccessToken;
            var api = new VerificationApi(this.Configuration, this.Logger);
            return await api.GetPaymentReceiptAsync(paymentId).ConfigureAwait(false);
        }
    }
}