using System.Text.Json.Serialization;

namespace LLMService.Shared.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AIFeatureModel
    {
        /// <summary>
        /// 温度
        /// <para>（1）较高的数值会使输出更加随机，而较低的数值会使其更加集中和确定<br />
        /// （2）默认0.95，范围 (0, 1.0]，不能为0<br />
        /// （3）建议该参数和top_p只设置1个<br />
        /// （4）建议top_p和temperature不要同时更改 
        /// </para>
        /// </summary>
        /// <value>
        /// 默认 0.95
        /// </value>
        /// <example>0.95</example>
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; } = 0.95F;
        /// <summary>
        /// 多样性
        /// <para>
        /// （1）影响输出文本的多样性，取值越大，生成文本的多样性越强<br />
        /// （2）默认0.8，取值范围 [0, 1.0]<br />
        /// （3）建议该参数和temperature只设置1个<br />
        /// （4）建议top_p和temperature不要同时更改
        /// </para>
        /// </summary>
        /// <value>
        /// 默认 0.8
        /// </value>
        /// <example>0.8</example>
        [JsonPropertyName("top_p")]
        public float TopP { get; set; } = 0.8F;
        /// <summary>
        /// 惩罚值:通过对已生成的token增加惩罚，减少重复生成的现象。
        /// <para>
        /// （1）值越大表示惩罚越大<br />（2）默认1.0，取值范围：[1.0, 2.0]
        /// </para>
        /// </summary>
        /// <value>
        /// 默认 1.5
        /// </value>
        /// <example>1.5</example>
        [JsonPropertyName("penalty_score")]
        public float PenaltyScore { get; set; } = 1.5F;
        /// <summary>
        /// 是否以流式接口的形式返回数据，默认false
        /// </summary>
        /// <value>
        ///   <c>true</c> if stream; otherwise, <c>false</c>.
        /// </value>
        /// <example>false</example>
        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
    }
}
