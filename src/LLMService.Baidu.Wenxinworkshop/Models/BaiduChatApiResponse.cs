using LLMService.Shared.Models;
using System.Text.Json.Serialization;
using static LLMService.Shared.Models.LLMApiDefaults;

namespace LLMService.Baidu.Wenxinworkshop.Models
{
    /// <summary>
    /// baidu API response wrapper
    /// </summary>
    /// <seealso cref="BaiduWenxinChatResponse" />
    public class BaiduChatApiResponse : IChatResponse<BaiduWenxinChatResponse>
    {
        /// <summary>
        /// Gets or sets the conversation identifier.
        /// </summary>
        /// <value>
        /// The conversation identifier.
        /// </value>
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; } = "";
        /// <summary>
        /// 大模型
        /// <para>可选项：ERNIE-Bot-turbo(default)|ERNIE-Bot-4|ERNIE-Bot</para>
        /// </summary>
        /// <example>2</example>
        [JsonPropertyName("model")]
        public LLM_ModelType ModelSchema { get; set; } = LLM_ModelType.ERNIE_BOT_TURBO;
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [JsonPropertyName("llm_response_data")]
        public BaiduWenxinChatResponse LLMResponseData { get; set; }
        /// <summary>
        /// Gets a value indicating whether [need clear history].
        /// </summary>
        /// <value>
        /// <c>true</c> if [need clear history]; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("need_clear_history")]
        public bool NeedClearHistory => LLMResponseData?.NeedClearHistory ?? false;

        /// <summary>
        /// latest aigc message.
        /// </summary>
        /// <value>
        /// The aigc message.
        /// </value>
        [JsonPropertyName("aigc_message")]
        public string AIGCMessage => LLMResponseData?.Result;
    }
}
