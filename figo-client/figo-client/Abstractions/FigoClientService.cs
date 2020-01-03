using Figo.Client.Core.Client;
using Figo.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Figo.Client.Abstractions
{
    public abstract class FigoClientService
    {
        protected readonly Configuration Configuration;
        protected readonly ILogger Logger;

        protected FigoClientService(Configuration configuration)
        {
            this.Configuration = configuration;
        }

        protected FigoClientService(IConfiguration configuration, ILogger logger)
        {
            this.Configuration = configuration.AsIReadableConfiguration();
            this.Logger = logger;
        }
    }
}