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
    ///     Security
    /// </summary>
    [DataContract]
    public class Security : IEquatable<Security>, IValidatableObject
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Security" /> class.
        /// </summary>
        /// <param name="accountId">Internal figo account ID..</param>
        /// <param name="amount">Monetary value in account currency..</param>
        /// <param name="amountOriginalCurrency">Monetary value in trading currency..</param>
        /// <param name="createdAt">Internal creation timestamp..</param>
        /// <param name="currency">Alphabetic currency code of the amount..</param>
        /// <param name="exchangeRate">Exchange rate between trading and accout price..</param>
        /// <param name="isin">International Securities Identification Number..</param>
        /// <param name="market">Market the security is traded on..</param>
        /// <param name="modifiedAt">Internal modification timestamp..</param>
        /// <param name="name">Name of the security..</param>
        /// <param name="price">Trading price..</param>
        /// <param name="priceCurrency">Alphabetic currency code of the price..</param>
        /// <param name="purchasePrice">Purchase price..</param>
        /// <param name="purchasePriceCurrency">Alphabetic currency code of the purchase price..</param>
        /// <param name="securityId">Internal figo security ID..</param>
        /// <param name="tradeTimestamp">Trading timestamp..</param>
        /// <param name="wkn">Domestic security identification number..</param>
        /// <param name="quantity">Quantity in stock..</param>
        public Security(string accountId = default, decimal amount = default, decimal amountOriginalCurrency = default, string createdAt = default,
                        string currency = default, decimal exchangeRate = default, string isin = default, string market = default, string modifiedAt = default,
                        string name = default, decimal price = default, string priceCurrency = default, decimal purchasePrice = default,
                        string purchasePriceCurrency = default, string securityId = default, string tradeTimestamp = default, string wkn = default,
                        decimal quantity = default)
        {
            this.AccountId = accountId;
            this.Amount = amount;
            this.AmountOriginalCurrency = amountOriginalCurrency;
            this.CreatedAt = createdAt;
            this.Currency = currency;
            this.ExchangeRate = exchangeRate;
            this.Isin = isin;
            this.Market = market;
            this.ModifiedAt = modifiedAt;
            this.Name = name;
            this.Price = price;
            this.PriceCurrency = priceCurrency;
            this.PurchasePrice = purchasePrice;
            this.PurchasePriceCurrency = purchasePriceCurrency;
            this.SecurityId = securityId;
            this.TradeTimestamp = tradeTimestamp;
            this.Wkn = wkn;
            this.Quantity = quantity;
        }

        /// <summary>
        ///     Internal figo account ID.
        /// </summary>
        /// <value>Internal figo account ID.</value>
        [DataMember(Name = "account_id", EmitDefaultValue = false)]
        public string AccountId { get; set; }

        /// <summary>
        ///     Monetary value in account currency.
        /// </summary>
        /// <value>Monetary value in account currency.</value>
        [DataMember(Name = "amount", EmitDefaultValue = false)]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Monetary value in trading currency.
        /// </summary>
        /// <value>Monetary value in trading currency.</value>
        [DataMember(Name = "amount_original_currency", EmitDefaultValue = false)]
        public decimal AmountOriginalCurrency { get; set; }

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
        ///     Exchange rate between trading and accout price.
        /// </summary>
        /// <value>Exchange rate between trading and accout price.</value>
        [DataMember(Name = "exchange_rate", EmitDefaultValue = false)]
        public decimal ExchangeRate { get; set; }

        /// <summary>
        ///     International Securities Identification Number.
        /// </summary>
        /// <value>International Securities Identification Number.</value>
        [DataMember(Name = "isin", EmitDefaultValue = false)]
        public string Isin { get; set; }

        /// <summary>
        ///     Market the security is traded on.
        /// </summary>
        /// <value>Market the security is traded on.</value>
        [DataMember(Name = "market", EmitDefaultValue = false)]
        public string Market { get; set; }

        /// <summary>
        ///     Internal modification timestamp.
        /// </summary>
        /// <value>Internal modification timestamp.</value>
        [DataMember(Name = "modified_at", EmitDefaultValue = false)]
        public string ModifiedAt { get; set; }

        /// <summary>
        ///     Name of the security.
        /// </summary>
        /// <value>Name of the security.</value>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        ///     Trading price.
        /// </summary>
        /// <value>Trading price.</value>
        [DataMember(Name = "price", EmitDefaultValue = false)]
        public decimal Price { get; set; }

        /// <summary>
        ///     Alphabetic currency code of the price.
        /// </summary>
        /// <value>Alphabetic currency code of the price.</value>
        [DataMember(Name = "price_currency", EmitDefaultValue = false)]
        public string PriceCurrency { get; set; }

        /// <summary>
        ///     Purchase price.
        /// </summary>
        /// <value>Purchase price.</value>
        [DataMember(Name = "purchase_price", EmitDefaultValue = false)]
        public decimal PurchasePrice { get; set; }

        /// <summary>
        ///     Alphabetic currency code of the purchase price.
        /// </summary>
        /// <value>Alphabetic currency code of the purchase price.</value>
        [DataMember(Name = "purchase_price_currency", EmitDefaultValue = false)]
        public string PurchasePriceCurrency { get; set; }

        /// <summary>
        ///     Internal figo security ID.
        /// </summary>
        /// <value>Internal figo security ID.</value>
        [DataMember(Name = "security_id", EmitDefaultValue = false)]
        public string SecurityId { get; set; }

        /// <summary>
        ///     Trading timestamp.
        /// </summary>
        /// <value>Trading timestamp.</value>
        [DataMember(Name = "trade_timestamp", EmitDefaultValue = false)]
        public string TradeTimestamp { get; set; }

        /// <summary>
        ///     Domestic security identification number.
        /// </summary>
        /// <value>Domestic security identification number.</value>
        [DataMember(Name = "wkn", EmitDefaultValue = false)]
        public string Wkn { get; set; }

        /// <summary>
        ///     Quantity in stock.
        /// </summary>
        /// <value>Quantity in stock.</value>
        [DataMember(Name = "quantity", EmitDefaultValue = false)]
        public decimal Quantity { get; set; }

        /// <summary>
        ///     Returns true if Security instances are equal
        /// </summary>
        /// <param name="input">Instance of Security to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Security input)
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
                    this.AmountOriginalCurrency == input.AmountOriginalCurrency ||
                    this.AmountOriginalCurrency.Equals(input.AmountOriginalCurrency)
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
                    this.ExchangeRate == input.ExchangeRate ||
                    this.ExchangeRate.Equals(input.ExchangeRate)
                ) &&
                (
                    this.Isin == input.Isin ||
                    this.Isin != null &&
                    this.Isin.Equals(input.Isin)
                ) &&
                (
                    this.Market == input.Market ||
                    this.Market != null &&
                    this.Market.Equals(input.Market)
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
                    this.Price == input.Price ||
                    this.Price.Equals(input.Price)
                ) &&
                (
                    this.PriceCurrency == input.PriceCurrency ||
                    this.PriceCurrency != null &&
                    this.PriceCurrency.Equals(input.PriceCurrency)
                ) &&
                (
                    this.PurchasePrice == input.PurchasePrice ||
                    this.PurchasePrice.Equals(input.PurchasePrice)
                ) &&
                (
                    this.PurchasePriceCurrency == input.PurchasePriceCurrency ||
                    this.PurchasePriceCurrency != null &&
                    this.PurchasePriceCurrency.Equals(input.PurchasePriceCurrency)
                ) &&
                (
                    this.SecurityId == input.SecurityId ||
                    this.SecurityId != null &&
                    this.SecurityId.Equals(input.SecurityId)
                ) &&
                (
                    this.TradeTimestamp == input.TradeTimestamp ||
                    this.TradeTimestamp != null &&
                    this.TradeTimestamp.Equals(input.TradeTimestamp)
                ) &&
                (
                    this.Wkn == input.Wkn ||
                    this.Wkn != null &&
                    this.Wkn.Equals(input.Wkn)
                ) &&
                (
                    this.Quantity == input.Quantity ||
                    this.Quantity.Equals(input.Quantity)
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
            sb.Append("class Security {\n");
            sb.Append("  AccountId: ").Append(this.AccountId).Append("\n");
            sb.Append("  Amount: ").Append(this.Amount).Append("\n");
            sb.Append("  AmountOriginalCurrency: ").Append(this.AmountOriginalCurrency).Append("\n");
            sb.Append("  CreatedAt: ").Append(this.CreatedAt).Append("\n");
            sb.Append("  Currency: ").Append(this.Currency).Append("\n");
            sb.Append("  ExchangeRate: ").Append(this.ExchangeRate).Append("\n");
            sb.Append("  Isin: ").Append(this.Isin).Append("\n");
            sb.Append("  Market: ").Append(this.Market).Append("\n");
            sb.Append("  ModifiedAt: ").Append(this.ModifiedAt).Append("\n");
            sb.Append("  Name: ").Append(this.Name).Append("\n");
            sb.Append("  Price: ").Append(this.Price).Append("\n");
            sb.Append("  PriceCurrency: ").Append(this.PriceCurrency).Append("\n");
            sb.Append("  PurchasePrice: ").Append(this.PurchasePrice).Append("\n");
            sb.Append("  PurchasePriceCurrency: ").Append(this.PurchasePriceCurrency).Append("\n");
            sb.Append("  SecurityId: ").Append(this.SecurityId).Append("\n");
            sb.Append("  TradeTimestamp: ").Append(this.TradeTimestamp).Append("\n");
            sb.Append("  Wkn: ").Append(this.Wkn).Append("\n");
            sb.Append("  Quantity: ").Append(this.Quantity).Append("\n");
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
            return this.Equals(input as Security);
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
                hashCode = hashCode * 59 + this.AmountOriginalCurrency.GetHashCode();
                if (this.CreatedAt != null)
                {
                    hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                }

                if (this.Currency != null)
                {
                    hashCode = hashCode * 59 + this.Currency.GetHashCode();
                }

                hashCode = hashCode * 59 + this.ExchangeRate.GetHashCode();
                if (this.Isin != null)
                {
                    hashCode = hashCode * 59 + this.Isin.GetHashCode();
                }

                if (this.Market != null)
                {
                    hashCode = hashCode * 59 + this.Market.GetHashCode();
                }

                if (this.ModifiedAt != null)
                {
                    hashCode = hashCode * 59 + this.ModifiedAt.GetHashCode();
                }

                if (this.Name != null)
                {
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                }

                hashCode = hashCode * 59 + this.Price.GetHashCode();
                if (this.PriceCurrency != null)
                {
                    hashCode = hashCode * 59 + this.PriceCurrency.GetHashCode();
                }

                hashCode = hashCode * 59 + this.PurchasePrice.GetHashCode();
                if (this.PurchasePriceCurrency != null)
                {
                    hashCode = hashCode * 59 + this.PurchasePriceCurrency.GetHashCode();
                }

                if (this.SecurityId != null)
                {
                    hashCode = hashCode * 59 + this.SecurityId.GetHashCode();
                }

                if (this.TradeTimestamp != null)
                {
                    hashCode = hashCode * 59 + this.TradeTimestamp.GetHashCode();
                }

                if (this.Wkn != null)
                {
                    hashCode = hashCode * 59 + this.Wkn.GetHashCode();
                }

                hashCode = hashCode * 59 + this.Quantity.GetHashCode();
                return hashCode;
            }
        }
    }
}