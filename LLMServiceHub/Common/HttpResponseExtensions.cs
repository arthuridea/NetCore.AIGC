namespace LLMServiceHub.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Builds the ai generated response feature.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="streamOutput">if set to <c>true</c> [stream output].</param>
        public static void BuildAIGeneratedResponseFeature(this HttpResponse response, bool streamOutput = false)
        {
            response.Headers.CacheControl = "no-cache";
            response.Headers.Append("Statement", "AI-generated");
            if(streamOutput)
            {
                response.ContentType = "text/event-stream;charset=utf-8";
            }
        }
    }
}
