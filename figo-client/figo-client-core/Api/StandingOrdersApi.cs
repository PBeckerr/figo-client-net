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
using System.Threading.Tasks;
using Figo.Client.Core.Client;
using Figo.Client.Core.Model;
using Microsoft.Extensions.Logging;

namespace Figo.Client.Core.Api
{
    /// <summary>
    ///     Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IStandingOrdersApiSync : IApiAccessor
    {
        #region Synchronous Operations

        /// <summary>
        ///     Get standing order
        /// </summary>
        /// <remarks>
        ///     Retrieve a single standing order by its ID.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="standingOrderId"></param>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>StandingOrder</returns>
        StandingOrder GetStandingOrder(string standingOrderId, List<string> accounts = null, bool? cents = null);

        /// <summary>
        ///     Get standing order
        /// </summary>
        /// <remarks>
        ///     Retrieve a single standing order by its ID.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="standingOrderId"></param>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>ApiResponse of StandingOrder</returns>
        ApiResponse<StandingOrder> GetStandingOrderWithHttpInfo(string standingOrderId, List<string> accounts = null, bool? cents = null);

        /// <summary>
        ///     Get standing order of account
        /// </summary>
        /// <remarks>
        ///     Retrieve a single standing order associated to a specific account by its ID.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>StandingOrder</returns>
        StandingOrder GetStandingOrderOfAccount(string accountId, string standingOrderId);

        /// <summary>
        ///     Get standing order of account
        /// </summary>
        /// <remarks>
        ///     Retrieve a single standing order associated to a specific account by its ID.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>ApiResponse of StandingOrder</returns>
        ApiResponse<StandingOrder> GetStandingOrderOfAccountWithHttpInfo(string accountId, string standingOrderId);

        /// <summary>
        ///     List standing orders
        /// </summary>
        /// <remarks>
        ///     Get a list of the standing orders of all accounts. You can additionally constrain the amount of standing orders
        ///     being returned by using the query parameters described as filters.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>StandingOrdersResponse</returns>
        StandingOrdersResponse ListStandingOrders(List<string> accounts = null, bool? cents = null);

        /// <summary>
        ///     List standing orders
        /// </summary>
        /// <remarks>
        ///     Get a list of the standing orders of all accounts. You can additionally constrain the amount of standing orders
        ///     being returned by using the query parameters described as filters.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>ApiResponse of StandingOrdersResponse</returns>
        ApiResponse<StandingOrdersResponse> ListStandingOrdersWithHttpInfo(List<string> accounts = null, bool? cents = null);

        /// <summary>
        ///     List standing orders of account
        /// </summary>
        /// <remarks>
        ///     Get a list of the standing orders of a specific account. You can additionally constrain the amount of standing
        ///     orders being returned by using the query parameters described below as filters.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>StandingOrdersResponse</returns>
        StandingOrdersResponse ListStandingOrdersOfAccount(string accountId, bool? cents = null);

        /// <summary>
        ///     List standing orders of account
        /// </summary>
        /// <remarks>
        ///     Get a list of the standing orders of a specific account. You can additionally constrain the amount of standing
        ///     orders being returned by using the query parameters described below as filters.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>ApiResponse of StandingOrdersResponse</returns>
        ApiResponse<StandingOrdersResponse> ListStandingOrdersOfAccountWithHttpInfo(string accountId, bool? cents = null);

        #endregion Synchronous Operations
    }

    /// <summary>
    ///     Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IStandingOrdersApiAsync : IApiAccessor
    {
        #region Asynchronous Operations

        /// <summary>
        ///     Get standing order
        /// </summary>
        /// <remarks>
        ///     Retrieve a single standing order by its ID.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="standingOrderId"></param>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of StandingOrder</returns>
        Task<StandingOrder> GetStandingOrderAsync(string standingOrderId, List<string> accounts = null, bool? cents = null);

        /// <summary>
        ///     Get standing order
        /// </summary>
        /// <remarks>
        ///     Retrieve a single standing order by its ID.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="standingOrderId"></param>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of ApiResponse (StandingOrder)</returns>
        Task<ApiResponse<StandingOrder>> GetStandingOrderAsyncWithHttpInfo(string standingOrderId, List<string> accounts = null, bool? cents = null);

        /// <summary>
        ///     Get standing order of account
        /// </summary>
        /// <remarks>
        ///     Retrieve a single standing order associated to a specific account by its ID.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>Task of StandingOrder</returns>
        Task<StandingOrder> GetStandingOrderOfAccountAsync(string accountId, string standingOrderId);

        /// <summary>
        ///     Get standing order of account
        /// </summary>
        /// <remarks>
        ///     Retrieve a single standing order associated to a specific account by its ID.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>Task of ApiResponse (StandingOrder)</returns>
        Task<ApiResponse<StandingOrder>> GetStandingOrderOfAccountAsyncWithHttpInfo(string accountId, string standingOrderId);

        /// <summary>
        ///     List standing orders
        /// </summary>
        /// <remarks>
        ///     Get a list of the standing orders of all accounts. You can additionally constrain the amount of standing orders
        ///     being returned by using the query parameters described as filters.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of StandingOrdersResponse</returns>
        Task<StandingOrdersResponse> ListStandingOrdersAsync(List<string> accounts = null, bool? cents = null);

        /// <summary>
        ///     List standing orders
        /// </summary>
        /// <remarks>
        ///     Get a list of the standing orders of all accounts. You can additionally constrain the amount of standing orders
        ///     being returned by using the query parameters described as filters.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of ApiResponse (StandingOrdersResponse)</returns>
        Task<ApiResponse<StandingOrdersResponse>> ListStandingOrdersAsyncWithHttpInfo(List<string> accounts = null, bool? cents = null);

        /// <summary>
        ///     List standing orders of account
        /// </summary>
        /// <remarks>
        ///     Get a list of the standing orders of a specific account. You can additionally constrain the amount of standing
        ///     orders being returned by using the query parameters described below as filters.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of StandingOrdersResponse</returns>
        Task<StandingOrdersResponse> ListStandingOrdersOfAccountAsync(string accountId, bool? cents = null);

        /// <summary>
        ///     List standing orders of account
        /// </summary>
        /// <remarks>
        ///     Get a list of the standing orders of a specific account. You can additionally constrain the amount of standing
        ///     orders being returned by using the query parameters described below as filters.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of ApiResponse (StandingOrdersResponse)</returns>
        Task<ApiResponse<StandingOrdersResponse>> ListStandingOrdersOfAccountAsyncWithHttpInfo(string accountId, bool? cents = null);

        #endregion Asynchronous Operations
    }

    /// <summary>
    ///     Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IStandingOrdersApi : IStandingOrdersApiSync, IStandingOrdersApiAsync
    {
    }

    /// <summary>
    ///     Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class StandingOrdersApi : IStandingOrdersApi
    {
        private ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StandingOrdersApi" /> class.
        /// </summary>
        /// <returns></returns>
        public StandingOrdersApi() : this((string) null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StandingOrdersApi" /> class.
        /// </summary>
        /// <returns></returns>
        public StandingOrdersApi(string basePath, ILogger logger = null)
        {
            this.Configuration = Core.Client.Configuration.MergeConfigurations(
                GlobalConfiguration.Instance,
                new Configuration {BasePath = basePath}
            );
            this.Client = new ApiClient(this.Configuration.BasePath, logger);
            this.AsynchronousClient = new ApiClient(this.Configuration.BasePath, logger);
            this.ExceptionFactory = Core.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StandingOrdersApi" /> class
        ///     using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public StandingOrdersApi(Configuration configuration, ILogger logger = null)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.Configuration = Core.Client.Configuration.MergeConfigurations(
                GlobalConfiguration.Instance,
                configuration
            );
            this.Client = new ApiClient(this.Configuration.BasePath, logger);
            this.AsynchronousClient = new ApiClient(this.Configuration.BasePath, logger);
            this.ExceptionFactory = Core.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StandingOrdersApi" /> class
        ///     using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public StandingOrdersApi(ISynchronousClient client, IAsynchronousClient asyncClient, IReadableConfiguration configuration)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            this.AsynchronousClient = asyncClient ?? throw new ArgumentNullException(nameof(asyncClient));
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.ExceptionFactory = Core.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        ///     The client for accessing this underlying API asynchronously.
        /// </summary>
        public IAsynchronousClient AsynchronousClient { get; set; }

        /// <summary>
        ///     The client for accessing this underlying API synchronously.
        /// </summary>
        public ISynchronousClient Client { get; set; }

        /// <summary>
        ///     Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public string GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        ///     Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public IReadableConfiguration Configuration { get; set; }

        /// <summary>
        ///     Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public ExceptionFactory ExceptionFactory
        {
            get
            {
                if (this._exceptionFactory != null && this._exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }

                return this._exceptionFactory;
            }
            set => this._exceptionFactory = value;
        }

        /// <summary>
        ///     Get standing order Retrieve a single standing order by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="standingOrderId"></param>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>StandingOrder</returns>
        public StandingOrder GetStandingOrder(string standingOrderId, List<string> accounts = null, bool? cents = null)
        {
            var localVarResponse = this.GetStandingOrderWithHttpInfo(standingOrderId, accounts, cents);
            return localVarResponse.Data;
        }

        /// <summary>
        ///     Get standing order Retrieve a single standing order by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="standingOrderId"></param>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>ApiResponse of StandingOrder</returns>
        public ApiResponse<StandingOrder> GetStandingOrderWithHttpInfo(string standingOrderId, List<string> accounts = null, bool? cents = null)
        {
            // verify the required parameter 'standingOrderId' is set
            if (standingOrderId == null)
            {
                throw new ApiException(400, "Missing required parameter 'standingOrderId' when calling StandingOrdersApi->GetStandingOrder");
            }

            var requestOptions = new RequestOptions();

            string[] contentTypes =
            {
            };

            // to determine the Accept header
            string[] accepts =
            {
                "application/json"
            };

            var localVarContentType = ClientUtils.SelectHeaderContentType(contentTypes);
            if (localVarContentType != null)
            {
                requestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = ClientUtils.SelectHeaderAccept(accepts);
            if (localVarAccept != null)
            {
                requestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (standingOrderId != null)
            {
                requestOptions.PathParameters.Add("standing-order-id", ClientUtils.ParameterToString(standingOrderId)); // path parameter
            }

            if (accounts != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("multi", "accounts", accounts))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            if (cents != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("", "cents", cents))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            // authentication (user_auth) required
            // oauth required
            if (!string.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                requestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var response = this.Client.Get<StandingOrder>("/v4/rest/standing_orders/{standing-order-id}", requestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                var exception = this.ExceptionFactory("GetStandingOrder", response);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return response;
        }

        /// <summary>
        ///     Get standing order Retrieve a single standing order by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="standingOrderId"></param>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of StandingOrder</returns>
        public async Task<StandingOrder> GetStandingOrderAsync(string standingOrderId, List<string> accounts = null, bool? cents = null)
        {
            var localVarResponse = await this.GetStandingOrderAsyncWithHttpInfo(standingOrderId, accounts, cents).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        ///     Get standing order Retrieve a single standing order by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="standingOrderId"></param>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of ApiResponse (StandingOrder)</returns>
        public async Task<ApiResponse<StandingOrder>> GetStandingOrderAsyncWithHttpInfo(string standingOrderId, List<string> accounts = null,
                                                                                        bool? cents = null)
        {
            // verify the required parameter 'standingOrderId' is set
            if (standingOrderId == null)
            {
                throw new ApiException(400, "Missing required parameter 'standingOrderId' when calling StandingOrdersApi->GetStandingOrder");
            }


            var requestOptions = new RequestOptions();

            string[] contentTypes =
            {
            };

            // to determine the Accept header
            string[] accepts =
            {
                "application/json"
            };

            foreach (var contentType in contentTypes)
            {
                requestOptions.HeaderParameters.Add("Content-Type", contentType);
            }

            foreach (var accept in accepts)
            {
                requestOptions.HeaderParameters.Add("Accept", accept);
            }

            if (standingOrderId != null)
            {
                requestOptions.PathParameters.Add("standing-order-id", ClientUtils.ParameterToString(standingOrderId)); // path parameter
            }

            if (accounts != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("multi", "accounts", accounts))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            if (cents != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("", "cents", cents))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            // authentication (user_auth) required
            // oauth required
            if (!string.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                requestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var response = await this.AsynchronousClient.GetAsync<StandingOrder>("/v4/rest/standing_orders/{standing-order-id}",
                                                                                 requestOptions,
                                                                                 this.Configuration).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                var exception = this.ExceptionFactory("GetStandingOrder", response);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return response;
        }

        /// <summary>
        ///     Get standing order of account Retrieve a single standing order associated to a specific account by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>StandingOrder</returns>
        public StandingOrder GetStandingOrderOfAccount(string accountId, string standingOrderId)
        {
            var localVarResponse = this.GetStandingOrderOfAccountWithHttpInfo(accountId, standingOrderId);
            return localVarResponse.Data;
        }

        /// <summary>
        ///     Get standing order of account Retrieve a single standing order associated to a specific account by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>ApiResponse of StandingOrder</returns>
        public ApiResponse<StandingOrder> GetStandingOrderOfAccountWithHttpInfo(string accountId, string standingOrderId)
        {
            // verify the required parameter 'accountId' is set
            if (accountId == null)
            {
                throw new ApiException(400, "Missing required parameter 'accountId' when calling StandingOrdersApi->GetStandingOrderOfAccount");
            }

            // verify the required parameter 'standingOrderId' is set
            if (standingOrderId == null)
            {
                throw new ApiException(400, "Missing required parameter 'standingOrderId' when calling StandingOrdersApi->GetStandingOrderOfAccount");
            }

            var requestOptions = new RequestOptions();

            string[] contentTypes =
            {
            };

            // to determine the Accept header
            string[] accepts =
            {
                "application/json"
            };

            var localVarContentType = ClientUtils.SelectHeaderContentType(contentTypes);
            if (localVarContentType != null)
            {
                requestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = ClientUtils.SelectHeaderAccept(accepts);
            if (localVarAccept != null)
            {
                requestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (accountId != null)
            {
                requestOptions.PathParameters.Add("account-id", ClientUtils.ParameterToString(accountId)); // path parameter
            }

            if (standingOrderId != null)
            {
                requestOptions.PathParameters.Add("standing-order-id", ClientUtils.ParameterToString(standingOrderId)); // path parameter
            }

            // authentication (user_auth) required
            // oauth required
            if (!string.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                requestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var response = this.Client.Get<StandingOrder>("/v4/rest/accounts/{account-id}/standing_orders/{standing-order-id}",
                                                          requestOptions,
                                                          this.Configuration);

            if (this.ExceptionFactory != null)
            {
                var exception = this.ExceptionFactory("GetStandingOrderOfAccount", response);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return response;
        }

        /// <summary>
        ///     Get standing order of account Retrieve a single standing order associated to a specific account by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>Task of StandingOrder</returns>
        public async Task<StandingOrder> GetStandingOrderOfAccountAsync(string accountId, string standingOrderId)
        {
            var localVarResponse = await this.GetStandingOrderOfAccountAsyncWithHttpInfo(accountId, standingOrderId).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        ///     Get standing order of account Retrieve a single standing order associated to a specific account by its ID.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="standingOrderId"></param>
        /// <returns>Task of ApiResponse (StandingOrder)</returns>
        public async Task<ApiResponse<StandingOrder>> GetStandingOrderOfAccountAsyncWithHttpInfo(string accountId, string standingOrderId)
        {
            // verify the required parameter 'accountId' is set
            if (accountId == null)
            {
                throw new ApiException(400, "Missing required parameter 'accountId' when calling StandingOrdersApi->GetStandingOrderOfAccount");
            }

            // verify the required parameter 'standingOrderId' is set
            if (standingOrderId == null)
            {
                throw new ApiException(400, "Missing required parameter 'standingOrderId' when calling StandingOrdersApi->GetStandingOrderOfAccount");
            }


            var requestOptions = new RequestOptions();

            string[] contentTypes =
            {
            };

            // to determine the Accept header
            string[] accepts =
            {
                "application/json"
            };

            foreach (var contentType in contentTypes)
            {
                requestOptions.HeaderParameters.Add("Content-Type", contentType);
            }

            foreach (var accept in accepts)
            {
                requestOptions.HeaderParameters.Add("Accept", accept);
            }

            if (accountId != null)
            {
                requestOptions.PathParameters.Add("account-id", ClientUtils.ParameterToString(accountId)); // path parameter
            }

            if (standingOrderId != null)
            {
                requestOptions.PathParameters.Add("standing-order-id", ClientUtils.ParameterToString(standingOrderId)); // path parameter
            }

            // authentication (user_auth) required
            // oauth required
            if (!string.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                requestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var response = await this.AsynchronousClient.GetAsync<StandingOrder>("/v4/rest/accounts/{account-id}/standing_orders/{standing-order-id}",
                                                                                 requestOptions,
                                                                                 this.Configuration).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                var exception = this.ExceptionFactory("GetStandingOrderOfAccount", response);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return response;
        }

        /// <summary>
        ///     List standing orders Get a list of the standing orders of all accounts. You can additionally constrain the amount
        ///     of standing orders being returned by using the query parameters described as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>StandingOrdersResponse</returns>
        public StandingOrdersResponse ListStandingOrders(List<string> accounts = null, bool? cents = null)
        {
            var localVarResponse = this.ListStandingOrdersWithHttpInfo(accounts, cents);
            return localVarResponse.Data;
        }

        /// <summary>
        ///     List standing orders Get a list of the standing orders of all accounts. You can additionally constrain the amount
        ///     of standing orders being returned by using the query parameters described as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>ApiResponse of StandingOrdersResponse</returns>
        public ApiResponse<StandingOrdersResponse> ListStandingOrdersWithHttpInfo(List<string> accounts = null, bool? cents = null)
        {
            var requestOptions = new RequestOptions();

            string[] contentTypes =
            {
            };

            // to determine the Accept header
            string[] accepts =
            {
                "application/json"
            };

            var localVarContentType = ClientUtils.SelectHeaderContentType(contentTypes);
            if (localVarContentType != null)
            {
                requestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = ClientUtils.SelectHeaderAccept(accepts);
            if (localVarAccept != null)
            {
                requestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (accounts != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("multi", "accounts", accounts))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            if (cents != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("", "cents", cents))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            // authentication (user_auth) required
            // oauth required
            if (!string.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                requestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var response = this.Client.Get<StandingOrdersResponse>("/v4/rest/standing_orders", requestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                var exception = this.ExceptionFactory("ListStandingOrders", response);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return response;
        }

        /// <summary>
        ///     List standing orders Get a list of the standing orders of all accounts. You can additionally constrain the amount
        ///     of standing orders being returned by using the query parameters described as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of StandingOrdersResponse</returns>
        public async Task<StandingOrdersResponse> ListStandingOrdersAsync(List<string> accounts = null, bool? cents = null)
        {
            var localVarResponse = await this.ListStandingOrdersAsyncWithHttpInfo(accounts, cents).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        ///     List standing orders Get a list of the standing orders of all accounts. You can additionally constrain the amount
        ///     of standing orders being returned by using the query parameters described as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accounts">Comma separated list of account IDs. (optional)</param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of ApiResponse (StandingOrdersResponse)</returns>
        public async Task<ApiResponse<StandingOrdersResponse>> ListStandingOrdersAsyncWithHttpInfo(List<string> accounts = null, bool? cents = null)
        {
            var requestOptions = new RequestOptions();

            string[] contentTypes =
            {
            };

            // to determine the Accept header
            string[] accepts =
            {
                "application/json"
            };

            foreach (var contentType in contentTypes)
            {
                requestOptions.HeaderParameters.Add("Content-Type", contentType);
            }

            foreach (var accept in accepts)
            {
                requestOptions.HeaderParameters.Add("Accept", accept);
            }

            if (accounts != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("multi", "accounts", accounts))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            if (cents != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("", "cents", cents))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            // authentication (user_auth) required
            // oauth required
            if (!string.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                requestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var response = await this.AsynchronousClient.GetAsync<StandingOrdersResponse>("/v4/rest/standing_orders", requestOptions, this.Configuration)
                                     .ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                var exception = this.ExceptionFactory("ListStandingOrders", response);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return response;
        }

        /// <summary>
        ///     List standing orders of account Get a list of the standing orders of a specific account. You can additionally
        ///     constrain the amount of standing orders being returned by using the query parameters described below as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>StandingOrdersResponse</returns>
        public StandingOrdersResponse ListStandingOrdersOfAccount(string accountId, bool? cents = null)
        {
            var localVarResponse = this.ListStandingOrdersOfAccountWithHttpInfo(accountId, cents);
            return localVarResponse.Data;
        }

        /// <summary>
        ///     List standing orders of account Get a list of the standing orders of a specific account. You can additionally
        ///     constrain the amount of standing orders being returned by using the query parameters described below as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>ApiResponse of StandingOrdersResponse</returns>
        public ApiResponse<StandingOrdersResponse> ListStandingOrdersOfAccountWithHttpInfo(string accountId, bool? cents = null)
        {
            // verify the required parameter 'accountId' is set
            if (accountId == null)
            {
                throw new ApiException(400, "Missing required parameter 'accountId' when calling StandingOrdersApi->ListStandingOrdersOfAccount");
            }

            var requestOptions = new RequestOptions();

            string[] contentTypes =
            {
            };

            // to determine the Accept header
            string[] accepts =
            {
                "application/json"
            };

            var localVarContentType = ClientUtils.SelectHeaderContentType(contentTypes);
            if (localVarContentType != null)
            {
                requestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = ClientUtils.SelectHeaderAccept(accepts);
            if (localVarAccept != null)
            {
                requestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (accountId != null)
            {
                requestOptions.PathParameters.Add("account-id", ClientUtils.ParameterToString(accountId)); // path parameter
            }

            if (cents != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("", "cents", cents))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            // authentication (user_auth) required
            // oauth required
            if (!string.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                requestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var response = this.Client.Get<StandingOrdersResponse>("/v4/rest/accounts/{account-id}/standing_orders", requestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                var exception = this.ExceptionFactory("ListStandingOrdersOfAccount", response);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return response;
        }

        /// <summary>
        ///     List standing orders of account Get a list of the standing orders of a specific account. You can additionally
        ///     constrain the amount of standing orders being returned by using the query parameters described below as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of StandingOrdersResponse</returns>
        public async Task<StandingOrdersResponse> ListStandingOrdersOfAccountAsync(string accountId, bool? cents = null)
        {
            var localVarResponse = await this.ListStandingOrdersOfAccountAsyncWithHttpInfo(accountId, cents).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        ///     List standing orders of account Get a list of the standing orders of a specific account. You can additionally
        ///     constrain the amount of standing orders being returned by using the query parameters described below as filters.
        /// </summary>
        /// <exception cref="ApiException">Thrown when fails to make API call</exception>
        /// <param name="accountId"></param>
        /// <param name="cents">If true amounts will be shown in cents. (optional, default to false)</param>
        /// <returns>Task of ApiResponse (StandingOrdersResponse)</returns>
        public async Task<ApiResponse<StandingOrdersResponse>> ListStandingOrdersOfAccountAsyncWithHttpInfo(string accountId, bool? cents = null)
        {
            // verify the required parameter 'accountId' is set
            if (accountId == null)
            {
                throw new ApiException(400, "Missing required parameter 'accountId' when calling StandingOrdersApi->ListStandingOrdersOfAccount");
            }


            var requestOptions = new RequestOptions();

            string[] contentTypes =
            {
            };

            // to determine the Accept header
            string[] accepts =
            {
                "application/json"
            };

            foreach (var contentType in contentTypes)
            {
                requestOptions.HeaderParameters.Add("Content-Type", contentType);
            }

            foreach (var accept in accepts)
            {
                requestOptions.HeaderParameters.Add("Accept", accept);
            }

            if (accountId != null)
            {
                requestOptions.PathParameters.Add("account-id", ClientUtils.ParameterToString(accountId)); // path parameter
            }

            if (cents != null)
            {
                foreach (var kvp in ClientUtils.ParameterToMultiMap("", "cents", cents))
                {
                    foreach (var value in kvp.Value)
                    {
                        requestOptions.QueryParameters.Add(kvp.Key, value);
                    }
                }
            }

            // authentication (user_auth) required
            // oauth required
            if (!string.IsNullOrEmpty(this.Configuration.AccessToken))
            {
                requestOptions.HeaderParameters.Add("Authorization", "Bearer " + this.Configuration.AccessToken);
            }

            // make the HTTP request

            var response = await this.AsynchronousClient.GetAsync<StandingOrdersResponse>("/v4/rest/accounts/{account-id}/standing_orders",
                                                                                          requestOptions,
                                                                                          this.Configuration).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                var exception = this.ExceptionFactory("ListStandingOrdersOfAccount", response);
                if (exception != null)
                {
                    throw exception;
                }
            }

            return response;
        }
    }
}