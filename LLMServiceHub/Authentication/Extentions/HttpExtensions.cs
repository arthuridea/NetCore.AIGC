using LLMServiceHub.Models;
using System;
using System.Text.Json;

namespace LLMServiceHub.Authentication.Extentions
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
    }
}
