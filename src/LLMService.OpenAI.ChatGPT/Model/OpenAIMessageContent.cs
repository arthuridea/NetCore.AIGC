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
    public class OpenAIMessageContent
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        [JsonPropertyName("text")]
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        [JsonPropertyName("image_url")]
        public OpenAIMessageUrl? ImageUrl { get; set; } = null;
    }

    /// <summary>
    /// 
    /// </summary>
    public struct OpenAIMessageUrl
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
