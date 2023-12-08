using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static LLMService.Shared.Models.BaiduApiDefaults;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// 对话请求实体
    /// </summary>
    /// <seealso cref="LLMServiceHub.Models.AIFeatureModel" />
    public class ChatRequest: AIFeatureModel
    {
        /// <summary>
        /// 会话编号
        /// </summary>
        /// <example></example>
        [JsonPropertyName("conversation_id")]
        public string ConversationId {  get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 单次对话消息
        /// </summary>
        /// <example></example>
        [JsonPropertyName("message")]
        [Required]
        public string Message {  get; set; }
        /// <summary>
        /// 大模型
        /// <para>可选项：ERNIE-Bot-turbo(default)|ERNIE-Bot-4|ERNIE-Bot</para>
        /// </summary>
        /// <example>2</example>
        [JsonPropertyName("model")]
        public LLM_ModelType ModelSchema { get; set; } = LLM_ModelType.ERNIE_BOT_TURBO;
        /// <summary>
        /// 表示最终用户的唯一标识符，可以监视和检测滥用行为，防止接口恶意调用
        /// </summary>
        /// <example>7ffe3194-2bf0-48ba-8dbd-e888d7d556d3</example>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = "";
    }
}
