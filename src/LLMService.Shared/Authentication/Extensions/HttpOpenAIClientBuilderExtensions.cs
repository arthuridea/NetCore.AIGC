using LLMService.Shared;
using LLMService.Shared.Authentication.Models;
using LLMService.Shared.Client;
using LLMService.Shared.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ***************************************************************************************
// DO NOT CHANGE THE NAMESPACE! It is .Net Core convention for configuration extensions. *
// ***************************************************************************************
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpOpenAIClientBuilderExtensions
    {
        /// <summary>
        /// Adds the open ai API authentication.
        /// </summary>
        /// <typeparam name="TAuthenticationHandler">The type of the authentication handler.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="identityAuthority">The identity authority.</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddOpenAIApiAuthentication<TAuthenticationHandler>(
            this IHttpClientBuilder builder,
            string identityAuthority)
            where TAuthenticationHandler : DelegatingHandler
        {
            builder.AddHttpMessageHandler(provider => CreateDelegatingHandler<TAuthenticationHandler>(provider, identityAuthority));

            return builder;
        }


        /// <summary>
        /// Creates the delegating handler.
        /// </summary>
        /// <typeparam name="TAuthenticationHandler">The type of the authentication handler.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <param name="identityAuthority">The identity authority.</param>
        /// <returns></returns>
        private static TAuthenticationHandler CreateDelegatingHandler<TAuthenticationHandler>(
            IServiceProvider provider,
            string identityAuthority)
            where TAuthenticationHandler : DelegatingHandler
        {
            var httpClient = CreateHttpClient(provider, identityAuthority);

            var option = provider.GetRequiredService<IOptionsSnapshot<OpenAIBackendServiceConfig>>();

            var constructor = new object[]
            {
                option,
                httpClient,
            };

            var handler = (TAuthenticationHandler)Activator.CreateInstance(typeof(TAuthenticationHandler), constructor);

            return handler;
        }

        /// <summary>
        /// Creates the HTTP client.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="identityAuthority">The identity authority.</param>
        /// <returns></returns>
        private static HttpClient CreateHttpClient(IServiceProvider provider, string identityAuthority)
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(identityAuthority);

            return httpClient;
        }
    }
}
