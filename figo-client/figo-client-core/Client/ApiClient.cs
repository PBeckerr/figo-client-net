/* 
 * Ongoing Account Information (+AIS)
 *
 * # Introduction  RegShield provides secure access to user financial data in compliance with PSD2.  To implement a Regshield workflow the partner initiates an operation by calling one of the endpoints to create a ShieldToken. This will generate a location which is used to send the user to the RegShield UI, where the user has total control what action will be performed on which of their financial sources. Upon successful completion of the UI workflow the partner gets a callback and can take further actions on their side.  The available operations in the context of the Account Information Service are:  * [Connecting to a financial source and granting access rights](#section/Introduction/Connecting-a-financial-source) * [Synchronizing data from a financial source](#section/Introduction/Synchronization) * [Initiating a payment from a connected financial source](#section/Introduction/Initiating-a-payment)   ## Connecting a financial source  The following listing enumerates all required steps to gain access to the user's financial data:  1. Create a [ShieldToken](#operation/createConnectShieldToken) for authentication, declaration of    intent and flow configuration. 2. Forward the user to the `location` provided in the ShieldToken response. This can be achieved by    using     * an Overlay/PopUp iframe (please ask your personal figo contact for details)     * a redirect in the same or a new window 3. The RegShield UI will guide the user through the process of selecting the financial sources they    want to provide access to. figo will then fetch the financial data associated with the user's    selection. 4. The RegShield UI redirects the user to the `redirect_uri` provided in the ShieldToken with the    following query-parameters    * `state`: The state that was provided when creating the ShieldToken.    * `success`: Indicator whether or not an initiated process was completed successfully.    * `code`: The Authorization Code to be processed by the partner. `code` is only returned on successful flow.     Example:      ```bash      https://example.com/callback?code=Oafd13...&state=a81132cf&success=true      ```    In the case of Overlay/PopUp iframe integration, a message will be posted   to the iframe container. Handling error cases is described in the   [Error Handling](#section/Introduction/Error-handling) section.   5. The partner exchanges the [authorization code for an access token and a refresh    token](#/paths/~1v3~1auth~1token/post). This invalidates the former existing refresh token for    the respective user. 6. The partner accesses the user's financial data by using the access token. 7. The aquired refresh tokens can be used to repeatedly    [create access tokens](#operation/createAccessToken) without the need for new authorization    codes. 8. The partner continues to access the user's financial data by using these access tokens.   <div class=\"diagram\">   sequenceDiagram     participant U as User Agent     participant P as Partner     participant API as RegShield API     participant UI as RegShield UI     U->>P:\\n     activate U     activate P     P->>+API: 1. Create ShieldToken     Note over P, API: state & redirect_uri are chosen<br/>by the Partner     API- ->>-P: ShieldToken     P- ->>U: Present RegShield UI location     deactivate P     U->>UI: 2. Open location     activate UI     Note left of UI: 3. User selects<br/>financial sources     UI- ->>U: 4. Redirect to redirect_uri     deactivate UI     U->>P: state, success & code     deactivate U     activate P     P->>+API: 5. Exchange Authorization Code     API- ->>-P: Access Token + Refresh Token     P->>+API: 6. List accounts     API- ->>-P: Accounts     deactivate P     loop Ongoing access       P->>+API: 7. Exchange Refresh Token       activate P       API- ->>-P: Access Token + Refresh Token       P->>+API: 8. List accounts       API- ->>-P: Accounts       deactivate P     end  </div>  ## Internationalization  RegShield UI supports internationalization. Localization defaults to German `de` when a locale isn't specified or supported.  RegShield UI does not attempt to read user's machine language preference i.e. `Accept-Language` header. You as a partner have to explicitly tell RegShield UI which language to display.  `lang` query parameter adheres to [ISO 639-1 codes](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes). RegShield UI currently supports German `de` and English `en` with more localization spupport on the way.  **Usage**\\ After creating a [ShieldToken](#operation/createShieldToken), append a query parameter `lang` to the generated ShieldToken `location` url.  ```js // Create ShieldToken successful response {   \"id\": \"3ca31c37-986a-454e-ad64-8e97143c86bc\",   \"location\": \"https://login.figo.io/?token=<ShieldToken>\" }  // Append `lang` parameter to \"location\" \"https://login.figo.io/?token=<ShieldToken>&lang=<language-key>\" ```  ## Synchronization  When connecting to a financial source the user can optionally allow figo to store their provider credentials. This allows figo to periodically synchronize the financial data from the provider and make these updates accessible to the partner.  The user can also explicitly request to synchronize their data with the financial source. This can be useful if the provider credentials are not stored at figo or if the user wants to make updated data available immediately. To achieve this the partner has to:  1. Create an appropriate [ShieldToken](#operation/createSyncShieldToken). 2. Forward the user to the `location` provided in the ShieldToken response. The user will be guided in    the RegShield UI through the synchronization. 3. The RegShield UI redirects the user to the `redirect_uri` provided in the ShieldToken with the following    query-parameters    * `success`: A boolean indicator if the synchronization was succesful.    * `state`: The state that was provided when creating the ShieldToken.     Example:      ```bash      https://example.com/callback?success=true&state=7a28967d      ```  <div class=\"diagram\">   sequenceDiagram     participant U as User Agent     participant P as Partner     participant API as RegShield API     participant UI as RegShield UI     opt Manual sync       U->>+P: \\n       activate P       activate U       P->>+API: 1. Create ShieldToken       API- ->>-P: ShieldToken       P - ->> U: Present RegShield UI location       deactivate P       U ->> UI: 2. Open location       activate UI       UI - ->> U: 3. Redirect to redirect_uri       deactivate UI       U ->> P: state & success indicator       deactivate U       activate P       deactivate P     end  </div>  ## Initiating a payment  To initiate a payment you must have purchased [figo initiator](https://www.figo.io/en/figo-initiator/).  The following listing enumerates all required steps to initate a payment with RegShield:  1. Create a [ShieldToken](#operation/createPaymentShieldToken) for authentication, declaration of    intent, flow configuration and payment information. 2. Forward the user to the `location` provided in the ShieldToken response. This can be achieved by    using     * an Overlay/PopUp frame (please ask your personal figo contact for details)     * a redirect in the same or a new window 3. The RegShield UI will guide the user through the process of executing the payment. 4. The RegShield UI redirects the user to the `redirect_uri` provided in the ShieldToken. 5. The partner [verifies the payment](#operation/getPaymentReceipt) with the ShieldToken ID.  <div class=\"diagram\"> sequenceDiagram   participant U as User Agent   participant P as Partner   participant API as RegShield API   participant UI as RegShield UI   U->>P:\\n   activate U   activate P   P->>+API: 1. Create ShieldToken   Note over P, API: A state & redirect_uri is chosen<br/>by the Partner   Note over P, API: account_id & tan_scheme_id chosen<br/>by the user are mandatory   API- ->>-P: ShieldToken   P- ->>U: Present RegShield UI location   deactivate P   U->>UI: 2. Open location   activate UI   Note left of UI: 3. User processes<br/>with the payment   UI- ->>U: 4. Redirect to redirect_uri   deactivate UI   U->>P: state & success   deactivate U   P->>+API: 5. Verify payment by ID   API- ->>-P: Payment information; </div>  ## iframe Integration  iframe integration works the same way as in a url redirect. The only difference in the flow is that instead of a url redirect callback, a message will be posted to the iframe container.  ```html   <iframe src=\"{ShieldToken.location}\"></iframe> ```  ```js   window.addEventListener('message', receiveMessage, false);    /_**    * postMessage listner    *    * @param  {event}  event    * @param  {Object} event.data            payload of the `postMessage` event    * @param  {String} event.data.location   callback url AS-IS. `https://example.com/callback?state=state&code=code`    * @param  {Object} event.data.query      query parameters. `{ state: \"state\", code: \"code\", success: \"true\" }`    * @param  {String} event.data.url        callback url without the query parameters. `https://example.com/callback`    *    *_/   function receiveMessage(event) {     ...   } ```  For development purposes the following domains are whitelisted for iframe integration: - `http://127.0.0.1` - `https://127.0.0.1` - `http://localhost` - `https://localhost`  All ports are allowed on the whitelisted domains.  **Note**\\ RegShield UI does **not** listen to any post messages from any source even if the receiver of the message uses `event.source.postMessage` to post back a message.  iframe integration and url callback are **mutually exclusive**. On iframe integration a, _callback_ will be made via `postMessage` API. On redirect integration, a specified URL callback endpoint will be called.  Details on `postMessage` can be found on [MDN](https://developer.mozilla.org/en-US/docs/Web/API/Window/postMessage).  ## Error handling  In the case where an error has occurred, `redirect_uri` provided in ShieldToken will be called with a query parameter `success=false`.  RegShield UI users can cancel the process at any time and return to partner's application. If a user has canceled a process, it is considered an erroneous state as no authorization code will be included in the callback.  Successful flow callback structure:\\ `<ShieldToken.redirect_uri>?state=<state>&code=<code>&success=true`  Successful flow [iframe post message](#section/Introduction/iframe-Integration) structure: ```js {   location: \"<ShieldToken.redirect_uri>?state=<state>&code=<code>&success=true\",   query: {     code: \"<code>\",     state: \"<state>\",     success: \"true\"   },   url: \"<ShieldToken.redirect_uri>\" } ```  Erroneous flow callback structure:\\ `<ShieldToken.redirect_uri>?state=<state>&success=false`  Erroneous flow [iframe post message](#section/Introduction/iframe-Integration) structure: ```js {   location: \"<ShieldToken.redirect_uri>?state=<state>&success=false\",   query: {     state: \"<state>\",     success: \"false\"   },   url: \"<ShieldToken.redirect_uri>\" } ```   ## Authentication  The RegShield API uses two forms of authentication depending on the requested resources    - Basic Auth   - OAuth2 Bearer Token  <!- - ReDoc-Inject: <security-definitions> - -> 
 *
 * The version of the OpenAPI document: 2.0.0
 * Contact: support@figo.io
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using RestSharpMethod = RestSharp.Method;

namespace Figo.Client.Core.Client
{
    /// <summary>
    ///     Allows RestSharp to Serialize/Deserialize JSON using our custom logic, but only when ContentType is JSON.
    /// </summary>
    internal class CustomJsonCodec : ISerializer, IDeserializer
    {
        private readonly IReadableConfiguration _configuration;

        private readonly string _contentType = "application/json";
        private readonly JsonSerializer _serializer;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            // OpenAPI generated types generally hide default constructors.
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    OverrideSpecifiedNames = true
                }
            }
        };

        public CustomJsonCodec(IReadableConfiguration configuration)
        {
            this._configuration = configuration;
            this._serializer = JsonSerializer.Create(this._serializerSettings);
        }

        public CustomJsonCodec(JsonSerializerSettings serializerSettings, IReadableConfiguration configuration)
        {
            this._serializerSettings = serializerSettings;
            this._serializer = JsonSerializer.Create(this._serializerSettings);
            this._configuration = configuration;
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public T Deserialize<T>(IRestResponse response)
        {
            var result = (T) this.Deserialize(response, typeof(T));
            return result;
        }

        public string Serialize(object obj)
        {
            var result = JsonConvert.SerializeObject(obj, this._serializerSettings);
            return result;
        }

        public string ContentType
        {
            get => this._contentType;
            set => throw new InvalidOperationException("Not allowed to set content type.");
        }

        /// <summary>
        ///     Deserialize the JSON string into a proper object.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="type">Object type.</param>
        /// <returns>Object representation of the JSON string.</returns>
        internal object Deserialize(IRestResponse response, Type type)
        {
            var headers = response.Headers;
            if (type == typeof(byte[])) // return byte array
            {
                return response.RawBytes;
            }

            // TODO: ? if (type.IsAssignableFrom(typeof(Stream)))
            if (type == typeof(Stream))
            {
                if (headers != null)
                {
                    var filePath = string.IsNullOrEmpty(this._configuration.TempFolderPath)
                        ? Path.GetTempPath()
                        : this._configuration.TempFolderPath;
                    var regex = new Regex(@"Content-Disposition=.*filename=['""]?([^'""\s]+)['""]?$");
                    foreach (var header in headers)
                    {
                        var match = regex.Match(header.ToString());
                        if (match.Success)
                        {
                            var fileName = filePath + ClientUtils.SanitizeFilename(match.Groups[1].Value.Replace("\"", "").Replace("'", ""));
                            File.WriteAllBytes(fileName, response.RawBytes);
                            return new FileStream(fileName, FileMode.Open);
                        }
                    }
                }

                var stream = new MemoryStream(response.RawBytes);
                return stream;
            }

            if (type.Name.StartsWith("System.Nullable`1[[System.DateTime")) // return a datetime object
            {
                return DateTime.Parse(response.Content, null, DateTimeStyles.RoundtripKind);
            }

            if (type == typeof(string) || type.Name.StartsWith("System.Nullable")) // return primitive type
            {
                return ClientUtils.ConvertType(response.Content, type);
            }

            // at this point, it must be a model (json)
            try
            {
                return JsonConvert.DeserializeObject(response.Content, type, this._serializerSettings);
            }
            catch (Exception e)
            {
                throw new ApiException(500, e.Message);
            }
        }
    }

    /// <summary>
    ///     Provides a default implementation of an Api client (both synchronous and asynchronous implementatios),
    ///     encapsulating general REST accessor use cases.
    /// </summary>
    public class ApiClient : ISynchronousClient, IAsynchronousClient
    {
        private readonly ILogger _logger;
        private readonly string _baseUrl;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiClient" />, defaulting to the global configurations' base url.
        /// </summary>
        public ApiClient()
        {
            this._baseUrl = GlobalConfiguration.Instance.BasePath;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiClient" />
        /// </summary>
        /// <param name="basePath">The target service's base path in URL format.</param>
        /// <exception cref="ArgumentException"></exception>
        public ApiClient(string basePath)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentException("basePath cannot be empty");
            }

            this._baseUrl = basePath;
        }
        
        public ApiClient(string basePath, ILogger logger)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentException("basePath cannot be empty");
            }

            this._baseUrl = basePath;
            this._logger = logger;
        }

        /// <summary>
        ///     Allows for extending request processing for <see cref="ApiClient" /> generated code.
        /// </summary>
        /// <param name="request">The RestSharp request object</param>
        private void InterceptRequest(IRestRequest request)
        {
        }

        /// <summary>
        ///     Allows for extending response processing for <see cref="ApiClient" /> generated code.
        /// </summary>
        /// <param name="request">The RestSharp request object</param>
        /// <param name="response">The RestSharp response object</param>
        private void InterceptResponse(IRestRequest request, IRestResponse response)
        {
        }

        /// <summary>
        ///     Constructs the RestSharp version of an http method
        /// </summary>
        /// <param name="method">Swagger Client Custom HttpMethod</param>
        /// <returns>RestSharp's HttpMethod instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private RestSharpMethod Method(HttpMethod method)
        {
            RestSharpMethod other;
            switch (method)
            {
                case HttpMethod.Get:
                    other = RestSharpMethod.GET;
                    break;
                case HttpMethod.Post:
                    other = RestSharpMethod.POST;
                    break;
                case HttpMethod.Put:
                    other = RestSharpMethod.PUT;
                    break;
                case HttpMethod.Delete:
                    other = RestSharpMethod.DELETE;
                    break;
                case HttpMethod.Head:
                    other = RestSharpMethod.HEAD;
                    break;
                case HttpMethod.Options:
                    other = RestSharpMethod.OPTIONS;
                    break;
                case HttpMethod.Patch:
                    other = RestSharpMethod.PATCH;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }

            return other;
        }

        /// <summary>
        ///     Provides all logic for constructing a new RestSharp <see cref="RestRequest" />.
        ///     At this point, all information for querying the service is known. Here, it is simply
        ///     mapped into the RestSharp request.
        /// </summary>
        /// <param name="method">The http verb.</param>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>[private] A new RestRequest instance.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private RestRequest newRequest(
            HttpMethod method,
            string path,
            RequestOptions options,
            IReadableConfiguration configuration)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var request = new RestRequest(this.Method(method))
            {
                Resource = path,
                JsonSerializer = new CustomJsonCodec(configuration)
            };

            if (options.PathParameters != null)
            {
                foreach (var pathParam in options.PathParameters)
                {
                    request.AddParameter(pathParam.Key, pathParam.Value, ParameterType.UrlSegment);
                }
            }

            if (options.QueryParameters != null)
            {
                foreach (var queryParam in options.QueryParameters)
                {
                    foreach (var value in queryParam.Value)
                    {
                        request.AddQueryParameter(queryParam.Key, value);
                    }
                }
            }

            if (configuration.DefaultHeaders != null)
            {
                foreach (var headerParam in configuration.DefaultHeaders)
                {
                    request.AddHeader(headerParam.Key, headerParam.Value);
                }
            }

            if (options.HeaderParameters != null)
            {
                foreach (var headerParam in options.HeaderParameters)
                {
                    foreach (var value in headerParam.Value)
                    {
                        request.AddHeader(headerParam.Key, value);
                    }
                }
            }

            if (options.FormParameters != null)
            {
                foreach (var formParam in options.FormParameters)
                {
                    request.AddParameter(formParam.Key, formParam.Value);
                }
            }

            if (options.Data != null)
            {
                if (options.HeaderParameters != null)
                {
                    var contentTypes = options.HeaderParameters["Content-Type"];
                    if (contentTypes == null || contentTypes.Any(header => header.Contains("application/json")))
                    {
                        request.RequestFormat = DataFormat.Json;
                    }
                }
                else
                {
                    // Here, we'll assume JSON APIs are more common. XML can be forced by adding produces/consumes to openapi spec explicitly.
                    request.RequestFormat = DataFormat.Json;
                }

                request.AddJsonBody(options.Data);
            }

            if (options.FileParameters != null)
            {
                foreach (var fileParam in options.FileParameters)
                {
                    var bytes = ClientUtils.ReadAsBytes(fileParam.Value);
                    if (fileParam.Value is FileStream fileStream)
                    {
                        request.Files.Add(FileParameter.Create(fileParam.Key, bytes, Path.GetFileName(fileStream.Name)));
                    }
                    else
                    {
                        request.Files.Add(FileParameter.Create(fileParam.Key, bytes, "no_file_name_provided"));
                    }
                }
            }

            if (options.Cookies != null && options.Cookies.Count > 0)
            {
                foreach (var cookie in options.Cookies)
                {
                    request.AddCookie(cookie.Name, cookie.Value);
                }
            }

            return request;
        }

        private ApiResponse<T> toApiResponse<T>(IRestResponse<T> response)
        {
            var result = response.Data;
            var transformed = new ApiResponse<T>(response.StatusCode, new Multimap<string, string>(), result)
            {
                ErrorText = response.ErrorMessage,
                Cookies = new List<Cookie>()
            };

            if (!string.IsNullOrEmpty(response.Content) && response.Content.IndexOf("Error", StringComparison.OrdinalIgnoreCase) > -1)
            {
                transformed.ErrorText = response.Content;
            }

            if (response.Headers != null)
            {
                foreach (var responseHeader in response.Headers)
                {
                    transformed.Headers.Add(responseHeader.Name, ClientUtils.ParameterToString(responseHeader.Value));
                }
            }

            if (response.Cookies != null)
            {
                foreach (var responseCookies in response.Cookies)
                {
                    transformed.Cookies.Add(
                        new Cookie(
                            responseCookies.Name,
                            responseCookies.Value,
                            responseCookies.Path,
                            responseCookies.Domain)
                    );
                }
            }

            return transformed;
        }

        private async Task<ApiResponse<T>> Exec<T>(RestRequest req, IReadableConfiguration configuration)
        {
            var client = new InterceptedRestClient(this._baseUrl, this._logger);

            client.ClearHandlers();
            if (req.JsonSerializer is IDeserializer existingDeserializer)
            {
                client.AddHandler(existingDeserializer, "application/json", "text/json", "text/x-json", "text/javascript", "*+json");
            }
            else
            {
                var codec = new CustomJsonCodec(configuration);
                client.AddHandler(codec, "application/json", "text/json", "text/x-json", "text/javascript", "*+json");
            }

            client.AddHandler(new XmlDeserializer(), "application/xml", "text/xml", "*+xml", "*");

            client.Timeout = configuration.Timeout;

            if (configuration.UserAgent != null)
            {
                client.UserAgent = configuration.UserAgent;
            }

            this.InterceptRequest(req);
            var response = await client.ExecuteTaskAsync<T>(req).ConfigureAwait(false);
            this.InterceptResponse(req, response);

            var result = this.toApiResponse(response);
            if (response.ErrorMessage != null)
            {
                result.ErrorText = response.ErrorMessage;
            }

            if (response.Cookies != null && response.Cookies.Count > 0)
            {
                if (result.Cookies == null)
                {
                    result.Cookies = new List<Cookie>();
                }

                foreach (var restResponseCookie in response.Cookies)
                {
                    var cookie = new Cookie(
                        restResponseCookie.Name,
                        restResponseCookie.Value,
                        restResponseCookie.Path,
                        restResponseCookie.Domain
                    )
                    {
                        Comment = restResponseCookie.Comment,
                        CommentUri = restResponseCookie.CommentUri,
                        Discard = restResponseCookie.Discard,
                        Expired = restResponseCookie.Expired,
                        Expires = restResponseCookie.Expires,
                        HttpOnly = restResponseCookie.HttpOnly,
                        Port = restResponseCookie.Port,
                        Secure = restResponseCookie.Secure,
                        Version = restResponseCookie.Version
                    };

                    result.Cookies.Add(cookie);
                }
            }

            return result;
        }

        #region IAsynchronousClient

        /// <summary>
        ///     Make a HTTP GET request (async).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public async Task<ApiResponse<T>> GetAsync<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            var config = configuration ?? GlobalConfiguration.Instance;
            return await this.Exec<T>(this.newRequest(HttpMethod.Get, path, options, config), config).ConfigureAwait(false);
        }

        /// <summary>
        ///     Make a HTTP POST request (async).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public async Task<ApiResponse<T>> PostAsync<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            var config = configuration ?? GlobalConfiguration.Instance;
            return await this.Exec<T>(this.newRequest(HttpMethod.Post, path, options, config), config).ConfigureAwait(false);
        }

        /// <summary>
        ///     Make a HTTP PUT request (async).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public async Task<ApiResponse<T>> PutAsync<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            var config = configuration ?? GlobalConfiguration.Instance;
            return await this.Exec<T>(this.newRequest(HttpMethod.Put, path, options, config), config).ConfigureAwait(false);
        }

        /// <summary>
        ///     Make a HTTP DELETE request (async).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public async Task<ApiResponse<T>> DeleteAsync<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            var config = configuration ?? GlobalConfiguration.Instance;
            return await this.Exec<T>(this.newRequest(HttpMethod.Delete, path, options, config), config).ConfigureAwait(false);
        }

        /// <summary>
        ///     Make a HTTP HEAD request (async).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public async Task<ApiResponse<T>> HeadAsync<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            var config = configuration ?? GlobalConfiguration.Instance;
            return await this.Exec<T>(this.newRequest(HttpMethod.Head, path, options, config), config).ConfigureAwait(false);
        }

        /// <summary>
        ///     Make a HTTP OPTION request (async).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public async Task<ApiResponse<T>> OptionsAsync<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            var config = configuration ?? GlobalConfiguration.Instance;
            return await this.Exec<T>(this.newRequest(HttpMethod.Options, path, options, config), config).ConfigureAwait(false);
        }

        /// <summary>
        ///     Make a HTTP PATCH request (async).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public async Task<ApiResponse<T>> PatchAsync<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            var config = configuration ?? GlobalConfiguration.Instance;
            return await this.Exec<T>(this.newRequest(HttpMethod.Patch, path, options, config), config).ConfigureAwait(false);
        }

        #endregion IAsynchronousClient

        #region ISynchronousClient

        /// <summary>
        ///     Make a HTTP GET request (synchronous).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public ApiResponse<T> Get<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            return this.GetAsync<T>(path, options, configuration).Result;
        }

        /// <summary>
        ///     Make a HTTP POST request (synchronous).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public ApiResponse<T> Post<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            return this.PostAsync<T>(path, options, configuration).Result;
        }

        /// <summary>
        ///     Make a HTTP PUT request (synchronous).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public ApiResponse<T> Put<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            return this.PutAsync<T>(path, options, configuration).Result;
        }

        /// <summary>
        ///     Make a HTTP DELETE request (synchronous).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public ApiResponse<T> Delete<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            return this.DeleteAsync<T>(path, options, configuration).Result;
        }

        /// <summary>
        ///     Make a HTTP HEAD request (synchronous).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public ApiResponse<T> Head<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            return this.HeadAsync<T>(path, options, configuration).Result;
        }

        /// <summary>
        ///     Make a HTTP OPTION request (synchronous).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public ApiResponse<T> Options<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            return this.OptionsAsync<T>(path, options, configuration).Result;
        }

        /// <summary>
        ///     Make a HTTP PATCH request (synchronous).
        /// </summary>
        /// <param name="path">The target path (or resource).</param>
        /// <param name="options">The additional request options.</param>
        /// <param name="configuration">
        ///     A per-request configuration object. It is assumed that any merge with
        ///     GlobalConfiguration has been done before calling this method.
        /// </param>
        /// <returns>A Task containing ApiResponse</returns>
        public ApiResponse<T> Patch<T>(string path, RequestOptions options, IReadableConfiguration configuration = null)
        {
            return this.PatchAsync<T>(path, options, configuration).Result;
        }

        #endregion ISynchronousClient
    }
}