using LLMService.Shared;
using LLMService.Shared.Models;
using LLMService.Shared.Extensions;
using LLMService.Shared.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using LLMService.Shared.ChatService;
using LLMService.Shared.Authentication.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace LLMService.Baidu.Wenxinworkshop
{
    /// <summary>
    /// 百度文心api服务
    /// </summary>
    public interface IBaiduErniebotLLMService
    {
        /// <summary>
        /// Chats the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task Chat(ChatRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ChatServiceBase{TRequestDto, TResponseDto, TBackendRequestDto, TBackendResponseDto, TChatMessage, TMessageContent, TChatServiceOption}" />
    /// <seealso cref="IBaiduErniebotLLMService" />
    public class BaiduErniebotLLMService :
        ChatServiceBase<ChatRequest, ChatApiResponse,
                        BaiduApiChatRequest, BaiduWenxinChatResponse,
                        ChatMessageBase, string,
                        OAuth2BackendServiceConfig>, IBaiduErniebotLLMService, IAIChatApiService<ChatRequest, ChatApiResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduErniebotLLMService"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="context">The context.</param>
        /// <param name="chatDataProvider">The chat data provider.</param>
        /// <param name="chatOption">The chat option.</param>
        /// <param name="logger">The logger.</param>
        public BaiduErniebotLLMService(
            IHttpClientFactory factory,
            IHttpContextAccessor context,
            IChatDataProvider<ChatMessageBase, string> chatDataProvider,
            IOptionsSnapshot<OAuth2BackendServiceConfig> chatOption,
            ILogger<BaiduErniebotLLMService> logger)
                : base(factory, context, chatDataProvider, chatOption, logger) { }


        /// <summary>
        /// Creates the API message.
        /// </summary>
        /// <param name="content">The message content.</param>
        /// <param name="type">The type.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        protected override string CreateMessageContent(string content, string type = "text", string role = "user")
        {
            return content;
        }

        /// <summary>
        /// LLMs the request mapping.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected override BaiduApiChatRequest LLMRequestMapping(ChatRequest source)
        {
            return new BaiduApiChatRequest
            {
                Temperature = source.Temperature,
                TopP = source.TopP,
                PenaltyScore = source.PenaltyScore,
                Stream = source.Stream,
                UserId = source.UserId,
            };
        }

        /// <summary>
        /// Gets the ai message.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        protected override ChatMessageBase GetAIMessage(BaiduWenxinChatResponse response)
        {
            var data = response.Result;
            return new ChatMessageBase
            {
                Content = data,
                Role = "assistant"
            };
        }
    }

}
