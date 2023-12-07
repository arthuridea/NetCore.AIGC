namespace LLMServiceHub.Service
{
    /// <summary>
    /// 
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
