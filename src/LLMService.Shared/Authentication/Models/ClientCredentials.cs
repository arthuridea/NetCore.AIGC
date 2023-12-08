namespace LLMService.Shared.Authentication.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientCredentials
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
    }
}
