using Asp.Versioning;
using LLMService.OpenAI.ChatGPT;
using LLMService.OpenAI.ChatGPT.Model;
using LLMService.Shared.Models;
using LLMServiceHub.Common;
using Microsoft.AspNetCore.Mvc;

namespace LLMServiceHub.Controller.v1_0
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    //[Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/chatgpt")]
    [ApiExplorerSettings(GroupName = "ChatGPT")]
    public class ChatGPTApiController(
        IChatGPTLLMService gptService): ControllerBase
    {

        /// <summary>
        /// The API service
        /// </summary>
        private readonly IChatGPTLLMService _apiService = gptService;

        /// <summary>
        /// 发起ChatGPT对话
        /// eg.
        /// what is the difference between star and planet?
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("chat")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(OpenAIChatResponse), 200)]
        [AppExceptionInterceptor(ReturnCode = -100001, ApiVersion = "1.0")]
        public async Task Chat(ChatRequest request)
        {
            await _apiService.Chat(request);
        }

    }
}
