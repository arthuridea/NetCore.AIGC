using System.Collections.Concurrent;

namespace LLMServiceHub.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="LLMServiceHub.Authentication.IAccessTokensCacheManager" />
    public class AccessTokensCacheManager : IAccessTokensCacheManager
    {
        /// <summary>
        /// The cache
        /// </summary>
        private readonly ConcurrentDictionary<string, AccessTokenCacheEntry>
            _cache = new();

        /// <summary>
        /// Adds the or update token.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="accessToken">The access token.</param>
        public void AddOrUpdateToken(string clientId, TokenResponse accessToken)
        {
            var newToken = new AccessTokenCacheEntry(accessToken);
            _cache.TryRemove(clientId, out _);
            _cache.TryAdd(clientId, newToken);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public TokenResponse GetToken(string clientId)
        {
            _cache.TryGetValue(clientId, out var tokenCacheEntry);
            return tokenCacheEntry?.IsValid == true
                ? tokenCacheEntry.Token
                : null;
        }

        /// <summary>
        /// 
        /// </summary>
        private class AccessTokenCacheEntry
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AccessTokenCacheEntry"/> class.
            /// </summary>
            /// <param name="token">The token.</param>
            public AccessTokenCacheEntry(TokenResponse token)
            {
                Token = token;
                RefreshAfterDate = DateTime.UtcNow + TimeSpan.FromSeconds(token.ExpirationInSeconds / 2.0);
            }

            /// <summary>
            /// Gets the token.
            /// </summary>
            /// <value>
            /// The token.
            /// </value>
            public TokenResponse Token { get; }

            /// <summary>
            /// Gets the refresh after date.
            /// </summary>
            /// <value>
            /// The refresh after date.
            /// </value>
            private DateTime RefreshAfterDate { get; }
            /// <summary>
            /// Returns true if ... is valid.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
            /// </value>
            public bool IsValid => DateTime.UtcNow < RefreshAfterDate;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IAccessTokensCacheManager
    {
        /// <summary>
        /// Adds the or update token.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="accessToken">The access token.</param>
        void AddOrUpdateToken(string clientId, TokenResponse accessToken);
        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        TokenResponse GetToken(string clientId);
    }
}
