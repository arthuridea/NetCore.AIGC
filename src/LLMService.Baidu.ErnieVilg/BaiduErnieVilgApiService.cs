using LLMService.Baidu.ErnieVilg.Models;
using LLMService.Shared;
using LLMService.Shared.Extensions;
using LLMService.Shared.Models;
using LLMService.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace LLMService.Baidu.ErnieVilg
{
    /// <summary>
    /// 百度智能绘图api
    /// </summary>
    public class BaiduErnieVilgApiService : IAIPaintApiService<PaintApplyRequest, PaintResultResponse>
    {
        const string _api_client_key = LLMServiceConsts.BaiduErnieVilgApiClientName;
        /// <summary>
        /// The HTTP client factory
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;
        /// <summary>
        /// The image provider
        /// </summary>
        private readonly IImageStorageProvider _imageProvider;
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="BaiduErnieVilgApiService"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="imgProvider"></param>
        /// <param name="logger">The logger.</param>
        public BaiduErnieVilgApiService(
            IHttpClientFactory factory,
            IImageStorageProvider imgProvider,
            ILogger<BaiduErnieVilgApiService> logger)
        {
            _httpClientFactory = factory;
            _imageProvider = imgProvider;
            _logger = logger;
        }

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns></returns>
        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient(_api_client_key);
            _logger.LogDebug("[API CLIENT]{0} -> {1}", _api_client_key, client.BaseAddress);
            return client;
        }

        /// <summary>
        /// 文生图
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<PaintResultResponse> Text2Image(PaintApplyRequest request)
        {
            if (string.IsNullOrEmpty(request.ConversationId))
            {
                request.ConversationId = Guid.NewGuid().ToString();
            }

            _logger.LogDebug(@"【CALL ErnieVilg V2】{0}", JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }));

            var _client = GetClient();

            string paintApiEndpoint = LLMApiDefaults.ErnieVilgV2ApiEndpoint;
            var response = await _client.PostAsJsonAsync(paintApiEndpoint, request);
            response.EnsureSuccessStatusCode();

            var taskResult = await response.DeserializeAsync<PaintApplyResponse>(_logger);


            if (taskResult.Data != null && !string.IsNullOrEmpty(taskResult.Data.TaskId))
            {
                var result = await challengePaintResult(_client, taskResult.Data.TaskId);
                return result;
            }

            return null;
        }

        private async Task<PaintResultResponse> challengePaintResult(HttpClient client, string id, int initDelayInSeconds = 3000)
        {
            PaintResultResponse result = new();
            string paintResultApiEndpoint = LLMApiDefaults.ErnieVilgV2ResultApiEndpoint;
            bool taskFinished = false;
            int retrys = 0;
            Thread.Sleep(initDelayInSeconds);
            while (!taskFinished && retrys < 10)
            {
                retrys++;
                var imgResultResponse = await client.PostAsJsonAsync<dynamic>(paintResultApiEndpoint, new
                {
                    task_id = id
                });

                imgResultResponse.EnsureSuccessStatusCode();

                result = await imgResultResponse.DeserializeAsync<PaintResultResponse>(_logger);
                if (result.Data.ProcessStatus == PaintTaskProcessStatus.Finished)
                {
                    taskFinished = true;
                    var images = result.Data.SubTaskResults
                                       .SelectMany(x => 
                                            x.Images.Select(img => img.Url));

                    foreach (var image in images)
                    {
                        _logger.LogDebug("{0}", image);
                        await _imageProvider.Save(image, $"aigc\\images\\{DateTime.Now:yyyyMM}\\{DateTime.Now:yyyyMMddHHmmssffff}.jpg");
                    }
                    break;
                }
                Thread.Sleep(3000);
            }
            return result;
        }
    }
}
