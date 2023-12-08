namespace LLMService.Shared.ServiceInterfaces
{
    /// <summary>
    /// Since AIGC APIs usually provide a temperally resource url, we may want to save aigc media locally.
    /// This provider shows a proper way to download resources. 
    /// </summary>
    public interface IImageStorageProvider
    {
        /// <summary>
        /// Saves the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="pathname"></param>
        /// <returns></returns>
        Task<string> Save(string url, string pathname);
    }
}
