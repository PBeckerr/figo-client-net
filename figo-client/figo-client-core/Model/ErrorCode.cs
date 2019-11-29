/* 
 * Ongoing Account Information (+AIS)
 *
 * # Introduction  RegShield provides secure access to user financial data in compliance with PSD2.  To implement a Regshield workflow the partner initiates an operation by calling one of the endpoints to create a ShieldToken. This will generate a location which is used to send the user to the RegShield UI, where the user has total control what action will be performed on which of their financial sources. Upon successful completion of the UI workflow the partner gets a callback and can take further actions on their side.  The available operations in the context of the Account Information Service are:  * [Connecting to a financial source and granting access rights](#section/Introduction/Connecting-a-financial-source) * [Synchronizing data from a financial source](#section/Introduction/Synchronization) * [Initiating a payment from a connected financial source](#section/Introduction/Initiating-a-payment)   ## Connecting a financial source  The following listing enumerates all required steps to gain access to the user's financial data:  1. Create a [ShieldToken](#operation/createConnectShieldToken) for authentication, declaration of    intent and flow configuration. 2. Forward the user to the `location` provided in the ShieldToken response. This can be achieved by    using     * an Overlay/PopUp iframe (please ask your personal figo contact for details)     * a redirect in the same or a new window 3. The RegShield UI will guide the user through the process of selecting the financial sources they    want to provide access to. figo will then fetch the financial data associated with the user's    selection. 4. The RegShield UI redirects the user to the `redirect_uri` provided in the ShieldToken with the    following query-parameters    * `state`: The state that was provided when creating the ShieldToken.    * `success`: Indicator whether or not an initiated process was completed successfully.    * `code`: The Authorization Code to be processed by the partner. `code` is only returned on successful flow.     Example:      ```bash      https://example.com/callback?code=Oafd13...&state=a81132cf&success=true      ```    In the case of Overlay/PopUp iframe integration, a message will be posted   to the iframe container. Handling error cases is described in the   [Error Handling](#section/Introduction/Error-handling) section.   5. The partner exchanges the [authorization code for an access token and a refresh    token](#/paths/~1v3~1auth~1token/post). This invalidates the former existing refresh token for    the respective user. 6. The partner accesses the user's financial data by using the access token. 7. The aquired refresh tokens can be used to repeatedly    [create access tokens](#operation/createAccessToken) without the need for new authorization    codes. 8. The partner continues to access the user's financial data by using these access tokens.   <div class=\"diagram\">   sequenceDiagram     participant U as User Agent     participant P as Partner     participant API as RegShield API     participant UI as RegShield UI     U->>P:\\n     activate U     activate P     P->>+API: 1. Create ShieldToken     Note over P, API: state & redirect_uri are chosen<br/>by the Partner     API- ->>-P: ShieldToken     P- ->>U: Present RegShield UI location     deactivate P     U->>UI: 2. Open location     activate UI     Note left of UI: 3. User selects<br/>financial sources     UI- ->>U: 4. Redirect to redirect_uri     deactivate UI     U->>P: state, success & code     deactivate U     activate P     P->>+API: 5. Exchange Authorization Code     API- ->>-P: Access Token + Refresh Token     P->>+API: 6. List accounts     API- ->>-P: Accounts     deactivate P     loop Ongoing access       P->>+API: 7. Exchange Refresh Token       activate P       API- ->>-P: Access Token + Refresh Token       P->>+API: 8. List accounts       API- ->>-P: Accounts       deactivate P     end  </div>  ## Internationalization  RegShield UI supports internationalization. Localization defaults to German `de` when a locale isn't specified or supported.  RegShield UI does not attempt to read user's machine language preference i.e. `Accept-Language` header. You as a partner have to explicitly tell RegShield UI which language to display.  `lang` query parameter adheres to [ISO 639-1 codes](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes). RegShield UI currently supports German `de` and English `en` with more localization spupport on the way.  **Usage**\\ After creating a [ShieldToken](#operation/createShieldToken), append a query parameter `lang` to the generated ShieldToken `location` url.  ```js // Create ShieldToken successful response {   \"id\": \"3ca31c37-986a-454e-ad64-8e97143c86bc\",   \"location\": \"https://login.figo.io/?token=<ShieldToken>\" }  // Append `lang` parameter to \"location\" \"https://login.figo.io/?token=<ShieldToken>&lang=<language-key>\" ```  ## Synchronization  When connecting to a financial source the user can optionally allow figo to store their provider credentials. This allows figo to periodically synchronize the financial data from the provider and make these updates accessible to the partner.  The user can also explicitly request to synchronize their data with the financial source. This can be useful if the provider credentials are not stored at figo or if the user wants to make updated data available immediately. To achieve this the partner has to:  1. Create an appropriate [ShieldToken](#operation/createSyncShieldToken). 2. Forward the user to the `location` provided in the ShieldToken response. The user will be guided in    the RegShield UI through the synchronization. 3. The RegShield UI redirects the user to the `redirect_uri` provided in the ShieldToken with the following    query-parameters    * `success`: A boolean indicator if the synchronization was succesful.    * `state`: The state that was provided when creating the ShieldToken.     Example:      ```bash      https://example.com/callback?success=true&state=7a28967d      ```  <div class=\"diagram\">   sequenceDiagram     participant U as User Agent     participant P as Partner     participant API as RegShield API     participant UI as RegShield UI     opt Manual sync       U->>+P: \\n       activate P       activate U       P->>+API: 1. Create ShieldToken       API- ->>-P: ShieldToken       P - ->> U: Present RegShield UI location       deactivate P       U ->> UI: 2. Open location       activate UI       UI - ->> U: 3. Redirect to redirect_uri       deactivate UI       U ->> P: state & success indicator       deactivate U       activate P       deactivate P     end  </div>  ## Initiating a payment  To initiate a payment you must have purchased [figo initiator](https://www.figo.io/en/figo-initiator/).  The following listing enumerates all required steps to initate a payment with RegShield:  1. Create a [ShieldToken](#operation/createPaymentShieldToken) for authentication, declaration of    intent, flow configuration and payment information. 2. Forward the user to the `location` provided in the ShieldToken response. This can be achieved by    using     * an Overlay/PopUp frame (please ask your personal figo contact for details)     * a redirect in the same or a new window 3. The RegShield UI will guide the user through the process of executing the payment. 4. The RegShield UI redirects the user to the `redirect_uri` provided in the ShieldToken. 5. The partner [verifies the payment](#operation/getPaymentReceipt) with the ShieldToken ID.  <div class=\"diagram\"> sequenceDiagram   participant U as User Agent   participant P as Partner   participant API as RegShield API   participant UI as RegShield UI   U->>P:\\n   activate U   activate P   P->>+API: 1. Create ShieldToken   Note over P, API: A state & redirect_uri is chosen<br/>by the Partner   Note over P, API: account_id & tan_scheme_id chosen<br/>by the user are mandatory   API- ->>-P: ShieldToken   P- ->>U: Present RegShield UI location   deactivate P   U->>UI: 2. Open location   activate UI   Note left of UI: 3. User processes<br/>with the payment   UI- ->>U: 4. Redirect to redirect_uri   deactivate UI   U->>P: state & success   deactivate U   P->>+API: 5. Verify payment by ID   API- ->>-P: Payment information; </div>  ## iframe Integration  iframe integration works the same way as in a url redirect. The only difference in the flow is that instead of a url redirect callback, a message will be posted to the iframe container.  ```html   <iframe src=\"{ShieldToken.location}\"></iframe> ```  ```js   window.addEventListener('message', receiveMessage, false);    /_**    * postMessage listner    *    * @param  {event}  event    * @param  {Object} event.data            payload of the `postMessage` event    * @param  {String} event.data.location   callback url AS-IS. `https://example.com/callback?state=state&code=code`    * @param  {Object} event.data.query      query parameters. `{ state: \"state\", code: \"code\", success: \"true\" }`    * @param  {String} event.data.url        callback url without the query parameters. `https://example.com/callback`    *    *_/   function receiveMessage(event) {     ...   } ```  For development purposes the following domains are whitelisted for iframe integration: - `http://127.0.0.1` - `https://127.0.0.1` - `http://localhost` - `https://localhost`  All ports are allowed on the whitelisted domains.  **Note**\\ RegShield UI does **not** listen to any post messages from any source even if the receiver of the message uses `event.source.postMessage` to post back a message.  iframe integration and url callback are **mutually exclusive**. On iframe integration a, _callback_ will be made via `postMessage` API. On redirect integration, a specified URL callback endpoint will be called.  Details on `postMessage` can be found on [MDN](https://developer.mozilla.org/en-US/docs/Web/API/Window/postMessage).  ## Error handling  In the case where an error has occurred, `redirect_uri` provided in ShieldToken will be called with a query parameter `success=false`.  RegShield UI users can cancel the process at any time and return to partner's application. If a user has canceled a process, it is considered an erroneous state as no authorization code will be included in the callback.  Successful flow callback structure:\\ `<ShieldToken.redirect_uri>?state=<state>&code=<code>&success=true`  Successful flow [iframe post message](#section/Introduction/iframe-Integration) structure: ```js {   location: \"<ShieldToken.redirect_uri>?state=<state>&code=<code>&success=true\",   query: {     code: \"<code>\",     state: \"<state>\",     success: \"true\"   },   url: \"<ShieldToken.redirect_uri>\" } ```  Erroneous flow callback structure:\\ `<ShieldToken.redirect_uri>?state=<state>&success=false`  Erroneous flow [iframe post message](#section/Introduction/iframe-Integration) structure: ```js {   location: \"<ShieldToken.redirect_uri>?state=<state>&success=false\",   query: {     state: \"<state>\",     success: \"false\"   },   url: \"<ShieldToken.redirect_uri>\" } ```   ## Authentication  The RegShield API uses two forms of authentication depending on the requested resources    - Basic Auth   - OAuth2 Bearer Token  <!- - ReDoc-Inject: <security-definitions> - -> 
 *
 * The version of the OpenAPI document: 2.0.0
 * Contact: support@figo.io
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Figo.Client.Core.Model
{
    /// <summary>
    ///     | Code    | Group  | Description                                      | | - -- -- -- | - -- -- - | - -- -- -- -- --
    ///     -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- - | | 1000  | Client | Invalid request
    ///     | | 1001  | Client | Entry already exists                             | | 1002  | Client |
    ///     Entity not found                                 | | 1003  | Client | Unauthorized
    ///     | | 1004  | Client | Invalid client authorization                     | | 1005  | Client |
    ///     Payment already processed                        | | 1006  | Client | Unprocessable entity
    ///     | | 1007  | Client | Forbidden                                        | | 10000 | User   |
    ///     Login credentials are invalid                    | | 10001 | User   | PIN is invalid
    ///     | | 10002 | User   | Online access is blocked                         | | 10003 | User   |
    ///     TAN scheme not activated                         | | 10004 | User   | TAN is invalid
    ///     | | 10005 | User   | No authorization for this account                | | 10006 | User   |
    ///     Transaction rejected                             | | 10007 | User   | PIN change necessary
    ///     | | 10008 | User   | No authorization for this business transaction   | | 10009 | User   |
    ///     HBCI activation necessary                        | | 10010 | User   | Account is blocked
    ///     | | 10011 | User   | Account no longer exists                         | | 10012 | User   |
    ///     TAN scheme is blocked                            | | 10013 | User   | Status of transaction
    ///     inconclusive               | | 10014 | User   | Account not activated for online banking         | |
    ///     10015 | User   | Redundant submissions                            | | 20000 | Bank   |
    ///     Processing at the bank not possible              | | 20001 | Bank   | Bank / account unkown
    ///     | | 20002 | Bank   | Transaction canceled                             | | 20003 | Bank   |
    ///     Maintenance                                      | | 20004 | Bank   | Technical migration
    ///     | | 20005 | Bank   | Transaction not possible                         | | 20006 | Bank   |
    ///     Login not possible                               | | 20007 | Bank   | Pop up
    ///     | | 30000 | figo   | Processing at figo not possible                  | | 30001 | figo   |
    ///     DEPRECATED figo user - invalid language code     | | 30002 | figo   | DEPRECATED figo user - username
    ///     already exists   | | 30003 | figo   | DEPRECATED figo user - username policy violation | | 30004
    ///      | figo   | DEPRECATED figo user - missing parameter         | | 30005 | figo   | Task is expired
    ///     | | 30006 | figo   | Service temporarily not available                | | 40000 | figo   |
    ///     Bank not supported                               |
    /// </summary>
    /// <value>
    ///     | Code    | Group  | Description                                      | | - -- -- -- | - -- -- - | - -- -- -- --
    ///     -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- - | | 1000  | Client | Invalid request
    ///     | | 1001  | Client | Entry already exists                             | | 1002  | Client |
    ///     Entity not found                                 | | 1003  | Client | Unauthorized
    ///     | | 1004  | Client | Invalid client authorization                     | | 1005  | Client |
    ///     Payment already processed                        | | 1006  | Client | Unprocessable entity
    ///     | | 1007  | Client | Forbidden                                        | | 10000 | User   |
    ///     Login credentials are invalid                    | | 10001 | User   | PIN is invalid
    ///     | | 10002 | User   | Online access is blocked                         | | 10003 | User   |
    ///     TAN scheme not activated                         | | 10004 | User   | TAN is invalid
    ///     | | 10005 | User   | No authorization for this account                | | 10006 | User   |
    ///     Transaction rejected                             | | 10007 | User   | PIN change necessary
    ///     | | 10008 | User   | No authorization for this business transaction   | | 10009 | User   |
    ///     HBCI activation necessary                        | | 10010 | User   | Account is blocked
    ///     | | 10011 | User   | Account no longer exists                         | | 10012 | User   |
    ///     TAN scheme is blocked                            | | 10013 | User   | Status of transaction
    ///     inconclusive               | | 10014 | User   | Account not activated for online banking         | |
    ///     10015 | User   | Redundant submissions                            | | 20000 | Bank   |
    ///     Processing at the bank not possible              | | 20001 | Bank   | Bank / account unkown
    ///     | | 20002 | Bank   | Transaction canceled                             | | 20003 | Bank   |
    ///     Maintenance                                      | | 20004 | Bank   | Technical migration
    ///     | | 20005 | Bank   | Transaction not possible                         | | 20006 | Bank   |
    ///     Login not possible                               | | 20007 | Bank   | Pop up
    ///     | | 30000 | figo   | Processing at figo not possible                  | | 30001 | figo   |
    ///     DEPRECATED figo user - invalid language code     | | 30002 | figo   | DEPRECATED figo user - username
    ///     already exists   | | 30003 | figo   | DEPRECATED figo user - username policy violation | | 30004
    ///      | figo   | DEPRECATED figo user - missing parameter         | | 30005 | figo   | Task is expired
    ///     | | 30006 | figo   | Service temporarily not available                | | 40000 | figo   |
    ///     Bank not supported                               |
    /// </value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorCode
    {
        /// <summary>
        ///     Enum NUMBER_1000 for value: 1000
        /// </summary>
        [EnumMember(Value = "1000")] NUMBER_1000 = 1,

        /// <summary>
        ///     Enum NUMBER_1001 for value: 1001
        /// </summary>
        [EnumMember(Value = "1001")] NUMBER_1001 = 2,

        /// <summary>
        ///     Enum NUMBER_1002 for value: 1002
        /// </summary>
        [EnumMember(Value = "1002")] NUMBER_1002 = 3,

        /// <summary>
        ///     Enum NUMBER_1003 for value: 1003
        /// </summary>
        [EnumMember(Value = "1003")] NUMBER_1003 = 4,

        /// <summary>
        ///     Enum NUMBER_1004 for value: 1004
        /// </summary>
        [EnumMember(Value = "1004")] NUMBER_1004 = 5,

        /// <summary>
        ///     Enum NUMBER_1005 for value: 1005
        /// </summary>
        [EnumMember(Value = "1005")] NUMBER_1005 = 6,

        /// <summary>
        ///     Enum NUMBER_1006 for value: 1006
        /// </summary>
        [EnumMember(Value = "1006")] NUMBER_1006 = 7,

        /// <summary>
        ///     Enum NUMBER_1007 for value: 1007
        /// </summary>
        [EnumMember(Value = "1007")] NUMBER_1007 = 8,

        /// <summary>
        ///     Enum NUMBER_10000 for value: 10000
        /// </summary>
        [EnumMember(Value = "10000")] NUMBER_10000 = 9,

        /// <summary>
        ///     Enum NUMBER_10001 for value: 10001
        /// </summary>
        [EnumMember(Value = "10001")] NUMBER_10001 = 10,

        /// <summary>
        ///     Enum NUMBER_10002 for value: 10002
        /// </summary>
        [EnumMember(Value = "10002")] NUMBER_10002 = 11,

        /// <summary>
        ///     Enum NUMBER_10003 for value: 10003
        /// </summary>
        [EnumMember(Value = "10003")] NUMBER_10003 = 12,

        /// <summary>
        ///     Enum NUMBER_10004 for value: 10004
        /// </summary>
        [EnumMember(Value = "10004")] NUMBER_10004 = 13,

        /// <summary>
        ///     Enum NUMBER_10005 for value: 10005
        /// </summary>
        [EnumMember(Value = "10005")] NUMBER_10005 = 14,

        /// <summary>
        ///     Enum NUMBER_10006 for value: 10006
        /// </summary>
        [EnumMember(Value = "10006")] NUMBER_10006 = 15,

        /// <summary>
        ///     Enum NUMBER_10007 for value: 10007
        /// </summary>
        [EnumMember(Value = "10007")] NUMBER_10007 = 16,

        /// <summary>
        ///     Enum NUMBER_10008 for value: 10008
        /// </summary>
        [EnumMember(Value = "10008")] NUMBER_10008 = 17,

        /// <summary>
        ///     Enum NUMBER_10009 for value: 10009
        /// </summary>
        [EnumMember(Value = "10009")] NUMBER_10009 = 18,

        /// <summary>
        ///     Enum NUMBER_10010 for value: 10010
        /// </summary>
        [EnumMember(Value = "10010")] NUMBER_10010 = 19,

        /// <summary>
        ///     Enum NUMBER_10011 for value: 10011
        /// </summary>
        [EnumMember(Value = "10011")] NUMBER_10011 = 20,

        /// <summary>
        ///     Enum NUMBER_10012 for value: 10012
        /// </summary>
        [EnumMember(Value = "10012")] NUMBER_10012 = 21,

        /// <summary>
        ///     Enum NUMBER_10013 for value: 10013
        /// </summary>
        [EnumMember(Value = "10013")] NUMBER_10013 = 22,

        /// <summary>
        ///     Enum NUMBER_10014 for value: 10014
        /// </summary>
        [EnumMember(Value = "10014")] NUMBER_10014 = 23,

        /// <summary>
        ///     Enum NUMBER_10015 for value: 10015
        /// </summary>
        [EnumMember(Value = "10015")] NUMBER_10015 = 24,

        /// <summary>
        ///     Enum NUMBER_20000 for value: 20000
        /// </summary>
        [EnumMember(Value = "20000")] NUMBER_20000 = 25,

        /// <summary>
        ///     Enum NUMBER_20001 for value: 20001
        /// </summary>
        [EnumMember(Value = "20001")] NUMBER_20001 = 26,

        /// <summary>
        ///     Enum NUMBER_20002 for value: 20002
        /// </summary>
        [EnumMember(Value = "20002")] NUMBER_20002 = 27,

        /// <summary>
        ///     Enum NUMBER_20003 for value: 20003
        /// </summary>
        [EnumMember(Value = "20003")] NUMBER_20003 = 28,

        /// <summary>
        ///     Enum NUMBER_20004 for value: 20004
        /// </summary>
        [EnumMember(Value = "20004")] NUMBER_20004 = 29,

        /// <summary>
        ///     Enum NUMBER_20005 for value: 20005
        /// </summary>
        [EnumMember(Value = "20005")] NUMBER_20005 = 30,

        /// <summary>
        ///     Enum NUMBER_20006 for value: 20006
        /// </summary>
        [EnumMember(Value = "20006")] NUMBER_20006 = 31,

        /// <summary>
        ///     Enum NUMBER_20007 for value: 20007
        /// </summary>
        [EnumMember(Value = "20007")] NUMBER_20007 = 32,

        /// <summary>
        ///     Enum NUMBER_30000 for value: 30000
        /// </summary>
        [EnumMember(Value = "30000")] NUMBER_30000 = 33,

        /// <summary>
        ///     Enum NUMBER_30001 for value: 30001
        /// </summary>
        [EnumMember(Value = "30001")] NUMBER_30001 = 34,

        /// <summary>
        ///     Enum NUMBER_30002 for value: 30002
        /// </summary>
        [EnumMember(Value = "30002")] NUMBER_30002 = 35,

        /// <summary>
        ///     Enum NUMBER_30003 for value: 30003
        /// </summary>
        [EnumMember(Value = "30003")] NUMBER_30003 = 36,

        /// <summary>
        ///     Enum NUMBER_30004 for value: 30004
        /// </summary>
        [EnumMember(Value = "30004")] NUMBER_30004 = 37,

        /// <summary>
        ///     Enum NUMBER_30005 for value: 30005
        /// </summary>
        [EnumMember(Value = "30005")] NUMBER_30005 = 38,

        /// <summary>
        ///     Enum NUMBER_30006 for value: 30006
        /// </summary>
        [EnumMember(Value = "30006")] NUMBER_30006 = 39,

        /// <summary>
        ///     Enum NUMBER_40000 for value: 40000
        /// </summary>
        [EnumMember(Value = "40000")] NUMBER_40000 = 40
    }
}