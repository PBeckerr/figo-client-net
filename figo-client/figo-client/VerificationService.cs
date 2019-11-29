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
    public class VerificationService : FigoClientService
    {
        public VerificationService(Configuration configuration) : base(configuration)
        {
        }

        public VerificationService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<PaymentReceipt> GetPaymentReceipt(AccessTokenDto token, string paymentId)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!token.IsValid)
            {
                throw new ArgumentException($"{nameof(token)} is expired.");
            }

            if (paymentId == null)
            {
                throw new ArgumentNullException(nameof(paymentId));
            }

            this.Configuration.AccessToken = token.AccessToken;
            var api = new VerificationApi(this.Configuration);
            return await api.GetPaymentReceiptAsync(paymentId).ConfigureAwait(false);
        }
    }
}