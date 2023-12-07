using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using LLMServiceHub.Authentication;
using LLMServiceHub.Authentication.Extentions;
using LLMServiceHub.Client;
using LLMServiceHub.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using static System.Net.WebRequestMethods;
using LLMServiceHub.Common;

namespace LLMServiceHub.Service
{
    /// <summary>
    /// 百度文心api服务
    /// </summary>
    /// <seealso cref="LLMServiceHub.Service.IBaiduWenxinApiService" />
    public class BaiduWenxinApiService : IBaiduWenxinApiService
    {
        const string _api_client_key = BaiduApiConsts.BaiduWenxinApiClientName;
        /// <summary>
        /// The HTTP client factory
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;
        /// <summary>
        /// The chat data provider
        /// </summary>
        private readonly IChatDataProvider<BaiduWenxinMessage> _chatDataProvider;

        ///// <summary>
        ///// The serve sent event handler
        ///// </summary>
        //private readonly IServeSentEventHandler _serveSentEventHandler;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduWenxinApiService"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="chatDataProvider">数据持久化服务接口，此处实现了一个简单的Memorycache记录当前会话</param>
        /// <param name="logger">The logger.</param>
        public BaiduWenxinApiService(
            IHttpClientFactory factory,
            IChatDataProvider<BaiduWenxinMessage> chatDataProvider,
            ILogger<BaiduWenxinApiService> logger)
        {
            _httpClientFactory = factory;
            _chatDataProvider = chatDataProvider;
            _logger = logger;
        }

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
        /// 发起对话
        /// </summary>
        /// <param name="request">请求实体</param>
        /// <param name="response">当前Action的HttpResponse对象</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <seealso cref="ChatRequest" />
        /// <example>
        ///  {
        ///  "temperature": 0.95,
        ///  "top_p": 0.8,
        ///  "penalty_score": 1.5,
        ///  "stream": true,
        ///  "message": "我是一位擅长儿童发展心理、行为分析的资深教育实践家,我具有丰富的教学经验,具备儿童发展心理学、教育学的丰富知识，并且具有丰富的撰写儿童观察记录报告的经验,  今天在益智区,观察了一会孩子们的活动，洋洋提出问题天上的云为什么不掉下来？,孩子的年龄是3-4岁, 请以发展的眼光观察评价儿童、依据提供的行为数据进行客观分析、加入反思或发展建议, 请为我生成针对于以上的建议观察要点,行为分析,教育建议,家园指导建议,观察记录报告",
        ///  "model": 2,
        ///  "user_id": "7ffe3194-2bf0-48ba-8dbd-e888d7d556d3"
        ///  }
        /// </example>
        public async Task Chat(ChatRequest request, HttpResponse response, CancellationToken cancellationToken = default)
        {
            #region 初始化会话内容，读取历史记录并加入当前会话中
            if (string.IsNullOrEmpty(request.ConversationId))
            {
                request.ConversationId = Guid.NewGuid().ToString();
            }

            var conversation = await _chatDataProvider.GetConversationHistory(request.ConversationId);

            await _chatDataProvider.AddChatMessage(conversation, request.Message, "user");

            _logger.LogDebug(@$"【CALL {request.ModelSchema}】{JsonSerializer.Serialize(conversation, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            })}");
            #endregion

            #region 准备HttpClient和请求对象实体
            var _client = GetClient();
            string chatApiEndpoint = BaiduApiDefaults.LLM_Models[request.ModelSchema];
            var postdata = new BaiduApiChatRequest
            {
                Messages = conversation,
                Temperature = request.Temperature,
                TopP = request.TopP,
                PenaltyScore = request.PenaltyScore,
                Stream = request.Stream,
                UserId = request.UserId,
            };
            #endregion

            #region 设置响应头

            #endregion

            if (!request.Stream) // 非SSE推流格式,正常读取返回json
            {
                #region 非流式请求
                ChatApiResponse result = new();
                var apiResponse = await _client.PostAsJsonAsync(chatApiEndpoint, postdata);
                apiResponse.EnsureSuccessStatusCode();

                result = await apiResponse.DeserializeAsync<ChatApiResponse>(logger: _logger);
                result.ConversationId = request.ConversationId;
                result.ModelSchema = request.ModelSchema;

                //只有流模式会返回是否结束标识，在非流式请求中直接设置为true.
                result.IsEnd = request.Stream ? result.IsEnd : true;

                if (!result.NeedClearHistory)
                {
                    await _chatDataProvider.AddChatMessage(conversation, result.Result, "assistant");
                    await _chatDataProvider.SaveChat(request.ConversationId, conversation);
                }
                else
                {
                    _chatDataProvider.ResetSession(request.ConversationId);
                }
                response.BuildAIGeneratedResponseFeature();
                await response.WriteAsJsonAsync(result);

                #endregion 

            }
            else // SSE推流格式
            {
                #region 返回SSE推流
                var content = JsonContent.Create(postdata);

                // HttpClient需要设置 HttpCompletionOption.ResponseHeadersRead 来保证响应头结束就返回，否则依然会等到所有内容都响应结束才统一返回，就没意义了.
                // 因此要调用SendAsync方法保证可以设置更多的参数

                // 准备请求报文内容
                var requestHttpMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_client.BaseAddress}{chatApiEndpoint.Substring(1)}"),
                    Content = content
                };
                var apiResponse = await _client.SendAsync(requestHttpMessage, HttpCompletionOption.ResponseHeadersRead);
                apiResponse.EnsureSuccessStatusCode();

                // 设置响应头
                // 设置响应头：返回SSE格式
                response.BuildAIGeneratedResponseFeature(true);
                // 在读取SSE推流前就开启输出! 
                await response.Body.FlushAsync();

                using var stream = await apiResponse.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);
                string sseSection = string.Empty;
                bool isEnd = false;
                // SSE 输出格式是一行一条信息 每行格式为  data: {json} \n
                // 因此对SSE流做行循环，间隔100ms推送到http response
                while (!isEnd)
                {
                    sseSection = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(sseSection))
                    {
                        await response.WriteAsync($"{sseSection} \n");
                        await response.WriteAsync("\n");
                        await response.Body.FlushAsync();
                        if (sseSection.Contains("\"is_end\":true"))
                        {
                            isEnd = true;
                            break;
                        }
                        await Task.Delay(100);
                    }
                }
                #endregion

            }

        }
    }
}
