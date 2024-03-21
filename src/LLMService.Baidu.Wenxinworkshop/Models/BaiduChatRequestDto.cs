using LLMService.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LLMService.Baidu.Wenxinworkshop.Models
{
    /// <summary>
    /// 百度用户请求
    /// </summary>
    /// <seealso cref="LLMService.Shared.Models.ChatRequest" />
    public class BaiduChatRequestDto: ChatRequest
    {
        /// <summary>
        /// 模型人设，主要用于人设设定，例如，你是xxx公司制作的AI助手，说明：
        /// （1）长度限制，最后一个message的content长度（即此轮对话的问题）、functions和system字段总内容不能超过20000个字符，且不能超过5000 tokens
        /// （2）如果同时使用system和functions，可能暂无法保证使用效果，持续进行优化
        /// </summary>
        /// <value>
        /// The system.
        /// </value>
        [JsonPropertyName("system")]
        public string System { get; set; }
    }
}
