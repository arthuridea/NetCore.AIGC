using LLMService.Shared.Models;

namespace LLMService.Shared.Authentication.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OAuth2BackendServiceConfig: IChatServiceOption
    {
        /// <summary>
        /// The client_id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// The client_secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the token endpoint.
        /// </summary>
        /// <value>
        /// The token endpoint.
        /// </value>
        public string TokenEndpoint { get; set; } = "connect/token";
        /// <summary>
        /// Required scopes separated by spaces.
        /// </summary>
        public string Scopes { get; set; } = string.Empty;

        /// <summary>
        /// Gets the name of the backend HTTP client.
        /// </summary>
        /// <value>
        /// The name of the backend HTTP client.
        /// </value>
        public string BackendHttpClientName { get; set; } = LLMServiceConsts.BaiduWenxinApiClientName;

        /// <summary>
        /// Gets a value indicating whether [support streaming].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [support streaming]; otherwise, <c>false</c>.
        /// </value>
        public bool SupportStreaming { get; set; } = true;

        /// <summary>
        /// Gets the backend stream end token pattern.
        /// </summary>
        /// <value>
        /// The backend stream end token pattern.
        /// </value>
        public string BackendStreamEndTokenPattern { get; set; } = "\"is_end\":true";
        /// <summary>
        /// Gets a value indicating whether this instance is backend stream end standalone.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is backend stream end standalone; otherwise, <c>false</c>.
        /// </value>
        public bool IsBackendStreamEndStandalone => false;
        /// <summary>
        /// Gets the backend stream prefix token.
        /// </summary>
        /// <value>
        /// The backend stream prefix token.
        /// </value>
        public string BackendStreamPrefixToken => "data:";

        /// <summary>
        /// Gets the sse push interval.
        /// </summary>
        /// <value>
        /// The sse push interval.
        /// </value>
        public int SSEPushInterval => 100;
    }
}
