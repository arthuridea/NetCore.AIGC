using LLMService.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LLMService.OpenAI.ChatGPT.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenAIChatMessage: IChatMessage<List<OpenAIMessageContent>>
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
        public List<OpenAIMessageContent> Content { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "default";
    }
}
