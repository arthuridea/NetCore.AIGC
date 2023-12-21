using LLMService.OpenAI.ChatGPT;
using LLMService.Shared;
using LLMService.Shared.Authentication.Handlers;
using LLMService.Shared.Authentication.Models;
using LLMService.Shared.Models;
using LLMService.Shared.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

// ***************************************************************************************
// DO NOT CHANGE THE NAMESPACE! It is .Net Core convention for configuration extensions. *
// ***************************************************************************************
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ChatGPTDependencyInjection
    {
        /// <summary>
        /// Adds the wenxinworkshop.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="openAIConfigKey"></param>
        /// <returns></returns>
        public static IServiceCollection AddChatGPT(this IServiceCollection services, IConfiguration configuration, string openAIConfigKey = "OpenAISettings")
        {
            // 百度配置文件
            var conf = configuration.GetSection(openAIConfigKey);

            services.AddOptions();
            services.Configure<OpenAIBackendServiceConfig>(conf);

            var openAISettings = configuration.GetSection(openAIConfigKey).Get<OpenAIBackendServiceConfig>();

            // 文心大模型客户端

            services.AddHttpClient(LLMServiceConsts.OpenAIApiClientName, client =>
            {
                client.BaseAddress = new Uri($"{LLMServiceConsts.OpenAIApiAuthority}/");
            })
            //.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            //{
            //    Proxy = new WebProxy(openAISettings.ProxyUrl, BypassOnLocal: true)
            //})
            .AddOpenAIApiAuthentication<OpenAIAuthenticationHandler>(
                    LLMServiceConsts.BaiduErnieVilgApiAuthority);


            //services.AddTransient<IAIChatApiService<ChatRequest, ChatApiResponse>, ChatGPTApiService>();
            services.AddTransient<IChatGPTLLMService, ChatGPTLLMService>();

            return services;
        }
    }
}
