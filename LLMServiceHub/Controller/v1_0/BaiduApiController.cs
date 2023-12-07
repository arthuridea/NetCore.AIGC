using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Polly;
using LLMServiceHub.Models;
using LLMServiceHub.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace LLMServiceHub.Controller.v1_0
{
    /// <summary>
    /// 集成百度AI类api
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    //[Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/baidu")]
    [ApiExplorerSettings(GroupName = "百度大模型")]
    public class BaiduApiController : ControllerBase
    {
        /// <summary>
        /// The API service
        /// </summary>
        private readonly IBaiduWenxinApiService _apiService;
        /// <summary>
        /// The ernie vilg API service
        /// </summary>
        private readonly IBaiduErnieVilgApiService _ernieVilgApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduApiController" /> class.
        /// </summary>
        /// <param name="baiduApiService">The baidu API service.</param>
        /// <param name="ernieVilgApiService">The ernie vilg API service.</param>
        public BaiduApiController(
            IBaiduWenxinApiService baiduApiService, 
            IBaiduErnieVilgApiService ernieVilgApiService)
        {
            _apiService = baiduApiService;
            _ernieVilgApiService = ernieVilgApiService;
        }

        /// <summary>
        /// 发起文心一言大模型对话
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("chat")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ChatApiResponse), 200)]
        public async Task Chat(ChatRequest request)
        {
            await _apiService.Chat(request, Response);
        }

        /// <summary>
        /// 文生图
        /// <para>封装文生图api以及回调获取生成图片api</para>
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("text2img")]
        [ProducesResponseType(typeof(PaintResultResponse), 200)]
        public async Task<IActionResult> ApplyText2Img(PaintApplyRequest request)
        {
            var result = await _ernieVilgApiService.Text2ImgV2(request);
            return Ok(result);
        }
    }
}
