using System;
using Figo.Client.Constants;
using Figo.Client.Core.Client;
using Microsoft.Extensions.Configuration;

namespace Figo.Client.Extensions
{
    public static class FigoConfigurationsExtensions
    {
        public static Configuration AsIReadableConfiguration(this IConfiguration configuration, string accessToken = default)
        {
            var username = configuration[ConfigurationConstants.FigoUserName];
            var password = configuration[ConfigurationConstants.FigoPassword];
            var basePath = configuration[ConfigurationConstants.FigoServiceUrl];

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException($"{ConfigurationConstants.FigoUserName} null in appsettings");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException($"{ConfigurationConstants.FigoPassword} null in appsettings");
            }

            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentNullException($"{ConfigurationConstants.FigoServiceUrl} null in appsettings");
            }

            var config = new Configuration
            {
                Username = username,
                Password = password,
                AccessToken = accessToken,
                BasePath = basePath
            };

            return (Configuration) Configuration.MergeConfigurations(GlobalConfiguration.Instance, config);
        }
    }
}