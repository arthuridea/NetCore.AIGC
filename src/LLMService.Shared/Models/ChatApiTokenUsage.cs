using System.Text.Json.Serialization;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatApiTokenUsage
    {
        /// <summary>
        /// Gets or sets the prompt tokens.
        /// </summary>
        /// <value>
        /// The prompt tokens.
        /// </value>
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }
        /// <summary>
        /// Gets or sets the completion tokens.
        /// </summary>
        /// <value>
        /// The completion tokens.
        /// </value>
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }
        /// <summary>
        /// Gets or sets the total tokens.
        /// </summary>
        /// <value>
        /// The total tokens.
        /// </value>
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
