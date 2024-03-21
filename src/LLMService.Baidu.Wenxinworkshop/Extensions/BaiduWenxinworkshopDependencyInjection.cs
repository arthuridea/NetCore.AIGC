using LLMService.Baidu.Wenxinworkshop;
using LLMService.Shared;
using LLMService.Shared.Authentication.Handlers;
using LLMService.Shared.Authentication.Models;
using LLMService.Shared.Models;
using LLMService.Shared.ServiceInterfaces;
using Microsoft.Extensions.Configuration;

// ***************************************************************************************
// DO NOT CHANGE THE NAMESPACE! It is .Net Core convention for configuration extensions. *
// ***************************************************************************************
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class BaiduWenxinworkshopDependencyInjection
    {
        /// <summary>
        /// Adds the wenxinworkshop.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="wenxinworkshopConfigKey"></param>
        /// <returns></returns>
        public static IServiceCollection AddWenxinworkshop(this IServiceCollection services, IConfiguration configuration, string wenxinworkshopConfigKey = "BaiduWenxinSettings")
        {
            // 百度配置文件
            var wenxinSettings = configuration.GetSection(wenxinworkshopConfigKey).Get<OAuth2BackendServiceConfig>();

            // 文心大模型客户端

            services.AddHttpClient(wenxinSettings.BackendHttpClientName, client =>
            {
                client.BaseAddress = new Uri($"{LLMServiceConsts.BaiduWenxinApiAuthority}/");
            })
            .AddBaiduApiAuthentication<BaiduApiAuthenticationHandler>(
                    wenxinSettings,
                    LLMServiceConsts.BaiduWenxinApiAuthority);


            //services.AddTransient<IAIChatApiService<ChatRequest, ChatApiResponse>, BaiduWenxinApiService>();
            //services.AddTransient<IBaiduWenxinApiService, BaiduWenxinApiService>();
            services.AddTransient<IBaiduErniebotLLMService, BaiduErniebotLLMService>();

            return services;
        }
    }
}
