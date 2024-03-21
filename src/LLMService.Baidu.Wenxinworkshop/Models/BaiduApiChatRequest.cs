using LLMService.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LLMService.Baidu.Wenxinworkshop.Models
{
    /// <summary>
    /// 内部调用百度api的接口
    /// TODO:字段不够全
    /// </summary>
    public class BaiduApiChatRequest: AIFeatureModel, IBackendChatRequest<ChatMessageBase>
    {
        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        [JsonPropertyName("messages")]
        [Required]
        public List<ChatMessageBase> Messages { get; set; }
        /// <summary>
        /// 模型人设，主要用于人设设定，例如，你是xxx公司制作的AI助手，说明：
        /// （1）长度限制，最后一个message的content长度（即此轮对话的问题）、functions和system字段总内容不能超过20000个字符，且不能超过5000 tokens
        /// （2）如果同时使用system和functions，可能暂无法保证使用效果，持续进行优化
        /// </summary>
        /// <value>
        /// The system.
        /// </value>
        [JsonPropertyName("system")]
        public string System {  get; set; }
        /// <summary>
        /// 表示最终用户的唯一标识符，可以监视和检测滥用行为，防止接口恶意调用
        /// </summary>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = "";
    }
}
