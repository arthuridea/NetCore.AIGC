using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LLMService.Shared.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Deserializes the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static async Task<T> DeserializeAsync<T>(this HttpResponseMessage response, ILogger logger = null)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            logger?.LogDebug(responseString);
            var result = JsonSerializer.Deserialize<T>(responseString);
            return result;
        }


        /// <summary>
        /// Builds the ai generated response feature.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="streamOutput">if set to <c>true</c> [stream output].</param>
        public static void BuildAIGeneratedResponseFeature(this HttpResponse response, bool streamOutput = false)
        {
            response.Headers.CacheControl = "no-cache";
            response.Headers.Append("Statement", "AI-generated");
            if (streamOutput)
            {
                response.ContentType = "text/event-stream;charset=utf-8";
            }
        }
    }
}
