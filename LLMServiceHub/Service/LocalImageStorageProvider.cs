namespace LLMServiceHub.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="LLMServiceHub.Service.IImageStorageProvider" />
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
            string localPathName = Path.Combine(_env.WebRootPath, pathname);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(url);
            if (response != null && response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                using(var stream  = await response.Content.ReadAsStreamAsync())
                {
                    _logger.LogDebug($"LOCAL PATH -> {localPathName}");
                    string directory = Path.GetDirectoryName(localPathName);
                    if(!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    using (var fs = new FileStream(localPathName, FileMode.Create))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }
            }
            _logger.LogInformation($"[{nameof(LocalImageStorageProvider)}] save: {url} -> {pathname}");
            return await Task.FromResult(localPathName);
        }
    }
}
