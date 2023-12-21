using LLMService.OpenAI.ChatGPT.Model;
using LLMService.Shared;
using LLMService.Shared.Authentication.Models;
using LLMService.Shared.ChatService;
using LLMService.Shared.Extensions;
using LLMService.Shared.Models;
using LLMService.Shared.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LLMService.OpenAI.ChatGPT
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ChatServiceBase{TRequestDto, TResponseDto, TBackendRequestDto, TBackendResponseDto, TChatMessage, TMessageContent, TChatServiceOption}" />
    public class ChatGPTLLMService: 
        ChatServiceBase<ChatRequest, OpenAIChatResponse, 
                        OpenAIBackendRequestModel, OpenAIBackendResponseModel, 
                        OpenAIChatMessage, List<OpenAIMessageContent>, 
                        OpenAIBackendServiceConfig>, IChatGPTLLMService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatGPTLLMService" /> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="context">The context.</param>
        /// <param name="chatDataProvider">The chat data provider.</param>
        /// <param name="chatOption">The chat option.</param>
        /// <param name="logger">The logger.</param>
        public ChatGPTLLMService(
            IHttpClientFactory factory,
            IHttpContextAccessor context,
            IChatDataProvider<OpenAIChatMessage, List<OpenAIMessageContent>> chatDataProvider,
            IOptionsSnapshot<OpenAIBackendServiceConfig> chatOption,
            ILogger<ChatGPTLLMService> logger)
                : base(factory, context, chatDataProvider, chatOption, logger) { }


        /// <summary>
        /// Creates the API message.
        /// </summary>
        /// <param name="content">The message content.</param>
        /// <param name="type">The type.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        protected override List<OpenAIMessageContent> CreateMessageContent(string content, string type = "text", string role = "user")
        {
            var ret = new List<OpenAIMessageContent>();
            var entity = new OpenAIMessageContent
            {
                Type = type
            };
            if(type=="text")
            {
                entity.Text = content;
                entity.ImageUrl = null;
            }
            if (type == "image_url")
            {
                entity.ImageUrl = new OpenAIMessageUrl { Url = content };
            }
            ret.Add(entity);
            return ret;
        }

        /// <summary>
        /// LLMs the request mapping.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected override OpenAIBackendRequestModel LLMRequestMapping(ChatRequest source)
        {
            return new OpenAIBackendRequestModel
            {
                Model = source.ModelSchema.GetAttribute<DisplayAttribute>().Name,
                Stream = source.Stream,
            };
        }

        /// <summary>
        /// Gets the ai message.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        protected override ChatMessageBase GetAIMessage(OpenAIBackendResponseModel response)
        {
            var data = response?.Choices?.First();
            return response.ObjectType == "chat.completion.chunk" ? data.Delta : data.Message;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IChatGPTLLMService
    {
        /// <summary>
        /// Chats the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task Chat(ChatRequest request, CancellationToken cancellationToken = default);
    }
    
}
