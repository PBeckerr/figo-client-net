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
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Figo.Client.Core.Model
{
    /// <summary>
    ///     AdditionalIcons
    /// </summary>
    [DataContract]
    public class AdditionalIcons : IEquatable<AdditionalIcons>, IValidatableObject
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AdditionalIcons" /> class.
        /// </summary>
        /// <param name="_48x48">URL to provider icon in aspect ratio 48x48px..</param>
        /// <param name="_60x60">URL to provider icon in aspect ratio 60x60px..</param>
        /// <param name="_72x72">URL to provider icon in aspect ratio 72x72px..</param>
        /// <param name="_84x84">URL to provider icon in aspect ratio 84x84px..</param>
        /// <param name="_96x96">URL to provider icon in aspect ratio 96x96px..</param>
        /// <param name="_120x120">URL to provider icon in aspect ratio 120x120px..</param>
        /// <param name="_144x144">URL to provider icon in aspect ratio 144x144px..</param>
        /// <param name="_192x192">URL to provider icon in aspect ratio 192x192px..</param>
        /// <param name="_256x256">URL to provider icon in aspect ratio 256x256px..</param>
        public AdditionalIcons(string _48x48 = default, string _60x60 = default, string _72x72 = default, string _84x84 = default, string _96x96 = default,
                               string _120x120 = default, string _144x144 = default, string _192x192 = default, string _256x256 = default)
        {
            this._48x48 = _48x48;
            this._60x60 = _60x60;
            this._72x72 = _72x72;
            this._84x84 = _84x84;
            this._96x96 = _96x96;
            this._120x120 = _120x120;
            this._144x144 = _144x144;
            this._192x192 = _192x192;
            this._256x256 = _256x256;
        }

        /// <summary>
        ///     URL to provider icon in aspect ratio 48x48px.
        /// </summary>
        /// <value>URL to provider icon in aspect ratio 48x48px.</value>
        [DataMember(Name = "48x48", EmitDefaultValue = false)]
        public string _48x48 { get; set; }

        /// <summary>
        ///     URL to provider icon in aspect ratio 60x60px.
        /// </summary>
        /// <value>URL to provider icon in aspect ratio 60x60px.</value>
        [DataMember(Name = "60x60", EmitDefaultValue = false)]
        public string _60x60 { get; set; }

        /// <summary>
        ///     URL to provider icon in aspect ratio 72x72px.
        /// </summary>
        /// <value>URL to provider icon in aspect ratio 72x72px.</value>
        [DataMember(Name = "72x72", EmitDefaultValue = false)]
        public string _72x72 { get; set; }

        /// <summary>
        ///     URL to provider icon in aspect ratio 84x84px.
        /// </summary>
        /// <value>URL to provider icon in aspect ratio 84x84px.</value>
        [DataMember(Name = "84x84", EmitDefaultValue = false)]
        public string _84x84 { get; set; }

        /// <summary>
        ///     URL to provider icon in aspect ratio 96x96px.
        /// </summary>
        /// <value>URL to provider icon in aspect ratio 96x96px.</value>
        [DataMember(Name = "96x96", EmitDefaultValue = false)]
        public string _96x96 { get; set; }

        /// <summary>
        ///     URL to provider icon in aspect ratio 120x120px.
        /// </summary>
        /// <value>URL to provider icon in aspect ratio 120x120px.</value>
        [DataMember(Name = "120x120", EmitDefaultValue = false)]
        public string _120x120 { get; set; }

        /// <summary>
        ///     URL to provider icon in aspect ratio 144x144px.
        /// </summary>
        /// <value>URL to provider icon in aspect ratio 144x144px.</value>
        [DataMember(Name = "144x144", EmitDefaultValue = false)]
        public string _144x144 { get; set; }

        /// <summary>
        ///     URL to provider icon in aspect ratio 192x192px.
        /// </summary>
        /// <value>URL to provider icon in aspect ratio 192x192px.</value>
        [DataMember(Name = "192x192", EmitDefaultValue = false)]
        public string _192x192 { get; set; }

        /// <summary>
        ///     URL to provider icon in aspect ratio 256x256px.
        /// </summary>
        /// <value>URL to provider icon in aspect ratio 256x256px.</value>
        [DataMember(Name = "256x256", EmitDefaultValue = false)]
        public string _256x256 { get; set; }

        /// <summary>
        ///     Returns true if AdditionalIcons instances are equal
        /// </summary>
        /// <param name="input">Instance of AdditionalIcons to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AdditionalIcons input)
        {
            if (input == null)
            {
                return false;
            }

            return
                (
                    this._48x48 == input._48x48 ||
                    this._48x48 != null &&
                    this._48x48.Equals(input._48x48)
                ) &&
                (
                    this._60x60 == input._60x60 ||
                    this._60x60 != null &&
                    this._60x60.Equals(input._60x60)
                ) &&
                (
                    this._72x72 == input._72x72 ||
                    this._72x72 != null &&
                    this._72x72.Equals(input._72x72)
                ) &&
                (
                    this._84x84 == input._84x84 ||
                    this._84x84 != null &&
                    this._84x84.Equals(input._84x84)
                ) &&
                (
                    this._96x96 == input._96x96 ||
                    this._96x96 != null &&
                    this._96x96.Equals(input._96x96)
                ) &&
                (
                    this._120x120 == input._120x120 ||
                    this._120x120 != null &&
                    this._120x120.Equals(input._120x120)
                ) &&
                (
                    this._144x144 == input._144x144 ||
                    this._144x144 != null &&
                    this._144x144.Equals(input._144x144)
                ) &&
                (
                    this._192x192 == input._192x192 ||
                    this._192x192 != null &&
                    this._192x192.Equals(input._192x192)
                ) &&
                (
                    this._256x256 == input._256x256 ||
                    this._256x256 != null &&
                    this._256x256.Equals(input._256x256)
                );
        }

        /// <summary>
        ///     To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }

        /// <summary>
        ///     Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AdditionalIcons {\n");
            sb.Append("  _48x48: ").Append(this._48x48).Append("\n");
            sb.Append("  _60x60: ").Append(this._60x60).Append("\n");
            sb.Append("  _72x72: ").Append(this._72x72).Append("\n");
            sb.Append("  _84x84: ").Append(this._84x84).Append("\n");
            sb.Append("  _96x96: ").Append(this._96x96).Append("\n");
            sb.Append("  _120x120: ").Append(this._120x120).Append("\n");
            sb.Append("  _144x144: ").Append(this._144x144).Append("\n");
            sb.Append("  _192x192: ").Append(this._192x192).Append("\n");
            sb.Append("  _256x256: ").Append(this._256x256).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        ///     Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        ///     Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as AdditionalIcons);
        }

        /// <summary>
        ///     Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                if (this._48x48 != null)
                {
                    hashCode = hashCode * 59 + this._48x48.GetHashCode();
                }

                if (this._60x60 != null)
                {
                    hashCode = hashCode * 59 + this._60x60.GetHashCode();
                }

                if (this._72x72 != null)
                {
                    hashCode = hashCode * 59 + this._72x72.GetHashCode();
                }

                if (this._84x84 != null)
                {
                    hashCode = hashCode * 59 + this._84x84.GetHashCode();
                }

                if (this._96x96 != null)
                {
                    hashCode = hashCode * 59 + this._96x96.GetHashCode();
                }

                if (this._120x120 != null)
                {
                    hashCode = hashCode * 59 + this._120x120.GetHashCode();
                }

                if (this._144x144 != null)
                {
                    hashCode = hashCode * 59 + this._144x144.GetHashCode();
                }

                if (this._192x192 != null)
                {
                    hashCode = hashCode * 59 + this._192x192.GetHashCode();
                }

                if (this._256x256 != null)
                {
                    hashCode = hashCode * 59 + this._256x256.GetHashCode();
                }

                return hashCode;
            }
        }
    }
}