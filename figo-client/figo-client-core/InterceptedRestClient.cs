using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace Figo.Client.Core
{
    public class InterceptedRestClient : RestClient
    {
        private readonly ILogger _logger;

        public InterceptedRestClient(string baseUrl, ILogger logger = null) : base(baseUrl)
        {
            this._logger = logger;
        }

        private void LogRequest(IRestRequest request, IRestResponse response, long durationMs)
        {
            this._logger?.LogDebug(FormatForLogging());

            string FormatForLogging()
            {
                var requestToLog = new
                {
                    resource = request.Resource,
                    // Parameters are custom anonymous objects in order to have the parameter type as a nice string
                    // otherwise it will just show the enum value
                    parameters = request.Parameters.Select(parameter => new
                    {
                        name = parameter.Name,
                        value = parameter.Value,
                        type = parameter.Type.ToString()
                    }),
                    // ToString() here to have the method as a nice string otherwise it will just show the enum value
                    method = request.Method.ToString(),
                    // This will generate the actual Uri used in the request
                    uri = this.BuildUri(request)
                };

                var responseToLog = new
                {
                    statusCode = response.StatusCode,
                    content = response.Content,
                    headers = response.Headers,
                    // The Uri that actually responded (could be different from the requestUri if a redirection occurred)
                    responseUri = response.ResponseUri,
                    errorMessage = response.ErrorMessage
                };

                return
                    $"Request completed in {durationMs} ms, {Environment.NewLine}Request: {JsonConvert.SerializeObject(requestToLog, Formatting.Indented)}, {Environment.NewLine}Response: {JsonConvert.SerializeObject(responseToLog, Formatting.Indented)}";
            }
        }
    }
}