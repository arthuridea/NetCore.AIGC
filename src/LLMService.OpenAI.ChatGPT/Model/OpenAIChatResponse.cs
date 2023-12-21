using LLMService.Shared.Models;
using System.Text.Json.Serialization;

namespace LLMService.OpenAI.ChatGPT.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IChatResponse{TChatApiResponse}" />
    public class OpenAIChatResponse: IChatResponse<OpenAIBackendResponseModel>
    {
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [JsonPropertyName("result")]
        public OpenAIBackendResponseModel Result { get; set; }
        /// <summary>
        /// Gets or sets the conversation identifier.
        /// </summary>
        /// <value>
        /// The conversation identifier.
        /// </value>
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; }
        /// <summary>
        /// Gets or sets the model schema.
        /// </summary>
        /// <value>
        /// The model schema.
        /// </value>
        [JsonPropertyName("model_schema")]
        public LLMApiDefaults.LLM_ModelType ModelSchema { get; set; }
        /// <summary>
        /// Gets a value indicating whether [need clear history].
        /// </summary>
        /// <value>
        /// <c>true</c> if [need clear history]; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("need_clear_history")]
        public bool NeedClearHistory { get; set; }


    }
}
