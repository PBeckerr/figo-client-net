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
    ///     Account
    /// </summary>
    [DataContract]
    public class Account : IEquatable<Account>, IValidatableObject
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Account" /> class.
        /// </summary>
        /// <param name="accountId">figo ID of account..</param>
        /// <param name="accountNumber">Domestic account identifier..</param>
        /// <param name="bankCode">Domestic bank code..</param>
        /// <param name="iban">International Bank Account Number (IBAN)..</param>
        /// <param name="bic">Business Identifier Code (SWIFT-BIC)..</param>
        /// <param name="accessId">figo ID of the provider access..</param>
        /// <param name="bankName">Name of bank or financial provider..</param>
        /// <param name="icon">icon.</param>
        /// <param name="currency">currency.</param>
        /// <param name="balance">balance.</param>
        /// <param name="type">type.</param>
        /// <param name="name">Name of the account..</param>
        /// <param name="owner">Name of the account holder..</param>
        /// <param name="autoSync">This flag indicates whether the account will be automatically synchronized..</param>
        /// <param name="savePin">
        ///     This flag indicates whether the user has chosen to save the PIN on the figo API backend. (default
        ///     to false).
        /// </param>
        /// <param name="supportedPayments">supportedPayments.</param>
        /// <param name="status">status.</param>
        public Account(string accountId = default, string accountNumber = default, string bankCode = default, string iban = default, string bic = default,
                       string accessId = default, string bankName = default, Icon icon = default, Currency currency = default, AccountBalance balance = default,
                       AccountType type = default, string name = default, string owner = default, bool autoSync = default, bool savePin = false,
                       AccountSupportedPayments supportedPayments = default, Status status = default)
        {
            this.AccountId = accountId;
            this.AccountNumber = accountNumber;
            this.BankCode = bankCode;
            this.Iban = iban;
            this.Bic = bic;
            this.AccessId = accessId;
            this.BankName = bankName;
            this.Icon = icon;
            this.Currency = currency;
            this.Balance = balance;
            this.Type = type;
            this.Name = name;
            this.Owner = owner;
            this.AutoSync = autoSync;
            // use default value if no "savePin" provided
            if (savePin == null)
            {
                this.SavePin = false;
            }
            else
            {
                this.SavePin = savePin;
            }

            this.SupportedPayments = supportedPayments;
            this.Status = status;
        }

        /// <summary>
        ///     Gets or Sets Currency
        /// </summary>
        [DataMember(Name = "currency", EmitDefaultValue = false)]
        public Currency? Currency { get; set; }

        /// <summary>
        ///     Gets or Sets Type
        /// </summary>
        [DataMember(Name = "type", EmitDefaultValue = false)]
        public AccountType? Type { get; set; }

        /// <summary>
        ///     figo ID of account.
        /// </summary>
        /// <value>figo ID of account.</value>
        [DataMember(Name = "account_id", EmitDefaultValue = false)]
        public string AccountId { get; set; }

        /// <summary>
        ///     Domestic account identifier.
        /// </summary>
        /// <value>Domestic account identifier.</value>
        [DataMember(Name = "account_number", EmitDefaultValue = false)]
        public string AccountNumber { get; set; }

        /// <summary>
        ///     Domestic bank code.
        /// </summary>
        /// <value>Domestic bank code.</value>
        [DataMember(Name = "bank_code", EmitDefaultValue = false)]
        public string BankCode { get; set; }

        /// <summary>
        ///     International Bank Account Number (IBAN).
        /// </summary>
        /// <value>International Bank Account Number (IBAN).</value>
        [DataMember(Name = "iban", EmitDefaultValue = false)]
        public string Iban { get; set; }

        /// <summary>
        ///     Business Identifier Code (SWIFT-BIC).
        /// </summary>
        /// <value>Business Identifier Code (SWIFT-BIC).</value>
        [DataMember(Name = "bic", EmitDefaultValue = false)]
        public string Bic { get; set; }

        /// <summary>
        ///     figo ID of the provider access.
        /// </summary>
        /// <value>figo ID of the provider access.</value>
        [DataMember(Name = "access_id", EmitDefaultValue = false)]
        public string AccessId { get; set; }

        /// <summary>
        ///     Name of bank or financial provider.
        /// </summary>
        /// <value>Name of bank or financial provider.</value>
        [DataMember(Name = "bank_name", EmitDefaultValue = false)]
        public string BankName { get; set; }

        /// <summary>
        ///     Gets or Sets Icon
        /// </summary>
        [DataMember(Name = "icon", EmitDefaultValue = false)]
        public Icon Icon { get; set; }

        /// <summary>
        ///     Gets or Sets Balance
        /// </summary>
        [DataMember(Name = "balance", EmitDefaultValue = false)]
        public AccountBalance Balance { get; set; }

        /// <summary>
        ///     Name of the account.
        /// </summary>
        /// <value>Name of the account.</value>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        ///     Name of the account holder.
        /// </summary>
        /// <value>Name of the account holder.</value>
        [DataMember(Name = "owner", EmitDefaultValue = false)]
        public string Owner { get; set; }

        /// <summary>
        ///     This flag indicates whether the account will be automatically synchronized.
        /// </summary>
        /// <value>This flag indicates whether the account will be automatically synchronized.</value>
        [DataMember(Name = "auto_sync", EmitDefaultValue = false)]
        public bool AutoSync { get; set; }

        /// <summary>
        ///     This flag indicates whether the user has chosen to save the PIN on the figo API backend.
        /// </summary>
        /// <value>This flag indicates whether the user has chosen to save the PIN on the figo API backend.</value>
        [DataMember(Name = "save_pin", EmitDefaultValue = false)]
        public bool SavePin { get; set; }

        /// <summary>
        ///     Gets or Sets SupportedPayments
        /// </summary>
        [DataMember(Name = "supported_payments", EmitDefaultValue = false)]
        public AccountSupportedPayments SupportedPayments { get; set; }

        /// <summary>
        ///     Gets or Sets Status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public Status Status { get; set; }

        /// <summary>
        ///     Returns true if Account instances are equal
        /// </summary>
        /// <param name="input">Instance of Account to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Account input)
        {
            if (input == null)
            {
                return false;
            }

            return
                (
                    this.AccountId == input.AccountId ||
                    this.AccountId != null &&
                    this.AccountId.Equals(input.AccountId)
                ) &&
                (
                    this.AccountNumber == input.AccountNumber ||
                    this.AccountNumber != null &&
                    this.AccountNumber.Equals(input.AccountNumber)
                ) &&
                (
                    this.BankCode == input.BankCode ||
                    this.BankCode != null &&
                    this.BankCode.Equals(input.BankCode)
                ) &&
                (
                    this.Iban == input.Iban ||
                    this.Iban != null &&
                    this.Iban.Equals(input.Iban)
                ) &&
                (
                    this.Bic == input.Bic ||
                    this.Bic != null &&
                    this.Bic.Equals(input.Bic)
                ) &&
                (
                    this.AccessId == input.AccessId ||
                    this.AccessId != null &&
                    this.AccessId.Equals(input.AccessId)
                ) &&
                (
                    this.BankName == input.BankName ||
                    this.BankName != null &&
                    this.BankName.Equals(input.BankName)
                ) &&
                (
                    this.Icon == input.Icon ||
                    this.Icon != null &&
                    this.Icon.Equals(input.Icon)
                ) &&
                (
                    this.Currency == input.Currency ||
                    this.Currency.Equals(input.Currency)
                ) &&
                (
                    this.Balance == input.Balance ||
                    this.Balance != null &&
                    this.Balance.Equals(input.Balance)
                ) &&
                (
                    this.Type == input.Type ||
                    this.Type.Equals(input.Type)
                ) &&
                (
                    this.Name == input.Name ||
                    this.Name != null &&
                    this.Name.Equals(input.Name)
                ) &&
                (
                    this.Owner == input.Owner ||
                    this.Owner != null &&
                    this.Owner.Equals(input.Owner)
                ) &&
                (
                    this.AutoSync == input.AutoSync ||
                    this.AutoSync.Equals(input.AutoSync)
                ) &&
                (
                    this.SavePin == input.SavePin ||
                    this.SavePin.Equals(input.SavePin)
                ) &&
                (
                    this.SupportedPayments == input.SupportedPayments ||
                    this.SupportedPayments != null &&
                    this.SupportedPayments.Equals(input.SupportedPayments)
                ) &&
                (
                    this.Status == input.Status ||
                    this.Status != null &&
                    this.Status.Equals(input.Status)
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
            sb.Append("class Account {\n");
            sb.Append("  AccountId: ").Append(this.AccountId).Append("\n");
            sb.Append("  AccountNumber: ").Append(this.AccountNumber).Append("\n");
            sb.Append("  BankCode: ").Append(this.BankCode).Append("\n");
            sb.Append("  Iban: ").Append(this.Iban).Append("\n");
            sb.Append("  Bic: ").Append(this.Bic).Append("\n");
            sb.Append("  AccessId: ").Append(this.AccessId).Append("\n");
            sb.Append("  BankName: ").Append(this.BankName).Append("\n");
            sb.Append("  Icon: ").Append(this.Icon).Append("\n");
            sb.Append("  Currency: ").Append(this.Currency).Append("\n");
            sb.Append("  Balance: ").Append(this.Balance).Append("\n");
            sb.Append("  Type: ").Append(this.Type).Append("\n");
            sb.Append("  Name: ").Append(this.Name).Append("\n");
            sb.Append("  Owner: ").Append(this.Owner).Append("\n");
            sb.Append("  AutoSync: ").Append(this.AutoSync).Append("\n");
            sb.Append("  SavePin: ").Append(this.SavePin).Append("\n");
            sb.Append("  SupportedPayments: ").Append(this.SupportedPayments).Append("\n");
            sb.Append("  Status: ").Append(this.Status).Append("\n");
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
            return this.Equals(input as Account);
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
                if (this.AccountId != null)
                {
                    hashCode = hashCode * 59 + this.AccountId.GetHashCode();
                }

                if (this.AccountNumber != null)
                {
                    hashCode = hashCode * 59 + this.AccountNumber.GetHashCode();
                }

                if (this.BankCode != null)
                {
                    hashCode = hashCode * 59 + this.BankCode.GetHashCode();
                }

                if (this.Iban != null)
                {
                    hashCode = hashCode * 59 + this.Iban.GetHashCode();
                }

                if (this.Bic != null)
                {
                    hashCode = hashCode * 59 + this.Bic.GetHashCode();
                }

                if (this.AccessId != null)
                {
                    hashCode = hashCode * 59 + this.AccessId.GetHashCode();
                }

                if (this.BankName != null)
                {
                    hashCode = hashCode * 59 + this.BankName.GetHashCode();
                }

                if (this.Icon != null)
                {
                    hashCode = hashCode * 59 + this.Icon.GetHashCode();
                }

                hashCode = hashCode * 59 + this.Currency.GetHashCode();
                if (this.Balance != null)
                {
                    hashCode = hashCode * 59 + this.Balance.GetHashCode();
                }

                hashCode = hashCode * 59 + this.Type.GetHashCode();
                if (this.Name != null)
                {
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                }

                if (this.Owner != null)
                {
                    hashCode = hashCode * 59 + this.Owner.GetHashCode();
                }

                hashCode = hashCode * 59 + this.AutoSync.GetHashCode();
                hashCode = hashCode * 59 + this.SavePin.GetHashCode();
                if (this.SupportedPayments != null)
                {
                    hashCode = hashCode * 59 + this.SupportedPayments.GetHashCode();
                }

                if (this.Status != null)
                {
                    hashCode = hashCode * 59 + this.Status.GetHashCode();
                }

                return hashCode;
            }
        }
    }
}