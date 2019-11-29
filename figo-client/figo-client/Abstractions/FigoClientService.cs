using Figo.Client.Core.Client;
using Figo.Client.Extensions;
using Microsoft.Extensions.Configuration;

namespace Figo.Client.Abstractions
{
    public abstract class FigoClientService
    {
        protected readonly Configuration Configuration;

        protected FigoClientService(Configuration configuration)
        {
            this.Configuration = configuration;
        }

        protected FigoClientService(IConfiguration configuration)
        {
            this.Configuration = configuration.AsIReadableConfiguration();
        }
    }
}