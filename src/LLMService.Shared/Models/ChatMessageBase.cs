using System.Text.Json.Serialization;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IChatMessage" />
    public class ChatMessageBase : IChatMessage
    {
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [JsonPropertyName("role")]
        public string Role { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
