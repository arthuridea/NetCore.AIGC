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
    public class OpenAIBackendRequestModel: IBackendChatRequest<OpenAIChatMessage>
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        [JsonPropertyName("messages")]
        public List<OpenAIChatMessage> Messages { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OpenAIBackendRequestModel"/> is stream.
        /// </summary>
        /// <value>
        ///   <c>true</c> if stream; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
    }
}
