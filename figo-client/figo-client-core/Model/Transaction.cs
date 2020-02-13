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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Figo.Client.Core.Model
{
    /// <summary>
    ///     Transaction
    /// </summary>
    [DataContract]
    public class Transaction : IEquatable<Transaction>, IValidatableObject
    {
        /// <summary>
        ///     Alphabetic currency code of the amount.
        /// </summary>
        /// <value>Alphabetic currency code of the amount.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CurrencyEnum
        {
            /// <summary>
            ///     Enum EUR for value: EUR
            /// </summary>
            [EnumMember(Value = "EUR")] EUR = 1
        }

        /// <summary>
        ///     Transaction type.
        /// </summary>
        /// <value>Transaction type.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum TypeEnum
        {
            /// <summary>
            ///     Enum Transfer for value: Empty
            /// </summary>
            [EnumMember(Value = "")] Empty = -1,

            /// <summary>
            ///     Enum Transfer for value: Unknown
            /// </summary>
            [EnumMember(Value = "Unknown")] Unknown = 0,
            /// <summary>
            ///     Enum Chargesorinterest for value: Charges or interest
            /// </summary>
            [EnumMember(Value = "Charges or interest")]
            Chargesorinterest = 1,

            /// <summary>
            ///     Enum Directdebit for value: Direct debit
            /// </summary>
            [EnumMember(Value = "Direct debit")] Directdebit = 2,

            /// <summary>
            ///     Enum GeldKarte for value: GeldKarte
            /// </summary>
            [EnumMember(Value = "GeldKarte")] GeldKarte = 3,

            /// <summary>
            ///     Enum Salaryorrent for value: Salary or rent
            /// </summary>
            [EnumMember(Value = "Salary or rent")] Salaryorrent = 4,

            /// <summary>
            ///     Enum Standingorder for value: Standing order
            /// </summary>
            [EnumMember(Value = "Standing order")] Standingorder = 5,

            /// <summary>
            ///     Enum Transfer for value: Transfer
            /// </summary>
            [EnumMember(Value = "Transfer")] Transfer = 6,
            [EnumMember(Value = "Versicherung")] Insurance = 7,
            [EnumMember(Value = "Kleidung")] Clothing = 8,
            [EnumMember(Value = "Sparen")] Savings = 9,
            [EnumMember(Value = "Lebensmittel")] Food = 10,
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Transaction" /> class.
        /// </summary>
        /// <param name="accountId">Internal figo account ID..</param>
        /// <param name="accountNumber">Domestic account identifier..</param>
        /// <param name="additionalInfo">additionalInfo.</param>
        /// <param name="amount">Amount of transaction..</param>
        /// <param name="bankCode">Domestic bank code..</param>
        /// <param name="bankName">Name of the bank or financial provider..</param>
        /// <param name="booked">Indicates if the transaction has been settled..</param>
        /// <param name="bookedAt">The date on which the transaction was booked..</param>
        /// <param name="bookingKey">*HBCI*: A key that indicates the purpose of a transaction..</param>
        /// <param name="bookingText">Booking text..</param>
        /// <param name="bic">The SWIFT Bank Identifier Code..</param>
        /// <param name="categories">List of categories..</param>
        /// <param name="createdAt">The date on which this transaction was added to the RegShield.</param>
        /// <param name="currency">Alphabetic currency code of the amount..</param>
        /// <param name="endToEndReference">
        ///     *HBCI*: A reference that can be filled by the payer of transaction, e.g. with your
        ///     customer number..
        /// </param>
        /// <param name="iban">The IBAN of the recipient or originator..</param>
        /// <param name="mandateReference">
        ///     *HBCI*: SEPA mandate reference (for SEPA direct debits), must be unique in combination
        ///     with creditor_id.
        /// </param>
        /// <param name="modifiedAt">Internal modification timestamp..</param>
        /// <param name="name">Name of originator or recipient..</param>
        /// <param name="primaNotaNumber">*HBCI*: Bank-internal number to group and identify transactions..</param>
        /// <param name="purpose">Purpose of transaction..</param>
        /// <param name="sepaPurposeCode">sepaPurposeCode.</param>
        /// <param name="sepaRemittanceInfo">*HBCI*: Pure purpose text without other SEPA fields..</param>
        /// <param name="transactionId">Internal figo transaction ID..</param>
        /// <param name="transactionCode">Transaction type as DTA Tx Key code..</param>
        /// <param name="type">Transaction type..</param>
        /// <param name="valueDate">The date on which the transaction was settled..</param>
        public Transaction(string accountId = default, string accountNumber = default, TransactionAdditionalInfo additionalInfo = default,
                           decimal amount = default, string bankCode = default, string bankName = default, bool booked = default, string bookedAt = default,
                           string bookingKey = default, string bookingText = default, string bic = default, List<TransactionCategories> categories = default,
                           string createdAt = default, CurrencyEnum? currency = default, string endToEndReference = default, string iban = default,
                           string mandateReference = default, string modifiedAt = default, string name = default, string primaNotaNumber = default,
                           string purpose = default, string sepaPurposeCode = default, string sepaRemittanceInfo = default, string transactionId = default,
                           int transactionCode = default, TypeEnum? type = default)
        {
            this.AccountId = accountId;
            this.AccountNumber = accountNumber;
            this.AdditionalInfo = additionalInfo;
            this.Amount = amount;
            this.BankCode = bankCode;
            this.BankName = bankName;
            this.Booked = booked;
            this.BookedAt = bookedAt;
            this.BookingKey = bookingKey;
            this.BookingText = bookingText;
            this.Bic = bic;
            this.Categories = categories;
            this.CreatedAt = createdAt;
            this.Currency = currency;
            this.EndToEndReference = endToEndReference;
            this.Iban = iban;
            this.MandateReference = mandateReference;
            this.ModifiedAt = modifiedAt;
            this.Name = name;
            this.PrimaNotaNumber = primaNotaNumber;
            this.Purpose = purpose;
            this.SepaPurposeCode = sepaPurposeCode;
            this.SepaRemittanceInfo = sepaRemittanceInfo;
            this.TransactionId = transactionId;
            this.TransactionCode = transactionCode;
            this.Type = type;
        }

        /// <summary>
        ///     Alphabetic currency code of the amount.
        /// </summary>
        /// <value>Alphabetic currency code of the amount.</value>
        [DataMember(Name = "currency", EmitDefaultValue = false)]
        public CurrencyEnum? Currency { get; set; }

        /// <summary>
        ///     Transaction type.
        /// </summary>
        /// <value>Transaction type.</value>
        [DataMember(Name = "type", EmitDefaultValue = false)]
        public TypeEnum? Type { get; set; }

        /// <summary>
        ///     Internal figo account ID.
        /// </summary>
        /// <value>Internal figo account ID.</value>
        [DataMember(Name = "account_id", EmitDefaultValue = false)]
        public string AccountId { get; set; }

        /// <summary>
        ///     Domestic account identifier.
        /// </summary>
        /// <value>Domestic account identifier.</value>
        [DataMember(Name = "account_number", EmitDefaultValue = false)]
        public string AccountNumber { get; set; }
        
        [DataMember(Name = "settled_at", EmitDefaultValue = false)]
        public string SettledAt { get; set; }

        /// <summary>
        ///     Gets or Sets AdditionalInfo
        /// </summary>
        [DataMember(Name = "additional_info", EmitDefaultValue = false)]
        public TransactionAdditionalInfo AdditionalInfo { get; set; }

        /// <summary>
        ///     Amount of transaction.
        /// </summary>
        /// <value>Amount of transaction.</value>
        [DataMember(Name = "amount", EmitDefaultValue = false)]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Domestic bank code.
        /// </summary>
        /// <value>Domestic bank code.</value>
        [DataMember(Name = "bank_code", EmitDefaultValue = false)]
        public string BankCode { get; set; }

        /// <summary>
        ///     Name of the bank or financial provider.
        /// </summary>
        /// <value>Name of the bank or financial provider.</value>
        [DataMember(Name = "bank_name", EmitDefaultValue = false)]
        public string BankName { get; set; }

        /// <summary>
        ///     Indicates if the transaction has been settled.
        /// </summary>
        /// <value>Indicates if the transaction has been settled.</value>
        [DataMember(Name = "booked", EmitDefaultValue = false)]
        public bool Booked { get; set; }

        /// <summary>
        ///     The date on which the transaction was booked.
        /// </summary>
        /// <value>The date on which the transaction was booked.</value>
        [DataMember(Name = "booked_at", EmitDefaultValue = false)]
        public string BookedAt { get; set; }

        /// <summary>
        ///     *HBCI*: A key that indicates the purpose of a transaction.
        /// </summary>
        /// <value>*HBCI*: A key that indicates the purpose of a transaction.</value>
        [DataMember(Name = "booking_key", EmitDefaultValue = false)]
        public string BookingKey { get; set; }

        /// <summary>
        ///     Booking text.
        /// </summary>
        /// <value>Booking text.</value>
        [DataMember(Name = "booking_text", EmitDefaultValue = false)]
        public string BookingText { get; set; }

        /// <summary>
        ///     The SWIFT Bank Identifier Code.
        /// </summary>
        /// <value>The SWIFT Bank Identifier Code.</value>
        [DataMember(Name = "bic", EmitDefaultValue = false)]
        public string Bic { get; set; }

        /// <summary>
        ///     List of categories.
        /// </summary>
        /// <value>List of categories.</value>
        [DataMember(Name = "categories", EmitDefaultValue = false)]
        public List<TransactionCategories> Categories { get; set; }

        /// <summary>
        ///     The date on which this transaction was added to the RegShield
        /// </summary>
        /// <value>The date on which this transaction was added to the RegShield</value>
        [DataMember(Name = "created_at", EmitDefaultValue = false)]
        public string CreatedAt { get; set; }

        /// <summary>
        ///     *HBCI*: A reference that can be filled by the payer of transaction, e.g. with your customer number.
        /// </summary>
        /// <value>*HBCI*: A reference that can be filled by the payer of transaction, e.g. with your customer number.</value>
        [DataMember(Name = "end_to_end_reference", EmitDefaultValue = false)]
        public string EndToEndReference { get; set; }

        /// <summary>
        ///     The IBAN of the recipient or originator.
        /// </summary>
        /// <value>The IBAN of the recipient or originator.</value>
        [DataMember(Name = "iban", EmitDefaultValue = false)]
        public string Iban { get; set; }

        /// <summary>
        ///     *HBCI*: SEPA mandate reference (for SEPA direct debits), must be unique in combination with creditor_id
        /// </summary>
        /// <value>*HBCI*: SEPA mandate reference (for SEPA direct debits), must be unique in combination with creditor_id</value>
        [DataMember(Name = "mandate_reference", EmitDefaultValue = false)]
        public string MandateReference { get; set; }

        /// <summary>
        ///     Internal modification timestamp.
        /// </summary>
        /// <value>Internal modification timestamp.</value>
        [DataMember(Name = "modified_at", EmitDefaultValue = false)]
        public string ModifiedAt { get; set; }

        /// <summary>
        ///     Name of originator or recipient.
        /// </summary>
        /// <value>Name of originator or recipient.</value>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        public string Name { get; set; }

        /// <summary>
        ///     *HBCI*: Bank-internal number to group and identify transactions.
        /// </summary>
        /// <value>*HBCI*: Bank-internal number to group and identify transactions.</value>
        [DataMember(Name = "prima_nota_number", EmitDefaultValue = false)]
        public string PrimaNotaNumber { get; set; }

        /// <summary>
        ///     Purpose of transaction.
        /// </summary>
        /// <value>Purpose of transaction.</value>
        [DataMember(Name = "purpose", EmitDefaultValue = false)]
        public string Purpose { get; set; }

        /// <summary>
        ///     Gets or Sets SepaPurposeCode
        /// </summary>
        [DataMember(Name = "sepa_purpose_code", EmitDefaultValue = false)]
        public string SepaPurposeCode { get; set; }

        /// <summary>
        ///     *HBCI*: Pure purpose text without other SEPA fields.
        /// </summary>
        /// <value>*HBCI*: Pure purpose text without other SEPA fields.</value>
        [DataMember(Name = "sepa_remittance_info", EmitDefaultValue = false)]
        public string SepaRemittanceInfo { get; set; }

        /// <summary>
        ///     Internal figo transaction ID.
        /// </summary>
        /// <value>Internal figo transaction ID.</value>
        [DataMember(Name = "transaction_id", EmitDefaultValue = false)]
        public string TransactionId { get; set; }

        /// <summary>
        ///     Transaction type as DTA Tx Key code.
        /// </summary>
        /// <value>Transaction type as DTA Tx Key code.</value>
        [DataMember(Name = "transaction_code", EmitDefaultValue = false)]
        public int TransactionCode { get; set; }

        /// <summary>
        ///     Returns true if Transaction instances are equal
        /// </summary>
        /// <param name="input">Instance of Transaction to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Transaction input)
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
                    this.AdditionalInfo == input.AdditionalInfo ||
                    this.AdditionalInfo != null &&
                    this.AdditionalInfo.Equals(input.AdditionalInfo)
                ) &&
                (
                    this.Amount == input.Amount ||
                    this.Amount.Equals(input.Amount)
                ) &&
                (
                    this.BankCode == input.BankCode ||
                    this.BankCode != null &&
                    this.BankCode.Equals(input.BankCode)
                ) &&
                (
                    this.BankName == input.BankName ||
                    this.BankName != null &&
                    this.BankName.Equals(input.BankName)
                ) &&
                (
                    this.Booked == input.Booked ||
                    this.Booked.Equals(input.Booked)
                ) &&
                (
                    this.BookedAt == input.BookedAt ||
                    this.BookedAt != null &&
                    this.BookedAt.Equals(input.BookedAt)
                ) &&
                (
                    this.BookingKey == input.BookingKey ||
                    this.BookingKey != null &&
                    this.BookingKey.Equals(input.BookingKey)
                ) &&
                (
                    this.BookingText == input.BookingText ||
                    this.BookingText != null &&
                    this.BookingText.Equals(input.BookingText)
                ) &&
                (
                    this.Bic == input.Bic ||
                    this.Bic != null &&
                    this.Bic.Equals(input.Bic)
                ) &&
                (
                    this.Categories == input.Categories ||
                    this.Categories != null &&
                    input.Categories != null &&
                    this.Categories.SequenceEqual(input.Categories)
                ) &&
                (
                    this.CreatedAt == input.CreatedAt ||
                    this.CreatedAt != null &&
                    this.CreatedAt.Equals(input.CreatedAt)
                ) &&
                (
                    this.Currency == input.Currency ||
                    this.Currency.Equals(input.Currency)
                ) &&
                (
                    this.EndToEndReference == input.EndToEndReference ||
                    this.EndToEndReference != null &&
                    this.EndToEndReference.Equals(input.EndToEndReference)
                ) &&
                (
                    this.Iban == input.Iban ||
                    this.Iban != null &&
                    this.Iban.Equals(input.Iban)
                ) &&
                (
                    this.MandateReference == input.MandateReference ||
                    this.MandateReference != null &&
                    this.MandateReference.Equals(input.MandateReference)
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
                    this.PrimaNotaNumber == input.PrimaNotaNumber ||
                    this.PrimaNotaNumber != null &&
                    this.PrimaNotaNumber.Equals(input.PrimaNotaNumber)
                ) &&
                (
                    this.Purpose == input.Purpose ||
                    this.Purpose != null &&
                    this.Purpose.Equals(input.Purpose)
                ) &&
                (
                    this.SepaPurposeCode == input.SepaPurposeCode ||
                    this.SepaPurposeCode != null &&
                    this.SepaPurposeCode.Equals(input.SepaPurposeCode)
                ) &&
                (
                    this.SepaRemittanceInfo == input.SepaRemittanceInfo ||
                    this.SepaRemittanceInfo != null &&
                    this.SepaRemittanceInfo.Equals(input.SepaRemittanceInfo)
                ) &&
                (
                    this.TransactionId == input.TransactionId ||
                    this.TransactionId != null &&
                    this.TransactionId.Equals(input.TransactionId)
                ) &&
                (
                    this.TransactionCode == input.TransactionCode ||
                    this.TransactionCode.Equals(input.TransactionCode)
                ) &&
                (
                    this.Type == input.Type ||
                    this.Type.Equals(input.Type)
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
            sb.Append("class Transaction {\n");
            sb.Append("  AccountId: ").Append(this.AccountId).Append("\n");
            sb.Append("  AccountNumber: ").Append(this.AccountNumber).Append("\n");
            sb.Append("  AdditionalInfo: ").Append(this.AdditionalInfo).Append("\n");
            sb.Append("  Amount: ").Append(this.Amount).Append("\n");
            sb.Append("  BankCode: ").Append(this.BankCode).Append("\n");
            sb.Append("  BankName: ").Append(this.BankName).Append("\n");
            sb.Append("  Booked: ").Append(this.Booked).Append("\n");
            sb.Append("  BookingDate: ").Append(this.BookedAt).Append("\n");
            sb.Append("  BookingKey: ").Append(this.BookingKey).Append("\n");
            sb.Append("  BookingText: ").Append(this.BookingText).Append("\n");
            sb.Append("  Bic: ").Append(this.Bic).Append("\n");
            sb.Append("  Categories: ").Append(this.Categories).Append("\n");
            sb.Append("  CreatedAt: ").Append(this.CreatedAt).Append("\n");
            sb.Append("  Currency: ").Append(this.Currency).Append("\n");
            sb.Append("  EndToEndReference: ").Append(this.EndToEndReference).Append("\n");
            sb.Append("  Iban: ").Append(this.Iban).Append("\n");
            sb.Append("  MandateReference: ").Append(this.MandateReference).Append("\n");
            sb.Append("  ModifiedAt: ").Append(this.ModifiedAt).Append("\n");
            sb.Append("  Name: ").Append(this.Name).Append("\n");
            sb.Append("  PrimaNotaNumber: ").Append(this.PrimaNotaNumber).Append("\n");
            sb.Append("  Purpose: ").Append(this.Purpose).Append("\n");
            sb.Append("  SepaPurposeCode: ").Append(this.SepaPurposeCode).Append("\n");
            sb.Append("  SepaRemittanceInfo: ").Append(this.SepaRemittanceInfo).Append("\n");
            sb.Append("  TransactionId: ").Append(this.TransactionId).Append("\n");
            sb.Append("  TransactionCode: ").Append(this.TransactionCode).Append("\n");
            sb.Append("  Type: ").Append(this.Type).Append("\n");
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
            return this.Equals(input as Transaction);
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

                if (this.AdditionalInfo != null)
                {
                    hashCode = hashCode * 59 + this.AdditionalInfo.GetHashCode();
                }

                hashCode = hashCode * 59 + this.Amount.GetHashCode();
                if (this.BankCode != null)
                {
                    hashCode = hashCode * 59 + this.BankCode.GetHashCode();
                }

                if (this.BankName != null)
                {
                    hashCode = hashCode * 59 + this.BankName.GetHashCode();
                }

                hashCode = hashCode * 59 + this.Booked.GetHashCode();
                if (this.BookedAt != null)
                {
                    hashCode = hashCode * 59 + this.BookedAt.GetHashCode();
                }

                if (this.BookingKey != null)
                {
                    hashCode = hashCode * 59 + this.BookingKey.GetHashCode();
                }

                if (this.BookingText != null)
                {
                    hashCode = hashCode * 59 + this.BookingText.GetHashCode();
                }

                if (this.Bic != null)
                {
                    hashCode = hashCode * 59 + this.Bic.GetHashCode();
                }

                if (this.Categories != null)
                {
                    hashCode = hashCode * 59 + this.Categories.GetHashCode();
                }

                if (this.CreatedAt != null)
                {
                    hashCode = hashCode * 59 + this.CreatedAt.GetHashCode();
                }

                hashCode = hashCode * 59 + this.Currency.GetHashCode();
                if (this.EndToEndReference != null)
                {
                    hashCode = hashCode * 59 + this.EndToEndReference.GetHashCode();
                }

                if (this.Iban != null)
                {
                    hashCode = hashCode * 59 + this.Iban.GetHashCode();
                }

                if (this.MandateReference != null)
                {
                    hashCode = hashCode * 59 + this.MandateReference.GetHashCode();
                }

                if (this.ModifiedAt != null)
                {
                    hashCode = hashCode * 59 + this.ModifiedAt.GetHashCode();
                }

                if (this.Name != null)
                {
                    hashCode = hashCode * 59 + this.Name.GetHashCode();
                }

                if (this.PrimaNotaNumber != null)
                {
                    hashCode = hashCode * 59 + this.PrimaNotaNumber.GetHashCode();
                }

                if (this.Purpose != null)
                {
                    hashCode = hashCode * 59 + this.Purpose.GetHashCode();
                }

                if (this.SepaPurposeCode != null)
                {
                    hashCode = hashCode * 59 + this.SepaPurposeCode.GetHashCode();
                }

                if (this.SepaRemittanceInfo != null)
                {
                    hashCode = hashCode * 59 + this.SepaRemittanceInfo.GetHashCode();
                }

                if (this.TransactionId != null)
                {
                    hashCode = hashCode * 59 + this.TransactionId.GetHashCode();
                }

                hashCode = hashCode * 59 + this.TransactionCode.GetHashCode();
                hashCode = hashCode * 59 + this.Type.GetHashCode();

                return hashCode;
            }
        }
    }
}