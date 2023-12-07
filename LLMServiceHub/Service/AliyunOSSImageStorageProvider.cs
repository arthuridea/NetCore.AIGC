namespace LLMServiceHub.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="LLMServiceHub.Service.IImageStorageProvider" />
    public class AliyunOSSImageStorageProvider : IImageStorageProvider
    {
        private readonly ILogger<AliyunOSSImageStorageProvider> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AliyunOSSImageStorageProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public AliyunOSSImageStorageProvider(ILogger<AliyunOSSImageStorageProvider> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Saves the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="pathname"></param>
        /// <returns></returns>
        public async Task<string> Save(string url, string pathname)
        {
            _logger.LogInformation($"[{nameof(AliyunOSSImageStorageProvider)}] save: {url} -> {pathname}");
            return await Task.FromResult(url);
        }
    }
}
