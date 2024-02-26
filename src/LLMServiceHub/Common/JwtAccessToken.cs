using System.Text.Json.Serialization;

namespace LLMServiceHub.Common
{
    /// <summary>
    /// access token 
    /// </summary>
    public class JwtAccessToken
    {
        /// <summary>
        /// jwt access token
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// refresh token
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// issued timestamp
        /// </summary>
        public long IssuedAt { get; set; }
        /// <summary>
        /// expires in seconds
        /// </summary>
        public long ExpiresInSeconds { get; set; } = 7200;
        /// <summary>
        /// expires timestamp
        /// </summary>
        public long ExpiresAt => IssuedAt + ExpiresInSeconds;
        /// <summary>
        /// bearer authorization header value
        /// </summary>
        [JsonIgnore]
        public string AuthorizationBearerHeader => $"Bearer {AccessToken}";
        /// <summary>
        /// expire flag
        /// </summary>
        [JsonIgnore]
        public bool IsExpired => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds() > ExpiresAt;
    }
}
