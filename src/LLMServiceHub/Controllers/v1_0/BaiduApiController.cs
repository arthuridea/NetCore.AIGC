using Asp.Versioning;
using LLMService.Baidu.ErnieVilg.Models;
using LLMService.Baidu.Wenxinworkshop;
using LLMService.Baidu.Wenxinworkshop.Models;
using LLMService.Shared.Extensions;
using LLMService.Shared.Models;
using LLMService.Shared.ServiceInterfaces;
using LLMServiceHub.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LLMServiceHub.Controllers.v1_0
{
    /// <summary>
    /// 集成百度AI类api
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaiduApiController" /> class.
    /// </remarks>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <param name="baiduApiService">The baidu API service.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="ernieVilgApiService">The ernie vilg API service.</param>

    // use oidc jwt authentication or local identity
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    #if !DEBUG
    [Authorize]
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{AppConsts.DefaultAuthScheme}")]
    #endif
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/baidu")]
    [ApiExplorerSettings(GroupName = "百度大模型")]
    public class BaiduApiController(
        IBaiduErniebotLLMService baiduApiService,
        ILogger<BaiduApiController> logger,
        IAIPaintApiService<PaintApplyRequest, PaintApiResult> ernieVilgApiService) : ControllerBase
    {
        /// <summary>
        /// The API service
        /// </summary>
        //private readonly IBaiduWenxinApiService _apiService = baiduApiService;
        private readonly IBaiduErniebotLLMService _apiService = baiduApiService;
        /// <summary>
        /// The ernie vilg API service
        /// </summary>
        private readonly IAIPaintApiService<PaintApplyRequest, PaintApiResult> _ernieVilgApiService = ernieVilgApiService;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger = logger;

        /// <summary>
        /// 发起文心一言大模型对话
        /// </summary>
        /// <param name="request">The request.</param>
        [HttpPost("chat")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(BaiduChatApiResponse), 200)]
        [AppExceptionInterceptor(ReturnCode = -100001, ApiVersion = "1.0")]
        public async Task Chat(BaiduChatRequestDto request)
        {
            _logger.LogInformation($">{User.Identity.Name}: [{request.ModelSchema}]{request.Message}");
            await _apiService.Chat(request);
        }

        /// <summary>
        /// 文生图
        /// <para>封装文生图api以及回调获取生成图片api</para>
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("text2img")]
        [ProducesResponseType(typeof(PaintApiResult), 200)]
        [AppExceptionInterceptor(ReturnCode = -100002, ApiVersion = "1.0")]
        public async Task<IActionResult> ApplyText2Img(PaintApplyRequest request)
        {
            var result = await _ernieVilgApiService.Text2Image(request);
            Response.BuildAIGeneratedResponseFeature();
            return Ok(result);
        }
    }
}
