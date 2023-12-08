namespace LLMServiceHub.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the name of the API.
        /// </summary>
        /// <value>
        /// The name of the API.
        /// </value>
        public string ApiName { get; set; }
        /// <summary>
        /// Gets or sets the identity server base URL.
        /// </summary>
        /// <value>
        /// The identity server base URL.
        /// </value>
        public string IdentityServerBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the API base URL.
        /// </summary>
        /// <value>
        /// The API base URL.
        /// </value>
        public string ApiBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the oidc swagger UI client identifier.
        /// </summary>
        /// <value>
        /// The oidc swagger UI client identifier.
        /// </value>
        public string OidcSwaggerUIClientId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [require HTTPS metadata].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require HTTPS metadata]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireHttpsMetadata { get; set; }

        /// <summary>
        /// Gets or sets the name of the oidc API.
        /// </summary>
        /// <value>
        /// The name of the oidc API.
        /// </value>
        public string OidcApiName { get; set; }

        /// <summary>
        /// Gets or sets the administration role.
        /// </summary>
        /// <value>
        /// The administration role.
        /// </value>
        public string AdministrationRole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [cors allow any origin].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cors allow any origin]; otherwise, <c>false</c>.
        /// </value>
        public bool CorsAllowAnyOrigin { get; set; }

        /// <summary>
        /// Gets or sets the cors allow origins.
        /// </summary>
        /// <value>
        /// The cors allow origins.
        /// </value>
        public string[] CorsAllowOrigins { get; set; }
    }
}
