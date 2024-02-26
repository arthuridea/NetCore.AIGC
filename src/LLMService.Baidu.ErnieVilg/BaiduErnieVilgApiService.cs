using LLMService.Baidu.ErnieVilg.Models;
using LLMService.Shared;
using LLMService.Shared.Extensions;
using LLMService.Shared.Models;
using LLMService.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace LLMService.Baidu.ErnieVilg
{
    /// <summary>
    /// 百度智能绘图api
    /// </summary>
    public class BaiduErnieVilgApiService : IAIPaintApiService<PaintApplyRequest, PaintApiResult>
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
        public async Task<PaintApiResult> Text2Image(PaintApplyRequest request)
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

        /// <summary>
        /// Challenges the paint result.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="saveRemoteImage">if set to <c>true</c> [save remote image].</param>
        /// <param name="createYearMonthFolder">if set to <c>true</c> [create year month folder].</param>
        /// <param name="strategy">The strategy.</param>
        /// <param name="initDelayInSeconds">The initialize delay in seconds.</param>
        /// <returns></returns>
        private async Task<PaintApiResult> challengePaintResult(
            HttpClient client, 
            string id, 
            bool saveRemoteImage=true, 
            bool createYearMonthFolder = true, 
            string strategy = "rnd_prefix", 
            int initDelayInSeconds = 3000)
        {
            PaintApiResult apiResult = new();
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

                        if (saveRemoteImage)
                        {
                            string savedPath = getSavedPath(image, null, createYearMonthFolder, strategy);
                            string localPathName = await _imageProvider.Save(image, savedPath);
                            apiResult.AIImages.Add(localPathName);
                        }
                        else
                        {
                            apiResult.AIImages.Add(image);
                        }
                    }
                    break;
                }
                Thread.Sleep(3000);
            }
            apiResult.LLMResponseData = result;
            return apiResult;
        }

        private string getSavedPath(string url, string root = null, bool createYearMonthFolder = true, string strategy = "rnd_prefix")
        {
            List<string> props = new();
            if(string.IsNullOrEmpty(root))
            {
                root = $"aigc{Path.PathSeparator}images";
            }
            props.Add(root);
            if (createYearMonthFolder)
            {
                props.Add($"{DateTime.Now:yyyyMM}");
            }

            string filename = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}{getFileExtension(url)}";
            if (strategy == "rnd_prefix")
            {
                string originalFilename = getOriginalFilenme(url);
                filename = $"{GenerateRandomString(5)}_{originalFilename}";
            }
            props.Add(filename);
            string path = Path.Combine(props.ToArray()).Replace('\\','/');
            return path;
        }

        private static string getOriginalFilenme(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            var uri = new Uri(url);
            return Path.GetFileName(uri.LocalPath);
        }

        private static string getFileExtension(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            var uri = new Uri(url);
            return Path.GetExtension(uri.LocalPath);
        }

        /// <summary>
        /// Generates the random string.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        private static string GenerateRandomString(int length)
        {
            string result = "";
            Random ran = new Random();
            string dictionary = "0123456789abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
            int len = dictionary.Length;
            char[] arrList = dictionary.ToCharArray();
            for (int i = 0; i < length; i++)
            {
                int RandKey = ran.Next(0, len - 1);
                result += arrList[RandKey];
            }

            return result;
        }
    }

}
