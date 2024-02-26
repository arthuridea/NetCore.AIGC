using LLMService.Shared.ServiceInterfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace LLMService.Baidu.ErnieVilg
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IImageStorageProvider" />
    public class LocalImageStorageProvider : IImageStorageProvider
    {
        private readonly ILogger<LocalImageStorageProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalImageStorageProvider"/> class.
        /// </summary>
        public LocalImageStorageProvider(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment env,
            ILogger<LocalImageStorageProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _env = env;
        }
        /// <summary>
        /// Saves the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="pathname"></param>
        /// <returns></returns>
        public async Task<string> Save(string url, string pathname)
        {
            string localPathName = Path.Combine(_env.WebRootPath, pathname).Replace('\\','/');
            _logger.LogDebug($"LOCAL PATH -> {localPathName}");

            var httpClient = _httpClientFactory.CreateClient();
            var fileBytes = await httpClient.GetByteArrayAsync(url);

            string directory = Path.GetDirectoryName(localPathName);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _logger.LogInformation($"[{nameof(LocalImageStorageProvider)}] save: {url} -> {localPathName}");
            await File.WriteAllBytesAsync(localPathName, fileBytes);

            return await Task.FromResult(localPathName);
        }
    }
}
