using System;
using System.Runtime.Serialization;
using Figo.Client.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Figo.Client.Models
{
    public class AccessTokenDto
    {
        /// <summary>
        ///     The type of the issued token.
        /// </summary>
        /// <value>The type of the issued token.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum TokenTypeEnum
        {
            /// <summary>
            ///     Enum Bearer for value: Bearer
            /// </summary>
            [EnumMember(Value = "Bearer")] Bearer = 1
        }

        /// <summary>
        ///     The access token issued by the RegShield.
        /// </summary>
        /// <value>The access token issued by the RegShield.</value>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        /// <summary>
        ///     The lifetime in seconds of the access token.
        /// </summary>
        /// <value>The lifetime in seconds of the access token.</value>
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }


        public bool IsValid
        {
            get
            {
                var expiresOn = this.Created.AddSeconds(this.ExpiresIn);
                return DateTime.UtcNow <= expiresOn;
            }
        }

        /// <summary>
        ///     The refresh token issued by the RegShield.
        /// </summary>
        /// <value>The refresh token issued by the RegShield.</value>
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        ///     The type of the issued token.
        /// </summary>
        /// <value>The type of the issued token.</value>
        [JsonProperty(PropertyName = "token_type")]
        public AccessToken.TokenTypeEnum? TokenType { get; set; }

        public static AccessTokenDto FromAccessTokenCreated(AccessToken dto)
        {
            return new AccessTokenDto
            {
                ExpiresIn = dto.ExpiresIn,
                RefreshToken = dto.RefreshToken,
                Created = DateTime.UtcNow,
                AccessToken = dto._AccessToken,
                TokenType = dto.TokenType
            };
        }
    }
}