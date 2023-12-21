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
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LLMService.Shared.ChatService
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequestDto">The type of the request dto.</typeparam>
    /// <typeparam name="TResponseDto">The type of the response dto.</typeparam>
    /// <typeparam name="TBackendRequestDto">The type of the backend request dto.</typeparam>
    /// <typeparam name="TBackendResponseDto">The type of the backend response dto.</typeparam>
    /// <typeparam name="TChatMessage">The type of the chat message.</typeparam>
    /// <typeparam name="TMessageContent">The type of the message content.</typeparam>
    /// <typeparam name="TChatServiceOption">The type of the chat service option.</typeparam>
    public abstract class ChatServiceBase<TRequestDto, TResponseDto, 
                                          TBackendRequestDto, TBackendResponseDto,
                                          TChatMessage, TMessageContent,
                                          TChatServiceOption>
        where TResponseDto: IChatResponse<TBackendResponseDto>, new()
        where TBackendRequestDto : IBackendChatRequest<TChatMessage>, new()
        where TBackendResponseDto : new()
        where TRequestDto: IChatRequest<string>
        where TChatMessage : IChatMessage<TMessageContent>, new()
        where TChatServiceOption : class, IChatServiceOption, new()
    {

        #region fields
        /// <summary>
        /// The chat option
        /// </summary>
        private readonly TChatServiceOption _chatOption;
        /// <summary>
        /// The API client key
        /// </summary>
        private readonly string _api_client_key;
        /// <summary>
        /// The HTTP client factory
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;
        /// <summary>
        /// The chat data provider
        /// </summary>
        private readonly IChatDataProvider<TChatMessage, TMessageContent> _chatDataProvider;

        /// <summary>
        /// The HTTP context
        /// </summary>
        private readonly IHttpContextAccessor _context;

        ///// <summary>
        ///// The serve sent event handler
        ///// </summary>
        //private readonly IServeSentEventHandler _serveSentEventHandler;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatServiceBase{TRequestDto, TResponseDto, TBackendRequestDto, TBackendResponseDto, TChatMessage, TMessageContent, TChatServiceOption}" /> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="context">The context.</param>
        /// <param name="chatDataProvider">The chat data provider.</param>
        /// <param name="chatOption">The chat option.</param>
        /// <param name="logger">The logger.</param>
        public ChatServiceBase(
            IHttpClientFactory factory,
            IHttpContextAccessor context,
            IChatDataProvider<TChatMessage, TMessageContent> chatDataProvider,
            IOptionsSnapshot<TChatServiceOption> chatOption,
            ILogger<ChatServiceBase<
                TRequestDto, TResponseDto, 
                TBackendRequestDto, TBackendResponseDto, 
                TChatMessage, TMessageContent, 
                TChatServiceOption>> logger)
        {
            _chatOption = chatOption?.Value ?? new();
            _api_client_key = _chatOption.BackendHttpClientName;
            _httpClientFactory = factory;
            _context = context;
            _chatDataProvider = chatDataProvider;
            _logger = logger;
        }

        /// <summary>
        /// Chats the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task Chat(TRequestDto request, CancellationToken cancellationToken = default)
        {
            var response = _context.HttpContext.Response;

            #region 准备HttpClient和请求对象实体
            string chatApiEndpoint = LLMApiDefaults.LLM_Models[request.ModelSchema];
            var dialog = await BuildBackendRequestData(request);
            var _client = GetClient();
            #endregion

            #region 设置响应头

            #endregion

            if (!_chatOption.SupportStreaming || !request.Stream) // 非SSE推流格式,正常读取返回json
            {
                #region 非流式请求
                TBackendResponseDto apiResponseData = new();
                TResponseDto result = new();
                var apiResponse = await _client.PostAsJsonAsync(chatApiEndpoint, dialog.BackendRequest, cancellationToken: cancellationToken);
                apiResponse.EnsureSuccessStatusCode();


                apiResponseData = await apiResponse.DeserializeAsync<TBackendResponseDto>(logger: _logger);
                result.ConversationId = request.ConversationId;
                result.ModelSchema = request.ModelSchema;
                result.Result = apiResponseData;

                //只有流模式会返回是否结束标识，在非流式请求中直接设置为true.
                //result.IsEnd = !request.Stream || result.IsEnd;

                if (!result.NeedClearHistory)
                {
                    var aiMessage = GetAIMessage(apiResponseData);
                    await _chatDataProvider.AddChatMessage(dialog.Conversation, CreateMessageContent(aiMessage.Content.ToString(), "text", "assistant"), "assistant");
                    await _chatDataProvider.SaveChat(request.ConversationId, dialog.Conversation);
                }
                else
                {
                    _chatDataProvider.ResetSession(request.ConversationId);
                }
                

                //var stringContent = await apiResponse.Content.ReadAsStringAsync();

                response.BuildAIGeneratedResponseFeature();
                await response.WriteAsJsonAsync(result, cancellationToken: cancellationToken);
                //await response.WriteAsync(stringContent);

                #endregion 

            }
            else // SSE推流格式
            {
                #region 返回SSE推流
                var content = JsonContent.Create(dialog.BackendRequest);

                // HttpClient需要设置 HttpCompletionOption.ResponseHeadersRead 来保证响应头结束就返回，否则依然会等到所有内容都响应结束才统一返回，就没意义了.
                // 因此要调用SendAsync方法保证可以设置更多的参数

                // 准备请求报文内容
                var requestHttpMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_client.BaseAddress}{chatApiEndpoint[1..]}"),
                    Content = content
                };
                var apiResponse = await _client.SendAsync(requestHttpMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                apiResponse.EnsureSuccessStatusCode();

                // 设置响应头
                // 设置响应头：返回SSE格式
                response.BuildAIGeneratedResponseFeature(true);
                // 在读取SSE推流前就开启输出! 
                await response.Body.FlushAsync(cancellationToken);

                using var stream = await apiResponse.Content.ReadAsStreamAsync(cancellationToken);
                using var reader = new StreamReader(stream);
                string sseSection = string.Empty;
                bool isEnd = false;
                StringBuilder aigc = new StringBuilder();
                // SSE 输出格式是一行一条信息 每行格式为  data: {json} \n
                // 因此对SSE流做行循环，间隔100ms推送到http response
                while (!isEnd)
                {
                    sseSection = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(sseSection))
                    {

                        var status = GetSegmentStatus(sseSection);
                        _logger.LogDebug($"standalone: {_chatOption.IsBackendStreamEndStandalone} | isEnd: {status.IsEnd} | wrap: {status.RequireWrap}");
                        if (status.RequireWrap)
                        {
                            var wrapped = WrapData(sseSection, request, aigc);
                            if(wrapped != null && !wrapped.Equals(default(TResponseDto)))
                            {
                                await GenerateSSEResponse(response, wrapped, cancellationToken);
                            }
                        }
                        else
                        {
                            await GenerateSSEResponse(response, sseSection, cancellationToken);
                        }

                        if (status.IsEnd)
                        {
                            isEnd = true;
                            if (aigc.Length > 0)
                            {
                                await _chatDataProvider.AddChatMessage(dialog.Conversation, CreateMessageContent(aigc.ToString(), "text", "assistant"), "assistant");
                                await _chatDataProvider.SaveChat(request.ConversationId, dialog.Conversation);
                            }
                            break;
                        }

                        await Task.Delay(100, cancellationToken);
                    }
                }
                #endregion

            }
        }

        #region helper methods

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns></returns>
        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient(_api_client_key);
            _logger.LogDebug($"[API CLIENT]{_api_client_key} -> {client.BaseAddress}");
            return client;
        }

        /// <summary>
        /// Builds the backend request data.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private async Task<(TBackendRequestDto BackendRequest, List<TChatMessage> Conversation)> BuildBackendRequestData(TRequestDto request)
        {
            TBackendRequestDto backendRequest = new();
            #region 初始化会话内容，读取历史记录并加入当前会话中
            if (string.IsNullOrEmpty(request.ConversationId))
            {
                request.ConversationId = Guid.NewGuid().ToString();
            }

            var conversation = await _chatDataProvider.GetConversationHistory(request.ConversationId);

            await _chatDataProvider.AddChatMessage(conversation, CreateMessageContent(request.Message, "text", "user"), "user");

#if DEBUG
            _logger.LogDebug(@$"【CALL {request.ModelSchema}】{JsonSerializer.Serialize(conversation, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            })}");
#endif
            #endregion

            backendRequest = LLMRequestMapping(request);
            backendRequest.Messages = conversation;

            return (BackendRequest: backendRequest, Conversation: conversation);
        }

        /// <summary>
        /// Gets the segment status.
        /// </summary>
        /// <param name="sseSection">The sse section.</param>
        /// <returns></returns>
        private (bool IsEnd, bool RequireWrap) GetSegmentStatus(string sseSection)
        {
            bool isEnd = sseSection.Contains(_chatOption.BackendStreamEndTokenPattern);
            bool requireWrap = !_chatOption.IsBackendStreamEndStandalone || !isEnd;
            return (IsEnd: isEnd, RequireWrap: requireWrap);
        }

        /// <summary>
        /// Wraps the data.
        /// </summary>
        /// <param name="sseSection">The sse section.</param>
        /// <param name="request">The request.</param>
        /// <param name="aigc">The aigc.</param>
        /// <returns></returns>
        private TResponseDto WrapData(string sseSection, TRequestDto request, StringBuilder aigc)
        {
            if (sseSection.StartsWith(_chatOption.BackendStreamPrefixToken))
            {
                TBackendResponseDto ldata = new();
                string lineApiResponse = sseSection.Substring(_chatOption.BackendStreamPrefixToken.Length);
                var lineJson = JsonDocument.Parse(lineApiResponse);
                ldata = lineJson.Deserialize<TBackendResponseDto>();
                if (ldata != null)
                {
                    IChatMessage<string> lcontent = GetAIMessage(ldata);
                    aigc.Append(lcontent.Content);


                    TResponseDto result = new();
                    result.ConversationId = request.ConversationId;
                    result.ModelSchema = request.ModelSchema;
                    result.Result = ldata;

                    return result;
                    //await GenerateSSEResponse(response, result, cancellationToken);
                }
            }
            return default;
        }

        /// <summary>
        /// Generates the sse response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <param name="content">The content.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task GenerateSSEResponse<T>(HttpResponse response, T content, CancellationToken cancellationToken)
        {
            if (content.GetType() == typeof(string))
            {
                await response.WriteAsync($"{content} \n", cancellationToken: cancellationToken);
            }
            else
            {
                string plainText = JsonSerializer.Serialize(content);
                await response.WriteAsync($"data: {plainText} \n", cancellationToken: cancellationToken);
            }
            await response.WriteAsync("\n", cancellationToken: cancellationToken);
            await response.Body.FlushAsync(cancellationToken);
        }
        #endregion

        #region abstract methods for subclass data mapping
        /// <summary>
        /// Creates the API message.
        /// </summary>
        /// <param name="content">The message content.</param>
        /// <param name="type">The type.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        protected abstract TMessageContent CreateMessageContent(string content, string type = "text", string role = "user");

        /// <summary>
        /// LLMs the request mapping.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected abstract TBackendRequestDto LLMRequestMapping(TRequestDto source);

        /// <summary>
        /// Gets the ai message.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        protected abstract ChatMessageBase GetAIMessage(TBackendResponseDto response);

        #endregion

    }
}
