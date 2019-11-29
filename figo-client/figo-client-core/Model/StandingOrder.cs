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
using Newtonsoft.Json.Converters;

namespace Figo.Client.Core.Model
{
    /// <summary>
    ///     StandingOrder
    /// </summary>
    [DataContract]
    public class StandingOrder : IEquatable<StandingOrder>, IValidatableObject
    {
        /// <summary>
        ///     Interval in which the standing order will be executed.
        /// </summary>
        /// <value>Interval in which the standing order will be executed.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum IntervalEnum
        {
            /// <summary>
            ///     Enum Weekly for value: weekly
            /// </summary>
            [EnumMember(Value = "weekly")] Weekly = 1,

            /// <summary>
            ///     Enum Monthly for value: monthly
            /// </summary>
            [EnumMember(Value = "monthly")] Monthly = 2,

            /// <summary>
            ///     Enum Twomonthly for value: two monthly
            /// </summary>
            [EnumMember(Value = "two monthly")] Twomonthly = 3,

            /// <summary>
            ///     Enum Fourmonthly for value: four monthly
            /// </summary>
            [EnumMember(Value = "four monthly")] Fourmonthly = 4,

            /// <summary>
            ///     Enum Quarterly for value: quarterly
            /// </summary>
            [EnumMember(Value = "quarterly")] Quarterly = 5,

            /// <summary>
            ///     Enum Halfyearly for value: half yearly
            /// </summary>
            [EnumMember(Value = "half yearly")] Halfyearly = 6,

            /// <summary>
            ///     Enum Yearly for value: yearly
            /// </summary>
            [EnumMember(Value = "yearly")] Yearly = 7
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StandingOrder" /> class.
        /// </summary>
        /// <param name="accountId">Internal figo account ID..</param>
        /// <param name="amount">Order amount..</param>
        /// <param name="cents">If true, the amount is submitted as cents..</param>
        /// <param name="createdAt">Internal creation timestamp..</param>
        /// <param name="currency">Alphabetic currency code of the amount..</param>
        /// <param name="executionDay">
        ///     Identifier of the day when the standing order will be regularly executed. Permitted values
        ///     depend on the interval..
        /// </param>
        /// <param name="firstExecutionDate">The first date when the standing order will be executed..</param>
        /// <param name="iban">The IBAN of the creditor or debtor..</param>
        /// <param name="interval">Interval in which the standing order will be executed..</param>
        /// <param name="lastExecutionDate">
        ///     The last date when the standing order will be executed. The day of month must be the
        ///     same as that of the first execution..
        /// </param>
        /// <param name="modifiedAt">Internal modification timestamp..</param>
        /// <param name="name">Name of creditor or debtor..</param>
        /// <param name="purpose">Purpose text..</param>
        /// <param name="standingOrderId">Internal figo standing order ID..</param>
        public StandingOrder(string accountId = default, decimal amount = default, bool cents = default, string createdAt = default, string currency = default,
                             int executionDay = default, string firstExecutionDate = default, string iban = default, IntervalEnum? interval = default,
                             string lastExecutionDate = default, string modifiedAt = default, string name = default, string purpose = default,
                             string standingOrderId = default)
        {
            this.AccountId = accountId;
            this.Amount = amount;
            this.Cents = cents;
            this.CreatedAt = createdAt;
            this.Currency = currency;
            this.ExecutionDay = executionDay;
            this.FirstExecutionDate = firstExecutionDate;
            this.Iban = iban;
            this.Interval = interval;
            this.LastExecutionDate = lastExecutionDate;
            this.ModifiedAt = modifiedAt;
            this.Name = name;
            this.Purpose = purpose;
            this.StandingOrderId = standingOrderId;
        }

        /// <summary>
        ///     Interval in which the standing order will be executed.
        /// </summary>
        /// <value>Interval in which the standing order will be executed.</value>
        [DataMember(Name = "interval", EmitDefaultValue = false)]
        public IntervalEnum? Interval { get; set; }

        /// <summary>
        ///     Internal figo account ID.
        /// </summary>
        /// <value>Internal figo account ID.</value>
        [DataMember(Name = "account_id", EmitDefaultValue = false)]
        public string AccountId { get; set; }

        /// <summary>
        ///     Order amount.
        /// </summary>
        /// <value>Order amount.</value>
        [DataMember(Name = "amount", EmitDefaultValue = false)]
        public decimal Amount { get; set; }

        /// <summary>
        ///     If true, the amount is submitted as cents.
        /// </summary>
        /// <value>If true, the amount is submitted as cents.</value>
        [DataMember(Name = "cents", EmitDefaultValue = false)]
        public bool Cents { get; set; }

        /// <summary>
        ///     Internal creation timestamp.
        /// </summary>
        /// <value>Internal creation timestamp.</value>
        [DataMember(Name = "created_at", EmitDefaultValue = false)]
        public string CreatedAt { get; set; }

        /// <summary>
        ///     Alphabetic currency code of the amount.
        /// </summary>
        /// <value>Alphabetic currency code of the amount.</value>
        [DataMember(Name = "currency", EmitDefaultValue = false)]
        public string Currency { get; set; }

        /// <summary>
        ///     Identifier of the day when the standing order will be regularly executed. Permitted values depend on the 
        ///     interval.
        /// </summary>
        /// <value>
        ///     Identifier of the day when the standing order will be regularly executed. Permitted values depend on the 
        ///     interval.
        /// </value>
        [DataMember(Name = "execution_day", EmitDefaultValue = false)]
        public int ExecutionDay { get; set; }

        /// <summary>
        ///     The first date when the standing order will be executed.
        /// </summary>
        /// <value>The first date when the standing order will be executed.</value>
        [DataMember(Name = "first_execution_date", EmitDefaultValue = false)]
        public string FirstExecutionDate { get; set; }

        /// <summary>
        ///     The IBAN of the creditor or debtor.
        /// </summary>
        /// <value>The IBAN of the creditor or debtor.</value>
        [DataMember(Name = "iban", EmitDefaultValue = false)]
        public string Iban { get; set; }

        /// <summary>
        ///     The last date when the standing order will be executed. The day of month must be the same as that of the first
        ///     execution.
        /// </summary>
        /// <value>
        ///     The last date when the standing order will be executed. The day of month must be the same as that of the first
        ///     execution.
        /// </value>
        [DataMember(Name = "last_execution_date", EmitDefaultValue = false)]
        public string LastExecutionDate { get; set; }

        /// <summary>
        ///     Internal modification timestamp.
        /// </summary>
        /// <value>Internal modification timestamp.</value>
        [DataMember(Name = "modified_at", EmitDefaultValue = false)]
        public string ModifiedAt { get; set; }

        /// <summary>
        ///     Name of creditor or debtor.
        /// </summary>
        /// <value>Name of creditor or debtor.</value>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        ///     Purpose text.
        /// </summary>
        /// <value>Purpose text.</value>
        [DataMember(Name = "purpose", EmitDefaultValue = false)]
        public string Purpose { get; set; }

        /// <summary>
        ///     Internal figo standing order ID.
        /// </summary>
        /// <value>Internal figo standing order ID.</value>
        [DataMember(Name = "standing_order_id", EmitDefaultValue = false)]
        public string StandingOrderId { get; set; }

        /// <summary>
        ///     Returns true if StandingOrder instances are equal
        /// </summary>
        /// <param name="input">Instance of StandingOrder to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(StandingOrder input)
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
                    this.Amount == input.Amount ||
                    this.Amount.Equals(input.Amount)
                ) &&
                (
                    this.Cents == input.Cents ||
                    this.Cents.Equals(input.Cents)
                ) &&
                (
                    this.CreatedAt == input.CreatedAt ||
                    this.CreatedAt != null &&
                    this.CreatedAt.Equals(input.CreatedAt)
                ) &&
                (
                    this.Currency == input.Currency ||
                    this.Currency != null &&
                    this.Currency.Equals(input.Currency)
                ) &&
                (
                    this.ExecutionDay == input.ExecutionDay ||
                    this.ExecutionDay.Equals(input.ExecutionDay)
                ) &&
                (
                    this.FirstExecutionDate == input.FirstExecutionDate ||
                    this.FirstExecutionDate != null &&
                    this.FirstExecutionDate.Equals(input.FirstExecutionDate)
                ) &&
                (
                    this.Iban == input.Iban ||
                    this.Iban != null &&
                    this.Iban.Equals(input.Iban)
                ) &&
                (
                    this.Interval == input.Interval ||
                    this.Interval.Equals(input.Interval)
                ) &&
                (
                    this.LastExecutionDate == input.LastExecutionDate ||
                    this.LastExecutionDate != null &&
                    this.LastExecutionDate.Equals(input.LastExecutionDate)
                ) &&
                (
                    this.ModifiedAt == input.ModifiedAt ||
                    this.ModifiedAt != null &&
                    this.ModifiedAt.Equals(input.ModifiedAt)
                ) &&
                (
                    this.Name == input.Name ||
                    this.Name != null &&
                    this.Name.Equals(input.Name)
                ) &&
                (
                    this.Purpose == input.Purpose ||
                    this.Purpose != null &&
                    this.Purpose.Equals(input.Purpose)
                ) &&
                (
                    this.StandingOrderId == input.StandingOrderId ||
                    this.StandingOrderId != null &&
                    this.StandingOrderId.Equals(input.StandingOrderId)
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
            sb.Append("class StandingOrder {\n");
            sb.Append("  AccountId: ").Append(this.AccountId).Append("\n");
            sb.Append("  Amount: ").Append(this.Amount).Append("\n");
            sb.Append("  Cents: ").Append(this.Cents).Append("\n");
            sb.Append("  CreatedAt: ").Append(this.CreatedAt).Append("\n");
            sb.Append("  Currency: ").Append(this.Currency).Append("\n");
            sb.Append("  ExecutionDay: ").Append(this.ExecutionDay).Append("\n");
            sb.Append("  FirstExecutionDate: ").Append(this.FirstExecutionDate).Append("\n");
            sb.Append("  Iban: ").Append(this.Iban).Append("\n");
            sb.Append("  Interval: ").Append(this.Interval).Append("\n");
            sb.Append("  LastExecutionDate: ").Append(this.LastExecutionDate).Append("\n");
            sb.Append("  ModifiedAt: ").Append(this.ModifiedAt).Append("\n");
            sb.Append("  Name: ").Append(this.Name).Append("\n");
            sb.Append("  Purpose: ").Append(this.Purpose).Append("\n");
            sb.Append("  StandingOrderId: ").Append(this.StandingOrderId).Append("\n");
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
            return this.Equals(input as StandingOrder);
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

                hashCode = hashCode * 59 + this.Amount.GetHashCode();
                hashCode = hashCode * 59 + this.Cents.GetHashCode();
                if (this.CreatedAt != null)
                {
                    hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                }

                if (this.Currency != null)
                {
                    hashCode = hashCode * 59 + this.Currency.GetHashCode();
                }

                hashCode = hashCode * 59 + this.ExecutionDay.GetHashCode();
                if (this.FirstExecutionDate != null)
                {
                    hashCode = hashCode * 59 + this.FirstExecutionDate.GetHashCode();
                }

                if (this.Iban != null)
                {
                    hashCode = hashCode * 59 + this.Iban.GetHashCode();
                }

                hashCode = hashCode * 59 + this.Interval.GetHashCode();
                if (this.LastExecutionDate != null)
                {
                    hashCode = hashCode * 59 + this.LastExecutionDate.GetHashCode();
                }

                if (this.ModifiedAt != null)
                {
                    hashCode = hashCode * 59 + this.ModifiedAt.GetHashCode();
                }

                if (this.Name != null)
                {
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                }

                if (this.Purpose != null)
                {
                    hashCode = hashCode * 59 + this.Purpose.GetHashCode();
                }

                if (this.StandingOrderId != null)
                {
                    hashCode = hashCode * 59 + this.StandingOrderId.GetHashCode();
                }

                return hashCode;
            }
        }
    }
}