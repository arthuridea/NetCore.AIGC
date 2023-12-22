using LLMService.Shared.Models;
using System.Text.Json.Serialization;

namespace LLMService.Baidu.Wenxinworkshop.Models
{
    /// <summary>
    /// baidu API response model
    /// </summary>
    public class BaiduWenxinChatResponse /*: IServerSentEventData*/
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
        /// Gets or sets the sentence identifier.
        /// </summary>
        /// <value>
        /// The sentence identifier.
        /// </value>
        [JsonPropertyName("sentence_id")]
        public int SentenceId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is end.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is end; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("is_end")]
        public bool IsEnd { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is truncated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is truncated; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("is_truncated")]
        public bool IsTruncated { get; set; }
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [JsonPropertyName("result")]
        public string Result { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [need clear history].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [need clear history]; otherwise, <c>false</c>.
        /// </value>
        [JsonPropertyName("need_clear_history")]
        public bool NeedClearHistory { get; set; }
        /// <summary>
        /// Gets or sets the ban round.
        /// </summary>
        /// <value>
        /// The ban round.
        /// </value>
        [JsonPropertyName("ban_round")]
        public int BanRound { get; set; }
        /// <summary>
        /// Gets or sets the usage.
        /// </summary>
        /// <value>
        /// The usage.
        /// </value>
        [JsonPropertyName("usage")]
        public ChatApiTokenUsage Usage { get; set; }
    }

}
