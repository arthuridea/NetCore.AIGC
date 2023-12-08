using System;
using System.Net.Http;
using LLMService.Shared;
using LLMService.Shared.Authentication.Handlers;
using LLMService.Shared.Authentication.Models;
using LLMService.Shared.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ***************************************************************************************
// DO NOT CHANGE THE NAMESPACE! It is .Net Core convention for configuration extensions. *
// ***************************************************************************************
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpClientBuilderExtensions
    {
        /// <summary>
        /// Adds the baidu API authentication.
        /// </summary>
        /// <typeparam name="TAuthenticationHandler">The type of the authentication handler.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="credentialsProvider">The credentials provider.</param>
        /// <param name="identityAuthorityProvider">The identity authority provider.</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddBaiduApiAuthentication<TAuthenticationHandler>(
            this IServiceCollection services,
            Func<IServiceProvider, ClientCredentials> credentialsProvider,
            Func<IServiceProvider, string> identityAuthorityProvider)
            where TAuthenticationHandler : DelegatingHandler
        {
            services.TryAddSingleton<IAccessTokensCacheManager, AccessTokensCacheManager>();

            services.AddHttpClient();
            var httpClientBuilder = services.AddHttpClient<BaiduApiClient>((provider, client) =>
            {
                var identityAuthority = identityAuthorityProvider.Invoke(provider);
                client.BaseAddress = new Uri(identityAuthority);
            })
            .AddHttpMessageHandler(provider =>
            {
                var credentials = credentialsProvider.Invoke(provider);
                var identityAuthority = identityAuthorityProvider.Invoke(provider);

                return CreateDelegatingHandler<TAuthenticationHandler>(provider, credentials, identityAuthority);
            });

            return httpClientBuilder;
        }

        /// <summary>
        /// Adds the baidu API authentication.
        /// </summary>
        /// <typeparam name="TAuthenticationHandler">The type of the authentication handler.</typeparam>
        /// <param name="services">The services.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="identityAuthority">The identity authority.</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddBaiduApiAuthentication<TAuthenticationHandler>(
            this IServiceCollection services, 
            ClientCredentials credentials, 
            string identityAuthority)
            where TAuthenticationHandler : DelegatingHandler
        {
            services.TryAddSingleton<IAccessTokensCacheManager, AccessTokensCacheManager>();

            services.AddHttpClient();
            var httpClientBuilder = services.AddHttpClient<BaiduApiClient>(LLMServiceConsts.BaiduWenxinApiClientName)
            .AddHttpMessageHandler(provider => CreateDelegatingHandler<TAuthenticationHandler>(provider, credentials, identityAuthority));

            return httpClientBuilder;
        }


        /// <summary>
        /// Adds the baidu API authentication.
        /// </summary>
        /// <typeparam name="TAuthenticationHandler">The type of the authentication handler.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="credentialsProvider">The credentials provider.</param>
        /// <param name="identityAuthorityProvider">The identity authority provider.</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddBaiduApiAuthentication<TAuthenticationHandler>(this IHttpClientBuilder builder,
            Func<IServiceProvider, ClientCredentials> credentialsProvider,
            Func<IServiceProvider, string> identityAuthorityProvider)
            where TAuthenticationHandler : DelegatingHandler
        {
            builder.Services.TryAddSingleton<IAccessTokensCacheManager, AccessTokensCacheManager>();
            builder.AddHttpMessageHandler(provider =>
            {
                var credentials = credentialsProvider.Invoke(provider);
                var identityAuthority = identityAuthorityProvider.Invoke(provider);

                return CreateDelegatingHandler<TAuthenticationHandler>(provider, credentials, identityAuthority);
            });

            return builder;
        }

        /// <summary>
        /// Adds the baidu API authentication.
        /// </summary>
        /// <typeparam name="TAuthenticationHandler">The type of the authentication handler.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="identityAuthority">The identity authority.</param>
        /// <returns></returns>
        public static IHttpClientBuilder AddBaiduApiAuthentication<TAuthenticationHandler>(
            this IHttpClientBuilder builder, 
            ClientCredentials credentials, 
            string identityAuthority)
            where TAuthenticationHandler : DelegatingHandler
        {
            builder.Services.AddOptions();
            builder.Services.TryAddSingleton<IAccessTokensCacheManager, AccessTokensCacheManager>();
            builder.AddHttpMessageHandler(provider => CreateDelegatingHandler<TAuthenticationHandler>(provider, credentials, identityAuthority));

            return builder;
        }

        /// <summary>
        /// Creates the delegating handler.
        /// </summary>
        /// <typeparam name="TAuthenticationHandler">The type of the authentication handler.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="identityAuthority">The identity authority.</param>
        /// <returns></returns>
        private static TAuthenticationHandler CreateDelegatingHandler<TAuthenticationHandler>(
            IServiceProvider provider,
            ClientCredentials credentials, 
            string identityAuthority)
            where TAuthenticationHandler : DelegatingHandler
        {
            var httpClient = CreateHttpClient(provider, identityAuthority);
            var accessTokensCacheManager = provider.GetRequiredService<IAccessTokensCacheManager>();

            var constructor = new object[]
            {
                accessTokensCacheManager,
                credentials,
                httpClient
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