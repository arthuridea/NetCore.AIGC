using LLMService.Shared.Authentication.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;

namespace LLMService.Shared.Authentication.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Net.Http.DelegatingHandler" />
    public class OpenAIAuthenticationHandler : DelegatingHandler
    {
        /// <summary>
        /// The API configuration
        /// </summary>
        private readonly OpenAIBackendServiceConfig _apiConfig;
        /// <summary>
        /// The access control HTTP client
        /// </summary>
        private readonly HttpClient _accessControlHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIAuthenticationHandler" /> class.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="accessControlHttpClient">The access control HTTP client.</param>
        public OpenAIAuthenticationHandler(
            IOptionsSnapshot<OpenAIBackendServiceConfig> option,
            HttpClient accessControlHttpClient)
        {
            _apiConfig = option?.Value ?? new OpenAIBackendServiceConfig();
            _accessControlHttpClient = accessControlHttpClient;


            if (_accessControlHttpClient.BaseAddress == null)
            {
                throw new AuthenticationHandlerException($"{nameof(HttpClient.BaseAddress)} should be set to Identity Server url");
            }

            if (!(bool)_accessControlHttpClient.BaseAddress?.AbsoluteUri.EndsWith('/'))
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
            HttpResponseMessage response = new(HttpStatusCode.Unauthorized);
            try
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiConfig.APIKey);
                
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                response.ReasonPhrase = $"Request failed";
                response.Content = new StringContent($"{ex.Message}.");
                response.StatusCode = HttpStatusCode.InternalServerError;
                return await Task.FromResult(response);
            }

        }
    }
}
