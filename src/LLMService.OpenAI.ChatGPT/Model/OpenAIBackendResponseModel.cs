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
    public class OpenAIBackendResponseModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the type of the object.
        /// </summary>
        /// <value>
        /// The type of the object.
        /// </value>
        [JsonPropertyName("object")]
        public string ObjectType { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        [JsonPropertyName("created")]
        public long Created { get; set; }
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        [JsonPropertyName("model")]
        public string Model {  get; set; }

        /// <summary>
        /// Gets or sets the system finger print.
        /// </summary>
        /// <value>
        /// The system finger print.
        /// </value>
        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerPrint {  get; set; }
        /// <summary>
        /// Gets or sets the usage.
        /// </summary>
        /// <value>
        /// The usage.
        /// </value>
        [JsonPropertyName("usage")]
        public ChatApiTokenUsage Usage { get; set; }
        /// <summary>
        /// Gets or sets the choices.
        /// </summary>
        /// <value>
        /// The choices.
        /// </value>
        [JsonPropertyName("choices")]
        public List<OpenAIChatCompletion> Choices { get; set; } = new();


    }

    /// <summary>
    /// 
    /// </summary>
    public class OpenAIChatCompletion
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        [JsonPropertyName("index")]
        public int Index { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonPropertyName("message")]
        public ChatMessageBase Message { get; set; }
        /// <summary>
        /// Gets or sets the delta.
        /// </summary>
        /// <value>
        /// The delta.
        /// </value>
        [JsonPropertyName("delta")]
        public ChatMessageBase Delta { get; set; } = null;
        /// <summary>
        /// Gets or sets the log probs.
        /// </summary>
        /// <value>
        /// The log probs.
        /// </value>
        [JsonPropertyName("logprobs")]
        public object LogProbs { get; set; }
        /// <summary>
        /// Gets or sets the finish reason.
        /// </summary>
        /// <value>
        /// The finish reason.
        /// </value>
        [JsonPropertyName("finish_reason")]
        public string FinishReason {  get; set; }
    }
}
