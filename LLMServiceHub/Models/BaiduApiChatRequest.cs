using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LLMServiceHub.Models
{
    /// <summary>
    /// 内部调用百度api的接口
    /// TODO:字段不够全
    /// </summary>
    public class BaiduApiChatRequest: AIFeatureModel
    {
        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        [JsonPropertyName("messages")]
        [Required]
        public List<BaiduWenxinMessage> Messages { get; set; }
        /// <summary>
        /// 表示最终用户的唯一标识符，可以监视和检测滥用行为，防止接口恶意调用
        /// </summary>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = "";
    }
}
