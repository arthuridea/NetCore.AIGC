namespace LLMServiceHub.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="LLMServiceHub.Service.IImageStorageProvider" />
    public class LocalImageStorageProvider : IImageStorageProvider
    {
        private readonly ILogger<LocalImageStorageProvider> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalImageStorageProvider"/> class.
        /// </summary>
        public LocalImageStorageProvider(ILogger<LocalImageStorageProvider> logger)
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
            _logger.LogInformation($"[{nameof(LocalImageStorageProvider)}] save: {url} -> {pathname}");
            return await Task.FromResult(url);
        }
    }
}
