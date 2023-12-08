using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Polly;
using Swashbuckle.AspNetCore.Annotations;
using LLMServiceHub.Common;
using LLMService.Shared.Extensions;
using LLMService.Shared.ServiceInterfaces;
using LLMService.Shared.Models;

namespace LLMServiceHub.Controller.v1_0
{
    /// <summary>
    /// 集成百度AI类api
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaiduApiController" /> class.
    /// </remarks>
    /// <param name="baiduApiService">The baidu API service.</param>
    /// <param name="ernieVilgApiService">The ernie vilg API service.</param>
    //[Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/baidu")]
    [ApiExplorerSettings(GroupName = "百度大模型")]
    public class BaiduApiController(
        IAIChatApiService<ChatRequest, ChatApiResponse> baiduApiService,
        IAIPaintApiService<PaintApplyRequest, PaintResultResponse> ernieVilgApiService) : ControllerBase
    {
        /// <summary>
        /// The API service
        /// </summary>
        private readonly IAIChatApiService<ChatRequest, ChatApiResponse> _apiService = baiduApiService;
        /// <summary>
        /// The ernie vilg API service
        /// </summary>
        private readonly IAIPaintApiService<PaintApplyRequest, PaintResultResponse> _ernieVilgApiService = ernieVilgApiService;

        /// <summary>
        /// 发起文心一言大模型对话
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("chat")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ChatApiResponse), 200)]
        [AppExceptionInterceptor(ReturnCode = -100001, ApiVersion = "1.0")]
        public async Task Chat(ChatRequest request)
        {
            await _apiService.Chat(request);
        }

        /// <summary>
        /// 文生图
        /// <para>封装文生图api以及回调获取生成图片api</para>
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("text2img")]
        [ProducesResponseType(typeof(PaintResultResponse), 200)]
        [AppExceptionInterceptor(ReturnCode = -100002, ApiVersion = "1.0")]
        public async Task<IActionResult> ApplyText2Img(PaintApplyRequest request)
        {
            var result = await _ernieVilgApiService.Text2Image(request);
            Response.BuildAIGeneratedResponseFeature();
            return Ok(result);
        }
    }
}
