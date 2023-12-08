using LLMService.Shared.Authentication.Models;
using LLMService.Shared.Extensions;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

namespace LLMService.Shared.Authentication.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DelegatingHandler" />
    public class BaiduApiAuthenticationHandler: DelegatingHandler
    {
        /// <summary>
        /// The client credentials
        /// </summary>
        private readonly ClientCredentials _clientCredentials;
        /// <summary>
        /// The access control HTTP client
        /// </summary>
        private readonly HttpClient _accessControlHttpClient;
        /// <summary>
        /// The access tokens cache manager
        /// </summary>
        private readonly IAccessTokensCacheManager _accessTokensCacheManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduApiAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="accessTokensCacheManager">The access tokens cache manager.</param>
        /// <param name="clientCredentials">The client credentials.</param>
        /// <param name="identityAuthority">The identity authority.</param>
        public BaiduApiAuthenticationHandler(
            IAccessTokensCacheManager accessTokensCacheManager,
            ClientCredentials clientCredentials,
            string identityAuthority)
            : this(accessTokensCacheManager,
                clientCredentials,
                new HttpClient { BaseAddress = new Uri(identityAuthority) })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduApiAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="accessTokensCacheManager">The access tokens cache manager.</param>
        /// <param name="clientCredentials">The client credentials.</param>
        /// <param name="accessControlHttpClient">The access control HTTP client.</param>
        /// <exception cref="LLMServiceHub.Authentication.AuthenticationHandlerException"></exception>
        public BaiduApiAuthenticationHandler(
            IAccessTokensCacheManager accessTokensCacheManager,
            ClientCredentials clientCredentials,
            HttpClient accessControlHttpClient)
        {
            _accessTokensCacheManager = accessTokensCacheManager;
            _clientCredentials = clientCredentials;
            _accessControlHttpClient = accessControlHttpClient;

            if (_accessControlHttpClient.BaseAddress == null)
            {
                throw new AuthenticationHandlerException($"{nameof(HttpClient.BaseAddress)} should be set to Identity Server url");
            }

            if (!(bool)_accessControlHttpClient.BaseAddress?.AbsoluteUri.EndsWith("/"))
            {
                _accessControlHttpClient.BaseAddress = new Uri(_accessControlHttpClient.BaseAddress.AbsoluteUri + "/");
            }
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            try
            {
                var token = await GetToken();
                if(token == null)
                {
                    response.ReasonPhrase = "Invalid token request.";
                    return await Task.FromResult(response);
                }

                request.Headers.Authorization = new AuthenticationHeaderValue(token.Scheme, token.AccessToken);
                var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
                query.Add("access_token", token.AccessToken);
                request.RequestUri = new Uri(request.RequestUri + "?" + query.ToString());

                return await base.SendAsync(request, cancellationToken);
            }
            catch(AuthenticationHandlerException ex)
            {
                response.ReasonPhrase = "Invalid Token";
                response.Content = new StringContent(ex.Message);
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                response.ReasonPhrase = $"Request failed";
                response.Content = new StringContent($"{ex.Message}.");
                response.StatusCode = HttpStatusCode.InternalServerError;
                return await Task.FromResult(response);
            }

        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <returns></returns>
        private async Task<TokenResponse> GetToken()
        {
            var token = _accessTokensCacheManager.GetToken(_clientCredentials.ClientId);
            if (token == null)
            {
                token = await GetNewToken(_clientCredentials);
                _accessTokensCacheManager.AddOrUpdateToken(_clientCredentials.ClientId, token);
            }

            return token;
        }

        /// <summary>
        /// Gets the new token.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        /// <exception cref="LLMServiceHub.Authentication.AuthenticationHandlerException"></exception>
        private async Task<TokenResponse> GetNewToken(ClientCredentials credentials)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, _clientCredentials.TokenEndpoint))
            {
                request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", credentials.ClientId),
                    new KeyValuePair<string, string>("client_secret", credentials.ClientSecret),
                    new KeyValuePair<string, string>("scope", credentials.Scopes)
                });

                var response = await _accessControlHttpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var tokenResponse = await response.DeserializeAsync<TokenResponse>();
                    return tokenResponse;
                }

                var errorMessage = await GetErrorMessageAsync(response);
                throw new AuthenticationHandlerException(errorMessage);
            }
        }

        /// <summary>
        /// Gets the error message asynchronous.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private static async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
        {
            var baseErrorMessage =
                $"Error occured while trying to get access token from identity authority {response.RequestMessage.RequestUri}.";

            var errorMessage = baseErrorMessage;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorResponse = await response.DeserializeAsync<TokenErrorResponse>();
                errorMessage = $"{errorMessage} Error details: {errorResponse.Error}";
            }
            else
            {
                errorMessage = $"{errorMessage} Status code: {(int)response.StatusCode} - {response.StatusCode}";
            }

            return errorMessage;
        }
    }
}
