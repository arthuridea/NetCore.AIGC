using LLMService.Baidu.ErnieVilg;
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
    public static class ErnieVilgDependencyInjection
    {
        /// <summary>
        /// Adds the wenxinworkshop.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="ernieVilgConfigKey"></param>
        /// <returns></returns>
        public static IServiceCollection AddErnieVilg(this IServiceCollection services, IConfiguration configuration, string ernieVilgConfigKey = "BaiduErnieVilgSettings")
        {
            // 百度配置文件
            var ernieVilgSettings = configuration.GetSection(ernieVilgConfigKey).Get<ClientCredentials>();

            // 智能绘画客户端
            services.AddHttpClient(LLMServiceConsts.BaiduErnieVilgApiClientName, client =>
            {
                client.BaseAddress = new Uri($"{LLMServiceConsts.BaiduErnieVilgApiAuthority}/");
            })
            .AddBaiduApiAuthentication<BaiduApiAuthenticationHandler>(
                    ernieVilgSettings,
                    LLMServiceConsts.BaiduErnieVilgApiAuthority);

            services.AddTransient<IAIPaintApiService<PaintApplyRequest, PaintResultResponse>, BaiduErnieVilgApiService>();

            return services;
        }
    }
}
