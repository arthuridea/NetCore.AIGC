using System.Text.Json.Serialization;

namespace LLMService.Shared.Authentication.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the scheme.
        /// </summary>
        /// <value>
        /// The scheme.
        /// </value>
        [JsonPropertyName("token_type")]
        public string Scheme { get; set; } = "Bearer";

        /// <summary>
        /// Gets or sets the expiration in seconds.
        /// </summary>
        /// <value>
        /// The expiration in seconds.
        /// </value>
        [JsonPropertyName("expires_in")]
        public long ExpirationInSeconds { get; set; }
    }
}
